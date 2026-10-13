using System;
using System.Collections.Generic;
using System.Linq;
using ReverseSolver.Core;
using ReverseSolver.Editing;

namespace ReverseSolver.Generation
{
    /* Search knobs the factory tunes per slot (G2 searches over these).
         Interlock  how strongly the peel prefers the piece that needs the fewest
                    seams flattened: high keeps more seams locked (more reading)
         EdgeFirst  preference for pieces already at the board's edge
         NailLate   0..1, where in the removal order nailed pieces sit (1 = late) */
    public sealed class GenParams
    {
        public double Interlock = 1.0;
        public double EdgeFirst = 0.0;
        public double NailLate = 0.5;

        public GenParams Clone() => (GenParams)MemberwiseClone();
        public override string ToString() => $"interlock {Interlock:0.00} edge {EdgeFirst:0.00} nailLate {NailLate:0.00}";
    }

    public sealed class Candidate
    {
        public string Id;
        public uint Seed;
        public SlotSpec Spec;
        public GenParams Params;
        public LevelDraft Draft;
        public LevelData Level;
        public int Flattened;          // seams opened by the peel
        public int Locked;             // inner seams still interlocked

        public override string ToString() => $"{Id} {Level.Width}x{Level.Height} pieces {Level.Pieces.Count} open {Level.Open}%";
    }

    /* G1: builds solvable boards by peeling a full board (PRODUCT.md "Faz 4"):
       every seam starts interlocked; pieces are taken out one by one along an
       empty corridor to the edge, and only the seams that would catch on the
       way are flattened. The removal order is the level's solution.

       Flattening only ever frees pieces (a flat seam never catches), so a move
       that was legal when it was chosen stays legal after later seams open:
       the recorded order solves the final board. Each candidate is still
       replayed by the game's own rules and checked with DesignCheck (0 bombs,
       0 freezes, 0 safety valves) before it is returned.

       Deterministic: the same spec, parameters and seed give the same board. */
    public static class Generator
    {
        const int Tab = 1, Socket = -1, Flat = 0;

        /* Up to `tries` seeds after `seed` until one passes; null if none does. */
        public static Candidate Make(SlotSpec spec, GenParams p, uint seed, int tries = 20, Catalog catalog = null)
        {
            for (int t = 0; t < tries; t++)
            {
                var c = TryMake(spec, p, seed + (uint)t * 7919u, catalog, out _);
                if (c != null) return c;
            }
            return null;
        }

        /* One attempt. `why` says what failed (for the speed/failure report). */
        public static Candidate TryMake(SlotSpec spec, GenParams p, uint seed, Catalog catalog, out string why)
        {
            var rng = new Mulberry32(seed == 0 ? 1u : seed);
            var b = new Build(spec, p, rng);
            if (!b.Pieces(out why)) return null;
            if (!b.Peel(out why)) return null;
            var c = b.ToCandidate(seed, catalog, out why);
            return c;
        }

        // ---- the build ---------------------------------------------------------------------

        sealed class Build
        {
            readonly SlotSpec _spec;
            readonly GenParams _p;
            readonly IRandom _rng;
            int _w, _h;
            int[,] _v, _hj;                                    // same layout as LevelData: V[y, x] x 0..W, H[y, x] y 0..H
            readonly List<Cell[]> _pieces = new List<Cell[]>();
            int[] _owner;
            bool[] _present;
            readonly Dictionary<Dir, HashSet<int>> _sealed = new Dictionary<Dir, HashSet<int>>();
            readonly List<(int piece, Dir dir)> _order = new List<(int, Dir)>();
            int _flattened;

            public Build(SlotSpec spec, GenParams p, IRandom rng) { _spec = spec; _p = p; _rng = rng; }

            int Next(int lo, int hiInclusive) => lo + (int)(_rng.NextDouble() * (hiInclusive - lo + 1));
            bool On(Cell c) => c.X >= 0 && c.Y >= 0 && c.X < _w && c.Y < _h;
            int Owner(Cell c) => _owner[c.Y * _w + c.X];

