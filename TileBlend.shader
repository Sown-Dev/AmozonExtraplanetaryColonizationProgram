Shader "Custom/TileInnerTransparency"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _TransparencyRatio ("Transparency Ratio", Range(0, 0.5)) = 0.1
    }

    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        LOD 100

        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off

        Pass
        {
            Name "TRANSPARENCY"

            HLSLPROGRAM
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
            float4 _MainTex_ST;

            float _TransparencyRatio;

            v2f vert(appdata_t v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }
            float random(float2 co)
            {
                return frac(sin(dot(co.xy, float2(12.9898,78.233))) * 43758.5453);
            }
            float4 frag(v2f i) : SV_Target
            {
                // Sample the texture
                float4 texColor = tex2D(_MainTex, i.uv);
                
                // Get the UV coordinates
                float2 uv = i.uv * 16; // Assuming tiles are 16x16 pixels

                // Calculate inner transparency based on UV coordinates
                float transparency = 0.0;

                // Randomly choose to set inner pixels to transparent
                if (uv.x < _TransparencyRatio * 16 || uv.x >= (1 - _TransparencyRatio) * 16 ||
                    uv.y < _TransparencyRatio * 16 || uv.y >= (1 - _TransparencyRatio) * 16)
                {
                    //make transparency chance  higher further from edge
                    
                    if(random(i.uv*100)<0.5){//-0.5*abs(uv.x-8)/8-0.5*abs(uv.y-8)/8 ){
                        transparency = 1.0; // Fully transparent
                    }
                }

                // Apply transparency to the color
                texColor.a *= (1 - transparency);
                
                return texColor;
            }
            ENDHLSL
        }
    }
}
