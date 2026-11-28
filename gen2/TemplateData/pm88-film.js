/* PM_88 §2: the opening film plays in the page from the link's first moment, while the game loads behind it.
   Intro.jslib's player used to start only once the game was up (a loader with an orange bar first); now its
   RS_PlayIntro takes this one over (window.rsFilm.attach) when the game asks for the story, or ends it
   (rsFilm.decline) when the game says this visit has none.
   - who sees it: a first visit (no rs_lang yet: the game writes it on every start) without ?lv / ?set / ?debug=1 /
     c=pt1; ?intro=1 always, ?intro=0 never. A visitor the page takes for new but the game does not is declined at
     once, the film acting as the loader.
   - muted at first (autoplay), "Sound on" unmutes it (the sound is in the films).
   - A, then the wake button (5 s at most), then B: as before. "Skip" only once the game is there.
   - at the end (or Skip) straight into the game: the game builds its first screen on that frame, the film fades
     .25 s later. If the game is not there yet the film holds its last frame with a thin gold line until it is.
   - a broken or stalled film gives way to the loader; the game then goes on without it.
   - times on window.rsFilm (performance.now(), ms from the link): first (the first frame), engine (the game
     loaded), ready (the game asked for the story), skip, gone (the film faded out); and a [film] console line. */
