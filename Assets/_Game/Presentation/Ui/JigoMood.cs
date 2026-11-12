namespace ReverseSolver.Presentation
{
    public enum JigoState { Normal, Tedirgin, Korku, Mutlu, Uzgun }

    public static class JigoMood
    {
        public const int WorriedSeconds = 10, FearSeconds = 3, FearFuse = 1;
        public const float HappySeconds = .6f, FadeSeconds = .25f;
        public const float BlinkDuration = .4f;

        public static float BlinkClosure(float age)
        {
            const float close = .14f, open = .215f;
            if (age <= 0 || age >= BlinkDuration) return 0;
            if (age < close) return Smooth(age / close);
            return age <= open ? 1 : Smooth((BlinkDuration - age) / (BlinkDuration - open));
        }

        static float Smooth(float t) => t * t * (3 - 2 * t);

        public static JigoState Read(float timeLeft, int? minBombFuse, bool recentHappy, bool lost, bool won)
        {
            if (lost) return JigoState.Uzgun;
            if (won) return JigoState.Mutlu;
            if (timeLeft <= FearSeconds || minBombFuse == FearFuse) return JigoState.Korku;
            if (timeLeft <= WorriedSeconds) return JigoState.Tedirgin;
            if (recentHappy) return JigoState.Mutlu;
            return JigoState.Normal;
        }
    }
}
