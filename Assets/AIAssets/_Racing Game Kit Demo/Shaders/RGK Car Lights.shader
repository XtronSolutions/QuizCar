Shader "Racing Game Kit/Car Lights" {
Properties {
	_Color ("Main Color", Color) = (1,1,1,1)
	_MainTex ("Base (RGB) Gloss (A)", 2D) = "white" {}
	_EmmisiveText ("Bright Texture (A)", 2D) = "white" {}
	_Intensity ("Brightness", Range(0.0,3.0)) = 0.0
}
SubShader {
	Tags { "RenderType"="Opaque" }
	LOD 200
	
CGPROGRAM
#pragma surface surf BlinnPhong

sampler2D _MainTex;
sampler2D _EmmisiveText;
fixed4 _Color;
float _Intensity;

struct Input {
	float2 uv_MainTex;
	float2 uv_EmmisiveText;
};

void surf (Input IN, inout SurfaceOutput o) {
	fixed4 tex = tex2D(_MainTex, IN.uv_MainTex);
	fixed4 c = tex * _Color;
	o.Albedo = c.rgb;
	o.Emission = c.rgb * tex2D(_EmmisiveText, IN.uv_EmmisiveText).a * _Intensity;
	o.Alpha = c.a;
}
ENDCG
} 
FallBack "Racing Game Kit/Car Lights"
}
