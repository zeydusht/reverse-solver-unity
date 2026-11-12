using System.Collections.Generic;
using ReverseSolver.Core;
using TMPro;
using UnityEngine;

namespace ReverseSolver.Presentation
{
    /* The web game's .prompt line and .bar of four .bst buttons (index.html):
       60pt buttons, 13pt apart, count badge top-right, name underneath; armed
       (.on) amber, out of stock and locked greyed, the level's new booster
       pulsing (.fresh). It only draws BoosterControls' state; taps come back
       through Hit(). */
    public sealed class BoosterBar
    {
        public const float Height = 97f, PromptH = 17f;

        readonly Transform _root;
        readonly float _screenWidth;
        readonly BoosterBar54 _t54;
        public BarAction HitAction(Vector2 pt) => _t54?.HitAction(pt) ?? BarAction.None;
        readonly TextView _prompt;
        readonly List<Slot> _slots = new List<Slot>();

        Transform _lockNotice;
        public void LockedNotice(Booster booster, GameSession session)
        {
            if (session.Level.Unlock == null || !session.Level.Unlock.TryGetValue(booster.Id(), out int level)) return;
            if (_lockNotice != null) Object.Destroy(_lockNotice.gameObject);
            var bubble = _lockNotice = new GameObject("Locked booster notice").transform;
            bubble.SetParent(_root, false);
            var rect = RectOf(booster);
            float x = Mathf.Clamp(rect.center.x, 88, _screenWidth - 88);
            bubble.localPosition = Draw.W(x, rect.yMin - 28);
            ShapeView.Create(bubble, "balloon", new Vector2(172, 34), 0, 3900).Radius(12).Fill(Palette.Navy);
            TextView.Create(bubble, "unlock level", Lang.T($"Unlocks at level {level}", $"Seviye {level}'de açılır"), 14, Vector4.one, order: 3901);
            Tweens.Run(bubble, 1.5f, t => {
                if (bubble == null) return;
                var label = bubble.GetComponentInChildren<TextView>();
                label.Alpha = Mathf.Clamp01((1 - t) * 5);
            }, () => { if (bubble != null) Object.Destroy(bubble.gameObject); });
        }

        public void ShowTutorial(bool showing) => _root.gameObject.SetActive(!showing);

        sealed class Slot
        {
            public Booster B;
            public Rect Rect;
            public ShapeView Face, Shine, Badge;
            public TextView Count, Name;
            public Transform Lock, Glyph;
            public string Key;
            public bool Fresh;
        }

