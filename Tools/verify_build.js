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
const only = opt('only', 'boot,namekeys,k1,fps,card,redesign,zeydbot' + (send ? ',a6b,m5,zeydsend' : '')).split(',');
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
let W = 375, H = 667;   // let: the qa check runs other phone sizes
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
  let id = 0; const pend = new Map(); const page = { exceptions: [], supabase: [], logs: [] };
  ws.addEventListener('message', e => {
    const m = JSON.parse(e.data);
    if (m.id && pend.has(m.id)) { pend.get(m.id)(m); pend.delete(m.id); }
    if (m.method === 'Runtime.exceptionThrown') page.exceptions.push(m.params.exceptionDetails.exception?.description || m.params.exceptionDetails.text);
    if (m.method === 'Runtime.consoleAPICalled') page.logs.push((m.params.args || []).map(a => a.value).join(' '));
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
  await p.open(dbg(`${base}/?set=classic&lv=1`));
  let b = ''; for (let i = 0; i < 20 && !/\d+\/\d+/.test(b); i++) { b = (await p.ev(`window.LT && LT.boot`)) || ''; await sleep(500); }
  const m = /(\d+)\s*\/\s*(\d+)/.exec(b);
  check('boot', !!m && m[1] === m[2] && +m[2] >= 40 && p.exceptions.length === 0, `açılış kontrolü "${b}", istisna ${p.exceptions.length}`);
  p.close();
}

async function k1() {
  const p = await newPage(); const styles = ['merkez', 'dış yarı', 'tahta dışı', 'fiske']; let k = 0, ok = 0; const fails = [];
  for (const lv of ['L01', 'L40']) {
    const l = levels.find(v => v.id === lv), pl = placement(l);
    await p.open(dbg(`${base}/?set=classic&lv=${+lv.slice(1)}`));
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
  for (const [name, url, card] of [['L40', `${base}/?set=classic&lv=40`, false], ['D02', `${base}/?set=designs&lv=D02`, true]]) {
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
  for (let y = Math.round(H * .45); y < H * .8; y += 18) await p.tap(W / 2, y);   // PM_MESAJ_29: slot 1 start card
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

/* ZeydBot: the 1-10 question also on a loss card. Z03 (debug, nothing sent): one move starts the clock,
   then the timer runs out; the card must show the question (screenshot zeydloss_Z03.png). */
async function zeydloss() {
  const file = path.join(ROOT, 'Assets/_Game/Levels/zeydbot.json');
  const set = JSON.parse(fs.readFileSync(file, 'utf8')).levels;
  const l = set.find(x => x.id === 'Z03'), p = await newPage();
  await p.open(dbg(`${base}/?set=zeydbot&lv=Z03`));
  for (let y = Math.round(H * .45); y < H * .8; y += 18) await p.tap(W / 2, y);
  await p.solve({ ...l, solution: l.solution.slice(0, 1) });
  await sleep((l.timer || 65) * 1000 + 6000);
  const buf = await p.png(); fs.writeFileSync(path.join(out, 'zeydloss_Z03.png'), buf);
  check('zeydloss', p.exceptions.length === 0, `Z03 süre bitti, kart ekran görüntüsü ${path.join(out, 'zeydloss_Z03.png')}, istisna ${p.exceptions.length}`);
  p.close();
}

/* --send only: the 1-10 answer after a LOSS reaches the plays row (PM 2026-10-21: Z34's bomb row had none).
   Z03 as the TEST player: one move, the timer runs out, the first 1-10 button on the loss card is tapped. */
async function zeydlosssend() {
  const file = path.join(ROOT, 'Assets/_Game/Levels/zeydbot.json');
  const l = JSON.parse(fs.readFileSync(file, 'utf8')).levels.find(x => x.id === 'Z03'), p = await newPage();
  const before = await rows();
  await p.open(`${base}/?set=zeydbot&lv=Z03`, { gate: true });
  await p.name(player);
  await sleep(1500);
  await p.solve({ ...l, solution: l.solution.slice(0, 1) });
  await sleep((l.timer || 65) * 1000 + 5000);
  const x = W / 2 - 120;
  let prev = await p.png();
  for (let y = Math.round(H * .25); y < H * .85; y += 12) {
    await p.tap(x, y);
    const now = await p.png();
    if (Buffer.compare(now, prev) !== 0) break;
    prev = now;
  }
  fs.writeFileSync(path.join(out, 'zeydlosssend_Z03.png'), await p.png());
  await sleep(17000);
  const r = (await rows()).filter(r => !before.some(b => b.row_id === r.row_id) && r.level_id === 'Z03')[0];
  check('zeydlosssend', !!r && r.result === 'time' && r.zorluk_puani != null, r ? `Z03 ${r.result}, zorluk_puani ${r.zorluk_puani}` : 'Z03 satırı gelmedi');
  p.close();
}

/* --send only: SET40 under ?set=redesign is recorded (PM_MESAJ_5): S01 without debug as the TEST player, the stored
   solution, the first 1-10 button; the plays row must carry zorluk_puani and the moves table the exits. */
async function redesignsend() {
  const l = JSON.parse(fs.readFileSync(path.join(ROOT, 'Assets/_Game/Levels/redesign.json'), 'utf8')).levels[0], p = await newPage();
  const before = await rows();
  await p.open(`${base}/?set=redesign&lv=${l.id}`, { gate: true });
  await p.name(player);
  await sleep(1500);
  for (let y = Math.round(H * .45); y < H * .8; y += 18) await p.tap(W / 2, y);   // an intro card, if any
  await p.solve(l);
  await sleep(2500);
  const x = W / 2 - 120;
  // Build 5: the card asks 👍/👎 first (the left button is "Beğendim"), then the 1-10 row; one tap per question
  for (let q = 0; q < 2; q++) {
    await sleep(3000);                                  // the win confetti and star pop change the picture on their own
    let prev = await p.png();
    for (let y = Math.round(H * .25); y < H * .85; y += 12) {
      await p.tap(x, y);
      const now = await p.png();
      if (Buffer.compare(now, prev) !== 0) break;
      prev = now;
    }
    await sleep(1200);
  }
  await sleep(17000);
  const r = (await rows()).filter(r => !before.some(b => b.row_id === r.row_id) && r.level_id === l.id)[0];
  let mv = [];
  if (r) mv = await (await fetch(`${SUPA}/rest/v1/moves?row_id=eq.${r.row_id}&select=kind`, { headers: { apikey: KEY, Authorization: 'Bearer ' + KEY } })).json();
  const exits = mv.filter(m => m.kind === 'exit').length;
  check('redesignsend', !!r && r.result === 'win' && r.zorluk_puani != null && exits === l.solution.length, r ? `${l.id} ${r.result}, keyif ${r.keyif}, zorluk_puani ${r.zorluk_puani}, moves exit ${exits}/${l.solution.length}` : `${l.id} satırı gelmedi`);
  p.close();
}

async function redesign() {
  const file = path.join(ROOT, 'Assets/_Game/Levels/redesign.json');
  const set = fs.existsSync(file) ? JSON.parse(fs.readFileSync(file, 'utf8')).levels : [];
  if (!set.length) { check('redesign', true, 'redesign.json boş: atlandı'); return; }
  const rlv = opt('rlv', '');   // --rlv S47,S18: play these instead of the first level, card shot per level
  if (rlv) {
    for (const id of rlv.split(',')) {
      const l = set.find(x => x.id === id), q = await newPage();
      await q.open(dbg(`${base}/?set=redesign&lv=${id}`));
      for (let y = Math.round(H * .45); y < H * .8; y += 18) await q.tap(W / 2, y);
      await q.solve(l); await sleep(2500);
      const img = readPng(await q.png()); fs.writeFileSync(path.join(out, `rlv_${id}.png`), await q.png());
      let card = 0; for (let y = 0; y < img.h; y++) { const [r, g, b] = img.at(img.w >> 1, y); if ((Math.abs(r - 20) < 6 && Math.abs(g - 72) < 6 && Math.abs(b - 76) < 6) || (r > 238 && g > 228 && b > 200 && b < 240)) card++; }
      check('redesign ' + id, card > 60 && q.exceptions.length === 0, `${id} lv ${l.lv} v${l.version} kayıtlı çözümle oynandı: sonuç kartı ${card > 60 ? 'var' : 'yok'}, istisna ${q.exceptions.length}`);
      q.close();
    }
    return;
  }
  const l = set[0], p = await newPage();
  await p.open(dbg(`${base}/?set=redesign&lv=${l.id}`));
  for (let y = Math.round(H * .45); y < H * .8; y += 18) await p.tap(W / 2, y);   // an intro card, if any
  if (l.tutorial === 1) {                              // PM_MESAJ_33 tutorial: guided move, guided jam (2nd piece to the right), the rest
    await p.solve({ ...l, solution: l.solution.slice(0, 1) }); await sleep(900);
    await p.solve({ ...l, solution: [{ piece: l.solution[1].piece, dir: 'R' }] }); await sleep(2800);
    await p.solve({ ...l, solution: l.solution.slice(1) });
  } else await p.solve(l);
  await sleep(2500);
  const buf = await p.png(); fs.writeFileSync(path.join(out, `redesign_${l.id}.png`), buf);
  const img = readPng(buf);
  let orange = 0; for (let y = 0; y < img.h; y++) { const [r, g, b] = img.at(img.w >> 1, y); if ((Math.abs(r - 20) < 6 && Math.abs(g - 72) < 6 && Math.abs(b - 76) < 6) || (r > 238 && g > 228 && b > 200 && b < 240)) orange++; }   // or Codex's cream card   // the result card's panel (#14484c): the preview art's button is not orange
  // phone-size screenshots of the preview art (intro card, board with obstacles, hard warning) for a look
  for (const id of ['', 'S43', 'S45', 'S28', 'S40']) {
    const q = await newPage(); await q.open(dbg(`${base}/?set=redesign${id ? '&lv=' + id : ''}`)); await sleep(2500);
    fs.writeFileSync(path.join(out, `art_${id}_card.png`), await q.png());
    for (let y = Math.round(H * .45); y < H * .85; y += 18) await q.tap(W / 2, y);
    await sleep(800); fs.writeFileSync(path.join(out, `art_${id}_board.png`), await q.png()); q.close();
  }
  check('redesign', orange > 60 && p.exceptions.length === 0, `${l.id} (${set.length} seviyeli set) kayıtlı çözümle oynandı: sonuç kartı ${orange > 60 ? 'var' : 'yok'}, istisna ${p.exceptions.length}`);
  p.close();
}

/* --send only: a ZeydBot attempt end to end on Z03 (no debug, sends as the TEST player): one touch that
   snaps back, a pause / resume, the stored solution, the first 1-10 button; then moves must hold a touch
   row with hold_ms, start + hide + show rows (PM_MESAJ_25), and the plays row a zorluk_puani. */
async function zeydsend() {
  const file = path.join(ROOT, 'Assets/_Game/Levels/zeydbot.json');
  const set = JSON.parse(fs.readFileSync(file, 'utf8')).levels;
  const l = set.find(x => x.id === 'Z03'), p = await newPage();
  const before = await rows();
  await p.open(`${base}/?set=zeydbot&lv=Z03`, { gate: true });
  await p.name(player);
  await sleep(1500);
  const pl = placement(l), m = l.solution[0], c = l.pieces[m.piece][0], [ux, uy] = DIR[m.dir];
  await p.drag(pl.x + (c[0] + .5) * pl.cell, pl.y + (c[1] + .5) * pl.cell, ux * pl.cell * .2, uy * pl.cell * .2, 300);   // a touch
  const vis = h => p.ev(`(function(){ Object.defineProperty(document,'hidden',{configurable:true,get:()=>${h}}); Object.defineProperty(document,'visibilityState',{configurable:true,get:()=>'${h ? 'hidden' : 'visible'}'}); document.dispatchEvent(new Event('visibilitychange')); return true; })()`);
  await vis(true); await sleep(1500); await vis(false); await sleep(1500);
  await p.solve(l);
  await sleep(2500);
  // the 1-10 rows sit above the next / retry buttons: the first button met going down the left column is "1"
  const x = W / 2 - 120;
  let before1 = await p.png();
  for (let y = Math.round(H * .25); y < H * .85; y += 12) {
    await p.tap(x, y);
    const now = await p.png();
    if (Buffer.compare(now, before1) !== 0) break;      // the card changed: the answer was taken
    before1 = now;
  }
  await sleep(17000);                                   // flushes run every 15 s
  const fresh = (await rows()).filter(r => !before.some(b => b.row_id === r.row_id) && r.level_id === 'Z03');
  const r = fresh[0];
  let mv = [];
  if (r) mv = await (await fetch(`${SUPA}/rest/v1/moves?row_id=eq.${r.row_id}&select=kind,hold_ms,move_no&order=move_no`, { headers: { apikey: KEY, Authorization: 'Bearer ' + KEY } })).json();
  const kinds = k => mv.filter(x => x.kind === k).length;
  const touchOk = mv.some(x => x.kind === 'touch' && x.hold_ms > 0);
  const ok = !!r && r.result === 'win' && r.zorluk_puani != null && touchOk && kinds('hide') >= 1 && kinds('show') >= 1 && kinds('start') === 1 && kinds('exit') === l.solution.length;   // PM_MESAJ_25: hide/show, start
  check('zeydsend', ok, r ? `Z03 ${r.result}, zorluk_puani ${r.zorluk_puani}, moves: exit ${kinds('exit')}/${l.solution.length}, touch ${kinds('touch')} (hold_ms ${(mv.find(x => x.kind === 'touch') || {}).hold_ms}), start ${kinds('start')}, hide ${kinds('hide')}, show ${kinds('show')}` : 'Z03 satırı gelmedi');
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

/* --send --only cohortic (PM_MESAJ_25 step 8): one browser profile as IC_KONTROL (not TEST_: v_attempts leaves those
   out), ?c=ic only on the first link. S41 won (+ profile question, 1-10), S50 lost to the bomb (Logs/veri/bomb_loss.json,
   SetV4.BombLoss), S42 one move then pause -> menu (quit), S43 two moves then the tab goes away (abandoned). */
/* --send --only ic33 (PM_MESAJ_32/33): a fresh ic profile plays level 1 (with the guided jam), 10 and 14 (unlock=1)
   for real; Supabase: plays level_version 2 for Z24/Z21, jams 0 for S41 and no jam row in its moves. */
async function ic33() {
  const set = JSON.parse(fs.readFileSync(path.join(ROOT, 'Assets/_Game/Levels/redesign.json'), 'utf8')).levels;
  const lv = id => set.find(x => x.id === id), name = opt('icname', 'IC_PM33'), p = await newPage();
  const light = async () => { const img = readPng(await p.png()); let s = 0, n = 0;
    for (let y = .3; y < .8; y += .05) for (let x = .2; x < .8; x += .05) { s += img.at(Math.round(img.w * x), Math.round(img.h * y)).slice(0, 3).reduce((a, b) => a + b, 0); n++; } return s / n; };
  const close = async () => { await sleep(3000); const open = await light();   // a cream card (profile, intro, 1-10): tap down until it goes
    for (let y = Math.round(H * .4); y < H * .9; y += 14) { await p.tap(W / 2, y); await sleep(350); if (await light() < open * .8) break; } await sleep(1000); };
  const l1 = lv('S41');
  await p.open(`${base}/?set=redesign&c=ic&unlock=1&lv=S41`, { gate: true });
  await p.name(name); await sleep(2000); await close();
  await p.solve({ ...l1, solution: l1.solution.slice(0, 1) }); await sleep(900);
  await p.solve({ ...l1, solution: [{ piece: l1.solution[1].piece, dir: 'R' }] }); await sleep(2800);
  await p.solve({ ...l1, solution: l1.solution.slice(1) }); await sleep(2500); await close();
  for (const id of ['Z24', 'Z21']) {
    await p.open(`${base}/?set=redesign&c=ic&unlock=1&lv=${id}`, { gate: true }); await sleep(3000); await close();
    await p.solve(lv(id)); await sleep(2500); await close();
  }
  await sleep(17000); await p.cdp('Page.navigate', { url: 'about:blank' }); await sleep(4000); p.close();
  const get = async q => (await (await fetch(`${SUPA}/rest/v1/${q}`, { headers: { apikey: KEY, Authorization: 'Bearer ' + KEY } })).json());
  const pl = await get(`plays?player=eq.${name}&select=level_id,level_version,result,jams,cohort&order=created_at`);
  const mv = await get(`moves?player=eq.${name}&level_id=eq.S41&select=kind&order=created_at`);
  console.log('PLAYS ' + JSON.stringify(pl));
  const v = id => pl.find(r => r.level_id === id), s41 = v('S41');
  const ok = ['Z24', 'Z21'].every(id => v(id) && v(id).level_version === 2) && s41 && s41.jams === 0 && !mv.some(m => m.kind === 'jam');
  check('ic33', ok, `plays ${pl.map(r => `${r.level_id} v${r.level_version} ${r.result} jams ${r.jams}`).join('; ')}; S41 moves jam satırı ${mv.filter(m => m.kind === 'jam').length}`);
}

async function cohortic() {
  const set = JSON.parse(fs.readFileSync(path.join(ROOT, 'Assets/_Game/Levels/redesign.json'), 'utf8')).levels;
  const lv = id => set.find(x => x.id === id), name = opt('icname', 'IC_KONTROL'), p = await newPage();
  const bomb = JSON.parse(fs.readFileSync(path.join(ROOT, 'Logs/veri/bomb_loss.json'), 'utf8'));
  const scan = async () => {                        // one tap on the first thing that changes the picture
    await sleep(3000); let prev = await p.png();
    for (let y = Math.round(H * .25); y < H * .9; y += 12) { await p.tap(W / 2, y); const now = await p.png(); if (Buffer.compare(now, prev) !== 0) break; prev = now; }
    await sleep(1200);
  };
  const intro = async () => { for (let y = Math.round(H * .45); y < H * .8; y += 18) await p.tap(W / 2, y); };
  await p.open(`${base}/?set=redesign&c=ic&lv=S41`, { gate: true });
  await p.name(name); await sleep(1500);
  await scan();                                     // profile: the first button down the card
  await intro(); await p.solve(lv('S41')); await sleep(2500);
  await scan();                                     // 1-10
  await sleep(16000);
  await p.open(`${base}/?set=redesign&lv=${bomb.id}`, { gate: true }); await sleep(4000);   // no c=: the cohort stays
  await intro(); await p.solve({ ...lv(bomb.id), solution: bomb.solution }); await sleep(2500);
  await scan();
  await sleep(16000);
  await p.open(`${base}/?set=redesign&lv=S42`, { gate: true }); await sleep(4000);
  await intro(); await p.solve({ ...lv('S42'), solution: lv('S42').solution.slice(0, 1) }); await sleep(800);
  await p.tap(W - 36, 33); await sleep(800);
  for (let y = Math.round(H * .85); y > H * .4; y -= 14) await p.tap(W / 2, y);
  await sleep(16000);
  await p.open(`${base}/?set=redesign&lv=S43`, { gate: true }); await sleep(4000);
  await intro(); await p.solve({ ...lv('S43'), solution: lv('S43').solution.slice(0, 2) });
  await sleep(17000);                               // the 15 s send
  await p.cdp('Page.navigate', { url: 'about:blank' }); await sleep(4000);
  p.close();
  const get = async q => (await (await fetch(`${SUPA}/rest/v1/${q}`, { headers: { apikey: KEY, Authorization: 'Bearer ' + KEY } })).json());
  const pl = await get(`plays?player=eq.${name}&select=level_id,result,attempt,cohort,platform,device_id,app_version,zorluk_puani,row_id&order=created_at`);
  const mv = await get(`moves?player=eq.${name}&select=row_id,level_id,move_no,kind,dir,cohort,device_id&order=created_at`);
  console.log('PLAYS ' + JSON.stringify(pl));
  console.log('MOVES ' + JSON.stringify(mv.map(m => `${m.level_id}#${m.move_no}:${m.kind}${m.dir ? '/' + m.dir : ''}:${m.cohort}`)));
  const kinds = new Set(mv.map(m => m.kind));
  const ok = pl.some(r => r.result === 'win') && pl.some(r => r.result === 'bomb') && pl.some(r => r.result === 'quit') &&
             pl.every(r => r.cohort === 'ic' && r.platform && r.device_id && r.app_version) && ['start', 'quit', 'exit'].every(k => kinds.has(k)) &&
             mv.some(m => m.level_id === 'S43') && !pl.some(r => r.level_id === 'S43');
  check('cohortic', ok, `plays ${pl.map(r => r.level_id + ':' + r.result + (r.zorluk_puani != null ? '/' + r.zorluk_puani : '')).join(' ')}; moves kinds ${[...kinds].join(',')}; S43 yarıda: ${mv.filter(m => m.level_id === 'S43').length} hamle satırı`);
}

/* --send only (PM_MESAJ_14): pause -> "Ana menü" on S45 after one move ends the attempt as a quit row. */
async function pausequit() {
  const l = JSON.parse(fs.readFileSync(path.join(ROOT, 'Assets/_Game/Levels/redesign.json'), 'utf8')).levels.find(x => x.id === 'S45'), p = await newPage();
  const before = await rows();
  await p.open(`${base}/?set=redesign&lv=${l.id}`, { gate: true });
  await p.name(player);
  await sleep(1500);
  for (let y = Math.round(H * .45); y < H * .9; y += 18) await p.tap(W / 2, y);   // the intro / start card
  await p.solve({ ...l, solution: l.solution.slice(0, 1) });
  await sleep(800);
  await p.tap(W - 36, 33); await sleep(800);                                     // pause
  for (let y = Math.round(H * .85); y > H * .4; y -= 14) { await p.tap(W / 2, y); }   // lowest card button first: "Ana menü"
  await sleep(17000);
  const r = (await rows()).filter(r => !before.some(b => b.row_id === r.row_id) && r.level_id === l.id)[0];
  check('pausequit', !!r && r.result === 'quit', r ? `${l.id} ${r.result}` : `${l.id} satırı gelmedi`);
  p.close();
}

/* --only funcard: the Build 5 win card before and after the 👍 tap (no debug, nothing sent: Supabase is blocked). */
async function funcard() {
  const l = JSON.parse(fs.readFileSync(path.join(ROOT, 'Assets/_Game/Levels/redesign.json'), 'utf8')).levels[0], p = await newPage();
  await p.open(`${base}/?set=redesign&lv=${l.id}`, { gate: true }); await p.name('TEST_QA'); await sleep(1500);
  for (let y = Math.round(H * .45); y < H * .8; y += 18) await p.tap(W / 2, y);
  await p.solve(l); await sleep(2500);
  fs.writeFileSync(path.join(out, 'fun_1.png'), await p.png());
  const x = W / 2 - 120; let prev = await p.png();
  for (let y = Math.round(H * .25); y < H * .85; y += 12) { await p.tap(x, y); const now = await p.png(); if (Buffer.compare(now, prev) !== 0) break; prev = now; }
  await sleep(1200); fs.writeFileSync(path.join(out, 'fun_2.png'), await p.png());
  check('funcard', p.exceptions.length === 0, out); p.close();
}

/* --only extra (PM_MESAJ_16): a chained piece mid-drag on S40 (locks must stay on it) and the avatar picker. */
async function extra() {
  const dir = opt('qa-out', path.join(ROOT, 'docs/qa', new Date().toISOString().slice(0, 10)));
  fs.mkdirSync(dir, { recursive: true });
  const set = JSON.parse(fs.readFileSync(path.join(ROOT, 'Assets/_Game/Levels/redesign.json'), 'utf8')).levels;
  const l = set.find(x => x.id === 'S40'), i = l.pieces.findIndex(c => c.length > 1), c = l.pieces[i][0];
  let p = await newPage(); await p.open(dbg(`${base}/?set=redesign&lv=S40`)); await sleep(2500);
  for (let y = Math.round(H * .45); y < H * .9; y += 18) await p.tap(W / 2, y);
  await sleep(600);
  const pl = placement(l), x0 = pl.x + (c[0] + .5) * pl.cell, y0 = pl.y + (c[1] + .5) * pl.cell;
  await p.touch('touchStart', x0, y0);
  for (let k = 1; k <= 8; k++) { await p.touch('touchMove', x0 + k * 3, y0 - k * 2); await sleep(16); }
  await sleep(300); fs.writeFileSync(path.join(dir, 'drag_mid.png'), await p.png());
  await p.touch('touchEnd', x0 + 24, y0 - 16); await sleep(900); fs.writeFileSync(path.join(dir, 'drag_after.png'), await p.png());
  const exc1 = p.exceptions.length; p.close();
  p = await newPage(); await p.open(dbg(`${base}/?set=redesign`)); await sleep(2500);
  await p.tap(40, 34); await sleep(700); fs.writeFileSync(path.join(dir, 'avatar_card.png'), await p.png());
  const exc2 = p.exceptions.length; p.close();
  // PM_MESAJ_16 B: the painted-board test on level 7
  p = await newPage(); await p.open(dbg(`${base}/?set=redesign&painting=1&lv=S46`)); await sleep(2500);
  for (let y = Math.round(H * .45); y < H * .9; y += 18) await p.tap(W / 2, y);
  await sleep(800); fs.writeFileSync(path.join(dir, 'painting_l7.png'), await p.png());
  check('extra', exc1 + exc2 + p.exceptions.length === 0, dir); p.close();
}

/* --only painting: the painted level 7 (?painting=1) at 3 phone sizes, resting and with a piece mid-drag. */
async function painting() {
  const dir = opt('qa-out', path.join(ROOT, 'docs/qa', new Date().toISOString().slice(0, 10)));
  fs.mkdirSync(dir, { recursive: true });
  const l = JSON.parse(fs.readFileSync(path.join(ROOT, 'Assets/_Game/Levels/redesign.json'), 'utf8')).levels.find(x => x.lv === 7);
  let exc = 0;
  const sizes = (opt('sizes', '375x667,390x844,430x932')).split(',').map(s => s.split('x').map(Number));
  for (const [w, h] of sizes) {
    W = w; H = h;
    const p = await newPage(); await p.open(dbg(`${base}/?set=redesign&painting=1${opt('edge', '') ? '&edge=' + opt('edge', '') : ''}&lv=${l.id}`)); await sleep(2500);
    for (let y = Math.round(H * .45); y < H * .9; y += 18) await p.tap(W / 2, y);
    await sleep(800); fs.writeFileSync(path.join(dir, `painting_${opt('edge', 'base')}_${w}x${h}.png`), await p.png());
    const pl = placement(l), c = l.pieces[l.solution[0].piece][0], x0 = pl.x + (c[0] + .5) * pl.cell, y0 = pl.y + (c[1] + .5) * pl.cell;
    await p.touch('touchStart', x0, y0);
    for (let k = 1; k <= 6; k++) { await p.touch('touchMove', x0 + k * 2, y0 + k * 2); await sleep(16); }
    await sleep(300); fs.writeFileSync(path.join(dir, `painting_drag_${w}x${h}.png`), await p.png());
    await p.touch('touchEnd', x0 + 12, y0 + 12); exc += p.exceptions.length; p.close();
  }
  W = 375; H = 667;
  check('painting', exc === 0, dir);
}

/* --only show (PM_MESAJ_17): the investor link as a visitor sees it (no debug, nothing sent): hint card, painted
   board, a win with the stored solution, at 3 phone sizes. */
async function show() {
  const dir = opt('qa-out', path.join(ROOT, 'docs/qa', new Date().toISOString().slice(0, 10)));
  fs.mkdirSync(dir, { recursive: true });
  const l = JSON.parse(fs.readFileSync(path.join(ROOT, 'Assets/_Game/Presentation/Resources/show.json'), 'utf8')).levels[0];
  let exc = 0, won = 0;
  for (const [w, h] of [[375, 667], [390, 844], [430, 932]]) {
    W = w; H = h;
    const p = await newPage(); await p.open(`${base}/?set=redesign&painting=1&lv=SHOW1`, { gate: true }); await sleep(3000);
    fs.writeFileSync(path.join(dir, `show_${w}x${h}_0_menu.png`), await p.png());
    await p.tap(W / 2, H - 70); await sleep(1500);                                  // the menu's Demo button
    fs.writeFileSync(path.join(dir, `show_${w}x${h}_1_kart.png`), await p.png());
    for (let y = Math.round(H * .45); y < H * .9; y += 18) await p.tap(W / 2, y);
    await sleep(800); fs.writeFileSync(path.join(dir, `show_${w}x${h}_2_tahta.png`), await p.png());
    await p.solve(l); await sleep(3500);
    const buf = await p.png(); fs.writeFileSync(path.join(dir, `show_${w}x${h}_3_kazanma.png`), buf);
    exc += p.exceptions.length; p.close();
  }
  W = 375; H = 667;
  check('show', exc === 0, `SHOW1 3 boyut → ${dir}, istisna ${exc}`);
}

/* --only paint40 (PM_MESAJ_18): every SET40 v3 level wears its own painting. At 3 phone sizes: the menu (the next
   level's painting in the corridor frame), levels 1, 2, 19, 30 and 40 on their painted boards, and level 1's win
   (the whole painting before the card, then the card). Debug pages, nothing sent. */
async function paint40() {
  const dir = opt('qa-out', path.join(ROOT, 'docs/qa', new Date().toISOString().slice(0, 10) + '-paint40'));
  fs.mkdirSync(dir, { recursive: true });
  const set = JSON.parse(fs.readFileSync(path.join(ROOT, 'Assets/_Game/Levels/redesign.json'), 'utf8')).levels;
  let exc = 0;
  for (const [w, h] of [[375, 667], [390, 844], [430, 932]]) {
    W = w; H = h;
    const shot = async (p, name) => fs.writeFileSync(path.join(dir, `${w}x${h}_${name}.png`), await p.png());
    const tapCard = async p => { for (let y = Math.round(H * .45); y < H * .9; y += 18) await p.tap(W / 2, y); await sleep(500); };
    let p = await newPage(); await p.open(dbg(`${base}/?set=redesign`)); await sleep(4000);
    await shot(p, '0_menu'); exc += p.exceptions.length; p.close();
    for (const n of [1, 2, 19, 30, 40]) {
      p = await newPage(); await p.open(dbg(`${base}/?set=redesign&lv=${set[n - 1].id}`)); await sleep(3500);
      await tapCard(p); await sleep(1500); await shot(p, `lv${String(n).padStart(2, '0')}`); exc += p.exceptions.length; p.close();
    }
    p = await newPage(); await p.open(dbg(`${base}/?set=redesign&lv=${set[0].id}`)); await sleep(3500);
    await p.solve(set[0]); await sleep(250); await shot(p, 'win_1_resim'); await sleep(2900); await shot(p, 'win_2_kart');
    exc += p.exceptions.length; p.close();
  }
  W = 375; H = 667;
  check('paint40', exc === 0, `3 boyut → ${dir}, istisna ${exc}`);
}

/* --only p19 (PM_MESAJ_19): menu, name gate, game, win card and settings at 3 phone sizes, in English and Turkish
   (?lang=). The gate and the menu as a new visitor sees them; the rest on debug pages (nothing sent). */
async function p19() {
  const dir = opt('qa-out', path.join(ROOT, 'docs/qa', new Date().toISOString().slice(0, 10) + '-p19'));
  fs.mkdirSync(dir, { recursive: true });
  const set = JSON.parse(fs.readFileSync(path.join(ROOT, 'Assets/_Game/Levels/redesign.json'), 'utf8')).levels;
  let exc = 0;
  for (const lang of ['en', 'tr'])
    for (const [w, h] of [[375, 667], [390, 844], [430, 932]]) {
      W = w; H = h;
      const shot = async (p, name) => fs.writeFileSync(path.join(dir, `${lang}_${w}x${h}_${name}.png`), await p.png());
      const tapCard = async p => { for (let y = Math.round(H * .45); y < H * .9; y += 18) await p.tap(W / 2, y); await sleep(500); };
      let p = await newPage(); await p.open(`${base}/?set=redesign&lang=${lang}`, { gate: true }); await sleep(1500);
      await shot(p, '1_isim'); await p.name('Zeynep Yılmaz12345'); await sleep(2500); await shot(p, '2_menu');
      await p.tap(W - 39, 38); await sleep(800); await shot(p, '5_ayarlar'); exc += p.exceptions.length; p.close();
      p = await newPage(); await p.open(dbg(`${base}/?set=redesign&lang=${lang}&lv=${set[29].id}`)); await sleep(2500);
      await tapCard(p); await sleep(1200); await shot(p, '3_oyun'); exc += p.exceptions.length; p.close();
      p = await newPage(); await p.open(dbg(`${base}/?set=redesign&lang=${lang}&lv=${set[0].id}`)); await sleep(2500);
      await p.solve(set[0]); await sleep(3200); await shot(p, '4_kazanma'); exc += p.exceptions.length; p.close();
    }
  W = 375; H = 667;
  check('p19', exc === 0, `2 dil x 3 boyut → ${dir}, istisna ${exc}`);
}

/* --only p20 (preview feedback 2026-10-28): level 1's hand (one glove, gone after two moves), level 2 without a
   hand, wall corners on 19/25/35/40 and SHOW1, and the progress recovery: a fresh browser, the name TEST_B5 (one
   S41 win on the server) -> the menu offers Level 2. That page reads Supabase (GET only; nothing is played). */
async function p20() {
  const dir = opt('qa-out', path.join(ROOT, 'docs/qa', new Date().toISOString().slice(0, 10) + '-p20b'));
  fs.mkdirSync(dir, { recursive: true });
  const set = JSON.parse(fs.readFileSync(path.join(ROOT, 'Assets/_Game/Levels/redesign.json'), 'utf8')).levels;
  let exc = 0, recovered = false;
  for (const [w, h] of [[375, 667], [390, 844], [430, 932]]) {
    W = w; H = h;
    const shot = async (p, name) => fs.writeFileSync(path.join(dir, `${w}x${h}_${name}.png`), await p.png());
    const tapCard = async p => { for (let y = Math.round(H * .45); y < H * .9; y += 18) await p.tap(W / 2, y); await sleep(500); };
    let p = await newPage(); await p.open(dbg(`${base}/?set=redesign&lv=${set[0].id}`)); await sleep(2500);
    await shot(p, 'lv01_0');
    await p.solve({ ...set[0], solution: set[0].solution.slice(0, 2) }); await sleep(800); await shot(p, 'lv01_2hamle');
    exc += p.exceptions.length; p.close();
    for (const n of [2, 19, 25, 35, 40]) {
      p = await newPage(); await p.open(dbg(`${base}/?set=redesign&lv=${set[n - 1].id}`)); await sleep(2500);
      await tapCard(p); await sleep(1200); await shot(p, `lv${String(n).padStart(2, '0')}`); exc += p.exceptions.length; p.close();
    }
    p = await newPage(); await p.open(`${base}/?set=redesign&painting=1&lv=SHOW1`, { gate: true }); await sleep(2500);
    await p.tap(W / 2, H - 70); await sleep(1500); await tapCard(p); await sleep(1000); await shot(p, 'show1'); exc += p.exceptions.length; p.close();
  }
  W = 390; H = 844;
  const p = await newPage(); await p.cdp('Network.setBlockedURLs', { urls: [] });
  await p.open(`${base}/?set=redesign&lang=en`, { gate: true }); await p.name('TEST_B5'); await sleep(5000);
  fs.writeFileSync(path.join(dir, 'recover_menu.png'), await p.png());
  recovered = p.supabase.every(m => m === 'GET' || m === 'OPTIONS') && p.supabase.includes('GET');   // OPTIONS: the CORS preflight
  exc += p.exceptions.length; p.close();
  W = 375; H = 667;
  check('p20', exc === 0 && recovered, `3 boyut → ${dir}, istisna ${exc}, Supabase yalnız okuma: ${recovered} (menü Level 2 mi: recover_menu.png)`);
}

/* --only music: the background music starts on the first touch (streamed MP3 from Pages, looped from its first
   sound for 57.6 s), the settings switch silences it, and a win ducks it. Reads window.rsMusic. */
async function music() {
  W = 390; H = 844;
  const set = JSON.parse(fs.readFileSync(path.join(ROOT, 'Assets/_Game/Levels/redesign.json'), 'utf8')).levels;
  // before any touch, on a visitor's page (the debug page's load panel is clicked by the script itself)
  const q = await newPage(); await q.open(`${base}/?set=redesign`, { gate: true }); await sleep(3000);
  const before = await q.ev(`(function(){var M=window.rsMusic; return M ? {playing: !!M.playing, ctx: !!M.ctx} : null;})()`); q.close();
  const p = await newPage(); await p.open(dbg(`${base}/?set=redesign&lv=${set[0].id}`)); await sleep(1500);
  const st = () => p.ev(`(function(){var M=window.rsMusic; if(!M) return null; return {ctx: M.ctx ? M.ctx.state : null, playing: !!M.playing,
    start: M.src ? +M.src.loopStart.toFixed(4) : null, len: M.src ? +(M.src.loopEnd - M.src.loopStart).toFixed(3) : null,
    gain: M.gain ? +M.gain.gain.value.toFixed(3) : null};})()`);
  await p.tap(W / 2, H * .85); await sleep(6000);
  const after = await st();
  await p.solve(set[0]); await sleep(1500);
  const ducked = await st(); await sleep(3500);
  const back = await st();
  const exc = p.exceptions.length; p.close();
  const ok = before && !before.playing && !before.ctx && after && after.playing && Math.abs(after.len - 57.6) < .01 && ducked.gain < .2 && back.gain > .3 && exc === 0;
  check('music', ok, `dokunmadan önce çalmıyor: ${before && !before.playing}; sonra ${JSON.stringify(after)}; kazanınca kısık ${ducked && ducked.gain}, sonra ${back && back.gain}; istisna ${exc}`);
  W = 375; H = 667;
}

/* --only chain (PM_MESAJ_20): the chain on painted boards, levels 24 and 40 and SHOW1 at 3 phone sizes, plus level
   40 with a chained piece held mid-drag (the chain and its locks must travel with it). Debug pages, nothing sent. */
async function chain() {
  const dir = opt('qa-out', path.join(ROOT, 'docs/qa', new Date().toISOString().slice(0, 10) + '-chain'));
  fs.mkdirSync(dir, { recursive: true });
  const set = JSON.parse(fs.readFileSync(path.join(ROOT, 'Assets/_Game/Levels/redesign.json'), 'utf8')).levels;
  let exc = 0;
  for (const [w, h] of [[375, 667], [390, 844], [430, 932]]) {
    W = w; H = h;
    const shot = async (p, name) => fs.writeFileSync(path.join(dir, `${w}x${h}_${name}.png`), await p.png());
    const tapCard = async p => { for (let y = Math.round(H * .45); y < H * .9; y += 18) await p.tap(W / 2, y); await sleep(500); };
    for (const n of [24, 40]) {
      const l = set[n - 1];
      const p = await newPage(); await p.open(dbg(`${base}/?set=redesign&lv=${l.id}`)); await sleep(2500);
      await tapCard(p); await sleep(1500); await shot(p, `lv${n}`);
      const ci = l.pieces.findIndex(pc => pc.length > 1);
      if (n === 40 && ci >= 0) {
        const pl = placement(l), c = l.pieces[ci][0], x0 = pl.x + (c[0] + .5) * pl.cell, y0 = pl.y + (c[1] + .5) * pl.cell;
        await p.touch('touchStart', x0, y0);
        for (let k = 1; k <= 6; k++) { await p.touch('touchMove', x0 + k * 3, y0); await sleep(16); }
        await sleep(300); await shot(p, `lv${n}_surukle`); await p.touch('touchEnd', x0 + 18, y0);
      }
      exc += p.exceptions.length; p.close();
    }
    const p = await newPage(); await p.open(`${base}/?set=redesign&painting=1&lv=SHOW1`, { gate: true }); await sleep(2500);
    await p.tap(W / 2, H - 70); await sleep(1500); await tapCard(p); await sleep(1000); await shot(p, 'show1'); exc += p.exceptions.length; p.close();
  }
  W = 375; H = 667;
  check('chain', exc === 0, `3 boyut → ${dir}, istisna ${exc}`);
}

/* --only p20menu (PM_MESAJ_26): every menu tab and card at 375x667, 390x844 and 430x932, both languages, with a long
   name, as if 12 levels were won (?won=12, debug only); plus 0 and 40 won at 390x844. Shots -> docs/qa/<date>-p20. */
async function p20menu() {
  const dir = opt('qa-out', path.join(ROOT, 'docs/qa', new Date().toISOString().slice(0, 10) + '-p20b'));
  fs.mkdirSync(dir, { recursive: true });
  let exc = 0, n = 0;
  // tab bar v3 (PM_MESAJ_27 B): the selected slot is twice as wide as the others
  let cur = 0;
  const tab = async (p, i) => { await p.tap(tabX(cur, i), H - 30); cur = i; await sleep(900); };
  const runs = [];
  for (const [w, h] of [[375, 667], [390, 844], [430, 932]]) for (const lang of ['tr', 'en']) runs.push([w, h, lang, 12]);
  runs.push([390, 844, 'tr', 0], [390, 844, 'tr', 40]);
  for (const [w, h, lang, won] of runs) {
    W = w; H = h;
    const shot = async (p, name) => { fs.writeFileSync(path.join(dir, `${lang}_${w}x${h}_won${won}_${name}.png`), await p.png()); n++; };
    const p = await newPage(); cur = 0;
    await p.open(dbg(`${base}/?set=redesign&hud=0&lang=${lang}&won=${won}&player=${encodeURIComponent('Zeynep Yılmazoğlu')}`));
    await sleep(2500); await p.tap(W / 2, H * .3); await sleep(500);              // the debug load-time panel
    await shot(p, '1_ev');
    await p.tap(...P20.settings(W, H)); await sleep(900); await shot(p, '7_ayarlar'); await p.tap(...P20.settingsClose(W, H)); await sleep(700);
    await p.tap(...P20.levels(W, H)); await sleep(1200); await shot(p, '8_bolumler'); await p.tap(...P20.back(W, H)); await sleep(900);
    await tab(p, 1); await shot(p, '2_koleksiyon');
    await p.tap(...P20.cell(W, H)); await sleep(1200); await shot(p, '3_tablo_detay'); await p.tap(...P20.close(W, H)); await sleep(700);
    await tab(p, 2); await shot(p, '4_jiggles');
    await tab(p, 3); await shot(p, '5_basarilar');
    await tab(p, 0); await sleep(300);
    await p.tap(...P20.profile(W, H)); await sleep(1000); await shot(p, '6_profil'); await p.tap(...P20.close(W, H)); await sleep(600);
    exc += p.exceptions.length; p.close();
  }
  W = 375; H = 667;
  check('p20menu', exc === 0, `${n} görüntü → ${dir}, istisna ${exc}`);
}
// tap points (pt) of the p20 menus, read off the 375x812 design and the first shots
/* --only perf (PM_MESAJ_28 B2): load time to the menu and from the menu to a playable level 1 under three networks
   (CPU x4), total download and the 10 biggest files, FPS (level 1, level 30, corridor, collection flick) and peak
   memory (JS heap + wasm memory). Only measures; -> docs/qa/<date>-perf.json and the console. */
async function perf() {
  W = 390; H = 844;
  const dir = path.join(ROOT, 'docs/qa', new Date().toLocaleDateString('sv') + '-perf'); fs.mkdirSync(dir, { recursive: true });
  const hook = `(function(){var M=WebAssembly.Memory;window.__mem=[];WebAssembly.Memory=function(d){var m=new M(d);window.__mem.push(m);return m};WebAssembly.Memory.prototype=M.prototype;
    var I=WebAssembly.instantiate;WebAssembly.instantiate=function(){return I.apply(this,arguments).then(function(r){try{var e=(r.instance||r).exports;for(var k in e)if(e[k] instanceof M)window.__mem.push(e[k])}catch(_){}return r})};
    var S=WebAssembly.instantiateStreaming;if(S)WebAssembly.instantiateStreaming=function(){return S.apply(this,arguments).then(function(r){try{var e=r.instance.exports;for(var k in e)if(e[k] instanceof M)window.__mem.push(e[k])}catch(_){}return r})};})()`;
  const memNow = async p => {
    const m = (await p.cdp('Performance.getMetrics')).result.metrics.find(x => x.name === 'JSHeapUsedSize');
    const w = await p.ev(`(window.__mem||[]).reduce(function(a,m){return Math.max(a,m.buffer.byteLength)},0)`);
    return { js: +(m.value / 1048576).toFixed(1), wasm: +(w / 1048576).toFixed(1) };
  };
  const nets = { 'Hızlı 4G': [9e6, 60], 'Yavaş 4G': [1.6e6, 150], '3G': [.75e6, 300] };
  const out = { load: {}, files: [], fps: {}, mem: {} };
  const only1 = opt('net', ''), doFps = opt('fps', '1') === '1';
  for (const [name, [bps, lat]] of Object.entries(nets)) {
    if (only1 !== '' && Object.keys(nets).indexOf(name) !== +only1) continue;   // --net 0|1|2
    const p = await newPage(); const files = new Map();
    await p.cdp('Performance.enable');
    await p.cdp('Network.setCacheDisabled', { cacheDisabled: true });
    await p.cdp('Network.emulateNetworkConditions', { offline: false, latency: lat, downloadThroughput: bps / 8, uploadThroughput: bps / 8 });
    await p.cdp('Emulation.setCPUThrottlingRate', { rate: 4 });
    await p.cdp('Page.addScriptToEvaluateOnNewDocument', { source: hook });
    const t0 = Date.now();
    await p.cdp('Page.navigate', { url: dbg(`${base}/?set=redesign&hud=0&player=TEST_PERF&lang=tr`) });
    let menu = null;
    while (Date.now() - t0 < 280000) { await sleep(500); menu = await p.ev(`window.LT && LT.end ? LT.end : null`); if (menu) break; }
    if (!menu) { out.load[name] = { menu: null, note: '280 sn içinde açılmadı, atlandı' }; p.close(); continue; }
    await sleep(2500);
    await p.ev(`(function(){var b=document.getElementById('timing'); if (b) b.style.display='none'})()`);
    await sleep(500);
    const lv0 = Date.now(); p.logs.length = 0;
    await p.tap(W / 2, H - 158);                                         // the Level button
    let ready = null;
    while (Date.now() - lv0 < 60000) { await sleep(100); if (p.logs.some(l => l.includes('[level-ready]'))) { ready = (Date.now() - lv0) / 1000; break; } }
    const res = await p.ev(`performance.getEntriesByType('resource').map(function(e){return [e.name.split('/').pop().split('?')[0], e.transferSize||e.encodedBodySize||0]})`);
    out.load[name] = { menu: +(menu / 1000).toFixed(1), level1: ready, mb: +(res.reduce((a, r) => a + r[1], 0) / 1048576).toFixed(2) };
    if (name === 'Hızlı 4G') { out.files = res.sort((a, b) => b[1] - a[1]).slice(0, 10).map(([n, b]) => [n, +(b / 1048576).toFixed(2)]); out.mem.afterLoad = await memNow(p); }
    console.log(`perf ${name}: menü ${out.load[name].menu} sn, bölüm 1 +${ready} sn, ${out.load[name].mb} MB`);
    p.close();
  }
  if (!doFps) { const f = path.join(dir, 'perf_' + (only1 || 'all') + '.json'); fs.writeFileSync(f, JSON.stringify(out, null, 1)); check('perf', true, JSON.stringify(out.load)); W = 375; H = 667; return; }
  // FPS and memory on the real GPU, no throttling
  const sample = async (p, ms, act) => {
    await p.ev(`window.__f=[];window.__fo=true;(function f(t){if(!window.__fo)return;window.__f.push(t);requestAnimationFrame(f)})(performance.now())`);
    const t0 = Date.now(); let i = 0;
    while (Date.now() - t0 < ms) { await act(i++); }
    const iv = await p.ev(`(function(){window.__fo=false;var f=window.__f,d=[];for(var i=1;i<f.length;i++)d.push(f[i]-f[i-1]);return d})()`);
    const s = iv.slice().sort((a, b) => a - b), f = v => +(1000 / v).toFixed(1);
    return s.length ? { median: f(s[s.length >> 1]), p95: f(s[Math.floor(s.length * .95)]), worst: f(s[s.length - 1]) } : null;
  };
  const drag = async (p, i) => { const cx = W / 2, cy = H / 2 - 20, a = i / 20; await p.touch(i ? 'touchMove' : 'touchStart', cx + 14 * Math.sin(a), cy + 10 * Math.cos(a)); await sleep(16); };
  let peak = { js: 0, wasm: 0 };
  const keep = m => { peak = { js: Math.max(peak.js, m.js), wasm: Math.max(peak.wasm, m.wasm) }; };
  for (const [name, q] of [['Bölüm 1', 'lv=1'], ['Bölüm 30', 'lv=30']]) {
    const p = await newPage(); await p.cdp('Performance.enable'); await p.cdp('Page.addScriptToEvaluateOnNewDocument', { source: hook });
    await p.open(dbg(`${base}/?set=redesign&hud=0&player=TEST_PERF&${q}`));
    for (let y = Math.round(H * .45); y < H * .8; y += 18) await p.tap(W / 2, y);   // intro card / tutorial
    out.fps[name] = await sample(p, 8000, i => drag(p, i)); await p.touch('touchEnd');
    keep(await memNow(p)); fs.writeFileSync(path.join(dir, `fps_${name.replace(' ', '')}.png`), await p.png()); p.close();
  }
  {
    const p = await newPage(); await p.cdp('Performance.enable'); await p.cdp('Page.addScriptToEvaluateOnNewDocument', { source: hook });
    await p.open(dbg(`${base}/?set=redesign&hud=0&player=TEST_PERF&won=40&corridor=1`));
    out.fps['Koridor'] = await sample(p, 3500, () => sleep(50));
    await sleep(1500);
    await p.tap(tabX(0, 1), H - 30); await sleep(1000);
    out.fps['Koleksiyon kaydırma'] = await sample(p, 6000, async i => {
      const up = i % 2 === 0, y0 = up ? 640 : 300, y1 = up ? 300 : 640;
      await p.touch('touchStart', W / 2, y0); for (let k = 1; k <= 6; k++) { p.touch('touchMove', W / 2, y0 + (y1 - y0) * k / 6); await sleep(16); }
      await p.touch('touchEnd'); await sleep(900);
    });
    keep(await memNow(p)); p.close();
  }
  out.mem.peak = peak;
  fs.writeFileSync(path.join(dir, 'perf.json'), JSON.stringify(out, null, 1));
  check('perf', true, Object.entries(out.load).map(([k, v]) => `${k}: menü ${v.menu} sn, +bölüm1 ${v.level1} sn`).join(' | ') +
    ' · FPS ' + Object.entries(out.fps).map(([k, v]) => `${k} ${v ? v.median : '-'}`).join(', ') + ` · tepe bellek JS ${peak.js} MB, wasm ${peak.wasm} MB → ${dir}`);
  W = 375; H = 667;
}

/* --only ftue (PM_MESAJ_28 B1): a first-time player on a clean device at 390x844 (touch), cohort ic: loader, name,
   profile, tutorial, levels 1-6, a bomb loss, its 1-10, then the menu tabs, settings and a replay from the levels map.
   One shot per step -> docs/qa/<date>-ftue/. Sends rows (TEST_ name) only with --send. */
async function ftue() {
  W = 390; H = 844;
  const set = JSON.parse(fs.readFileSync(path.join(ROOT, 'Assets/_Game/Levels/redesign.json'), 'utf8')).levels;
  const bomb = JSON.parse(fs.readFileSync(path.join(ROOT, 'Logs/veri/bomb_loss.json'), 'utf8'));
  const dir = path.join(ROOT, 'docs/qa', new Date().toLocaleDateString('sv') + '-ftue'); fs.mkdirSync(dir, { recursive: true });
  const p = await newPage(); let n = 0;
  const shot = async name => { fs.writeFileSync(path.join(dir, `${String(++n).padStart(2, '0')}_${name}.png`), await p.png()); };
  const scan = async () => {
    await sleep(2500); let prev = await p.png();
    for (let y = Math.round(H * .25); y < H * .9; y += 12) { await p.tap(W / 2, y); const now = await p.png(); if (Buffer.compare(now, prev) !== 0) break; prev = now; }
    await sleep(1200);
  };
  const intro = async () => { for (let y = Math.round(H * .45); y < H * .8; y += 18) await p.tap(W / 2, y); };
  const lang = opt('lang', 'tr'), part = +opt('part', '1');
  n = (part - 1) * 30;                                                      // shot numbers stay in order across parts
  const card = async name => {                                              // the intro card, then the obstacle spotlight
    await sleep(800); await shot(name + '_kart');
    let prev = await p.png();
    for (let y = Math.round(H * .5); y < H * .85; y += 16) { await p.tap(W / 2, y); await sleep(250); const now = await p.png(); if (Buffer.compare(now, prev) !== 0) break; prev = now; }
    await sleep(900); await shot(name + '_spot');
    await p.tap(W / 2, H * .5); await sleep(900);
  };
  if (part > 1) {
    await p.cdp('Page.navigate', { url: `${base}/?set=redesign&c=ic&lang=${lang}` });
    for (let i = 0; i < 120; i++) { await sleep(1000); if (await p.ev(`!!document.getElementById('rs-name') || !!(window.LT && LT.end)`)) break; }
    await sleep(1500); await p.name(opt('player', 'TEST_FTUE')); await sleep(2500); await scan();
  }
  if (part === 1) {
  await p.cdp('Page.navigate', { url: `${base}/?set=redesign&c=ic&lang=${lang}` });
  await sleep(1500); await shot('yukleme');
  for (let i = 0; i < 120; i++) { await sleep(1000); if (await p.ev(`!!document.getElementById('rs-name') || !!(window.LT && LT.end)`)) break; }
  await sleep(1500); await shot('isim');
  await p.name(opt('player', 'TEST_FTUE')); await sleep(2500); await shot('profil_sorusu');
  await scan(); await shot('menu_ilk');
  await p.tap(W / 2, H - 158); await sleep(2500); await shot('b1_acilis');
  const l1 = set[0];
  await p.solve({ ...l1, solution: l1.solution.slice(0, 1) }); await sleep(600); await shot('b1_egitim_adim2');
  await p.solve({ ...l1, solution: l1.solution.slice(1, 2) }); await sleep(900); await shot('b1_kural_karti');
  await p.tap(W / 2, H / 2); await sleep(900); await shot('b1_serbest');
  await p.solve({ ...l1, solution: l1.solution.slice(2) }); await sleep(2500); await shot('b1_kazanma');
  await scan(); await shot('b1_sonrasi');
  }
  for (let i = part === 1 ? 1 : part === 2 ? 3 : 6; i < (part === 1 ? 3 : 6); i++) {
    const l = set[i];
    await p.cdp('Page.navigate', { url: `${base}/?set=redesign&lv=${l.id}&lang=${lang}` }); await sleep(7000);
    await card(`b${i + 1}`);
    await p.solve(l); await sleep(2500); await shot(`b${i + 1}_kazanma`);
    await scan(); await sleep(1500);
  }
  if (part === 2) {
  await p.cdp('Page.navigate', { url: `${base}/?set=redesign&lv=${bomb.id}&lang=${lang}` }); await sleep(7000);
  await intro(); await p.solve({ ...set.find(x => x.id === bomb.id), solution: bomb.solution }); await sleep(2500); await shot('kayip');
  await scan(); await shot('kayip_1_10');
  await scan(); await shot('kayip_sonrasi');
  }
  if (part === 3) {
  await p.cdp('Page.navigate', { url: `${base}/?set=redesign&lang=${lang}` }); await sleep(7000); await shot('menu_ev');
  for (const [i, name] of [[1, 'koleksiyon'], [2, 'jiggles'], [3, 'basarilar']]) { await p.tap(tabX(i === 1 ? 0 : i - 1, i), H - 30); await sleep(1000); await shot('menu_' + name); }
  await p.tap(tabX(3, 0), H - 30); await sleep(1000);
  await p.tap(...P20.settings(W, H)); await sleep(1000); await shot('ayarlar'); await p.tap(...P20.settingsClose(W, H)); await sleep(800);
  await p.tap(...P20.levels(W, H)); await sleep(1500); await shot('bolumler');
  await scan(); await shot('bolumler_tekrar_oyna');
  }
  const exc = p.exceptions.length; p.close(); W = 375; H = 667;
  check('ftue', exc === 0, `${n} görüntü → ${dir}, istisna ${exc}`);
}

const tabX = (sel, i) => { let x = 0; for (let j = 0; j < i; j++) x += (j === sel ? 2 : 1); return W * (x + (i === sel ? 1 : .5)) / 5; };
const P20 = {
  settings: (w, h) => [w - 40, 49], profile: (w, h) => [50, 49], levels: (w, h) => [(w - 228) / 2 - 29, h - 158], back: (w, h) => [30, 38],
  settingsClose: (w, h) => [w / 2 + Math.min(329, w - 30) / 2 - 4, h / 2 - Math.min(500, h - 100) / 2 + 4],
  cell: (w, h) => [w / 2 - 110, 180], close: (w, h) => [w / 2 + 135, h / 2 - 255],
};

/* --only flick (PM_MESAJ_27 A2): a fast touch flick on the Collection list (40 won, 390x844); the list must travel at
   least one screen height. Reads the game's "[scroll] v .. pt/s, .. pt, .. s" line (debug only). */
async function flick() {
  W = 390; H = 844;
  const p = await newPage();
  await p.open(dbg(`${base}/?set=redesign&won=40&hud=0`));
  await sleep(2500); await p.tap(W / 2, H * .3); await sleep(500);
  await p.tap(tabX(0, 1), H - 30); await sleep(1000);
  const results = [];
  for (const [y0, y1] of [[640, 300], [300, 640]]) {
    await p.touch('touchStart', W / 2, y0);
    // a real flick: the moves go out without waiting for each reply, spread over ~100 ms
    const moves = [];
    for (let i = 1; i <= 6; i++) { moves.push(p.touch('touchMove', W / 2, y0 + (y1 - y0) * i / 6)); await sleep(16); }
    await Promise.all(moves);
    await p.touch('touchEnd'); await sleep(3000);
    const line = p.logs.filter(l => l.includes('[scroll]')).pop() || '';
    const m = line.match(/v (-?\d+) pt\/s, (-?\d+) pt, ([\d.]+) s/);
    results.push(m ? { v: +m[1], pt: +m[2], s: +m[3] } : null);
    p.logs.length = 0;
  }
  const ok = results.every(r => r && Math.abs(r.pt) >= H);
  check('flick', ok, results.map(r => r ? `${r.v} pt/s → ${r.pt} pt, ${r.s} s` : 'günlük yok').join(' | ') + `, ekran ${H} pt`);
  p.close(); W = 375; H = 667;
}

/* --send --only rating (PM_MESAJ_29 1): the plays row goes when the level ends, the 1-10 answer follows through
   rpc/set_rating. Three fresh profiles (the question is asked once per level): (a) S41 won, tab closed without an
   answer; (b) S41 won, 7; (c) S50 lost to the bomb, 3. Card shots in docs/qa/<date>-rating; the chip spots come from
   --chip7 x,y and --chip3 x,y (390x844, read off the first card shot). */
async function rating() {
  const set = JSON.parse(fs.readFileSync(path.join(ROOT, 'Assets/_Game/Levels/redesign.json'), 'utf8')).levels;
  const l = set.find(x => x.id === 'S41'), bomb = JSON.parse(fs.readFileSync(path.join(ROOT, 'Logs/veri/bomb_loss.json'), 'utf8'));
  const tag = opt('icname', 'IC_P29'), dir = path.join(ROOT, 'docs/qa', new Date(Date.now() - new Date().getTimezoneOffset() * 60000).toISOString().slice(0, 10) + '-rating'), xy = k => opt(k, '').split(',').map(Number);
  fs.mkdirSync(dir, { recursive: true });
  const intro = async p => { for (let y = Math.round(H * .45); y < H * .8; y += 18) await p.tap(W / 2, y); };
  const tutorialWin = async p => {
    await intro(p); await p.solve({ ...l, solution: l.solution.slice(0, 2) }); await sleep(900);
    await p.tap(W / 2, H / 2); await sleep(900);
    await p.solve({ ...l, solution: l.solution.slice(2) });
  };
  const start = async (name, lv) => {
    const p = await newPage();
    await p.open(`${base}/?set=redesign&c=ic&lv=${lv}`, { gate: true }); await p.name(name); await sleep(1500);
    for (let y = Math.round(H * .25); y < H * .9; y += 12) await p.tap(W / 2, y);   // the profile question
    await sleep(1000);
    return p;
  };
  const parts = opt('rparts', 'abc');
  let p;
  // (a) win, no answer, the tab goes 3 s after the card
  if (parts.includes('a')) { p = await start(tag + 'A', 'S41'); await tutorialWin(p); await sleep(5500);
  fs.writeFileSync(path.join(dir, 'a_kart.png'), await p.png());
  await p.cdp('Page.navigate', { url: 'about:blank' }); await sleep(3000); p.close(); }
  // (b) win, 7
  if (parts.includes('b')) { p = await start(tag + 'B', 'S41'); await tutorialWin(p); await sleep(5500);
  const [x7, y7] = xy('chip7'); await p.tap(x7, y7); await sleep(400);
  fs.writeFileSync(path.join(dir, 'b_7_secildi.png'), await p.png()); await sleep(2500);
  fs.writeFileSync(path.join(dir, 'b_sonra.png'), await p.png());
  await p.cdp('Page.navigate', { url: 'about:blank' }); await sleep(3000); p.close(); }
  // (c) bomb loss, 3
  if (parts.includes('c')) { p = await start(tag + 'C', bomb.id); await intro(p); await p.solve({ ...set.find(x => x.id === bomb.id), solution: bomb.solution }); await sleep(3500);
  fs.writeFileSync(path.join(dir, 'c_kayip_kart.png'), await p.png());
  const [x3, y3] = xy('chip3'); await p.tap(x3, y3); await sleep(2500);
  await p.cdp('Page.navigate', { url: 'about:blank' }); await sleep(3000); p.close(); }
  await sleep(4000);
  const get = async q => (await (await fetch(`${SUPA}/rest/v1/${q}`, { headers: { apikey: KEY, Authorization: 'Bearer ' + KEY } })).json());
  const rows = await get(`plays?player=in.(${tag}A,${tag}B,${tag}C)&select=player,level_id,result,attempt,zorluk_puani,keyif,cohort,app_version,row_id,created_at&order=created_at`);
  console.log('PLAYS ' + JSON.stringify(rows));
  const of = n => rows.filter(r => r.player === tag + n);
  const a = of('A').some(r => r.result === 'win'), b = of('B').some(r => r.result === 'win' && r.zorluk_puani === 7),
        c = of('C').some(r => r.result === 'bomb' && r.zorluk_puani === 3);
  check('rating', a && b && c, `(a) cevapsız kazanma satırı ${a ? 'var' : 'YOK'} · (b) 7 ${b ? 'var' : 'YOK'} · (c) kayıp 3 ${c ? 'var' : 'YOK'}`);
}

/* --only spot (PM_MESAJ_31): the tutorial veil at 390x844 and 375x667 (debug, no sending): level 1 (two drag
   steps), 3, 6, 10, 14 (obstacle spotlight), 5 (scissors: button, target). Shots in docs/qa/<date>-spot; the veil
   tone at 5 fixed spots (sum of RGB) is printed per shot. */
async function spot() {
  const set = JSON.parse(fs.readFileSync(path.join(ROOT, 'Assets/_Game/Levels/redesign.json'), 'utf8')).levels;
  const dir = path.join(ROOT, 'docs/qa', new Date(Date.now() - new Date().getTimezoneOffset() * 60000).toISOString().slice(0, 10) + '-spot');
  fs.mkdirSync(dir, { recursive: true });
  const lines = [];
  for (const [w, h] of [[390, 844], [375, 667]]) {
    W = w; H = h;
    for (const slot of opt('slots', '1,3,6,10,14,5').split(',').map(Number)) {
      const l = set[slot - 1], p = await newPage();
      const shot = async name => {
        const buf = await p.png(); fs.writeFileSync(path.join(dir, `${w}x${h}_b${slot}_${name}.png`), buf);
        const img = readPng(buf), pts = [[.06, .2], [.94, .35], [.5, .975], [.06, .62], [.94, .8]];
        lines.push(`${w}x${h} b${slot} ${name}: ` + pts.map(([x, y]) => img.at(Math.round(img.w * x), Math.round(img.h * y)).slice(0, 3).reduce((a, b) => a + b, 0)).join(' '));
      };
      // the intro card is cream and animates, so stop when the mean brightness drops (card closed), not on any change
      const light = async () => { const img = readPng(await p.png()); let s = 0, n = 0;
        for (let y = .3; y < .8; y += .05) for (let x = .2; x < .8; x += .05) { s += img.at(Math.round(img.w * x), Math.round(img.h * y)).slice(0, 3).reduce((a, b) => a + b, 0); n++; } return s / n; };
      const scan = async (from, to, step) => {
        await sleep(2500); const open = await light();
        for (let y = Math.round(H * from); y < H * to; y += step) { await p.tap(W / 2, y); await sleep(400); if (await light() < open * .75) break; }
        await sleep(1200);
      };
      await p.open(dbg(`${base}/?set=redesign&lv=${l.id}&lang=tr`)); await sleep(3000);
      if (slot === 1) {
        await sleep(1500); await shot('1');
        await p.solve({ ...l, solution: l.solution.slice(0, 1) }); await sleep(1500); await shot('2');
      } else {
        await scan(.45, .85, 14); await shot('1');
        if (slot === 5) { await p.tap(77 * W / 375, H - 65); await sleep(1500); await shot('2'); }
      }
      p.close();
    }
  }
  W = 375; H = 667;
  fs.writeFileSync(path.join(dir, 'ton.txt'), lines.join(String.fromCharCode(10)) + String.fromCharCode(10));
  check('spot', true, `${lines.length} görüntü → ${dir} (ton: ton.txt)`);
}

async function son() {
  const set = JSON.parse(fs.readFileSync(path.join(ROOT, 'Assets/_Game/Levels/redesign.json'), 'utf8')).levels;
  const dir = path.join(ROOT, 'docs/qa', new Date(Date.now() - new Date().getTimezoneOffset() * 60000).toISOString().slice(0, 10) + '-son');
  fs.mkdirSync(dir, { recursive: true });
  let exc = 0, n = 0;
  const shot = async (p, name) => { fs.writeFileSync(path.join(dir, name + '.png'), await p.png()); n++; };
  W = 390; H = 844;
  const l = set.find(x => x.id === 'S41'), p = await newPage();
  await p.open(dbg(base + '/?set=redesign&lv=S41&lang=tr')); await sleep(4500); await shot(p, 'b1_adim1');
  await p.solve({ ...l, solution: l.solution.slice(0, 1) }); await sleep(1500); await shot(p, 'b1_adim2');
  await p.solve({ ...l, solution: [{ piece: 6, dir: 'R' }] });
  const start = Date.now(), times = [0, 300, 600, 1000, 1500];
  for (const [i, ms] of times.entries()) {
    await sleep(Math.max(0, start + ms - Date.now())); await shot(p, `b1_seq_${i}`);
  }
  await sleep(2500); await shot(p, 'b1_adim3');
  exc += p.exceptions.length; p.close();

  for (const [id, name] of [['Z24', 'b10'], ['Z21', 'b14']]) {
    const p = await newPage();
    const light = async () => { const img = readPng(await p.png()); let s = 0, n = 0;
      for (let y = .3; y < .8; y += .05) for (let x = .2; x < .8; x += .05) { s += img.at(Math.round(img.w * x), Math.round(img.h * y)).slice(0, 3).reduce((a, b) => a + b, 0); n++; } return s / n; };
    const scan = async (from, to, step) => {
      await sleep(2500); const open = await light();
      for (let y = Math.round(H * from); y < H * to; y += step) { await p.tap(W / 2, y); await sleep(400); if (await light() < open * .75) break; }
      await sleep(1200);
    };
    await p.open(dbg(`${base}/?set=redesign&c=ic&unlock=1&lv=${id}&lang=tr`)); await sleep(3000);
    await scan(.45, .85, 14); await shot(p, name);
    exc += p.exceptions.length; p.close();
  }

  let cur = 0;
  const tab = async (p, i) => { await p.tap(tabX(cur, i), H - 30); cur = i; await sleep(900); };
  for (const [w, h] of [[375, 667], [390, 844], [430, 932]]) {
    W = w; H = h;
    const p = await newPage(); cur = 0;
    await p.open(dbg(`${base}/?set=redesign&hud=0&lang=tr&won=12&player=${encodeURIComponent('Zeynep Yılmazoğlu')}`));
    await sleep(2500); await p.tap(W / 2, H * .3); await sleep(500);
    await p.tap(...P20.levels(W, H)); await sleep(1200); await shot(p, `${w}x${h}_harita`); await p.tap(...P20.back(W, H)); await sleep(900);
    await tab(p, 1); await shot(p, `${w}x${h}_koleksiyon`);
    await p.tap(...P20.cell(W, H)); await sleep(1200); await p.tap(...P20.close(W, H)); await sleep(700);
    await tab(p, 2); await shot(p, `${w}x${h}_jiggles`);
    exc += p.exceptions.length; p.close();
    // 4 won: the next painting (level 5, "Sıradaki") is on the first screen of the collection
    const q = await newPage(); cur = 0;
    await q.open(dbg(`${base}/?set=redesign&hud=0&lang=tr&won=4&player=${encodeURIComponent('Zeynep Yılmazoğlu')}`));
    await sleep(2500); await q.tap(W / 2, H * .3); await sleep(500);
    await tab(q, 1); await shot(q, `${w}x${h}_koleksiyon_siradaki`);
    exc += q.exceptions.length; q.close();
  }
  W = 375; H = 667;
  check('son', exc === 0, n + ' görüntü → ' + dir);
}

/* --send --only booster (PM_MESAJ_30): fresh profiles (cohort ic) open the booster levels 5 (scissors), 9 (wand),
   15 (clock), 23 (hammer): the button step, the target step (tap on the lit spot), the effect; then pause -> menu
   (a quit row). Level 10: the chain spotlight. Shots in docs/qa/<date>-booster; Supabase: moves booster rows,
   plays b_* 0. --bparts 5,9,15,23,10 picks levels. */
async function booster() {
  const set = JSON.parse(fs.readFileSync(path.join(ROOT, 'Assets/_Game/Levels/redesign.json'), 'utf8')).levels;
  const tag = opt('icname', 'IC_P30'), parts = opt('bparts', '5,9,15,23,10').split(',').map(Number);
  const dir = path.join(ROOT, 'docs/qa', new Date(Date.now() - new Date().getTimezoneOffset() * 60000).toISOString().slice(0, 10) + '-booster');
  fs.mkdirSync(dir, { recursive: true });
  const btn = { 5: 0, 9: 1, 23: 2, 15: 3 };                            // bar order: scissors, wand, hammer, clock
  const lit = async p => {                                            // centre of the bright spot between HUD and bar
    const img = readPng(await p.png()); let sx = 0, sy = 0, n = 0;
    for (let y = Math.round(img.h * .15); y < img.h * .68; y += 2) for (let x = 0; x < img.w; x += 2) {
      const [r, g, b] = img.at(x, y); if (r + g + b > 330 && !(r > 225 && g > 225 && b > 225)) { sx += x; sy += y; n++; } }   // not the white glove
    return n ? [sx / n / 2, sy / n / 2, n] : null;
  };
  const shot = async (p, name) => fs.writeFileSync(path.join(dir, name + '.png'), await p.png());
  for (const slot of parts) {
    const l = set[slot - 1], name = `${tag}_${slot}`, p = await newPage();
    await p.open(`${base}/?set=redesign&c=ic&lv=${l.id}`, { gate: true }); await p.name(name); await sleep(1500);
    const scan = async (from, to, step) => {        // tap down the screen until the picture changes (one card closed)
      await sleep(2500); let prev = await p.png();
      for (let y = Math.round(H * from); y < H * to; y += step) { await p.tap(W / 2, y); await sleep(250); const now = await p.png(); if (Buffer.compare(now, prev) !== 0) break; prev = now; }
      await sleep(1200);
    };
    await scan(.25, .9, 12); await shot(p, `b${slot}_0a`);   // profile question (or the start card when none)
    await scan(.45, .85, 14); await shot(p, `b${slot}_0b`);  // the start card (or the obstacle spotlight)
    await sleep(800); await shot(p, `b${slot}_1_tus`);
    if (slot in btn) {
      await p.tap([77, 150, 222, 295][btn[slot]] * W / 375, H - 65); await sleep(1500); await shot(p, `b${slot}_2_hedef`);
      if (slot === 5 || slot === 23) { const c = await lit(p); if (c) await p.tap(c[0], c[1]); await sleep(1800); }
      await shot(p, `b${slot}_3_etki`);
      if (slot === 15) {                              // a normal use: 3 -> 2 (through the confirm card, if any)
        await p.tap(295 * W / 375, H - 65); await sleep(1000); await shot(p, `b15_4_onay`);
        await scan(.4, .85, 14); await sleep(1000); await shot(p, `b15_5_stok`);
      }
    }
    await p.tap(W - 36, 33); await sleep(800);                          // pause -> menu: the attempt ends as quit
    for (let y = Math.round(H * .85); y > H * .4; y -= 14) await p.tap(W / 2, y);
    await sleep(17000); await p.cdp('Page.navigate', { url: 'about:blank' }); await sleep(2000); p.close();
  }
  const get = async q => (await (await fetch(`${SUPA}/rest/v1/${q}`, { headers: { apikey: KEY, Authorization: 'Bearer ' + KEY } })).json());
  const pl = await get(`plays?player=like.${tag}_*&select=player,level_id,result,b_scissors,b_wand,b_hammer,b_clock&order=created_at`);
  const mv = await get(`moves?player=like.${tag}_*&select=player,level_id,move_no,kind,dir&order=created_at`);
  console.log('PLAYS ' + JSON.stringify(pl));
  console.log('MOVES ' + JSON.stringify(mv.map(m => `${m.player}:${m.kind}${m.dir ? '/' + m.dir : ''}`)));
  // the guided use is not counted; level 15's second clock use is the player's own (b_clock 1)
  const ok = pl.length > 0 && pl.every(r => !r.b_scissors && !r.b_wand && !r.b_hammer && r.b_clock === (r.level_id === set[14].id ? 1 : 0));
  check('booster', ok, `plays ${pl.map(r => `${r.level_id} b_*=${r.b_scissors}${r.b_wand}${r.b_hammer}${r.b_clock}`).join(' ')}; moves ${mv.filter(m => ['scissors', 'wand', 'hammer', 'clock'].includes(m.kind)).map(m => m.player.split('_').pop() + ':' + m.kind).join(' ')}`);
}

/* --send --only replay (PM_MESAJ_26): one browser profile (cohort ic) wins S41, then plays it again from the
   menu path; plays must hold attempt 1 and attempt 2 with cohort, device_id, platform and app_version. */
async function replay() {
  const set = JSON.parse(fs.readFileSync(path.join(ROOT, 'Assets/_Game/Levels/redesign.json'), 'utf8')).levels;
  const l = set.find(x => x.id === 'S41'), name = opt('icname', 'IC_REPLAY'), p = await newPage();
  const intro = async () => { for (let y = Math.round(H * .45); y < H * .8; y += 18) await p.tap(W / 2, y); };
  await p.open(`${base}/?set=redesign&c=ic&lv=S41`, { gate: true }); await p.name(name); await sleep(1500);
  for (let y = Math.round(H * .25); y < H * .9; y += 12) await p.tap(W / 2, y + 0);   // the profile question
  for (let k = 0; k < 2; k++) {
    if (k) { await p.open(`${base}/?set=redesign&lv=S41`, { gate: true }); await sleep(4000); }
    // PM_MESAJ_28 tutorial: two guided moves, the rule card (a tap), then the rest
    await intro(); await p.solve({ ...l, solution: l.solution.slice(0, 2) }); await sleep(900);
    await p.tap(W / 2, H / 2); await sleep(900);
    await p.solve({ ...l, solution: l.solution.slice(2) }); await sleep(18000);
  }
  await p.cdp('Page.navigate', { url: 'about:blank' }); await sleep(3000); p.close();
  const rows = await (await fetch(`${SUPA}/rest/v1/plays?player=eq.${name}&select=level_id,result,attempt,cohort,platform,device_id,app_version&order=created_at`, { headers: { apikey: KEY, Authorization: 'Bearer ' + KEY } })).json();
  console.log('PLAYS ' + JSON.stringify(rows));
  const s41 = rows.filter(r => r.level_id === 'S41');
  const ok = s41.some(r => r.attempt === 1) && s41.some(r => r.attempt === 2) && rows.every(r => r.cohort === 'ic' && r.platform && r.device_id && /^web-p20/.test(r.app_version || ''));
  check('replay', ok, `S41 denemeleri ${s41.map(r => r.attempt + ':' + r.result).join(' ')}; ${rows[0] ? rows[0].app_version : '-'}`);
}

/* --only corridor (PM_MESAJ_24): win -> Next -> the corridor slide -> next level. Levels 1, 15 and 39 at phone,
   desktop and narrow sizes (shots at the start, middle and end of the slide, no #00ff00 anywhere, bay 2 not the
   dark placeholder at the end), level 40's end card (no slide), level 1 -> 2 on a slow network from the first byte (the next painting
   was fetched while 1 was played; 14 -> 15 by hand). Debug pages, nothing sent. */
async function corridor() {
  const dir = opt('qa-out', path.join(ROOT, 'docs/qa', new Date().toISOString().slice(0, 10) + '-corridor'));
  fs.mkdirSync(dir, { recursive: true });
  const set = JSON.parse(fs.readFileSync(path.join(ROOT, 'Assets/_Game/Levels/redesign.json'), 'utf8')).levels;
  const wall = img => {           // the corridor's teal wall at the left edge, 30 % down (the hall and the intro card have none there)
    const k = Math.min(Math.max(img.w / 780, img.h / 1688), 1.3 * img.h / 1688), x0 = Math.max(0, Math.round(img.w / 2 - 390 * k)), x1 = x0 + Math.round(90 * k);
    let n = 0; const y = Math.round(img.h * .3); for (let x = x0; x < x1; x++) { const [r, g, b] = img.at(x, y); if (r < 70 && g > 70 && g < 150 && b > 75 && b < 160) n++; } return n > (x1 - x0) * .4;
  };
  const green = img => { let n = 0; for (let y = 0; y < img.h; y += 2) for (let x = 0; x < img.w; x += 2) { const [r, g, b] = img.at(x, y); if (g > 200 && r < 90 && b < 90) n++; } return n; };
  const bay2 = img => {          // bay 2's centre once the slide is over: the painting, or the dark (38,30,26) placeholder
    const k = Math.min(Math.max(img.w / 780, img.h / 1688), 1.3 * img.h / 1688), [r, g, b] = img.at(Math.round(img.w / 2 + 1.75 * k), Math.round(img.h / 2 - 257 * k));
    return Math.abs(r - 38) < 12 && Math.abs(g - 30) < 12 && Math.abs(b - 26) < 12 ? 'yer tutucu' : 'tablo';
  };
  const play = async (n, tag, slow, menu) => {   // menu: the card's Menu button, tapped again 0.1 s later (the card waits .12 s, so the slide is .0-.1 s old)
    const l = set[n - 1], p = await newPage();
    // slow: 1.5 MB/s and 150 ms from the first byte, so the next painting comes in while this level is played
    if (slow) { await p.cdp('Network.setCacheDisabled', { cacheDisabled: true }); await p.cdp('Network.emulateNetworkConditions', { offline: false, latency: 150, downloadThroughput: 1.5e6, uploadThroughput: 1.5e6 }); }
    await p.open(dbg(`${base}/?set=redesign&lv=${l.id}`));
    await sleep(2500); for (let y = Math.round(H * .45); y < H * .9; y += 18) await p.tap(W / 2, y); await sleep(500);   // as paint40
    if (opt('probe', '')) for (let i = 0; i < 4; i++) { await sleep(700); fs.writeFileSync(path.join(dir, `${W}x${H}_${tag}_probe${i}.png`), await p.png()); }
    await p.solve(l); await sleep(3000);
    let started = false, t0 = 0;
    for (let i = 0, y; (y = Math.round(H * (menu ? .63 : .4)) + 14 * i) < H * (menu ? .85 : .97) && !started; i++) { t0 = Date.now(); await p.tap(menu ? W * .69 : W / 2, y); if (menu) { await sleep(100); await p.tap(W * .69, y); } await sleep(120); started = p.logs.some(x => /\[corridor\]/.test(x)); }
    const res = { started, exc: 0, green: 0, end: '', at: [] };
    if (started) {
      const shots = [];                                   // slide: fade .25 + hold .35 + 3 s; the shot times are kept
      for (const [t, name] of [[450, 'a_bas'], [2100, 'b_orta'], [3450, 'c_son'], [5000, 'd_sonraki']]) {
        await sleep(Math.max(0, t0 + t - Date.now())); res.at.push(Date.now() - t0);
        const buf = await p.png(); fs.writeFileSync(path.join(dir, `${W}x${H}_${tag}_${name}.png`), buf); shots.push(readPng(buf));
      }
      res.green = shots.slice(0, 3).reduce((a, i) => a + green(i), 0); res.end = res.at[2] < 3550 ? bay2(shots[2]) : 'geç çekildi'; res.after = !wall(shots[3]); res.log = (p.logs.find(x => /\[corridor\]/.test(x)) || '').replace(/.*\[corridor\] /, ''); res.dur = parseFloat((p.logs.find(x => /\[corridor-end\]/.test(x)) || '').replace(/.*\[corridor-end\] /, '').replace(',', '.')) || 0;
    } else fs.writeFileSync(path.join(dir, `${W}x${H}_${tag}_kart.png`), await p.png());
    fs.writeFileSync(path.join(dir, `${W}x${H}_${tag}_log.txt`), p.logs.filter(x => /corridor/.test(x)).join(' | '));
    res.exc = p.exceptions.length; p.close(); return res;
  };
  const play2 = async (n, tag, slow, menu) => { const r = await play(n, tag, slow, menu); return r.started || n === 40 ? r : play(n, tag, slow, menu); };   // the scripted solve misses now and then (swiftshader)
  let ok = true; const notes = [];
  for (const [w, h] of [[375, 667], [390, 844], [900, 640], [320, 720]]) {
    W = w; H = h;
    for (const n of (opt('lvs', '') ? opt('lvs').split(',').filter(x => x !== 'none').map(Number) : [1, 15, 39])) {
      const r = await play2(n, `lv${n}`);
      const good = r.started && r.green === 0 && r.end !== 'yer tutucu' && r.after && r.exc === 0 && /hazır/.test(r.log);
      ok = ok && good; if (!good || w === 375) notes.push(`${w}x${h} ${n}→${n + 1}: ${r.started ? `yeşil ${r.green}, son ${r.end} (${r.at.join('/')} ms), sonraki bölüm ${r.after ? 'açıldı' : 'YOK'}` : 'koridor BAŞLAMADI'}, istisna ${r.exc}`);
    }
  }
  W = 375; H = 667;
  for (const n of (opt('menus', '') ? opt('menus').split(',').filter(x => x !== 'none').map(Number) : [1, 15])) {   // PM_MESAJ_24B: win -> Ana menü
    const r = await play2(n, `lv${n}_menu`, false, true);
    const good = r.started && /menü/.test(r.log || '') && r.green === 0 && r.end !== 'yer tutucu' && r.dur >= 3.5 && r.exc === 0;
    ok = ok && good; notes.push(`${n}→menü: ${r.started ? `${r.log}, yeşil ${r.green}, süre ${r.dur} sn (erken dokunuş ${r.dur >= 3.5 ? 'atlatmadı' : 'ATLATTI'})` : 'koridor BAŞLAMADI'}, istisna ${r.exc}`);
  }
  const last = await play(40, 'lv40');
  ok = ok && !last.started && last.exc === 0; notes.push(`40: ${last.started ? 'koridor OYNADI (olmamalı)' : 'bitiş kartı, koridor yok'}`);
  const slow = await play2(1, 'lv1_yavas', true);
  ok = ok && slow.started && slow.green === 0 && /hazır/.test(slow.log); notes.push(`1→2 yavaş ağ: ${slow.log || '?'}, bölme 2 ${slow.end || '?'}`);
  check('corridor', ok, `${notes.join('; ')} → ${dir}`);
}

/* --only qa (PM_MESAJ_14): phone-size screenshots of the preview's screens at 375x667, 390x844 and 430x932 into
   --qa-out (default docs/qa/<today>). Debug pages (nothing is sent); the lose shot waits for S45's 60 s timer. */
async function qa() {
  const dir = opt('qa-out', path.join(ROOT, 'docs/qa', new Date().toISOString().slice(0, 10)));
  fs.mkdirSync(dir, { recursive: true });
  const set = JSON.parse(fs.readFileSync(path.join(ROOT, 'Assets/_Game/Levels/redesign.json'), 'utf8')).levels;
  const lv = id => set.find(l => l.id === id);
  const sizes = [[375, 667], [390, 844], [430, 932]];
  let n = 0, exc = 0;
  for (const [w, h] of sizes) {
    W = w; H = h;
    const shot = async (p, name) => { fs.writeFileSync(path.join(dir, `${w}x${h}_${name}.png`), await p.png()); n++; };
    const tapCard = async p => { for (let y = Math.round(H * .45); y < H * .9; y += 18) await p.tap(W / 2, y); await sleep(500); };
    let p = await newPage(); await p.open(dbg(`${base}/?set=redesign`)); await sleep(2500);
    await shot(p, '01_menu');
    await p.tap(W - 39, 38); await sleep(700); await shot(p, '08_ayarlar'); exc += p.exceptions.length; p.close();
    p = await newPage(); await p.open(dbg(`${base}/?set=redesign&lv=S28`)); await sleep(2500);
    await shot(p, '02_baslangic'); await tapCard(p); await shot(p, '03_oyun');
    await p.tap(W - 36, 33); await sleep(700); await shot(p, '04_pause'); await tapCard(p);
    await p.tap(39, 33); await sleep(700); await shot(p, '07_bolum_listesi'); exc += p.exceptions.length; p.close();
    p = await newPage(); await p.open(dbg(`${base}/?set=redesign&lv=S41`)); await sleep(2500);
    await p.solve(lv('S41')); await sleep(3000); await shot(p, '05_kazanma'); exc += p.exceptions.length; p.close();
    p = await newPage(); await p.open(dbg(`${base}/?set=redesign&lv=S45`)); await sleep(2500); await tapCard(p);
    const l = lv('S45'); await p.solve({ ...l, solution: l.solution.slice(0, 1) }); await sleep(64000);
    await shot(p, '06_kaybetme'); exc += p.exceptions.length; p.close();
  }
  check('qa', exc === 0, `${n} ekran görüntüsü → ${dir}, istisna ${exc}`);
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
    if (only.includes('qa')) await qa();
    if (only.includes('show')) await show();
    if (only.includes('paint40')) await paint40();
    if (only.includes('p19')) await p19();
    if (only.includes('p20')) await p20();
    if (only.includes('music')) await music();
    if (only.includes('chain')) await chain();
    if (only.includes('corridor')) await corridor();
    if (only.includes('p20menu')) await p20menu();
    if (only.includes('flick')) await flick();
    if (only.includes('perf')) await perf();
    if (only.includes('ftue')) await ftue();
    if (only.includes('painting')) await painting();
    if (only.includes('extra')) await extra();
    if (only.includes('funcard')) await funcard();
    if (send && only.includes('zeydsend')) await zeydsend();
    if (only.includes('zeydloss')) await zeydloss();
    if (send && only.includes('zeydlosssend')) await zeydlosssend();
    if (send && only.includes('redesignsend')) await redesignsend();
    if (send && only.includes('pausequit')) await pausequit();
    if (send && only.includes('cohortic')) await cohortic();
    if (send && only.includes('ic33')) await ic33();
    if (send && only.includes('rating')) await rating();
    if (send && only.includes('booster')) await booster();
    if (only.includes('spot')) await spot();
    if (only.includes("son")) await son();
    if (send && only.includes('replay')) await replay();
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
