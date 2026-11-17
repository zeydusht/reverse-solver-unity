using ReverseSolver.Core;
using UnityEngine;

namespace ReverseSolver.Presentation
{
    /* PM_82 §3: the guided level 1 of sl100 (p20's 3x3, S41): the gloved hand (big, looping, a short gold trail the
       way it moves), Jigo talking in the cream chat box (the text types out; a tap completes it), no darkening, the
       next piece glowing gold, no clock and no loss.
         1  "Hi, I'm Jigo! …"                       a tap goes on
         2  slide the lower left piece down          (piece 6, down)
         3  try pushing the lower right one right    (piece 8, right: it runs into its neighbour on purpose)
         4  "Oops! It bumped into its neighbour …"   a short red glow where they touch
         5  push it down, where nothing blocks it    (piece 8, down)
         6  "Awesome! Now save the rest yourself!"   the hand goes; free play (6 s idle: an open piece glows)
         7  the last piece: "You saved all the Jiggles!", then the win screen
       A wrong piece shakes and Jigo says "Try this one first!"; 4 s without a move brings the hand back. The same
       step machine as the p21 guided opening (GameRoot.P21: PressP21Tutorial, RejectP21Direction, FinishP21Jam). */
    public sealed partial class GameRoot
    {
        bool _guideIntro82, _guideFree82, _guideLast82;
        float _guideIdleAt82, _guideHintAt82;
        TutorialHand _guideHand82;
        int _guideHint82 = -1;

        bool Guide82Level => _slSet && _index == 0;
        bool Guide82 => Guide82Level && !_guideFree82;

        (int Step, int Piece, Dir? Direction, bool Blocked) Guide82Slide => _p21Action switch
        {
            0 => (2, 6, Dir.D, false),
            1 => (3, 8, Dir.R, true),
            _ => (5, 8, Dir.D, false),
        };

        string Guide82Text => _guideIntro82
            ? Lang.T("Hi, I'm Jigo! The Count trapped my friends in this painting.", "Merhaba, ben Jigo! Kont arkadaşlarımı bu tabloya hapsetti.")
            : _p21Action switch
            {
                0 => Lang.T("Slide this piece down to save a Jiggle!", "Bu parçayı aşağı kaydır, bir Jiggle'ı kurtar!"),
                1 => Lang.T("Great, you saved a Jiggle! Now try pushing this one right.", "Harika, bir Jiggle kurtuldu! Şimdi şunu sağa itmeyi dene."),
                _ => Lang.T("Push it down, where nothing blocks it.", "Bunu hiçbir yere çarpmadan aşağı itmen lazım."),
            };

        void ResetGuide82()
        {
            _guideIntro82 = Guide82Level; _guideFree82 = _guideLast82 = false;
            _guideIdleAt82 = Time.unscaledTime; _guideHintAt82 = -1; _guideHint82 = -1;
            if (!Guide82Level) return;
            _session.PreventTimeout = true; _session.ClockOnFirstMove = false;   // no clock, no loss
            _session.TutorialMove = true;
        }

        /* The view for the current step (GameRoot.BuildP21Tutorial calls this on level 1 of sl100). */
        void BuildGuide82()
        {
            RemoveGuideHand82();
            if (_board == null || _session.IsOver) return;
            if (_guideIntro82)
            {
                _p21View = P21TutorialView.Story(_world, _screen, _safe.w, _board, -1, Guide82Text, true);
                return;
            }
            if (_guideFree82)
            {
                _p21View = P21TutorialView.Story(_world, _screen, _safe.w, _board, -1,
                    _guideLast82 ? Lang.T("You saved all the Jiggles!", "Bütün Jiggle'lar kurtuldu!") : Lang.T("Awesome! Now save the rest yourself!", "Süpersin! Kalan Jiggle'ları sen kurtar!"), true);
                return;
            }
            var slide = Guide82Slide;
            _p21View = P21TutorialView.Guided(_world, _screen, _safe.w, _board, slide.Piece, Guide82Text, slide.Direction);
            ShowGuideHand82();
            _hud?.PauseTutorialTime();
        }

        void ShowGuideHand82()
        {
            RemoveGuideHand82();
            if (!Guide82 || _guideIntro82 || _board == null) return;
            var slide = Guide82Slide;
            if (!slide.Direction.HasValue || !_session.Board.IsPresent(slide.Piece)) return;
            var visible = new Rect(-_board.Origin, _screen);
            _guideHand82 = TutorialHand.Show(_board, _board.PieceViewOf(slide.Piece), new Move(slide.Piece, slide.Direction.Value), 3300, visible);
            _guideHand82.transform.localScale = Vector3.one;
        }

        void RemoveGuideHand82()
        {
            if (_guideHand82 != null) _guideHand82.Remove();
            _guideHand82 = null;
        }

        /* A press on level 1: the intro goes on with a tap; while a step waits, the hand rests and comes back after 4 s. */
        bool PressGuide82(Vector2 pt)
        {
            if (!Guide82Level) return false;
            _guideIdleAt82 = Time.unscaledTime;
            if (_guideIntro82)
            {
                _guideIntro82 = false;
                _p21View?.Remove();
                BuildGuide82();
                return true;
            }
            RemoveGuideHand82();
            if (!Guide82) return false;
            int target = _board.GrabAt(pt, out _);
            if (target >= 0 && target != Guide82Slide.Piece)
            {
                _p21View?.Feedback(false, Lang.T("Try this one first!", "Önce bunu dene!"));
                _guideHintAt82 = Time.unscaledTime + 1.4f;   // the step's own line comes back
            }
            return false;   // the step machine handles the drag (PressP21Tutorial)
        }

