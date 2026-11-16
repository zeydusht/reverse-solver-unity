using System.Collections.Generic;
using ReverseSolver.Core;
using UnityEngine;

namespace ReverseSolver.Presentation
{
    /* PM_75: the transitions Zeyd asked back and the slow dive.
         win -> Next     the corridor slides to the next painting (PM_MESAJ_24), then the dive into it
         win -> Menu     the corridor slides to the next painting, then the menu
         menu -> Level N the dive into the menu's painting
       &gecis=yok: no dive (the corridor still slides; menu -> level at once). */
    public sealed partial class GameRoot
    {
        Dive75 _dive75;

        /* Menu -> level: the camera comes round to face the menu's painting, dives in, the level comes out of it. */
        bool DiveFromMenu75(int index)
        {
            if (!Dive75.On || _menu == null || _menu.Background == null || _menu.PaintingTexture == null || !_p21Lessons) return false;
            var quad = _menu.PaintingQuad();
            if (quad == null) return false;
            var level = _set.Levels[Mathf.Clamp(index, 0, _set.Levels.Count - 1)];
            var fade = MenuFader();
            var bg = _menu.Background;
            var setup = new Dive75.Setup
            {
                Cam = _cam, Screen = _screen, Quad = quad, Crop = _menu.PaintingCrop, Picture = _menu.PaintingTexture,
                // the copies take the wall's own place (the menu's controls above them fade), then rise over everything
                BoardAspect = (float)level.Width / level.Height, Order = bg.sortingOrder - 2, TopOrder = DiveOrder,
                Backdrop = Draw.Hex("#0B2A3A"), FadeUi = fade,
                Hide = () => { if (bg != null) bg.enabled = false; if (_menu?.PaintingSprite != null) _menu.PaintingSprite.enabled = false; },
            };
            // PM_76: the camera arc through the drawn views (when they are loaded); else the facing homography
            setup.Arc = ArcAssets76.Views; setup.ArcSize = ArcAssets76.Size;
            if (setup.Arc == null) setup.Walls.Add(bg);
            return StartDive75(setup, index);
        }

        /* Win -> next: the corridor slides to the next painting; at its end the dive takes over from the corridor. */
        bool CorridorToNext75()
        {
            if (!_p21Lessons || _board == null || _index + 1 >= _set.Levels.Count) return false;
            int next = _index + 1;
            var won = _board.Image as Texture2D;
            CorridorTransition c = null;
            var nextPic = _images.Get(ImageOf(next), t => { if (c != null) c.SetNext(t); });
            c = CorridorTransition.Play(_screen, CorridorOrder, won, nextPic, () =>
            {
                _corridor = null;
                if (!Dive75.On) { StartLevel(next); return; }
                var tex = c.NextTexture;
                var quad = c.NextQuad();
                var level = _set.Levels[next];
                var setup = new Dive75.Setup
                {
                    Cam = _cam, Screen = _screen, Quad = quad, Crop = c.NextCrop, Picture = tex,
                    BoardAspect = (float)level.Width / level.Height, Order = DiveOrder, TopOrder = DiveOrder,
                    Backdrop = new Color(.09f, .07f, .06f, 1),
                };
                setup.Walls.Add(c.Strip);
                foreach (var column in c.NearColumns) setup.Walls.Add(column);   // PM_76: the near columns ride along
                // PM_76: the slide is still moving at the handover (80 %): the turn starts at speed, the approach soon after
                setup.FaceSeconds = .4f; setup.NearAt = .12f; setup.FaceFromMotion = true;
                if (tex == null || quad == null || !StartDive75(setup, next)) { StartLevel(next); return; }
                c.DestroyNow();
            }, handoff: Dive75.On);
            if (c == null) return false;
            _corridor = c;
            return true;
        }

        /* Win -> menu: the corridor slides to the next painting, the menu fades in under it (PM_MESAJ_24). */
        bool CorridorToMenu75()
        {
            if (!_p21Lessons || _board == null || _index + 1 >= _set.Levels.Count) return false;
            int next = _index + 1;
            CorridorTransition c = null;
            var nextPic = _images.Get(ImageOf(next), t => { if (c != null) c.SetNext(t); });
            c = CorridorTransition.Play(_screen, CorridorOrder, _board.Image as Texture2D, nextPic, () => { _corridor = null; ShowMenu(); });
            if (c == null) return false;
            _corridor = c;
            return true;
        }

        bool StartDive75(Dive75.Setup setup, int index)
        {
            _diving = true;
            Transform card = null;
            float top = 0, bottom = 0;
            Transform hud = null, bar = null;
            setup.Build = () =>
            {
                StartLevel(index);
                if (_board == null || _hud == null || _bar == null) return null;
                _board.SkipDeal();
                hud = _hud.Root; bar = _bar.Root;
                top = Hud54.TopBottom(_screen, _safe.y) + 20; bottom = _screen.y - Hud54.BottomTop(_screen, _safe.w) + 20;
                card = _card?.Root;
                if (card != null) card.gameObject.SetActive(false);
                return _board;
            };
            setup.Slide = k =>
            {
                if (hud != null) hud.localPosition = Draw.W(0, -top * (1 - k));
                if (bar != null) bar.localPosition = Draw.W(0, bottom * (1 - k));
            };
            setup.Done = () =>
            {
                _diving = false; _dive75 = null;
                if (card != null) card.gameObject.SetActive(true);
            };
            _dive75 = Dive75.Run(setup);
            return true;
        }

        /* The menu's own controls (everything but the wall and its painting) fade with the first turn of the camera. */
        System.Action<float> MenuFader()
        {
            var bg = _menu.Background; var painting = _menu.PaintingSprite;
            var sprites = new List<(SpriteRenderer r, float a)>();
            var texts = new List<(TMPro.TMP_Text t, float a)>();
            var shapes = new List<ShapeView>();
            foreach (var r in _world.GetComponentsInChildren<SpriteRenderer>())
                if (r != bg && r != painting) sprites.Add((r, r.color.a));
            foreach (var t in _world.GetComponentsInChildren<TMPro.TMP_Text>()) texts.Add((t, t.alpha));
            foreach (var s in _world.GetComponentsInChildren<ShapeView>()) shapes.Add(s);
            float last = -1;
            return a =>
            {
                if (Mathf.Approximately(a, last)) return;
                last = a;
                foreach (var (r, a0) in sprites) if (r != null) { var c = r.color; c.a = a0 * a; r.color = c; }
                foreach (var (t, a0) in texts) if (t != null) t.alpha = a0 * a;
                foreach (var s in shapes) if (s != null) s.Alpha(a);
            };
        }
    }
}
