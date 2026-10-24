using System.Collections.Generic;
using System.IO;
using ReverseSolver.Core;
using ReverseSolver.Core.Json;
using ReverseSolver.Editing;
using ReverseSolver.Generation;
using UnityEngine;

namespace ReverseSolver.LevelEditor
{
    /* SET40 v3 (PM_MESAJ_6/7): the new tutorial and introduction levels for a NEW player. Each is drawn
       by the factory's generator inside a hand-set spec and written to Builds/setv3/gen.json; the set
       itself (order, reused levels, cards, flags) is assembled by Tools/make_set40_v3.py.

         bash Tools/unity_batch.sh method ReverseSolver.LevelEditor.SetV3.Gen */
    public static class SetV3
    {
        static void Log(string s) => Debug.Log("[setv3] " + s);

        // slot, board, bodies, nails, seals, chains (remote), bombs, genre
        static readonly (int slot, int w, int h, int bodies, int nails, int seals, int chains, int remote, int bombs, Genre g)[] Slots =
        {
            (1, 3, 3, 9, 0, 0, 0, 0, 0, Genre.Tanitim),
            (2, 4, 4, 16, 0, 0, 0, 0, 0, Genre.Tanitim),
            (3, 4, 5, 20, 1, 0, 0, 0, 0, Genre.Tanitim),
            (5, 5, 5, 25, 2, 0, 0, 0, 0, Genre.Kolay),
            (6, 4, 5, 20, 0, 0, 0, 0, 1, Genre.Tanitim),
            (7, 5, 5, 25, 0, 0, 0, 0, 1, Genre.Kolay),
            (8, 5, 6, 30, 0, 0, 0, 0, 2, Genre.Kolay),
            (10, 4, 5, 19, 0, 0, 1, 0, 0, Genre.Tanitim),
            (11, 5, 5, 24, 0, 0, 1, 0, 0, Genre.Kolay),
            (12, 5, 6, 28, 0, 0, 2, 2, 0, Genre.Kolay),
            (13, 5, 6, 29, 0, 0, 1, 0, 1, Genre.Kolay),
            (14, 5, 5, 25, 0, 1, 0, 0, 0, Genre.Tanitim),
            (15, 5, 6, 28, 1, 1, 1, 0, 0, Genre.Kolay),
        };

        public static void Gen()
        {
            Directory.CreateDirectory("Builds/setv3");
            var lines = new List<string>();
            foreach (var s in Slots)
            {
                var spec = new SlotSpec
                {
                    Slot = s.slot, Genre = s.g, Boards = new[] { (s.w, s.h) },
                    MinBodies = s.bodies, MaxBodies = s.bodies,
                    MinNails = s.nails, MaxNails = s.nails, MinSeals = s.seals, MaxSeals = s.seals,
                    MinChains = s.chains, MaxChains = s.chains, MinRemote = s.remote, MaxRemote = s.remote,
                    MinBombs = s.bombs, MaxBombs = s.bombs,
                };
                // easy for a new player: many pieces free at the start, generous bomb fuse
                var p = new GenParams { Interlock = 0.3, EdgeFirst = 0.8, BombMargin = 8 };
                Candidate c = null;
                for (uint seed = 1; seed < 400 && c == null; seed += 37)
                    c = Generator.Make(spec, p, 3000u + (uint)s.slot * 1000u + seed);
                if (c == null) { Log($"slot {s.slot}: üretilemedi"); continue; }
                var w = new JsonWriter();
                c.Draft.WriteJson(w);
                string level = w.ToString();
                lines.Add("    " + level.Substring(0, level.Length - 1) + $",\"v3slot\":{s.slot}}}");
                Log($"slot {s.slot}: {c}");
            }
            File.WriteAllText("Builds/setv3/gen.json", "{\"format\":1,\"levels\":[\n" + string.Join(",\n", lines) + "\n]}\n",
                              new System.Text.UTF8Encoding(false));
            Log($"yazıldı: Builds/setv3/gen.json ({lines.Count}/{Slots.Length})");
        }
    }
}
