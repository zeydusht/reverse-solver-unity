// Rounded rectangles, circles and capsules with the few CSS effects the web
// game uses: vertical or radial gradient, 45-degree stripes (sealed walls),
// stroke, outer drop shadow, inner shadow and a soft glow.
//
// uv0 = position in points relative to the shape's centre (y down). The quad
// is the shape's size plus _Pad on every side so shadows and glows fit.
// Colours arrive as vectors in sRGB and are converted once at the end.
Shader "ReverseSolver/Shape"
{
    Properties
    {
        _Size ("Size (pt)", Vector) = (100, 100, 0, 0)
        _Radius ("Corner radius (pt)", Float) = 8
        _Mode ("0 linear, 1 radial, 2 stripes, 3 ring arc", Float) = 0
        _ColorA ("Top / centre", Vector) = (1, 1, 1, 1)
        _ColorB ("Bottom / middle", Vector) = (0.5, 0.5, 0.5, 1)
        _ColorC ("Radial outer", Vector) = (0, 0, 0, 1)
        _Radial ("Radial centre xy (0..1), radii zw (0..1)", Vector) = (0.32, 0.26, 0.6, 0.5)
        _Stroke ("Stroke colour", Vector) = (0, 0, 0, 0)
        _StrokeWidth ("Stroke width (pt)", Float) = 0
        _Shadow ("Drop shadow colour", Vector) = (0, 0, 0, 0)
        _ShadowParams ("Shadow offset x, y, blur (pt)", Vector) = (0, 2, 4, 0)
        _Inner ("Inner shadow colour", Vector) = (0, 0, 0, 0)
        _InnerParams ("Inner offset y, blur (pt)", Vector) = (2, 3, 0, 0)
        _Glow ("Glow colour", Vector) = (0, 0, 0, 0)
        _GlowSize ("Glow size (pt)", Float) = 0
        _Arc ("Ring arc: gap start, gap end (radians)", Vector) = (0, 0, 0, 0)
        _Alpha ("Opacity", Float) = 1
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" "RenderPipeline"="UniversalPipeline" "IgnoreProjector"="True" }
        Blend One OneMinusSrcAlpha
        ZWrite Off
        Cull Off

        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"

            // Colours are authored in sRGB. In a Gamma project (ours, so blending
            // matches the browser's CSS) they go out as they are.
            float3 ToOutput(float3 c)
            {
            #ifdef UNITY_COLORSPACE_GAMMA
                return c;
            #else
                return SRGBToLinear(c);
            #endif
            }

            CBUFFER_START(UnityPerMaterial)
                float4 _Size, _ColorA, _ColorB, _ColorC, _Radial, _Stroke, _Shadow, _ShadowParams;
                float4 _Inner, _InnerParams, _Glow, _Arc;
                float _Radius, _Mode, _StrokeWidth, _GlowSize, _Alpha;
            CBUFFER_END

            struct A { float4 pos : POSITION; float2 uv : TEXCOORD0; };
            struct V { float4 pos : SV_POSITION; float2 p : TEXCOORD0; };

            V vert(A i) { V o; o.pos = TransformObjectToHClip(i.pos.xyz); o.p = i.uv; return o; }

            float sdRound(float2 p, float2 half, float r)
            {
                r = min(r, min(half.x, half.y));
                float2 d = abs(p) - half + r;
                return length(max(d, 0.0)) + min(max(d.x, d.y), 0.0) - r;
            }

            float4 over(float4 top, float4 bottom) { return top + bottom * (1.0 - top.a); }
            float4 pm(float4 c, float a) { return float4(c.rgb, 1) * (c.a * a); }

            float4 frag(V i) : SV_Target
            {
                float2 p = i.p;
                float aa = max(length(fwidth(p)) * 0.75, 1e-3);
                float2 half = _Size.xy * 0.5;
                float d = sdRound(p, half, _Radius);
                float inside = saturate(0.5 - d / aa);

                // fill
                float4 fillC = _ColorA;
                if (_Mode < 0.5)
                {
                    fillC = lerp(_ColorA, _ColorB, saturate(p.y / _Size.y + 0.5));
                }
                else if (_Mode < 1.5)
                {
                    float2 uv = p / _Size.xy + 0.5;
                    float t = length((uv - _Radial.xy) / max(_Radial.zw, 1e-3));
                    fillC = t < 0.6 ? lerp(_ColorA, _ColorB, t / 0.6) : lerp(_ColorB, _ColorC, saturate((t - 0.6) / 0.4));
                }
                else if (_Mode < 2.5)
                {
                    float s = frac(((p.x - p.y) / 1.41421356) / 18.0);     // 45deg, 9pt bands
                    fillC = s < 0.5 ? _ColorA : _ColorB;
                }
                else
                {
                    // ring arc: a stroke-only circle with an angular gap
                    float ang = atan2(p.y, p.x);
                    bool gap = _Arc.x != _Arc.y && ang > _Arc.x && ang < _Arc.y;
                    float ring = abs(length(p) - half.x + _StrokeWidth * 0.5) - _StrokeWidth * 0.5;
                    float a = gap ? 0 : saturate(0.5 - ring / aa);
                    float4 rc = pm(_ColorA, a * _Alpha);
                    rc.rgb = ToOutput(rc.rgb / max(rc.a, 1e-4)) * rc.a;
                    return rc;
                }

                float4 col = pm(fillC, inside);

                // inner shadow: a darker band along the top edge (CSS inset 0 2px 3px)
                if (_Inner.a > 0)
                {
                    float dIn = sdRound(p - float2(0, _InnerParams.x), half, _Radius);
                    float band = saturate((dIn + _InnerParams.y) / max(_InnerParams.y, 1e-3));
                    col = over(pm(_Inner, band * inside), col);
                }

                // stroke on the outline
                if (_StrokeWidth > 0)
                {
                    float sd = abs(d + _StrokeWidth * 0.5) - _StrokeWidth * 0.5;
                    col = over(pm(_Stroke, saturate(0.5 - sd / aa)), col);
                }

                // glow and drop shadow underneath
                float4 under = float4(0, 0, 0, 0);
                if (_Shadow.a > 0)
                {
                    float ds = sdRound(p - _ShadowParams.xy, half, _Radius);
                    float b = max(_ShadowParams.z, 1e-3);
                    under = pm(_Shadow, 1.0 - smoothstep(-b * 0.5, b, ds));
                }
                if (_GlowSize > 0 && _Glow.a > 0)
                    under = over(pm(_Glow, 1.0 - smoothstep(0, _GlowSize, d)), under);
                col = over(col, under);

                col *= saturate(_Alpha);
                col.rgb = ToOutput(col.rgb / max(col.a, 1e-4)) * col.a;
                return col;
            }
            ENDHLSL
        }
    }
}
