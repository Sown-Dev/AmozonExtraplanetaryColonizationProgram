Shader "Custom/ColorReplacerWithShading"
{
    Properties
    {
        _MainTex ("Sprite Texture", 2D) = "white" {}
        _Color1 ("Original Color 1", Color) = (1, 1, 1, 1)
        _ReplaceColor1 ("Replace Color 1", Color) = (1, 1, 1, 1)
        _Color2 ("Original Color 2", Color) = (1, 1, 1, 1)
        _ReplaceColor2 ("Replace Color 2", Color) = (1, 1, 1, 1)
        _Color3 ("Original Color 3", Color) = (1, 1, 1, 1)
        _ReplaceColor3 ("Replace Color 3", Color) = (1, 1, 1, 1)
        _Threshold ("Color Match Threshold", Range(0, 1)) = 0.1
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        LOD 100

        Pass
        {
            Blend SrcAlpha OneMinusSrcAlpha
            Cull Off
            ZWrite Off

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float4 _Color1;
            float4 _ReplaceColor1;
            float4 _Color2;
            float4 _ReplaceColor2;
            float4 _Color3;
            float4 _ReplaceColor3;
            float _Threshold;

            struct appdata_t
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            v2f vert (appdata_t v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            // Compute luminance (perceived brightness)
            float Luminance(float3 color)
            {
                return dot(color, float3(0.299, 0.587, 0.114)); // Standard RGB luminance formula
            }

            // Function to replace color while preserving shading
            float4 ReplaceColorWithShading(float4 currentColor, float4 targetColor, float4 replacementColor, float threshold)
            {
                float3 colorDifference = currentColor.rgb - targetColor.rgb;
                float distance = sqrt(dot(colorDifference, colorDifference)); // Euclidean distance

                if (distance < threshold)
                {
                    // Compute the brightness adjustment
                    float originalLuminance = Luminance(currentColor.rgb);
                    float targetLuminance = Luminance(targetColor.rgb);
                    float luminanceRatio = originalLuminance / (targetLuminance + 0.0001); // Avoid division by zero

                    // Scale the replacement color by the luminance ratio
                    float3 adjustedColor = replacementColor.rgb * luminanceRatio;
                    return float4(adjustedColor, currentColor.a); // Preserve alpha
                }

                return currentColor;
            }

            float4 frag(v2f i) : SV_Target
            {
                float4 texColor = tex2D(_MainTex, i.uv);

                // Apply color replacements with shading preservation
                texColor = ReplaceColorWithShading(texColor, _Color1, _ReplaceColor1, _Threshold);
                texColor = ReplaceColorWithShading(texColor, _Color2, _ReplaceColor2, _Threshold);
                texColor = ReplaceColorWithShading(texColor, _Color3, _ReplaceColor3, _Threshold);

                return texColor;
            }
            ENDCG
        }
    }
    FallBack "Sprites/Default"
}
