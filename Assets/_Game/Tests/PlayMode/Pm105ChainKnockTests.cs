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
    /* PM_105 §1: Zeyd: "the chained piece leaps, it flies." A chained pair is one rigid piece with two cells far apart;
       the rubber knock (PM_93/94) scaled its transform about the pair's bounding box, so the squash and stretch moved the
       far cell by (squash x span) and widening moved it sideways. Now each cell deforms about its own leading edge and the
       pair only slides.
       Six cases: the gap before the first contact (0, 1, 3 cells) x which end touches first (the near end A, or the far
       end B: "the chain's limit"). Every frame: no cell's leading edge passes its own contact, the two cells' leading
       edges move together (the far end is not flung), neither moves sideways, and on the way back neither passes home by
       more than 6 % of a cell. The frames are written for the before/after GIF (docs/art/pm105/zincir_kareler.json). */
    public sealed class Pm105ChainKnockTests
    {
        const int W = 6, H = 8;
        const float Cell = 60;
        static readonly Cell A = new Cell(1, 0), B = new Cell(4, 2);

        static LevelData Level(int gapA, int gapB)
        {
            var pieces = new List<string> { $"[[{A.X},{A.Y}],[{B.X},{B.Y}]]" };
            int ya = A.Y + gapA + 1, yb = B.Y + gapB + 1;
            if (ya < H) pieces.Add($"[[{A.X},{ya}]]");
            if (yb < H) pieces.Add($"[[{B.X},{yb}]]");
            string v = "[null," + string.Join(",", Enumerable.Repeat("0", W - 1)) + ",null]";
            string edge = "[" + string.Join(",", Enumerable.Repeat("null", W)) + "]";
            string middle = "[" + string.Join(",", Enumerable.Repeat("0", W)) + "]";
            string json = $"{{\"format\":1,\"levels\":[{{\"id\":\"PM105\",\"version\":1,\"lv\":31,\"w\":{W},\"h\":{H},\"timer\":60," +
                $"\"pieces\":[{string.Join(",", pieces)}],\"vEdge\":[{string.Join(",", Enumerable.Repeat(v, H))}]," +
                $"\"hEdge\":[{edge},{string.Join(",", Enumerable.Repeat(middle, H - 1))},{edge}]}}]}}";
            return LevelParser.Parse(json).Levels[0];
        }

        public sealed class Frame { public float t; public float[] a, b; }   // a/b: x0, y0, x1, y1 of each cell's body (points)
        public sealed class Case { public string name; public bool perCell; public float contact; public List<Frame> frames = new List<Frame>(), checks = new List<Frame>(); }
        static readonly List<Case> Recorded = new List<Case>();

        static IEnumerator Knock(string name, int gapA, int gapB, bool perCell, Case into)
        {
            BoardView.ChainPerCell = perCell;
            var root = new GameObject("pm105");
            var session = new GameSession(Level(gapA, gapB), 1, "test", new Mulberry32(1));
            var board = BoardView.Create(root.transform, session, Cell, Vector2.zero);
            yield return null;
            var pv = board.PieceViewOf(0);
            into.name = name; into.perCell = perCell;
            into.contact = Contact.Distance(session.Board, 0, Dir.D);
            board.EndDrag(0, DragRelease.SnappedBack, Dir.D, 0, autoSlide: true);
            float start = Time.realtimeSinceStartup, last = -1;
            while (Time.realtimeSinceStartup - start < .75f)
            {
                float t = Time.realtimeSinceStartup - start;
                Rect a = pv.CellBody(0), b = pv.CellBody(1);
                var f = new Frame { t = t, a = new[] { a.xMin, a.yMin, a.xMax, a.yMax }, b = new[] { b.xMin, b.yMin, b.xMax, b.yMax } };
                into.checks.Add(f);
                if (t - last >= 1 / 60f) { into.frames.Add(f); last = t; }   // the GIF's frames: 60 a second
                yield return null;
            }
            Object.Destroy(root);
            BoardView.ChainPerCell = true;
            yield return null;
        }

        static readonly (string name, int gapA, int gapB)[] Cases =
        {
            ("boşluksuz, temas yakın", 0, 2), ("1 boşluk, temas yakın", 1, 3), ("3 boşluk, temas yakın", 3, 5),
            ("boşluksuz, zincir ucu yakın", 2, 0), ("1 boşluk, zincir ucu yakın", 3, 1), ("3 boşluk, zincir ucu yakın", 5, 3),
        };

        [UnityTest, Timeout(120000)]
        public IEnumerator AChainedPairKnocksWithoutFlinging()
        {
            float tol = .015f * Cell;
            var fails = new List<string>();
            Recorded.Clear();
            foreach (var (name, gapA, gapB) in Cases)
            {
                var c = new Case();
                yield return Knock(name, gapA, gapB, true, c);
                Recorded.Add(c);
                float reach = Mathf.Max(c.contact, BoardView.ChainNudge) * Cell;   // no room: the pair's short nudge
                float homeA = A.Y * Cell + Cell, homeB = B.Y * Cell + Cell;   // the leading (lower) edges at home
                float maxLead = 0, maxSplit = 0, maxSide = 0, minLead = 0;
                foreach (var f in c.checks)
                {
                    float leadA = f.a[3] - homeA, leadB = f.b[3] - homeB;
                    maxLead = Mathf.Max(maxLead, leadA, leadB);
                    minLead = Mathf.Min(minLead, leadA, leadB);   // the leading edge back past home
                    maxSplit = Mathf.Max(maxSplit, Mathf.Abs(leadA - leadB));
                    maxSide = Mathf.Max(maxSide, Mathf.Abs((f.a[0] + f.a[2]) * .5f - (A.X + .5f) * Cell), Mathf.Abs((f.b[0] + f.b[2]) * .5f - (B.X + .5f) * Cell));
                }
                Debug.Log($"[zincir105] {name}: temas {c.contact:0.00} hücre, en ileri {maxLead / Cell:0.000}, uçlar arası fark {maxSplit / Cell:0.000}, yana {maxSide / Cell:0.000}, geri aşma {-minLead / Cell:0.000} hücre, {c.checks.Count} kare");
                if (maxLead > reach + tol) fails.Add($"{name}: a cell passed its contact ({maxLead / Cell:0.000} > {c.contact:0.000})");
                if (maxSplit > tol) fails.Add($"{name}: the far end was flung ({maxSplit / Cell:0.000} cells apart)");
                if (maxSide > tol) fails.Add($"{name}: a cell moved sideways ({maxSide / Cell:0.000} cells)");
                if (-minLead > (BoardView.ChainOver + .02f) * Cell) fails.Add($"{name}: it passed home by {-minLead / Cell:0.000} cells");
                if (c.contact >= BoardView.SlamMin && maxLead < reach * .8f) fails.Add($"{name}: it never reached its contact ({maxLead / Cell:0.000})");
            }
            // the bug, measured the old way (for the GIF and to show the test sees it)
            foreach (var (name, gapA, gapB) in Cases.Where(k => k.gapA == 3 || k.gapB == 3))
            {
                var c = new Case();
                yield return Knock(name, gapA, gapB, false, c);
                Recorded.Add(c);
                float split = c.checks.Max(f => Mathf.Abs((f.a[3] - (A.Y * Cell + Cell)) - (f.b[3] - (B.Y * Cell + Cell))));
                Debug.Log($"[zincir105] ESKİ {name}: uçlar arası fark {split / Cell:0.000} hücre");
            }
            var json = new System.Text.StringBuilder("{\"cell\":60,\"w\":6,\"h\":8,\"a\":[1,0],\"b\":[4,2],\"cases\":[");
            for (int i = 0; i < Recorded.Count; i++)
            {
                var c = Recorded[i];
                json.Append(i > 0 ? "," : "").Append($"{{\"name\":\"{c.name}\",\"perCell\":{(c.perCell ? "true" : "false")},\"contact\":{c.contact.ToString(System.Globalization.CultureInfo.InvariantCulture)},\"frames\":[");
                json.Append(string.Join(",", c.frames.Select(f => "[" + string.Join(",", new[] { f.t }.Concat(f.a).Concat(f.b).Select(x => x.ToString("0.###", System.Globalization.CultureInfo.InvariantCulture))) + "]")));
                json.Append("]}");
            }
            json.Append("]}");
            System.IO.File.WriteAllText("docs/art/pm105/zincir_kareler.json", json.ToString());
            Assert.That(fails, Is.Empty, string.Join("\n", fails));
        }
    }
}
