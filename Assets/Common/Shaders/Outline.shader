Shader "Outline" {
    Properties {
		_Color("Color", Color) = (1,1,1,1)
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_Outline("Outline Thickness", Range(0.1, 4)) = 0.5
	}
 
	CGINCLUDE 
	#include "Lighting.cginc"

	sampler2D _MainTex;
	fixed4 _MainTex_ST;
	fixed3 _Color;	
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
			"RenderType" = "Transparent"
			"Queue" = "Transparent"
		}
		
		Pass{
			Name "Outline"

			ZWrite Off
			ZTest Greater
			Blend SrcAlpha OneMinusSrcAlpha
 
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag	
 
			v2f vert(appdata v)
			{
				v2f o;
				o.pos = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
				o.worldPos = mul(unity_ObjectToWorld, v.vertex);	
				o.worldNormal = UnityObjectToWorldNormal(v.normal);
				return o;
			}
 
			fixed4 frag(v2f i) : COLOR
			{
				fixed3 worldViewDir = normalize(UnityWorldSpaceViewDir(i.worldPos));
				fixed t = 1 - dot(i.worldNormal, worldViewDir);
				fixed4 c;
				c.rgb = tex2D(_MainTex, i.uv) * _Color;
				c.a = pow(t, 1 / _Outline);
				return c;
			}
			ENDCG
		}

		Pass 
		{
			Name "Base"
			
			ZWrite On
			ZTest Less
			Blend One Zero

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			v2f vert (appdata v) {
				v2f o;
				o.pos = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
				o.worldPos = mul(unity_ObjectToWorld, v.vertex);
				o.worldNormal = UnityObjectToWorldNormal(v.normal);
				return o;
			}

			fixed4 frag (v2f i) : COLOR {
				fixed3 lightDir = normalize(UnityWorldSpaceLightDir(i.worldPos));
				fixed diff = max(0, dot (i.worldNormal, lightDir));
				fixed4 c;
				c.rgb = tex2D(_MainTex, i.uv) * _Color * _LightColor0 * diff;
				c.a = 1;
				return c;
			}
			ENDCG
		}
	} 
}