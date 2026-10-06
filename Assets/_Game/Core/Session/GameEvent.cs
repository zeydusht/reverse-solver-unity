using System;

namespace ReverseSolver.Core
{
    public enum Booster { Scissors, Wand, Hammer, Clock }

    public static class BoosterExt
    {
        public static readonly Booster[] All = { Booster.Scissors, Booster.Wand, Booster.Hammer, Booster.Clock };

        /* Ids used in the level file and in telemetry (b_scissors, ...). */
        public static string Id(this Booster b) => b switch
        {
            Booster.Scissors => "scissors",
            Booster.Wand => "wand",
            Booster.Hammer => "hammer",
            _ => "clock"
        };
    }

    public enum Outcome { None, Win, Time, Bomb, Quit }

    public static class OutcomeExt
    {
        /* Values of the `result` column in the plays table. */
        public static string Id(this Outcome o) => o switch
        {
            Outcome.Win => "win",
            Outcome.Time => "time",
            Outcome.Bomb => "bomb",
            Outcome.Quit => "quit",
            _ => ""
        };
    }

    /* Why a command was refused. Refused commands change nothing and raise no event. */
    public enum CommandResult
    {
        Ok,
        GameOver,        // the attempt has already ended
        NoSuchPiece,     // not a piece, or already removed
        Pinned,          // nail counter above zero
        Blocked,         // no exit in that direction
        Locked,          // booster not unlocked on this level
        OutOfStock,      // booster count is zero
        NotAJoint,       // scissors: not a tabbed joint between two different pieces
        NoArrangement    // wand: no solvable arrangement found; nothing spent
    }

    public enum EventKind
    {
        PieceRemoved,    // Piece, Dir (Dir is meaningless when ByHammer)
        Jammed,          // Piece
        NailsTicked,     // every nail counter above zero went down by one
        NailFreed,       // Piece: its counter reached zero on a tick
        ValveReleased,   // Piece: safety valve pulled the longest-waiting nail
        BombsTicked,     // every fuse on a present piece went down by one
        BombDefused,     // Piece: a bomb left the board with its piece
        TimeTick,        // Value = seconds left
        TimeAdded,       // Value = seconds added
        JointCut,        // X, Y, Vertical
        JointsReshuffled,
        BoosterUsed,     // Booster
        Finished         // Outcome
    }

    /* One thing that happened in a session. Presentation listens to these; the
       full list is also kept so a replay can be compared event by event. */
    public readonly struct GameEvent : IEquatable<GameEvent>
    {
        public readonly EventKind Kind;
        public readonly int Piece;
        public readonly Dir Dir;
        public readonly bool ByHammer;
        public readonly int Value;
        public readonly int X, Y;
        public readonly bool Vertical;
        public readonly Booster Booster;
        public readonly Outcome Outcome;

        GameEvent(EventKind kind, int piece = -1, Dir dir = Dir.U, bool byHammer = false, int value = 0,
                  int x = 0, int y = 0, bool vertical = false, Booster booster = Booster.Scissors,
                  Outcome outcome = Outcome.None)
        {
            Kind = kind; Piece = piece; Dir = dir; ByHammer = byHammer; Value = value;
            X = x; Y = y; Vertical = vertical; Booster = booster; Outcome = outcome;
        }

        internal static GameEvent Removed(int piece, Dir dir, bool hammer) => new GameEvent(EventKind.PieceRemoved, piece, dir, hammer);
        internal static GameEvent Jam(int piece) => new GameEvent(EventKind.Jammed, piece);
        internal static GameEvent Simple(EventKind kind) => new GameEvent(kind);
        internal static GameEvent ForPiece(EventKind kind, int piece) => new GameEvent(kind, piece);
        internal static GameEvent Time(EventKind kind, int value) => new GameEvent(kind, value: value);
        internal static GameEvent Cut(int x, int y, bool vertical) => new GameEvent(EventKind.JointCut, x: x, y: y, vertical: vertical);
        internal static GameEvent Used(Booster b) => new GameEvent(EventKind.BoosterUsed, booster: b);
        internal static GameEvent End(Outcome o) => new GameEvent(EventKind.Finished, outcome: o);

        public bool Equals(GameEvent o) =>
            Kind == o.Kind && Piece == o.Piece && Dir == o.Dir && ByHammer == o.ByHammer && Value == o.Value &&
            X == o.X && Y == o.Y && Vertical == o.Vertical && Booster == o.Booster && Outcome == o.Outcome;
        public override bool Equals(object obj) => obj is GameEvent e && Equals(e);
        public override int GetHashCode() => ((int)Kind * 397) ^ Piece ^ (Value << 8);

        public override string ToString() => Kind switch
        {
            EventKind.PieceRemoved => ByHammer ? $"Removed {Piece} by hammer" : $"Removed {Piece} {Dir}",
            EventKind.Jammed or EventKind.NailFreed or EventKind.ValveReleased or EventKind.BombDefused => $"{Kind} {Piece}",
            EventKind.TimeTick or EventKind.TimeAdded => $"{Kind} {Value}",
            EventKind.JointCut => $"JointCut {(Vertical ? "V" : "H")}({X},{Y})",
            EventKind.BoosterUsed => $"BoosterUsed {Booster.Id()}",
            EventKind.Finished => $"Finished {Outcome.Id()}",
            _ => Kind.ToString()
        };
    }
}
