using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using ReverseSolver.Core;

/* PM_83 §2: the gold and booster flow of 1000 virtual players through sl100 (dotnet run -c Release --project Tools/sl100 -- economy).
   The chests come from Core's ChestEconomy with the live config's numbers (LiveConfig.Default, the same as live/config.json).
   Assumptions (the player model; written to the report):
     - first attempt wins with the level's model v2 prediction (docs/analiz/v2/pred_sl100.csv); each retry +0.12, at most 0.97;
     - a booster used in an attempt: +0.10; used on a hard/very hard level with probability 0.6, otherwise 0.15, if one is held;
     - time up: the R1 ad (+30 s, once an attempt) is taken by 40 %, then gold continues 900 / 1900 / 2900 if the gold is there,
       by 50 % / 30 % / 15 %; a continue saves the attempt with 55 %;
     - +50 a win, 30 % watch R2 (2x the 50), the hard bonus, the chests; boosters start at 3 when unlocked (+ the chest bonus kept
       for a booster not yet unlocked), as BoosterInventory does; no R3 / R4 (optional ads), no purchases. */
static class Economy
{
    sealed class Player
    {
        public int Gold, Boosters, GoldFromChests, GoldFromWins, GoldSpent, BoostersFromChests, BoostersUsed, Attempts;
        public int[] Stock = new int[4];
        public int[] Bonus = new int[4];
        public bool[] Unlocked = new bool[4];
    }

