namespace ReverseSolver.Presentation
{
    /// <summary>Win events queue a single travel for the next visible menu, never losses or startup.</summary>
    public sealed class CorridorVictoryQueue
    {
        public bool Pending { get; private set; }
        public float StartedAt { get; private set; } = -1f;
        public void Won() { Pending = true; }
        public void EnterMenu(float now)
        {
            if (!Pending) return;
            Pending = false;
            StartedAt = now;
        }
        public void LeaveMenu() { StartedAt = -1f; }
        public bool Playing(float now) => StartedAt >= 0 && now - StartedAt < CorridorVictory.Duration;
    }
}
