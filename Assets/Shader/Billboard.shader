Shader "Billboard" 
{
   Properties 
   {
      _MainTex ("Texture Image", 2D) = "white" {}
   }

   SubShader 
   {
		//Tags{ "DisableBatching" = "True" }

      	Pass 
		{   
			Cull Off

			CGPROGRAM

			#pragma vertex vert  
			#pragma fragment frag 

			// User-specified uniforms            
			uniform sampler2D _MainTex;

			struct vertexInput 
			{
				float4 vertex : POSITION;
				float4 tex : TEXCOORD0;
			};

			struct vertexOutput 
			{
				float4 pos : SV_POSITION;
				float4 tex : TEXCOORD0;
			};

			vertexOutput vert(vertexInput input) 
			{
				vertexOutput output;


// 计算billboard的一种方法，view space 
//				output.pos = mul(UNITY_MATRIX_P, 
//				  mul(UNITY_MATRIX_MV, float4(0.0, 0.0, 0.0, 1.0))
//				  + float4(input.vertex.x, input.vertex.y, 0.0, 0.0));
// ----------


//				float3 lookat = _WorldSpaceCameraPos - input.vertex.xyz;
//				float3 right = cross((0, 1, 0), lookat);
//				float3 up = cross(lookat, right);

				output.pos = mul(UNITY_MATRIX_MVP, input.vertex);

				output.tex = input.tex;

				return output;
			}

			float4 frag(vertexOutput input) : COLOR
			{
				float4 col = tex2D(_MainTex, float2(input.tex.xy));   

				if(col.a < 0.5)
				{
					discard;
				}

				return col;
			}

		 	ENDCG
		}
   }
}