Shader "Custom/OutlineShader" {
    Properties{
        _MainTex("Texture", 2D) = "white" {}
        _OutlineColor("Outline Color", Color) = (1, 1, 0, 1)
        _OutlineWidth("Outline Width", Range(0, 0.1)) = 0.05
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
            };

            void surf(Input IN, inout SurfaceOutput o) {
                half4 c = tex2D(_MainTex, IN.uv_MainTex);
                o.Albedo = c.rgb;
                o.Alpha = c.a;

                half4 outlineColor = tex2D(_MainTex, IN.uv_MainTex + float2(_OutlineWidth, 0))
                                   + tex2D(_MainTex, IN.uv_MainTex - float2(_OutlineWidth, 0))
                                   + tex2D(_MainTex, IN.uv_MainTex + float2(0, _OutlineWidth))
                                   + tex2D(_MainTex, IN.uv_MainTex - float2(0, _OutlineWidth));

                if (outlineColor.a < 1 && c.a > 0) {
                    o.Albedo = _OutlineColor.rgb;
                }
            }
            ENDCG
        }
            FallBack "Diffuse"
}
