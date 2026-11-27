// PM_116: in-game screenshots for the colour check (390 x 844 at 2x): each level opened straight (&lv=N), the board
// settled, then a piece held under the finger (the lifted / selected look), for each variant (&renk=...).
// usage: node Tools/pm116_shots.js <out dir> <levels, e.g. 1,3,7> [--variants "0,canli"] [--local Builds/publish/br] [--port 9381] [--card 3,7 (levels with a lesson card)] [--cardat 195,568] [--hold 195,430]
const fs = require('fs'), path = require('path'), http = require('http'), os = require('os'), { spawn } = require('child_process');
const args = process.argv.slice(2);
const opt = (n, d) => { const i = args.indexOf('--' + n); return i >= 0 ? args[i + 1] : d; };
const outDir = path.resolve(args[0]), levels = args[1].split(','), variants = opt('variants', '0,canli').split(',');
const cards = opt('card', '').split(',');
const ROOT = path.resolve(__dirname, '..'), W = 390, H = 844, PORT = +opt('port', 9381);
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
(async () => {
  fs.mkdirSync(outDir, { recursive: true });
  const server = await serve(path.resolve(ROOT, opt('local', 'Builds/publish/br')));
  const base = `http://127.0.0.1:${server.address().port}`;
  const profile = fs.mkdtempSync(path.join(os.tmpdir(), 'rs-shots-'));
  const chrome = spawn(CHROME, ['--headless=new', `--remote-debugging-port=${PORT}`, '--use-angle=d3d11', '--enable-gpu', '--ignore-gpu-blocklist',
    `--user-data-dir=${profile}`, '--no-first-run', 'about:blank'], { stdio: 'ignore' });
  let target;
  for (let i = 0; i < 60 && !target; i++) { await sleep(250); try { target = await (await fetch(`http://127.0.0.1:${PORT}/json/new?about:blank`, { method: 'PUT' })).json(); } catch { } }
  const ws = new WebSocket(target.webSocketDebuggerUrl);
  await new Promise(r => ws.addEventListener('open', r));
  let id = 0; const pend = new Map();
  const cdp = (method, params = {}) => new Promise(r => { const i = ++id; pend.set(i, r); ws.send(JSON.stringify({ id: i, method, params })); });
  ws.addEventListener('message', e => { const m = JSON.parse(e.data); if (m.id && pend.has(m.id)) { pend.get(m.id)(m); pend.delete(m.id); } });
  const ev = async x => (await cdp('Runtime.evaluate', { expression: x, returnByValue: true })).result?.result?.value;
  const touch = (type, x, y) => cdp('Input.dispatchTouchEvent', { type, touchPoints: type === 'touchEnd' ? [] : [{ x, y, id: 1 }] });
  const shot = async f => fs.writeFileSync(path.join(outDir, f), Buffer.from((await cdp('Page.captureScreenshot', { format: 'png' })).result.data, 'base64'));
  await cdp('Runtime.enable'); await cdp('Page.enable'); await cdp('Network.enable');
  await cdp('Network.setBlockedURLs', { urls: ['*supabase.co*'] });
  await cdp('Emulation.setDeviceMetricsOverride', { width: W, height: H, deviceScaleFactor: 2, mobile: true });
  await cdp('Emulation.setTouchEmulationEnabled', { enabled: true, maxTouchPoints: 5 });
  for (const lv of levels) for (const v of variants) {
    await cdp('Page.navigate', { url: `${base}/?c=p23&lang=tr&intro=0&test=1&lv=${lv}${v === '0' ? '' : '&renk=' + v}` });
    for (let i = 0; i < 120; i++) { await sleep(500); if (await ev(`!!(window.LT && LT.end)`)) break; }
    await sleep(6500);   // the level's intro card (if any) and the board settled
    if (cards.includes(lv)) {   // the lesson's card: tap its button (Anladım), then the board settles
      const [cx, cy] = opt('cardat', '195,568').split(',').map(Number);
      await touch('touchStart', cx, cy); await sleep(80); await touch('touchEnd'); await sleep(2500);
    }
    await shot(`lv${lv}_${v}.png`);
    const [hx, hy] = (opt('hold', '195,430')).split(',').map(Number);
    await touch('touchStart', hx, hy); await sleep(120);
    for (let k = 1; k <= 4; k++) { await touch('touchMove', hx, hy + 3 * k); await sleep(30); }   // a short drag: the piece lifted, as under the finger
    await sleep(350);
    await shot(`lv${lv}_${v}_tut.png`);
    await touch('touchEnd'); await sleep(300);
    console.log(`[shots] lv ${lv} ${v}`);
  }
  ws.close(); try { chrome.kill(); } catch { } server.close(); await sleep(500); try { fs.rmSync(profile, { recursive: true, force: true }); } catch { }
  process.exit(0);
})().catch(e => { console.error(e); process.exit(1); });
