using System;
using System.Collections.Generic;

namespace ReverseSolver.Core
{
    public enum ContinueKind { Time, Bomb }

    public sealed class ContinueOffer
    {
        public ContinueKind Kind { get; }
        public int Price { get; }
        public int Seconds { get; }
        public bool Affordable { get; }
        internal ContinueOffer(ContinueKind kind, int price, int seconds, bool affordable)
        { Kind = kind; Price = price; Seconds = seconds; Affordable = affordable; }
    }

    // Product parameters live here; independent of presentation, storage and Unity.
    public sealed class ContinueRules
    {
        public IReadOnlyList<int> Prices { get; }
        public int Seconds { get; }
        public int ShortSeconds { get; }
        public int ShortLimit { get; }
        public int BombFuse { get; }
        public int WinCoins { get; }
        public int StartCoins { get; }
        /* PM_81: live/config.json's baslangic_altin (the game sets it at start; 5000 by default). */
        public static int DefaultStartCoins = 5000;
        public ContinueRules(int[] prices = null, int seconds = 30, int shortSeconds = 20,
            int shortLimit = 60, int bombFuse = 5, int winCoins = 50, int startCoins = -1)
        {
            if (startCoins < 0) startCoins = DefaultStartCoins;
            Prices = Array.AsReadOnly((int[])(prices ?? new[] { 900, 1900, 2900 }).Clone());
            Seconds = seconds; ShortSeconds = shortSeconds; ShortLimit = shortLimit;
            BombFuse = bombFuse; WinCoins = winCoins; StartCoins = startCoins;
        }
        public ContinueOffer Offer(ContinueKind kind, int continuesUsed, int levelTimer, int coins)
        {
            if (continuesUsed < 0 || continuesUsed >= Math.Min(3, Prices.Count)) return null;
            int price = Prices[continuesUsed];
            return new ContinueOffer(kind, price, kind == ContinueKind.Bomb ? 0
                : levelTimer < ShortLimit ? ShortSeconds : Seconds, coins >= price);
        }
    }
}
