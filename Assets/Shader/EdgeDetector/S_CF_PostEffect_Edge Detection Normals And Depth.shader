Shader "CF/PostEffect/Edge Detection Normals And Depth" {
	Properties {
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_EdgeOnly ("Edge Only", Float) = 1.0
		[HDR]_EdgeColor ("Edge Color", Color) = (0, 0, 0, 1)
		_BackgroundColor ("Background Color", Color) = (1, 1, 1, 1)
		_SampleDistance ("Sample Distance", Float) = 1.0
		_Sensitivity ("Sensitivity", Vector) = (1, 1, 1, 1)
	}
	SubShader {
		CGINCLUDE // 使用CGINCLUDE在pass外面定义代码
		
		#include "UnityCG.cginc"
		
		sampler2D _MainTex;
		//纹素大小，如果_MainTex纹理分辨率是1280x720，那么_MainTex_TexelSize.x = 1/1280 _MainTex_TexelSize.y = 1/720
		half4 _MainTex_TexelSize;  
		fixed _EdgeOnly;
		fixed4 _EdgeColor;
		fixed4 _BackgroundColor;
		float _SampleDistance;
		half4 _Sensitivity;
		
		sampler2D _CameraDepthNormalsTexture;
		
		struct v2f {
			float4 pos : SV_POSITION;
			half2 uv[5]: TEXCOORD0;
		};
		  
		v2f vert(appdata_img v) {
			v2f o;
			o.pos = UnityObjectToClipPos(v.vertex);
			
			half2 uv = v.texcoord;
			o.uv[0] = uv;//纹理采样的位置
			
			#if UNITY_UV_STARTS_AT_TOP
			if (_MainTex_TexelSize.y < 0)
				uv.y = 1 - uv.y;
			#endif
			
			o.uv[1] = uv + _MainTex_TexelSize.xy * half2(1,1) * _SampleDistance;//右上
			o.uv[2] = uv + _MainTex_TexelSize.xy * half2(-1,-1) * _SampleDistance;//左下
			o.uv[3] = uv + _MainTex_TexelSize.xy * half2(-1,1) * _SampleDistance;//左上
			o.uv[4] = uv + _MainTex_TexelSize.xy * half2(1,-1) * _SampleDistance;//右下
					 
			return o;
		}
		
		half CheckSame(half4 sample1, half4 sample2) {
			half2 sample1Normal = sample1.xy;//这里并没有真正解码取到法线值，只是用了xy分量，因为我们只需计算差异程度即可
			float sample1Depth = DecodeFloatRG(sample1.zw);
			half2 sample2Normal = sample2.xy;
			float sample2Depth = DecodeFloatRG(sample2.zw);
			
			// difference in normals
			// do not bother decoding normals - there's no need here
			half2 diffNormal = abs(sample1Normal - sample2Normal) * _Sensitivity.x;
			int isSameNormal = (diffNormal.x + diffNormal.y) < 0.1;//注意这里将bool付给int   0是假  1是真
			// difference in depth
			float diffDepth = abs(sample1Depth - sample2Depth) * _Sensitivity.y;
			// scale the required threshold by the distance
			int isSameDepth = diffDepth < 0.1 * sample1Depth;
			
			// return:
			// 1 - if normals and depth are similar enough 如果法线和深度足够相似
			// 0 - otherwise
			return isSameNormal * isSameDepth ? 1.0 : 0.0; //法线和深度只要有一个差异过大，都认为此处存在边沿
		}
		
		fixed4 fragRobertsCrossDepthAndNormal(v2f i) : SV_Target {
			half4 sample1 = tex2D(_CameraDepthNormalsTexture, i.uv[1]);
			half4 sample2 = tex2D(_CameraDepthNormalsTexture, i.uv[2]);
			half4 sample3 = tex2D(_CameraDepthNormalsTexture, i.uv[3]);
			half4 sample4 = tex2D(_CameraDepthNormalsTexture, i.uv[4]);
			
			half edge = 1.0;
			
			edge *= CheckSame(sample1, sample2);
			edge *= CheckSame(sample3, sample4);
			
			fixed4 withEdgeColor = lerp(_EdgeColor, tex2D(_MainTex, i.uv[0]), edge);
			fixed4 onlyEdgeColor = lerp(_EdgeColor, _BackgroundColor, edge);
			
			return lerp(withEdgeColor, onlyEdgeColor, _EdgeOnly);
		}
		
		ENDCG
		
		Pass { 
			ZTest Always Cull Off ZWrite Off
			
			CGPROGRAM      
			
			#pragma vertex vert  
			#pragma fragment fragRobertsCrossDepthAndNormal
			
			ENDCG  
		}
	} 
	FallBack Off
}
