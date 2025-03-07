Shader "Custom/URP_PixelatedWater"
{
    Properties
    {
        _BaseMap ("Water Texture", 2D) = "white" {}
        _WaveSpeed ("Wave Speed", Range(0, 5)) = 1.0
        _WaveStrength ("Wave Strength", Range(0, 1)) = 0.02
        _PixelSize ("Pixelation Size", Range(1, 128)) = 32
        _Smoothness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0
        _Alpha ("Transparency", Range(0, 1)) = 1.0 // New Transparency Property
    }

    SubShader
    {
        Tags { "RenderPipeline" = "UniversalPipeline" "Queue"="Transparent" }
        Pass
        {
            Name "ForwardLit"
            Tags { "LightMode" = "UniversalForward" }

            Blend SrcAlpha OneMinusSrcAlpha // Enables transparency blending
            ZWrite Off  // Disables writing to depth buffer (helps with transparency sorting)
            Cull Off    // Makes the shader render both sides of the water plane

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

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
            };

            sampler2D _BaseMap;
            float4 _BaseMap_ST;
            float _WaveSpeed;
            float _WaveStrength;
            float _PixelSize;
            float _Smoothness;
            float _Metallic;
            float _Alpha; // Transparency Control

            Varyings vert (Attributes IN)
            {
                Varyings OUT;
                OUT.positionCS = TransformObjectToHClip(IN.positionOS.xyz);
                OUT.worldPos = TransformObjectToWorld(IN.positionOS.xyz);
                OUT.uv = TRANSFORM_TEX(IN.uv, _BaseMap);
                return OUT;
            }

            half4 frag (Varyings IN) : SV_Target
            {
                // Ripple effect using sine waves
                float timeOffset = _Time.y * _WaveSpeed;
                float2 waveOffset = float2(
                    sin(IN.worldPos.y * 10.0 + timeOffset),
                    cos(IN.worldPos.x * 10.0 + timeOffset)
                ) * _WaveStrength;

                // Apply pixelation
                float2 pixelatedUV = floor((IN.uv + waveOffset) * _PixelSize) / _PixelSize;

                // Sample texture
                half4 col = tex2D(_BaseMap, pixelatedUV);

                // Apply transparency
                col.a *= _Alpha;

                return col;
            }
            ENDHLSL
        }
    }
}
