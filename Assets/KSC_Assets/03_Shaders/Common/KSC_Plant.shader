Shader "KSC_Plant"
{
    Properties
    {
        [NoScaleOffset]MainTex("MainTex", 2D) = "white" {}
        MainColor("MainColor", Color) = (0, 0, 0, 0)
        VarColor("VarColor", Color) = (0, 0, 0, 0)
        [NoScaleOffset]ThicknessTex("ThicknessTex", 2D) = "white" {}
        [HDR]SSSColor("SSSColor", Color) = (0, 0, 0, 0)
        [NoScaleOffset]NormalTex("NormalTex", 2D) = "bump" {}
        Normal_Intensity("Normal Intensity", Range(0, 3)) = 1
        [NoScaleOffset]MaskTex("MaskTex", 2D) = "grey" {}
        Smoothness("Smoothness", Range(0, 1)) = 0.5
        AlphaClip("AlphaClip", Range(0, 1)) = 0
        Wind_Speed("Wind Speed", Range(0, 2)) = 0
        Wind_Frequency("Wind Frequency", Range(0, 2)) = 0
        Wind_Strength("Wind Strength", Range(0, 1)) = 0
        Wind_Direction("Wind Direction", Vector) = (1, 0, 0, 0)

        _Specular ("Specular", Range(0, 0.08)) = 0.04
        _EnvReflection ("Environment Reflection", Range (0,1)) = 1 

        [HideInInspector][NoScaleOffset]unity_Lightmaps("unity_Lightmaps", 2DArray) = "" {}
        [HideInInspector][NoScaleOffset]unity_LightmapsInd("unity_LightmapsInd", 2DArray) = "" {}
        [HideInInspector][NoScaleOffset]unity_ShadowMasks("unity_ShadowMasks", 2DArray) = "" {}
    }
    SubShader
    {
        Tags
        {
            "RenderPipeline"="UniversalPipeline"
            "RenderType"="Opaque"
            "UniversalMaterialType" = "Lit"
            "Queue"="AlphaTest"
        }
        Pass
        {
            Name "Universal Forward"
            Tags
            {
                "LightMode" = "UniversalForward"
            }

            // Render State
            Cull Off
        Blend One Zero
        ZTest LEqual
        ZWrite On

            // Debug
            // <None>

            // --------------------------------------------------
            // Pass

            HLSLPROGRAM

            // Pragmas
            #pragma target 4.5
        #pragma exclude_renderers gles gles3 glcore
        #pragma multi_compile_instancing
        #pragma multi_compile_fog
        #pragma multi_compile _ DOTS_INSTANCING_ON
        #pragma vertex vert
        #pragma fragment frag

            // DotsInstancingOptions: <None>
            // HybridV1InjectedBuiltinProperties: <None>

            // Keywords
            #pragma multi_compile _ _SCREEN_SPACE_OCCLUSION
        #pragma multi_compile _ LIGHTMAP_ON
        #pragma multi_compile _ DIRLIGHTMAP_COMBINED
        #pragma multi_compile _ _MAIN_LIGHT_SHADOWS _MAIN_LIGHT_SHADOWS_CASCADE _MAIN_LIGHT_SHADOWS_SCREEN
        #pragma multi_compile _ADDITIONAL_LIGHTS_VERTEX _ADDITIONAL_LIGHTS _ADDITIONAL_OFF
        #pragma multi_compile _ _ADDITIONAL_LIGHT_SHADOWS
        #pragma multi_compile _ _SHADOWS_SOFT
        #pragma multi_compile _ LIGHTMAP_SHADOW_MIXING
        #pragma multi_compile _ SHADOWS_SHADOWMASK
            // GraphKeywords: <None>

            // Defines
            #define _AlphaClip 1
            #define _NORMALMAP 1
            #define _NORMAL_DROPOFF_TS 1
            #define ATTRIBUTES_NEED_NORMAL
            #define ATTRIBUTES_NEED_TANGENT
            #define ATTRIBUTES_NEED_TEXCOORD0
            #define ATTRIBUTES_NEED_TEXCOORD1
            #define VARYINGS_NEED_POSITION_WS
            #define VARYINGS_NEED_NORMAL_WS
            #define VARYINGS_NEED_TANGENT_WS
            #define VARYINGS_NEED_TEXCOORD0
            #define VARYINGS_NEED_VIEWDIRECTION_WS
            #define VARYINGS_NEED_FOG_AND_VERTEX_LIGHT
            #define FEATURES_GRAPH_VERTEX
            /* WARNING: $splice Could not find named fragment 'PassInstancing' */
            #define SHADERPASS SHADERPASS_FORWARD
            /* WARNING: $splice Could not find named fragment 'DotsInstancingVars' */

            // Includes
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
        //#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
                // Graph Properties
            CBUFFER_START(UnityPerMaterial)
        float4 MainTex_TexelSize;
        float4 MainColor;
        float4 VarColor;
        float4 ThicknessTex_TexelSize;
        float4 SSSColor;
        float4 NormalTex_TexelSize;
        float Normal_Intensity;
        float4 MaskTex_TexelSize;
        float Smoothness;
        float AlphaClip;
        float Wind_Speed;
        float Wind_Frequency;
        float Wind_Strength;
        float3 Wind_Direction;
        float _Specular;
        float _EnvReflection;
        CBUFFER_END

        #include ".\Assets\KSC_Assets\03_Shaders\Common\Lighting_KSC.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Shadows.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"

            // --------------------------------------------------
            // Structs and Packing

            struct Attributes
        {
            float3 positionOS : POSITION;
            float3 normalOS : NORMAL;
            float4 tangentOS : TANGENT;
            float4 uv0 : TEXCOORD0;
            float4 uv1 : TEXCOORD1;
            #if UNITY_ANY_INSTANCING_ENABLED
            uint instanceID : INSTANCEID_SEMANTIC;
            #endif
        };
        struct Varyings
        {
            float4 positionCS : SV_POSITION;
            float3 positionWS;
            float3 normalWS;
            float4 tangentWS;
            float4 texCoord0;
            float3 viewDirectionWS;
            #if defined(LIGHTMAP_ON)
            float2 lightmapUV;
            #endif
            #if !defined(LIGHTMAP_ON)
            float3 sh;
            #endif
            float4 fogFactorAndVertexLight;
            float4 shadowCoord;
            #if UNITY_ANY_INSTANCING_ENABLED
            uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
            #endif
        };
        struct SurfaceDescriptionInputs
        {
            float3 TangentSpaceNormal;
            float3 AbsoluteWorldSpacePosition;
            float4 uv0;
        };
        struct VertexDescriptionInputs
        {
            float3 ObjectSpaceNormal;
            float3 WorldSpaceNormal;
            float3 ObjectSpaceTangent;
            float3 WorldSpaceTangent;
            float3 ObjectSpaceBiTangent;
            float3 WorldSpaceBiTangent;
            float3 ObjectSpacePosition;
            float3 AbsoluteWorldSpacePosition;
            float3 TimeParameters;
        };
        struct PackedVaryings
        {
            float4 positionCS : SV_POSITION;
            float3 interp0 : TEXCOORD0;
            float3 interp1 : TEXCOORD1;
            float4 interp2 : TEXCOORD2;
            float4 interp3 : TEXCOORD3;
            float3 interp4 : TEXCOORD4;
            #if defined(LIGHTMAP_ON)
            float2 interp5 : TEXCOORD5;
            #endif
            #if !defined(LIGHTMAP_ON)
            float3 interp6 : TEXCOORD6;
            #endif
            float4 interp7 : TEXCOORD7;
            float4 interp8 : TEXCOORD8;
            #if UNITY_ANY_INSTANCING_ENABLED
            uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
            #endif
        };

            PackedVaryings PackVaryings (Varyings input)
        {
            PackedVaryings output;
            output.positionCS = input.positionCS;
            output.interp0.xyz =  input.positionWS;
            output.interp1.xyz =  input.normalWS;
            output.interp2.xyzw =  input.tangentWS;
            output.interp3.xyzw =  input.texCoord0;
            output.interp4.xyz =  input.viewDirectionWS;
            #if defined(LIGHTMAP_ON)
            output.interp5.xy =  input.lightmapUV;
            #endif
            #if !defined(LIGHTMAP_ON)
            output.interp6.xyz =  input.sh;
            #endif
            output.interp7.xyzw =  input.fogFactorAndVertexLight;
            output.interp8.xyzw =  input.shadowCoord;
            #if UNITY_ANY_INSTANCING_ENABLED
            output.instanceID = input.instanceID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            output.cullFace = input.cullFace;
            #endif
            return output;
        }
        Varyings UnpackVaryings (PackedVaryings input)
        {
            Varyings output;
            output.positionCS = input.positionCS;
            output.positionWS = input.interp0.xyz;
            output.normalWS = input.interp1.xyz;
            output.tangentWS = input.interp2.xyzw;
            output.texCoord0 = input.interp3.xyzw;
            output.viewDirectionWS = input.interp4.xyz;
            #if defined(LIGHTMAP_ON)
            output.lightmapUV = input.interp5.xy;
            #endif
            #if !defined(LIGHTMAP_ON)
            output.sh = input.interp6.xyz;
            #endif
            output.fogFactorAndVertexLight = input.interp7.xyzw;
            output.shadowCoord = input.interp8.xyzw;
            #if UNITY_ANY_INSTANCING_ENABLED
            output.instanceID = input.instanceID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            output.cullFace = input.cullFace;
            #endif
            return output;
        }

            // --------------------------------------------------
            // Graph
/*
            // Graph Properties
            CBUFFER_START(UnityPerMaterial)
        float4 MainTex_TexelSize;
        float4 MainColor;
        float4 VarColor;
        float4 ThicknessTex_TexelSize;
        float4 SSSColor;
        float4 NormalTex_TexelSize;
        float Normal_Intensity;
        float4 MaskTex_TexelSize;
        float Smoothness;
        float AlphaClip;
        float Wind_Speed;
        float Wind_Frequency;
        float Wind_Strength;
        float3 Wind_Direction;
        float _Specular;
        float _EnvReflection;
        CBUFFER_END*/

        // Object and Global properties
        SAMPLER(SamplerState_Linear_Repeat);
        TEXTURE2D(MainTex);
        SAMPLER(samplerMainTex);
        TEXTURE2D(ThicknessTex);
        SAMPLER(samplerThicknessTex);
        TEXTURE2D(NormalTex);
        SAMPLER(samplerNormalTex);
        TEXTURE2D(MaskTex);
        SAMPLER(samplerMaskTex);

            // Graph Functions
            
        void Unity_Distance_float3(float3 A, float3 B, out float Out)
        {
            Out = distance(A, B);
        }

        void Unity_Normalize_float3(float3 In, out float3 Out)
        {
            Out = normalize(In);
        }

        void Unity_Multiply_float(float A, float B, out float Out)
        {
            Out = A * B;
        }

        void Unity_Add_float(float A, float B, out float Out)
        {
            Out = A + B;
        }

        void Unity_Sine_float(float In, out float Out)
        {
            Out = sin(In);
        }

        void Unity_Multiply_float(float3 A, float3 B, out float3 Out)
        {
            Out = A * B;
        }

        void Unity_Add_float3(float3 A, float3 B, out float3 Out)
        {
            Out = A + B;
        }

        void Unity_Multiply_float(float4 A, float4 B, out float4 Out)
        {
            Out = A * B;
        }

        void Unity_Fraction_float(float In, out float Out)
        {
            Out = frac(In);
        }

        void Unity_Lerp_float4(float4 A, float4 B, float4 T, out float4 Out)
        {
            Out = lerp(A, B, T);
        }

        void Unity_NormalStrength_float(float3 In, float Strength, out float3 Out)
        {
            Out = float3(In.rg * Strength, lerp(1, In.b, saturate(Strength)));
        }

        // 42362d8732025af29918c5cc8ad4975c
        #include "Assets/KSC_Assets/03_Shaders/Common/Lighting_Function.hlsl"

        void Unity_DotProduct_float3(float3 A, float3 B, out float Out)
        {
            Out = dot(A, B);
        }

        void Unity_Saturate_float(float In, out float Out)
        {
            Out = saturate(In);
        }

            // Graph Vertex
            struct VertexDescription
        {
            float3 Position;
            float3 Normal;
            float3 Tangent;
        };

        VertexDescription VertexDescriptionFunction(VertexDescriptionInputs IN)
        {
            VertexDescription description = (VertexDescription)0;
            float _Distance_a9453349007d43d7aa8e8ef2f9e0a6cb_Out_2;
            Unity_Distance_float3(float3(0, 0, 0), IN.ObjectSpacePosition, _Distance_a9453349007d43d7aa8e8ef2f9e0a6cb_Out_2);
            float3 _Property_294994c591114f76808f84d55e0400b9_Out_0 = Wind_Direction;
            float3 _Normalize_7a91dcb4e52548df84db8cb487a5210f_Out_1;
            Unity_Normalize_float3(_Property_294994c591114f76808f84d55e0400b9_Out_0, _Normalize_7a91dcb4e52548df84db8cb487a5210f_Out_1);
            float _Property_b1049a414de54aaf954fc9acaf3ffde3_Out_0 = Wind_Speed;
            float _Multiply_8cc8be463d9b4875839e9776be38d510_Out_2;
            Unity_Multiply_float(IN.TimeParameters.x, _Property_b1049a414de54aaf954fc9acaf3ffde3_Out_0, _Multiply_8cc8be463d9b4875839e9776be38d510_Out_2);
            float _Property_1cc960ad477c47dfa058e2368a6a0e59_Out_0 = Wind_Frequency;
            float _Split_47b65f07839b45a2a7f9fd07f6ae4f85_R_1 = SHADERGRAPH_OBJECT_POSITION[0];
            float _Split_47b65f07839b45a2a7f9fd07f6ae4f85_G_2 = SHADERGRAPH_OBJECT_POSITION[1];
            float _Split_47b65f07839b45a2a7f9fd07f6ae4f85_B_3 = SHADERGRAPH_OBJECT_POSITION[2];
            float _Split_47b65f07839b45a2a7f9fd07f6ae4f85_A_4 = 0;
            float _Add_5ff2f4775c81475eaa4e1d2f1038221c_Out_2;
            Unity_Add_float(_Split_47b65f07839b45a2a7f9fd07f6ae4f85_R_1, _Split_47b65f07839b45a2a7f9fd07f6ae4f85_G_2, _Add_5ff2f4775c81475eaa4e1d2f1038221c_Out_2);
            float _Add_0b5c85958b4a44b4a1471bfb43d09937_Out_2;
            Unity_Add_float(_Add_5ff2f4775c81475eaa4e1d2f1038221c_Out_2, _Split_47b65f07839b45a2a7f9fd07f6ae4f85_G_2, _Add_0b5c85958b4a44b4a1471bfb43d09937_Out_2);
            float _Multiply_36b4b25245ef46a6aa344b72d18b123a_Out_2;
            Unity_Multiply_float(_Property_1cc960ad477c47dfa058e2368a6a0e59_Out_0, _Add_0b5c85958b4a44b4a1471bfb43d09937_Out_2, _Multiply_36b4b25245ef46a6aa344b72d18b123a_Out_2);
            float _Add_112d774bc523412da770f4e752e0aeae_Out_2;
            Unity_Add_float(_Multiply_8cc8be463d9b4875839e9776be38d510_Out_2, _Multiply_36b4b25245ef46a6aa344b72d18b123a_Out_2, _Add_112d774bc523412da770f4e752e0aeae_Out_2);
            float _Sine_703274f04df4470980c37a4a595551c8_Out_1;
            Unity_Sine_float(_Add_112d774bc523412da770f4e752e0aeae_Out_2, _Sine_703274f04df4470980c37a4a595551c8_Out_1);
            float _Multiply_b11879df31064a879f94ea423ab2b2a5_Out_2;
            Unity_Multiply_float(_Sine_703274f04df4470980c37a4a595551c8_Out_1, 0.5, _Multiply_b11879df31064a879f94ea423ab2b2a5_Out_2);
            float _Add_efce9d790e58416f86a8ec3243204620_Out_2;
            Unity_Add_float(_Multiply_b11879df31064a879f94ea423ab2b2a5_Out_2, 0.5, _Add_efce9d790e58416f86a8ec3243204620_Out_2);
            float _Property_79b44419d559467d899224e854ea3824_Out_0 = Wind_Strength;
            float _Multiply_47956fa9e0804e8381e9c610915da0e6_Out_2;
            Unity_Multiply_float(_Add_efce9d790e58416f86a8ec3243204620_Out_2, _Property_79b44419d559467d899224e854ea3824_Out_0, _Multiply_47956fa9e0804e8381e9c610915da0e6_Out_2);
            float3 _Multiply_448ee0f8bc4f41a8bcf38fdc2b440495_Out_2;
            Unity_Multiply_float(_Normalize_7a91dcb4e52548df84db8cb487a5210f_Out_1, (_Multiply_47956fa9e0804e8381e9c610915da0e6_Out_2.xxx), _Multiply_448ee0f8bc4f41a8bcf38fdc2b440495_Out_2);
            float3 _Multiply_4e49ed416c034b90b06a8a56b2d0a49a_Out_2;
            Unity_Multiply_float((_Distance_a9453349007d43d7aa8e8ef2f9e0a6cb_Out_2.xxx), _Multiply_448ee0f8bc4f41a8bcf38fdc2b440495_Out_2, _Multiply_4e49ed416c034b90b06a8a56b2d0a49a_Out_2);
            float3 _Add_c136859739784517b3f510aa5bb4e469_Out_2;
            Unity_Add_float3(_Multiply_4e49ed416c034b90b06a8a56b2d0a49a_Out_2, IN.AbsoluteWorldSpacePosition, _Add_c136859739784517b3f510aa5bb4e469_Out_2);
            float3 _Transform_47fd3d4cb64f4d52b2077aaa7435e77b_Out_1 = TransformWorldToObject(_Add_c136859739784517b3f510aa5bb4e469_Out_2.xyz);
            description.Position = _Transform_47fd3d4cb64f4d52b2077aaa7435e77b_Out_1;
            description.Normal = IN.ObjectSpaceNormal;
            description.Tangent = IN.ObjectSpaceTangent;
            return description;
        }

            // Graph Pixel
            struct SurfaceDescription
        {
            float3 BaseColor;
            float3 NormalTS;
            float3 Emission;
            float Metallic;
            float Smoothness;
            float Occlusion;
            float Alpha;
            float AlphaClipThreshold;
        };

        SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
        {
            SurfaceDescription surface = (SurfaceDescription)0;
            float4 _Property_1931a4995a954dc59a2a81efc2ac7bc3_Out_0 = MainColor;
            UnityTexture2D _Property_aed0c1bacf314879a280662567e38a31_Out_0 = UnityBuildTexture2DStructNoScale(MainTex);
            float4 _SampleTexture2D_361c1baa7e5e484ab38560dd3341b6d8_RGBA_0 = SAMPLE_TEXTURE2D(_Property_aed0c1bacf314879a280662567e38a31_Out_0.tex, _Property_aed0c1bacf314879a280662567e38a31_Out_0.samplerstate, IN.uv0.xy);
            float _SampleTexture2D_361c1baa7e5e484ab38560dd3341b6d8_R_4 = _SampleTexture2D_361c1baa7e5e484ab38560dd3341b6d8_RGBA_0.r;
            float _SampleTexture2D_361c1baa7e5e484ab38560dd3341b6d8_G_5 = _SampleTexture2D_361c1baa7e5e484ab38560dd3341b6d8_RGBA_0.g;
            float _SampleTexture2D_361c1baa7e5e484ab38560dd3341b6d8_B_6 = _SampleTexture2D_361c1baa7e5e484ab38560dd3341b6d8_RGBA_0.b;
            float _SampleTexture2D_361c1baa7e5e484ab38560dd3341b6d8_A_7 = _SampleTexture2D_361c1baa7e5e484ab38560dd3341b6d8_RGBA_0.a;
            float4 _Multiply_72a5e6242399449aad550d9798747548_Out_2;
            Unity_Multiply_float(_Property_1931a4995a954dc59a2a81efc2ac7bc3_Out_0, _SampleTexture2D_361c1baa7e5e484ab38560dd3341b6d8_RGBA_0, _Multiply_72a5e6242399449aad550d9798747548_Out_2);
            float4 _Property_2e6c583a57424a8180974b05f7e49119_Out_0 = VarColor;
            float4 _Multiply_c5a7db016b7b4a19a5d1eb518bbd1ebe_Out_2;
            Unity_Multiply_float(_Property_2e6c583a57424a8180974b05f7e49119_Out_0, _SampleTexture2D_361c1baa7e5e484ab38560dd3341b6d8_RGBA_0, _Multiply_c5a7db016b7b4a19a5d1eb518bbd1ebe_Out_2);
            float _Split_47b65f07839b45a2a7f9fd07f6ae4f85_R_1 = SHADERGRAPH_OBJECT_POSITION[0];
            float _Split_47b65f07839b45a2a7f9fd07f6ae4f85_G_2 = SHADERGRAPH_OBJECT_POSITION[1];
            float _Split_47b65f07839b45a2a7f9fd07f6ae4f85_B_3 = SHADERGRAPH_OBJECT_POSITION[2];
            float _Split_47b65f07839b45a2a7f9fd07f6ae4f85_A_4 = 0;
            float _Add_5ff2f4775c81475eaa4e1d2f1038221c_Out_2;
            Unity_Add_float(_Split_47b65f07839b45a2a7f9fd07f6ae4f85_R_1, _Split_47b65f07839b45a2a7f9fd07f6ae4f85_G_2, _Add_5ff2f4775c81475eaa4e1d2f1038221c_Out_2);
            float _Add_0b5c85958b4a44b4a1471bfb43d09937_Out_2;
            Unity_Add_float(_Add_5ff2f4775c81475eaa4e1d2f1038221c_Out_2, _Split_47b65f07839b45a2a7f9fd07f6ae4f85_G_2, _Add_0b5c85958b4a44b4a1471bfb43d09937_Out_2);
            float _Fraction_e6c8ad87e4c042d9a9a488035ab25129_Out_1;
            Unity_Fraction_float(_Add_0b5c85958b4a44b4a1471bfb43d09937_Out_2, _Fraction_e6c8ad87e4c042d9a9a488035ab25129_Out_1);
            float4 _Lerp_3f9a5249281541e1be1f190753e33b9d_Out_3;
            Unity_Lerp_float4(_Multiply_72a5e6242399449aad550d9798747548_Out_2, _Multiply_c5a7db016b7b4a19a5d1eb518bbd1ebe_Out_2, (_Fraction_e6c8ad87e4c042d9a9a488035ab25129_Out_1.xxxx), _Lerp_3f9a5249281541e1be1f190753e33b9d_Out_3);
            UnityTexture2D _Property_9a5e1d21ff144daeb1ad132f767da44e_Out_0 = UnityBuildTexture2DStructNoScale(NormalTex);
            float4 _SampleTexture2D_418ee20218b748af9c02a36c83fb414e_RGBA_0 = SAMPLE_TEXTURE2D(_Property_9a5e1d21ff144daeb1ad132f767da44e_Out_0.tex, _Property_9a5e1d21ff144daeb1ad132f767da44e_Out_0.samplerstate, IN.uv0.xy);
            _SampleTexture2D_418ee20218b748af9c02a36c83fb414e_RGBA_0.rgb = UnpackNormal(_SampleTexture2D_418ee20218b748af9c02a36c83fb414e_RGBA_0);
            float _SampleTexture2D_418ee20218b748af9c02a36c83fb414e_R_4 = _SampleTexture2D_418ee20218b748af9c02a36c83fb414e_RGBA_0.r;
            float _SampleTexture2D_418ee20218b748af9c02a36c83fb414e_G_5 = _SampleTexture2D_418ee20218b748af9c02a36c83fb414e_RGBA_0.g;
            float _SampleTexture2D_418ee20218b748af9c02a36c83fb414e_B_6 = _SampleTexture2D_418ee20218b748af9c02a36c83fb414e_RGBA_0.b;
            float _SampleTexture2D_418ee20218b748af9c02a36c83fb414e_A_7 = _SampleTexture2D_418ee20218b748af9c02a36c83fb414e_RGBA_0.a;
            float _Property_c176ee8debc744ebae505ad20fab01dd_Out_0 = Normal_Intensity;
            float3 _NormalStrength_05e86a137c744443b9dfa31f57ac64cd_Out_2;
            Unity_NormalStrength_float((_SampleTexture2D_418ee20218b748af9c02a36c83fb414e_RGBA_0.xyz), _Property_c176ee8debc744ebae505ad20fab01dd_Out_0, _NormalStrength_05e86a137c744443b9dfa31f57ac64cd_Out_2);
            float4 _Property_bf6324ebb86f442ba9760ead28e9f8e6_Out_0 = IsGammaSpace() ? LinearToSRGB(SSSColor) : SSSColor;
            float3 _MainLightCustomFunction_13744cd490944daa89a8ca8ea82bf801_Direction_0;
            float3 _MainLightCustomFunction_13744cd490944daa89a8ca8ea82bf801_Color_2;
            float _MainLightCustomFunction_13744cd490944daa89a8ca8ea82bf801_DistanceAtten_3;
            float _MainLightCustomFunction_13744cd490944daa89a8ca8ea82bf801_ShadowAtten_4;
            MainLight_float(IN.AbsoluteWorldSpacePosition, _MainLightCustomFunction_13744cd490944daa89a8ca8ea82bf801_Direction_0, _MainLightCustomFunction_13744cd490944daa89a8ca8ea82bf801_Color_2, _MainLightCustomFunction_13744cd490944daa89a8ca8ea82bf801_DistanceAtten_3, _MainLightCustomFunction_13744cd490944daa89a8ca8ea82bf801_ShadowAtten_4);
            float3 _Multiply_a5ee958a43d6486c8adafb051e9f9094_Out_2;
            Unity_Multiply_float(_MainLightCustomFunction_13744cd490944daa89a8ca8ea82bf801_Color_2, (_MainLightCustomFunction_13744cd490944daa89a8ca8ea82bf801_ShadowAtten_4.xxx), _Multiply_a5ee958a43d6486c8adafb051e9f9094_Out_2);
            float3 _Normalize_1f2add49bcef4d38aca69e47723ca228_Out_1;
            Unity_Normalize_float3(-1 * mul((float3x3)UNITY_MATRIX_M, transpose(mul(UNITY_MATRIX_I_M, UNITY_MATRIX_I_V)) [2].xyz), _Normalize_1f2add49bcef4d38aca69e47723ca228_Out_1);
            float _DotProduct_0440c3f3fd044a749b3a938d67815af5_Out_2;
            Unity_DotProduct_float3(_MainLightCustomFunction_13744cd490944daa89a8ca8ea82bf801_Direction_0, _Normalize_1f2add49bcef4d38aca69e47723ca228_Out_1, _DotProduct_0440c3f3fd044a749b3a938d67815af5_Out_2);
            float _Saturate_94e73a0bcc7a4d4680346dd6b19a67bd_Out_1;
            Unity_Saturate_float(_DotProduct_0440c3f3fd044a749b3a938d67815af5_Out_2, _Saturate_94e73a0bcc7a4d4680346dd6b19a67bd_Out_1);
            float3 _Multiply_0720425b373e459b88a6cee46d6b07df_Out_2;
            Unity_Multiply_float(_Multiply_a5ee958a43d6486c8adafb051e9f9094_Out_2, (_Saturate_94e73a0bcc7a4d4680346dd6b19a67bd_Out_1.xxx), _Multiply_0720425b373e459b88a6cee46d6b07df_Out_2);
            float3 _Multiply_c699ae0d77424fb590424f54dc343760_Out_2;
            Unity_Multiply_float((_Property_bf6324ebb86f442ba9760ead28e9f8e6_Out_0.xyz), _Multiply_0720425b373e459b88a6cee46d6b07df_Out_2, _Multiply_c699ae0d77424fb590424f54dc343760_Out_2);
            float3 _Multiply_10edc177c328439db60721337094f9b1_Out_2;
            Unity_Multiply_float((_Lerp_3f9a5249281541e1be1f190753e33b9d_Out_3.xyz), _Multiply_c699ae0d77424fb590424f54dc343760_Out_2, _Multiply_10edc177c328439db60721337094f9b1_Out_2);
            UnityTexture2D _Property_a295afe4790e404dab7b599e53bee6fc_Out_0 = UnityBuildTexture2DStructNoScale(ThicknessTex);
            float4 _SampleTexture2D_76eba85b0f1b44d3b740b14c94eb642b_RGBA_0 = SAMPLE_TEXTURE2D(_Property_a295afe4790e404dab7b599e53bee6fc_Out_0.tex, _Property_a295afe4790e404dab7b599e53bee6fc_Out_0.samplerstate, IN.uv0.xy);
            float _SampleTexture2D_76eba85b0f1b44d3b740b14c94eb642b_R_4 = _SampleTexture2D_76eba85b0f1b44d3b740b14c94eb642b_RGBA_0.r;
            float _SampleTexture2D_76eba85b0f1b44d3b740b14c94eb642b_G_5 = _SampleTexture2D_76eba85b0f1b44d3b740b14c94eb642b_RGBA_0.g;
            float _SampleTexture2D_76eba85b0f1b44d3b740b14c94eb642b_B_6 = _SampleTexture2D_76eba85b0f1b44d3b740b14c94eb642b_RGBA_0.b;
            float _SampleTexture2D_76eba85b0f1b44d3b740b14c94eb642b_A_7 = _SampleTexture2D_76eba85b0f1b44d3b740b14c94eb642b_RGBA_0.a;
            float3 _Multiply_b49f1dc358b14c73ba3e39551f96c7f9_Out_2;
            Unity_Multiply_float(_Multiply_10edc177c328439db60721337094f9b1_Out_2, (_SampleTexture2D_76eba85b0f1b44d3b740b14c94eb642b_RGBA_0.xyz), _Multiply_b49f1dc358b14c73ba3e39551f96c7f9_Out_2);
            float _Property_37adf16feb2c4f05af1e9de427c73874_Out_0 = Smoothness;
            UnityTexture2D _Property_94b068eae63c41fca823ae460e4c3a7f_Out_0 = UnityBuildTexture2DStructNoScale(MaskTex);
            float4 _SampleTexture2D_15fbde470d214b42aa9b8f3980d14645_RGBA_0 = SAMPLE_TEXTURE2D(_Property_94b068eae63c41fca823ae460e4c3a7f_Out_0.tex, _Property_94b068eae63c41fca823ae460e4c3a7f_Out_0.samplerstate, IN.uv0.xy);
            float _SampleTexture2D_15fbde470d214b42aa9b8f3980d14645_R_4 = _SampleTexture2D_15fbde470d214b42aa9b8f3980d14645_RGBA_0.r;
            float _SampleTexture2D_15fbde470d214b42aa9b8f3980d14645_G_5 = _SampleTexture2D_15fbde470d214b42aa9b8f3980d14645_RGBA_0.g;
            float _SampleTexture2D_15fbde470d214b42aa9b8f3980d14645_B_6 = _SampleTexture2D_15fbde470d214b42aa9b8f3980d14645_RGBA_0.b;
            float _SampleTexture2D_15fbde470d214b42aa9b8f3980d14645_A_7 = _SampleTexture2D_15fbde470d214b42aa9b8f3980d14645_RGBA_0.a;
            float _Multiply_485390079595403c811e1320cbe9d1c2_Out_2;
            Unity_Multiply_float(_Property_37adf16feb2c4f05af1e9de427c73874_Out_0, _SampleTexture2D_15fbde470d214b42aa9b8f3980d14645_A_7, _Multiply_485390079595403c811e1320cbe9d1c2_Out_2);
            float _Property_7784b184630844a1bd70b6904d69c970_Out_0 = AlphaClip;
            surface.BaseColor = (_Lerp_3f9a5249281541e1be1f190753e33b9d_Out_3.xyz);
            surface.NormalTS = _NormalStrength_05e86a137c744443b9dfa31f57ac64cd_Out_2;
            surface.Emission = _Multiply_b49f1dc358b14c73ba3e39551f96c7f9_Out_2;
            surface.Metallic = 0;
            surface.Smoothness = _Multiply_485390079595403c811e1320cbe9d1c2_Out_2;
            surface.Occlusion = 1;
            surface.Alpha = _SampleTexture2D_361c1baa7e5e484ab38560dd3341b6d8_A_7;
            surface.AlphaClipThreshold = _Property_7784b184630844a1bd70b6904d69c970_Out_0;
            return surface;
        }

            // --------------------------------------------------
            // Build Graph Inputs

            VertexDescriptionInputs BuildVertexDescriptionInputs(Attributes input)
        {
            VertexDescriptionInputs output;
            ZERO_INITIALIZE(VertexDescriptionInputs, output);

            output.ObjectSpaceNormal =           input.normalOS;
            output.WorldSpaceNormal =            TransformObjectToWorldNormal(input.normalOS);
            output.ObjectSpaceTangent =          input.tangentOS.xyz;
            output.WorldSpaceTangent =           TransformObjectToWorldDir(input.tangentOS.xyz);
            output.ObjectSpaceBiTangent =        normalize(cross(input.normalOS, input.tangentOS) * (input.tangentOS.w > 0.0f ? 1.0f : -1.0f) * GetOddNegativeScale());
            output.WorldSpaceBiTangent =         TransformObjectToWorldDir(output.ObjectSpaceBiTangent);
            output.ObjectSpacePosition =         input.positionOS;
            output.AbsoluteWorldSpacePosition =  GetAbsolutePositionWS(TransformObjectToWorld(input.positionOS));
            output.TimeParameters =              _TimeParameters.xyz;

            return output;
        }
            SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
        {
            SurfaceDescriptionInputs output;
            ZERO_INITIALIZE(SurfaceDescriptionInputs, output);



            output.TangentSpaceNormal =          float3(0.0f, 0.0f, 1.0f);


            output.AbsoluteWorldSpacePosition =  GetAbsolutePositionWS(input.positionWS);
            output.uv0 =                         input.texCoord0;
        #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
        #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN output.FaceSign =                    IS_FRONT_VFACE(input.cullFace, true, false);
        #else
        #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
        #endif
        #undef BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN

            return output;
        }

            // --------------------------------------------------
            // Main

            #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/Varyings.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/PBRForwardPass.hlsl"

            ENDHLSL
        }
        Pass
        {
            Name "GBuffer"
            Tags
            {
                "LightMode" = "UniversalGBuffer"
            }

            // Render State
            Cull Off
        Blend One Zero
        ZTest LEqual
        ZWrite On

            // Debug
            // <None>

            // --------------------------------------------------
            // Pass

            HLSLPROGRAM

            // Pragmas
            #pragma target 4.5
        #pragma exclude_renderers gles gles3 glcore
        #pragma multi_compile_instancing
        #pragma multi_compile_fog
        #pragma multi_compile _ DOTS_INSTANCING_ON
        #pragma vertex vert
        #pragma fragment frag

            // DotsInstancingOptions: <None>
            // HybridV1InjectedBuiltinProperties: <None>

            // Keywords
            #pragma multi_compile _ LIGHTMAP_ON
        #pragma multi_compile _ DIRLIGHTMAP_COMBINED
        #pragma multi_compile _ _MAIN_LIGHT_SHADOWS _MAIN_LIGHT_SHADOWS_CASCADE _MAIN_LIGHT_SHADOWS_SCREEN
        #pragma multi_compile _ _SHADOWS_SOFT
        #pragma multi_compile _ _MIXED_LIGHTING_SUBTRACTIVE
        #pragma multi_compile _ _GBUFFER_NORMALS_OCT
            // GraphKeywords: <None>

            // Defines
            #define _AlphaClip 1
            #define _NORMALMAP 1
            #define _NORMAL_DROPOFF_TS 1
            #define ATTRIBUTES_NEED_NORMAL
            #define ATTRIBUTES_NEED_TANGENT
            #define ATTRIBUTES_NEED_TEXCOORD0
            #define ATTRIBUTES_NEED_TEXCOORD1
            #define VARYINGS_NEED_POSITION_WS
            #define VARYINGS_NEED_NORMAL_WS
            #define VARYINGS_NEED_TANGENT_WS
            #define VARYINGS_NEED_TEXCOORD0
            #define VARYINGS_NEED_VIEWDIRECTION_WS
            #define VARYINGS_NEED_FOG_AND_VERTEX_LIGHT
            #define FEATURES_GRAPH_VERTEX
            /* WARNING: $splice Could not find named fragment 'PassInstancing' */
            #define SHADERPASS SHADERPASS_GBUFFER
            /* WARNING: $splice Could not find named fragment 'DotsInstancingVars' */

            // Includes
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
        //#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
                // Graph Properties
            CBUFFER_START(UnityPerMaterial)
        float4 MainTex_TexelSize;
        float4 MainColor;
        float4 VarColor;
        float4 ThicknessTex_TexelSize;
        float4 SSSColor;
        float4 NormalTex_TexelSize;
        float Normal_Intensity;
        float4 MaskTex_TexelSize;
        float Smoothness;
        float AlphaClip;
        float Wind_Speed;
        float Wind_Frequency;
        float Wind_Strength;
        float3 Wind_Direction;
        float _Specular;
        float _EnvReflection;
        CBUFFER_END

        #include ".\Assets\KSC_Assets\03_Shaders\Common\Lighting_KSC.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Shadows.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"

            // --------------------------------------------------
            // Structs and Packing

            struct Attributes
        {
            float3 positionOS : POSITION;
            float3 normalOS : NORMAL;
            float4 tangentOS : TANGENT;
            float4 uv0 : TEXCOORD0;
            float4 uv1 : TEXCOORD1;
            #if UNITY_ANY_INSTANCING_ENABLED
            uint instanceID : INSTANCEID_SEMANTIC;
            #endif
        };
        struct Varyings
        {
            float4 positionCS : SV_POSITION;
            float3 positionWS;
            float3 normalWS;
            float4 tangentWS;
            float4 texCoord0;
            float3 viewDirectionWS;
            #if defined(LIGHTMAP_ON)
            float2 lightmapUV;
            #endif
            #if !defined(LIGHTMAP_ON)
            float3 sh;
            #endif
            float4 fogFactorAndVertexLight;
            float4 shadowCoord;
            #if UNITY_ANY_INSTANCING_ENABLED
            uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
            #endif
        };
        struct SurfaceDescriptionInputs
        {
            float3 TangentSpaceNormal;
            float3 AbsoluteWorldSpacePosition;
            float4 uv0;
        };
        struct VertexDescriptionInputs
        {
            float3 ObjectSpaceNormal;
            float3 WorldSpaceNormal;
            float3 ObjectSpaceTangent;
            float3 WorldSpaceTangent;
            float3 ObjectSpaceBiTangent;
            float3 WorldSpaceBiTangent;
            float3 ObjectSpacePosition;
            float3 AbsoluteWorldSpacePosition;
            float3 TimeParameters;
        };
        struct PackedVaryings
        {
            float4 positionCS : SV_POSITION;
            float3 interp0 : TEXCOORD0;
            float3 interp1 : TEXCOORD1;
            float4 interp2 : TEXCOORD2;
            float4 interp3 : TEXCOORD3;
            float3 interp4 : TEXCOORD4;
            #if defined(LIGHTMAP_ON)
            float2 interp5 : TEXCOORD5;
            #endif
            #if !defined(LIGHTMAP_ON)
            float3 interp6 : TEXCOORD6;
            #endif
            float4 interp7 : TEXCOORD7;
            float4 interp8 : TEXCOORD8;
            #if UNITY_ANY_INSTANCING_ENABLED
            uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
            #endif
        };

            PackedVaryings PackVaryings (Varyings input)
        {
            PackedVaryings output;
            output.positionCS = input.positionCS;
            output.interp0.xyz =  input.positionWS;
            output.interp1.xyz =  input.normalWS;
            output.interp2.xyzw =  input.tangentWS;
            output.interp3.xyzw =  input.texCoord0;
            output.interp4.xyz =  input.viewDirectionWS;
            #if defined(LIGHTMAP_ON)
            output.interp5.xy =  input.lightmapUV;
            #endif
            #if !defined(LIGHTMAP_ON)
            output.interp6.xyz =  input.sh;
            #endif
            output.interp7.xyzw =  input.fogFactorAndVertexLight;
            output.interp8.xyzw =  input.shadowCoord;
            #if UNITY_ANY_INSTANCING_ENABLED
            output.instanceID = input.instanceID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            output.cullFace = input.cullFace;
            #endif
            return output;
        }
        Varyings UnpackVaryings (PackedVaryings input)
        {
            Varyings output;
            output.positionCS = input.positionCS;
            output.positionWS = input.interp0.xyz;
            output.normalWS = input.interp1.xyz;
            output.tangentWS = input.interp2.xyzw;
            output.texCoord0 = input.interp3.xyzw;
            output.viewDirectionWS = input.interp4.xyz;
            #if defined(LIGHTMAP_ON)
            output.lightmapUV = input.interp5.xy;
            #endif
            #if !defined(LIGHTMAP_ON)
            output.sh = input.interp6.xyz;
            #endif
            output.fogFactorAndVertexLight = input.interp7.xyzw;
            output.shadowCoord = input.interp8.xyzw;
            #if UNITY_ANY_INSTANCING_ENABLED
            output.instanceID = input.instanceID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            output.cullFace = input.cullFace;
            #endif
            return output;
        }

            // --------------------------------------------------
            // Graph

           // Graph Properties
           /* CBUFFER_START(UnityPerMaterial)
        float4 MainTex_TexelSize;
        float4 MainColor;
        float4 VarColor;
        float4 ThicknessTex_TexelSize;
        float4 SSSColor;
        float4 NormalTex_TexelSize;
        float Normal_Intensity;
        float4 MaskTex_TexelSize;
        float Smoothness;
        float AlphaClip;
        float Wind_Speed;
        float Wind_Frequency;
        float Wind_Strength;
        float3 Wind_Direction;
        float _Specular;
        float _EnvReflection;
        CBUFFER_END*/

        // Object and Global properties
        SAMPLER(SamplerState_Linear_Repeat);
        TEXTURE2D(MainTex);
        SAMPLER(samplerMainTex);
        TEXTURE2D(ThicknessTex);
        SAMPLER(samplerThicknessTex);
        TEXTURE2D(NormalTex);
        SAMPLER(samplerNormalTex);
        TEXTURE2D(MaskTex);
        SAMPLER(samplerMaskTex);

            // Graph Functions
            
        void Unity_Distance_float3(float3 A, float3 B, out float Out)
        {
            Out = distance(A, B);
        }

        void Unity_Normalize_float3(float3 In, out float3 Out)
        {
            Out = normalize(In);
        }

        void Unity_Multiply_float(float A, float B, out float Out)
        {
            Out = A * B;
        }

        void Unity_Add_float(float A, float B, out float Out)
        {
            Out = A + B;
        }

        void Unity_Sine_float(float In, out float Out)
        {
            Out = sin(In);
        }

        void Unity_Multiply_float(float3 A, float3 B, out float3 Out)
        {
            Out = A * B;
        }

        void Unity_Add_float3(float3 A, float3 B, out float3 Out)
        {
            Out = A + B;
        }

        void Unity_Multiply_float(float4 A, float4 B, out float4 Out)
        {
            Out = A * B;
        }

        void Unity_Fraction_float(float In, out float Out)
        {
            Out = frac(In);
        }

        void Unity_Lerp_float4(float4 A, float4 B, float4 T, out float4 Out)
        {
            Out = lerp(A, B, T);
        }

        void Unity_NormalStrength_float(float3 In, float Strength, out float3 Out)
        {
            Out = float3(In.rg * Strength, lerp(1, In.b, saturate(Strength)));
        }

        // 42362d8732025af29918c5cc8ad4975c
        #include "Assets/KSC_Assets/03_Shaders/Common/Lighting_Function.hlsl"

        void Unity_DotProduct_float3(float3 A, float3 B, out float Out)
        {
            Out = dot(A, B);
        }

        void Unity_Saturate_float(float In, out float Out)
        {
            Out = saturate(In);
        }

            // Graph Vertex
            struct VertexDescription
        {
            float3 Position;
            float3 Normal;
            float3 Tangent;
        };

        VertexDescription VertexDescriptionFunction(VertexDescriptionInputs IN)
        {
            VertexDescription description = (VertexDescription)0;
            float _Distance_a9453349007d43d7aa8e8ef2f9e0a6cb_Out_2;
            Unity_Distance_float3(float3(0, 0, 0), IN.ObjectSpacePosition, _Distance_a9453349007d43d7aa8e8ef2f9e0a6cb_Out_2);
            float3 _Property_294994c591114f76808f84d55e0400b9_Out_0 = Wind_Direction;
            float3 _Normalize_7a91dcb4e52548df84db8cb487a5210f_Out_1;
            Unity_Normalize_float3(_Property_294994c591114f76808f84d55e0400b9_Out_0, _Normalize_7a91dcb4e52548df84db8cb487a5210f_Out_1);
            float _Property_b1049a414de54aaf954fc9acaf3ffde3_Out_0 = Wind_Speed;
            float _Multiply_8cc8be463d9b4875839e9776be38d510_Out_2;
            Unity_Multiply_float(IN.TimeParameters.x, _Property_b1049a414de54aaf954fc9acaf3ffde3_Out_0, _Multiply_8cc8be463d9b4875839e9776be38d510_Out_2);
            float _Property_1cc960ad477c47dfa058e2368a6a0e59_Out_0 = Wind_Frequency;
            float _Split_47b65f07839b45a2a7f9fd07f6ae4f85_R_1 = SHADERGRAPH_OBJECT_POSITION[0];
            float _Split_47b65f07839b45a2a7f9fd07f6ae4f85_G_2 = SHADERGRAPH_OBJECT_POSITION[1];
            float _Split_47b65f07839b45a2a7f9fd07f6ae4f85_B_3 = SHADERGRAPH_OBJECT_POSITION[2];
            float _Split_47b65f07839b45a2a7f9fd07f6ae4f85_A_4 = 0;
            float _Add_5ff2f4775c81475eaa4e1d2f1038221c_Out_2;
            Unity_Add_float(_Split_47b65f07839b45a2a7f9fd07f6ae4f85_R_1, _Split_47b65f07839b45a2a7f9fd07f6ae4f85_G_2, _Add_5ff2f4775c81475eaa4e1d2f1038221c_Out_2);
            float _Add_0b5c85958b4a44b4a1471bfb43d09937_Out_2;
            Unity_Add_float(_Add_5ff2f4775c81475eaa4e1d2f1038221c_Out_2, _Split_47b65f07839b45a2a7f9fd07f6ae4f85_G_2, _Add_0b5c85958b4a44b4a1471bfb43d09937_Out_2);
            float _Multiply_36b4b25245ef46a6aa344b72d18b123a_Out_2;
            Unity_Multiply_float(_Property_1cc960ad477c47dfa058e2368a6a0e59_Out_0, _Add_0b5c85958b4a44b4a1471bfb43d09937_Out_2, _Multiply_36b4b25245ef46a6aa344b72d18b123a_Out_2);
            float _Add_112d774bc523412da770f4e752e0aeae_Out_2;
            Unity_Add_float(_Multiply_8cc8be463d9b4875839e9776be38d510_Out_2, _Multiply_36b4b25245ef46a6aa344b72d18b123a_Out_2, _Add_112d774bc523412da770f4e752e0aeae_Out_2);
            float _Sine_703274f04df4470980c37a4a595551c8_Out_1;
            Unity_Sine_float(_Add_112d774bc523412da770f4e752e0aeae_Out_2, _Sine_703274f04df4470980c37a4a595551c8_Out_1);
            float _Multiply_b11879df31064a879f94ea423ab2b2a5_Out_2;
            Unity_Multiply_float(_Sine_703274f04df4470980c37a4a595551c8_Out_1, 0.5, _Multiply_b11879df31064a879f94ea423ab2b2a5_Out_2);
            float _Add_efce9d790e58416f86a8ec3243204620_Out_2;
            Unity_Add_float(_Multiply_b11879df31064a879f94ea423ab2b2a5_Out_2, 0.5, _Add_efce9d790e58416f86a8ec3243204620_Out_2);
            float _Property_79b44419d559467d899224e854ea3824_Out_0 = Wind_Strength;
            float _Multiply_47956fa9e0804e8381e9c610915da0e6_Out_2;
            Unity_Multiply_float(_Add_efce9d790e58416f86a8ec3243204620_Out_2, _Property_79b44419d559467d899224e854ea3824_Out_0, _Multiply_47956fa9e0804e8381e9c610915da0e6_Out_2);
            float3 _Multiply_448ee0f8bc4f41a8bcf38fdc2b440495_Out_2;
            Unity_Multiply_float(_Normalize_7a91dcb4e52548df84db8cb487a5210f_Out_1, (_Multiply_47956fa9e0804e8381e9c610915da0e6_Out_2.xxx), _Multiply_448ee0f8bc4f41a8bcf38fdc2b440495_Out_2);
            float3 _Multiply_4e49ed416c034b90b06a8a56b2d0a49a_Out_2;
            Unity_Multiply_float((_Distance_a9453349007d43d7aa8e8ef2f9e0a6cb_Out_2.xxx), _Multiply_448ee0f8bc4f41a8bcf38fdc2b440495_Out_2, _Multiply_4e49ed416c034b90b06a8a56b2d0a49a_Out_2);
            float3 _Add_c136859739784517b3f510aa5bb4e469_Out_2;
            Unity_Add_float3(_Multiply_4e49ed416c034b90b06a8a56b2d0a49a_Out_2, IN.AbsoluteWorldSpacePosition, _Add_c136859739784517b3f510aa5bb4e469_Out_2);
            float3 _Transform_47fd3d4cb64f4d52b2077aaa7435e77b_Out_1 = TransformWorldToObject(_Add_c136859739784517b3f510aa5bb4e469_Out_2.xyz);
            description.Position = _Transform_47fd3d4cb64f4d52b2077aaa7435e77b_Out_1;
            description.Normal = IN.ObjectSpaceNormal;
            description.Tangent = IN.ObjectSpaceTangent;
            return description;
        }

            // Graph Pixel
            struct SurfaceDescription
        {
            float3 BaseColor;
            float3 NormalTS;
            float3 Emission;
            float Metallic;
            float Smoothness;
            float Occlusion;
            float Alpha;
            float AlphaClipThreshold;
        };

        SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
        {
            SurfaceDescription surface = (SurfaceDescription)0;
            float4 _Property_1931a4995a954dc59a2a81efc2ac7bc3_Out_0 = MainColor;
            UnityTexture2D _Property_aed0c1bacf314879a280662567e38a31_Out_0 = UnityBuildTexture2DStructNoScale(MainTex);
            float4 _SampleTexture2D_361c1baa7e5e484ab38560dd3341b6d8_RGBA_0 = SAMPLE_TEXTURE2D(_Property_aed0c1bacf314879a280662567e38a31_Out_0.tex, _Property_aed0c1bacf314879a280662567e38a31_Out_0.samplerstate, IN.uv0.xy);
            float _SampleTexture2D_361c1baa7e5e484ab38560dd3341b6d8_R_4 = _SampleTexture2D_361c1baa7e5e484ab38560dd3341b6d8_RGBA_0.r;
            float _SampleTexture2D_361c1baa7e5e484ab38560dd3341b6d8_G_5 = _SampleTexture2D_361c1baa7e5e484ab38560dd3341b6d8_RGBA_0.g;
            float _SampleTexture2D_361c1baa7e5e484ab38560dd3341b6d8_B_6 = _SampleTexture2D_361c1baa7e5e484ab38560dd3341b6d8_RGBA_0.b;
            float _SampleTexture2D_361c1baa7e5e484ab38560dd3341b6d8_A_7 = _SampleTexture2D_361c1baa7e5e484ab38560dd3341b6d8_RGBA_0.a;
            float4 _Multiply_72a5e6242399449aad550d9798747548_Out_2;
            Unity_Multiply_float(_Property_1931a4995a954dc59a2a81efc2ac7bc3_Out_0, _SampleTexture2D_361c1baa7e5e484ab38560dd3341b6d8_RGBA_0, _Multiply_72a5e6242399449aad550d9798747548_Out_2);
            float4 _Property_2e6c583a57424a8180974b05f7e49119_Out_0 = VarColor;
            float4 _Multiply_c5a7db016b7b4a19a5d1eb518bbd1ebe_Out_2;
            Unity_Multiply_float(_Property_2e6c583a57424a8180974b05f7e49119_Out_0, _SampleTexture2D_361c1baa7e5e484ab38560dd3341b6d8_RGBA_0, _Multiply_c5a7db016b7b4a19a5d1eb518bbd1ebe_Out_2);
            float _Split_47b65f07839b45a2a7f9fd07f6ae4f85_R_1 = SHADERGRAPH_OBJECT_POSITION[0];
            float _Split_47b65f07839b45a2a7f9fd07f6ae4f85_G_2 = SHADERGRAPH_OBJECT_POSITION[1];
            float _Split_47b65f07839b45a2a7f9fd07f6ae4f85_B_3 = SHADERGRAPH_OBJECT_POSITION[2];
            float _Split_47b65f07839b45a2a7f9fd07f6ae4f85_A_4 = 0;
            float _Add_5ff2f4775c81475eaa4e1d2f1038221c_Out_2;
            Unity_Add_float(_Split_47b65f07839b45a2a7f9fd07f6ae4f85_R_1, _Split_47b65f07839b45a2a7f9fd07f6ae4f85_G_2, _Add_5ff2f4775c81475eaa4e1d2f1038221c_Out_2);
            float _Add_0b5c85958b4a44b4a1471bfb43d09937_Out_2;
            Unity_Add_float(_Add_5ff2f4775c81475eaa4e1d2f1038221c_Out_2, _Split_47b65f07839b45a2a7f9fd07f6ae4f85_G_2, _Add_0b5c85958b4a44b4a1471bfb43d09937_Out_2);
            float _Fraction_e6c8ad87e4c042d9a9a488035ab25129_Out_1;
            Unity_Fraction_float(_Add_0b5c85958b4a44b4a1471bfb43d09937_Out_2, _Fraction_e6c8ad87e4c042d9a9a488035ab25129_Out_1);
            float4 _Lerp_3f9a5249281541e1be1f190753e33b9d_Out_3;
            Unity_Lerp_float4(_Multiply_72a5e6242399449aad550d9798747548_Out_2, _Multiply_c5a7db016b7b4a19a5d1eb518bbd1ebe_Out_2, (_Fraction_e6c8ad87e4c042d9a9a488035ab25129_Out_1.xxxx), _Lerp_3f9a5249281541e1be1f190753e33b9d_Out_3);
            UnityTexture2D _Property_9a5e1d21ff144daeb1ad132f767da44e_Out_0 = UnityBuildTexture2DStructNoScale(NormalTex);
            float4 _SampleTexture2D_418ee20218b748af9c02a36c83fb414e_RGBA_0 = SAMPLE_TEXTURE2D(_Property_9a5e1d21ff144daeb1ad132f767da44e_Out_0.tex, _Property_9a5e1d21ff144daeb1ad132f767da44e_Out_0.samplerstate, IN.uv0.xy);
            _SampleTexture2D_418ee20218b748af9c02a36c83fb414e_RGBA_0.rgb = UnpackNormal(_SampleTexture2D_418ee20218b748af9c02a36c83fb414e_RGBA_0);
            float _SampleTexture2D_418ee20218b748af9c02a36c83fb414e_R_4 = _SampleTexture2D_418ee20218b748af9c02a36c83fb414e_RGBA_0.r;
            float _SampleTexture2D_418ee20218b748af9c02a36c83fb414e_G_5 = _SampleTexture2D_418ee20218b748af9c02a36c83fb414e_RGBA_0.g;
            float _SampleTexture2D_418ee20218b748af9c02a36c83fb414e_B_6 = _SampleTexture2D_418ee20218b748af9c02a36c83fb414e_RGBA_0.b;
            float _SampleTexture2D_418ee20218b748af9c02a36c83fb414e_A_7 = _SampleTexture2D_418ee20218b748af9c02a36c83fb414e_RGBA_0.a;
            float _Property_c176ee8debc744ebae505ad20fab01dd_Out_0 = Normal_Intensity;
            float3 _NormalStrength_05e86a137c744443b9dfa31f57ac64cd_Out_2;
            Unity_NormalStrength_float((_SampleTexture2D_418ee20218b748af9c02a36c83fb414e_RGBA_0.xyz), _Property_c176ee8debc744ebae505ad20fab01dd_Out_0, _NormalStrength_05e86a137c744443b9dfa31f57ac64cd_Out_2);
            float4 _Property_bf6324ebb86f442ba9760ead28e9f8e6_Out_0 = IsGammaSpace() ? LinearToSRGB(SSSColor) : SSSColor;
            float3 _MainLightCustomFunction_13744cd490944daa89a8ca8ea82bf801_Direction_0;
            float3 _MainLightCustomFunction_13744cd490944daa89a8ca8ea82bf801_Color_2;
            float _MainLightCustomFunction_13744cd490944daa89a8ca8ea82bf801_DistanceAtten_3;
            float _MainLightCustomFunction_13744cd490944daa89a8ca8ea82bf801_ShadowAtten_4;
            MainLight_float(IN.AbsoluteWorldSpacePosition, _MainLightCustomFunction_13744cd490944daa89a8ca8ea82bf801_Direction_0, _MainLightCustomFunction_13744cd490944daa89a8ca8ea82bf801_Color_2, _MainLightCustomFunction_13744cd490944daa89a8ca8ea82bf801_DistanceAtten_3, _MainLightCustomFunction_13744cd490944daa89a8ca8ea82bf801_ShadowAtten_4);
            float3 _Multiply_a5ee958a43d6486c8adafb051e9f9094_Out_2;
            Unity_Multiply_float(_MainLightCustomFunction_13744cd490944daa89a8ca8ea82bf801_Color_2, (_MainLightCustomFunction_13744cd490944daa89a8ca8ea82bf801_ShadowAtten_4.xxx), _Multiply_a5ee958a43d6486c8adafb051e9f9094_Out_2);
            float3 _Normalize_1f2add49bcef4d38aca69e47723ca228_Out_1;
            Unity_Normalize_float3(-1 * mul((float3x3)UNITY_MATRIX_M, transpose(mul(UNITY_MATRIX_I_M, UNITY_MATRIX_I_V)) [2].xyz), _Normalize_1f2add49bcef4d38aca69e47723ca228_Out_1);
            float _DotProduct_0440c3f3fd044a749b3a938d67815af5_Out_2;
            Unity_DotProduct_float3(_MainLightCustomFunction_13744cd490944daa89a8ca8ea82bf801_Direction_0, _Normalize_1f2add49bcef4d38aca69e47723ca228_Out_1, _DotProduct_0440c3f3fd044a749b3a938d67815af5_Out_2);
            float _Saturate_94e73a0bcc7a4d4680346dd6b19a67bd_Out_1;
            Unity_Saturate_float(_DotProduct_0440c3f3fd044a749b3a938d67815af5_Out_2, _Saturate_94e73a0bcc7a4d4680346dd6b19a67bd_Out_1);
            float3 _Multiply_0720425b373e459b88a6cee46d6b07df_Out_2;
            Unity_Multiply_float(_Multiply_a5ee958a43d6486c8adafb051e9f9094_Out_2, (_Saturate_94e73a0bcc7a4d4680346dd6b19a67bd_Out_1.xxx), _Multiply_0720425b373e459b88a6cee46d6b07df_Out_2);
            float3 _Multiply_c699ae0d77424fb590424f54dc343760_Out_2;
            Unity_Multiply_float((_Property_bf6324ebb86f442ba9760ead28e9f8e6_Out_0.xyz), _Multiply_0720425b373e459b88a6cee46d6b07df_Out_2, _Multiply_c699ae0d77424fb590424f54dc343760_Out_2);
            float3 _Multiply_10edc177c328439db60721337094f9b1_Out_2;
            Unity_Multiply_float((_Lerp_3f9a5249281541e1be1f190753e33b9d_Out_3.xyz), _Multiply_c699ae0d77424fb590424f54dc343760_Out_2, _Multiply_10edc177c328439db60721337094f9b1_Out_2);
            UnityTexture2D _Property_a295afe4790e404dab7b599e53bee6fc_Out_0 = UnityBuildTexture2DStructNoScale(ThicknessTex);
            float4 _SampleTexture2D_76eba85b0f1b44d3b740b14c94eb642b_RGBA_0 = SAMPLE_TEXTURE2D(_Property_a295afe4790e404dab7b599e53bee6fc_Out_0.tex, _Property_a295afe4790e404dab7b599e53bee6fc_Out_0.samplerstate, IN.uv0.xy);
            float _SampleTexture2D_76eba85b0f1b44d3b740b14c94eb642b_R_4 = _SampleTexture2D_76eba85b0f1b44d3b740b14c94eb642b_RGBA_0.r;
            float _SampleTexture2D_76eba85b0f1b44d3b740b14c94eb642b_G_5 = _SampleTexture2D_76eba85b0f1b44d3b740b14c94eb642b_RGBA_0.g;
            float _SampleTexture2D_76eba85b0f1b44d3b740b14c94eb642b_B_6 = _SampleTexture2D_76eba85b0f1b44d3b740b14c94eb642b_RGBA_0.b;
            float _SampleTexture2D_76eba85b0f1b44d3b740b14c94eb642b_A_7 = _SampleTexture2D_76eba85b0f1b44d3b740b14c94eb642b_RGBA_0.a;
            float3 _Multiply_b49f1dc358b14c73ba3e39551f96c7f9_Out_2;
            Unity_Multiply_float(_Multiply_10edc177c328439db60721337094f9b1_Out_2, (_SampleTexture2D_76eba85b0f1b44d3b740b14c94eb642b_RGBA_0.xyz), _Multiply_b49f1dc358b14c73ba3e39551f96c7f9_Out_2);
            float _Property_37adf16feb2c4f05af1e9de427c73874_Out_0 = Smoothness;
            UnityTexture2D _Property_94b068eae63c41fca823ae460e4c3a7f_Out_0 = UnityBuildTexture2DStructNoScale(MaskTex);
            float4 _SampleTexture2D_15fbde470d214b42aa9b8f3980d14645_RGBA_0 = SAMPLE_TEXTURE2D(_Property_94b068eae63c41fca823ae460e4c3a7f_Out_0.tex, _Property_94b068eae63c41fca823ae460e4c3a7f_Out_0.samplerstate, IN.uv0.xy);
            float _SampleTexture2D_15fbde470d214b42aa9b8f3980d14645_R_4 = _SampleTexture2D_15fbde470d214b42aa9b8f3980d14645_RGBA_0.r;
            float _SampleTexture2D_15fbde470d214b42aa9b8f3980d14645_G_5 = _SampleTexture2D_15fbde470d214b42aa9b8f3980d14645_RGBA_0.g;
            float _SampleTexture2D_15fbde470d214b42aa9b8f3980d14645_B_6 = _SampleTexture2D_15fbde470d214b42aa9b8f3980d14645_RGBA_0.b;
            float _SampleTexture2D_15fbde470d214b42aa9b8f3980d14645_A_7 = _SampleTexture2D_15fbde470d214b42aa9b8f3980d14645_RGBA_0.a;
            float _Multiply_485390079595403c811e1320cbe9d1c2_Out_2;
            Unity_Multiply_float(_Property_37adf16feb2c4f05af1e9de427c73874_Out_0, _SampleTexture2D_15fbde470d214b42aa9b8f3980d14645_A_7, _Multiply_485390079595403c811e1320cbe9d1c2_Out_2);
            float _Property_7784b184630844a1bd70b6904d69c970_Out_0 = AlphaClip;
            surface.BaseColor = (_Lerp_3f9a5249281541e1be1f190753e33b9d_Out_3.xyz);
            surface.NormalTS = _NormalStrength_05e86a137c744443b9dfa31f57ac64cd_Out_2;
            surface.Emission = _Multiply_b49f1dc358b14c73ba3e39551f96c7f9_Out_2;
            surface.Metallic = 0;
            surface.Smoothness = _Multiply_485390079595403c811e1320cbe9d1c2_Out_2;
            surface.Occlusion = 1;
            surface.Alpha = _SampleTexture2D_361c1baa7e5e484ab38560dd3341b6d8_A_7;
            surface.AlphaClipThreshold = _Property_7784b184630844a1bd70b6904d69c970_Out_0;
            return surface;
        }

            // --------------------------------------------------
            // Build Graph Inputs

            VertexDescriptionInputs BuildVertexDescriptionInputs(Attributes input)
        {
            VertexDescriptionInputs output;
            ZERO_INITIALIZE(VertexDescriptionInputs, output);

            output.ObjectSpaceNormal =           input.normalOS;
            output.WorldSpaceNormal =            TransformObjectToWorldNormal(input.normalOS);
            output.ObjectSpaceTangent =          input.tangentOS.xyz;
            output.WorldSpaceTangent =           TransformObjectToWorldDir(input.tangentOS.xyz);
            output.ObjectSpaceBiTangent =        normalize(cross(input.normalOS, input.tangentOS) * (input.tangentOS.w > 0.0f ? 1.0f : -1.0f) * GetOddNegativeScale());
            output.WorldSpaceBiTangent =         TransformObjectToWorldDir(output.ObjectSpaceBiTangent);
            output.ObjectSpacePosition =         input.positionOS;
            output.AbsoluteWorldSpacePosition =  GetAbsolutePositionWS(TransformObjectToWorld(input.positionOS));
            output.TimeParameters =              _TimeParameters.xyz;

            return output;
        }
            SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
        {
            SurfaceDescriptionInputs output;
            ZERO_INITIALIZE(SurfaceDescriptionInputs, output);



            output.TangentSpaceNormal =          float3(0.0f, 0.0f, 1.0f);


            output.AbsoluteWorldSpacePosition =  GetAbsolutePositionWS(input.positionWS);
            output.uv0 =                         input.texCoord0;
        #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
        #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN output.FaceSign =                    IS_FRONT_VFACE(input.cullFace, true, false);
        #else
        #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
        #endif
        #undef BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN

            return output;
        }

            // --------------------------------------------------
            // Main

            #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/Varyings.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/UnityGBuffer.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/PBRGBufferPass.hlsl"

            ENDHLSL
        }
        Pass
        {
            Name "ShadowCaster"
            Tags
            {
                "LightMode" = "ShadowCaster"
            }

            // Render State
            Cull Off
        Blend One Zero
        ZTest LEqual
        ZWrite On
        ColorMask 0

            // Debug
            // <None>

            // --------------------------------------------------
            // Pass

            HLSLPROGRAM

            // Pragmas
            #pragma target 4.5
        #pragma exclude_renderers gles gles3 glcore
        #pragma multi_compile_instancing
        #pragma multi_compile _ DOTS_INSTANCING_ON
        #pragma vertex vert
        #pragma fragment frag

            // DotsInstancingOptions: <None>
            // HybridV1InjectedBuiltinProperties: <None>

            // Keywords
            #pragma multi_compile _ _CASTING_PUNCTUAL_LIGHT_SHADOW
            // GraphKeywords: <None>

            // Defines
            #define _AlphaClip 1
            #define _NORMALMAP 1
            #define _NORMAL_DROPOFF_TS 1
            #define ATTRIBUTES_NEED_NORMAL
            #define ATTRIBUTES_NEED_TANGENT
            #define ATTRIBUTES_NEED_TEXCOORD0
            #define VARYINGS_NEED_TEXCOORD0
            #define FEATURES_GRAPH_VERTEX
            /* WARNING: $splice Could not find named fragment 'PassInstancing' */
            #define SHADERPASS SHADERPASS_SHADOWCASTER
            /* WARNING: $splice Could not find named fragment 'DotsInstancingVars' */

            // Includes
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
     //#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
                // Graph Properties
            CBUFFER_START(UnityPerMaterial)
        float4 MainTex_TexelSize;
        float4 MainColor;
        float4 VarColor;
        float4 ThicknessTex_TexelSize;
        float4 SSSColor;
        float4 NormalTex_TexelSize;
        float Normal_Intensity;
        float4 MaskTex_TexelSize;
        float Smoothness;
        float AlphaClip;
        float Wind_Speed;
        float Wind_Frequency;
        float Wind_Strength;
        float3 Wind_Direction;
        float _Specular;
        float _EnvReflection;
        CBUFFER_END

        #include ".\Assets\KSC_Assets\03_Shaders\Common\Lighting_KSC.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"

            // --------------------------------------------------
            // Structs and Packing

            struct Attributes
        {
            float3 positionOS : POSITION;
            float3 normalOS : NORMAL;
            float4 tangentOS : TANGENT;
            float4 uv0 : TEXCOORD0;
            #if UNITY_ANY_INSTANCING_ENABLED
            uint instanceID : INSTANCEID_SEMANTIC;
            #endif
        };
        struct Varyings
        {
            float4 positionCS : SV_POSITION;
            float4 texCoord0;
            #if UNITY_ANY_INSTANCING_ENABLED
            uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
            #endif
        };
        struct SurfaceDescriptionInputs
        {
            float4 uv0;
        };
        struct VertexDescriptionInputs
        {
            float3 ObjectSpaceNormal;
            float3 WorldSpaceNormal;
            float3 ObjectSpaceTangent;
            float3 WorldSpaceTangent;
            float3 ObjectSpaceBiTangent;
            float3 WorldSpaceBiTangent;
            float3 ObjectSpacePosition;
            float3 AbsoluteWorldSpacePosition;
            float3 TimeParameters;
        };
        struct PackedVaryings
        {
            float4 positionCS : SV_POSITION;
            float4 interp0 : TEXCOORD0;
            #if UNITY_ANY_INSTANCING_ENABLED
            uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
            #endif
        };

            PackedVaryings PackVaryings (Varyings input)
        {
            PackedVaryings output;
            output.positionCS = input.positionCS;
            output.interp0.xyzw =  input.texCoord0;
            #if UNITY_ANY_INSTANCING_ENABLED
            output.instanceID = input.instanceID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            output.cullFace = input.cullFace;
            #endif
            return output;
        }
        Varyings UnpackVaryings (PackedVaryings input)
        {
            Varyings output;
            output.positionCS = input.positionCS;
            output.texCoord0 = input.interp0.xyzw;
            #if UNITY_ANY_INSTANCING_ENABLED
            output.instanceID = input.instanceID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            output.cullFace = input.cullFace;
            #endif
            return output;
        }

            // --------------------------------------------------
            // Graph

            // Graph Properties
            /* CBUFFER_START(UnityPerMaterial)
        float4 MainTex_TexelSize;
        float4 MainColor;
        float4 VarColor;
        float4 ThicknessTex_TexelSize;
        float4 SSSColor;
        float4 NormalTex_TexelSize;
        float Normal_Intensity;
        float4 MaskTex_TexelSize;
        float Smoothness;
        float AlphaClip;
        float Wind_Speed;
        float Wind_Frequency;
        float Wind_Strength;
        float3 Wind_Direction;
        float _Specular;
        float _EnvReflection;
        CBUFFER_END*/

        // Object and Global properties
        SAMPLER(SamplerState_Linear_Repeat);
        TEXTURE2D(MainTex);
        SAMPLER(samplerMainTex);
        TEXTURE2D(ThicknessTex);
        SAMPLER(samplerThicknessTex);
        TEXTURE2D(NormalTex);
        SAMPLER(samplerNormalTex);
        TEXTURE2D(MaskTex);
        SAMPLER(samplerMaskTex);

            // Graph Functions
            
        void Unity_Distance_float3(float3 A, float3 B, out float Out)
        {
            Out = distance(A, B);
        }

        void Unity_Normalize_float3(float3 In, out float3 Out)
        {
            Out = normalize(In);
        }

        void Unity_Multiply_float(float A, float B, out float Out)
        {
            Out = A * B;
        }

        void Unity_Add_float(float A, float B, out float Out)
        {
            Out = A + B;
        }

        void Unity_Sine_float(float In, out float Out)
        {
            Out = sin(In);
        }

        void Unity_Multiply_float(float3 A, float3 B, out float3 Out)
        {
            Out = A * B;
        }

        void Unity_Add_float3(float3 A, float3 B, out float3 Out)
        {
            Out = A + B;
        }

            // Graph Vertex
            struct VertexDescription
        {
            float3 Position;
            float3 Normal;
            float3 Tangent;
        };

        VertexDescription VertexDescriptionFunction(VertexDescriptionInputs IN)
        {
            VertexDescription description = (VertexDescription)0;
            float _Distance_a9453349007d43d7aa8e8ef2f9e0a6cb_Out_2;
            Unity_Distance_float3(float3(0, 0, 0), IN.ObjectSpacePosition, _Distance_a9453349007d43d7aa8e8ef2f9e0a6cb_Out_2);
            float3 _Property_294994c591114f76808f84d55e0400b9_Out_0 = Wind_Direction;
            float3 _Normalize_7a91dcb4e52548df84db8cb487a5210f_Out_1;
            Unity_Normalize_float3(_Property_294994c591114f76808f84d55e0400b9_Out_0, _Normalize_7a91dcb4e52548df84db8cb487a5210f_Out_1);
            float _Property_b1049a414de54aaf954fc9acaf3ffde3_Out_0 = Wind_Speed;
            float _Multiply_8cc8be463d9b4875839e9776be38d510_Out_2;
            Unity_Multiply_float(IN.TimeParameters.x, _Property_b1049a414de54aaf954fc9acaf3ffde3_Out_0, _Multiply_8cc8be463d9b4875839e9776be38d510_Out_2);
            float _Property_1cc960ad477c47dfa058e2368a6a0e59_Out_0 = Wind_Frequency;
            float _Split_47b65f07839b45a2a7f9fd07f6ae4f85_R_1 = SHADERGRAPH_OBJECT_POSITION[0];
            float _Split_47b65f07839b45a2a7f9fd07f6ae4f85_G_2 = SHADERGRAPH_OBJECT_POSITION[1];
            float _Split_47b65f07839b45a2a7f9fd07f6ae4f85_B_3 = SHADERGRAPH_OBJECT_POSITION[2];
            float _Split_47b65f07839b45a2a7f9fd07f6ae4f85_A_4 = 0;
            float _Add_5ff2f4775c81475eaa4e1d2f1038221c_Out_2;
            Unity_Add_float(_Split_47b65f07839b45a2a7f9fd07f6ae4f85_R_1, _Split_47b65f07839b45a2a7f9fd07f6ae4f85_G_2, _Add_5ff2f4775c81475eaa4e1d2f1038221c_Out_2);
            float _Add_0b5c85958b4a44b4a1471bfb43d09937_Out_2;
            Unity_Add_float(_Add_5ff2f4775c81475eaa4e1d2f1038221c_Out_2, _Split_47b65f07839b45a2a7f9fd07f6ae4f85_G_2, _Add_0b5c85958b4a44b4a1471bfb43d09937_Out_2);
            float _Multiply_36b4b25245ef46a6aa344b72d18b123a_Out_2;
            Unity_Multiply_float(_Property_1cc960ad477c47dfa058e2368a6a0e59_Out_0, _Add_0b5c85958b4a44b4a1471bfb43d09937_Out_2, _Multiply_36b4b25245ef46a6aa344b72d18b123a_Out_2);
            float _Add_112d774bc523412da770f4e752e0aeae_Out_2;
            Unity_Add_float(_Multiply_8cc8be463d9b4875839e9776be38d510_Out_2, _Multiply_36b4b25245ef46a6aa344b72d18b123a_Out_2, _Add_112d774bc523412da770f4e752e0aeae_Out_2);
            float _Sine_703274f04df4470980c37a4a595551c8_Out_1;
            Unity_Sine_float(_Add_112d774bc523412da770f4e752e0aeae_Out_2, _Sine_703274f04df4470980c37a4a595551c8_Out_1);
            float _Multiply_b11879df31064a879f94ea423ab2b2a5_Out_2;
            Unity_Multiply_float(_Sine_703274f04df4470980c37a4a595551c8_Out_1, 0.5, _Multiply_b11879df31064a879f94ea423ab2b2a5_Out_2);
            float _Add_efce9d790e58416f86a8ec3243204620_Out_2;
            Unity_Add_float(_Multiply_b11879df31064a879f94ea423ab2b2a5_Out_2, 0.5, _Add_efce9d790e58416f86a8ec3243204620_Out_2);
            float _Property_79b44419d559467d899224e854ea3824_Out_0 = Wind_Strength;
            float _Multiply_47956fa9e0804e8381e9c610915da0e6_Out_2;
            Unity_Multiply_float(_Add_efce9d790e58416f86a8ec3243204620_Out_2, _Property_79b44419d559467d899224e854ea3824_Out_0, _Multiply_47956fa9e0804e8381e9c610915da0e6_Out_2);
            float3 _Multiply_448ee0f8bc4f41a8bcf38fdc2b440495_Out_2;
            Unity_Multiply_float(_Normalize_7a91dcb4e52548df84db8cb487a5210f_Out_1, (_Multiply_47956fa9e0804e8381e9c610915da0e6_Out_2.xxx), _Multiply_448ee0f8bc4f41a8bcf38fdc2b440495_Out_2);
            float3 _Multiply_4e49ed416c034b90b06a8a56b2d0a49a_Out_2;
            Unity_Multiply_float((_Distance_a9453349007d43d7aa8e8ef2f9e0a6cb_Out_2.xxx), _Multiply_448ee0f8bc4f41a8bcf38fdc2b440495_Out_2, _Multiply_4e49ed416c034b90b06a8a56b2d0a49a_Out_2);
            float3 _Add_c136859739784517b3f510aa5bb4e469_Out_2;
            Unity_Add_float3(_Multiply_4e49ed416c034b90b06a8a56b2d0a49a_Out_2, IN.AbsoluteWorldSpacePosition, _Add_c136859739784517b3f510aa5bb4e469_Out_2);
            float3 _Transform_47fd3d4cb64f4d52b2077aaa7435e77b_Out_1 = TransformWorldToObject(_Add_c136859739784517b3f510aa5bb4e469_Out_2.xyz);
            description.Position = _Transform_47fd3d4cb64f4d52b2077aaa7435e77b_Out_1;
            description.Normal = IN.ObjectSpaceNormal;
            description.Tangent = IN.ObjectSpaceTangent;
            return description;
        }

            // Graph Pixel
            struct SurfaceDescription
        {
            float Alpha;
            float AlphaClipThreshold;
        };

        SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
        {
            SurfaceDescription surface = (SurfaceDescription)0;
            UnityTexture2D _Property_aed0c1bacf314879a280662567e38a31_Out_0 = UnityBuildTexture2DStructNoScale(MainTex);
            float4 _SampleTexture2D_361c1baa7e5e484ab38560dd3341b6d8_RGBA_0 = SAMPLE_TEXTURE2D(_Property_aed0c1bacf314879a280662567e38a31_Out_0.tex, _Property_aed0c1bacf314879a280662567e38a31_Out_0.samplerstate, IN.uv0.xy);
            float _SampleTexture2D_361c1baa7e5e484ab38560dd3341b6d8_R_4 = _SampleTexture2D_361c1baa7e5e484ab38560dd3341b6d8_RGBA_0.r;
            float _SampleTexture2D_361c1baa7e5e484ab38560dd3341b6d8_G_5 = _SampleTexture2D_361c1baa7e5e484ab38560dd3341b6d8_RGBA_0.g;
            float _SampleTexture2D_361c1baa7e5e484ab38560dd3341b6d8_B_6 = _SampleTexture2D_361c1baa7e5e484ab38560dd3341b6d8_RGBA_0.b;
            float _SampleTexture2D_361c1baa7e5e484ab38560dd3341b6d8_A_7 = _SampleTexture2D_361c1baa7e5e484ab38560dd3341b6d8_RGBA_0.a;
            float _Property_7784b184630844a1bd70b6904d69c970_Out_0 = AlphaClip;
            surface.Alpha = _SampleTexture2D_361c1baa7e5e484ab38560dd3341b6d8_A_7;
            surface.AlphaClipThreshold = _Property_7784b184630844a1bd70b6904d69c970_Out_0;
            return surface;
        }

            // --------------------------------------------------
            // Build Graph Inputs

            VertexDescriptionInputs BuildVertexDescriptionInputs(Attributes input)
        {
            VertexDescriptionInputs output;
            ZERO_INITIALIZE(VertexDescriptionInputs, output);

            output.ObjectSpaceNormal =           input.normalOS;
            output.WorldSpaceNormal =            TransformObjectToWorldNormal(input.normalOS);
            output.ObjectSpaceTangent =          input.tangentOS.xyz;
            output.WorldSpaceTangent =           TransformObjectToWorldDir(input.tangentOS.xyz);
            output.ObjectSpaceBiTangent =        normalize(cross(input.normalOS, input.tangentOS) * (input.tangentOS.w > 0.0f ? 1.0f : -1.0f) * GetOddNegativeScale());
            output.WorldSpaceBiTangent =         TransformObjectToWorldDir(output.ObjectSpaceBiTangent);
            output.ObjectSpacePosition =         input.positionOS;
            output.AbsoluteWorldSpacePosition =  GetAbsolutePositionWS(TransformObjectToWorld(input.positionOS));
            output.TimeParameters =              _TimeParameters.xyz;

            return output;
        }
            SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
        {
            SurfaceDescriptionInputs output;
            ZERO_INITIALIZE(SurfaceDescriptionInputs, output);





            output.uv0 =                         input.texCoord0;
        #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
        #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN output.FaceSign =                    IS_FRONT_VFACE(input.cullFace, true, false);
        #else
        #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
        #endif
        #undef BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN

            return output;
        }

            // --------------------------------------------------
            // Main

            #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/Varyings.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShadowCasterPass.hlsl"

            ENDHLSL
        }
        Pass
        {
            Name "DepthOnly"
            Tags
            {
                "LightMode" = "DepthOnly"
            }

            // Render State
            Cull Off
        Blend One Zero
        ZTest LEqual
        ZWrite On
        ColorMask 0

            // Debug
            // <None>

            // --------------------------------------------------
            // Pass

            HLSLPROGRAM

            // Pragmas
            #pragma target 4.5
        #pragma exclude_renderers gles gles3 glcore
        #pragma multi_compile_instancing
        #pragma multi_compile _ DOTS_INSTANCING_ON
        #pragma vertex vert
        #pragma fragment frag

            // DotsInstancingOptions: <None>
            // HybridV1InjectedBuiltinProperties: <None>

            // Keywords
            // PassKeywords: <None>
            // GraphKeywords: <None>

            // Defines
            #define _AlphaClip 1
            #define _NORMALMAP 1
            #define _NORMAL_DROPOFF_TS 1
            #define ATTRIBUTES_NEED_NORMAL
            #define ATTRIBUTES_NEED_TANGENT
            #define ATTRIBUTES_NEED_TEXCOORD0
            #define VARYINGS_NEED_TEXCOORD0
            #define FEATURES_GRAPH_VERTEX
            /* WARNING: $splice Could not find named fragment 'PassInstancing' */
            #define SHADERPASS SHADERPASS_DEPTHONLY
            /* WARNING: $splice Could not find named fragment 'DotsInstancingVars' */

            // Includes
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
 //#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
                // Graph Properties
            CBUFFER_START(UnityPerMaterial)
        float4 MainTex_TexelSize;
        float4 MainColor;
        float4 VarColor;
        float4 ThicknessTex_TexelSize;
        float4 SSSColor;
        float4 NormalTex_TexelSize;
        float Normal_Intensity;
        float4 MaskTex_TexelSize;
        float Smoothness;
        float AlphaClip;
        float Wind_Speed;
        float Wind_Frequency;
        float Wind_Strength;
        float3 Wind_Direction;
        float _Specular;
        float _EnvReflection;
        CBUFFER_END

        #include ".\Assets\KSC_Assets\03_Shaders\Common\Lighting_KSC.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"

            // --------------------------------------------------
            // Structs and Packing

            struct Attributes
        {
            float3 positionOS : POSITION;
            float3 normalOS : NORMAL;
            float4 tangentOS : TANGENT;
            float4 uv0 : TEXCOORD0;
            #if UNITY_ANY_INSTANCING_ENABLED
            uint instanceID : INSTANCEID_SEMANTIC;
            #endif
        };
        struct Varyings
        {
            float4 positionCS : SV_POSITION;
            float4 texCoord0;
            #if UNITY_ANY_INSTANCING_ENABLED
            uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
            #endif
        };
        struct SurfaceDescriptionInputs
        {
            float4 uv0;
        };
        struct VertexDescriptionInputs
        {
            float3 ObjectSpaceNormal;
            float3 WorldSpaceNormal;
            float3 ObjectSpaceTangent;
            float3 WorldSpaceTangent;
            float3 ObjectSpaceBiTangent;
            float3 WorldSpaceBiTangent;
            float3 ObjectSpacePosition;
            float3 AbsoluteWorldSpacePosition;
            float3 TimeParameters;
        };
        struct PackedVaryings
        {
            float4 positionCS : SV_POSITION;
            float4 interp0 : TEXCOORD0;
            #if UNITY_ANY_INSTANCING_ENABLED
            uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
            #endif
        };

            PackedVaryings PackVaryings (Varyings input)
        {
            PackedVaryings output;
            output.positionCS = input.positionCS;
            output.interp0.xyzw =  input.texCoord0;
            #if UNITY_ANY_INSTANCING_ENABLED
            output.instanceID = input.instanceID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            output.cullFace = input.cullFace;
            #endif
            return output;
        }
        Varyings UnpackVaryings (PackedVaryings input)
        {
            Varyings output;
            output.positionCS = input.positionCS;
            output.texCoord0 = input.interp0.xyzw;
            #if UNITY_ANY_INSTANCING_ENABLED
            output.instanceID = input.instanceID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            output.cullFace = input.cullFace;
            #endif
            return output;
        }

            // --------------------------------------------------
            // Graph

            // Graph Properties
          /* CBUFFER_START(UnityPerMaterial)
        float4 MainTex_TexelSize;
        float4 MainColor;
        float4 VarColor;
        float4 ThicknessTex_TexelSize;
        float4 SSSColor;
        float4 NormalTex_TexelSize;
        float Normal_Intensity;
        float4 MaskTex_TexelSize;
        float Smoothness;
        float AlphaClip;
        float Wind_Speed;
        float Wind_Frequency;
        float Wind_Strength;
        float3 Wind_Direction;
        float _Specular;
        float _EnvReflection;
        CBUFFER_END*/

        // Object and Global properties
        SAMPLER(SamplerState_Linear_Repeat);
        TEXTURE2D(MainTex);
        SAMPLER(samplerMainTex);
        TEXTURE2D(ThicknessTex);
        SAMPLER(samplerThicknessTex);
        TEXTURE2D(NormalTex);
        SAMPLER(samplerNormalTex);
        TEXTURE2D(MaskTex);
        SAMPLER(samplerMaskTex);

            // Graph Functions
            
        void Unity_Distance_float3(float3 A, float3 B, out float Out)
        {
            Out = distance(A, B);
        }

        void Unity_Normalize_float3(float3 In, out float3 Out)
        {
            Out = normalize(In);
        }

        void Unity_Multiply_float(float A, float B, out float Out)
        {
            Out = A * B;
        }

        void Unity_Add_float(float A, float B, out float Out)
        {
            Out = A + B;
        }

        void Unity_Sine_float(float In, out float Out)
        {
            Out = sin(In);
        }

        void Unity_Multiply_float(float3 A, float3 B, out float3 Out)
        {
            Out = A * B;
        }

        void Unity_Add_float3(float3 A, float3 B, out float3 Out)
        {
            Out = A + B;
        }

            // Graph Vertex
            struct VertexDescription
        {
            float3 Position;
            float3 Normal;
            float3 Tangent;
        };

        VertexDescription VertexDescriptionFunction(VertexDescriptionInputs IN)
        {
            VertexDescription description = (VertexDescription)0;
            float _Distance_a9453349007d43d7aa8e8ef2f9e0a6cb_Out_2;
            Unity_Distance_float3(float3(0, 0, 0), IN.ObjectSpacePosition, _Distance_a9453349007d43d7aa8e8ef2f9e0a6cb_Out_2);
            float3 _Property_294994c591114f76808f84d55e0400b9_Out_0 = Wind_Direction;
            float3 _Normalize_7a91dcb4e52548df84db8cb487a5210f_Out_1;
            Unity_Normalize_float3(_Property_294994c591114f76808f84d55e0400b9_Out_0, _Normalize_7a91dcb4e52548df84db8cb487a5210f_Out_1);
            float _Property_b1049a414de54aaf954fc9acaf3ffde3_Out_0 = Wind_Speed;
            float _Multiply_8cc8be463d9b4875839e9776be38d510_Out_2;
            Unity_Multiply_float(IN.TimeParameters.x, _Property_b1049a414de54aaf954fc9acaf3ffde3_Out_0, _Multiply_8cc8be463d9b4875839e9776be38d510_Out_2);
            float _Property_1cc960ad477c47dfa058e2368a6a0e59_Out_0 = Wind_Frequency;
            float _Split_47b65f07839b45a2a7f9fd07f6ae4f85_R_1 = SHADERGRAPH_OBJECT_POSITION[0];
            float _Split_47b65f07839b45a2a7f9fd07f6ae4f85_G_2 = SHADERGRAPH_OBJECT_POSITION[1];
            float _Split_47b65f07839b45a2a7f9fd07f6ae4f85_B_3 = SHADERGRAPH_OBJECT_POSITION[2];
            float _Split_47b65f07839b45a2a7f9fd07f6ae4f85_A_4 = 0;
            float _Add_5ff2f4775c81475eaa4e1d2f1038221c_Out_2;
            Unity_Add_float(_Split_47b65f07839b45a2a7f9fd07f6ae4f85_R_1, _Split_47b65f07839b45a2a7f9fd07f6ae4f85_G_2, _Add_5ff2f4775c81475eaa4e1d2f1038221c_Out_2);
            float _Add_0b5c85958b4a44b4a1471bfb43d09937_Out_2;
            Unity_Add_float(_Add_5ff2f4775c81475eaa4e1d2f1038221c_Out_2, _Split_47b65f07839b45a2a7f9fd07f6ae4f85_G_2, _Add_0b5c85958b4a44b4a1471bfb43d09937_Out_2);
            float _Multiply_36b4b25245ef46a6aa344b72d18b123a_Out_2;
            Unity_Multiply_float(_Property_1cc960ad477c47dfa058e2368a6a0e59_Out_0, _Add_0b5c85958b4a44b4a1471bfb43d09937_Out_2, _Multiply_36b4b25245ef46a6aa344b72d18b123a_Out_2);
            float _Add_112d774bc523412da770f4e752e0aeae_Out_2;
            Unity_Add_float(_Multiply_8cc8be463d9b4875839e9776be38d510_Out_2, _Multiply_36b4b25245ef46a6aa344b72d18b123a_Out_2, _Add_112d774bc523412da770f4e752e0aeae_Out_2);
            float _Sine_703274f04df4470980c37a4a595551c8_Out_1;
            Unity_Sine_float(_Add_112d774bc523412da770f4e752e0aeae_Out_2, _Sine_703274f04df4470980c37a4a595551c8_Out_1);
            float _Multiply_b11879df31064a879f94ea423ab2b2a5_Out_2;
            Unity_Multiply_float(_Sine_703274f04df4470980c37a4a595551c8_Out_1, 0.5, _Multiply_b11879df31064a879f94ea423ab2b2a5_Out_2);
            float _Add_efce9d790e58416f86a8ec3243204620_Out_2;
            Unity_Add_float(_Multiply_b11879df31064a879f94ea423ab2b2a5_Out_2, 0.5, _Add_efce9d790e58416f86a8ec3243204620_Out_2);
            float _Property_79b44419d559467d899224e854ea3824_Out_0 = Wind_Strength;
            float _Multiply_47956fa9e0804e8381e9c610915da0e6_Out_2;
            Unity_Multiply_float(_Add_efce9d790e58416f86a8ec3243204620_Out_2, _Property_79b44419d559467d899224e854ea3824_Out_0, _Multiply_47956fa9e0804e8381e9c610915da0e6_Out_2);
            float3 _Multiply_448ee0f8bc4f41a8bcf38fdc2b440495_Out_2;
            Unity_Multiply_float(_Normalize_7a91dcb4e52548df84db8cb487a5210f_Out_1, (_Multiply_47956fa9e0804e8381e9c610915da0e6_Out_2.xxx), _Multiply_448ee0f8bc4f41a8bcf38fdc2b440495_Out_2);
            float3 _Multiply_4e49ed416c034b90b06a8a56b2d0a49a_Out_2;
            Unity_Multiply_float((_Distance_a9453349007d43d7aa8e8ef2f9e0a6cb_Out_2.xxx), _Multiply_448ee0f8bc4f41a8bcf38fdc2b440495_Out_2, _Multiply_4e49ed416c034b90b06a8a56b2d0a49a_Out_2);
            float3 _Add_c136859739784517b3f510aa5bb4e469_Out_2;
            Unity_Add_float3(_Multiply_4e49ed416c034b90b06a8a56b2d0a49a_Out_2, IN.AbsoluteWorldSpacePosition, _Add_c136859739784517b3f510aa5bb4e469_Out_2);
            float3 _Transform_47fd3d4cb64f4d52b2077aaa7435e77b_Out_1 = TransformWorldToObject(_Add_c136859739784517b3f510aa5bb4e469_Out_2.xyz);
            description.Position = _Transform_47fd3d4cb64f4d52b2077aaa7435e77b_Out_1;
            description.Normal = IN.ObjectSpaceNormal;
            description.Tangent = IN.ObjectSpaceTangent;
            return description;
        }

            // Graph Pixel
            struct SurfaceDescription
        {
            float Alpha;
            float AlphaClipThreshold;
        };

        SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
        {
            SurfaceDescription surface = (SurfaceDescription)0;
            UnityTexture2D _Property_aed0c1bacf314879a280662567e38a31_Out_0 = UnityBuildTexture2DStructNoScale(MainTex);
            float4 _SampleTexture2D_361c1baa7e5e484ab38560dd3341b6d8_RGBA_0 = SAMPLE_TEXTURE2D(_Property_aed0c1bacf314879a280662567e38a31_Out_0.tex, _Property_aed0c1bacf314879a280662567e38a31_Out_0.samplerstate, IN.uv0.xy);
            float _SampleTexture2D_361c1baa7e5e484ab38560dd3341b6d8_R_4 = _SampleTexture2D_361c1baa7e5e484ab38560dd3341b6d8_RGBA_0.r;
            float _SampleTexture2D_361c1baa7e5e484ab38560dd3341b6d8_G_5 = _SampleTexture2D_361c1baa7e5e484ab38560dd3341b6d8_RGBA_0.g;
            float _SampleTexture2D_361c1baa7e5e484ab38560dd3341b6d8_B_6 = _SampleTexture2D_361c1baa7e5e484ab38560dd3341b6d8_RGBA_0.b;
            float _SampleTexture2D_361c1baa7e5e484ab38560dd3341b6d8_A_7 = _SampleTexture2D_361c1baa7e5e484ab38560dd3341b6d8_RGBA_0.a;
            float _Property_7784b184630844a1bd70b6904d69c970_Out_0 = AlphaClip;
            surface.Alpha = _SampleTexture2D_361c1baa7e5e484ab38560dd3341b6d8_A_7;
            surface.AlphaClipThreshold = _Property_7784b184630844a1bd70b6904d69c970_Out_0;
            return surface;
        }

            // --------------------------------------------------
            // Build Graph Inputs

            VertexDescriptionInputs BuildVertexDescriptionInputs(Attributes input)
        {
            VertexDescriptionInputs output;
            ZERO_INITIALIZE(VertexDescriptionInputs, output);

            output.ObjectSpaceNormal =           input.normalOS;
            output.WorldSpaceNormal =            TransformObjectToWorldNormal(input.normalOS);
            output.ObjectSpaceTangent =          input.tangentOS.xyz;
            output.WorldSpaceTangent =           TransformObjectToWorldDir(input.tangentOS.xyz);
            output.ObjectSpaceBiTangent =        normalize(cross(input.normalOS, input.tangentOS) * (input.tangentOS.w > 0.0f ? 1.0f : -1.0f) * GetOddNegativeScale());
            output.WorldSpaceBiTangent =         TransformObjectToWorldDir(output.ObjectSpaceBiTangent);
            output.ObjectSpacePosition =         input.positionOS;
            output.AbsoluteWorldSpacePosition =  GetAbsolutePositionWS(TransformObjectToWorld(input.positionOS));
            output.TimeParameters =              _TimeParameters.xyz;

            return output;
        }
            SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
        {
            SurfaceDescriptionInputs output;
            ZERO_INITIALIZE(SurfaceDescriptionInputs, output);





            output.uv0 =                         input.texCoord0;
        #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
        #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN output.FaceSign =                    IS_FRONT_VFACE(input.cullFace, true, false);
        #else
        #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
        #endif
        #undef BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN

            return output;
        }

            // --------------------------------------------------
            // Main

            #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/Varyings.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/DepthOnlyPass.hlsl"

            ENDHLSL
        }
        Pass
        {
            Name "DepthNormals"
            Tags
            {
                "LightMode" = "DepthNormals"
            }

            // Render State
            Cull Off
        Blend One Zero
        ZTest LEqual
        ZWrite On

            // Debug
            // <None>

            // --------------------------------------------------
            // Pass

            HLSLPROGRAM

            // Pragmas
            #pragma target 4.5
        #pragma exclude_renderers gles gles3 glcore
        #pragma multi_compile_instancing
        #pragma multi_compile _ DOTS_INSTANCING_ON
        #pragma vertex vert
        #pragma fragment frag

            // DotsInstancingOptions: <None>
            // HybridV1InjectedBuiltinProperties: <None>

            // Keywords
            // PassKeywords: <None>
            // GraphKeywords: <None>

            // Defines
            #define _AlphaClip 1
            #define _NORMALMAP 1
            #define _NORMAL_DROPOFF_TS 1
            #define ATTRIBUTES_NEED_NORMAL
            #define ATTRIBUTES_NEED_TANGENT
            #define ATTRIBUTES_NEED_TEXCOORD0
            #define ATTRIBUTES_NEED_TEXCOORD1
            #define VARYINGS_NEED_NORMAL_WS
            #define VARYINGS_NEED_TANGENT_WS
            #define VARYINGS_NEED_TEXCOORD0
            #define FEATURES_GRAPH_VERTEX
            /* WARNING: $splice Could not find named fragment 'PassInstancing' */
            #define SHADERPASS SHADERPASS_DEPTHNORMALSONLY
            /* WARNING: $splice Could not find named fragment 'DotsInstancingVars' */

            // Includes
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
 //#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
                // Graph Properties
            CBUFFER_START(UnityPerMaterial)
        float4 MainTex_TexelSize;
        float4 MainColor;
        float4 VarColor;
        float4 ThicknessTex_TexelSize;
        float4 SSSColor;
        float4 NormalTex_TexelSize;
        float Normal_Intensity;
        float4 MaskTex_TexelSize;
        float Smoothness;
        float AlphaClip;
        float Wind_Speed;
        float Wind_Frequency;
        float Wind_Strength;
        float3 Wind_Direction;
        float _Specular;
        float _EnvReflection;
        CBUFFER_END

        #include ".\Assets\KSC_Assets\03_Shaders\Common\Lighting_KSC.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"

            // --------------------------------------------------
            // Structs and Packing

            struct Attributes
        {
            float3 positionOS : POSITION;
            float3 normalOS : NORMAL;
            float4 tangentOS : TANGENT;
            float4 uv0 : TEXCOORD0;
            float4 uv1 : TEXCOORD1;
            #if UNITY_ANY_INSTANCING_ENABLED
            uint instanceID : INSTANCEID_SEMANTIC;
            #endif
        };
        struct Varyings
        {
            float4 positionCS : SV_POSITION;
            float3 normalWS;
            float4 tangentWS;
            float4 texCoord0;
            #if UNITY_ANY_INSTANCING_ENABLED
            uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
            #endif
        };
        struct SurfaceDescriptionInputs
        {
            float3 TangentSpaceNormal;
            float4 uv0;
        };
        struct VertexDescriptionInputs
        {
            float3 ObjectSpaceNormal;
            float3 WorldSpaceNormal;
            float3 ObjectSpaceTangent;
            float3 WorldSpaceTangent;
            float3 ObjectSpaceBiTangent;
            float3 WorldSpaceBiTangent;
            float3 ObjectSpacePosition;
            float3 AbsoluteWorldSpacePosition;
            float3 TimeParameters;
        };
        struct PackedVaryings
        {
            float4 positionCS : SV_POSITION;
            float3 interp0 : TEXCOORD0;
            float4 interp1 : TEXCOORD1;
            float4 interp2 : TEXCOORD2;
            #if UNITY_ANY_INSTANCING_ENABLED
            uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
            #endif
        };

            PackedVaryings PackVaryings (Varyings input)
        {
            PackedVaryings output;
            output.positionCS = input.positionCS;
            output.interp0.xyz =  input.normalWS;
            output.interp1.xyzw =  input.tangentWS;
            output.interp2.xyzw =  input.texCoord0;
            #if UNITY_ANY_INSTANCING_ENABLED
            output.instanceID = input.instanceID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            output.cullFace = input.cullFace;
            #endif
            return output;
        }
        Varyings UnpackVaryings (PackedVaryings input)
        {
            Varyings output;
            output.positionCS = input.positionCS;
            output.normalWS = input.interp0.xyz;
            output.tangentWS = input.interp1.xyzw;
            output.texCoord0 = input.interp2.xyzw;
            #if UNITY_ANY_INSTANCING_ENABLED
            output.instanceID = input.instanceID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            output.cullFace = input.cullFace;
            #endif
            return output;
        }

            // --------------------------------------------------
            // Graph

            /* CBUFFER_START(UnityPerMaterial)
        float4 MainTex_TexelSize;
        float4 MainColor;
        float4 VarColor;
        float4 ThicknessTex_TexelSize;
        float4 SSSColor;
        float4 NormalTex_TexelSize;
        float Normal_Intensity;
        float4 MaskTex_TexelSize;
        float Smoothness;
        float AlphaClip;
        float Wind_Speed;
        float Wind_Frequency;
        float Wind_Strength;
        float3 Wind_Direction;
        float _Specular;
        float _EnvReflection;
        CBUFFER_END*/

        // Object and Global properties
        SAMPLER(SamplerState_Linear_Repeat);
        TEXTURE2D(MainTex);
        SAMPLER(samplerMainTex);
        TEXTURE2D(ThicknessTex);
        SAMPLER(samplerThicknessTex);
        TEXTURE2D(NormalTex);
        SAMPLER(samplerNormalTex);
        TEXTURE2D(MaskTex);
        SAMPLER(samplerMaskTex);

            // Graph Functions
            
        void Unity_Distance_float3(float3 A, float3 B, out float Out)
        {
            Out = distance(A, B);
        }

        void Unity_Normalize_float3(float3 In, out float3 Out)
        {
            Out = normalize(In);
        }

        void Unity_Multiply_float(float A, float B, out float Out)
        {
            Out = A * B;
        }

        void Unity_Add_float(float A, float B, out float Out)
        {
            Out = A + B;
        }

        void Unity_Sine_float(float In, out float Out)
        {
            Out = sin(In);
        }

        void Unity_Multiply_float(float3 A, float3 B, out float3 Out)
        {
            Out = A * B;
        }

        void Unity_Add_float3(float3 A, float3 B, out float3 Out)
        {
            Out = A + B;
        }

        void Unity_NormalStrength_float(float3 In, float Strength, out float3 Out)
        {
            Out = float3(In.rg * Strength, lerp(1, In.b, saturate(Strength)));
        }

            // Graph Vertex
            struct VertexDescription
        {
            float3 Position;
            float3 Normal;
            float3 Tangent;
        };

        VertexDescription VertexDescriptionFunction(VertexDescriptionInputs IN)
        {
            VertexDescription description = (VertexDescription)0;
            float _Distance_a9453349007d43d7aa8e8ef2f9e0a6cb_Out_2;
            Unity_Distance_float3(float3(0, 0, 0), IN.ObjectSpacePosition, _Distance_a9453349007d43d7aa8e8ef2f9e0a6cb_Out_2);
            float3 _Property_294994c591114f76808f84d55e0400b9_Out_0 = Wind_Direction;
            float3 _Normalize_7a91dcb4e52548df84db8cb487a5210f_Out_1;
            Unity_Normalize_float3(_Property_294994c591114f76808f84d55e0400b9_Out_0, _Normalize_7a91dcb4e52548df84db8cb487a5210f_Out_1);
            float _Property_b1049a414de54aaf954fc9acaf3ffde3_Out_0 = Wind_Speed;
            float _Multiply_8cc8be463d9b4875839e9776be38d510_Out_2;
            Unity_Multiply_float(IN.TimeParameters.x, _Property_b1049a414de54aaf954fc9acaf3ffde3_Out_0, _Multiply_8cc8be463d9b4875839e9776be38d510_Out_2);
            float _Property_1cc960ad477c47dfa058e2368a6a0e59_Out_0 = Wind_Frequency;
            float _Split_47b65f07839b45a2a7f9fd07f6ae4f85_R_1 = SHADERGRAPH_OBJECT_POSITION[0];
            float _Split_47b65f07839b45a2a7f9fd07f6ae4f85_G_2 = SHADERGRAPH_OBJECT_POSITION[1];
            float _Split_47b65f07839b45a2a7f9fd07f6ae4f85_B_3 = SHADERGRAPH_OBJECT_POSITION[2];
            float _Split_47b65f07839b45a2a7f9fd07f6ae4f85_A_4 = 0;
            float _Add_5ff2f4775c81475eaa4e1d2f1038221c_Out_2;
            Unity_Add_float(_Split_47b65f07839b45a2a7f9fd07f6ae4f85_R_1, _Split_47b65f07839b45a2a7f9fd07f6ae4f85_G_2, _Add_5ff2f4775c81475eaa4e1d2f1038221c_Out_2);
            float _Add_0b5c85958b4a44b4a1471bfb43d09937_Out_2;
            Unity_Add_float(_Add_5ff2f4775c81475eaa4e1d2f1038221c_Out_2, _Split_47b65f07839b45a2a7f9fd07f6ae4f85_G_2, _Add_0b5c85958b4a44b4a1471bfb43d09937_Out_2);
            float _Multiply_36b4b25245ef46a6aa344b72d18b123a_Out_2;
            Unity_Multiply_float(_Property_1cc960ad477c47dfa058e2368a6a0e59_Out_0, _Add_0b5c85958b4a44b4a1471bfb43d09937_Out_2, _Multiply_36b4b25245ef46a6aa344b72d18b123a_Out_2);
            float _Add_112d774bc523412da770f4e752e0aeae_Out_2;
            Unity_Add_float(_Multiply_8cc8be463d9b4875839e9776be38d510_Out_2, _Multiply_36b4b25245ef46a6aa344b72d18b123a_Out_2, _Add_112d774bc523412da770f4e752e0aeae_Out_2);
            float _Sine_703274f04df4470980c37a4a595551c8_Out_1;
            Unity_Sine_float(_Add_112d774bc523412da770f4e752e0aeae_Out_2, _Sine_703274f04df4470980c37a4a595551c8_Out_1);
            float _Multiply_b11879df31064a879f94ea423ab2b2a5_Out_2;
            Unity_Multiply_float(_Sine_703274f04df4470980c37a4a595551c8_Out_1, 0.5, _Multiply_b11879df31064a879f94ea423ab2b2a5_Out_2);
            float _Add_efce9d790e58416f86a8ec3243204620_Out_2;
            Unity_Add_float(_Multiply_b11879df31064a879f94ea423ab2b2a5_Out_2, 0.5, _Add_efce9d790e58416f86a8ec3243204620_Out_2);
            float _Property_79b44419d559467d899224e854ea3824_Out_0 = Wind_Strength;
            float _Multiply_47956fa9e0804e8381e9c610915da0e6_Out_2;
            Unity_Multiply_float(_Add_efce9d790e58416f86a8ec3243204620_Out_2, _Property_79b44419d559467d899224e854ea3824_Out_0, _Multiply_47956fa9e0804e8381e9c610915da0e6_Out_2);
            float3 _Multiply_448ee0f8bc4f41a8bcf38fdc2b440495_Out_2;
            Unity_Multiply_float(_Normalize_7a91dcb4e52548df84db8cb487a5210f_Out_1, (_Multiply_47956fa9e0804e8381e9c610915da0e6_Out_2.xxx), _Multiply_448ee0f8bc4f41a8bcf38fdc2b440495_Out_2);
            float3 _Multiply_4e49ed416c034b90b06a8a56b2d0a49a_Out_2;
            Unity_Multiply_float((_Distance_a9453349007d43d7aa8e8ef2f9e0a6cb_Out_2.xxx), _Multiply_448ee0f8bc4f41a8bcf38fdc2b440495_Out_2, _Multiply_4e49ed416c034b90b06a8a56b2d0a49a_Out_2);
            float3 _Add_c136859739784517b3f510aa5bb4e469_Out_2;
            Unity_Add_float3(_Multiply_4e49ed416c034b90b06a8a56b2d0a49a_Out_2, IN.AbsoluteWorldSpacePosition, _Add_c136859739784517b3f510aa5bb4e469_Out_2);
            float3 _Transform_47fd3d4cb64f4d52b2077aaa7435e77b_Out_1 = TransformWorldToObject(_Add_c136859739784517b3f510aa5bb4e469_Out_2.xyz);
            description.Position = _Transform_47fd3d4cb64f4d52b2077aaa7435e77b_Out_1;
            description.Normal = IN.ObjectSpaceNormal;
            description.Tangent = IN.ObjectSpaceTangent;
            return description;
        }

            // Graph Pixel
            struct SurfaceDescription
        {
            float3 NormalTS;
            float Alpha;
            float AlphaClipThreshold;
        };

        SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
        {
            SurfaceDescription surface = (SurfaceDescription)0;
            UnityTexture2D _Property_9a5e1d21ff144daeb1ad132f767da44e_Out_0 = UnityBuildTexture2DStructNoScale(NormalTex);
            float4 _SampleTexture2D_418ee20218b748af9c02a36c83fb414e_RGBA_0 = SAMPLE_TEXTURE2D(_Property_9a5e1d21ff144daeb1ad132f767da44e_Out_0.tex, _Property_9a5e1d21ff144daeb1ad132f767da44e_Out_0.samplerstate, IN.uv0.xy);
            _SampleTexture2D_418ee20218b748af9c02a36c83fb414e_RGBA_0.rgb = UnpackNormal(_SampleTexture2D_418ee20218b748af9c02a36c83fb414e_RGBA_0);
            float _SampleTexture2D_418ee20218b748af9c02a36c83fb414e_R_4 = _SampleTexture2D_418ee20218b748af9c02a36c83fb414e_RGBA_0.r;
            float _SampleTexture2D_418ee20218b748af9c02a36c83fb414e_G_5 = _SampleTexture2D_418ee20218b748af9c02a36c83fb414e_RGBA_0.g;
            float _SampleTexture2D_418ee20218b748af9c02a36c83fb414e_B_6 = _SampleTexture2D_418ee20218b748af9c02a36c83fb414e_RGBA_0.b;
            float _SampleTexture2D_418ee20218b748af9c02a36c83fb414e_A_7 = _SampleTexture2D_418ee20218b748af9c02a36c83fb414e_RGBA_0.a;
            float _Property_c176ee8debc744ebae505ad20fab01dd_Out_0 = Normal_Intensity;
            float3 _NormalStrength_05e86a137c744443b9dfa31f57ac64cd_Out_2;
            Unity_NormalStrength_float((_SampleTexture2D_418ee20218b748af9c02a36c83fb414e_RGBA_0.xyz), _Property_c176ee8debc744ebae505ad20fab01dd_Out_0, _NormalStrength_05e86a137c744443b9dfa31f57ac64cd_Out_2);
            UnityTexture2D _Property_aed0c1bacf314879a280662567e38a31_Out_0 = UnityBuildTexture2DStructNoScale(MainTex);
            float4 _SampleTexture2D_361c1baa7e5e484ab38560dd3341b6d8_RGBA_0 = SAMPLE_TEXTURE2D(_Property_aed0c1bacf314879a280662567e38a31_Out_0.tex, _Property_aed0c1bacf314879a280662567e38a31_Out_0.samplerstate, IN.uv0.xy);
            float _SampleTexture2D_361c1baa7e5e484ab38560dd3341b6d8_R_4 = _SampleTexture2D_361c1baa7e5e484ab38560dd3341b6d8_RGBA_0.r;
            float _SampleTexture2D_361c1baa7e5e484ab38560dd3341b6d8_G_5 = _SampleTexture2D_361c1baa7e5e484ab38560dd3341b6d8_RGBA_0.g;
            float _SampleTexture2D_361c1baa7e5e484ab38560dd3341b6d8_B_6 = _SampleTexture2D_361c1baa7e5e484ab38560dd3341b6d8_RGBA_0.b;
            float _SampleTexture2D_361c1baa7e5e484ab38560dd3341b6d8_A_7 = _SampleTexture2D_361c1baa7e5e484ab38560dd3341b6d8_RGBA_0.a;
            float _Property_7784b184630844a1bd70b6904d69c970_Out_0 = AlphaClip;
            surface.NormalTS = _NormalStrength_05e86a137c744443b9dfa31f57ac64cd_Out_2;
            surface.Alpha = _SampleTexture2D_361c1baa7e5e484ab38560dd3341b6d8_A_7;
            surface.AlphaClipThreshold = _Property_7784b184630844a1bd70b6904d69c970_Out_0;
            return surface;
        }

            // --------------------------------------------------
            // Build Graph Inputs

            VertexDescriptionInputs BuildVertexDescriptionInputs(Attributes input)
        {
            VertexDescriptionInputs output;
            ZERO_INITIALIZE(VertexDescriptionInputs, output);

            output.ObjectSpaceNormal =           input.normalOS;
            output.WorldSpaceNormal =            TransformObjectToWorldNormal(input.normalOS);
            output.ObjectSpaceTangent =          input.tangentOS.xyz;
            output.WorldSpaceTangent =           TransformObjectToWorldDir(input.tangentOS.xyz);
            output.ObjectSpaceBiTangent =        normalize(cross(input.normalOS, input.tangentOS) * (input.tangentOS.w > 0.0f ? 1.0f : -1.0f) * GetOddNegativeScale());
            output.WorldSpaceBiTangent =         TransformObjectToWorldDir(output.ObjectSpaceBiTangent);
            output.ObjectSpacePosition =         input.positionOS;
            output.AbsoluteWorldSpacePosition =  GetAbsolutePositionWS(TransformObjectToWorld(input.positionOS));
            output.TimeParameters =              _TimeParameters.xyz;

            return output;
        }
            SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
        {
            SurfaceDescriptionInputs output;
            ZERO_INITIALIZE(SurfaceDescriptionInputs, output);



            output.TangentSpaceNormal =          float3(0.0f, 0.0f, 1.0f);


            output.uv0 =                         input.texCoord0;
        #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
        #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN output.FaceSign =                    IS_FRONT_VFACE(input.cullFace, true, false);
        #else
        #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
        #endif
        #undef BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN

            return output;
        }

            // --------------------------------------------------
            // Main

            #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/Varyings.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/DepthNormalsOnlyPass.hlsl"

            ENDHLSL
        }
        Pass
        {
            Name "Meta"
            Tags
            {
                "LightMode" = "Meta"
            }

            // Render State
            Cull Off

            // Debug
            // <None>

            // --------------------------------------------------
            // Pass

            HLSLPROGRAM

            // Pragmas
            #pragma target 4.5
        #pragma exclude_renderers gles gles3 glcore
        #pragma vertex vert
        #pragma fragment frag

            // DotsInstancingOptions: <None>
            // HybridV1InjectedBuiltinProperties: <None>

            // Keywords
            #pragma shader_feature _ _SMOOTHNESS_TEXTURE_ALBEDO_CHANNEL_A
            // GraphKeywords: <None>

            // Defines
            #define _AlphaClip 1
            #define _NORMALMAP 1
            #define _NORMAL_DROPOFF_TS 1
            #define ATTRIBUTES_NEED_NORMAL
            #define ATTRIBUTES_NEED_TANGENT
            #define ATTRIBUTES_NEED_TEXCOORD0
            #define ATTRIBUTES_NEED_TEXCOORD1
            #define ATTRIBUTES_NEED_TEXCOORD2
            #define VARYINGS_NEED_POSITION_WS
            #define VARYINGS_NEED_TEXCOORD0
            #define FEATURES_GRAPH_VERTEX
            /* WARNING: $splice Could not find named fragment 'PassInstancing' */
            #define SHADERPASS SHADERPASS_META
            /* WARNING: $splice Could not find named fragment 'DotsInstancingVars' */

            // Includes
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
  //#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
                // Graph Properties
            CBUFFER_START(UnityPerMaterial)
        float4 MainTex_TexelSize;
        float4 MainColor;
        float4 VarColor;
        float4 ThicknessTex_TexelSize;
        float4 SSSColor;
        float4 NormalTex_TexelSize;
        float Normal_Intensity;
        float4 MaskTex_TexelSize;
        float Smoothness;
        float AlphaClip;
        float Wind_Speed;
        float Wind_Frequency;
        float Wind_Strength;
        float3 Wind_Direction;
        float _Specular;
        float _EnvReflection;
        CBUFFER_END

        #include ".\Assets\KSC_Assets\03_Shaders\Common\Lighting_KSC.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/MetaInput.hlsl"

            // --------------------------------------------------
            // Structs and Packing

            struct Attributes
        {
            float3 positionOS : POSITION;
            float3 normalOS : NORMAL;
            float4 tangentOS : TANGENT;
            float4 uv0 : TEXCOORD0;
            float4 uv1 : TEXCOORD1;
            float4 uv2 : TEXCOORD2;
            #if UNITY_ANY_INSTANCING_ENABLED
            uint instanceID : INSTANCEID_SEMANTIC;
            #endif
        };
        struct Varyings
        {
            float4 positionCS : SV_POSITION;
            float3 positionWS;
            float4 texCoord0;
            #if UNITY_ANY_INSTANCING_ENABLED
            uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
            #endif
        };
        struct SurfaceDescriptionInputs
        {
            float3 AbsoluteWorldSpacePosition;
            float4 uv0;
        };
        struct VertexDescriptionInputs
        {
            float3 ObjectSpaceNormal;
            float3 WorldSpaceNormal;
            float3 ObjectSpaceTangent;
            float3 WorldSpaceTangent;
            float3 ObjectSpaceBiTangent;
            float3 WorldSpaceBiTangent;
            float3 ObjectSpacePosition;
            float3 AbsoluteWorldSpacePosition;
            float3 TimeParameters;
        };
        struct PackedVaryings
        {
            float4 positionCS : SV_POSITION;
            float3 interp0 : TEXCOORD0;
            float4 interp1 : TEXCOORD1;
            #if UNITY_ANY_INSTANCING_ENABLED
            uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
            #endif
        };

            PackedVaryings PackVaryings (Varyings input)
        {
            PackedVaryings output;
            output.positionCS = input.positionCS;
            output.interp0.xyz =  input.positionWS;
            output.interp1.xyzw =  input.texCoord0;
            #if UNITY_ANY_INSTANCING_ENABLED
            output.instanceID = input.instanceID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            output.cullFace = input.cullFace;
            #endif
            return output;
        }
        Varyings UnpackVaryings (PackedVaryings input)
        {
            Varyings output;
            output.positionCS = input.positionCS;
            output.positionWS = input.interp0.xyz;
            output.texCoord0 = input.interp1.xyzw;
            #if UNITY_ANY_INSTANCING_ENABLED
            output.instanceID = input.instanceID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            output.cullFace = input.cullFace;
            #endif
            return output;
        }

            // --------------------------------------------------
            // Graph

            // Graph Properties
            /* CBUFFER_START(UnityPerMaterial)
        float4 MainTex_TexelSize;
        float4 MainColor;
        float4 VarColor;
        float4 ThicknessTex_TexelSize;
        float4 SSSColor;
        float4 NormalTex_TexelSize;
        float Normal_Intensity;
        float4 MaskTex_TexelSize;
        float Smoothness;
        float AlphaClip;
        float Wind_Speed;
        float Wind_Frequency;
        float Wind_Strength;
        float3 Wind_Direction;
        float _Specular;
        float _EnvReflection;
        CBUFFER_END*/

        // Object and Global properties
        SAMPLER(SamplerState_Linear_Repeat);
        TEXTURE2D(MainTex);
        SAMPLER(samplerMainTex);
        TEXTURE2D(ThicknessTex);
        SAMPLER(samplerThicknessTex);
        TEXTURE2D(NormalTex);
        SAMPLER(samplerNormalTex);
        TEXTURE2D(MaskTex);
        SAMPLER(samplerMaskTex);

            // Graph Functions
            
        void Unity_Distance_float3(float3 A, float3 B, out float Out)
        {
            Out = distance(A, B);
        }

        void Unity_Normalize_float3(float3 In, out float3 Out)
        {
            Out = normalize(In);
        }

        void Unity_Multiply_float(float A, float B, out float Out)
        {
            Out = A * B;
        }

        void Unity_Add_float(float A, float B, out float Out)
        {
            Out = A + B;
        }

        void Unity_Sine_float(float In, out float Out)
        {
            Out = sin(In);
        }

        void Unity_Multiply_float(float3 A, float3 B, out float3 Out)
        {
            Out = A * B;
        }

        void Unity_Add_float3(float3 A, float3 B, out float3 Out)
        {
            Out = A + B;
        }

        void Unity_Multiply_float(float4 A, float4 B, out float4 Out)
        {
            Out = A * B;
        }

        void Unity_Fraction_float(float In, out float Out)
        {
            Out = frac(In);
        }

        void Unity_Lerp_float4(float4 A, float4 B, float4 T, out float4 Out)
        {
            Out = lerp(A, B, T);
        }

        // 42362d8732025af29918c5cc8ad4975c
        #include "Assets/KSC_Assets/03_Shaders/Common/Lighting_Function.hlsl"

        void Unity_DotProduct_float3(float3 A, float3 B, out float Out)
        {
            Out = dot(A, B);
        }

        void Unity_Saturate_float(float In, out float Out)
        {
            Out = saturate(In);
        }

            // Graph Vertex
            struct VertexDescription
        {
            float3 Position;
            float3 Normal;
            float3 Tangent;
        };

        VertexDescription VertexDescriptionFunction(VertexDescriptionInputs IN)
        {
            VertexDescription description = (VertexDescription)0;
            float _Distance_a9453349007d43d7aa8e8ef2f9e0a6cb_Out_2;
            Unity_Distance_float3(float3(0, 0, 0), IN.ObjectSpacePosition, _Distance_a9453349007d43d7aa8e8ef2f9e0a6cb_Out_2);
            float3 _Property_294994c591114f76808f84d55e0400b9_Out_0 = Wind_Direction;
            float3 _Normalize_7a91dcb4e52548df84db8cb487a5210f_Out_1;
            Unity_Normalize_float3(_Property_294994c591114f76808f84d55e0400b9_Out_0, _Normalize_7a91dcb4e52548df84db8cb487a5210f_Out_1);
            float _Property_b1049a414de54aaf954fc9acaf3ffde3_Out_0 = Wind_Speed;
            float _Multiply_8cc8be463d9b4875839e9776be38d510_Out_2;
            Unity_Multiply_float(IN.TimeParameters.x, _Property_b1049a414de54aaf954fc9acaf3ffde3_Out_0, _Multiply_8cc8be463d9b4875839e9776be38d510_Out_2);
            float _Property_1cc960ad477c47dfa058e2368a6a0e59_Out_0 = Wind_Frequency;
            float _Split_47b65f07839b45a2a7f9fd07f6ae4f85_R_1 = SHADERGRAPH_OBJECT_POSITION[0];
            float _Split_47b65f07839b45a2a7f9fd07f6ae4f85_G_2 = SHADERGRAPH_OBJECT_POSITION[1];
            float _Split_47b65f07839b45a2a7f9fd07f6ae4f85_B_3 = SHADERGRAPH_OBJECT_POSITION[2];
            float _Split_47b65f07839b45a2a7f9fd07f6ae4f85_A_4 = 0;
            float _Add_5ff2f4775c81475eaa4e1d2f1038221c_Out_2;
            Unity_Add_float(_Split_47b65f07839b45a2a7f9fd07f6ae4f85_R_1, _Split_47b65f07839b45a2a7f9fd07f6ae4f85_G_2, _Add_5ff2f4775c81475eaa4e1d2f1038221c_Out_2);
            float _Add_0b5c85958b4a44b4a1471bfb43d09937_Out_2;
            Unity_Add_float(_Add_5ff2f4775c81475eaa4e1d2f1038221c_Out_2, _Split_47b65f07839b45a2a7f9fd07f6ae4f85_G_2, _Add_0b5c85958b4a44b4a1471bfb43d09937_Out_2);
            float _Multiply_36b4b25245ef46a6aa344b72d18b123a_Out_2;
            Unity_Multiply_float(_Property_1cc960ad477c47dfa058e2368a6a0e59_Out_0, _Add_0b5c85958b4a44b4a1471bfb43d09937_Out_2, _Multiply_36b4b25245ef46a6aa344b72d18b123a_Out_2);
            float _Add_112d774bc523412da770f4e752e0aeae_Out_2;
            Unity_Add_float(_Multiply_8cc8be463d9b4875839e9776be38d510_Out_2, _Multiply_36b4b25245ef46a6aa344b72d18b123a_Out_2, _Add_112d774bc523412da770f4e752e0aeae_Out_2);
            float _Sine_703274f04df4470980c37a4a595551c8_Out_1;
            Unity_Sine_float(_Add_112d774bc523412da770f4e752e0aeae_Out_2, _Sine_703274f04df4470980c37a4a595551c8_Out_1);
            float _Multiply_b11879df31064a879f94ea423ab2b2a5_Out_2;
            Unity_Multiply_float(_Sine_703274f04df4470980c37a4a595551c8_Out_1, 0.5, _Multiply_b11879df31064a879f94ea423ab2b2a5_Out_2);
            float _Add_efce9d790e58416f86a8ec3243204620_Out_2;
            Unity_Add_float(_Multiply_b11879df31064a879f94ea423ab2b2a5_Out_2, 0.5, _Add_efce9d790e58416f86a8ec3243204620_Out_2);
            float _Property_79b44419d559467d899224e854ea3824_Out_0 = Wind_Strength;
            float _Multiply_47956fa9e0804e8381e9c610915da0e6_Out_2;
            Unity_Multiply_float(_Add_efce9d790e58416f86a8ec3243204620_Out_2, _Property_79b44419d559467d899224e854ea3824_Out_0, _Multiply_47956fa9e0804e8381e9c610915da0e6_Out_2);
            float3 _Multiply_448ee0f8bc4f41a8bcf38fdc2b440495_Out_2;
            Unity_Multiply_float(_Normalize_7a91dcb4e52548df84db8cb487a5210f_Out_1, (_Multiply_47956fa9e0804e8381e9c610915da0e6_Out_2.xxx), _Multiply_448ee0f8bc4f41a8bcf38fdc2b440495_Out_2);
            float3 _Multiply_4e49ed416c034b90b06a8a56b2d0a49a_Out_2;
            Unity_Multiply_float((_Distance_a9453349007d43d7aa8e8ef2f9e0a6cb_Out_2.xxx), _Multiply_448ee0f8bc4f41a8bcf38fdc2b440495_Out_2, _Multiply_4e49ed416c034b90b06a8a56b2d0a49a_Out_2);
            float3 _Add_c136859739784517b3f510aa5bb4e469_Out_2;
            Unity_Add_float3(_Multiply_4e49ed416c034b90b06a8a56b2d0a49a_Out_2, IN.AbsoluteWorldSpacePosition, _Add_c136859739784517b3f510aa5bb4e469_Out_2);
            float3 _Transform_47fd3d4cb64f4d52b2077aaa7435e77b_Out_1 = TransformWorldToObject(_Add_c136859739784517b3f510aa5bb4e469_Out_2.xyz);
            description.Position = _Transform_47fd3d4cb64f4d52b2077aaa7435e77b_Out_1;
            description.Normal = IN.ObjectSpaceNormal;
            description.Tangent = IN.ObjectSpaceTangent;
            return description;
        }

            // Graph Pixel
            struct SurfaceDescription
        {
            float3 BaseColor;
            float3 Emission;
            float Alpha;
            float AlphaClipThreshold;
        };

        SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
        {
            SurfaceDescription surface = (SurfaceDescription)0;
            float4 _Property_1931a4995a954dc59a2a81efc2ac7bc3_Out_0 = MainColor;
            UnityTexture2D _Property_aed0c1bacf314879a280662567e38a31_Out_0 = UnityBuildTexture2DStructNoScale(MainTex);
            float4 _SampleTexture2D_361c1baa7e5e484ab38560dd3341b6d8_RGBA_0 = SAMPLE_TEXTURE2D(_Property_aed0c1bacf314879a280662567e38a31_Out_0.tex, _Property_aed0c1bacf314879a280662567e38a31_Out_0.samplerstate, IN.uv0.xy);
            float _SampleTexture2D_361c1baa7e5e484ab38560dd3341b6d8_R_4 = _SampleTexture2D_361c1baa7e5e484ab38560dd3341b6d8_RGBA_0.r;
            float _SampleTexture2D_361c1baa7e5e484ab38560dd3341b6d8_G_5 = _SampleTexture2D_361c1baa7e5e484ab38560dd3341b6d8_RGBA_0.g;
            float _SampleTexture2D_361c1baa7e5e484ab38560dd3341b6d8_B_6 = _SampleTexture2D_361c1baa7e5e484ab38560dd3341b6d8_RGBA_0.b;
            float _SampleTexture2D_361c1baa7e5e484ab38560dd3341b6d8_A_7 = _SampleTexture2D_361c1baa7e5e484ab38560dd3341b6d8_RGBA_0.a;
            float4 _Multiply_72a5e6242399449aad550d9798747548_Out_2;
            Unity_Multiply_float(_Property_1931a4995a954dc59a2a81efc2ac7bc3_Out_0, _SampleTexture2D_361c1baa7e5e484ab38560dd3341b6d8_RGBA_0, _Multiply_72a5e6242399449aad550d9798747548_Out_2);
            float4 _Property_2e6c583a57424a8180974b05f7e49119_Out_0 = VarColor;
            float4 _Multiply_c5a7db016b7b4a19a5d1eb518bbd1ebe_Out_2;
            Unity_Multiply_float(_Property_2e6c583a57424a8180974b05f7e49119_Out_0, _SampleTexture2D_361c1baa7e5e484ab38560dd3341b6d8_RGBA_0, _Multiply_c5a7db016b7b4a19a5d1eb518bbd1ebe_Out_2);
            float _Split_47b65f07839b45a2a7f9fd07f6ae4f85_R_1 = SHADERGRAPH_OBJECT_POSITION[0];
            float _Split_47b65f07839b45a2a7f9fd07f6ae4f85_G_2 = SHADERGRAPH_OBJECT_POSITION[1];
            float _Split_47b65f07839b45a2a7f9fd07f6ae4f85_B_3 = SHADERGRAPH_OBJECT_POSITION[2];
            float _Split_47b65f07839b45a2a7f9fd07f6ae4f85_A_4 = 0;
            float _Add_5ff2f4775c81475eaa4e1d2f1038221c_Out_2;
            Unity_Add_float(_Split_47b65f07839b45a2a7f9fd07f6ae4f85_R_1, _Split_47b65f07839b45a2a7f9fd07f6ae4f85_G_2, _Add_5ff2f4775c81475eaa4e1d2f1038221c_Out_2);
            float _Add_0b5c85958b4a44b4a1471bfb43d09937_Out_2;
            Unity_Add_float(_Add_5ff2f4775c81475eaa4e1d2f1038221c_Out_2, _Split_47b65f07839b45a2a7f9fd07f6ae4f85_G_2, _Add_0b5c85958b4a44b4a1471bfb43d09937_Out_2);
            float _Fraction_e6c8ad87e4c042d9a9a488035ab25129_Out_1;
            Unity_Fraction_float(_Add_0b5c85958b4a44b4a1471bfb43d09937_Out_2, _Fraction_e6c8ad87e4c042d9a9a488035ab25129_Out_1);
            float4 _Lerp_3f9a5249281541e1be1f190753e33b9d_Out_3;
            Unity_Lerp_float4(_Multiply_72a5e6242399449aad550d9798747548_Out_2, _Multiply_c5a7db016b7b4a19a5d1eb518bbd1ebe_Out_2, (_Fraction_e6c8ad87e4c042d9a9a488035ab25129_Out_1.xxxx), _Lerp_3f9a5249281541e1be1f190753e33b9d_Out_3);
            float4 _Property_bf6324ebb86f442ba9760ead28e9f8e6_Out_0 = IsGammaSpace() ? LinearToSRGB(SSSColor) : SSSColor;
            float3 _MainLightCustomFunction_13744cd490944daa89a8ca8ea82bf801_Direction_0;
            float3 _MainLightCustomFunction_13744cd490944daa89a8ca8ea82bf801_Color_2;
            float _MainLightCustomFunction_13744cd490944daa89a8ca8ea82bf801_DistanceAtten_3;
            float _MainLightCustomFunction_13744cd490944daa89a8ca8ea82bf801_ShadowAtten_4;
            MainLight_float(IN.AbsoluteWorldSpacePosition, _MainLightCustomFunction_13744cd490944daa89a8ca8ea82bf801_Direction_0, _MainLightCustomFunction_13744cd490944daa89a8ca8ea82bf801_Color_2, _MainLightCustomFunction_13744cd490944daa89a8ca8ea82bf801_DistanceAtten_3, _MainLightCustomFunction_13744cd490944daa89a8ca8ea82bf801_ShadowAtten_4);
            float3 _Multiply_a5ee958a43d6486c8adafb051e9f9094_Out_2;
            Unity_Multiply_float(_MainLightCustomFunction_13744cd490944daa89a8ca8ea82bf801_Color_2, (_MainLightCustomFunction_13744cd490944daa89a8ca8ea82bf801_ShadowAtten_4.xxx), _Multiply_a5ee958a43d6486c8adafb051e9f9094_Out_2);
            float3 _Normalize_1f2add49bcef4d38aca69e47723ca228_Out_1;
            Unity_Normalize_float3(-1 * mul((float3x3)UNITY_MATRIX_M, transpose(mul(UNITY_MATRIX_I_M, UNITY_MATRIX_I_V)) [2].xyz), _Normalize_1f2add49bcef4d38aca69e47723ca228_Out_1);
            float _DotProduct_0440c3f3fd044a749b3a938d67815af5_Out_2;
            Unity_DotProduct_float3(_MainLightCustomFunction_13744cd490944daa89a8ca8ea82bf801_Direction_0, _Normalize_1f2add49bcef4d38aca69e47723ca228_Out_1, _DotProduct_0440c3f3fd044a749b3a938d67815af5_Out_2);
            float _Saturate_94e73a0bcc7a4d4680346dd6b19a67bd_Out_1;
            Unity_Saturate_float(_DotProduct_0440c3f3fd044a749b3a938d67815af5_Out_2, _Saturate_94e73a0bcc7a4d4680346dd6b19a67bd_Out_1);
            float3 _Multiply_0720425b373e459b88a6cee46d6b07df_Out_2;
            Unity_Multiply_float(_Multiply_a5ee958a43d6486c8adafb051e9f9094_Out_2, (_Saturate_94e73a0bcc7a4d4680346dd6b19a67bd_Out_1.xxx), _Multiply_0720425b373e459b88a6cee46d6b07df_Out_2);
            float3 _Multiply_c699ae0d77424fb590424f54dc343760_Out_2;
            Unity_Multiply_float((_Property_bf6324ebb86f442ba9760ead28e9f8e6_Out_0.xyz), _Multiply_0720425b373e459b88a6cee46d6b07df_Out_2, _Multiply_c699ae0d77424fb590424f54dc343760_Out_2);
            float3 _Multiply_10edc177c328439db60721337094f9b1_Out_2;
            Unity_Multiply_float((_Lerp_3f9a5249281541e1be1f190753e33b9d_Out_3.xyz), _Multiply_c699ae0d77424fb590424f54dc343760_Out_2, _Multiply_10edc177c328439db60721337094f9b1_Out_2);
            UnityTexture2D _Property_a295afe4790e404dab7b599e53bee6fc_Out_0 = UnityBuildTexture2DStructNoScale(ThicknessTex);
            float4 _SampleTexture2D_76eba85b0f1b44d3b740b14c94eb642b_RGBA_0 = SAMPLE_TEXTURE2D(_Property_a295afe4790e404dab7b599e53bee6fc_Out_0.tex, _Property_a295afe4790e404dab7b599e53bee6fc_Out_0.samplerstate, IN.uv0.xy);
            float _SampleTexture2D_76eba85b0f1b44d3b740b14c94eb642b_R_4 = _SampleTexture2D_76eba85b0f1b44d3b740b14c94eb642b_RGBA_0.r;
            float _SampleTexture2D_76eba85b0f1b44d3b740b14c94eb642b_G_5 = _SampleTexture2D_76eba85b0f1b44d3b740b14c94eb642b_RGBA_0.g;
            float _SampleTexture2D_76eba85b0f1b44d3b740b14c94eb642b_B_6 = _SampleTexture2D_76eba85b0f1b44d3b740b14c94eb642b_RGBA_0.b;
            float _SampleTexture2D_76eba85b0f1b44d3b740b14c94eb642b_A_7 = _SampleTexture2D_76eba85b0f1b44d3b740b14c94eb642b_RGBA_0.a;
            float3 _Multiply_b49f1dc358b14c73ba3e39551f96c7f9_Out_2;
            Unity_Multiply_float(_Multiply_10edc177c328439db60721337094f9b1_Out_2, (_SampleTexture2D_76eba85b0f1b44d3b740b14c94eb642b_RGBA_0.xyz), _Multiply_b49f1dc358b14c73ba3e39551f96c7f9_Out_2);
            float _Property_7784b184630844a1bd70b6904d69c970_Out_0 = AlphaClip;
            surface.BaseColor = (_Lerp_3f9a5249281541e1be1f190753e33b9d_Out_3.xyz);
            surface.Emission = _Multiply_b49f1dc358b14c73ba3e39551f96c7f9_Out_2;
            surface.Alpha = _SampleTexture2D_361c1baa7e5e484ab38560dd3341b6d8_A_7;
            surface.AlphaClipThreshold = _Property_7784b184630844a1bd70b6904d69c970_Out_0;
            return surface;
        }

            // --------------------------------------------------
            // Build Graph Inputs

            VertexDescriptionInputs BuildVertexDescriptionInputs(Attributes input)
        {
            VertexDescriptionInputs output;
            ZERO_INITIALIZE(VertexDescriptionInputs, output);

            output.ObjectSpaceNormal =           input.normalOS;
            output.WorldSpaceNormal =            TransformObjectToWorldNormal(input.normalOS);
            output.ObjectSpaceTangent =          input.tangentOS.xyz;
            output.WorldSpaceTangent =           TransformObjectToWorldDir(input.tangentOS.xyz);
            output.ObjectSpaceBiTangent =        normalize(cross(input.normalOS, input.tangentOS) * (input.tangentOS.w > 0.0f ? 1.0f : -1.0f) * GetOddNegativeScale());
            output.WorldSpaceBiTangent =         TransformObjectToWorldDir(output.ObjectSpaceBiTangent);
            output.ObjectSpacePosition =         input.positionOS;
            output.AbsoluteWorldSpacePosition =  GetAbsolutePositionWS(TransformObjectToWorld(input.positionOS));
            output.TimeParameters =              _TimeParameters.xyz;

            return output;
        }
            SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
        {
            SurfaceDescriptionInputs output;
            ZERO_INITIALIZE(SurfaceDescriptionInputs, output);





            output.AbsoluteWorldSpacePosition =  GetAbsolutePositionWS(input.positionWS);
            output.uv0 =                         input.texCoord0;
        #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
        #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN output.FaceSign =                    IS_FRONT_VFACE(input.cullFace, true, false);
        #else
        #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
        #endif
        #undef BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN

            return output;
        }

            // --------------------------------------------------
            // Main

            #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/Varyings.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/LightingMetaPass.hlsl"

            ENDHLSL
        }
        Pass
        {
            // Name: <None>
            Tags
            {
                "LightMode" = "Universal2D"
            }

            // Render State
            Cull Off
        Blend One Zero
        ZTest LEqual
        ZWrite On

            // Debug
            // <None>

            // --------------------------------------------------
            // Pass

            HLSLPROGRAM

            // Pragmas
            #pragma target 4.5
        #pragma exclude_renderers gles gles3 glcore
        #pragma vertex vert
        #pragma fragment frag

            // DotsInstancingOptions: <None>
            // HybridV1InjectedBuiltinProperties: <None>

            // Keywords
            // PassKeywords: <None>
            // GraphKeywords: <None>

            // Defines
            #define _AlphaClip 1
            #define _NORMALMAP 1
            #define _NORMAL_DROPOFF_TS 1
            #define ATTRIBUTES_NEED_NORMAL
            #define ATTRIBUTES_NEED_TANGENT
            #define ATTRIBUTES_NEED_TEXCOORD0
            #define VARYINGS_NEED_TEXCOORD0
            #define FEATURES_GRAPH_VERTEX
            /* WARNING: $splice Could not find named fragment 'PassInstancing' */
            #define SHADERPASS SHADERPASS_2D
            /* WARNING: $splice Could not find named fragment 'DotsInstancingVars' */

            // Includes
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
 //#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
                // Graph Properties
            CBUFFER_START(UnityPerMaterial)
        float4 MainTex_TexelSize;
        float4 MainColor;
        float4 VarColor;
        float4 ThicknessTex_TexelSize;
        float4 SSSColor;
        float4 NormalTex_TexelSize;
        float Normal_Intensity;
        float4 MaskTex_TexelSize;
        float Smoothness;
        float AlphaClip;
        float Wind_Speed;
        float Wind_Frequency;
        float Wind_Strength;
        float3 Wind_Direction;
        float _Specular;
        float _EnvReflection;
        CBUFFER_END

        #include ".\Assets\KSC_Assets\03_Shaders\Common\Lighting_KSC.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"

            // --------------------------------------------------
            // Structs and Packing

            struct Attributes
        {
            float3 positionOS : POSITION;
            float3 normalOS : NORMAL;
            float4 tangentOS : TANGENT;
            float4 uv0 : TEXCOORD0;
            #if UNITY_ANY_INSTANCING_ENABLED
            uint instanceID : INSTANCEID_SEMANTIC;
            #endif
        };
        struct Varyings
        {
            float4 positionCS : SV_POSITION;
            float4 texCoord0;
            #if UNITY_ANY_INSTANCING_ENABLED
            uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
            #endif
        };
        struct SurfaceDescriptionInputs
        {
            float4 uv0;
        };
        struct VertexDescriptionInputs
        {
            float3 ObjectSpaceNormal;
            float3 WorldSpaceNormal;
            float3 ObjectSpaceTangent;
            float3 WorldSpaceTangent;
            float3 ObjectSpaceBiTangent;
            float3 WorldSpaceBiTangent;
            float3 ObjectSpacePosition;
            float3 AbsoluteWorldSpacePosition;
            float3 TimeParameters;
        };
        struct PackedVaryings
        {
            float4 positionCS : SV_POSITION;
            float4 interp0 : TEXCOORD0;
            #if UNITY_ANY_INSTANCING_ENABLED
            uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
            #endif
        };

            PackedVaryings PackVaryings (Varyings input)
        {
            PackedVaryings output;
            output.positionCS = input.positionCS;
            output.interp0.xyzw =  input.texCoord0;
            #if UNITY_ANY_INSTANCING_ENABLED
            output.instanceID = input.instanceID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            output.cullFace = input.cullFace;
            #endif
            return output;
        }
        Varyings UnpackVaryings (PackedVaryings input)
        {
            Varyings output;
            output.positionCS = input.positionCS;
            output.texCoord0 = input.interp0.xyzw;
            #if UNITY_ANY_INSTANCING_ENABLED
            output.instanceID = input.instanceID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            output.cullFace = input.cullFace;
            #endif
            return output;
        }

            // --------------------------------------------------
            // Graph

            // Graph Properties
           /* CBUFFER_START(UnityPerMaterial)
        float4 MainTex_TexelSize;
        float4 MainColor;
        float4 VarColor;
        float4 ThicknessTex_TexelSize;
        float4 SSSColor;
        float4 NormalTex_TexelSize;
        float Normal_Intensity;
        float4 MaskTex_TexelSize;
        float Smoothness;
        float AlphaClip;
        float Wind_Speed;
        float Wind_Frequency;
        float Wind_Strength;
        float3 Wind_Direction;
        float _Specular;
        float _EnvReflection;
        CBUFFER_END*/
        // Object and Global properties
        SAMPLER(SamplerState_Linear_Repeat);
        TEXTURE2D(MainTex);
        SAMPLER(samplerMainTex);
        TEXTURE2D(ThicknessTex);
        SAMPLER(samplerThicknessTex);
        TEXTURE2D(NormalTex);
        SAMPLER(samplerNormalTex);
        TEXTURE2D(MaskTex);
        SAMPLER(samplerMaskTex);

            // Graph Functions
            
        void Unity_Distance_float3(float3 A, float3 B, out float Out)
        {
            Out = distance(A, B);
        }

        void Unity_Normalize_float3(float3 In, out float3 Out)
        {
            Out = normalize(In);
        }

        void Unity_Multiply_float(float A, float B, out float Out)
        {
            Out = A * B;
        }

        void Unity_Add_float(float A, float B, out float Out)
        {
            Out = A + B;
        }

        void Unity_Sine_float(float In, out float Out)
        {
            Out = sin(In);
        }

        void Unity_Multiply_float(float3 A, float3 B, out float3 Out)
        {
            Out = A * B;
        }

        void Unity_Add_float3(float3 A, float3 B, out float3 Out)
        {
            Out = A + B;
        }

        void Unity_Multiply_float(float4 A, float4 B, out float4 Out)
        {
            Out = A * B;
        }

        void Unity_Fraction_float(float In, out float Out)
        {
            Out = frac(In);
        }

        void Unity_Lerp_float4(float4 A, float4 B, float4 T, out float4 Out)
        {
            Out = lerp(A, B, T);
        }

            // Graph Vertex
            struct VertexDescription
        {
            float3 Position;
            float3 Normal;
            float3 Tangent;
        };

        VertexDescription VertexDescriptionFunction(VertexDescriptionInputs IN)
        {
            VertexDescription description = (VertexDescription)0;
            float _Distance_a9453349007d43d7aa8e8ef2f9e0a6cb_Out_2;
            Unity_Distance_float3(float3(0, 0, 0), IN.ObjectSpacePosition, _Distance_a9453349007d43d7aa8e8ef2f9e0a6cb_Out_2);
            float3 _Property_294994c591114f76808f84d55e0400b9_Out_0 = Wind_Direction;
            float3 _Normalize_7a91dcb4e52548df84db8cb487a5210f_Out_1;
            Unity_Normalize_float3(_Property_294994c591114f76808f84d55e0400b9_Out_0, _Normalize_7a91dcb4e52548df84db8cb487a5210f_Out_1);
            float _Property_b1049a414de54aaf954fc9acaf3ffde3_Out_0 = Wind_Speed;
            float _Multiply_8cc8be463d9b4875839e9776be38d510_Out_2;
            Unity_Multiply_float(IN.TimeParameters.x, _Property_b1049a414de54aaf954fc9acaf3ffde3_Out_0, _Multiply_8cc8be463d9b4875839e9776be38d510_Out_2);
            float _Property_1cc960ad477c47dfa058e2368a6a0e59_Out_0 = Wind_Frequency;
            float _Split_47b65f07839b45a2a7f9fd07f6ae4f85_R_1 = SHADERGRAPH_OBJECT_POSITION[0];
            float _Split_47b65f07839b45a2a7f9fd07f6ae4f85_G_2 = SHADERGRAPH_OBJECT_POSITION[1];
            float _Split_47b65f07839b45a2a7f9fd07f6ae4f85_B_3 = SHADERGRAPH_OBJECT_POSITION[2];
            float _Split_47b65f07839b45a2a7f9fd07f6ae4f85_A_4 = 0;
            float _Add_5ff2f4775c81475eaa4e1d2f1038221c_Out_2;
            Unity_Add_float(_Split_47b65f07839b45a2a7f9fd07f6ae4f85_R_1, _Split_47b65f07839b45a2a7f9fd07f6ae4f85_G_2, _Add_5ff2f4775c81475eaa4e1d2f1038221c_Out_2);
            float _Add_0b5c85958b4a44b4a1471bfb43d09937_Out_2;
            Unity_Add_float(_Add_5ff2f4775c81475eaa4e1d2f1038221c_Out_2, _Split_47b65f07839b45a2a7f9fd07f6ae4f85_G_2, _Add_0b5c85958b4a44b4a1471bfb43d09937_Out_2);
            float _Multiply_36b4b25245ef46a6aa344b72d18b123a_Out_2;
            Unity_Multiply_float(_Property_1cc960ad477c47dfa058e2368a6a0e59_Out_0, _Add_0b5c85958b4a44b4a1471bfb43d09937_Out_2, _Multiply_36b4b25245ef46a6aa344b72d18b123a_Out_2);
            float _Add_112d774bc523412da770f4e752e0aeae_Out_2;
            Unity_Add_float(_Multiply_8cc8be463d9b4875839e9776be38d510_Out_2, _Multiply_36b4b25245ef46a6aa344b72d18b123a_Out_2, _Add_112d774bc523412da770f4e752e0aeae_Out_2);
            float _Sine_703274f04df4470980c37a4a595551c8_Out_1;
            Unity_Sine_float(_Add_112d774bc523412da770f4e752e0aeae_Out_2, _Sine_703274f04df4470980c37a4a595551c8_Out_1);
            float _Multiply_b11879df31064a879f94ea423ab2b2a5_Out_2;
            Unity_Multiply_float(_Sine_703274f04df4470980c37a4a595551c8_Out_1, 0.5, _Multiply_b11879df31064a879f94ea423ab2b2a5_Out_2);
            float _Add_efce9d790e58416f86a8ec3243204620_Out_2;
            Unity_Add_float(_Multiply_b11879df31064a879f94ea423ab2b2a5_Out_2, 0.5, _Add_efce9d790e58416f86a8ec3243204620_Out_2);
            float _Property_79b44419d559467d899224e854ea3824_Out_0 = Wind_Strength;
            float _Multiply_47956fa9e0804e8381e9c610915da0e6_Out_2;
            Unity_Multiply_float(_Add_efce9d790e58416f86a8ec3243204620_Out_2, _Property_79b44419d559467d899224e854ea3824_Out_0, _Multiply_47956fa9e0804e8381e9c610915da0e6_Out_2);
            float3 _Multiply_448ee0f8bc4f41a8bcf38fdc2b440495_Out_2;
            Unity_Multiply_float(_Normalize_7a91dcb4e52548df84db8cb487a5210f_Out_1, (_Multiply_47956fa9e0804e8381e9c610915da0e6_Out_2.xxx), _Multiply_448ee0f8bc4f41a8bcf38fdc2b440495_Out_2);
            float3 _Multiply_4e49ed416c034b90b06a8a56b2d0a49a_Out_2;
            Unity_Multiply_float((_Distance_a9453349007d43d7aa8e8ef2f9e0a6cb_Out_2.xxx), _Multiply_448ee0f8bc4f41a8bcf38fdc2b440495_Out_2, _Multiply_4e49ed416c034b90b06a8a56b2d0a49a_Out_2);
            float3 _Add_c136859739784517b3f510aa5bb4e469_Out_2;
            Unity_Add_float3(_Multiply_4e49ed416c034b90b06a8a56b2d0a49a_Out_2, IN.AbsoluteWorldSpacePosition, _Add_c136859739784517b3f510aa5bb4e469_Out_2);
            float3 _Transform_47fd3d4cb64f4d52b2077aaa7435e77b_Out_1 = TransformWorldToObject(_Add_c136859739784517b3f510aa5bb4e469_Out_2.xyz);
            description.Position = _Transform_47fd3d4cb64f4d52b2077aaa7435e77b_Out_1;
            description.Normal = IN.ObjectSpaceNormal;
            description.Tangent = IN.ObjectSpaceTangent;
            return description;
        }

            // Graph Pixel
            struct SurfaceDescription
        {
            float3 BaseColor;
            float Alpha;
            float AlphaClipThreshold;
        };

        SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
        {
            SurfaceDescription surface = (SurfaceDescription)0;
            float4 _Property_1931a4995a954dc59a2a81efc2ac7bc3_Out_0 = MainColor;
            UnityTexture2D _Property_aed0c1bacf314879a280662567e38a31_Out_0 = UnityBuildTexture2DStructNoScale(MainTex);
            float4 _SampleTexture2D_361c1baa7e5e484ab38560dd3341b6d8_RGBA_0 = SAMPLE_TEXTURE2D(_Property_aed0c1bacf314879a280662567e38a31_Out_0.tex, _Property_aed0c1bacf314879a280662567e38a31_Out_0.samplerstate, IN.uv0.xy);
            float _SampleTexture2D_361c1baa7e5e484ab38560dd3341b6d8_R_4 = _SampleTexture2D_361c1baa7e5e484ab38560dd3341b6d8_RGBA_0.r;
            float _SampleTexture2D_361c1baa7e5e484ab38560dd3341b6d8_G_5 = _SampleTexture2D_361c1baa7e5e484ab38560dd3341b6d8_RGBA_0.g;
            float _SampleTexture2D_361c1baa7e5e484ab38560dd3341b6d8_B_6 = _SampleTexture2D_361c1baa7e5e484ab38560dd3341b6d8_RGBA_0.b;
            float _SampleTexture2D_361c1baa7e5e484ab38560dd3341b6d8_A_7 = _SampleTexture2D_361c1baa7e5e484ab38560dd3341b6d8_RGBA_0.a;
            float4 _Multiply_72a5e6242399449aad550d9798747548_Out_2;
            Unity_Multiply_float(_Property_1931a4995a954dc59a2a81efc2ac7bc3_Out_0, _SampleTexture2D_361c1baa7e5e484ab38560dd3341b6d8_RGBA_0, _Multiply_72a5e6242399449aad550d9798747548_Out_2);
            float4 _Property_2e6c583a57424a8180974b05f7e49119_Out_0 = VarColor;
            float4 _Multiply_c5a7db016b7b4a19a5d1eb518bbd1ebe_Out_2;
            Unity_Multiply_float(_Property_2e6c583a57424a8180974b05f7e49119_Out_0, _SampleTexture2D_361c1baa7e5e484ab38560dd3341b6d8_RGBA_0, _Multiply_c5a7db016b7b4a19a5d1eb518bbd1ebe_Out_2);
            float _Split_47b65f07839b45a2a7f9fd07f6ae4f85_R_1 = SHADERGRAPH_OBJECT_POSITION[0];
            float _Split_47b65f07839b45a2a7f9fd07f6ae4f85_G_2 = SHADERGRAPH_OBJECT_POSITION[1];
            float _Split_47b65f07839b45a2a7f9fd07f6ae4f85_B_3 = SHADERGRAPH_OBJECT_POSITION[2];
            float _Split_47b65f07839b45a2a7f9fd07f6ae4f85_A_4 = 0;
            float _Add_5ff2f4775c81475eaa4e1d2f1038221c_Out_2;
            Unity_Add_float(_Split_47b65f07839b45a2a7f9fd07f6ae4f85_R_1, _Split_47b65f07839b45a2a7f9fd07f6ae4f85_G_2, _Add_5ff2f4775c81475eaa4e1d2f1038221c_Out_2);
            float _Add_0b5c85958b4a44b4a1471bfb43d09937_Out_2;
            Unity_Add_float(_Add_5ff2f4775c81475eaa4e1d2f1038221c_Out_2, _Split_47b65f07839b45a2a7f9fd07f6ae4f85_G_2, _Add_0b5c85958b4a44b4a1471bfb43d09937_Out_2);
            float _Fraction_e6c8ad87e4c042d9a9a488035ab25129_Out_1;
            Unity_Fraction_float(_Add_0b5c85958b4a44b4a1471bfb43d09937_Out_2, _Fraction_e6c8ad87e4c042d9a9a488035ab25129_Out_1);
            float4 _Lerp_3f9a5249281541e1be1f190753e33b9d_Out_3;
            Unity_Lerp_float4(_Multiply_72a5e6242399449aad550d9798747548_Out_2, _Multiply_c5a7db016b7b4a19a5d1eb518bbd1ebe_Out_2, (_Fraction_e6c8ad87e4c042d9a9a488035ab25129_Out_1.xxxx), _Lerp_3f9a5249281541e1be1f190753e33b9d_Out_3);
            float _Property_7784b184630844a1bd70b6904d69c970_Out_0 = AlphaClip;
            surface.BaseColor = (_Lerp_3f9a5249281541e1be1f190753e33b9d_Out_3.xyz);
            surface.Alpha = _SampleTexture2D_361c1baa7e5e484ab38560dd3341b6d8_A_7;
            surface.AlphaClipThreshold = _Property_7784b184630844a1bd70b6904d69c970_Out_0;
            return surface;
        }

            // --------------------------------------------------
            // Build Graph Inputs

            VertexDescriptionInputs BuildVertexDescriptionInputs(Attributes input)
        {
            VertexDescriptionInputs output;
            ZERO_INITIALIZE(VertexDescriptionInputs, output);

            output.ObjectSpaceNormal =           input.normalOS;
            output.WorldSpaceNormal =            TransformObjectToWorldNormal(input.normalOS);
            output.ObjectSpaceTangent =          input.tangentOS.xyz;
            output.WorldSpaceTangent =           TransformObjectToWorldDir(input.tangentOS.xyz);
            output.ObjectSpaceBiTangent =        normalize(cross(input.normalOS, input.tangentOS) * (input.tangentOS.w > 0.0f ? 1.0f : -1.0f) * GetOddNegativeScale());
            output.WorldSpaceBiTangent =         TransformObjectToWorldDir(output.ObjectSpaceBiTangent);
            output.ObjectSpacePosition =         input.positionOS;
            output.AbsoluteWorldSpacePosition =  GetAbsolutePositionWS(TransformObjectToWorld(input.positionOS));
            output.TimeParameters =              _TimeParameters.xyz;

            return output;
        }
            SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
        {
            SurfaceDescriptionInputs output;
            ZERO_INITIALIZE(SurfaceDescriptionInputs, output);





            output.uv0 =                         input.texCoord0;
        #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
        #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN output.FaceSign =                    IS_FRONT_VFACE(input.cullFace, true, false);
        #else
        #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
        #endif
        #undef BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN

            return output;
        }

            // --------------------------------------------------
            // Main

            #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/Varyings.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/PBR2DPass.hlsl"

            ENDHLSL
        }
    }
    SubShader
    {
        Tags
        {
            "RenderPipeline"="UniversalPipeline"
            "RenderType"="Opaque"
            "UniversalMaterialType" = "Lit"
            "Queue"="AlphaTest"
        }
        Pass
        {
            Name "Universal Forward"
            Tags
            {
                "LightMode" = "UniversalForward"
            }

            // Render State
            Cull Off
        Blend One Zero
        ZTest LEqual
        ZWrite On

            // Debug
            // <None>

            // --------------------------------------------------
            // Pass

            HLSLPROGRAM

            // Pragmas
            #pragma target 2.0
        #pragma only_renderers gles gles3 glcore d3d11
        #pragma multi_compile_instancing
        #pragma multi_compile_fog
        #pragma vertex vert
        #pragma fragment frag

            // DotsInstancingOptions: <None>
            // HybridV1InjectedBuiltinProperties: <None>

            // Keywords
            #pragma multi_compile _ _SCREEN_SPACE_OCCLUSION
        #pragma multi_compile _ LIGHTMAP_ON
        #pragma multi_compile _ DIRLIGHTMAP_COMBINED
        #pragma multi_compile _ _MAIN_LIGHT_SHADOWS _MAIN_LIGHT_SHADOWS_CASCADE _MAIN_LIGHT_SHADOWS_SCREEN
        #pragma multi_compile _ADDITIONAL_LIGHTS_VERTEX _ADDITIONAL_LIGHTS _ADDITIONAL_OFF
        #pragma multi_compile _ _ADDITIONAL_LIGHT_SHADOWS
        #pragma multi_compile _ _SHADOWS_SOFT
        #pragma multi_compile _ LIGHTMAP_SHADOW_MIXING
        #pragma multi_compile _ SHADOWS_SHADOWMASK
            // GraphKeywords: <None>

            // Defines
            #define _AlphaClip 1
            #define _NORMALMAP 1
            #define _NORMAL_DROPOFF_TS 1
            #define ATTRIBUTES_NEED_NORMAL
            #define ATTRIBUTES_NEED_TANGENT
            #define ATTRIBUTES_NEED_TEXCOORD0
            #define ATTRIBUTES_NEED_TEXCOORD1
            #define VARYINGS_NEED_POSITION_WS
            #define VARYINGS_NEED_NORMAL_WS
            #define VARYINGS_NEED_TANGENT_WS
            #define VARYINGS_NEED_TEXCOORD0
            #define VARYINGS_NEED_VIEWDIRECTION_WS
            #define VARYINGS_NEED_FOG_AND_VERTEX_LIGHT
            #define FEATURES_GRAPH_VERTEX
            /* WARNING: $splice Could not find named fragment 'PassInstancing' */
            #define SHADERPASS SHADERPASS_FORWARD
            /* WARNING: $splice Could not find named fragment 'DotsInstancingVars' */

            // Includes
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
         //#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
                // Graph Properties
            CBUFFER_START(UnityPerMaterial)
        float4 MainTex_TexelSize;
        float4 MainColor;
        float4 VarColor;
        float4 ThicknessTex_TexelSize;
        float4 SSSColor;
        float4 NormalTex_TexelSize;
        float Normal_Intensity;
        float4 MaskTex_TexelSize;
        float Smoothness;
        float AlphaClip;
        float Wind_Speed;
        float Wind_Frequency;
        float Wind_Strength;
        float3 Wind_Direction;
        float _Specular;
        float _EnvReflection;
        CBUFFER_END

        #include ".\Assets\KSC_Assets\03_Shaders\Common\Lighting_KSC.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Shadows.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"

            // --------------------------------------------------
            // Structs and Packing

            struct Attributes
        {
            float3 positionOS : POSITION;
            float3 normalOS : NORMAL;
            float4 tangentOS : TANGENT;
            float4 uv0 : TEXCOORD0;
            float4 uv1 : TEXCOORD1;
            #if UNITY_ANY_INSTANCING_ENABLED
            uint instanceID : INSTANCEID_SEMANTIC;
            #endif
        };
        struct Varyings
        {
            float4 positionCS : SV_POSITION;
            float3 positionWS;
            float3 normalWS;
            float4 tangentWS;
            float4 texCoord0;
            float3 viewDirectionWS;
            #if defined(LIGHTMAP_ON)
            float2 lightmapUV;
            #endif
            #if !defined(LIGHTMAP_ON)
            float3 sh;
            #endif
            float4 fogFactorAndVertexLight;
            float4 shadowCoord;
            #if UNITY_ANY_INSTANCING_ENABLED
            uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
            #endif
        };
        struct SurfaceDescriptionInputs
        {
            float3 TangentSpaceNormal;
            float3 AbsoluteWorldSpacePosition;
            float4 uv0;
        };
        struct VertexDescriptionInputs
        {
            float3 ObjectSpaceNormal;
            float3 WorldSpaceNormal;
            float3 ObjectSpaceTangent;
            float3 WorldSpaceTangent;
            float3 ObjectSpaceBiTangent;
            float3 WorldSpaceBiTangent;
            float3 ObjectSpacePosition;
            float3 AbsoluteWorldSpacePosition;
            float3 TimeParameters;
        };
        struct PackedVaryings
        {
            float4 positionCS : SV_POSITION;
            float3 interp0 : TEXCOORD0;
            float3 interp1 : TEXCOORD1;
            float4 interp2 : TEXCOORD2;
            float4 interp3 : TEXCOORD3;
            float3 interp4 : TEXCOORD4;
            #if defined(LIGHTMAP_ON)
            float2 interp5 : TEXCOORD5;
            #endif
            #if !defined(LIGHTMAP_ON)
            float3 interp6 : TEXCOORD6;
            #endif
            float4 interp7 : TEXCOORD7;
            float4 interp8 : TEXCOORD8;
            #if UNITY_ANY_INSTANCING_ENABLED
            uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
            #endif
        };

            PackedVaryings PackVaryings (Varyings input)
        {
            PackedVaryings output;
            output.positionCS = input.positionCS;
            output.interp0.xyz =  input.positionWS;
            output.interp1.xyz =  input.normalWS;
            output.interp2.xyzw =  input.tangentWS;
            output.interp3.xyzw =  input.texCoord0;
            output.interp4.xyz =  input.viewDirectionWS;
            #if defined(LIGHTMAP_ON)
            output.interp5.xy =  input.lightmapUV;
            #endif
            #if !defined(LIGHTMAP_ON)
            output.interp6.xyz =  input.sh;
            #endif
            output.interp7.xyzw =  input.fogFactorAndVertexLight;
            output.interp8.xyzw =  input.shadowCoord;
            #if UNITY_ANY_INSTANCING_ENABLED
            output.instanceID = input.instanceID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            output.cullFace = input.cullFace;
            #endif
            return output;
        }
        Varyings UnpackVaryings (PackedVaryings input)
        {
            Varyings output;
            output.positionCS = input.positionCS;
            output.positionWS = input.interp0.xyz;
            output.normalWS = input.interp1.xyz;
            output.tangentWS = input.interp2.xyzw;
            output.texCoord0 = input.interp3.xyzw;
            output.viewDirectionWS = input.interp4.xyz;
            #if defined(LIGHTMAP_ON)
            output.lightmapUV = input.interp5.xy;
            #endif
            #if !defined(LIGHTMAP_ON)
            output.sh = input.interp6.xyz;
            #endif
            output.fogFactorAndVertexLight = input.interp7.xyzw;
            output.shadowCoord = input.interp8.xyzw;
            #if UNITY_ANY_INSTANCING_ENABLED
            output.instanceID = input.instanceID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            output.cullFace = input.cullFace;
            #endif
            return output;
        }

            // --------------------------------------------------
            // Graph

            // Graph Properties
             /* CBUFFER_START(UnityPerMaterial)
        float4 MainTex_TexelSize;
        float4 MainColor;
        float4 VarColor;
        float4 ThicknessTex_TexelSize;
        float4 SSSColor;
        float4 NormalTex_TexelSize;
        float Normal_Intensity;
        float4 MaskTex_TexelSize;
        float Smoothness;
        float AlphaClip;
        float Wind_Speed;
        float Wind_Frequency;
        float Wind_Strength;
        float3 Wind_Direction;
        float _Specular;
        float _EnvReflection;
        CBUFFER_END*/

        // Object and Global properties
        SAMPLER(SamplerState_Linear_Repeat);
        TEXTURE2D(MainTex);
        SAMPLER(samplerMainTex);
        TEXTURE2D(ThicknessTex);
        SAMPLER(samplerThicknessTex);
        TEXTURE2D(NormalTex);
        SAMPLER(samplerNormalTex);
        TEXTURE2D(MaskTex);
        SAMPLER(samplerMaskTex);

            // Graph Functions
            
        void Unity_Distance_float3(float3 A, float3 B, out float Out)
        {
            Out = distance(A, B);
        }

        void Unity_Normalize_float3(float3 In, out float3 Out)
        {
            Out = normalize(In);
        }

        void Unity_Multiply_float(float A, float B, out float Out)
        {
            Out = A * B;
        }

        void Unity_Add_float(float A, float B, out float Out)
        {
            Out = A + B;
        }

        void Unity_Sine_float(float In, out float Out)
        {
            Out = sin(In);
        }

        void Unity_Multiply_float(float3 A, float3 B, out float3 Out)
        {
            Out = A * B;
        }

        void Unity_Add_float3(float3 A, float3 B, out float3 Out)
        {
            Out = A + B;
        }

        void Unity_Multiply_float(float4 A, float4 B, out float4 Out)
        {
            Out = A * B;
        }

        void Unity_Fraction_float(float In, out float Out)
        {
            Out = frac(In);
        }

        void Unity_Lerp_float4(float4 A, float4 B, float4 T, out float4 Out)
        {
            Out = lerp(A, B, T);
        }

        void Unity_NormalStrength_float(float3 In, float Strength, out float3 Out)
        {
            Out = float3(In.rg * Strength, lerp(1, In.b, saturate(Strength)));
        }

        // 42362d8732025af29918c5cc8ad4975c
        #include "Assets/KSC_Assets/03_Shaders/Common/Lighting_Function.hlsl"

        void Unity_DotProduct_float3(float3 A, float3 B, out float Out)
        {
            Out = dot(A, B);
        }

        void Unity_Saturate_float(float In, out float Out)
        {
            Out = saturate(In);
        }

            // Graph Vertex
            struct VertexDescription
        {
            float3 Position;
            float3 Normal;
            float3 Tangent;
        };

        VertexDescription VertexDescriptionFunction(VertexDescriptionInputs IN)
        {
            VertexDescription description = (VertexDescription)0;
            float _Distance_a9453349007d43d7aa8e8ef2f9e0a6cb_Out_2;
            Unity_Distance_float3(float3(0, 0, 0), IN.ObjectSpacePosition, _Distance_a9453349007d43d7aa8e8ef2f9e0a6cb_Out_2);
            float3 _Property_294994c591114f76808f84d55e0400b9_Out_0 = Wind_Direction;
            float3 _Normalize_7a91dcb4e52548df84db8cb487a5210f_Out_1;
            Unity_Normalize_float3(_Property_294994c591114f76808f84d55e0400b9_Out_0, _Normalize_7a91dcb4e52548df84db8cb487a5210f_Out_1);
            float _Property_b1049a414de54aaf954fc9acaf3ffde3_Out_0 = Wind_Speed;
            float _Multiply_8cc8be463d9b4875839e9776be38d510_Out_2;
            Unity_Multiply_float(IN.TimeParameters.x, _Property_b1049a414de54aaf954fc9acaf3ffde3_Out_0, _Multiply_8cc8be463d9b4875839e9776be38d510_Out_2);
            float _Property_1cc960ad477c47dfa058e2368a6a0e59_Out_0 = Wind_Frequency;
            float _Split_47b65f07839b45a2a7f9fd07f6ae4f85_R_1 = SHADERGRAPH_OBJECT_POSITION[0];
            float _Split_47b65f07839b45a2a7f9fd07f6ae4f85_G_2 = SHADERGRAPH_OBJECT_POSITION[1];
            float _Split_47b65f07839b45a2a7f9fd07f6ae4f85_B_3 = SHADERGRAPH_OBJECT_POSITION[2];
            float _Split_47b65f07839b45a2a7f9fd07f6ae4f85_A_4 = 0;
            float _Add_5ff2f4775c81475eaa4e1d2f1038221c_Out_2;
            Unity_Add_float(_Split_47b65f07839b45a2a7f9fd07f6ae4f85_R_1, _Split_47b65f07839b45a2a7f9fd07f6ae4f85_G_2, _Add_5ff2f4775c81475eaa4e1d2f1038221c_Out_2);
            float _Add_0b5c85958b4a44b4a1471bfb43d09937_Out_2;
            Unity_Add_float(_Add_5ff2f4775c81475eaa4e1d2f1038221c_Out_2, _Split_47b65f07839b45a2a7f9fd07f6ae4f85_G_2, _Add_0b5c85958b4a44b4a1471bfb43d09937_Out_2);
            float _Multiply_36b4b25245ef46a6aa344b72d18b123a_Out_2;
            Unity_Multiply_float(_Property_1cc960ad477c47dfa058e2368a6a0e59_Out_0, _Add_0b5c85958b4a44b4a1471bfb43d09937_Out_2, _Multiply_36b4b25245ef46a6aa344b72d18b123a_Out_2);
            float _Add_112d774bc523412da770f4e752e0aeae_Out_2;
            Unity_Add_float(_Multiply_8cc8be463d9b4875839e9776be38d510_Out_2, _Multiply_36b4b25245ef46a6aa344b72d18b123a_Out_2, _Add_112d774bc523412da770f4e752e0aeae_Out_2);
            float _Sine_703274f04df4470980c37a4a595551c8_Out_1;
            Unity_Sine_float(_Add_112d774bc523412da770f4e752e0aeae_Out_2, _Sine_703274f04df4470980c37a4a595551c8_Out_1);
            float _Multiply_b11879df31064a879f94ea423ab2b2a5_Out_2;
            Unity_Multiply_float(_Sine_703274f04df4470980c37a4a595551c8_Out_1, 0.5, _Multiply_b11879df31064a879f94ea423ab2b2a5_Out_2);
            float _Add_efce9d790e58416f86a8ec3243204620_Out_2;
            Unity_Add_float(_Multiply_b11879df31064a879f94ea423ab2b2a5_Out_2, 0.5, _Add_efce9d790e58416f86a8ec3243204620_Out_2);
            float _Property_79b44419d559467d899224e854ea3824_Out_0 = Wind_Strength;
            float _Multiply_47956fa9e0804e8381e9c610915da0e6_Out_2;
            Unity_Multiply_float(_Add_efce9d790e58416f86a8ec3243204620_Out_2, _Property_79b44419d559467d899224e854ea3824_Out_0, _Multiply_47956fa9e0804e8381e9c610915da0e6_Out_2);
            float3 _Multiply_448ee0f8bc4f41a8bcf38fdc2b440495_Out_2;
            Unity_Multiply_float(_Normalize_7a91dcb4e52548df84db8cb487a5210f_Out_1, (_Multiply_47956fa9e0804e8381e9c610915da0e6_Out_2.xxx), _Multiply_448ee0f8bc4f41a8bcf38fdc2b440495_Out_2);
            float3 _Multiply_4e49ed416c034b90b06a8a56b2d0a49a_Out_2;
            Unity_Multiply_float((_Distance_a9453349007d43d7aa8e8ef2f9e0a6cb_Out_2.xxx), _Multiply_448ee0f8bc4f41a8bcf38fdc2b440495_Out_2, _Multiply_4e49ed416c034b90b06a8a56b2d0a49a_Out_2);
            float3 _Add_c136859739784517b3f510aa5bb4e469_Out_2;
            Unity_Add_float3(_Multiply_4e49ed416c034b90b06a8a56b2d0a49a_Out_2, IN.AbsoluteWorldSpacePosition, _Add_c136859739784517b3f510aa5bb4e469_Out_2);
            float3 _Transform_47fd3d4cb64f4d52b2077aaa7435e77b_Out_1 = TransformWorldToObject(_Add_c136859739784517b3f510aa5bb4e469_Out_2.xyz);
            description.Position = _Transform_47fd3d4cb64f4d52b2077aaa7435e77b_Out_1;
            description.Normal = IN.ObjectSpaceNormal;
            description.Tangent = IN.ObjectSpaceTangent;
            return description;
        }

            // Graph Pixel
            struct SurfaceDescription
        {
            float3 BaseColor;
            float3 NormalTS;
            float3 Emission;
            float Metallic;
            float Smoothness;
            float Occlusion;
            float Alpha;
            float AlphaClipThreshold;
        };

        SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
        {
            SurfaceDescription surface = (SurfaceDescription)0;
            float4 _Property_1931a4995a954dc59a2a81efc2ac7bc3_Out_0 = MainColor;
            UnityTexture2D _Property_aed0c1bacf314879a280662567e38a31_Out_0 = UnityBuildTexture2DStructNoScale(MainTex);
            float4 _SampleTexture2D_361c1baa7e5e484ab38560dd3341b6d8_RGBA_0 = SAMPLE_TEXTURE2D(_Property_aed0c1bacf314879a280662567e38a31_Out_0.tex, _Property_aed0c1bacf314879a280662567e38a31_Out_0.samplerstate, IN.uv0.xy);
            float _SampleTexture2D_361c1baa7e5e484ab38560dd3341b6d8_R_4 = _SampleTexture2D_361c1baa7e5e484ab38560dd3341b6d8_RGBA_0.r;
            float _SampleTexture2D_361c1baa7e5e484ab38560dd3341b6d8_G_5 = _SampleTexture2D_361c1baa7e5e484ab38560dd3341b6d8_RGBA_0.g;
            float _SampleTexture2D_361c1baa7e5e484ab38560dd3341b6d8_B_6 = _SampleTexture2D_361c1baa7e5e484ab38560dd3341b6d8_RGBA_0.b;
            float _SampleTexture2D_361c1baa7e5e484ab38560dd3341b6d8_A_7 = _SampleTexture2D_361c1baa7e5e484ab38560dd3341b6d8_RGBA_0.a;
            float4 _Multiply_72a5e6242399449aad550d9798747548_Out_2;
            Unity_Multiply_float(_Property_1931a4995a954dc59a2a81efc2ac7bc3_Out_0, _SampleTexture2D_361c1baa7e5e484ab38560dd3341b6d8_RGBA_0, _Multiply_72a5e6242399449aad550d9798747548_Out_2);
            float4 _Property_2e6c583a57424a8180974b05f7e49119_Out_0 = VarColor;
            float4 _Multiply_c5a7db016b7b4a19a5d1eb518bbd1ebe_Out_2;
            Unity_Multiply_float(_Property_2e6c583a57424a8180974b05f7e49119_Out_0, _SampleTexture2D_361c1baa7e5e484ab38560dd3341b6d8_RGBA_0, _Multiply_c5a7db016b7b4a19a5d1eb518bbd1ebe_Out_2);
            float _Split_47b65f07839b45a2a7f9fd07f6ae4f85_R_1 = SHADERGRAPH_OBJECT_POSITION[0];
            float _Split_47b65f07839b45a2a7f9fd07f6ae4f85_G_2 = SHADERGRAPH_OBJECT_POSITION[1];
            float _Split_47b65f07839b45a2a7f9fd07f6ae4f85_B_3 = SHADERGRAPH_OBJECT_POSITION[2];
            float _Split_47b65f07839b45a2a7f9fd07f6ae4f85_A_4 = 0;
            float _Add_5ff2f4775c81475eaa4e1d2f1038221c_Out_2;
            Unity_Add_float(_Split_47b65f07839b45a2a7f9fd07f6ae4f85_R_1, _Split_47b65f07839b45a2a7f9fd07f6ae4f85_G_2, _Add_5ff2f4775c81475eaa4e1d2f1038221c_Out_2);
            float _Add_0b5c85958b4a44b4a1471bfb43d09937_Out_2;
            Unity_Add_float(_Add_5ff2f4775c81475eaa4e1d2f1038221c_Out_2, _Split_47b65f07839b45a2a7f9fd07f6ae4f85_G_2, _Add_0b5c85958b4a44b4a1471bfb43d09937_Out_2);
            float _Fraction_e6c8ad87e4c042d9a9a488035ab25129_Out_1;
            Unity_Fraction_float(_Add_0b5c85958b4a44b4a1471bfb43d09937_Out_2, _Fraction_e6c8ad87e4c042d9a9a488035ab25129_Out_1);
            float4 _Lerp_3f9a5249281541e1be1f190753e33b9d_Out_3;
            Unity_Lerp_float4(_Multiply_72a5e6242399449aad550d9798747548_Out_2, _Multiply_c5a7db016b7b4a19a5d1eb518bbd1ebe_Out_2, (_Fraction_e6c8ad87e4c042d9a9a488035ab25129_Out_1.xxxx), _Lerp_3f9a5249281541e1be1f190753e33b9d_Out_3);
            UnityTexture2D _Property_9a5e1d21ff144daeb1ad132f767da44e_Out_0 = UnityBuildTexture2DStructNoScale(NormalTex);
            float4 _SampleTexture2D_418ee20218b748af9c02a36c83fb414e_RGBA_0 = SAMPLE_TEXTURE2D(_Property_9a5e1d21ff144daeb1ad132f767da44e_Out_0.tex, _Property_9a5e1d21ff144daeb1ad132f767da44e_Out_0.samplerstate, IN.uv0.xy);
            _SampleTexture2D_418ee20218b748af9c02a36c83fb414e_RGBA_0.rgb = UnpackNormal(_SampleTexture2D_418ee20218b748af9c02a36c83fb414e_RGBA_0);
            float _SampleTexture2D_418ee20218b748af9c02a36c83fb414e_R_4 = _SampleTexture2D_418ee20218b748af9c02a36c83fb414e_RGBA_0.r;
            float _SampleTexture2D_418ee20218b748af9c02a36c83fb414e_G_5 = _SampleTexture2D_418ee20218b748af9c02a36c83fb414e_RGBA_0.g;
            float _SampleTexture2D_418ee20218b748af9c02a36c83fb414e_B_6 = _SampleTexture2D_418ee20218b748af9c02a36c83fb414e_RGBA_0.b;
            float _SampleTexture2D_418ee20218b748af9c02a36c83fb414e_A_7 = _SampleTexture2D_418ee20218b748af9c02a36c83fb414e_RGBA_0.a;
            float _Property_c176ee8debc744ebae505ad20fab01dd_Out_0 = Normal_Intensity;
            float3 _NormalStrength_05e86a137c744443b9dfa31f57ac64cd_Out_2;
            Unity_NormalStrength_float((_SampleTexture2D_418ee20218b748af9c02a36c83fb414e_RGBA_0.xyz), _Property_c176ee8debc744ebae505ad20fab01dd_Out_0, _NormalStrength_05e86a137c744443b9dfa31f57ac64cd_Out_2);
            float4 _Property_bf6324ebb86f442ba9760ead28e9f8e6_Out_0 = IsGammaSpace() ? LinearToSRGB(SSSColor) : SSSColor;
            float3 _MainLightCustomFunction_13744cd490944daa89a8ca8ea82bf801_Direction_0;
            float3 _MainLightCustomFunction_13744cd490944daa89a8ca8ea82bf801_Color_2;
            float _MainLightCustomFunction_13744cd490944daa89a8ca8ea82bf801_DistanceAtten_3;
            float _MainLightCustomFunction_13744cd490944daa89a8ca8ea82bf801_ShadowAtten_4;
            MainLight_float(IN.AbsoluteWorldSpacePosition, _MainLightCustomFunction_13744cd490944daa89a8ca8ea82bf801_Direction_0, _MainLightCustomFunction_13744cd490944daa89a8ca8ea82bf801_Color_2, _MainLightCustomFunction_13744cd490944daa89a8ca8ea82bf801_DistanceAtten_3, _MainLightCustomFunction_13744cd490944daa89a8ca8ea82bf801_ShadowAtten_4);
            float3 _Multiply_a5ee958a43d6486c8adafb051e9f9094_Out_2;
            Unity_Multiply_float(_MainLightCustomFunction_13744cd490944daa89a8ca8ea82bf801_Color_2, (_MainLightCustomFunction_13744cd490944daa89a8ca8ea82bf801_ShadowAtten_4.xxx), _Multiply_a5ee958a43d6486c8adafb051e9f9094_Out_2);
            float3 _Normalize_1f2add49bcef4d38aca69e47723ca228_Out_1;
            Unity_Normalize_float3(-1 * mul((float3x3)UNITY_MATRIX_M, transpose(mul(UNITY_MATRIX_I_M, UNITY_MATRIX_I_V)) [2].xyz), _Normalize_1f2add49bcef4d38aca69e47723ca228_Out_1);
            float _DotProduct_0440c3f3fd044a749b3a938d67815af5_Out_2;
            Unity_DotProduct_float3(_MainLightCustomFunction_13744cd490944daa89a8ca8ea82bf801_Direction_0, _Normalize_1f2add49bcef4d38aca69e47723ca228_Out_1, _DotProduct_0440c3f3fd044a749b3a938d67815af5_Out_2);
            float _Saturate_94e73a0bcc7a4d4680346dd6b19a67bd_Out_1;
            Unity_Saturate_float(_DotProduct_0440c3f3fd044a749b3a938d67815af5_Out_2, _Saturate_94e73a0bcc7a4d4680346dd6b19a67bd_Out_1);
            float3 _Multiply_0720425b373e459b88a6cee46d6b07df_Out_2;
            Unity_Multiply_float(_Multiply_a5ee958a43d6486c8adafb051e9f9094_Out_2, (_Saturate_94e73a0bcc7a4d4680346dd6b19a67bd_Out_1.xxx), _Multiply_0720425b373e459b88a6cee46d6b07df_Out_2);
            float3 _Multiply_c699ae0d77424fb590424f54dc343760_Out_2;
            Unity_Multiply_float((_Property_bf6324ebb86f442ba9760ead28e9f8e6_Out_0.xyz), _Multiply_0720425b373e459b88a6cee46d6b07df_Out_2, _Multiply_c699ae0d77424fb590424f54dc343760_Out_2);
            float3 _Multiply_10edc177c328439db60721337094f9b1_Out_2;
            Unity_Multiply_float((_Lerp_3f9a5249281541e1be1f190753e33b9d_Out_3.xyz), _Multiply_c699ae0d77424fb590424f54dc343760_Out_2, _Multiply_10edc177c328439db60721337094f9b1_Out_2);
            UnityTexture2D _Property_a295afe4790e404dab7b599e53bee6fc_Out_0 = UnityBuildTexture2DStructNoScale(ThicknessTex);
            float4 _SampleTexture2D_76eba85b0f1b44d3b740b14c94eb642b_RGBA_0 = SAMPLE_TEXTURE2D(_Property_a295afe4790e404dab7b599e53bee6fc_Out_0.tex, _Property_a295afe4790e404dab7b599e53bee6fc_Out_0.samplerstate, IN.uv0.xy);
            float _SampleTexture2D_76eba85b0f1b44d3b740b14c94eb642b_R_4 = _SampleTexture2D_76eba85b0f1b44d3b740b14c94eb642b_RGBA_0.r;
            float _SampleTexture2D_76eba85b0f1b44d3b740b14c94eb642b_G_5 = _SampleTexture2D_76eba85b0f1b44d3b740b14c94eb642b_RGBA_0.g;
            float _SampleTexture2D_76eba85b0f1b44d3b740b14c94eb642b_B_6 = _SampleTexture2D_76eba85b0f1b44d3b740b14c94eb642b_RGBA_0.b;
            float _SampleTexture2D_76eba85b0f1b44d3b740b14c94eb642b_A_7 = _SampleTexture2D_76eba85b0f1b44d3b740b14c94eb642b_RGBA_0.a;
            float3 _Multiply_b49f1dc358b14c73ba3e39551f96c7f9_Out_2;
            Unity_Multiply_float(_Multiply_10edc177c328439db60721337094f9b1_Out_2, (_SampleTexture2D_76eba85b0f1b44d3b740b14c94eb642b_RGBA_0.xyz), _Multiply_b49f1dc358b14c73ba3e39551f96c7f9_Out_2);
            float _Property_37adf16feb2c4f05af1e9de427c73874_Out_0 = Smoothness;
            UnityTexture2D _Property_94b068eae63c41fca823ae460e4c3a7f_Out_0 = UnityBuildTexture2DStructNoScale(MaskTex);
            float4 _SampleTexture2D_15fbde470d214b42aa9b8f3980d14645_RGBA_0 = SAMPLE_TEXTURE2D(_Property_94b068eae63c41fca823ae460e4c3a7f_Out_0.tex, _Property_94b068eae63c41fca823ae460e4c3a7f_Out_0.samplerstate, IN.uv0.xy);
            float _SampleTexture2D_15fbde470d214b42aa9b8f3980d14645_R_4 = _SampleTexture2D_15fbde470d214b42aa9b8f3980d14645_RGBA_0.r;
            float _SampleTexture2D_15fbde470d214b42aa9b8f3980d14645_G_5 = _SampleTexture2D_15fbde470d214b42aa9b8f3980d14645_RGBA_0.g;
            float _SampleTexture2D_15fbde470d214b42aa9b8f3980d14645_B_6 = _SampleTexture2D_15fbde470d214b42aa9b8f3980d14645_RGBA_0.b;
            float _SampleTexture2D_15fbde470d214b42aa9b8f3980d14645_A_7 = _SampleTexture2D_15fbde470d214b42aa9b8f3980d14645_RGBA_0.a;
            float _Multiply_485390079595403c811e1320cbe9d1c2_Out_2;
            Unity_Multiply_float(_Property_37adf16feb2c4f05af1e9de427c73874_Out_0, _SampleTexture2D_15fbde470d214b42aa9b8f3980d14645_A_7, _Multiply_485390079595403c811e1320cbe9d1c2_Out_2);
            float _Property_7784b184630844a1bd70b6904d69c970_Out_0 = AlphaClip;
            surface.BaseColor = (_Lerp_3f9a5249281541e1be1f190753e33b9d_Out_3.xyz);
            surface.NormalTS = _NormalStrength_05e86a137c744443b9dfa31f57ac64cd_Out_2;
            surface.Emission = _Multiply_b49f1dc358b14c73ba3e39551f96c7f9_Out_2;
            surface.Metallic = 0;
            surface.Smoothness = _Multiply_485390079595403c811e1320cbe9d1c2_Out_2;
            surface.Occlusion = 1;
            surface.Alpha = _SampleTexture2D_361c1baa7e5e484ab38560dd3341b6d8_A_7;
            surface.AlphaClipThreshold = _Property_7784b184630844a1bd70b6904d69c970_Out_0;
            return surface;
        }

            // --------------------------------------------------
            // Build Graph Inputs

            VertexDescriptionInputs BuildVertexDescriptionInputs(Attributes input)
        {
            VertexDescriptionInputs output;
            ZERO_INITIALIZE(VertexDescriptionInputs, output);

            output.ObjectSpaceNormal =           input.normalOS;
            output.WorldSpaceNormal =            TransformObjectToWorldNormal(input.normalOS);
            output.ObjectSpaceTangent =          input.tangentOS.xyz;
            output.WorldSpaceTangent =           TransformObjectToWorldDir(input.tangentOS.xyz);
            output.ObjectSpaceBiTangent =        normalize(cross(input.normalOS, input.tangentOS) * (input.tangentOS.w > 0.0f ? 1.0f : -1.0f) * GetOddNegativeScale());
            output.WorldSpaceBiTangent =         TransformObjectToWorldDir(output.ObjectSpaceBiTangent);
            output.ObjectSpacePosition =         input.positionOS;
            output.AbsoluteWorldSpacePosition =  GetAbsolutePositionWS(TransformObjectToWorld(input.positionOS));
            output.TimeParameters =              _TimeParameters.xyz;

            return output;
        }
            SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
        {
            SurfaceDescriptionInputs output;
            ZERO_INITIALIZE(SurfaceDescriptionInputs, output);



            output.TangentSpaceNormal =          float3(0.0f, 0.0f, 1.0f);


            output.AbsoluteWorldSpacePosition =  GetAbsolutePositionWS(input.positionWS);
            output.uv0 =                         input.texCoord0;
        #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
        #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN output.FaceSign =                    IS_FRONT_VFACE(input.cullFace, true, false);
        #else
        #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
        #endif
        #undef BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN

            return output;
        }

            // --------------------------------------------------
            // Main

            #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/Varyings.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/PBRForwardPass.hlsl"

            ENDHLSL
        }
        Pass
        {
            Name "ShadowCaster"
            Tags
            {
                "LightMode" = "ShadowCaster"
            }

            // Render State
            Cull Off
        Blend One Zero
        ZTest LEqual
        ZWrite On
        ColorMask 0

            // Debug
            // <None>

            // --------------------------------------------------
            // Pass

            HLSLPROGRAM

            // Pragmas
            #pragma target 2.0
        #pragma only_renderers gles gles3 glcore d3d11
        #pragma multi_compile_instancing
        #pragma vertex vert
        #pragma fragment frag

            // DotsInstancingOptions: <None>
            // HybridV1InjectedBuiltinProperties: <None>

            // Keywords
            #pragma multi_compile _ _CASTING_PUNCTUAL_LIGHT_SHADOW
            // GraphKeywords: <None>

            // Defines
            #define _AlphaClip 1
            #define _NORMALMAP 1
            #define _NORMAL_DROPOFF_TS 1
            #define ATTRIBUTES_NEED_NORMAL
            #define ATTRIBUTES_NEED_TANGENT
            #define ATTRIBUTES_NEED_TEXCOORD0
            #define VARYINGS_NEED_TEXCOORD0
            #define FEATURES_GRAPH_VERTEX
            /* WARNING: $splice Could not find named fragment 'PassInstancing' */
            #define SHADERPASS SHADERPASS_SHADOWCASTER
            /* WARNING: $splice Could not find named fragment 'DotsInstancingVars' */

            // Includes
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
         //#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
                // Graph Properties
            CBUFFER_START(UnityPerMaterial)
        float4 MainTex_TexelSize;
        float4 MainColor;
        float4 VarColor;
        float4 ThicknessTex_TexelSize;
        float4 SSSColor;
        float4 NormalTex_TexelSize;
        float Normal_Intensity;
        float4 MaskTex_TexelSize;
        float Smoothness;
        float AlphaClip;
        float Wind_Speed;
        float Wind_Frequency;
        float Wind_Strength;
        float3 Wind_Direction;
        float _Specular;
        float _EnvReflection;
        CBUFFER_END

        #include ".\Assets\KSC_Assets\03_Shaders\Common\Lighting_KSC.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"

            // --------------------------------------------------
            // Structs and Packing

            struct Attributes
        {
            float3 positionOS : POSITION;
            float3 normalOS : NORMAL;
            float4 tangentOS : TANGENT;
            float4 uv0 : TEXCOORD0;
            #if UNITY_ANY_INSTANCING_ENABLED
            uint instanceID : INSTANCEID_SEMANTIC;
            #endif
        };
        struct Varyings
        {
            float4 positionCS : SV_POSITION;
            float4 texCoord0;
            #if UNITY_ANY_INSTANCING_ENABLED
            uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
            #endif
        };
        struct SurfaceDescriptionInputs
        {
            float4 uv0;
        };
        struct VertexDescriptionInputs
        {
            float3 ObjectSpaceNormal;
            float3 WorldSpaceNormal;
            float3 ObjectSpaceTangent;
            float3 WorldSpaceTangent;
            float3 ObjectSpaceBiTangent;
            float3 WorldSpaceBiTangent;
            float3 ObjectSpacePosition;
            float3 AbsoluteWorldSpacePosition;
            float3 TimeParameters;
        };
        struct PackedVaryings
        {
            float4 positionCS : SV_POSITION;
            float4 interp0 : TEXCOORD0;
            #if UNITY_ANY_INSTANCING_ENABLED
            uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
            #endif
        };

            PackedVaryings PackVaryings (Varyings input)
        {
            PackedVaryings output;
            output.positionCS = input.positionCS;
            output.interp0.xyzw =  input.texCoord0;
            #if UNITY_ANY_INSTANCING_ENABLED
            output.instanceID = input.instanceID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            output.cullFace = input.cullFace;
            #endif
            return output;
        }
        Varyings UnpackVaryings (PackedVaryings input)
        {
            Varyings output;
            output.positionCS = input.positionCS;
            output.texCoord0 = input.interp0.xyzw;
            #if UNITY_ANY_INSTANCING_ENABLED
            output.instanceID = input.instanceID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            output.cullFace = input.cullFace;
            #endif
            return output;
        }

            // --------------------------------------------------
            // Graph

            // Graph Properties
          /* CBUFFER_START(UnityPerMaterial)
        float4 MainTex_TexelSize;
        float4 MainColor;
        float4 VarColor;
        float4 ThicknessTex_TexelSize;
        float4 SSSColor;
        float4 NormalTex_TexelSize;
        float Normal_Intensity;
        float4 MaskTex_TexelSize;
        float Smoothness;
        float AlphaClip;
        float Wind_Speed;
        float Wind_Frequency;
        float Wind_Strength;
        float3 Wind_Direction;
        float _Specular;
        float _EnvReflection;
        CBUFFER_END*/

        // Object and Global properties
        SAMPLER(SamplerState_Linear_Repeat);
        TEXTURE2D(MainTex);
        SAMPLER(samplerMainTex);
        TEXTURE2D(ThicknessTex);
        SAMPLER(samplerThicknessTex);
        TEXTURE2D(NormalTex);
        SAMPLER(samplerNormalTex);
        TEXTURE2D(MaskTex);
        SAMPLER(samplerMaskTex);

            // Graph Functions
            
        void Unity_Distance_float3(float3 A, float3 B, out float Out)
        {
            Out = distance(A, B);
        }

        void Unity_Normalize_float3(float3 In, out float3 Out)
        {
            Out = normalize(In);
        }

        void Unity_Multiply_float(float A, float B, out float Out)
        {
            Out = A * B;
        }

        void Unity_Add_float(float A, float B, out float Out)
        {
            Out = A + B;
        }

        void Unity_Sine_float(float In, out float Out)
        {
            Out = sin(In);
        }

        void Unity_Multiply_float(float3 A, float3 B, out float3 Out)
        {
            Out = A * B;
        }

        void Unity_Add_float3(float3 A, float3 B, out float3 Out)
        {
            Out = A + B;
        }

            // Graph Vertex
            struct VertexDescription
        {
            float3 Position;
            float3 Normal;
            float3 Tangent;
        };

        VertexDescription VertexDescriptionFunction(VertexDescriptionInputs IN)
        {
            VertexDescription description = (VertexDescription)0;
            float _Distance_a9453349007d43d7aa8e8ef2f9e0a6cb_Out_2;
            Unity_Distance_float3(float3(0, 0, 0), IN.ObjectSpacePosition, _Distance_a9453349007d43d7aa8e8ef2f9e0a6cb_Out_2);
            float3 _Property_294994c591114f76808f84d55e0400b9_Out_0 = Wind_Direction;
            float3 _Normalize_7a91dcb4e52548df84db8cb487a5210f_Out_1;
            Unity_Normalize_float3(_Property_294994c591114f76808f84d55e0400b9_Out_0, _Normalize_7a91dcb4e52548df84db8cb487a5210f_Out_1);
            float _Property_b1049a414de54aaf954fc9acaf3ffde3_Out_0 = Wind_Speed;
            float _Multiply_8cc8be463d9b4875839e9776be38d510_Out_2;
            Unity_Multiply_float(IN.TimeParameters.x, _Property_b1049a414de54aaf954fc9acaf3ffde3_Out_0, _Multiply_8cc8be463d9b4875839e9776be38d510_Out_2);
            float _Property_1cc960ad477c47dfa058e2368a6a0e59_Out_0 = Wind_Frequency;
            float _Split_47b65f07839b45a2a7f9fd07f6ae4f85_R_1 = SHADERGRAPH_OBJECT_POSITION[0];
            float _Split_47b65f07839b45a2a7f9fd07f6ae4f85_G_2 = SHADERGRAPH_OBJECT_POSITION[1];
            float _Split_47b65f07839b45a2a7f9fd07f6ae4f85_B_3 = SHADERGRAPH_OBJECT_POSITION[2];
            float _Split_47b65f07839b45a2a7f9fd07f6ae4f85_A_4 = 0;
            float _Add_5ff2f4775c81475eaa4e1d2f1038221c_Out_2;
            Unity_Add_float(_Split_47b65f07839b45a2a7f9fd07f6ae4f85_R_1, _Split_47b65f07839b45a2a7f9fd07f6ae4f85_G_2, _Add_5ff2f4775c81475eaa4e1d2f1038221c_Out_2);
            float _Add_0b5c85958b4a44b4a1471bfb43d09937_Out_2;
            Unity_Add_float(_Add_5ff2f4775c81475eaa4e1d2f1038221c_Out_2, _Split_47b65f07839b45a2a7f9fd07f6ae4f85_G_2, _Add_0b5c85958b4a44b4a1471bfb43d09937_Out_2);
            float _Multiply_36b4b25245ef46a6aa344b72d18b123a_Out_2;
            Unity_Multiply_float(_Property_1cc960ad477c47dfa058e2368a6a0e59_Out_0, _Add_0b5c85958b4a44b4a1471bfb43d09937_Out_2, _Multiply_36b4b25245ef46a6aa344b72d18b123a_Out_2);
            float _Add_112d774bc523412da770f4e752e0aeae_Out_2;
            Unity_Add_float(_Multiply_8cc8be463d9b4875839e9776be38d510_Out_2, _Multiply_36b4b25245ef46a6aa344b72d18b123a_Out_2, _Add_112d774bc523412da770f4e752e0aeae_Out_2);
            float _Sine_703274f04df4470980c37a4a595551c8_Out_1;
            Unity_Sine_float(_Add_112d774bc523412da770f4e752e0aeae_Out_2, _Sine_703274f04df4470980c37a4a595551c8_Out_1);
            float _Multiply_b11879df31064a879f94ea423ab2b2a5_Out_2;
            Unity_Multiply_float(_Sine_703274f04df4470980c37a4a595551c8_Out_1, 0.5, _Multiply_b11879df31064a879f94ea423ab2b2a5_Out_2);
            float _Add_efce9d790e58416f86a8ec3243204620_Out_2;
            Unity_Add_float(_Multiply_b11879df31064a879f94ea423ab2b2a5_Out_2, 0.5, _Add_efce9d790e58416f86a8ec3243204620_Out_2);
            float _Property_79b44419d559467d899224e854ea3824_Out_0 = Wind_Strength;
            float _Multiply_47956fa9e0804e8381e9c610915da0e6_Out_2;
            Unity_Multiply_float(_Add_efce9d790e58416f86a8ec3243204620_Out_2, _Property_79b44419d559467d899224e854ea3824_Out_0, _Multiply_47956fa9e0804e8381e9c610915da0e6_Out_2);
            float3 _Multiply_448ee0f8bc4f41a8bcf38fdc2b440495_Out_2;
            Unity_Multiply_float(_Normalize_7a91dcb4e52548df84db8cb487a5210f_Out_1, (_Multiply_47956fa9e0804e8381e9c610915da0e6_Out_2.xxx), _Multiply_448ee0f8bc4f41a8bcf38fdc2b440495_Out_2);
            float3 _Multiply_4e49ed416c034b90b06a8a56b2d0a49a_Out_2;
            Unity_Multiply_float((_Distance_a9453349007d43d7aa8e8ef2f9e0a6cb_Out_2.xxx), _Multiply_448ee0f8bc4f41a8bcf38fdc2b440495_Out_2, _Multiply_4e49ed416c034b90b06a8a56b2d0a49a_Out_2);
            float3 _Add_c136859739784517b3f510aa5bb4e469_Out_2;
            Unity_Add_float3(_Multiply_4e49ed416c034b90b06a8a56b2d0a49a_Out_2, IN.AbsoluteWorldSpacePosition, _Add_c136859739784517b3f510aa5bb4e469_Out_2);
            float3 _Transform_47fd3d4cb64f4d52b2077aaa7435e77b_Out_1 = TransformWorldToObject(_Add_c136859739784517b3f510aa5bb4e469_Out_2.xyz);
            description.Position = _Transform_47fd3d4cb64f4d52b2077aaa7435e77b_Out_1;
            description.Normal = IN.ObjectSpaceNormal;
            description.Tangent = IN.ObjectSpaceTangent;
            return description;
        }

            // Graph Pixel
            struct SurfaceDescription
        {
            float Alpha;
            float AlphaClipThreshold;
        };

        SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
        {
            SurfaceDescription surface = (SurfaceDescription)0;
            UnityTexture2D _Property_aed0c1bacf314879a280662567e38a31_Out_0 = UnityBuildTexture2DStructNoScale(MainTex);
            float4 _SampleTexture2D_361c1baa7e5e484ab38560dd3341b6d8_RGBA_0 = SAMPLE_TEXTURE2D(_Property_aed0c1bacf314879a280662567e38a31_Out_0.tex, _Property_aed0c1bacf314879a280662567e38a31_Out_0.samplerstate, IN.uv0.xy);
            float _SampleTexture2D_361c1baa7e5e484ab38560dd3341b6d8_R_4 = _SampleTexture2D_361c1baa7e5e484ab38560dd3341b6d8_RGBA_0.r;
            float _SampleTexture2D_361c1baa7e5e484ab38560dd3341b6d8_G_5 = _SampleTexture2D_361c1baa7e5e484ab38560dd3341b6d8_RGBA_0.g;
            float _SampleTexture2D_361c1baa7e5e484ab38560dd3341b6d8_B_6 = _SampleTexture2D_361c1baa7e5e484ab38560dd3341b6d8_RGBA_0.b;
            float _SampleTexture2D_361c1baa7e5e484ab38560dd3341b6d8_A_7 = _SampleTexture2D_361c1baa7e5e484ab38560dd3341b6d8_RGBA_0.a;
            float _Property_7784b184630844a1bd70b6904d69c970_Out_0 = AlphaClip;
            surface.Alpha = _SampleTexture2D_361c1baa7e5e484ab38560dd3341b6d8_A_7;
            surface.AlphaClipThreshold = _Property_7784b184630844a1bd70b6904d69c970_Out_0;
            return surface;
        }

            // --------------------------------------------------
            // Build Graph Inputs

            VertexDescriptionInputs BuildVertexDescriptionInputs(Attributes input)
        {
            VertexDescriptionInputs output;
            ZERO_INITIALIZE(VertexDescriptionInputs, output);

            output.ObjectSpaceNormal =           input.normalOS;
            output.WorldSpaceNormal =            TransformObjectToWorldNormal(input.normalOS);
            output.ObjectSpaceTangent =          input.tangentOS.xyz;
            output.WorldSpaceTangent =           TransformObjectToWorldDir(input.tangentOS.xyz);
            output.ObjectSpaceBiTangent =        normalize(cross(input.normalOS, input.tangentOS) * (input.tangentOS.w > 0.0f ? 1.0f : -1.0f) * GetOddNegativeScale());
            output.WorldSpaceBiTangent =         TransformObjectToWorldDir(output.ObjectSpaceBiTangent);
            output.ObjectSpacePosition =         input.positionOS;
            output.AbsoluteWorldSpacePosition =  GetAbsolutePositionWS(TransformObjectToWorld(input.positionOS));
            output.TimeParameters =              _TimeParameters.xyz;

            return output;
        }
            SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
        {
            SurfaceDescriptionInputs output;
            ZERO_INITIALIZE(SurfaceDescriptionInputs, output);





            output.uv0 =                         input.texCoord0;
        #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
        #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN output.FaceSign =                    IS_FRONT_VFACE(input.cullFace, true, false);
        #else
        #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
        #endif
        #undef BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN

            return output;
        }

            // --------------------------------------------------
            // Main

            #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/Varyings.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShadowCasterPass.hlsl"

            ENDHLSL
        }
        Pass
        {
            Name "DepthOnly"
            Tags
            {
                "LightMode" = "DepthOnly"
            }

            // Render State
            Cull Off
        Blend One Zero
        ZTest LEqual
        ZWrite On
        ColorMask 0

            // Debug
            // <None>

            // --------------------------------------------------
            // Pass

            HLSLPROGRAM

            // Pragmas
            #pragma target 2.0
        #pragma only_renderers gles gles3 glcore d3d11
        #pragma multi_compile_instancing
        #pragma vertex vert
        #pragma fragment frag

            // DotsInstancingOptions: <None>
            // HybridV1InjectedBuiltinProperties: <None>

            // Keywords
            // PassKeywords: <None>
            // GraphKeywords: <None>

            // Defines
            #define _AlphaClip 1
            #define _NORMALMAP 1
            #define _NORMAL_DROPOFF_TS 1
            #define ATTRIBUTES_NEED_NORMAL
            #define ATTRIBUTES_NEED_TANGENT
            #define ATTRIBUTES_NEED_TEXCOORD0
            #define VARYINGS_NEED_TEXCOORD0
            #define FEATURES_GRAPH_VERTEX
            /* WARNING: $splice Could not find named fragment 'PassInstancing' */
            #define SHADERPASS SHADERPASS_DEPTHONLY
            /* WARNING: $splice Could not find named fragment 'DotsInstancingVars' */

            // Includes
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
         //#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
                // Graph Properties
            CBUFFER_START(UnityPerMaterial)
        float4 MainTex_TexelSize;
        float4 MainColor;
        float4 VarColor;
        float4 ThicknessTex_TexelSize;
        float4 SSSColor;
        float4 NormalTex_TexelSize;
        float Normal_Intensity;
        float4 MaskTex_TexelSize;
        float Smoothness;
        float AlphaClip;
        float Wind_Speed;
        float Wind_Frequency;
        float Wind_Strength;
        float3 Wind_Direction;
        float _Specular;
        float _EnvReflection;
        CBUFFER_END

        #include ".\Assets\KSC_Assets\03_Shaders\Common\Lighting_KSC.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"

            // --------------------------------------------------
            // Structs and Packing

            struct Attributes
        {
            float3 positionOS : POSITION;
            float3 normalOS : NORMAL;
            float4 tangentOS : TANGENT;
            float4 uv0 : TEXCOORD0;
            #if UNITY_ANY_INSTANCING_ENABLED
            uint instanceID : INSTANCEID_SEMANTIC;
            #endif
        };
        struct Varyings
        {
            float4 positionCS : SV_POSITION;
            float4 texCoord0;
            #if UNITY_ANY_INSTANCING_ENABLED
            uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
            #endif
        };
        struct SurfaceDescriptionInputs
        {
            float4 uv0;
        };
        struct VertexDescriptionInputs
        {
            float3 ObjectSpaceNormal;
            float3 WorldSpaceNormal;
            float3 ObjectSpaceTangent;
            float3 WorldSpaceTangent;
            float3 ObjectSpaceBiTangent;
            float3 WorldSpaceBiTangent;
            float3 ObjectSpacePosition;
            float3 AbsoluteWorldSpacePosition;
            float3 TimeParameters;
        };
        struct PackedVaryings
        {
            float4 positionCS : SV_POSITION;
            float4 interp0 : TEXCOORD0;
            #if UNITY_ANY_INSTANCING_ENABLED
            uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
            #endif
        };

            PackedVaryings PackVaryings (Varyings input)
        {
            PackedVaryings output;
            output.positionCS = input.positionCS;
            output.interp0.xyzw =  input.texCoord0;
            #if UNITY_ANY_INSTANCING_ENABLED
            output.instanceID = input.instanceID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            output.cullFace = input.cullFace;
            #endif
            return output;
        }
        Varyings UnpackVaryings (PackedVaryings input)
        {
            Varyings output;
            output.positionCS = input.positionCS;
            output.texCoord0 = input.interp0.xyzw;
            #if UNITY_ANY_INSTANCING_ENABLED
            output.instanceID = input.instanceID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            output.cullFace = input.cullFace;
            #endif
            return output;
        }

            // --------------------------------------------------
            // Graph

            // Graph Properties
            /* CBUFFER_START(UnityPerMaterial)
        float4 MainTex_TexelSize;
        float4 MainColor;
        float4 VarColor;
        float4 ThicknessTex_TexelSize;
        float4 SSSColor;
        float4 NormalTex_TexelSize;
        float Normal_Intensity;
        float4 MaskTex_TexelSize;
        float Smoothness;
        float AlphaClip;
        float Wind_Speed;
        float Wind_Frequency;
        float Wind_Strength;
        float3 Wind_Direction;
        float _Specular;
        float _EnvReflection;
        CBUFFER_END*/

        // Object and Global properties
        SAMPLER(SamplerState_Linear_Repeat);
        TEXTURE2D(MainTex);
        SAMPLER(samplerMainTex);
        TEXTURE2D(ThicknessTex);
        SAMPLER(samplerThicknessTex);
        TEXTURE2D(NormalTex);
        SAMPLER(samplerNormalTex);
        TEXTURE2D(MaskTex);
        SAMPLER(samplerMaskTex);

            // Graph Functions
            
        void Unity_Distance_float3(float3 A, float3 B, out float Out)
        {
            Out = distance(A, B);
        }

        void Unity_Normalize_float3(float3 In, out float3 Out)
        {
            Out = normalize(In);
        }

        void Unity_Multiply_float(float A, float B, out float Out)
        {
            Out = A * B;
        }

        void Unity_Add_float(float A, float B, out float Out)
        {
            Out = A + B;
        }

        void Unity_Sine_float(float In, out float Out)
        {
            Out = sin(In);
        }

        void Unity_Multiply_float(float3 A, float3 B, out float3 Out)
        {
            Out = A * B;
        }

        void Unity_Add_float3(float3 A, float3 B, out float3 Out)
        {
            Out = A + B;
        }

            // Graph Vertex
            struct VertexDescription
        {
            float3 Position;
            float3 Normal;
            float3 Tangent;
        };

        VertexDescription VertexDescriptionFunction(VertexDescriptionInputs IN)
        {
            VertexDescription description = (VertexDescription)0;
            float _Distance_a9453349007d43d7aa8e8ef2f9e0a6cb_Out_2;
            Unity_Distance_float3(float3(0, 0, 0), IN.ObjectSpacePosition, _Distance_a9453349007d43d7aa8e8ef2f9e0a6cb_Out_2);
            float3 _Property_294994c591114f76808f84d55e0400b9_Out_0 = Wind_Direction;
            float3 _Normalize_7a91dcb4e52548df84db8cb487a5210f_Out_1;
            Unity_Normalize_float3(_Property_294994c591114f76808f84d55e0400b9_Out_0, _Normalize_7a91dcb4e52548df84db8cb487a5210f_Out_1);
            float _Property_b1049a414de54aaf954fc9acaf3ffde3_Out_0 = Wind_Speed;
            float _Multiply_8cc8be463d9b4875839e9776be38d510_Out_2;
            Unity_Multiply_float(IN.TimeParameters.x, _Property_b1049a414de54aaf954fc9acaf3ffde3_Out_0, _Multiply_8cc8be463d9b4875839e9776be38d510_Out_2);
            float _Property_1cc960ad477c47dfa058e2368a6a0e59_Out_0 = Wind_Frequency;
            float _Split_47b65f07839b45a2a7f9fd07f6ae4f85_R_1 = SHADERGRAPH_OBJECT_POSITION[0];
            float _Split_47b65f07839b45a2a7f9fd07f6ae4f85_G_2 = SHADERGRAPH_OBJECT_POSITION[1];
            float _Split_47b65f07839b45a2a7f9fd07f6ae4f85_B_3 = SHADERGRAPH_OBJECT_POSITION[2];
            float _Split_47b65f07839b45a2a7f9fd07f6ae4f85_A_4 = 0;
            float _Add_5ff2f4775c81475eaa4e1d2f1038221c_Out_2;
            Unity_Add_float(_Split_47b65f07839b45a2a7f9fd07f6ae4f85_R_1, _Split_47b65f07839b45a2a7f9fd07f6ae4f85_G_2, _Add_5ff2f4775c81475eaa4e1d2f1038221c_Out_2);
            float _Add_0b5c85958b4a44b4a1471bfb43d09937_Out_2;
            Unity_Add_float(_Add_5ff2f4775c81475eaa4e1d2f1038221c_Out_2, _Split_47b65f07839b45a2a7f9fd07f6ae4f85_G_2, _Add_0b5c85958b4a44b4a1471bfb43d09937_Out_2);
            float _Multiply_36b4b25245ef46a6aa344b72d18b123a_Out_2;
            Unity_Multiply_float(_Property_1cc960ad477c47dfa058e2368a6a0e59_Out_0, _Add_0b5c85958b4a44b4a1471bfb43d09937_Out_2, _Multiply_36b4b25245ef46a6aa344b72d18b123a_Out_2);
            float _Add_112d774bc523412da770f4e752e0aeae_Out_2;
            Unity_Add_float(_Multiply_8cc8be463d9b4875839e9776be38d510_Out_2, _Multiply_36b4b25245ef46a6aa344b72d18b123a_Out_2, _Add_112d774bc523412da770f4e752e0aeae_Out_2);
            float _Sine_703274f04df4470980c37a4a595551c8_Out_1;
            Unity_Sine_float(_Add_112d774bc523412da770f4e752e0aeae_Out_2, _Sine_703274f04df4470980c37a4a595551c8_Out_1);
            float _Multiply_b11879df31064a879f94ea423ab2b2a5_Out_2;
            Unity_Multiply_float(_Sine_703274f04df4470980c37a4a595551c8_Out_1, 0.5, _Multiply_b11879df31064a879f94ea423ab2b2a5_Out_2);
            float _Add_efce9d790e58416f86a8ec3243204620_Out_2;
            Unity_Add_float(_Multiply_b11879df31064a879f94ea423ab2b2a5_Out_2, 0.5, _Add_efce9d790e58416f86a8ec3243204620_Out_2);
            float _Property_79b44419d559467d899224e854ea3824_Out_0 = Wind_Strength;
            float _Multiply_47956fa9e0804e8381e9c610915da0e6_Out_2;
            Unity_Multiply_float(_Add_efce9d790e58416f86a8ec3243204620_Out_2, _Property_79b44419d559467d899224e854ea3824_Out_0, _Multiply_47956fa9e0804e8381e9c610915da0e6_Out_2);
            float3 _Multiply_448ee0f8bc4f41a8bcf38fdc2b440495_Out_2;
            Unity_Multiply_float(_Normalize_7a91dcb4e52548df84db8cb487a5210f_Out_1, (_Multiply_47956fa9e0804e8381e9c610915da0e6_Out_2.xxx), _Multiply_448ee0f8bc4f41a8bcf38fdc2b440495_Out_2);
            float3 _Multiply_4e49ed416c034b90b06a8a56b2d0a49a_Out_2;
            Unity_Multiply_float((_Distance_a9453349007d43d7aa8e8ef2f9e0a6cb_Out_2.xxx), _Multiply_448ee0f8bc4f41a8bcf38fdc2b440495_Out_2, _Multiply_4e49ed416c034b90b06a8a56b2d0a49a_Out_2);
            float3 _Add_c136859739784517b3f510aa5bb4e469_Out_2;
            Unity_Add_float3(_Multiply_4e49ed416c034b90b06a8a56b2d0a49a_Out_2, IN.AbsoluteWorldSpacePosition, _Add_c136859739784517b3f510aa5bb4e469_Out_2);
            float3 _Transform_47fd3d4cb64f4d52b2077aaa7435e77b_Out_1 = TransformWorldToObject(_Add_c136859739784517b3f510aa5bb4e469_Out_2.xyz);
            description.Position = _Transform_47fd3d4cb64f4d52b2077aaa7435e77b_Out_1;
            description.Normal = IN.ObjectSpaceNormal;
            description.Tangent = IN.ObjectSpaceTangent;
            return description;
        }

            // Graph Pixel
            struct SurfaceDescription
        {
            float Alpha;
            float AlphaClipThreshold;
        };

        SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
        {
            SurfaceDescription surface = (SurfaceDescription)0;
            UnityTexture2D _Property_aed0c1bacf314879a280662567e38a31_Out_0 = UnityBuildTexture2DStructNoScale(MainTex);
            float4 _SampleTexture2D_361c1baa7e5e484ab38560dd3341b6d8_RGBA_0 = SAMPLE_TEXTURE2D(_Property_aed0c1bacf314879a280662567e38a31_Out_0.tex, _Property_aed0c1bacf314879a280662567e38a31_Out_0.samplerstate, IN.uv0.xy);
            float _SampleTexture2D_361c1baa7e5e484ab38560dd3341b6d8_R_4 = _SampleTexture2D_361c1baa7e5e484ab38560dd3341b6d8_RGBA_0.r;
            float _SampleTexture2D_361c1baa7e5e484ab38560dd3341b6d8_G_5 = _SampleTexture2D_361c1baa7e5e484ab38560dd3341b6d8_RGBA_0.g;
            float _SampleTexture2D_361c1baa7e5e484ab38560dd3341b6d8_B_6 = _SampleTexture2D_361c1baa7e5e484ab38560dd3341b6d8_RGBA_0.b;
            float _SampleTexture2D_361c1baa7e5e484ab38560dd3341b6d8_A_7 = _SampleTexture2D_361c1baa7e5e484ab38560dd3341b6d8_RGBA_0.a;
            float _Property_7784b184630844a1bd70b6904d69c970_Out_0 = AlphaClip;
            surface.Alpha = _SampleTexture2D_361c1baa7e5e484ab38560dd3341b6d8_A_7;
            surface.AlphaClipThreshold = _Property_7784b184630844a1bd70b6904d69c970_Out_0;
            return surface;
        }

            // --------------------------------------------------
            // Build Graph Inputs

            VertexDescriptionInputs BuildVertexDescriptionInputs(Attributes input)
        {
            VertexDescriptionInputs output;
            ZERO_INITIALIZE(VertexDescriptionInputs, output);

            output.ObjectSpaceNormal =           input.normalOS;
            output.WorldSpaceNormal =            TransformObjectToWorldNormal(input.normalOS);
            output.ObjectSpaceTangent =          input.tangentOS.xyz;
            output.WorldSpaceTangent =           TransformObjectToWorldDir(input.tangentOS.xyz);
            output.ObjectSpaceBiTangent =        normalize(cross(input.normalOS, input.tangentOS) * (input.tangentOS.w > 0.0f ? 1.0f : -1.0f) * GetOddNegativeScale());
            output.WorldSpaceBiTangent =         TransformObjectToWorldDir(output.ObjectSpaceBiTangent);
            output.ObjectSpacePosition =         input.positionOS;
            output.AbsoluteWorldSpacePosition =  GetAbsolutePositionWS(TransformObjectToWorld(input.positionOS));
            output.TimeParameters =              _TimeParameters.xyz;

            return output;
        }
            SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
        {
            SurfaceDescriptionInputs output;
            ZERO_INITIALIZE(SurfaceDescriptionInputs, output);





            output.uv0 =                         input.texCoord0;
        #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
        #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN output.FaceSign =                    IS_FRONT_VFACE(input.cullFace, true, false);
        #else
        #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
        #endif
        #undef BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN

            return output;
        }

            // --------------------------------------------------
            // Main

            #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/Varyings.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/DepthOnlyPass.hlsl"

            ENDHLSL
        }
        Pass
        {
            Name "DepthNormals"
            Tags
            {
                "LightMode" = "DepthNormals"
            }

            // Render State
            Cull Off
        Blend One Zero
        ZTest LEqual
        ZWrite On

            // Debug
            // <None>

            // --------------------------------------------------
            // Pass

            HLSLPROGRAM

            // Pragmas
            #pragma target 2.0
        #pragma only_renderers gles gles3 glcore d3d11
        #pragma multi_compile_instancing
        #pragma vertex vert
        #pragma fragment frag

            // DotsInstancingOptions: <None>
            // HybridV1InjectedBuiltinProperties: <None>

            // Keywords
            // PassKeywords: <None>
            // GraphKeywords: <None>

            // Defines
            #define _AlphaClip 1
            #define _NORMALMAP 1
            #define _NORMAL_DROPOFF_TS 1
            #define ATTRIBUTES_NEED_NORMAL
            #define ATTRIBUTES_NEED_TANGENT
            #define ATTRIBUTES_NEED_TEXCOORD0
            #define ATTRIBUTES_NEED_TEXCOORD1
            #define VARYINGS_NEED_NORMAL_WS
            #define VARYINGS_NEED_TANGENT_WS
            #define VARYINGS_NEED_TEXCOORD0
            #define FEATURES_GRAPH_VERTEX
            /* WARNING: $splice Could not find named fragment 'PassInstancing' */
            #define SHADERPASS SHADERPASS_DEPTHNORMALSONLY
            /* WARNING: $splice Could not find named fragment 'DotsInstancingVars' */

            // Includes
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
        //#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
                // Graph Properties
            CBUFFER_START(UnityPerMaterial)
        float4 MainTex_TexelSize;
        float4 MainColor;
        float4 VarColor;
        float4 ThicknessTex_TexelSize;
        float4 SSSColor;
        float4 NormalTex_TexelSize;
        float Normal_Intensity;
        float4 MaskTex_TexelSize;
        float Smoothness;
        float AlphaClip;
        float Wind_Speed;
        float Wind_Frequency;
        float Wind_Strength;
        float3 Wind_Direction;
        float _Specular;
        float _EnvReflection;
        CBUFFER_END

        #include ".\Assets\KSC_Assets\03_Shaders\Common\Lighting_KSC.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"

            // --------------------------------------------------
            // Structs and Packing

            struct Attributes
        {
            float3 positionOS : POSITION;
            float3 normalOS : NORMAL;
            float4 tangentOS : TANGENT;
            float4 uv0 : TEXCOORD0;
            float4 uv1 : TEXCOORD1;
            #if UNITY_ANY_INSTANCING_ENABLED
            uint instanceID : INSTANCEID_SEMANTIC;
            #endif
        };
        struct Varyings
        {
            float4 positionCS : SV_POSITION;
            float3 normalWS;
            float4 tangentWS;
            float4 texCoord0;
            #if UNITY_ANY_INSTANCING_ENABLED
            uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
            #endif
        };
        struct SurfaceDescriptionInputs
        {
            float3 TangentSpaceNormal;
            float4 uv0;
        };
        struct VertexDescriptionInputs
        {
            float3 ObjectSpaceNormal;
            float3 WorldSpaceNormal;
            float3 ObjectSpaceTangent;
            float3 WorldSpaceTangent;
            float3 ObjectSpaceBiTangent;
            float3 WorldSpaceBiTangent;
            float3 ObjectSpacePosition;
            float3 AbsoluteWorldSpacePosition;
            float3 TimeParameters;
        };
        struct PackedVaryings
        {
            float4 positionCS : SV_POSITION;
            float3 interp0 : TEXCOORD0;
            float4 interp1 : TEXCOORD1;
            float4 interp2 : TEXCOORD2;
            #if UNITY_ANY_INSTANCING_ENABLED
            uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
            #endif
        };

            PackedVaryings PackVaryings (Varyings input)
        {
            PackedVaryings output;
            output.positionCS = input.positionCS;
            output.interp0.xyz =  input.normalWS;
            output.interp1.xyzw =  input.tangentWS;
            output.interp2.xyzw =  input.texCoord0;
            #if UNITY_ANY_INSTANCING_ENABLED
            output.instanceID = input.instanceID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            output.cullFace = input.cullFace;
            #endif
            return output;
        }
        Varyings UnpackVaryings (PackedVaryings input)
        {
            Varyings output;
            output.positionCS = input.positionCS;
            output.normalWS = input.interp0.xyz;
            output.tangentWS = input.interp1.xyzw;
            output.texCoord0 = input.interp2.xyzw;
            #if UNITY_ANY_INSTANCING_ENABLED
            output.instanceID = input.instanceID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            output.cullFace = input.cullFace;
            #endif
            return output;
        }

            // --------------------------------------------------
            // Graph

            // Graph Properties
            /* CBUFFER_START(UnityPerMaterial)
        float4 MainTex_TexelSize;
        float4 MainColor;
        float4 VarColor;
        float4 ThicknessTex_TexelSize;
        float4 SSSColor;
        float4 NormalTex_TexelSize;
        float Normal_Intensity;
        float4 MaskTex_TexelSize;
        float Smoothness;
        float AlphaClip;
        float Wind_Speed;
        float Wind_Frequency;
        float Wind_Strength;
        float3 Wind_Direction;
        float _Specular;
        float _EnvReflection;
        CBUFFER_END*/

        // Object and Global properties
        SAMPLER(SamplerState_Linear_Repeat);
        TEXTURE2D(MainTex);
        SAMPLER(samplerMainTex);
        TEXTURE2D(ThicknessTex);
        SAMPLER(samplerThicknessTex);
        TEXTURE2D(NormalTex);
        SAMPLER(samplerNormalTex);
        TEXTURE2D(MaskTex);
        SAMPLER(samplerMaskTex);

            // Graph Functions
            
        void Unity_Distance_float3(float3 A, float3 B, out float Out)
        {
            Out = distance(A, B);
        }

        void Unity_Normalize_float3(float3 In, out float3 Out)
        {
            Out = normalize(In);
        }

        void Unity_Multiply_float(float A, float B, out float Out)
        {
            Out = A * B;
        }

        void Unity_Add_float(float A, float B, out float Out)
        {
            Out = A + B;
        }

        void Unity_Sine_float(float In, out float Out)
        {
            Out = sin(In);
        }

        void Unity_Multiply_float(float3 A, float3 B, out float3 Out)
        {
            Out = A * B;
        }

        void Unity_Add_float3(float3 A, float3 B, out float3 Out)
        {
            Out = A + B;
        }

        void Unity_NormalStrength_float(float3 In, float Strength, out float3 Out)
        {
            Out = float3(In.rg * Strength, lerp(1, In.b, saturate(Strength)));
        }

            // Graph Vertex
            struct VertexDescription
        {
            float3 Position;
            float3 Normal;
            float3 Tangent;
        };

        VertexDescription VertexDescriptionFunction(VertexDescriptionInputs IN)
        {
            VertexDescription description = (VertexDescription)0;
            float _Distance_a9453349007d43d7aa8e8ef2f9e0a6cb_Out_2;
            Unity_Distance_float3(float3(0, 0, 0), IN.ObjectSpacePosition, _Distance_a9453349007d43d7aa8e8ef2f9e0a6cb_Out_2);
            float3 _Property_294994c591114f76808f84d55e0400b9_Out_0 = Wind_Direction;
            float3 _Normalize_7a91dcb4e52548df84db8cb487a5210f_Out_1;
            Unity_Normalize_float3(_Property_294994c591114f76808f84d55e0400b9_Out_0, _Normalize_7a91dcb4e52548df84db8cb487a5210f_Out_1);
            float _Property_b1049a414de54aaf954fc9acaf3ffde3_Out_0 = Wind_Speed;
            float _Multiply_8cc8be463d9b4875839e9776be38d510_Out_2;
            Unity_Multiply_float(IN.TimeParameters.x, _Property_b1049a414de54aaf954fc9acaf3ffde3_Out_0, _Multiply_8cc8be463d9b4875839e9776be38d510_Out_2);
            float _Property_1cc960ad477c47dfa058e2368a6a0e59_Out_0 = Wind_Frequency;
            float _Split_47b65f07839b45a2a7f9fd07f6ae4f85_R_1 = SHADERGRAPH_OBJECT_POSITION[0];
            float _Split_47b65f07839b45a2a7f9fd07f6ae4f85_G_2 = SHADERGRAPH_OBJECT_POSITION[1];
            float _Split_47b65f07839b45a2a7f9fd07f6ae4f85_B_3 = SHADERGRAPH_OBJECT_POSITION[2];
            float _Split_47b65f07839b45a2a7f9fd07f6ae4f85_A_4 = 0;
            float _Add_5ff2f4775c81475eaa4e1d2f1038221c_Out_2;
            Unity_Add_float(_Split_47b65f07839b45a2a7f9fd07f6ae4f85_R_1, _Split_47b65f07839b45a2a7f9fd07f6ae4f85_G_2, _Add_5ff2f4775c81475eaa4e1d2f1038221c_Out_2);
            float _Add_0b5c85958b4a44b4a1471bfb43d09937_Out_2;
            Unity_Add_float(_Add_5ff2f4775c81475eaa4e1d2f1038221c_Out_2, _Split_47b65f07839b45a2a7f9fd07f6ae4f85_G_2, _Add_0b5c85958b4a44b4a1471bfb43d09937_Out_2);
            float _Multiply_36b4b25245ef46a6aa344b72d18b123a_Out_2;
            Unity_Multiply_float(_Property_1cc960ad477c47dfa058e2368a6a0e59_Out_0, _Add_0b5c85958b4a44b4a1471bfb43d09937_Out_2, _Multiply_36b4b25245ef46a6aa344b72d18b123a_Out_2);
            float _Add_112d774bc523412da770f4e752e0aeae_Out_2;
            Unity_Add_float(_Multiply_8cc8be463d9b4875839e9776be38d510_Out_2, _Multiply_36b4b25245ef46a6aa344b72d18b123a_Out_2, _Add_112d774bc523412da770f4e752e0aeae_Out_2);
            float _Sine_703274f04df4470980c37a4a595551c8_Out_1;
            Unity_Sine_float(_Add_112d774bc523412da770f4e752e0aeae_Out_2, _Sine_703274f04df4470980c37a4a595551c8_Out_1);
            float _Multiply_b11879df31064a879f94ea423ab2b2a5_Out_2;
            Unity_Multiply_float(_Sine_703274f04df4470980c37a4a595551c8_Out_1, 0.5, _Multiply_b11879df31064a879f94ea423ab2b2a5_Out_2);
            float _Add_efce9d790e58416f86a8ec3243204620_Out_2;
            Unity_Add_float(_Multiply_b11879df31064a879f94ea423ab2b2a5_Out_2, 0.5, _Add_efce9d790e58416f86a8ec3243204620_Out_2);
            float _Property_79b44419d559467d899224e854ea3824_Out_0 = Wind_Strength;
            float _Multiply_47956fa9e0804e8381e9c610915da0e6_Out_2;
            Unity_Multiply_float(_Add_efce9d790e58416f86a8ec3243204620_Out_2, _Property_79b44419d559467d899224e854ea3824_Out_0, _Multiply_47956fa9e0804e8381e9c610915da0e6_Out_2);
            float3 _Multiply_448ee0f8bc4f41a8bcf38fdc2b440495_Out_2;
            Unity_Multiply_float(_Normalize_7a91dcb4e52548df84db8cb487a5210f_Out_1, (_Multiply_47956fa9e0804e8381e9c610915da0e6_Out_2.xxx), _Multiply_448ee0f8bc4f41a8bcf38fdc2b440495_Out_2);
            float3 _Multiply_4e49ed416c034b90b06a8a56b2d0a49a_Out_2;
            Unity_Multiply_float((_Distance_a9453349007d43d7aa8e8ef2f9e0a6cb_Out_2.xxx), _Multiply_448ee0f8bc4f41a8bcf38fdc2b440495_Out_2, _Multiply_4e49ed416c034b90b06a8a56b2d0a49a_Out_2);
            float3 _Add_c136859739784517b3f510aa5bb4e469_Out_2;
            Unity_Add_float3(_Multiply_4e49ed416c034b90b06a8a56b2d0a49a_Out_2, IN.AbsoluteWorldSpacePosition, _Add_c136859739784517b3f510aa5bb4e469_Out_2);
            float3 _Transform_47fd3d4cb64f4d52b2077aaa7435e77b_Out_1 = TransformWorldToObject(_Add_c136859739784517b3f510aa5bb4e469_Out_2.xyz);
            description.Position = _Transform_47fd3d4cb64f4d52b2077aaa7435e77b_Out_1;
            description.Normal = IN.ObjectSpaceNormal;
            description.Tangent = IN.ObjectSpaceTangent;
            return description;
        }

            // Graph Pixel
            struct SurfaceDescription
        {
            float3 NormalTS;
            float Alpha;
            float AlphaClipThreshold;
        };

        SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
        {
            SurfaceDescription surface = (SurfaceDescription)0;
            UnityTexture2D _Property_9a5e1d21ff144daeb1ad132f767da44e_Out_0 = UnityBuildTexture2DStructNoScale(NormalTex);
            float4 _SampleTexture2D_418ee20218b748af9c02a36c83fb414e_RGBA_0 = SAMPLE_TEXTURE2D(_Property_9a5e1d21ff144daeb1ad132f767da44e_Out_0.tex, _Property_9a5e1d21ff144daeb1ad132f767da44e_Out_0.samplerstate, IN.uv0.xy);
            _SampleTexture2D_418ee20218b748af9c02a36c83fb414e_RGBA_0.rgb = UnpackNormal(_SampleTexture2D_418ee20218b748af9c02a36c83fb414e_RGBA_0);
            float _SampleTexture2D_418ee20218b748af9c02a36c83fb414e_R_4 = _SampleTexture2D_418ee20218b748af9c02a36c83fb414e_RGBA_0.r;
            float _SampleTexture2D_418ee20218b748af9c02a36c83fb414e_G_5 = _SampleTexture2D_418ee20218b748af9c02a36c83fb414e_RGBA_0.g;
            float _SampleTexture2D_418ee20218b748af9c02a36c83fb414e_B_6 = _SampleTexture2D_418ee20218b748af9c02a36c83fb414e_RGBA_0.b;
            float _SampleTexture2D_418ee20218b748af9c02a36c83fb414e_A_7 = _SampleTexture2D_418ee20218b748af9c02a36c83fb414e_RGBA_0.a;
            float _Property_c176ee8debc744ebae505ad20fab01dd_Out_0 = Normal_Intensity;
            float3 _NormalStrength_05e86a137c744443b9dfa31f57ac64cd_Out_2;
            Unity_NormalStrength_float((_SampleTexture2D_418ee20218b748af9c02a36c83fb414e_RGBA_0.xyz), _Property_c176ee8debc744ebae505ad20fab01dd_Out_0, _NormalStrength_05e86a137c744443b9dfa31f57ac64cd_Out_2);
            UnityTexture2D _Property_aed0c1bacf314879a280662567e38a31_Out_0 = UnityBuildTexture2DStructNoScale(MainTex);
            float4 _SampleTexture2D_361c1baa7e5e484ab38560dd3341b6d8_RGBA_0 = SAMPLE_TEXTURE2D(_Property_aed0c1bacf314879a280662567e38a31_Out_0.tex, _Property_aed0c1bacf314879a280662567e38a31_Out_0.samplerstate, IN.uv0.xy);
            float _SampleTexture2D_361c1baa7e5e484ab38560dd3341b6d8_R_4 = _SampleTexture2D_361c1baa7e5e484ab38560dd3341b6d8_RGBA_0.r;
            float _SampleTexture2D_361c1baa7e5e484ab38560dd3341b6d8_G_5 = _SampleTexture2D_361c1baa7e5e484ab38560dd3341b6d8_RGBA_0.g;
            float _SampleTexture2D_361c1baa7e5e484ab38560dd3341b6d8_B_6 = _SampleTexture2D_361c1baa7e5e484ab38560dd3341b6d8_RGBA_0.b;
            float _SampleTexture2D_361c1baa7e5e484ab38560dd3341b6d8_A_7 = _SampleTexture2D_361c1baa7e5e484ab38560dd3341b6d8_RGBA_0.a;
            float _Property_7784b184630844a1bd70b6904d69c970_Out_0 = AlphaClip;
            surface.NormalTS = _NormalStrength_05e86a137c744443b9dfa31f57ac64cd_Out_2;
            surface.Alpha = _SampleTexture2D_361c1baa7e5e484ab38560dd3341b6d8_A_7;
            surface.AlphaClipThreshold = _Property_7784b184630844a1bd70b6904d69c970_Out_0;
            return surface;
        }

            // --------------------------------------------------
            // Build Graph Inputs

            VertexDescriptionInputs BuildVertexDescriptionInputs(Attributes input)
        {
            VertexDescriptionInputs output;
            ZERO_INITIALIZE(VertexDescriptionInputs, output);

            output.ObjectSpaceNormal =           input.normalOS;
            output.WorldSpaceNormal =            TransformObjectToWorldNormal(input.normalOS);
            output.ObjectSpaceTangent =          input.tangentOS.xyz;
            output.WorldSpaceTangent =           TransformObjectToWorldDir(input.tangentOS.xyz);
            output.ObjectSpaceBiTangent =        normalize(cross(input.normalOS, input.tangentOS) * (input.tangentOS.w > 0.0f ? 1.0f : -1.0f) * GetOddNegativeScale());
            output.WorldSpaceBiTangent =         TransformObjectToWorldDir(output.ObjectSpaceBiTangent);
            output.ObjectSpacePosition =         input.positionOS;
            output.AbsoluteWorldSpacePosition =  GetAbsolutePositionWS(TransformObjectToWorld(input.positionOS));
            output.TimeParameters =              _TimeParameters.xyz;

            return output;
        }
            SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
        {
            SurfaceDescriptionInputs output;
            ZERO_INITIALIZE(SurfaceDescriptionInputs, output);



            output.TangentSpaceNormal =          float3(0.0f, 0.0f, 1.0f);


            output.uv0 =                         input.texCoord0;
        #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
        #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN output.FaceSign =                    IS_FRONT_VFACE(input.cullFace, true, false);
        #else
        #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
        #endif
        #undef BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN

            return output;
        }

            // --------------------------------------------------
            // Main

            #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/Varyings.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/DepthNormalsOnlyPass.hlsl"

            ENDHLSL
        }
        Pass
        {
            Name "Meta"
            Tags
            {
                "LightMode" = "Meta"
            }

            // Render State
            Cull Off

            // Debug
            // <None>

            // --------------------------------------------------
            // Pass

            HLSLPROGRAM

            // Pragmas
            #pragma target 2.0
        #pragma only_renderers gles gles3 glcore d3d11
        #pragma vertex vert
        #pragma fragment frag

            // DotsInstancingOptions: <None>
            // HybridV1InjectedBuiltinProperties: <None>

            // Keywords
            #pragma shader_feature _ _SMOOTHNESS_TEXTURE_ALBEDO_CHANNEL_A
            // GraphKeywords: <None>

            // Defines
            #define _AlphaClip 1
            #define _NORMALMAP 1
            #define _NORMAL_DROPOFF_TS 1
            #define ATTRIBUTES_NEED_NORMAL
            #define ATTRIBUTES_NEED_TANGENT
            #define ATTRIBUTES_NEED_TEXCOORD0
            #define ATTRIBUTES_NEED_TEXCOORD1
            #define ATTRIBUTES_NEED_TEXCOORD2
            #define VARYINGS_NEED_POSITION_WS
            #define VARYINGS_NEED_TEXCOORD0
            #define FEATURES_GRAPH_VERTEX
            /* WARNING: $splice Could not find named fragment 'PassInstancing' */
            #define SHADERPASS SHADERPASS_META
            /* WARNING: $splice Could not find named fragment 'DotsInstancingVars' */

            // Includes
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
         //#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
                // Graph Properties
            CBUFFER_START(UnityPerMaterial)
        float4 MainTex_TexelSize;
        float4 MainColor;
        float4 VarColor;
        float4 ThicknessTex_TexelSize;
        float4 SSSColor;
        float4 NormalTex_TexelSize;
        float Normal_Intensity;
        float4 MaskTex_TexelSize;
        float Smoothness;
        float AlphaClip;
        float Wind_Speed;
        float Wind_Frequency;
        float Wind_Strength;
        float3 Wind_Direction;
        float _Specular;
        float _EnvReflection;
        CBUFFER_END

        #include ".\Assets\KSC_Assets\03_Shaders\Common\Lighting_KSC.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/MetaInput.hlsl"

            // --------------------------------------------------
            // Structs and Packing

            struct Attributes
        {
            float3 positionOS : POSITION;
            float3 normalOS : NORMAL;
            float4 tangentOS : TANGENT;
            float4 uv0 : TEXCOORD0;
            float4 uv1 : TEXCOORD1;
            float4 uv2 : TEXCOORD2;
            #if UNITY_ANY_INSTANCING_ENABLED
            uint instanceID : INSTANCEID_SEMANTIC;
            #endif
        };
        struct Varyings
        {
            float4 positionCS : SV_POSITION;
            float3 positionWS;
            float4 texCoord0;
            #if UNITY_ANY_INSTANCING_ENABLED
            uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
            #endif
        };
        struct SurfaceDescriptionInputs
        {
            float3 AbsoluteWorldSpacePosition;
            float4 uv0;
        };
        struct VertexDescriptionInputs
        {
            float3 ObjectSpaceNormal;
            float3 WorldSpaceNormal;
            float3 ObjectSpaceTangent;
            float3 WorldSpaceTangent;
            float3 ObjectSpaceBiTangent;
            float3 WorldSpaceBiTangent;
            float3 ObjectSpacePosition;
            float3 AbsoluteWorldSpacePosition;
            float3 TimeParameters;
        };
        struct PackedVaryings
        {
            float4 positionCS : SV_POSITION;
            float3 interp0 : TEXCOORD0;
            float4 interp1 : TEXCOORD1;
            #if UNITY_ANY_INSTANCING_ENABLED
            uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
            #endif
        };

            PackedVaryings PackVaryings (Varyings input)
        {
            PackedVaryings output;
            output.positionCS = input.positionCS;
            output.interp0.xyz =  input.positionWS;
            output.interp1.xyzw =  input.texCoord0;
            #if UNITY_ANY_INSTANCING_ENABLED
            output.instanceID = input.instanceID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            output.cullFace = input.cullFace;
            #endif
            return output;
        }
        Varyings UnpackVaryings (PackedVaryings input)
        {
            Varyings output;
            output.positionCS = input.positionCS;
            output.positionWS = input.interp0.xyz;
            output.texCoord0 = input.interp1.xyzw;
            #if UNITY_ANY_INSTANCING_ENABLED
            output.instanceID = input.instanceID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            output.cullFace = input.cullFace;
            #endif
            return output;
        }

            // --------------------------------------------------
            // Graph

            // Graph Properties
            /* CBUFFER_START(UnityPerMaterial)
        float4 MainTex_TexelSize;
        float4 MainColor;
        float4 VarColor;
        float4 ThicknessTex_TexelSize;
        float4 SSSColor;
        float4 NormalTex_TexelSize;
        float Normal_Intensity;
        float4 MaskTex_TexelSize;
        float Smoothness;
        float AlphaClip;
        float Wind_Speed;
        float Wind_Frequency;
        float Wind_Strength;
        float3 Wind_Direction;
        float _Specular;
        float _EnvReflection;
        CBUFFER_END*/

        // Object and Global properties
        SAMPLER(SamplerState_Linear_Repeat);
        TEXTURE2D(MainTex);
        SAMPLER(samplerMainTex);
        TEXTURE2D(ThicknessTex);
        SAMPLER(samplerThicknessTex);
        TEXTURE2D(NormalTex);
        SAMPLER(samplerNormalTex);
        TEXTURE2D(MaskTex);
        SAMPLER(samplerMaskTex);

            // Graph Functions
            
        void Unity_Distance_float3(float3 A, float3 B, out float Out)
        {
            Out = distance(A, B);
        }

        void Unity_Normalize_float3(float3 In, out float3 Out)
        {
            Out = normalize(In);
        }

        void Unity_Multiply_float(float A, float B, out float Out)
        {
            Out = A * B;
        }

        void Unity_Add_float(float A, float B, out float Out)
        {
            Out = A + B;
        }

        void Unity_Sine_float(float In, out float Out)
        {
            Out = sin(In);
        }

        void Unity_Multiply_float(float3 A, float3 B, out float3 Out)
        {
            Out = A * B;
        }

        void Unity_Add_float3(float3 A, float3 B, out float3 Out)
        {
            Out = A + B;
        }

        void Unity_Multiply_float(float4 A, float4 B, out float4 Out)
        {
            Out = A * B;
        }

        void Unity_Fraction_float(float In, out float Out)
        {
            Out = frac(In);
        }

        void Unity_Lerp_float4(float4 A, float4 B, float4 T, out float4 Out)
        {
            Out = lerp(A, B, T);
        }

        // 42362d8732025af29918c5cc8ad4975c
        #include "Assets/KSC_Assets/03_Shaders/Common/Lighting_Function.hlsl"

        void Unity_DotProduct_float3(float3 A, float3 B, out float Out)
        {
            Out = dot(A, B);
        }

        void Unity_Saturate_float(float In, out float Out)
        {
            Out = saturate(In);
        }

            // Graph Vertex
            struct VertexDescription
        {
            float3 Position;
            float3 Normal;
            float3 Tangent;
        };

        VertexDescription VertexDescriptionFunction(VertexDescriptionInputs IN)
        {
            VertexDescription description = (VertexDescription)0;
            float _Distance_a9453349007d43d7aa8e8ef2f9e0a6cb_Out_2;
            Unity_Distance_float3(float3(0, 0, 0), IN.ObjectSpacePosition, _Distance_a9453349007d43d7aa8e8ef2f9e0a6cb_Out_2);
            float3 _Property_294994c591114f76808f84d55e0400b9_Out_0 = Wind_Direction;
            float3 _Normalize_7a91dcb4e52548df84db8cb487a5210f_Out_1;
            Unity_Normalize_float3(_Property_294994c591114f76808f84d55e0400b9_Out_0, _Normalize_7a91dcb4e52548df84db8cb487a5210f_Out_1);
            float _Property_b1049a414de54aaf954fc9acaf3ffde3_Out_0 = Wind_Speed;
            float _Multiply_8cc8be463d9b4875839e9776be38d510_Out_2;
            Unity_Multiply_float(IN.TimeParameters.x, _Property_b1049a414de54aaf954fc9acaf3ffde3_Out_0, _Multiply_8cc8be463d9b4875839e9776be38d510_Out_2);
            float _Property_1cc960ad477c47dfa058e2368a6a0e59_Out_0 = Wind_Frequency;
            float _Split_47b65f07839b45a2a7f9fd07f6ae4f85_R_1 = SHADERGRAPH_OBJECT_POSITION[0];
            float _Split_47b65f07839b45a2a7f9fd07f6ae4f85_G_2 = SHADERGRAPH_OBJECT_POSITION[1];
            float _Split_47b65f07839b45a2a7f9fd07f6ae4f85_B_3 = SHADERGRAPH_OBJECT_POSITION[2];
            float _Split_47b65f07839b45a2a7f9fd07f6ae4f85_A_4 = 0;
            float _Add_5ff2f4775c81475eaa4e1d2f1038221c_Out_2;
            Unity_Add_float(_Split_47b65f07839b45a2a7f9fd07f6ae4f85_R_1, _Split_47b65f07839b45a2a7f9fd07f6ae4f85_G_2, _Add_5ff2f4775c81475eaa4e1d2f1038221c_Out_2);
            float _Add_0b5c85958b4a44b4a1471bfb43d09937_Out_2;
            Unity_Add_float(_Add_5ff2f4775c81475eaa4e1d2f1038221c_Out_2, _Split_47b65f07839b45a2a7f9fd07f6ae4f85_G_2, _Add_0b5c85958b4a44b4a1471bfb43d09937_Out_2);
            float _Multiply_36b4b25245ef46a6aa344b72d18b123a_Out_2;
            Unity_Multiply_float(_Property_1cc960ad477c47dfa058e2368a6a0e59_Out_0, _Add_0b5c85958b4a44b4a1471bfb43d09937_Out_2, _Multiply_36b4b25245ef46a6aa344b72d18b123a_Out_2);
            float _Add_112d774bc523412da770f4e752e0aeae_Out_2;
            Unity_Add_float(_Multiply_8cc8be463d9b4875839e9776be38d510_Out_2, _Multiply_36b4b25245ef46a6aa344b72d18b123a_Out_2, _Add_112d774bc523412da770f4e752e0aeae_Out_2);
            float _Sine_703274f04df4470980c37a4a595551c8_Out_1;
            Unity_Sine_float(_Add_112d774bc523412da770f4e752e0aeae_Out_2, _Sine_703274f04df4470980c37a4a595551c8_Out_1);
            float _Multiply_b11879df31064a879f94ea423ab2b2a5_Out_2;
            Unity_Multiply_float(_Sine_703274f04df4470980c37a4a595551c8_Out_1, 0.5, _Multiply_b11879df31064a879f94ea423ab2b2a5_Out_2);
            float _Add_efce9d790e58416f86a8ec3243204620_Out_2;
            Unity_Add_float(_Multiply_b11879df31064a879f94ea423ab2b2a5_Out_2, 0.5, _Add_efce9d790e58416f86a8ec3243204620_Out_2);
            float _Property_79b44419d559467d899224e854ea3824_Out_0 = Wind_Strength;
            float _Multiply_47956fa9e0804e8381e9c610915da0e6_Out_2;
            Unity_Multiply_float(_Add_efce9d790e58416f86a8ec3243204620_Out_2, _Property_79b44419d559467d899224e854ea3824_Out_0, _Multiply_47956fa9e0804e8381e9c610915da0e6_Out_2);
            float3 _Multiply_448ee0f8bc4f41a8bcf38fdc2b440495_Out_2;
            Unity_Multiply_float(_Normalize_7a91dcb4e52548df84db8cb487a5210f_Out_1, (_Multiply_47956fa9e0804e8381e9c610915da0e6_Out_2.xxx), _Multiply_448ee0f8bc4f41a8bcf38fdc2b440495_Out_2);
            float3 _Multiply_4e49ed416c034b90b06a8a56b2d0a49a_Out_2;
            Unity_Multiply_float((_Distance_a9453349007d43d7aa8e8ef2f9e0a6cb_Out_2.xxx), _Multiply_448ee0f8bc4f41a8bcf38fdc2b440495_Out_2, _Multiply_4e49ed416c034b90b06a8a56b2d0a49a_Out_2);
            float3 _Add_c136859739784517b3f510aa5bb4e469_Out_2;
            Unity_Add_float3(_Multiply_4e49ed416c034b90b06a8a56b2d0a49a_Out_2, IN.AbsoluteWorldSpacePosition, _Add_c136859739784517b3f510aa5bb4e469_Out_2);
            float3 _Transform_47fd3d4cb64f4d52b2077aaa7435e77b_Out_1 = TransformWorldToObject(_Add_c136859739784517b3f510aa5bb4e469_Out_2.xyz);
            description.Position = _Transform_47fd3d4cb64f4d52b2077aaa7435e77b_Out_1;
            description.Normal = IN.ObjectSpaceNormal;
            description.Tangent = IN.ObjectSpaceTangent;
            return description;
        }

            // Graph Pixel
            struct SurfaceDescription
        {
            float3 BaseColor;
            float3 Emission;
            float Alpha;
            float AlphaClipThreshold;
        };

        SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
        {
            SurfaceDescription surface = (SurfaceDescription)0;
            float4 _Property_1931a4995a954dc59a2a81efc2ac7bc3_Out_0 = MainColor;
            UnityTexture2D _Property_aed0c1bacf314879a280662567e38a31_Out_0 = UnityBuildTexture2DStructNoScale(MainTex);
            float4 _SampleTexture2D_361c1baa7e5e484ab38560dd3341b6d8_RGBA_0 = SAMPLE_TEXTURE2D(_Property_aed0c1bacf314879a280662567e38a31_Out_0.tex, _Property_aed0c1bacf314879a280662567e38a31_Out_0.samplerstate, IN.uv0.xy);
            float _SampleTexture2D_361c1baa7e5e484ab38560dd3341b6d8_R_4 = _SampleTexture2D_361c1baa7e5e484ab38560dd3341b6d8_RGBA_0.r;
            float _SampleTexture2D_361c1baa7e5e484ab38560dd3341b6d8_G_5 = _SampleTexture2D_361c1baa7e5e484ab38560dd3341b6d8_RGBA_0.g;
            float _SampleTexture2D_361c1baa7e5e484ab38560dd3341b6d8_B_6 = _SampleTexture2D_361c1baa7e5e484ab38560dd3341b6d8_RGBA_0.b;
            float _SampleTexture2D_361c1baa7e5e484ab38560dd3341b6d8_A_7 = _SampleTexture2D_361c1baa7e5e484ab38560dd3341b6d8_RGBA_0.a;
            float4 _Multiply_72a5e6242399449aad550d9798747548_Out_2;
            Unity_Multiply_float(_Property_1931a4995a954dc59a2a81efc2ac7bc3_Out_0, _SampleTexture2D_361c1baa7e5e484ab38560dd3341b6d8_RGBA_0, _Multiply_72a5e6242399449aad550d9798747548_Out_2);
            float4 _Property_2e6c583a57424a8180974b05f7e49119_Out_0 = VarColor;
            float4 _Multiply_c5a7db016b7b4a19a5d1eb518bbd1ebe_Out_2;
            Unity_Multiply_float(_Property_2e6c583a57424a8180974b05f7e49119_Out_0, _SampleTexture2D_361c1baa7e5e484ab38560dd3341b6d8_RGBA_0, _Multiply_c5a7db016b7b4a19a5d1eb518bbd1ebe_Out_2);
            float _Split_47b65f07839b45a2a7f9fd07f6ae4f85_R_1 = SHADERGRAPH_OBJECT_POSITION[0];
            float _Split_47b65f07839b45a2a7f9fd07f6ae4f85_G_2 = SHADERGRAPH_OBJECT_POSITION[1];
            float _Split_47b65f07839b45a2a7f9fd07f6ae4f85_B_3 = SHADERGRAPH_OBJECT_POSITION[2];
            float _Split_47b65f07839b45a2a7f9fd07f6ae4f85_A_4 = 0;
            float _Add_5ff2f4775c81475eaa4e1d2f1038221c_Out_2;
            Unity_Add_float(_Split_47b65f07839b45a2a7f9fd07f6ae4f85_R_1, _Split_47b65f07839b45a2a7f9fd07f6ae4f85_G_2, _Add_5ff2f4775c81475eaa4e1d2f1038221c_Out_2);
            float _Add_0b5c85958b4a44b4a1471bfb43d09937_Out_2;
            Unity_Add_float(_Add_5ff2f4775c81475eaa4e1d2f1038221c_Out_2, _Split_47b65f07839b45a2a7f9fd07f6ae4f85_G_2, _Add_0b5c85958b4a44b4a1471bfb43d09937_Out_2);
            float _Fraction_e6c8ad87e4c042d9a9a488035ab25129_Out_1;
            Unity_Fraction_float(_Add_0b5c85958b4a44b4a1471bfb43d09937_Out_2, _Fraction_e6c8ad87e4c042d9a9a488035ab25129_Out_1);
            float4 _Lerp_3f9a5249281541e1be1f190753e33b9d_Out_3;
            Unity_Lerp_float4(_Multiply_72a5e6242399449aad550d9798747548_Out_2, _Multiply_c5a7db016b7b4a19a5d1eb518bbd1ebe_Out_2, (_Fraction_e6c8ad87e4c042d9a9a488035ab25129_Out_1.xxxx), _Lerp_3f9a5249281541e1be1f190753e33b9d_Out_3);
            float4 _Property_bf6324ebb86f442ba9760ead28e9f8e6_Out_0 = IsGammaSpace() ? LinearToSRGB(SSSColor) : SSSColor;
            float3 _MainLightCustomFunction_13744cd490944daa89a8ca8ea82bf801_Direction_0;
            float3 _MainLightCustomFunction_13744cd490944daa89a8ca8ea82bf801_Color_2;
            float _MainLightCustomFunction_13744cd490944daa89a8ca8ea82bf801_DistanceAtten_3;
            float _MainLightCustomFunction_13744cd490944daa89a8ca8ea82bf801_ShadowAtten_4;
            MainLight_float(IN.AbsoluteWorldSpacePosition, _MainLightCustomFunction_13744cd490944daa89a8ca8ea82bf801_Direction_0, _MainLightCustomFunction_13744cd490944daa89a8ca8ea82bf801_Color_2, _MainLightCustomFunction_13744cd490944daa89a8ca8ea82bf801_DistanceAtten_3, _MainLightCustomFunction_13744cd490944daa89a8ca8ea82bf801_ShadowAtten_4);
            float3 _Multiply_a5ee958a43d6486c8adafb051e9f9094_Out_2;
            Unity_Multiply_float(_MainLightCustomFunction_13744cd490944daa89a8ca8ea82bf801_Color_2, (_MainLightCustomFunction_13744cd490944daa89a8ca8ea82bf801_ShadowAtten_4.xxx), _Multiply_a5ee958a43d6486c8adafb051e9f9094_Out_2);
            float3 _Normalize_1f2add49bcef4d38aca69e47723ca228_Out_1;
            Unity_Normalize_float3(-1 * mul((float3x3)UNITY_MATRIX_M, transpose(mul(UNITY_MATRIX_I_M, UNITY_MATRIX_I_V)) [2].xyz), _Normalize_1f2add49bcef4d38aca69e47723ca228_Out_1);
            float _DotProduct_0440c3f3fd044a749b3a938d67815af5_Out_2;
            Unity_DotProduct_float3(_MainLightCustomFunction_13744cd490944daa89a8ca8ea82bf801_Direction_0, _Normalize_1f2add49bcef4d38aca69e47723ca228_Out_1, _DotProduct_0440c3f3fd044a749b3a938d67815af5_Out_2);
            float _Saturate_94e73a0bcc7a4d4680346dd6b19a67bd_Out_1;
            Unity_Saturate_float(_DotProduct_0440c3f3fd044a749b3a938d67815af5_Out_2, _Saturate_94e73a0bcc7a4d4680346dd6b19a67bd_Out_1);
            float3 _Multiply_0720425b373e459b88a6cee46d6b07df_Out_2;
            Unity_Multiply_float(_Multiply_a5ee958a43d6486c8adafb051e9f9094_Out_2, (_Saturate_94e73a0bcc7a4d4680346dd6b19a67bd_Out_1.xxx), _Multiply_0720425b373e459b88a6cee46d6b07df_Out_2);
            float3 _Multiply_c699ae0d77424fb590424f54dc343760_Out_2;
            Unity_Multiply_float((_Property_bf6324ebb86f442ba9760ead28e9f8e6_Out_0.xyz), _Multiply_0720425b373e459b88a6cee46d6b07df_Out_2, _Multiply_c699ae0d77424fb590424f54dc343760_Out_2);
            float3 _Multiply_10edc177c328439db60721337094f9b1_Out_2;
            Unity_Multiply_float((_Lerp_3f9a5249281541e1be1f190753e33b9d_Out_3.xyz), _Multiply_c699ae0d77424fb590424f54dc343760_Out_2, _Multiply_10edc177c328439db60721337094f9b1_Out_2);
            UnityTexture2D _Property_a295afe4790e404dab7b599e53bee6fc_Out_0 = UnityBuildTexture2DStructNoScale(ThicknessTex);
            float4 _SampleTexture2D_76eba85b0f1b44d3b740b14c94eb642b_RGBA_0 = SAMPLE_TEXTURE2D(_Property_a295afe4790e404dab7b599e53bee6fc_Out_0.tex, _Property_a295afe4790e404dab7b599e53bee6fc_Out_0.samplerstate, IN.uv0.xy);
            float _SampleTexture2D_76eba85b0f1b44d3b740b14c94eb642b_R_4 = _SampleTexture2D_76eba85b0f1b44d3b740b14c94eb642b_RGBA_0.r;
            float _SampleTexture2D_76eba85b0f1b44d3b740b14c94eb642b_G_5 = _SampleTexture2D_76eba85b0f1b44d3b740b14c94eb642b_RGBA_0.g;
            float _SampleTexture2D_76eba85b0f1b44d3b740b14c94eb642b_B_6 = _SampleTexture2D_76eba85b0f1b44d3b740b14c94eb642b_RGBA_0.b;
            float _SampleTexture2D_76eba85b0f1b44d3b740b14c94eb642b_A_7 = _SampleTexture2D_76eba85b0f1b44d3b740b14c94eb642b_RGBA_0.a;
            float3 _Multiply_b49f1dc358b14c73ba3e39551f96c7f9_Out_2;
            Unity_Multiply_float(_Multiply_10edc177c328439db60721337094f9b1_Out_2, (_SampleTexture2D_76eba85b0f1b44d3b740b14c94eb642b_RGBA_0.xyz), _Multiply_b49f1dc358b14c73ba3e39551f96c7f9_Out_2);
            float _Property_7784b184630844a1bd70b6904d69c970_Out_0 = AlphaClip;
            surface.BaseColor = (_Lerp_3f9a5249281541e1be1f190753e33b9d_Out_3.xyz);
            surface.Emission = _Multiply_b49f1dc358b14c73ba3e39551f96c7f9_Out_2;
            surface.Alpha = _SampleTexture2D_361c1baa7e5e484ab38560dd3341b6d8_A_7;
            surface.AlphaClipThreshold = _Property_7784b184630844a1bd70b6904d69c970_Out_0;
            return surface;
        }

            // --------------------------------------------------
            // Build Graph Inputs

            VertexDescriptionInputs BuildVertexDescriptionInputs(Attributes input)
        {
            VertexDescriptionInputs output;
            ZERO_INITIALIZE(VertexDescriptionInputs, output);

            output.ObjectSpaceNormal =           input.normalOS;
            output.WorldSpaceNormal =            TransformObjectToWorldNormal(input.normalOS);
            output.ObjectSpaceTangent =          input.tangentOS.xyz;
            output.WorldSpaceTangent =           TransformObjectToWorldDir(input.tangentOS.xyz);
            output.ObjectSpaceBiTangent =        normalize(cross(input.normalOS, input.tangentOS) * (input.tangentOS.w > 0.0f ? 1.0f : -1.0f) * GetOddNegativeScale());
            output.WorldSpaceBiTangent =         TransformObjectToWorldDir(output.ObjectSpaceBiTangent);
            output.ObjectSpacePosition =         input.positionOS;
            output.AbsoluteWorldSpacePosition =  GetAbsolutePositionWS(TransformObjectToWorld(input.positionOS));
            output.TimeParameters =              _TimeParameters.xyz;

            return output;
        }
            SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
        {
            SurfaceDescriptionInputs output;
            ZERO_INITIALIZE(SurfaceDescriptionInputs, output);





            output.AbsoluteWorldSpacePosition =  GetAbsolutePositionWS(input.positionWS);
            output.uv0 =                         input.texCoord0;
        #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
        #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN output.FaceSign =                    IS_FRONT_VFACE(input.cullFace, true, false);
        #else
        #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
        #endif
        #undef BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN

            return output;
        }

            // --------------------------------------------------
            // Main

            #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/Varyings.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/LightingMetaPass.hlsl"

            ENDHLSL
        }
        Pass
        {
            // Name: <None>
            Tags
            {
                "LightMode" = "Universal2D"
            }

            // Render State
            Cull Off
        Blend One Zero
        ZTest LEqual
        ZWrite On

            // Debug
            // <None>

            // --------------------------------------------------
            // Pass

            HLSLPROGRAM

            // Pragmas
            #pragma target 2.0
        #pragma only_renderers gles gles3 glcore d3d11
        #pragma multi_compile_instancing
        #pragma vertex vert
        #pragma fragment frag

            // DotsInstancingOptions: <None>
            // HybridV1InjectedBuiltinProperties: <None>

            // Keywords
            // PassKeywords: <None>
            // GraphKeywords: <None>

            // Defines
            #define _AlphaClip 1
            #define _NORMALMAP 1
            #define _NORMAL_DROPOFF_TS 1
            #define ATTRIBUTES_NEED_NORMAL
            #define ATTRIBUTES_NEED_TANGENT
            #define ATTRIBUTES_NEED_TEXCOORD0
            #define VARYINGS_NEED_TEXCOORD0
            #define FEATURES_GRAPH_VERTEX
            /* WARNING: $splice Could not find named fragment 'PassInstancing' */
            #define SHADERPASS SHADERPASS_2D
            /* WARNING: $splice Could not find named fragment 'DotsInstancingVars' */

            // Includes
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
         //#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
                // Graph Properties
            CBUFFER_START(UnityPerMaterial)
        float4 MainTex_TexelSize;
        float4 MainColor;
        float4 VarColor;
        float4 ThicknessTex_TexelSize;
        float4 SSSColor;
        float4 NormalTex_TexelSize;
        float Normal_Intensity;
        float4 MaskTex_TexelSize;
        float Smoothness;
        float AlphaClip;
        float Wind_Speed;
        float Wind_Frequency;
        float Wind_Strength;
        float3 Wind_Direction;
        float _Specular;
        float _EnvReflection;
        CBUFFER_END

        #include ".\Assets\KSC_Assets\03_Shaders\Common\Lighting_KSC.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"

            // --------------------------------------------------
            // Structs and Packing

            struct Attributes
        {
            float3 positionOS : POSITION;
            float3 normalOS : NORMAL;
            float4 tangentOS : TANGENT;
            float4 uv0 : TEXCOORD0;
            #if UNITY_ANY_INSTANCING_ENABLED
            uint instanceID : INSTANCEID_SEMANTIC;
            #endif
        };
        struct Varyings
        {
            float4 positionCS : SV_POSITION;
            float4 texCoord0;
            #if UNITY_ANY_INSTANCING_ENABLED
            uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
            #endif
        };
        struct SurfaceDescriptionInputs
        {
            float4 uv0;
        };
        struct VertexDescriptionInputs
        {
            float3 ObjectSpaceNormal;
            float3 WorldSpaceNormal;
            float3 ObjectSpaceTangent;
            float3 WorldSpaceTangent;
            float3 ObjectSpaceBiTangent;
            float3 WorldSpaceBiTangent;
            float3 ObjectSpacePosition;
            float3 AbsoluteWorldSpacePosition;
            float3 TimeParameters;
        };
        struct PackedVaryings
        {
            float4 positionCS : SV_POSITION;
            float4 interp0 : TEXCOORD0;
            #if UNITY_ANY_INSTANCING_ENABLED
            uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
            #endif
        };

            PackedVaryings PackVaryings (Varyings input)
        {
            PackedVaryings output;
            output.positionCS = input.positionCS;
            output.interp0.xyzw =  input.texCoord0;
            #if UNITY_ANY_INSTANCING_ENABLED
            output.instanceID = input.instanceID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            output.cullFace = input.cullFace;
            #endif
            return output;
        }
        Varyings UnpackVaryings (PackedVaryings input)
        {
            Varyings output;
            output.positionCS = input.positionCS;
            output.texCoord0 = input.interp0.xyzw;
            #if UNITY_ANY_INSTANCING_ENABLED
            output.instanceID = input.instanceID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            output.cullFace = input.cullFace;
            #endif
            return output;
        }

            // --------------------------------------------------
            // Graph

            // Graph Properties
             /* CBUFFER_START(UnityPerMaterial)
        float4 MainTex_TexelSize;
        float4 MainColor;
        float4 VarColor;
        float4 ThicknessTex_TexelSize;
        float4 SSSColor;
        float4 NormalTex_TexelSize;
        float Normal_Intensity;
        float4 MaskTex_TexelSize;
        float Smoothness;
        float AlphaClip;
        float Wind_Speed;
        float Wind_Frequency;
        float Wind_Strength;
        float3 Wind_Direction;
        float _Specular;
        float _EnvReflection;
        CBUFFER_END*/

        // Object and Global properties
        SAMPLER(SamplerState_Linear_Repeat);
        TEXTURE2D(MainTex);
        SAMPLER(samplerMainTex);
        TEXTURE2D(ThicknessTex);
        SAMPLER(samplerThicknessTex);
        TEXTURE2D(NormalTex);
        SAMPLER(samplerNormalTex);
        TEXTURE2D(MaskTex);
        SAMPLER(samplerMaskTex);

            // Graph Functions
            
        void Unity_Distance_float3(float3 A, float3 B, out float Out)
        {
            Out = distance(A, B);
        }

        void Unity_Normalize_float3(float3 In, out float3 Out)
        {
            Out = normalize(In);
        }

        void Unity_Multiply_float(float A, float B, out float Out)
        {
            Out = A * B;
        }

        void Unity_Add_float(float A, float B, out float Out)
        {
            Out = A + B;
        }

        void Unity_Sine_float(float In, out float Out)
        {
            Out = sin(In);
        }

        void Unity_Multiply_float(float3 A, float3 B, out float3 Out)
        {
            Out = A * B;
        }

        void Unity_Add_float3(float3 A, float3 B, out float3 Out)
        {
            Out = A + B;
        }

        void Unity_Multiply_float(float4 A, float4 B, out float4 Out)
        {
            Out = A * B;
        }

        void Unity_Fraction_float(float In, out float Out)
        {
            Out = frac(In);
        }

        void Unity_Lerp_float4(float4 A, float4 B, float4 T, out float4 Out)
        {
            Out = lerp(A, B, T);
        }

            // Graph Vertex
            struct VertexDescription
        {
            float3 Position;
            float3 Normal;
            float3 Tangent;
        };

        VertexDescription VertexDescriptionFunction(VertexDescriptionInputs IN)
        {
            VertexDescription description = (VertexDescription)0;
            float _Distance_a9453349007d43d7aa8e8ef2f9e0a6cb_Out_2;
            Unity_Distance_float3(float3(0, 0, 0), IN.ObjectSpacePosition, _Distance_a9453349007d43d7aa8e8ef2f9e0a6cb_Out_2);
            float3 _Property_294994c591114f76808f84d55e0400b9_Out_0 = Wind_Direction;
            float3 _Normalize_7a91dcb4e52548df84db8cb487a5210f_Out_1;
            Unity_Normalize_float3(_Property_294994c591114f76808f84d55e0400b9_Out_0, _Normalize_7a91dcb4e52548df84db8cb487a5210f_Out_1);
            float _Property_b1049a414de54aaf954fc9acaf3ffde3_Out_0 = Wind_Speed;
            float _Multiply_8cc8be463d9b4875839e9776be38d510_Out_2;
            Unity_Multiply_float(IN.TimeParameters.x, _Property_b1049a414de54aaf954fc9acaf3ffde3_Out_0, _Multiply_8cc8be463d9b4875839e9776be38d510_Out_2);
            float _Property_1cc960ad477c47dfa058e2368a6a0e59_Out_0 = Wind_Frequency;
            float _Split_47b65f07839b45a2a7f9fd07f6ae4f85_R_1 = SHADERGRAPH_OBJECT_POSITION[0];
            float _Split_47b65f07839b45a2a7f9fd07f6ae4f85_G_2 = SHADERGRAPH_OBJECT_POSITION[1];
            float _Split_47b65f07839b45a2a7f9fd07f6ae4f85_B_3 = SHADERGRAPH_OBJECT_POSITION[2];
            float _Split_47b65f07839b45a2a7f9fd07f6ae4f85_A_4 = 0;
            float _Add_5ff2f4775c81475eaa4e1d2f1038221c_Out_2;
            Unity_Add_float(_Split_47b65f07839b45a2a7f9fd07f6ae4f85_R_1, _Split_47b65f07839b45a2a7f9fd07f6ae4f85_G_2, _Add_5ff2f4775c81475eaa4e1d2f1038221c_Out_2);
            float _Add_0b5c85958b4a44b4a1471bfb43d09937_Out_2;
            Unity_Add_float(_Add_5ff2f4775c81475eaa4e1d2f1038221c_Out_2, _Split_47b65f07839b45a2a7f9fd07f6ae4f85_G_2, _Add_0b5c85958b4a44b4a1471bfb43d09937_Out_2);
            float _Multiply_36b4b25245ef46a6aa344b72d18b123a_Out_2;
            Unity_Multiply_float(_Property_1cc960ad477c47dfa058e2368a6a0e59_Out_0, _Add_0b5c85958b4a44b4a1471bfb43d09937_Out_2, _Multiply_36b4b25245ef46a6aa344b72d18b123a_Out_2);
            float _Add_112d774bc523412da770f4e752e0aeae_Out_2;
            Unity_Add_float(_Multiply_8cc8be463d9b4875839e9776be38d510_Out_2, _Multiply_36b4b25245ef46a6aa344b72d18b123a_Out_2, _Add_112d774bc523412da770f4e752e0aeae_Out_2);
            float _Sine_703274f04df4470980c37a4a595551c8_Out_1;
            Unity_Sine_float(_Add_112d774bc523412da770f4e752e0aeae_Out_2, _Sine_703274f04df4470980c37a4a595551c8_Out_1);
            float _Multiply_b11879df31064a879f94ea423ab2b2a5_Out_2;
            Unity_Multiply_float(_Sine_703274f04df4470980c37a4a595551c8_Out_1, 0.5, _Multiply_b11879df31064a879f94ea423ab2b2a5_Out_2);
            float _Add_efce9d790e58416f86a8ec3243204620_Out_2;
            Unity_Add_float(_Multiply_b11879df31064a879f94ea423ab2b2a5_Out_2, 0.5, _Add_efce9d790e58416f86a8ec3243204620_Out_2);
            float _Property_79b44419d559467d899224e854ea3824_Out_0 = Wind_Strength;
            float _Multiply_47956fa9e0804e8381e9c610915da0e6_Out_2;
            Unity_Multiply_float(_Add_efce9d790e58416f86a8ec3243204620_Out_2, _Property_79b44419d559467d899224e854ea3824_Out_0, _Multiply_47956fa9e0804e8381e9c610915da0e6_Out_2);
            float3 _Multiply_448ee0f8bc4f41a8bcf38fdc2b440495_Out_2;
            Unity_Multiply_float(_Normalize_7a91dcb4e52548df84db8cb487a5210f_Out_1, (_Multiply_47956fa9e0804e8381e9c610915da0e6_Out_2.xxx), _Multiply_448ee0f8bc4f41a8bcf38fdc2b440495_Out_2);
            float3 _Multiply_4e49ed416c034b90b06a8a56b2d0a49a_Out_2;
            Unity_Multiply_float((_Distance_a9453349007d43d7aa8e8ef2f9e0a6cb_Out_2.xxx), _Multiply_448ee0f8bc4f41a8bcf38fdc2b440495_Out_2, _Multiply_4e49ed416c034b90b06a8a56b2d0a49a_Out_2);
            float3 _Add_c136859739784517b3f510aa5bb4e469_Out_2;
            Unity_Add_float3(_Multiply_4e49ed416c034b90b06a8a56b2d0a49a_Out_2, IN.AbsoluteWorldSpacePosition, _Add_c136859739784517b3f510aa5bb4e469_Out_2);
            float3 _Transform_47fd3d4cb64f4d52b2077aaa7435e77b_Out_1 = TransformWorldToObject(_Add_c136859739784517b3f510aa5bb4e469_Out_2.xyz);
            description.Position = _Transform_47fd3d4cb64f4d52b2077aaa7435e77b_Out_1;
            description.Normal = IN.ObjectSpaceNormal;
            description.Tangent = IN.ObjectSpaceTangent;
            return description;
        }

            // Graph Pixel
            struct SurfaceDescription
        {
            float3 BaseColor;
            float Alpha;
            float AlphaClipThreshold;
        };

        SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
        {
            SurfaceDescription surface = (SurfaceDescription)0;
            float4 _Property_1931a4995a954dc59a2a81efc2ac7bc3_Out_0 = MainColor;
            UnityTexture2D _Property_aed0c1bacf314879a280662567e38a31_Out_0 = UnityBuildTexture2DStructNoScale(MainTex);
            float4 _SampleTexture2D_361c1baa7e5e484ab38560dd3341b6d8_RGBA_0 = SAMPLE_TEXTURE2D(_Property_aed0c1bacf314879a280662567e38a31_Out_0.tex, _Property_aed0c1bacf314879a280662567e38a31_Out_0.samplerstate, IN.uv0.xy);
            float _SampleTexture2D_361c1baa7e5e484ab38560dd3341b6d8_R_4 = _SampleTexture2D_361c1baa7e5e484ab38560dd3341b6d8_RGBA_0.r;
            float _SampleTexture2D_361c1baa7e5e484ab38560dd3341b6d8_G_5 = _SampleTexture2D_361c1baa7e5e484ab38560dd3341b6d8_RGBA_0.g;
            float _SampleTexture2D_361c1baa7e5e484ab38560dd3341b6d8_B_6 = _SampleTexture2D_361c1baa7e5e484ab38560dd3341b6d8_RGBA_0.b;
            float _SampleTexture2D_361c1baa7e5e484ab38560dd3341b6d8_A_7 = _SampleTexture2D_361c1baa7e5e484ab38560dd3341b6d8_RGBA_0.a;
            float4 _Multiply_72a5e6242399449aad550d9798747548_Out_2;
            Unity_Multiply_float(_Property_1931a4995a954dc59a2a81efc2ac7bc3_Out_0, _SampleTexture2D_361c1baa7e5e484ab38560dd3341b6d8_RGBA_0, _Multiply_72a5e6242399449aad550d9798747548_Out_2);
            float4 _Property_2e6c583a57424a8180974b05f7e49119_Out_0 = VarColor;
            float4 _Multiply_c5a7db016b7b4a19a5d1eb518bbd1ebe_Out_2;
            Unity_Multiply_float(_Property_2e6c583a57424a8180974b05f7e49119_Out_0, _SampleTexture2D_361c1baa7e5e484ab38560dd3341b6d8_RGBA_0, _Multiply_c5a7db016b7b4a19a5d1eb518bbd1ebe_Out_2);
            float _Split_47b65f07839b45a2a7f9fd07f6ae4f85_R_1 = SHADERGRAPH_OBJECT_POSITION[0];
            float _Split_47b65f07839b45a2a7f9fd07f6ae4f85_G_2 = SHADERGRAPH_OBJECT_POSITION[1];
            float _Split_47b65f07839b45a2a7f9fd07f6ae4f85_B_3 = SHADERGRAPH_OBJECT_POSITION[2];
            float _Split_47b65f07839b45a2a7f9fd07f6ae4f85_A_4 = 0;
            float _Add_5ff2f4775c81475eaa4e1d2f1038221c_Out_2;
            Unity_Add_float(_Split_47b65f07839b45a2a7f9fd07f6ae4f85_R_1, _Split_47b65f07839b45a2a7f9fd07f6ae4f85_G_2, _Add_5ff2f4775c81475eaa4e1d2f1038221c_Out_2);
            float _Add_0b5c85958b4a44b4a1471bfb43d09937_Out_2;
            Unity_Add_float(_Add_5ff2f4775c81475eaa4e1d2f1038221c_Out_2, _Split_47b65f07839b45a2a7f9fd07f6ae4f85_G_2, _Add_0b5c85958b4a44b4a1471bfb43d09937_Out_2);
            float _Fraction_e6c8ad87e4c042d9a9a488035ab25129_Out_1;
            Unity_Fraction_float(_Add_0b5c85958b4a44b4a1471bfb43d09937_Out_2, _Fraction_e6c8ad87e4c042d9a9a488035ab25129_Out_1);
            float4 _Lerp_3f9a5249281541e1be1f190753e33b9d_Out_3;
            Unity_Lerp_float4(_Multiply_72a5e6242399449aad550d9798747548_Out_2, _Multiply_c5a7db016b7b4a19a5d1eb518bbd1ebe_Out_2, (_Fraction_e6c8ad87e4c042d9a9a488035ab25129_Out_1.xxxx), _Lerp_3f9a5249281541e1be1f190753e33b9d_Out_3);
            float _Property_7784b184630844a1bd70b6904d69c970_Out_0 = AlphaClip;
            surface.BaseColor = (_Lerp_3f9a5249281541e1be1f190753e33b9d_Out_3.xyz);
            surface.Alpha = _SampleTexture2D_361c1baa7e5e484ab38560dd3341b6d8_A_7;
            surface.AlphaClipThreshold = _Property_7784b184630844a1bd70b6904d69c970_Out_0;
            return surface;
        }

            // --------------------------------------------------
            // Build Graph Inputs

            VertexDescriptionInputs BuildVertexDescriptionInputs(Attributes input)
        {
            VertexDescriptionInputs output;
            ZERO_INITIALIZE(VertexDescriptionInputs, output);

            output.ObjectSpaceNormal =           input.normalOS;
            output.WorldSpaceNormal =            TransformObjectToWorldNormal(input.normalOS);
            output.ObjectSpaceTangent =          input.tangentOS.xyz;
            output.WorldSpaceTangent =           TransformObjectToWorldDir(input.tangentOS.xyz);
            output.ObjectSpaceBiTangent =        normalize(cross(input.normalOS, input.tangentOS) * (input.tangentOS.w > 0.0f ? 1.0f : -1.0f) * GetOddNegativeScale());
            output.WorldSpaceBiTangent =         TransformObjectToWorldDir(output.ObjectSpaceBiTangent);
            output.ObjectSpacePosition =         input.positionOS;
            output.AbsoluteWorldSpacePosition =  GetAbsolutePositionWS(TransformObjectToWorld(input.positionOS));
            output.TimeParameters =              _TimeParameters.xyz;

            return output;
        }
            SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
        {
            SurfaceDescriptionInputs output;
            ZERO_INITIALIZE(SurfaceDescriptionInputs, output);





            output.uv0 =                         input.texCoord0;
        #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
        #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN output.FaceSign =                    IS_FRONT_VFACE(input.cullFace, true, false);
        #else
        #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
        #endif
        #undef BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN

            return output;
        }

            // --------------------------------------------------
            // Main

            #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/Varyings.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/PBR2DPass.hlsl"

            ENDHLSL
        }
    }
    CustomEditorForRenderPipeline "ShaderGraph.PBRMasterGUI" "UnityEngine.Rendering.Universal.UniversalRenderPipelineAsset"
    FallBack "Hidden/Shader Graph/FallbackError"
}