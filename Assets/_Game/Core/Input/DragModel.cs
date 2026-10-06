using System;

namespace ReverseSolver.Core
{
    /* What a drag can act on. GameSession implements it through SessionDragTarget;
       tests can supply their own. */
    public interface IDragTarget
    {
        bool IsOver { get; }
        bool IsPresent(int piece);
        bool IsPinned(int piece);
        TravelLimit Probe(int piece, Dir dir);
        CommandResult Exit(int piece, Dir dir);
        void Jam(int piece);
    }

    public sealed class SessionDragTarget : IDragTarget
    {
        readonly GameSession _s;
        public SessionDragTarget(GameSession s) { _s = s; }
        public bool IsOver => _s.IsOver;
        public bool IsPresent(int piece) => piece >= 0 && piece < _s.Level.Pieces.Count && _s.Board.IsPresent(piece);
        public bool IsPinned(int piece) => _s.NailAt(piece) > 0;
        public TravelLimit Probe(int piece, Dir dir) => _s.Probe(piece, dir);
        public CommandResult Exit(int piece, Dir dir) => _s.TryExit(piece, dir);
        public void Jam(int piece) => _s.RecordJam(piece);
    }

    public enum DragRelease { None, Exited, SnappedBack }

    /* The web game's drag (index.html: pointerdown / pointermove / endDrag),
       without the DOM. Distances are in the same units as CellSize (points).

         - Pressing a nailed piece refuses the drag and counts a jam.
         - The axis is chosen once the finger has moved MinSwipe, by the larger
           component; after that the sign of that component picks the direction.
         - The piece follows the finger up to its travel limit. A piece that can
           leave follows to (cells + 1.15) cells; a blocked one gives a little
           past the stop: 0.30 cell when it catches on a tab, 0.07 when it hits
           something solid.
         - Pulling more than 4 points past that limit counts one jam, once per
           drag and direction.
         - On release, a piece that can leave and has travelled 42% of the exit
           distance leaves; anything else springs back. */
    public sealed class DragModel
    {
        public const float MinSwipe = 7f;
        public const float GiveSoft = .30f;
        public const float GiveHard = .07f;
        public const float ExitOvershoot = 1.15f;
        public const float ExitFraction = .42f;
        public const float JamSlack = 4f;

        readonly IDragTarget _target;

        public float CellSize { get; set; }
        public int Piece { get; private set; } = -1;
        public bool Active => Piece >= 0;
        public Dir? Direction { get; private set; }
        public TravelLimit Limit { get; private set; }
        /* Current displacement along Direction, never negative. */
        public float Offset { get; private set; }
        /* The piece currently stopping the drag, -1 if none (for highlighting). */
        public int Blocker { get; private set; } = -1;

        float _sx, _sy;
        bool _axisChosen, _horizontal, _jammed;

        public DragModel(IDragTarget target, float cellSize)
        {
            _target = target ?? throw new ArgumentNullException(nameof(target));
            CellSize = cellSize;
        }

        /* Returns false if no drag starts (nothing there, game over, or nailed). */
        public bool Press(int piece, float x, float y)
        {
            if (Active) Cancel();
            if (_target.IsOver || !_target.IsPresent(piece)) return false;
            if (_target.IsPinned(piece))
            {
                _target.Jam(piece);                      // web: a press on a pinned piece is a jam
                return false;
            }
            Piece = piece;
            _sx = x; _sy = y;
            _axisChosen = false;
            _jammed = false;
            Direction = null;
            Offset = 0;
            Blocker = -1;
            return true;
        }

        public void Move(float x, float y)
        {
            if (!Active) return;
            float dx = x - _sx, dy = y - _sy;
            if (!_axisChosen)
            {
                if (Math.Sqrt(dx * dx + dy * dy) < MinSwipe) return;
                _axisChosen = true;
                _horizontal = Math.Abs(dx) > Math.Abs(dy);
            }
            float raw = _horizontal ? dx : dy;
            Dir dir = _horizontal ? (raw < 0 ? Dir.L : Dir.R) : (raw < 0 ? Dir.U : Dir.D);
            if (Direction != dir)
            {
                Direction = dir;
                Limit = _target.Probe(Piece, dir);
                _jammed = false;
                Blocker = -1;
            }

            float max = MaxOffset();
            float want = Math.Abs(raw);
            Offset = Math.Min(want, max);
            if (!Limit.Exit && want > max + JamSlack && !_jammed)
            {
                _jammed = true;
                Blocker = Limit.Blocker;
                _target.Jam(Piece);
            }
        }

        public DragRelease Release(out int piece, out Dir dir)
        {
            piece = Piece;
            dir = Direction ?? Dir.U;
            if (!Active) return DragRelease.None;
            bool leaves = Direction.HasValue && Limit.Exit &&
                          Offset >= (Limit.Cells + ExitOvershoot) * CellSize * ExitFraction;
            Piece = -1;
            Blocker = -1;
            if (leaves && _target.Exit(piece, dir) == CommandResult.Ok) return DragRelease.Exited;
            return DragRelease.SnappedBack;
        }

        /* Pointer lost (cancel, blur): springs back without acting. */
        public void Cancel()
        {
            Piece = -1;
            Blocker = -1;
            Offset = 0;
            Direction = null;
        }

        float MaxOffset()
        {
            if (Limit.Exit) return (Limit.Cells + ExitOvershoot) * CellSize;
            float give = Limit.Hard ? GiveHard : GiveSoft;
            return (Limit.Cells + give) * CellSize;
        }
    }
}
