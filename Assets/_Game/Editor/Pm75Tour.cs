using System.Globalization;
using System.IO;
using System.Text;
using ReverseSolver.Core;
using ReverseSolver.Presentation;
using UnityEngine;

namespace ReverseSolver.EditorTools
{
    /* PM_75: where a stuck piece visibly slides before it stops (Contact.Distance), along each level's stored
       solution, with 390x844 screen points for the screenshot tour (Tools/pm72_tour.js "solve" then "drag").
         bash Tools/unity_batch.sh method ReverseSolver.EditorTools.Pm75Tour.Slides   -> Logs/pm75/slides.json */
    public static class Pm75Tour
    {
        public static void Slides()
        {
            var set = P21Onboarding.P22(LevelParser.Parse(File.ReadAllText("Assets/_Game/Levels/redesign.json")));
            var screen = new Vector2(390, 844);
            string F(float v) => v.ToString("0.##", CultureInfo.InvariantCulture);
            var sb = new StringBuilder("[");
            int count = 0;
            foreach (var l in set.Levels)
            {
                if (l.Solution == null) continue;
                var o = GameRoot.Hud54BoardPlacement(screen, Vector4.zero, l, out float cell);
                var board = new Board(l);
                for (int k = 0; k <= Mathf.Min(12, l.Solution.Count); k++)
                {
                    foreach (int p in board.PresentPieces())
                    {
                        if (l.Nails.ContainsKey(p) || TravelRule.CanExit(board, p)) continue;
                        foreach (var d in DirExt.All)
                        {
                            var lim = TravelRule.Limit(board, p, d);
                            if (lim.Exit) continue;
                            float c = Contact.Distance(board, p, d, lim);
                            if (c < .25f) continue;
                            var at = l.Pieces[p][0];
                            sb.Append(count++ > 0 ? ",\n" : "").Append($"{{\"n\":{l.Number},\"k\":{k},\"piece\":{p},\"dir\":\"{d}\",\"contact\":{F(c)},\"cells\":{lim.Cells}," +
                                $"\"hard\":{(lim.Hard ? "true" : "false")},\"x\":{F(o.x + (at.X + .5f) * cell)},\"y\":{F(o.y + (at.Y + .5f) * cell)},\"cell\":{F(cell)}}}");
                        }
                    }
                    if (k < l.Solution.Count) board.Remove(l.Solution[k].Piece);
                }
            }
            Directory.CreateDirectory("Logs/pm75");
            File.WriteAllText("Logs/pm75/slides.json", sb.Append("]").ToString());
            Debug.Log($"[pm75] {count} slides -> Logs/pm75/slides.json");
        }
    }
}
