Shader "Unlit/Quill Shader"
{
	Properties
    {
        [Toggle] _EnableTransparency("Enable Transparency", Int) = 1
        [Enum(Yes,0,No,2)] _Cull("Double Sided", Int) = 2
        [Toggle] _EnableFog("Enable Fog", Int) = 0
    }

    SubShader
    {
        Tags{ "RenderType" = "Opaque" "Queue" = "Geometry" }
        LOD 100
        Blend Off
        Cull[_Cull]
        ZWrite On
        ZTest On

        Pass
        {
            AlphaToMask[_EnableTransparency]

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_fog

            #pragma shader_feature _ENABLEFOG_ON
            #pragma shader_feature _ENABLETRANSPARENCY_ON
            #include "UnityCG.cginc"

            struct inVertex
            {
                float4 vertex : POSITION;
                float4 color : COLOR;
                float3 normal : NORMAL;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                #ifdef _ENABLEFOG_ON
                UNITY_FOG_COORDS(1)

                #endif

                #ifdef _ENABLETRANSPARENCY_ON

                float4 screenPos : TEXCOORD2;

                #endif
                float4 color : COLOR;
                float3 normal : NORMAL;
            };

            v2f vert(inVertex v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.color = v.color;
                o.normal = v.normal;
                #ifdef _ENABLEFOG_ON
                UNITY_TRANSFER_FOG(o, o.vertex);

                #endif

                #ifdef _ENABLETRANSPARENCY_ON

                o.screenPos = ComputeScreenPos(o.vertex);

                #endif
                
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float4 col = i.color;

                #ifdef _ENABLEFOG_ON
                UNITY_APPLY_FOG(i.fogCoord, col);
                #endif

                #ifdef _ENABLETRANSPARENCY_ON
                float2 pos = _ScreenParams.xy * i.screenPos.xy / i.screenPos.w;
                const int MSAASampleCount = 8;
                float ran = frac(52.9829189*frac(dot(pos, float2(0.06711056,0.00583715))));
                col.a = clamp(col.a + 0.99*(ran-0.5)/float(MSAASampleCount), 0.0, 1.0);
                #endif
                
                // Add shiny bands
                
                float timeOff = 1 - frac(_Time[1]); // return the fractional part of time * 2
                float dist = min(timeOff, 1 - timeOff);
                dist *= dist * dist;
                float timeShimmer = dist;
                
                half2 noiseUv = i.vertex.xy + _Time.xy;
                float rand = frac(sin(dot(noiseUv, float2(12.9898, 78.233))) * 43758.5453); // The bands. You could make a vec3 with time as the z. 
                rand = 1 + (rand * 0.05); // 1 + (rand *.05) for horizontal bands

                //float mult = .75 * (1 + timeShimmer * rand);
                //col = col * mult;
                
                float3 normal = i.normal;
                
                col.xyz *= 1.0 + (0.5 + (normal * 0.5)) * .1;

                float lineWidth = .05;
                
                // Nice vertical val from about .2 to .6
                float y = i.vertex.y * .002;
                //val *= rand;
                
                float x = i.vertex.x * .136;
                
                float bdModVal = -.5;
                float bothdirX = (fmod(x, bdModVal) - bdModVal*0.5);
                
                float time01 = fmod(2.5 * _Time[0] + x, 1);
                if (bothdirX < 0) {
                    time01 = 1.0 - time01;
                }
                
                // --- v Good Line v ---
                //return float4(time01, time01, time01, time01);
                
                //float width = .15 + fmod(normal.x + normal.y *.137, .15) * 2.0 + fmod(time01 + normal.x + x, .55) * .5;
                float width = fmod(time01 + normal.x + x, .55) * .5;
                    
                
                float vTime = time01;
                float lB = vTime - .5 * width;
                float uB = vTime + .5 * width;
                
                if(y > lB && y < uB){
                    float t = (y - lB) / width;
                    float dist = min (distance(t, 0), distance(t, 1));
                    dist = dist * dist;
                    col += dist;
              
                    //float fade = sin(t)
                    //return col + t;
                    //return col + .5 * (1 + sin( time01 * 200 ));
                    //return 1.4 * col;
                    //return float4(1, 0, 0, 1);
                }
                // return float4(val,val,val,1);
                

                // Debug  
                //return float4(i.vertex.x, i.vertex.y, i.vertex.z, 1) * .003;
                //return float4(i.vertex.x, i.vertex.x, i.vertex.x, 1) * .003;// grey (left) to white (right)
                //return float4(i.vertex.y, i.vertex.y, i.vertex.y, 1) * .003;// grey (feet) to white (head)
                //return float4(i.vertex.z, i.vertex.z, i.vertex.z, 1) * .003;// black (small)
                
                
                
                return col;
            }
            ENDCG
        }
    }
}
