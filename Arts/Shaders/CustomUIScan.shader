// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/UI/Scan"
{
	Properties
	{
		[PerRendererData] _MainTex("Sprite Texture", 2D) = "white" {}
	_Color("Tint", Color) = (1,1,1,1)

		_Origin("Orign", Float) = 0.3
		_Peak("Peak", Float) = 1.5
		_Threshold("Threshold", Float) = 0.5
		_Speed("Speed", Float) = 1.3

		_StencilComp("Stencil Comparison", Float) = 8
		_Stencil("Stencil ID", Float) = 0
		_StencilOp("Stencil Operation", Float) = 0
		_StencilWriteMask("Stencil Write Mask", Float) = 255
		_StencilReadMask("Stencil Read Mask", Float) = 255

		_ColorMask("Color Mask", Float) = 15

		[Toggle(UNITY_UI_ALPHACLIP)] _UseUIAlphaClip("Use Alpha Clip", Float) = 0
	}

		SubShader
	{
		Tags
	{
		"Queue" = "Transparent"
		"IgnoreProjector" = "True"
		"RenderType" = "Transparent"
		"PreviewType" = "Plane"
		"CanUseSpriteAtlas" = "True"
	}

		Stencil
	{
		Ref[_Stencil]
		Comp[_StencilComp]
		Pass[_StencilOp]
		ReadMask[_StencilReadMask]
		WriteMask[_StencilWriteMask]
	}

		Lighting Off
		ZWrite Off
		ZTest[unity_GUIZTestMode]
		Blend SrcAlpha OneMinusSrcAlpha
		ColorMask[_ColorMask]

		Pass
	{
		CGPROGRAM
#pragma vertex vert
#pragma fragment frag

#include "UnityCG.cginc"
#include "UnityUI.cginc"

#pragma multi_compile __ UNITY_UI_ALPHACLIP

	struct appdata_t
	{
		float4 vertex   : POSITION;
		float4 color    : COLOR;
		float2 texcoord : TEXCOORD0;
	};

	struct v2f
	{
		float4 vertex   : SV_POSITION;
		fixed4 color : COLOR;
		half2 texcoord  : TEXCOORD0;
		float4 worldPosition : TEXCOORD1;
	};

	fixed4 _Color;
	fixed4 _TextureSampleAdd;
	float4 _ClipRect;

	float _Origin;
	float _Peak;
	float _Threshold;
	float _Speed;

	v2f vert(appdata_t IN)
	{
		v2f OUT;
		OUT.worldPosition = IN.vertex;
		OUT.vertex = UnityObjectToClipPos(OUT.worldPosition);

		OUT.texcoord = IN.texcoord;

#ifdef UNITY_HALF_TEXEL_OFFSET
		OUT.vertex.xy += (_ScreenParams.zw - 1.0)*float2(-1,1);
#endif

		OUT.color = IN.color * _Color;
		return OUT;
	}

	sampler2D _MainTex;

	// To do: remove if else.
	fixed4 frag(v2f IN) : SV_Target
	{
		half4 color = (tex2D(_MainTex, IN.texcoord) + _TextureSampleAdd) * IN.color;
		color.a *= UnityGet2DClipping(IN.worldPosition.xy, _ClipRect);

		float pie = 3.1416f;
		float param = abs(((_Time.y * _Speed - pie / 2) % pie) - pie / 2) / (pie / 2) * _Peak;
		float angle = ((_Time.y * _Speed) % pie) - pie / 2;
		if (angle < 0) {
			float tmp1 = param - max(IN.texcoord.y - _Origin, 0);// distance to middle line.
			float tmp2 = param - max(tmp1, 0);// 
			color.rgb *= 1 + tmp2 * (sign(tmp1) * 0.5f + 0.5f) - tmp2 * (param - _Threshold)* (sign(param - _Threshold) * 0.5f + 0.5f)  * (sign(tmp1) * 0.5f + 0.5f);
		} else {
			float tmp1 = (_Peak - param) - max((1 - _Origin) - IN.texcoord.y, 0);
			float tmp2 = (_Peak - param) - max(tmp1, 0);
			color.rgb *= 1 + tmp2 * (sign(tmp1) * 0.5f + 0.5f) - tmp2 * (1 - param) * (sign(1 - param) * 0.5f + 0.5f) * (sign(tmp1) * 0.5f + 0.5f);
		}

		//color.rgb = color.a;
#ifdef UNITY_UI_ALPHACLIP
		clip(color.a - 0.001);
#endif

		return color;
	}
		ENDCG
	}
	}
}
