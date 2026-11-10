using ReverseSolver.Core;
using UnityEngine;

namespace ReverseSolver.Presentation
{
    public sealed partial class GameRoot
    {
        bool _p21Flow, _resetVisit, _p21Correction, _p21JamPending;
        int _p21Step, _p21Wrong;
        float _p21StepAt, _p21AdvanceAt = -1, _p21BannerUntil;
        string _moveInput;
        P21TutorialView _p21View;
        P21Story _p21Story;
        bool _p21StoryDismissed, _p21StoryLast;
        bool P21NewArt => Art.T54 && P21Onboarding.Enabled(TelemetryMeta.Current?.Cohort, _redesignSet, PlayOverride.Active, true, "1");
        bool P21StoryLevel => P21NewArt && !_p21Flow && _index == 0;
        readonly P21LockHints _p21LockHints = new P21LockHints();
        P21TutorialView _p21LockBubble;

        void OnP21LockedAttempt(GameEvent e)
        {
            if (!Art.T54 || TelemetryMeta.Current?.Cohort != "p21" || e.Kind != EventKind.Jammed) return;
            int remaining = _session.NailAt(e.Piece);
            if (remaining <= 0 || _board == null) return;
            _board.WarnLocked(e.Piece);
            if (!_p21LockHints.TryShow(remaining)) return;
            _p21LockBubble?.Remove();
            _p21LockBubble = P21TutorialView.Show(_world, _screen, _board, -1, Texts.LockWarning(remaining), null);
            Destroy(_p21LockBubble.gameObject, 2.4f);
        }
        TelemetryQueue _tutorialQueue;
        TelemetrySender _tutorialSender;
        bool P21Prelude => _p21Flow && _index < 2;
        bool P21Guided => _p21Flow && _session?.Level.Id == P21Onboarding.TutorialId;
        int P21Target => _p21Step == 1 ? 2 : _p21Step == 2 ? 1 : _p21Step == 3 ? 0 : 3;

        void ResetP21Tutorial()
        {
            _p21Step = 1; _p21Wrong = 0; _p21Correction = _p21JamPending = false;
            _p21StepAt = Time.unscaledTime; _p21AdvanceAt = -1;
            _p21Story = P21StoryLevel ? new P21Story() : null;
            _p21StoryDismissed = _p21StoryLast = false;
            if (P21StoryLevel)
            {
                _session.PreventTimeout = true; _session.ClockOnFirstMove = false;
                _p21Story.Refresh(_session);
            }
            _p21BannerUntil = (_p21Flow || P21NewArt) && _index < 3 ? Time.unscaledTime + 1.5f : -1;
            if (P21Guided) _session.TutorialMove = true;
        }

        string P21Instruction => _p21Step == 1 ? Lang.T("Slide the piece down!", "Parçayı aşağı kaydır!")
            : _p21Step == 2 ? (_p21Correction
                ? Lang.T("Blocked! Pieces can't pass through each other. Push it out toward the open side.", "Önü kapalı! Parçalar başka parçanın içinden geçemez. Boş tarafa, dışarı doğru it.")
                : Lang.T("Now push this one left.", "Şimdi bunu sola it."))
            : _p21Step == 3 ? Lang.T("Which way gets it out? Try!", "Hangi yöne itersen çıkar? Dene!")
            : Lang.T("Clear every piece to free the picture!", "Bütün parçaları çıkarınca resim kurtulur!");

        void BuildP21Tutorial()
        {
            _p21View = null;
            if ((!_p21Flow && !P21NewArt) || _board == null) return;
            if (P21StoryLevel && !_session.IsOver)
                _p21View = P21TutorialView.Story(_world, _screen, _safe.w, _board, _p21Story.Target,
                    _p21StoryLast ? Texts.P21StoryLast : Texts.P21StoryStart, !_p21StoryDismissed);
            if (P21Guided && !_session.IsOver)
            {
                _p21View = P21TutorialView.Show(_world, _screen, _board, P21Target, P21Instruction,
                    _p21Step == 1 ? (Dir?)Dir.D : _p21Step == 2 ? (_p21Correction ? Dir.R : Dir.L) : null);
            }
            else if (_p21Flow && _index == 1 && !_session.IsOver && _session.Moves == 0)
                _p21View = P21TutorialView.Show(_world, _screen, _board, -1,
                    Lang.T("Clear the pieces with an open path first.", "Önce önü açık parçaları çıkar."), null);
            if (P21Guided)
            {
                var picture = Resources.Load<Texture2D>("Art/yeni/tutorial/Painting/jigo-meadow");
                if (picture != null) _board.SetImage(picture);
                _hud?.PauseTutorialTime();
            }
            if (Time.unscaledTime < _p21BannerUntil)
                P21TutorialView.Banner(_world, _screen, _safe.y, _p21BannerUntil - Time.unscaledTime);
        }