    public static void Run(string[] a)
    {
        int players = a.Length > 1 ? int.Parse(a[1]) : 1000;
        var set = LevelParser.Parse(File.ReadAllText("Assets/_Game/Levels/sl100.json"));
        var pred = new Dictionary<int, double>();
        foreach (var line in File.ReadAllLines("docs/analiz/v2/pred_sl100.csv").Skip(1))
        {
            var c = line.Split(',');
            pred[int.Parse(c[0])] = double.Parse(c[9], CultureInfo.InvariantCulture);
        }
        var cfg = LiveConfig.Default;
        int[] marks = { 5, 10, 19, 25, 50, 75, 100 };
        var at = marks.ToDictionary(m => m, m => new List<Player>());
        var rnd = new Random(83);
        int chestCount = 0, countChests = 0;
        foreach (var l in set.Levels) { var k = ChestEconomy.KindOf(l, cfg); if (k != ChestKind.None) chestCount++; if (k == ChestKind.Count) countChests++; }
        for (int n = 0; n < players; n++)
        {
            var p = new Player { Gold = cfg.StartCoins };
            foreach (var l in set.Levels)
            {
                foreach (var b in ChestEconomy.Unlocked(l))
                    if (!p.Unlocked[(int)b]) { p.Unlocked[(int)b] = true; p.Stock[(int)b] = 3 + p.Bonus[(int)b]; p.Bonus[(int)b] = 0; }
                bool hard = l.Role == "hard" || l.Role == "veryhard";
                double p1 = pred.TryGetValue(l.Number, out var v) ? v : .8;
                bool won = false;
                for (int attempt = 0; !won; attempt++)
                {
                    p.Attempts++;
                    double pw = Math.Min(.97, p1 + .12 * attempt);
                    var held = Enumerable.Range(0, 4).Where(i => p.Unlocked[i] && p.Stock[i] > 0).ToList();
                    if (held.Count > 0 && rnd.NextDouble() < (hard ? .6 : .15))
                    {
                        int b = held[rnd.Next(held.Count)];
                        p.Stock[b]--; p.BoostersUsed++; pw = Math.Min(.98, pw + .10);
                    }
                    if (rnd.NextDouble() < pw) { won = true; break; }
                    // time up: R1, then the gold continues
                    if (rnd.NextDouble() < .4 && rnd.NextDouble() < .55) { won = true; break; }
                    for (int k = 0; k < cfg.ContinuePrices.Length && !won; k++)
                    {
                        int price = cfg.ContinuePrices[k];
                        double take = k == 0 ? .5 : k == 1 ? .3 : .15;
                        if (p.Gold < price || rnd.NextDouble() >= take) break;
                        p.Gold -= price; p.GoldSpent += price;
                        if (rnd.NextDouble() < .55) won = true;
                    }
                }
                int win = 50 + (rnd.NextDouble() < .3 ? 50 : 0);
                p.Gold += win; p.GoldFromWins += win;
                int bonus = ChestEconomy.HardBonus(l, cfg);
                p.Gold += bonus; p.GoldFromWins += bonus;
                var stock = (int[])p.Stock.Clone();
                var chest = ChestEconomy.Plan(l, cfg, stock, rnd.NextDouble);
                if (chest.Kind != ChestKind.None)
                {
                    p.Gold += chest.Coins; p.GoldFromChests += chest.Coins;
                    foreach (var b in chest.Boosters)
                    {
                        if (p.Unlocked[(int)b]) p.Stock[(int)b]++; else p.Bonus[(int)b]++;
                        p.BoostersFromChests++;
                    }
                }
                if (at.ContainsKey(l.Number))
                    at[l.Number].Add(new Player
                    {
                        Gold = p.Gold, Boosters = p.Stock.Sum(), GoldFromChests = p.GoldFromChests, GoldFromWins = p.GoldFromWins, GoldSpent = p.GoldSpent,
                        BoostersFromChests = p.BoostersFromChests, BoostersUsed = p.BoostersUsed, Attempts = p.Attempts, Stock = (int[])p.Stock.Clone(),
                    });
            }
        }
        // one player's chests (deterministic parts) for the table of chest contents
        var sb = new StringBuilder();
        sb.AppendLine("# PM_83: Sandık ekonomisi, 1→100 sanal oyuncu");
        sb.AppendLine();
        sb.AppendLine($"`dotnet run -c Release --project Tools/sl100 -- economy {players}`: {players} sanal oyuncu, sl100'ün 100 bölümü, sandıklar oyunun kendi kuralıyla (Core `ChestEconomy`, `live/config.json` sayıları).");
        sb.AppendLine();
        sb.AppendLine($"Sandık: 100 bölümde {chestCount} sandık ({chestCount - countChests} normal, {countChests} Kont).");
        sb.AppendLine();
        sb.AppendLine("| Bölüm sonu | Altın (ort.) | Altın (ortanca) | Elde booster (ort.) | Sandıktan altın | Kazanmadan altın | Devama harcanan | Sandıktan booster | Kullanılan booster | Deneme |");
        sb.AppendLine("|---|---|---|---|---|---|---|---|---|---|");
        foreach (int m in marks)
        {
            var list = at[m];
            double Avg(Func<Player, int> f) => list.Average(f);
            var gold = list.Select(x => x.Gold).OrderBy(x => x).ToList();
            sb.AppendLine($"| {m} | {Avg(x => x.Gold):0} | {gold[gold.Count / 2]} | {Avg(x => x.Boosters):0.0} | {Avg(x => x.GoldFromChests):0} | {Avg(x => x.GoldFromWins):0} | {Avg(x => x.GoldSpent):0} | {Avg(x => x.BoostersFromChests):0.0} | {Avg(x => x.BoostersUsed):0.0} | {Avg(x => x.Attempts):0.0} |");
        }
        sb.AppendLine();
        sb.AppendLine("Booster dağılımı (100. bölüm sonu, ortalama elde): " + string.Join(", ", BoosterExt.All.Select(b => $"{b.Id()} {at[100].Average(x => x.Stock[(int)b]):0.0}")) + ".");
        sb.AppendLine();
        sb.AppendLine("## Sandık içerikleri");
        sb.AppendLine();
        sb.AppendLine("| Bölüm | Sandık | Altın | Booster |");
        sb.AppendLine("|---|---|---|---|");
        foreach (var l in set.Levels)
        {
            var k = ChestEconomy.KindOf(l, cfg);
            if (k == ChestKind.None) { if (ChestEconomy.HardBonus(l, cfg) > 0) sb.AppendLine($"| {l.Number} | zor bölüm bonusu | +{ChestEconomy.HardBonus(l, cfg)} | — |"); continue; }
            var r = ChestEconomy.Plan(l, cfg, new int[4], () => .5);
            sb.AppendLine($"| {l.Number} | {(k == ChestKind.Count ? "Kont" : "normal")} | {r.Coins} | {(r.Boosters.Length == 0 ? "—" : r.Boosters.Length + " (açılmışlardan: " + string.Join(", ", ChestEconomy.Unlocked(l).Select(b => b.Id())) + ")")} |");
        }
        sb.AppendLine();
        sb.AppendLine("## Oyuncu modeli (varsayımlar)");
        sb.AppendLine("- İlk deneme model v2 tahminiyle kazanılır (`docs/analiz/v2/pred_sl100.csv`); her tekrar +0,12 (en çok 0,97).");
        sb.AppendLine("- Booster: zor/çok zor bölümde %60, diğerlerinde %15 kullanılır (elde varsa); kullanılan booster +0,10.");
        sb.AppendLine("- Süre dolunca: %40 reklamla +30 sn (R1, %55 kurtarır); sonra altınla devam 900 / 1900 / 2900, alma olasılığı %50 / %30 / %15, %55 kurtarır.");
        sb.AppendLine("- Kazanma +50; %30'u 2× altın reklamını izler (R2). Zor bölüm bonusu ve sandıklar oyunun kuralıyla. R3/R4 ve satın alma yok.");
        sb.AppendLine("- Booster açılınca 3 ile başlar (BoosterInventory); açılmamış booster sandıktan gelirse açılınca eklenir.");
        File.WriteAllText("docs/art/pm83/EKONOMI.md", sb.ToString());
        Console.WriteLine(sb.ToString().Split('\n').Take(16).Aggregate((x, y) => x + "\n" + y));
    }
}
