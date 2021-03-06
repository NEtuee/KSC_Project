// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Bridge_ColorMask"
{
	Properties
	{
		_T_bridge_PaintMask("T_bridge_PaintMask", 2D) = "gray" {}
		_T_Material001_AlbedoSmt("T_Material.001_AlbedoSmt", 2D) = "white" {}
		_T_Material001_AO("T_Material.001_AO", 2D) = "gray" {}
		_T_Material001_Metallic("T_Material.001_Metallic", 2D) = "gray" {}
		_T_Material001_Normal("T_Material.001_Normal", 2D) = "bump" {}
		_MaskColor("MaskColor", Color) = (1,1,1,0)
		_Normal_Intensity("Normal_Intensity", Range( 0 , 3)) = 0
		_Albedo_Multiply("Albedo_Multiply", Color) = (0,0,0,0)
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "Geometry+0" }
		Cull Back
		CGPROGRAM
		#include "UnityStandardUtils.cginc"
		#pragma target 3.0
		#pragma surface surf Standard keepalpha addshadow fullforwardshadows 
		struct Input
		{
			float2 uv_texcoord;
		};

		uniform float _Normal_Intensity;
		uniform sampler2D _T_Material001_Normal;
		uniform float4 _T_Material001_Normal_ST;
		uniform float4 _MaskColor;
		uniform sampler2D _T_bridge_PaintMask;
		uniform float4 _T_bridge_PaintMask_ST;
		uniform sampler2D _T_Material001_AlbedoSmt;
		uniform float4 _T_Material001_AlbedoSmt_ST;
		uniform float4 _Albedo_Multiply;
		uniform sampler2D _T_Material001_Metallic;
		uniform float4 _T_Material001_Metallic_ST;
		uniform sampler2D _T_Material001_AO;
		uniform float4 _T_Material001_AO_ST;

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float2 uv_T_Material001_Normal = i.uv_texcoord * _T_Material001_Normal_ST.xy + _T_Material001_Normal_ST.zw;
			o.Normal = UnpackScaleNormal( tex2D( _T_Material001_Normal, uv_T_Material001_Normal ), _Normal_Intensity );
			float2 uv_T_bridge_PaintMask = i.uv_texcoord * _T_bridge_PaintMask_ST.xy + _T_bridge_PaintMask_ST.zw;
			float4 tex2DNode1 = tex2D( _T_bridge_PaintMask, uv_T_bridge_PaintMask );
			float2 uv_T_Material001_AlbedoSmt = i.uv_texcoord * _T_Material001_AlbedoSmt_ST.xy + _T_Material001_AlbedoSmt_ST.zw;
			float4 tex2DNode2 = tex2D( _T_Material001_AlbedoSmt, uv_T_Material001_AlbedoSmt );
			o.Albedo = ( ( ( _MaskColor * tex2DNode1 ) + ( tex2DNode2 * ( 1.0 - tex2DNode1 ) ) ) * _Albedo_Multiply ).rgb;
			float2 uv_T_Material001_Metallic = i.uv_texcoord * _T_Material001_Metallic_ST.xy + _T_Material001_Metallic_ST.zw;
			o.Metallic = tex2D( _T_Material001_Metallic, uv_T_Material001_Metallic ).r;
			o.Smoothness = tex2DNode2.a;
			float2 uv_T_Material001_AO = i.uv_texcoord * _T_Material001_AO_ST.xy + _T_Material001_AO_ST.zw;
			o.Occlusion = tex2D( _T_Material001_AO, uv_T_Material001_AO ).r;
			o.Alpha = 1;
		}

		ENDCG
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=18100
696;189;1279;913;1118.025;998.1093;1.3;True;False
Node;AmplifyShaderEditor.SamplerNode;1;-809.912,-582.1394;Inherit;True;Property;_T_bridge_PaintMask;T_bridge_PaintMask;0;0;Create;True;0;0;False;0;False;-1;428075d9c736cb345b462eb5d9d19767;428075d9c736cb345b462eb5d9d19767;True;0;False;gray;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;6;-656.4294,-833.8825;Inherit;False;Property;_MaskColor;MaskColor;5;0;Create;True;0;0;False;0;False;1,1,1,0;1,0.5098038,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.OneMinusNode;12;-468.8102,-370.0158;Inherit;False;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SamplerNode;2;-934.1608,-201.1356;Inherit;True;Property;_T_Material001_AlbedoSmt;T_Material.001_AlbedoSmt;1;0;Create;True;0;0;False;0;False;-1;None;603623b0fd8fd3345bab11dd9f83dce8;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;7;-379.4006,-666.1963;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;11;-293.1535,-349.4868;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;16;-1218.9,86.56247;Inherit;False;Property;_Normal_Intensity;Normal_Intensity;6;0;Create;True;0;0;False;0;False;0;0;0;3;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;9;-212.6537,-523.0868;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.ColorNode;18;-78.02493,-345.5094;Inherit;False;Property;_Albedo_Multiply;Albedo_Multiply;7;0;Create;True;0;0;False;0;False;0,0,0,0;0,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;5;-927.0008,7.521463;Inherit;True;Property;_T_Material001_Normal;T_Material.001_Normal;4;0;Create;True;0;0;False;0;False;-1;17422f9b7d1434349b8ef9abeaa087f5;17422f9b7d1434349b8ef9abeaa087f5;True;0;True;bump;Auto;True;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;4;-922.9324,218.9838;Inherit;True;Property;_T_Material001_Metallic;T_Material.001_Metallic;3;0;Create;True;0;0;False;0;False;-1;7267e6517f9060d4b83198f09086b2f8;7267e6517f9060d4b83198f09086b2f8;True;0;False;gray;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;3;-916.196,435.123;Inherit;True;Property;_T_Material001_AO;T_Material.001_AO;2;0;Create;True;0;0;False;0;False;-1;22004bf431447d746829ae8d5bd5da9c;22004bf431447d746829ae8d5bd5da9c;True;0;False;gray;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;17;49.27563,-544.4096;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;0,0;Float;False;True;-1;2;ASEMaterialInspector;0;0;Standard;Bridge_ColorMask;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Opaque;0.5;True;True;0;False;Opaque;;Geometry;All;14;all;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;True;0;0;False;-1;0;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;12;0;1;0
WireConnection;7;0;6;0
WireConnection;7;1;1;0
WireConnection;11;0;2;0
WireConnection;11;1;12;0
WireConnection;9;0;7;0
WireConnection;9;1;11;0
WireConnection;5;5;16;0
WireConnection;17;0;9;0
WireConnection;17;1;18;0
WireConnection;0;0;17;0
WireConnection;0;1;5;0
WireConnection;0;3;4;1
WireConnection;0;4;2;4
WireConnection;0;5;3;1
ASEEND*/
//CHKSM=3F446F584EF4F297DDCC87F222B7156264596C74