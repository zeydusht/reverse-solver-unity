// PM_108: the menu's dive, recorded for the preview videos: the screen (CDP screencast frames, f*.jpg + times.json,
// the page's clock in s), the game's own sound (Unity's WebAudio and the music's, apart, as Tools/pm107_miks.js:
// efektler.webm / muzik.webm, meta.json their start on the same clock) and the dive's log (log.txt: "<ms> <line>",
// the [cerceve] / [dalis] / [gecis] / [mix] lines, as pm72_tour.js TOUR_LOG_TS=1 writes it, for pm92_flow_check.py
// and pm91_video.py).
// usage: node Tools/pm108_dalis.js <out dir> [--query "&dalis108=0"] [--local Builds/publish/br] [--port 9371] [--noframes]
const fs = require('fs'), path = require('path'), http = require('http'), os = require('os'), { spawn } = require('child_process');
const args = process.argv.slice(2);
const opt = (n, d) => { const i = args.indexOf('--' + n); return i >= 0 ? args[i + 1] : d; };
const outDir = path.resolve(args[0]);
const ROOT = path.resolve(__dirname, '..');
const W = 390, H = 844, PORT = +opt('port', 9371), FRAMES = !args.includes('--noframes');
const CHROME = 'C:/Program Files/Google/Chrome/Application/chrome.exe';
const sleep = ms => new Promise(r => setTimeout(r, ms));

function serve(root) {
  const types = { '.html': 'text/html', '.js': 'application/javascript', '.css': 'text/css', '.json': 'application/json', '.wasm': 'application/wasm', '.png': 'image/png' };
  const server = http.createServer((req, res) => {
    let f = path.resolve(root, '.' + decodeURIComponent(new URL(req.url, 'http://x').pathname));
    if (!f.startsWith(root)) { res.writeHead(403); return res.end(); }
    if (fs.existsSync(f) && fs.statSync(f).isDirectory()) f = path.join(f, 'index.html');
    fs.stat(f, (err, s) => {
      if (err) { res.writeHead(404); return res.end(); }
      res.writeHead(200, { 'Content-Type': types[path.extname(f)] || 'application/octet-stream', 'Content-Length': s.size });
      fs.createReadStream(f).pipe(res);
    });
  });
  return new Promise(r => server.listen(0, '127.0.0.1', () => r(server)));
}

