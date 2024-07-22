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
                HLSLPROGRAM
                #pragma vertex vertFunction
                #pragma fragment fragFunction
                #pragma target 2.0

                #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
                #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
                #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Input.hlsl"

                struct Attributes {
                    float4 vertex : POSITION;
                    float2 uv : TEXCOORD0;
                    float4 color : COLOR;
                };

                struct Varyings {
                    float4 vertex : SV_POSITION;
                    float2 uv : TEXCOORD0;
                    float4 color : COLOR;
                    float3 worldPos : TEXCOORD1;
                };

                TEXTURE2D(_MainTex);
                SAMPLER(sampler_MainTex);

                float _GlobalLightLevel;
                float _MinGlobalLightLevel;
                float _MaxGlobalLightLevel;

                Varyings vertFunction(Attributes v) {
                    Varyings o;
                    o.vertex = TransformObjectToHClip(v.vertex);
                    o.uv = v.uv;
                    o.color = v.color;
                    o.worldPos = TransformObjectToWorld(v.vertex).xyz;
                    return o;
                }

                half4 fragFunction(Varyings i) : SV_Target {
                    // Sample the texture
                    half4 col = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv);

                    // Calculate shading based on GlobalLightLevel and vertex color alpha
                    float shade = (_MaxGlobalLightLevel - _MinGlobalLightLevel) * _GlobalLightLevel + _MinGlobalLightLevel;
                    shade *= i.color.a;
                    shade = clamp(1 - shade, _MinGlobalLightLevel, _MaxGlobalLightLevel);

                    // Apply shading
                    col = lerp(col, half4(0, 0, 0, 1), shade);

                    // Apply URP lighting
                    Light mainLight = GetMainLight();
                    half3 lighting = mainLight.color;

                    col.rgb *= lighting;

                    return col;
                }

                ENDHLSL
            }
        }

            FallBack "Diffuse"
}