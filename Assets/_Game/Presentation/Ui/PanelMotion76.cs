using System.Collections.Generic;
using UnityEngine;

namespace ReverseSolver.Presentation
{
    /* PM_76: panels open and close softly instead of popping (Zeyd: "robotic"). Open .34 s: the veil fades in, the
       panel grows from 88 % with a 5 % spring and fades in. Close .24 s (ease in-out): it shrinks to 94 % and fades
       with its veil, then the object is destroyed. Works on any root with a "veil" child: everything else (but a
       full-screen "glow") is moved under a "panel" child that scales about the panel's centre. */
    public sealed class PanelMotion76 : MonoBehaviour
    {
        public const float OpenSeconds = .34f, CloseSeconds = .24f;
        public static bool Animated = true;

        Transform _panel;
        Vector2 _pivot;
        ShapeView _veil;
        readonly List<(SpriteRenderer r, float a)> _sprites = new List<(SpriteRenderer, float)>();
        readonly List<(TextView t, float a)> _texts = new List<(TextView, float)>();
        readonly List<ShapeView> _shapes = new List<ShapeView>();
        readonly List<(SpriteRenderer r, float a)> _glows = new List<(SpriteRenderer, float)>();
        float _start;
        bool _closing;

        public static PanelMotion76 Open(Transform root, Vector2 pivot)
        {
            if (!Animated || root == null) return null;
            var m = root.gameObject.AddComponent<PanelMotion76>();
            m._pivot = pivot;
            m._panel = new GameObject("panel").transform;
            m._panel.SetParent(root, false);
            var keep = new List<Transform>();
            foreach (Transform child in root)
            {
                if (child == m._panel) continue;
                if (child.name == "veil") { m._veil = child.GetComponent<ShapeView>(); continue; }
                if (child.name.Contains("glow")) { if (child.TryGetComponent<SpriteRenderer>(out var g)) m._glows.Add((g, g.color.a)); continue; }
                keep.Add(child);
            }
            foreach (var child in keep) child.SetParent(m._panel, false);
            foreach (var r in m._panel.GetComponentsInChildren<SpriteRenderer>(true)) m._sprites.Add((r, r.color.a));
            foreach (var t in m._panel.GetComponentsInChildren<TextView>(true)) m._texts.Add((t, t.Alpha));
            foreach (var v in m._panel.GetComponentsInChildren<ShapeView>(true)) m._shapes.Add(v);
            m._start = Time.unscaledTime;
            m.Pose(0);
            return m;
        }

        /* Closes and destroys the root (the game has already let go of it). */
        public static bool Close(Transform root)
        {
            if (root == null || !root.TryGetComponent<PanelMotion76>(out var m) || m._closing) return false;
            m._closing = true; m._start = Time.unscaledTime;
            return true;
        }

        void Update()
        {
            float t = (Time.unscaledTime - _start) / (_closing ? CloseSeconds : OpenSeconds);
            Pose(Mathf.Clamp01(t));
            if (_closing && t >= 1) Destroy(gameObject);
            else if (!_closing && t >= 1) enabled = false;
        }

        void Pose(float k)
        {
            float scale, alpha, veil;
            if (!_closing)
            {
                scale = Mathf.LerpUnclamped(.88f, 1f, OutBack(k, 1.25f));
                alpha = Mathf.Clamp01(k / .55f);
                veil = 1 - (1 - k) * (1 - k);
            }
            else
            {
                float e = k * k * (3 - 2 * k);
                scale = Mathf.Lerp(1f, .94f, e);
                alpha = 1 - e;
                veil = 1 - e;
            }
            if (_panel != null)
            {
                _panel.localScale = new Vector3(scale, scale, 1);
                _panel.localPosition = Draw.W(_pivot * (1 - scale));
            }
            foreach (var (r, a) in _sprites) if (r != null) { var c = r.color; c.a = a * alpha; r.color = c; }
            foreach (var (tv, a) in _texts) if (tv != null) tv.Alpha = a * alpha;
            foreach (var v in _shapes) if (v != null) v.Alpha(alpha);
            foreach (var (g, a) in _glows) if (g != null) { var c = g.color; c.a = a * veil; g.color = c; }
            if (_veil != null) _veil.Alpha(veil);
        }

        /* Ease out with a spring: overshoot ≈ 5 % with k = 1.25. */
        public static float OutBack(float t, float k) { t = Mathf.Clamp01(t) - 1; return 1 + (k + 1) * t * t * t + k * t * t; }
    }
}
