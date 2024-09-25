Shader "Custom/ColorReplaceTransparent"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _ReplaceColor ("Replace Color", Color) = (1,1,1,1)
        _Threshold ("Threshold", Range(0, 1)) = 0.001
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        LOD 100

        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
        Cull Off

        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float3 positionOS : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct Varyings
            {
                float2 uv : TEXCOORD0;
                float4 positionHCS : SV_POSITION;
            };

            // Declare the texture and its sampler
            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);

            // Uniforms
            float4 _ReplaceColor;
            float _Threshold;

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                OUT.positionHCS = TransformObjectToHClip(IN.positionOS);
                OUT.uv = IN.uv;
                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target
            {
                half4 texColor = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, IN.uv);
                // Calculate the brightness to determine if the pixel is near black
                float brightness = dot(texColor.rgb, float3(0.2126, 0.7152, 0.0722)); // Using luminance formula
                // Replace color if pixel is darker than threshold and not completely transparent
                if (brightness < _Threshold && texColor.a > 0)
                {
                    return half4(_ReplaceColor.rgb, _ReplaceColor.a);
                }
                return texColor; // Return original texture color if conditions are not met
            }
            ENDHLSL
        }
    }
    FallBack "Diffuse"
}
