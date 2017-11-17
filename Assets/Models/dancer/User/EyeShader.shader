Shader "Custom/EyeShader" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_EyePower("Eye Power",Range(0,4)) = 1
		_EyeGloss("Eye Gloss",2D) = "black" {}
		_Cubemap("CubeMap", CUBE) = ""{}
		_SpecularColor ("Specular Color", Color) = (1,1,1,1)
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		_LightPower("Light Power",Range(1,50)) = 0.5
		_Wrap("Wrap",Range(0,2)) = 0.0
		_OffsetRatio("Reflect Ratio",Range(0,1)) = 0.0
		_Smooth("Gloss Smooth",Range(0,1)) = 0.5
		_RefLerp("Reflect Lerp",Range(0,1)) = 0
		_CubemapRate("Cubemap Rate",Range(0,2)) = 1
	}
	SubShader {
		Tags { "RenderType"="Opaque" "IgnoreProjector"="True"}
		LOD 200
		
		//ZWrite On 

		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf SSSCustom 

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0

		sampler2D _MainTex;
		sampler2D _EyeGloss;
		samplerCUBE _Cubemap;

		struct Input {
			float2 uv_MainTex;
		};

		struct SurfaceOutputSSS
		{
			float2 UV;
		    fixed3 Albedo;      
		    fixed3 Normal;      
		    half3  Emission;
		    fixed  Alpha;        // alpha for transparencies
		};

		half 	_Glossiness;
		half 	_Metallic;
		fixed4 	_Color;
		fixed4 	_SpecularColor;
		fixed4  _SSSColor;
		half   	_LightPower;
		half 	_Wrap;
		half    _OffsetRatio;
		half 	_Smooth;
		half    _RefLerp;
		half	_CubemapRate;
		half 	_EyePower;

		// Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
		// See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
		// #pragma instancing_options assumeuniformscaling
		UNITY_INSTANCING_CBUFFER_START(Props)
			// put more per-instance properties here
		UNITY_INSTANCING_CBUFFER_END

		void surf (Input IN, inout SurfaceOutputSSS o) {
			// Albedo comes from a texture tinted by color
			fixed4 c = tex2D (_MainTex, IN.uv_MainTex ) * _Color;
			o.Albedo = c.rgb;
			o.UV = IN.uv_MainTex;
			o.Alpha = c.a;
		}


		inline half3 reflect1(half3 L,half3 N,half ratio)
		{
			half d = dot(N,L);
			return (2 * d + ratio * 0.0134) * N - L;
		}
	
		inline half4 LightingSSSCustom (inout SurfaceOutputSSS s, half3 lightDir, half3 viewDir, half atten)
		{
			half3 normal = normalize(s.Normal);
			half3 lightDirNor = normalize(viewDir);
			half3 lightDirNor2 = normalize(lightDir);
			half3 viewDirNor = normalize(viewDir);
			
			half3 reflDir = reflect1(lightDirNor2, normal,_OffsetRatio);
			//half3 reflDir = reflect(lightDirNor, normal);

			half  nl = saturate(dot(normal, lightDirNor));
			half  nl2 = saturate(dot(normal, lightDirNor2));

			half  diff = (nl + _Wrap) / (1 + _Wrap);
			half  spec =  pow(saturate(abs(dot(viewDirNor,reflDir))), _LightPower);
			
			fixed3 glossTex = tex2D (_EyeGloss, s.UV + float2(0.3,0) - viewDir * 0.5 - lightDirNor2 * 0.25).rgb;

			fixed3 ramp = 1 - smoothstep(spec - _Smooth, spec + _Smooth , nl);

			half  gloss = pow(saturate(0.1 + abs(dot(viewDirNor,reflDir))), 1.5);

			float3 ref = texCUBE(_Cubemap, float4(lightDirNor, 1.0)).rgb;
			float3 ref2 = texCUBE(_Cubemap, float4(viewDirNor, 1.0)).rgb;

			half3 diffuseColor =  min(1,diff + gloss) * _CubemapRate * (2 * ref + ref2) * _LightColor0.rgb * s.Albedo * atten;
			half3 specularColor = (glossTex + 2*(ramp * _EyePower) * pow(nl,16))* _SpecularColor.rgb *_LightColor0.rgb * atten ;
			
			half3 color = (min(diffuseColor + specularColor,1)) * min(nl2 + 0.5,1) ;

			return half4(color,s.Alpha);
		}

		ENDCG
	}
	FallBack "Diffuse"
}
