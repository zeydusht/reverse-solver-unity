// Background music (Presentation/Ui/Music.cs). PM_96: two loops, the lobby's (menu) and the level's, each an MP3 from
// GitHub Pages decoded into a WebAudio buffer and looped sample-exactly, crossfaded on the dive; and the win fanfare
// (a WAV) on the same clock, so the win screen can follow its beats (RS_MusicFanfareTime: the position the speaker is
// playing). Starts on the first touch / key (browsers need one), pauses with the page.
//   voice (a source + its own gain: the fades) -> duck (the level's, under a loss) -> panel (PM_98: 35 % under a pause or
//   settings panel) -> M.gain (the switch, x volume) -> out
//   fanfare -> its own gain (the Sound switch) -> out
// M.gain stays the master: the opening story (Intro.jslib) silences it while it plays.
mergeInto(LibraryManager.library, {
  /* PM_104_EK: the sound file Unity is loading (AudioDecodeWatch, around Resources.Load), so the page's decode guard
     (index.html) can name a file Safari could not decode; and those names, the new ones since the last call. */
  RS_AudioName: function (namePtr) { window.rsAudioName = namePtr ? UTF8ToString(namePtr) : ''; },
  RS_AudioFailsTake: function () {
    var f = window.rsAudioFails || [], i = window.rsAudioFailsTaken || 0, out = [];
    for (; i < f.length; i++) if (f[i].error !== 'rejection' && f[i].error !== 'error') out.push(f[i].name || (f[i].size + ' B'));
    window.rsAudioFailsTaken = i;
    var s = out.join('\n'), n = lengthBytesUTF8(s) + 1, b = _malloc(n);
    stringToUTF8(s, b, n);
    return b;
  },
  RS_MusicInit: function (lobbyUrl, lobbyStart, lobbyLength, levelUrl, levelStart, levelLength, fanfareUrl, volume, on) {
    var M = window.rsMusic = window.rsMusic || {};
    M.volume = volume; M.on = !!on;
    if (M.init) return;
    M.init = true;
    M.tracks = {
      lobby: { url: UTF8ToString(lobbyUrl), start: lobbyStart, length: lobbyLength, offset: 0, resume: true },
      level: { url: UTF8ToString(levelUrl), start: levelStart, length: levelLength, offset: 0, resume: false }
    };
    M.fan = { url: UTF8ToString(fanfareUrl), t0: -1, end: 0 };
    M.scene = 'lobby';
    var Ctx = window.AudioContext || window.webkitAudioContext;
    if (!Ctx) return;
    var events = ['touchend', 'click', 'pointerup', 'keydown'];
    function decode(url) {
      return fetch(url).then(function (r) { if (!r.ok) throw 0; return r.arrayBuffer(); })
        .then(function (b) { return new Promise(function (ok, no) { window.rsAudioName = url.split('/').pop(); var p = M.ctx.decodeAudioData(b, ok, no); window.rsAudioName = ''; if (p && p.catch) p.catch(no); }); });   // PM_104_EK: Safari's promise too
    }
    function load(name) {
      var T = M.tracks[name];
      return decode(T.url).then(function (buf) {
        var d = buf.getChannelData(0), n = Math.min(d.length, Math.floor(buf.sampleRate * .3)), i = 0;
        while (i < n && Math.abs(d[i]) < 0.004) i++;
        T.loopStart = T.start >= 0 ? T.start : i < n ? i / buf.sampleRate : 0;   // the old loop: its lead-in by its first sound
        T.loopEnd = Math.min(buf.duration, T.loopStart + T.length);
        T.buf = buf;
        if (M.scene === name && !T.v) M.voice(T, Math.max(M.ctx.currentTime, M.fan.end), M.fan.end > M.ctx.currentTime ? 1 : .3);
      }).catch(function () {});
    }
    function gesture() {
      try {
        if (!M.ctx) {
          M.ctx = new Ctx();
          M.gain = M.ctx.createGain();
          M.gain.gain.value = M.on && !(window.rsIntro && window.rsIntro.active) ? M.volume : 0;
          M.gain.connect(M.ctx.destination);
          M.panel = M.ctx.createGain(); M.panel.gain.value = M.panelOn ? .35 : 1; M.panel.connect(M.gain);
          ['lobby', 'level'].forEach(function (name) { var T = M.tracks[name]; T.duck = M.ctx.createGain(); T.duck.connect(M.panel); });
          M.fan.gain = M.ctx.createGain(); M.fan.gain.connect(M.ctx.destination);
          // the scene's loop first, then the other one, then the fanfare (small)
          var first = M.scene === 'level' ? 'level' : 'lobby', second = first === 'lobby' ? 'level' : 'lobby';
          load(first).then(function () { return load(second); }).then(function () {
            return decode(M.fan.url).then(function (buf) { M.fan.buf = buf; }).catch(function () {});
          });
        }
        if (M.ctx.state !== 'running') M.ctx.resume();
        else events.forEach(function (e) { document.removeEventListener(e, gesture, true); });
      } catch (e) {}
    }
    /* A new voice of a track at `when`, its gain from 0 up to 1 over `fade` s: the lobby where it stopped, the level
       from its start. */
    M.voice = function (T, when, fade) {
      if (!T.buf || !M.ctx) return;
      var src = M.ctx.createBufferSource(), g = M.ctx.createGain();
      src.buffer = T.buf; src.loop = true; src.loopStart = T.loopStart; src.loopEnd = T.loopEnd;
      src.connect(g); g.connect(T.duck);
      var at = T.resume ? T.offset % (T.loopEnd - T.loopStart) : 0;
      g.gain.value = 0;
      g.gain.setValueAtTime(fade > 0 ? 0 : 1, when);
      if (fade > 0) g.gain.linearRampToValueAtTime(1, when + fade);
      src.start(when, T.loopStart + at);
      T.v = { src: src, g: g, when: when, began: when - at };
    };
    /* The track's voice fades out from `when` over `fade` s and stops there (the lobby remembers its place). */
    M.fadeOut = function (T, when, fade) {
      var v = T.v; if (!v) return;
      T.v = null;
      var now = M.ctx.currentTime, from = Math.max(now, when), end = from + Math.max(fade, .005);
      v.g.gain.cancelScheduledValues(now);
      v.g.gain.setValueAtTime(v.g.gain.value, from);
      v.g.gain.linearRampToValueAtTime(0, end);
      try { v.src.stop(end + .02); } catch (e) {}
      if (T.resume) T.offset = Math.max(0, end - v.began);
    };
    events.forEach(function (e) { document.addEventListener(e, gesture, true); });
    document.addEventListener('visibilitychange', function () {
      if (!M.ctx) return;
      if (document.hidden) M.ctx.suspend(); else M.ctx.resume();
    });
  },

  RS_MusicSetOn: function (on) {
    var M = window.rsMusic; if (!M) return;
    M.on = !!on;
    if (M.gain) { var t = M.ctx.currentTime; M.gain.gain.cancelScheduledValues(t); M.gain.gain.setTargetAtTime(M.on && !(window.rsIntro && window.rsIntro.active) ? M.volume : 0, t, .08); }
  },

  /* 0: the lobby, 1: the level (always from its top). The other one fades out over `fadeOut` s; this one comes in over
     `fadeIn` s. The lobby already playing stays as it is. */
  RS_MusicScene: function (scene, fadeOut, fadeIn) {
    var M = window.rsMusic; if (!M || !M.tracks) return;
    var name = scene === 1 ? 'level' : 'lobby', other = name === 'level' ? 'lobby' : 'level';
    M.scene = name;
    if (!M.ctx) return;
    var now = M.ctx.currentTime, T = M.tracks[name];
    M.fan.end = 0;
    M.fadeOut(M.tracks[other], now, fadeOut);
    T.duck.gain.cancelScheduledValues(now); T.duck.gain.setTargetAtTime(1, now, .05);
    if (name === 'level') M.fadeOut(T, now, .08);
    else if (T.v && T.v.when <= now) return;
    else M.fadeOut(T, now, .005);   // a lobby waiting for a fanfare's end: now instead
    M.voice(T, now, fadeIn);
  },

  /* PM_98 §3: a pause or settings panel: the music down to 35 % in .15 s (on), back in .3 s (off). */
  RS_MusicPanel: function (on) {
    var M = window.rsMusic; if (!M) return;
    M.panelOn = !!on;
    if (!M.ctx || !M.panel) return;
    var g = M.panel.gain, t = M.ctx.currentTime;
    g.cancelScheduledValues(t); g.setValueAtTime(g.value, t);
    g.linearRampToValueAtTime(on ? .35 : 1, t + (on ? .15 : .3));
  },

  /* A loss / time up: the level's music down to 40 % (on) and back (off). */
  RS_MusicDuck: function (on) {
    var M = window.rsMusic; if (!M || !M.ctx || !M.tracks) return;
    var g = M.tracks.level.duck.gain, t = M.ctx.currentTime;
    g.cancelScheduledValues(t); g.setValueAtTime(g.value, t);
    g.linearRampToValueAtTime(on ? .4 : 1, t + (on ? .25 : .5));
  },

  /* The win: the fanfare starts (`sound` 0: silent, its clock still runs), the music is cut in .15 s, and after the
     fanfare the lobby's music comes back in 1 s. Returns its length (s); 0: no fanfare (the screen keeps its own time). */
  RS_MusicFanfare: function (sound) {
    var M = window.rsMusic; if (!M || !M.ctx || !M.fan || M.ctx.state !== 'running') { if (M && M.fan) M.fan.t0 = -1; return 0; }
    var now = M.ctx.currentTime;
    if (!M.fan.buf) {   // no fanfare (not loaded): the music still goes from the level's to the lobby's
      M.fan.t0 = -1; M.fan.end = now + 1.5;
      M.fadeOut(M.tracks.level, now, .15); M.fadeOut(M.tracks.lobby, now, .15);
      M.scene = 'lobby'; M.voice(M.tracks.lobby, now + 1.5, 1);
      return 0;
    }
    var t0 = now + .03, len = M.fan.buf.duration;
    if (M.fan.src) { try { M.fan.src.stop(); } catch (e) {} }
    var src = M.ctx.createBufferSource(); src.buffer = M.fan.buf; src.connect(M.fan.gain);
    M.fan.gain.gain.setValueAtTime(sound ? 1 : 0, now);
    src.start(t0); M.fan.src = src; M.fan.t0 = t0; M.fan.end = t0 + len;
    M.fadeOut(M.tracks.level, t0, .15);
    M.fadeOut(M.tracks.lobby, t0, .15);
    M.scene = 'lobby';
    M.voice(M.tracks.lobby, t0 + len, 1);   // on the audio clock, at the fanfare's end
    return len;
  },

  /* The fanfare's position (s) at the speaker now: its output timestamp moved on to this moment; -100: none. */
  RS_MusicFanfareTime: function () {
    var M = window.rsMusic; if (!M || !M.ctx || !M.fan || M.fan.t0 < 0 || M.ctx.state !== 'running') return -100;
    var t;
    if (M.ctx.getOutputTimestamp) {
      var o = M.ctx.getOutputTimestamp();
      if (o && o.contextTime > 0 && o.performanceTime > 0) t = o.contextTime + (performance.now() - o.performanceTime) / 1000;
    }
    if (t === undefined) t = M.ctx.currentTime - (M.ctx.outputLatency || 0) - (M.ctx.baseLatency || 0);
    return t - M.fan.t0;
  }
});
