// PM_100 §2: records the game's own sound effects (Unity's WebAudio output, tapped inside the page) while a level of
// sl100 (--lv, default 2: single-cell pieces, no lesson) is played by its solution. Before each move, the first piece
// that now has an emptied cell in front of it and another piece further on is flicked that way: it slides into the gap
// and jams (the lift-off, then the knock at the contact). Later the same piece leaves by its own solution move (the
// exit); the recording stops after it. Every move's release is written to <out>/moves.json against the recording's
// clock, the game's [ses] lines (&seslog=1) to <out>/log.txt. Then Tools/pm100_measure.py measures each move's sound.
// usage: node Tools/pm100_record.js <out dir> [--local Builds/publish/br] [--lv 2] [--query "&sfx=1.5"] [--port 9352]
const fs = require('fs'), path = require('path'), http = require('http'), os = require('os'), { spawn } = require('child_process');
const args = process.argv.slice(2);
const opt = (n, d) => { const i = args.indexOf('--' + n); return i >= 0 ? args[i + 1] : d; };
const outDir = path.resolve(args[0]);
const ROOT = path.resolve(__dirname, '..');
const W = 390, H = 844, PORT = +opt('port', 9352);
const CHROME = 'C:/Program Files/Google/Chrome/Application/chrome.exe';
const sleep = ms => new Promise(r => setTimeout(r, ms));
const DIR = { L: [-1, 0], R: [1, 0], U: [0, -1], D: [0, 1] };
const sl100 = JSON.parse(fs.readFileSync(path.join(ROOT, 'Assets/_Game/Levels/sl100.json'), 'utf8')).levels;

