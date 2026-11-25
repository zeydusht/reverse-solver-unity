// PM_107 §D2 (Zeyd: "the collection's background seems to slide / jump"): the frame times while the menu goes to the
// Koleksiyon tab and its grid is scrolled. Every requestAnimationFrame's gap is kept (Unity draws on rAF); a frame over
// 50 ms is a hitch the eye sees as a jump in a moving page. Real GPU Chrome, 390 x 844.
// --cpu 6: the CPU 6x slower (Chrome's throttling: a phone's pace, roughly)
// usage: node Tools/pm107_kare.js [--local Builds/publish/br] [--port 9371] [--query "&x=1"] [--cpu 6] [--out file.json]
const fs = require('fs'), path = require('path'), http = require('http'), os = require('os'), { spawn } = require('child_process');
const args = process.argv.slice(2);
const opt = (n, d) => { const i = args.indexOf('--' + n); return i >= 0 ? args[i + 1] : d; };
const ROOT = path.resolve(__dirname, '..');
const W = 390, H = 844, PORT = +opt('port', 9371);
const CHROME = 'C:/Program Files/Google/Chrome/Application/chrome.exe';
const sleep = ms => new Promise(r => setTimeout(r, ms));
function serve(root) {
  const types = { '.html': 'text/html', '.js': 'application/javascript', '.json': 'application/json', '.wasm': 'application/wasm', '.png': 'image/png', '.jpg': 'image/jpeg' };
  const server = http.createServer((req, res) => {
    let f = path.resolve(root, '.' + decodeURIComponent(new URL(req.url, 'http://x').pathname));
    if (!f.startsWith(root)) { res.writeHead(403); return res.end(); }
    if (fs.existsSync(f) && fs.statSync(f).isDirectory()) f = path.join(f, 'index.html');
    fs.stat(f, (err, s) => { if (err) { res.writeHead(404); return res.end(); } res.writeHead(200, { 'Content-Type': types[path.extname(f)] || 'application/octet-stream', 'Content-Length': s.size }); fs.createReadStream(f).pipe(res); });
  });
  return new Promise(r => server.listen(0, '127.0.0.1', () => r(server)));
}
(async () => {
  const server = await serve(path.resolve(ROOT, opt('local', 'Builds/publish/br')));
  const base = `http://127.0.0.1:${server.address().port}`;
  const profile = fs.mkdtempSync(path.join(os.tmpdir(), 'rs-kare-'));
  const chrome = spawn(CHROME, ['--headless=new', `--remote-debugging-port=${PORT}`, '--use-angle=d3d11', '--enable-gpu', '--ignore-gpu-blocklist', `--user-data-dir=${profile}`, '--no-first-run', 'about:blank'], { stdio: 'ignore' });
  let target;
  for (let i = 0; i < 60 && !target; i++) { await sleep(250); try { target = await (await fetch(`http://127.0.0.1:${PORT}/json/new?about:blank`, { method: 'PUT' })).json(); } catch { } }
  const ws = new WebSocket(target.webSocketDebuggerUrl);
  await new Promise(r => ws.addEventListener('open', r));
  let id = 0; const pend = new Map();
  const cdp = (method, params = {}) => new Promise(r => { const i = ++id; pend.set(i, r); ws.send(JSON.stringify({ id: i, method, params })); });
  ws.addEventListener('message', e => { const m = JSON.parse(e.data); if (m.id && pend.has(m.id)) { pend.get(m.id)(m); pend.delete(m.id); } });
  const ev = async x => (await cdp('Runtime.evaluate', { expression: x, returnByValue: true })).result?.result?.value;
  const touch = (type, x, y) => cdp('Input.dispatchTouchEvent', { type, touchPoints: type === 'touchEnd' ? [] : [{ x, y, id: 1 }] });
  const tap = async (x, y) => { await touch('touchStart', x, y); await sleep(50); await touch('touchEnd'); };
  const drag = async (x, y, dy, ms) => { await touch('touchStart', x, y); const n = Math.round(ms / 16); for (let i = 1; i <= n; i++) { await touch('touchMove', x, y + dy * i / n); await sleep(16); } await touch('touchEnd'); };
  await cdp('Runtime.enable'); await cdp('Page.enable'); await cdp('Network.enable');
  await cdp('Network.setBlockedURLs', { urls: ['*supabase.co*'] });
  await cdp('Emulation.setDeviceMetricsOverride', { width: W, height: H, deviceScaleFactor: 2, mobile: true });
  await cdp('Emulation.setTouchEmulationEnabled', { enabled: true, maxTouchPoints: 5 });
  await cdp('Page.navigate', { url: `${base}/?c=p23&test=1&lang=tr&intro=0${opt('query', '')}` });
  for (let i = 0; i < 120; i++) { await sleep(500); if (await ev('!!(window.LT && LT.end)')) break; }
  await sleep(5000); await tap(12, 430); await sleep(2500);
  if (+opt('cpu', 1) > 1) { await cdp('Emulation.setCPUThrottlingRate', { rate: +opt('cpu', 1) }); await sleep(1500); }
  await ev(`(function(){ window.__gaps = []; var last = performance.now(), mark = ''; window.__mark = function (m) { mark = m; }; function f(t) { window.__gaps.push([Math.round(t - last), mark]); last = t; requestAnimationFrame(f); } requestAnimationFrame(f); })()`);
  await sleep(500);
  const phases = [];
  const phase = async (name, act, ms) => { await ev(`__mark(${JSON.stringify(name)})`); await act(); await sleep(ms); };
  await phase('menü (durgun)', async () => { }, 800);
  await phase('Koleksiyon sekmesine geçiş', () => tap(182, 795), 1500);
  await phase('aşağı kaydırma', () => drag(195, 600, -260, 500), 1200);
  await phase('yukarı kaydırma', () => drag(195, 300, 260, 500), 1200);
  await phase('Ev sekmesine dönüş', () => tap(70, 795), 1500);
  await phase('Koleksiyon sekmesine ikinci geçiş', () => tap(182, 795), 1500);
  const gaps = await ev('window.__gaps');
  const by = {};
  for (const [g, m] of gaps) { if (!m) continue; (by[m] = by[m] || []).push(g); }
  const out = Object.entries(by).map(([m, g]) => { const s = [...g].sort((a, b) => a - b); return { faz: m, kare: g.length, medyan: s[s.length >> 1], en_uzun: s[s.length - 1], '50ms_ustu': g.filter(x => x > 50).length, ilk20: g.slice(0, 20) }; });
  for (const o of out) console.log(`[kare] ${o.faz}: ${o.kare} kare, medyan ${o.medyan} ms, en uzun ${o.en_uzun} ms, 50 ms üstü ${o['50ms_ustu']}  ilk kareler ${o.ilk20.join(' ')}`);
  if (opt('out')) fs.writeFileSync(opt('out'), JSON.stringify(out, null, 1));
  ws.close(); chrome.kill(); server.close(); await sleep(400); try { fs.rmSync(profile, { recursive: true, force: true }); } catch { }
  process.exit(0);
})().catch(e => { console.error(e); process.exit(1); });
