using ReverseSolver.Core;
using UnityEngine;

namespace ReverseSolver.Presentation
{
    public sealed partial class GameRoot
    {
        bool P21WinArt => Art.P21Gameplay || Art.T54 && _testMode;
        P21WinView _win21;
        bool _winPreview21;
        Rect[] WinTargets21()
        {
            var targets = new Rect[4];
            foreach (var b in BoosterExt.All) targets[(int)b] = _bar.RectOf(b);
            return targets;
        }
        void ShowWin21(bool chest = false, bool preview = false)
        {
            if (_win21 != null) return;
            CancelCardTap(); CloseCard(); _introOpen = false; _endAt = -1;
            _winPreview21 = preview;
            var save = _save;
            if (preview)
            {
                save = new SaveData(new MemoryStore(), () => "chest-preview");
                save.TestMode = _testMode;   // PM_72: the test link shows its gold in the preview too
                save.RecordChestWin21(chest ? 3 : _session.Level.Number);
            }
            else _save.RecordChestWin21(_session.Level.Number);
            // PM_75: the Next button reads "Level N+1", like the menu's "Level N"
            P21WinView.NextTitle = _index + 1 < _set.Levels.Count
                ? string.Format(Texts.PlayLevel, _p21Flow ? _index + 2 : _set.Levels[_index + 1].Number) : null;
            _win21 = new P21WinView(transform, _screen, WinTargets21(), save, _session.Level.Pieces.Count,
                preview || ContinueEnabled ? _continueRules.WinCoins : 0, chest, NextWin21, () =>
                {
                    if (preview) return;
                    _inventory = new BoosterInventory(Host.Current.Store, _session.Level, i => i >= 0 && i < _set.Levels.Count && _progress.Won(i));
                    _inventory.Apply(_session); RefreshPlaying();
                }, () => { _win21?.Destroy(); _win21 = null; _winPreview21 = false; if (!CorridorToMenu75()) ShowMenu(); });   // PM_75: the corridor back
        }
        void NextWin21()
        {
            bool preview = _winPreview21;
            _win21?.Destroy(); _win21 = null; _winPreview21 = false;
            if (preview) { Layout(); return; }
            // PM_75: the corridor slides to the next painting, the slow dive takes over at its end
            if (_index + 1 < _set.Levels.Count) { if (!CorridorToNext75()) StartLevel(_index + 1); } else ShowMenu();
        }

        // ---- PM_74: warm-up ---------------------------------------------------------------------------------------

        /* The first level start of a visit spent one ~0.3 s frame on first use (shaders, textures, fonts); later starts
           do not. Once per visit, while the menu is built, the next level's HUD, board and booster bar are drawn for two
           frames behind the menu's opaque background (a sorting group under it), then dropped. */
        static bool _warmed;
        void WarmLevelOnce()
        {
            if (_warmed || !_p21Lessons || _set == null || _progress == null || Host.Current.Query("isinma") == "0") return;
            _warmed = true;
            var level = _set.Levels[Mathf.Clamp(_progress.Next(), 0, _set.Levels.Count - 1)];
            var root = new GameObject("PM74 warm-up").transform; root.SetParent(_world, false);
            var group = root.gameObject.AddComponent<UnityEngine.Rendering.SortingGroup>();
            group.sortingOrder = -32000;   // under the background (-1000): drawn, never seen
            try
            {
                var session = new GameSession(level, 1, "warm", new Mulberry32(1));
                var origin = PlayingBoardPlacement(_screen, _safe, level, out float cell);
                BoardView.Create(root, session, cell, origin, P21Boosters);
                new Hud(root, _screen.x, _safe.y + BodyPad, HudOrder, _screen, _safe.y, session, UseHud54);
                new BoosterBar(root, _screen, _safe.w, BarOrder, UseHud54);
            }
            catch (System.Exception e) { Debug.LogWarning("[warm] " + e.Message); }
            StartCoroutine(DropWarm(root.gameObject));
        }
        System.Collections.IEnumerator DropWarm(GameObject warm)
        {
            yield return null; yield return null;
            if (warm != null) Destroy(warm);
        }

        // ---- PM_74: the dive into the painting ------------------------------------------------------------------

        bool _diving;
        const int DiveOrder = 9000;

        /* Menu -> level: into the menu's painting (the one the level frees), then out of the board's. */
        bool DiveFromMenu(int index)
        {
            if (!Dive74.On || _menu == null || !(_menu.PaintingBounds is Bounds b) || !_p21Lessons) return false;
            _diving = true;
            Dive74.Rise(_cam, Dive74.In * .6f, Dive74.In * .4f, DiveOrder + 10);
            Dive74.Run(_cam, new Vector3(b.center.x, b.center.y, -10), Dive74.CoverSize(b.size, _screen), Dive74.In, true, () =>
            {
                StartLevel(index);
                DiveOut();
            });
            return true;
        }