            /* Board, chains, walls; every inner seam interlocked with a random owner. */
            public bool Pieces(out string why)
            {
                why = null;
                int chains = Next(_spec.MinChains, _spec.MaxChains);
                int remotes = Math.Min(chains, Next(_spec.MinRemote, _spec.MaxRemote));
                if (remotes < _spec.MinRemote) { why = "chains fewer than the remote minimum"; return false; }
                var boards = _spec.Boards.Where(bd => bd.w * bd.h - chains >= _spec.MinBodies && bd.w * bd.h - chains <= _spec.MaxBodies).ToArray();
                if (boards.Length == 0) { why = "no board fits the body range"; return false; }
                (_w, _h) = boards[Next(0, boards.Length - 1)];
                _v = new int[_h, _w + 1];
                _hj = new int[_h + 1, _w];
                for (int y = 0; y < _h; y++) for (int x = 1; x < _w; x++) _v[y, x] = _rng.NextDouble() < .5 ? 1 : -1;
                for (int y = 1; y < _h; y++) for (int x = 0; x < _w; x++) _hj[y, x] = _rng.NextDouble() < .5 ? 1 : -1;

                var used = new bool[_w * _h];
                var cells = new List<Cell>();
                for (int y = 0; y < _h; y++) for (int x = 0; x < _w; x++) cells.Add(new Cell(x, y));
                for (int i = 0; i < chains; i++)
                {
                    bool remote = i < remotes;
                    Cell a = default, bc = default; bool ok = false;
                    for (int t = 0; t < 50 && !ok; t++)
                    {
                        a = cells[Next(0, cells.Count - 1)];
                        if (used[a.Y * _w + a.X]) continue;
                        if (remote)
                        {
                            // same row or column, 2-3 cells apart: the pair slides as one body
                            bool row = _rng.NextDouble() < .5; int gap = Next(2, 3) * (_rng.NextDouble() < .5 ? -1 : 1);
                            bc = row ? new Cell(a.X + gap, a.Y) : new Cell(a.X, a.Y + gap);
                        }
                        else bc = a.Step(DirExt.All[Next(0, 3)]);
                        ok = On(bc) && !used[bc.Y * _w + bc.X];
                    }
                    if (!ok) { why = "no room for a chain"; return false; }
                    used[a.Y * _w + a.X] = used[bc.Y * _w + bc.X] = true;
                    if (a.Y * _w + a.X > bc.Y * _w + bc.X) (a, bc) = (bc, a);
                    _pieces.Add(new[] { a, bc });
                    if (Math.Abs(a.X - bc.X) + Math.Abs(a.Y - bc.Y) == 1) SetJointBetween(a, bc, 0);
                }
                foreach (var c in cells) if (!used[c.Y * _w + c.X]) _pieces.Add(new[] { c });
                // stable order: by the anchor cell, like LevelDraft
                _pieces.Sort((x, y) => (x[0].Y * _w + x[0].X).CompareTo(y[0].Y * _w + y[0].X));
                _owner = new int[_w * _h];
                for (int i = 0; i < _pieces.Count; i++) foreach (var c in _pieces[i]) _owner[c.Y * _w + c.X] = i;
                _present = Enumerable.Repeat(true, _pieces.Count).ToArray();

                int seals = Next(_spec.MinSeals, _spec.MaxSeals);
                for (int i = 0, t = 0; i < seals && t < 100; t++)
                {
                    var side = DirExt.All[Next(0, 3)];
                    int lane = Next(0, (side.IsVertical() ? _w : _h) - 1);
                    if (!_sealed.TryGetValue(side, out var set)) _sealed[side] = set = new HashSet<int>();
                    if (set.Add(lane)) i++;
                }
                return true;
            }

            // ---- seams --------------------------------------------------------------------

            /* The joint on `side` of cell c: (vertical?, x, y) in LevelData indexing. */
            static (bool v, int x, int y) JointOf(Cell c, Dir side) => side switch
            {
                Dir.L => (true, c.X, c.Y),
                Dir.R => (true, c.X + 1, c.Y),
                Dir.U => (false, c.X, c.Y),
                _ => (false, c.X, c.Y + 1),
            };

