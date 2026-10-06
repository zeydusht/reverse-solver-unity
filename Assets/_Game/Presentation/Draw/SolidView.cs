using System.Collections.Generic;
using UnityEngine;

namespace ReverseSolver.Presentation
{
    /* Flat polygons with ReverseSolver/Solid: the star of the results card and
       small arrow heads. Points are in local layout space (y down). */
    [RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
    public sealed class SolidView : MonoBehaviour
    {
        static readonly int AlphaId = Shader.PropertyToID("_Alpha");
        MeshRenderer _r;
        MaterialPropertyBlock _b;
        Mesh _mesh;

        public static SolidView Polygon(Transform parent, string name, IList<Vector2> points, Vector4 color, int order)
        {
            var s = Draw.Child<SolidView>(parent, name);
            s._r = s.GetComponent<MeshRenderer>();
            s._r.sharedMaterial = Materials.Solid;
            s._r.sortingOrder = order;
            s._b = new MaterialPropertyBlock();
            s.SetPolygon(points, color);
            s.Alpha(1);
            return s;
        }

        /* Fan triangulation: fine for convex shapes and for stars when the first point is the centre. */
        public void SetPolygon(IList<Vector2> pts, Vector4 color)
        {
            if (_mesh == null) { _mesh = new Mesh { name = "poly" }; GetComponent<MeshFilter>().sharedMesh = _mesh; }
            var v = new Vector3[pts.Count];
            var c = new Color[pts.Count];
            for (int i = 0; i < pts.Count; i++) { v[i] = Draw.W(pts[i]); c[i] = new Color(color.x, color.y, color.z, color.w); }
            var t = new List<int>();
            for (int i = 1; i < pts.Count - 1; i++) { t.Add(0); t.Add(i); t.Add(i + 1); }
            _mesh.Clear();
            _mesh.vertices = v;
            _mesh.colors = c;
            _mesh.triangles = t.ToArray();
            _mesh.RecalculateBounds();
        }

        /* Five-pointed star centred at the origin; first vertex is the centre for the fan. */
        public static List<Vector2> Star(float outer, float inner)
        {
            var p = new List<Vector2> { Vector2.zero };
            for (int i = 0; i <= 10; i++)
            {
                float a = -Mathf.PI / 2 + i * Mathf.PI / 5;
                float r = i % 2 == 0 ? outer : inner;
                p.Add(new Vector2(Mathf.Cos(a) * r, Mathf.Sin(a) * r));
            }
            return p;
        }

        public void Alpha(float a) { _b.SetFloat(AlphaId, a); _r.SetPropertyBlock(_b); }
        public int SortingOrder { get => _r.sortingOrder; set => _r.sortingOrder = value; }

        void OnDestroy() { if (_mesh != null) Destroy(_mesh); }
    }
}
