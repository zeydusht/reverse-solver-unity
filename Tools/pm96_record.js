// PM_96 §5: records the win of sl100's level 3 with its sound, inside the page: the game's canvas (captureStream, 60 fps)
// and the music context's output (the level's music, the fanfare, the lobby's coming back; delayed by the context's
// output latency, so the sound is where the speaker plays it), as one WebM; the game's [vurus] / [muzik] log lines go
// to <out>/log.txt. The PM_96 music files (not on GitHub Pages yet) are served from Assets/Audio/Music96 and
// Fanfare96 (Fetch interception). Then Tools/pm96_measure.py measures each event against the fanfare's beats.
// usage: node Tools/pm96_record.js <out dir> [--local Builds/publish/br] [--query "&kazanma=e"] [--port 9351]
const fs = require('fs'), path = require('path'), http = require('http'), os = require('os'), { spawn } = require('child_process');
const args = process.argv.slice(2);
const opt = (n, d) => { const i = args.indexOf('--' + n); return i >= 0 ? args[i + 1] : d; };
const outDir = path.resolve(args[0]);
const ROOT = path.resolve(__dirname, '..');
const W = 390, H = 844, PORT = +opt('port', 9351);
const CHROME = 'C:/Program Files/Google/Chrome/Application/chrome.exe';
const sleep = ms => new Promise(r => setTimeout(r, ms));
const DIR = { L: [-1, 0], R: [1, 0], U: [0, -1], D: [0, 1] };
const sl100 = JSON.parse(fs.readFileSync(path.join(ROOT, 'Assets/_Game/Levels/sl100.json'), 'utf8')).levels;

function placement(l) {   // as Tools/verify_build.js (BoardLayout + GameRoot.BoardPlacement)
  const gap = Math.min(34, Math.max(24, (W - 44 * l.w) / 2));
  const cell = Math.floor(Math.min((W - 2 * gap) / l.w, (H - 262) / l.h, 78));
  const top = 4 + 10 + 46 + 6, bottom = H - 4 - (17 + 97);
  return { cell, x: (W - l.w * cell) / 2, y: (top + bottom - l.h * cell) / 2 };
}
function hudPlacement(l) {   // as Tools/verify_build.js (GameRoot.Hud54BoardPlacement)
  const s = W / 390, band = 18.2, air = 12;
  let cell = Math.min(placement(l).cell, Math.floor((W - 2 * (band + air)) / l.w));
  const top = Math.max(H * .19 + 9 * s, 168 * s), bottom = H - H * .086 - 15.5 * s;
  const margin = Math.max(band, cell * .25) + air;
  if (l.h * cell + 2 * margin > bottom - top) cell = Math.floor(Math.min(cell, (bottom - top - 2 * margin) / l.h));
  return { cell, x: (W - l.w * cell) / 2, y: (top + bottom - l.h * cell) / 2 };
}

