using System.Collections.Generic;
using UnityEngine;

namespace ReverseSolver.Presentation
{
    /* PM_76: the menu arrives instead of popping (after the opening, and back from a level). The top bar drops in,
       the "Level N" button and the tab bar rise in, each with a 4 % spring, overlapping by about a third; from a
       paused level the pause's navy fades away over it (.45 s). Total .6 s. */
    public static class MenuEnter76
    {
        public const float Seconds = .6f, OverlaySeconds = .45f;

        public static void Run(Transform menu, Transform tabs, Vector2 screen, bool overlay, Vector4 overlayColor, int overlayOrder)
        {
            if (menu == null) return;
            var parts = new List<(Transform t, Vector3 home, float delay, float dy)>();
            foreach (Transform child in menu)
            {
                if (child.name == "background") continue;
                float y = -child.position.y;
                if (y < screen.y * .2f) parts.Add((child, child.localPosition, 0f, -34));
                else if (y > screen.y * .62f) parts.Add((child, child.localPosition, .1f, 38));
            }
            if (tabs != null) parts.Add((tabs, tabs.localPosition, .05f, 46));
            var sprites = new List<(SpriteRenderer r, float a, float delay)>();
            var texts = new List<(TextView t, float a, float delay)>();
            var shapes = new List<(ShapeView s, float delay)>();
            foreach (var (t, _, delay, _) in parts)
            {
                foreach (var r in t.GetComponentsInChildren<SpriteRenderer>()) sprites.Add((r, r.color.a, delay));
                foreach (var v in t.GetComponentsInChildren<TextView>()) texts.Add((v, v.Alpha, delay));
                foreach (var s in t.GetComponentsInChildren<ShapeView>()) shapes.Add((s, delay));
                if (t.TryGetComponent<SpriteRenderer>(out var own)) sprites.Add((own, own.color.a, delay));
                if (t.TryGetComponent<TextView>(out var ownText)) texts.Add((ownText, ownText.Alpha, delay));
                if (t.TryGetComponent<ShapeView>(out var ownShape)) shapes.Add((ownShape, delay));
            }
            ShapeView veil = null;
            if (overlay)
            {
                veil = ShapeView.Create(menu, "menu enter veil", screen * 1.2f, 0, overlayOrder).Radius(0).Fill(overlayColor);
                veil.transform.localPosition = Draw.W(screen / 2);
            }
            float Part(float t, float delay) => Mathf.Clamp01((t - delay) / (Seconds - .1f));
            void Pose(float k)
            {
                float t = k * Seconds;
                foreach (var (tr, home, delay, dy) in parts)
                    if (tr != null) tr.localPosition = home + Draw.W(0, dy * (1 - PanelMotion76.OutBack(Part(t, delay), 1.0f)));
                foreach (var (r, a, delay) in sprites) if (r != null) { var c = r.color; c.a = a * Mathf.Clamp01(Part(t, delay) / .55f); r.color = c; }
                foreach (var (v, a, delay) in texts) if (v != null) v.Alpha = a * Mathf.Clamp01(Part(t, delay) / .55f);
                foreach (var (s, delay) in shapes) if (s != null) s.Alpha(Mathf.Clamp01(Part(t, delay) / .55f));
                if (veil != null) veil.Alpha(1 - BoosterFx74.OutCubic(Mathf.Clamp01(t / OverlaySeconds)));
            }
            Pose(0);
            Tweens.Run(menu, Seconds, Pose, () =>
            {
                Pose(1);
                if (veil != null) Object.Destroy(veil.gameObject);
            });
        }
    }
}
