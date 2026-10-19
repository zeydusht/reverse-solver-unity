#!/usr/bin/env node
// Build verification without a phone (PRODUCT.md, Build 3 protocol). Starts its own headless
// Chrome on the real GPU (swiftshader runs ~3 FPS: too slow to win a level or measure FPS) with
// a throwaway profile, at 375x667, and checks a published or local build:
//
//   boot   ?lv=1&debug=1: the boot check reports 40/40, no page exceptions
//   k1     12 edge pulls by touch on L01 and L40 (centre, outer half, from outside the board,
//          flick): each must leave on the first try (Exited in the debug line; >= 9 of 10 passes)
//   fps    10 s of continuous dragging on L40 (colours) and D02 (picture): median / p95 / worst,
//          from requestAnimationFrame intervals (recorded; under 55 median is flagged)
//   namekeys  the name gate with real key events (Input.dispatchKeyEvent): the typed name lands in
//          the input and Enter starts the game. (The other checks set the input's value directly,
//          which is how a keyboard capture bug once slipped through.) Nothing is sent: supabase.co
//          is blocked and the profile is thrown away.
//   card   D01 won at 375x667: card bottom <= 667 pt, primary button >= 44 pt,
//          gap between the title and the picture's frame >= 8 pt
//   redesign  the Faz 4 set (redesign.json) with ?debug=1&set=redesign: its first level opens
//   zeydbot   the ZeydBot set (zeydbot.json) with ?debug=1&set=zeydbot (nothing sent): Z01 opens and its stored solution wins
//          and is won with its stored solution (result card shown, no exceptions); skipped
//          while redesign.json has no levels
//
// Without --send nothing reaches Supabase: every page runs with ?debug=1 (debug never sends)
// and supabase.co is blocked as well. With --send --player TEST_xxx it also runs, as that player:
//   a6b    new profile, ?lv=2: name gate, 20 s wait, name, win L02: one row, attempt 1,
//          dur close to the time after the name (the gate is not counted), no keyif value
//          (a row Supabase refused, e.g. for an unknown column, shows up as 0 new rows)
//   m5     reload keeps the name; a hidden page sends one quit row; a debug win sends nothing;
//          row_id unique
// Rows are read back with the publishable (anon) key, the same one the game ships with.
//
// usage: node Tools/verify_build.js <base url of the build, e.g. https://…/br>  |  --local <build folder, e.g. Builds/publish/br>
//                                   [--out dir] [--only boot,k1,fps,card] [--send --player TEST_NAME]
//                                   [--chrome path] [--port 9333]
// Exit code 0 when every check passed. Screenshots go to --out (default: a temp folder).
const fs = require('fs'), os = require('os'), path = require('path'), { spawn } = require('child_process');
const { readPng } = require('./lib/png.js');

