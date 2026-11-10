Shader "ReverseSolver/CorridorVictory" {
 Properties {
  [PerRendererData] _MainTex("Approved corridor",2D)="white"{}
  _Source("Original approved artwork",2D)="white"{}
  _Wall("Reconstructed wall",2D)="white"{}
  _Floor("Floor plane",2D)="white"{}
  _Frame("Painting and frame",2D)="black"{}
  _Columns("Foreground columns",2D)="black"{}
  _Arches("Source validity: wall R, floor G",2D)="black"{}
  _Picture("Current level painting",2D)="white"{}
  _Color("Tint",Color)=(1,1,1,1)
 }
 SubShader {
  Tags { "Queue"="Transparent" "RenderType"="Transparent" "IgnoreProjector"="True" }
  Cull Off Lighting Off ZWrite Off
  Blend SrcAlpha OneMinusSrcAlpha
  Pass {
   CGPROGRAM
   #pragma vertex vert
   #pragma fragment frag
   #pragma target 3.0
   #include "UnityCG.cginc"
   sampler2D _MainTex,_Source,_Wall,_Floor,_Frame,_Columns,_Arches,_Picture;
   float _Progress,_HasPicture;
   float3 _CameraOffset;
   float2 _PictureSize;
   fixed4 _Color;
   struct appdata {float4 vertex:POSITION;float2 uv:TEXCOORD0;fixed4 color:COLOR;};
   struct v2f {float4 position:SV_POSITION;float2 uv:TEXCOORD0;fixed4 color:COLOR;};
   v2f vert(appdata v){v2f o;o.position=UnityObjectToClipPos(v.vertex);o.uv=v.uv;o.color=v.color*_Color;return o;}
   #include "CorridorProjection.cginc"
   fixed4 frag(v2f i):SV_Target{return Corridor(i.uv)*i.color;}
   ENDCG
  }
 }
}
