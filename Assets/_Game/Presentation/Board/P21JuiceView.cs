using ReverseSolver.Core;
using UnityEngine;

namespace ReverseSolver.Presentation
{
    // Board-owned, prewarmed pools. No tweens, coroutines or loads in the animation loop.
    public sealed class P21JuiceView : MonoBehaviour
    {
        public static bool Enabled => Art.T54 && TelemetryMeta.Current?.Cohort == "p21";
        const float Quick = 1.2f;
        struct Puff
        {
            public SpriteRenderer View;
            public Sprite[] Frames;
            public float At, Life, Size;
            public Color Tint;
        }
        struct Badge
        {
            public SpriteRenderer View;
            public TextView Text;
            public float At;
        }
        readonly Puff[] _puffs = new Puff[16];
        readonly Badge[] _badges = new Badge[2];
        readonly string[] _combos = new string[100];
        Sprite[] _pop, _confetti, _metal, _praise;
        BoardView _board;
        float _lastExit = -10;
        int _next, _combo, _praisedMove;

        public static P21JuiceView Create(BoardView board)
        {
            var v = Draw.Child<P21JuiceView>(board.transform, "P21 juice");
            v._board = board;
            v._praisedMove = board.Session.Moves;
            v._pop = Frames("pop", 9); v._confetti = Frames("confetti", 6); v._metal = Frames("metal", 9);
            v._praise = new[] { Load("great"), Load("super"), Load("perfect") };
            for (int i = 0; i < v._puffs.Length; i++)
                v._puffs[i] = new Puff { View = Layer(v.transform, "puff", 2200), At = -1 };
            for (int i = 0; i < v._badges.Length; i++)
            {
                var r = Layer(v.transform, "badge", 2250);
                var text = TextView.Create(r.transform, "label", "", i == 0 ? 27 : 23, Vector4.one, order: 2251)
                    .Heading(TextView.RimTeal);
                v._badges[i] = new Badge { View = r, Text = text, At = -1 };
                text.gameObject.SetActive(false);
            }
            for (int i = 2; i < v._combos.Length; i++) v._combos[i] = "x" + i;
            v._badges[0].View.sprite = Load("combo");
            return v;
        }

        static Sprite Load(string key) => Resources.Load<Sprite>("Art/yeni/juice21/" + key);
        static Sprite[] Frames(string key, int count)
        {
            var frames = new Sprite[count];
            for (int i = 0; i < count; i++) frames[i] = Load(key + "/" + (i + 1).ToString("00"));
            return frames;
        }
        static SpriteRenderer Layer(Transform parent, string name, int order)
        {
            var r = Draw.Child<SpriteRenderer>(parent, name);
            r.sortingOrder = order; r.enabled = false; return r;
        }

        public void OnEvent(GameEvent e, PieceView piece)
        {
            if (e.Kind == EventKind.JointCut)
            {
                var at = e.Vertical ? new Vector2(e.X, e.Y + .5f) : new Vector2(e.X + .5f, e.Y);
                Burst(_metal, at * _board.Cell, Color.white, .45f, 1.8f);
                return;
            }
            if (piece == null) return;
            var point = Vector2.zero;
            foreach (var c in piece.Cells) point += new Vector2(c.X + .5f, c.Y + .5f) * _board.Cell;
            point /= piece.Cells.Length;
            point += piece.Offset;
            if (e.Kind == EventKind.NailFreed || (e.Kind == EventKind.PieceRemoved && e.ByHammer && piece.Cells.Length > 1))
                Burst(_metal, point, Color.white, .45f, 1.8f);
            if (e.Kind != EventKind.PieceRemoved) return;
            var tint = PieceView.CellColor(_board.Session.Level, piece.Cells[0]);
            Burst(_pop, point, tint, .36f, 1.9f);
            Burst(_confetti, point, tint, .65f, 2.5f);
            float now = Time.unscaledTime;
            _combo = now - _lastExit <= Quick ? _combo + 1 : 1; _lastExit = now;
            if (_combo < 2) return;
            // Badges sit inside the board, away from both the timer and booster bar.
            var badgePoint = new Vector2(_board.Size.x * .5f, _board.Cell * .45f);
            ShowBadge(0, _badges[0].View.sprite, _combos[Mathf.Min(_combo, 99)], badgePoint, _board.Cell * 1.3f);
            int moves = _board.Session.Moves;
            if (moves - _praisedMove < 3) return;
            _praisedMove = moves;
            int tier = Mathf.Min(2, _combo - 2);
            ShowBadge(1, _praise[tier], Texts.JuicePraise(tier), badgePoint + new Vector2(0, _board.Cell * .8f), _board.Cell * 2.6f);
        }

        void Burst(Sprite[] frames, Vector2 point, Color tint, float life, float size)
        {
            ref var p = ref _puffs[_next++ % _puffs.Length];
            p.Frames = frames; p.At = Time.unscaledTime; p.Life = life; p.Size = _board.Cell * size; p.Tint = tint;
            p.View.transform.localPosition = Draw.W(point);
        }
        void ShowBadge(int index, Sprite sprite, string label, Vector2 point, float width)
        {
            ref var b = ref _badges[index];
            b.At = Time.unscaledTime; b.View.sprite = sprite;
            b.View.transform.localPosition = Draw.W(point);
            // Keep text in points: scale the renderer's size rather than its transform.
            b.View.drawMode = SpriteDrawMode.Sliced;
            b.View.size = sprite != null ? new Vector2(width, width * sprite.bounds.size.y / sprite.bounds.size.x) : Vector2.one;
            b.Text.Text = label; b.Text.gameObject.SetActive(true);
        }
        void Update()
        {
            float now = Time.unscaledTime;
            for (int i = 0; i < _puffs.Length; i++)
            {
                ref var p = ref _puffs[i];
                if (p.At < 0) continue;
                float t = (now - p.At) / p.Life;
                if (t >= 1) { p.View.enabled = false; p.At = -1; continue; }
                var sprite = p.Frames[Mathf.Min((int)(t * p.Frames.Length), p.Frames.Length - 1)];
                p.View.sprite = sprite; p.View.enabled = sprite != null;
                if (sprite == null) continue;
                float scale = p.Size / sprite.bounds.size.x * (1 + .12f * Mathf.Sin(t * Mathf.PI));
                p.View.transform.localScale = Vector3.one * scale;
                var tint = p.Tint; tint.a *= 1 - t * t; p.View.color = tint;
            }
            for (int i = 0; i < _badges.Length; i++)
            {
                ref var b = ref _badges[i];
                if (b.At < 0) continue;
                float t = (now - b.At) / .8f;
                if (t >= 1) { b.View.enabled = false; b.Text.gameObject.SetActive(false); b.At = -1; continue; }
                b.View.enabled = b.View.sprite != null;
                b.View.transform.localScale = Vector3.one * (1 + .12f * Mathf.Sin(Mathf.Min(1, t * 4) * Mathf.PI));
                float alpha = 1 - Mathf.InverseLerp(.65f, 1, t);
                b.View.color = new Color(1, 1, 1, alpha); b.Text.Alpha = alpha;
            }
        }
    }
}
