using System.Collections.Generic;
using ReverseSolver.Core;
using UnityEngine;

namespace ReverseSolver.Presentation
{
    /* PM_82 on the sl100 set:
         lessons      the obstacle lessons (lock 3, chain 7, bomb 11, wall 21) and the booster lessons (scissors 6, hammer
                      13, clock 18, wand 25) come from the level data, not from p22's slot numbers; the scripted steps
                      of tutorial.json are found by what the level teaches
         hard levels  no start card to tap: a short ribbon "Hard level" (red) / "Very hard level" (dark red, the
                      Count's seal) as the level begins, the menu's button in the same colours (Screens.Menu)
         R3           a booster at 0: a small video mark on its slot; a tap shows a rewarded ad, +1 of it (r3_gunluk a day)
         R4           the menu's gift: a rewarded ad, +100 gold (r4_gunluk a day); not there when no ad is ready */
    public sealed partial class GameRoot
    {
        /* tutorial.json's slot for what this level teaches (0: nothing scripted; level 1 has its own guide). */
        static int ScriptSlot82(LevelData l)
        {
            if (l == null || l.Number == 1) return 0;
            switch (l.Lesson)
            {
                case "lock": return 3;
                case "bomb": return 6;
                case "chain": return 10;
                case "wall": return 14;
            }
            switch (l.BoosterIntro?.Id)
            {
                case "scissors": return 5;
                case "wand": return 9;
                case "clock": return 15;
                case "hammer": return 23;
            }
            return 0;
        }

        /* The start card on sl100: lessons, booster intros and the generic slots as before; a hard level gets the
           short ribbon instead of a card. */
        static bool SlIntro82(LevelData l, bool generic) => !l.Hard && Screens.HasIntro(l, generic);

        // ---- the hard ribbon ----

        bool _hardRibbonDue82;
        Transform _hardRibbon82;

        /* PM_86 §6: a touch closes the ribbon at once (a quick .12 s fade). */
        void DismissHardRibbon82()
        {
            var root = _hardRibbon82; _hardRibbon82 = null;
            if (root == null) return;
            Tweens.Kill(root);
            var sprites = root.GetComponentsInChildren<SpriteRenderer>();
            var texts = root.GetComponentsInChildren<TextView>();
            Tweens.Run(root, .12f, k =>
            {
                if (root == null) return;
                float a = 1 - k * k;
                foreach (var r in sprites) if (r != null) { var col = r.color; col.a = Mathf.Min(col.a, a); r.color = col; }
                foreach (var t in texts) if (t != null) t.Alpha = Mathf.Min(t.Alpha, a);
            }, () => { if (root != null) Destroy(root.gameObject); });
        }
        void QueueHardRibbon82(LevelData l, bool firstAttempt)
        {
            _hardRibbonDue82 = _slSet && l.Hard && firstAttempt;
        }

        void TickHardRibbon82()
        {
            if (!_hardRibbonDue82 || _diving || _mode != Mode.Playing || _board == null || _card != null) return;
            _hardRibbonDue82 = false;
            bool very = _session.Level.Role == "veryhard";
            var root = new GameObject("PM82 hard ribbon").transform;
            _hardRibbon82 = root;
            root.SetParent(_world, false);
            float w = Mathf.Min(_screen.x - 40, 320), h = 64;
            var c = new Vector2(_screen.x / 2, _board.Origin.y + _board.Size.y * .5f);
            var art = Art.Place(root, very ? "level-button-darkred" : "level-button-red", new Vector2(w, h * 1.25f), CardOrder - 10, sliced: true);
            if (art != null) art.transform.localPosition = Draw.W(c);
            var t = TextView.Create(root, "title", very ? Texts.VeryHardTitle : Texts.HardTitle, 30, Palette.Cream, order: CardOrder - 8)
                .Title3D().FitWidth(w - 70, 18);
            t.transform.localPosition = Draw.W(c);
            if (very)
            {
                var seal = Art.Place(root, "yeni/T50/map/badge_kont", Vector2.one * 70, CardOrder - 7);
                if (seal != null) seal.transform.localPosition = Draw.W(c + new Vector2(-w / 2 + 8, -h * .55f));
                Haptics.Play(Haptics.Kind.Heavy);   // PM_86 §4: the Count's seal at a very hard level's start
            }
            // in with a small spring, a short look, out (about 1.8 s); taps go through to the board
            Tweens.Run(root, 1.8f, k =>
            {
                if (root == null) return;
                float s = k < .15f ? Mathf.LerpUnclamped(.6f, 1, PanelMotion76.OutBack(k / .15f, 1.4f)) : 1;
                float a = k < .12f ? k / .12f : k > .8f ? 1 - (k - .8f) / .2f : 1;
                root.localScale = Vector3.one * s;
                root.localPosition = Draw.W(c * (1 - s));
                foreach (var r in root.GetComponentsInChildren<SpriteRenderer>()) { var col = r.color; col.a = a; r.color = col; }
                t.Alpha = a;
            });
            Destroy(root.gameObject, 1.9f);
        }

