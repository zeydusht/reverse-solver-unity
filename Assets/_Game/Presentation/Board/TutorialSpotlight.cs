using System.Collections.Generic;
using ReverseSolver.Core;
using UnityEngine;

namespace ReverseSolver.Presentation
{
    // Screen coordinates (points, y down). Above board and HUD, below instruction/cards.
    public sealed class TutorialSpotlight : MonoBehaviour
    {
        const int Order = 3300;
        readonly List<ShapeView> _shapes = new List<ShapeView>();
        readonly List<SolidView> _polygons = new List<SolidView>();
        Transform _finger;
        SpriteRenderer _glove;
        Vector2 _start, _end;
        float _phase, _success = -1;
        ShapeView _flash;

        ShapeView Box(string name, Rect r, Vector4 color, int order = Order, float radius = 0)
        {
            var s = ShapeView.Create(transform, name, r.size, 0, order).Radius(radius).Fill(color);
            s.transform.localPosition = Draw.W(r.center);
            _shapes.Add(s);
            return s;
        }

        // Horizontal bands subtract the UNION of apertures, so overlapping holes never darken one another.
        public static List<Rect> VeilRects(Vector2 screen, List<Rect> holes)
        {
            var ys = new List<float> { 0, screen.y };
            foreach (var h in holes) { ys.Add(Mathf.Clamp(h.yMin, 0, screen.y)); ys.Add(Mathf.Clamp(h.yMax, 0, screen.y)); }
            ys.Sort();
            var result = new List<Rect>();
            for (int i = 1; i < ys.Count; i++)
            {
                float top = ys[i - 1], bottom = ys[i];
                if (bottom <= top) continue;
                var spans = new List<Rect>();
                foreach (var h in holes) if (h.yMin < bottom && h.yMax > top) spans.Add(h);
                spans.Sort((x, y) => x.xMin.CompareTo(y.xMin));
                float cursor = 0;
                foreach (var h in spans)
                {
                    float left = Mathf.Clamp(h.xMin, 0, screen.x), right = Mathf.Clamp(h.xMax, 0, screen.x);
                    if (left > cursor) result.Add(new Rect(cursor, top, left - cursor, bottom - top));
                    cursor = Mathf.Max(cursor, right);
                }
                if (cursor < screen.x) result.Add(new Rect(cursor, top, screen.x - cursor, bottom - top));
            }
            return result;
        }

        static void Rounded(List<Rect> holes, Rect r, float radius)
        {
            radius = Mathf.Min(radius, Mathf.Min(r.width, r.height) / 2);
            for (float y = r.yMin; y < r.yMax; y += 3)
            {
                float end = Mathf.Min(y + 3, r.yMax), mid = (y + end) / 2;
                float dy = Mathf.Max(r.yMin + radius - mid, mid - (r.yMax - radius));
                float inset = dy > 0 ? radius - Mathf.Sqrt(Mathf.Max(0, radius * radius - dy * dy)) : 0;
                holes.Add(new Rect(r.xMin + inset, y, r.width - 2 * inset, end - y));
            }
        }

        void Veil(Vector2 screen, List<Rect> holes)
        {
            foreach (var r in VeilRects(screen, holes)) Box("VeilBand", r, Draw.Rgba(0, 0, 0, .6f));
        }
        void Veil(Vector2 screen, Rect hole, bool rounded)
        {
            var holes = new List<Rect>();
            if (rounded) Rounded(holes, hole, 9); else holes.Add(hole);
            Veil(screen, holes);
        }

        static Rect CellRect(BoardView board, Cell c, float pad = 6) => new Rect(
            board.Origin + new Vector2(c.X * board.Cell - pad, c.Y * board.Cell - pad), Vector2.one * (board.Cell + pad * 2));