            int Get((bool v, int x, int y) j) => j.v ? _v[j.y, j.x] : _hj[j.y, j.x];
            void Set((bool v, int x, int y) j, int value) { if (j.v) _v[j.y, j.x] = value; else _hj[j.y, j.x] = value; }

            void SetJointBetween(Cell a, Cell b, int value)
            {
                foreach (var d in DirExt.All) if (a.Step(d).Equals(b)) { Set(JointOf(a, d), value); return; }
            }

            /* TravelRule.EdgeOf on the build's joints. */
            int EdgeOf(Cell c, Dir side)
            {
                bool rim = side == Dir.L ? c.X == 0 : side == Dir.R ? c.X == _w - 1 : side == Dir.U ? c.Y == 0 : c.Y == _h - 1;
                if (rim) return Flat;
                int v = Get(JointOf(c, side));
                if (v == 0) return Flat;
                if (side == Dir.L || side == Dir.U) return v == -1 ? Tab : Socket;
                return v == 1 ? Tab : Socket;
            }

            /* Can `piece` leave towards `dir` once the listed seams are flattened?
               Mirrors TravelRule.Limit: corridor empty, no wall in the lane, and at
               every position (home included) a present side neighbour must not catch. */
            bool Plan(int piece, Dir dir, HashSet<(bool, int, int)> flatten)
            {
                var cells = _pieces[piece];
                if (_sealed.TryGetValue(dir, out var lanes))
                    foreach (var c in cells) if (lanes.Contains(dir.IsVertical() ? c.X : c.Y)) return false;
                foreach (var c in cells)
                    for (var n = c.Step(dir); On(n); n = n.Step(dir))
                    {
                        int o = Owner(n);
                        if (o != piece && _present[o]) return false;
                    }
                Dir perpA = dir.IsVertical() ? Dir.L : Dir.U, perpB = dir.IsVertical() ? Dir.R : Dir.D;
                var at = new Cell[cells.Length];
                for (int k = 0; ; k++)
                {
                    bool allOff = true;
                    for (int i = 0; i < cells.Length; i++) { at[i] = cells[i].Step(dir, k); allOff &= !On(at[i]); }
                    if (allOff) return true;
                    for (int i = 0; i < cells.Length; i++)
                    {
                        if (!On(at[i])) continue;
                        foreach (var side in new[] { perpA, perpB })
                        {
                            var n = at[i].Step(side);
                            if (!On(n) || Array.IndexOf(at, n) >= 0) continue;
                            int o = Owner(n);
                            if (o == piece || !_present[o]) continue;
                            if (EdgeOf(cells[i], side) == Tab) flatten.Add(JointOf(cells[i], side));
                            if (EdgeOf(n, side.Opposite()) == Tab) flatten.Add(JointOf(n, side.Opposite()));
                        }
                    }
                }
            }

            /* Take the pieces out one by one; the order is the solution. */
            public bool Peel(out string why)
            {
                why = null;
                var options = new List<(int piece, Dir dir, HashSet<(bool, int, int)> flat, double weight)>();
                for (int left = _pieces.Count; left > 0; left--)
                {
                    options.Clear();
                    for (int p = 0; p < _pieces.Count; p++)
                    {
                        if (!_present[p]) continue;
                        foreach (var d in DirExt.All)
                        {
                            var flat = new HashSet<(bool, int, int)>();
                            if (!Plan(p, d, flat)) continue;
                            double w = Math.Exp(-_p.Interlock * flat.Count);
                            if (_p.EdgeFirst != 0 && TouchesEdge(p)) w *= Math.Exp(_p.EdgeFirst);
                            options.Add((p, d, flat, w));
                        }
                    }
                    if (options.Count == 0) { why = "peel stuck"; return false; }
                    double total = options.Sum(o => o.weight), r = _rng.NextDouble() * total;
                    var pick = options[options.Count - 1];
                    foreach (var o in options) { r -= o.weight; if (r <= 0) { pick = o; break; } }
                    foreach (var j in pick.flat) { if (Get(j) != 0) _flattened++; Set(j, 0); }
                    _present[pick.piece] = false;
                    _order.Add((pick.piece, pick.dir));
                }
                return true;
            }

