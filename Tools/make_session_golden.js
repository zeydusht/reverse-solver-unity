// Golden session scenarios for the C# GameSession, produced by the web
// game's own code through Tools/web_harness.js.
//
// 1. Level scenarios (M2 criterion 1a): on each of the 40 levels, a simple
//    nail-aware player: every turn it takes the free, un-nailed pieces; if a
//    bomb is on the board it removes the bomb piece when it can, otherwise a
//    piece blocking it, otherwise the lowest index. Its moves are recorded
//    and every step's state is snapshotted.
// 2. Synthetic boards for the edge cases of the move order (criterion 2), the
//    safety valve (3), boosters (4) and endings (5).
//
// The C# tests replay each command through the real GameSession API and
// compare the snapshot after every step.
//
// usage: node Tools/make_session_golden.js
const fs = require('fs'), path = require('path');
const harness = require('./web_harness');
const DIRS = ['U', 'D', 'L', 'R'];

function snapshot(web, valve) {
  const g = web.g, s = g.state;
  const sorted = o => Object.fromEntries(Object.keys(o).map(Number).sort((a, b) => a - b).map(k => [k, o[k]]));
  const rec = s.over ? web.lastRecord() : null;
  return {
    present: s.present.size, nails: sorted(s.nails), bombs: sorted(s.bombs),
    time: s.time, moves: s.moves, valve,
    stock: g.BOOSTERS.map(b => b.count), used: g.BOOSTERS.map(b => s.used[b.id]),
    res: rec ? rec.res : '', stars: rec ? rec.st : 0,
    V: s.V.map(r => r.map(v => v ?? 0)), H: s.H.map(r => r.map(v => v ?? 0)),
  };
}

// Runs commands against the web game; returns per-step snapshots.
function run(web, commands) {
  const g = web.g, steps = [];
  for (const c of commands) {
    g.prompt.textContent = '';
    switch (c[0]) {
      case 'exit': web.exit(c[1], c[2]); break;
      case 'hammer': web.hammer(c[1]); break;
      case 'scissors': web.scissors(c[1], c[2], c[3]); break;
      case 'wand': web.wand(); break;
      case 'clock': web.clock(); break;
      case 'tick': web.tick(c[1]); break;
      default: throw new Error('unknown command ' + c[0]);
    }
    const valve = g.prompt.textContent === 'Çivi kendiliğinden söktü';
    steps.push({ cmd: c, after: snapshot(web, valve) });
  }
  return steps;
}

function nailAwarePlayer(web) {
  const g = web.g, cmds = [];
  while (!g.state.over && g.state.present.size) {
    const cand = g.freePieces().filter(p => g.nailAt(p) <= 0);
    if (!cand.length) break;
    let pick = cand[0];
    const bombs = Object.keys(g.state.bombs).map(Number).filter(b => g.state.present.has(b))
      .sort((a, b) => g.state.bombs[a] - g.state.bombs[b] || a - b);
    if (bombs.length) {
      const b = bombs[0];
      if (cand.includes(b)) pick = b;
      else {
        const blockers = DIRS.map(d => g.travelLimit(b, d).blocker).filter(x => x !== undefined && cand.includes(x));
        if (blockers.length) pick = blockers[0];
      }
    }
    const dir = DIRS.find(d => g.travelLimit(pick, d).exit);
    cmds.push(['exit', pick, dir]);
    web.exit(pick, dir);
  }
  return cmds;
}

// ---- synthetic boards -------------------------------------------------------

// A one-row board of single-cell pieces with flat seams unless given.
function row(id, n, extra = {}) {
  const pieces = [...Array(n)].map((_, x) => [[x, 0]]);
  const lv = Object.assign({
    id, version: 1, lv: 50, w: n, h: 1, timer: 60,
    boosters: { scissors: 0, wand: 0, hammer: 0, clock: 0 },
    unlock: { scissors: 0, wand: 0, hammer: 0, clock: 0 },
    bintro: null, intro: null, pieces,
    vEdge: [[null, ...Array(n - 1).fill(0), null]], hEdge: [Array(n).fill(null), Array(n).fill(null)],
    sealed: {}, nails: {}, bombs: {}, colors: Array(n).fill(0), palette: ['#888888', '#444444'],
    solution: [], load: 0, open: 0, art: 'test'
  }, extra);
  return lv;
}

// 2x2, every joint tabbed +1: nothing can move, and a wand that always draws
// below .5 keeps producing the same arrangement, so it must give up.
function lockedSquare(id) {
  return {
    id, version: 1, lv: 50, w: 2, h: 2, timer: 60,
    boosters: { scissors: 0, wand: 2, hammer: 0, clock: 0 },
    unlock: { scissors: 0, wand: 0, hammer: 0, clock: 0 },
    bintro: null, intro: null, pieces: [[[0, 0]], [[1, 0]], [[0, 1]], [[1, 1]]],
    vEdge: [[null, 1, null], [null, 1, null]], hEdge: [[null, null], [1, 1], [null, null]],
    sealed: {}, nails: {}, bombs: {}, colors: [0, 0, 0, 0], palette: ['#888888'],
    solution: [], load: 0, open: 0, art: 'test'
  };
}

