Shader "ReverseSolver/Veil"
{
    Properties
    {
        _Color ("Veil colour", Color) = (0, 0, 0, 0.6)
        _Feather ("Edge feather (pt)", Float) = 7
        _Count ("Hole count", Int) = 0
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" "RenderPipeline"="UniversalPipeline" "IgnoreProjector"="True" }
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

            CBUFFER_START(UnityPerMaterial)
                float4 _Color;
                float4 _Shape[16];
                float4 _ShapeP[16];
                float _Feather;
                int _Count;
            CBUFFER_END

            struct A { float4 position : POSITION; };
            struct V { float4 position : SV_POSITION; float2 world : TEXCOORD0; };
            V vert(A i)
            {
                V o;
                float3 world = TransformObjectToWorld(i.position.xyz);
                o.position = TransformWorldToHClip(world);
                o.world = world.xy;
                return o;
            }

            float4 frag(V i) : SV_Target
            {
                float d = 1e10;
                for (int k = 0; k < min(_Count, 16); k++)
                {
                    float2 p = i.world - _Shape[k].xy;
                    float c = _ShapeP[k].y, s = _ShapeP[k].z;
                    p = float2(c * p.x + s * p.y, -s * p.x + c * p.y);
                    float2 halfSize = _Shape[k].zw;
                    float radius = min(_ShapeP[k].x, min(halfSize.x, halfSize.y));
                    float2 q = abs(p) - halfSize + radius;
                    float sd = length(max(q, 0.0)) + min(max(q.x, q.y), 0.0) - radius;
                    d = min(d, sd);
                }
                float alpha = _Color.a * smoothstep(-_Feather / 2, _Feather / 2, d);
                return float4(_Color.rgb, alpha);
            }
            ENDHLSL
        }
    }
}