            bool TouchesEdge(int p) => _pieces[p].Any(c => c.X == 0 || c.Y == 0 || c.X == _w - 1 || c.Y == _h - 1);

            // ---- obstacles, the level, the checks ------------------------------------------------

            public Candidate ToCandidate(uint seed, Catalog catalog, out string why)
            {
                why = null;
                var step = new int[_pieces.Count];                     // moves made before this piece leaves
                for (int i = 0; i < _order.Count; i++) step[_order[i].piece] = i;
                int bodies = _pieces.Count;

                var d = LevelDraft.New(_w, _h);
                d.SetNumber(_spec.Slot);
                for (int y = 0; y < _h; y++) for (int x = 1; x < _w; x++) d.SetJoint(x, y, true, _v[y, x]);
                for (int y = 1; y < _h; y++) for (int x = 0; x < _w; x++) d.SetJoint(x, y, false, _hj[y, x]);
                foreach (var pc in _pieces) if (pc.Length == 2) d.AddChain(pc[0], pc[1]);
                foreach (var kv in _sealed) foreach (int lane in kv.Value) d.SetSealed(kv.Key, lane, true);

                // bombs: a fuse that the recorded order beats by the genre's margin (G2 recalibrates
                // it from the shortest dig); never on the first piece out
                int bombs = Next(_spec.MinBombs, _spec.MaxBombs);
                var bombed = new HashSet<int>();
                var bombable = Enumerable.Range(0, bodies).Where(p => step[p] >= 1).ToList();
                for (int i = 0; i < bombs; i++)
                {
                    if (bombable.Count == 0) { why = "no piece for a bomb"; return null; }
                    int p = bombable[Next(0, bombable.Count - 1)];
                    bombable.Remove(p);
                    bombed.Add(p);
                    d.SetBomb(_pieces[p][0], step[p] + 1 + DifficultyModel.Of(_spec.Genre).BombMargin);
                }

                // nails: the genre's counter (≤ 40 % of the bodies) on single pieces that leave
                // no earlier than the counter allows; NailLate picks how late
                int nails = Next(_spec.MinNails, _spec.MaxNails);
                var nailedAt = new List<int>();
                for (int i = 0; i < nails; i++)
                {
                    int counter = DifficultyModel.NailCounter(bodies, _spec.Genre, i);
                    var ok = Enumerable.Range(0, bodies)
                        .Where(p => _pieces[p].Length == 1 && !bombed.Contains(p) && !nailedAt.Contains(p) && step[p] >= counter)
                        .OrderBy(p => step[p]).ToList();
                    if (ok.Count == 0) { why = "no piece late enough for a nail"; return null; }
                    int lo = (int)Math.Floor(_p.NailLate * (ok.Count - 1) * .8), hi = Math.Min(ok.Count - 1, lo + Math.Max(1, ok.Count / 5));
                    int p = ok[Next(lo, hi)];
                    nailedAt.Add(p);
                    d.SetNail(_pieces[p][0], counter);
                }

                d.SetTimer(60);
                ApplyLook(d, catalog);

                // the draft numbers pieces the same way (by anchor cell); map our order onto it
                var level = d.ToLevelData();
                var index = new Dictionary<Cell, int>();
                for (int i = 0; i < level.Pieces.Count; i++) index[level.Pieces[i][0]] = i;
                if (level.Pieces.Count != bodies) { why = "piece count mismatch"; return null; }
                d.Solution = _order.Select(o => new Move(index[_pieces[o.piece][0]], o.dir)).ToList();
                d.Id = $"C{_spec.Slot:00}-{seed:x8}";
                level = d.ToLevelData();

                if (!Replays(level)) { why = "recorded order does not replay"; return null; }
                var check = DesignCheck.Run(level, DesignCheck.QuickBudget * 5);
                if (!check.FollowsRule) { why = "design check: " + check.Verdict; return null; }
                if (!check.Fits) { why = "cells under 44 pt"; return null; }

                int locked = 0;
                for (int y = 0; y < _h; y++) for (int x = 1; x < _w; x++) if (_v[y, x] != 0) locked++;
                for (int y = 1; y < _h; y++) for (int x = 0; x < _w; x++) if (_hj[y, x] != 0) locked++;
                return new Candidate { Id = d.Id, Seed = seed, Spec = _spec, Params = _p.Clone(), Draft = d, Level = level, Flattened = _flattened, Locked = locked };
            }

