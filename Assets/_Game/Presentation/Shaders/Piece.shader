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
//              +1 tab, -1 socket, 0 flat; uv2 = the cell's (x, y) on the board;
//              color = base colour (sRGB).
// Per piece (MaterialPropertyBlock): see Properties.
//
// Level image (PRODUCT.md G1): with _ImageMix > 0 the fill and the dark rim
// come from _Image at the point's place on the whole board, so tabs carry on
// the neighbour's picture like a real jigsaw. The white rim stays, the
// gradient and highlight are kept but softer, and contrast is eased a little,
// so tabs and sockets still read on any picture.
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
        _Rim ("Outer rim colour (sRGB) and amount", Vector) = (1, 1, 1, 0.84)
        _SkinTex ("Codex piece body texture (tileable)", 2D) = "white" {}
        _SkinMix ("Body texture amount (0..1)", Float) = 0
        _KitOn ("Draw with the Codex piece kit (0/1)", Float) = 0
        _KitMask ("Kit masks, 9x9 atlas (alpha)", 2D) = "white" {}
        _KitOver ("Kit overlays, 9x9 atlas", 2D) = "black" {}
        _KitFlat ("Kit flat pieces, 9x9 atlas", 2D) = "white" {}
        _Image ("Board image (sRGB)", 2D) = "white" {}
        _BoardCells ("Board size in cells (w, h)", Vector) = (1, 1, 0, 0)
        _ImageMix ("Image amount (0..1)", Float) = 0
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
                float _PtToUnit, _Lift, _Gray, _Glow, _Flash, _Alpha, _ImageMix;
                float4 _GlowColor, _BoardCells, _Image_ST, _Rim;
                float _SkinMix, _KitOn;
            CBUFFER_END
            TEXTURE2D(_KitMask); SAMPLER(sampler_KitMask);
            TEXTURE2D(_KitOver); SAMPLER(sampler_KitOver);
            TEXTURE2D(_KitFlat); SAMPLER(sampler_KitFlat);
            TEXTURE2D(_SkinTex);
            SAMPLER(sampler_SkinTex);
            TEXTURE2D(_Image);
            SAMPLER(sampler_Image);

            struct A { float4 pos : POSITION; float2 uv : TEXCOORD0; float4 edges : TEXCOORD1; float2 cell : TEXCOORD2; float4 color : COLOR; };
            struct V { float4 pos : SV_POSITION; float2 p : TEXCOORD0; nointerpolation float4 edges : TEXCOORD1; nointerpolation float4 color : TEXCOORD2; nointerpolation float2 cell : TEXCOORD3; };

            V vert(A i)
            {
                V o;
                o.pos = TransformObjectToHClip(i.pos.xyz);
                o.p = i.uv; o.edges = i.edges; o.color = i.color; o.cell = i.cell;
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

            /* Codex kit (Assets/Art/Pieces/Kit81): tile of piece_TOP-RIGHT-BOTTOM-LEFT in a 9x9 atlas,
               index = a(U)*27 + a(R)*9 + a(D)*3 + a(L) with O(tab)=0, F=1, I(notch)=2, row 0 at the top.
               Tile: 640 px canvas, the 384 px cell centred = cell units -33.3..133.3. */
            float2 kitUV(float2 q, float4 e)
            {
                float4 a = e > 0.5 ? 0.0 : (e < -0.5 ? 2.0 : 1.0);
                float idx = a.x * 27.0 + a.y * 9.0 + a.z * 3.0 + a.w;
                float2 t = saturate((q + 33.3333) / 166.6667);
                float col = fmod(idx, 9.0), row = floor(idx / 9.0);
                return (float2(col, 8.0 - row) + float2(t.x, 1.0 - t.y) * 0.996 + 0.002) / 9.0;
            }

            // 1 inside the kit tile, 0 outside (the quad's pad is wider than the tile: no edge-pixel smear)
            float kitIn(float2 q)
            {
                float2 t = (q + 33.3333) / 166.6667;
                return step(0.0, t.x) * step(t.x, 1.0) * step(0.0, t.y) * step(t.y, 1.0);
            }

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
                float3 darkC = shade(baseC, -0.5);

                // level image: this point's place on the whole board (y up in the texture)
                float imageAmt = saturate(_ImageMix);
                if (imageAmt > 0.001)
                {
                    float2 bp = (i.cell * 100.0 + p) / (max(_BoardCells.xy, 1.0) * 100.0);
                    float3 img = SAMPLE_TEXTURE2D(_Image, sampler_Image, saturate(float2(bp.x, 1.0 - bp.y))).rgb;
                    float il = dot(img, float3(0.299, 0.587, 0.114));
                    img = lerp(img, float3(il, il, il), 0.55 * pinned) * lerp(1.0, 0.82, pinned);
                    img = lerp(img, float3(0.5, 0.5, 0.5), 0.12);                                   // ease contrast so the cut reads
                    float3 imgFill = lerp(shade(img, 0.12), shade(img, -0.10), t);    // the gradient, softer
                    imgFill = imgFill + (1.0 - imgFill) * hA * 0.55;                  // the highlight, softer
                    fill = lerp(fill, imgFill, imageAmt);
                    darkC = lerp(darkC, shade(img, -0.55), imageAmt);
                }
                // Codex skin body (skin.json "body"): the tileable texture, one tile per cell, under the picture
                float3 skinTex = SAMPLE_TEXTURE2D(_SkinTex, sampler_SkinTex, i.p).rgb;
                fill = lerp(fill, fill * skinTex * 1.12, _SkinMix);
                fill = fill + (1.0 - fill) * saturate(_Flash) * 0.9;

                float cWhite = saturate(0.5 - dWhite / aa);
                float cDark  = saturate(0.5 - dDark / aa);
                float cFill  = saturate(0.5 - dFill / aa);

                float4 col; float shA, gA;
                float offY = lerp(2.0, 12.0, lift) * _PtToUnit, blur = lerp(4.0, 18.0, lift) * _PtToUnit, gR = 9.0 * _PtToUnit;
                if (_KitOn > 0.5)
                {
                    float2 kuv = kitUV(q, i.edges);
                    float kin = kitIn(q);
                    float m = SAMPLE_TEXTURE2D(_KitMask, sampler_KitMask, kuv).a * kin;
                    float4 ov = SAMPLE_TEXTURE2D(_KitOver, sampler_KitOver, kuv) * kin;
                    if (imageAmt > 0.001)
                        col = over(float4(ov.rgb * ov.a, ov.a) * m, float4(fill, 1) * m);   // painting clipped by the mask, overlay on top
                    else
                    {
                        float4 fl = SAMPLE_TEXTURE2D(_KitFlat, sampler_KitFlat, kuv) * kin;
                        float fl_l = dot(fl.rgb, float3(0.299, 0.587, 0.114));
                        fl.rgb = lerp(fl.rgb, fl_l.xxx, 0.55 * pinned) * lerp(1.0, 0.82, pinned);
                        fl.rgb = fl.rgb + (1.0 - fl.rgb) * saturate(_Flash) * 0.9;
                        col = float4(fl.rgb * fl.a, fl.a);
                    }
                    float ms = SAMPLE_TEXTURE2D(_KitMask, sampler_KitMask, kitUV(q - float2(0, offY), i.edges)).a * kitIn(q - float2(0, offY));
                    shA = lerp(0.42, 0.55, lift) * ms;
                    float mg = 0.0;
                    for (int k = 0; k < 4; k++)
                    {
                        float2 o = float2(k == 0 ? gR : k == 1 ? -gR : 0.0, k == 2 ? gR : k == 3 ? -gR : 0.0);
                        mg = max(mg, SAMPLE_TEXTURE2D(_KitMask, sampler_KitMask, kitUV(q + o, i.edges)).a * kitIn(q + o));
                    }
                    gA = saturate(_Glow) * mg * (1.0 - m * 0.6);
                }
                else
                {
                    col = float4(_Rim.rgb, 1) * (_Rim.a * cWhite);   // white rim; Codex skin: dark outline
                    col = over(float4(darkC, 1) * cDark, col);
                    col = over(float4(fill, 1) * cFill, col);

                    // drop shadow: 0 2px 4px .42 at rest, 0 12px 18px .55 when lifted (CSS px = points)
                    offY = lerp(2.0, 12.0, lift) * _PtToUnit;
                    blur = lerp(4.0, 18.0, lift) * _PtToUnit;
                    float dSh = sdScaled(q - float2(0, offY), i.edges, 1.088);
                    shA = lerp(0.42, 0.55, lift) * (1.0 - smoothstep(-blur * 0.5, blur, dSh));

                    // glow (blocker coral, popped amber): drop-shadow(0 0 7px colour)
                    gR = 9.0 * _PtToUnit;
                    gA = saturate(_Glow) * (1.0 - smoothstep(0.0, gR, dWhite)) * (1.0 - cWhite * 0.6);
                }
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
