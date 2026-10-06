using UnityEngine;

namespace ReverseSolver.Presentation
{
    /* World units are points (CSS px in the web game). The camera maps the
       screen so that (0, 0) is the top-left corner and y grows downward in
       layout code; Draw converts layout (x, y-down) to world (x, -y). */
    public static class Draw
    {
        public static Vector3 W(float x, float y, float z = 0) => new Vector3(x, -y, z);
        public static Vector3 W(Vector2 p, float z = 0) => new Vector3(p.x, -p.y, z);

        /* "#rrggbb" or "#rrggbbaa" as an sRGB vector; shaders convert to linear. */
        public static Vector4 Hex(string hex, float alpha = 1f)
        {
            ColorUtility.TryParseHtmlString(hex, out var c);
            return new Vector4(c.r, c.g, c.b, c.a * alpha);
        }

        public static Vector4 Rgba(float r, float g, float b, float a) => new Vector4(r / 255f, g / 255f, b / 255f, a);

        /* A quad covering size + pad on each side, centred on the transform.
           uv0 is the position in points relative to the centre, y down. */
        public static Mesh CenteredQuad(Vector2 size, float pad)
        {
            float hx = size.x * 0.5f + pad, hy = size.y * 0.5f + pad;
            var m = new Mesh { name = "quad" };
            m.vertices = new[] { new Vector3(-hx, hy), new Vector3(hx, hy), new Vector3(hx, -hy), new Vector3(-hx, -hy) };
            m.uv = new[] { new Vector2(-hx, -hy), new Vector2(hx, -hy), new Vector2(hx, hy), new Vector2(-hx, hy) };
            m.triangles = new[] { 0, 1, 2, 0, 2, 3 };
            m.bounds = new Bounds(Vector3.zero, new Vector3(hx * 2, hy * 2, 1));
            return m;
        }

        public static T Child<T>(Transform parent, string name) where T : Component
        {
            var go = new GameObject(name);
            go.transform.SetParent(parent, false);
            return go.AddComponent<T>();
        }

        public static void Order(Renderer r, int order)
        {
            r.sortingOrder = order;
        }
    }

    /* Shaders and materials, found once. The shaders must be referenced by a
       material in the build: GameRoot keeps the three materials serialized. */
    public static class Materials
    {
        public static Material Piece, Shape, Solid, Background;

        public static void Init(Material piece, Material shape, Material solid, Material background)
        {
            Piece = piece; Shape = shape; Solid = solid; Background = background;
        }
    }
}
