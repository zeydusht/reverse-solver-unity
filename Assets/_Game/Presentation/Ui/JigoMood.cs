namespace ReverseSolver.Presentation
{
    public enum JigoState { Normal, Tedirgin, Korku, Mutlu, Uzgun }

    public static class JigoMood
    {
        public const int WorriedSeconds = 10, FearSeconds = 3, FearFuse = 1;
        public const float HappySeconds = .6f, FadeSeconds = .15f, FramesPerSecond = 8f;

        public static JigoState Read(float timeLeft, int? minBombFuse, bool recentHappy, bool lost, bool won)
        {
            if (lost) return JigoState.Uzgun;
            if (timeLeft <= FearSeconds || minBombFuse == FearFuse) return JigoState.Korku;
            if (timeLeft <= WorriedSeconds) return JigoState.Tedirgin;
            if (recentHappy) return JigoState.Mutlu;
            return JigoState.Normal;
        }
    }
}
