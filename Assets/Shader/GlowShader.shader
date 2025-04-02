Shader "Custom/GlowShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}  // 主纹理
        _GlowColor ("Glow Color", Color) = (1,1,1,1)  // 发光颜色
        _GlowIntensity ("Glow Intensity", Range(0, 10)) = 1  // 发光强度
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;  // 主纹理
            float4 _GlowColor;   // 发光颜色
            float _GlowIntensity;  // 发光强度

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // 采样主纹理
                fixed4 texColor = tex2D(_MainTex, i.uv);
                // 计算自发光效果
                fixed4 glow = _GlowColor * _GlowIntensity;
                // 将主纹理颜色与发光颜色混合
                fixed4 finalColor = texColor + glow;
                return finalColor;
            }
            ENDCG
        }
    }
}
