using UnityEngine;

namespace ReverseSolver.Presentation
{
    /* PM_73: a short trail behind a piece that leaves the board the right way (never on a jam).
         strip (&iz=serit, default): a band as wide as the piece, narrowing and fading backwards (a kite tail), in the
                picture's main tone under the piece (gold for a bomb piece), a gold sparkle at its tip.
         ghost (&iz=hayalet): two or three fading copies of the piece along its path.
       Length follows the release speed (a fast flick long, a slow push short) and never reaches behind the piece's
       home cell; every point lives Life seconds, so the trail is gone ~Life after the piece. Drawn under every piece
       (neighbours stay on top); at most Max trails at once. Board-owned, pooled meshes, no allocation per frame. */
    public sealed class ExitTrail : MonoBehaviour
    {
        public enum Kind { Strip, Ghost }
        public static Kind Style = Kind.Strip;
        // tuning (ExitTrail settings): length in cells at full speed, opacity at the head, life of a trail point
        public static float LengthCells = 2.6f, MinCells = .6f, Opacity = .88f, Life = .22f;
        public const float FullSpeed = 1000f;   // pt/s of release speed that gives LengthCells
        public const int Max = 3, Ghosts = 3, Segments = 8;
        public const int Order = 98;            // under every piece (PieceOrder 100), over the tray
        static int Layer => Order;
        public static readonly Color Gold = new Color(.98f, .78f, .3f, 1);

        sealed class Trail
        {
            public PieceView Piece;
            public Vector2 Axis, Perp, Mid;     // Mid: piece centre at release (board pt), Perp: across the motion
            public float Back0, Home, Width, MaxLength, HeadAlong, GoneAt = -1, StartOffset;
            public readonly float[] Times = new float[24], Along = new float[24];
            public int Count;
            public Color Tone;
            public bool Live;
            public SpriteRenderer Strip;
            public SpriteRenderer[] Sparks;
            public MeshRenderer[] GhostViews;
            public float[] GhostAt;
            public int GhostCount;
            public float NextGhost;
        }
        readonly Trail[] _trails = new Trail[Max];
        int _next;
        float _cell;
        Sprite _spark;
        static Sprite _band;
        const int BandW = 96, BandH = 32;
        /* The kite tail as one sprite: head at the right edge (pivot), narrowing to 22 % and fading to the left. */
        static Sprite Band()
        {
            if (_band != null) return _band;
            var tex = new Texture2D(BandW, BandH, TextureFormat.RGBA32, false) { wrapMode = TextureWrapMode.Clamp, filterMode = FilterMode.Bilinear };
            var px = new Color32[BandW * BandH];
            for (int x = 0; x < BandW; x++)
            {
                float k = 1 - (x + .5f) / BandW;                           // 0 head .. 1 tail
                // full width for the first third, then the kite tail; opaque near the piece, fading to the tip
                float half = (k < .33f ? 1f : Mathf.Lerp(1f, .22f, (k - .33f) / .67f)) * BandH * .5f, a = Mathf.Pow(1 - k, .7f);
                for (int y = 0; y < BandH; y++)
                {
                    float d = Mathf.Abs(y + .5f - BandH * .5f);
                    float edge = Mathf.Clamp01(half - d + .5f);                // soft 1 px edge
                    px[y * BandW + x] = new Color32(255, 255, 255, (byte)(255 * a * edge));
                }
            }
            tex.SetPixels32(px); tex.Apply(false, true);
            return _band = Sprite.Create(tex, new Rect(0, 0, BandW, BandH), new Vector2(1, .5f), 1);
        }

        public static ExitTrail Create(Transform board, float cell)
        {
            var t = Draw.Child<ExitTrail>(board, "Exit trails");
            t._cell = cell;
            t._spark = Resources.Load<Sprite>("Art/yeni/juice21/spark/03");
            for (int i = 0; i < Max; i++) t._trails[i] = t.Make();   // prewarmed with the board, like the juice
            return t;
        }

