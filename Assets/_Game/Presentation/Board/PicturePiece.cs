using System.Collections.Generic;
using UnityEngine;

namespace ReverseSolver.Presentation
{
    /* G1-b: the finished picture on the result card ("Resmi tamamladın"),
       drawn as one big flat-edged jigsaw piece with the piece shader in its
       picture mode, so it has the same white rim and soft light as the
       pieces the player just cleared. No new shader or material. */
    [RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
    public sealed class PicturePiece : MonoBehaviour
    {
        const float Pad = 12f;     // cell units around the piece: rims and shadow

        Mesh _mesh;

        /* size: points on screen (width, height); centred on the parent's origin. */
        public static PicturePiece Create(Transform parent, Texture picture, Vector2 size, int order)
        {
            var v = Draw.Child<PicturePiece>(parent, "Picture");
            var r = v.GetComponent<MeshRenderer>();
            r.sharedMaterial = Materials.Piece;
            r.sortingOrder = order;
            float u0 = -Pad, u1 = 100 + Pad;
            var verts = new List<Vector3>();
            var uv = new List<Vector2>();
            foreach (var (u, w) in new[] { (u0, u0), (u1, u0), (u1, u1), (u0, u1) })
            {
                verts.Add(Draw.W((u / 100f - .5f) * size.x, (w / 100f - .5f) * size.y));
                uv.Add(new Vector2(u, w));
            }
            v._mesh = new Mesh { name = "picture" };
            v._mesh.SetVertices(verts);
            v._mesh.SetUVs(0, uv);
            v._mesh.SetUVs(1, new List<Vector4> { Vector4.zero, Vector4.zero, Vector4.zero, Vector4.zero });   // all edges flat
            v._mesh.SetUVs(2, new List<Vector2> { Vector2.zero, Vector2.zero, Vector2.zero, Vector2.zero });   // the one cell (0, 0)
            v._mesh.SetColors(new List<Color> { Color.gray, Color.gray, Color.gray, Color.gray });
            v._mesh.SetTriangles(new[] { 0, 1, 2, 0, 2, 3 }, 0);
            v._mesh.RecalculateBounds();
            v.GetComponent<MeshFilter>().sharedMesh = v._mesh;
            var b = new MaterialPropertyBlock();
            b.SetFloat("_PtToUnit", 100f / Mathf.Min(size.x, size.y));
            b.SetFloat("_Alpha", 1f);
            b.SetVector("_BoardCells", new Vector4(1, 1, 0, 0));
            b.SetTexture("_Image", picture);
            b.SetFloat("_ImageMix", 1f);
            r.SetPropertyBlock(b);
            return v;
        }

        void OnDestroy() { if (_mesh != null) Destroy(_mesh); }
    }
}
