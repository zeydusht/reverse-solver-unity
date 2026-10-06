// Runs the web game's whole script (../web-reference/index.html) under Node
// with an inert DOM, so its session logic (flyOut, tickNails, releaseValve,
// tickBombs, finish, boosters, clock) can be driven directly and used as the
// reference for the C# port.
//
//   const web = require('./web_harness')(seed);
//   web.load(levelIndex);        // loadLevel(i, true)
//   web.exit(piece, dir);        // a successful drag-out, as flyOut does
//   web.tick(n);                 // n clock seconds
//
// Math.random is replaced by mulberry32(seed), the same generator as the C#
// Mulberry32 test RNG, so wand reshuffles match draw for draw.
const fs = require('fs'), path = require('path');

function mulberry32(seed) {
  let a = seed >>> 0;
  return function () {
    a = (a + 0x6D2B79F5) >>> 0;
    let t = a;
    t = Math.imul(t ^ (t >>> 15), t | 1);
    t ^= t + Math.imul(t ^ (t >>> 7), t | 61);
    return ((t ^ (t >>> 14)) >>> 0) / 4294967296;
  };
}

// Inert DOM node: every property is another inert node, every call returns one.
const INERT = new Proxy(function () {}, {
  get: (t, k) => k === Symbol.toPrimitive ? () => '' : k === 'length' ? 0 : INERT,
  set: () => true,
  apply: () => INERT,
  construct: () => INERT,
});

// rng: optional replacement for Math.random (defaults to mulberry32(seed)).
module.exports = function load(seed = 1, src, rng) {
  src = src || path.join(__dirname, '..', '..', 'web-reference', 'index.html');
  const html = fs.readFileSync(src, 'utf8');
  const scripts = [...html.matchAll(/<script>([\s\S]*?)<\/script>/g)].map(m => m[1]);
  const main = scripts.find(s => s.includes('const LEVELS ='));
  if (!main) throw new Error('game script not found');

  const intervals = [];
  // The prompt line is the only DOM text the rules write that matters (the
  // safety valve announces itself there), so it is a real object.
  const promptEl = { textContent: '' };
  const document = new Proxy(INERT, {
    get: (t, k) => k === 'getElementById' ? (id => id === 'prompt' ? promptEl : INERT) : INERT,
  });
  const env = {
    window: { innerWidth: 400, innerHeight: 800, addEventListener() {}, SUPABASE_URL: '', SUPABASE_KEY: '' },
    document,
    navigator: {},
    location: { protocol: 'file:' },
    setInterval: fn => { intervals.push(fn); return intervals.length; },
    clearInterval: id => { if (id) intervals[id - 1] = null; },
    setTimeout: () => 0,
  };
  // The game draws from Math.random at startup (the telemetry session id), so
  // the generator is re-seeded on every load(): a level's first draw is then
  // the seed's first draw, as in a fresh C# GameSession.
  const fresh = () => rng || mulberry32(seed);
  let current = fresh();
  env.random = () => current();

  const body = main + `
    ;return { get state(){ return state; }, get LEVEL(){ return LEVEL; }, LEVELS, BOOSTERS, LOG,
      loadLevel, flyOut, useHammer, useWand, useClock, spend, finish, freePieces, solvable,
      travelLimit, nailAt, prompt };`;
  const fn = new Function('window', 'document', 'navigator', 'location', 'setInterval', 'clearInterval',
                          'setTimeout', 'Math', body);
  const MathSeeded = Object.create(Math, { random: { value: env.random } });
  const g = fn(env.window, env.document, env.navigator, env.location, env.setInterval, env.clearInterval,
               env.setTimeout, MathSeeded);

  const el = idx => ({ dataset: { i: String(idx) }, classList: INERT, style: {}, remove() {} });
  return {
    g,
    load(i) { intervals.length = 0; current = fresh(); g.loadLevel(i, true); },
    exit(piece, dir) {
      const lim = g.travelLimit(piece, dir);
      if (!lim.exit) throw new Error(`piece ${piece} ${dir} cannot exit`);
      g.flyOut(el(piece), piece, dir, lim.cells);
    },
    hammer(piece) { g.useHammer(el(piece)); },
    wand() { g.useWand(g.BOOSTERS[1]); },
    clock() { g.useClock(g.BOOSTERS[3]); },
    scissors(kind, x, y) {                     // as addJoint's click handler
      if (kind === 'v') g.state.V[y][x] = 0; else g.state.H[y][x] = 0;
      g.spend(g.BOOSTERS[0]);
    },
    tick(n = 1) {
      for (let s = 0; s < n; s++) for (const f of intervals) if (f) f();
    },
    lastRecord() { const r = g.LOG.rows; return r[r.length - 1]; },
  };
};

module.exports.mulberry32 = mulberry32;
