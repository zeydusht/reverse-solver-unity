using System;
using System.Collections.Generic;
using ReverseSolver.Editing;

namespace ReverseSolver.Generation
{
    /* What one slot of the redesigned set may contain (PRODUCT.md "Faz 4 tanımı",
       docs/FAZ4_SEVIYE_FABRIKASI.md "Sıra planı"). The generator only ever draws
       inside these limits; the difficulty filter (G2) judges what comes out. */
    public sealed class SlotSpec
    {
        // "Yeni şey" ids: obstacles change the board, boosters only arrive with an intro card.
        public const string Nail = "nail", Scissors = "scissors", Seal = "seal", Wand = "wand",
                            Chain = "chain", Clock = "clock", Bomb = "bomb", Hammer = "hammer";

        /* The slot each new thing is introduced in (PM 2026-10-13 ~18:40). */
        public static readonly IReadOnlyDictionary<string, int> IntroducedAt = new Dictionary<string, int>
        {
            [Nail] = 3, [Scissors] = 5, [Seal] = 7, [Wand] = 9, [Chain] = 12, [Clock] = 15, [Bomb] = 20, [Hammer] = 23
        };

        public static string Label(string id) => id switch
        {
            Nail => "çivi", Scissors => "makas", Seal => "mühür", Wand => "değnek", Chain => "zincir",
            Clock => "saat", Bomb => "bomba", Hammer => "çekiç", null => "", _ => id
        };

        public static bool IsObstacle(string id) => id == Nail || id == Seal || id == Chain || id == Bomb;

        public int Slot;
        public Genre Genre;
        public string NewThing;                          // null when the slot brings nothing new

        public (int w, int h)[] Boards = Array.Empty<(int, int)>();
        public int MinBodies, MaxBodies;                 // "gövde": pieces after chaining
        public int MinNails, MaxNails;
        public int MinSeals, MaxSeals;                   // wall segments (side, lane)
        public int MinChains, MaxChains;                 // all two-cell pieces
        public int MinRemote, MaxRemote;                 // of those, chains between cells that do not touch
        public int MinBombs, MaxBombs;

        public bool Allows(string thing) => IntroducedAt.TryGetValue(thing, out int at) && Slot >= at;

        public override string ToString() =>
            $"slot {Slot} {Genre.Label()}{(NewThing != null ? " +" + NewThing : "")} bodies {MinBodies}-{MaxBodies} " +
            $"nails {MinNails}-{MaxNails} seals {MinSeals}-{MaxSeals} chains {MinChains}-{MaxChains} (remote {MinRemote}-{MaxRemote}) bombs {MinBombs}-{MaxBombs}";

        /* Default limits from the genre's bands (DifficultyModel.BandsOf) and the
           introduction calendar: nothing appears before its slot, an introduction
           slot shows its new thing alone (one new thing per level), 1–11 have
           single-cell pieces only, bombs from 20. */
        public static SlotSpec For(int slot, Genre genre, string newThing = null)
        {
            var s = new SlotSpec { Slot = slot, Genre = genre, NewThing = newThing };
            var bodies = DifficultyModel.BandsOf(genre).Pieces;
            s.MinBodies = (int)bodies.Min.Value;
            s.MaxBodies = (int)bodies.Max.Value;
            s.Boards = genre switch
            {
                Genre.Tanitim or Genre.Nefes or Genre.Kolay => new[] { (4, 5), (5, 5), (5, 6) },
                Genre.Orta => new[] { (5, 6), (6, 6) },
                Genre.Zor => new[] { (6, 6) },
                _ => new[] { (6, 6), (6, 7) },
            };
            if (slot <= 2) s.Boards = new[] { (4, 5) };

            bool easy = genre == Genre.Tanitim || genre == Genre.Nefes || genre == Genre.Kolay;
            int Upto(int easyMax, int ortaMax, int zorMax, int cokMax) =>
                easy ? easyMax : genre == Genre.Orta ? ortaMax : genre == Genre.Zor ? zorMax : cokMax;

            if (s.Allows(Nail)) { s.MinNails = easy ? 0 : 1; s.MaxNails = Upto(1, 2, 2, 2); }
            if (s.Allows(Seal)) { s.MinSeals = genre == Genre.Zor || genre == Genre.CokZor ? 1 : 0; s.MaxSeals = Upto(1, 2, 2, 3); }
            // chains: the band's "uzak zincir" row (Kolay 0, Orta 0-1, Zor 1-2, Çok zor 2-3); all chains at
            // least that many, easy kinds one adjacent pair at most (PM 2026-10-13 ~19:10)
            if (s.Allows(Chain))
            {
                (s.MinRemote, s.MaxRemote) = easy ? (0, 0) : genre == Genre.Orta ? (0, 1) : genre == Genre.Zor ? (1, 2) : (2, 3);
                (s.MinChains, s.MaxChains) = easy ? (0, 1) : genre == Genre.Orta ? (0, 2) : genre == Genre.Zor ? (1, 3) : (2, 4);
            }
            if (s.Allows(Bomb) && genre != Genre.Kolay)
            {
                // Çok zor: the bomb is optional (PM 2026-10-14 gece (2), A5)
                s.MinBombs = genre == Genre.Zor ? 1 : 0;
                s.MaxBombs = genre == Genre.CokZor ? 2 : 1;
            }

            // slot 4 practises the nail right after its introduction (PM 2026-10-14 gece (2))
            if (slot == 4) s.MinNails = s.MaxNails = 1;

            // An introduction shows its obstacle exactly once and nothing else new to read.
            if (genre == Genre.Tanitim && newThing != null)
            {
                s.MinNails = s.MaxNails = newThing == Nail ? 1 : 0;
                s.MinSeals = s.MaxSeals = newThing == Seal ? 1 : 0;
                // the chain introduction shows both kinds: one adjacent pair and one remote pair (the only
                // intro with two pieces of its obstacle; PM 2026-10-13 ~19:10)
                s.MinChains = s.MaxChains = newThing == Chain ? 2 : 0;
                s.MinRemote = s.MaxRemote = newThing == Chain ? 1 : 0;
                s.MinBombs = s.MaxBombs = newThing == Bomb ? 1 : 0;
            }
            else if (newThing != null && IsObstacle(newThing))
                throw new ArgumentException($"slot {slot}: an obstacle is introduced in a Tanıtım slot");
            return s;
        }
    }
}
