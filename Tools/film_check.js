// PM_88 §2: the opening film in the page, measured in a real Chrome (GPU, headless, a fresh profile = a first visit).
// usage: node Tools/film_check.js <out dir> [--local Builds/publish/br | --base https://…] [--net 4g|none] [--port 9371]
//        [--query "c=p23&lang=tr"]
//   1. the link opened (intro on its own: a first visit): times from the navigation to the film's first frame, to
//      the game loaded and to the game ready (it asks for the story; Skip shows), from window.rsFilm;
//   2. Skip tapped 1.5 s after the game is ready: every captured frame from the tap until the film has gone is
//      checked for an empty one (one colour over the screen: no film, no game);
//   3. the same link again (rs_lang now set: a returning visit): no film, the loader in the game's look (shot).
// --net 4g: a 4G line, --mbps down (10) and --rtt ms (60), 3 Mbit/s up; the cache off. (--mbps 4 --rtt 150: a slow one.)
// Frames go to <out dir>/film/f####.jpg with times.json; the loader shot to <out dir>/loader.png.
const fs = require('fs'), path = require('path'), http = require('http'), os = require('os'), { spawn } = require('child_process');
const args = process.argv.slice(2);
const opt = (n, d) => { const i = args.indexOf('--' + n); return i >= 0 ? args[i + 1] : d; };
const outDir = args[0];
const [W, H] = [390, 844], PORT = +opt('port', 9371), NET = opt('net', 'none');
const CHROME = 'C:/Program Files/Google/Chrome/Application/chrome.exe';
const sleep = ms => new Promise(r => setTimeout(r, ms));

function serve(root) {
  const types = { '.html': 'text/html', '.js': 'application/javascript', '.css': 'text/css', '.json': 'application/json', '.wasm': 'application/wasm', '.png': 'image/png', '.jpg': 'image/jpeg', '.mp4': 'video/mp4', '.ttf': 'font/ttf' };
  const server = http.createServer((req, res) => {
    let f = path.resolve(root, '.' + decodeURIComponent(new URL(req.url, 'http://x').pathname));
    if (!f.startsWith(root)) { res.writeHead(403); return res.end(); }
    if (fs.existsSync(f) && fs.statSync(f).isDirectory()) f = path.join(f, 'index.html');
    fs.stat(f, (err, s) => {
      if (err) { res.writeHead(404); return res.end(); }
      const range = req.headers.range && /bytes=(\d*)-(\d*)/.exec(req.headers.range);
      const type = types[path.extname(f)] || 'application/octet-stream';
      if (range) {   // video needs ranges (Safari and Chrome ask for them)
        const a = range[1] ? +range[1] : 0, b = range[2] ? +range[2] : s.size - 1;
        res.writeHead(206, { 'Content-Type': type, 'Content-Range': `bytes ${a}-${b}/${s.size}`, 'Accept-Ranges': 'bytes', 'Content-Length': b - a + 1 });
        return fs.createReadStream(f, { start: a, end: b }).pipe(res);
      }
      res.writeHead(200, { 'Content-Type': type, 'Content-Length': s.size, 'Accept-Ranges': 'bytes' });
      fs.createReadStream(f).pipe(res);
    });
  });
  return new Promise(r => server.listen(0, '127.0.0.1', () => r(server)));
}

