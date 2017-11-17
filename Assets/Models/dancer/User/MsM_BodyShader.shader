// Toony Colors Pro+Mobile 2
// (c) 2014-2017 Jean Moreno

Shader "Toony Colors Pro 2/User/MsM_BodyShader"
{
	Properties
	{
	[TCP2HeaderHelp(BASE, Base Properties)]
		//TOONY COLORS
		_Color ("Color", Color) = (0.5,0.5,0.5,1.0)
		_HColor ("Highlight Color", Color) = (0.6,0.6,0.6,1.0)
		_SColor ("Shadow Color", Color) = (0.3,0.3,0.3,1.0)
		
		//DIFFUSE
		_MainTex ("Main Texture (RGB)", 2D) = "white" {}
	[TCP2Separator]
		
		//TOONY COLORS RAMP
		_RampThreshold ("Ramp Threshold", Range(0,1)) = 0.5
		_RampSmooth ("Ramp Smoothing", Range(0.001,1)) = 0.1
	[TCP2Separator]
	
	[Header(Masks)]
		[NoScaleOffset]
		_Mask1 ("Mask 1 (Specular,Reflection)", 2D) = "black" {}
		_EmissionMap ("Emission Map", 2D) = "black" {}
		_EmissionColor("Emission Color", Color) = (1,1,1)
		_EmissionPower("Emission Power",Float) = 1
	[TCP2Separator]
	
	[TCP2HeaderHelp(NORMAL MAPPING, Normal Bump Map)]
		//BUMP
		_BumpMap ("Normal map (RGB)", 2D) = "bump" {}
	[TCP2Separator]
	
	[TCP2HeaderHelp(SPECULAR, Specular)]
		//SPECULAR
		_SpecColor ("Specular Color", Color) = (0.5, 0.5, 0.5, 1)
		_Shininess ("Shininess", Range(0.0,2)) = 0.1
		_SpecSmooth ("Smoothness", Range(0,1)) = 0.05
	[TCP2Separator]
	
	[TCP2HeaderHelp(REFLECTION, Reflection)]
		//REFLECTION
		[NoScaleOffset] _Cube ("Reflection Cubemap", Cube) = "_Skybox" {}
		_ReflectColor ("Reflection Color (RGB) Strength (Alpha)", Color) = (1,1,1,0.5)
		_ReflectRoughness ("Reflection Roughness", Range(0,9)) = 0
	[TCP2Separator]
	
	[TCP2HeaderHelp(TRANSPARENCY)]
		//Alpha Testing
		_Cutoff ("Alpha cutoff", Range(0,1)) = 0.5
		_ColorContrast("Color Contrast", Range(1,2)) = 1.5
		//Avoid compile error if the properties are ending with a drawer
		[HideInInspector] __dummy__ ("unused", Float) = 0
	}
	
	SubShader
	{
		Tags {"Queue"="Geometry" "RenderType"="Opaque"}

		ZWrite On

		CGPROGRAM
		
		#pragma surface surf ToonyColorsCustom nofog fullforwardshadows halfasview addshadow //noforwardadd 
		#pragma target 3.0
		#pragma multi_compile TCP2_SPEC_TOON

		//================================================================
		// VARIABLES
		
		#include "UnityPBSLighting.cginc"

		fixed4 _Color;
		sampler2D _MainTex;
		sampler2D _Mask1;
		samplerCUBE _Cube;
		fixed4 _ReflectColor;
		fixed _ReflectRoughness;
		sampler2D _BumpMap;
		sampler2D _EmissionMap;
		fixed3 _EmissionColor;
		fixed  _EmissionPower;
		fixed _Shininess;
		fixed _Cutoff;
		fixed _ColorContrast;
		
		struct Input
		{
			half2 uv_MainTex;
			half2 uv_BumpMap;
			float3 worldRefl;
			INTERNAL_DATA
		};
		
		//================================================================
		// CUSTOM LIGHTING
		
		//Lighting-related variables
		fixed4 _HColor;
		fixed4 _SColor;
		float _RampThreshold;
		float _RampSmooth;
		fixed _SpecSmooth;
		
		
		//Custom SurfaceOutput
		struct SurfaceOutputCustom
		{
			fixed3 Albedo;
			fixed3 Normal;
			float3 WorldPos;
			fixed3 Emission;
			half   Specular;
			fixed  Gloss;
			fixed  Alpha;
			fixed3 EmissionMask;
		};
		
		inline half4 LightingToonyColorsCustom (inout SurfaceOutputCustom s,half3 viewDir, UnityGI gi)
		{
			half3 lightDir = gi.light.dir;

			s.Normal = normalize(s.Normal);
			fixed ndl = max(0, dot(s.Normal, lightDir)*0.5 + 0.5);
			fixed3 ramp = smoothstep(_RampThreshold-_RampSmooth*0.5, _RampThreshold+_RampSmooth*0.5, ndl);

			_SColor = lerp(_HColor, _SColor, _SColor.a);
			ramp = lerp(_SColor.rgb, _HColor.rgb, ramp);
			//Specular
			half3 h = normalize(lightDir + viewDir);
			float ndh = max(0, dot (s.Normal, h));
			float spec = pow(ndh, s.Specular*128.0) * s.Gloss * 2.0;
			spec = smoothstep(0.5-_SpecSmooth*0.5, 0.5+_SpecSmooth*0.5, spec);

			fixed4 c;
			c.rgb = s.Albedo * (gi.light.color + gi.indirect.diffuse) * ramp;
			
			c.rgb += gi.indirect.specular * _SpecColor.rgb * spec;
			c.a = s.Alpha + _SpecColor.a * spec;

			c.rgb *= _ColorContrast;

			c.rgb += s.EmissionMask * _EmissionColor * _EmissionPower;

			return c;

		}

		inline void LightingToonyColorsCustom_GI (inout SurfaceOutputCustom s,UnityGIInput data,inout UnityGI gi)
		{
		#if defined(UNITY_PASS_DEFERRED) && UNITY_ENABLE_REFLECTION_BUFFERS
			    gi = UnityGlobalIllumination(data, 1, s.Normal);
			#else
			    Unity_GlossyEnvironmentData g = UnityGlossyEnvironmentSetup(1, data.worldViewDir, s.Normal, s.Albedo);
			    gi = UnityGlobalIllumination(data, 1, s.Normal, g);
		#endif
		}

		//================================================================
		// SURFACE FUNCTION

		void surf(Input IN, inout SurfaceOutputCustom o)
		{
			fixed4 mainTex = tex2D(_MainTex, IN.uv_MainTex);
			
			//Masks
			fixed4 mask1 = tex2D(_Mask1, IN.uv_MainTex);
			o.EmissionMask = tex2D(_EmissionMap, IN.uv_MainTex).rgb;

			o.Albedo = mainTex.rgb * _Color.rgb;
			o.Alpha = mainTex.a * _Color.a;
			
			//Cutout (Alpha Testing)
			clip (o.Alpha - _Cutoff);
			
			//Specular
			o.Gloss = mask1.r;
			o.Specular = _Shininess;
			
			//Normal map
			half4 normalMap = tex2D(_BumpMap, IN.uv_BumpMap.xy);
			o.Normal = UnpackNormal(normalMap);
			
			//Reflection
			half3 worldRefl = WorldReflectionVector(IN, o.Normal);
			fixed4 reflColor = texCUBElod(_Cube, half4(worldRefl.xyz, _ReflectRoughness));
			reflColor *= mask1.g;
			reflColor.rgb *= _ReflectColor.rgb * _ReflectColor.a;
			o.Emission += reflColor.rgb;
		}

		ENDCG
	}

	Fallback "Diffuse"
	CustomEditor "TCP2_MaterialInspector_SG"
}
