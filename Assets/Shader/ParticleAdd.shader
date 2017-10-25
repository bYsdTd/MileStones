Shader "Custom/ParticleAdd"
{
	Properties
	{
		_MainTex ("Base (RGB), Alpha (A)", 2D) = "black" {}
		_TintColor ("Tint Color", Color) = (1,1,1,1)
	}
	
	SubShader
	{
		LOD 200

		Tags
		{
			"Queue" = "Transparent"
			"IgnoreProjector" = "True"
			"RenderType" = "Transparent"
		}
		
		Pass
		{
			Cull Off
			Lighting Off
			ZWrite Off
			Fog { Mode Off }
			Offset -1, -1
			Blend SrcAlpha One

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag			
			#include "UnityCG.cginc"

			sampler2D _MainTex;
			float4 _MainTex_ST;
			float4 _TintColor;

			struct appdata_t
			{
				float4 vertex : POSITION;
				float2 texcoord : TEXCOORD0;
				fixed4 color : COLOR;
			};
	
			struct v2f
			{
				float4 vertex : SV_POSITION;
				half2 texcoord : TEXCOORD0;
				fixed4 color : COLOR;
			};
	
			v2f vert (appdata_t v)
			{
				v2f o;
				o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
				o.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);
				o.color = v.color;
				return o;
			}
				
			fixed4 frag (v2f IN) : COLOR
			{
				fixed4 col = tex2D(_MainTex, IN.texcoord) * IN.color * _TintColor;
				return col;
			}
			ENDCG
		}
	}
}
