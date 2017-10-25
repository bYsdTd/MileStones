Shader "Unlit/BillboardClipCalc"
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
				float3 color : TEXCOORD1;
				float4 vertex : SV_POSITION;
			};

			sampler2D _MainTex;

			v2f vert (appdata v)
			{
				v2f o;

				o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
				o.vertex.xyz /= o.vertex.w;

				o.vertex.xy += v.vertexOffset.xy * float2(0.2, 0.05);
				o.vertex.w = 1;

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
