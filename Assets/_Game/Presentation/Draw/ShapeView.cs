using UnityEngine;

namespace ReverseSolver.Presentation
{
    /* One shape drawn with ReverseSolver/Shape. Fluent setters write into a
       MaterialPropertyBlock; call Apply() (or any setter, which applies) after
       changing values. Colours are sRGB vectors (see Draw.Hex). */
    [RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
    public sealed class ShapeView : MonoBehaviour
    {
        static readonly int SizeId = Shader.PropertyToID("_Size"), RadiusId = Shader.PropertyToID("_Radius"),
            ModeId = Shader.PropertyToID("_Mode"), AId = Shader.PropertyToID("_ColorA"), BId = Shader.PropertyToID("_ColorB"),
            CId = Shader.PropertyToID("_ColorC"), RadialId = Shader.PropertyToID("_Radial"),
            StrokeId = Shader.PropertyToID("_Stroke"), StrokeWId = Shader.PropertyToID("_StrokeWidth"),
            ShadowId = Shader.PropertyToID("_Shadow"), ShadowPId = Shader.PropertyToID("_ShadowParams"),
            InnerId = Shader.PropertyToID("_Inner"), InnerPId = Shader.PropertyToID("_InnerParams"),
            GlowId = Shader.PropertyToID("_Glow"), GlowSizeId = Shader.PropertyToID("_GlowSize"),
            ArcId = Shader.PropertyToID("_Arc"), AlphaId = Shader.PropertyToID("_Alpha");

        MeshRenderer _r;
        MaterialPropertyBlock _b;
        Mesh _mesh;
        public Vector2 Size { get; private set; }

        public static ShapeView Create(Transform parent, string name, Vector2 size, float pad, int order)
        {
            var s = Draw.Child<ShapeView>(parent, name);
            s.Init(size, pad, order);
            return s;
        }

        void Init(Vector2 size, float pad, int order)
        {
            _r = GetComponent<MeshRenderer>();
            _r.sharedMaterial = Materials.Shape;
            _r.sortingOrder = order;
            _b = new MaterialPropertyBlock();
            Resize(size, pad);
            Alpha(1);
        }

        public ShapeView Resize(Vector2 size, float pad)
        {
            Size = size;
            if (_mesh != null) Destroy(_mesh);
            _mesh = Draw.CenteredQuad(size, pad);
            GetComponent<MeshFilter>().sharedMesh = _mesh;
            _b.SetVector(SizeId, size);
            return Apply();
        }

        public ShapeView Radius(float r) { _b.SetFloat(RadiusId, r); return Apply(); }
        public ShapeView Fill(Vector4 c) => Gradient(c, c);
        public ShapeView Gradient(Vector4 top, Vector4 bottom) { _b.SetFloat(ModeId, 0); _b.SetVector(AId, top); _b.SetVector(BId, bottom); return Apply(); }

        /* CSS radial-gradient(rx% ry% at cx% cy%, a, b 60%, c) */
        public ShapeView Radial(Vector4 center, Vector4 mid, Vector4 outer, Vector4 posAndRadii)
        {
            _b.SetFloat(ModeId, 1); _b.SetVector(AId, center); _b.SetVector(BId, mid); _b.SetVector(CId, outer);
            _b.SetVector(RadialId, posAndRadii);
            return Apply();
        }

        public ShapeView Stripes(Vector4 a, Vector4 b) { _b.SetFloat(ModeId, 2); _b.SetVector(AId, a); _b.SetVector(BId, b); return Apply(); }

        /* A circle outline (diameter = Size.x) with an optional angular gap (radians, atan2 with y down). */
        public ShapeView Ring(Vector4 c, float width, float gapFrom = 0, float gapTo = 0)
        {
            _b.SetFloat(ModeId, 3); _b.SetVector(AId, c); _b.SetFloat(StrokeWId, width);
            _b.SetVector(ArcId, new Vector4(gapFrom, gapTo));
            return Apply();
        }

        public ShapeView Stroke(Vector4 c, float width) { _b.SetVector(StrokeId, c); _b.SetFloat(StrokeWId, width); return Apply(); }
        public ShapeView Shadow(Vector4 c, float dx, float dy, float blur) { _b.SetVector(ShadowId, c); _b.SetVector(ShadowPId, new Vector4(dx, dy, blur)); return Apply(); }
        public ShapeView Inner(Vector4 c, float dy, float blur) { _b.SetVector(InnerId, c); _b.SetVector(InnerPId, new Vector4(dy, blur)); return Apply(); }
        public ShapeView Glow(Vector4 c, float size) { _b.SetVector(GlowId, c); _b.SetFloat(GlowSizeId, size); return Apply(); }
        public ShapeView Alpha(float a) { _b.SetFloat(AlphaId, a); return Apply(); }

        public ShapeView Apply() { _r.SetPropertyBlock(_b); return this; }

        public int SortingOrder { get => _r.sortingOrder; set => _r.sortingOrder = value; }

        void OnDestroy() { if (_mesh != null) Destroy(_mesh); }
    }
}
