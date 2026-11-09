// Background music (Presentation/Ui/Music.cs): the MP3 from GitHub Pages decoded into a WebAudio buffer and looped
// sample-exactly. The encoder's lead-in silence is skipped by finding the first sound (the piece starts loud) and the
// loop runs for the WAV's exact length from there. Starts on the first touch / key (browsers need one), pauses with
// the page; a gain node for the switch and the win duck.
mergeInto(LibraryManager.library, {
  RS_MusicInit: function (urlPtr, volume, loopSeconds, on) {
    var M = window.rsMusic = window.rsMusic || {};
    M.url = UTF8ToString(urlPtr); M.volume = volume; M.loop = loopSeconds; M.on = !!on;
    if (M.init) return;
    M.init = true;
    var Ctx = window.AudioContext || window.webkitAudioContext;
    if (!Ctx) return;
    var events = ['touchend', 'click', 'pointerup', 'keydown'];
    function load() {
      fetch(M.url).then(function (r) { if (!r.ok) throw 0; return r.arrayBuffer(); })
        .then(function (b) { return new Promise(function (ok, no) { M.ctx.decodeAudioData(b, ok, no); }); })
        .then(function (buf) {
          var d = buf.getChannelData(0), n = Math.min(d.length, Math.floor(buf.sampleRate * .3)), i = 0;
          while (i < n && Math.abs(d[i]) < 0.004) i++;
          var start = i < n ? i / buf.sampleRate : 0, end = Math.min(buf.duration, start + M.loop);
          var src = M.ctx.createBufferSource();
          src.buffer = buf; src.loop = true; src.loopStart = start; src.loopEnd = end;
          src.connect(M.gain); src.start(0, start);
          M.src = src; M.playing = true;
        })
        .catch(function () {});
    }
    function gesture() {
      try {
        if (!M.ctx) {
          M.ctx = new Ctx();
          M.gain = M.ctx.createGain();
          M.gain.gain.value = M.on && !(window.rsIntro && window.rsIntro.active) ? M.volume : 0;
          M.gain.connect(M.ctx.destination);
          load();
        }
        if (M.ctx.state !== 'running') M.ctx.resume();
        else events.forEach(function (e) { document.removeEventListener(e, gesture, true); });
      } catch (e) {}
    }
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

  RS_MusicDuck: function (delay, seconds) {
    var M = window.rsMusic; if (!M || !M.gain || !M.on || (window.rsIntro && window.rsIntro.active)) return;
    var g = M.gain.gain, t = M.ctx.currentTime;
    g.cancelScheduledValues(t);
    g.setValueAtTime(g.value, t);
    g.setTargetAtTime(M.volume * .25, t + delay, .06);
    g.setTargetAtTime(M.volume, t + delay + seconds, .25);
  }
});
