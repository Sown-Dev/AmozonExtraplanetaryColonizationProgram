Shader "Custom/UniformOutline"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _OutlineColor ("Outline Color", Color) = (0,0,0,1)
        _AddColor ("Add Color", Color) = (0,0,0,0)
        _OutlineThickness ("Outline Thickness", Float) = 1.0
        // Assuming a uniform Pixels Per Unit for illustration purposes
        _PixelsPerUnit ("Pixels Per Unit", Float) = 100.0
    }
    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        LOD 100

        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off

        Pass
        {
            Name "OUTLINE"

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _OutlineColor;
            float _OutlineThickness;
            float _PixelsPerUnit;
            float4 _AddColor;

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = TransformObjectToHClip(v.vertex.xyz);
                o.uv = v.uv;
                return o;
            }

            float4 frag(v2f i) : SV_Target
            {
                
                // Adjust outline thickness relative to the sprite's size in the world
                float thickness = _OutlineThickness / _PixelsPerUnit;

                // Sample the texture
                float4 texColor = tex2D(_MainTex, i.uv);
                float maxAlpha = texColor.a;

                texColor.rgb += _AddColor.rgb;

         // Ensure we don't go below zero in any channel
             texColor.rgb = max(texColor.rgb, 0);
                texColor.rgb = min(texColor.rgb, 1);

                // Examining surrounding pixels
                for (int y = -1; y <= 1; y++)
                {
                    for (int x = -1; x <= 1; x++)
                    {
                        float2 offset = thickness * float2(x, y);
                        maxAlpha = max(maxAlpha, tex2D(_MainTex, i.uv + offset).a > 0.9f ? 1 : 0);
                    }
                }

                float4 outlineColor = _OutlineColor;
                outlineColor.a = maxAlpha > texColor.a ? maxAlpha : 0;
                return lerp(texColor, outlineColor, outlineColor.a);
            }
            ENDHLSL
        }
    }
}