// every node that connects to a context's speakers also connects to that context's recorder tap
const TAP = `(function () {
  var connect = AudioNode.prototype.connect;
  window.__taps = [];
  // every context gets its tap as it is made (Unity's is made at load, before its first sound)
  ['AudioContext', 'webkitAudioContext'].forEach(function (name) {
    var AC = window[name]; if (!AC) return;
    var Wrapped = function (o) { var c = o === undefined ? new AC() : new AC(o); c.__tap = c.createMediaStreamDestination(); window.__taps.push(c); return c; };
    Wrapped.prototype = AC.prototype; window[name] = Wrapped;
  });
  AudioNode.prototype.connect = function (dest) {
    var r = connect.apply(this, arguments);
    try {
      if (dest instanceof AudioDestinationNode) {
        var c = this.context;
        if (!c.__tap) { c.__tap = c.createMediaStreamDestination(); window.__taps.push(c); }
        c.__n = (c.__n || 0) + 1;
        connect.call(this, c.__tap);
      }
    } catch (e) {}
    return r;
  };
  window.__pm100 = {
    start: function () {
      // Unity's: the context (not the music's) with the most sources connected to its speakers
      var M = window.rsMusic, c = window.__taps.filter(function (x) { return !M || x !== M.ctx; }).sort(function (a, b) { return (b.__n || 0) - (a.__n || 0); })[0];
      this.contexts = window.__taps.map(function (x) { return (x === (M && M.ctx) ? 'music ' : '') + x.sampleRate + ' Hz, ' + (x.__n || 0) + ' connected, ' + x.state; });
      if (!c) return -1;
      // a very quiet 25 Hz hum into the recording only (the encoder drops silence, which would lose the timeline);
      // Tools/pm100_measure.py filters it out
      var o = c.createOscillator(), g = c.createGain(); o.frequency.value = 25; g.gain.value = .003;
      connect.call(o, g); connect.call(g, c.__tap); o.start();
      var rec = new MediaRecorder(c.__tap.stream, { mimeType: 'audio/webm;codecs=opus', audioBitsPerSecond: 256e3 });
      var chunks = []; rec.ondataavailable = function (e) { if (e.data.size) chunks.push(e.data); };
      rec.start(100); this.rec = rec; this.chunks = chunks; this.t0 = performance.now(); this.epoch = Date.now();
      // the music's context too, apart; PM_107: made on the first touch, so started when it appears (its start kept)
      var self = this;
      var music = function () {
        var M = window.rsMusic;
        if (!(M && M.ctx && M.ctx.__tap) || self.mrec) return !!self.mrec;
        self.musicStart = (performance.now() - self.t0) / 1000;
        var mo = M.ctx.createOscillator(), mg = M.ctx.createGain(); mo.frequency.value = 25; mg.gain.value = .003;
        connect.call(mo, mg); connect.call(mg, M.ctx.__tap); mo.start();
        var mrec = new MediaRecorder(M.ctx.__tap.stream, { mimeType: 'audio/webm;codecs=opus', audioBitsPerSecond: 256e3 });
        var mchunks = []; mrec.ondataavailable = function (e) { if (e.data.size) mchunks.push(e.data); };
        mrec.start(100); self.mrec = mrec; self.mchunks = mchunks;
        return true;
      };
      if (!music()) { var iv = setInterval(function () { if (music()) clearInterval(iv); }, 50); }
      return c.sampleRate;
    },
    now: function () { return (performance.now() - this.t0) / 1000; },
    stopMusic: function () {
      var self = this;
      if (!self.mrec) return Promise.resolve('');
      return new Promise(function (ok) {
        self.mrec.onstop = function () {
          new Blob(self.mchunks, { type: 'audio/webm' }).arrayBuffer().then(function (b) {
            var u = new Uint8Array(b), s = '';
            for (var i = 0; i < u.length; i += 32768) s += String.fromCharCode.apply(null, u.subarray(i, i + 32768));
            ok(btoa(s));
          });
        };
        self.mrec.stop();
      });
    },
    stop: function () {
      var self = this;
      return new Promise(function (ok) {
        self.rec.onstop = function () {
          new Blob(self.chunks, { type: 'audio/webm' }).arrayBuffer().then(function (b) {
            var u = new Uint8Array(b), s = '';
            for (var i = 0; i < u.length; i += 32768) s += String.fromCharCode.apply(null, u.subarray(i, i + 32768));
            ok(btoa(s));
          });
        };
        self.rec.stop();
      });
    }
  };
})();`;


