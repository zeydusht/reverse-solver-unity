// Flat vertex-coloured triangles (stars, arrow heads). Vertex colours are sRGB.
Shader "ReverseSolver/Solid"
{
    Properties { _Alpha ("Opacity", Float) = 1 }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" "RenderPipeline"="UniversalPipeline" }
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
                float _Alpha;
            CBUFFER_END
            struct A { float4 pos : POSITION; float4 color : COLOR; };
            struct V { float4 pos : SV_POSITION; float4 color : COLOR; };
            V vert(A i) { V o; o.pos = TransformObjectToHClip(i.pos.xyz); o.color = i.color; return o; }
            float4 frag(V i) : SV_Target
            {
                float a = i.color.a * saturate(_Alpha);
                return float4(ToOutput(i.color.rgb) * a, a);
            }
            ENDHLSL
        }
    }
}
