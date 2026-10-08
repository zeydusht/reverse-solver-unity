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
        static readonly int LiftId = Shader.PropertyToID("_Lift"), GrayId = Shader.PropertyToID("_Gray"),
            GlowId = Shader.PropertyToID("_Glow"), GlowColorId = Shader.PropertyToID("_GlowColor"),
            FlashId = Shader.PropertyToID("_Flash"), AlphaId = Shader.PropertyToID("_Alpha"),
            PtId = Shader.PropertyToID("_PtToUnit"), ImageId = Shader.PropertyToID("_Image"),
            BoardCellsId = Shader.PropertyToID("_BoardCells"), ImageMixId = Shader.PropertyToID("_ImageMix");

        public int Index { get; private set; }
        public Cell[] Cells { get; private set; }

        MeshRenderer _r;
        MaterialPropertyBlock _b;
        Mesh _mesh;
        float _cell;
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
            v._b.SetVector(BoardCellsId, new Vector4(board.Level.Width, board.Level.Height, 0, 0));
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
            Apply();
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
        public Vector2 Offset => new Vector2(transform.localPosition.x, -transform.localPosition.y);

        public void SetLift(float t) { Lifted = t; _b.SetFloat(LiftId, t); Apply(); }
        public void SetPinned(bool on) { _b.SetFloat(GrayId, on ? 1 : 0); Apply(); }
        public void SetGlow(Vector4 color, float amount) { _b.SetVector(GlowColorId, color); _b.SetFloat(GlowId, amount); Apply(); }
        public void SetFlash(float t) { _b.SetFloat(FlashId, t); Apply(); }

        /* G1: the level's picture, mix 0 (colours) .. 1 (picture). */
        public void SetImage(Texture image, float mix)
        {
            if (image != null) _b.SetTexture(ImageId, image);
            _b.SetFloat(ImageMixId, image != null ? mix : 0);
            Apply();
        }
        public void SetAlpha(float a)
        {
            _b.SetFloat(AlphaId, a); Apply();
            foreach (var s in GetComponentsInChildren<ShapeView>()) s.Alpha(a);
            foreach (var t in GetComponentsInChildren<TextView>()) t.Alpha = a;
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
            }
        }

        void Apply() => _r.SetPropertyBlock(_b);
        void OnDestroy() { if (_mesh != null) Destroy(_mesh); }
    }
}
