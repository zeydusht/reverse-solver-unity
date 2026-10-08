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

    /* How the last drag ended, for the ?debug=1 line (PRODUCT.md K1: is it a
       cancel, too short a pull, or a press that missed the piece?). */
    public sealed class DragReport
    {
        public DragRelease Result;
        public Dir? Direction;
        public float Offset;          // points travelled along Direction
        public float Needed;          // points needed to leave by the 42% rule (0: cannot leave that way)
        public float Speed;           // finger speed along Direction at release, pt/s
        public bool Flick;            // left by the flick rule
        public bool Cancelled;        // ended by the system (touch cancel, focus loss)
        public bool PressedOutside;   // grabbed from the margin outside the board

        public override string ToString() =>
            $"{(Cancelled ? "Cancel→" : "")}{Result}{(Flick ? " (fiske)" : "")} " +
            $"{Offset:0}/{Needed:0} pt {Speed:0} pt/s {(PressedOutside ? "dışarıdan" : "içeriden")}";
    }

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
           distance leaves; anything else springs back.

       Deliberate differences from the web (PRODUCT.md K1, from Zeyd's iPhone:
       pieces in the outer columns were hard to pull out past the screen edge):
         - Flick: a piece that can leave also leaves when the finger moves at
           FlickSpeed or more in that direction at release and the piece has
           travelled FlickFraction of the exit distance.
         - A drag the system cancels (edge gesture, focus loss) leaves if the
           42% rule holds at that moment, otherwise springs back (Abort).
       Neither counts a jam. Time comes in from outside (Move/Release take a
       timestamp in seconds); without one the flick never applies. */
    public sealed class DragModel
    {
        public const float MinSwipe = 7f;
        public const float GiveSoft = .30f;
        public const float GiveHard = .07f;
        public const float ExitOvershoot = 1.15f;
        public const float ExitFraction = .42f;
        public const float JamSlack = 4f;
        public const float FlickSpeed = 900f;        // pt/s along the exit direction
        public const float FlickFraction = .20f;
        public const double SpeedWindow = .10;       // s of movement the release speed is measured over

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
        /* How the previous drag ended; null before the first one. */
        public DragReport Last { get; private set; }

        float _sx, _sy;
        bool _axisChosen, _horizontal, _jammed, _outside;
        readonly float[] _px = new float[16], _py = new float[16];
        readonly double[] _pt = new double[16];
        int _samples;

        public DragModel(IDragTarget target, float cellSize)
        {
            _target = target ?? throw new ArgumentNullException(nameof(target));
            CellSize = cellSize;
        }

        /* Returns false if no drag starts (nothing there, game over, or nailed). */
        public bool Press(int piece, float x, float y) => Press(piece, x, y, -1, false);

        /* time: seconds (any origin), -1 if unknown. outside: grabbed from the margin. */
        public bool Press(int piece, float x, float y, double time, bool outside)
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
            _outside = outside;
            _samples = 0;
            Sample(x, y, time);
            return true;
        }

        public void Move(float x, float y) => Move(x, y, -1);

        public void Move(float x, float y, double time)
        {
            if (!Active) return;
            Sample(x, y, time);
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

        public DragRelease Release(out int piece, out Dir dir) => End(out piece, out dir, flickAllowed: true, cancelled: false);

        /* The system ended the touch (touchcancel, focus loss): the piece leaves
           if the 42% rule holds right now, otherwise it springs back. No flick. */
        public DragRelease Abort(out int piece, out Dir dir) => End(out piece, out dir, flickAllowed: false, cancelled: true);

        DragRelease End(out int piece, out Dir dir, bool flickAllowed, bool cancelled)
        {
            piece = Piece;
            dir = Direction ?? Dir.U;
            if (!Active) return DragRelease.None;
            float exitDistance = (Limit.Cells + ExitOvershoot) * CellSize;
            bool canLeave = Direction.HasValue && Limit.Exit;
            float speed = Speed();
            bool byRule = canLeave && Offset >= exitDistance * ExitFraction;
            bool byFlick = flickAllowed && canLeave && !byRule &&
                           speed >= FlickSpeed && Offset >= exitDistance * FlickFraction;
            Piece = -1;
            Blocker = -1;
            var result = (byRule || byFlick) && _target.Exit(piece, dir) == CommandResult.Ok
                ? DragRelease.Exited : DragRelease.SnappedBack;
            Last = new DragReport
            {
                Result = result, Direction = Direction, Offset = Offset,
                Needed = canLeave ? exitDistance * ExitFraction : 0, Speed = speed,
                Flick = byFlick && result == DragRelease.Exited, Cancelled = cancelled, PressedOutside = _outside
            };
            return result;
        }

        /* Drag dropped for the game's own reasons (level ends, layout changes):
           springs back without acting. */
        public void Cancel()
        {
            Piece = -1;
            Blocker = -1;
            Offset = 0;
            Direction = null;
        }

        void Sample(float x, float y, double time)
        {
            if (time < 0) return;
            if (_samples == _pt.Length)
            {
                System.Array.Copy(_px, 1, _px, 0, _px.Length - 1);
                System.Array.Copy(_py, 1, _py, 0, _py.Length - 1);
                System.Array.Copy(_pt, 1, _pt, 0, _pt.Length - 1);
                _samples--;
            }
            _px[_samples] = x; _py[_samples] = y; _pt[_samples] = time;
            _samples++;
        }

        /* Finger speed along Direction over the last SpeedWindow, pt/s (0 without timestamps). */
        float Speed()
        {
            if (!Direction.HasValue || _samples < 2) return 0;
            int last = _samples - 1, first = last;
            while (first > 0 && _pt[last] - _pt[first - 1] <= SpeedWindow) first--;
            if (first == last) first = last - 1;
            double dt = _pt[last] - _pt[first];
            if (dt <= 1e-4) return 0;
            var d = Direction.Value;
            float along = d.Dx() * (_px[last] - _px[first]) + d.Dy() * (_py[last] - _py[first]);
            return (float)(along / dt);
        }

        float MaxOffset()
        {
            if (Limit.Exit) return (Limit.Cells + ExitOvershoot) * CellSize;
            float give = Limit.Hard ? GiveHard : GiveSoft;
            return (Limit.Cells + give) * CellSize;
        }
    }
}