function hudPlacement(l) {   // as Tools/verify_build.js (GameRoot.Hud54BoardPlacement)
  const s = W / 390, band = 18.2, air = 12;
  const gap = Math.min(34, Math.max(24, (W - 44 * l.w) / 2));
  let cell = Math.min(Math.floor(Math.min((W - 2 * gap) / l.w, (H - 262) / l.h, 78)), Math.floor((W - 2 * (band + air)) / l.w));
  const top = Math.max(H * .19 + 9 * s, 168 * s), bottom = H - H * .086 - 15.5 * s;
  const margin = Math.max(band, cell * .25) + air;
  if (l.h * cell + 2 * margin > bottom - top) cell = Math.floor(Math.min(cell, (bottom - top - 2 * margin) / l.h));
  return { cell, x: (W - l.w * cell) / 2, y: (top + bottom - l.h * cell) / 2 };
}

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
      rec.start(100); this.rec = rec; this.chunks = chunks; this.t0 = performance.now();
      return c.sampleRate;
    },
    now: function () { return (performance.now() - this.t0) / 1000; },
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
  const server = await serve(path.resolve(ROOT, opt('local', 'Builds/publish/br')));
  const base = `http://127.0.0.1:${server.address().port}`;
  const profile = fs.mkdtempSync(path.join(os.tmpdir(), 'rs-rec-'));
  const chrome = spawn(CHROME, ['--headless=new', `--remote-debugging-port=${PORT}`, '--use-angle=d3d11', '--enable-gpu', '--ignore-gpu-blocklist',
    '--autoplay-policy=no-user-gesture-required', `--user-data-dir=${profile}`, '--no-first-run', 'about:blank'], { stdio: 'ignore' });
  let target;
  for (let i = 0; i < 60 && !target; i++) { await sleep(250); try { target = await (await fetch(`http://127.0.0.1:${PORT}/json/new?about:blank`, { method: 'PUT' })).json(); } catch { } }
  const ws = new WebSocket(target.webSocketDebuggerUrl);
  await new Promise(r => ws.addEventListener('open', r));
  let id = 0; const pend = new Map(), logs = [];
  const cdp = (method, params = {}) => new Promise(r => { const i = ++id; pend.set(i, r); ws.send(JSON.stringify({ id: i, method, params })); });
  ws.addEventListener('message', e => {
    const m = JSON.parse(e.data);
    if (m.id && pend.has(m.id)) { pend.get(m.id)(m); pend.delete(m.id); }
    if (m.method === 'Runtime.consoleAPICalled') {
      const line = (m.params.args || []).map(a => a.value).join(' ');
      if (/^\[(ses|gecis)\]/.test(line)) { logs.push(line); console.log(line); }
    }
  });
  const ev = async (x, awaitPromise = false) => (await cdp('Runtime.evaluate', { expression: x, returnByValue: true, awaitPromise })).result?.result?.value;
  const touch = (type, x, y) => cdp('Input.dispatchTouchEvent', { type, touchPoints: type === 'touchEnd' ? [] : [{ x, y, id: 1 }] });
  const drag = async (x, y, dx, dy, ms) => {
    await touch('touchStart', x, y); await sleep(40);
    const n = Math.max(3, Math.round(ms / 16));
    for (let i = 1; i <= n; i++) { await touch('touchMove', x + dx * i / n, y + dy * i / n); await sleep(16); }
    await touch('touchEnd');
  };
  await cdp('Runtime.enable'); await cdp('Page.enable'); await cdp('Network.enable');
  await cdp('Network.setBlockedURLs', { urls: ['*supabase.co*'] });
  await cdp('Emulation.setDeviceMetricsOverride', { width: W, height: H, deviceScaleFactor: 2, mobile: true });
  await cdp('Emulation.setTouchEmulationEnabled', { enabled: true, maxTouchPoints: 5 });
  await cdp('Page.addScriptToEvaluateOnNewDocument', { source: TAP });
  const LV = +opt('lv', 2);
  await cdp('Page.navigate', { url: `${base}/?c=p23&test=1&lang=tr&lv=${LV}&intro=0&seslog=1${opt('query', '')}` });
  for (let i = 0; i < 120; i++) { await sleep(1000); if (await ev(`!!(window.LT && LT.end)`)) break; }
  await sleep(6000);
  await touch('touchStart', 12, 430); await sleep(50); await touch('touchEnd');   // the first touch: the audio starts
  await sleep(1500);
  await touch('touchStart', 12, 700); await sleep(60); await touch('touchEnd'); await sleep(1200);   // a card, if any
  await sleep(3000);
  const l = sl100.find(x => x.lv === LV), p = hudPlacement(l);
  const sr = await ev('__pm100.start()');
  console.log(`kayıt başladı (${sr} Hz); bağlamlar: ${JSON.stringify(await ev('__pm100.contexts'))}`);
  await sleep(1500);
  const moves = [];
  const play = async (piece, dir, what) => {
    const c = l.pieces[piece][0], [ux, uy] = DIR[dir];
    const x0 = p.x + (c[0] + .5) * p.cell, y0 = p.y + (c[1] + .5) * p.cell;
    // an exit: dragged off the board; a jam: a short flick (the piece then slides on to its contact by itself), quick
    // (a fast slide: ~30 ms to the contact) or gentle (a slower one)
    const room = what === 'jam' ? p.cell * .45 : what === 'jam-short' ? p.cell * .2 : what === 'jam-gentle' ? p.cell * .35 : ux < 0 ? x0 - 1 : ux > 0 ? W - 1 - x0 : uy < 0 ? y0 - 1 : H - 1 - y0;
    const at = await ev('__pm100.now()');
    await drag(x0, y0, ux * room, uy * room, what === 'jam' ? 50 : what === 'jam-short' ? 25 : what === 'jam-gentle' ? 90 : 160);
    const release = await ev('__pm100.now()');
    moves.push({ piece, dir, what, at, release });
    await sleep(1300);
  };
  // the board's cells (single-cell pieces): a jam with room is a piece whose next cell is empty and a piece further on
  const at = new Map(); l.pieces.forEach((cells, i) => cells.forEach(c => at.set(c[0] + ',' + c[1], i)));
  const gone = new Set(); let jammed = -1;
  const gapJam = need => {
    for (let i = 0; i < l.pieces.length; i++) {
      if (gone.has(i) || l.pieces[i].length !== 1) continue;
      for (const [d, [ux, uy]] of Object.entries(DIR)) {
        let [x, y] = l.pieces[i][0], empty = 0;
        for (;;) {
          x += ux; y += uy;
          if (x < 0 || y < 0 || x >= l.w || y >= l.h) break;
          const q = at.get(x + ',' + y);
          if (q === undefined || gone.has(q)) { empty++; continue; }
          if (empty >= need) return [i, d];
          break;
        }
      }
    }
    return null;
  };
  for (const m of l.solution) {
    if (jammed < 0) {
      const j = gapJam(2) || (m === l.solution[Math.floor(l.solution.length / 2)] ? gapJam(1) : null);   // two cells of room if it comes
      if (j) { jammed = j[0]; await play(j[0], j[1], 'jam'); await play(j[0], j[1], 'jam-short'); await play(j[0], j[1], 'jam-gentle'); }
    }
    await play(m.piece, m.dir, m.piece === jammed ? 'exit-same' : 'exit');
    gone.add(m.piece);
    if (m.piece === jammed) break;
  }
  await sleep(800);
  const b64 = sr > 0 ? await ev('__pm100.stop()', true) : '';
  fs.writeFileSync(path.join(outDir, 'efektler.webm'), Buffer.from(b64 || '', 'base64'));
  fs.writeFileSync(path.join(outDir, 'moves.json'), JSON.stringify({ sampleRate: sr, moves }, null, 1));
  fs.writeFileSync(path.join(outDir, 'log.txt'), logs.join('\n') + '\n');
  console.log(`webm ${((b64 || '').length * .75 / 1e3).toFixed(0)} KB, ${moves.length} hamle, ${logs.length} log satırı`);
  ws.close(); chrome.kill(); server.close();
  await sleep(500); try { fs.rmSync(profile, { recursive: true, force: true }); } catch { }
  process.exit(0);
})().catch(e => { console.error(e); process.exit(1); });