        /* Win -> next: the finished picture shrinks to the wall on the left as the next one comes in from the right
           in its frame; the camera dives into the new one; the level comes out of it. */
        bool DiveToNext()
        {
            if (!Dive74.On || _board == null || !_p21Lessons) return false;
            int next = _index + 1;
            var won = _board.Image;
            var nextTex = _images.Get(ImageOf(next));
            if (won == null || nextTex == null) return false;
            _diving = true;
            var rect = new Rect(_board.Origin, _board.Size);
            var gallery = new GameObject("PM74 gallery").transform;
            var wall = ShapeView.Create(gallery, "wall", _screen * 1.2f, 0, DiveOrder).Radius(0).Fill(Draw.Hex("#0E2244"));
            wall.transform.localPosition = Draw.W(_screen / 2);
            // the win screen's blue light stays on the wall, so the win and the gallery read as one moment
            float cover = Mathf.Max(_screen.x / 390, _screen.y / 844);
            var glow = Art.Place(gallery, "T50/win/win_bg_glow", new Vector2(390, 844) * cover, DiveOrder + 1);
            if (glow != null) { glow.transform.localPosition = Draw.W(_screen / 2); glow.color = new Color(1, 1, 1, .85f); }
            Transform Framed(string name, Texture tex, Vector2 size)
            {
                var holder = new GameObject(name).transform; holder.SetParent(gallery, false);
                PicturePiece.Create(holder, tex, size, DiveOrder + 2);
                float band = 13f, fk = band / (300f / 3f);
                var frame = Art.Place(holder, "board-frame", new Vector2((size.x + band * 1.4f) / fk, (size.y + band * 1.4f) / fk), DiveOrder + 3, sliced: true);
                if (frame != null) frame.transform.localScale = new Vector3(fk, fk, 1);
                return holder;
            }
            var a = Framed("won", won, rect.size);
            // the next picture keeps its own proportions, at the won board's width
            var levelNext = _set.Levels[next];
            var nextSize = new Vector2(rect.width, rect.width * levelNext.Height / levelNext.Width);
            if (nextSize.y > rect.height * 1.25f) nextSize *= rect.height * 1.25f / nextSize.y;
            var bNext = Framed("next", nextTex, nextSize);
            var centre = rect.center;
            const float Hang = .26f, small = .62f;
            Tweens.Run(gallery, Hang, t =>
            {
                if (gallery == null) return;
                float e = BoosterFx74.InOutCubic(t);
                wall.Alpha(Mathf.Clamp01(t * 3));
                a.localPosition = Draw.W(Vector2.Lerp(centre, new Vector2(-_screen.x * .2f, centre.y - 20), e));
                a.localScale = Vector3.one * Mathf.Lerp(1, small * .8f, e);
                bNext.localPosition = Draw.W(Vector2.Lerp(new Vector2(_screen.x * 1.25f, centre.y + 10), centre, e));
                bNext.localScale = Vector3.one * Mathf.Lerp(small * .8f, small, e);
            }, () =>
            {
                var target = new Rect(centre - nextSize * small / 2, nextSize * small);
                Dive74.Rise(_cam, .26f * .5f, .26f * .5f, DiveOrder + 10);
                Dive74.Run(_cam, new Vector3(target.center.x, -target.center.y, -10), Dive74.CoverSize(target.size, _screen), .26f, true, () =>
                {
                    if (gallery != null) Destroy(gallery.gameObject);
                    StartLevel(next);
                    DiveOut();
                });
            });
            return true;
        }

        /* After the opening story: level 1 comes out of the painting the story ends on. */
        void DiveOutOfStory()
        {
            if (!Dive74.On || !_p21Lessons || _board == null || _mode != Mode.Playing) return;
            _diving = true;
            DiveOut(.32f);   // the story's video fades out over the zoomed board first
        }

        /* After StartLevel: the camera starts on the board's picture, pulls back; the HUD and the bar slide in. */
        void DiveOut(float delay = 0)
        {
            if (_board == null || _hud == null || _bar == null) { _diving = false; return; }
            _board.SkipDeal();
            var home = _cam.transform.position; float homeSize = _cam.orthographicSize;
            var rect = new Rect(_board.Origin, _board.Size);
            _cam.transform.position = new Vector3(rect.center.x, -rect.center.y, -10);
            _cam.orthographicSize = Dive74.CoverSize(rect.size, _screen);
            Dive74.Fade(_cam, Dive74.Light + .08f);
            var hud = _hud.Root; var bar = _bar.Root;
            float top = Hud54.TopBottom(_screen, _safe.y) + 20, bottom = _screen.y - Hud54.BottomTop(_screen, _safe.w) + 20;
            var card = _card?.Root;
            if (card != null) card.gameObject.SetActive(false);
            void Slide(float k)
            {
                if (hud != null) hud.localPosition = Draw.W(0, -top * (1 - k));
                if (bar != null) bar.localPosition = Draw.W(0, bottom * (1 - k));
            }
            Slide(0);
            if (delay > 0)
            {
                var camAt = _cam.transform.position; float sizeAt = _cam.orthographicSize;
                Tweens.Run(new object(), delay, _ => { if (_cam != null) { _cam.transform.position = camAt; _cam.orthographicSize = sizeAt; } },
                    () => DiveOut2(home, homeSize, card, Slide));
                return;
            }
            DiveOut2(home, homeSize, card, Slide);
        }

        void DiveOut2(Vector3 home, float homeSize, Transform card, System.Action<float> Slide)
        {
            Dive74.Run(_cam, home, homeSize, Dive74.Out, false, () =>
            {
                Tweens.Run(new object(), Dive74.HudSlide * .4f, t => Slide(Mathf.Lerp(.6f, 1, BoosterFx74.OutCubic(t))), () =>
                {
                    Slide(1);
                    _diving = false;
                    if (card != null) card.gameObject.SetActive(true);
                });
            }, e => Slide(Mathf.Clamp01((e - .55f) / .45f) * .6f));
        }
        void TestWin21()
        {
            if (!P21WinArt) return;
            if (Host.Current.Query("sandik") == "1") ShowWin21(true, true);
            else if (Host.Current.Query("kazan") == "1") ShowWin21(false, true);
        }
    }
}
