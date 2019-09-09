Shader "Custom/TexturePickDiffuse" {
	Properties {
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		_MaskTex("Mask Texture", 2D) = "white"{}
		_Alpha("Mask Alpha", Range(0,1)) = 1
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200

		CGPROGRAM
        
        #pragma surface surf Lambert noforwardadd
        
        sampler2D _MainTex;
        sampler2D _MaskTex;
        float _Alpha;
        struct Input {
            float2 uv_MainTex;
            float2 uv4_MaskTex;
        };
  
        void surf (Input IN, inout SurfaceOutput o) {
            fixed4 c = tex2D(_MainTex, IN.uv_MainTex);
            fixed4 c2 = tex2D(_MaskTex, IN.uv4_MaskTex);
            o.Albedo = c.rgb * (1 - c2.a * _Alpha) + c2.rgb * c2.a * _Alpha;
            o.Alpha = c.a;
        }
        ENDCG
	}
	FallBack "Diffuse"
}