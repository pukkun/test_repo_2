Shader "Custom/BlurVer2"
{
    Properties
    {
        _MainTex("Texture", 2D) = "white" {}
        _BlurSize("Blur Size", Float) = 0.002
    }
        SubShader
        {
            Tags { "Queue" = "Transparent" }
            GrabPass { "_GrabTexture" }

            Pass
            {
                CGPROGRAM
                #pragma vertex vert
                #pragma fragment frag
                #include "UnityCG.cginc"

                struct appdata
                {
                    float4 vertex : POSITION;
                    float2 uv : TEXCOORD0;
                };

                struct v2f
                {
                    float2 uv : TEXCOORD0;
                    float4 vertex : SV_POSITION;
                    float4 grabUV : TEXCOORD1;
                };

                sampler2D _GrabTexture;
                float _BlurSize;

                v2f vert(appdata v)
                {
                    v2f o;
                    o.vertex = UnityObjectToClipPos(v.vertex);
                    o.uv = v.uv;
                    o.grabUV = ComputeGrabScreenPos(o.vertex);
                    return o;
                }

                fixed4 frag(v2f i) : SV_Target
                {
                    fixed4 col = 0;
                    float2 uv = i.grabUV.xy / i.grabUV.w;

                    // Simple box blur
                    for (int x = -2; x <= 2; x++)
                    for (int y = -2; y <= 2; y++)
                    {
                        col += tex2D(_GrabTexture, uv + float2(x, y) * _BlurSize);
                    }
                    col /= 25.0; // Average the samples
                    return col;
                }
                ENDCG
            }
        }
}