using UnityEngine;

namespace ReverseSolver.Presentation
{
    public enum FlickState { None, Pending, Move, Cancel }

    public readonly struct FlickResult
    {
        public readonly FlickState State;
        // Layout coordinates: right +x, down +y. No dependency on Core rules.
        public readonly Vector2 Direction;
        public FlickResult(FlickState state, Vector2 direction = default)
        { State = state; Direction = direction; }
    }

    /// <summary>One touch, measured in CSS points. Create a new instance on press.</summary>
    public sealed class FlickGesture
    {
        public const float DeadZone = 8f, MaxDistance = 18f, CellFraction = .3f,
                           SpeedThreshold = 600f, Ambiguity = .2f;
        bool _locked, _horizontal, _ended;

        public FlickResult Classify(Vector2 start, Vector2 current, Vector2 velocity, bool released, float cell)
        {
            if (_ended) return new FlickResult(FlickState.None);
            var delta = current - start;
            float x = Mathf.Abs(delta.x), y = Mathf.Abs(delta.y), dominant = Mathf.Max(x, y);
            if (delta.magnitude < DeadZone)
                return Finish(released ? FlickState.None : FlickState.Pending, released);
            if (Mathf.Abs(x - y) <= dominant * Ambiguity)
                return Finish(released ? FlickState.Cancel : FlickState.Pending, released);
            if (!_locked) { _horizontal = x > y; _locked = true; }
            float along = _horizontal ? delta.x : delta.y;
            var direction = _horizontal ? new Vector2(Mathf.Sign(along), 0) : new Vector2(0, Mathf.Sign(along));
            float travel = Mathf.Abs(along);
            bool enough = travel >= Mathf.Min(CellFraction * cell, MaxDistance);
            float speed = Vector2.Dot(velocity, direction);
            if (enough && (speed >= SpeedThreshold || released))
            { _ended = true; return new FlickResult(FlickState.Move, direction); }
            if (released) return Finish(FlickState.Cancel, true);
            return new FlickResult(FlickState.Pending, direction);
        }

        FlickResult Finish(FlickState state, bool ended)
        { _ended = ended; return new FlickResult(state); }
    }
}
