using System.Collections;
using System.Collections.Generic;
using ReverseSolver.Core.Json;
using UnityEngine;
using UnityEngine.Rendering;

namespace ReverseSolver.Presentation
{
    /* PM_76: the menu painting's drawn views (Resources/Art/yeni/pm76/aci, Tools/import_pm76_angles.py) and the
       corridor's parallax layers. Loaded and drawn once while the menu waits — one texture every few frames, behind
       the menu's opaque background — so the dive and the corridor never stall on a first use (PM_76: no frame over
       20 ms in a transition). */
    public static class ArcAssets76
    {
        const string Folder = "Art/yeni/pm76/";
        static List<Dive75.ArcView> _views;
        static Texture2D _corridorWall, _corridorFg;
        static bool _started, _ready;
        static readonly string[] UiFolders = { "Art/yeni/T45/Buttons", "Art/yeni/T45/Pause", "Art/yeni/T48/settings", "Art/yeni/T50/win",
                                               "Art/yeni/boosters21/Selection", "Art/yeni/boosters21/Boosters/Wand", "Art/yeni/devam_v2" };

        public static bool Ready => _ready;
        public static Vector2 Size { get; private set; } = new Vector2(1180, 1770);
        public static List<Dive75.ArcView> Views => _ready ? _views : null;
        public static Texture2D CorridorWall => _ready ? _corridorWall : null;
        public static Texture2D CorridorFg => _ready ? _corridorFg : null;

        public static void Preload(MonoBehaviour host, Transform parent)
        {
            if (_started || host == null) return;
            _started = true;
            host.StartCoroutine(Load(parent));
        }

        static IEnumerator Load(Transform parent)
        {
            yield return new WaitForSecondsRealtime(.8f);   // after the menu's own arrival (MenuEnter76, .6 s)
            var json = Resources.Load<TextAsset>(Folder + "aci/views");
            if (json == null) yield break;
            var root = JsonReader.Parse(json.text);
            if (!root.TryGet("views", out var list)) yield break;
            if (root.TryGet("size", out var sizeNode)) Size = new Vector2((float)sizeNode[0].AsNumber, (float)sizeNode[1].AsNumber);
            var views = new List<Dive75.ArcView>();
            var warm = new GameObject("PM76 warm-up").transform;
            if (parent != null) warm.SetParent(parent, false);
            var group = warm.gameObject.AddComponent<SortingGroup>();
            group.sortingOrder = -32000;   // under the menu's background: drawn (uploaded), never seen
            for (int i = 0; i < list.Count; i++)
            {
                var v = list[i];
                v.TryGet("name", out var nameNode);
                string name = nameNode.AsString;
                var bgReq = Resources.LoadAsync<Texture2D>(Folder + "aci/" + name + "_bg");
                yield return bgReq;
                var fgReq = Resources.LoadAsync<Texture2D>(Folder + "aci/" + name + "_fg");
                yield return fgReq;
                v.TryGet("canvas", out var canvasNode); v.TryGet("fg", out var fgNode);
                var canvas = new Vector2[4];
                for (int c = 0; c < 4; c++) canvas[c] = new Vector2((float)canvasNode[c][0].AsNumber, (float)canvasNode[c][1].AsNumber);
                var fgRect = Rect.MinMaxRect((float)fgNode[0].AsNumber, (float)fgNode[1].AsNumber, (float)fgNode[2].AsNumber, (float)fgNode[3].AsNumber);
                var view = new Dive75.ArcView { Bg = bgReq.asset as Texture2D, Fg = fgReq.asset as Texture2D, FgPx = fgRect, Canvas = canvas };
                views.Add(view);
                Draw(warm, view.Bg as Texture2D); yield return null; yield return null;
                Draw(warm, view.Fg as Texture2D); yield return null;
            }
            // the corridor's own strip (Play loads it; loaded now it costs nothing then)
            var stripReq = Resources.LoadAsync<Sprite>("corridor_strip"); yield return stripReq;
            Draw(warm, (stripReq.asset as Sprite)?.texture); yield return null;
            var wallReq = Resources.LoadAsync<Texture2D>(Folder + "koridor/wall"); yield return wallReq;
            var fgReq2 = Resources.LoadAsync<Texture2D>(Folder + "koridor/fg"); yield return fgReq2;
            _corridorWall = wallReq.asset as Texture2D; _corridorFg = fgReq2.asset as Texture2D;
            Draw(warm, _corridorWall); yield return null; yield return null;
            Draw(warm, _corridorFg); yield return null; yield return null;
            // the panels' art (pause, settings, win/loss light, booster selection): their first open must not stall
            foreach (var folder in UiFolders)
            {
                var sprites = Resources.LoadAll<Sprite>(folder);
                for (int i = 0; i < sprites.Length; i++)
                {
                    if (warm == null) break;
                    var r = new GameObject("warm ui").AddComponent<SpriteRenderer>();
                    r.transform.SetParent(warm, false); r.sprite = sprites[i]; r.transform.localScale = Vector3.one * .01f;
                    if (i % 6 == 5) yield return null;
                }
                yield return null;
            }
            // the fonts' glyphs (Baloo headings, the body font): every letter the Turkish and English texts use
            if (warm != null)
            {
                const string chars = "ABCÇDEFGĞHIİJKLMNOÖPRSŞTUÜVYZabcçdefgğhıijklmnoöprsştuüvyzQWXqwx0123456789+-:·.,!?%()'\"/";
                TextView.Create(warm, "warm heading", chars, 22, Palette.Cream, order: 0).Heading(TextView.RimTeal);
                TextView.Create(warm, "warm body", chars, 15, Palette.Navy, false, order: 0);
                TextView.Create(warm, "warm title", chars, 30, Palette.Cream, order: 0).Title3D();
                yield return null; yield return null;
            }
            if (warm != null) Object.Destroy(warm.gameObject);
            _views = views;
            _ready = views.Count >= 2 && views.TrueForAll(x => x.Bg != null);
        }

        static void Draw(Transform warm, Texture2D tex)
        {
            if (warm == null || tex == null) return;
            var r = new GameObject("warm").AddComponent<SpriteRenderer>();
            r.transform.SetParent(warm, false);
            r.sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(.5f, .5f), 100);
            r.transform.localScale = Vector3.one * .01f;
        }
    }
}
