using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using ReverseSolver.Core;

namespace ReverseSolver.Generation
{
    /* PM_71: what a test cohort really did with each level, for DifficultyModelV2.PWin1Blended.
       One row per level as the cohort played it: id, version, the slot it had, n players, d_obs (IRT difficulty of
       an average player's first try, logit; docs/analiz/v2/irt.py: p = 1 / (1 + e^d)). The cohort's own versions of
       the levels are kept so the model can be run on what they played.
         pt1: docs/analiz/v2/irt_level_d.csv on Assets/_Game/Levels/redesign.json (all 40 versions match).
         other cohorts (p22 next): docs/analiz/v2/irt_level_d_<cohort>.csv on the levels they played. */
    public sealed class PlayData
    {
        public sealed class Row
        {
            public string Id;
            public int Version, Slot, N;
            public double D;
        }

        /* w = n / (n + Prior): ten players weigh as much as the model. */
        public const double Prior = 10;
        public const string Pt1Csv = "docs/analiz/v2/irt_level_d.csv", Pt1Levels = "Assets/_Game/Levels/redesign.json";
        public static string CsvPath(string cohort) => cohort == "pt1" ? Pt1Csv : $"docs/analiz/v2/irt_level_d_{cohort}.csv";

        public readonly string Cohort;
        readonly Dictionary<string, Row> _rows = new Dictionary<string, Row>();
        readonly Dictionary<string, LevelData> _played = new Dictionary<string, LevelData>();
        readonly Dictionary<string, double> _modelAtPlay = new Dictionary<string, double>();

        PlayData(string cohort) { Cohort = cohort; }
        public static PlayData Empty(string cohort) => new PlayData(cohort);

        /* Rows whose version the given set does not have are left out (the model cannot be run on them). */
        public static PlayData Parse(string cohort, string csv, LevelSet played)
        {
            var d = new PlayData(cohort);
            var byId = new Dictionary<string, LevelData>();
            foreach (var l in played.Levels) byId[l.Id] = l;
            var lines = csv.Replace("\r", "").Split('\n');
            var head = new List<string>(lines[0].Split(','));
            int Col(string name) => head.IndexOf(name);
            for (int i = 1; i < lines.Length; i++)
            {
                if (lines[i].Trim() == "") continue;
                var v = lines[i].Split(',');
                var row = new Row
                {
                    Id = v[Col("level_id")], Version = int.Parse(v[Col("level_version")], CultureInfo.InvariantCulture),
                    Slot = Col("pt1_slot") >= 0 ? int.Parse(v[Col("pt1_slot")], CultureInfo.InvariantCulture) : 0,
                    N = int.Parse(v[Col("n")], CultureInfo.InvariantCulture), D = double.Parse(v[Col("d_obs")], CultureInfo.InvariantCulture)
                };
                if (!byId.TryGetValue(row.Id, out var level) || level.Version != row.Version || row.N <= 0) continue;
                d._rows[row.Id] = row; d._played[row.Id] = level;
            }
            return d;
        }

        public static PlayData Load(string cohort, string levelsPath)
        {
            string csv = CsvPath(cohort);
            if (!File.Exists(csv) || !File.Exists(levelsPath)) return Empty(cohort);
            return Parse(cohort, File.ReadAllText(csv), LevelParser.Parse(File.ReadAllText(levelsPath)));
        }

        static PlayData _pt1;
        /* The default for the factory, the curve check and the reports. */
        public static PlayData Pt1 => _pt1 ?? (_pt1 = Load("pt1", Pt1Levels));

        public int Count => _rows.Count;
        public Row Find(string id) => id != null && _rows.TryGetValue(id, out var r) ? r : null;

        /* The model's p_win1 on the version the cohort played (cached). */
        public double ModelAtPlay(Row row)
        {
            if (!_modelAtPlay.TryGetValue(row.Id, out double p))
                _modelAtPlay[row.Id] = p = DifficultyModelV2.PWin1(DifficultyV2.Compute(_played[row.Id]));
            return p;
        }
    }
}
