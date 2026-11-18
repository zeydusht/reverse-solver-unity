using ReverseSolver.Core;
using UnityEngine;

namespace ReverseSolver.Presentation
{
    /* PM_85 §1: the guide's arrow. One thick, flat cream ribbon along the piece's way out, its head the way it goes
       (Codex's art in Art/yeni/tutorial/Arrow85: tail, body (stretched), head, drawn pointing down). It opens the
       way first: it grows out of the piece in ~.3 s. Then
         Still   it stays (the guided steps 2–5: the hand drags along it)
         Nudge   it nudges twice back and forth along the way, rests, and again (PM_84's hint)
       Without the art: the same ribbon drawn flat (cream, a soft dark shadow). Only graphics. */
    public sealed class GuideArrow85 : MonoBehaviour
    {
        public enum Mode { Still, Nudge }
        public const float GrowSeconds = .3f;

        Transform _pivot;
        SpriteRenderer _tail, _body, _head;
        SolidView _flatShadow, _flat;
        Vector2 _start, _dir;
        float _length, _width, _born, _fadeOut = -1, _nudge;
        Mode _mode;
        int _order;
        static Sprite[] _art;

        static Sprite[] Art85()
        {
            if (_art == null)
            {
                var t = Resources.Load<Sprite>("Art/yeni/tutorial/Arrow85/tail");
                var b = Resources.Load<Sprite>("Art/yeni/tutorial/Arrow85/body");
                var h = Resources.Load<Sprite>("Art/yeni/tutorial/Arrow85/head");
                _art = b != null && h != null ? new[] { t, b, h } : new Sprite[0];
            }
            return _art.Length > 0 ? _art : null;
        }

        /* start/end in the parent's layout points (y down); width the ribbon's width in points. */
        public static GuideArrow85 Show(Transform parent, Vector2 start, Vector2 end, float width, int order, Mode mode, float delay = 0)
        {
            var a = new GameObject("PM85 arrow").AddComponent<GuideArrow85>();
            a.transform.SetParent(parent, false);
            a._start = start; a._length = Mathf.Max(width * 2.2f, Vector2.Distance(start, end));
            a._dir = (end - start).sqrMagnitude > 1e-4f ? (end - start).normalized : Vector2.down;
            a._width = width; a._order = order; a._mode = mode; a._born = Time.unscaledTime + delay;
            a._nudge = width * .55f;
            a._pivot = new GameObject("pivot").transform;
            a._pivot.SetParent(a.transform, false);
            // the art points down (layout +y): turn it to the way
            float rot = Mathf.Atan2(a._dir.x, a._dir.y) * Mathf.Rad2Deg;   // down 0, right 90, up 180, left -90
            a._pivot.localRotation = Quaternion.Euler(0, 0, rot);
            var art = Art85();
            if (art != null)
            {
                if (art[0] != null) a._tail = a.Part("tail", art[0], order);
                a._body = a.Part("body", art[1], order);
                a._head = a.Part("head", art[2], order + 1);
            }
            else
            {
                a._flatShadow = SolidView.Polygon(a._pivot, "shadow", new Vector2[3], Draw.Rgba(10, 30, 40, .35f), order);
                a._flat = SolidView.Polygon(a._pivot, "ribbon", new Vector2[3], Draw.Hex("#fff3d6"), order + 1);
            }
            a.Update();
            return a;
        }

        SpriteRenderer Part(string name, Sprite s, int order)
        {
            var r = Draw.Child<SpriteRenderer>(_pivot, name);
            r.sprite = s; r.sortingOrder = order;
            return r;
        }

        public static GuideArrow85 ForMove(Transform parent, BoardView board, Vector2 screen, Move move, int order, Mode mode, float delay = 0)
        {
            float s = board.Cell;
            var cells = board.Session.Level.Pieces[move.Piece];
            float x0 = float.MaxValue, y0 = float.MaxValue, x1 = 0, y1 = 0;
            foreach (var c in cells) { x0 = Mathf.Min(x0, c.X * s); y0 = Mathf.Min(y0, c.Y * s); x1 = Mathf.Max(x1, (c.X + 1) * s); y1 = Mathf.Max(y1, (c.Y + 1) * s); }
            var centre = board.Origin + new Vector2((x0 + x1) / 2, (y0 + y1) / 2);
            var dir = new Vector2(move.Dir.Dx(), move.Dir.Dy());
            var start = centre - dir * s * .1f;   // from the piece's middle: the way reads from the piece itself
            var edge = move.Dir switch
            {
                Dir.L => new Vector2(board.Origin.x, centre.y),
                Dir.R => new Vector2(board.Origin.x + board.Size.x, centre.y),
                Dir.U => new Vector2(centre.x, board.Origin.y),
                _ => new Vector2(centre.x, board.Origin.y + board.Size.y)
            };
            var end = edge + dir * s * .9f;     // past the board's edge, as far as the lit way
            end = new Vector2(Mathf.Clamp(end.x, 10, screen.x - 10), Mathf.Clamp(end.y, 10, screen.y - 10));
            float width = Mathf.Clamp(s * .32f, 18, 32);
            return Show(parent, start, end, width, order, mode, delay);
        }

