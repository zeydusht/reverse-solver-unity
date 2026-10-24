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
        public static AudioClip Sound(string key) => Load<AudioClip>("audio/" + key);

        /* An icon sprite centred at c (pt); false when the art is off (draw the vector icon instead). */
        public static bool Icon(Transform parent, string key, Vector2 c, float size, int order)
        {
            var r = Place(parent, key, new Vector2(size, size), order);
            if (r != null) r.transform.localPosition = Draw.W(c);
            return r != null;
        }

        /* A sprite fitted inside size (pt), centred on the parent's origin; sliced sprites fill size exactly. */
        public static SpriteRenderer Place(Transform parent, string key, Vector2 size, int order, bool sliced = false)
        {
            var s = Get(key);
            if (s == null) return null;
            var r = new GameObject(key).AddComponent<SpriteRenderer>();
            r.transform.SetParent(parent, false);
            r.sprite = s;
            r.sortingOrder = order;
            if (sliced) { r.drawMode = SpriteDrawMode.Sliced; r.size = size; }
            else
            {
                float k = Mathf.Min(size.x / s.bounds.size.x, size.y / s.bounds.size.y);
                r.transform.localScale = new Vector3(k, k, 1);
            }
            return r;
        }
    }
}
