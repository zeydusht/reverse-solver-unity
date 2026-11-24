using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using ReverseSolver.Core;
using ReverseSolver.Presentation;
using UnityEngine;
using UnityEngine.TestTools;

namespace ReverseSolver.PlayMode.Tests
{
    /* PM_106 §4: Zeyd: "Did you take the rubber off the chain? At the wall it felt like there was none." The rule: the
       rubber (squash and spring back, lastik 2) on every jam alike, whatever stops the piece: a piece in its way (moving
       into it or caught at the side by a tab), the red wall, a chained pair, a locked piece, a bomb piece; with room and
       with none. On p23's own levels (sl100: 20 drawn at random, and every obstacle at least once), each jam flicked as
       the game does (EndDrag, autoSlide) and every frame measured: the cell that squashes most, its length on the axis
       against its length at rest. Each squash at least 90 % of lastik 2's (BoardView.RubberSquashAt(2)). The table is
       written to docs/art/pm106/lastik_olcum.md. */
    public sealed class Pm106RubberTests
    {
        const float Cell = 60;

        sealed class Jam { public LevelData level; public int played; public int piece; public Dir dir; public string kind; public float contact; }

        /* The level after its solution's first `played` moves (a packed board has no room anywhere: the jams with room
           come as pieces leave). */
        static GameSession After(LevelData level, int played)
        {
            var session = new GameSession(level, 1, "test", new Mulberry32(1));
            for (int i = 0; i < played && i < level.Solution.Count; i++)
                if (session.TryExit(level.Solution[i].Piece, level.Solution[i].Dir) != CommandResult.Ok) break;
            return session;
        }

        static IEnumerable<Jam> JamsOf(LevelData level, int played)
        {
            var session = After(level, played);
            if (session.IsOver) yield break;
            var b = session.Board;
            for (int p = 0; p < level.Pieces.Count; p++)
                if (b.IsPresent(p))
                foreach (Dir d in new[] { Dir.U, Dir.D, Dir.L, Dir.R })
                {
                    var l = TravelRule.Limit(b, p, d);
                    bool nail = session.NailAt(p) > 0;
                    if (l.Exit && !nail) continue;
                    string kind = nail ? "kilitli parça"
                        : level.Pieces[p].Length > 1 ? "zincirli parça"
                        : level.Bombs.ContainsKey(p) ? "bombalı parça"
                        : l.Blocker < 0 ? "kırmızı duvar"
                        : l.Hard ? "komşu parça" : "yandan takılma (diş)";
                    float contact = l.Exit ? 0 : Contact.Distance(b, p, d, l);
                    yield return new Jam { level = level, played = played, piece = p, dir = d, kind = kind, contact = contact };
                }
        }

        static IEnumerator Measure(Jam j, float[] result)
        {
            var root = new GameObject("pm106");
            var session = After(j.level, j.played);
            var board = BoardView.Create(root.transform, session, Cell, Vector2.zero);
            yield return null;
            var pv = board.PieceViewOf(j.piece);
            int n = j.level.Pieces[j.piece].Length;
            bool vertical = j.dir == Dir.U || j.dir == Dir.D;
            var rest = new float[n];
            for (int i = 0; i < n; i++) { var r = pv.CellBody(i); rest[i] = vertical ? r.height : r.width; }
            board.EndDrag(j.piece, DragRelease.SnappedBack, j.dir, 0, autoSlide: true);
            float start = Time.realtimeSinceStartup, squash = 0;
            while (Time.realtimeSinceStartup - start < .8f)
            {
                for (int i = 0; i < n; i++)
                {
                    var r = pv.CellBody(i);
                    squash = Mathf.Max(squash, 1 - (vertical ? r.height : r.width) / rest[i]);
                }
                yield return null;
            }
            result[0] = squash;
            Object.Destroy(root);
            yield return null;
        }

