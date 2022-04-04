// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/BoundaryGlowShader2"
{
	Properties
	{
		_Color("Main Color", Color) = (1,1,1,1)
		_Halo("Halo", Range(0, 1)) = 0.65
		_MainTex("Texture", 2D) = "white" {}
	}
		SubShader
	{
		Tags{ "Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Transparent" }
		Lighting Off
		Blend SrcAlpha OneMinusSrcAlpha
		Cull Off
		Pass
	{
		CGPROGRAM
#pragma vertex vert
#pragma fragment frag
#include "UnityCG.cginc"
		float4 _Color;
	sampler2D _MainTex;
	float _Shift;
	float _Halo;

	struct v2f {
		float4 pos : POSITION;
		float4 color : COLOR0;
		float2  uv : TEXCOORD0;
		float alpha : TEXCOORD1;
		float4 fragPos : TEXCOORD2;
		float4 fade : TEXCOORD3;
	};

	float4 _MainTex_ST;

	v2f vert(appdata_base v)
	{
		v2f o;
		o.color = _Color;
		o.pos = UnityObjectToClipPos(v.vertex);
		o.pos = UnityPixelSnap(o.pos);
		o.uv = v.texcoord;
		o.fragPos = mul(UNITY_MATRIX_MV, v.vertex);
		return o;
	}

	half4 frag(v2f i) : SV_Target
	{
		float4 outColor = i.color;
		half4 texcol = tex2D(_MainTex, i.uv);
		
		float4 objectOrigin = mul(UNITY_MATRIX_MV, float4(0, 0, 0, 1));
		float dist = distance(objectOrigin, i.fragPos);
		float percent = 115 / dist;
		outColor = float4(min(1, outColor.x * percent), min(1, outColor.y * percent), min(1, outColor.z * percent), 1);
		return outColor * texcol;
		
	}
		ENDCG
	}
	}
		FallBack "Default"
}