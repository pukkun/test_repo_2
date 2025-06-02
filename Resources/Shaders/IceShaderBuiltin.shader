Shader "Custom/IceShaderBuiltin" {
    Properties{
        _Color("Color", Color) = (0.7, 0.8, 1.0, 0.5) // Light blue with some initial alpha
        _MainTex("Albedo (RGB)", 2D) = "white" {}
        _Glossiness("Smoothness", Range(0,1)) = 0.7
        _Metallic("Metallic", Range(0,1)) = 0.0
        _RefractionAmount("Refraction Amount", Range(0, 0.1)) = 0.02
    }
        SubShader{
            Tags { "RenderType" = "Transparent" "Queue" = "Transparent" }
            LOD 200

            CGPROGRAM
            #pragma surface surf Standard alpha:blend // Use alpha:blend for transparency
            #pragma target 3.0

            sampler2D _MainTex;
            fixed4 _Color;
            half _Glossiness;
            half _Metallic;
            half _RefractionAmount;

            struct Input {
                float2 uv_MainTex;
                float3 worldNormal;
                float3 worldPos;
            };

            void surf(Input IN, inout SurfaceOutputStandard o) {
                fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
                o.Albedo = c.rgb;
                o.Metallic = _Metallic;
                o.Smoothness = _Glossiness;
                o.Alpha = c.a;

                // Refraction (Basic Approximation)
                float3 viewDir = normalize(_WorldSpaceCameraPos - IN.worldPos);
                float3 refractedDir = refract(viewDir, IN.worldNormal, 1.0 / 1.33); // IOR of ice is about 1.33
                float2 distortedUV = IN.uv_MainTex + refractedDir.xy * _RefractionAmount;
                fixed4 sceneColor = tex2D(_MainTex, distortedUV); // Sample the scene (using _MainTex as a placeholder, we don't have access to the actual scene texture in Surface shaders in Built-in RP)

                o.Albedo += sceneColor.rgb * 0.5; // Add some of the refracted color.  Adjust the 0.5 as needed.
            }

            ENDCG
        }
            FallBack "Standard"
}