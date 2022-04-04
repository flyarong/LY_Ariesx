// Simplified Diffuse shader. Differences from regular Diffuse one:
// - no Main Color
// - fully supports only 1 directional light. Other lights can affect it, but it will be per-vertex/SH.

Shader "Custom/Diffuse" {
Properties {
	_MainTex ("Base (RGB)", 2D) = "white" {}
}
SubShader {
	Tags { "RenderType"="Transparent" }
	LOD 150
	Blend SrcAlpha OneMinusSrcAlpha

CGPROGRAM
#pragma surface surf Lambert noforwardadd

sampler2D _MainTex;

struct Input {
	float2 uv_MainTex;
};

void surf (Input IN, inout SurfaceOutput o) {
	fixed4 c = tex2D(_MainTex, IN.uv_MainTex);
	o.Alpha = 0;// c.a * (c.r + c.g + c.b);
	o.Albedo = c.rgb * o.Alpha;
}
ENDCG
}

Fallback "Mobile/VertexLit"
}