        [UnityTest, Timeout(300000)]
        public IEnumerator EveryJamIsRubber()
        {
            var set = LevelParser.Parse(System.IO.File.ReadAllText("Assets/Resources/Live/levels.json"));
            var all = set.Levels.Select((l, i) => (l, i)).ToList();
            var rng = new System.Random(106);
            var cases = new List<(int number, Jam jam, string why)>();
            // 20 levels at random, one jam each (any kind), part way through the level (none to half its solution played)
            foreach (var (l, i) in all.OrderBy(_ => rng.Next()).Take(20).OrderBy(x => x.i))
            {
                var jams = JamsOf(l, rng.Next(l.Solution.Count / 2 + 1)).ToList();
                if (jams.Count == 0) continue;
                cases.Add((i + 1, jams[rng.Next(jams.Count)], "rastgele"));
            }
            // every kind at least once, with room and with none
            foreach (var kind in new[] { "komşu parça", "yandan takılma (diş)", "kırmızı duvar", "zincirli parça", "kilitli parça", "bombalı parça" })
                foreach (bool room in new[] { true, false })
                {
                    var hit = all.SelectMany(x => Enumerable.Range(0, x.l.Solution.Count).SelectMany(m => JamsOf(x.l, m)).Select(jm => (x.i, jm)))
                        .FirstOrDefault(x => x.jm.kind == kind && (x.jm.contact >= BoardView.SlamMin) == room);
                    if (hit.jm != null) cases.Add((hit.i + 1, hit.jm, room ? "tür, boşluklu" : "tür, boşluksuz"));
                }
            float need = .9f * BoardView.RubberSquashAt(2);
            var rows = new List<string>();
            var fails = new List<string>();
            var kinds = new HashSet<string>();
            foreach (var (number, jam, why) in cases)
            {
                var r = new float[1];
                yield return Measure(jam, r);
                kinds.Add(jam.kind);
                string row = $"| {number} | {jam.kind} | {why} | {jam.played} | {jam.piece} {jam.dir} | {jam.contact:0.00} | {r[0]:0.000} | {r[0] / BoardView.RubberSquashAt(2) * 100:0}% | {(r[0] >= need ? "geçti" : "KALDI")} |";
                rows.Add(row);
                Debug.Log("[lastik106] " + row);
                if (r[0] < need) fails.Add(row);
            }
            var md = new System.Text.StringBuilder();
            md.AppendLine("# PM_106 §4: lastik ölçümü (Pm106RubberTests)");
            md.AppendLine();
            md.AppendLine($"Her satırda takılma, oyundaki gibi fırlatılarak (EndDrag, autoSlide) çalıştırıldı ve her karede ölçüldü. Ölçülen: en çok ezilen hücrenin eksen boyunca boyunun dinlenmedeki boyuna göre kısalması. Lastik 2: {BoardView.RubberSquashAt(2):0.00}; eşik %90: {need:0.000}.");
            md.AppendLine();
            md.AppendLine("| Bölüm | Engel | Seçim | Önce çıkan | Parça, yön | Temas (hücre) | Büzülme | Lastik 2'ye göre | Sonuç |");
            md.AppendLine("|---|---|---|---|---|---|---|---|---|");
            foreach (var row in rows) md.AppendLine(row);
            System.IO.Directory.CreateDirectory("docs/art/pm106");
            System.IO.File.WriteAllText("docs/art/pm106/lastik_olcum.md", md.ToString());
            Debug.Log($"[lastik106] {rows.Count} takılma, {kinds.Count} engel türü, kalan {fails.Count}");
            Assert.That(kinds.Count, Is.GreaterThanOrEqualTo(6), "the obstacle kinds found: " + string.Join(", ", kinds));
            Assert.That(rows.Count(r => r.Contains("tür, boşluklu")), Is.GreaterThanOrEqualTo(5), "jams with room, per kind");
            Assert.That(fails, Is.Empty, string.Join("\n", fails));
        }
    }
}
