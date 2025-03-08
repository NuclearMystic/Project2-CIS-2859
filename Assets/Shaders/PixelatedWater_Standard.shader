Shader "Custom/URP_PixelatedWater_Shore"
{
    Properties
    {
        _BaseMap ("Water Texture", 2D) = "white" {}
        _WaveSpeed ("Wave Speed", Range(0, 5)) = 1.0
        _WaveStrength ("Wave Strength", Range(0, 1)) = 0.02
        _PixelSize ("Pixelation Size", Range(1, 128)) = 32
        _Smoothness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0
        _Alpha ("Transparency", Range(0, 1)) = 1.0
        _ShoreFade ("Shoreline Blend Strength", Range(0.01, 20.0)) = 1.0
        _ShoreColor ("Shoreline Color", Color) = (1, 1, 1, 1)
    }

    SubShader
    {
        Tags { "RenderPipeline" = "UniversalPipeline" "Queue"="Transparent" }
        Pass
        {
            Name "ForwardLit"
            Tags { "LightMode" = "UniversalForward" }

            Blend SrcAlpha OneMinusSrcAlpha 
            ZWrite Off  
            Cull Off   

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            TEXTURE2D_X(_CameraDepthTexture);
            SAMPLER(sampler_CameraDepthTexture);

            struct Attributes
            {
                float4 positionOS   : POSITION;
                float2 uv          : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionCS   : SV_POSITION;
                float2 uv           : TEXCOORD0;
                float3 worldPos     : TEXCOORD1;
                float4 screenPos    : TEXCOORD2;
            };

            sampler2D _BaseMap;
            float4 _BaseMap_ST;
            float _WaveSpeed;
            float _WaveStrength;
            float _PixelSize;
            float _Smoothness;
            float _Metallic;
            float _Alpha;
            float _ShoreFade;
            float4 _ShoreColor;

            float LinearEyeDepth(float rawDepth)
            {
                #if UNITY_REVERSED_Z
                    return 1.0 - rawDepth; 
                #else
                    return rawDepth;
                #endif
            }


            Varyings vert (Attributes IN)
            {
                Varyings OUT;
                OUT.positionCS = TransformObjectToHClip(IN.positionOS.xyz);
                OUT.worldPos = TransformObjectToWorld(IN.positionOS.xyz);
                OUT.uv = TRANSFORM_TEX(IN.uv, _BaseMap);
                OUT.screenPos = ComputeScreenPos(OUT.positionCS);
                return OUT;
            }

            half4 frag (Varyings IN) : SV_Target
            {
                float timeOffset = _Time.y * _WaveSpeed;
                float2 waveOffset = float2(
                    sin(IN.worldPos.y * 10.0 + timeOffset),
                    cos(IN.worldPos.x * 10.0 + timeOffset)
                ) * _WaveStrength;

                float2 pixelatedUV = floor((IN.uv + waveOffset) * _PixelSize) / _PixelSize;

                half4 waterColor = tex2D(_BaseMap, pixelatedUV);

                float rawDepth = SAMPLE_TEXTURE2D_X(_CameraDepthTexture, sampler_CameraDepthTexture, IN.screenPos.xy / IN.screenPos.w).r;
                float sceneDepth = LinearEyeDepth(rawDepth);
                float waterDepth = LinearEyeDepth(IN.positionCS.z);

                float depthDiff = saturate((sceneDepth - waterDepth) * _ShoreFade);

                half4 finalColor = lerp(_ShoreColor, waterColor, depthDiff);
                
                finalColor.a *= _Alpha;

                return finalColor;
            }
            ENDHLSL
        }
    }
}
