Shader "Custom/TileMoveOutlineShader" {
    Properties {
        _MainTex ("Sprite Texture", 2D) = "white" {}
        _AreaOffset ("Area Offset", Vector) = (0, -4, 0, 0)  // Offset in pixels
        _AreaSize ("Area Size", Vector) = (8, 8, 0, 0)       // Width and height of the area
        _Direction ("Move Direction", Vector) = (1, 0, 0, 0) // Direction: (1,0)=right, (-1,0)=left, (0,1)=up, (0,-1)=down
        _Speed ("Speed", Float) = 1.0                        // Speed multiplier for movement
        _OutlineColor ("Outline Color", Color) = (0,0,0,1)
        _OutlineThickness ("Outline Thickness", Float) = 1.0
        _PixelsPerUnit ("Pixels Per Unit", Float) = 100.0
        _AddColor ("Add Color", Color) = (0,0,0,0)
    }
    SubShader {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        LOD 100
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off

        Pass {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            // Input texture
            sampler2D _MainTex;

            // Parameters for tile movement
            float4 _AreaOffset;
            float4 _AreaSize;
            float4 _Direction;
            float _Speed;

            // Parameters for outline
            float4 _OutlineColor;
            float _OutlineThickness;
            float _PixelsPerUnit;
            float4 _AddColor;

            struct appdata {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f {
                float2 uv : TEXCOORD0;
                float4 pos : SV_POSITION;
            };

            v2f vert(appdata v) {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            // Fragment shader
            fixed4 frag(v2f i) : SV_Target {
                float2 uv = i.uv;

                // Convert UV to pixel space
                float2 spriteSize = float2(16.0, 16.0); // Size of the whole sprite in pixels
                float2 pixelUV = uv * spriteSize;

                // Area offset and size
                float2 areaOffset = _AreaOffset.xy;
                float2 areaSize = _AreaSize.xy;

                // Check if pixel is inside the specified area
                if (pixelUV.x >= areaOffset.x && pixelUV.x < areaOffset.x + areaSize.x &&
                    pixelUV.y >= areaOffset.y && pixelUV.y < areaOffset.y + areaSize.y) {
                    
                    // Calculate pixel movement based on time and direction
                    float2 moveOffset = _Direction.xy * floor(_Time.x * 24.0 * _Speed) % areaSize;

                    // Wrap the pixels within the area
                    float2 localPos = pixelUV - areaOffset; // Local position in the 8x8 area
                    localPos = fmod(localPos + moveOffset + areaSize, areaSize); // Wrap around the area
                    pixelUV = localPos + areaOffset; // Convert back to global position
                }

                // Convert back to UV space
                float2 finalUV = pixelUV / spriteSize;

                // Sample the texture
                float4 texColor = tex2D(_MainTex, finalUV);
                texColor.rgb += _AddColor.rgb;
                texColor.rgb = saturate(texColor.rgb);

                // Outline logic
                float thickness = _OutlineThickness / _PixelsPerUnit;
                float maxAlpha = texColor.a;

                // Check surrounding pixels for the outline
                for (int y = -1; y <= 1; y++) {
                    for (int x = -1; x <= 1; x++) {
                        float2 offset = thickness * float2(x, y);
                        maxAlpha = max(maxAlpha, tex2D(_MainTex, finalUV + offset).a > 0.9f ? 1 : 0);
                    }
                }

                float4 outlineColor = _OutlineColor;
                outlineColor.a = maxAlpha > texColor.a ? maxAlpha : 0;
                float4 result = lerp(texColor, outlineColor, outlineColor.a);

                // Handle out-of-bound UVs to avoid border issues
                if (finalUV.x < 0 || finalUV.x > 1 || finalUV.y < 0 || finalUV.y > 1) {
                    return float4(0, 0, 0, 0); // Black or transparent
                }

                return result;
            }
            ENDCG
        }
    }
}
