using System;
using ReverseSolver.Core.Json;

namespace ReverseSolver.Core
{
    /* PM_81 §3 / PM_82 §4: the ad placements and when they may show. Pure rules; the game asks, the ad network
       (Google AdMob on iOS, PM_101; none on the web) shows.
         R1  rewarded      the time-up panel: +30 s, once per attempt
         R2  rewarded      the win screen: 2x gold (+50 -> +100); Next is always one tap
         R3  rewarded      a booster at 0: +1 of it, at most r3_gunluk a day
         R4  rewarded      the menu gift: +100 gold, at most r4_gunluk a day
         I1  interstitial  only after a win, on Next, before the corridor (see InterstitialDue)
       No banner. */
    public enum AdPlacement { R1Continue, R2DoubleGold, R3Booster, R4Gift, I1Interstitial }

    public static class AdPlacements
    {
        /* The telemetry name of a placement. */
        public static string Name(AdPlacement p) => p switch
        {
            AdPlacement.R1Continue => "r1_sure",
            AdPlacement.R2DoubleGold => "r2_altin",
            AdPlacement.R3Booster => "r3_booster",
            AdPlacement.R4Gift => "r4_hediye",
            _ => "i1_gecis",
        };

        public static bool Rewarded(AdPlacement p) => p != AdPlacement.I1Interstitial;

        /* Whether the config lets the placement exist at all. */
        public static bool Enabled(LiveConfig c, AdPlacement p) => c.AdsOn && p switch
        {
            AdPlacement.R1Continue => c.R1On,
            AdPlacement.R2DoubleGold => c.R2On,
            AdPlacement.R3Booster => c.R3On,
            AdPlacement.R4Gift => c.R4On,
            _ => c.InterstitialOn,
        };
    }

    /* What the ad rules remember on the device (one JSON value in the store). Times are seconds of a wall clock the
       game passes in (Unix time on the device); the day is the device's local date "yyyy-MM-dd". */
    public sealed class AdState
    {
        public int WinsSinceInterstitial;      // wins since the last interstitial (or since the first open)
        public double LastInterstitialAt = -1; // -1: never
        public string Day = "";
        public int R3Today, R4Today;

        public static AdState Parse(string json)
        {
            var s = new AdState();
            if (string.IsNullOrEmpty(json)) return s;
            try
            {
                var o = JsonReader.Parse(json);
                if (o.TryGet("wins", out var w) && w.Kind == JsonKind.Number) s.WinsSinceInterstitial = (int)w.AsNumber;
                if (o.TryGet("last", out var l) && l.Kind == JsonKind.Number) s.LastInterstitialAt = l.AsNumber;
                if (o.TryGet("day", out var d) && d.Kind == JsonKind.String) s.Day = d.AsString;
                if (o.TryGet("r3", out var r3) && r3.Kind == JsonKind.Number) s.R3Today = (int)r3.AsNumber;
                if (o.TryGet("r4", out var r4) && r4.Kind == JsonKind.Number) s.R4Today = (int)r4.AsNumber;
            }
            catch (JsonException) { }
            return s;
        }

        public string ToJson() => new JsonWriter().BeginObject().Field("wins", WinsSinceInterstitial).Field("last", LastInterstitialAt)
            .Field("day", Day).Field("r3", R3Today).Field("r4", R4Today).EndObject().ToString();

        /* A new day resets the daily counters. */
        public void Today(string day)
        {
            if (day == Day) return;
            Day = day; R3Today = R4Today = 0;
        }
    }

    public static class AdRules
    {
        /* I1 after a won level (wonLevel: its 1-based number), on Next, before the corridor. Never after a loss; from
           gecis_baslangic_bolum on; at least gecis_her_n_bolum wins and gecis_min_sn seconds since the last one; never in
           the first gecis_ilk_oturum_sessiz_dk minutes of the player's first session. */
        public static bool InterstitialDue(LiveConfig c, AdState s, int wonLevel, bool afterLoss, double now,
                                           bool firstSession, double sessionStart)
        {
            if (!AdPlacements.Enabled(c, AdPlacement.I1Interstitial) || afterLoss) return false;
            if (wonLevel < c.InterstitialFromLevel) return false;
            if (s.WinsSinceInterstitial < c.InterstitialEveryN) return false;
            if (s.LastInterstitialAt >= 0 && now - s.LastInterstitialAt < c.InterstitialMinSeconds) return false;
            if (firstSession && now - sessionStart < c.FirstSessionQuietMinutes * 60.0) return false;
            return true;
        }

        /* Every win counts towards the next interstitial. */
        public static void Won(AdState s) => s.WinsSinceInterstitial++;

        public static void InterstitialShown(AdState s, double now)
        {
            s.WinsSinceInterstitial = 0;
            s.LastInterstitialAt = now;
        }

        /* R3 / R4: the daily caps (R1 and R2 have none: once per attempt / per win screen). */
        public static bool UnderDailyCap(LiveConfig c, AdState s, AdPlacement p, string day)
        {
            s.Today(day);
            return p == AdPlacement.R3Booster ? s.R3Today < c.R3Daily : p == AdPlacement.R4Gift ? s.R4Today < c.R4Daily : true;
        }

        public static void Rewarded(AdState s, AdPlacement p, string day)
        {
            s.Today(day);
            if (p == AdPlacement.R3Booster) s.R3Today++;
            else if (p == AdPlacement.R4Gift) s.R4Today++;
        }

        /* Whether a rewarded button shows: allowed by the config and the caps, and an ad is loaded. Never a dead button. */
        public static bool ShowRewardButton(LiveConfig c, AdState s, AdPlacement p, string day, bool adReady) =>
            adReady && AdPlacements.Enabled(c, p) && UnderDailyCap(c, s, p, day);
    }
}
