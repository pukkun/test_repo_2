Shader "Custom/TransparentOverlapStandard"
{
    Properties
    {
        _Color("Color", Color) = (1,1,1,1)
        _MainTex("Texture", 2D) = "white" {}
        _Alpha("Alpha", Range(0,1)) = 0.5
        _ZWrite("ZWrite", Int) = 1
    }
        SubShader
        {
            Tags { "RenderType" = "Transparent" "Queue" = "Transparent" }
            LOD 200

            ZWrite[_ZWrite]
            Blend One OneMinusSrcAlpha

            CGPROGRAM
            #pragma surface surf Standard fullforwardshadows alpha:fade

            #pragma target 3.0

            sampler2D _MainTex;
            fixed4 _Color;
            half _Alpha;

            struct Input
            {
                float2 uv_MainTex;
            };

            void surf(Input IN, inout SurfaceOutputStandard o)
            {
                fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
                o.Albedo = c.rgb;
                o.Metallic = 0.0;
                o.Smoothness = 0.5;
                o.Alpha = _Alpha * c.a; // Kết hợp alpha từ texture (nếu có)
            }
            ENDCG
        }
            FallBack "Standard"
}