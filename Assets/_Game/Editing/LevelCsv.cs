using System.Collections.Generic;
using System.Globalization;
using System.Text;
using ReverseSolver.Core;

namespace ReverseSolver.Editing
{
    /* The level feature table for the difficulty work (PRODUCT.md, Faz 4):
       one row per level, web levels and designs. Written for a Turkish Excel:
       ';' between fields, decimal comma, UTF-8 with a BOM (without the BOM
       Excel reads the file as ANSI and breaks ç, ğ, ı, ş). */
    public static class LevelCsv
    {
        public static readonly string[] Columns =
        {
            "set", "id", "version", "lv", "w", "h", "parca", "zincir", "civi", "civi_toplam",
            "bomba", "fitil_en_kisa", "muhur_serit", "muhur_kenar", "acik_kenar_orani", "sure_sn",
            "cozum_hamle", "karar", "telefon_hucre_pt"
        };

        /* Faz 2 structural difficulty, added when monteCarloRuns > 0 (MonteCarlo). */
        public static readonly string[] MonteCarloColumns =
        {
            "rastgele_kazanma", "rastgele_patlama", "rastgele_ort_hamle", "rastgele_ort_dallanma"
        };

        static readonly CultureInfo Tr = CultureInfo.GetCultureInfo("tr-TR");

        public static string Build(IEnumerable<(string set, LevelData level)> rows, int nodeBudget = DesignCheck.QuickBudget,
                                   int monteCarloRuns = 0)
        {
            var sb = new StringBuilder();
            sb.Append(string.Join(";", Columns));
            if (monteCarloRuns > 0) sb.Append(';').Append(string.Join(";", MonteCarloColumns));
            sb.Append("\r\n");
            foreach (var (set, l) in rows)
            {
                var st = LevelStats.Of(l);
                var check = DesignCheck.Run(l, nodeBudget);
                int nailSum = 0, shortestFuse = 0;
                foreach (var n in l.Nails.Values) nailSum += n;
                foreach (var f in l.Bombs.Values) shortestFuse = shortestFuse == 0 ? f : System.Math.Min(shortestFuse, f);
                double open = st.Joints == 0 ? 0 : (double)st.OpenJoints / st.Joints;
                var cells = new[]
                {
                    set, l.Id, I(l.Version), I(l.Number), I(l.Width), I(l.Height), I(st.Pieces), I(st.Chains),
                    I(st.Nails), I(nailSum), I(st.Bombs), I(shortestFuse), I(st.SealedLanes), I(st.SealedSides),
                    open.ToString("0.00", Tr), I(l.TimerSeconds), I(check.Solution.Count), Verdict(check.Verdict),
                    I(check.CellSize)
                };
                for (int i = 0; i < cells.Length; i++)
                {
                    if (i > 0) sb.Append(';');
                    sb.Append(Field(cells[i]));
                }
                if (monteCarloRuns > 0)
                {
                    var mc = MonteCarlo.Run(l, monteCarloRuns);
                    sb.Append(';').Append(mc.WinRate.ToString("0.000", Tr)).Append(';').Append(mc.BombRate.ToString("0.000", Tr))
                      .Append(';').Append(mc.MeanMoves.ToString("0.0", Tr)).Append(';').Append(mc.MeanBranching.ToString("0.00", Tr));
                }
                sb.Append("\r\n");
            }
            return sb.ToString();
        }

        /* The file's bytes: UTF-8 with its BOM. */
        public static byte[] ToBytes(string csv)
        {
            var enc = new UTF8Encoding(true);
            var pre = enc.GetPreamble();
            var body = enc.GetBytes(csv);
            var all = new byte[pre.Length + body.Length];
            pre.CopyTo(all, 0);
            body.CopyTo(all, pre.Length);
            return all;
        }

        static string I(int v) => v.ToString(CultureInfo.InvariantCulture);

        static string Verdict(DesignVerdict v) => v switch
        {
            DesignVerdict.Solvable => "cozulebilir",
            DesignVerdict.NeedsValve => "supap",
            DesignVerdict.Frozen => "donuk",
            DesignVerdict.Bomb => "bomba",
            _ => "dogrulanamadi"
        };

        static string Field(string s) =>
            s.IndexOfAny(new[] { ';', '"', '\r', '\n' }) < 0 ? s : "\"" + s.Replace("\"", "\"\"") + "\"";
    }
}
