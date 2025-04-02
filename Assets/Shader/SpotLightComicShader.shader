// Made with Amplify Shader Editor v1.9.7.1
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "New Amplify Shader"
{
	Properties
	{
		_Shading1("Shading1", Float) = 0.1
		_Shading2("Shading2", Float) = 0.4
		_Scale("Scale", Float) = 8.12
		_DotSize("DotSize", Float) = 0.5
		_Float2("Float 2", Float) = 0.8
		_Texture0("Texture 0", 2D) = "white" {}
		_DotColor("DotColor", Color) = (0,0,0,0)
		_MyLightDirWorld("_MyLightDirWorld", Vector) = (0,0,0,0)
		[HideInInspector] _texcoord( "", 2D ) = "white" {}

	}
	
	SubShader
	{
		
		
		Tags { "RenderType"="Opaque" }
	LOD 100

		CGINCLUDE
		#pragma target 3.0
		ENDCG
		Blend Off
		AlphaToMask Off
		Cull Back
		ColorMask RGBA
		ZWrite On
		ZTest LEqual
		Offset 0 , 0
		
		
		
		Pass
		{
			Name "Unlit"

			CGPROGRAM

			#define ASE_VERSION 19701


			#ifndef UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX
			//only defining to not throw compilation error over Unity 5.5
			#define UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input)
			#endif
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile_instancing
			#include "UnityCG.cginc"
			#include "UnityShaderVariables.cginc"


			struct appdata
			{
				float4 vertex : POSITION;
				float4 color : COLOR;
				float4 ase_texcoord : TEXCOORD0;
				float3 ase_normal : NORMAL;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};
			
			struct v2f
			{
				float4 vertex : SV_POSITION;
				#ifdef ASE_NEEDS_FRAG_WORLD_POSITION
				float3 worldPos : TEXCOORD0;
				#endif
				float4 ase_texcoord1 : TEXCOORD1;
				float4 ase_texcoord2 : TEXCOORD2;
				float3 ase_normal : NORMAL;
				float4 ase_texcoord3 : TEXCOORD3;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};

			uniform sampler2D _Texture0;
			uniform float4 _Texture0_ST;
			uniform float4 _DotColor;
			uniform float _Scale;
			uniform float _Float2;
			uniform float _DotSize;
			uniform float _Shading1;
			uniform float _Shading2;
			uniform float3 _MyLightDirWorld;

			
			v2f vert ( appdata v )
			{
				v2f o;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
				UNITY_TRANSFER_INSTANCE_ID(v, o);

				float4 ase_clipPos = UnityObjectToClipPos(v.vertex);
				float4 screenPos = ComputeScreenPos(ase_clipPos);
				o.ase_texcoord2 = screenPos;
				float3 ase_worldNormal = UnityObjectToWorldNormal(v.ase_normal);
				o.ase_texcoord3.xyz = ase_worldNormal;
				
				o.ase_texcoord1.xy = v.ase_texcoord.xy;
				o.ase_normal = v.ase_normal;
				
				//setting value to unused interpolator channels and avoid initialization warnings
				o.ase_texcoord1.zw = 0;
				o.ase_texcoord3.w = 0;
				float3 vertexValue = float3(0, 0, 0);
				#if ASE_ABSOLUTE_VERTEX_POS
				vertexValue = v.vertex.xyz;
				#endif
				vertexValue = vertexValue;
				#if ASE_ABSOLUTE_VERTEX_POS
				v.vertex.xyz = vertexValue;
				#else
				v.vertex.xyz += vertexValue;
				#endif
				o.vertex = UnityObjectToClipPos(v.vertex);

				#ifdef ASE_NEEDS_FRAG_WORLD_POSITION
				o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
				#endif
				return o;
			}
			
			fixed4 frag (v2f i ) : SV_Target
			{
				UNITY_SETUP_INSTANCE_ID(i);
				UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);
				fixed4 finalColor;
				#ifdef ASE_NEEDS_FRAG_WORLD_POSITION
				float3 WorldPosition = i.worldPos;
				#endif
				float2 uv_Texture0 = i.ase_texcoord1.xy * _Texture0_ST.xy + _Texture0_ST.zw;
				float4 temp_cast_0 = (1.0).xxxx;
				float4 screenPos = i.ase_texcoord2;
				float4 ase_screenPosNorm = screenPos / screenPos.w;
				ase_screenPosNorm.z = ( UNITY_NEAR_CLIP_VALUE >= 0 ) ? ase_screenPosNorm.z : ase_screenPosNorm.z * 0.5 + 0.5;
				float cos58 = cos( _Float2 );
				float sin58 = sin( _Float2 );
				float2 rotator58 = mul( ( _Scale * ase_screenPosNorm ).xy - float2( 0,0 ) , float2x2( cos58 , -sin58 , sin58 , cos58 )) + float2( 0,0 );
				float3 normalizeResult5 = normalize( i.ase_normal );
				float3 objToWorldDir105 = mul( unity_ObjectToWorld, float4( _MyLightDirWorld, 0 ) ).xyz;
				float3 normalizeResult106 = normalize( objToWorldDir105 );
				float3 ase_worldNormal = i.ase_texcoord3.xyz;
				float3 normalizeResult117 = normalize( ase_worldNormal );
				float dotResult109 = dot( normalizeResult106 , normalizeResult117 );
				float normalizeResult6 = normalize( saturate( dotResult109 ) );
				float3 temp_cast_2 = (normalizeResult6).xxx;
				float dotResult4 = dot( normalizeResult5 , temp_cast_2 );
				float smoothstepResult53 = smoothstep( _Shading1 , _Shading2 , saturate( dotResult4 ));
				float temp_output_55_0 = ( 1.0 - smoothstepResult53 );
				float2 appendResult11_g1 = (float2(( _DotSize * temp_output_55_0 ) , ( ( _DotSize * ( _ScreenParams.x / _ScreenParams.y ) ) * temp_output_55_0 )));
				float temp_output_17_0_g1 = length( ( (frac( rotator58 )*2.0 + -1.0) / appendResult11_g1 ) );
				float4 lerpResult64 = lerp( _DotColor , temp_cast_0 , ( 1.0 - saturate( ( ( 1.0 - temp_output_17_0_g1 ) / fwidth( temp_output_17_0_g1 ) ) ) ));
				
				
				finalColor = ( tex2D( _Texture0, uv_Texture0 ) * lerpResult64 );
				return finalColor;
			}
			ENDCG
		}
	}
	CustomEditor "ASEMaterialInspector"
	
	Fallback Off
}
/*ASEBEGIN
Version=19701
Node;AmplifyShaderEditor.Vector3Node;104;-3424,416;Inherit;False;Property;_MyLightDirWorld;_MyLightDirWorld;7;0;Create;True;0;0;0;False;0;False;0,0,0;0,0,0;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.TransformDirectionNode;105;-2896,432;Inherit;False;Object;World;False;Fast;False;1;0;FLOAT3;0,0,0;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.WorldNormalVector;115;-2832,688;Inherit;False;False;1;0;FLOAT3;0,0,1;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.NormalizeNode;106;-2672,432;Inherit;False;False;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.NormalizeNode;117;-2592,688;Inherit;False;False;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.DotProductOpNode;109;-2368,432;Inherit;False;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;110;-2224,432;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.NormalVertexDataNode;2;-2320,144;Inherit;False;0;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.NormalizeNode;5;-2032,144;Inherit;False;False;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.NormalizeNode;6;-2032,432;Inherit;False;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.DotProductOpNode;4;-1760,224;Inherit;False;2;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ScreenParams;45;-1408,-112;Inherit;False;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SaturateNode;7;-1584,240;Inherit;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;11;-1392,416;Inherit;False;Property;_Shading2;Shading2;1;0;Create;True;0;0;0;False;0;False;0.4;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;9;-1392,336;Inherit;False;Property;_Shading1;Shading1;0;0;Create;True;0;0;0;False;0;False;0.1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.ScreenPosInputsNode;36;-1584,-528;Float;True;0;False;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;38;-1504,-672;Inherit;False;Property;_Scale;Scale;2;0;Create;True;0;0;0;False;0;False;8.12;100;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleDivideOpNode;46;-1184,-80;Inherit;True;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;44;-1152,-240;Inherit;False;Property;_DotSize;DotSize;3;0;Create;True;0;0;0;False;0;False;0.5;2;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SmoothstepOpNode;53;-1168,224;Inherit;True;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;39;-1328,-640;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT4;0,0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.RangedFloatNode;59;-1232,-480;Inherit;False;Property;_Float2;Float 2;4;0;Create;True;0;0;0;False;0;False;0.8;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;55;-944,208;Inherit;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;47;-880,-192;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RotatorNode;58;-992,-608;Inherit;False;3;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;2;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;54;-592,-48;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;56;-640,-288;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.FractNode;37;-704,-560;Inherit;True;1;0;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.FunctionNode;40;-432,-512;Inherit;True;Ellipse;-1;;1;3ba94b7b3cfd5f447befde8107c04d52;0;3;2;FLOAT2;0,0;False;7;FLOAT;0.5;False;9;FLOAT;0.5;False;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;48;-192,-576;Inherit;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;65;-112,-656;Inherit;False;Constant;_Float3;Float 3;6;0;Create;True;0;0;0;False;0;False;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;69;-144,-864;Inherit;False;Property;_DotColor;DotColor;6;0;Create;True;0;0;0;False;0;False;0,0,0,0;0.6320754,0.5748309,0.5748309,0;True;True;0;6;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT3;5
Node;AmplifyShaderEditor.TexturePropertyNode;62;-608,-1136;Inherit;True;Property;_Texture0;Texture 0;5;0;Create;True;0;0;0;False;0;False;None;352167e52fa3b0c43ac690f5e2debc2b;False;white;Auto;Texture2D;-1;0;2;SAMPLER2D;0;SAMPLERSTATE;1
Node;AmplifyShaderEditor.SamplerNode;61;-336,-1104;Inherit;True;Property;_TextureSample0;Texture Sample 0;5;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;6;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT3;5
Node;AmplifyShaderEditor.LerpOp;64;176,-464;Inherit;True;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;63;416,-704;Inherit;True;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.ObjSpaceLightDirHlpNode;67;-2880,32;Inherit;True;1;0;FLOAT;0;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;119;832,-752;Float;False;True;-1;2;ASEMaterialInspector;100;5;New Amplify Shader;0770190933193b94aaa3065e307002fa;True;Unlit;0;0;Unlit;2;False;True;0;1;False;;0;False;;0;1;False;;0;False;;True;0;False;;0;False;;False;False;False;False;False;False;False;False;False;True;0;False;;False;True;0;False;;False;True;True;True;True;True;0;False;;False;False;False;False;False;False;False;True;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;True;1;False;;True;3;False;;True;True;0;False;;0;False;;True;1;RenderType=Opaque=RenderType;True;2;False;0;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;0;;0;0;Standard;1;Vertex Position,InvertActionOnDeselection;1;0;0;1;True;False;;False;0
WireConnection;105;0;104;0
WireConnection;106;0;105;0
WireConnection;117;0;115;0
WireConnection;109;0;106;0
WireConnection;109;1;117;0
WireConnection;110;0;109;0
WireConnection;5;0;2;0
WireConnection;6;0;110;0
WireConnection;4;0;5;0
WireConnection;4;1;6;0
WireConnection;7;0;4;0
WireConnection;46;0;45;1
WireConnection;46;1;45;2
WireConnection;53;0;7;0
WireConnection;53;1;9;0
WireConnection;53;2;11;0
WireConnection;39;0;38;0
WireConnection;39;1;36;0
WireConnection;55;0;53;0
WireConnection;47;0;44;0
WireConnection;47;1;46;0
WireConnection;58;0;39;0
WireConnection;58;2;59;0
WireConnection;54;0;47;0
WireConnection;54;1;55;0
WireConnection;56;0;44;0
WireConnection;56;1;55;0
WireConnection;37;0;58;0
WireConnection;40;2;37;0
WireConnection;40;7;56;0
WireConnection;40;9;54;0
WireConnection;48;0;40;0
WireConnection;61;0;62;0
WireConnection;64;0;69;0
WireConnection;64;1;65;0
WireConnection;64;2;48;0
WireConnection;63;0;61;0
WireConnection;63;1;64;0
WireConnection;119;0;63;0
ASEEND*/
//CHKSM=8489FDA4DD7089546473E1E3D986079735A72AC1