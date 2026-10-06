using ReverseSolver.Core;
using UnityEngine;

namespace ReverseSolver.Presentation
{
    /* The web game's ICONS / INTRO_ICONS (24x24 SVG line icons, white, stroke
       ~2.1) rebuilt from shapes. Each draws centred on parent at `size` points. */
    public static class Icons
    {
        static Vector4 White => ReverseSolver.Presentation.Draw.Hex("#ffffff");

        public static void Draw(string id, Transform parent, float size, int order)
        {
            float u = size / 24f;                                   // SVG unit
            switch (id)
            {
                case "scissors": Scissors(parent, u, order); break;
                case "wand": Wand(parent, u, order); break;
                case "hammer": Hammer(parent, u, order); break;
                case "clock": Clock(parent, u, order); break;
                case "chain": Chain(parent, u, order); break;
                case "bomb": Bomb(parent, u, order); break;
                case "wall": Wall(parent, u, order); break;
                case "lock": Lock(parent, u, order); break;
                default: Hammer(parent, u, order); break;
            }
        }

        public static string IdOf(Booster b) => b.Id();

        // SVG point (0..24, y down) to local layout point centred on (12, 12)
        static Vector2 P(float x, float y, float u) => new Vector2((x - 12) * u, (y - 12) * u);

        static void Line(Transform t, Vector2 a, Vector2 b, float w, Vector4 c, int order)
        {
            var d = b - a;
            var s = ShapeView.Create(t, "line", new Vector2(d.magnitude + w, w), 1, order).Radius(w / 2).Fill(c);
            s.transform.localPosition = ReverseSolver.Presentation.Draw.W((a + b) / 2);
            s.transform.localRotation = Quaternion.Euler(0, 0, -Mathf.Atan2(d.y, d.x) * Mathf.Rad2Deg);
        }

        static void Ring(Transform t, Vector2 c, float r, float w, Vector4 col, int order)
        {
            var s = ShapeView.Create(t, "ring", new Vector2(r * 2 + w, r * 2 + w), 1, order).Radius(r + w / 2).Ring(col, w);
            s.transform.localPosition = ReverseSolver.Presentation.Draw.W(c);
        }

        static void Dot(Transform t, Vector2 c, float r, Vector4 col, int order)
        {
            var s = ShapeView.Create(t, "dot", new Vector2(r * 2, r * 2), 1, order).Radius(r).Fill(col);
            s.transform.localPosition = ReverseSolver.Presentation.Draw.W(c);
        }

        static void Scissors(Transform t, float u, int o)
        {
            float w = 2.1f * u;
            Ring(t, P(6.4f, 18.2f, u), 2.7f * u, w, White, o);
            Ring(t, P(17.6f, 18.2f, u), 2.7f * u, w, White, o);
            Line(t, P(8.4f, 16.3f, u), P(19.2f, 3.4f, u), w, White, o);
            Line(t, P(15.6f, 16.3f, u), P(4.8f, 3.4f, u), w, White, o);
            Dot(t, P(12, 11.4f, u), 1f * u, White, o + 1);
        }

        static void Wand(Transform t, float u, int o)
        {
            float w = 2.1f * u;
            Line(t, P(3.6f, 20.4f, u), P(13.2f, 10.8f, u), w, White, o);
            var star = SolidView.Star(5.3f * u, 2.3f * u);
            var sv = SolidView.Polygon(t, "star", star, White, o);
            sv.transform.localPosition = ReverseSolver.Presentation.Draw.W(P(17, 9, u));
            Line(t, P(6.2f, 5.6f, u), P(6.2f, 8.4f, u), w * .8f, White, o);
            Line(t, P(4.8f, 7f, u), P(7.6f, 7f, u), w * .8f, White, o);
        }

