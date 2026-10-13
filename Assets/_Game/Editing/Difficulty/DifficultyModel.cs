using System;
using System.Collections.Generic;
using ReverseSolver.Core;

namespace ReverseSolver.Editing
{
    /* The level "kind" a designer picks; look only, never raises the version. */
    public enum Genre { Tanitim, Nefes, Kolay, Orta, Zor, CokZor }

    public static class GenreExt
    {
        public static readonly Genre[] All = { Genre.Tanitim, Genre.Nefes, Genre.Kolay, Genre.Orta, Genre.Zor, Genre.CokZor };

        public static string Label(this Genre g) => g switch
        {
            Genre.Tanitim => "Tanıtım", Genre.Nefes => "Nefes", Genre.Kolay => "Kolay",
            Genre.Orta => "Orta", Genre.Zor => "Zor", _ => "Çok zor"
        };

        /* Stored in designs.json as "genre": the label without Turkish letters. */
        public static string Id(this Genre g) => g switch
        {
            Genre.Tanitim => "tanitim", Genre.Nefes => "nefes", Genre.Kolay => "kolay",
            Genre.Orta => "orta", Genre.Zor => "zor", _ => "cokzor"
        };

        public static bool TryParse(string id, out Genre g)
        {
            foreach (var x in All) if (x.Id() == id) { g = x; return true; }
            g = Genre.Orta;
            return false;
        }
    }

    /* A target band; Min/Max null = open on that side. */
    public readonly struct Band
    {
        public readonly double? Min, Max;
        public Band(double? min, double? max) { Min = min; Max = max; }
        public bool Contains(double v) => (!Min.HasValue || v >= Min.Value - 1e-9) && (!Max.HasValue || v <= Max.Value + 1e-9);
        public override string ToString() =>
            Min.HasValue && Max.HasValue ? $"{Min:0.##}–{Max:0.##}" : Min.HasValue ? $"{Min:0.##}+" : Max.HasValue ? $"< {Max:0.##}" : "—";
    }

    /* The difficulty model's starting values (PRODUCT.md "Zorluk modeli (başlangıç
       değerleri)", from the training set; a hypothesis until playtest data says
       otherwise, constants change only with the PM). Editor only.

         time        T = ceil5(g * s * P), never above 75 s
                     g pieces, P the kind's time share, s seconds per piece for the
                     median player: from structure 0.6 + 0.16 * reading per move,
                     +0.1 with a bomb, 1.35 on the game's first level (from play data
                     comes later, Kanal B)
         fuse        shortest dig + the kind's bomb margin
         nails       counter = pieces * share: easy 20-25 %, medium 30 %, hard the
                     first 25-30 % and the next 35-40 %; never above 40 %
         stars       3 with >= 45 % of the time left, 2 with >= 20 %, else 1; the
                     median player keeps 1 - 1/P of the time
         reading     sum over moves of (n + 1)/(k + 1), n pieces left, k that can leave,
                     divided by the pieces (StructureMetrics)
         bottleneck  a move with 10+ pieces left and only one that can leave */
    public static class DifficultyModel
    {
        public const int MaxTime = 75;
        public const double MaxNailShare = .40;

        public readonly struct Params
        {
            public readonly double S, P;
            public readonly int BombMargin;
            public Params(double s, double p, int margin) { S = s; P = p; BombMargin = margin; }
        }

        public static Params Of(Genre g) => g switch
        {
            Genre.Tanitim => new Params(1.1, 2.2, 10),
            Genre.Nefes => new Params(1.0, 2.0, 8),
            Genre.Kolay => new Params(1.0, 2.0, 8),
            Genre.Orta => new Params(1.1, 1.6, 8),
            Genre.Zor => new Params(1.2, 1.3, 5),
            _ => new Params(1.3, 1.15, 2)
        };

        /* ceil to the next multiple of 5 (a tiny epsilon keeps 50.0000001 at 50). */
        public static int Ceil5(double seconds) => (int)(Math.Ceiling(seconds / 5 - 1e-9) * 5);

        public static int Time(int pieces, double s, double p) => Ceil5(pieces * s * p);

        /* s from structure (the only source tonight besides the kind's own s). */
        public static double SecondsPerPiece(double readingPerMove, bool hasBomb, bool firstLevel)
        {
            if (firstLevel) return 1.35;
            return 0.6 + 0.16 * readingPerMove + (hasBomb ? 0.1 : 0);
        }

        public static int Stars(double timeLeftShare) => timeLeftShare >= .45 ? 3 : timeLeftShare >= .20 ? 2 : 1;

