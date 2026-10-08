using System.Collections.Generic;
using ReverseSolver.Core;
using UnityEngine;

namespace ReverseSolver.Presentation
{
    /* The board as the web game draws it: tray, slots, sealed-edge walls and
       the pieces with their chains, nails and bombs. It only reads the session
       and follows its events; moves happen through DragModel -> GameSession.

       Layout is in points with (0, 0) at the board's top-left corner. */
    public sealed class BoardView : MonoBehaviour
    {
        // sorting: tray < slots < pieces < chained pieces < walls < lifted piece
        const int TrayOrder = 0, SlotOrder = 5, PieceOrder = 100, PieceStride = 12, ChainedBoost = 600,
                  WallOrder = 1300, LiftOrder = 1500;

        public GameSession Session { get; private set; }
        public float Cell { get; private set; }
        public Vector2 Origin { get; private set; }
        public Vector2 Size => new Vector2(Session.Level.Width * Cell, Session.Level.Height * Cell);

        readonly Dictionary<int, PieceView> _pieces = new Dictionary<int, PieceView>();
        readonly Dictionary<int, NailBadge> _nails = new Dictionary<int, NailBadge>();
        readonly Dictionary<int, BombBadge> _bombs = new Dictionary<int, BombBadge>();
        int _blocker = -1, _dragged = -1;

        public static BoardView Create(Transform parent, GameSession session, float cell, Vector2 origin)
        {
            var v = new GameObject("Board").AddComponent<BoardView>();
            v.transform.SetParent(parent, false);
            v.Session = session;
            v.Cell = cell;
            v.Origin = origin;
            v.transform.localPosition = Draw.W(origin);
            v.Build();
            session.EventRaised += v.OnEvent;
            return v;
        }

        void OnDestroy()
        {
            if (Session != null) Session.EventRaised -= OnEvent;
            foreach (var p in _pieces.Values) if (p != null) Tweens.Kill(p);
        }

        void Build()
        {
            var l = Session.Level;
            float S = Cell, W = l.Width * S, H = l.Height * S;

            // .tray: padding 18, radius 20, gradient #0f3339 -> #0a262b, inset highlights, 0 22px 44px rgba(3,14,16,.6)
            var tray = ShapeView.Create(transform, "Tray", new Vector2(W + 36, H + 36), 60, TrayOrder)
                .Radius(20).Gradient(Draw.Hex("#0f3339"), Draw.Hex("#0a262b"))
                .Inner(Draw.Rgba(0, 0, 0, .55f), 8, 22)
                .Stroke(Draw.Rgba(255, 255, 255, .06f), 1)
                .Shadow(Draw.Rgba(3, 14, 16, .6f), 0, 22, 44);
            tray.transform.localPosition = Draw.W(W / 2, H / 2);

            // .slot: radius 4, rgba(4,20,23,.42), inset 0 2px 3px rgba(0,0,0,.5)
            for (int y = 0; y < l.Height; y++)
                for (int x = 0; x < l.Width; x++)
                {
                    var slot = ShapeView.Create(transform, "Slot", new Vector2(S, S), 2, SlotOrder)
                        .Radius(4).Fill(Draw.Rgba(4, 20, 23, .42f)).Inner(Draw.Rgba(0, 0, 0, .5f), 2, 3);
                    slot.transform.localPosition = Draw.W((x + .5f) * S, (y + .5f) * S);
                }

            // .wall: repeating-linear-gradient(45deg, #d9412f 0 9px, #f6efe6 9px 18px), ring 2px #7d1f16
            float t = Mathf.Max(7f, S * .16f), ov = S * .06f;
            foreach (var side in l.Sealed)
                foreach (int lane in side.Value)
                {
                    Rect r = side.Key switch
                    {
                        Dir.U => new Rect(lane * S - ov, -t * 1.5f, S + ov * 2, t),
                        Dir.D => new Rect(lane * S - ov, H + t * .5f, S + ov * 2, t),
                        Dir.L => new Rect(-t * 1.5f, lane * S - ov, t, S + ov * 2),
                        _ => new Rect(W + t * .5f, lane * S - ov, t, S + ov * 2)
                    };
                    var wall = ShapeView.Create(transform, "Wall", r.size, 10, WallOrder)
                        .Radius(3).Stripes(Draw.Hex("#d9412f"), Draw.Hex("#f6efe6"))
                        .Stroke(Draw.Hex("#7d1f16"), 2).Shadow(Draw.Rgba(0, 0, 0, .55f), 0, 3, 8);
                    wall.transform.localPosition = Draw.W(r.center);
                }

            int chainNo = 0;
            for (int i = 0; i < l.Pieces.Count; i++)
            {
                var cells = l.Pieces[i];
                if (!Session.Board.IsPresent(i)) { if (cells.Length > 1) chainNo++; continue; }
                bool chained = cells.Length > 1;
                var pv = PieceView.Create(transform, Session.Board, i, S, PieceOrder + i * PieceStride + (chained ? ChainedBoost : 0));
                _pieces[i] = pv;
                if (chained)
                    ChainView.Build(pv.transform, Center(cells[0]), Center(cells[1]), S,
                                    ChainView.Tints[chainNo++ % ChainView.Tints.Length], pv.SortingOrder + 1);
                if (Session.NailAt(i) > 0)
                {
                    var n = new NailBadge(pv.transform, pv.FirstCellCenter, S, pv.SortingOrder + 8) { Count = Session.NailAt(i) };
                    _nails[i] = n;
                    pv.SetPinned(true);
                }
                if (Session.FuseAt(i) > 0)
                    _bombs[i] = new BombBadge(pv.transform, pv.FirstCellCenter, S, pv.SortingOrder + 8) { Fuse = Session.FuseAt(i) };
            }
        }

