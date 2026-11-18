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
         6  "Awesome! Now save the rest yourself!"   the hand goes; free play (PM_84: at once a piece glows with the
                                                     arrow, "You can move this one!"; then the 5 s idle hint)
         7  the last piece out (its flight over): "You saved all the Jiggles!", then the win screen
       A wrong piece shakes and Jigo says "Try this one first!"; 4 s without a move brings the hand back. The same
       step machine as the p21 guided opening (GameRoot.P21: PressP21Tutorial, RejectP21Direction, FinishP21Jam). */
    public sealed partial class GameRoot
    {
        bool _guideIntro82, _guideFree82, _guideLast82;
        float _guideIdleAt82, _guideHintAt82;
        Component _guideHand82;      // PM_85: p20's glove with TASK41's move (e, the default); PM_83: a, d; none for b and c
        GuideSpot83 _guideSpot83;    // PM_83: the dark with the lit way (steps 2–5)
        float GuideChatTop83 => Hud54.TopBottom(_screen, _safe.y);

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
            _guideIdleAt82 = Time.unscaledTime; _guideHintAt82 = -1; _guideLastAt82 = -1;
            ResetHint84();
            GuideSpot83.ReadFlag(Host.Current.Query("el"));
            if (!Guide82Level) return;
            _session.PreventTimeout = true; _session.ClockOnFirstMove = false;   // no clock, no loss
            _session.TutorialMove = true;
        }

        /* The view for the current step (GameRoot.BuildP21Tutorial calls this on level 1 of sl100). */
        void BuildGuide82()
        {
            RemoveGuideHand82();
            RemoveSpot83();
            if (_board == null || _session.IsOver && !_guideLast82) return;
            if (_guideIntro82)
            {
                _p21View = P21TutorialView.Story(_world, _screen, _safe.w, _board, -1, Guide82Text, true, GuideChatTop83);
                return;
            }
            if (_guideFree82)
            {
                _p21View = P21TutorialView.Story(_world, _screen, _safe.w, _board, -1,
                    _guideLast82 ? Lang.T("You saved all the Jiggles!", "Bütün Jiggle'lar kurtuldu!") : Lang.T("Awesome! Now save the rest yourself!", "Süpersin! Kalan Jiggle'ları sen kurtar!"), true, GuideChatTop83);
                return;
            }
            var slide = Guide82Slide;
            _p21View = P21TutorialView.Guided(_world, _screen, _safe.w, _board, slide.Piece, Guide82Text, slide.Direction, GuideChatTop83, GuideSpot83.Variant == "c");
            if (slide.Direction.HasValue && _session.Board.IsPresent(slide.Piece))
                _guideSpot83 = GuideSpot83.Show(_world, _screen, _board, new Move(slide.Piece, slide.Direction.Value));
            ShowGuideHand82(GuideArrow85.GrowSeconds);   // PM_85: the arrow opens the way, then the hand comes
            _hud?.PauseTutorialTime();
        }

        void ShowGuideHand82(float delay = 0)
        {
            RemoveGuideHand82();
            if (!Guide82 || _guideIntro82 || _board == null) return;
            var slide = Guide82Slide;
            if (!slide.Direction.HasValue || !_session.Board.IsPresent(slide.Piece)) return;
            var move = new Move(slide.Piece, slide.Direction.Value);
            switch (GuideSpot83.Variant)
            {
                case "e":   // PM_85: p20's glove with TASK41's move
                    _guideHand82 = GloveHand85.Show(_world, _board, _screen, move, 3345, delay);
                    break;
                case "a":   // TASK41's white glove
                    _guideHand82 = Task41Hand.Show(_world, _board, _screen, move, 3345);
                    break;
                case "d":   // p20's gloved hand (PM_82)
                    var visible = new Rect(-_board.Origin, _screen);
                    var hand = TutorialHand.Show(_board, _board.PieceViewOf(slide.Piece), move, 3320, visible);
                    hand.transform.localScale = Vector3.one;
                    _guideHand82 = hand;
                    break;
            }
        }

        void RemoveGuideHand82()
        {
            if (_guideHand82 is TutorialHand t) t.Remove();
            else if (_guideHand82 != null) Destroy(_guideHand82.gameObject);
            _guideHand82 = null;
        }

        /* The dark goes with the step (a new one comes with the next step); in free play it lifts. */
        void RemoveSpot83()
        {
            if (_guideSpot83 != null) _guideSpot83.FadeOut();
            _guideSpot83 = null;
        }

        /* PM_83 test: the guide's dark is on (steps 2–5) / which hand shows. */
        public bool GuideDark83 => _guideSpot83 != null && _guideSpot83.Darkened;
        public string GuideHand83 => _guideHand82 == null ? "none" : _guideHand82 is TutorialHand ? "d" : _guideHand82 is GloveHand85 ? "e" : "a";

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
            TickGuideLast82();
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
        }

        /* PM_84 §1: the hint (levels 1..ipucu_son_bolum of sl100; none after, hard and very hard included): 5 s
           without a move, the next piece that leaves (HintPicker) glows and beats, the arrow runs out of it; a touch
           takes it away, the next 5 s bring it back. The first time on a level Jigo says "You can move this one!"
           in the box (on level 1 in free play: at once, on its first free move). */
        HintCue84 _hint84;
        float _hint84At = -1, _hint84LineUntil = -1;
        bool _hint84Said;
        P21TutorialView _hint84Line;
        public int Hint84Piece => _hint84 != null ? _hint84.Piece : -1;
        static string HintLine84 => Lang.T("You can move this one!", "Bu parçayı oynatabilirsin!");

        bool HintLevel84 => _slSet && _index < Live.Config.HintLastLevel && (!Guide82Level || _guideFree82 && !_guideLast82);

        void ResetHint84()
        {
            ClearHint84();
            _hint84At = -1; _hint84Said = false;
            RemoveHintLine84();
        }

        void TickHint84()
        {
            if (_hint84LineUntil >= 0 && Time.unscaledTime >= _hint84LineUntil) RemoveHintLine84();
            if (!HintLevel84 || _session == null || _session.IsOver || _board == null || _diving) { ClearHint84(); return; }
            bool dragging = _drag != null && _drag.Active;
            if (dragging) { _guideIdleAt82 = Time.unscaledTime; ClearHint84(); return; }
            if (_hint84 != null && !_session.Board.IsPresent(_hint84.Piece)) ClearHint84();
            bool due = _hint84At >= 0 ? Time.unscaledTime >= _hint84At : Time.unscaledTime - _guideIdleAt82 > 5;
            if (_hint84 != null || !due || BoosterBusy21) return;
            _hint84At = -1;
            var move = HintPicker.Next(_session);
            if (!move.HasValue) { _guideIdleAt82 = Time.unscaledTime; return; }
            _hint84 = HintCue84.Show(_world, _board, _screen, move.Value, 3330);
            if (_hint84Said) return;
            _hint84Said = true;
            if (Guide82Level && _p21View != null) { _p21View.StoryText(HintLine84); return; }
            RemoveHintLine84();
            _hint84Line = P21TutorialView.Story(_world, _screen, _safe.w, _board, -1, HintLine84, true, GuideChatTop83);
            _hint84LineUntil = Time.unscaledTime + 2.6f;
        }

        void ClearHint84()
        {
            if (_hint84 != null) _hint84.Remove();
            _hint84 = null;
        }

        void RemoveHintLine84()
        {
            _hint84Line?.Remove(); _hint84Line = null; _hint84LineUntil = -1;
        }

        /* A touch on the board: the hint goes at once; the idle clock starts again. */
        void PressHint84()
        {
            if (_hint84 == null && _hint84Line == null) return;
            _guideIdleAt82 = Time.unscaledTime;
            ClearHint84();
            RemoveHintLine84();
        }

        /* A move happened (a piece left or jammed): the idle clock restarts; level 1 in free play: after the second
           to last piece the box goes, and once the last piece is out (its flight over) "You saved all the Jiggles!"
           comes, then the win screen (PM_84 §2). */
        void MovedGuide82(GameEvent e)
        {
            _guideIdleAt82 = Time.unscaledTime;
            ClearHint84();
            if (!Guide82Level || !_guideFree82 || e.Kind != EventKind.PieceRemoved) return;
            if (_session.Board.PresentCount == 0 && !_guideLast82)
            {
                _guideLast82 = true;
                _guideLastAt82 = -1;   // shown when the piece's flight ends (TickGuideLast82)
                _p21View?.Remove(); _p21View = null;
            }
            else if (_session.Board.PresentCount > 0 && _p21View != null) { _p21View.Remove(); _p21View = null; }
        }

        float _guideLastAt82 = -1;
        const float GuideLastHold82 = 1.5f;

        /* The win screen waits for the last line (GameRoot's end tick asks this). */
        bool GuideLastHolds82 => Guide82Level && _guideLast82 && (_guideLastAt82 < 0 || Time.unscaledTime - _guideLastAt82 < GuideLastHold82);

        void TickGuideLast82()
        {
            if (!Guide82Level || !_guideLast82 || _guideLastAt82 >= 0 || _board == null) return;
            if (Time.unscaledTime < _board.FlyingUntil || _pendingExits.Count > 0) return;
            _guideLastAt82 = Time.unscaledTime;
            BuildGuide82();   // the box again, with the last line
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