(async () => {
  let base = opt('base', ''), server;
  if (!base) { server = await serve(path.resolve(opt('local', 'Builds/publish/br'))); base = `http://127.0.0.1:${server.address().port}`; }
  base = base.replace(/\/+$/, '');
  const filmDir = path.join(outDir, 'film');
  fs.mkdirSync(filmDir, { recursive: true });
  for (const f of fs.readdirSync(filmDir)) if (/^f\d+\.jpg$/.test(f)) fs.unlinkSync(path.join(filmDir, f));
  const profile = fs.mkdtempSync(path.join(os.tmpdir(), 'rs-film-'));
  const chrome = spawn(CHROME, ['--headless=new', `--remote-debugging-port=${PORT}`, '--use-angle=d3d11', '--enable-gpu', '--ignore-gpu-blocklist',
    '--autoplay-policy=no-user-gesture-required', `--user-data-dir=${profile}`, '--no-first-run', 'about:blank'], { stdio: 'ignore' });
  let target;
  for (let i = 0; i < 60 && !target; i++) { await sleep(250); try { target = await (await fetch(`http://127.0.0.1:${PORT}/json/new?about:blank`, { method: 'PUT' })).json(); } catch { } }
  const ws = new WebSocket(target.webSocketDebuggerUrl);
  await new Promise(r => ws.addEventListener('open', r));
  let id = 0, recording = false, n = 0; const pend = new Map(), times = [];
  const cdp = (method, params = {}) => new Promise(r => { const i = ++id; pend.set(i, r); ws.send(JSON.stringify({ id: i, method, params })); });
  ws.addEventListener('message', e => {
    const m = JSON.parse(e.data);
    if (m.id && pend.has(m.id)) { pend.get(m.id)(m); pend.delete(m.id); }
    if (m.method === 'Runtime.consoleAPICalled') {
      const text = m.params.args.map(a => a.value ?? '').join(' ');
      if (text.startsWith('[film]')) console.log(text.slice(0, 300));
    }
    if (m.method === 'Page.screencastFrame') {
      cdp('Page.screencastFrameAck', { sessionId: m.params.sessionId });
      if (!recording) return;
      fs.writeFileSync(path.join(filmDir, `f${String(n++).padStart(4, '0')}.jpg`), Buffer.from(m.params.data, 'base64'));
      times.push(m.params.metadata.timestamp);
    }
  });
  const ev = async x => (await cdp('Runtime.evaluate', { expression: x, returnByValue: true })).result?.result?.value;
  const touch = (type, x, y) => cdp('Input.dispatchTouchEvent', { type, touchPoints: type === 'touchEnd' ? [] : [{ x, y, id: 1 }] });
  await cdp('Page.enable'); await cdp('Runtime.enable'); await cdp('Network.enable');
  await cdp('Network.setBlockedURLs', { urls: ['*supabase.co*'] });
  await cdp('Network.setCacheDisabled', { cacheDisabled: true });
  const MBPS = +opt('mbps', 10), RTT = +opt('rtt', 60);
  if (NET === '4g') await cdp('Network.emulateNetworkConditions', { offline: false, latency: RTT, downloadThroughput: MBPS * 1e6 / 8, uploadThroughput: 3e6 / 8 });
  await cdp('Emulation.setDeviceMetricsOverride', { width: W, height: H, deviceScaleFactor: 2, mobile: true });
  await cdp('Emulation.setTouchEmulationEnabled', { enabled: true, maxTouchPoints: 5 });
  const query = opt('query', 'c=p23&lang=tr');
  await cdp('Page.startScreencast', { format: 'jpeg', quality: 70, maxWidth: W, maxHeight: H, everyNthFrame: 1 });
  recording = true;
  await cdp('Page.navigate', { url: `${base}/?${query}` });
  // 1. the film from the first moment; the game loading under it
  let F = null;
  for (let i = 0; i < 1800; i++) {
    await sleep(100);
    F = await ev('window.rsFilm ? { first: rsFilm.first, engine: rsFilm.engine, ready: rsFilm.ready, readyAt: rsFilm.readyAt, nav: performance.timeOrigin } : null');
    if (F && F.ready) break;
  }
  if (!F) { console.log('no film on the first visit'); }
  else {
    console.log(`first visit (${NET === '4g' ? MBPS + ' Mbit/s, ' + RTT + ' ms' : 'no limit'}): film's first frame ${(F.first / 1000).toFixed(2)} s, game loaded ${(F.engine / 1000).toFixed(2)} s, game ready ${(F.readyAt / 1000).toFixed(2)} s after the link`);
    // 2. Skip, 1.5 s after the game is ready (its button shows a second after the film's first frame)
    await sleep(1500);
    const r = await ev("(function(){var b=document.getElementById('rsIntroSkip'); if(!b||b.style.display==='none') return null; var q=b.getBoundingClientRect(); return [q.x+q.width/2,q.y+q.height/2];})()");
    if (!r) {
      // the film waited for the game (it ended first) and went on by itself: the hand-over is checked from there
      await sleep(2500);
      const G = await ev('({ ready: rsFilm.readyAt, gone: rsFilm.gone })');
      console.log(`no Skip (the film had ended and waited): game ready -> film gone ${((G.gone - G.ready) / 1000).toFixed(2)} s`);
      fs.writeFileSync(path.join(outDir, 'skip.json'), JSON.stringify({ tapAt: (F.nav + G.ready) / 1000, skip: G.ready, gone: G.gone, nav: F.nav, waited: true }));
    }
    else {
      const tapAt = Date.now() / 1000;
      await touch('touchStart', r[0], r[1]); await sleep(50); await touch('touchEnd');
      await sleep(2500);
      const G = await ev('({ skip: rsFilm.skip, gone: rsFilm.gone })');
      console.log(`skip -> film gone ${((G.gone - G.skip) / 1000).toFixed(2)} s`);
      fs.writeFileSync(path.join(outDir, 'skip.json'), JSON.stringify({ tapAt, skip: G.skip, gone: G.gone, nav: F.nav }));
    }
  }
  recording = false;
  await cdp('Page.stopScreencast');
  fs.writeFileSync(path.join(filmDir, 'times.json'), JSON.stringify(times));
  // 3. a returning visit: the loader in the game's look, no film
  await cdp('Page.navigate', { url: `${base}/?${query}` });
  let shot = false;
  for (let i = 0; i < 200; i++) {
    await sleep(50);
    const s = await ev("(function(){var l=document.getElementById('loader'); return { film: !!window.rsFilm, loader: !!l && !l.classList.contains('done'), fill: (document.getElementById('fill')||{}).style ? document.getElementById('fill').style.width : '' };})()");
    if (s && s.loader && !shot && i > 6) {
      fs.writeFileSync(path.join(outDir, 'loader.png'), Buffer.from((await cdp('Page.captureScreenshot', { format: 'png' })).result.data, 'base64'));
      shot = true; console.log(`returning visit: film ${s.film ? 'SHOWN (wrong)' : 'none'}, loader shot at ${s.fill}`);
    }
    if (s && !s.loader && i > 6) break;
  }
  console.log(`${n} frames -> ${filmDir}`);
  ws.close(); chrome.kill(); server?.close();
  await sleep(500);
  try { fs.rmSync(profile, { recursive: true, force: true }); } catch { }
  process.exit(0);
})().catch(e => { console.error(e); process.exit(1); });