        static List<Rect> ObstacleHoles(BoardView board, TutorialStep step)
        {
            var holes = new List<Rect>();
            var level = board.Session.Level;
            float s = board.Cell;
            TutorialScript.ObstacleOf(level, step.engel, out var cell, out _);
            if (step.engel == "wall")
            {
                foreach (var wall in level.Sealed)
                    foreach (int lane in wall.Value)
                    {
                        var c = wall.Key.IsVertical() ? new Cell(lane, wall.Key == Dir.U ? 0 : level.Height - 1)
                            : new Cell(wall.Key == Dir.L ? 0 : level.Width - 1, lane);
                        var center = CellRect(board, c, 0).center + new Vector2(wall.Key.Dx(), wall.Key.Dy()) * s * .6f;
                        holes.Add(new Rect(center - Vector2.one * (s / 2 + 8), Vector2.one * (s + 16)));
                    }
                return holes;
            }
            int piece = board.Session.Board.PieceAt(cell);
            if (piece < 0) return holes;
            var cells = level.Pieces[piece];
            foreach (var c in cells) Rounded(holes, CellRect(board, c), 9);
            // Badge/fuse can extend above the first cell. Circle approximation includes its counter.
            var badge = CellRect(board, cells[0], s * .18f);
            if (step.engel == "nail" || step.engel == "bomb") Rounded(holes, badge, badge.width / 2);
            if (step.engel == "chain")
            {
                // A chained pair is represented by two cells of ONE logical piece.
                var c0 = CellRect(board, cells[0], s * .16f).center;
                var c1 = CellRect(board, cells[1], s * .16f).center;
                const float width = 12;
                int count = Mathf.Max(1, Mathf.CeilToInt(Vector2.Distance(c0, c1) / (width / 2)));
                for (int k = 0; k <= count; k++)
                {
                    var center = Vector2.Lerp(c0, c1, k / (float)count);
                    Rounded(holes, new Rect(center - Vector2.one * width, Vector2.one * width * 2), width);
                }
                foreach (var m in level.Solution) if (m.Piece == piece)
                {
                    foreach (var c in cells)
                    {
                        var r = CellRect(board, c);
                        if (m.Dir == Dir.L) r.xMin = board.Origin.x - 6;
                        if (m.Dir == Dir.R) r.xMax = board.Origin.x + board.Size.x + 6;
                        if (m.Dir == Dir.U) r.yMin = board.Origin.y - 6;
                        if (m.Dir == Dir.D) r.yMax = board.Origin.y + board.Size.y + 6;
                        holes.Add(r);
                    }
                    break;
                }
            }
            return holes;
        }

        public static TutorialSpotlight ShowBooster(Transform parent, Vector2 screen, BoardView board,
            TutorialStep step, Rect button, int piece, (int x, int y, bool vertical) joint)
        {
            var v = new GameObject("BoosterTutorialSpotlight").AddComponent<TutorialSpotlight>();
            v.transform.SetParent(parent, false);
            var holes = new List<Rect>();
            if (step.tur == "booster")
            {
                Rounded(holes, button, 16);
                v._start = button.center;
            }
            else if (step.booster == "scissors")
            {
                var a = new Cell(joint.x, joint.y);
                var b = new Cell(joint.x - (joint.vertical ? 1 : 0), joint.y - (joint.vertical ? 0 : 1));
                Rounded(holes, CellRect(board, a), 9); Rounded(holes, CellRect(board, b), 9);
                v._start = board.Origin + new Vector2((joint.x + (joint.vertical ? 0 : .5f)) * board.Cell,
                    (joint.y + (joint.vertical ? .5f : 0)) * board.Cell);
            }
            else
            {
                foreach (var c in board.Session.Level.Pieces[piece]) Rounded(holes, CellRect(board, c), 9);
                v._start = CellRect(board, board.Session.Level.Pieces[piece][0]).center;
            }
            v.Veil(screen, holes);
            v._end = v._start;
            v._finger = new GameObject("TapFinger").transform;
            v._finger.SetParent(v.transform, false);
            v._glove = Art.Place(v._finger, "hand-swipe", Vector2.one * 80, Order + 6);
            if (v._glove != null) v._glove.transform.localPosition = Draw.W(0, 34);
            var label = TextView.Create(v.transform, "Instruction", Texts.TutorialCopy(step.metinAnahtari).Length > 48 ? Wrap(Texts.TutorialCopy(step.metinAnahtari)) : Texts.TutorialCopy(step.metinAnahtari), 24,
                Palette.Cream, order: 3350).Banner(TextView.OnTeal).FitWidth(screen.x - 32, 14);
            label.transform.localPosition = Draw.W(screen.x / 2, Mathf.Min(button.yMin - 38, board.Origin.y + board.Size.y + 38));
            v.Animate(0);
            return v;
        }

