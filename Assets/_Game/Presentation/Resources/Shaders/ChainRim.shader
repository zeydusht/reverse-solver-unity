// PM_116 (Zeyd: the teal chain on a dark picture): the sprite as it is, with a thin light rim round its shape (the
// sprite's own cream edge carried a few texels further out). The sprite's colours are not touched.
Shader "ReverseSolver/ChainRim"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite", 2D) = "white" {}
        _RimColor ("Rim colour (sRGB)", Color) = (1, 0.957, 0.855, 0.95)
        _RimPx ("Rim width (texels)", Float) = 4
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" "RenderPipeline"="UniversalPipeline" }
        Blend One OneMinusSrcAlpha
        ZWrite Off
        Cull Off
        Pass
        {
            Tags { "LightMode"="Universal2D" }
            HLSLPROGRAM
            #pragma target 3.0
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            TEXTURE2D(_MainTex); SAMPLER(sampler_MainTex);
            float4 _MainTex_TexelSize;
            CBUFFER_START(UnityPerMaterial)
                float4 _RimColor;
                float _RimPx;
            CBUFFER_END

            struct Attributes { float4 position : POSITION; float2 uv : TEXCOORD0; half4 color : COLOR; };
            struct Varyings { float4 position : SV_POSITION; float2 uv : TEXCOORD0; half4 color : COLOR; };
            Varyings vert(Attributes input)
            {
                Varyings output;
                output.position = TransformObjectToHClip(input.position.xyz);
                output.uv = input.uv;
                output.color = input.color;
                return output;
            }

            half4 frag(Varyings input) : SV_Target
            {
                half4 c = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, input.uv) * input.color;
                // the shape's alpha nearby: 12 directions at the rim's width and at half of it
                float near = 0;
                float2 px = _MainTex_TexelSize.xy * _RimPx;
                [unroll] for (int k = 0; k < 12; k++)
                {
                    float a = k * 0.5235988;
                    float2 d = float2(cos(a), sin(a)) * px;
                    near = max(near, SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, input.uv + d).a);
                    near = max(near, SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, input.uv + d * 0.5).a);
                }
                float rim = saturate(near * 1.4) * _RimColor.a * input.color.a;
                // the sprite over its rim (premultiplied)
                float3 rgb = c.rgb * c.a + _RimColor.rgb * rim * (1 - c.a);
                float alpha = c.a + rim * (1 - c.a);
                return half4(rgb, alpha);
            }
            ENDHLSL
        }
    }
}
