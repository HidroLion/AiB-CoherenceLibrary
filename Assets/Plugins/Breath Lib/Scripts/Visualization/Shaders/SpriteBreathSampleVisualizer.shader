// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Sprites/Default"
{
	Properties
	{
		[PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
		_Color ("Tint", Color) = (1,1,1,1)
		[MaterialToggle] PixelSnap ("Pixel snap", Float) = 0

		_AirInLungs ( "Air In Lungs", Float) = 0
		_No ( "No", Float) = 0
		_Nasal ( "Nasal", Float) = 0
		_Pitch ( "Pitch", Float) = 0
		_Volume ( "Volume", Float) = 0

		_PitchColorA ("Pitch Color A", Color) = (0,0,0,1)
		_PitchColorB ("Pitch Color B", Color) = (1,1,1,1)

		_Frequency ("Frequency", Float) = 1
		_Amplifier ("Amplifier", Float) = 0.4
	}

	SubShader
	{
		Tags
		{ 
			"Queue"="Transparent" 
			"IgnoreProjector"="True" 
			"RenderType"="Transparent" 
			"PreviewType"="Plane"
			"CanUseSpriteAtlas"="True"
		}

		Cull Off
		Lighting Off
		ZWrite Off
		Blend One OneMinusSrcAlpha

		Pass
		{
		CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile _ PIXELSNAP_ON
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
				fixed4 color    : COLOR;
				float2 texcoord  : TEXCOORD0;
			};
			
			fixed4 _Color;
			fixed4 _PitchColorA;
			fixed4 _PitchColorB;

			float _AirInLungs;
			float _No;
			float _Nasal;
			float _Pitch;
			float _Volume;
			float _Frequency;
			float _Amplifier;

			v2f vert(appdata_t IN)
			{
				v2f OUT;
				OUT.vertex = UnityObjectToClipPos(IN.vertex);
				OUT.texcoord = IN.texcoord;
				OUT.color = IN.color * _Color;


				#ifdef PIXELSNAP_ON
				OUT.vertex = UnityPixelSnap (OUT.vertex);
				#endif

				return OUT;
			}

			sampler2D _MainTex;
			sampler2D _AlphaTex;
			float _AlphaSplitEnabled;

			fixed4 SampleSpriteTexture (float2 uv)
			{
				fixed4 color = tex2D (_MainTex, uv);

#if UNITY_TEXTURE_ALPHASPLIT_ALLOWED
				if (_AlphaSplitEnabled)
					color.a = tex2D (_AlphaTex, uv).r;
#endif //UNITY_TEXTURE_ALPHASPLIT_ALLOWED

				return color;
			}

			fixed4 frag(v2f IN) : SV_Target
			{
				float2 center = float2(0.5, 0.5);
				float2 offset = IN.texcoord - center;

				float angle = atan2(offset.y, offset.x);
				float volume = (cos(angle * _Frequency + _Time.x) + 1) / 2;
				// volume = volume * _Volume * _Amplifier;
				// volume = lerp(0.8, 1, (1 - volume));

				volume = 1 - volume;

				volume = lerp(0, _Volume, pow(volume, 0.2));



				float distanceToCenter = length(offset);

				float scale = 0.15 + _AirInLungs * 0.85;
				scale = scale - volume;

				if (scale < 0)
				{
					scale = 0;
				}

				fixed4 pitchColor = lerp(_PitchColorA, _PitchColorB, _Pitch);

				fixed4 c = SampleSpriteTexture(IN.texcoord) * IN.color * pitchColor;

				if (distanceToCenter > scale)
				{
					c.a = 0;
				}

				c.rgb *= c.a;
				return c;
			}
		ENDCG
		}
	}
}