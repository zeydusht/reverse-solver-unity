// PM_107 §C5: the mix's scenario recordings. Records the game's own output (Unity's WebAudio and the music's, apart; the
// recorder taps each context where it reaches the speakers: after the limiter) with every [mix] / [ses] / [gecis] /
// [booster ses] log line on the recording's clock; then Tools/pm107_miks_olc.py measures it (drowned sounds, peaks).
//   --scenario bolum    a level of single pieces: 10 exits and 3 jams between them
//              saat     lv 18 with the clock booster (&booster=saat): the tick-tock running, 6 exits over it
//              zincir   lv 31 (chained pairs): its solution's first moves, the pairs sliding and knocking
//              bomba    lv 15: every move but the bomb's piece, until the bomb goes off (the bang, the loss)
//              kazanma  level 4 won by its solution: the fanfare and the win screen (the chest's own screen is not reached this way): the fanfare, its events, the chest
//              menu     the menu: the four tabs and back, Ayarlar open and closed, a painting's detail
//              dalis    the menu's Play: the dive into the painting (its air from the start of the motion)
//   --before            the old mix (&miks=0&limiter=0: the original files, no ducking, no limiter)
// usage: node Tools/pm107_miks.js <out dir> --scenario <name> [--before] [--local Builds/publish/br] [--port 9361]
const fs = require('fs'), path = require('path'), http = require('http'), os = require('os'), { spawn } = require('child_process');
const args = process.argv.slice(2);
const opt = (n, d) => { const i = args.indexOf('--' + n); return i >= 0 ? args[i + 1] : d; };
const outDir = path.resolve(args[0]);
const ROOT = path.resolve(__dirname, '..');
const W = 390, H = 844, PORT = +opt('port', 9361), SC = opt('scenario', 'bolum'), BEFORE = args.includes('--before');
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
  const server = await serve(path.resolve(ROOT, opt('local', 'Builds/publish/br')));
  const base = `http://127.0.0.1:${server.address().port}`;
  const profile = fs.mkdtempSync(path.join(os.tmpdir(), 'rs-miks-'));
  const chrome = spawn(CHROME, ['--headless=new', `--remote-debugging-port=${PORT}`, '--use-angle=d3d11', '--enable-gpu', '--ignore-gpu-blocklist',
    '--autoplay-policy=no-user-gesture-required', `--user-data-dir=${profile}`, '--no-first-run', 'about:blank'], { stdio: 'ignore' });
  const done = async code => { try { chrome.kill(); } catch { } server.close(); await sleep(500); try { fs.rmSync(profile, { recursive: true, force: true }); } catch { } process.exit(code); };
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
      if (/^\[(mix|ses|gecis|booster ses)\]/.test(line)) logs.push({ at: m.params.timestamp, line });
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
  const tap = async (x, y) => { await touch('touchStart', x, y); await sleep(50); await touch('touchEnd'); };
  await cdp('Runtime.enable'); await cdp('Page.enable'); await cdp('Network.enable');
  await cdp('Network.setBlockedURLs', { urls: ['*supabase.co*'] });
  await cdp('Emulation.setDeviceMetricsOverride', { width: W, height: H, deviceScaleFactor: 2, mobile: true });
  await cdp('Emulation.setTouchEmulationEnabled', { enabled: true, maxTouchPoints: 5 });
  await cdp('Page.addScriptToEvaluateOnNewDocument', { source: TAP });
  const LV = { bolum: 4, saat: 18, zincir: 31, bomba: 15, kazanma: 4, menu: 0, dalis: 0 }[SC];   // 18 is the clock's lesson (no move before the clock)
  const extra = { saat: '&booster=saat' }[SC] || '';
  const q = `c=p23&lang=tr&intro=0&seslog=1&test=1${LV ? `&lv=${LV}` : ''}${extra}${BEFORE ? '&miks=0&limiter=0' : ''}${opt('query', '')}`;
  await cdp('Page.navigate', { url: `${base}/?${q}` });
  for (let i = 0; i < 120; i++) { await sleep(500); if (await ev(`!!(window.LT && LT.end)`)) break; }
  // recording from the moment the game is up (the win of &sandik=1 comes with the level)
  const sr = await ev('__pm100.start()');
  const epoch = await ev('__pm100.epoch');
  console.log(`[kayıt] ${SC}${BEFORE ? ' (önce)' : ''}: başladı (${sr} Hz)`);
  {
    await sleep(5000);
    await tap(12, 430); await sleep(1500);   // the first touch: the audio starts
    if (SC !== 'menu') { await tap(12, 700); await sleep(1200); await tap(W / 2, 708); await sleep(1200); }   // a card, if any (its button)
    await sleep(2000);
  }
  const l = LV ? sl100.find(x => x.lv === LV) : null, p = l ? hudPlacement(l) : null;
  const gone = new Set();
  const at = new Map(); if (l) l.pieces.forEach((cells, i) => cells.forEach(c => at.set(c[0] + ',' + c[1], i)));
  const ahead = (x, y) => (x < 0 || y < 0 || x >= l.w || y >= l.h) ? -2 : (at.has(x + ',' + y) && !gone.has(at.get(x + ',' + y)) ? at.get(x + ',' + y) : -1);
  const move = async (piece, dir, what) => {
    const c = l.pieces[piece][0], [ux, uy] = DIR[dir];
    const x0 = p.x + (c[0] + .5) * p.cell, y0 = p.y + (c[1] + .5) * p.cell;
    const room = what === 'jam' ? p.cell * .45 : ux < 0 ? x0 - 1 : ux > 0 ? W - 1 - x0 : uy < 0 ? y0 - 1 : H - 1 - y0;
    await drag(x0, y0, ux * room, uy * room, what === 'exit' ? 140 : 60);
    await sleep(what === 'exit' ? 650 : 900);
  };
  const jamOf = () => {
    for (let i = 0; i < l.pieces.length; i++) {
      if (gone.has(i) || l.pieces[i].length !== 1) continue;
      for (const [d, [ux, uy]] of Object.entries(DIR)) {
        let [x, y] = l.pieces[i][0]; x += ux; y += uy;
        if (ahead(x, y) >= 0) return [i, d];
      }
    }
    return null;
  };
  if (SC === 'bolum') {
    let k = 0;
    for (const m of l.solution) {
      if (k >= 10) break;
      await move(m.piece, m.dir, 'exit'); gone.add(m.piece); k++;
      if (k === 3 || k === 6 || k === 9) { const j = jamOf(); if (j) await move(j[0], j[1], 'jam'); }
    }
    await sleep(1200);
  } else if (SC === 'saat') {
    // the flag's freeze is over by now: the clock's button in the bar (4th), pressed while recording
    await tap(W * .692, H * .93); await sleep(900);
    if (!logs.some(x => /tiktak/.test(x.line) && x.at > epoch)) { await tap(W / 2, p.y + p.cell * l.h / 2); await sleep(700); }
    let k = 0;
    for (const m of l.solution) { if (k >= 6) break; if (l.bombs && l.bombs[m.piece]) continue; await move(m.piece, m.dir, 'exit'); gone.add(m.piece); k++; }
    await sleep(2000);
  } else if (SC === 'zincir') {
    let k = 0;
    for (const m of l.solution) { if (k >= 8) break; await move(m.piece, m.dir, 'exit'); gone.add(m.piece); k++; const j = k % 3 === 0 ? jamOf() : null; if (j) await move(j[0], j[1], 'jam'); }
    await sleep(1500);
  } else if (SC === 'bomba') {
    const bomb = Object.keys(l.bombs || {}).map(Number);
    for (const m of l.solution) {
      if (bomb.includes(m.piece)) continue;
      await move(m.piece, m.dir, 'exit'); gone.add(m.piece);
      if (logs.some(x => /Patlama/.test(x.line))) break;
    }
    await sleep(4500);
  } else if (SC === 'kazanma') {
    for (const m of l.solution) { await move(m.piece, m.dir, 'exit'); gone.add(m.piece); }
    await sleep(12000);
    for (let y = Math.round(H * .55); y < H * .9; y += 40) { await tap(W / 2, y); await sleep(700); }   // the chest, its buttons
    await sleep(3000);
  } else if (SC === 'dalis') {
    logs.push({ at: Date.now(), line: '[mix] olay Oyna' });
    await tap(W / 2, H * .79); await sleep(9000);
  } else if (SC === 'menu') {
    for (const x of [117, 195, 273, 341, 49]) { await tap(x, 810); await sleep(1300); }
    await tap(355, 52); await sleep(1300);   // Ayarlar (top right): the language there and back (not the story: its film has its own sound)
    await tap(241, 486); await sleep(900); await tap(149, 486); await sleep(900);
    await tap(354, 174); await sleep(1200);   // its X
    await tap(117, 810); await sleep(1200); await tap(W / 2, 400); await sleep(1600); await tap(30, 60); await sleep(1200); await tap(49, 810); await sleep(1500);
  }
  const b64 = sr > 0 ? await ev('__pm100.stop()', true) : '';
  const m64 = sr > 0 ? await ev('__pm100.stopMusic()', true) : '';
  fs.writeFileSync(path.join(outDir, 'efektler.webm'), Buffer.from(b64 || '', 'base64'));
  if (m64) fs.writeFileSync(path.join(outDir, 'muzik.webm'), Buffer.from(m64, 'base64'));
  fs.writeFileSync(path.join(outDir, 'meta.json'), JSON.stringify({ scenario: SC, before: BEFORE, musicStart: (await ev('__pm100.musicStart')) || 0, musicGains: await ev('(function(){var M=window.rsMusic;if(!M||!M.ctx)return null;var T=M.tracks||{};return {gain:M.gain&&M.gain.gain.value,mix:M.mix?M.mix.gain.value:null,panel:M.panel&&M.panel.gain.value,volume:M.volume,on:M.on,fan:M.fan&&M.fan.gain?M.fan.gain.gain.value:null,lobbyDuck:T.lobby&&T.lobby.duck?T.lobby.duck.gain.value:null,levelDuck:T.level&&T.level.duck?T.level.duck.gain.value:null,voices:(M.voices||[]).length}})()') }));
  if (!m64) { try { fs.unlinkSync(path.join(outDir, 'muzik.webm')); } catch { } }
  fs.writeFileSync(path.join(outDir, 'log.json'), JSON.stringify(logs.map(x => ({ t: (x.at - epoch) / 1000, line: x.line })), null, 1));
  const shot = await cdp('Page.captureScreenshot', { format: 'png' }); fs.writeFileSync(path.join(outDir, 'son.png'), Buffer.from(shot.result.data, 'base64'));
  console.log(`[kayıt] ${SC}: efekt ${((b64 || '').length * .75 / 1e3).toFixed(0)} KB, müzik ${((m64 || '').length * .75 / 1e3).toFixed(0)} KB, ${logs.filter(x => /^\[mix\]/.test(x.line)).length} [mix] satırı`);
  ws.close(); await done(0);
})().catch(e => { console.error(e); process.exit(1); });