(function () {
  var q = new URLSearchParams(location.search), param = q.get('intro');
  var seen = false;
  try { seen = localStorage.getItem('rs_lang') !== null; } catch (_) {}
  var filtered = q.get('debug') === '1' || q.has('lv') || q.has('set') || q.get('c') === 'pt1';
  if (param === '0' || !(param === '1' || (!filtered && !seen))) return;
  var lang = q.get('lang');
  if (lang !== 'tr' && lang !== 'en') { try { lang = localStorage.getItem('rs_lang'); } catch (_) {} }
  var tr = lang === 'tr';
  var T = { wake: tr ? 'Uyandır!' : 'Tap to wake!', skip: tr ? 'Atla' : 'Skip', sound: tr ? 'Sesi aç' : 'Sound on',
            mute: tr ? 'Sesi kapat' : 'Sound off', play: tr ? 'Oynat' : 'Play' };
  var F = window.rsFilm = { events: [], ready: false, done: false, failed: false, sound: false };
  var parts = ['A', 'B'], at = 0, videos = {}, current, phase = 'loading', elapsed = 0, frozen = 0, stalled = 0, lastTime = 0;
  var previous = performance.now(), emitFn = null, finished = false, holding = false, playToken = 0;
  function log(m) { try { console.log('[film] ' + m); } catch (_) {} }
  function ms(t) { return t ? (t / 1000).toFixed(2) + ' s' : '-'; }
  function emit(v) { if (emitFn) emitFn(v); else F.events.push(v); }
  function sec() { return current && isFinite(current.currentTime) ? current.currentTime.toFixed(2) : '0'; }

  var overlay = document.createElement('div'); overlay.id = 'rsIntro';
  overlay.style.cssText = 'position:fixed;inset:0;z-index:9000;background:#000;overflow:hidden;opacity:1;transition:opacity 300ms;touch-action:manipulation;font-family:NunitoBlack,sans-serif;font-weight:900;color:#FFF6D8';
  var style = document.createElement('style');
  style.textContent = '@keyframes rsIntroPulse{50%{transform:translateX(-50%) scale(1.07)}}#rsIntro button{font:inherit;color:#FFF6D8;cursor:pointer;border:0;text-shadow:0 2px 1px #1E5A0E;min-height:44px}';
  overlay.appendChild(style);
  function button(id, text, css) {
    var b = document.createElement('button'); b.id = id; b.textContent = text; b.style.cssText = css; return b;
  }
  var corner = 'position:absolute;z-index:4;top:calc(12px + env(safe-area-inset-top));padding:10px 20px;border-radius:16px;background:#0B2A36cc;font-size:18px;';
  var skipButton = button('rsIntroSkip', T.skip, corner + 'display:none;right:calc(14px + env(safe-area-inset-right))');
  var soundButton = button('rsFilmSound', '🔇 ' + T.sound, corner + 'left:calc(14px + env(safe-area-inset-left))');
  var wakeButton = button('rsIntroWake', T.wake, 'display:none;position:absolute;z-index:3;bottom:calc(40px + env(safe-area-inset-bottom));left:50%;transform:translateX(-50%);width:260px;height:120px;background:transparent url(TemplateData/t23-name-button-primary.png) center/100% 100% no-repeat;font-size:28px;animation:rsIntroPulse 1s ease-in-out infinite');
  var playButton = button('rsFilmPlay', '▶ ' + T.play, 'display:none;position:absolute;z-index:5;top:50%;left:50%;transform:translate(-50%,-50%);padding:16px 30px;border-radius:20px;background:#0B2A36dd;font-size:26px');
  var hold = document.createElement('div');   // the game not there yet at the film's end: a thin gold line
  hold.style.cssText = 'display:none;position:absolute;z-index:3;left:50%;bottom:calc(56px + env(safe-area-inset-bottom));width:200px;height:3px;margin-left:-100px;border-radius:2px;background:rgba(255,246,216,.28)';
  var holdFill = document.createElement('div');
  holdFill.style.cssText = 'height:100%;width:0;border-radius:2px;background:linear-gradient(90deg,#E3B54E,#FFE47E);box-shadow:0 0 6px rgba(255,228,126,.8);transition:width .15s';
  hold.appendChild(holdFill);
  [hold, wakeButton, playButton, skipButton, soundButton].forEach(function (e) { overlay.appendChild(e); });
  document.body.appendChild(overlay);
  F.overlay = overlay;

  function silenceMusic() {
    var M = window.rsMusic;
    if (M && M.gain) { M.gain.gain.cancelScheduledValues(M.ctx.currentTime); M.gain.gain.value = 0; }
  }
  function restoreMusic() {
    var M = window.rsMusic;
    if (M && M.gain) { M.gain.gain.cancelScheduledValues(M.ctx.currentTime); M.gain.gain.value = M.on ? M.volume : 0; }
  }
  function resize() {
    Object.keys(videos).forEach(function (p) { videos[p].style.objectFit = innerWidth / innerHeight < .7 ? 'cover' : 'contain'; });
  }
  function gone() {
    overlay.style.opacity = '0';
    setTimeout(function () {
      Object.keys(videos).forEach(function (p) { var v = videos[p]; v.removeAttribute('src'); v.load(); v.remove(); });
      overlay.remove(); videos = {}; F.overlay = null; F.gone = performance.now();
      log('first frame ' + ms(F.first) + ', game loaded ' + ms(F.engine) + ', game ready ' + ms(F.ready && F.readyAt) +
          ', skip ' + ms(F.skip) + ', gone ' + ms(F.gone) + (F.failed ? ' (failed)' : ''));
    }, 300);
  }
  function stop() {
    clearInterval(timer);
    Object.keys(videos).forEach(function (p) { videos[p].pause(); });
    document.removeEventListener('visibilitychange', visibility);
    window.removeEventListener('resize', resize);
    skipButton.disabled = wakeButton.disabled = soundButton.disabled = true;
  }
  /* The film is over (its end or Skip): into the game if it is there, else hold the last frame. */
  function finish() {
    if (finished) return;
    if (!F.ready) {
      if (!holding) { holding = true; phase = 'hold'; hold.style.display = 'block'; skipButton.style.display = 'none'; soundButton.style.display = 'none'; log('held at the end: the game is not there yet'); }
      return;
    }
    finished = true; F.done = true;
    stop(); restoreMusic();
    emit('done');
    setTimeout(gone, 250);   // the game draws its first screen first
  }
  function fail() {
    if (finished || F.failed) return;
    F.failed = true;
    emit('fail|' + parts[at] + '|' + sec());
    log('failed in ' + parts[at] + ' at ' + sec() + ' s');
    if (F.ready) { finished = true; F.done = true; stop(); restoreMusic(); emit('done'); setTimeout(gone, 250); return; }
    stop(); finished = true; gone();   // the loader under it
  }
  function prepare(part) {
    if (videos[part]) return videos[part];
    var v = document.createElement('video');
    v.setAttribute('playsinline', ''); v.setAttribute('webkit-playsinline', '');
    v.muted = !F.sound; if (v.muted) v.setAttribute('muted', '');
    v.preload = 'auto'; v.dataset.part = part;
    v.style.cssText = 'position:absolute;inset:0;width:100%;height:100%;visibility:hidden';
    videos[part] = v; overlay.insertBefore(v, hold); resize();
    v.addEventListener('error', function () { if (v === current) fail(); });
    v.addEventListener('playing', function () {
      if (finished || v !== current) return;
      Object.keys(videos).forEach(function (p) { videos[p].style.visibility = videos[p] === v ? 'visible' : 'hidden'; });
      playButton.style.display = 'none';
      if (!v.dataset.shown) {
        v.dataset.shown = '1';
        if (part === 'A') {
          F.first = performance.now(); emit('shown|A|0');
          log('first frame at ' + ms(F.first));
        }
        if (at + 1 < parts.length) prepare(parts[at + 1]);
      }
      phase = 'playing'; stalled = 0; lastTime = v.currentTime;
    });
    v.addEventListener('ended', function () {
      if (finished || v !== current) return;
      if (part === 'A') { phase = 'wake'; frozen = 0; wakeButton.style.display = 'block'; }
      else next();
    });
    v.src = 'intro/intro' + part + '.mp4'; v.load();
    return v;
  }
  function play() {
    if (document.hidden || finished) return;
    var v = current, token = ++playToken;
    var done = function () {
      if (document.hidden || current !== v || token !== playToken || finished) return;
      if (!v.muted) { v.muted = true; F.sound = false; soundButton.textContent = '🔇 ' + T.sound; v.play().catch(blocked); return; }
      blocked();
    };
    var blocked = function () { if (!finished && current === v) playButton.style.display = 'block'; };   // iOS Low Power: a tap starts it
    try { var p = v.play(); if (p) p.catch(done); } catch (e) { done(); }
  }
  function start() {
    phase = 'loading'; stalled = 0; lastTime = 0;
    current = prepare(parts[at]);
    if (current.error) fail(); else play();
  }
  function next() { if (++at >= parts.length) finish(); else start(); }
  function wakeUp(tapped) {
    if (finished || phase !== 'wake') return;
    emit('wake|A|' + (tapped ? Math.min(5, frozen).toFixed(2) : '5') + '|' + (tapped ? '1' : '0'));
    wakeButton.style.display = 'none'; next();
  }
  wakeButton.onclick = function () { wakeUp(true); };
  skipButton.onclick = function () {
    if (finished || !F.ready) return;
    F.skip = performance.now();
    emit('skip|' + parts[at] + '|' + sec());
    finish();
  };
  playButton.onclick = function () { playButton.style.display = 'none'; if (current) current.play().catch(function () {}); };
  soundButton.onclick = function () {
    F.sound = !F.sound;
    soundButton.textContent = (F.sound ? '🔊 ' + T.mute : '🔇 ' + T.sound);
    Object.keys(videos).forEach(function (p) {
      var v = videos[p];
      v.muted = !F.sound;
      if (F.sound) v.removeAttribute('muted');
      // a part not playing yet is unlocked by this tap (iOS lets a video play with sound only from a tap)
      if (F.sound && v !== current) { var r = v.play(); v.pause(); if (r) r.then(function () { if (v !== current) v.pause(); }).catch(function () {}); }
    });
    if (F.sound && current && current.paused && phase !== 'wake' && phase !== 'hold') current.play().catch(function () {});
  };
  function visibility() {
    previous = performance.now();
    if (document.hidden) { playToken++; if (current) current.pause(); }
    else if (phase === 'loading' || phase === 'playing') play();
  }
  document.addEventListener('visibilitychange', visibility);
  window.addEventListener('resize', resize);
  var timer = setInterval(function () {
    var now = performance.now(), dt = (now - previous) / 1000; previous = now;
    if (document.hidden || finished) return;
    if (current && current.dataset.shown) { elapsed += dt; if (elapsed >= 1 && F.ready && !holding) skipButton.style.display = 'block'; }
    if (phase === 'wake') { frozen += dt; if (frozen >= 5) wakeUp(false); }
    else if (phase === 'loading' || phase === 'playing') {
      if (current.currentTime > lastTime + .01) { stalled = 0; lastTime = current.currentTime; }
      else if (playButton.style.display !== 'block') { stalled += dt; if (stalled >= 8) fail(); }
    }
  }, 100);

  /* The game's loading progress (index.html) for the held end. */
  F.progress = function (p) { holdFill.style.width = Math.round(p * 100) + '%'; };
  /* RS_PlayIntro: the game wants the story and is there: its events from now on (the ones so far first). */
  F.attach = function (fn) {
    emitFn = fn; F.ready = true; F.readyAt = performance.now();
    var ev = F.events; F.events = [];
    ev.forEach(function (v) { fn(v); });
    log('game ready at ' + ms(F.readyAt) + (holding ? ' (the film was waiting)' : ''));
    if (F.failed || finished) { fn('done'); return; }
    silenceMusic();
    if (holding) { holding = false; finish(); }
  };
  /* The game has no story for this visit: the film goes now (it was the loader). */
  F.decline = function () {
    F.ready = true; F.readyAt = performance.now(); emitFn = function () {};
    log('declined by the game at ' + ms(F.readyAt));
    if (finished) return;
    finished = true; F.done = true; stop(); gone();
  };
  start();
})();
