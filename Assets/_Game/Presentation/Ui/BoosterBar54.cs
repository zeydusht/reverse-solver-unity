using ReverseSolver.Core;
using UnityEngine;

namespace ReverseSolver.Presentation
{
    public enum BarAction { None, Pause }

    sealed class BoosterBar54
    {
        sealed class Slot
        {
            public Rect Rect;
            public SpriteRenderer Body, Icon, Badge, Lock;
            public TextView Count;
            public BoosterButton State;
            public bool Armed, Fresh, Initialized;
            public int Stock, Language;
        }

        readonly Slot[] _slots = new Slot[4];
        readonly Rect _pause;
        readonly SpriteRenderer _pauseBody;
        readonly TextView _pauseLabel, _prompt;
        readonly float _scale;
        int _language;
        string _notice;
        float _noticeUntil, _pressedUntil;

        public BoosterBar54(Transform root, Vector2 screen, float safeBottom, int order)
        {
            _scale = Hud54.Scale(screen.x);
            float top = Hud54.BottomTop(screen, safeBottom), cy = top + 15.5f * _scale;
            Art54.Stretch(root, Art54.Bottom, new Vector2(screen.x / 2, (top + screen.y) / 2), new Vector2(screen.x, screen.y - top), order);
            _prompt = TextView.Create(root, "prompt", "", 12.5f * _scale, Palette.Cream, order: order + 8);
            _prompt.transform.localPosition = Draw.W(screen.x / 2, Hud54.ButtonsTop(screen, safeBottom) - 12 * _scale);
            for (int i = 0; i < 5; i++)
            {
                float cx = (42 + i * 76.5f) * _scale;
                var c = new Vector2(cx, cy);
                var rect = new Rect(cx - 31 * _scale, cy - 31 * _scale, 62 * _scale, 62 * _scale);
                Art54.Place(root, Art54.Socket, c, Vector2.one * (62 * _scale), order + 1);
                var body = Art54.Place(root, i == 4 ? Art54.Settings[0] : Art54.Bodies[0], c, Vector2.one * (62 * _scale), order + 2);
                var icon = Art54.Place(root, i == 4 ? Art54.Pause : Art54.Boosters[i], c,
                    Vector2.one * ((i == 4 ? 32 : 62 * 195f / 232) * _scale), order + 3);
                if (i == 4)
                {
                    _pause = rect; _pauseBody = body; _language = Lang.Revision;
                    _pauseLabel = TextView.Create(root, "pauseLabel", Lang.T("Pause", "Durdur"), 11 * _scale, Palette.Cream, order: order + 4).Heading(TextView.RimTeal);
                    _pauseLabel.transform.localPosition = Draw.W(cx, cy + 39 * _scale);
                    continue;
                }
                var badgeCenter = c + new Vector2(25, 25) * _scale;
                var slot = _slots[i] = new Slot { Rect = rect, Body = body, Icon = icon };
                slot.Badge = Art54.Place(root, Art54.Badge, badgeCenter, Vector2.one * (24 * _scale), order + 4);
                slot.Lock = Art54.Place(root, Art54.Lock, badgeCenter, Vector2.one * (15 * _scale), order + 6);
                slot.Count = TextView.Create(root, "count", "", 15 * _scale, Palette.Cream, order: order + 5).Font(Art.Lilita).Outline(Palette.Navy, .25f);
                slot.Count.transform.localPosition = Draw.W(badgeCenter);
            }
        }

        public Rect RectOf(Booster b) => _slots[(int)b].Rect;
        public Booster? Hit(Vector2 pt)
        {
            for (int i = 0; i < 4; i++) if (_slots[i].Rect.Contains(pt)) return (Booster)i;
            return null;
        }
        public BarAction HitAction(Vector2 pt)
        {
            if (!_pause.Contains(pt)) return BarAction.None;
            _pressedUntil = Time.unscaledTime + .12f;
            _pauseBody.sprite = Art.Get(Art54.Settings[1]);
            return BarAction.Pause;
        }

        public void Notice(string text, float seconds) { _notice = text; _noticeUntil = Time.unscaledTime + seconds; }

        string _lastPrompt;
        bool _lastQuiet;
        public void Refresh(BoosterControls bar, GameSession session, bool quiet)
        {
            int language = Lang.Revision;
            bool languageChanged = _language != language;
            if (languageChanged) { _language = language; _pauseLabel.Text = Lang.T("Pause", "Durdur"); }
            string prompt = quiet ? "" : !string.IsNullOrEmpty(bar.Prompt) ? bar.Prompt : Time.unscaledTime < _noticeUntil ? _notice : "";
            if (prompt != _lastPrompt || quiet != _lastQuiet || languageChanged)
            {
                _lastPrompt = prompt; _lastQuiet = quiet;
                _prompt.Text = !quiet && !string.IsNullOrEmpty(bar.Prompt) ? Texts.BoosterPrompt(bar) : prompt;
            }
            if (_pressedUntil > 0 && Time.unscaledTime >= _pressedUntil)
            {
                _pressedUntil = 0; _pauseBody.sprite = Art.Get(Art54.Settings[0]);
            }
            for (int i = 0; i < 4; i++)
            {
                var b = (Booster)i; var s = _slots[i];
                var state = session.FreeUse == b && session.IsUnlocked(b) ? BoosterButton.Ready : bar.StateOf(b);
                bool armed = bar.Armed == b, fresh = bar.IsFresh(b) && state == BoosterButton.Ready && !armed;
                int stock = session.Stock(b);
                float glow = fresh ? .9f + .1f * Mathf.Sin(Time.unscaledTime * Mathf.PI * 2 / 1.3f) : 1;
                s.Body.color = new Color(1, glow, glow, 1);
                if (s.Initialized && s.State == state && s.Armed == armed && s.Fresh == fresh && s.Stock == stock && s.Language == language) continue;
                s.Initialized = true; s.State = state; s.Armed = armed; s.Fresh = fresh; s.Stock = stock; s.Language = language;
                bool locked = state == BoosterButton.Locked;
                s.Body.sprite = Art.Get(Art54.Bodies[armed ? 1 : locked ? 2 : state == BoosterButton.Out ? 3 : 0]);
                s.Icon.color = locked || state == BoosterButton.Out ? new Color(.55f, .55f, .55f, 1) : Color.white;
                s.Badge.gameObject.SetActive(!locked);
                s.Lock.gameObject.SetActive(locked);
                s.Count.Text = locked ? "" : stock.ToString();
            }
        }
    }
}
