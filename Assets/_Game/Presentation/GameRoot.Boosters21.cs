using ReverseSolver.Core;
using UnityEngine;

namespace ReverseSolver.Presentation
{
    public sealed partial class GameRoot
    {
        bool P21Boosters => Art.P21Gameplay || Art.T54 && _testMode;
        P21BoosterView _boosterView21;
        Booster? _selected21, _effect21;
        int _region21 = -1, _piece21;
        (int x, int y, bool vertical) _joint21;
        readonly bool[] _regions21 = new bool[4];
        Vector2 _target21;
        float _effectAge21, _clockAge21 = -1;
        bool _effectApplied21;
        bool BoosterBusy21 => P21Boosters && _effect21.HasValue;
        bool BoosterSelection21 => P21Boosters && _selected21.HasValue;

        void ResetBoosters21()
        {
            _selected21 = _effect21 = null; _region21 = -1; _clockAge21 = -1;
            _boosterView21 = null;
        }

        void BuildBoosters21()
        {
            _boosterView21 = P21Boosters ? new P21BoosterView(_world, _screen, _safe.y, _board, _bar) : null;
            _boosterView21?.Selection(_selected21, _region21, _regions21);
            if (_boosterView21 == null) return;
            if (_effect21 == Booster.Hammer) _target21 = PieceCenter21(_piece21);
            if (_effect21 == Booster.Wand) _target21 = _boosterView21.ZoneRect(_region21).center;
            if (_effect21 == Booster.Scissors) _target21 = JointCenter21();
        }

        Vector2 JointCenter21() => _board.Origin + new Vector2(_joint21.x + (_joint21.vertical ? 0 : .5f), _joint21.y + (_joint21.vertical ? .5f : 0)) * _board.Cell;
        Vector2 PieceCenter21(int piece)
        {
            var center = Vector2.zero;
            foreach (var c in _session.Level.Pieces[piece]) center += new Vector2(c.X + .5f, c.Y + .5f);
            return _board.Origin + center / _session.Level.Pieces[piece].Length * _board.Cell;
        }

        void TestBooster21()
        {
            if (!P21Boosters) return;
            string flag = Host.Current.Query("booster");
            Booster? b = flag == "cekic" ? Booster.Hammer : flag == "makas" ? Booster.Scissors
                : flag == "degnek" ? Booster.Wand : flag == "saat" ? Booster.Clock : (Booster?)null;
            if (!b.HasValue) return;
            _session.TestBoosterUnlock = b;
            _session.SetStock(b.Value, System.Math.Max(1, _session.Stock(b.Value)));
            _introOpen = false; _tutorialSteps.Clear(); if (_spotlight != null) Destroy(_spotlight.gameObject); _spotlight = null;
            _hand?.Remove(); _hand = null; CloseCard(); _bar.ShowTutorial(false);
            SelectBooster21(b.Value);
        }

        void SelectBooster21(Booster b)
        {
            if (!_session.IsUnlocked(b)) { _bar.LockedNotice(b, _session); return; }
            if (b == Booster.Clock && P21ClockPaused)
            {
                _bar.Notice(Lang.T("This lesson has no timer.", "Bu derste süre sınırı yok."), 1.5f);
                return;
            }
            if (_selected21 == b)
            {
                _selected21 = null; _region21 = -1; _boosters.Disarm();
                if (_trainingUse == b) { _trainingUse = null; _session.FreeUse = null; }
            }
            else
            {
                bool training = TutorialActive && TutorialScript.BoosterOf(_tutorialSteps[_tutorialAt].booster, out var taught) && taught == b;
                if (!_session.IsUnlocked(b) || (_session.Stock(b) == 0 && _session.FreeUse != b && !training))
                { _boosters.Tap(b); return; }
                if (_selected21.HasValue && _trainingUse == _selected21)
                { _trainingUse = null; _session.FreeUse = null; }
                if (training) _session.FreeUse = b;
                if (training) { _trainingUse = b; if (_spotlight != null) Destroy(_spotlight.gameObject); _spotlight = null; }
                _drag.Cancel(); _boosters.Disarm(); _selected21 = null; _region21 = -1;
                if (b == Booster.Clock)
                {
                    if (_session.FreezeLeft > 0) { _boosterView21?.Selection(null, -1, _regions21); RefreshPlaying(); return; }
                    _clockAge21 = 0; StartEffect21(b, Vector2.zero); return;
                }
                _selected21 = b;
                if (b == Booster.Scissors || b == Booster.Hammer)
                {
                    int stock = _session.Stock(b);
                    _session.SetStock(b, System.Math.Max(1, stock)); _boosters.Tap(b); _session.SetStock(b, stock);
                }
                if (b == Booster.Wand)
                {
                    bool any = false;
                    for (int i = 0; i < 4; i++) any |= _regions21[i] = _session.CanWandRegion(i);
                    // PM_66: the wand must free a piece; when no quadrant can, say so and spend nothing.
                    if (!any)
                    {
                        _selected21 = null;
                        if (_trainingUse == b) { _trainingUse = null; _session.FreeUse = null; }
                        _bar.Notice(Texts.WandNothing, 2f);
                        _boosterView21?.Selection(null, -1, _regions21); RefreshPlaying(); return;
                    }
                }
            }
            _boosterView21?.Selection(_selected21, _region21, _regions21);
            RefreshPlaying();
        }

