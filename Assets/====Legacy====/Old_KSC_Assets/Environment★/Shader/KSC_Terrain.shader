// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "KSC_Terrain"
{
	Properties
	{
		_TessValue( "Max Tessellation", Range( 1, 32 ) ) = 27.2
		_TessMin( "Tess Min Distance", Float ) = 10
		_TessMax( "Tess Max Distance", Float ) = 25
		_Displasment("Displasment", Range( 0 , 3)) = 0
		_T_soil_A("T_soil_A", 2D) = "white" {}
		_T_Soil_MOH("T_Soil_MOH", 2D) = "gray" {}
		_T_soil_N("T_soil_N", 2D) = "bump" {}
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "Geometry-100" }
		Cull Back
		CGPROGRAM
		#include "Tessellation.cginc"
		#pragma target 4.6
		#pragma multi_compile_instancing
		#pragma instancing_options assumeuniformscaling nomatrices nolightprobe nolightmap forwardadd
		#pragma surface surf Standard keepalpha addshadow fullforwardshadows vertex:vertexDataFunc tessellate:tessFunction 
		struct Input
		{
			float2 uv_texcoord;
		};

		#ifdef UNITY_INSTANCING_ENABLED//ASE Terrain Instancing
			sampler2D _TerrainHeightmapTexture;//ASE Terrain Instancing
			sampler2D _TerrainNormalmapTexture;//ASE Terrain Instancing
		#endif//ASE Terrain Instancing
		UNITY_INSTANCING_BUFFER_START( Terrain )//ASE Terrain Instancing
			UNITY_DEFINE_INSTANCED_PROP( float4, _TerrainPatchInstanceData )//ASE Terrain Instancing
		UNITY_INSTANCING_BUFFER_END( Terrain)//ASE Terrain Instancing
		CBUFFER_START( UnityTerrain)//ASE Terrain Instancing
			#ifdef UNITY_INSTANCING_ENABLED//ASE Terrain Instancing
				float4 _TerrainHeightmapRecipSize;//ASE Terrain Instancing
				float4 _TerrainHeightmapScale;//ASE Terrain Instancing
			#endif//ASE Terrain Instancing
		CBUFFER_END//ASE Terrain Instancing
		uniform sampler2D _T_Soil_MOH;
		uniform float4 _T_Soil_MOH_ST;
		uniform float _Displasment;
		uniform sampler2D _T_soil_N;
		uniform float4 _T_soil_N_ST;
		uniform sampler2D _T_soil_A;
		uniform float4 _T_soil_A_ST;
		uniform float _TessValue;
		uniform float _TessMin;
		uniform float _TessMax;


		void ApplyMeshModification( inout appdata_full v )
		{
			#if defined(UNITY_INSTANCING_ENABLED) && !defined(SHADER_API_D3D11_9X)
				float2 patchVertex = v.vertex.xy;
				float4 instanceData = UNITY_ACCESS_INSTANCED_PROP(Terrain, _TerrainPatchInstanceData);
				
				float4 uvscale = instanceData.z * _TerrainHeightmapRecipSize;
				float4 uvoffset = instanceData.xyxy * uvscale;
				uvoffset.xy += 0.5f * _TerrainHeightmapRecipSize.xy;
				float2 sampleCoords = (patchVertex.xy * uvscale.xy + uvoffset.xy);
				
				float hm = UnpackHeightmap(tex2Dlod(_TerrainHeightmapTexture, float4(sampleCoords, 0, 0)));
				v.vertex.xz = (patchVertex.xy + instanceData.xy) * _TerrainHeightmapScale.xz * instanceData.z;
				v.vertex.y = hm * _TerrainHeightmapScale.y;
				v.vertex.w = 1.0f;
				
				v.texcoord.xy = (patchVertex.xy * uvscale.zw + uvoffset.zw);
				v.texcoord3 = v.texcoord2 = v.texcoord1 = v.texcoord;
				
				#ifdef TERRAIN_INSTANCED_PERPIXEL_NORMAL
					v.normal = float3(0, 1, 0);
					//data.tc.zw = sampleCoords;
				#else
					float3 nor = tex2Dlod(_TerrainNormalmapTexture, float4(sampleCoords, 0, 0)).xyz;
					v.normal = 2.0f * nor - 1.0f;
				#endif
			#endif
		}


		float4 tessFunction( appdata_full v0, appdata_full v1, appdata_full v2 )
		{
			return UnityDistanceBasedTess( v0.vertex, v1.vertex, v2.vertex, _TessMin, _TessMax, _TessValue );
		}

		void vertexDataFunc( inout appdata_full v )
		{
			ApplyMeshModification(v);;
			float2 uv_T_Soil_MOH = v.texcoord * _T_Soil_MOH_ST.xy + _T_Soil_MOH_ST.zw;
			float4 tex2DNode38 = tex2Dlod( _T_Soil_MOH, float4( uv_T_Soil_MOH, 0, 0.0) );
			float3 ase_vertexNormal = v.normal.xyz;
			v.vertex.xyz += ( ( tex2DNode38.b * _Displasment ) * ase_vertexNormal );
		}

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float2 uv_T_soil_N = i.uv_texcoord * _T_soil_N_ST.xy + _T_soil_N_ST.zw;
			o.Normal = UnpackNormal( tex2D( _T_soil_N, uv_T_soil_N ) );
			float2 uv_T_soil_A = i.uv_texcoord * _T_soil_A_ST.xy + _T_soil_A_ST.zw;
			o.Albedo = tex2D( _T_soil_A, uv_T_soil_A ).rgb;
			float2 uv_T_Soil_MOH = i.uv_texcoord * _T_Soil_MOH_ST.xy + _T_Soil_MOH_ST.zw;
			float4 tex2DNode38 = tex2D( _T_Soil_MOH, uv_T_Soil_MOH );
			o.Metallic = tex2DNode38.r;
			o.Occlusion = tex2DNode38.g;
			o.Alpha = 1;
		}

		ENDCG
		UsePass "Hidden/Nature/Terrain/Utilities/PICKING"
		UsePass "Hidden/Nature/Terrain/Utilities/SELECTION"
	}

	Dependency "BaseMapShader"="ASESampleShaders/SimpleTerrainBase"
	Dependency "UnityTerrain"="Nature/Terrain/Standard"
	Fallback "Nature/Terrain/Standard"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=18100
