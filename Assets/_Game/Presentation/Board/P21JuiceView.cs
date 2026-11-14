using ReverseSolver.Core;
using UnityEngine;

namespace ReverseSolver.Presentation
{
    /* PM_72: no combo (no praise words, badges, frame wave, shock ring or counter; T47 is gone). Every exit gets a
       small satisfying moment instead: a small opaque gold spark where the piece leaves the board (0.25 s, never
       covering the board); a bomb piece a slightly bigger burst and its bomb icon going out. Sound and haptics are
       played by GameRoot. Board-owned, prewarmed pools: no tweens, coroutines or loads in the animation loop. */
    public sealed class P21JuiceView : MonoBehaviour
    {
        public static bool Enabled => Art.P21Gameplay;
        public const float SparkLife = .25f, SparkSize = .95f, BombSparkLife = .32f, BombSparkSize = 1.45f, FizzleLife = .38f;
        struct Puff
        {
            public SpriteRenderer View;
            public Sprite[] Frames;
            public float At, Life, Size;
            public Color Tint;
            public bool Fizzle;
        }
        readonly Puff[] _puffs = new Puff[12];
        Sprite[] _spark, _metal, _bomb;
        BoardView _board;
        int _next;

        public static P21JuiceView Create(BoardView board)
        {
            var v = Draw.Child<P21JuiceView>(board.transform, "P21 juice");
            v._board = board;
            v._spark = Frames("spark", 6); v._metal = Frames("metal", 9);   // spark: Tools/make_pm72_spark.py
            v._bomb = new[] { Art.Get("bomb-v2") ?? Art.Get("board-bomb") };
            for (int i = 0; i < v._puffs.Length; i++)
                v._puffs[i] = new Puff { View = Layer(v.transform, "spark", 2200), At = -1 };
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
                Burst(_metal, at * _board.Cell, Color.white, .4f, 1.2f);
                return;
            }
            if (piece == null) return;
            var point = Vector2.zero;
            foreach (var c in piece.Cells) point += new Vector2(c.X + .5f, c.Y + .5f) * _board.Cell;
            point /= piece.Cells.Length;
            point += piece.Offset;
            if (e.Kind == EventKind.NailFreed) Burst(_metal, point, Color.white, .35f, 1.1f);   // the lock breaking
            if (e.Kind == EventKind.PieceRemoved && e.ByHammer) ExitAt(piece, point);
        }

        /* Where the piece leaves the board (BoardView, as it crosses the edge). */
        public void ExitAt(PieceView piece, Vector2 point)
        {
            bool bomb = _board.Session.Level.Bombs.ContainsKey(piece.Index);
            Burst(_spark, point, Color.white, bomb ? BombSparkLife : SparkLife, bomb ? BombSparkSize : SparkSize);
            if (bomb && _bomb[0] != null) Burst(_bomb, point, Color.white, FizzleLife, .55f, fizzle: true);
        }

        void Burst(Sprite[] frames, Vector2 point, Color tint, float life, float size, bool fizzle = false)
        {
            ref var p = ref _puffs[_next++ % _puffs.Length];
            p.Frames = frames; p.At = Time.unscaledTime; p.Life = life; p.Size = _board.Cell * size; p.Tint = tint; p.Fizzle = fizzle;
            p.View.transform.localPosition = Draw.W(point);
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
                if (p.Fizzle)
                {
                    // the bomb going out: it greys, shrinks a little and fades
                    p.View.transform.localScale = Vector3.one * (p.Size / sprite.bounds.size.x * Mathf.Lerp(1f, .7f, t));
                    float g = Mathf.Lerp(1f, .45f, Mathf.Clamp01(t * 2));
                    p.View.color = new Color(g, g, g, 1 - t * t);
                    continue;
                }
                float scale = p.Size / sprite.bounds.size.x * (.85f + .25f * Mathf.Sin(t * Mathf.PI * .5f));
                p.View.transform.localScale = Vector3.one * scale;
                var tint = p.Tint; tint.a = t < .6f ? 1 : 1 - (t - .6f) / .4f; p.View.color = tint;   // opaque, then a quick fade
            }
        }
    }
}
