//这个实现《unity shader入门经典》中P287页中下部分只提了一句的功能：只对指定对象进行描边

Shader "CF/SpecifyObjectOutline/Edge Detection Normals And Depth"
{
    Properties
    {
        [HDR]_EdgeColor ("Edge Color", Color) = (0, 0, 0, 1)
//        _BackgroundColor ("Background Color", Color) = (1, 1, 1, 1)
        _SampleDistance ("Sample Distance", float) = 1.0
        _NormalSensitivity ("_NormalSensitivity", range(0,2)) = 1
        _DepthSensitivity ("_DepthSensitivity", range(0,2)) = 1
        
    }
    SubShader
    {
        Tags
        {
            "Queue" = "Geometry+550"//不透明物体之后绘制
            "RenderType" = "Opaque"
        }
        
        CGINCLUDE// 使用CGINCLUDE在pass外面定义代码

        #include "UnityCG.cginc"

        fixed4 _EdgeColor;
        // fixed4 _BackgroundColor;
        float _SampleDistance;
        float _NormalSensitivity;
        float _DepthSensitivity ;
        
        sampler2D _CameraDepthNormalsTexture;//
        //half4 _CameraDepthNormalsTexturex_TexelSize;//这个应该是不支持的   不是所有纹理后面加一个_TexelSize就能获取它的纹素尺寸
        
        // UNITY_DECLARE_DEPTH_TEXTURE(_CameraDepthTexture);

        struct v2f
        {
            float4 pos : SV_POSITION;
            float4 uv[5]: TEXCOORD0;//修改成float4
            float4 screenPos: TEXCOORD5;
        };

        v2f vert(appdata_img v)
        {
            v2f o;
            //v.vertex  顶点模型空间坐标
            o.pos = UnityObjectToClipPos(v.vertex);

            //计算顶点屏幕位置  返回的是齐次坐标下的屏幕坐标值
            o.screenPos = ComputeScreenPos(o.pos);//关键

            // o.screenPos = o.screenPos / o.screenPos.w;
            
            //TODO  顶点0-1的屏幕坐标
            float4 uv = o.screenPos;
            //中心采样的位置
            o.uv[0] = uv; 

            
            // float size = o.screenPos.w;
            // float2 size = float2(1.0/1920.0,1.0/1080.0);
            float2 size = float2(1.0/1920.0,1.0/1920.0);
            // float2 size = float2(1,1);
            // float2 size = float2(1.0/1080.0,1.0/1920.0);
            // o.uv[1] = float4((uv.xyz + size * fixed3(1, 1, 0) * _SampleDistance),uv.w); //右上
            // o.uv[1] = uv * _SampleDistance;
            //偏移后的采样位置
            o.uv[1] = float4((uv.xy + size * fixed2(1 , 1) * _SampleDistance),uv.zw); //右上fixed2(1 , 1)
            o.uv[2] = float4((uv.xy + size * fixed2(-1,-1) * _SampleDistance),uv.zw); //左下fixed2(-1,-1)
            o.uv[3] = float4((uv.xy + size * fixed2(-1, 1) * _SampleDistance),uv.zw); //左上fixed2(-1, 1)
            o.uv[4] = float4((uv.xy + size * fixed2(1, -1) * _SampleDistance),uv.zw); //右下fixed2(1, -1)

            return o;
        }

        half CheckSame(half4 sample1, half4 sample2)
        {
            //这里并没有真正解码取到法线值，只是用了xy分量，因为我们只需计算差异程度即可
            //注意这里 法线信息被编码到xy通道，深度信息被编码到zw通道
            half2 sample1Normal = sample1.xy; 
            float sample1Depth = DecodeFloatRG(sample1.zw);
            half2 sample2Normal = sample2.xy;
            float sample2Depth = DecodeFloatRG(sample2.zw);

            // difference in normals
            // do not bother decoding normals - there's no need here  法线差异
            half2 diffNormal = abs(sample1Normal - sample2Normal) * _NormalSensitivity;
            int isSameNormal = (diffNormal.x + diffNormal.y) < 0.1; //注意这里将bool付给int   0是假  1是真
            // difference in depth  深度差异
            float diffDepth = abs(sample1Depth - sample2Depth) * _DepthSensitivity;
            // scale the required threshold by the distance
            int isSameDepth = diffDepth < 0.1 * sample1Depth;

            // return:
            // 1 - if normals and depth are similar enough 如果法线和深度足够相似  这里不是边沿
            // 0 - otherwise  这里是边沿
            
            //法线和深度只要有一个差异过大，都认为此处存在边沿,存在边沿返回0
            return isSameNormal * isSameDepth ? 1.0 : 0.0; 
        }

        fixed4 fragRobertsCrossDepthAndNormal(v2f i) : SV_Target
        {
            // float4 s = tex2Dproj(_CameraDepthNormalsTexture,half3(1,1,1));
            // float4 s2 = tex2Dproj(_CameraDepthNormalsTexture,float4(1,10,0,0));
            
            float4 sample1 = tex2Dproj(_CameraDepthNormalsTexture, i.uv[1]);//关键
            float4 sample2 = tex2Dproj(_CameraDepthNormalsTexture, i.uv[2]);
            float4 sample3 = tex2Dproj(_CameraDepthNormalsTexture, i.uv[3]);
            float4 sample4 = tex2Dproj(_CameraDepthNormalsTexture, i.uv[4]);

            half edge = 1.0;
            edge *= CheckSame(sample1, sample2);
            edge *= CheckSame(sample3, sample4);

            
            // half4 sample = tex2D(_CameraDepthNormalsTexture, i.screenPos);
            //取消注释这段代码以显示深度
            // half4 sample = tex2Dproj(_CameraDepthNormalsTexture, i.uv[0]);//i.screenPos
            // float depth = DecodeFloatRG(sample.zw);
            // return fixed4(depth,depth,depth,1);
            
            
            if (edge < 0.5) //这里是边
                return _EdgeColor;    
            else//这里不是边
                discard;//丢弃像素
                // return fixed4(0,0,0,1);
            return 0;
        }
		
		ENDCG

        Pass
        {
            ZTest Always 
//            Cull Off 
//            Cull front
            ZWrite Off

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment fragRobertsCrossDepthAndNormal
            ENDCG
        }
    }
    FallBack Off
}