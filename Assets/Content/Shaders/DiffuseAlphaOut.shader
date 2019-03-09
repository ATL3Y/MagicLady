Shader "Custom/DiffuseAlphaOut"
{
	Properties
	{
		_MainTex("Texture", 2D) = "white" {}
		_Alpha("AlphaOut", Range(0,1)) = 0.5
	}
	
	SubShader
	{
		Tags{ "LightMode" = "ForwardBase" "Queue" = "Geometry" }
		LOD 100

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc" // for UnityObjectToWorldNormal
			#include "UnityLightingCommon.cginc" // for _LightColor0

			sampler2D _MainTex;
			float4 _MainTex_ST;
			half _Alpha;

			struct appdata
			{
				float4 vertex : POSITION;
				float3 normal: NORMAL;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				fixed4 diffuse : COLOR0; // diffuse lighting color
				float4 vertex : SV_POSITION;
				
			};

			v2f vert(appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				// get vertex normal in world space
				half3 worldNormal = UnityObjectToWorldNormal(v.normal);
				// dot product between normal and light direction for
				// standard diffuse (Lambert) lighting
				half nl = max(0, dot(worldNormal, _WorldSpaceLightPos0.xyz));
				// factor in the light color
				o.diffuse = nl * _LightColor0;
				return o;
			}

			fixed4 frag(v2f i) : SV_Target
			{

				// sample texture
				fixed4 col = tex2D(_MainTex, i.uv);
				// multiply by lighting
				col *= i.diffuse;
				col.a = _Alpha;

				return col;
			}
			ENDCG
		}
	}
}
