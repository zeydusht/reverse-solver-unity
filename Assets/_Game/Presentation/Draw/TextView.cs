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
        static TMP_FontAsset _fredoka;

        /* Another font (counter digits, PM_MESAJ_12); the weight is in the font itself. */
        public TextView Font(TMP_FontAsset font)
        {
            if (font == null) return this;
            _t.font = font;
            _t.fontStyle = FontStyles.Normal;
            return this;
        }

        /* A soft drop shadow under the glyphs (TMP underlay), offset in points downwards. */
        public TextView Shadow(Vector4 color, float down)
        {
            var m = _t.fontMaterial;
            m.EnableKeyword("UNDERLAY_ON");
            m.SetColor("_UnderlayColor", new Color(color.x, color.y, color.z, color.w));
            m.SetFloat("_UnderlayOffsetY", -down * .4f);
            m.SetFloat("_UnderlaySoftness", .15f);
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

        public static readonly Vector4 OnGreen = Draw.Hex("#1E4D05"), OnTeal = Draw.Hex("#0B2A36");
        static TMP_FontAsset _title;
        public static TMP_FontAsset TitleFont => _title != null ? _title : (_title = Resources.Load<TMP_FontAsset>("Fonts/Title"));

        static TMP_FontAsset _heading;
        public static TMP_FontAsset HeadingFont => _heading != null ? _heading : (_heading = Resources.Load<TMP_FontAsset>("Fonts/Baloo"));
        public static readonly Vector4 RimGreen = Draw.Hex("#385018"), RimTeal = Draw.Hex("#0B3E47"), RimRed = Draw.Hex("#5A1414");

        /* PM_MESAJ_42: card titles and buttons, one style everywhere (font_demo_PM.png, row 3): Baloo 2 ExtraBold,
           #FCFADE -> #F0E6A6 down the glyphs, a thin rim in the button's dark tone (~6 % of the height) and a hard
           shadow of the same colour ~9 % below. */
        public TextView Heading(Vector4 rim)
        {
            if (HeadingFont == null) return Banner(rim);
            Font(HeadingFont);
            Color = Vector4.one;
            _t.enableVertexGradient = true;
            Color32 top = new Color32(0xFC, 0xFA, 0xDE, 255), bottom = new Color32(0xF0, 0xE6, 0xA6, 255);
            _t.colorGradient = new VertexGradient(top, top, bottom, bottom);
            // PM_76: one shared material per look (a material per text made each panel's first frame slow)
            Shared("heading", rim, .24f, rim, -.55f);
            return this;
        }

        /* PM_76: the heading looks as shared materials, keyed by font and the look's values. */
        static readonly System.Collections.Generic.Dictionary<string, Material> _looks = new System.Collections.Generic.Dictionary<string, Material>();
        void Shared(string look, Vector4 rim, float outline, Vector4 under, float underY)
        {
            string C(Vector4 c) => $"{Mathf.RoundToInt(c.x * 255)},{Mathf.RoundToInt(c.y * 255)},{Mathf.RoundToInt(c.z * 255)},{Mathf.RoundToInt(c.w * 255)}";
            string key = $"{look}|{_t.font.name}|{C(rim)}|{outline}|{C(under)}|{underY}";
            if (!_looks.TryGetValue(key, out var m) || m == null)
            {
                m = new Material(_t.font.material) { name = "TMP " + look };
                m.SetFloat(ShaderUtilities.ID_FaceDilate, .2f);   // the rim straddles the edge: keep the face full
                m.SetFloat(ShaderUtilities.ID_OutlineWidth, outline);
                m.SetColor(ShaderUtilities.ID_OutlineColor, new Color(rim.x, rim.y, rim.z, rim.w));
                m.EnableKeyword("UNDERLAY_ON");
                m.SetColor("_UnderlayColor", new Color(under.x, under.y, under.z, 1));
                m.SetFloat("_UnderlayOffsetY", underY);
                m.SetFloat("_UnderlayDilate", .44f);
                m.SetFloat("_UnderlaySoftness", 0f);
                _looks[key] = m;
            }
            _t.fontSharedMaterial = m;
            _t.UpdateMeshPadding();
        }

        /* Tutorial instruction over the veil: Baloo heading in the p21/p22 game (PM_65), the old banner elsewhere. */
        public TextView Instruction() => Art.P21Gameplay ? Heading(RimTeal) : Banner(OnTeal);

        /* PM_65: approved title style without a ribbon: cream fill, blue rim, a gold "thickness" below. */
        public TextView Title3D()
        {
            Heading(Draw.Hex("#234670"));
            if (HeadingFont == null) return this;
            // on a cream panel the blue rim carries the letters; a gold "thickness" below (shared, PM_76)
            Shared("title3d", Draw.Hex("#234670"), .34f, new Vector4(.78f, .55f, .16f, 1), -.7f);
            return this;
        }

        /* PM_MESAJ_27 A3: menu banners in Nunito Black, cream-to-yellow fill, thick dark outline, soft shadow below. */
        public TextView Banner(Vector4 outline)
        {
            Font(TitleFont);
            Color = Vector4.one;
            _t.enableVertexGradient = true;
            Color32 top = new Color32(255, 255, 255, 255), bottom = new Color32(255, 241, 176, 255);
            _t.colorGradient = new VertexGradient(top, top, bottom, bottom);
            Outline(outline, .3f);
            var m = _t.fontMaterial;
            m.SetFloat(ShaderUtilities.ID_FaceDilate, .28f);   // the outline straddles the edge: grow the face so it stays full
            m.EnableKeyword("UNDERLAY_ON");
            m.SetColor("_UnderlayColor", new Color(0, 0, 0, .5f));
            m.SetFloat("_UnderlayOffsetY", -.6f);
            m.SetFloat("_UnderlayDilate", .3f);
            m.SetFloat("_UnderlaySoftness", .1f);
            _t.UpdateMeshPadding();
            return this;
        }

        public static TextView Create(Transform parent, string name, string text, float sizePt, Vector4 color,
                                      bool bold = true, TextAlignmentOptions align = TextAlignmentOptions.Center, int order = 0)
        {
            var v = Draw.Child<TextView>(parent, name);
            v._t = v.GetComponent<TextMeshPro>();
            // preview art: Turkish texts in Fredoka (PM_MESAJ_15); digits / English labels set Lilita themselves
            if (Art.On) { _fredoka ??= Resources.Load<TMP_FontAsset>("Fonts/Fredoka"); if (_fredoka != null) v._t.font = _fredoka; }
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

        /* Fit profile names without wrapping, then shorten at the minimum size. */
        public TextView FitWidth(float widthPt, float minSizePt)
        {
            // PM_76: one proportional step first (width follows the size), then the 1 pt steps only for the last bit
            float w0 = Width;
            if (w0 > widthPt && w0 > 0)
                _t.fontSize = Mathf.Max(minSizePt * PointToFontSize, Mathf.Floor(_t.fontSize * widthPt / w0 / PointToFontSize) * PointToFontSize);
            while (Width > widthPt && _t.fontSize > minSizePt * PointToFontSize)
                _t.fontSize = Mathf.Max(minSizePt * PointToFontSize, _t.fontSize - PointToFontSize);
            if (Width <= widthPt) return this;
            string full = Text;
            var starts = System.Globalization.StringInfo.ParseCombiningCharacters(full);
            for (int i = starts.Length - 1; i >= 0; i--)
            {
                Text = full.Substring(0, starts[i]) + "…";
                if (Width <= widthPt) break;
            }
            return this;
        }
    }
}
