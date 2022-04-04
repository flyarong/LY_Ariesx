// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/UI/ExposureAll"
{
	Properties
	{
		[PerRendererData] _MainTex("Sprite Texture", 2D) = "white" {}
		_Exposure("Exposure", Range(0, 1)) = 0

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

		Cull Off
		Lighting Off
		ZWrite Off
		ZTest[unity_GUIZTestMode]
		Blend SrcAlpha OneMinusSrcAlpha
		ColorMask[_ColorMask]
		// Why grabpass with named texture not work.
		GrabPass
	{
		"_BackgroundTexture"
	}


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
		float4 grabPos : TEXCOORD2;
	};

	float _Exposure;
	fixed4 _TextureSampleAdd;
	float4 _ClipRect;
	sampler2D _BackgroundTexture;

	float3 RGB2HSV(float3 c) {
		float4 K = float4(0.0, -1.0 / 3.0, 2.0 / 3.0, -1.0);
		float4 p = lerp(float4(c.bg, K.wz), float4(c.gb, K.xy), step(c.b, c.g));
		float4 q = lerp(float4(p.xyw, c.r), float4(c.r, p.yzx), step(p.x, c.r));

		float d = q.x - min(q.w, q.y);
		float e = 1.0e-10;
		return float3(abs(q.z + (q.w - q.y) / (6.0 * d + e)), d / (q.x + e), q.x);
	}

	float3 HSV2RGB(float3 c) {
		float4 K = float4(1.0, 2.0 / 3.0, 1.0 / 3.0, 3.0);
		float3 p = abs(frac(c.xxx + K.xyz) * 6.0 - K.www);
		return c.z * lerp(K.xxx, saturate(p - K.xxx), c.y);
	}

	v2f vert(appdata_t IN)
	{
		v2f OUT;
		OUT.worldPosition = IN.vertex;
		OUT.vertex = UnityObjectToClipPos(OUT.worldPosition);
		OUT.texcoord = IN.texcoord;
#ifdef UNITY_HALF_TEXEL_OFFSET
		OUT.vertex.xy += (_ScreenParams.zw - 1.0)*float2(-1,1);
#endif
		half4 pos = UnityObjectToClipPos(IN.vertex);
		OUT.grabPos = ComputeGrabScreenPos(pos);

		return OUT;
	}

	sampler2D _MainTex;

	fixed4 frag(v2f IN) : SV_Target
	{
		half4 bgcolor = tex2Dproj(_BackgroundTexture, IN.grabPos);

		half4 c = (tex2D(_MainTex, IN.texcoord) + _TextureSampleAdd) * IN.color;
		c.a *= UnityGet2DClipping(IN.worldPosition.xy, _ClipRect);
		c.rgb = (fixed3(1, 1, 1) - bgcolor.rgb) * _Exposure + bgcolor.rgb;
#ifdef UNITY_UI_ALPHACLIP
		clip(c.a - 0.001);
#endif

		return c;
	}
		ENDCG
	}
	}
}
