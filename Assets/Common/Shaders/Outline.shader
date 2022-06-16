Shader "Outline" {
    Properties {
		_Color("Color", Color) = (1,1,1,1)
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_Outline("Outline Thickness", Range(0.1, 4)) = 0.5
	}
 
	CGINCLUDE 
	#include "UnityCG.cginc"
	#include "Lighting.cginc"
	#include "AutoLight.cginc"

	sampler2D _MainTex;
	fixed4 _MainTex_ST;
	fixed4 _Color;	
	fixed _Outline; 
 
	struct appdata {
		half4 vertex : POSITION;
		half4 texcoord : TEXCOORD0;
		half3 normal : NORMAL;
	};	
 
	struct v2f {
		UNITY_POSITION(pos);
		float2 uv : TEXCOORD0;
		float3 worldPos : TEXCOORD1;
		float3 worldNormal : TEXCOORD2;
	};
	ENDCG
 
	SubShader 
	{
		Tags {
			"RenderType"="Transparent"
			"Queue" = "Transparent"
		}
		
		Pass{
			Name "Outline"
 
			ZTest GEqual
			Blend SrcAlpha OneMinusSrcAlpha
 
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag	
 
			v2f vert(appdata v)
			{
				v2f o;
				o.pos = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
				o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;	
				o.worldNormal = UnityObjectToWorldNormal(v.normal);
				return o;
			}
 
			fixed4 frag(v2f i) : COLOR
			{
				fixed3 worldViewDir = normalize(UnityWorldSpaceViewDir(i.worldPos));
				fixed t = 1 - dot(i.worldNormal, worldViewDir);

				fixed4 o = tex2D(_MainTex, i.uv.xy) * _Color;
				o.a = pow(t, 1 / _Outline);
				return o;
			}
			ENDCG
		}
 
		Pass 
		{
			Name "Base"
 
			ZTest Less

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			v2f vert (appdata_full v) {
				v2f o;
				o.pos = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
				o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
				o.worldNormal = UnityObjectToWorldNormal(v.normal);
				UNITY_TRANSFER_FOG(o,o.pos);
				return o;
			}

			fixed4 frag (v2f i) : SV_Target {
				UNITY_EXTRACT_FOG(i);
				fixed3 lightDir = normalize(UnityWorldSpaceLightDir(i.worldPos));
				float3 worldViewDir = normalize(UnityWorldSpaceViewDir(i.worldPos));
				worldViewDir = normalize(worldViewDir + lightDir);

				SurfaceOutput o;
				UNITY_INITIALIZE_OUTPUT(SurfaceOutput,o);
				o.Albedo = tex2D(_MainTex, i.uv) * _Color;
				o.Normal = i.worldNormal;

				UNITY_LIGHT_ATTENUATION(atten, i, i.worldPos)

				UnityGI gi;
				UNITY_INITIALIZE_OUTPUT(UnityGI, gi);
				gi.indirect.diffuse = 0;
				gi.indirect.specular = 0;
				gi.light.color = _LightColor0.rgb;
				gi.light.dir = lightDir;

				UnityGIInput giInput;
				UNITY_INITIALIZE_OUTPUT(UnityGIInput, giInput);
				giInput.light = gi.light;
				giInput.worldPos = i.worldPos;
				giInput.worldViewDir = worldViewDir;
				giInput.atten = atten;
				giInput.lightmapUV = 0.0;
				giInput.ambient.rgb = 0.0;
				giInput.probeHDR[0] = unity_SpecCube0_HDR;
				giInput.probeHDR[1] = unity_SpecCube1_HDR;
				LightingBlinnPhong_GI(o, giInput, gi);

				fixed4 c = LightingBlinnPhong (o, worldViewDir, gi);
				UNITY_APPLY_FOG(_unity_fogCoord, c);
				return c;
			}
			ENDCG
		}
	} 
}