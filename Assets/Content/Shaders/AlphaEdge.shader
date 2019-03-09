Shader "Custom/AlphaEdge"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_EdgeColor("EdgeColor", Color) = (0,0,0,1)
		_EdgeSize("EdgeSize", Range(0,.01)) = 0.001
	}
	SubShader
	{
		// No culling or depth
		Cull Off ZWrite Off ZTest Always

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
			};

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				return o;
			}
			
			sampler2D _MainTex;
			fixed4 _EdgeColor;
			half _EdgeSize;

			fixed4 frag (v2f i) : SV_Target
			{
				half spread = _EdgeSize;

				fixed4 col = tex2D(_MainTex, i.uv);
				
				float diff = 0;

				float left = tex2D(_MainTex, i.uv - fixed2(spread, 0)).a;
				float right = tex2D(_MainTex, i.uv + fixed2(spread, 0)).a;
				float up = tex2D(_MainTex, i.uv - fixed2(0, spread)).a;
				float down = tex2D(_MainTex, i.uv + fixed2(0, spread)).a;
				
				diff += abs(col.a - left);
				diff += abs(col.a - right);
				diff += abs(col.a - up);
				diff += abs(col.a - down);

				if (diff > 0) {
					col.rgb = _EdgeColor;
				}

				// just invert the colors
				col.rgb -= diff;
				// col.rgb = 1 - col.rgb;
				return col;
			}
			ENDCG
		}
	}
}