        /* The median player's share of time left: 1 - (g * s) / T. */
        public static double MedianTimeLeft(int pieces, double s, int timer) => timer <= 0 ? 0 : 1 - pieces * s / timer;

        public static int Fuse(int shortestDig, Genre g) => shortestDig + Of(g).BombMargin;

        /* Nail counter for the i-th nail (0-based) of a level of `pieces` pieces. */
        public static int NailCounter(int pieces, Genre g, int index)
        {
            double share = g switch
            {
                Genre.Orta => .30,
                Genre.Zor or Genre.CokZor => index == 0 ? .275 : .375,
                _ => .225                                   // Tanıtım, Nefes, Kolay: 20-25 %
            };
            share = Math.Min(share, MaxNailShare);
            return Math.Max(1, (int)Math.Round(pieces * share, MidpointRounding.AwayFromZero));
        }

        // ---- target bands ------------------------------------------------------------

        public sealed class Bands
        {
            public Band Pieces, OpenPercent, FreeAtStart, Reading, Bottleneck, BombMargin, TimeShare, MedianLeft;
            public bool BombsAllowed = true;
        }

        public static Bands BandsOf(Genre g)
        {
            switch (g)
            {
                case Genre.Orta:
                    return new Bands { Pieces = new Band(29, 36), OpenPercent = new Band(50, 58), FreeAtStart = new Band(6, 8),
                        Reading = new Band(2.7, 3.3), Bottleneck = new Band(0, 1), BombMargin = new Band(8, null),
                        TimeShare = new Band(1.5, 1.8), MedianLeft = new Band(.33, .44) };
                case Genre.Zor:
                    return new Bands { Pieces = new Band(33, 36), OpenPercent = new Band(45, 52), FreeAtStart = new Band(5, 7),
                        Reading = new Band(3.3, 4.0), Bottleneck = new Band(1, 3), BombMargin = new Band(4, 7),
                        TimeShare = new Band(1.25, 1.4), MedianLeft = new Band(.20, .29) };
                case Genre.CokZor:
                    return new Bands { Pieces = new Band(33, 40), OpenPercent = new Band(42, 50), FreeAtStart = new Band(4, 6),
                        Reading = new Band(4.0, null), Bottleneck = new Band(3, null), BombMargin = new Band(1, 3),
                        TimeShare = new Band(1.1, 1.2), MedianLeft = new Band(.09, .17) };
                default:                                    // Kolay; Tanıtım and Nefes use the easy bands
                    // Tanıtım and Nefes may carry a bomb (the level that teaches it), with a wide margin (PM 2026-10-13 07:45)
                    bool bombs = g == Genre.Tanitim || g == Genre.Nefes;
                    return new Bands { Pieces = new Band(20, 30), OpenPercent = new Band(58, 68), FreeAtStart = new Band(8, null),
                        Reading = new Band(null, 2.7), Bottleneck = new Band(0, 0),
                        BombMargin = bombs ? new Band(8, null) : new Band(null, null),
                        TimeShare = new Band(2.0, null), MedianLeft = new Band(.50, null), BombsAllowed = bombs };
            }
        }

        // ---- one level ------------------------------------------------------------------

        public sealed class Measure
        {
            public string Name, Value, BandText, Hint;
            public bool InBand;
            public bool HasBand;
        }

        public sealed class Report
        {
            public Genre Genre;
            public int Pieces;
            public double S;
            public string SSource;
            public int SuggestedTime;
            public bool TimeCapped;
            public double TimeShareNow;                      // T / (g * s) for the level's own timer
            public double MedianLeftNow, MedianLeftSuggested;
            public int StarsNow, StarsSuggested;
            public readonly List<(int piece, int fuse, int suggested, bool exact)> Fuses = new List<(int, int, int, bool)>();
            public readonly List<(int piece, int counter, int suggested)> Nails = new List<(int, int, int)>();
            public readonly List<Measure> Measures = new List<Measure>();
            public readonly List<string> Warnings = new List<string>();
            public double RandomWinRate;
        }