        /* PieceAt plus the margin around the board: a press up to EdgeGrab.Reach
           outside takes the nearest edge piece. */
        public int GrabAt(Vector2 screenPt, out bool outside)
        {
            var p = screenPt - Origin;
            return EdgeGrab.PieceAt(Session.Board, p.x, p.y, Cell, out outside);
        }

        /* G1: show the level's picture on every piece; fades in over `fade` seconds. */
        public void SetImage(Texture image, float fade = 0)
        {
            _image = image;
            if (fade <= 0 || image == null) { foreach (var p in _pieces.Values) if (p != null) p.SetImage(image, 1); _imageMix = 1; return; }
            Tweens.Run(this, fade, t => { _imageMix = t; foreach (var p in _pieces.Values) if (p != null) p.SetImage(_image, t); }, null);
        }

        Texture _image;
        float _imageMix;
        public bool HasImage => _image != null;

        public PieceView PieceViewOf(int piece) => _pieces.TryGetValue(piece, out var p) ? p : null;

        // ---- web effects (M6) ----------------------------------------------------------

        /* .pc.deal on a level's first draw: from scale .72, 8 pt up and transparent,
           .34s cubic-bezier(.2,.9,.3,1.15), each piece (x + y) * 22 ms later. */
        public void Deal()
        {
            foreach (var pv in _pieces.Values)
            {
                var p = pv;
                var c = p.Cells[0];
                float delay = (c.X + c.Y) * .022f, total = delay + .34f;
                p.SetDeal(.72f, -8f);
                p.SetAlpha(0);
                Tweens.Run((p, "deal"), total, t =>
                {
                    if (p == null) return;
                    float k = Mathf.Clamp01((t * total - delay) / .34f);
                    float e = Tweens.Deal(k);
                    p.SetDeal(Mathf.LerpUnclamped(.72f, 1f, e), Mathf.LerpUnclamped(-8f, 0f, e));
                    p.SetAlpha(Mathf.Clamp01(e));
                }, () => { if (p != null) { p.SetDeal(1, 0); p.SetAlpha(1); } });
            }
        }

        /* .pc.clang: a pinned piece pressed shakes -3, +3 pt over .3s. */
        public void Clang(int piece)
        {
            if (!_pieces.TryGetValue(piece, out var pv)) return;
            Tweens.Run((pv, "clang"), .3f, t =>
            {
                float x = t < .25f ? -3f * (t / .25f) : t < .6f ? Mathf.Lerp(-3f, 3f, (t - .25f) / .35f) : Mathf.Lerp(3f, 0f, (t - .6f) / .4f);
                if (pv != null) pv.SetOffset(new Vector2(x, 0));
            }, () => { if (pv != null) pv.SetOffset(Vector2.zero); });
        }