        static void Hammer(Transform t, float u, int o)
        {
            var root = new GameObject("hammer").transform;
            root.SetParent(t, false);
            root.localRotation = Quaternion.Euler(0, 0, 38);          // rotate(-38) in y-down space
            var head = ShapeView.Create(root, "head", new Vector2(12.8f * u, 5.6f * u), 1, o).Radius(1.6f * u).Fill(White);
            head.transform.localPosition = ReverseSolver.Presentation.Draw.W(P(12, 6.2f, u));
            var handle = ShapeView.Create(root, "handle", new Vector2(3.6f * u, 11.6f * u), 1, o).Radius(1.5f * u)
                .Fill(new Vector4(0, 0, 0, 0)).Stroke(White, 2.1f * u);
            handle.transform.localPosition = ReverseSolver.Presentation.Draw.W(P(12, 14.8f, u));
        }

        static void Clock(Transform t, float u, int o)
        {
            float w = 2.1f * u;
            Ring(t, P(12, 13.6f, u), 7.4f * u, w, White, o);
            Line(t, P(12, 9.6f, u), P(12, 13.6f, u), w, White, o);
            Line(t, P(12, 13.6f, u), P(14.6f, 15.5f, u), w, White, o);
            var knob = ShapeView.Create(t, "knob", new Vector2(3.6f * u, 2.6f * u), 1, o).Radius(1 * u)
                .Fill(new Vector4(0, 0, 0, 0)).Stroke(White, w * .9f);
            knob.transform.localPosition = ReverseSolver.Presentation.Draw.W(P(12, 3.3f, u));
            Line(t, P(5, 6.6f, u), P(6.7f, 8.3f, u), w, White, o);
            Line(t, P(19, 6.6f, u), P(17.3f, 8.3f, u), w, White, o);
        }

        static void Chain(Transform t, float u, int o)
        {
            float w = 2.1f * u;
            Ring(t, P(6.4f, 17.6f, u), 3.2f * u, w, White, o);
            Ring(t, P(17.6f, 6.4f, u), 3.2f * u, w, White, o);
            Line(t, P(8.7f, 15.3f, u), P(15.3f, 8.7f, u), w, White, o);
        }

        static void Bomb(Transform t, float u, int o)
        {
            float w = 2.1f * u;
            Ring(t, P(10.6f, 14.4f, u), 6.4f * u, w, White, o);
            Line(t, P(15.4f, 9.6f, u), P(18, 7, u), w, White, o);
            Line(t, P(17.2f, 4.4f, u), P(17.2f, 6.4f, u), w * .8f, White, o);
            Line(t, P(20.4f, 4f, u), P(20.4f, 5.8f, u), w * .8f, White, o);
        }

        static void Wall(Transform t, float u, int o)
        {
            float w = 2f * u;
            var box = ShapeView.Create(t, "box", new Vector2(18 * u, 14 * u), 1, o).Radius(1.6f * u)
                .Fill(new Vector4(0, 0, 0, 0)).Stroke(White, w);
            box.transform.localPosition = ReverseSolver.Presentation.Draw.W(P(12, 12, u));
            Line(t, P(3, 9.7f, u), P(21, 9.7f, u), w, White, o);
            Line(t, P(3, 14.3f, u), P(21, 14.3f, u), w, White, o);
            Line(t, P(9, 5, u), P(9, 9.7f, u), w, White, o);
            Line(t, P(15, 9.7f, u), P(15, 14.3f, u), w, White, o);
            Line(t, P(9, 14.3f, u), P(9, 19, u), w, White, o);
        }

        /* Small padlock for locked buttons (the web game used the 🔒 emoji). */
        static void Lock(Transform t, float u, int o)
        {
            var body = ShapeView.Create(t, "body", new Vector2(12 * u, 9 * u), 1, o).Radius(2 * u).Fill(White);
            body.transform.localPosition = ReverseSolver.Presentation.Draw.W(P(12, 15.5f, u));
            Ring(t, P(12, 10, u), 3.6f * u, 2.2f * u, White, o);
        }
    }
}
