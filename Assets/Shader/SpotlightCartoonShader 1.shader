// Made with Amplify Shader Editor v1.9.7.1
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "New Amplify Shader"
{
	Properties
	{
		_Shading1("Shading1", Float) = 0.1
		_Shading2("Shading2", Float) = 0.4
		_ShadowColor1("ShadowColor1", Color) = (0.3879066,0,0.4056604,0)
		_ShadowColor2("ShadowColor2", Color) = (0.06692817,0,0.1226415,0)
		_BaseColor("BaseColor", Color) = (0.06692817,0,0.1226415,0)
		_Float2("Float 2", Float) = 0
		_SpecularRange("SpecularRange", Float) = 42.56
		_MyLightDirWorld("_MyLightDirWorld", Vector) = (0,0,0,0)
		[HDR]_SpecularColor("SpecularColor", Color) = (0,0,0,0)

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
			#define ASE_NEEDS_FRAG_WORLD_POSITION


			struct appdata
			{
				float4 vertex : POSITION;
				float4 color : COLOR;
				float3 ase_normal : NORMAL;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};
			
			struct v2f
			{
				float4 vertex : SV_POSITION;
				#ifdef ASE_NEEDS_FRAG_WORLD_POSITION
				float3 worldPos : TEXCOORD0;
				#endif
				float3 ase_normal : NORMAL;
				float4 ase_texcoord1 : TEXCOORD1;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};

			uniform float4 _ShadowColor2;
			uniform float _Shading1;
			uniform float3 _MyLightDirWorld;
			uniform float4 _ShadowColor1;
			uniform float _Shading2;
			uniform float4 _BaseColor;
			uniform float _Float2;
			uniform float _SpecularRange;
			uniform float4 _SpecularColor;

			
			v2f vert ( appdata v )
			{
				v2f o;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
				UNITY_TRANSFER_INSTANCE_ID(v, o);

				float3 ase_worldNormal = UnityObjectToWorldNormal(v.ase_normal);
				o.ase_texcoord1.xyz = ase_worldNormal;
				
				o.ase_normal = v.ase_normal;
				
				//setting value to unused interpolator channels and avoid initialization warnings
				o.ase_texcoord1.w = 0;
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
				float4 temp_cast_0 = (1.0).xxxx;
				float3 normalizeResult5 = normalize( i.ase_normal );
				float3 objToWorldDir37 = mul( unity_ObjectToWorld, float4( _MyLightDirWorld, 0 ) ).xyz;
				float3 normalizeResult39 = normalize( objToWorldDir37 );
				float3 ase_worldNormal = i.ase_texcoord1.xyz;
				float3 normalizeResult40 = normalize( ase_worldNormal );
				float dotResult41 = dot( normalizeResult39 , normalizeResult40 );
				float temp_output_42_0 = saturate( dotResult41 );
				float normalizeResult6 = normalize( temp_output_42_0 );
				float3 temp_cast_1 = (normalizeResult6).xxx;
				float dotResult4 = dot( normalizeResult5 , temp_cast_1 );
				float temp_output_7_0 = saturate( dotResult4 );
				float4 lerpResult20 = lerp( _ShadowColor2 , temp_cast_0 , step( _Shading1 , temp_output_7_0 ));
				float4 temp_cast_2 = (1.0).xxxx;
				float4 lerpResult12 = lerp( _ShadowColor1 , temp_cast_2 , step( _Shading2 , temp_output_7_0 ));
				float3 ase_viewVectorWS = ( _WorldSpaceCameraPos.xyz - WorldPosition );
				float3 ase_viewDirWS = normalize( ase_viewVectorWS );
				float3 normalizeResult26 = normalize( ( temp_output_42_0 + ase_viewDirWS ) );
				float dotResult27 = dot( normalizeResult5 , normalizeResult26 );
				
				
				finalColor = ( ( ( lerpResult20 * lerpResult12 ) * _BaseColor ) + float4( ( step( _Float2 , pow( saturate( dotResult27 ) , _SpecularRange ) ) * _SpecularColor.rgb ) , 0.0 ) );
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
Node;AmplifyShaderEditor.Vector3Node;36;-2523.179,503.2389;Inherit;False;Property;_MyLightDirWorld;_MyLightDirWorld;8;0;Create;True;0;0;0;False;0;False;0,0,0;0,0,0;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.TransformDirectionNode;37;-2299.179,503.2389;Inherit;False;Object;World;False;Fast;False;1;0;FLOAT3;0,0,0;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.WorldNormalVector;38;-2315.179,759.2389;Inherit;False;False;1;0;FLOAT3;0,0,1;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.NormalizeNode;39;-2075.179,503.2389;Inherit;False;False;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.NormalizeNode;40;-2091.179,759.2389;Inherit;False;False;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.DotProductOpNode;41;-1867.179,503.2389;Inherit;False;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.NormalVertexDataNode;2;-1248,272;Inherit;False;0;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ViewDirInputsCoordNode;24;-1264,832;Inherit;False;World;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.SaturateNode;42;-1723.179,503.2389;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.NormalizeNode;5;-960,272;Inherit;False;False;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.NormalizeNode;6;-960,560;Inherit;False;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;25;-960,784;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.DotProductOpNode;4;-480,352;Inherit;False;2;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.NormalizeNode;26;-816,784;Inherit;False;False;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RangedFloatNode;9;-368,224;Inherit;False;Property;_Shading1;Shading1;0;0;Create;True;0;0;0;False;0;False;0.1;0.08;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;11;-368,624;Inherit;False;Property;_Shading2;Shading2;2;0;Create;True;0;0;0;False;0;False;0.4;0.45;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;7;-336,352;Inherit;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.DotProductOpNode;27;-608,768;Inherit;False;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.StepOpNode;8;-112,352;Inherit;True;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;14;-96,592;Inherit;False;Property;_ShadowColor1;ShadowColor1;3;0;Create;True;0;0;0;False;0;False;0.3879066,0,0.4056604,0;0.4528302,0.4528302,0.4528302,0;True;True;0;6;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT3;5
Node;AmplifyShaderEditor.RangedFloatNode;21;-32,224;Inherit;False;Constant;_Float1;Float 0;3;0;Create;True;0;0;0;False;0;False;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;18;-96,-64;Inherit;False;Property;_ShadowColor2;ShadowColor2;4;0;Create;True;0;0;0;False;0;False;0.06692817,0,0.1226415,0;0.2133855,0.3235432,0.509434,0;True;True;0;6;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT3;5
Node;AmplifyShaderEditor.StepOpNode;10;0,896;Inherit;True;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;17;16,800;Inherit;False;Constant;_Float0;Float 0;3;0;Create;True;0;0;0;False;0;False;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;28;-464,1168;Inherit;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;30;-480,1504;Inherit;False;Property;_SpecularRange;SpecularRange;7;0;Create;True;0;0;0;False;0;False;42.56;65;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;12;272,608;Inherit;True;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.LerpOp;20;272,208;Inherit;True;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.PowerNode;29;-240,1280;Inherit;False;False;2;0;FLOAT;0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;32;-96,1200;Inherit;False;Property;_Float2;Float 2;6;0;Create;True;0;0;0;False;0;False;0;0.5;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;15;576,448;Inherit;True;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.ColorNode;22;576,784;Inherit;False;Property;_BaseColor;BaseColor;5;0;Create;True;0;0;0;False;0;False;0.06692817,0,0.1226415,0;0.6792453,0.1819865,0.1819865,0;True;True;0;6;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT3;5
Node;AmplifyShaderEditor.StepOpNode;31;128,1280;Inherit;True;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;34;112,1520;Inherit;False;Property;_SpecularColor;SpecularColor;9;1;[HDR];Create;True;0;0;0;False;0;False;0,0,0,0;1,1,1,0;True;True;0;6;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT3;5
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;23;880,608;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;35;464,1344;Inherit;True;2;2;0;FLOAT;0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleAddOpNode;33;1168,624;Inherit;True;2;2;0;COLOR;0,0,0,0;False;1;FLOAT3;0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.Vector3Node;3;-1616,656;Inherit;False;Property;_LightDir;LightDir;1;0;Create;True;0;0;0;False;0;False;1,1,1;1,1,1;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;1;1376,624;Float;False;True;-1;2;ASEMaterialInspector;100;5;New Amplify Shader;0770190933193b94aaa3065e307002fa;True;Unlit;0;0;Unlit;2;False;True;0;1;False;;0;False;;0;1;False;;0;False;;True;0;False;;0;False;;False;False;False;False;False;False;False;False;False;True;0;False;;False;True;0;False;;False;True;True;True;True;True;0;False;;False;False;False;False;False;False;False;True;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;True;1;False;;True;3;False;;True;True;0;False;;0;False;;True;1;RenderType=Opaque=RenderType;True;2;False;0;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;0;;0;0;Standard;1;Vertex Position,InvertActionOnDeselection;1;0;0;1;True;False;;False;0
WireConnection;37;0;36;0
WireConnection;39;0;37;0
WireConnection;40;0;38;0
WireConnection;41;0;39;0
WireConnection;41;1;40;0
WireConnection;42;0;41;0
WireConnection;5;0;2;0
WireConnection;6;0;42;0
WireConnection;25;0;42;0
WireConnection;25;1;24;0
WireConnection;4;0;5;0
WireConnection;4;1;6;0
WireConnection;26;0;25;0
WireConnection;7;0;4;0
WireConnection;27;0;5;0
WireConnection;27;1;26;0
WireConnection;8;0;9;0
WireConnection;8;1;7;0
WireConnection;10;0;11;0
WireConnection;10;1;7;0
WireConnection;28;0;27;0
WireConnection;12;0;14;0
WireConnection;12;1;17;0
WireConnection;12;2;10;0
WireConnection;20;0;18;0
WireConnection;20;1;21;0
WireConnection;20;2;8;0
WireConnection;29;0;28;0
WireConnection;29;1;30;0
WireConnection;15;0;20;0
WireConnection;15;1;12;0
WireConnection;31;0;32;0
WireConnection;31;1;29;0
WireConnection;23;0;15;0
WireConnection;23;1;22;0
WireConnection;35;0;31;0
WireConnection;35;1;34;5
WireConnection;33;0;23;0
WireConnection;33;1;35;0
WireConnection;1;0;33;0
ASEEND*/
//CHKSM=18CBD1781819C2A16ED70926F96D46885201BEC6