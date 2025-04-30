Shader "Custom/SimpleTiledWaterColor"
{
    Properties
    {
        // The water base texture (should tile seamlessly)
        _MainTex ("Water Texture", 2D) = "white" {}
        // A tileable noise texture that will drive the UV distortion
        _NoiseTex ("Noise Texture", 2D) = "white" {}
        // Scale for the UVs so that each tile gets its own repeating unit.
        // For a 16x16 tilemap, set this to 16 (or adjust as needed).
        _TileScale ("Tile Scale", Float) = 16.0
        // Controls the strength of the distortion applied via the noise texture.
        _DistortStrength ("Distortion Strength", Float) = 0.05
        // Controls how quickly the noise texture scrolls, and thus how fast the animation is.
        _Speed ("Animation Speed", Float) = 1.0
        // Input water color to tint the water texture.
        _WaterColor ("Water Color", Color) = (1,1,1,1)
    }
    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            // Properties
            sampler2D _MainTex;
            sampler2D _NoiseTex;
            float _TileScale;
            float _DistortStrength;
            float _Speed;
            fixed4 _WaterColor;
            float4 _MainTex_ST; // automatically populated for Tiling and Offset

            // Vertex input structure
            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            // Data passed to the fragment shader
            struct v2f
            {
                float2 uv : TEXCOORD0;
                float2 uvNoise : TEXCOORD1;
                float4 vertex : SV_POSITION;
            };

            // Vertex shader:
            // - Transforms vertices.
            // - Scales and passes UV coordinates for tiling the water texture.
            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                // Apply the built-in texture scaling/offset and then multiply by _TileScale.
                o.uv = TRANSFORM_TEX(v.uv, _MainTex) * _TileScale;
                // Use the same UV space for the noise texture.
                o.uvNoise = v.uv * _TileScale;
                return o;
            }

            // Fragment shader:
            // - Uses time-based animation to scroll the noise texture.
            // - Distorts the water texture UVs using the noise.
            // - Tints the final water texture sample with _WaterColor.
            fixed4 frag (v2f i) : SV_Target
            {
                // Get animated time factor (using _Time.y for seconds)
                float t = _Time.y * _Speed;
                
                // Calculate a diagonal scrolling offset for the noise texture.
                float2 noiseOffset = float2(t, t);

                // Sample the noise texture at the animated coordinate.
                // Assumes the noise texture returns values in [0, 1], so center by subtracting 0.5.
                float noiseValue = tex2D(_NoiseTex, i.uvNoise + noiseOffset).r - 0.5;

                // Distort the water texture's UVs by adding the noise value scaled by _DistortStrength.
                float2 uvDistorted = i.uv + _DistortStrength * noiseValue;

                // Sample the water texture with the distorted UV coordinates.
                fixed4 texColor = tex2D(_MainTex, uvDistorted);

                // Multiply (tint) the water texture by the input water color.
                fixed4 col = texColor * _WaterColor;

                return col;
            }
            ENDCG
        }
    }
    FallBack "Unlit/Texture"
}
