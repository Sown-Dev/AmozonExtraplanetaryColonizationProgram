Shader "Custom/TilemapExpandAndRandomizeShader"
{
    Properties
    {
        _MainTex ("Tilemap Texture", 2D) = "white" {}
        _RandomSeed ("Random Seed", Range(0.0, 100.0)) = 0.0 // Seed for random transparency
    }
    SubShader
    {
        Tags { "RenderType" = "Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

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

            sampler2D _MainTex;
            float4 _MainTex_TexelSize;
            float _RandomSeed;

            v2f vert (appdata_t v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            float4 frag (v2f i) : SV_Target
            {
                // Calculate the 16x16 tile size and the expanded size
                float2 tileSize = float2(16.0, 16.0);
                float2 expandedSize = tileSize + 4.0; // Expand by 2 pixels

                // Calculate UVs for expanded tile
                float2 expandedUV = i.uv * expandedSize;

                // Sample the main texture
                float4 color = tex2D(_MainTex, expandedUV);

                // Get the inner pixel coordinates (14x14) and check for transparency
                int2 innerPixelCoord = int2(floor(expandedUV));
                bool isInnerBorder = (innerPixelCoord.x >= 2 && innerPixelCoord.x < 14) &&
                                     (innerPixelCoord.y >= 2 && innerPixelCoord.y < 14);

                // Randomly determine if the pixel should be transparent
                if (isInnerBorder)
                {
                    // Use a simple hash function for randomness based on pixel coordinates and seed
                    float randomValue = frac(sin(dot(float2(innerPixelCoord.x, innerPixelCoord.y), float2(12.9898, 78.233))) * 43758.5453 + _RandomSeed);
                    
                    // Set pixel to transparent with a certain probability (e.g., 20%)
                    if (randomValue < 0.2)
                    {
                        color.a = 0.0; // Make transparent
                    }
                }

                return color;
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}