            /* The recorded order wins under the game's rules with no safety valve. */
            static bool Replays(LevelData level)
            {
                var s = new GameSession(level, 1, "generator", new Mulberry32(1));
                foreach (var m in level.Solution)
                    if (s.TryExit(m.Piece, m.Dir) != CommandResult.Ok) return false;
                return s.Outcome == Outcome.Win && s.ValveCount == 0;
            }

            /* Two-colour picture (the web levels' style) and the slot's intro cards. */
            void ApplyLook(LevelDraft d, Catalog catalog)
            {
                var pal = Catalog.Palettes[Next(0, Catalog.Palettes.Length - 1)];
                d.SetPaletteColour(0, pal.a);
                d.SetPaletteColour(1, pal.b);
                int pattern = Next(0, 3);
                for (int y = 0; y < _h; y++)
                    for (int x = 0; x < _w; x++)
                    {
                        int mx = Math.Min(x, _w - 1 - x), my = Math.Min(y, _h - 1 - y);
                        int ink = pattern switch
                        {
                            0 => (mx + my) % 2,                               // rings
                            1 => Math.Abs(x - (_w - 1) / 2.0) + Math.Abs(y - (_h - 1) / 2.0) <= Math.Min(_w, _h) / 2.0 ? 1 : 0,   // diamond
                            2 => y < _h / 2 ? 0 : 1,                           // horizon
                            _ => (x + y) % 3 == 0 ? 1 : 0,                     // stripes
                        };
                        d.SetColor(new Cell(x, y), ink);
                    }
                d.Art = "Fabrika";
                foreach (var id in LevelDraft.BoosterIds)
                {
                    int at = SlotSpec.IntroducedAt[id];
                    d.SetUnlock(id, at);
                    d.SetBooster(id, _spec.Slot >= at ? Catalog.Stock[id] : 0);
                }
                if (catalog == null || _spec.NewThing == null) return;
                if (SlotSpec.IsObstacle(_spec.NewThing)) d.Intro = catalog.IntroFor(_spec.NewThing);
                else d.BoosterIntro = catalog.IntroFor(_spec.NewThing);
            }
        }
    }

    /* Look data borrowed from the web levels: palettes, booster stock, intro cards. */
    public sealed class Catalog
    {
        public static readonly (string a, string b)[] Palettes =
        {
            ("#ef5342", "#4a93c4"), ("#4fd0b0", "#4a5f8a"), ("#f0c04a", "#7d6bc0"), ("#e34a3c", "#42607a"), ("#e05a3f", "#33566b")
        };
        /* The web game's stock once a booster is open (late web levels). */
        public static readonly IReadOnlyDictionary<string, int> Stock = new Dictionary<string, int>
        {
            ["scissors"] = 5, ["wand"] = 3, ["hammer"] = 7, ["clock"] = 10
        };

        readonly Dictionary<string, LevelIntro> _intros = new Dictionary<string, LevelIntro>();

        /* Intro cards of the web levels: obstacles by icon, boosters by id. */
        public static Catalog From(LevelSet web)
        {
            var c = new Catalog();
            foreach (var l in web.Levels)
            {
                if (l.Intro != null)
                {
                    string key = l.Intro.Icon switch { "hammer" => SlotSpec.Nail, "wall" => SlotSpec.Seal, "chain" => SlotSpec.Chain, "bomb" => SlotSpec.Bomb, _ => null };
                    if (key != null && !c._intros.ContainsKey(key)) c._intros[key] = l.Intro;
                }
                if (l.BoosterIntro?.Id != null && !c._intros.ContainsKey(l.BoosterIntro.Id)) c._intros[l.BoosterIntro.Id] = l.BoosterIntro;
            }
            return c;
        }

        public LevelIntro IntroFor(string thing) => _intros.TryGetValue(thing, out var i)
            ? new LevelIntro { Id = i.Id, Icon = i.Icon, Title = i.Title, Body = i.Body, Tip = i.Tip } : null;
    }
}
