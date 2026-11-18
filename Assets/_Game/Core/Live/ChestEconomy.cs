using System;
using System.Collections.Generic;

namespace ReverseSolver.Core
{
    public enum ChestKind { None, Normal, Count }

    public sealed class ChestReward
    {
        public ChestKind Kind;
        public int Coins;
        public Booster[] Boosters = new Booster[0];
    }

    /* PM_83 §2: the chest economy of the sl100 game (the numbers in live/config.json).
         normal chest   every 5th level (5, 10, 15 …): 1–20 100 gold, 21–50 150, 51–100 200; from level 10 + 1 booster
         Count's chest  a very hard level won (19, 29 … 99): twice the normal gold + 2 boosters
         hard level     (14, 24 … 94): no chest, +100 gold on the win screen ("Hard level bonus")
       Boosters only from those unlocked by this level (the set's unlock levels), the least held weighted most. */
    public static class ChestEconomy
    {
        public static ChestKind KindOf(LevelData l, LiveConfig c)
        {
            if (l == null) return ChestKind.None;
            if (l.Role == "veryhard") return ChestKind.Count;
            return c.ChestEvery > 0 && l.Number > 0 && l.Number % c.ChestEvery == 0 ? ChestKind.Normal : ChestKind.None;
        }

        /* The normal chest's gold for a level: the first tier whose upper bound the level is within, else the last. */
        public static int TierCoins(int level, LiveConfig c)
        {
            var coins = c.ChestCoins;
            if (coins == null || coins.Length == 0) return 0;
            var bounds = c.ChestTierUpTo ?? new int[0];
            for (int i = 0; i < bounds.Length && i < coins.Length; i++)
                if (level <= bounds[i]) return coins[i];
            return coins[Math.Min(bounds.Length, coins.Length - 1)];
        }

        public static int HardBonus(LevelData l, LiveConfig c) => l != null && l.Role == "hard" ? c.HardBonus : 0;

        /* The boosters this level has unlocked (its unlock level reached), in the game's order. */
        public static List<Booster> Unlocked(LevelData l)
        {
            var list = new List<Booster>();
            if (l?.Unlock == null) return list;
            foreach (var b in BoosterExt.All)
                if (l.Unlock.TryGetValue(b.Id(), out int at) && at <= l.Number) list.Add(b);
            return list;
        }

        /* n boosters from the unlocked ones; weight 1/(1+held)^2, so the least held comes most often; each pick counts
           as held for the next one. random: [0, 1). */
        public static Booster[] Pick(int n, IList<Booster> unlocked, int[] stock, Func<double> random)
        {
            if (n <= 0 || unlocked == null || unlocked.Count == 0) return new Booster[0];
            var held = new int[4];
            for (int i = 0; i < 4; i++) held[i] = stock != null && i < stock.Length ? Math.Max(0, stock[i]) : 0;
            var picks = new Booster[n];
            for (int k = 0; k < n; k++)
            {
                double total = 0;
                var w = new double[unlocked.Count];
                for (int i = 0; i < unlocked.Count; i++) { double h = 1 + held[(int)unlocked[i]]; w[i] = 1 / (h * h); total += w[i]; }
                double r = Math.Min(Math.Max(random(), 0), .999999) * total;
                int pick = unlocked.Count - 1;
                for (int i = 0; i < unlocked.Count; i++) { if (r < w[i]) { pick = i; break; } r -= w[i]; }
                picks[k] = unlocked[pick];
                held[(int)unlocked[pick]]++;
            }
            return picks;
        }

        public static ChestReward Plan(LevelData l, LiveConfig c, int[] stock, Func<double> random)
        {
            var r = new ChestReward { Kind = KindOf(l, c) };
            if (r.Kind == ChestKind.None) return r;
            int coins = TierCoins(l.Number, c);
            int boosters = l.Number >= c.ChestBoosterFrom ? c.ChestBoosters : 0;
            if (r.Kind == ChestKind.Count) { coins *= c.CountCoinFactor; boosters = c.CountBoosters; }
            r.Coins = coins;
            r.Boosters = Pick(boosters, Unlocked(l), stock, random);
            return r;
        }
    }
}