        /* A piece starts leaving: cells' centre and back edge from its cells, the motion axis, the release speed. */
        public void Begin(PieceView piece, Vector2 axis, float releaseSpeed, Color tone)
        {
            var tr = _trails[_next % Max] ?? (_trails[_next % Max] = Make());
            _next++;
            tr.Piece = piece; tr.Axis = axis; tr.Perp = new Vector2(-axis.y, axis.x); tr.Live = true; tr.GoneAt = -1;
            for (int i = 0; i < tr.GhostCount; i++) if (tr.GhostViews[i] != null) { Destroy(tr.GhostViews[i].gameObject); tr.GhostViews[i] = null; }
            tr.Tone = tone; tr.Count = 0; tr.GhostCount = 0; tr.NextGhost = 0;
            var offset = piece.Offset;
            float lo = float.MaxValue, hi = float.MinValue, back = float.MaxValue; var mid = Vector2.zero;
            foreach (var c in piece.Cells)
            {
                var center = new Vector2((c.X + .5f) * _cell, (c.Y + .5f) * _cell);
                mid += center;
                float across = Vector2.Dot(center, tr.Perp), along = Vector2.Dot(center, axis);
                lo = Mathf.Min(lo, across); hi = Mathf.Max(hi, across); back = Mathf.Min(back, along - _cell * .5f);
            }
            tr.Mid = mid / piece.Cells.Length;
            tr.Width = (hi - lo + _cell) * .86f;
            tr.Home = back;                                          // the trail never reaches behind the home cell
            tr.StartOffset = Vector2.Dot(offset, axis);
            tr.Back0 = back;
            float k = Mathf.Clamp01(releaseSpeed / FullSpeed);
            tr.MaxLength = Mathf.Lerp(MinCells, LengthCells, k) * _cell;
            tr.HeadAlong = back + tr.StartOffset;
            Record(tr, Time.unscaledTime);
            tr.Strip.enabled = Style == Kind.Strip;
            foreach (var s in tr.Sparks) s.enabled = false;
            foreach (var g in tr.GhostViews) if (g != null) g.enabled = false;
        }

        Trail Make()
        {
            var tr = new Trail();
            tr.Strip = Draw.Child<SpriteRenderer>(transform, "trail");
            tr.Strip.sprite = Band(); tr.Strip.sortingOrder = Layer; tr.Strip.enabled = false;
            tr.Sparks = new SpriteRenderer[2];
            for (int i = 0; i < tr.Sparks.Length; i++)
            {
                var s = Draw.Child<SpriteRenderer>(transform, "trail spark");
                s.sprite = _spark; s.sortingOrder = Layer + 1; s.enabled = false;
                tr.Sparks[i] = s;
            }
            tr.GhostViews = new MeshRenderer[Ghosts];
            tr.GhostAt = new float[Ghosts];
            return tr;
        }

        static void Record(Trail tr, float now)
        {
            if (tr.Count == tr.Times.Length)
            {
                System.Array.Copy(tr.Times, 1, tr.Times, 0, tr.Count - 1);
                System.Array.Copy(tr.Along, 1, tr.Along, 0, tr.Count - 1);
                tr.Count--;
            }
            tr.Times[tr.Count] = now; tr.Along[tr.Count] = tr.HeadAlong; tr.Count++;
        }

