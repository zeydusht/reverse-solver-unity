using System;
using System.Collections.Generic;
using UnityEngine;

namespace ReverseSolver.Presentation
{
    /* Minimal time-based tweens, driven from one MonoBehaviour. Only what the
       M3 effects need: settle back, fly out, a bomb pulse and a card fade. */
    public sealed class Tweens : MonoBehaviour
    {
        sealed class T { public float Start, Duration; public Action<float> Step; public Action Done; public object Key; }

        readonly List<T> _live = new List<T>();
        static Tweens _instance;

        public static Tweens Instance
        {
            get
            {
                if (_instance == null) _instance = new GameObject("Tweens").AddComponent<Tweens>();
                return _instance;
            }
        }

        /* Runs step(t) for t in 0..1 over duration seconds. A new tween with the
           same key replaces the running one. */
        public static void Run(object key, float duration, Action<float> step, Action done = null)
        {
            var self = Instance;
            if (key != null) self._live.RemoveAll(t => Equals(t.Key, key));
            self._live.Add(new T { Start = Time.unscaledTime, Duration = Mathf.Max(duration, 1e-4f), Step = step, Done = done, Key = key });
            step(0);
        }

        public static void Kill(object key) => Instance._live.RemoveAll(t => Equals(t.Key, key));

        void Update()
        {
            float now = Time.unscaledTime;
            for (int i = _live.Count - 1; i >= 0; i--)
            {
                if (i >= _live.Count) continue;
                var t = _live[i];
                float k = Mathf.Clamp01((now - t.Start) / t.Duration);
                t.Step(k);
                if (k >= 1f)
                {
                    _live.Remove(t);
                    t.Done?.Invoke();
                }
            }
        }

        // ---- easing: the web game's CSS cubic-beziers -------------------------------

        /* cubic-bezier(x1, y1, x2, y2) evaluated at time x (Newton on x, then y). */
        public static float Bezier(float x1, float y1, float x2, float y2, float x)
        {
            float t = x;
            for (int i = 0; i < 6; i++)
            {
                float cx = Cubic(x1, x2, t) - x;
                float d = CubicD(x1, x2, t);
                if (Mathf.Abs(d) < 1e-5f) break;
                t = Mathf.Clamp01(t - cx / d);
            }
            return Cubic(y1, y2, t);
        }

        static float Cubic(float a, float b, float t) => 3 * a * t * (1 - t) * (1 - t) + 3 * b * t * t * (1 - t) + t * t * t;
        static float CubicD(float a, float b, float t) => 3 * a * (1 - t) * (1 - t) + 6 * (b - a) * t * (1 - t) + 3 * (1 - b) * t * t;

        public static float Settle(float x) => Bezier(.34f, 1.4f, .5f, 1f, x);   // .pc.snap .22s
        public static float FlyOut(float x) => Bezier(.3f, 0f, .6f, 1f, x);      // .pc.fly .3s
    }
}
