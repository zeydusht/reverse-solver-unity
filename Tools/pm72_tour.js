// PM_72: screenshot tour (gif_capture.js plus ["solve", n, k]: plays the first k moves (all when omitted) of p22 level n's
// stored solution, from Logs/pm72/p22_levels.json (Pm72Tour.Dump)). Frames are off unless ["rec", true].
// Records a scripted play of a build as frames, then a GIF (Tools/gif_encode.py).
// usage: node Tools/gif_capture.js <scenario.json> <out dir> [--local Builds/publish/br | --base https://…]
// scenario: { "url": "?c=p22&test=1&lang=tr&lv=12", "wait": 9000, "steps": [ step, … ] }
//   step: ["wait", ms] | ["tap", x, y] | ["flick", x, y, dx, dy, ms] | ["drag", x, y, dx, dy, ms, holdMs]
//         | ["rec", true|false]   (frames are kept only while recording; default on after load)
//         | ["name", "GIF"]       (fills the first-visit name gate)
//         | ["shot", "file"]      (a full-size PNG screenshot to <out dir>/file.png)
//         | ["nav", "?query"]     (opens another URL of the same build, storage kept)
//         | ["eval", "js"]        (prints the value of a page expression, e.g. the load timings)
// The Chrome profile lives in %TEMP% and is deleted afterwards; Supabase is blocked (no rows written).
// One tour at a time: Chrome's debugging port is fixed (--port to change). Under a heavy Unity build a run can
// come back empty (no times.json); run it again.
const fs = require('fs'), path = require('path'), http = require('http'), os = require('os'), { spawn } = require('child_process');
const args = process.argv.slice(2);
const opt = (n, d) => { const i = args.indexOf('--' + n); return i >= 0 ? args[i + 1] : d; };
const [scenarioFile, outDir] = args;
const scenario = JSON.parse(fs.readFileSync(scenarioFile, 'utf8'));
const LEVELS = JSON.parse(fs.readFileSync(opt('levels', 'Logs/pm72/p22_levels.json'), 'utf8'));
const DIRS = { L: [-1, 0], R: [1, 0], U: [0, -1], D: [0, 1] };
const [W, H] = opt('size', '390x844').split('x').map(Number), PORT = +opt('port', 9342);   // PM_78: --size 375x667
const CHROME = 'C:/Program Files/Google/Chrome/Application/chrome.exe';
const sleep = ms => new Promise(r => setTimeout(r, ms));

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

