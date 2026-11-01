using System.Collections.Generic;
using ReverseSolver.Core;
using UnityEngine;

namespace ReverseSolver.Presentation
{
    // Screen coordinates (points, y down). Above every board badge, below HUD/cards.
    public sealed class TutorialSpotlight : MonoBehaviour
    {
        const int Order = 2400;
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

        void Veil(Vector2 screen, Rect hole, bool rounded)
        {
            hole = Rect.MinMaxRect(Mathf.Clamp(hole.xMin, 0, screen.x), Mathf.Clamp(hole.yMin, 0, screen.y),
                Mathf.Clamp(hole.xMax, 0, screen.x), Mathf.Clamp(hole.yMax, 0, screen.y));
            var black = Draw.Rgba(0, 0, 0, .6f);
            Box("Top", new Rect(0, 0, screen.x, hole.yMin), black);
            Box("Bottom", new Rect(0, hole.yMax, screen.x, screen.y - hole.yMax), black);
            Box("Left", new Rect(0, hole.yMin, hole.xMin, hole.height), black);
            Box("Right", new Rect(hole.xMax, hole.yMin, screen.x - hole.xMax, hole.height), black);
            if (!rounded) return;
            float r = 9;
            // Four small corner wedges round the aperture without dimming its interior.
            for (int k = 0; k < 4; k++)
            {
                float sx = k % 2 == 0 ? 1 : -1, sy = k < 2 ? 1 : -1;
                var corner = new Vector2(sx > 0 ? hole.xMin : hole.xMax, sy > 0 ? hole.yMin : hole.yMax);
                var c = corner + new Vector2(sx * r, sy * r);
                var pts = new List<Vector2> { corner };
                for (int j = 0; j <= 12; j++)
                {
                    float a = j * Mathf.PI / 24;
                    pts.Add(c - new Vector2(sx * r * Mathf.Cos(a), sy * r * Mathf.Sin(a)));
                }
                _polygons.Add(SolidView.Polygon(transform, "RoundedCorner", pts, black, Order));
            }
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
                v.Arrow(v._start, v._end, Mathf.Min(16, s * .24f));
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
                TutorialScript.ObstacleOf(board.Session.Level, step.engel, out var c, out var side);
                var center = board.Origin + new Vector2((c.X + .5f) * s, (c.Y + .5f) * s);
                // Include the wall strip outside the boundary, rather than illuminating only a piece.
                if (side.HasValue) center += new Vector2(side.Value.Dx(), side.Value.Dy()) * s * .6f;
                hole = new Rect(center - Vector2.one * (s / 2 + 6), Vector2.one * (s + 12));
                v.Veil(screen, hole, true);
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

        void Arrow(Vector2 from, Vector2 to, float width)
        {
            var d = (to - from).normalized;
            var n = new Vector2(-d.y, d.x);
            float head = Mathf.Min(width * 1.5f, Vector2.Distance(from, to) * .5f);
            var baseAt = to - d * head;
            var color = Draw.Rgba(255, 233, 151, .55f);
            var shaft = Box("ArrowShaft", new Rect((from + baseAt) / 2 - new Vector2(Vector2.Distance(from, baseAt), width) / 2,
                new Vector2(Vector2.Distance(from, baseAt), width)), color, Order + 2, 2);
            shaft.transform.localRotation = Quaternion.Euler(0, 0, -Mathf.Atan2(d.y, d.x) * Mathf.Rad2Deg);
            _polygons.Add(SolidView.Polygon(transform, "ArrowHead", new[] { to, baseAt + n * width, baseAt - n * width }, color, Order + 2));
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
                Arrow(p + new Vector2(20, 0), p + new Vector2(w * .36f, 0), 7);
                if (i == 1) Box("Blocker", new Rect(p + new Vector2(w * .25f, -13), Vector2.one * 26), Draw.Hex("#ed7461"), Order + 4, 5);
                var mark = center + new Vector2(w * .34f, 15 + i * 58);
                if (i == 0 && MenuUi.Sprite(transform, "icon-green-check", mark, Vector2.one * 28, Order + 5) != null) continue;
                MenuUi.Label(transform, i == 0 ? "✓" : "×", mark, 28, Order + 5).Color = Draw.Hex(i == 0 ? "#3e9a51" : "#d9412f");
            }
        }

        public void Restart() { _phase = 0; Animate(0); }
        public void Success() { _success = 0; if (_finger != null) _finger.gameObject.SetActive(false); }

        void Update()
        {
            if (_success >= 0)
            {
                _success += Time.unscaledDeltaTime;
                float fade = 1 - Mathf.Clamp01((_success - .1f) / .2f);
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