        void TickGuide82()
        {
            if (!Guide82Level || _session == null || _session.IsOver || _board == null) return;
            if (_guideHintAt82 >= 0 && Time.unscaledTime >= _guideHintAt82 && Guide82 && !_guideIntro82)
            {
                _guideHintAt82 = -1;
                _p21View?.StoryText(Guide82Text);
            }
            bool dragging = _drag != null && _drag.Active;
            if (dragging) _guideIdleAt82 = Time.unscaledTime;
            // 4 s without a move: the hand again
            if (Guide82 && !_guideIntro82 && _guideHand82 == null && !dragging && _p21AdvanceAt < 0 && Time.unscaledTime - _guideIdleAt82 > 4)
            {
                ShowGuideHand82();
                _guideIdleAt82 = Time.unscaledTime;
            }
            if (_guideFree82) IdleHint82();
        }

        /* Free play (level 1 after the guide, level 2): 6 s without a move, an open piece glows until the next one. */
        void IdleHint82()
        {
            if (_session == null || _session.IsOver || _board == null) return;
            bool dragging = _drag != null && _drag.Active;
            if (dragging) { _guideIdleAt82 = Time.unscaledTime; ClearHint82(); return; }
            if (_guideHint82 >= 0 && !_session.Board.IsPresent(_guideHint82)) ClearHint82();
            if (_guideHint82 < 0 && Time.unscaledTime - _guideIdleAt82 > 6)
            {
                foreach (var m in _session.Level.Solution)
                    if (_session.Board.IsPresent(m.Piece) && _session.NailAt(m.Piece) <= 0 && _session.Probe(m.Piece, m.Dir).Exit)
                    { _guideHint82 = m.Piece; break; }
                if (_guideHint82 < 0)
                    for (int p = 0; p < _session.Level.Pieces.Count && _guideHint82 < 0; p++)
                        if (_session.Board.IsPresent(p) && _session.NailAt(p) <= 0)
                            foreach (var d in new[] { Dir.U, Dir.D, Dir.L, Dir.R })
                                if (_session.Probe(p, d).Exit) { _guideHint82 = p; break; }
            }
            if (_guideHint82 >= 0)
                _board.PieceViewOf(_guideHint82)?.SetGlow(Draw.Hex("#ffcf59"), .55f + .25f * Mathf.Sin(Time.unscaledTime * 3.5f));
        }

        void ClearHint82()
        {
            if (_guideHint82 >= 0 && _board != null) _board.PieceViewOf(_guideHint82)?.SetGlow(Vector4.zero, 0);
            _guideHint82 = -1;
        }

        /* Level 2 of sl100: no guide; the 6 s idle hint only. */
        bool IdleHintLevel82 => _slSet && (_index == 1 || Guide82Level && _guideFree82);

        void TickIdleHint82()
        {
            if (_slSet && _index == 1) IdleHint82();
        }

        /* A move happened (a piece left or jammed): the idle clock restarts. */
        void MovedGuide82(GameEvent e)
        {
            _guideIdleAt82 = Time.unscaledTime;
            if (e.Kind == EventKind.PieceRemoved && e.Piece == _guideHint82) _guideHint82 = -1;
            else ClearHint82();
            if (!Guide82Level || !_guideFree82 || e.Kind != EventKind.PieceRemoved) return;
            if (_session.Board.PresentCount <= 1 && !_guideLast82)
            {
                _guideLast82 = true;
                _p21View?.Remove(); _p21View = null;
                BuildGuide82();   // the chat box again, with the last line
            }
            else if (_session.Board.PresentCount > 1 && _p21View != null) { _p21View.Remove(); _p21View = null; }
        }

        /* Step 4: a short red glow where the pushed piece meets the one it ran into. */
        void JamGlow82(int piece, Dir dir)
        {
            if (_board == null) return;
            var c = _session.Level.Pieces[piece][0];
            var at = _board.Origin + new Vector2(c.X + .5f + dir.Dx() * .5f, c.Y + .5f + dir.Dy() * .5f) * _board.Cell;
            // the neighbour it hit: across the tab above (S41's piece 8 is held by piece 5's tab)
            if (dir == Dir.R || dir == Dir.L) at = _board.Origin + new Vector2(c.X + .5f, c.Y) * _board.Cell;
            var glow = ShapeView.Create(_world, "jam glow", Vector2.one * _board.Cell * .55f, 24, 3340).Radius(_board.Cell * .27f)
                .Fill(Draw.Rgba(255, 70, 60, .55f)).Glow(Draw.Rgba(255, 60, 50, 1f), 18);
            glow.transform.localPosition = Draw.W(at);
            Tweens.Run(glow.transform, .9f, t => { if (glow != null) glow.Alpha(.9f * (1 - t) * (1 - t)); });
            Destroy(glow.gameObject, 1f);
        }
    }
}
