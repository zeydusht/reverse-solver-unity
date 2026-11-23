// PM_102 §2 and §5: records the game's own sounds (Unity's WebAudio output, tapped inside the page; and the music's,
// apart) with every [ses] / [gecis] / [booster ses] log line at its time on the recording's clock (<out>/log.json).
//   --mode jams    level 2 (single-cell pieces): 10 jams with room (a piece flicked into an emptied cell towards
//                  another) and 10 without (into a neighbour), each 1.2 s apart (<out>/moves.json)
//   --mode gecis   level 2 played by its solution and won; Next (the corridor, the dive into level 3); the pause's Main
//                  menu (level -> menu); a tab and back (the pages' swish); the menu's Play (the dive from the menu)
// Then Tools/pm102_measure.py: the knocks' peaks, or each transition's event against its sound's start.
// usage: node Tools/pm102_record.js <out dir> --mode jams|gecis [--local Builds/publish/br] [--port 9353]
const fs = require('fs'), path = require('path'), http = require('http'), os = require('os'), { spawn } = require('child_process');
const args = process.argv.slice(2);
const opt = (n, d) => { const i = args.indexOf('--' + n); return i >= 0 ? args[i + 1] : d; };
const outDir = path.resolve(args[0]);
const ROOT = path.resolve(__dirname, '..');
const W = 390, H = 844, PORT = +opt('port', 9353), MODE = opt('mode', 'jams');
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
      rec.start(100); this.rec = rec; this.chunks = chunks; this.t0 = performance.now(); this.epoch = Date.now();
      // the music's context too, apart (the corridor's whoosh against the music coming back)
      if (M && M.ctx && M.ctx.__tap) {
        var mo = M.ctx.createOscillator(), mg = M.ctx.createGain(); mo.frequency.value = 25; mg.gain.value = .003;
        connect.call(mo, mg); connect.call(mg, M.ctx.__tap); mo.start();
        var mrec = new MediaRecorder(M.ctx.__tap.stream, { mimeType: 'audio/webm;codecs=opus', audioBitsPerSecond: 256e3 });
        var mchunks = []; mrec.ondataavailable = function (e) { if (e.data.size) mchunks.push(e.data); };
        mrec.start(100); this.mrec = mrec; this.mchunks = mchunks;
      }
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
      if (/^\[(ses|gecis|booster ses|koridor|dalis)\]/.test(line)) { logs.push({ at: m.params.timestamp, line }); console.log(line); }
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
  const epoch = await ev('__pm100.epoch');
  console.log(`kayıt başladı (${sr} Hz)`);
  await sleep(1500);
  const moves = [];
  const at = new Map(); l.pieces.forEach((cells, i) => cells.forEach(c => at.set(c[0] + ',' + c[1], i)));
  const gone = new Set();
  const play = async (piece, dir, what) => {
    const c = l.pieces[piece][0], [ux, uy] = DIR[dir];
    const x0 = p.x + (c[0] + .5) * p.cell, y0 = p.y + (c[1] + .5) * p.cell;
    const room = what === 'jam' ? p.cell * .45 : what === 'nudge' ? p.cell * .3 : ux < 0 ? x0 - 1 : ux > 0 ? W - 1 - x0 : uy < 0 ? y0 - 1 : H - 1 - y0;
    const t = await ev('__pm100.now()');
    await drag(x0, y0, ux * room, uy * room, what === 'exit' ? 160 : 50);
    moves.push({ piece, dir, what, at: t, release: await ev('__pm100.now()') });
    await sleep(what === 'exit' ? 900 : 1200);
  };
  const shot = async name => { const r = await cdp('Page.captureScreenshot', { format: 'png' }); fs.writeFileSync(path.join(outDir, name + '.png'), Buffer.from(r.result.data, 'base64')); };
  const tap = async (x, y) => { await touch('touchStart', x, y); await sleep(50); await touch('touchEnd'); };
  const seen = re => logs.some(x => re.test(x.line));
  const waitLog = async (re, s) => { for (let i = 0; i < s * 4 && !seen(re); i++) await sleep(250); return seen(re); };
  // the next cell's piece (or -1: empty, -2: the edge)
  const ahead = (x, y) => (x < 0 || y < 0 || x >= l.w || y >= l.h) ? -2 : (at.has(x + ',' + y) && !gone.has(at.get(x + ',' + y)) ? at.get(x + ',' + y) : -1);
  const find = room => {
    for (let i = 0; i < l.pieces.length; i++) {
      if (gone.has(i) || l.pieces[i].length !== 1) continue;
      for (const [d, [ux, uy]] of Object.entries(DIR)) {
        let [x, y] = l.pieces[i][0], empty = 0, q;
        for (;;) { x += ux; y += uy; q = ahead(x, y); if (q === -1) { empty++; continue; } break; }
        if (q >= 0 && (room ? empty >= 2 : empty === 0)) return [i, d];
      }
    }
    return null;
  };
  if (MODE === 'jams') {
    let gap = null, nudge = find(false);
    for (const m of l.solution) { gap = find(true); if (gap) break; await play(m.piece, m.dir, 'exit'); gone.add(m.piece); }
    if (!nudge) nudge = find(false);
    console.log(`boşluklu takılma: parça ${gap}, boşluksuz: ${nudge}`);
    for (let k = 0; k < 10; k++) { if (gap) await play(gap[0], gap[1], 'jam'); if (nudge) await play(nudge[0], nudge[1], 'nudge'); }
  } else {
    // win level 2, then Next: the corridor and the dive into level 3
    for (const m of l.solution) { await play(m.piece, m.dir, 'exit'); gone.add(m.piece); }
    await sleep(3500);
    await shot('1_kazanma');
    // Next: one tap at a time, waiting for the corridor (a new touch would skip its slide)
    for (let y = Math.round(H * .5); y < H * .93 && !seen(/olay koridor açıldı/); y += 14) { await tap(W / 2, y); await waitLog(/olay koridor açıldı/, 1.6); }
    await waitLog(/olay HUD oturdu/, 14);
    await sleep(1500);
    await shot('2_bolum3');
    for (let y = Math.round(H * .45); y < H * .8; y += 18) { await tap(W / 2, y); await sleep(120); }   // level 3's card
    await sleep(1500);
    // the pause, its Main menu (the lowest card button): level -> menu
    await tap(W * .89, H * .933); await sleep(900);   // the HUD's pause (Durdur), bottom right
    await shot('3_duraklat');
    logs.push({ at: Date.now(), line: '[gecis] olay bölüm -> menü (dokunuş)' });
    for (let y = Math.round(H * .85); y > H * .4; y -= 14) { await tap(W / 2, y); await sleep(120); }
    await sleep(4000);
    await shot('4_menu');
    // a tab and back: the pages' swish
    await tap(W * .875, H - 40); await sleep(1500); await shot('5_sekme'); await tap(W * .125, H - 40); await sleep(1500);
    // the menu's Play: the dive from the menu
    logs.push({ at: Date.now(), line: '[gecis] olay menü -> bölüm (Oyna dokunuşu)' });
    await tap(W / 2, H * .79);   // the menu's Play ("Bölüm N")
    await waitLog(/olay HUD oturdu[\s\S]*$/, 1);
    for (let i = 0; i < 40; i++) { await sleep(250); if (logs.filter(x => /olay HUD oturdu/.test(x.line)).length >= 2) break; }
    await sleep(1500);
  }
  const b64 = sr > 0 ? await ev('__pm100.stop()', true) : '';
  const m64 = sr > 0 ? await ev('__pm100.stopMusic()', true) : '';
  fs.writeFileSync(path.join(outDir, 'efektler.webm'), Buffer.from(b64 || '', 'base64'));
  if (m64) fs.writeFileSync(path.join(outDir, 'muzik.webm'), Buffer.from(m64, 'base64'));
  fs.writeFileSync(path.join(outDir, 'moves.json'), JSON.stringify({ sampleRate: sr, mode: MODE, moves }, null, 1));
  fs.writeFileSync(path.join(outDir, 'log.json'), JSON.stringify(logs.map(x => ({ t: (x.at - epoch) / 1000, line: x.line })), null, 1));
  console.log(`webm ${((b64 || '').length * .75 / 1e3).toFixed(0)} KB, müzik ${((m64 || '').length * .75 / 1e3).toFixed(0)} KB, ${moves.length} hamle, ${logs.length} log satırı`);
  ws.close(); chrome.kill(); server.close();
  await sleep(500); try { fs.rmSync(profile, { recursive: true, force: true }); } catch { }
  process.exit(0);
})().catch(e => { console.error(e); process.exit(1); });
