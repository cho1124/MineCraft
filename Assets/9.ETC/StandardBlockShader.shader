/*Shader "Minecraft/Blocks" {

	Properties{
		_MainTex("Block Texture Atlas", 2D) = "white" {}
	}

		SubShader{

			Tags {"RenderType" = "Opaque" "RenderPipeline" = "UniversalPipeline"}
			LOD 100


			Pass {

				CGPROGRAM
					#pragma vertex vertFunction
					#pragma fragment fragFunction
					#pragma target 2.0


					//#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
					//#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
					#include "UnityCG.cginc"

					struct appdata {

						float4 vertex : POSITION;
						float2 uv : TEXCOORD0;
						float4 color : COLOR;

					};

					struct v2f {

						float4 vertex : SV_POSITION;
						float2 uv : TEXCOORD0;
						float3 normal : TEXCOORD1;
						float4 color : COLOR;

					};

					sampler2D _MainTex;
					float GlobalLightLevel;
					float minGlobalLightLevel;
					float maxGlobalLightLevel;

					v2f vertFunction(appdata v) {

						v2f o;

						o.vertex = UnityObjectToClipPos(v.vertex);
						o.uv = v.uv;
						o.color = v.color;

						return o;

					}

					fixed4 fragFunction(v2f i) : SV_Target {

						fixed4 col = tex2D(_MainTex, i.uv);

						float shade = (maxGlobalLightLevel - minGlobalLightLevel) * GlobalLightLevel + minGlobalLightLevel;
						shade *= i.color.a;
						shade = clamp(1 - shade, minGlobalLightLevel, maxGlobalLightLevel);
						

						
						
						col = lerp(col, float4(0, 0, 0, 1), shade);

						return col;

					}

					ENDCG

			}
			UsePass "Universal Render Pipeline/Lit/ShadowCaster"

	}

}*/


Shader "Minecraft/Blocks" {
    Properties{
        _MainTex("Block Texture Atlas", 2D) = "white" {}
        _GlobalLightLevel("Global Light Level", Range(0, 1)) = 1.0
        _MinGlobalLightLevel("Min Global Light Level", Range(0, 1)) = 0.0
        _MaxGlobalLightLevel("Max Global Light Level", Range(0, 1)) = 1.0
    }

        SubShader{
            Tags {"RenderType" = "Opaque" "RenderPipeline" = "UniversalPipeline"}
            LOD 100

            Pass {
                Tags { "LightMode" = "UniversalForward" }
                HLSLPROGRAM
                    #pragma vertex vertFunction
                    #pragma fragment fragFunction
                    #pragma target 2.0

                    #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
                    #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
                    #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"

                    struct appdata {
                        float4 vertex : POSITION;
                        float2 uv : TEXCOORD0;
                        float3 normal : NORMAL;
                        float4 color : COLOR;
                    };

                    struct v2f {
                        float4 vertex : SV_POSITION;
                        float2 uv : TEXCOORD0;
                        float3 normal : TEXCOORD1;
                        float4 color : COLOR;
                        float3 worldPos : TEXCOORD2;
                    };

                    sampler2D _MainTex;
                    float4 _MainTex_ST;
                    float _GlobalLightLevel;
                    float _MinGlobalLightLevel;
                    float _MaxGlobalLightLevel;

                    v2f vertFunction(appdata v) {
                        v2f o;
                        o.vertex = TransformObjectToHClip(v.vertex);
                        o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                        o.color = v.color;
                        o.normal = TransformObjectToWorldNormal(v.normal);
                        o.worldPos = TransformObjectToWorld(v.vertex);
                        return o;
                    }

                    half4 fragFunction(v2f i) : SV_Target {
                        float4 col = tex2D(_MainTex, i.uv);

                        // 최소 밝기 적용
                        float shade = (_MaxGlobalLightLevel - _MinGlobalLightLevel) * _GlobalLightLevel + _MinGlobalLightLevel;
                        shade *= i.color.a;
                        shade = clamp(1 - shade, _MinGlobalLightLevel, _MaxGlobalLightLevel);

                        col = lerp(col, float4(0, 0, 0, 1), shade);

                        // 조명 계산
                        half3 ambient = UNITY_LIGHTMODEL_AMBIENT.xyz;
                        half3 normal = normalize(i.normal);
                        Light mainLight = GetMainLight();
                        half3 lightDir = normalize(mainLight.direction);
                        half NdotL = max(dot(normal, lightDir), 0.0);
                        half3 diffuse = mainLight.color * NdotL;

                        // 최소 밝기 적용
                        half3 lighting = ambient + diffuse;
                        lighting = max(lighting, _MinGlobalLightLevel);

                        col.rgb *= lighting;

                        return col;
                    }

                ENDHLSL
            }
            UsePass "Universal Render Pipeline/Lit/ShadowCaster"
        }
}