        /* .pc.popped: a freed piece flashes bright with an amber glow, .5s ease. */
        void Popped(int piece)
        {
            if (!_pieces.TryGetValue(piece, out var pv)) return;
            var amber = Draw.Hex("#f0a742");
            Tweens.Run((pv, "pop"), .5f, t =>
            {
                if (pv == null) return;
                float k = 1 - Tweens.Ease(t);
                pv.SetFlash(.55f * k);
                pv.SetGlow(amber, k);
            }, () => { if (pv != null) { pv.SetFlash(0); pv.SetGlow(Vector4.zero, 0); } });
        }

        Vector2 Center(Cell c) => new Vector2((c.X + .5f) * Cell, (c.Y + .5f) * Cell);

        // ---- hit testing ------------------------------------------------------------

        /* The present piece under a screen point (points, y down), or -1. */
        public int PieceAt(Vector2 screenPt)
        {
            var p = screenPt - Origin;
            int x = Mathf.FloorToInt(p.x / Cell), y = Mathf.FloorToInt(p.y / Cell);
            return Session.Board.PieceAt(new Cell(x, y));
        }

        public bool Contains(Vector2 screenPt)
        {
            var p = screenPt - Origin;
            return p.x >= -18 && p.y >= -18 && p.x <= Size.x + 18 && p.y <= Size.y + 18;
        }

        // ---- drag ---------------------------------------------------------------------

        /* Called every frame while a drag is active: the piece sits exactly at
           the finger (no smoothing), lifted, with the blocking piece glowing. */
        public void ShowDrag(DragModel drag)
        {
            if (!drag.Active || !_pieces.TryGetValue(drag.Piece, out var pv)) return;
            if (_dragged != drag.Piece)
            {
                _dragged = drag.Piece;
                Tweens.Kill(pv);
                pv.SortingOrder = LiftOrder;
                pv.SetLift(1);
            }
            var d = drag.Direction;
            pv.SetOffset(d.HasValue ? new Vector2(d.Value.Dx(), d.Value.Dy()) * drag.Offset : Vector2.zero);
            SetBlocker(drag.Blocker);
        }

        public void EndDrag(int piece, DragRelease result, Dir dir, int travelCells)
        {
            SetBlocker(-1);
            _dragged = -1;
            if (!_pieces.TryGetValue(piece, out var pv)) return;
            if (result == DragRelease.Exited) { FlyOut(pv, dir, travelCells); return; }

            // .pc.snap: transform .22s cubic-bezier(.34,1.4,.5,1)
            var from = pv.Offset;
            int home = HomeOrder(piece);
            Tweens.Run(pv, .22f, t =>
            {
                pv.SetOffset(Vector2.LerpUnclamped(from, Vector2.zero, Tweens.Settle(t)));
                pv.SetLift(1 - t);
            }, () => { pv.SortingOrder = home; pv.SetLift(0); });
        }

        int HomeOrder(int piece) =>
            PieceOrder + piece * PieceStride + (Session.Level.Pieces[piece].Length > 1 ? ChainedBoost : 0);

        void FlyOut(PieceView pv, Dir dir, int travelCells)
        {
            // .pc.fly: to (cells + 2.6) cells, .3s cubic-bezier(.3,0,.6,1), fading out
            var from = pv.Offset;
            var to = new Vector2(dir.Dx(), dir.Dy()) * ((travelCells + 2.6f) * Cell);
            _pieces.Remove(pv.Index);
            Tweens.Run(pv, .3f, t =>
            {
                pv.SetOffset(Vector2.LerpUnclamped(from, to, Tweens.FlyOut(t)));
                pv.SetAlpha(1 - t);
            }, () => Destroy(pv.gameObject));
        }

        void SetBlocker(int piece)
        {
            if (piece == _blocker) return;
            if (_blocker >= 0 && _pieces.TryGetValue(_blocker, out var old)) old.SetGlow(Vector4.zero, 0);
            _blocker = piece;
            // .pc.blocker: drop-shadow(0 0 7px var(--coral))
            if (piece >= 0 && _pieces.TryGetValue(piece, out var pv)) pv.SetGlow(Draw.Hex("#e8674c"), 1);
        }