        void LateUpdate()
        {
            float now = Time.unscaledTime;
            foreach (var tr in _trails)
            {
                if (tr == null || !tr.Live) continue;
                bool alive = tr.Piece != null;
                if (alive)
                {
                    tr.HeadAlong = tr.Back0 + Vector2.Dot(tr.Piece.Offset, tr.Axis);
                    Record(tr, now);
                }
                else if (tr.GoneAt < 0) tr.GoneAt = now;
                float fade = tr.GoneAt < 0 ? 1 : 1 - (now - tr.GoneAt) / Life;
                if (fade <= 0)
                {
                    tr.Live = false; tr.Strip.enabled = false;
                    foreach (var s in tr.Sparks) s.enabled = false;
                    for (int i = 0; i < tr.GhostCount; i++) if (tr.GhostViews[i] != null) { Destroy(tr.GhostViews[i].gameObject); tr.GhostViews[i] = null; }
                    tr.GhostCount = 0;
                    continue;
                }
                // tail: the oldest point younger than Life, no longer than MaxLength, never behind the home cell
                float tail = tr.HeadAlong;
                for (int i = 0; i < tr.Count; i++)
                    if (now - tr.Times[i] < Life) { tail = tr.Along[i]; break; }
                tail = Mathf.Max(tail, tr.HeadAlong - tr.MaxLength, tr.Home);
                float length = tr.HeadAlong - tail;
                if (Style == Kind.Strip) DrawStrip(tr, tail, length, fade, now);
                else DrawGhosts(tr, alive, fade, now);
            }
        }

        void DrawStrip(Trail tr, float tail, float length, float fade, float now)
        {
            var across = tr.Mid - tr.Axis * Vector2.Dot(tr.Mid, tr.Axis);   // the line the piece moves on
            var head = across + tr.Axis * tr.HeadAlong;
            var strip = tr.Strip;
            strip.enabled = length > 1;
            strip.transform.localPosition = Draw.W(head);
            strip.transform.localRotation = Quaternion.Euler(0, 0, Mathf.Atan2(-tr.Axis.y, tr.Axis.x) * Mathf.Rad2Deg);
            var size = strip.sprite != null ? (Vector2)strip.sprite.bounds.size : new Vector2(BandW, BandH);
            strip.transform.localScale = new Vector3(Mathf.Max(.01f, length) / size.x, tr.Width / size.y, 1);
            var c = tr.Tone; c.a = Opacity * fade; strip.color = c;
            // gold sparkle at the tip of the tail
            var tip = across + tr.Axis * (tr.HeadAlong - length * .92f);
            for (int i = 0; i < tr.Sparks.Length; i++)
            {
                var s = tr.Sparks[i];
                s.enabled = _spark != null && length > _cell * .3f;
                if (!s.enabled) continue;
                float side = i == 0 ? .18f : -.14f;
                s.transform.localPosition = Draw.W(tip + tr.Perp * (tr.Width * side) - tr.Axis * (i * _cell * .12f));
                float twinkle = .75f + .25f * Mathf.Sin(now * 40 + i * 2);
                s.transform.localScale = Vector3.one * (_cell * (i == 0 ? .34f : .24f) * twinkle / Mathf.Max(.01f, _spark.bounds.size.x));
                s.transform.localRotation = Quaternion.Euler(0, 0, now * 400 + i * 45);
                s.color = new Color(Gold.r, Gold.g, Gold.b, fade * .95f);
            }
        }

        void DrawGhosts(Trail tr, bool alive, float fade, float now)
        {
            // a copy every ~45 ms while the piece is moving, three at most, each fading out over Life
            if (alive && tr.GhostCount < Ghosts && now >= tr.NextGhost && tr.HeadAlong - tr.Home > _cell * .25f)
            {
                var g = tr.Piece.Ghost(transform, Layer - tr.GhostCount);
                tr.GhostViews[tr.GhostCount] = g; tr.GhostAt[tr.GhostCount] = now; tr.GhostCount++;
                tr.NextGhost = now + .045f;
            }
            for (int i = 0; i < tr.GhostCount; i++)
            {
                var g = tr.GhostViews[i];
                if (g == null) continue;
                float a = Opacity * .75f * Mathf.Clamp01(1 - (now - tr.GhostAt[i]) / Life) * (1 - i * .18f);
                PieceView.SetGhostAlpha(g, a);
                g.enabled = a > .01f;
            }
        }

    }
}
