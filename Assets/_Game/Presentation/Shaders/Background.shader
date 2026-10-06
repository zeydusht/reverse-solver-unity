// The web game's page background (index.html body + .orbs + .vig):
//   linear-gradient(178deg, #1c565c 0%, #123c42 46%, #0a2226 100%)
//   three blurred orbs (blur 58px, container opacity .5)
//   radial vignette 120% x 85% at 50% 32%, transparent 45% -> rgba(4,16,18,.72)
// uv0 = 0..1 across the screen; _Screen = screen size in points.
Shader "ReverseSolver/Background"
{
    Properties { _Screen ("Screen size (pt)", Vector) = (390, 844, 0, 0) }
    SubShader
    {
        Tags { "RenderType"="Opaque" "Queue"="Geometry" "RenderPipeline"="UniversalPipeline" }
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
                float4 _Screen;
            CBUFFER_END

            struct A { float4 pos : POSITION; float2 uv : TEXCOORD0; };
            struct V { float4 pos : SV_POSITION; float2 uv : TEXCOORD0; };
            V vert(A i) { V o; o.pos = TransformObjectToHClip(i.pos.xyz); o.uv = i.uv; return o; }

            float3 hex(int c) { return float3((c >> 16) & 255, (c >> 8) & 255, c & 255) / 255.0; }

            // A blurred disc: diameter dia, centre c (pt), CSS blur(58px) ~ gaussian sigma 58.
            float orb(float2 p, float2 c, float dia)
            {
                float d = length(p - c) - dia * 0.5;
                return 1.0 - smoothstep(-58.0, 58.0, d);
            }

            float4 frag(V i) : SV_Target
            {
                float2 sz = _Screen.xy;
                float2 p = float2(i.uv.x, 1.0 - i.uv.y) * sz;     // pt, y down
                float vw = sz.x / 100.0, vh = sz.y / 100.0;

                float t = p.y / sz.y;                              // 178deg is ~vertical
                float3 c = t < 0.46 ? lerp(hex(0x1c565c), hex(0x123c42), t / 0.46)
                                    : lerp(hex(0x123c42), hex(0x0a2226), (t - 0.46) / 0.54);

                float a1 = 0.5 * 0.34 * orb(p, float2(-16 * vw + 28 * vw, -12 * vw + 28 * vw), 56 * vw);
                float a2 = 0.5 * 0.30 * orb(p, float2(sz.x + 18 * vw - 26 * vw, 22 * vh + 26 * vw), 52 * vw);
                float a3 = 0.5 * 0.26 * orb(p, float2(-8 * vw + 30 * vw, sz.y + 18 * vh - 30 * vw), 60 * vw);
                c = lerp(c, hex(0xe0564a), a1);
                c = lerp(c, hex(0x5f9fc9), a2);
                c = lerp(c, hex(0x9179c2), a3);

                float2 vq = (p - float2(0.5 * sz.x, 0.32 * sz.y)) / (float2(1.2, 0.85) * sz * 0.5);
                float vt = saturate((length(vq) - 0.45) / 0.55);
                c = lerp(c, hex(0x041012), 0.72 * vt);

                return float4(ToOutput(c), 1);
            }
            ENDHLSL
        }
    }
}
