// Golden values for the C# port of the travel rule, produced by running the
// web game's own rule code (edgeOf, catches, travelLimit, freePieces,
// solvable) from ../web-reference/index.html under Node.
//
// For each level, at three points along the recorded solution (start, 1/3,
// 2/3), every present piece is probed in all four directions.
//
// usage: node Tools/make_travel_golden.js [path/to/index.html]
const fs = require('fs'), path = require('path');
const root = path.resolve(__dirname, '..');
const src = process.argv[2] || path.join(root, '..', 'web-reference', 'index.html');
const html = fs.readFileSync(src, 'utf8');

const levelsLine = /^const LEVELS = (\[.*\]);\s*$/m.exec(html)[1];
const start = html.indexOf('let LEVEL = LEVELS[0];');
const end = html.indexOf('/* ------------------------------------------------------------ piece art */');
if (start < 0 || end < 0) throw new Error('rule code markers not found');

// The rule block only touches the DOM to grab two elements at load time.
const api = new Function('LEVELS', 'document', html.slice(start, end) + `
  return {
    load(i){ LEVEL = LEVELS[i];
      state.present = new Set(LEVEL.pieces.map((_, k) => k));
      state.V = JSON.parse(JSON.stringify(LEVEL.vEdge));
      state.H = JSON.parse(JSON.stringify(LEVEL.hEdge)); },
    remove(i){ state.present.delete(i); },
    present(){ return [...state.present]; },
    travelLimit, freePieces, solvable, exitDirs, occupancy
  };`)(JSON.parse(levelsLine), { getElementById: () => null });

const DIRS = ['U', 'D', 'L', 'R'];             // web iteration order, also C# Dir order
const LEVELS = JSON.parse(levelsLine);
const out = { source: 'zeydusht/reverse-solver index.html', dirs: DIRS, levels: [] };

for (let i = 0; i < LEVELS.length; i++) {
  const L = LEVELS[i], sol = L.solution, n = sol.length;
  api.load(i);
  const rec = { id: 'L' + String(L.lv).padStart(2, '0'), solvable: api.solvable(),
                free0: api.freePieces().sort((a, b) => a - b), greedyOrder: [], states: [] };

  // Greedy order, using the web loop: first present piece (ascending) with any exit.
  { const p = new Set(api.present());
    while (p.size) {
      let moved = false;
      for (const k of p) {
        api.load(i); for (let j = 0; j < L.pieces.length; j++) if (!p.has(j)) api.remove(j);
        if (api.exitDirs(k, api.occupancy()).length) { rec.greedyOrder.push(k); p.delete(k); moved = true; break; }
      }
      if (!moved) break;
    } }

  const probeAt = new Set([0, Math.floor(n / 3), Math.floor(2 * n / 3)]);
  api.load(i);
  for (let step = 0; step <= n; step++) {
    if (probeAt.has(step)) {
      const limits = [];
      for (const k of api.present().sort((a, b) => a - b))
        DIRS.forEach((d, di) => {
          const r = api.travelLimit(k, d);
          limits.push([k, di, r.cells, r.exit ? 1 : 0, r.blocker === undefined ? -1 : r.blocker, r.hard ? 1 : 0]);
        });
      rec.states.push({ step, limits });
    }
    if (step < n) api.remove(sol[step].piece);
  }
  out.levels.push(rec);
}

const dst = path.join(root, 'Assets/_Game/Tests/EditMode/Golden/travel_golden.json');
fs.writeFileSync(dst, JSON.stringify(out));
const probes = out.levels.reduce((a, l) => a + l.states.reduce((b, s) => b + s.limits.length, 0), 0);
console.log(`${out.levels.length} levels, ${probes} probes, solvable ${out.levels.filter(l => l.solvable).length}/40 -> ${path.relative(root, dst)} (${fs.statSync(dst).size} bytes)`);
