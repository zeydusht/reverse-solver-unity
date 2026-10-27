using System.Collections.Generic;
using System.IO;
using System.Linq;
using ReverseSolver.Core;
using ReverseSolver.Core.Json;
using ReverseSolver.Editing;
using ReverseSolver.Generation;
using UnityEngine;

namespace ReverseSolver.LevelEditor
{
    /* SHOW1 (PM_MESAJ_17): one investor demo level outside SET40, 6x6 or 7x7 with a bomb, 1-2 nails, one remote chain
       and a wall. Drawn by the factory's generator, kept only when SmartPlayer wins cleanly, the chained piece has to
       leave before the bomb (the "key piece") and a first-time player wins often (FunSim, MonteCarlo). Writes
       Assets/_Game/Presentation/Resources/show.json; Tools/make_show1.py adds the cards.

         bash Tools/unity_batch.sh method ReverseSolver.LevelEditor.ShowLevel.Make */
    public static class ShowLevel
    {
        static void Log(string s) => Debug.Log("[show] " + s);

        public static void Make()
        {
            var best = new List<(double score, LevelDraft d, string why)>();
            foreach (var (w, h) in new[] { (6, 6), (7, 7) })
            {
                int cells = w * h;
                var spec = new SlotSpec
                {
                    Slot = 20, Genre = Genre.Orta, Boards = new[] { (w, h) }, MinBodies = cells - 1, MaxBodies = cells - 1,
                    MinNails = 1, MaxNails = 2, MinSeals = 1, MaxSeals = 2, MinChains = 1, MaxChains = 1,
                    MinRemote = 1, MaxRemote = 1, MinBombs = 1, MaxBombs = 1,
                };
                var p = new GenParams { Interlock = .7, EdgeFirst = .3, BombMargin = 8, Knot = .3, Ease = .4 };
                for (uint seed = 1; seed <= 400; seed++)
                {
                    var c = Generator.TryMake(spec, p, 70000u + seed * 131u + (uint)w * 7u, null, out _);
                    if (c == null) continue;
                    var l = c.Level;
                    var sp = SmartPlayer.Play(l);
                    if (sp.Verdict != SmartVerdict.Won || sp.Valves > 0) continue;
                    // key piece: the chained pair leaves before the bomb, and not at once
                    int chain = Enumerable.Range(0, l.Pieces.Count).First(i => l.Pieces[i].Length > 1);
                    int bomb = l.Bombs.Keys.First();
                    var order = sp.Moves.Select(m => m.Piece).ToList();
                    int ic = order.IndexOf(chain), ib = order.IndexOf(bomb);
                    if (ic < 0 || ib < 0 || ic > ib || ib < 3) continue;
                    var fun = FunSim.Run(l, Genre.Orta);
                    var mc = MonteCarlo.Run(l, 200);
                    if (fun.FirstWinRate < .55) continue;
                    double score = fun.FirstWinRate + .3 * mc.WinRate + .02 * (ib - ic);
                    best.Add((score, c.Draft.Clone(), $"{w}x{h} seed {seed}: ilk deneme {fun.FirstWinRate:P0}, MC {mc.WinRate:P0}, zincir sıra {ic} → bomba {ib}, timer {l.TimerSeconds}"));
                    if (best.Count >= 12) break;
                }
            }
            if (best.Count == 0) { Log("aday yok"); return; }
            var pick = best.OrderByDescending(b => b.score).First();
            Log("seçilen: " + pick.why);
            foreach (var b in best.OrderByDescending(b => b.score).Skip(1).Take(3)) Log("yedek: " + b.why);

            var d = pick.d.Clone();
            d.Id = "SHOW1";
            var lv = d.ToLevelData();
            var e = new RedesignStore.Entry { Draft = d, Slot = 0 };
            RedesignStore.HardScores(e, lv);
            Log("metrikler: " + string.Join(", ", e.Scores.Select(kv => $"{kv.Key} {kv.Value:0.##}")));
            var sp2 = SmartPlayer.Play(lv);
            d.Solution = sp2.Moves.ToList();
            var jw = new JsonWriter();
            d.WriteJson(jw);
            Directory.CreateDirectory("Assets/_Game/Levels");
            File.WriteAllText("Assets/_Game/Presentation/Resources/show.json", "{\n  \"format\": 1,\n  \"note\": \"SHOW1 (PM_MESAJ_17): investor demo, outside SET40.\",\n  \"levels\": [\n    " + jw + "\n  ]\n}\n",
                              new System.Text.UTF8Encoding(false));
            Log("yazıldı: Assets/_Game/Presentation/Resources/show.json");
        }

        /* Tension without losses: timer 150 s, then the shortest bomb fuse whose FunSim first-try win stays >= 75 %.
             bash Tools/unity_batch.sh method ReverseSolver.LevelEditor.ShowLevel.Tune */
        public static void Tune()
        {
            const string path = "Assets/_Game/Presentation/Resources/show.json";
            var set = LevelParser.Parse(File.ReadAllText(path));
            var d = LevelDraft.FromLevel(set.Levels[0]);
            d.SetTimer(150);
            int bomb = set.Levels[0].Bombs.Keys.First();
            int chosen = set.Levels[0].Bombs[bomb];
            var bombCell = set.Levels[0].Pieces[bomb][0];
            foreach (int fuse in new[] { 10, 12, 14, 16, 18, 20, 24 })
            {
                d.SetBomb(bombCell, fuse);
                var l = d.ToLevelData();
                var sp = SmartPlayer.Play(l);
                if (sp.Verdict != SmartVerdict.Won) { Log($"fitil {fuse}: SmartPlayer {sp.Verdict}"); continue; }
                var fun = FunSim.Run(l, Genre.Orta);
                var mc = MonteCarlo.Run(l, 200);
                Log($"fitil {fuse}: ilk deneme {fun.FirstWinRate:P0}, MC {mc.WinRate:P0}");
                if (fun.FirstWinRate >= .75) { chosen = fuse; break; }
            }
            d.SetBomb(bombCell, chosen);
            var lv = d.ToLevelData();
            d.Solution = SmartPlayer.Play(lv).Moves.ToList();
            var fin = FunSim.Run(lv, Genre.Orta);
            var e = new RedesignStore.Entry { Draft = d, Slot = 0 };
            RedesignStore.HardScores(e, lv);
            Log($"SONUÇ fitil {chosen}, süre {d.Timer}: ilk deneme {fin.FirstWinRate:P0}; " + string.Join(", ", e.Scores.Select(kv => $"{kv.Key} {kv.Value:0.##}")));
            var jw = new JsonWriter();
            d.WriteJson(jw);
            Save(path, jw.ToString());
        }

        static void Save(string path, string level) =>
            File.WriteAllText(path, "{\n  \"format\": 1,\n  \"note\": \"SHOW1 (PM_MESAJ_17): investor demo, outside SET40.\",\n  \"levels\": [\n    " + level + "\n  ]\n}\n",
                              new System.Text.UTF8Encoding(false));
    }
}
