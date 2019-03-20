Shader "Custom/FillLines" 
{
    Properties
    {
        _MainTexFilled("Main Texture Filled", 2D) = "white"{} // Need the uv off this tex.  This is our main tex.
        _Gain("Gain", Range(0.0, 10.0)) = 10.0
        _TexUnfilled("Texture Unfilled", 2D) = "red"{}
        _TexUnfilledAlpha("Texture Unfilled Alpha", Range(0.0, 1.0)) = 0.0 // Default to not painting the background. 
        _FillMap( "Fill Map", 2D ) = "green"{} // Green = fill all
        _FillAmount("Fill Amount", Range(0.0, 1.0)) = 1.0
        _Flicker("Flicker", Range(0.0, 1.0)) = 0.0
        _SpecColor("Specular Color", Color) = ( 1.0, 1.0, 1.0, 1.0 )
        _Shininess("Shininess", Float) = 10.0
        _RimColor("Rim Color", Color) = ( 1.0, 1.0, 1.0, 1.0 )
        _RimPower("Rim Power", Range(1.0, 10.0)) = 3.0
    }
    SubShader
        {
        Pass
        {
            Tags { "LightMode" = "ForwardBase" "Queue" = "Geometry" } 

            // ZWrite Off // don't write to depth buffer 

            Blend SrcAlpha OneMinusSrcAlpha // use alpha blending

            CGPROGRAM
            //pragmas
            #pragma vertex vert
            #pragma fragment frag
            //#pragma exclude_renderers Flash

            //user defined variables
            uniform sampler2D _MainTexFilled;
            uniform float _Gain;
            uniform float4 _MainTexFilled_ST;
            uniform sampler2D _TexUnfilled;
            uniform float _TexUnfilledAlpha;
            uniform sampler2D _FillMap;
            uniform float _FillAmount;
            uniform float _Flicker;
            uniform float4 _SpecColor;
            uniform float _Shininess;
            uniform float4 _RimColor;
            uniform float _RimPower;

            // Unity difined variables
            uniform float4 _LightColor0;

            // Base input struct
            struct vertexInput
            {
                float4 vertex: POSITION;
                float3 normal: NORMAL;
                float4 texcoord: TEXCOORD0;
            };

            struct vertexOutput
            {
                float4 pos: SV_POSITION;
                float4 tex: TEXCOORD0;
                float4 posworld: TEXCOORD1;
                float3 normalDir: TEXCOORD2;
            };

            // Vertex function
            vertexOutput vert(vertexInput v)
            {
                vertexOutput o;

                o.pos = UnityObjectToClipPos(v.vertex);
                o.posworld = mul(unity_ObjectToWorld, v.vertex);
                o.normalDir = normalize( mul(unity_WorldToObject, float4(v.normal, 0.0) ).xyz );
                o.tex = v.texcoord;
                return o;
            }

            // Fragment function
            float4 frag(vertexOutput o):COLOR
            {
                float3 normalDirection = o.normalDir;
                float3 viewDirection = normalize( _WorldSpaceCameraPos.xyz - o.posworld.xyz );
                float3 lightDirection;
                float atten;
                if( _WorldSpaceLightPos0.w == 0.0 )
                { 
                    // Directional lights
                    atten = 1.0;
                    lightDirection = normalize( _WorldSpaceLightPos0.xyz );
                }
                else
                {
                    float3 fragmentToLight = _WorldSpaceLightPos0.xyz - o.posworld.xyz;
                    float dist = length( fragmentToLight );
                    atten = 1.0 / dist;
                    lightDirection = normalize( fragmentToLight );
                }

                // Lighting
                float3 diffuseReflection = atten * _LightColor0.xyz * saturate( dot ( normalDirection, lightDirection ) );
                float3 specularReflection = diffuseReflection * _SpecColor.xyz * pow( saturate( dot( reflect( -lightDirection, normalDirection ), viewDirection ) ), _Shininess );

                // Rim lighting
                float3 rim = 1 - saturate( dot( viewDirection, normalDirection ) );
                float3 rimLighting = saturate( dot( lightDirection, normalDirection ) * _RimColor.xyz * _LightColor0.xyz * pow( rim, _RimPower ) );

                float3 lightFinal = UNITY_LIGHTMODEL_AMBIENT.xyz + diffuseReflection + specularReflection + rimLighting * 5.0;

                // Texture Map
                half2 uv = o.tex.xy * _MainTexFilled_ST.xy + _MainTexFilled_ST.zw;
                float4 mainTexFilled = tex2D( _MainTexFilled, uv );
                float4 texUnfilled = tex2D( _TexUnfilled, uv ); 
                float4 fillMap = tex2D( _FillMap, uv );
                float4 finalColor = ( 1.0, 1.0, 1.0, 1.0 );


                // Use this to control what parts of the map you apply fill to. 
                // Circuits.
                if ( fillMap.g > 0.0 ) // Anything that is not black.
                {
                    // This is how much you want to fill rn.
                    // It's 1 - fillMap.g because of the texture.
                    float thisFill = (1 - fillMap.r);
                    float fillCheck = _FillAmount - thisFill;

                    if (fillCheck > -.01)
                    {
                        float timeOff = 1-frac(_Time[1]);
                        float dist = abs(timeOff - fillMap.r);
                        dist = min(dist, 1 - dist);
                        dist *= dist*dist;
                        float timeShimmer = dist;

                        float fillCheckFade = min(fillCheck, .05) * 20.0;
                        // float fillCheckFade = 1.0;

                        float startFade = min(thisFill, .1) * 10.0;

                        half2 noiseUv = uv + _Time.xy;
                        float rand = frac(sin(dot(noiseUv, float2(12.9898, 78.233))) * 43758.5453); // The bands. You could make a vec3 with time as the z. 
                        rand = 1+(rand * 0.05); // 1-(rand *.05) for vertical bands

                        // float rand = 1.0 - (frac(uv.x * 437 + uv.y * 113) * 1.0);

                        // This is what you want to fill it with.
                        finalColor = float4(mainTexFilled.xyz + lightFinal * .3, 1.0) * timeShimmer * startFade * rand * fillCheckFade * (1.0 - _Flicker); // + texUnfilled * _Flicker;  
                        finalColor *= _Gain; 
                    }
                    else
                    {
                        // This is how you're coloring in the part that isn't filled yet.
                        finalColor = float4(texUnfilled.xyz + lightFinal * .3, 1.0);
                        finalColor.a = _TexUnfilledAlpha;
                    }
                }
                // Non-circuits.
                else
                {
                    // This could use texUnfilled or mainTexFilled, since it's only the non-circuit parts.
                    finalColor = texUnfilled;
                    finalColor.a = 0.0;
                }

                return finalColor;
            }
            ENDCG
        }

    }
//  FallBack "Diffuse"
}