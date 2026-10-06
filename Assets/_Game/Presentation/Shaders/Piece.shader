// One jigsaw cell, drawn like the web game's cellSVG (index.html):
//   white rim (shape scaled 1.088, 84% white), dark rim (scaled 1.032,
//   base darkened 50%), vertical gradient fill (base +44% light at the top,
//   -22% at the bottom), soft highlight at (30%, 18%) of the box, and the CSS
//   drop shadow under the whole thing.
//
// Geometry is a signed distance field in cell units: the cell is 0..100 on
// both axes (y down, like the SVG), tabs are circles of radius 18 centred 8
// units outside an edge, sockets the same circles 8 units inside.
//
// Per vertex:  uv0 = cell-unit position, uv1 = edge profile (U, R, D, L) with
//              +1 tab, -1 socket, 0 flat; color = base colour (sRGB).
// Per piece (MaterialPropertyBlock): see Properties.
Shader "ReverseSolver/Piece"
{
    Properties
    {
        _PtToUnit ("Cell units per point", Float) = 1.6
        _Lift ("Lifted while dragged (0..1)", Float) = 0
        _Gray ("Pinned look (0..1)", Float) = 0
        _GlowColor ("Glow colour (sRGB)", Color) = (0.91, 0.40, 0.30, 1)
        _Glow ("Glow amount (0..1)", Float) = 0
        _Flash ("Brightness flash (0..1)", Float) = 0
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
                float _PtToUnit, _Lift, _Gray, _Glow, _Flash, _Alpha;
                float4 _GlowColor;
            CBUFFER_END

            struct A { float4 pos : POSITION; float2 uv : TEXCOORD0; float4 edges : TEXCOORD1; float4 color : COLOR; };
            struct V { float4 pos : SV_POSITION; float2 p : TEXCOORD0; nointerpolation float4 edges : TEXCOORD1; nointerpolation float4 color : TEXCOORD2; };

            V vert(A i)
            {
                V o;
                o.pos = TransformObjectToHClip(i.pos.xyz);
                o.p = i.uv; o.edges = i.edges; o.color = i.color;
                return o;
            }

            static const float R = 18.0, OFF = 8.0;

            float sdBox(float2 p) { float2 d = abs(p - 50.0) - 50.0; return length(max(d, 0.0)) + min(max(d.x, d.y), 0.0); }

            float sdPiece(float2 p, float4 e)
            {
                float d = sdBox(p);
                float2 tabC[4]  = { float2(50, -OFF), float2(100 + OFF, 50), float2(50, 100 + OFF), float2(-OFF, 50) };
                float2 sockC[4] = { float2(50,  OFF), float2(100 - OFF, 50), float2(50, 100 - OFF), float2( OFF, 50) };
                float ev[4] = { e.x, e.y, e.z, e.w };
                float cut = 1e5;
                [unroll] for (int k = 0; k < 4; k++)
                {
                    if (ev[k] > 0.5)  d = min(d, length(p - tabC[k]) - R);
                    if (ev[k] < -0.5) cut = min(cut, length(p - sockC[k]) - R);
                }
                return max(d, -cut);
            }

            // Shape scaled by s about the cell centre, as an SVG scale() would.
            float sdScaled(float2 p, float4 e, float s) { return sdPiece((p - 50.0) / s + 50.0, e) * s; }

            float3 shade(float3 c, float amt) { return amt > 0 ? c + (1.0 - c) * amt : c * (1.0 + amt); }

            float4 over(float4 top, float4 bottom) { return top + bottom * (1.0 - top.a); }   // premultiplied

            float4 frag(V i) : SV_Target
            {
                float2 p = i.p;
                float aa = max(length(fwidth(p)) * 0.75, 1e-3);
                float lift = saturate(_Lift);

                // lifted pieces grow 5% around their centre (web .pc.lift ... scale(1.05))
                float2 q = (p - 50.0) / (1.0 + 0.05 * lift) + 50.0;

                float dWhite = sdScaled(q, i.edges, 1.088);
                float dDark  = sdScaled(q, i.edges, 1.032);
                float dFill  = sdPiece(q, i.edges);

                float3 baseC = i.color.rgb;
                float pinned = saturate(_Gray);
                float lum = dot(baseC, float3(0.299, 0.587, 0.114));
                baseC = lerp(baseC, lum.xxx, 0.55 * pinned) * lerp(1.0, 0.82, pinned);

                // fill: gradient over the -32..132 box of the SVG
                float t = saturate((q.y + 32.0) / 164.0);
                float3 fill = lerp(shade(baseC, 0.44), shade(baseC, -0.22), t);
                // highlight: radialGradient cx 30% cy 18% r 56%, white .46 -> .05 at .55 -> 0
                float hr = length(q - float2(-32.0 + 0.30 * 164.0, -32.0 + 0.18 * 164.0)) / (0.56 * 164.0);
                float hA = hr < 0.55 ? lerp(0.46, 0.05, hr / 0.55) : lerp(0.05, 0.0, saturate((hr - 0.55) / 0.45));
                fill = fill + (1.0 - fill) * hA;
                fill = fill + (1.0 - fill) * saturate(_Flash) * 0.9;

                float cWhite = saturate(0.5 - dWhite / aa);
                float cDark  = saturate(0.5 - dDark / aa);
                float cFill  = saturate(0.5 - dFill / aa);

                float4 col = float4(1, 1, 1, 1) * (0.84 * cWhite);
                col = over(float4(shade(baseC, -0.5), 1) * cDark, col);
                col = over(float4(fill, 1) * cFill, col);

                // drop shadow: 0 2px 4px .42 at rest, 0 12px 18px .55 when lifted (CSS px = points)
                float offY = lerp(2.0, 12.0, lift) * _PtToUnit;
                float blur = lerp(4.0, 18.0, lift) * _PtToUnit;
                float dSh = sdScaled(q - float2(0, offY), i.edges, 1.088);
                float shA = lerp(0.42, 0.55, lift) * (1.0 - smoothstep(-blur * 0.5, blur, dSh));

                // glow (blocker coral, popped amber): drop-shadow(0 0 7px colour)
                float gR = 9.0 * _PtToUnit;
                float gA = saturate(_Glow) * (1.0 - smoothstep(0.0, gR, dWhite)) * (1.0 - cWhite * 0.6);
                float4 under = over(float4(_GlowColor.rgb, 1) * gA, float4(0, 0, 0, 1) * shA);
                col = over(col, under);

                col *= saturate(_Alpha);
                col.rgb = ToOutput(col.rgb / max(col.a, 1e-4)) * col.a;   // colours are authored in sRGB
                return col;
            }
            ENDHLSL
        }
    }
}
