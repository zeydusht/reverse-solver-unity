using System.Collections.Generic;
using ReverseSolver.Core;
using UnityEngine;

namespace ReverseSolver.Presentation
{
    // Pure layout math; coordinates are y-down.
    public static class ChainCurve
    {
        public readonly struct Sample
        {
            public readonly Vector2 Position;
            public readonly float Angle;
            public Sample(Vector2 position, float angle) { Position = position; Angle = angle; }
        }

        public static float Sag(Vector2 a, Vector2 b, float cell)
        {
            float length = Vector2.Distance(a, b);
            return length <= 0 ? 0 : Mathf.Clamp(.10f * length, 0, .40f * cell) * Mathf.Sqrt(Mathf.Abs(b.x - a.x) / length);
        }

        public static Vector2 Point(Vector2 a, Vector2 b, float sag, float t) =>
            Vector2.LerpUnclamped(a, b, t) + new Vector2(0, 4 * sag * t * (1 - t));

        public static float Angle(Vector2 a, Vector2 b, float sag, float t) =>
            Mathf.Atan2(b.y - a.y + 4 * sag * (1 - 2 * t), b.x - a.x) * Mathf.Rad2Deg;

        public static Sample[] Samples(Vector2 a, Vector2 b, float sag, float spacing)
        {
            const int steps = 512;
            var lengths = new float[steps + 1];
            var previous = a;
            for (int i = 1; i <= steps; i++)
            {
                var p = Point(a, b, sag, (float)i / steps);
                lengths[i] = lengths[i - 1] + Vector2.Distance(previous, p);
                previous = p;
            }
            int intervals = Mathf.Max(1, Mathf.RoundToInt(lengths[steps] / Mathf.Max(.0001f, spacing)));
            var result = new Sample[intervals - 1];
            int segment = 1;
            for (int i = 1; i < intervals; i++)
            {
                float distance = lengths[steps] * i / intervals;
                while (segment < steps && lengths[segment] < distance) segment++;
                float t = (segment - 1 + Mathf.InverseLerp(lengths[segment - 1], lengths[segment], distance)) / steps;
                result[i - 1] = new Sample(Point(a, b, sag, t), Angle(a, b, sag, t));
            }
            return result;
        }

        public static float FitSag(Vector2 a, Vector2 b, float sag, float cell, ISet<Cell> occupied)
        {
            if (occupied == null || occupied.Count == 0) return sag;
            int steps = Mathf.Max(512, Mathf.CeilToInt(Vector2.Distance(a, b) / cell * 100));
            var chord = new HashSet<Cell>();
            Cell At(Vector2 p) => new Cell(Mathf.FloorToInt(p.x / cell), Mathf.FloorToInt(p.y / cell));
            for (int i = 0; i <= steps; i++) chord.Add(At(Point(a, b, 0, (float)i / steps)));
            for (int attempt = 0; attempt <= 12; attempt++)
            {
                bool clear = true;
                for (int i = 1; i < steps; i++)
                {
                    var c = At(Point(a, b, sag, (float)i / steps));
                    if (occupied.Contains(c) && !chord.Contains(c)) { clear = false; break; }
                }
                if (clear) return sag;
                sag *= .8f;
            }
            return 0;
        }
    }
}