        public void FadeOut() { if (_fadeOut < 0) _fadeOut = Time.unscaledTime; }
        public bool Grown => Time.unscaledTime - _born >= GrowSeconds;

        void Update()
        {
            float now = Time.unscaledTime, age = now - _born;
            float grow = Mathf.Clamp01(age / GrowSeconds);
            grow = 1 - (1 - grow) * (1 - grow) * (1 - grow);   // out-cubic: it shoots out and settles
            float alpha = age < 0 ? 0 : Mathf.Clamp01(age / .12f);
            if (_fadeOut >= 0)
            {
                float k = Mathf.Clamp01((now - _fadeOut) / .15f);
                alpha *= 1 - k;
                if (k >= 1) { Destroy(gameObject); return; }
            }
            float shift = 0;
            if (_mode == Mode.Nudge && age > GrowSeconds)
            {
                // twice forth and back (1 s), then a rest (.7 s)
                float p = Mathf.Repeat(age - GrowSeconds, 1.7f);
                if (p < 1f) shift = Mathf.Sin(p * 2 * Mathf.PI * 2 - Mathf.PI / 2) * .5f + .5f;
            }
            transform.localPosition = Draw.W(_start + _dir * shift * _nudge);
            Lay(Mathf.Lerp(_width * 1.4f, _length, grow), alpha);
        }

        /* The ribbon `len` long from the pivot down (layout +y in the pivot's frame). */
        void Lay(float len, float alpha)
        {
            float w = _width;
            if (_body != null)
            {
                float px = w / 84f;                                         // points per art pixel (the shaft is 84 px wide)
                float tailH = _tail != null ? 64 * px : 0, headH = 160 * px;
                float bodyLen = Mathf.Max(.01f, len - tailH - headH * .98f);
                Place(_tail, tailH / 2, px, 64 * px);
                Place(_body, tailH + bodyLen / 2, px, bodyLen, stretch: true);
                Place(_head, tailH + bodyLen + headH / 2 - headH * .02f, px, headH);
                foreach (var r in new[] { _tail, _body, _head }) if (r != null) r.color = new Color(1, 1, 1, alpha);
                return;
            }
            float hw = w * 1.05f, hl = w * 1.15f, body = Mathf.Max(0, len - hl);
            var ribbon = new[] { new Vector2(-w / 2, 0), new Vector2(w / 2, 0), new Vector2(w / 2, body), new Vector2(hw, body), new Vector2(0, len), new Vector2(-hw, body), new Vector2(-w / 2, body) };
            // fan from the centre of the shaft (the shape is star-convex from there)
            var pts = new Vector2[ribbon.Length + 2];
            pts[0] = new Vector2(0, body * .5f);
            for (int i = 0; i < ribbon.Length; i++) pts[i + 1] = ribbon[i];
            pts[pts.Length - 1] = ribbon[0];
            var sh = new Vector2[pts.Length];
            for (int i = 0; i < pts.Length; i++) sh[i] = pts[i] * 1.08f + new Vector2(0, 2);
            _flat.SetPolygon(pts, Draw.Hex("#fff3d6"));
            _flatShadow.SetPolygon(sh, Draw.Rgba(10, 30, 40, .35f));
            _flat.Alpha(alpha); _flatShadow.Alpha(alpha);
        }

        void Place(SpriteRenderer r, float y, float px, float h, bool stretch = false)
        {
            if (r == null) return;
            var size = r.sprite.bounds.size;   // world units of the sprite at scale 1
            float ppx = size.x / (r.sprite.rect.width);   // world units per art pixel
            float sx = px / ppx;
            float sy = stretch ? h / size.y : sx;
            r.transform.localScale = new Vector3(sx, sy, 1);
            r.transform.localPosition = Draw.W(0, y);
        }
    }
}
