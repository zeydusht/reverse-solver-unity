using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using ReverseSolver.Core;
using ReverseSolver.Presentation;
using UnityEngine;

namespace ReverseSolver.EditorTools
{
    /* PM_72: p22's levels with their 390x844 board placement and solution, for the screenshot tour (Tools/pm72_tour.js).
         bash Tools/unity_batch.sh method ReverseSolver.EditorTools.Pm72Tour.Dump   -> Logs/pm72/p22_levels.json */
    public static class Pm72Tour
    {
        public static void Dump()
        {
            var set = P21Onboarding.P22(LevelParser.Parse(File.ReadAllText("Assets/_Game/Levels/redesign.json")));
            var screen = new Vector2(390, 844);
            var sb = new StringBuilder("[");
            string F(float v) => v.ToString("0.##", CultureInfo.InvariantCulture);
            foreach (var l in set.Levels)
            {
                var o = GameRoot.Hud54BoardPlacement(screen, Vector4.zero, l, out float cell);
                var sol = (l.Solution ?? new System.Collections.Generic.List<Move>()).Select(m =>
                {
                    var c = l.Pieces[m.Piece][0];
                    return $"[{F(o.x + (c.X + .5f) * cell)},{F(o.y + (c.Y + .5f) * cell)},\"{m.Dir}\",{m.Piece}]";
                });
                sb.Append(sb.Length > 1 ? ",\n" : "").Append($"{{\"n\":{l.Number},\"id\":\"{l.Id}\",\"w\":{l.Width},\"h\":{l.Height},\"cell\":{F(cell)},\"x\":{F(o.x)},\"y\":{F(o.y)}," +
                    $"\"timer\":{l.TimerSeconds},\"bombs\":{l.Bombs.Count},\"nails\":{l.Nails.Count},\"sol\":[{string.Join(",", sol)}]}}");
            }
            Directory.CreateDirectory("Logs/pm72");
            File.WriteAllText("Logs/pm72/p22_levels.json", sb.Append("]").ToString());
            Debug.Log($"[pm72] {set.Levels.Count} levels -> Logs/pm72/p22_levels.json");
        }
    }
}