        public BoosterBar(Transform parent, Vector2 screen, float bottomInset, int order, bool t54 = false)
        {
            _screenWidth = screen.x;
            _root = new GameObject("BoosterBar").transform;
            _root.SetParent(parent, false);
            if (t54 && Art.T54) { _t54 = new BoosterBar54(_root, screen, bottomInset, order); return; }
            float top = screen.y - bottomInset - 4 - Height;
            float cy = top + 7 + 30;
            float x0 = screen.x / 2 - (4 * 60 + 3 * 13) / 2f + 30;

            _prompt = TextView.Create(_root, "prompt", "", 12.5f, Draw.Hex("#f0a742"), order: order);
            _prompt.transform.localPosition = Draw.W(screen.x / 2, top - PromptH / 2);

            foreach (var b in BoosterExt.All)
            {
                float cx = x0 + (int)b * 73;
                var s = new Slot { B = b, Rect = new Rect(cx - 30, cy - 30, 60, 60) };
                // .bst: radius 16, gradient #6ecbf5 -> #2b86cf, 3px white ring, 0 6px 13px rgba(0,0,0,.42)
                s.Face = ShapeView.Create(_root, "bst", new Vector2(60, 60), 20, order).Radius(16).Skin(Art.T14 ? $"booster-{b.Id()}-normal" : Art.Get("booster-button") != null ? "booster-button" : "booster-normal", false)
                    .Stroke(Draw.Rgba(255, 255, 255, .92f), 3).Shadow(Draw.Rgba(0, 0, 0, .42f), 0, 6, 13)
                    .Inner(Draw.Rgba(0, 0, 0, .22f), -3, 6);
                s.Face.transform.localPosition = Draw.W(cx, cy);
                // radial-gradient(64% 46% at 28% 16%, rgba(255,255,255,.62), transparent 70%)
                s.Shine = ShapeView.Create(_root, "shine", new Vector2(56, 56), 0, order + 1).Radius(14)
                    .Radial(Draw.Rgba(255, 255, 255, .62f), Draw.Rgba(255, 255, 255, .18f), Draw.Rgba(255, 255, 255, 0),
                            new Vector4(.28f, .16f, .64f * 1.17f, .46f * 1.17f));
                s.Shine.transform.localPosition = Draw.W(cx, cy);
                s.Glyph = new GameObject("glyph").transform;
                s.Glyph.SetParent(_root, false);
                s.Glyph.localPosition = Draw.W(cx, cy);
                if (!Art.T14) Icons.Draw(b.Id(), s.Glyph, Art.On ? 27.2f : 33, order + 2);   // Task 14 buttons carry their icon   // art icons draw at 1.25x: 34 pt = ~75 % of the inner disc (PALETTE: never on the ring)
                // .badge: 24px circle at top -8 right -8, gradient #63c2ef -> #2f8fd6, 3px white ring
                // PM_MESAJ_13 (preview): red badge at the bottom right, 2 px light rim, white Fredoka digits
                var bc = Art.On ? new Vector2(cx + 30 - 5, cy + 30 - 5) : new Vector2(cx + 30 - 4, cy - 30 + 4);
                s.Badge = ShapeView.Create(_root, "badge", new Vector2(24, 24), 2, order + 3).Radius(12)
                    .Gradient(Draw.Hex("#63c2ef"), Draw.Hex("#2f8fd6")).Stroke(Draw.Rgba(255, 255, 255, .92f), 3);
                if (Art.On) s.Badge.Fill(Palette.Red).Stroke(Palette.WarnWhite, 2).Skin("booster-badge", false);   // PALETTE (Task 15): red, warning-white rim, cream digits
                else s.Badge.Skin("booster-badge-stock", false);
                s.Badge.transform.localPosition = Draw.W(bc);
                s.Count = TextView.Create(_root, "count", "", Art.On ? 15f : 12.5f, Draw.Hex("#ffffff"), order: order + 4);
                if (Art.On) s.Count.Font(Art.T14 ? Art.Lilita : Resources.Load<TMP_FontAsset>("Fonts/Counter"));
                if (Art.T14) s.Count.Color = Palette.Cream;
                s.Count.transform.localPosition = Draw.W(bc);
                s.Lock = new GameObject("lock").transform;
                s.Lock.SetParent(_root, false);
                s.Lock.localPosition = Draw.W(bc);
                if (!Art.T14) Icons.Draw("lock", s.Lock, 13, order + 4);
                // .bst-name: 9px, letter-spacing .16em, uppercase, muted
                s.Name = TextView.Create(_root, "name", Texts.BoosterName(b), 9, Draw.Hex("#8fb2b3"), order: order);
                s.Name.GetComponent<TextMeshPro>().characterSpacing = 16;
                s.Name.transform.localPosition = Draw.W(cx, cy + 30 + 5 + 6);
                if (Art.On) s.Name.gameObject.SetActive(false);                  // PM_MESAJ_13: no names under the buttons
                _slots.Add(s);
            }
        }

        string _notice;
        float _noticeUntil;

        /* A passing message on the prompt line (web: the valve's "Çivi kendiliğinden
           söküldü" for 1.6 s; the web says "söktü"); a booster's own hint takes precedence. */
        public void Notice(string text, float seconds)
        {
            if (_t54 != null) { _t54.Notice(text, seconds); return; }
            _notice = text;
            _noticeUntil = Time.unscaledTime + seconds;
        }