const args = process.argv.slice(2);
const flag = n => args.includes('--' + n);
const opt = (n, d) => { const i = args.indexOf('--' + n); return i >= 0 ? args[i + 1] : d; };
const local = opt('local', null);
let base = local ? '' : (args[0] || '').replace(/\/+$/, '');
if (!local && !/^https?:\/\//.test(base)) { console.error('usage: node Tools/verify_build.js <base url> | --local <build folder>  [--send --player TEST_NAME]'); process.exit(2); }
if (local && !fs.existsSync(path.join(local, 'index.html'))) { console.error(`--local: no index.html in ${local}`); process.exit(2); }
const send = flag('send'), player = opt('player', '');
if (send && !/^TEST_[A-Z0-9_]+$/.test(player)) { console.error('--send needs --player TEST_NAME (TEST_ names are left out of the analysis)'); process.exit(2); }
if (!send && player) { console.error('--player only makes sense with --send'); process.exit(2); }
const only = opt('only', 'boot,namekeys,k1,fps,card,redesign,zeydbot' + (send ? ',a6b,m5' : '')).split(',');
const out = opt('out', null) || fs.mkdtempSync(path.join(os.tmpdir(), 'rs-verify-'));
fs.mkdirSync(out, { recursive: true });
const PORT = +opt('port', 9333);
const CHROME = opt('chrome', ['C:/Program Files/Google/Chrome/Application/chrome.exe', 'C:/Program Files (x86)/Google/Chrome/Application/chrome.exe',
  '/Applications/Google Chrome.app/Contents/MacOS/Google Chrome', '/usr/bin/google-chrome'].find(p => fs.existsSync(p)));
const ROOT = path.join(__dirname, '..');
const levels = JSON.parse(fs.readFileSync(path.join(ROOT, 'Assets/_Game/Levels/levels.json'), 'utf8')).levels;
const designs = JSON.parse(fs.readFileSync(path.join(ROOT, 'Assets/_Game/Levels/designs.json'), 'utf8')).levels;
const KEY = 'sb_publishable_S-DzN5GrUyMR2izo8vHc3Q_0kaGiVDK';   // publishable key, as in TelemetrySettings.cs
const SUPA = 'https://vwneqdhzfevgbuwoadod.supabase.co';
const W = 375, H = 667;
const sleep = ms => new Promise(r => setTimeout(r, ms));

/* Edge pieces of L01 and L40 that can leave outward at the start (single cell, no nail, no wall):
   found with the rule engine (TravelRule.Limit(...).Exit on the start board). The live 40 levels
   are frozen; if levels.json changes, re-derive them. [level, x, y, direction] */
const EDGES = [['L01', 0, 0, 'L'], ['L01', 3, 0, 'R'], ['L01', 0, 4, 'L'], ['L01', 2, 4, 'D'], ['L01', 3, 4, 'D'],
  ['L40', 0, 0, 'L'], ['L40', 3, 0, 'U'], ['L40', 4, 0, 'U'], ['L40', 5, 0, 'U'], ['L40', 0, 1, 'L'], ['L40', 5, 4, 'R'], ['L40', 0, 6, 'L']];
const DIR = { L: [-1, 0], R: [1, 0], U: [0, -1], D: [0, 1] };

function placement(l) {   // BoardLayout + GameRoot.BoardPlacement at W x H, no safe insets
  const gap = Math.max(19, Math.min(34, (W - 44 * l.w) / 2));
  const cell = Math.floor(Math.min((W - 2 * gap) / l.w, (H - 262) / l.h, 78));
  const top = 4 + 10 + 46 + 6, bottom = H - 4 - (17 + 97);
  return { cell, x: (W - l.w * cell) / 2, y: (top + bottom - l.h * cell) / 2 };
}
const report = [];
const check = (name, pass, detail) => { report.push({ name, pass, detail }); console.log(`${pass ? 'GEÇTİ' : 'KALDI'}  ${name}: ${detail}`); };

async function startChrome() {
  if (!CHROME) throw new Error('Chrome not found; pass --chrome');
  const profile = fs.mkdtempSync(path.join(os.tmpdir(), 'rs-chrome-'));
  const proc = spawn(CHROME, ['--headless=new', `--remote-debugging-port=${PORT}`, '--use-angle=d3d11', '--enable-gpu', '--ignore-gpu-blocklist',
    `--user-data-dir=${profile}`, '--no-first-run', 'about:blank'], { stdio: 'ignore' });
  for (let i = 0; i < 40; i++) { await sleep(250); try { await (await fetch(`http://127.0.0.1:${PORT}/json/version`)).json(); proc.profile = profile; return proc; } catch { } }
  throw new Error('Chrome did not start');
}

async function newPage() {
  const t = await (await fetch(`http://127.0.0.1:${PORT}/json/new?about:blank`, { method: 'PUT' })).json();
  const ws = new WebSocket(t.webSocketDebuggerUrl);
  await new Promise(r => ws.addEventListener('open', r));
  let id = 0; const pend = new Map(); const page = { exceptions: [], supabase: [] };
  ws.addEventListener('message', e => {
    const m = JSON.parse(e.data);
    if (m.id && pend.has(m.id)) { pend.get(m.id)(m); pend.delete(m.id); }
    if (m.method === 'Runtime.exceptionThrown') page.exceptions.push(m.params.exceptionDetails.exception?.description || m.params.exceptionDetails.text);
    if (m.method === 'Network.requestWillBeSent' && /supabase\.co/.test(m.params.request.url)) page.supabase.push(m.params.request.method);
  });
  const cdp = (method, params = {}) => new Promise(r => { const i = ++id; pend.set(i, r); ws.send(JSON.stringify({ id: i, method, params })); });
  Object.assign(page, {
    cdp, close: () => ws.close(),
    ev: async x => (await cdp('Runtime.evaluate', { expression: x, returnByValue: true })).result?.result?.value,
    png: async (clip) => Buffer.from((await cdp('Page.captureScreenshot', { format: 'png', ...(clip ? { clip: { ...clip, scale: 2 } } : {}) })).result.data, 'base64'),
    touch: (type, x, y) => cdp('Input.dispatchTouchEvent', { type, touchPoints: type === 'touchEnd' ? [] : [{ x, y, id: 1 }] }),
    mouse: (type, x, y) => cdp('Input.dispatchMouseEvent', { type, x, y, button: 'left', buttons: type === 'mouseReleased' ? 0 : 1, clickCount: 1 }),
  });
  await cdp('Runtime.enable'); await cdp('Page.enable'); await cdp('Network.enable');
  if (!send) await cdp('Network.setBlockedURLs', { urls: ['*supabase.co*'] });
  await cdp('Emulation.setDeviceMetricsOverride', { width: W, height: H, deviceScaleFactor: 2, mobile: true });
  await cdp('Emulation.setTouchEmulationEnabled', { enabled: true, maxTouchPoints: 5 });
  await cdp('Storage.clearDataForOrigin', { origin: new URL(base).origin, storageTypes: 'all' });
  page.open = async (url, { gate = false } = {}) => {
    await cdp('Page.navigate', { url });
    for (let i = 0; i < 120; i++) { await sleep(1000); if (await page.ev(`!!(window.LT && LT.end) || !!document.getElementById('rs-name')`)) break; }
    if (gate) { await sleep(1500); return; }
    for (let i = 0; i < 20; i++) {            // the debug load panel opens a moment after the load ends
      await sleep(500);
      if (await page.ev(`(function(){var b=document.getElementById('timing'); if (b && getComputedStyle(b).display!=='none') { b.click(); return true; } return false;})()`)) break;
    }
    await sleep(1500);
  };
  page.tap = async (x, y) => { await page.touch('touchStart', x, y); await sleep(60); await page.touch('touchEnd'); await sleep(150); };
  page.drag = async (x, y, dx, dy, ms) => {
    await page.touch('touchStart', x, y); await sleep(40);
    const n = Math.max(3, Math.round(ms / 16));
    for (let i = 1; i <= n; i++) { await page.touch('touchMove', x + dx * i / n, y + dy * i / n); await sleep(16); }
    await page.touch('touchEnd'); await sleep(900);
  };
  // play a stored solution (pieces dragged to the screen edge, as a player would)
  page.solve = async l => {
    const p = placement(l);
    for (const m of l.solution) {
      const c = l.pieces[m.piece][0], [ux, uy] = DIR[m.dir];
      const x0 = p.x + (c[0] + .5) * p.cell, y0 = p.y + (c[1] + .5) * p.cell;
      const room = ux < 0 ? x0 - 1 : ux > 0 ? W - 1 - x0 : uy < 0 ? y0 - 1 : H - 1 - y0;
      await page.drag(x0, y0, ux * room, uy * room, 160);
    }
  };
  page.name = async n => { await page.ev(`(function(){ var i=document.getElementById('rs-name-in'); i.value='${n}'; document.getElementById('rs-name-go').click(); return true; })()`); await sleep(1200); };
  return page;
}

/* --local: the folder is copied to a temp dir (serving the original would lock it, and WebBuild
   copies each new build into Builds/publish) and served on 127.0.0.1 like GitHub Pages: .br/.gz
   files go out as plain bytes, without Content-Encoding (the build decompresses them itself). */
async function serveCopy(src) {
  const dir = fs.mkdtempSync(path.join(os.tmpdir(), 'rs-build-'));
  fs.cpSync(src, dir, { recursive: true });
  const types = { '.html': 'text/html; charset=utf-8', '.js': 'application/javascript', '.css': 'text/css', '.json': 'application/json',
    '.png': 'image/png', '.jpg': 'image/jpeg', '.jpeg': 'image/jpeg', '.ico': 'image/x-icon', '.wasm': 'application/wasm' };
  const server = require('http').createServer((req, res) => {
    let f = path.join(dir, decodeURIComponent(new URL(req.url, 'http://x').pathname));
    if (!f.startsWith(dir)) { res.writeHead(403); return res.end(); }
    if (fs.existsSync(f) && fs.statSync(f).isDirectory()) f = path.join(f, 'index.html');
    if (!fs.existsSync(f)) { res.writeHead(404); return res.end(); }
    const ext = path.extname(f).toLowerCase();
    res.writeHead(200, { 'Content-Type': ext === '.br' || ext === '.gz' ? 'application/octet-stream' : types[ext] || 'application/octet-stream' });
    fs.createReadStream(f).pipe(res);
  });
  await new Promise(r => server.listen(0, '127.0.0.1', r));
  return { dir, base: `http://127.0.0.1:${server.address().port}`,
           close: () => { server.close(); fs.rmSync(dir, { recursive: true, force: true }); } };
}

const dbg = url => url + (url.includes('?') ? '&' : '?') + 'debug=1';
const rows = async () => (await (await fetch(`${SUPA}/rest/v1/plays?player=eq.${player}&select=*&order=created_at`, { headers: { apikey: KEY, Authorization: 'Bearer ' + KEY } })).json());

/* Types into the name gate the way a computer keyboard does. */
async function nameKeys() {
  const p = await newPage();
  if (send) await p.cdp('Network.setBlockedURLs', { urls: ['*supabase.co*'] });   // this check never sends
  await p.cdp('Emulation.setDeviceMetricsOverride', { width: 1280, height: 800, deviceScaleFactor: 1, mobile: false });
  await p.cdp('Emulation.setTouchEmulationEnabled', { enabled: false });
  await p.open(`${base}/`, { gate: true });
  const gate = await p.ev(`!!document.getElementById('rs-name')`);
  if (!gate) { check('namekeys', false, 'isim kapısı açılmadı'); p.close(); return; }
  await sleep(1500);                                       // the gate focuses its input after a moment; Unity is running
  const r = await p.ev(`(function(){var b=document.getElementById('rs-name-in').getBoundingClientRect();return {x:b.x+b.width/2,y:b.y+b.height/2};})()`);
  await p.mouse('mousePressed', r.x, r.y); await p.mouse('mouseReleased', r.x, r.y); await sleep(200);
  const name = 'TEST_KEYS';
  for (const ch of name) {
    const code = ch === '_' ? 'Minus' : 'Key' + ch.toUpperCase();
    const vk = ch === '_' ? 189 : ch.toUpperCase().charCodeAt(0);
    await p.cdp('Input.dispatchKeyEvent', { type: 'keyDown', key: ch, code, text: ch, unmodifiedText: ch, windowsVirtualKeyCode: vk });
    await p.cdp('Input.dispatchKeyEvent', { type: 'keyUp', key: ch, code, windowsVirtualKeyCode: vk });
    await sleep(40);
  }
  const typed = await p.ev(`(document.getElementById('rs-name-in')||{}).value`);
  await p.cdp('Input.dispatchKeyEvent', { type: 'keyDown', key: 'Enter', code: 'Enter', text: String.fromCharCode(13), windowsVirtualKeyCode: 13 });
  await p.cdp('Input.dispatchKeyEvent', { type: 'keyUp', key: 'Enter', code: 'Enter', windowsVirtualKeyCode: 13 });
  await sleep(1500);
  const gone = !(await p.ev(`!!document.getElementById('rs-name')`));
  check('namekeys', typed === name && gone && p.exceptions.length === 0,
        `klavyeyle yazılan "${typed ?? ''}" (beklenen "${name}"), Enter'dan sonra kapı ${gone ? 'kapandı' : 'açık kaldı'}, istisna ${p.exceptions.length}`);
  p.close();
}

async function boot() {
  const p = await newPage();
  await p.open(dbg(`${base}/?lv=1`));
  let b = ''; for (let i = 0; i < 20 && !/\d+\/\d+/.test(b); i++) { b = (await p.ev(`window.LT && LT.boot`)) || ''; await sleep(500); }
  const m = /(\d+)\s*\/\s*(\d+)/.exec(b);
  check('boot', !!m && m[1] === m[2] && +m[2] >= 40 && p.exceptions.length === 0, `açılış kontrolü "${b}", istisna ${p.exceptions.length}`);
  p.close();
}

async function k1() {
  const p = await newPage(); const styles = ['merkez', 'dış yarı', 'tahta dışı', 'fiske']; let k = 0, ok = 0; const fails = [];
  for (const lv of ['L01', 'L40']) {
    const l = levels.find(v => v.id === lv), pl = placement(l);
    await p.open(dbg(`${base}/?lv=${+lv.slice(1)}`));
    for (const [, x, y, d] of EDGES.filter(e => e[0] === lv)) {
      const [ux, uy] = DIR[d], style = styles[k++ % styles.length];
      const off = style === 'dış yarı' ? .3 * pl.cell : style === 'tahta dışı' ? .5 * pl.cell + 8 : 0;
      const cx = pl.x + (x + .5) * pl.cell + ux * off, cy = pl.y + (y + .5) * pl.cell + uy * off;
      const dist = style === 'fiske' ? 1.1 * pl.cell : 2.5 * pl.cell;
      await p.drag(cx, cy, ux * dist, uy * dist, style === 'fiske' ? 70 : 320);
      const after = await p.png({ x: 0, y: 56, width: W, height: 42 });
      fs.writeFileSync(path.join(out, `k1_${lv}_${x}${y}${d}.png`), after);
      // The debug line is drawn in the canvas; the piece counter is too. A piece that left changes the
      // board, so compare the board area before and after the pull instead of reading text.
      const gone = await pieceGone(p, pl, x, y);
      if (gone) ok++; else fails.push(`${lv} (${x},${y}) ${d} ${style}`);
    }
    fs.writeFileSync(path.join(out, `k1_${lv}_after.png`), await p.png());
  }
  const n = EDGES.length;
  check('k1', ok >= Math.ceil(n * .9), `${ok}/${n} tek seferde çıktı${fails.length ? '; çıkmayan: ' + fails.join(', ') : ''} (debug satırları: ${out}${path.sep}k1_*.png)`);
  p.close();
}

/* The cell is empty when its centre shows the board's dark well colour instead of a piece. */
async function pieceGone(p, pl, x, y) {
  await sleep(300);
  const img = readPng(await p.png());
  const px = Math.round((pl.x + (x + .5) * pl.cell) * 2), py = Math.round((pl.y + (y + .5) * pl.cell) * 2);
  let dark = 0, n = 0;
  for (let dy = -6; dy <= 6; dy += 3) for (let dx = -6; dx <= 6; dx += 3) { const [r, g, b] = img.at(px + dx, py + dy); n++; if (r + g + b < 120) dark++; }
  return dark / n > .8;
}

async function fps() {
  const res = {};
  for (const [name, url, card] of [['L40', `${base}/?lv=40`, false], ['D02', `${base}/?set=designs&lv=D02`, true]]) {
    const p = await newPage();
    await p.open(dbg(url));
    if (card) for (let y = Math.round(H * .45); y < H * .8; y += 18) await p.tap(W / 2, y);   // D02's intro card
    await p.ev(`window.__f=[];(function f(t){window.__f.push(t);requestAnimationFrame(f)})(performance.now())`);
    const cx = W / 2, cy = H / 2 - 20;
    await p.touch('touchStart', cx, cy); await sleep(40); await p.ev(`window.__f=[]`);
    const t0 = Date.now(); let i = 0;
    while (Date.now() - t0 < 10000) { const a = i++ / 20; await p.touch('touchMove', cx + 14 * Math.sin(a), cy + 10 * Math.cos(a)); await sleep(16); }
    const iv = await p.ev(`(function(){var f=window.__f,d=[];for(var i=1;i<f.length;i++)d.push(f[i]-f[i-1]);return d})()`);
    await p.touch('touchEnd');
    const s = iv.slice().sort((a, b) => a - b), f = v => +(1000 / v).toFixed(1);
    res[name] = { median: f(s[s.length >> 1]), p95: f(s[Math.floor(s.length * .95)]), worst: f(s[s.length - 1]), exceptions: p.exceptions.length };
    fs.writeFileSync(path.join(out, `fps_${name}.png`), await p.png());
    p.close();
  }
  const low = Object.entries(res).filter(([, v]) => v.median < 55 || v.exceptions);
  check('fps', low.length === 0, Object.entries(res).map(([k, v]) => `${k} medyan ${v.median} / p95 ${v.p95} / en düşük ${v.worst}`).join('; ') + ' (PC, gerçek GPU; iPhone değil)');
}

async function card() {
  const p = await newPage(), d01 = designs.find(v => v.id === 'D01');
  await p.open(dbg(`${base}/?set=designs&lv=D01`));
  await sleep(1500);                                   // the picture arrives after the board
  await p.solve(d01);
  await sleep(2500);
  const buf = await p.png(); fs.writeFileSync(path.join(out, 'card_D01_375x667.png'), buf);
  const img = readPng(buf), near = (c, t, e = 4) => c.every((v, i) => Math.abs(v - t[i]) <= e);
  const CARD = [20, 72, 76], ORANGE = [240, 167, 66], col = Math.round(img.w * .27);
  let bottom = -1; for (let y = img.h - 1; y >= 0; y--) if (near(img.at(col, y), CARD)) { bottom = y; break; }
  let o0 = -1, o1 = -1; for (let y = 0; y < img.h; y++) if (near(img.at(img.w >> 1, y), ORANGE, 10)) { if (o0 < 0) o0 = y; o1 = y; }
  // title vs frame: rows with light pixels across the picture's width; the frame is the first wide light band
  const light = y => { let n = 0; for (let x = Math.round(img.w * .2); x < img.w * .8; x++) { const [r, g, b] = img.at(x, y); if (r > 190 && g > 190 && b > 190) n++; } return n; };
  let frame = -1; for (let y = 0; y < img.h * .6; y++) if (light(y) > img.w * .4) { frame = y; break; }
  let title = frame - 1; while (title > 0 && light(title) === 0) title--;
  const pt = v => +(v / 2).toFixed(1), btn = o0 < 0 ? 0 : pt(o1 - o0 + 1), gap = frame < 0 ? -1 : pt(frame - title - 1);
  const ok = bottom > 0 && pt(bottom) <= H && btn >= 44 && gap >= 8 && p.exceptions.length === 0;
  check('card', ok, `kart altı ${pt(bottom)} pt (≤ ${H}), ana düğme ${btn} pt (≥ 44), başlık–çerçeve ${gap} pt (≥ 8), istisna ${p.exceptions.length}` +
    (frame < 0 ? ' — resim çerçevesi bulunamadı: kart "Resmi tamamladın" değil mi?' : ''));
  p.close();
}

async function zeydbot() {
  const file = path.join(ROOT, 'Assets/_Game/Levels/zeydbot.json');
  const set = fs.existsSync(file) ? JSON.parse(fs.readFileSync(file, 'utf8')).levels : [];
  if (!set.length) { check('zeydbot', true, 'zeydbot.json yok: atlandı'); return; }
  const l = set[0], p = await newPage();
  await p.open(dbg(`${base}/?set=zeydbot&lv=${l.id}`));
  for (let y = Math.round(H * .45); y < H * .8; y += 18) await p.tap(W / 2, y);
  await p.solve(l);
  await sleep(2500);
  const buf = await p.png(); fs.writeFileSync(path.join(out, `zeydbot_${l.id}.png`), buf);
  const img = readPng(buf);
  let orange = 0; for (let y = 0; y < img.h; y++) { const [r, g, b] = img.at(img.w >> 1, y); if (Math.abs(r - 240) < 12 && Math.abs(g - 167) < 12 && Math.abs(b - 66) < 12) orange++; }
  check('zeydbot', orange > 60 && p.exceptions.length === 0, `${l.id} (${set.length} seviyeli set) kayıtlı çözümle oynandı: sonuç kartı ${orange > 60 ? 'var' : 'yok'}, istisna ${p.exceptions.length}`);
  p.close();
}

async function redesign() {
  const file = path.join(ROOT, 'Assets/_Game/Levels/redesign.json');
  const set = fs.existsSync(file) ? JSON.parse(fs.readFileSync(file, 'utf8')).levels : [];
  if (!set.length) { check('redesign', true, 'redesign.json boş: atlandı'); return; }
  const l = set[0], p = await newPage();
  await p.open(dbg(`${base}/?set=redesign&lv=${l.id}`));
  for (let y = Math.round(H * .45); y < H * .8; y += 18) await p.tap(W / 2, y);   // an intro card, if any
  await p.solve(l);
  await sleep(2500);
  const buf = await p.png(); fs.writeFileSync(path.join(out, `redesign_${l.id}.png`), buf);
  const img = readPng(buf);
  let orange = 0; for (let y = 0; y < img.h; y++) { const [r, g, b] = img.at(img.w >> 1, y); if (Math.abs(r - 240) < 12 && Math.abs(g - 167) < 12 && Math.abs(b - 66) < 12) orange++; }
  check('redesign', orange > 60 && p.exceptions.length === 0, `${l.id} (${set.length} seviyeli set) kayıtlı çözümle oynandı: sonuç kartı ${orange > 60 ? 'var' : 'yok'}, istisna ${p.exceptions.length}`);
  p.close();
}

async function a6b() {
  const p = await newPage(), l02 = levels[1];
  const before = await rows();
  await p.open(`${base}/?lv=2`, { gate: true });
  const gate = await p.ev(`!!document.getElementById('rs-name')`);
  await sleep(20000);
  await p.name(player);
  const t0 = Date.now(); await p.solve(l02); const wall = (Date.now() - t0) / 1000;
  await sleep(17000);                                  // a flush runs every 15 s
  const fresh = (await rows()).filter(r => !before.some(b => b.row_id === r.row_id));
  const r = fresh.find(v => v.level_id === 'L02');
  const fun = r && r.keyif != null;                    // AskFun is off: no 👍/👎 value in the row
  const ok = gate && fresh.length === 1 && r && r.result === 'win' && r.attempt === 1 && Math.abs(r.dur - wall) <= 4 && !fun;
  check('a6b', ok, `kapı ${gate}, yeni satır ${fresh.length}` + (r ? `, L02 ${r.result} deneme ${r.attempt}, dur ${r.dur} sn / isimden sonra ${wall.toFixed(0)} sn (kapıdaki 20 sn sayılmamalı), keyif ${fun ? r.keyif : 'yok'}` : ''));
  return p;                                             // m5 continues in the same profile
}

async function m5(p) {
  const l01 = levels[0], l02 = levels[1];
  const r0 = await rows();
  await p.open(`${base}/`, { gate: true });
  const gateAgain = await p.ev(`!!document.getElementById('rs-name')`);
  await p.open(`${base}/?lv=2`, { gate: true });
  await sleep(1500);
  const pl = placement(l02), m = l02.solution[0], c = l02.pieces[m.piece][0], [ux, uy] = DIR[m.dir];
  await p.drag(pl.x + (c[0] + .5) * pl.cell, pl.y + (c[1] + .5) * pl.cell, ux * pl.cell * .2, uy * pl.cell * .2, 200);
  await p.ev(`(function(){ Object.defineProperty(document,'hidden',{configurable:true,get:()=>true}); Object.defineProperty(document,'visibilityState',{configurable:true,get:()=>'hidden'}); document.dispatchEvent(new Event('visibilitychange')); return true; })()`);
  await sleep(2000);
  await p.open(`${base}/`, { gate: true });            // a new visit sends what was left
  await sleep(17000);
  const r1 = await rows(), quit = r1.filter(r => !r0.some(b => b.row_id === r.row_id));
  await p.open(dbg(`${base}/?lv=1`));
  await p.solve(l01);
  await sleep(17000);
  const r2 = await rows();
  const ok = !gateAgain && quit.length === 1 && quit[0].result === 'quit' && r2.length === r1.length && new Set(r2.map(r => r.row_id)).size === r2.length;
  check('m5', ok, `yenilemede isim ${gateAgain ? 'yeniden soruldu' : 'kaldı'}, bırakma satırı ${quit.length} (${quit.map(q => q.level_id + ' ' + q.result).join(', ')}), debug kazancı ${r2.length - r1.length} satır, row_id tekil ${new Set(r2.map(r => r.row_id)).size === r2.length}`);
  p.close();
}

(async () => {
  console.log(`Build doğrulama: ${local ? "yerel " + local : base}  (${send ? 'GÖNDERİM AÇIK, oyuncu ' + player : 'gönderim yok'})  görüntüler: ${out}`);
  const server = local ? await serveCopy(local) : null;
  if (server) { base = server.base; console.log(`yerel kopya ${server.dir} → ${base}`); }
  const chrome = await startChrome();
  try {
    if (only.includes('boot')) await boot();
    if (only.includes('namekeys')) await nameKeys();
    if (only.includes('k1')) await k1();
    if (only.includes('fps')) await fps();
    if (only.includes('card')) await card();
    if (only.includes('redesign')) await redesign();
    if (only.includes('zeydbot')) await zeydbot();
    if (send && only.includes('a6b')) { const p = await a6b(); if (only.includes('m5')) await m5(p); else p.close(); }
  } catch (e) { check('çalıştırma', false, e.stack || String(e)); }
  finally {
    chrome.kill(); if (server) server.close();
    await sleep(1500); try { fs.rmSync(chrome.profile, { recursive: true, force: true }); } catch { }   // throwaway profile
  }
  const failed = report.filter(r => !r.pass);
  console.log(`\nÖzet: ${report.length - failed.length}/${report.length} geçti${failed.length ? ' — kalan: ' + failed.map(f => f.name).join(', ') : ''}`);
  fs.writeFileSync(path.join(out, 'report.json'), JSON.stringify({ base, send, player: send ? player : null, at: new Date().toISOString(), report }, null, 1));
  process.exit(failed.length ? 1 : 0);
})();