        // ---- scissors joints ----------------------------------------------------------

        readonly List<(Vector2 at, int x, int y, bool vertical, ShapeView view)> _joints =
            new List<(Vector2, int, int, bool, ShapeView)>();

        /* Web .joint: amber dots on the cuttable joints, pulsing, size max(14, S*.30). */
        public void ShowJoints(List<(int x, int y, bool vertical)> joints)
        {
            HideJoints();
            float d = Mathf.Max(14f, Cell * .30f);
            foreach (var (x, y, v) in joints)
            {
                var at = v ? new Vector2(x * Cell, (y + .5f) * Cell) : new Vector2((x + .5f) * Cell, y * Cell);
                var dot = ShapeView.Create(transform, "joint", new Vector2(d, d), 6, LiftOrder + 50).Radius(d / 2)
                    .Fill(Draw.Hex("#f0a742")).Stroke(Draw.Rgba(240, 167, 66, .3f), 3);
                dot.transform.localPosition = Draw.W(at);
                _joints.Add((at, x, y, v, dot));
                Pulse(dot);
            }
        }

        // @keyframes pulse { 0%,100% scale 1; 50% scale 1.3 } 1.1s
        void Pulse(ShapeView dot) =>
            Tweens.Run(dot, 1.1f, t => { if (dot != null) dot.transform.localScale = Vector3.one * (1 + .3f * Mathf.Sin(t * Mathf.PI)); },
                       () => { if (dot != null) Pulse(dot); });

        public void HideJoints()
        {
            foreach (var j in _joints) { Tweens.Kill(j.view); if (j.view != null) Destroy(j.view.gameObject); }
            _joints.Clear();
        }

        /* Nearest joint within a 44pt touch target (22pt radius), or null. */
        public (int x, int y, bool vertical)? JointAt(Vector2 screenPt)
        {
            var p = screenPt - Origin;
            float best = 22f * 22f;
            (int, int, bool)? hit = null;
            foreach (var j in _joints)
            {
                float d2 = (j.at - p).sqrMagnitude;
                if (d2 <= best) { best = d2; hit = (j.x, j.y, j.vertical); }
            }
            return hit;
        }

        // ---- session events -----------------------------------------------------------

        void OnEvent(GameEvent e)
        {
            switch (e.Kind)
            {
                case EventKind.PieceRemoved:
                    // a drag-out is animated by EndDrag; anything else (hammer, M4) just leaves
                    if (e.ByHammer && _pieces.TryGetValue(e.Piece, out var hv)) FlyOut(hv, Dir.U, 0);
                    _nails.Remove(e.Piece);
                    _bombs.Remove(e.Piece);
                    break;
                case EventKind.NailsTicked:
                    RefreshNails();
                    foreach (var n in _nails.Values) n.Tick();
                    break;
                case EventKind.NailFreed:
                case EventKind.ValveReleased:
                    RefreshNails();
                    Popped(e.Piece);
                    break;
                case EventKind.BombsTicked:
                    foreach (var b in _bombs) { b.Value.Fuse = Session.FuseAt(b.Key); b.Value.Tick(); }
                    break;
                case EventKind.JointCut:
                case EventKind.JointsReshuffled:
                    // tabs moved: every piece's outline may have changed
                    foreach (var p in _pieces.Values) p.Rebuild(Session.Board);
                    break;
                case EventKind.Finished:
                    if (e.Outcome == Outcome.Bomb)
                        foreach (var b in _bombs)
                            if (Session.FuseAt(b.Key) <= 0) b.Value.Explode();
                    break;
            }
        }

        void RefreshNails()
        {
            var gone = new List<int>();
            foreach (var n in _nails)
            {
                int c = Session.NailAt(n.Key);
                if (c > 0) { n.Value.Count = c; continue; }
                Destroy(n.Value.Root.gameObject);
                if (_pieces.TryGetValue(n.Key, out var pv)) pv.SetPinned(false);
                gone.Add(n.Key);
            }
            foreach (var k in gone) _nails.Remove(k);
        }
    }
}
