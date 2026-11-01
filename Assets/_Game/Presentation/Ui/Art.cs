using System.Collections.Generic;
using UnityEngine;

namespace ReverseSolver.Presentation
{
    /* Codex art (docs/art/HANDOFF.md) for the SET40 v3 preview. Which A/B option is used is chosen in ONE
       place: Tools/art_choice.py copies that option's files into Resources/Art/ (the build carries only the
       chosen one). Art is on only for ?set=redesign (GameRoot sets On); the live set keeps the drawn look.
       Every file is @3x: 3 px = 1 pt, the game's world unit. A missing file falls back to the drawn look. */
    public static class Art
    {
        public static bool On;

        // PM_MESAJ_16: the player's Jiggle avatar, kept in the browser (PlayerPrefs = localStorage on WebGL), never sent.
        // Until Codex's five avatars arrive (Task 15): tinted copies of the approved one (blue, pink, red, green, purple).
        public static readonly string[] AvatarKeys = { "avatar-blue", "avatar-pink", "avatar-red", "avatar-green", "avatar-purple" };
        static readonly Color[] AvatarTints = { Color.white, new Color(1f, .55f, .8f), new Color(1f, .42f, .42f), new Color(.55f, 1f, .5f), new Color(.75f, .55f, 1f) };
        public static int AvatarIndex
        {
            get => Mathf.Clamp(PlayerPrefs.GetInt("rs_avatar", 0), 0, 4);
            set { PlayerPrefs.SetInt("rs_avatar", value); PlayerPrefs.Save(); }
        }
        /* The avatar sprite for slot i (Codex's own file when present, else the tinted placeholder). */
        public static SpriteRenderer Avatar(Transform parent, int i, Vector2 size, int order)
        {
            var r = Place(parent, "t20-" + AvatarKeys[i], size, order);
            if (r != null) return r;
            r = Place(parent, AvatarKeys[i], size, order);
            if (r != null) return r;
            r = Place(parent, "m13-profile-avatar", size, order);
            if (r != null) r.color = AvatarTints[i];
            return r;
        }
        // settings (PM_MESAJ_9): sound on/off, kept in the browser between visits
        public static bool Muted
        {
            get => PlayerPrefs.GetInt("rs_mute", 0) == 1;
            set { PlayerPrefs.SetInt("rs_mute", value ? 1 : 0); PlayerPrefs.Save(); }
        }
        static readonly Dictionary<string, Object> _cache = new Dictionary<string, Object>();

        static T Load<T>(string key) where T : Object
        {
            if (!On) return null;
            if (!_cache.TryGetValue(key, out var o)) _cache[key] = o = Resources.Load<T>("Art/" + key);
            return o as T;
        }

        public static Sprite Get(string key) => Load<Sprite>(key);
        /* Codex Task 14 in-game set present (cards with a title banner, combined booster buttons, icon pills). */
        public static bool T14 => Get("loss-reason-bomb") != null;
        /* Codex Task 15 present (HUD pills without baked icons, short tall card buttons, Jiggles counter icon). */
        public static bool T15 => Get("jiggle-counter") != null;
        public static TMPro.TMP_FontAsset Lilita => On ? Resources.Load<TMPro.TMP_FontAsset>("Fonts/Lilita") : null;
        public static Texture2D Texture(string key) => Load<Texture2D>(key);
        public static AudioClip Sound(string key) => Load<AudioClip>("audio/" + key);

        /* An icon sprite centred at c (pt); false when the art is off (draw the vector icon instead). */
        public static bool Icon(Transform parent, string key, Vector2 c, float size, int order)
        {
            var r = Place(parent, key, new Vector2(size, size), order);
            if (r != null) r.transform.localPosition = Draw.W(c);
            return r != null;
        }

        /* A sprite fitted inside size (pt), centred on the parent's origin; sliced sprites fill size exactly. */
        public static SpriteRenderer Place(Transform parent, string key, Vector2 size, int order, bool sliced = false, bool tiled = false)
        {
            var s = Get(key);
            if (s == null) return null;
            var r = new GameObject(key).AddComponent<SpriteRenderer>();
            r.transform.SetParent(parent, false);
            r.sprite = s;
            r.sortingOrder = order;
            if (tiled) { r.drawMode = SpriteDrawMode.Tiled; r.tileMode = SpriteTileMode.Continuous; r.size = size; }
            else if (sliced) { r.drawMode = SpriteDrawMode.Sliced; r.size = size; }
            else
            {
                float k = Mathf.Min(size.x / s.bounds.size.x, size.y / s.bounds.size.y);
                r.transform.localScale = new Vector3(k, k, 1);
            }
            return r;
        }
    }
}