140;212;1045;756;803.5056;443.6833;1.798592;True;False
Node;AmplifyShaderEditor.SamplerNode;38;10.81156,202.4954;Inherit;True;Property;_T_Soil_MOH;T_Soil_MOH;7;0;Create;True;0;0;False;0;False;-1;4a436e1c26fefa24083d4cf37cb3e072;4a436e1c26fefa24083d4cf37cb3e072;True;0;False;gray;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;42;61.49219,438.5801;Inherit;False;Property;_Displasment;Displasment;5;0;Create;True;0;0;False;0;False;0;0;0;3;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;41;382.4922,352.5801;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.NormalVertexDataNode;43;361.4922,473.5801;Inherit;False;0;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;37;10.62461,-258.2417;Inherit;True;Property;_T_soil_A;T_soil_A;6;0;Create;True;0;0;False;0;False;-1;c7eab5e9aa56daa49ba5bda06474b3ae;c7eab5e9aa56daa49ba5bda06474b3ae;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;39;11.21103,-22.67789;Inherit;True;Property;_T_soil_N;T_soil_N;8;0;Create;True;0;0;False;0;False;-1;293f57b8a2a8bbc469a605987f3ebe39;293f57b8a2a8bbc469a605987f3ebe39;True;0;True;bump;Auto;True;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;45;558.4922,214.5801;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;693.876,-172.068;Float;False;True;-1;6;ASEMaterialInspector;0;0;Standard;KSC_Terrain;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Opaque;0.5;True;True;-100;False;Opaque;;Geometry;All;14;all;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;True;0;27.2;10;25;False;0;True;0;0;False;-1;0;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;Nature/Terrain/Standard;-1;-1;-1;0;0;False;2;BaseMapShader=ASESampleShaders/SimpleTerrainBase;UnityTerrain=Nature/Terrain/Standard;0;False;-1;-1;0;False;-1;0;0;0;True;0.1;False;-1;0;False;-1;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;41;0;38;3
WireConnection;41;1;42;0
WireConnection;45;0;41;0
WireConnection;45;1;43;0
WireConnection;0;0;37;0
WireConnection;0;1;39;0
WireConnection;0;3;38;1
WireConnection;0;5;38;2
WireConnection;0;11;45;0
ASEEND*/
//CHKSM=EA3C1D714D45162EBF702E50C7824C965DA251E1