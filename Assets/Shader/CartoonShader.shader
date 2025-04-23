// Made with Amplify Shader Editor v1.9.7.1
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Cartoon Shader"
{
	Properties
	{
		_Shading1("Shading1", Float) = 0.1
		_LightDir("LightDir", Vector) = (1,1,1,0)
		_Shading2("Shading2", Float) = 0.4
		_ShadowColor1("ShadowColor1", Color) = (0.3879066,0,0.4056604,0)
		_ShadowColor2("ShadowColor2", Color) = (0.06692817,0,0.1226415,0)
		_BaseColor("BaseColor", Color) = (0.06692817,0,0.1226415,0)
		_Float2("Float 2", Float) = 0
		_SpecularRange("SpecularRange", Float) = 42.56
		[HDR]_SpecularColor("SpecularColor", Color) = (0,0,0,0)
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "Geometry+0" "IsEmissive" = "true"  }
		Cull Back
		CGINCLUDE
		#include "UnityPBSLighting.cginc"
		#include "Lighting.cginc"
		#pragma target 3.0
		#define ASE_VERSION 19701
		struct Input
		{
			float3 worldNormal;
			float3 worldPos;
		};

		uniform float4 _ShadowColor2;
		uniform float _Shading1;
		uniform float3 _LightDir;
		uniform float4 _ShadowColor1;
		uniform float _Shading2;
		uniform float4 _BaseColor;
		uniform float _Float2;
		uniform float _SpecularRange;
		uniform float4 _SpecularColor;

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float4 temp_cast_0 = (1.0).xxxx;
			float3 ase_worldNormal = i.worldNormal;
			float3 ase_vertexNormal = mul( unity_WorldToObject, float4( ase_worldNormal, 0 ) );
			ase_vertexNormal = normalize( ase_vertexNormal );
			float3 normalizeResult5 = normalize( ase_vertexNormal );
			float3 normalizeResult6 = normalize( _LightDir );
			float dotResult4 = dot( normalizeResult5 , normalizeResult6 );
			float temp_output_7_0 = saturate( dotResult4 );
			float4 lerpResult20 = lerp( _ShadowColor2 , temp_cast_0 , step( _Shading1 , temp_output_7_0 ));
			float4 temp_cast_1 = (1.0).xxxx;
			float4 lerpResult12 = lerp( _ShadowColor1 , temp_cast_1 , step( _Shading2 , temp_output_7_0 ));
			float3 ase_worldPos = i.worldPos;
			float3 ase_viewVectorWS = ( _WorldSpaceCameraPos.xyz - ase_worldPos );
			float3 ase_viewDirWS = normalize( ase_viewVectorWS );
			float3 normalizeResult26 = normalize( ( _LightDir + ase_viewDirWS ) );
			float dotResult27 = dot( normalizeResult5 , normalizeResult26 );
			o.Emission = ( ( ( lerpResult20 * lerpResult12 ) * _BaseColor ) + float4( ( step( _Float2 , pow( saturate( dotResult27 ) , _SpecularRange ) ) * _SpecularColor.rgb ) , 0.0 ) ).rgb;
			o.Alpha = 1;
		}

		ENDCG
		CGPROGRAM
		#pragma surface surf Standard keepalpha fullforwardshadows 

		ENDCG
		Pass
		{
			Name "ShadowCaster"
			Tags{ "LightMode" = "ShadowCaster" }
			ZWrite On
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 3.0
			#pragma multi_compile_shadowcaster
			#pragma multi_compile UNITY_PASS_SHADOWCASTER
			#pragma skip_variants FOG_LINEAR FOG_EXP FOG_EXP2
			#include "HLSLSupport.cginc"
			#if ( SHADER_API_D3D11 || SHADER_API_GLCORE || SHADER_API_GLES || SHADER_API_GLES3 || SHADER_API_METAL || SHADER_API_VULKAN )
				#define CAN_SKIP_VPOS
			#endif
			#include "UnityCG.cginc"
			#include "Lighting.cginc"
			#include "UnityPBSLighting.cginc"
			struct v2f
			{
				V2F_SHADOW_CASTER;
				float3 worldPos : TEXCOORD1;
				float3 worldNormal : TEXCOORD2;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};
			v2f vert( appdata_full v )
			{
				v2f o;
				UNITY_SETUP_INSTANCE_ID( v );
				UNITY_INITIALIZE_OUTPUT( v2f, o );
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO( o );
				UNITY_TRANSFER_INSTANCE_ID( v, o );
				float3 worldPos = mul( unity_ObjectToWorld, v.vertex ).xyz;
				half3 worldNormal = UnityObjectToWorldNormal( v.normal );
				o.worldNormal = worldNormal;
				o.worldPos = worldPos;
				TRANSFER_SHADOW_CASTER_NORMALOFFSET( o )
				return o;
			}
			half4 frag( v2f IN
			#if !defined( CAN_SKIP_VPOS )
			, UNITY_VPOS_TYPE vpos : VPOS
			#endif
			) : SV_Target
			{
				UNITY_SETUP_INSTANCE_ID( IN );
				Input surfIN;
				UNITY_INITIALIZE_OUTPUT( Input, surfIN );
				float3 worldPos = IN.worldPos;
				half3 worldViewDir = normalize( UnityWorldSpaceViewDir( worldPos ) );
				surfIN.worldPos = worldPos;
				surfIN.worldNormal = IN.worldNormal;
				SurfaceOutputStandard o;
				UNITY_INITIALIZE_OUTPUT( SurfaceOutputStandard, o )
				surf( surfIN, o );
				#if defined( CAN_SKIP_VPOS )
				float2 vpos = IN.pos;
				#endif
				SHADOW_CASTER_FRAGMENT( IN )
			}
			ENDCG
		}
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=19701
Node;AmplifyShaderEditor.ViewDirInputsCoordNode;24;-976,880;Inherit;False;World;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.Vector3Node;3;-976,624;Inherit;False;Property;_LightDir;LightDir;1;0;Create;True;0;0;0;False;0;False;1,1,1;1,1,1;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.NormalVertexDataNode;2;-832,336;Inherit;False;0;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.NormalizeNode;6;-800,624;Inherit;False;False;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleAddOpNode;25;-752,848;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.NormalizeNode;5;-640,336;Inherit;False;False;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.DotProductOpNode;4;-480,352;Inherit;False;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.NormalizeNode;26;-624,848;Inherit;False;False;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RangedFloatNode;11;-368,624;Inherit;False;Property;_Shading2;Shading2;2;0;Create;True;0;0;0;False;0;False;0.4;0.45;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;7;-336,352;Inherit;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.DotProductOpNode;27;-448,832;Inherit;False;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;9;-320,224;Inherit;False;Property;_Shading1;Shading1;0;0;Create;True;0;0;0;False;0;False;0.1;0.08;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;14;-96,592;Inherit;False;Property;_ShadowColor1;ShadowColor1;3;0;Create;True;0;0;0;False;0;False;0.3879066,0,0.4056604,0;0.01960784,0.3293175,0.5647059,0;True;True;0;6;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT3;5
Node;AmplifyShaderEditor.StepOpNode;10;0,896;Inherit;True;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;17;16,800;Inherit;False;Constant;_Float0;Float 0;3;0;Create;True;0;0;0;False;0;False;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;28;-320,832;Inherit;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;18;-16,16;Inherit;False;Property;_ShadowColor2;ShadowColor2;4;0;Create;True;0;0;0;False;0;False;0.06692817,0,0.1226415,0;0.05098039,0.5013449,0.6235294,0;True;True;0;6;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT3;5
Node;AmplifyShaderEditor.RangedFloatNode;21;64,224;Inherit;False;Constant;_Float1;Float 0;3;0;Create;True;0;0;0;False;0;False;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.StepOpNode;8;0,352;Inherit;True;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;30;-240,1280;Inherit;False;Property;_SpecularRange;SpecularRange;7;0;Create;True;0;0;0;False;0;False;42.56;65;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;12;272,608;Inherit;True;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.LerpOp;20;272,208;Inherit;True;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;32;0,1152;Inherit;False;Property;_Float2;Float 2;6;0;Create;True;0;0;0;False;0;False;0;0.5;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.PowerNode;29;0,1248;Inherit;False;False;2;0;FLOAT;0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;15;576,448;Inherit;True;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.ColorNode;22;544,672;Inherit;False;Property;_BaseColor;BaseColor;5;0;Create;True;0;0;0;False;0;False;0.06692817,0,0.1226415,0;0.1641509,1,0.984562,0;True;True;0;6;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT3;5
Node;AmplifyShaderEditor.StepOpNode;31;304,896;Inherit;True;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;34;272,1152;Inherit;False;Property;_SpecularColor;SpecularColor;8;1;[HDR];Create;True;0;0;0;False;0;False;0,0,0,0;1,1,1,0;True;True;0;6;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT3;5
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;23;880,608;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;35;544,896;Inherit;True;2;2;0;FLOAT;0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleAddOpNode;33;1040,608;Inherit;True;2;2;0;COLOR;0,0,0,0;False;1;FLOAT3;0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;36;1296,592;Float;False;True;-1;2;ASEMaterialInspector;0;0;Standard;Cartoon Shader;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;False;;0;False;;False;0;False;;0;False;;False;0;Opaque;0.5;True;True;0;False;Opaque;;Geometry;All;12;all;True;True;True;True;0;False;;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;2;15;10;25;False;0.5;True;0;0;False;;0;False;;0;0;False;;0;False;;0;False;;0;False;;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;True;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;;-1;0;False;;0;0;0;False;0.1;False;;0;False;;False;17;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;16;FLOAT4;0,0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;6;0;3;0
WireConnection;25;0;3;0
WireConnection;25;1;24;0
WireConnection;5;0;2;0
WireConnection;4;0;5;0
WireConnection;4;1;6;0
WireConnection;26;0;25;0
WireConnection;7;0;4;0
WireConnection;27;0;5;0
WireConnection;27;1;26;0
WireConnection;10;0;11;0
WireConnection;10;1;7;0
WireConnection;28;0;27;0
WireConnection;8;0;9;0
WireConnection;8;1;7;0
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
WireConnection;36;2;33;0
ASEEND*/
//CHKSM=BFFB1BE356CEB5DFA7400389975F102FCE9E8C87