(async () => {
  fs.mkdirSync(outDir, { recursive: true });
  for (const f of fs.readdirSync(outDir)) if (/^f\d+\.jpg$/.test(f)) fs.unlinkSync(path.join(outDir, f));
  const server = await serve(path.resolve(ROOT, opt('local', 'Builds/publish/br')));
  const base = `http://127.0.0.1:${server.address().port}`;
  const profile = fs.mkdtempSync(path.join(os.tmpdir(), 'rs-dalis-'));
  const chrome = spawn(CHROME, ['--headless=new', `--remote-debugging-port=${PORT}`, '--use-angle=d3d11', '--enable-gpu', '--ignore-gpu-blocklist',
    '--autoplay-policy=no-user-gesture-required', `--user-data-dir=${profile}`, '--no-first-run', 'about:blank'], { stdio: 'ignore' });
  const done = async code => { try { chrome.kill(); } catch { } server.close(); await sleep(500); try { fs.rmSync(profile, { recursive: true, force: true }); } catch { } process.exit(code); };
  let target;
  for (let i = 0; i < 60 && !target; i++) { await sleep(250); try { target = await (await fetch(`http://127.0.0.1:${PORT}/json/new?about:blank`, { method: 'PUT' })).json(); } catch { } }
  const ws = new WebSocket(target.webSocketDebuggerUrl);
  await new Promise(r => ws.addEventListener('open', r));
  let id = 0, recording = false, n = 0; const pend = new Map(), logs = [], times = [];
  const cdp = (method, params = {}) => new Promise(r => { const i = ++id; pend.set(i, r); ws.send(JSON.stringify({ id: i, method, params })); });
  ws.addEventListener('message', e => {
    const m = JSON.parse(e.data);
    if (m.id && pend.has(m.id)) { pend.get(m.id)(m); pend.delete(m.id); }
    if (m.method === 'Runtime.consoleAPICalled') {
      const line = (m.params.args || []).map(a => a.value ?? '').join(' ');
      if (/^\[(cerceve|dalis|gecis|mix|ses|yay|varis)\]/.test(line)) logs.push(m.params.timestamp.toFixed(1) + ' ' + line.slice(0, 1500));
    }
    if (m.method === 'Page.screencastFrame') {
      cdp('Page.screencastFrameAck', { sessionId: m.params.sessionId });
      if (!recording) return;
      fs.writeFileSync(path.join(outDir, `f${String(n++).padStart(4, '0')}.jpg`), Buffer.from(m.params.data, 'base64'));
      times.push(m.params.metadata.timestamp);
    }
  });
  const ev = async (x, awaitPromise = false) => (await cdp('Runtime.evaluate', { expression: x, returnByValue: true, awaitPromise })).result?.result?.value;
  const touch = (type, x, y) => cdp('Input.dispatchTouchEvent', { type, touchPoints: type === 'touchEnd' ? [] : [{ x, y, id: 1 }] });
  const tap = async (x, y) => { await touch('touchStart', x, y); await sleep(50); await touch('touchEnd'); };
  await cdp('Runtime.enable'); await cdp('Page.enable'); await cdp('Network.enable');
  await cdp('Network.setBlockedURLs', { urls: ['*supabase.co*'] });
  await cdp('Emulation.setDeviceMetricsOverride', { width: W, height: H, deviceScaleFactor: 2, mobile: true });
  await cdp('Emulation.setTouchEmulationEnabled', { enabled: true, maxTouchPoints: 5 });
  await cdp('Page.addScriptToEvaluateOnNewDocument', { source: TAP });
  const q = `c=p23&lang=tr&intro=0&seslog=1&dalislog=1&test=1${opt('query', '')}`;
  await cdp('Page.navigate', { url: `${base}/?${q}` });
  for (let i = 0; i < 120; i++) { await sleep(500); if (await ev(`!!(window.LT && LT.end)`)) break; }
  const sr = await ev('__pm100.start()');
  const epoch = await ev('__pm100.epoch');
  await sleep(5000);
  await tap(12, 430); await sleep(2500);   // the first touch: the audio starts (the lobby's music)
  if (FRAMES) await cdp('Page.startScreencast', { format: 'jpeg', quality: 85, maxWidth: W, maxHeight: H, everyNthFrame: 1 });
  recording = true;
  await sleep(700);
  await tap(W / 2, H * .79);   // Oyna
  await sleep(4800);
  recording = false;
  if (FRAMES) await cdp('Page.stopScreencast');
  const b64 = sr > 0 ? await ev('__pm100.stop()', true) : '';
  const m64 = sr > 0 ? await ev('__pm100.stopMusic()', true) : '';
  fs.writeFileSync(path.join(outDir, 'efektler.webm'), Buffer.from(b64 || '', 'base64'));
  if (m64) fs.writeFileSync(path.join(outDir, 'muzik.webm'), Buffer.from(m64, 'base64'));
  fs.writeFileSync(path.join(outDir, 'meta.json'), JSON.stringify({ query: q, epoch, musicStart: (await ev('__pm100.musicStart')) || 0 }));
  fs.writeFileSync(path.join(outDir, 'times.json'), JSON.stringify(times));
  fs.writeFileSync(path.join(outDir, 'log.txt'), logs.join(String.fromCharCode(10)) + String.fromCharCode(10));
  const sum = logs.filter(l => / \[dalis\] /.test(l)).pop() || '(no [dalis] line)';
  console.log(`[dalis108] ${n} frames, ${logs.filter(l => /\[cerceve\]/.test(l)).length} [cerceve], sound ${((b64 || '').length * .75 / 1e3).toFixed(0)} KB; ${sum.slice(sum.indexOf('[dalis]'))}`);
  ws.close(); await done(0);
})().catch(e => { console.error(e); process.exit(1); });