        // ---- R3: a booster at 0 ----

        readonly List<GameObject> _r3Marks = new List<GameObject>();
        string _r3Key;

        bool OfferBooster82(Booster b) => _session != null && !_session.IsOver && _session.IsUnlocked(b) && _session.Stock(b) == 0 &&
                                          Ads.CanOffer(AdPlacement.R3Booster);

        /* The video marks on the empty slots (rebuilt when what they show changes). */
        void TickR3Marks82()
        {
            if (_bar == null || _session == null || _mode != Mode.Playing) { ClearR3Marks82(); return; }
            var key = new System.Text.StringBuilder();
            foreach (var b in BoosterExt.All) key.Append(OfferBooster82(b) ? '1' : '0');
            string k = key.ToString();
            if (k == _r3Key && _r3Marks.TrueForAll(m => m != null)) return;
            ClearR3Marks82();
            _r3Key = k;
            foreach (var b in BoosterExt.All)
            {
                if (!OfferBooster82(b)) continue;
                var r = _bar.RectOf(b);
                var root = new GameObject("R3 mark " + b).transform;
                root.SetParent(_world, false);
                var at = new Vector2(r.xMax - 12, r.yMin + 12);
                ShapeView.Create(root, "badge", new Vector2(30, 22), 4, BarOrder + 40).Radius(7).Fill(Draw.Hex("#C62828")).Stroke(Palette.Cream, 2)
                    .transform.localPosition = Draw.W(at);
                SolidView.Polygon(root, "play", new[] { at + new Vector2(-4, -6), at + new Vector2(6, 0), at + new Vector2(-4, 6) }, Palette.Cream, BarOrder + 41);
                _r3Marks.Add(root.gameObject);
            }
        }

        void ClearR3Marks82()
        {
            foreach (var m in _r3Marks) if (m != null) Destroy(m);
            _r3Marks.Clear();
            _r3Key = null;
        }

        /* A tap on an empty slot with the mark: the ad, then +1 of that booster. True when it took the tap. */
        bool PressBoosterAd82(Booster b)
        {
            if (!OfferBooster82(b)) return false;
            Ads.ShowReward(AdPlacement.R3Booster, ok =>
            {
                if (!ok || _session == null) return;
                _inventory?.Grant(b, 1);
                _session.SetStock(b, _session.Stock(b) + 1);
                _r3Key = null;
                RefreshPlaying();
                _bar?.Notice(Lang.T("+1 booster!", "+1 booster!"), 1.2f);
            });
            return true;
        }

        // ---- R4: the menu's gift ----

        Rect _gift82;
        Transform _giftView82;

        void BuildGift82()
        {
            _gift82 = default;
            if (_menu == null || !Ads.CanOffer(AdPlacement.R4Gift, true)) return;
            var at = new Vector2(_screen.x - 44, _menu.Play.yMin - 52);
            _giftView82 = new GameObject("R4 gift").transform;
            _giftView82.SetParent(_menu.Root, false);
            _giftView82.localPosition = Draw.W(at);
            Art.Place(_giftView82, "yeni/win21/Reward/Chest/closed", new Vector2(64, 55), HudOrder + 20);
            var coin = Art.Place(_giftView82, P21WinView.CoinKey, new Vector2(28, 28), HudOrder + 22);   // PM_83: the gold it gives
            if (coin != null) coin.transform.localPosition = Draw.W(24, -20);
            MenuUi.Label(_giftView82, Lang.T("Gift", "Hediye"), new Vector2(0, 38), 13, HudOrder + 21, width: 80);
            _gift82 = new Rect(at - new Vector2(36, 34), new Vector2(72, 82));
        }

        bool PressGift82(Vector2 pt)
        {
            if (_gift82.width <= 0 || !_gift82.Contains(pt)) return false;
            Ads.ShowReward(AdPlacement.R4Gift, ok =>
            {
                if (!ok) return;
                _save.SetCoins((int)System.Math.Min(int.MaxValue, (long)_save.Coins + Live.Config.R4Coins));
                if (_mode == Mode.Menu) Layout();
            });
            return true;
        }

        /* Every frame (Update): the hard ribbon, the R3 marks, the gift's wobble. */
        void Tick82()
        {
            TickHardRibbon82();
            TickR3Marks82();
            if (_giftView82 != null) _giftView82.localRotation = Quaternion.Euler(0, 0, 4 * Mathf.Sin(Time.unscaledTime * 3) * (Mathf.Sin(Time.unscaledTime * .9f) > .6f ? 1 : 0));
        }
    }
}
