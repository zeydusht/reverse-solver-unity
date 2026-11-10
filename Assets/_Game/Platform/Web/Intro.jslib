mergeInto(LibraryManager.library, {
  RS_PlayIntro: function (partsPtr, wakePtr, skipPtr, endingPtr, sound, objPtr, methodPtr) {
    var parts = UTF8ToString(partsPtr), wake = UTF8ToString(wakePtr), skip = UTF8ToString(skipPtr);
    var ending = UTF8ToString(endingPtr), obj = UTF8ToString(objPtr), method = UTF8ToString(methodPtr);
    var I = window.rsIntro = {}, videos = {}, tracks = {}, at = 0, current, source;
    var finished = false, phase = 'loading', elapsed = 0, frozen = 0, held = 0, stalled = 0, lastTime = 0;
    var previous = performance.now(), playToken = 0, gestures = ['touchend', 'click', 'pointerup', 'keydown'];
    function emit(value) { SendMessage(obj, method, value); }
    function sec() { return current && isFinite(current.currentTime) ? current.currentTime.toFixed(2) : '0'; }
    var overlay = document.createElement('div'); overlay.id = 'rsIntro';
    overlay.style.cssText = 'position:fixed;inset:0;z-index:9000;background:#000;overflow:hidden;opacity:1;transition:opacity 300ms;touch-action:manipulation;font-family:NunitoBlack,sans-serif;font-weight:900;color:#FFF6D8';
    var style = document.createElement('style');
    style.textContent = '@keyframes rsIntroPulse{50%{transform:translateX(-50%) scale(1.07)}}#rsIntro button{font:inherit;color:#FFF6D8;cursor:pointer;border:0;text-shadow:0 2px 1px #1E5A0E;min-height:44px}';
    overlay.appendChild(style);
    var skipButton = document.createElement('button'); skipButton.id = 'rsIntroSkip'; skipButton.textContent = skip;
    skipButton.style.cssText = 'display:none;position:absolute;z-index:4;top:calc(12px + env(safe-area-inset-top));right:calc(14px + env(safe-area-inset-right));padding:10px 20px;border-radius:16px;background:#0B2A36cc;font-size:18px';
    var wakeButton = document.createElement('button'); wakeButton.id = 'rsIntroWake'; wakeButton.textContent = wake;
    wakeButton.style.cssText = 'display:none;position:absolute;z-index:3;bottom:calc(40px + env(safe-area-inset-bottom));left:50%;transform:translateX(-50%);width:260px;height:120px;background:transparent url(TemplateData/t23-name-button-primary.png) center/100% 100% no-repeat;font-size:28px;animation:rsIntroPulse 1s ease-in-out infinite';
    var endingText = document.createElement('div'); endingText.id = 'rsIntroEnding'; endingText.textContent = ending;
    endingText.style.cssText = 'position:absolute;z-index:2;top:68%;left:8%;width:84%;text-align:center;font-size:clamp(28px,7vw,48px);text-shadow:0 3px 4px #000;opacity:0;transition:opacity .6s;pointer-events:none';
    overlay.appendChild(endingText); overlay.appendChild(wakeButton); overlay.appendChild(skipButton);
    document.body.appendChild(overlay);
    I.overlay = overlay;
    function stopAudio() {
      if (!source) return;
      try { source.stop(); source.disconnect(); } catch (e) {}
      source = null;
    }
    function silenceMusic() {
      var M = window.rsMusic;
      if (M && M.gain) { M.gain.gain.cancelScheduledValues(M.ctx.currentTime); M.gain.gain.value = 0; }
    }
    function audio() {
      if (finished) return;
      silenceMusic();
      var M = window.rsMusic, part = parts[at], track = tracks[part];
      if (!sound || !M || !M.ctx || !track) return;
      Object.keys(tracks).forEach(function (p) {
        var t = tracks[p];
        if (!t.bytes || t.decoding) return;
        t.decoding = true;
        M.ctx.decodeAudioData(t.bytes, function (buffer) { if (!finished) { t.buffer = buffer; audio(); } }, function () {});
        t.bytes = null;
      });
      if (document.hidden || phase !== 'playing' || current.paused || M.ctx.state !== 'running' || !track.buffer || source) return;
      var offset = current.currentTime;
      if (offset >= track.buffer.duration) return;
      source = M.ctx.createBufferSource(); source.buffer = track.buffer;
      source.connect(M.ctx.destination); source.start(0, offset);
    }
    function resize() {
      Object.keys(videos).forEach(function (p) { videos[p].style.objectFit = innerWidth / innerHeight < .7 ? 'cover' : 'contain'; });
    }
    function finish() {
      if (finished) return;
      finished = true; I.active = false;
      clearInterval(timer); stopAudio();
      Object.keys(videos).forEach(function (p) { videos[p].pause(); });
      document.removeEventListener('visibilitychange', visibility);
      window.removeEventListener('resize', resize);
      gestures.forEach(function (e) { document.removeEventListener(e, audio, true); });
      skipButton.disabled = true; wakeButton.disabled = true;
      var M = window.rsMusic;
      if (M && M.gain) { M.gain.gain.cancelScheduledValues(M.ctx.currentTime); M.gain.gain.value = M.on ? M.volume : 0; }
      emit('done');
      setTimeout(function () {
        overlay.style.opacity = '0';
        setTimeout(function () {
          Object.keys(videos).forEach(function (p) { var v = videos[p]; v.removeAttribute('src'); v.load(); v.remove(); });
          overlay.remove(); videos = {}; tracks = {}; I.overlay = null;
        }, 300);
      }, 250);
    }
    function fail() {
      if (finished) return;
      emit('fail|' + parts[at] + '|' + sec()); finish();
    }
    function prepare(part) {
      if (videos[part]) return videos[part];
      var v = document.createElement('video');
      v.setAttribute('playsinline', ''); v.setAttribute('webkit-playsinline', ''); v.setAttribute('muted', '');
      v.muted = true; v.preload = 'auto'; v.dataset.part = part;
      v.style.cssText = 'position:absolute;inset:0;width:100%;height:100%;visibility:hidden';
      videos[part] = v; overlay.insertBefore(v, endingText); resize();
      v.addEventListener('error', function () { if (v === current) fail(); });
      v.addEventListener('playing', function () {
        if (finished || v !== current) return;
        Object.keys(videos).forEach(function (p) { videos[p].style.visibility = videos[p] === v ? 'visible' : 'hidden'; });
        if (!v.dataset.shown) {
          v.dataset.shown = '1';
          if (part === 'A' || part === 'C') emit('shown|' + part + '|0');
          if (at + 1 < parts.length) prepare(parts[at + 1]);
        }
        phase = 'playing'; stalled = 0; lastTime = v.currentTime; audio();
      });
      v.addEventListener('seeking', stopAudio);
      v.addEventListener('seeked', audio);
      v.addEventListener('waiting', stopAudio);
      v.addEventListener('ended', function () {
        if (finished || v !== current) return;
        stopAudio();
        if (part === 'A' && parts.indexOf('B') >= 0) { phase = 'wake'; frozen = 0; wakeButton.style.display = 'block'; }
        else if (part === 'C') { phase = 'hold'; held = 0; endingText.style.opacity = '1'; }
        else next();
      });
      v.src = 'intro/intro' + part + '.mp4'; v.load();
      if (sound) {
        var track = tracks[part] = {};
        // Shipped MP3s use kurgu.py's default mix C; --ses A/B can rebuild alternatives.
        fetch('intro/intro' + part + '.mp3').then(function (r) { if (!r.ok) throw 0; return r.arrayBuffer(); })
          .then(function (bytes) { if (!finished) { track.bytes = bytes; audio(); } }).catch(function () {});
      }
      return v;
    }
    function play() {
      if (document.hidden || finished) return;
      var v = current, token = ++playToken;
      try { var promise = v.play(); if (promise) promise.catch(function () { if (!document.hidden && current === v && token === playToken) fail(); }); }
      catch (e) { fail(); }
    }
    function start() {
      phase = 'loading'; stalled = 0; lastTime = 0; stopAudio();
      current = prepare(parts[at]); I.part = parts[at];
      if (current.error) fail(); else play();
    }
    function next() { if (++at >= parts.length) finish(); else start(); }
    function wakeUp(tapped) {
      if (finished || phase !== 'wake') return;
      emit('wake|A|' + (tapped ? Math.min(5, frozen).toFixed(2) : '5') + '|' + (tapped ? '1' : '0'));
      wakeButton.style.display = 'none'; next();
    }
    wakeButton.onclick = function () { wakeUp(true); };
    skipButton.onclick = function () { if (!finished) { emit('skip|' + parts[at] + '|' + sec()); finish(); } };
    function visibility() {
      previous = performance.now();
      if (document.hidden) { playToken++; current.pause(); stopAudio(); }
      else if (phase === 'loading' || phase === 'playing') play();
    }
    document.addEventListener('visibilitychange', visibility);
    window.addEventListener('resize', resize);
    gestures.forEach(function (e) { document.addEventListener(e, audio, true); });
    I.active = true;
    var timer = setInterval(function () {
      var now = performance.now(), dt = (now - previous) / 1000; previous = now;
      if (document.hidden || finished) return;
      audio();
      if (current && current.dataset.shown) { elapsed += dt; if (elapsed >= 1) skipButton.style.display = 'block'; }
      if (phase === 'wake') { frozen += dt; if (frozen >= 5) wakeUp(false); }
      else if (phase === 'hold') { held += dt; if (held >= 1) finish(); }
      else {
        if (current.currentTime > lastTime + .01) { stalled = 0; lastTime = current.currentTime; }
        else { stalled += dt; if (stalled >= 8) fail(); }
        if (parts[at] === 'C' && isFinite(current.duration) && current.duration - current.currentTime <= 1.6) endingText.style.opacity = '1';
      }
    }, 100);
    silenceMusic(); start();
  }
});
