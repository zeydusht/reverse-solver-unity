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
        TextView _instruction;
        float _instructionWidth;
        bool _stuck;

        ShapeView Box(string name, Rect r, Vector4 color, int order = Order, float radius = 0)
        {
            var s = ShapeView.Create(transform, name, r.size, 0, order).Radius(radius).Fill(color);
            s.transform.localPosition = Draw.W(r.center);
            _shapes.Add(s);
            return s;
        }

        // Layout points, y down; Angle is in radians in that same coordinate system.
        public readonly struct Hole
        {
            public readonly Vector2 Center, Size;
            public readonly float Radius, Angle;
            public Hole(Rect rect, float radius = 0, float angle = 0)
            { Center = rect.center; Size = rect.size; Radius = radius; Angle = angle; }
        }

        const int MaxHoles = 16;
        Material _veilMaterial;
        Mesh _veilMesh;
        float _veilAlpha;

        void Veil(Vector2 screen, List<Hole> holes, float alpha = .6f)
        {
            if (holes.Count > MaxHoles) throw new System.ArgumentException("Tutorial veil supports at most 16 holes.");
            var shader = Resources.Load<Shader>("Shaders/Veil");
            if (shader == null) throw new System.InvalidOperationException("Missing Shaders/Veil resource.");
            _veilMaterial = new Material(shader) { name = "TutorialVeil" };
            _veilAlpha = alpha;
            var shapes = new Vector4[MaxHoles];
            var parameters = new Vector4[MaxHoles];
            for (int i = 0; i < holes.Count; i++)
            {
                var h = holes[i];
                // Both fragment position and holes use world coordinates, including parent transforms.
                var center = transform.TransformPoint(Draw.W(h.Center));
                var axis = transform.TransformVector(Draw.W(Mathf.Cos(h.Angle), Mathf.Sin(h.Angle)));
                shapes[i] = new Vector4(center.x, center.y, h.Size.x / 2, h.Size.y / 2);
                parameters[i] = new Vector4(h.Radius, axis.x, axis.y, 0);
            }
            _veilMaterial.SetVectorArray("_Shape", shapes);
            _veilMaterial.SetVectorArray("_ShapeP", parameters);
            _veilMaterial.SetInt("_Count", holes.Count);
            _veilMaterial.SetVector("_Color", new Vector4(0, 0, 0, alpha));
            _veilMaterial.SetFloat("_Feather", 7);
            var quad = new GameObject("Veil", typeof(MeshFilter), typeof(MeshRenderer));
            quad.transform.SetParent(transform, false);
            quad.transform.localPosition = Draw.W(screen / 2);
            _veilMesh = Draw.CenteredQuad(screen, 1); // Overdraw screen edges to avoid raster seams.
            quad.GetComponent<MeshFilter>().sharedMesh = _veilMesh;
            var renderer = quad.GetComponent<MeshRenderer>();
            renderer.sharedMaterial = _veilMaterial;
            renderer.sortingOrder = Order;
        }

        void OnDestroy()
        {
            if (_veilMaterial != null) Destroy(_veilMaterial);
            if (_veilMesh != null) Destroy(_veilMesh);
        }

        static Rect CellRect(BoardView board, Cell c) => CellRect(board.Origin, board.Cell, c);
        static Rect CellRect(Vector2 origin, float size, Cell c) => new Rect(
            origin + new Vector2(c.X * size, c.Y * size), Vector2.one * size);
        static Hole Circle(Vector2 center, float diameter) => new Hole(
            new Rect(center - Vector2.one * diameter / 2, Vector2.one * diameter), diameter / 2);
        static void PieceHoles(List<Hole> holes, Vector2 origin, float size, Cell[] cells)
        {
            foreach (var c in cells) holes.Add(new Hole(CellRect(origin, size, c), size * .18f));
        }

        public static List<Hole> DragHoles(Vector2 origin, float size, Cell[] cells, Dir direction, Vector2 end)
        {
            var holes = new List<Hole>();
            PieceHoles(holes, origin, size, cells);
            var bounds = CellRect(origin, size, cells[0]);
            foreach (var c in cells)
            {
                var r = CellRect(origin, size, c);
                bounds = Rect.MinMaxRect(Mathf.Min(bounds.xMin, r.xMin), Mathf.Min(bounds.yMin, r.yMin),
                    Mathf.Max(bounds.xMax, r.xMax), Mathf.Max(bounds.yMax, r.yMax));
            }
            // Begin at the outgoing edge: the piece itself remains cell-shaped.
            var path = direction switch
            {
                Dir.L => Rect.MinMaxRect(end.x, bounds.yMin, bounds.xMin, bounds.yMax),
                Dir.R => Rect.MinMaxRect(bounds.xMax, bounds.yMin, end.x, bounds.yMax),
                Dir.U => Rect.MinMaxRect(bounds.xMin, end.y, bounds.xMax, bounds.yMin),
                _ => Rect.MinMaxRect(bounds.xMin, bounds.yMax, bounds.xMax, end.y)
            };
            holes.Add(new Hole(path));
            return holes;
        }

        static float Cross(Vector2 a, Vector2 b) => a.x * b.y - a.y * b.x;
        static bool Intersects(Cell[] a, Cell[] b)
        {
            Vector2 p = new Vector2(a[0].X, a[0].Y), q = new Vector2(b[0].X, b[0].Y);
            var r = new Vector2(a[1].X, a[1].Y) - p;
            var t = new Vector2(b[1].X, b[1].Y) - q;
            float denominator = Cross(r, t);
            if (Mathf.Abs(denominator) < .0001f)
            {
                if (Mathf.Abs(Cross(q - p, r)) > .0001f) return false;
                float rr = Vector2.Dot(r, r);
                float u = Vector2.Dot(q - p, r) / rr, v = u + Vector2.Dot(t, r) / rr;
                return Mathf.Max(Mathf.Min(u, v), 0) <= Mathf.Min(Mathf.Max(u, v), 1);
            }
            float alongA = Cross(q - p, t) / denominator, alongB = Cross(q - p, r) / denominator;
            return alongA >= 0 && alongA <= 1 && alongB >= 0 && alongB <= 1;
        }

        public static int ChainPiece(LevelData level)
        {
            int best = -1, least = int.MaxValue;
            for (int i = 0; i < level.Pieces.Count; i++)
            {
                if (level.Pieces[i].Length < 2) continue;
                int crossings = 0;
                for (int j = 0; j < level.Pieces.Count; j++)
                    if (i != j && level.Pieces[j].Length > 1 && Intersects(level.Pieces[i], level.Pieces[j])) crossings++;
                if (crossings < least) { best = i; least = crossings; }
            }
            return best;
        }

        public static List<Hole> ChainHoles(Vector2 origin, float size, LevelData level)
        {
            var holes = new List<Hole>();
            int piece = ChainPiece(level);
            if (piece < 0) return holes;
            var cells = level.Pieces[piece];
            PieceHoles(holes, origin, size, cells);
            var a = CellRect(origin, size, cells[0]).center;
            var b = CellRect(origin, size, cells[1]).center;
            var delta = b - a;
            var strip = new Vector2(delta.magnitude, 14);
            holes.Add(new Hole(new Rect((a + b) / 2 - strip / 2, strip), 0, Mathf.Atan2(delta.y, delta.x)));
            return holes;
        }

        public static List<Hole> ObstacleHoles(BoardView board, TutorialStep step)
        {
            var holes = new List<Hole>();
            var level = board.Session.Level;
            float s = board.Cell;
            if (step.engel == "chain") return ChainHoles(board.Origin, s, level);
            TutorialScript.ObstacleOf(level, step.engel, out var cell, out _);
            if (step.engel == "wall")
            {
                // Match BoardView's contiguous wall strips, including corner extensions.
                bool art = Art.On;
                float rim = Art.Get("board-frame") != null ? (Art.T14 ? 26 : 20) * .7f : 0;
                float thickness = art ? Mathf.Max(s * .25f, rim, 8) : Mathf.Max(7, s * .16f);
                float gap = art ? 0 : thickness * .5f, overlap = art ? 0 : s * .06f;
                bool Sealed(Dir d, int lane) => level.Sealed.TryGetValue(d, out var lanes) && System.Array.IndexOf(lanes, lane) >= 0;
                foreach (var side in level.Sealed)
                {
                    var lanes = new List<int>(side.Value); lanes.Sort();
                    for (int k = 0; k < lanes.Count; k++)
                    {
                        int a = lanes[k], b = a;
                        if (art) while (k + 1 < lanes.Count && lanes[k + 1] == b + 1) b = lanes[++k];
                        float from = a * s - overlap, to = (b + 1) * s + overlap;
                        if (art && side.Key.IsVertical())
                        {
                            int row = side.Key == Dir.U ? 0 : level.Height - 1;
                            if (a == 0 && Sealed(Dir.L, row)) from = -thickness - gap;
                            if (b == level.Width - 1 && Sealed(Dir.R, row)) to = board.Size.x + thickness + gap;
                        }
                        var r = side.Key switch
                        {
                            Dir.U => new Rect(from, -thickness - gap, to - from, thickness),
                            Dir.D => new Rect(from, board.Size.y + gap, to - from, thickness),
                            Dir.L => new Rect(-thickness - gap, from, thickness, to - from),
                            _ => new Rect(board.Size.x + gap, from, thickness, to - from)
                        };
                        r.position += board.Origin;
                        holes.Add(new Hole(r, 4));
                    }
                }
                return holes;
            }
            int piece = board.Session.Board.PieceAt(cell);
            if (piece < 0) return holes;
            var cells = level.Pieces[piece];
            PieceHoles(holes, board.Origin, s, cells);
            var center = CellRect(board, cells[0]).center;
            if (step.engel == "nail")
            {
                float diameter = Art.Get("board-nail") != null ? s * (Art.T14 ? .86f : .8f)
                    : Art.Get("icon-nail") != null ? s * .8f : Mathf.Max(22, s * .46f);
                holes.Add(Circle(center, diameter + 4));
            }
            if (step.engel == "bomb")
            {
                bool modern = Art.Get("bomb-v2") != null || Art.Get("board-bomb") != null;
                float diameter = modern ? s * (Art.T14 ? 1 : 1.06f)
                    : Art.Get("icon-bomb") != null ? s * .8f : Mathf.Max(24, s * .52f) * 1.5f;
                if (modern && Art.Get("bomb-v2") == null) center.y -= diameter * .1f;
                holes.Add(Circle(center, diameter));
            }
            return holes;
        }

        public static TutorialSpotlight ShowBooster(Transform parent, Vector2 screen, BoardView board,
            TutorialStep step, Rect button, int piece, (int x, int y, bool vertical) joint)
        {
            var v = new GameObject("BoosterTutorialSpotlight").AddComponent<TutorialSpotlight>();
            v.transform.SetParent(parent, false);
            var holes = new List<Hole>();
            if (step.tur == "booster")
            {
                holes.Add(Circle(button.center, Mathf.Max(button.width, button.height) + 8));
                v._start = button.center;
            }
            else if (step.booster == "scissors")
            {
                var a = new Cell(joint.x, joint.y);
                var b = new Cell(joint.x - (joint.vertical ? 1 : 0), joint.y - (joint.vertical ? 0 : 1));
                holes.Add(new Hole(CellRect(board, a), board.Cell * .18f)); holes.Add(new Hole(CellRect(board, b), board.Cell * .18f));
                v._start = board.Origin + new Vector2((joint.x + (joint.vertical ? 0 : .5f)) * board.Cell,
                    (joint.y + (joint.vertical ? .5f : 0)) * board.Cell);
            }
            else
            {
                PieceHoles(holes, board.Origin, board.Cell, board.Session.Level.Pieces[piece]);
                v._start = CellRect(board, board.Session.Level.Pieces[piece][0]).center;
            }
            v.Veil(screen, holes);
            v._end = v._start;
            v.Finger(80);
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
            if (step.tur == "surukle" || step.tur == "hata")
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
                v.Veil(screen, DragHoles(board.Origin, s, cells, move.Dir, v._end));
                v.Arrow(v._start, v._end, Mathf.Min(24, s * .36f));
                v.Finger(s * 1.4f);
                v._flash = v.Box("Success", new Rect(v._start - Vector2.one * s * .55f, Vector2.one * s * 1.1f), Draw.Rgba(255, 241, 176, .65f), Order + 3, s * .15f);
                v._flash.Alpha(0);
                v.Animate(0);
            }
            else
            {
                v.Veil(screen, ObstacleHoles(board, step));
            }
            string text = step.tur == "hata" ? Texts.TutorialErrorDrag(move.Dir)
                : step.tur == "surukle" ? Texts.TutorialDrag(move.Dir) : Texts.TutorialCopy(step.metinAnahtari);
            // Replace the booster row while the tutorial owns input. Wrap long obstacle copy first.
            if (text.Length > 48) text = Wrap(text);
            var label = TextView.Create(v.transform, "Instruction", text, 24, Palette.Cream, order: 3350).Banner(TextView.OnTeal).FitWidth(screen.x - 32, 14);
            label.transform.localPosition = Draw.W(screen.x / 2, Mathf.Min(screen.y - 60, board.Origin.y + board.Size.y + 65));
            v._instruction = label;
            v._instructionWidth = screen.x - 32;
            return v;
        }

        void Finger(float size)
        {
            _finger = new GameObject("FingerPivot").transform;
            _finger.SetParent(transform, false);
            _glove = Art.Place(_finger, "hand-swipe", Vector2.one * size, Order + 6);
            // The upright sprite's fingertip is at (150, 15) in its 288px canvas.
            if (_glove != null) _glove.transform.localPosition = Draw.W(-size * 6 / 288, size * 129 / 288);
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
            Veil(screen, new List<Hole>(), .35f);
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

        public void Stuck()
        {
            _stuck = true;
            if (_finger != null) _finger.gameObject.SetActive(false);
            foreach (var s in _shapes) if (s != null) s.Alpha(0);
            foreach (var p in _polygons) if (p != null) p.Alpha(0);
            if (_instruction != null)
            {
                _instruction.Text = Wrap(Texts.TutorialCopy("stuck"));
                _instruction.FitWidth(_instructionWidth, 14);
            }
        }

        public void Restart() { _phase = 0; Animate(0); }
        public void Success() { _success = 0; if (_finger != null) _finger.gameObject.SetActive(false); }

        void Update()
        {
            if (_stuck) return;
            if (_success >= 0)
            {
                _success += Time.unscaledDeltaTime;
                float fade = 1 - Mathf.Clamp01(_success / .2f);
                if (_veilMaterial != null) _veilMaterial.SetVector("_Color", new Vector4(0, 0, 0, _veilAlpha * fade));
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
            float lift = Mathf.Clamp01((phase - .8f) / .2f);
            _finger.localPosition = Draw.W(Vector2.Lerp(_start, _end, drag) + new Vector2(0, -10 * (1 - Mathf.Clamp01(phase / .15f)) - 10 * lift));
            _finger.localScale = Vector3.one * (phase < .2f ? Mathf.Lerp(1, .9f, phase / .2f) : Mathf.Lerp(.9f, 1, lift));
            if (_glove != null) _glove.color = new Color(1, 1, 1, 1 - Mathf.Clamp01((phase - .8f) / .2f));
        }
    }
}
