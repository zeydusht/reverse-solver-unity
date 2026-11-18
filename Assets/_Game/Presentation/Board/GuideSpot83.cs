using System.Collections.Generic;
using ReverseSolver.Core;
using UnityEngine;

namespace ReverseSolver.Presentation
{
    /* PM_83 §1: the guided steps of level 1 (2–5): the screen ~60 % dark with only the piece to play and its way out
       lit (one veil mask, the p20 spotlight's Shaders/Veil), a dashed arrow sliding along that way, and one of the
       tutorial hands from the game's history (&el=):
         a  Codex TASK41's white glove (10 Nov, Zeyd approved; PM_59): drag frames, fingertip on the piece, slides out
         b  PM_60's guide (11 Nov): no hand, the piece glows
         c  codex/01-onboarding (12 Nov): gold dots and a small arrow at the piece
         d  p20's gloved hand (PM_36, 3 Nov; PM_82's choice): the back of the glove nudging the way
       The default is a. Only graphics: input goes through DragModel and Core as before. */
    public sealed class GuideSpot83 : MonoBehaviour
    {
        const int Order = 3300, MaxHoles = 16;
        public static string Variant = "a";
        public static bool HandIsTask41 => Variant == "a";

        Material _mat;
        Mesh _mesh;
        float _alpha = .6f, _born, _fadeOut = -1;
        SpriteRenderer _arrow;
        Vector2 _start, _end, _dir;
        float _len, _arrowBase;
        bool _trail = true;

        public static void ReadFlag(string value)
        {
            Variant = value == "b" || value == "c" || value == "d" ? value : "a";
        }

        /* The lit way of a move: from the piece to a cell past the board's edge (as the p20 spotlight). */
        static void Way(BoardView board, Vector2 screen, Move move, out Cell[] cells, out Vector2 start, out Vector2 end)
        {
            float s = board.Cell;
            cells = board.Session.Level.Pieces[move.Piece];
            float x0 = float.MaxValue, y0 = float.MaxValue, x1 = 0, y1 = 0;
            foreach (var c in cells) { x0 = Mathf.Min(x0, c.X * s); y0 = Mathf.Min(y0, c.Y * s); x1 = Mathf.Max(x1, (c.X + 1) * s); y1 = Mathf.Max(y1, (c.Y + 1) * s); }
            start = board.Origin + new Vector2((x0 + x1) / 2, (y0 + y1) / 2);
            end = move.Dir switch
            {
                Dir.L => new Vector2(board.Origin.x, start.y),
                Dir.R => new Vector2(board.Origin.x + board.Size.x, start.y),
                Dir.U => new Vector2(start.x, board.Origin.y),
                _ => new Vector2(start.x, board.Origin.y + board.Size.y)
            };
            var outward = new Vector2(move.Dir.Dx(), move.Dir.Dy()) * s * .9f;
            end = new Vector2(Mathf.Clamp(end.x + outward.x, 12, screen.x - 12), Mathf.Clamp(end.y + outward.y, 12, screen.y - 12));
        }

        public static GuideSpot83 Show(Transform parent, Vector2 screen, BoardView board, Move move)
        {
            var v = new GameObject("PM83 guide spotlight").AddComponent<GuideSpot83>();
            v.transform.SetParent(parent, false);
            v._born = Time.unscaledTime;
            Way(board, screen, move, out var cells, out v._start, out v._end);
            v._dir = new Vector2(move.Dir.Dx(), move.Dir.Dy());
            v._len = Vector2.Distance(v._start, v._end);
            v.Veil(screen, TutorialSpotlight.DragHoles(board.Origin, board.Cell, cells, move.Dir, v._end));
            if (Variant != "c")
            {
                // the dashed arrow (TASK41's path art) sliding the way out
                var sprite = Resources.Load<Sprite>("Art/yeni/tutorial/Path/straight-down");
                if (sprite != null)
                {
                    v._arrow = Draw.Child<SpriteRenderer>(v.transform, "sliding arrow");
                    v._arrow.sprite = sprite; v._arrow.sortingOrder = Order + 2;
                    float side = Mathf.Max(board.Cell * 1.1f, v._len / .72f);
                    v._arrow.transform.localScale = Vector3.one * side / sprite.bounds.size.y;
                    float rot = move.Dir == Dir.D ? 0 : move.Dir == Dir.U ? 180 : move.Dir == Dir.R ? 90 : -90;
                    v._arrow.transform.localRotation = Quaternion.Euler(0, 0, rot);
                    v._arrowBase = side;
                }
            }
            v.Update();
            return v;
        }

        void Veil(Vector2 screen, List<TutorialSpotlight.Hole> holes)
        {
            var shader = Resources.Load<Shader>("Shaders/Veil");
            if (shader == null || holes.Count > MaxHoles) return;
            _mat = new Material(shader) { name = "PM83 guide veil" };
            var shapes = new Vector4[MaxHoles];
            var parameters = new Vector4[MaxHoles];
            for (int i = 0; i < holes.Count; i++)
            {
                var h = holes[i];
                var center = transform.TransformPoint(Draw.W(h.Center));
                var axis = transform.TransformVector(Draw.W(Mathf.Cos(h.Angle), Mathf.Sin(h.Angle)));
                shapes[i] = new Vector4(center.x, center.y, h.Size.x / 2, h.Size.y / 2);
                parameters[i] = new Vector4(h.Radius, axis.x, axis.y, 0);
            }
            _mat.SetVectorArray("_Shape", shapes);
            _mat.SetVectorArray("_ShapeP", parameters);
            _mat.SetInt("_Count", holes.Count);
            _mat.SetVector("_Color", new Vector4(0, 0, 0, 0));
            _mat.SetFloat("_Feather", 7);
            var quad = new GameObject("Veil", typeof(MeshFilter), typeof(MeshRenderer));
            quad.transform.SetParent(transform, false);
            quad.transform.localPosition = Draw.W(screen / 2);
            _mesh = Draw.CenteredQuad(screen, 1);
            quad.GetComponent<MeshFilter>().sharedMesh = _mesh;
            var r = quad.GetComponent<MeshRenderer>();
            r.sharedMaterial = _mat; r.sortingOrder = Order;
        }

