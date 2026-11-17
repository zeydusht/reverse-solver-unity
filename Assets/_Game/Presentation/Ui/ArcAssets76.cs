using System.Collections;
using ReverseSolver.Core.Json;
using UnityEngine;
using UnityEngine.Rendering;

namespace ReverseSolver.Presentation
{
    /* PM_76/77: what the transitions use, loaded and drawn once while the menu waits — a texture every few frames,
       behind the menu's opaque background — so no transition stalls on a first use: the corridor's strip, the
       diorama's layers (PM_78: the menu picture's plate and its front cut-outs, Resources/Art/yeni/pm78/diorama,
       Tools/pm78_import.py), the panels' art and the fonts' glyphs. */
    public static class ArcAssets76
    {
        const string Folder = "Art/yeni/pm78/diorama/";
        static Diorama78.Art _diorama;
        static bool _started, _ready;
        static readonly string[] UiFolders = { "Art/yeni/T45/Buttons", "Art/yeni/T45/Pause", "Art/yeni/T48/settings", "Art/yeni/T50/win",
                                               "Art/yeni/boosters21/Selection", "Art/yeni/boosters21/Boosters/Wand", "Art/yeni/devam_v2" };

        public static bool Ready => _ready;
        /* PM_78: the menu picture as a diorama (null until loaded, or when the art is missing). */
        public static Diorama78.Art Diorama => _diorama;

        /* &gecis=yay: the menu's room is the diorama, so it is needed at the menu's first frame. */
        public static Diorama78.Art LoadDioramaNow()
        {
            if (_diorama != null) return _diorama;
            var art = ReadArt();
            if (art == null) return null;
            art.Room = Resources.Load<Texture2D>(Folder + "oda");
            foreach (var f in art.Fronts) f.Tex = Resources.Load<Texture2D>(Folder + f.Name);
            if (art.Room != null) _diorama = art;
            return _diorama;
        }

        /* diorama.json (Tools/pm78_import.py): the plate, the solved start camera, Zeyd's keys, the front cut-outs. */
        static Diorama78.Art ReadArt()
        {
            var json = Resources.Load<TextAsset>(Folder + "diorama");
            if (json == null) return null;
            var art = new Diorama78.Art();
            var root = JsonReader.Parse(json.text);
            Vector2 P(JsonNode n) => new Vector2((float)n[0].AsNumber, (float)n[1].AsNumber);
            Vector2 V(string key, Vector2 fallback) => root.TryGet(key, out var n) && n.Count >= 2 ? P(n) : fallback;
            float F(string key, float fallback) => root.TryGet(key, out var n) ? (float)n.AsNumber : fallback;
            art.PlateSize = V("plate_size", art.PlateSize); art.ImageOrigin = V("image_origin", art.ImageOrigin); art.ImageSize = V("image_size", art.ImageSize);
            art.F = F("f", art.F); art.Cx = F("cx", art.Cx); art.Cy = F("cy", art.Cy);
            art.StartDeg = F("start_deg", art.StartDeg); art.D = F("d", art.D); art.H = F("h", art.H); art.Yc = F("yc", art.Yc);
            art.Seconds = F("seconds", art.Seconds);
            if (root.TryGet("canvas_px", out var c) && c.Count == 4)
                for (int i = 0; i < 4; i++) art.CanvasPx[i] = P(c[i]);
            if (root.TryGet("keys", out var k) && root.TryGet("durs", out var d) && k.Count >= 2 && d.Count == k.Count)
            {
                art.Keys = new Vector2[k.Count]; art.Durs = new float[k.Count];
                for (int i = 0; i < k.Count; i++) { art.Keys[i] = P(k[i]); art.Durs[i] = (float)d[i].AsNumber; }
            }
            if (root.TryGet("fronts", out var fronts))
                for (int i = 0; i < fronts.Count; i++)
                {
                    var f = fronts[i];
                    if (!f.TryGet("name", out var name) || !f.TryGet("px", out var px) || px.Count < 4) continue;
                    art.Fronts.Add(new Diorama78.Front
                    {
                        Name = name.AsString,
                        Px = Rect.MinMaxRect((float)px[0].AsNumber, (float)px[1].AsNumber, (float)px[2].AsNumber, (float)px[3].AsNumber),
                        Depth = f.TryGet("depth", out var dep) ? (float)dep.AsNumber : 0
                    });
                }
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
            // the flight is the default (PM_79; not with &gecis=eski): the plate is ~10 MB once decompressed (iPhone has no
            // DXT); with the flag the menu has loaded it already
            var art = _diorama == null && Dive75.Orbit ? ReadArt() : null;
            if (art != null)
            {
                var roomReq = Resources.LoadAsync<Texture2D>(Folder + "oda"); yield return roomReq;
                art.Room = roomReq.asset as Texture2D; Draw(warm, art.Room as Texture2D); yield return null; yield return null;
                foreach (var f in art.Fronts)
                {
                    var req = Resources.LoadAsync<Texture2D>(Folder + f.Name); yield return req;
                    f.Tex = req.asset as Texture2D; Draw(warm, f.Tex as Texture2D); yield return null;
                }
                if (art.Room != null && _diorama == null) _diorama = art;
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