const synthetic = [
  { name: 'valve pulls the longest-waiting nail; tie goes to the lowest index',
    level: row('S01', 3, { nails: { 1: 5, 2: 5 } }), commands: [['exit', 0, 'L']] },
  { name: 'nail frees and a fuse hits zero on the same move: bomb wins',
    level: row('S02', 3, { nails: { 1: 1 }, bombs: { 2: 1 } }), commands: [['exit', 0, 'L']] },
  { name: 'removing the bomb piece itself defuses it',
    level: row('S03', 2, { bombs: { 0: 1 } }), commands: [['exit', 0, 'L'], ['exit', 1, 'R']] },
  { name: 'last piece out while a fuse would reach zero: win',
    level: row('S04', 2, { bombs: { 1: 2 } }), commands: [['exit', 0, 'L'], ['exit', 1, 'R']] },
  { name: 'fuse reaches zero on a non-final move: bomb',
    level: row('S05', 3, { bombs: { 2: 2 } }), commands: [['exit', 0, 'L'], ['exit', 1, 'U']] },
  { name: 'clock runs out exactly at zero',
    level: row('S06', 2, { timer: 3 }), commands: [['tick', 2], ['tick', 1]] },
  { name: 'clock booster adds 20 s; win keeps the stars formula',
    level: row('S07', 2, { timer: 10, boosters: { scissors: 0, wand: 0, hammer: 0, clock: 2 } }),
    commands: [['tick', 6], ['clock'], ['tick', 3], ['exit', 0, 'L'], ['exit', 1, 'R']] },
  { name: 'hammer removes a nailed piece and a bomb piece, ticking like a move',
    level: row('S08', 3, { nails: { 1: 9 }, bombs: { 2: 5 }, boosters: { scissors: 0, wand: 0, hammer: 2, clock: 0 } }),
    commands: [['hammer', 1], ['hammer', 2], ['exit', 0, 'L']] },
  { name: 'stars: 2 with 20-45% of the timer left',
    level: row('S09', 1, { timer: 10 }), commands: [['tick', 6], ['exit', 0, 'L']] },
  { name: 'stars: 1 under 20% of the timer left',
    level: row('S10', 1, { timer: 10 }), commands: [['tick', 9], ['exit', 0, 'L']] },
  // Piece 4 at (1,1) can only leave downward; the tab of the vertical joint on
  // its left catches on the way. Cutting that joint frees it.
  { name: 'scissors flattens a joint and frees the piece behind it',
    level: Object.assign(row('S11', 3, { boosters: { scissors: 1, wand: 0, hammer: 0, clock: 0 } }),
      { w: 3, h: 2, pieces: [[[0, 0]], [[1, 0]], [[2, 0]], [[0, 1]], [[1, 1]], [[2, 1]]],
        vEdge: [[null, 0, 0, null], [null, 1, 0, null]], hEdge: [[null, null, null], [1, 1, 1], [null, null, null]],
        colors: [0, 0, 0, 0, 0, 0] }),
    commands: [['scissors', 'v', 1, 1], ['exit', 4, 'D']] },
];

// ---- generate -----------------------------------------------------------------

const out = { source: 'zeydusht/reverse-solver index.html via Tools/web_harness.js', dirs: DIRS, levels: [], synthetic: [], wand: [] };

{
  const web = harness(1);
  for (let i = 0; i < web.g.LEVELS.length; i++) {
    web.load(i);
    const cmds = nailAwarePlayer(web);
    web.load(i);
    const steps = run(web, cmds);
    out.levels.push({ id: 'L' + String(web.g.LEVEL.lv).padStart(2, '0'), steps });
  }
}

for (const sc of synthetic) {
  const web = harness(1);
  web.g.LEVELS.push(sc.level);
  web.load(web.g.LEVELS.length - 1);
  out.synthetic.push({ name: sc.name, seed: 1, level: sc.level, levelJson: JSON.stringify(sc.level), steps: run(web, sc.commands) });
}

// Wand on real levels where it is unlocked and stocked, several seeds; then
// play the board out with the nail-aware player.
for (const id of ['L06', 'L07', 'L10', 'L16', 'L31']) {
  for (const seed of [1, 2, 3]) {
    const web = harness(seed);
    const i = web.g.LEVELS.findIndex(l => 'L' + String(l.lv).padStart(2, '0') === id);
    web.load(i);
    const before = run(web, [['wand']]);
    const rest = nailAwarePlayer(web);
    const web2 = harness(seed);
    web2.load(i);
    out.wand.push({ id, seed, steps: run(web2, [['wand'], ...rest]) });
  }
}

// Wand that can never find a solvable arrangement: every draw is 0 -> +1.
{
  const web = harness(1, null, () => 0);
  const lv = lockedSquare('S12');
  web.g.LEVELS.push(lv);
  web.load(web.g.LEVELS.length - 1);
  out.synthetic.push({ name: 'wand gives up after 200 tries and spends nothing', seed: 'zero', level: lv, levelJson: JSON.stringify(lv),
                       steps: run(web, [['wand']]) });
}

const dst = path.join(__dirname, '..', 'Assets/_Game/Tests/EditMode/Golden/session_golden.json');
fs.writeFileSync(dst, JSON.stringify(out));
const res = {};
for (const l of out.levels) { const r = l.steps.length ? l.steps[l.steps.length - 1].after.res : 'none'; res[r] = (res[r] || 0) + 1; }
console.log('levels:', res, '| bombs at', out.levels.filter(l => l.steps.at(-1).after.res === 'bomb').map(l => l.id).join(','));
console.log('synthetic:', out.synthetic.map(s => `${s.level.id}=${s.steps.at(-1).after.res || 'open'}`).join(' '));
console.log('wand runs:', out.wand.length, '| size', fs.statSync(dst).size, 'bytes');
