// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/Sprite/Gaussian"
{
	Properties
	{
		_MainTex("Texture", 2D) = "white" {}
		_Color("Color", Color) = (1,1,1,1)
		[MaterialToggle] PixelSnap("Pixel snap", Float) = 0
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

			Cull Back
			Lighting Off
			ZWrite On
			Blend One One
			Pass
		{

			CGPROGRAM
	#pragma vertex vert
	#pragma fragment frag
	#pragma multi_compile _ PIXELSNAP_ON
	#pragma shader_feature ETC1_EXTERNAL_ALPHA
	#include "UnityCG.cginc"

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
			float2 texcoord  : TEXCOORD0;
		};

		fixed4 _Color;

		v2f vert(appdata_t IN)
		{
			v2f OUT;
			OUT.vertex = UnityObjectToClipPos(IN.vertex);
			float2 offset = IN.texcoord - float2(0.5f, 0.5f);
			OUT.texcoord = IN.texcoord;// +offset * 0.02f;
			OUT.color = IN.color;
	#ifdef PIXELSNAP_ON
			OUT.vertex = UnityPixelSnap(OUT.vertex);
	#endif

			return OUT;
		}

		sampler2D _MainTex;
		uniform half4 _MainTex_TexelSize;
		sampler2D _AlphaTex;

		fixed4 SampleSpriteTexture(float2 uv)
		{
			fixed4 color = tex2D(_MainTex, uv);
			color.rgb = (1, 1, 0);
	#if ETC1_EXTERNAL_ALPHA
			// get the color from an external texture (usecase: Alpha support for ETC1 on android)
			color.a = tex2D(_AlphaTex, uv).r;
	#endif //ETC1_EXTERNAL_ALPHA

			return color;
		}

		static float blurAmount = 0.01f;
		static const float2 Offset[8] =
		{
			float2(0.5,0.5),
			float2(-0.5,0.5),
			float2(-0.5,-0.5),
			float2(0.5,-0.5),
			float2(1.0, 0),
			float2(0, 1.0),
			float2(-1.0, 0),
			float2(0, -1.0),
		};

		fixed4 frag(v2f IN) : SV_Target
		{
			fixed4 c = half4(0, 0, 0, 0);
		for (int j = 0; j < 8; j++) {
			float2 coordinate = IN.texcoord + _MainTex_TexelSize.xy * Offset[j] * 4;
			c += tex2D(_MainTex, float2(coordinate.x - 3 * blurAmount, coordinate.y)) * 0.0105;
			c += tex2D(_MainTex, float2(coordinate.x - 2 * blurAmount, coordinate.y)) * 0.0405;
			c += tex2D(_MainTex, float2(coordinate.x - 1 * blurAmount, coordinate.y)) * 0.116;
			c += tex2D(_MainTex, float2(coordinate.x, coordinate.y)) * 0.162;
			c += tex2D(_MainTex, float2(coordinate.x + 1 * blurAmount, coordinate.y)) * 0.116;
			c += tex2D(_MainTex, float2(coordinate.x + 2 * blurAmount, coordinate.y)) * 0.0405;
			c += tex2D(_MainTex, float2(coordinate.x + 3 * blurAmount, coordinate.y)) * 0.0105;
		}

		c.rgb = _Color.rgb;
		c.rgb *= (c.a * _Color.a / 8);
		return c;
		}
		ENDCG
	}


				Pass
		{
			CGPROGRAM
	#pragma vertex vert
	#pragma fragment frag
	#pragma multi_compile _ PIXELSNAP_ON
	#pragma shader_feature ETC1_EXTERNAL_ALPHA
	#include "UnityCG.cginc"

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
			float2 texcoord  : TEXCOORD0;
		};

		fixed4 _Color;

		v2f vert(appdata_t IN)
		{
			v2f OUT;
			OUT.vertex = UnityObjectToClipPos(IN.vertex);
			float2 offset = IN.texcoord - float2(0.5f, 0.5f);
			OUT.texcoord = IN.texcoord;// +offset * 0.02f;
			OUT.color = IN.color;
	#ifdef PIXELSNAP_ON
			OUT.vertex = UnityPixelSnap(OUT.vertex);
	#endif

			return OUT;
		}

		sampler2D _MainTex;
		uniform half4 _MainTex_TexelSize;
		sampler2D _AlphaTex;

		fixed4 SampleSpriteTexture(float2 uv)
		{
			fixed4 color = tex2D(_MainTex, uv);
			color.rgb = (1, 1, 0);
	#if ETC1_EXTERNAL_ALPHA
			// get the color from an external texture (usecase: Alpha support for ETC1 on android)
			color.a = tex2D(_AlphaTex, uv).r;
	#endif //ETC1_EXTERNAL_ALPHA

			return color;
		}

		static float blurAmount = 0.01f;

		static const float2 Offset[8] =
		{
			float2(0.5,0.5),
			float2(-0.5,0.5),
			float2(-0.5,-0.5),
			float2(0.5,-0.5),
			float2(1.0, 0),
			float2(0, 1.0),
			float2(-1.0, 0),
			float2(0, -1.0),
		};

		fixed4 frag(v2f IN) : SV_Target
		{
			fixed4 c = half4(0, 0, 0, 0);

			for (int j = 0; j < 8; j++) {
				float2 coordinate = IN.texcoord + _MainTex_TexelSize.xy * Offset[j] * 4;
				c += tex2D(_MainTex, float2(coordinate.x, coordinate.y - 3 * blurAmount)) * 0.0105;
				c += tex2D(_MainTex, float2(coordinate.x, coordinate.y - 2 * blurAmount)) * 0.0405;
				c += tex2D(_MainTex, float2(coordinate.x, coordinate.y - 1 * blurAmount)) * 0.116;
				c += tex2D(_MainTex, float2(coordinate.x, coordinate.y)) * 0.162;
				c += tex2D(_MainTex, float2(coordinate.x, coordinate.y + 1 * blurAmount)) * 0.116;
				c += tex2D(_MainTex, float2(coordinate.x, coordinate.y + 2 * blurAmount)) * 0.0405;
				c += tex2D(_MainTex, float2(coordinate.x, coordinate.y + 3 * blurAmount)) * 0.0105;
			}

			c.rgb = _Color.rgb;
			c.rgb *= (c.a * _Color.a / 8);
			return c;
		}
		ENDCG
		}

		}
}