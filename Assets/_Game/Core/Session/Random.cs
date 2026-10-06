namespace ReverseSolver.Core
{
    /* Randomness is passed in, never taken from the engine, so a session can be
       replayed exactly from a seed (tests, Monte Carlo, bug reports). */
    public interface IRandom
    {
        /* Uniform in [0, 1), like JavaScript's Math.random. */
        double NextDouble();
    }

    /* mulberry32: tiny, fast and identical to the JavaScript version in
       Tools/web_harness.js, so a seeded C# session and the web game make the
       same draws in the same order. */
    public sealed class Mulberry32 : IRandom
    {
        uint _a;

        public Mulberry32(uint seed) { _a = seed; }

        public double NextDouble()
        {
            unchecked
            {
                _a += 0x6D2B79F5;
                uint t = _a;
                t = (t ^ (t >> 15)) * (t | 1);
                t ^= t + (t ^ (t >> 7)) * (t | 61);
                return (t ^ (t >> 14)) / 4294967296.0;
            }
        }
    }
}
