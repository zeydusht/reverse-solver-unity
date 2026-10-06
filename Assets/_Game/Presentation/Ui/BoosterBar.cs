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
        static readonly string[] Names = { "MAKAS", "DEĞNEK", "ÇEKİÇ", "SAAT" };   // fixed: no culture-dependent ToUpper

        readonly Transform _root;
        readonly TextView _prompt;
        readonly List<Slot> _slots = new List<Slot>();

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

        public BoosterBar(Transform parent, Vector2 screen, float bottomInset, int order)
        {
            _root = new GameObject("BoosterBar").transform;
            _root.SetParent(parent, false);
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
                s.Face = ShapeView.Create(_root, "bst", new Vector2(60, 60), 20, order).Radius(16)
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
                Icons.Draw(b.Id(), s.Glyph, 33, order + 2);
                // .badge: 24px circle at top -8 right -8, gradient #63c2ef -> #2f8fd6, 3px white ring
                var bc = new Vector2(cx + 30 - 4, cy - 30 + 4);
                s.Badge = ShapeView.Create(_root, "badge", new Vector2(24, 24), 2, order + 3).Radius(12)
                    .Gradient(Draw.Hex("#63c2ef"), Draw.Hex("#2f8fd6")).Stroke(Draw.Rgba(255, 255, 255, .92f), 3);
                s.Badge.transform.localPosition = Draw.W(bc);
                s.Count = TextView.Create(_root, "count", "", 12.5f, Draw.Hex("#ffffff"), order: order + 4);
                s.Count.transform.localPosition = Draw.W(bc);
                s.Lock = new GameObject("lock").transform;
                s.Lock.SetParent(_root, false);
                s.Lock.localPosition = Draw.W(bc);
                Icons.Draw("lock", s.Lock, 13, order + 4);
                // .bst-name: 9px, letter-spacing .16em, uppercase, muted
                s.Name = TextView.Create(_root, "name", Names[(int)b], 9, Draw.Hex("#8fb2b3"), order: order);
                s.Name.GetComponent<TextMeshPro>().characterSpacing = 16;
                s.Name.transform.localPosition = Draw.W(cx, cy + 30 + 5 + 6);
                _slots.Add(s);
            }
        }

        public void Refresh(BoosterControls bar, GameSession session)
        {
            _prompt.Text = bar.Prompt;
            foreach (var s in _slots)
            {
                var state = bar.StateOf(s.B);
                bool on = bar.Armed == s.B;
                bool fresh = bar.IsFresh(s.B) && state == BoosterButton.Ready && !on;
                string key = $"{state}|{on}|{fresh}|{session.Stock(s.B)}";
                if (key == s.Key) continue;
                s.Key = key;

                Vector4 top, bottom;
                if (on) { top = Draw.Hex("#ffc866"); bottom = Draw.Hex("#f0a742"); }
                else if (state == BoosterButton.Locked) { top = Draw.Hex("#5a6266"); bottom = Draw.Hex("#33393c"); }   // grayscale(1) brightness(.5)
                else if (state == BoosterButton.Out) { top = Draw.Hex("#8f9a9f"); bottom = Draw.Hex("#4f5a60"); }      // grayscale(.85) brightness(.72)
                else { top = Draw.Hex("#6ecbf5"); bottom = Draw.Hex("#2b86cf"); }
                s.Face.Gradient(top, bottom);
                s.Face.Glow(on ? Draw.Rgba(240, 167, 66, .8f) : new Vector4(0, 0, 0, 0), on ? 17 : 0);
                s.Shine.Alpha(state == BoosterButton.Ready || on ? 1 : .35f);

                bool locked = state == BoosterButton.Locked;
                s.Count.Text = locked ? "" : session.Stock(s.B).ToString();
                s.Lock.gameObject.SetActive(locked);
                s.Badge.Gradient(locked ? Draw.Hex("#6b7479") : Draw.Hex("#63c2ef"), locked ? Draw.Hex("#3f474b") : Draw.Hex("#2f8fd6"));
                s.Name.Text = locked ? $"{bar.UnlockLevel(s.B)}. BÖLÜM" : Names[(int)s.B];

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

        public Booster? Hit(Vector2 pt)
        {
            foreach (var s in _slots) if (s.Rect.Contains(pt)) return s.B;
            return null;
        }
    }
}