        public static TutorialSpotlight Show(Transform parent, Vector2 screen, BoardView board, TutorialStep step, Move move)
        {
            var v = new GameObject("TutorialSpotlight").AddComponent<TutorialSpotlight>();
            v.transform.SetParent(parent, false);
            if (step.tur == "kural") { v.Rule(screen); return v; }
            float s = board.Cell;
            Rect hole;
            if (step.tur == "surukle")
            {
                var cells = board.Session.Level.Pieces[move.Piece];
                float x0 = float.MaxValue, y0 = float.MaxValue, x1 = 0, y1 = 0;
                foreach (var c in cells) { x0 = Mathf.Min(x0, c.X * s); y0 = Mathf.Min(y0, c.Y * s); x1 = Mathf.Max(x1, (c.X + 1) * s); y1 = Mathf.Max(y1, (c.Y + 1) * s); }
                v._start = board.Origin + new Vector2((x0 + x1) / 2, (y0 + y1) / 2);
                v._end = move.Dir switch
                {
                    Dir.L => new Vector2(board.Origin.x, v._start.y),
                    Dir.R => new Vector2(board.Origin.x + board.Size.x, v._start.y),
                    Dir.U => new Vector2(v._start.x, board.Origin.y),
                    _ => new Vector2(v._start.x, board.Origin.y + board.Size.y)
                };
                // the arrow (and the lit lane) runs a cell past the board's edge, so a piece already on the edge still shows its way out
                var outward = new Vector2(move.Dir.Dx(), move.Dir.Dy()) * s * .9f;
                v._end = new Vector2(Mathf.Clamp(v._end.x + outward.x, 12, screen.x - 12), Mathf.Clamp(v._end.y + outward.y, 12, screen.y - 12));
                if (move.Dir == Dir.L) x0 = v._end.x - board.Origin.x;
                if (move.Dir == Dir.R) x1 = v._end.x - board.Origin.x;
                if (move.Dir == Dir.U) y0 = v._end.y - board.Origin.y;
                if (move.Dir == Dir.D) y1 = v._end.y - board.Origin.y;
                hole = new Rect(board.Origin + new Vector2(x0 - 3, y0 - 3), new Vector2(x1 - x0 + 6, y1 - y0 + 6));
                v.Veil(screen, hole, false);
                v.Arrow(v._start, v._end, Mathf.Min(24, s * .36f));
                v._finger = new GameObject("FingerPivot").transform;
                v._finger.SetParent(v.transform, false);
                // Pivot is the fingertip. Rotate the wrist and the artwork together.
                float angle = move.Dir == Dir.U ? 0 : move.Dir == Dir.R ? -90 : move.Dir == Dir.D ? 180 : 90;
                v._finger.localRotation = Quaternion.Euler(0, 0, angle);
                v._glove = Art.Place(v._finger, "hand-swipe", Vector2.one * s * 1.4f, Order + 6);
                if (v._glove != null) v._glove.transform.localPosition = Draw.W(0, s * .6f);
                v._flash = v.Box("Success", new Rect(v._start - Vector2.one * s * .55f, Vector2.one * s * 1.1f), Draw.Rgba(255, 241, 176, .65f), Order + 3, s * .15f);
                v._flash.Alpha(0);
                v.Animate(0);
            }
            else
            {
                v.Veil(screen, ObstacleHoles(board, step));
            }
            string text = step.tur == "surukle" ? Texts.TutorialDrag(move.Dir) : Texts.TutorialCopy(step.metinAnahtari);
            // Replace the booster row while the tutorial owns input. Wrap long obstacle copy first.
            if (text.Length > 48) text = Wrap(text);
            var label = TextView.Create(v.transform, "Instruction", text, 24, Palette.Cream, order: 3350).Banner(TextView.OnTeal).FitWidth(screen.x - 32, 14);
            label.transform.localPosition = Draw.W(screen.x / 2, Mathf.Min(screen.y - 60, board.Origin.y + board.Size.y + 65));
            return v;
        }

        static string Wrap(string text)
        {
            int mid = text.Length / 2, split = text.IndexOf(' ', mid);
            return split < 0 ? text : text.Substring(0, split) + "\n" + text.Substring(split + 1);
        }