        public static Report Analyze(LevelData level, Genre genre, StructureMetrics m, double randomWinRate, bool firstLevel = false)
        {
            var p = Of(genre);
            var bands = BandsOf(genre);
            var st = LevelStats.Of(level);
            int g = level.Pieces.Count;
            bool hasBomb = level.Bombs.Count > 0;
            var r = new Report { Genre = genre, Pieces = g, RandomWinRate = randomWinRate };

            r.S = SecondsPerPiece(m.ReadingPerMove, hasBomb, firstLevel);
            r.SSource = firstLevel ? "oyunun ilk bölümü (1,35)"
                : $"yapıdan: 0,6 + 0,16 × okuma {m.ReadingPerMove:0.00}" + (hasBomb ? " + 0,1 (bomba)" : "");
            int t = Time(g, r.S, p.P);
            r.TimeCapped = t > MaxTime;
            r.SuggestedTime = Math.Min(t, MaxTime);
            r.TimeShareNow = g * r.S <= 0 ? 0 : level.TimerSeconds / (g * r.S);
            r.MedianLeftNow = MedianTimeLeft(g, r.S, level.TimerSeconds);
            r.MedianLeftSuggested = MedianTimeLeft(g, r.S, r.SuggestedTime);
            r.StarsNow = Stars(r.MedianLeftNow);
            r.StarsSuggested = Stars(r.MedianLeftSuggested);

            foreach (var b in level.Bombs)
                if (m.BombDigs.TryGetValue(b.Key, out var dig))
                    r.Fuses.Add((b.Key, b.Value, Fuse(dig.dig, genre), dig.exact));
            var nailed = new List<int>(level.Nails.Keys);
            nailed.Sort();
            for (int i = 0; i < nailed.Count; i++)
                r.Nails.Add((nailed[i], level.Nails[nailed[i]], NailCounter(g, genre, i)));

            void Add(string name, double value, string text, Band band, bool hasBand, string hint)
                => r.Measures.Add(new Measure { Name = name, Value = text, BandText = hasBand ? band.ToString() : "—",
                                                InBand = !hasBand || band.Contains(value), HasBand = hasBand, Hint = hint });
            Add("Gövde", g, g.ToString(), bands.Pieces, true, "Parça sayısı (zincirli çift tek gövde).");
            Add("Açık dikiş %", st.OpenPercent, st.OpenPercent.ToString(), bands.OpenPercent, true, "Düz (çıkıntısız) iç kenarların oranı.");
            Add("İlk hamlede serbest (k₀)", m.FreeAtStart, m.FreeAtStart.ToString(), bands.FreeAtStart, true, "Başlangıçta çıkabilen, çivisiz parça sayısı.");
            Add("Hamle başına okuma", m.ReadingPerMove, m.ReadingPerMove.ToString("0.00"), bands.Reading, true,
                "Her hamlede (kalan + 1) / (serbest + 1) toplamı, gövdeye bölünmüş: yüksekse oyuncu daha çok tahta okur.");
            Add("Darboğaz", m.Bottlenecks10, m.Bottlenecks10.ToString(), bands.Bottleneck, true, "10+ parça kalmışken yalnızca 1 parçanın çıkabildiği hamle sayısı.");
            if (hasBomb)
            {
                double margin = m.FuseMargin;
                bool ok = bands.BombsAllowed && bands.BombMargin.Contains(margin);
                r.Measures.Add(new Measure { Name = "Bomba payı", Value = (m.FuseMarginExact ? "" : "≥") + margin.ToString("0"),
                    BandText = bands.BombsAllowed ? bands.BombMargin.ToString() : "bomba yok", HasBand = true, InBand = ok,
                    Hint = "Fitil − en kısa kazma yolu (en sıkı bomba)." + (m.FuseMarginExact ? "" : " Yaklaşık: çözücünün yolu kullanıldı.") });
            }
            Add("Süre payı", r.TimeShareNow, r.TimeShareNow.ToString("0.00"), bands.TimeShare, true, "Süre / (gövde × s).");
            Add("Medyan oyuncunun kalan süresi", r.MedianLeftNow, $"%{r.MedianLeftNow * 100:0}", bands.MedianLeft, true, "1 − gövde × s / süre.");
            r.Measures.Add(new Measure { Name = "Rastgele oyuncunun kazanması", Value = $"%{randomWinRate * 100:0}", BandText = "—",
                HasBand = false, InBand = true, Hint = "Çivilere uyan rastgele oyuncu (Monte Carlo); yalnızca bomba baskısını gösterir." });

            if (level.TimerSeconds > MaxTime) r.Warnings.Add($"Süre {level.TimerSeconds} sn: 75 sn'yi geçmemeli.");
            if (r.TimeCapped) r.Warnings.Add($"Formül {t} sn veriyor; öneri 75 sn'ye indirildi.");
            foreach (var n in r.Nails)
                if (n.counter > g * MaxNailShare) r.Warnings.Add($"Çivi sayacı {n.counter}, gövdenin %40'ını ({g * MaxNailShare:0}) geçiyor.");
            if (!bands.BombsAllowed && hasBomb) r.Warnings.Add($"{genre.Label()} türünde bomba olmamalı.");
            return r;
        }
    }
}
