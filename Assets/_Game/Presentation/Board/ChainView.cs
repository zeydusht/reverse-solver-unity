using UnityEngine;

namespace ReverseSolver.Presentation
{
    /* The web game's chainSVG: a dark cable between the two cell centres,
       alternating tint/white links along it, and a ring on each half. */
    public static class ChainView
    {
        public static readonly string[] Tints = { "#ffcc4d", "#5fe0d0", "#ff8fb8", "#9ecbff" };

        public static void Build(Transform parent, Vector2 a, Vector2 b, float s, string tintHex, int order)
        {
            var root = new GameObject("Chain").transform;
            root.SetParent(parent, false);
            var tint = Draw.Hex(tintHex);
            Vector2 dir = b - a;
            float len = dir.magnitude;
            float ang = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            float disc = s * .33f;

            // cable: stroke #16242b width S*.15, round caps, opacity .6
            var cable = ShapeView.Create(root, "cable", new Vector2(len + s * .15f, s * .15f), 2, order)
                .Radius(s * .075f).Fill(Draw.Hex("#16242b", .6f));
            Place(cable.transform, (a + b) / 2, ang);

            float inner = len - disc * 2;
            int n = Mathf.Max(2, Mathf.RoundToInt(inner / (s * .30f)));
            for (int i = 0; i < n; i++)
            {
                float t = (disc + inner * (i + .5f) / n) / len;
                var c = Vector2.Lerp(a, b, t);
                // ellipse rx S*.135 ry S*.088, stroke S*.055, alternating tint / white
                // .chain { filter: drop-shadow(0 2px 4px rgba(0,0,0,.55)) }
                var link = ShapeView.Create(root, "link", new Vector2(s * .27f, s * .176f), 6, order + 1)
                    .Radius(s * .088f).Fill(new Vector4(0, 0, 0, 0))
                    .Stroke(i % 2 == 1 ? Draw.Hex("#ffffff") : tint, s * .055f)
                    .Shadow(Draw.Rgba(0, 0, 0, .35f), 0, 2, 4);
                Place(link.transform, c, ang);
            }
            Ring(root, a, disc, s, tint, order + 2);
            Ring(root, b, disc, s, tint, order + 2);
        }

        static void Ring(Transform root, Vector2 c, float disc, float s, Vector4 tint, int order)
        {
            Circle(root, c, disc * 1.34f, Draw.Hex("#0d1a20", .38f), order);
            Circle(root, c, disc * 1.20f, new Vector4(tint.x, tint.y, tint.z, .30f), order + 1);
            ShapeView.Create(root, "disc", new Vector2(disc * 2, disc * 2), s * .1f, order + 2)
                .Radius(disc).Fill(Draw.Hex("#3a4c57")).Stroke(tint, s * .095f);
            Place(root.GetChild(root.childCount - 1), c, 0);
            Circle(root, c, disc * .46f, tint, order + 3);
            Circle(root, c + new Vector2(-disc * .27f, -disc * .31f), disc * .19f, Draw.Hex("#ffffff", .6f), order + 4);
        }

        static void Circle(Transform root, Vector2 c, float r, Vector4 color, int order)
        {
            var v = ShapeView.Create(root, "circle", new Vector2(r * 2, r * 2), 1, order).Radius(r).Fill(color);
            Place(v.transform, c, 0);
        }

        static void Place(Transform t, Vector2 center, float angleDeg)
        {
            t.localPosition = Draw.W(center);
            t.localRotation = Quaternion.Euler(0, 0, -angleDeg);   // layout is y-down
        }
    }
}
