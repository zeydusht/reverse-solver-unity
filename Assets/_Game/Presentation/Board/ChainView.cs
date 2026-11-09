using UnityEngine;

namespace ReverseSolver.Presentation
{
    /* The web game's chainSVG: a dark cable between the two cell centres,
       alternating tint/white links along it, and a ring on each half. */
    public static class ChainView
    {
        public static readonly string[] Tints = { "#ffcc4d", "#5fe0d0", "#ff8fb8", "#9ecbff" };
        static readonly string[] V3Colours = { "altin", "turkuaz", "mor", "mercan" };

        public static void Build(Transform parent, Vector2 a, Vector2 b, float s, string tintHex, int order)
        {
            var root = new GameObject("Chain").transform;
            root.SetParent(parent, false);
            var tint = Draw.Hex(tintHex);
            Vector2 dir = b - a;
            float len = dir.magnitude;
            float ang = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            float disc = s * .33f;

            // Codex Task 17 (PM_MESAJ_20): coloured v3 chains, the pairs of a level gold, teal, purple, coral in turn; the
            // link and both locks share the colour (and the lock's sign: star, drop, moon, heart). Codex's measures: the
            // 128x64 px tile is 21.7 % of a cell tall (about 19 % visible, its cream halo included) and repeats every
            // 43.4 %; the 160 px lock is 65 % of a cell, hung from its ring at (0.50, 0.83). No tint, no extra rim.
            string colour = V3Colours[Mathf.Max(0, System.Array.IndexOf(Tints, tintHex)) % V3Colours.Length];
            if (Art.Get("chain-link-v3-" + colour) != null)
            {
                const float tileW = 128f / 3f, tileH = 64f / 3f;          // sprite units (Resources/Art: 3 px per unit)
                float ks = s * .217143f / tileH;
                var strip = Art.Place(root, "chain-link-v3-" + colour, new Vector2(len / ks, tileH), order, tiled: true);
                strip.transform.localScale = new Vector3(ks, ks, 1);
                Place(strip.transform, (a + b) / 2, ang);
                // PM_MESAJ_22: the lock's round body (114 px wide, centred at x 80, y 110.5 from the top of the 160 px
                // sprite) sits on the cell centre and is 45 % of a cell wide; the chain runs centre to centre into it
                float lk = s * .45f * 160f / 114f;
                foreach (var e in new[] { a, b })
                {
                    var end = Art.Place(root, "chain-lock-v3-" + colour, new Vector2(lk, lk), order + 2);
                    Place(end.transform, e - new Vector2(0, lk * (110.5f - 80f) / 160f), 0);   // layout is y-down
                }
                return;
            }

            if (Art.Get("chain-repeat") != null)
            {
                // Task 14 chain (fallback when the v3 art is missing): tile 96x48 px = 32x16 pt, 0.24 cell band
                float ks = s * .24f / 16f;
                var strip = Art.Place(root, "chain-repeat", new Vector2(len / ks, 16f), order, tiled: true);
                strip.transform.localScale = new Vector3(ks, ks, 1);
                Place(strip.transform, (a + b) / 2, ang);
                foreach (var e in new[] { a, b })
                {
                    float lk = s * .45f * (Art.T14 ? 1.45f : 1.6f);   // the lock itself ~45 % of the cell (art has a margin)
                    var end = Art.Place(root, "chain-end", new Vector2(lk, lk), order + 2);
                    Place(end.transform, e, 0);
                }
                return;
            }

            // cable: stroke #16242b width S*.15, round caps, opacity .6
            var cable = ShapeView.Create(root, "cable", new Vector2(len + s * .15f, s * .15f), 2, order)
                .Radius(s * .075f).Fill(Draw.Hex("#16242b", .6f)).Skin("chain-cable", false, tiled: true);
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
                    .Shadow(Draw.Rgba(0, 0, 0, .35f), 0, 2, 4).Skin("chain-link", false);
                Place(link.transform, c, ang);
            }
            // Codex chain end discs, one colour per chain (chain-disc-1..4 in the order of Tints)
            int k = System.Array.IndexOf(Tints, tintHex) % 4 + 1;
            if (Art.Get("chain-disc-" + k) != null)
            {
                foreach (var e in new[] { a, b })
                {
                    var r = Art.Place(root, "chain-disc-" + k, new Vector2(disc * 2.4f, disc * 2.4f), order + 2);
                    Place(r.transform, e, 0);
                }
                return;
            }
            Ring(root, a, disc, s, tint, order + 2);
            Ring(root, b, disc, s, tint, order + 2);
        }

        public static ChainRope BuildT28(Transform parent, Vector2 a, Vector2 b, float cell, int chainNumber,
                                         int order, System.Collections.Generic.ISet<ReverseSolver.Core.Cell> occupied)
        {
            var rope = new GameObject("Chain").AddComponent<ChainRope>();
            rope.transform.SetParent(parent, false);
            rope.Init(a, b, cell, chainNumber % 5, order, occupied);
            return rope;
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
