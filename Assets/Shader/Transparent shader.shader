// Made with Amplify Shader Editor v1.9.7.1
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Transparent shader"
{
	Properties
	{
		_TextureSample0("Texture Sample 0", 2D) = "white" {}
		_Alpha("Alpha", Float) = 0.2
		_SpeedSine("SpeedSine", Float) = 0.2
		[HDR]_Color0("Color 0", Color) = (0,0,0,0)
		_Float4("Float 4", Float) = 0.2
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Custom"  "Queue" = "Geometry+0" }
		Cull Back
		ZWrite Off
		CGPROGRAM
		#include "UnityShaderVariables.cginc"
		#pragma target 3.0
		#define ASE_VERSION 19701
		#pragma surface surf Standard keepalpha addshadow fullforwardshadows 
		struct Input
		{
			float2 uv_texcoord;
		};

		uniform float4 _Color0;
		uniform sampler2D _TextureSample0;
		uniform float _Alpha;
		uniform float _Float4;
		uniform float _SpeedSine;

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float2 temp_cast_0 = (0.1).xx;
			float2 uv_TexCoord81 = i.uv_texcoord * temp_cast_0;
			float4 tex2DNode85 = tex2D( _TextureSample0, ( uv_TexCoord81 * 1.0 ) );
			float2 temp_cast_1 = (0.5).xx;
			float2 break19_g4 = temp_cast_1;
			float temp_output_1_0_g4 = ( _SpeedSine * _Time.y );
			float sinIn7_g4 = sin( temp_output_1_0_g4 );
			float sinInOffset6_g4 = sin( ( temp_output_1_0_g4 + 1.0 ) );
			float lerpResult20_g4 = lerp( break19_g4.x , break19_g4.y , frac( ( sin( ( ( sinIn7_g4 - sinInOffset6_g4 ) * 91.2228 ) ) * 43758.55 ) ));
			clip( tex2DNode85.g - ( _Alpha + ( _Float4 * ( lerpResult20_g4 + sinIn7_g4 ) ) ));
			o.Albedo = ( _Color0 * tex2DNode85 ).rgb;
			o.Alpha = 1;
		}

		ENDCG
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=19701
Node;AmplifyShaderEditor.SimpleTimeNode;111;-960,496;Inherit;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;172;-944,384;Inherit;False;Property;_SpeedSine;SpeedSine;3;0;Create;True;0;0;0;False;0;False;0.2;2;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;112;-912,624;Inherit;False;Constant;_Speed;Speed;13;0;Create;True;0;0;0;False;0;False;0.5;0.5;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;171;-752,464;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;167;-832,0;Inherit;False;Constant;_UV1;UV;1;0;Create;True;0;0;0;False;0;False;0.1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;83;-688,32;Inherit;False;Constant;_UV;UV;1;0;Create;True;0;0;0;False;0;False;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;168;-576,480;Inherit;False;Noise Sine Wave;-1;;4;a6eff29f739ced848846e3b648af87bd;0;2;1;FLOAT;0;False;2;FLOAT2;-0.5,0.5;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;170;-576,352;Inherit;False;Property;_Float4;Float 4;5;0;Create;True;0;0;0;False;0;False;0.2;0.05;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;81;-624,-80;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;84;-448,-144;Inherit;True;2;2;0;FLOAT2;0,0;False;1;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;87;-64,192;Inherit;False;Property;_Alpha;Alpha;2;0;Create;True;0;0;0;False;0;False;0.2;0.55;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;110;-256,432;Inherit;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;85;-48,-160;Inherit;True;Property;_TextureSample0;Texture Sample 0;1;0;Create;True;0;0;0;False;0;False;-1;5816d78c683f91e40b7736a2eaddb596;a21b2ad21e1aef24aabb0ca39246f277;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;6;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT3;5
Node;AmplifyShaderEditor.SimpleAddOpNode;166;176,208;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ClipNode;86;400,-128;Inherit;True;3;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.ColorNode;173;480,-336;Inherit;False;Property;_Color0;Color 0;4;1;[HDR];Create;True;0;0;0;False;0;False;0,0,0,0;0,1.888683,4,0;True;True;0;6;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT3;5
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;157;800,-160;Inherit;True;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;175;1152,-192;Float;False;True;-1;2;ASEMaterialInspector;0;0;Standard;Transparent shader;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;2;False;;0;False;;False;0;False;;0;False;;False;0;Custom;0.5;True;True;0;False;Custom;;Geometry;All;12;all;True;True;True;True;0;False;;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;2;15;10;25;False;0.5;True;0;0;False;;0;False;;0;0;False;;0;False;;0;False;;0;False;;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;True;Relative;0;;0;-1;-1;-1;0;False;0;0;False;;-1;0;False;;0;0;0;False;0.1;False;;0;False;;False;17;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;16;FLOAT4;0,0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;171;0;172;0
WireConnection;171;1;111;0
WireConnection;168;1;171;0
WireConnection;168;2;112;0
WireConnection;81;0;167;0
WireConnection;84;0;81;0
WireConnection;84;1;83;0
WireConnection;110;0;170;0
WireConnection;110;1;168;0
WireConnection;85;1;84;0
WireConnection;166;0;87;0
WireConnection;166;1;110;0
WireConnection;86;0;85;0
WireConnection;86;1;85;2
WireConnection;86;2;166;0
WireConnection;157;0;173;0
WireConnection;157;1;86;0
WireConnection;175;0;157;0
ASEEND*/
//CHKSM=5BF81E1330E79645B4C587FF854854470930E367