function serve(root) {
  const types = { '.html': 'text/html', '.js': 'application/javascript', '.css': 'text/css', '.json': 'application/json', '.wasm': 'application/wasm', '.png': 'image/png', '.mp4': 'video/mp4' };
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

// the in-page recorder: the canvas and the music context, one WebM
const RECORDER = `window.__pm96 = {
  start: function () {
    var M = window.rsMusic, c = document.querySelector('canvas');
    var dest = M.ctx.createMediaStreamDestination(), delay = M.ctx.createDelay(1);
    var lat = 0;
    if (M.ctx.getOutputTimestamp) { var o = M.ctx.getOutputTimestamp(); if (o && o.contextTime > 0) lat = Math.max(0, M.ctx.currentTime - (o.contextTime + (performance.now() - o.performanceTime) / 1000)); }
    if (!lat) lat = (M.ctx.outputLatency || 0) + (M.ctx.baseLatency || 0);
    delay.delayTime.value = lat;
    M.gain.connect(delay); M.fan.gain.connect(delay); delay.connect(dest);
    var stream = new MediaStream(c.captureStream(60).getVideoTracks().concat(dest.stream.getAudioTracks()));
    var type = ['video/webm;codecs=h264,opus', 'video/webm;codecs=vp8,opus'].filter(function (t) { return MediaRecorder.isTypeSupported(t); })[0];
    var rec = new MediaRecorder(stream, { mimeType: type, videoBitsPerSecond: 8e6, audioBitsPerSecond: 192e3 });   // h264: the lightest encoder, no frames dropped
    var chunks = []; rec.ondataavailable = function (e) { if (e.data.size) chunks.push(e.data); };
    rec.start(200);
    this.rec = rec; this.chunks = chunks; this.lat = lat;
    return lat;
  },
  stop: function () {
    var self = this;
    return new Promise(function (ok) {
      self.rec.onstop = function () {
        new Blob(self.chunks, { type: 'video/webm' }).arrayBuffer().then(function (b) {
          var u = new Uint8Array(b), s = '';
          for (var i = 0; i < u.length; i += 32768) s += String.fromCharCode.apply(null, u.subarray(i, i + 32768));
          ok(btoa(s));
        });
      };
      self.rec.stop();
    });
  }
};`;

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
  const local = { 'music/pm96/': ['Music96', 'Music98', 'Fanfare96'].map(d => path.join(ROOT, 'Assets/Audio', d)) };   // PM_99: the PM_98 loops too
  ws.addEventListener('message', e => {
    const m = JSON.parse(e.data);
    if (m.id && pend.has(m.id)) { pend.get(m.id)(m); pend.delete(m.id); }
    if (m.method === 'Runtime.consoleAPICalled') {
      const line = (m.params.args || []).map(a => a.value).join(' ');
      if (/^\[(vurus|muzik|ses)\]/.test(line)) { logs.push(`${Date.now()} ${line}`); console.log(line); }
    }
    if (m.method === 'Fetch.requestPaused') {
      const u = m.params.request.url, name = u.split('/').pop();
      const file = local['music/pm96/'].map(d => path.join(d, name)).find(f => fs.existsSync(f));
      if (!file) { cdp('Fetch.continueRequest', { requestId: m.params.requestId }); return; }
      const type = name.endsWith('.wav') ? 'audio/wav' : 'audio/mpeg';
      cdp('Fetch.fulfillRequest', { requestId: m.params.requestId, responseCode: 200, body: fs.readFileSync(file).toString('base64'),
        responseHeaders: [{ name: 'Content-Type', value: type }, { name: 'Access-Control-Allow-Origin', value: '*' }] });
    }
  });
  const ev = async (x, awaitPromise = false) => (await cdp('Runtime.evaluate', { expression: x, returnByValue: true, awaitPromise })).result?.result?.value;
  const touch = (type, x, y) => cdp('Input.dispatchTouchEvent', { type, touchPoints: type === 'touchEnd' ? [] : [{ x, y, id: 1 }] });
  const drag = async (x, y, dx, dy, ms) => {
    await touch('touchStart', x, y); await sleep(40);
    const n = Math.max(3, Math.round(ms / 16));
    for (let i = 1; i <= n; i++) { await touch('touchMove', x + dx * i / n, y + dy * i / n); await sleep(16); }
    await touch('touchEnd'); await sleep(900);
  };
  await cdp('Runtime.enable'); await cdp('Page.enable'); await cdp('Network.enable');
  await cdp('Network.setBlockedURLs', { urls: ['*supabase.co*'] });
  await cdp('Fetch.enable', { patterns: [{ urlPattern: '*reverse-solver-unity/music/pm9*' }] });
  await cdp('Emulation.setDeviceMetricsOverride', { width: W, height: H, deviceScaleFactor: 2, mobile: true });
  await cdp('Emulation.setTouchEmulationEnabled', { enabled: true, maxTouchPoints: 5 });
  await cdp('Page.addScriptToEvaluateOnNewDocument', { source: RECORDER });
  await cdp('Page.navigate', { url: `${base}/?c=p23&test=1&lang=tr&lv=3&intro=0&vuruslog=1${opt('query', '')}` });
  for (let i = 0; i < 120; i++) { await sleep(1000); if (await ev(`!!(window.LT && LT.end)`)) break; }
  await sleep(6000);
  await touch('touchStart', 12, 430); await sleep(50); await touch('touchEnd');   // the first touch: the audio starts (left of the board)
  for (let i = 0; i < 40; i++) { await sleep(500); if (await ev(`!!(window.rsMusic && rsMusic.fan && rsMusic.fan.buf && rsMusic.tracks.level.v)`)) break; }
  await sleep(1500);
  const shot = async name => { if (!args.includes('--shots')) return; const r = await cdp('Page.captureScreenshot', { format: 'png' }); fs.writeFileSync(path.join(outDir, name + '.png'), Buffer.from(r.result.data, 'base64')); };
  await shot('0_start');
  await touch('touchStart', 195, 575); await sleep(60); await touch('touchEnd'); await sleep(1200);   // level 3's card: "Got it"
  await shot('0_card_closed');
  const l = sl100.find(x => x.lv === 3), p = hudPlacement(l);
  const moves = l.solution;
  const play = async m => {
    const c = l.pieces[m.piece][0], [ux, uy] = DIR[m.dir];
    const x0 = p.x + (c[0] + .5) * p.cell, y0 = p.y + (c[1] + .5) * p.cell;
    const room = ux < 0 ? x0 - 1 : ux > 0 ? W - 1 - x0 : uy < 0 ? y0 - 1 : H - 1 - y0;
    await drag(x0, y0, ux * room, uy * room, 160);
  };
  await sleep(3000);   // the lock lesson's first words
  // two rounds of all but the last move (the lesson can hold a move back; a gone piece's cell drags nothing), then the
  // recording and the last move; more rounds while no win screen comes
  for (let round = 0; round < 2; round++)
    for (let k = 0; k < moves.length - 1; k++) { await play(moves[k]); if (round === 0 && k < 2) await shot(`1_move${k + 1}`); }
  const lat = await ev('__pm96.start()');
  console.log(`kayıt başladı (çıkış gecikmesi ${(lat * 1000).toFixed(1)} ms)`);
  await sleep(4000);   // the encoder gives no frames in its first ~2.3 s
  await play(moves[moves.length - 1]);
  for (let round = 0; round < 3 && !logs.some(x => x.includes('[vurus]')); round++) {
    await sleep(1500);
    if (logs.some(x => x.includes('[vurus]'))) break;
    for (const m of moves) await play(m);
  }
  await sleep(5200);   // the win screen, the fanfare, the lobby's music back
  const b64 = await ev('__pm96.stop()', true);
  fs.writeFileSync(path.join(outDir, 'kayit.webm'), Buffer.from(b64 || '', 'base64'));
  fs.writeFileSync(path.join(outDir, 'log.txt'), logs.join('\n') + '\n');
  console.log(`webm ${((b64 || '').length * .75 / 1e6).toFixed(2)} MB, ${logs.length} log satırı`);
  ws.close(); chrome.kill(); server.close();
  await sleep(500); try { fs.rmSync(profile, { recursive: true, force: true }); } catch { }
  process.exit(0);
})().catch(e => { console.error(e); process.exit(1); });