(async () => {
  let base = opt('base', ''), server;
  if (!base) { server = await serve(path.resolve(opt('local', 'Builds/publish/br'))); base = `http://127.0.0.1:${server.address().port}`; }
  base = base.replace(/\/+$/, '');
  fs.mkdirSync(outDir, { recursive: true });
  for (const f of fs.readdirSync(outDir)) if (/^f\d+\.jpg$/.test(f)) fs.unlinkSync(path.join(outDir, f));
  const profile = fs.mkdtempSync(path.join(os.tmpdir(), 'rs-gif-'));
  const chrome = spawn(CHROME, ['--headless=new', `--remote-debugging-port=${PORT}`, '--use-angle=d3d11', '--enable-gpu', '--ignore-gpu-blocklist',
    `--user-data-dir=${profile}`, '--no-first-run', 'about:blank'], { stdio: 'ignore' });
  let target;
  for (let i = 0; i < 60 && !target; i++) { await sleep(250); try { target = await (await fetch(`http://127.0.0.1:${PORT}/json/new?about:blank`, { method: 'PUT' })).json(); } catch { } }
  const ws = new WebSocket(target.webSocketDebuggerUrl);
  await new Promise(r => ws.addEventListener('open', r));
  let id = 0, recording = false, n = 0; const pend = new Map(), times = [];
  const cdp = (method, params = {}) => new Promise(r => { const i = ++id; pend.set(i, r); ws.send(JSON.stringify({ id: i, method, params })); });
  ws.addEventListener('message', e => {
    const m = JSON.parse(e.data);
    if (m.id && pend.has(m.id)) { pend.get(m.id)(m); pend.delete(m.id); }
    if (m.method === 'Runtime.consoleAPICalled' && process.env.TOUR_LOG) {
      const text = m.params.args.map(a => a.value ?? '').join(' ');
      if (text.includes(process.env.TOUR_LOG)) console.log(text.slice(0, 300));
    }
    if (m.method === 'Page.screencastFrame') {
      cdp('Page.screencastFrameAck', { sessionId: m.params.sessionId });
      if (!recording) return;
      fs.writeFileSync(path.join(outDir, `f${String(n++).padStart(4, '0')}.jpg`), Buffer.from(m.params.data, 'base64'));
      times.push(m.params.metadata.timestamp);
    }
  });
  const ev = async x => (await cdp('Runtime.evaluate', { expression: x, returnByValue: true })).result?.result?.value;
  const touch = (type, x, y) => cdp('Input.dispatchTouchEvent', { type, touchPoints: type === 'touchEnd' ? [] : [{ x, y, id: 1 }] });
  const move = async (x, y, dx, dy, ms, hold = 0) => {
    await touch('touchStart', x, y); await sleep(30);
    const steps = Math.max(3, Math.round(ms / 16));
    for (let i = 1; i <= steps; i++) { await touch('touchMove', x + dx * i / steps, y + dy * i / steps); await sleep(16); }
    if (hold) await sleep(hold);
    await touch('touchEnd');
  };
  await cdp('Page.enable'); await cdp('Runtime.enable'); await cdp('Network.enable');
  await cdp('Network.setBlockedURLs', { urls: ['*supabase.co*'] });
  await cdp('Emulation.setDeviceMetricsOverride', { width: W, height: H, deviceScaleFactor: 2, mobile: true });
  await cdp('Emulation.setTouchEmulationEnabled', { enabled: true, maxTouchPoints: 5 });
  if (opt('safe')) {   // --safe top,bottom: an iPhone home-screen notch and home bar (env(safe-area-inset-*))
    const [top, bottom] = opt('safe').split(',').map(Number);
    const r = await cdp('Emulation.setSafeAreaInsetsOverride', { insets: { top, topMax: top, bottom, bottomMax: bottom, left: 0, leftMax: 0, right: 0, rightMax: 0 } });
    if (r.error) console.log('safe area override failed:', r.error.message);
  }
  const url = new URL(base + '/' + scenario.url.replace(/^\//, ''));
  if (!url.searchParams.has('intro')) url.searchParams.set('intro', '0');
  await cdp('Page.navigate', { url: url.href });
  for (const ms of scenario.early || []) {   // "early": [ms, …] screenshots while the page loads (the loader)
    await sleep(ms);
    fs.writeFileSync(path.join(outDir, `early_${ms}.png`), Buffer.from((await cdp('Page.captureScreenshot', { format: 'png' })).result.data, 'base64'));
  }
  for (let i = 0; i < 120; i++) { await sleep(1000); if (await ev("!!(window.LT && LT.end) || !!document.getElementById('rs-name')")) break; }
  await sleep(scenario.wait ?? 6000);
  await cdp('Page.startScreencast', { format: 'jpeg', quality: 80, maxWidth: W, maxHeight: H, everyNthFrame: 1 });
  recording = !!scenario.rec;
  for (const s of scenario.steps) {
    const [kind, ...a] = s;
    if (kind === 'wait') await sleep(a[0]);
    else if (kind === 'tap') { await touch('touchStart', a[0], a[1]); await sleep(60); await touch('touchEnd'); }
    else if (kind === 'flick') await move(a[0], a[1], a[2], a[3], a[4] ?? 70);
    else if (kind === 'drag') await move(a[0], a[1], a[2], a[3], a[4] ?? 320, a[5] ?? 0);
    else if (kind === 'rec') recording = a[0];
    else if (kind === 'solve') {
      const l = LEVELS.find(x => x.n === a[0]);
      const from = a[3] ?? 0;
      for (const [x, y, d] of l.sol.slice(from, from + (a[1] ?? l.sol.length))) {
        const [ux, uy] = DIRS[d], room = ux < 0 ? x - 2 : ux > 0 ? W - 2 - x : uy < 0 ? y - 2 : H - 2 - y;
        await move(x, y, ux * room, uy * room, 160); await sleep(a[2] ?? 650);
      }
    }
    else if (kind === 'shot') fs.writeFileSync(path.join(outDir, a[0] + '.png'), Buffer.from((await cdp('Page.captureScreenshot', { format: 'png' })).result.data, 'base64'));
    else if (kind === 'nav') {
      const u = new URL(base + '/' + a[0].replace(/^\//, '')); if (!u.searchParams.has('intro')) u.searchParams.set('intro', '0');
      await cdp('Page.navigate', { url: u.href });
      for (let i = 0; i < 120; i++) { await sleep(1000); if (await ev("!!(window.LT && LT.end) || !!document.getElementById('rs-name')")) break; }
      await sleep(a[1] ?? 4000);
    }
    else if (kind === 'eval') console.log('[eval]', JSON.stringify(await ev(a[0])));
    else if (kind === 'name') await ev(`(function(){ var i=document.getElementById('rs-name-in'); if(!i) return false; i.value='${a[0]}'; document.getElementById('rs-name-go').click(); return true; })()`);
  }
  await cdp('Page.stopScreencast');
  fs.writeFileSync(path.join(outDir, 'times.json'), JSON.stringify(times));
  console.log(`${n} frames -> ${outDir}`);
  ws.close(); chrome.kill(); server?.close();
  await sleep(500);
  try { fs.rmSync(profile, { recursive: true, force: true }); } catch { }
  process.exit(0);
})().catch(e => { console.error(e); process.exit(1); });
