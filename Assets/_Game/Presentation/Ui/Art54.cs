using UnityEngine;

namespace ReverseSolver.Presentation
{
    // Resource keys shared by the HUD and booster bar.
    static class Art54
    {
        public const string Header = "hud/header", Top = "hud/top-strip", Bottom = "hud/bottom-strip",
            Panel = "hud/counter-panel", Piece = "hud/piece", Window = "hud/portrait-window",
            Socket = "hud/socket", Badge = "hud/badge",
            Lock = "hud/locked-icon", Pause = "hud/pause", Joy = "jigo/sevinc/01", NormalFrame = "jigo/normal/01";
        public static readonly string[] Bodies = { "hud/booster-normal", "hud/booster-pressed", "hud/booster-locked", "hud/booster-zero" };
        public static readonly string[] Settings = { "hud/settings-normal", "hud/settings-pressed", "hud/settings-locked", "hud/settings-zero" };
        public static readonly string[] Boosters = { "hud/scissors", "hud/wand", "hud/hammer", "hud/clock" };
        public static readonly string[] Moods = { "normal", "tedirgin", "korku", "mutlu", "uzgun" };

        public static SpriteRenderer Place(Transform parent, string key, Vector2 c, Vector2 size, int order)
        {
            var r = Art.Place(parent, key, size, order);
            if (r != null) r.transform.localPosition = Draw.W(c);
            return r;
        }

        public static SpriteRenderer Stretch(Transform parent, string key, Vector2 c, Vector2 size, int order)
        {
            var r = Place(parent, key, c, size, order);
            if (r != null) r.transform.localScale = new Vector3(size.x / r.sprite.bounds.size.x, size.y / r.sprite.bounds.size.y, 1);
            return r;
        }
    }
}
