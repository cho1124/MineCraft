Shader "Example/URPUnlitShaderBasic"
{
    Properties
    {
        _R("R",Range(0,1)) = 0
        _G("G",Range(0,1)) = 0
        _B("B",Range(0,1)) = 0

        _Brightness("밝기조절", Range(-1,1)) = 0
    }

        SubShader
    {
        Tags { "RenderType" = "Opaque" "RenderPipeline" = "UniversalPipeline" }

        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            // C Buffer 로 감싸준다
            CBUFFER_START(UnityPerMaterial)
            float _R;
            float _G;
            float _B;
            float _Brightness;
            CBUFFER_END

            struct Attributes
            {
                float4 positionOS   : POSITION;
            };

            struct Varyings
            {
                float4 positionHCS  : SV_POSITION;
            };

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);
                return OUT;
            }

            half4 frag() : SV_Target
            {
                float4 finalColor;
                finalColor.rgb = float3(_R, _G, _B);
                finalColor.rgb += _Brightness;
                finalColor.a = 1;

                //아래 둘의 결과물은 똑같다.
                return saturate(finalColor);
                //return clamp(finalColor,0,1);
            }
            ENDHLSL
        }
    }
}
