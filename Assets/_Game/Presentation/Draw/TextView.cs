using TMPro;
using UnityEngine;

namespace ReverseSolver.Presentation
{
    /* World-space TextMeshPro sized in points, matching the web game's CSS
       font sizes. Text is centred on the transform unless aligned otherwise. */
    [RequireComponent(typeof(TextMeshPro))]
    public sealed class TextView : MonoBehaviour
    {
        /* TextMeshPro (3D) draws fontSize 10 one world unit tall; our unit is a point. */
        public const float PointToFontSize = 10f;

        TextMeshPro _t;

        /* Another font (counter digits, PM_MESAJ_12); the weight is in the font itself. */
        public TextView Font(TMP_FontAsset font)
        {
            if (font == null) return this;
            _t.font = font;
            _t.fontStyle = FontStyles.Normal;
            return this;
        }

        /* A dark outline around the glyphs (counters on obstacle art, PM_MESAJ_11). */
        public TextView Outline(Vector4 color, float width)
        {
            _t.outlineWidth = width;
            _t.outlineColor = new Color(color.x, color.y, color.z, color.w);
            return this;
        }
        Vector4 _color;

        public static TextView Create(Transform parent, string name, string text, float sizePt, Vector4 color,
                                      bool bold = true, TextAlignmentOptions align = TextAlignmentOptions.Center, int order = 0)
        {
            var v = Draw.Child<TextView>(parent, name);
            v._t = v.GetComponent<TextMeshPro>();
            v._t.textWrappingMode = TextWrappingModes.NoWrap;
            v._t.overflowMode = TextOverflowModes.Overflow;
            v._t.alignment = align;
            v._t.fontStyle = bold ? FontStyles.Bold : FontStyles.Normal;
            v._t.fontSize = sizePt * PointToFontSize;
            v._t.rectTransform.sizeDelta = new Vector2(10, 2);
            v.SortingOrder = order;
            v._color = color;
            v.Alpha = 1;
            v.Text = text;
            return v;
        }

        public string Text { get => _t.text; set { if (_t.text != value) _t.text = value; } }

        public Vector4 Color { get => _color; set { _color = value; Alpha = _alphaMul; } }

        float _alphaMul = 1;
        public float Alpha
        {
            get => _alphaMul;
            set
            {
                _alphaMul = value;
                // TMP colours are sRGB-authored like everything else here
                _t.color = new Color(_color.x, _color.y, _color.z, _color.w * value);
            }
        }

        public float SizePt { set => _t.fontSize = value * PointToFontSize; }

        public int SortingOrder
        {
            get => _t.sortingOrder;
            set => _t.sortingOrder = value;
        }

        /* Width of the laid-out text in points. */
        public float Width => _t.GetPreferredValues().x;
    }
}