        bool PressBooster21(Vector2 point)
        {
            if (!P21Boosters || _session.IsOver) return false;
            if (BoosterBusy21) return true;
            var b = _bar.Hit(point);
            if (TutorialActive && !_selected21.HasValue &&
                (_tutorialSteps[_tutorialAt].tur != "booster" ||
                 !TutorialScript.BoosterOf(_tutorialSteps[_tutorialAt].booster, out var taught) || b != taught)) return false;
            if (b.HasValue) { SelectBooster21(b.Value); return true; }
            if (!_selected21.HasValue) return false;
            if (_selected21 == Booster.Wand)
            {
                if (_region21 >= 0 && _boosterView21.ConfirmRect.Contains(point))
                { StartEffect21(Booster.Wand, _boosterView21.ZoneRect(_region21).center); return true; }
                for (int i = 0; i < 4; i++)
                    if (_regions21[i] && _boosterView21.ZoneRect(i).Contains(point))
                    { _region21 = i; _boosterView21.Selection(_selected21, _region21, _regions21); break; }
            }
            else if (_selected21 == Booster.Scissors)
            {
                var j = _board.JointAt(point);
                if (j.HasValue)
                {
                    _joint21 = j.Value;
                    StartEffect21(Booster.Scissors, JointCenter21());
                }
            }
            else
            {
                int piece = _board.PieceAt(point);
                if (piece >= 0)
                {
                    _piece21 = piece; StartEffect21(Booster.Hammer, PieceCenter21(piece));
                }
            }
            return true;
        }

        void StartEffect21(Booster booster, Vector2 target)
        {
            _selected21 = null; _boosters.Disarm();
            _effect21 = booster; _effectAge21 = 0; _effectApplied21 = false; _target21 = target;
            _boosterView21?.Selection(null, -1, _regions21); RefreshPlaying();
        }

        void TickBoosters21()
        {
            if (!P21Boosters || _boosterView21 == null) return;
            bool active = !_pageHidden && _card == null && _list == null && _continuePanel == null && _celebration == null;
            if (active && _clockAge21 >= 0) _clockAge21 += Time.unscaledDeltaTime;
            if (active && _effect21.HasValue)
            {
                _effectAge21 += Time.unscaledDeltaTime;
                var b = _effect21.Value;
                float hit = b == Booster.Hammer ? .47f : b == Booster.Scissors ? P21BoosterView.ScissorsHitSeconds
                    : b == Booster.Clock ? P21BoosterView.ClockTouchSeconds : .7f;
                if (!_effectApplied21 && _effectAge21 >= hit)
                {
                    _effectApplied21 = true;
                    bool skipTarget = _trainingUse == b && TutorialActive && _tutorialSteps[_tutorialAt].tur == "booster" &&
                        _tutorialAt + 1 < _tutorialSteps.Count && _tutorialSteps[_tutorialAt + 1].tur == "booster_hedef";
                    if (skipTarget) _tutorialAt++;
                    var result = b == Booster.Hammer ? _session.UseHammer(_piece21)
                        : b == Booster.Scissors ? _session.UseScissors(_joint21.x, _joint21.y, _joint21.vertical)
                        : b == Booster.Clock ? _session.UseClock() : _session.UseWandRegion(_region21);
                    if (result == CommandResult.Ok)
                    {
                        if (b == Booster.Hammer) _board.BoosterShake();
                        if (b == Booster.Wand)
                        {
                            _board.RegionDance(_region21, _session.WandFreed);
                            if (_session.WandFreed.Count > 0) _bar.Notice(Texts.WandFreed(_session.WandFreed.Count), 1.8f);
                        }
                        if (_session.Outcome == Outcome.Win) _endAt = Mathf.Max(_endAt, Time.unscaledTime + (b == Booster.Wand ? 1.5f : 1) - _effectAge21 + .03f);
                    }
                    else
                    {
                        if (skipTarget) _tutorialAt--;
                        if (b == Booster.Clock) _clockAge21 = -1;
                        _trainingUse = null; _session.FreeUse = null;
                        _bar.Notice(result == CommandResult.NoArrangement ? Texts.WandFailed : Texts.BoosterPrompt(_boosters), 1.6f);
                    }
                }
                float duration = b == Booster.Wand ? 1.5f : b == Booster.Clock ? P21BoosterView.ClockTouchSeconds : b == Booster.Scissors ? P21BoosterView.ScissorsDuration : 1;
                if (_effectAge21 >= duration) _effect21 = null;
            }
            _boosterView21.Selection(_selected21, _region21, _regions21);
            _boosterView21.TickSelection();
            _bar.ShowScissorsInSlot(_effect21 != Booster.Scissors);
            _boosterView21.Effect(_effect21 == Booster.Clock ? null : _effect21, _effectAge21, _target21, _joint21.vertical);
            if (P21BoosterView.ShieldClock)
            {
                bool shieldAway = _boosterView21.Clock(_clockAge21, _session.FreezeLeft, active, _session.IsOver);
                _hud.ShowClockShield(shieldAway);
            }
            else _hud.ShowClockPortrait(_boosterView21.BrushClock(_clockAge21, _session.FreezeLeft, _session.IsOver));
            _hud.SetFrozen(_session.FreezeLeft > 0 && !_session.IsOver);
        }
    }
}
