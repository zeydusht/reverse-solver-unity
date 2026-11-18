using UnityEngine;

namespace ReverseSolver.Presentation
{
    /* PM_86 §6: a counter that changes counts to its new value (eased out, a small pop), as in other puzzle games:
       the menu's gold, the booster counts. */
    public static class CountUp86
    {
        static readonly System.Collections.Generic.Dictionary<TextView, int> Targets = new System.Collections.Generic.Dictionary<TextView, int>();

        /* Whether t is already counting to that value. */
        public static bool Toward(TextView t, int to) => t != null && Targets.TryGetValue(t, out int v) && v == to;

        public static void To(TextView t, int from, int to, float seconds = .4f)
        {
            if (t == null) return;
            Targets[t] = to;
            if (from == to) { t.Text = to.ToString(); return; }
            var tr = t.transform; var baseScale = tr.localScale;
            t.Text = from.ToString();
            Tweens.Run(t, seconds, k =>
            {
                if (t == null) return;
                t.Text = Mathf.RoundToInt(Mathf.Lerp(from, to, BoosterFx74.OutCubic(k))).ToString();
                tr.localScale = baseScale * (1 + .18f * Mathf.Sin(Mathf.Clamp01(k) * Mathf.PI));
            }, () => { Targets.Remove(t); if (t != null) { t.Text = to.ToString(); tr.localScale = baseScale; } });
        }
    }
}