        /* quiet: a tutorial spotlight is up with its own line; the bar's prompt would sit under it (PM_MESAJ_30) */
        public void Refresh(BoosterControls bar, GameSession session, bool quiet = false)
        {
            if (_t54 != null) { _t54.Refresh(bar, session, quiet); return; }
            _prompt.Text = quiet ? "" : !string.IsNullOrEmpty(bar.Prompt) ? Texts.BoosterPrompt(bar)
                         : Time.unscaledTime < _noticeUntil ? _notice : "";
            foreach (var s in _slots)
            {
                var state = session.FreeUse == s.B && session.IsUnlocked(s.B) ? BoosterButton.Ready : bar.StateOf(s.B);
                bool on = bar.Armed == s.B;
                bool fresh = bar.IsFresh(s.B) && state == BoosterButton.Ready && !on;
                string key = $"{state}|{on}|{fresh}|{session.Stock(s.B)}|{Lang.Tr}";
                if (key == s.Key) continue;
                s.Key = key;

                Vector4 top, bottom;
                if (on) { top = Draw.Hex("#ffc866"); bottom = Draw.Hex("#f0a742"); }
                else if (state == BoosterButton.Locked) { top = Draw.Hex("#5a6266"); bottom = Draw.Hex("#33393c"); }   // grayscale(1) brightness(.5)
                else if (state == BoosterButton.Out) { top = Draw.Hex("#8f9a9f"); bottom = Draw.Hex("#4f5a60"); }      // grayscale(.85) brightness(.72)
                else { top = Draw.Hex("#6ecbf5"); bottom = Draw.Hex("#2b86cf"); }
                s.Face.Gradient(top, bottom);
                if (Art.T14)
                {
                    // Task 14: one combined sprite per state (icon included); PM_MESAJ_30: an empty one is greyed
                    s.Face.SkinAs($"booster-{s.B.Id()}-" + (on ? "pressed" : state == BoosterButton.Locked ? "locked" : state == BoosterButton.Out ? "zero" : "normal"));
                    s.Face.Tint(state == BoosterButton.Out && !on ? new Color(.5f, .52f, .54f) : Color.white);
                }
                else if (Art.Get("booster-button") != null)
                    // Codex Task 10: one round button for every state, told apart by tone (locked grey, out dimmed, active warm)
                    s.Face.Tint(on ? Palette.C(Palette.GoldGlow) : state == BoosterButton.Locked ? Palette.C(Palette.TealDark) * 2.2f
                                : state == BoosterButton.Out ? Palette.C(Palette.Cream, .55f) : Color.white);   // PALETTE tints
                else s.Face.SkinAs(on ? "booster-active" : state == BoosterButton.Locked ? "booster-locked"
                                   : state == BoosterButton.Out ? "booster-out" : "booster-normal");
                s.Face.Glow(on ? Draw.Rgba(240, 167, 66, .8f) : new Vector4(0, 0, 0, 0), on ? 17 : 0);
                s.Shine.Alpha(s.Face.Skinned ? 0 : state == BoosterButton.Ready || on ? 1 : .35f);

                bool locked = state == BoosterButton.Locked;
                if (Art.T14) s.Badge.gameObject.SetActive(!locked);   // the locked button art carries its own lock: no empty badge
                s.Count.Text = locked ? "" : session.Stock(s.B).ToString();
                s.Lock.gameObject.SetActive(locked);
                if (Art.On) s.Badge.Fill(locked ? Palette.TealDark : Palette.Red);   // PALETTE: coral stock badge, dark teal when locked
                else s.Badge.Gradient(locked ? Draw.Hex("#6b7479") : Draw.Hex("#63c2ef"), locked ? Draw.Hex("#3f474b") : Draw.Hex("#2f8fd6"));
                s.Badge.SkinAs(locked ? "booster-badge-lock" : "booster-badge-stock");
                s.Name.Text = locked ? string.Format(Texts.LockedLevel, bar.UnlockLevel(s.B)) : Texts.BoosterName(s.B);

                bool wasFresh = s.Fresh;
                s.Fresh = fresh;
                if (fresh && !wasFresh) Pulse(s);
                if (!fresh) { Tweens.Kill(s); }
            }
        }

        // .bst.fresh: amber glow breathing, 1.3s
        void Pulse(Slot s)
        {
            Tweens.Run(s, 1.3f, t => s.Face.Glow(Draw.Rgba(240, 167, 66, .9f * Mathf.Sin(t * Mathf.PI)), 20),
                       () => { if (s.Fresh && s.Face != null) Pulse(s); });
        }

        public Rect RectOf(Booster b) => _t54 != null ? _t54.RectOf(b) : _slots.Find(s => s.B == b).Rect;
        public void ShowScissorsInSlot(bool show) => _t54?.ShowScissorsInSlot(show);

        public Booster? Hit(Vector2 pt)
        {
            if (_t54 != null) return _t54.Hit(pt);
            foreach (var s in _slots) if (s.Rect.Contains(pt)) return s.B;
            return null;
        }
    }
}
