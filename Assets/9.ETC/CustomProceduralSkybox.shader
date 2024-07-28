Shader "Custom/ProceduralSkyboxWithSunAndMoonTexture"
{
    Properties
    {
        _SunTex("Sun Texture", 2D) = "white" {}
        _SunTexIntensity("Sun Texture Intensity", Range(0, 1)) = 1.0
        _SunTexColor("Sun Texture Color", Color) = (1, 1, 1, 1)

        _SunSize("Sun Size", Range(0.00001, 1.0)) = 0.1
        _SunColor("Sun Color", Color) = (1, 1, 1, 1)

        _MoonTex("Moon Texture", 2D) = "white" {}
        _MoonTexIntensity("Moon Texture Intensity", Range(0, 1)) = 1.0
        _MoonTexColor("Moon Texture Color", Color) = (1, 1, 1, 1)

        _MoonSize("Moon Size", Range(0.00001, 1.0)) = 0.1
        _MoonColor("Moon Color", Color) = (1, 1, 1, 1)

        _SkyTopColor("Sky Top Color", Color) = (0.172549, 0.568627, 0.694118, 1)
        _SkyBottomColor("Sky Bottom Color", Color) = (0.764706, 0.815686, 0.85098, 1)
        _SkyLeftColor("Sky Left Color", Color) = (0.172549, 0.568627, 0.694118, 1)
        _SkyRightColor("Sky Right Color", Color) = (0.172549, 0.568627, 0.694118, 1)
        _SkyFrontColor("Sky Front Color", Color) = (0.172549, 0.568627, 0.694118, 1)
        _SkyBackColor("Sky Back Color", Color) = (0.172549, 0.568627, 0.694118, 1)
        _SkyExponent("Sky Exponent", Range(0.1, 10.0)) = 2.5
    }
        SubShader
        {
            Tags { "Queue" = "Background" "RenderType" = "Background" }
            LOD 100

            Pass
            {
                CGPROGRAM
                #pragma vertex vert
                #pragma fragment frag
                #include "UnityCG.cginc"

                // Sun
                sampler2D _SunTex;
                float _SunTexIntensity;
                float4 _SunTexColor;

                float _SunSize;
                float4 _SunColor;

                // Moon
                sampler2D _MoonTex;
                float _MoonTexIntensity;
                float4 _MoonTexColor;

                float _MoonSize;
                float4 _MoonColor;

                float4 _SkyTopColor;
                float4 _SkyBottomColor;
                float4 _SkyLeftColor;
                float4 _SkyRightColor;
                float4 _SkyFrontColor;
                float4 _SkyBackColor;
                float _SkyExponent;

                struct appdata
                {
                    float4 vertex : POSITION;
                };

                struct v2f
                {
                    float4 pos : SV_POSITION;
                    float3 worldPos : TEXCOORD0;
                };

                v2f vert(appdata v)
                {
                    v2f o;
                    o.pos = UnityObjectToClipPos(v.vertex);
                    o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                    return o;
                }

                half4 frag(v2f i) : SV_Target
                {
                    float3 worldDir = normalize(i.worldPos);

                    // 각 방향에 대한 색상을 설정합니다.
                    float3 skyColor;
                    if (worldDir.y > 0.5) skyColor = _SkyTopColor.rgb;
                    else if (worldDir.y < -0.5) skyColor = _SkyBottomColor.rgb;
                    else if (worldDir.x > 0.5) skyColor = _SkyRightColor.rgb;
                    else if (worldDir.x < -0.5) skyColor = _SkyLeftColor.rgb;
                    else if (worldDir.z > 0.5) skyColor = _SkyFrontColor.rgb;
                    else skyColor = _SkyBackColor.rgb;

                    float horizonBlend = pow(saturate(worldDir.y), _SkyExponent);
                    skyColor = lerp(_SkyBottomColor.rgb, skyColor, horizonBlend);

                    // 태양 방향과 텍스처 샘플링
                    float3 sunDir = float3(1.0, 0.0, 0.0);
                    float sunIntensity = dot(worldDir, sunDir);
                    sunIntensity = smoothstep(1.0 - _SunSize, 1.0, sunIntensity);

                    float2 sunUV = (worldDir.xy * 0.5 + 0.5);
                    half4 sunTexColor = tex2D(_SunTex, sunUV);
                    half4 sunColor = lerp(_SunColor, sunTexColor * _SunTexColor, _SunTexIntensity);

                    // 달 방향과 텍스처 샘플링
                    float3 moonDir = float3(-1.0, 0.0, 0.0);
                    float moonIntensity = dot(worldDir, moonDir);
                    moonIntensity = smoothstep(1.0 - _MoonSize, 1.0, moonIntensity);

                    float2 moonUV = (worldDir.xy * 0.5 + 0.5);
                    half4 moonTexColor = tex2D(_MoonTex, moonUV);
                    half4 moonColor = lerp(_MoonColor, moonTexColor * _MoonTexColor, _MoonTexIntensity);

                    float3 finalColor = skyColor;
                    finalColor = lerp(finalColor, sunColor.rgb, sunIntensity);
                    finalColor = lerp(finalColor, moonColor.rgb, moonIntensity);

                    return half4(finalColor, 1.0);
                }
                ENDCG
            }
        }
}
