Shader "ReverseSolver/JigoPortrait"
{
    Properties
    {
        [PerRendererData] _MainTex ("Portrait", 2D) = "white" {}
        _ClosedEyes ("Closed eyes", 2D) = "white" {}
        _Window ("Window center and half size", Vector) = (0, 0, 1, 1)
        _Blink ("Blink closure", Range(0, 1)) = 0
        _Opacity ("Opacity", Range(0, 1)) = 1
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" "RenderPipeline"="UniversalPipeline" }
        Blend SrcAlpha OneMinusSrcAlpha
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
            TEXTURE2D(_ClosedEyes); SAMPLER(sampler_ClosedEyes);
            CBUFFER_START(UnityPerMaterial)
                float4 _Window;
                float _Blink;
                float _Opacity;
            CBUFFER_END

            struct Attributes { float4 position : POSITION; float2 uv : TEXCOORD0; };
            struct Varyings { float4 position : SV_POSITION; float2 uv : TEXCOORD0; float2 world : TEXCOORD1; };
            Varyings vert(Attributes input)
            {
                Varyings output;
                float3 world = TransformObjectToWorld(input.position.xyz);
                output.position = TransformWorldToHClip(world);
                output.world = world.xy;
                output.uv = input.uv;
                return output;
            }

            float EyeArea(float2 uv, float x)
            {
                float2 d = abs(uv - float2(x, .399)) / float2(.093, .091);
                return 1 - smoothstep(.87, 1, max(d.x, d.y));
            }

            half4 frag(Varyings input) : SV_Target
            {
                float2 p = (input.world - _Window.xy) / _Window.zw;
                float x = abs(p.x) / .851;
                float arch = .02 + .825 * sqrt(saturate(1 - x * x));
                float aperture = min(.851 - abs(p.x), min(p.y + .802, arch - p.y));
                half4 colour = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, input.uv);
                if (_Blink > 0)
                {
                    float2 uv = input.uv;
                    float eyes = max(EyeArea(uv, .389), EyeArea(uv, .611));
                    float lid = smoothstep(-.012, .012, uv.y - lerp(.51, .287, _Blink));
                    half3 closed = SAMPLE_TEXTURE2D(_ClosedEyes, sampler_ClosedEyes, uv).rgb;
                    colour.rgb = lerp(colour.rgb, closed, eyes * lid);
                }
                colour.a *= _Opacity * smoothstep(0, .019, aperture);
                return colour;
            }
            ENDHLSL
        }
    }
}
