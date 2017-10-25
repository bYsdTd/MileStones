Shader "Custom/BattleActor"
{
	Properties
	{
		_MainTex ("Base (RGB), Alpha (A)", 2D) = "black" {}
		_Color ("Color", Color) = (1, 1, 1, 1)
	}
	
	SubShader
	{
		LOD 200

		Tags
		{
			"DisableBatching" = "True"
			"RenderType" = "Opaque"
		}
		
		Pass
		{
			Cull Off
			Lighting Off
			ZWrite On
			Fog { Mode Off }
			Offset -1, -1
			Blend SrcAlpha OneMinusSrcAlpha

			CGPROGRAM
			#pragma target 3.0
			#pragma vertex vert
			#pragma fragment frag			
			#include "UnityCG.cginc"

			sampler2D _MainTex;
			float4 _MainTex_ST;
			float _SrcHue;
			float _DstHue;
			float  _Fuzz;
			float4 _Color;
	
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
				//o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);


				o.vertex = mul(UNITY_MATRIX_P, 
								mul(UNITY_MATRIX_MV, float4(0.0, 0.0, 0.0, 1.0)) 
								+ float4(v.vertex.x, v.vertex.y, 0.0, 0.0));

				o.texcoord = v.texcoord;
				o.color = v.color;
				return o;
			}
			
			float3 rgb2hsv(float3 c)
			{
				float4 K = float4(0.0, -1.0 / 3.0, 2.0 / 3.0, -1.0);
				float k1 = step(c.b, c.g);
				float4 p = lerp(float4(c.bg, K.wz), float4(c.gb, K.xy), k1);
				float k2 = step(p.x, c.r);
				float4 q = lerp(float4(p.xyw, c.r), float4(c.r, p.yzx), k2);
				
				float d = q.x - min(q.w, q.y);
				float e = 1.0e-10;
				return float3(abs(q.z + (q.w - q.y) / (6.0 * d + e)), d / (q.x + e), q.x);
			}

			float3 hsv2rgb(float3 c)
			{
				float4 K = float4(1.0, 2.0 / 3.0, 1.0 / 3.0, 3.0);
				float3 p = abs(frac(c.xxx + K.xyz) * 6.0 - K.www);
				return c.z * lerp(K.xxx, clamp(p - K.xxx, 0.0, 1.0), c.y);
			}
			
			float ReplaceColor(float col, float src, float dst, float fuz){
				float ch = col - src;
				if (abs(ch) < fuz){
					return fmod(col - src + dst + 360.0, 360.0);
				}
				else{
					return col;
				}
			}
						
			fixed4 frag (v2f IN) : COLOR
			{
//				float3 rgbArg = IN.color.rgb;
//				fixed4 colIn = IN.color;
//				colIn.rgb = float3(1.0, 1.0, 1.0);
				fixed4 col = tex2D(_MainTex, IN.texcoord);

				clip(col.a - 0.6f);

//				float3 rgbTex = col.rgb;
//				float3 hsv = rgb2hsv(rgbTex);
//				hsv.r = hsv.r * 360.0;
//				hsv.r = ReplaceColor(hsv.r, rgbArg.r * 255.0, rgbArg.g * 255.0, (1 - rgbArg.b) * 255.0);
//				hsv.r = hsv.r * 0.0027777778;
//				rgbTex = hsv2rgb(hsv);
//				col.rgb = rgbTex;
				return col * _Color;
			}
			ENDCG
		}
	}
}
