Shader "Custom/SoftShadowReceiverFrag"
{
    Properties
    {
        _MainTex("Texture", 2D) = "white" {}
        _Color("Color", Color) = (1,1,1,1)
    }
        SubShader
        {
            Tags { "RenderType" = "Opaque" }
            LOD 100

            Pass
            {
                // Forward Base pass for main light and shadows
                Tags { "LightMode" = "ForwardBase" }

                CGPROGRAM
                #pragma vertex vert
                #pragma fragment frag
            // Enable shadow support
            #pragma multi_compile_fwdbase

            #include "UnityCG.cginc"
            #include "AutoLight.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float3 normal : NORMAL;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 pos : SV_POSITION;
                float3 worldNormal : TEXCOORD1;
                float3 worldPos : TEXCOORD2;
                SHADOW_COORDS(3) // Shadow coordinates
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            fixed4 _Color;
            float4 _LightColor0; // Main light color

            v2f vert(appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.worldNormal = UnityObjectToWorldNormal(v.normal);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                TRANSFER_SHADOW(o); // Compute shadow coordinates
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                // Sample texture
                fixed4 col = tex2D(_MainTex, i.uv) * _Color;

            // Basic Lambert lighting
            float3 normal = normalize(i.worldNormal);
            float3 lightDir = normalize(_WorldSpaceLightPos0.xyz);
            float NdotL = max(0, dot(normal, lightDir));

            // Sample shadow map
            float shadow = SHADOW_ATTENUATION(i); // Soft shadow attenuation

            // Combine lighting and shadow
            fixed3 lighting = _LightColor0.rgb * NdotL * shadow;
            col.rgb *= lighting + UNITY_LIGHTMODEL_AMBIENT.rgb;

            return col;
        }
        ENDCG
    }

            // Additional pass for shadow caster
            Pass
            {
                Tags { "LightMode" = "ShadowCaster" }

                CGPROGRAM
                #pragma vertex vert
                #pragma fragment frag
                #pragma multi_compile_shadowcaster

                #include "UnityCG.cginc"

                struct v2f
                {
                    V2F_SHADOW_CASTER;
                };

                v2f vert(appdata_base v)
                {
                    v2f o;
                    TRANSFER_SHADOW_CASTER_NORMALOFFSET(o)
                    return o;
                }

                float4 frag(v2f i) : SV_Target
                {
                    SHADOW_CASTER_FRAGMENT(i)
                }
                ENDCG
            }
        }
}