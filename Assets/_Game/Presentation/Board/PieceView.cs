using System.Collections.Generic;
using ReverseSolver.Core;
using UnityEngine;

namespace ReverseSolver.Presentation
{
    /* One piece: all its cells in one mesh drawn with ReverseSolver/Piece, plus
       its chain, nail and bomb as children so they move together. The
       transform's local position is the drag offset; the cells sit at their
       board positions inside it. */
    [RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
    public sealed class PieceView : MonoBehaviour
    {
        const float Pad = 40f;     // cell units around a cell: tabs (26), rims, shadow
        static readonly int RimId = Shader.PropertyToID("_Rim");
        static readonly int LiftId = Shader.PropertyToID("_Lift"), GrayId = Shader.PropertyToID("_Gray"),
            GlowId = Shader.PropertyToID("_Glow"), GlowColorId = Shader.PropertyToID("_GlowColor"),
            FlashId = Shader.PropertyToID("_Flash"), AlphaId = Shader.PropertyToID("_Alpha"),
            PtId = Shader.PropertyToID("_PtToUnit"), ImageId = Shader.PropertyToID("_Image"),
            BoardCellsId = Shader.PropertyToID("_BoardCells"), ImageSTId = Shader.PropertyToID("_Image_ST"), ImageMixId = Shader.PropertyToID("_ImageMix");

        public int Index { get; private set; }
        public Cell[] Cells { get; private set; }

        MeshRenderer _r;
        MaterialPropertyBlock _b;
        Mesh _mesh;
        float _cell;
        float _aspect = 1;   // PM_89: the board's width / height (the picture's crop)
        public float Lifted { get; private set; }

        public static PieceView Create(Transform parent, Board board, int index, float cell, int order)
        {
            var v = Draw.Child<PieceView>(parent, $"Piece {index}");
            v.Index = index;
            v.Cells = board.Level.Pieces[index];
            v._cell = cell;
            v._r = v.GetComponent<MeshRenderer>();
            v._r.sharedMaterial = Materials.Piece;
            v._r.sortingOrder = order;
            v._b = new MaterialPropertyBlock();
            v._b.SetFloat(PtId, 100f / cell);
            v._b.SetFloat(AlphaId, 1f);
            // Codex piece skin (Assets/Art/PieceSkin/skin.json "outline" #123F46): a dark, clear outline in the preview
            v._b.SetVector(RimId, Art.On ? Draw.Hex("#123F46") : new Vector4(1, 1, 1, .84f));
            var skin = Art.Texture("skin-body");                       // Codex piece body (PieceSkin/body-256.png)
            if (skin != null) { v._b.SetTexture("_SkinTex", skin); v._b.SetFloat("_SkinMix", .35f); }
            // Codex piece kit (Kit81 via KitAtlas): shapes from the kit; the procedural drawing stays the fallback
            var kitMask = Art.Texture("kit-mask");
            if (kitMask != null)
            {
                v._b.SetTexture("_KitMask", kitMask);
                v._b.SetTexture("_KitOver", Art.Texture("kit-overlay"));
                v._b.SetTexture("_KitFlat", Art.Texture("kit-flat"));
                v._b.SetFloat("_KitOn", 1);
                v._b.SetFloat("_SkinMix", 0);
            }
            v._b.SetVector(BoardCellsId, new Vector4(board.Level.Width, board.Level.Height, 0, 0));
            v._aspect = (float)board.Level.Width / Mathf.Max(1, board.Level.Height);
            v.Rebuild(board);
            return v;
        }

        /* Edges depend on the joints, which boosters change (M4): rebuild then. */
        public void Rebuild(Board board)
        {
            var l = board.Level;
            var verts = new List<Vector3>();
            var uv = new List<Vector2>();
            var edges = new List<Vector4>();
            var colors = new List<Color>();
            var cells = new List<Vector2>();
            var tris = new List<int>();
            foreach (var c in Cells)
            {
                var e = new Vector4(TravelRule.EdgeOf(board, c.X, c.Y, Dir.U), TravelRule.EdgeOf(board, c.X, c.Y, Dir.R),
                                    TravelRule.EdgeOf(board, c.X, c.Y, Dir.D), TravelRule.EdgeOf(board, c.X, c.Y, Dir.L));
                var col = CellColor(l, c);
                int b = verts.Count;
                float u0 = -Pad, u1 = 100 + Pad;
                foreach (var (u, w) in new[] { (u0, u0), (u1, u0), (u1, u1), (u0, u1) })
                {
                    verts.Add(Draw.W(c.X * _cell + u * _cell / 100f, c.Y * _cell + w * _cell / 100f));
                    uv.Add(new Vector2(u, w));
                    edges.Add(e);
                    colors.Add(col);
                    cells.Add(new Vector2(c.X, c.Y));
                }
                tris.AddRange(new[] { b, b + 1, b + 2, b, b + 2, b + 3 });
            }
            if (_mesh == null) { _mesh = new Mesh { name = "piece" }; GetComponent<MeshFilter>().sharedMesh = _mesh; }
            _mesh.Clear();
            _mesh.SetVertices(verts);
            _mesh.SetUVs(0, uv);
            _mesh.SetUVs(1, edges);
            _mesh.SetUVs(2, cells);
            _mesh.SetColors(colors);
            _mesh.SetTriangles(tris, 0);
            _mesh.RecalculateBounds();
            _home = null; _shapedNow = false;
            Apply();
        }

        /* PM_105: a chained pair is one rigid piece whose two cells sit far apart. Scaling its transform (the rubber
           knock's squash and stretch, about the pair's bounding box) moved the far cell by the squash times the span:
           it jumped and flew. Here each cell deforms about its own point instead (pivot: from the cell's centre, in
           points; weight per cell 0..1 of the deformation), the transform only slides. (1, 1) puts the mesh back. */
        Vector3[] _home, _shaped;
        bool _shapedNow;
        public void SetCellScale(Vector2 scale, Vector2 pivot, float[] weight = null)
        {
            if (_mesh == null) return;
            if (_home == null || _home.Length != _mesh.vertexCount) { _home = _mesh.vertices; _shaped = new Vector3[_home.Length]; }
            bool identity = scale == Vector2.one;
            if (identity && !_shapedNow) return;
            for (int i = 0; i < Cells.Length && i * 4 + 3 < _home.Length; i++)
            {
                var s = Vector2.LerpUnclamped(Vector2.one, scale, weight == null || i >= weight.Length ? 1 : weight[i]);
                var p = Draw.W(new Vector2((Cells[i].X + .5f) * _cell, (Cells[i].Y + .5f) * _cell) + pivot);
                for (int k = 0; k < 4; k++)
                {
                    var v = _home[i * 4 + k];
                    _shaped[i * 4 + k] = new Vector3(p.x + (v.x - p.x) * s.x, p.y + (v.y - p.y) * s.y, v.z);
                }
            }
            _mesh.vertices = identity ? _home : _shaped;
            _shapedNow = !identity;
        }
        public void ResetShape() { if (_shapedNow) SetCellScale(Vector2.one, Vector2.zero); }

        /* The drawn body of cell i (its square, without tabs) where it is now, in the board's layout points (y down):
           the mesh as shaped, then the transform. */
        public Rect CellBody(int i)
        {
            var v = _mesh.vertices;
            float f0 = Pad / (100 + 2 * Pad), f1 = (100 + Pad) / (100 + 2 * Pad);
            Vector3 a = Vector3.Lerp(v[i * 4], v[i * 4 + 2], f0), c = Vector3.Lerp(v[i * 4], v[i * 4 + 2], f1);
            a = Vector3.Scale(a, transform.localScale) + transform.localPosition;
            c = Vector3.Scale(c, transform.localScale) + transform.localPosition;
            return Rect.MinMaxRect(Mathf.Min(a.x, c.x), Mathf.Min(-a.y, -c.y), Mathf.Max(a.x, c.x), Mathf.Max(-a.y, -c.y));
        }

        /* web colorOf: palette[colors[y * w + x]] */
        public static Color CellColor(LevelData l, Cell c)
        {
            string hex = "#888888";
            int i = c.Y * l.Width + c.X;
            if (l.Colors.Length > i && l.Palette.Length > 0)
                hex = l.Palette[Mathf.Clamp(l.Colors[i], 0, l.Palette.Length - 1)];
            ColorUtility.TryParseHtmlString(hex, out var col);
            return col;
        }

        /* Centre of the first cell in board layout space, where badges sit. */
        public Vector2 FirstCellCenter => new Vector2((Cells[0].X + .5f) * _cell, (Cells[0].Y + .5f) * _cell);

        public void SetOffset(Vector2 offset) => transform.localPosition = Draw.W(offset);

        /* .pc.deal frame: scale about the first cell's centre, lifted dy points. */
        public void SetDeal(float scale, float dy)
        {
            transform.localScale = new Vector3(scale, scale, 1);
            transform.localPosition = Draw.W(FirstCellCenter * (1 - scale) + new Vector2(0, dy));
        }
        public Vector2 Offset => new Vector2(transform.localPosition.x, -transform.localPosition.y);

        /* PM_84 hint: a pulse about the piece's own centre (1 = at rest, in place). */
        public void SetPulse(float scale)
        {
            float x0 = float.MaxValue, y0 = float.MaxValue, x1 = float.MinValue, y1 = float.MinValue;
            foreach (var c in Cells) { x0 = Mathf.Min(x0, c.X); y0 = Mathf.Min(y0, c.Y); x1 = Mathf.Max(x1, c.X + 1); y1 = Mathf.Max(y1, c.Y + 1); }
            var centre = new Vector2(x0 + x1, y0 + y1) * .5f * _cell;
            transform.localScale = new Vector3(scale, scale, 1);
            transform.localPosition = Draw.W(centre * (1 - scale));
        }

        public void SetLift(float t) { Lifted = t; _b.SetFloat(LiftId, t); Apply(); }
        public void SetPinned(bool on) { _b.SetFloat(GrayId, on ? 1 : 0); Apply(); }
        public void SetGlow(Vector4 color, float amount) { _b.SetVector(GlowColorId, color); _b.SetFloat(GlowId, amount); Apply(); }
        public void SetFlash(float t) { _b.SetFloat(FlashId, t); Apply(); }

        /* G1: the level's picture, mix 0 (colours) .. 1 (picture). */
        public void SetImage(Texture image, float mix)
        {
            if (image != null) { _b.SetTexture(ImageId, image); _b.SetVector(ImageSTId, LevelImages.CropST(image, _aspect)); }
            _b.SetFloat(ImageMixId, image != null ? mix : 0);
            Apply();
        }
        public void SetAlpha(float a)
        {
            _b.SetFloat(AlphaId, a); Apply();
            foreach (var s in GetComponentsInChildren<ShapeView>()) s.Alpha(a);
            foreach (var t in GetComponentsInChildren<TextView>()) t.Alpha = a;
        }

        /* PM_73 (ExitTrail ghost): a copy of the piece's body where it is now, same mesh and look, no badges or rope. */
        public MeshRenderer Ghost(Transform parent, int order, bool ownMesh = false)
        {
            var go = new GameObject("ghost");
            go.transform.SetParent(parent, false);
            go.transform.localPosition = transform.localPosition; go.transform.localScale = transform.localScale;
            var mesh = GetComponent<MeshFilter>().sharedMesh;
            // PM_76: a copy of the outline as it is now (the piece's own mesh is rebuilt in place right after)
            go.AddComponent<MeshFilter>().sharedMesh = ownMesh ? Object.Instantiate(mesh) : mesh;
            var r = go.AddComponent<MeshRenderer>();
            r.sharedMaterial = _r.sharedMaterial; r.sortingOrder = order;
            var b = new MaterialPropertyBlock(); _r.GetPropertyBlock(b); b.SetFloat(LiftId, 0); r.SetPropertyBlock(b);
            return r;
        }
        public static void SetGhostGlow(MeshRenderer r, Vector4 color, float amount)
        {
            _ghostBlock ??= new MaterialPropertyBlock();
            r.GetPropertyBlock(_ghostBlock); _ghostBlock.SetVector(GlowColorId, color); _ghostBlock.SetFloat(GlowId, amount); r.SetPropertyBlock(_ghostBlock);
        }
        static MaterialPropertyBlock _ghostBlock;
        /* PM_84 (the dive's cuts): when each side of each cell starts to be drawn (s on the drawing's clock), in the
           mesh, read by the piece shader while SetReveal is on (the dive draws the cut in, PM_86). */
        public void SetSeamStarts(System.Func<Cell, Dir, float> start)
        {
            if (_mesh == null) return;
            var list = new List<Vector4>();
            foreach (var c in Cells)
            {
                var v = new Vector4(start(c, Dir.U), start(c, Dir.R), start(c, Dir.D), start(c, Dir.L));
                for (int k = 0; k < 4; k++) list.Add(v);
            }
            _mesh.SetUVs(3, list);
        }
        static readonly int H0Id = Shader.PropertyToID("_ImageH0"), H1Id = Shader.PropertyToID("_ImageH1"),
            H2Id = Shader.PropertyToID("_ImageH2"), WarpId = Shader.PropertyToID("_ImageWarp");
        /* PM_90 §3 (the dive's pull back): the picture through h (3 x 3, row-major: board uv, v up -> picture uv);
           null: the board's own crop again. */
        public void SetImageWarp(double[] h)
        {
            if (h == null) _b.SetFloat(WarpId, 0);
            else
            {
                _b.SetVector(H0Id, new Vector4((float)h[0], (float)h[1], (float)h[2], 0));
                _b.SetVector(H1Id, new Vector4((float)h[3], (float)h[4], (float)h[5], 0));
                _b.SetVector(H2Id, new Vector4((float)h[6], (float)h[7], (float)h[8], 0));
                _b.SetFloat(WarpId, 1);
            }
            Apply();
        }
        static readonly int RvOnId = Shader.PropertyToID("_RvOn"), RvTimeId = Shader.PropertyToID("_RvTime"),
            RvEdgeId = Shader.PropertyToID("_RvEdgeSec"), RvLookId = Shader.PropertyToID("_RvLook");
        /* PM_86 dive: the piece as the plain picture with its cut drawn in (time on the drawing's clock, SetSeamStarts),
           taking its own look with look 0..1; off: as always. */
        public void SetReveal(bool on, float time, float edgeSeconds, float look)
        {
            _b.SetFloat(RvOnId, on ? 1 : 0); _b.SetFloat(RvTimeId, time); _b.SetFloat(RvEdgeId, edgeSeconds); _b.SetFloat(RvLookId, look);
            Apply();
        }
        public static void SetGhostAlpha(MeshRenderer r, float a)
        {
            _ghostBlock ??= new MaterialPropertyBlock();
            r.GetPropertyBlock(_ghostBlock); _ghostBlock.SetFloat(AlphaId, a); r.SetPropertyBlock(_ghostBlock);
        }

        public int SortingOrder
        {
            get => _r.sortingOrder;
            set
            {
                int delta = value - _r.sortingOrder;
                _r.sortingOrder = value;
                foreach (var s in GetComponentsInChildren<ShapeView>()) s.SortingOrder += delta;
                foreach (var t in GetComponentsInChildren<TextView>()) t.SortingOrder += delta;
                // Codex art on the piece (chain strip and end locks, obstacle art) rises with it while dragged
                foreach (var sr in GetComponentsInChildren<SpriteRenderer>()) sr.sortingOrder += delta;
            }
        }

        void Apply() => _r.SetPropertyBlock(_b);
        void OnDestroy() { if (_mesh != null) Destroy(_mesh); }
    }
}