        public bool Darkened => _mat != null;

        /* The jam of step 4 (the arrow pointed at a blocked way): the arrow goes, the dark stays. */
        public void HideTrail() { _trail = false; if (_arrow != null) _arrow.enabled = false; }

        /* Free play: the dark lifts in 0.25 s, then the object goes. */
        public void FadeOut() { if (_fadeOut < 0) _fadeOut = Time.unscaledTime; }

        void Update()
        {
            float now = Time.unscaledTime;
            float a = _alpha * Mathf.Clamp01((now - _born) / .2f);
            if (_fadeOut >= 0)
            {
                float k = Mathf.Clamp01((now - _fadeOut) / .25f);
                a *= 1 - k;
                if (k >= 1) { Destroy(gameObject); return; }
            }
            if (_mat != null) _mat.SetVector("_Color", new Vector4(0, 0, 0, a));
            if (_arrow != null && _trail)
            {
                // 1.2 s: the arrow slides a third of the way along, fading in and out
                float p = Mathf.Repeat(now - _born, 1.2f) / 1.2f;
                var mid = (_start + _end) / 2 + _dir * _len * .1f;
                _arrow.transform.localPosition = Draw.W(mid + _dir * (p - .5f) * _len * .45f);
                _arrow.color = new Color(1, 1, 1, Mathf.Sin(p * Mathf.PI) * (_fadeOut >= 0 ? 0 : 1));
            }
        }

        void OnDestroy()
        {
            if (_mat != null) Destroy(_mat);
            if (_mesh != null) Destroy(_mesh);
        }
    }

    /* &el=a: TASK41's white glove (Hand/drag 01–05): the fingertip presses on the piece, slides the way out, lifts,
       and again (1.6 s); the frames tilt the hand as it drags. */
    public sealed class Task41Hand : MonoBehaviour
    {
        SpriteRenderer _r;
        Sprite[] _frames;
        Vector2 _start, _end;
        float _size, _born;
        const float Period = 1.6f;
        // the fingertip in the 352 x 412 frame (from its top-left)
        static readonly Vector2 Tip = new Vector2(156f / 352, 30f / 412);

        public static Task41Hand Show(Transform parent, BoardView board, Vector2 screen, Move move, int order)
        {
            var frames = new Sprite[5];
            for (int i = 0; i < 5; i++) frames[i] = Resources.Load<Sprite>($"Art/yeni/tutorial/Hand/drag/{i + 1:00}");
            if (frames[0] == null) return null;
            var h = new GameObject("TASK41 hand").AddComponent<Task41Hand>();
            h.transform.SetParent(parent, false);
            h._frames = frames; h._born = Time.unscaledTime;
            var cells = board.Session.Level.Pieces[move.Piece];
            float s = board.Cell, x0 = float.MaxValue, y0 = float.MaxValue, x1 = 0, y1 = 0;
            foreach (var c in cells) { x0 = Mathf.Min(x0, c.X * s); y0 = Mathf.Min(y0, c.Y * s); x1 = Mathf.Max(x1, (c.X + 1) * s); y1 = Mathf.Max(y1, (c.Y + 1) * s); }
            h._start = board.Origin + new Vector2((x0 + x1) / 2, (y0 + y1) / 2);
            h._end = h._start + new Vector2(move.Dir.Dx(), move.Dir.Dy()) * s * .95f;
            h._size = Mathf.Clamp(s * 1.15f, 72, 118);   // the hand's height
            h._r = Draw.Child<SpriteRenderer>(h.transform, "glove");
            h._r.sortingOrder = order;
            h.Update();
            return h;
        }

        void Update()
        {
            float p = Mathf.Repeat(Time.unscaledTime - _born, Period) / Period;
            // .0–.15 comes down and presses, .15–.75 drags, .75–1 lifts and fades
            float drag = Mathf.SmoothStep(0, 1, Mathf.Clamp01((p - .15f) / .6f));
            float lift = Mathf.Clamp01((p - .78f) / .22f);
            int frame = p < .1f ? 0 : p < .15f ? 1 : p < .78f ? 2 + Mathf.Min(2, (int)((p - .15f) / .21f)) : 1;
            _r.sprite = _frames[frame] ?? _frames[0];
            var tip = Vector2.Lerp(_start, _end, drag) + new Vector2(0, -12 * (1 - Mathf.Clamp01(p / .12f)) - 12 * lift);
            float k = _size / _r.sprite.bounds.size.y * (p < .15f ? Mathf.Lerp(1.06f, .96f, p / .15f) : Mathf.Lerp(.96f, 1.04f, lift));
            _r.transform.localScale = Vector3.one * k;
            // the sprite's pivot is its centre: put the fingertip on the point (points, y down)
            var b = _r.sprite.bounds.size * k;   // 1 world unit = 1 point
            var centre = tip + new Vector2((.5f - Tip.x) * b.x, (.5f - Tip.y) * b.y);
            _r.transform.localPosition = Draw.W(centre);
            float alpha = p < .06f ? p / .06f : 1 - lift;
            _r.color = new Color(1, 1, 1, alpha);
        }

        public void Remove() { if (this != null) Destroy(gameObject); }
    }
}
