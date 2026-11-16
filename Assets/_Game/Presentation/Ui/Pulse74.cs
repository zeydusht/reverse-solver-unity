using UnityEngine;

namespace ReverseSolver.Presentation
{
    /* PM_74: the menu's Play button breathes (1.0 <-> 1.03 every 1.6 s), inviting the tap. Each target scales about
       its own centre; the press effect moves the label, never its scale. */
    public sealed class Pulse74 : MonoBehaviour
    {
        public const float Period = 1.6f, Amount = .03f;
        Transform[] _targets;
        Vector3[] _base;
        float _start;

        public static Pulse74 On(GameObject host, params Transform[] targets)
        {
            var p = host.AddComponent<Pulse74>();
            p._targets = targets; p._base = new Vector3[targets.Length];
            for (int i = 0; i < targets.Length; i++) p._base[i] = targets[i] != null ? targets[i].localScale : Vector3.one;
            p._start = Time.unscaledTime;
            return p;
        }

        void Update()
        {
            float k = 1 + Amount * .5f * (1 - Mathf.Cos((Time.unscaledTime - _start) * Mathf.PI * 2 / Period));
            for (int i = 0; i < _targets.Length; i++) if (_targets[i] != null) _targets[i].localScale = _base[i] * k;
        }
    }
}
