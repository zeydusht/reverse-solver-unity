using System;
using System.Collections.Generic;
using ReverseSolver.Core.Json;

namespace ReverseSolver.Core
{
    /* PM_81 §2: what changes without a build, from live/config.json. Every key is optional; a missing or broken
       file keeps the defaults built in here (the PM's values). Keys are the PM's (Turkish). */
    public sealed class LiveConfig
    {
        public bool AdsOn = true;                    // reklam_acik
        public int InterstitialFromLevel = 12;       // gecis_baslangic_bolum
        public int InterstitialEveryN = 3;           // gecis_her_n_bolum
        public int InterstitialMinSeconds = 120;     // gecis_min_sn
        public int FirstSessionQuietMinutes = 10;    // gecis_ilk_oturum_sessiz_dk
        public bool R1On = true, R2On = true, R3On = true, R4On = true, InterstitialOn = true;   // r1_acik … gecis_acik
        public int R3Daily = 5;                      // r3_gunluk (booster +1)
        public int R4Daily = 3;                      // r4_gunluk (menu gift)
        public int R4Coins = 100;                    // r4_altin
        public int[] ContinuePrices = { 900, 1900, 2900 };   // devam_fiyatlari
        public int StartCoins = 5000;                // baslangic_altin
        public int ExtraSeconds = 30;                // ek_sure_sn (+30 s: gold and the R1 ad)
        public string Source = "default";            // where it came from: default, embedded, cache, remote

        public static LiveConfig Default => new LiveConfig();

        public static LiveConfig Parse(string json, string source = "remote")
        {
            var c = new LiveConfig();
            if (string.IsNullOrEmpty(json)) return c;
            JsonNode root;
            try { root = JsonReader.Parse(json); }
            catch (JsonException) { return c; }
            if (root.Kind != JsonKind.Object) return c;
            c.Source = source;
            c.AdsOn = Bool(root, "reklam_acik", c.AdsOn);
            c.InterstitialOn = Bool(root, "gecis_acik", c.InterstitialOn);
            c.InterstitialFromLevel = Int(root, "gecis_baslangic_bolum", c.InterstitialFromLevel, 1, 10000);
            c.InterstitialEveryN = Int(root, "gecis_her_n_bolum", c.InterstitialEveryN, 1, 1000);
            c.InterstitialMinSeconds = Int(root, "gecis_min_sn", c.InterstitialMinSeconds, 0, 86400);
            c.FirstSessionQuietMinutes = Int(root, "gecis_ilk_oturum_sessiz_dk", c.FirstSessionQuietMinutes, 0, 1440);
            c.R1On = Bool(root, "r1_acik", c.R1On);
            c.R2On = Bool(root, "r2_acik", c.R2On);
            c.R3On = Bool(root, "r3_acik", c.R3On);
            c.R4On = Bool(root, "r4_acik", c.R4On);
            c.R3Daily = Int(root, "r3_gunluk", c.R3Daily, 0, 100);
            c.R4Daily = Int(root, "r4_gunluk", c.R4Daily, 0, 100);
            c.R4Coins = Int(root, "r4_altin", c.R4Coins, 0, 100000);
            c.StartCoins = Int(root, "baslangic_altin", c.StartCoins, 0, 10000000);
            c.ExtraSeconds = Int(root, "ek_sure_sn", c.ExtraSeconds, 5, 600);
            if (root.TryGet("devam_fiyatlari", out var prices) && prices.Kind == JsonKind.Array)
            {
                var list = new List<int>();
                foreach (var p in prices.Items)
                    if (p.Kind == JsonKind.Number && p.AsNumber >= 0) list.Add((int)p.AsNumber);
                if (list.Count >= 1) c.ContinuePrices = list.ToArray();
            }
            return c;
        }

        public string ToJson()
        {
            var w = new JsonWriter().BeginObject()
                .Field("reklam_acik", AdsOn).Field("gecis_acik", InterstitialOn)
                .Field("gecis_baslangic_bolum", InterstitialFromLevel).Field("gecis_her_n_bolum", InterstitialEveryN)
                .Field("gecis_min_sn", InterstitialMinSeconds).Field("gecis_ilk_oturum_sessiz_dk", FirstSessionQuietMinutes)
                .Field("r1_acik", R1On).Field("r2_acik", R2On).Field("r3_acik", R3On).Field("r4_acik", R4On)
                .Field("r3_gunluk", R3Daily).Field("r4_gunluk", R4Daily).Field("r4_altin", R4Coins)
                .Field("baslangic_altin", StartCoins).Field("ek_sure_sn", ExtraSeconds);
            w.Key("devam_fiyatlari").BeginArray();
            foreach (int p in ContinuePrices) w.Value(p);
            w.EndArray();
            return w.EndObject().ToString();
        }

        static bool Bool(JsonNode o, string key, bool fallback) =>
            o.TryGet(key, out var n) && n.Kind == JsonKind.Bool ? n.AsBool : fallback;

        static int Int(JsonNode o, string key, int fallback, int min, int max)
        {
            if (!o.TryGet(key, out var n) || n.Kind != JsonKind.Number) return fallback;
            double v = n.AsNumber;
            return v < min || v > max ? fallback : (int)v;
        }
    }
}
