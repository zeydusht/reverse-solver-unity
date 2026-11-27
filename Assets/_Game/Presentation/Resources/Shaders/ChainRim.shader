// PM_116 (Zeyd: the teal chain on a dark picture): the sprite's shape in one light colour, drawn a little larger
// behind the sprite itself, so the chain gets a thin light rim. The chain's own sprite and colours are not touched.
Shader "ReverseSolver/ChainRim"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite", 2D) = "white" {}
        _RimColor ("Rim colour (sRGB)", Color) = (1, 0.957, 0.855, 0.92)
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
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            TEXTURE2D(_MainTex); SAMPLER(sampler_MainTex);
            CBUFFER_START(UnityPerMaterial)
                float4 _RimColor;
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
                float a = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, input.uv).a * _RimColor.a * input.color.a;
                return half4(_RimColor.rgb * a, a);   // premultiplied
            }
            ENDHLSL
        }
    }
}
