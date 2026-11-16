using System.Collections;
using ReverseSolver.Core.Json;
using UnityEngine;
using UnityEngine.Rendering;

namespace ReverseSolver.Presentation
{
    /* PM_76/77: what the transitions use, loaded and drawn once while the menu waits — a texture every few frames,
       behind the menu's opaque background — so no transition stalls on a first use: the corridor's strip, the
       diorama's layers (PM_77, Resources/Art/yeni/pm77/diorama, Tools/import_pm77_diorama.py), the panels' art and
       the fonts' glyphs. */
    public static class ArcAssets76
    {
        const string Folder = "Art/yeni/pm77/diorama/";
        static Diorama77.Art _diorama;
        static bool _started, _ready;
        static readonly string[] UiFolders = { "Art/yeni/T45/Buttons", "Art/yeni/T45/Pause", "Art/yeni/T48/settings", "Art/yeni/T50/win",
                                               "Art/yeni/boosters21/Selection", "Art/yeni/boosters21/Boosters/Wand", "Art/yeni/devam_v2" };

        public static bool Ready => _ready;
        /* PM_77: the menu's room as a diorama (null until loaded, or when the art is missing). */
        public static Diorama77.Art Diorama => _diorama;

        /* PM_77 &gecis=yay: the menu's room is the diorama, so it is needed at the menu's first frame. */
        public static Diorama77.Art LoadDioramaNow()
        {
            if (_diorama != null) return _diorama;
            var art = ReadArt();
            if (art == null) return null;
            art.Wall = Resources.Load<Texture2D>(Folder + "duvar");
            art.Floor = Resources.Load<Texture2D>(Folder + "zemin");
            art.Column = Resources.Load<Texture2D>(Folder + "on_sutun");
            if (art.Wall != null && art.Floor != null) _diorama = art;
            return _diorama;
        }

        static Diorama77.Art ReadArt()
        {
            var json = Resources.Load<TextAsset>(Folder + "diorama");
            if (json == null) return null;
            var art = new Diorama77.Art();
            var root = JsonReader.Parse(json.text);
            Vector2 V(string key, Vector2 fallback) =>
                root.TryGet(key, out var n) && n.Count >= 2 ? new Vector2((float)n[0].AsNumber, (float)n[1].AsNumber) : fallback;
            art.WallPx = V("wall_size", art.WallPx); art.FloorPx = V("floor_size", art.FloorPx); art.ColumnPx = V("column_size", art.ColumnPx);
            if (root.TryGet("canvas", out var c) && c.Count >= 4)
                art.CanvasPx = Rect.MinMaxRect((float)c[0].AsNumber, (float)c[1].AsNumber, (float)c[2].AsNumber, (float)c[3].AsNumber);
            if (root.TryGet("px_per_unit", out var ppu)) art.PxPerUnit = (float)ppu.AsNumber;
            art.FloorPxPerUnit = root.TryGet("floor_px_per_unit", out var fp) ? (float)fp.AsNumber : art.PxPerUnit;
            art.ColumnPxPerUnit = root.TryGet("column_px_per_unit", out var cp) ? (float)cp.AsNumber : art.PxPerUnit;
            return art;
        }

        public static void Preload(MonoBehaviour host, Transform parent)
        {
            if (_started || host == null) return;
            _started = true;
            host.StartCoroutine(Load(parent));
        }

        static IEnumerator Load(Transform parent)
        {
            yield return new WaitForSecondsRealtime(.8f);   // after the menu's own arrival (MenuEnter76, .6 s)
            var warm = new GameObject("PM76 warm-up").transform;
            if (parent != null) warm.SetParent(parent, false);
            var group = warm.gameObject.AddComponent<SortingGroup>();
            group.sortingOrder = -32000;   // under the menu's background: drawn (uploaded), never seen
            // the diorama
            // only for &gecis=yay (the arc waits for Zeyd's approval): without it nobody needs ~8 MB of textures (iPhone
            // has no DXT: there they would be decompressed, ~40 MB); with it the menu has loaded them already
            var art = _diorama == null && Dive75.Orbit ? ReadArt() : null;
            if (art != null)
            {
                var wallReq = Resources.LoadAsync<Texture2D>(Folder + "duvar"); yield return wallReq;
                art.Wall = wallReq.asset as Texture2D; Draw(warm, art.Wall as Texture2D); yield return null; yield return null;
                var floorReq = Resources.LoadAsync<Texture2D>(Folder + "zemin"); yield return floorReq;
                art.Floor = floorReq.asset as Texture2D; Draw(warm, art.Floor as Texture2D); yield return null; yield return null;
                var columnReq = Resources.LoadAsync<Texture2D>(Folder + "on_sutun"); yield return columnReq;
                art.Column = columnReq.asset as Texture2D; Draw(warm, art.Column as Texture2D); yield return null;
                if (art.Wall != null && art.Floor != null && _diorama == null) _diorama = art;
            }
            // the corridor's own strip (Play loads it; loaded now it costs nothing then)
            var stripReq = Resources.LoadAsync<Sprite>("corridor_strip"); yield return stripReq;
            Draw(warm, (stripReq.asset as Sprite)?.texture); yield return null;
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
                const string chars = "ABCÇDEFGĞHIİJKLMNOÖPRSŞTUÜVYZabcçdefgğhıijklmnoöprsştuüvyzQWXqwx0123456789+-:·.,!?%()'/";
                TextView.Create(warm, "warm heading", chars, 22, Palette.Cream, order: 0).Heading(TextView.RimTeal);
                TextView.Create(warm, "warm body", chars, 15, Palette.Navy, false, order: 0);
                TextView.Create(warm, "warm title", chars, 30, Palette.Cream, order: 0).Title3D();
                yield return null; yield return null;
            }
            if (warm != null) Object.Destroy(warm.gameObject);
            _ready = true;
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
