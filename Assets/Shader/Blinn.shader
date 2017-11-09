// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/Blinn" 
{
	Properties 
	{
		_MainTex ("Base (RGB) Gloss (A)", 2D) = "white" {}
	}

	SubShader 
	{
		Tags { "RenderType"="Opaque" "LightMode" = "ForwardBase" }

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"
			#include "Lighting.cginc"

			sampler2D _MainTex;

			struct v2f
			{
				float4 vertex : SV_POSITION;
				float2 uv : TEXCOORD0;
				fixed3 world_normal : TEXCOORD1;
				fixed3 world_pos : TEXCOORD2;
			};

			v2f vert (float4 vertex : POSITION, float3 normal : NORMAL, float2 uv : TEXCOORD0)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(vertex);
				o.uv = uv;
				o.world_normal = UnityObjectToWorldNormal(normal);
				o.world_pos = mul(unity_ObjectToWorld, vertex);

				return o;
			}
				
			fixed4 frag (v2f i) : SV_Target
			{
				float3 light_dir = normalize(UnityWorldSpaceLightDir(i.world_pos));
	
				float n_dot_l = max(0, dot(i.world_normal, light_dir));
	
				float4 diffuse_color = tex2D(_MainTex, i.uv);
				
				fixed4 c;

				c.rgb = diffuse_color.rgb * _LightColor0.rgb * n_dot_l;
				c.a = diffuse_color.a;

				return c;
			}
			ENDCG
		}

	}
}