        void Arrow(Vector2 from, Vector2 to, float width, bool rule = false)
        {
            var d = (to - from).normalized;
            var n = new Vector2(-d.y, d.x);
            float head = Mathf.Max(18, width * 1.5f);
            if (Vector2.Distance(from, to) < head) from = to - d * head;
            var baseAt = to - d * head;
            var color = rule ? Palette.Navy : Draw.Rgba(255, 233, 151, 1);
            int arrowOrder = Order + 7;
            var shaft = Box("ArrowShaft", new Rect((from + baseAt) / 2 - new Vector2(Vector2.Distance(from, baseAt), width) / 2,
                new Vector2(Vector2.Distance(from, baseAt), width)), color, arrowOrder, 2);
            shaft.transform.localRotation = Quaternion.Euler(0, 0, -Mathf.Atan2(d.y, d.x) * Mathf.Rad2Deg);
            _polygons.Add(SolidView.Polygon(transform, "ArrowHead", new[] { to, baseAt + n * width, baseAt - n * width }, color, arrowOrder));
        }

        void Rule(Vector2 screen)
        {
            Box("RuleVeil", new Rect(Vector2.zero, screen), Draw.Rgba(0, 0, 0, .35f));
            var center = screen / 2;
            float w = Mathf.Min(screen.x - 32, 370);
            Box("RulePanel", new Rect(center - new Vector2(w / 2, 125), new Vector2(w, 250)), Palette.Cream, Order + 1, 22).Skin("t20-modal-panel");
            MenuUi.Label(transform, Texts.TutorialCopy("rule"), center + new Vector2(0, -60), 20, Order + 4,
                dark: true, titleFont: true).FitWidth(w - 28, 14);
            for (int i = 0; i < 2; i++)
            {
                var p = center + new Vector2(-w * .27f, 15 + i * 58);
                Box("MiniPiece", new Rect(p - Vector2.one * 13, Vector2.one * 26), Draw.Hex("#47b9cf"), Order + 3, 5);
                Arrow(p + new Vector2(20, 0), p + new Vector2(w * .36f, 0), 7, true);
                if (i == 1) Box("Blocker", new Rect(p + new Vector2(w * .25f, -13), Vector2.one * 26), Draw.Hex("#ed7461"), Order + 4, 5);
                var mark = center + new Vector2(w * .34f, 15 + i * 58);
                if (i == 0 && MenuUi.Sprite(transform, "icon-green-check", mark, Vector2.one * 28, Order + 5) != null) continue;
                if (i == 0) MenuUi.Label(transform, "✓", mark, 32, Order + 8).Color = Draw.Hex("#3e9a51");
                else for (int k = 0; k < 2; k++)
                {
                    var stroke = Box("RedCross", new Rect(mark - new Vector2(18, 3), new Vector2(36, 6)), Draw.Hex("#d9412f"), Order + 8, 2);
                    stroke.transform.localRotation = Quaternion.Euler(0, 0, k == 0 ? 45 : -45);
                }
            }
        }

        public void Restart() { _phase = 0; Animate(0); }
        public void Success() { _success = 0; if (_finger != null) _finger.gameObject.SetActive(false); }

        void Update()
        {
            if (_success >= 0)
            {
                _success += Time.unscaledDeltaTime;
                float fade = 1 - Mathf.Clamp01(_success / .2f);
                foreach (var label in GetComponentsInChildren<TextView>()) label.Alpha = fade;
                foreach (var s in _shapes) if (s != null && s != _flash) s.Alpha(fade);
                foreach (var p in _polygons) if (p != null) p.Alpha(fade);
                if (_flash != null) { _flash.Alpha(Mathf.Sin(Mathf.Clamp01(_success / .3f) * Mathf.PI)); _flash.transform.localScale = Vector3.one * (1 + _success); }
                return;
            }
            _phase = (_phase + Time.unscaledDeltaTime) % 1.6f;
            Animate(_phase / 1.6f);
        }

        void Animate(float phase)
        {
            if (_finger == null) return;
            float drag = Mathf.SmoothStep(0, 1, Mathf.Clamp01((phase - .2f) / .6f));
            _finger.localPosition = Draw.W(Vector2.Lerp(_start, _end, drag) + new Vector2(0, -10 * (1 - Mathf.Clamp01(phase / .15f))));
            _finger.localScale = Vector3.one * (phase < .2f ? Mathf.Lerp(1, .9f, phase / .2f) : .9f);
            if (_glove != null) _glove.color = new Color(1, 1, 1, 1 - Mathf.Clamp01((phase - .8f) / .2f));
        }
    }
}
