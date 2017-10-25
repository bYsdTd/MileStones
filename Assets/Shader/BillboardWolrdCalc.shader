Shader "Unlit/BillboardWolrdCalc"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
	}
	SubShader
	{
		Tags { "RenderType"="Opaque" }
		LOD 100

		Pass
		{
			Cull Off
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
				float2 vertexOffset : TEXCOORD1;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
			};

			sampler2D _MainTex;

			v2f vert (appdata v)
			{
				v2f o;

				float3 right = UNITY_MATRIX_IT_MV[0].xyz;
				float3 up = UNITY_MATRIX_IT_MV[1].xyz;

				v.vertex.xyz += v.vertexOffset.x * right + v.vertexOffset.y * up;
				o.vertex = mul(UNITY_MATRIX_VP, float4(v.vertex.xyz, 1.0));

				o.uv = v.uv;


				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				// sample the texture
				fixed4 col = tex2D(_MainTex, i.uv);
				return col;
			}
			ENDCG
		}
	}
}
