Shader "CF/PostEffect/Edge Detection Normals And Depth 4Dir" {
    Properties {
        _MainTex ("Base (RGB)", 2D) = "white" {}
        _EdgeOnly ("Edge Only", Float) = 1.0
        [HDR]_EdgeColor ("Edge Color", Color) = (0, 0, 0, 1)
        _BackgroundColor ("Background Color", Color) = (1, 1, 1, 1)
        _SampleDistance ("Sample Distance", Float) = 1.0
        _Sensitivity ("Sensitivity", Vector) = (1, 1, 0, 0)
        _MainTex_TexelSize ("MainTex_TexelSize", Vector) = (1,1,0,0)
    }

    SubShader {
        Tags { "RenderType"="Opaque" }
        LOD 100

        CGINCLUDE
        #include "UnityCG.cginc"

        sampler2D _MainTex;
        half4 _MainTex_TexelSize;
        fixed _EdgeOnly;
        fixed4 _EdgeColor;
        fixed4 _BackgroundColor;
        float _SampleDistance;
        half4 _Sensitivity;

        sampler2D _CameraDepthNormalsTexture;

        struct v2f {
            float4 pos : SV_POSITION;
            half2 uv[5] : TEXCOORD0;
        };

        v2f vert(appdata_img v) {
            v2f o;
            o.pos = UnityObjectToClipPos(v.vertex);

            half2 uv = v.texcoord;

            #if UNITY_UV_STARTS_AT_TOP
            if (_MainTex_TexelSize.y < 0)
                uv.y = 1.0 - uv.y;
            #endif

            o.uv[0] = uv; // center
            o.uv[1] = uv + _MainTex_TexelSize.xy * half2(1, 0) * _SampleDistance;  // right
            o.uv[2] = uv + _MainTex_TexelSize.xy * half2(-1, 0) * _SampleDistance; // left
            o.uv[3] = uv + _MainTex_TexelSize.xy * half2(0, 1) * _SampleDistance;  // up
            o.uv[4] = uv + _MainTex_TexelSize.xy * half2(0, -1) * _SampleDistance; // down

            return o;
        }

        // 法线与深度相似性判断
        half CheckSame(half4 sample1, half4 sample2) {
            half2 normal1 = sample1.xy;
            float depth1 = DecodeFloatRG(sample1.zw);
            half2 normal2 = sample2.xy;
            float depth2 = DecodeFloatRG(sample2.zw);

            if (depth1 <= 0.0001 || depth2 <= 0.0001)
                return 1.0;

            half2 diffNormal = abs(normal1 - normal2) * _Sensitivity.x;
            float diffDepth = abs(depth1 - depth2) * _Sensitivity.y;

            return ((diffNormal.x + diffNormal.y) < 0.1 && diffDepth < 0.01) ? 1.0 : 0.0;
        }

        fixed4 frag(v2f i) : SV_Target {
            half4 center = tex2D(_CameraDepthNormalsTexture, i.uv[0]);
            half4 right  = tex2D(_CameraDepthNormalsTexture, i.uv[1]);
            half4 left   = tex2D(_CameraDepthNormalsTexture, i.uv[2]);
            half4 up     = tex2D(_CameraDepthNormalsTexture, i.uv[3]);
            half4 down   = tex2D(_CameraDepthNormalsTexture, i.uv[4]);

            // 四方向边缘判断
            half edge = 1.0;
            edge *= CheckSame(center, right);
            edge *= CheckSame(center, left);
            edge *= CheckSame(center, up);
            edge *= CheckSame(center, down);

            fixed4 onlyEdgeColor = lerp(_EdgeColor, _BackgroundColor, edge);
            fixed4 withEdgeColor = lerp(_EdgeColor, tex2D(_MainTex, i.uv[0]), edge);
            return lerp(withEdgeColor, onlyEdgeColor, _EdgeOnly);
        }

        ENDCG

        Pass {
            ZTest Always Cull Off ZWrite Off

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            ENDCG
        }
    }

    FallBack Off
}