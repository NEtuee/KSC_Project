// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "KSC/Environment/KSC_Standard"
{
	Properties
	{
		_BlendHeight("BlendHeight", Float) = 0
		_BlendThickness("BlendThickness", Range( 0 , 50)) = 2
		_Falloff("Falloff", Range( 0.1 , 100)) = 0
		_NoiseFadeTex("NoiseFadeTex", 2D) = "white" {}
		_Noise_tileing("Noise_tileing", Range( 0 , 3)) = 2
		_ColorTint("ColorTint", Color) = (1,1,1,0)
		_MainAlbedo("MainAlbedo", 2D) = "white" {}
		_MainAlbedo_smt("MainAlbedo_smt", Range( 0 , 3)) = 1
		[Normal]_MainNormal("Main Normal", 2D) = "bump" {}
		_NormalIntensity("Normal Intensity", Range( 0 , 5)) = 1
		_Blend_Tint("Blend_Tint", Color) = (1,1,1,0)
		_BlendAlbedo("BlendAlbedo", 2D) = "white" {}
		_BlendAlbedo_smt("BlendAlbedo_smt", Range( 0 , 3)) = 0
		[Normal]_BlendNormal("Blend Normal", 2D) = "bump" {}
		_BlendNormalIntensity("BlendNormal Intensity", Range( 0 , 5)) = 1
		[Toggle]_Detailmap_Albedo_ONOFF("Detailmap_Albedo_ONOFF", Float) = 0
		[Toggle]_Detailmap_Normal_ONOFF("Detailmap_Normal_ONOFF", Float) = 0
		_DetailmapAlbedo("Detailmap(Albedo)", 2D) = "black" {}
		_Detailmap_Tint("Detailmap_Tint", Color) = (0,0,0,0)
		[Normal]_DetailmapNormal("Detailmap(Normal)", 2D) = "bump" {}
		_Detailmap_Nomal_Intensity("Detailmap_Nomal_Intensity", Range( 0 , 3)) = 1
		_AO("AO", 2D) = "gray" {}
		_Metallic("Metallic", 2D) = "gray" {}
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
		#pragma surface surf Standard keepalpha addshadow fullforwardshadows exclude_path:deferred 
		struct Input
		{
			float2 uv_texcoord;
			float3 worldPos;
		};

		uniform float _Detailmap_Normal_ONOFF;
		uniform float _BlendNormalIntensity;
		uniform sampler2D _BlendNormal;
		uniform float4 _BlendNormal_ST;
		uniform float _NormalIntensity;
		uniform sampler2D _MainNormal;
		uniform float4 _MainNormal_ST;
		uniform sampler2D _TB_DEPTH;
		uniform float TB_OFFSET_X;
		uniform float TB_OFFSET_Z;
		uniform float TB_SCALE;
		uniform float TB_FARCLIP;
		uniform float TB_OFFSET_Y;
		uniform float _BlendHeight;
		uniform float _BlendThickness;
		uniform sampler2D _NoiseFadeTex;
		uniform float _Noise_tileing;
		uniform float _Falloff;
		uniform float _Detailmap_Nomal_Intensity;
		uniform sampler2D _DetailmapNormal;
		uniform float4 _DetailmapNormal_ST;
		uniform float _Detailmap_Albedo_ONOFF;
		uniform float4 _Blend_Tint;
		uniform sampler2D _BlendAlbedo;
		uniform float4 _BlendAlbedo_ST;
		uniform sampler2D _MainAlbedo;
		uniform float4 _MainAlbedo_ST;
		uniform float4 _ColorTint;
		uniform sampler2D _DetailmapAlbedo;
		uniform float4 _DetailmapAlbedo_ST;
		uniform float4 _Detailmap_Tint;
		uniform sampler2D _Metallic;
		uniform float4 _Metallic_ST;
		uniform float _BlendAlbedo_smt;
		uniform float _MainAlbedo_smt;
		uniform sampler2D _AO;
		uniform float4 _AO_ST;

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float2 uv_BlendNormal = i.uv_texcoord * _BlendNormal_ST.xy + _BlendNormal_ST.zw;
			float3 tex2DNode68 = UnpackScaleNormal( tex2D( _BlendNormal, uv_BlendNormal ), _BlendNormalIntensity );
			float2 uv_MainNormal = i.uv_texcoord * _MainNormal_ST.xy + _MainNormal_ST.zw;
			float3 tex2DNode36 = UnpackScaleNormal( tex2D( _MainNormal, uv_MainNormal ), _NormalIntensity );
			float3 ase_worldPos = i.worldPos;
			float worldY5 = ase_worldPos.y;
			float4 temp_cast_0 = (worldY5).xxxx;
			float2 appendResult7 = (float2(ase_worldPos.x , ase_worldPos.z));
			float2 appendResult8 = (float2(TB_OFFSET_X , TB_OFFSET_Z));
			float4 temp_cast_1 = (( TB_OFFSET_Y + _BlendHeight )).xxxx;
			float4 clampResult28 = clamp( ( ( ( temp_cast_0 - ( tex2D( _TB_DEPTH, ( ( appendResult7 - appendResult8 ) / TB_SCALE ) ) * TB_FARCLIP ) ) - temp_cast_1 ) / ( _BlendThickness * tex2D( _NoiseFadeTex, ( (ase_worldPos).xyzz * _Noise_tileing ).xy ) ) ) , float4( 0,0,0,0 ) , float4( 1,1,1,0 ) );
			float4 temp_cast_3 = (_Falloff).xxxx;
			float4 clampResult31 = clamp( pow( clampResult28 , temp_cast_3 ) , float4( 0,0,0,0 ) , float4( 1,1,1,0 ) );
			float3 lerpResult76 = lerp( tex2DNode68 , tex2DNode36 , clampResult31.r);
			float3 break141 = saturate( lerpResult76 );
			float2 uv_DetailmapNormal = i.uv_texcoord * _DetailmapNormal_ST.xy + _DetailmapNormal_ST.zw;
			float3 tex2DNode115 = UnpackScaleNormal( tex2D( _DetailmapNormal, uv_DetailmapNormal ), _Detailmap_Nomal_Intensity );
			float3 break140 = tex2DNode115;
			float normalizeResult153 = normalize( ( ( break141.z + break140.z ) * 0.5 ) );
			float3 appendResult150 = (float3(( ( break141.x + break140.x ) * 0.5 ) , ( ( break141.y + break140.y ) * 0.5 ) , normalizeResult153));
			o.Normal = (( _Detailmap_Normal_ONOFF )?( saturate( appendResult150 ) ):( lerpResult76 ));
			float2 uv_BlendAlbedo = i.uv_texcoord * _BlendAlbedo_ST.xy + _BlendAlbedo_ST.zw;
			float4 tex2DNode34 = tex2D( _BlendAlbedo, uv_BlendAlbedo );
			float2 uv_MainAlbedo = i.uv_texcoord * _MainAlbedo_ST.xy + _MainAlbedo_ST.zw;
			float4 tex2DNode33 = tex2D( _MainAlbedo, uv_MainAlbedo );
			float4 lerpResult35 = lerp( ( float4( (_Blend_Tint).rgb , 0.0 ) * tex2DNode34 ) , float4( ( (tex2DNode33).rgb * (_ColorTint).rgb ) , 0.0 ) , clampResult31.r);
			float2 uv_DetailmapAlbedo = i.uv_texcoord * _DetailmapAlbedo_ST.xy + _DetailmapAlbedo_ST.zw;
			o.Albedo = (( _Detailmap_Albedo_ONOFF )?( ( ( lerpResult35 + float4( (( ( tex2D( _DetailmapAlbedo, uv_DetailmapAlbedo ) * float4( (unity_ColorSpaceDouble).rgb , 0.0 ) ) * _Detailmap_Tint )).rgb , 0.0 ) ) * 0.5 ) ):( lerpResult35 )).rgb;
			float2 uv_Metallic = i.uv_texcoord * _Metallic_ST.xy + _Metallic_ST.zw;
			float4 tex2DNode157 = tex2D( _Metallic, uv_Metallic );
			o.Metallic = tex2DNode157.r;
			float lerpResult55 = lerp( ( _BlendAlbedo_smt * tex2DNode34.a ) , ( _MainAlbedo_smt * tex2DNode33.a ) , clampResult31.r);
			o.Smoothness = lerpResult55;
			float2 uv_AO = i.uv_texcoord * _AO_ST.xy + _AO_ST.zw;
			o.Occlusion = tex2D( _AO, uv_AO ).r;
			o.Alpha = 1;
		}

		ENDCG
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=18100
63.2;-4;1182;926;-3756.959;334.1198;1;True;False
Node;AmplifyShaderEditor.WorldPosInputsNode;4;-1599.37,-109.8375;Inherit;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.RangedFloatNode;3;-1903.133,290.7337;Float;False;Global;TB_OFFSET_Z;TB_OFFSET_Z;0;0;Create;True;0;0;True;0;False;0;-265.5;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;2;-1905.252,178.7464;Float;False;Global;TB_OFFSET_X;TB_OFFSET_X;0;0;Create;True;0;0;True;0;False;0;-265.5;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;7;-1314.182,21.60019;Inherit;True;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.DynamicAppendNode;8;-1319.446,239.1034;Inherit;True;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;1;-1089.66,273.576;Float;False;Global;TB_SCALE;TB_SCALE;0;0;Create;True;0;0;True;0;False;0;531;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;10;-1058.069,107.1445;Inherit;False;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleDivideOpNode;11;-862.0684,105.2445;Inherit;False;2;0;FLOAT2;0,0;False;1;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;5;-1327.85,-102.7914;Inherit;False;worldY;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SwizzleNode;108;-206.3915,588.749;Inherit;False;FLOAT4;0;1;2;3;1;0;FLOAT3;0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.RangedFloatNode;109;-259.2272,716.8372;Inherit;False;Property;_Noise_tileing;Noise_tileing;5;0;Create;True;0;0;False;0;False;2;1;0;3;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;16;-546.0917,240.1215;Float;False;Global;TB_FARCLIP;TB_FARCLIP;1;0;Create;True;0;0;True;0;False;0;74.59;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;12;-631.0842,4.936687;Inherit;True;Global;_TB_DEPTH;TB_DEPTH;0;1;[HideInInspector];Create;True;0;0;True;0;False;-1;4a14f9b9a4ffd82489f1692de1ff16f6;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.GetLocalVarNode;18;-241.9641,39.79688;Inherit;False;5;worldY;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;17;-199.4631,169.5586;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;162;-62.52222,392.9252;Float;False;Property;_BlendHeight;BlendHeight;1;0;Create;True;0;0;False;0;False;0;-13;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;24;-76.8914,291.5386;Float;False;Global;TB_OFFSET_Y;TB_OFFSET_Y;0;0;Create;True;0;0;True;0;False;0;-4.589996;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;106;27.2086,677.3489;Inherit;False;2;2;0;FLOAT4;0,0,0,0;False;1;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;19;44.47029,156.7789;Inherit;False;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;25;-7.316873,521.8212;Float;False;Property;_BlendThickness;BlendThickness;2;0;Create;True;0;0;True;0;False;2;11.6;0;50;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;110;181.5323,608.8912;Inherit;True;Property;_NoiseFadeTex;NoiseFadeTex;4;0;Create;True;0;0;False;0;False;-1;None;efefeed45829d144a8be123c1a129f7c;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleAddOpNode;161;133.4778,268.9252;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;111;521.2152,335.4712;Inherit;False;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;22;267.7106,144.5763;Inherit;False;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleDivideOpNode;26;569.3083,157.1847;Inherit;False;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.ClampOpNode;28;756.0082,150.9847;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;COLOR;1,1,1,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;30;636.5123,43.28592;Float;False;Property;_Falloff;Falloff;3;0;Create;True;0;0;True;0;False;0;0.5;0.1;100;0;1;FLOAT;0
Node;AmplifyShaderEditor.PowerNode;29;1010.512,152.2859;Inherit;False;False;2;0;COLOR;0,0,0,0;False;1;FLOAT;1;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;38;750.3657,689.5891;Inherit;False;Property;_NormalIntensity;Normal Intensity;10;0;Create;True;0;0;False;0;False;1;1;0;5;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;70;798.6595,432.5661;Inherit;False;Property;_BlendNormalIntensity;BlendNormal Intensity;17;0;Create;True;0;0;False;0;False;1;1;0;5;0;1;FLOAT;0
Node;AmplifyShaderEditor.ClampOpNode;31;1175.512,152.2859;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;COLOR;1,1,1,0;False;1;COLOR;0
Node;AmplifyShaderEditor.BreakToComponentsNode;32;1352.512,79.28589;Inherit;False;COLOR;1;0;COLOR;0,0,0,0;False;16;FLOAT;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT;5;FLOAT;6;FLOAT;7;FLOAT;8;FLOAT;9;FLOAT;10;FLOAT;11;FLOAT;12;FLOAT;13;FLOAT;14;FLOAT;15
Node;AmplifyShaderEditor.SamplerNode;68;1143.552,354.0678;Inherit;True;Property;_BlendNormal;Blend Normal;15;1;[Normal];Create;True;0;0;False;0;False;-1;None;293f57b8a2a8bbc469a605987f3ebe39;True;0;True;bump;Auto;True;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;36;1133.883,633.7822;Inherit;True;Property;_MainNormal;Main Normal;9;1;[Normal];Create;True;0;0;False;0;False;-1;None;a9f0226ad958b81458d73cd902b015e0;True;0;True;bump;Auto;True;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.LerpOp;76;2574.535,565.5908;Inherit;False;3;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RangedFloatNode;122;1701.334,1251.011;Inherit;False;Property;_Detailmap_Nomal_Intensity;Detailmap_Nomal_Intensity;23;0;Create;True;0;0;False;0;False;1;1;0;3;0;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;155;2790.69,661.3619;Inherit;False;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SamplerNode;115;2038.691,1143.366;Inherit;True;Property;_DetailmapNormal;Detailmap(Normal);22;1;[Normal];Create;True;0;0;True;0;False;-1;None;None;True;0;True;bump;Auto;True;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorSpaceDouble;131;2711.767,-68.05322;Inherit;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ComponentMaskNode;133;2947.767,-37.05322;Inherit;False;True;True;True;False;1;0;COLOR;0,0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.BreakToComponentsNode;140;3002.126,958.8288;Inherit;False;FLOAT3;1;0;FLOAT3;0,0,0;False;16;FLOAT;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT;5;FLOAT;6;FLOAT;7;FLOAT;8;FLOAT;9;FLOAT;10;FLOAT;11;FLOAT;12;FLOAT;13;FLOAT;14;FLOAT;15
Node;AmplifyShaderEditor.ColorNode;104;1238.066,-845.686;Inherit;False;Property;_Blend_Tint;Blend_Tint;12;0;Create;True;0;0;False;0;False;1,1,1,0;1,1,1,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;33;1042.603,-373.0702;Inherit;True;Property;_MainAlbedo;MainAlbedo;7;0;Create;True;0;0;False;0;False;-1;None;b0c4836ae3bc7b845b1784fc4ea1e103;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;81;2735.256,-264.0703;Inherit;True;Property;_DetailmapAlbedo;Detailmap(Albedo);20;0;Create;True;0;0;False;0;False;-1;None;None;True;0;False;black;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.BreakToComponentsNode;141;2999.092,773.3035;Inherit;False;FLOAT3;1;0;FLOAT3;0,0,0;False;16;FLOAT;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT;5;FLOAT;6;FLOAT;7;FLOAT;8;FLOAT;9;FLOAT;10;FLOAT;11;FLOAT;12;FLOAT;13;FLOAT;14;FLOAT;15
Node;AmplifyShaderEditor.ColorNode;99;1029.563,-179.0462;Inherit;False;Property;_ColorTint;ColorTint;6;0;Create;True;0;0;False;0;False;1,1,1,0;1,1,1,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;132;3138.767,-230.0532;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT3;0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode;151;3480.324,1123.506;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;34;1234.224,-654.4552;Inherit;True;Property;_BlendAlbedo;BlendAlbedo;13;0;Create;True;0;0;False;0;False;-1;None;c7eab5e9aa56daa49ba5bda06474b3ae;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ComponentMaskNode;105;1619.073,-775.686;Inherit;False;True;True;True;False;1;0;COLOR;0,0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.ColorNode;114;3184.622,-58.71988;Inherit;False;Property;_Detailmap_Tint;Detailmap_Tint;21;0;Create;True;0;0;False;0;False;0,0,0,0;0,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ComponentMaskNode;101;1381.834,-358.3987;Inherit;False;True;True;True;False;1;0;COLOR;0,0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RangedFloatNode;146;3457.935,864.6409;Inherit;False;Constant;_Float0;Float 0;15;0;Create;True;0;0;False;0;False;0.5;0.5;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.ComponentMaskNode;100;1372.834,-196.3987;Inherit;False;True;True;True;False;1;0;COLOR;0,0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleAddOpNode;142;3322.799,776.7372;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;113;3396.622,-203.7199;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;102;1865.073,-680.686;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;152;3740.649,1165.654;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;143;3316.523,924.879;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;97;1576.536,-335.9244;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.NormalizeNode;153;3957.587,1200.364;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;147;3686.713,718.439;Inherit;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ComponentMaskNode;82;3536.919,-228.0021;Inherit;False;True;True;True;False;1;0;COLOR;0,0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.LerpOp;35;2354.033,-586.2176;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;144;3687.823,942.2791;Inherit;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;57;1745.765,-286.782;Float;False;Property;_MainAlbedo_smt;MainAlbedo_smt;8;0;Create;True;0;0;True;0;False;1;1;0;3;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;88;3807.112,-103.9121;Inherit;False;Constant;_Just05;Just0.5;15;0;Create;True;0;0;False;0;False;0.5;0.5;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;58;1933.761,-475.3857;Float;False;Property;_BlendAlbedo_smt;BlendAlbedo_smt;14;0;Create;True;0;0;False;0;False;0;0;0;3;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;87;3762.176,-246.1261;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT3;0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.DynamicAppendNode;150;3978.067,577.6457;Inherit;False;FLOAT3;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;59;2135.761,-246.3857;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;156;4081.412,593.7405;Inherit;False;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;56;2136.62,-393.5421;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;86;3941.079,-243.2089;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.ToggleSwitchNode;96;4092.621,-128.4588;Inherit;False;Property;_Detailmap_Albedo_ONOFF;Detailmap_Albedo_ONOFF;18;0;Create;True;0;0;False;0;False;0;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;46;2109.417,904.222;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;75;2220.028,319.3174;Inherit;False;FLOAT4;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.LerpOp;55;2248.395,86.36926;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;69;1523.532,380.0326;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;44;1603.775,922.7051;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;72;2002.863,409.7307;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;60;4412.042,256.0356;Inherit;True;Property;_AO;AO;24;0;Create;True;0;0;False;0;False;-1;None;None;True;0;False;gray;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.DynamicAppendNode;125;2708.71,1442.424;Inherit;True;FLOAT3;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.ToggleSwitchNode;45;1778.05,841.3586;Inherit;False;Property;_G_Filp;G_Filp;11;0;Create;True;0;0;False;0;False;0;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;79;935.0354,41.54724;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;127;2513.282,1520.302;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.NormalizeNode;128;2510.58,1619.924;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;41;2114.656,798.944;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;157;4038.613,3.259071;Inherit;True;Property;_Metallic;Metallic;25;0;Create;True;0;0;False;0;False;-1;None;082a7ecb4e76d6c4dab23be550dd2aac;True;0;False;gray;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.NormalizeNode;73;1677.555,457.3106;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ToggleSwitchNode;137;4138.201,436.0182;Inherit;False;Property;_Detailmap_Normal_ONOFF;Detailmap_Normal_ONOFF;19;0;Create;True;0;0;False;0;False;0;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;74;2007.027,288.3173;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;40;2342.716,829.944;Inherit;False;FLOAT3;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.ToggleSwitchNode;71;1705.236,340.8637;Inherit;False;Property;_BlendNormal_G_Filp;BlendNormal_G_Filp;16;0;Create;True;0;0;False;0;False;0;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;120;2521.292,1420.342;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;158;4445.837,106.5804;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;160;4067.194,237.8814;Inherit;False;Property;_Metaillic_amount;Metaillic_amount;26;0;Create;True;0;0;False;0;False;0;1;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.NormalizeNode;47;1831.25,1032.986;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;4879.605,-25.96687;Float;False;True;-1;2;ASEMaterialInspector;0;0;Standard;KSC/Environment/KSC_Standard;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Opaque;0.5;True;True;0;False;Opaque;;Geometry;ForwardOnly;14;all;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;True;0;0;False;-1;0;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;7;0;4;1
WireConnection;7;1;4;3
WireConnection;8;0;2;0
WireConnection;8;1;3;0
WireConnection;10;0;7;0
WireConnection;10;1;8;0
WireConnection;11;0;10;0
WireConnection;11;1;1;0
WireConnection;5;0;4;2
WireConnection;108;0;4;0
WireConnection;12;1;11;0
WireConnection;17;0;12;0
WireConnection;17;1;16;0
WireConnection;106;0;108;0
WireConnection;106;1;109;0
WireConnection;19;0;18;0
WireConnection;19;1;17;0
WireConnection;110;1;106;0
WireConnection;161;0;24;0
WireConnection;161;1;162;0
WireConnection;111;0;25;0
WireConnection;111;1;110;0
WireConnection;22;0;19;0
WireConnection;22;1;161;0
WireConnection;26;0;22;0
WireConnection;26;1;111;0
WireConnection;28;0;26;0
WireConnection;29;0;28;0
WireConnection;29;1;30;0
WireConnection;31;0;29;0
WireConnection;32;0;31;0
WireConnection;68;5;70;0
WireConnection;36;5;38;0
WireConnection;76;0;68;0
WireConnection;76;1;36;0
WireConnection;76;2;32;0
WireConnection;155;0;76;0
WireConnection;115;5;122;0
WireConnection;133;0;131;0
WireConnection;140;0;115;0
WireConnection;141;0;155;0
WireConnection;132;0;81;0
WireConnection;132;1;133;0
WireConnection;151;0;141;2
WireConnection;151;1;140;2
WireConnection;105;0;104;0
WireConnection;101;0;33;0
WireConnection;100;0;99;0
WireConnection;142;0;141;0
WireConnection;142;1;140;0
WireConnection;113;0;132;0
WireConnection;113;1;114;0
WireConnection;102;0;105;0
WireConnection;102;1;34;0
WireConnection;152;0;151;0
WireConnection;152;1;146;0
WireConnection;143;0;141;1
WireConnection;143;1;140;1
WireConnection;97;0;101;0
WireConnection;97;1;100;0
WireConnection;153;0;152;0
WireConnection;147;0;142;0
WireConnection;147;1;146;0
WireConnection;82;0;113;0
WireConnection;35;0;102;0
WireConnection;35;1;97;0
WireConnection;35;2;32;0
WireConnection;144;0;143;0
WireConnection;144;1;146;0
WireConnection;87;0;35;0
WireConnection;87;1;82;0
WireConnection;150;0;147;0
WireConnection;150;1;144;0
WireConnection;150;2;153;0
WireConnection;59;0;57;0
WireConnection;59;1;33;4
WireConnection;156;0;150;0
WireConnection;56;0;58;0
WireConnection;56;1;34;4
WireConnection;86;0;87;0
WireConnection;86;1;88;0
WireConnection;96;0;35;0
WireConnection;96;1;86;0
WireConnection;46;0;38;0
WireConnection;46;1;45;0
WireConnection;75;0;74;0
WireConnection;75;1;72;0
WireConnection;75;2;73;0
WireConnection;55;0;56;0
WireConnection;55;1;59;0
WireConnection;55;2;32;0
WireConnection;69;0;68;2
WireConnection;44;0;36;2
WireConnection;72;0;70;0
WireConnection;72;1;71;0
WireConnection;125;0;120;0
WireConnection;125;1;127;0
WireConnection;125;2;128;0
WireConnection;45;0;36;2
WireConnection;45;1;44;0
WireConnection;127;0;115;2
WireConnection;127;1;122;0
WireConnection;128;0;115;3
WireConnection;41;0;38;0
WireConnection;41;1;36;1
WireConnection;73;0;68;3
WireConnection;137;0;76;0
WireConnection;137;1;156;0
WireConnection;74;0;70;0
WireConnection;74;1;68;1
WireConnection;40;0;41;0
WireConnection;40;1;46;0
WireConnection;40;2;47;0
WireConnection;71;1;69;0
WireConnection;120;0;115;1
WireConnection;120;1;122;0
WireConnection;158;0;157;0
WireConnection;47;0;36;3
WireConnection;0;0;96;0
WireConnection;0;1;137;0
WireConnection;0;3;157;0
WireConnection;0;4;55;0
WireConnection;0;5;60;0
ASEEND*/
//CHKSM=5102952CBA0F72B6616C81AF1603186659A03D28