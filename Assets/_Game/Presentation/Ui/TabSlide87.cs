using System;
using UnityEngine;

namespace ReverseSolver.Presentation
{
    /* PM_87 §1: the menu's pages side by side, in the bar's order; a tab pressed (or the page dragged) slides both:
       the old page out, the new one in at the same pace, each with its own background. The old page is a picture of
       itself (the camera rendered once, its bar hidden) laid over the new page's world; the new page is live and
       slides in beside it (its own overflow stays under the picture). The bar stays put (its root is held against
       the world's move).
       The pace is the reference game's, measured in ref_alt_menu.mp4: a fast start that eases out, 88 % of the way
       in .3 s, all of it in .6 s (an ease-out cubic). */
    public sealed class TabSlide87 : MonoBehaviour
    {
        public static float Seconds = .6f;   // &sekmesure= (s)
        public static int Live;               // slides on screen (MenuArt waits with its decoding)
        public static bool Log;               // &sekmelog=1: every slide frame's time and offset
        RenderTexture _rt;
        Transform _picture, _world, _bar;
        Vector2 _screen;
        int _dir;
        float _p, _from, _to, _t, _len = -1;
        Action _done;
        bool _over;
        public int Dir => _dir;
        public float Progress => _p;
        public bool Animating => _len > 0;
        int _seen;
        public float BuildMs, StartMs;   // the new page's build (ms) and the frame that showed it first
        public int Frames { get; private set; }
        public int Slow { get; private set; }
        public float MaxDt { get; private set; }

        /* Before the new page is built: the old one's picture. dir 1: the new page comes from the right (the content
           flows left), -1: from the left. */
        public static TabSlide87 Begin(Camera cam, Vector2 screen, int order, int dir, Transform parent, Material spriteMat)
        {
            var go = new GameObject("PM87 tab slide");
            go.transform.SetParent(parent, false);
            var s = go.AddComponent<TabSlide87>();
            Live++;
            s._screen = screen; s._dir = dir;
            int w = Mathf.Max(2, Screen.width), h = Mathf.Max(2, Screen.height);
            s._rt = new RenderTexture(w, h, 24) { name = "tab slide", filterMode = FilterMode.Bilinear, wrapMode = TextureWrapMode.Clamp };
            var prev = cam.targetTexture;
            cam.targetTexture = s._rt;
            cam.Render();
            cam.targetTexture = prev;
            var q = new GameObject("old page");
            q.transform.SetParent(go.transform, false);
            var mesh = new Mesh { name = "tab slide" };
            mesh.vertices = new[] { Draw.W(0, 0), Draw.W(screen.x, 0), Draw.W(screen.x, screen.y), Draw.W(0, screen.y) };
            mesh.uv = new[] { new Vector2(0, 1), new Vector2(1, 1), new Vector2(1, 0), new Vector2(0, 0) };
            mesh.triangles = new[] { 0, 1, 2, 0, 2, 3 };
            mesh.colors = new[] { Color.white, Color.white, Color.white, Color.white };
            q.AddComponent<MeshFilter>().sharedMesh = mesh;
            var r = q.AddComponent<MeshRenderer>();
            r.sharedMaterial = new Material(spriteMat) { mainTexture = s._rt };
            r.sortingOrder = order;
            s._picture = q.transform;
            return s;
        }

        /* The new page's world and its bar (held still). */
        public void Attach(Transform world, Transform bar)
        {
            _world = world; _bar = bar;
            Set(_p);
        }

        /* p: how far the new page is in (0: the old page whole, 1: the new one). */
        public void Set(float p)
        {
            _p = p;
            float x = -_dir * _screen.x * p;
            if (_picture != null) _picture.localPosition = new Vector3(x, 0, 0);
            float wx = _dir * _screen.x * (1 - p);
            if (_world != null) _world.localPosition = new Vector3(wx, _world.localPosition.y, _world.localPosition.z);
            if (_bar != null) _bar.localPosition = new Vector3(-wx, _bar.localPosition.y, _bar.localPosition.z);
        }

        /* From where it is to `to` (1: the new page, 0: back to the old one) over `seconds`, an ease-out cubic. */
        public void Animate(float to, float seconds, Action done)
        {
            _from = _p; _to = to; _t = 0; _len = Mathf.Max(.05f, seconds); _done = done;
        }

        void LateUpdate()
        {
            if (_over) return;
            float dt = Time.unscaledDeltaTime;
            // the first two frames' times are the press's (the frame before it, and the one that built the new page:
            // BuildMs); from the third on the slide's own (a drag's too)
            _seen++;
            if (_seen == 2) StartMs = dt * 1000;
            if (_seen > 2) { Frames++; if (dt > .02f) Slow++; MaxDt = Mathf.Max(MaxDt, dt); }
            if (_len <= 0) return;
            _t += Mathf.Min(dt, 1 / 30f);
            float k = Mathf.Clamp01(_t / _len);
            Set(Mathf.LerpUnclamped(_from, _to, BoosterFx74.OutCubic(k)));
            if (Log) Debug.Log($"[sekme] {Time.realtimeSinceStartup * 1000:0} {dt * 1000:0.0} p {_p:0.0000}");
            if (k >= 1) End();
        }

        /* Done (or cut short by another press): the world back in place, the picture gone. */
        public void End()
        {
            if (_over) return;
            _over = true;
            if (_world != null) _world.localPosition = new Vector3(0, _world.localPosition.y, _world.localPosition.z);
            if (_bar != null) _bar.localPosition = new Vector3(0, _bar.localPosition.y, _bar.localPosition.z);
            if (_len > 0) Debug.Log($"[sekme] slide {(_to > .5f ? "in" : "back")}: build {BuildMs:0.0} ms, first frame {StartMs:0.0} ms; then {Frames} frames, {Slow} over 20 ms, longest {MaxDt * 1000:0.0} ms");
            var done = _done; _done = null;
            Destroy(gameObject);
            done?.Invoke();
        }

        void OnDestroy()
        {
            Live = Mathf.Max(0, Live - 1);
            if (_rt != null) { _rt.Release(); Destroy(_rt); _rt = null; }
            var r = _picture != null ? _picture.GetComponent<MeshRenderer>() : null;
            if (r != null) Destroy(r.sharedMaterial);
        }
    }
}