        void TickP21Tutorial()
        {
            if (!P21Guided || _session.IsOver || _p21AdvanceAt < 0 || Time.unscaledTime < _p21AdvanceAt) return;
            _p21AdvanceAt = -1; _p21Step++; _p21Wrong = 0; _p21StepAt = Time.unscaledTime;
            _p21View?.Remove();
            BuildP21Tutorial();
        }

        void PressP21Tutorial(Vector2 pt)
        {
            if (_session.IsOver || _p21AdvanceAt >= 0) return;
            int target = _board.GrabAt(pt, out bool outside);
            if (target != P21Target)
            {
                if (target >= 0) { _p21Wrong++; _board.TutorialShake(target, Dir.R); }
                return;
            }
            _tutorialPress = pt;
            if (_drag.Press(target, pt.x, pt.y, Time.unscaledTimeAsDouble, outside)) BeginGesture(pt);
        }

        bool RejectP21Direction(Vector2 pt)
        {
            // Only the two explicitly demonstrated gestures are constrained. Exploration uses Core unchanged.
            if (!_drag.Active || _p21Step > 2) return false;
            var expected = _p21Step == 1 ? Dir.D : _p21Correction ? Dir.R : Dir.L;
            var delta = pt - _tutorialPress;
            if (!TutorialScript.WrongDirection(expected, _drag.Direction, delta.x, delta.y, out var dir)) return false;
            int piece = _drag.Piece; _drag.Cancel(); _p21Wrong++;
            _board.TutorialShake(piece, dir);
            _p21View?.Feedback(false, P21Instruction);
            return true;
        }

        bool FinishP21Jam(bool released)
        {
            if (!_p21JamPending) return false;
            _p21JamPending = false;
            int piece = _drag.Piece;
            var direction = _drag.Direction ?? Dir.L;
            _drag.Cancel(); _board.TutorialShake(piece, direction);
            _committedTouch = !released;
            return true;
        }

        void OnP21TutorialEvent(GameEvent e)
        {
            if (P21StoryLevel && e.Kind == EventKind.PieceRemoved)
            {
                if (e.Piece == _p21Story.Target)
                {
                    _p21Step = _p21Story.Step; RecordP21Step("exit"); _p21StepAt = Time.unscaledTime;
                }
                _p21Story.Refresh(_session, e.Piece);
                _p21View?.StoryTarget(_board, _p21Story.Target);
                if (_session.Board.PresentCount == 1 && !_p21StoryLast)
                {
                    _p21StoryLast = true; _p21StoryDismissed = false;
                    _p21View?.StoryText(Texts.P21StoryLast);
                }
                if (_session.Board.PresentCount == 0) { _p21View?.Remove(); _p21View = null; }
                return;
            }
            if (!P21Guided)
            {
                if (_p21Flow && _index == 1 && e.Kind == EventKind.PieceRemoved) { _p21View?.Remove(); _p21View = null; }
                return;
            }
            if (e.Piece != P21Target) return;
            if (e.Kind == EventKind.Jammed)
            {
                _p21Wrong++;
                _p21JamPending = true;
                if (_p21Step == 2) _p21Correction = true;
                _p21View?.Remove(); BuildP21Tutorial();
                _p21View?.Feedback(false, P21Instruction);
                RecordP21Step("jam");
            }
            else if (e.Kind == EventKind.PieceRemoved)
            {
                RecordP21Step("exit");
                _p21View?.Feedback(true, Lang.T("Great!", "Harika!"));
                _p21AdvanceAt = Time.unscaledTime + .65f;
                if (_p21Step == 4) MarkIntro("rs_p21_tutorial_seen");
            }
        }

        void RecordP21Step(string result)
        {
            if (_tutorialSender == null || !_tutorialSender.Enabled) return;
            string id = System.Guid.NewGuid().ToString("N");
            var m = TelemetryMeta.Current;
            var row = new Core.Json.JsonWriter().BeginObject().Field("row_id", id).Field("attempt_row_id", _rowId)
                .Field("player", _save.Player).Field("cohort", m?.Cohort).Field("device_id", m?.DeviceId).Field("app_version", m?.AppVersion)
                .Field("level_id", _session.Level.Id).Field("level_version", _session.Level.Version).Field("attempt", _session.Attempt)
                .Field("event", "tut_step").Field("step", _p21Step).Field("result", result)
                .Key("seconds").Raw(Mathf.Max(0, Time.unscaledTime - _p21StepAt).ToString(System.Globalization.CultureInfo.InvariantCulture))
                .Field("wrong_tries", _p21Wrong).EndObject().ToString();
            _tutorialQueue.Add(id, row); _tutorialSender.Flush();
        }
    }
}
