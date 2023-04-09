Shader "red_team" {
    Properties{
        _MainTex("Texture", 2D) = "white" {}
        _OutlineColor("Outline Color", Color) = (0, 0, 0, 1)
        _OutlineWidth("Outline Width", Range(0.001, 0.1)) = 0.01
    }

        SubShader{
            Tags {"Queue" = "Transparent" "RenderType" = "Transparent"}
            LOD 100

            CGPROGRAM
            #pragma surface surf Lambert

            sampler2D _MainTex;
            float _OutlineWidth;
            float4 _OutlineColor;

            struct Input {
                float2 uv_MainTex;
                float3 viewDir;
            };

            void surf(Input IN, inout SurfaceOutput o) {
                float2 uv = IN.uv_MainTex;
                float dist = tex2D(_MainTex, uv).a;

                float minDist = 1.0;
                for (int x = -1; x <= 1; x++) {
                    for (int y = -1; y <= 1; y++) {
                        float2 offset = float2(x, y) * _OutlineWidth;
                        minDist = min(minDist, tex2D(_MainTex, uv + offset).a);
                    }
                }

                o.Albedo = _OutlineColor.rgb;
                o.Alpha = 1 - (dist - minDist);
            }
            ENDCG
        }
            FallBack "Diffuse"
}
