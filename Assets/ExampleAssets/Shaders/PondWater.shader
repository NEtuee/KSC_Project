Shader "Environment/PondWater"
{
    Properties
    { 

        [HDR]_WaterColor ("Water Main Color", Color) = (1,1,1,1)
        [HDR]_SideWaterColor ("Side Water Color", Color) = (1,1,1,1)
        [Space][Space][Space][Space]

        _DepthRange("DepthRange",Range(-2,10)) = 0
        _DepthHardness("Depth Edge Softness", Range(1,20)) = 1

        [Space][Space][Space][Space]
         _MainTex ("Edge Texture", 2D) = "white"{}
         _EdgeNoiseContrast ("Edge Noise ConTrast", Range(0,20)) = 1


        [Space]
        _WaterWaveSpeed ("Water Wave Speed", Range(-20,20)) = 0.1
        _WaterWaveHeight ("Water Wave Height", Range(0,2)) = 1.0
        _WaterFresnel ("Water Fresnel", Range(1,30)) = 10
        _WaterFresnelOffset ("_WaterFresnel Offset", Range(0,10)) = 1
    
        [Space][Space][Space][Space]

        [NORMAL]_BumpTex1 ("Water Bump", 2D) = "bump"{}
        _BumpIntensity ("Bump Intensity", Range(0,3)) = 1.0
        _Bumpspeed_x ("Bump x Speed", Range(-2,2)) = 0.1
        _Bumpspeed_y ("Bump y Speed", Range(-2,2)) = 0.1

        [Space][Space][Space][Space]
        [NORMAL]_BumpTex2("Water Bump2", 2D) = "bump"{}
         _Bump2Intensity ("Bump2 Intensity", Range(0,3)) = 1.0
        _Bump2speed_x("Bump x Speed", Range(-2,2)) = 0.1
        _Bump2speed_y("Bump y Speed", Range(-2,2)) = 0.1

        [Space][Space][Space][Space]
        _SpecularTex ("SpecularTexutre", 2D) = "white" {}
        _SpecularIntensity ("Specular Intensity", Range(0,5)) = 0.5
        _SpecularLevel ("Specular Power", Range(0,200)) = 20

     

        [Space][Space][Space][Space]
        [CUBE]_CubeTex ("CUBE Reflection", Cube) = "CUBE"{}
        _CubeMapIntenSity ("CUBE map Intensity", Range(0,3)) = 1
        _reflectamount ("Reflect Amount", Range (0,1)) = 0.5

    }

    SubShader
    {

        Tags { "RenderType" = "Transparent" "RenderPipeline" = "UniversalRenderPipeline" "Queue" = "Transparent"}

        Pass
        {   
            
            Blend SrcAlpha OneMinusSrcAlpha // 전통적인 알파 블랜드, 소스의 알파 + (1 - 소스의 알파)
            
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"            


            struct Attributes
            {   
                float2 uv : TEXCOORD0;
                float2 uv2 : TEXCOORD1;
                float2 uv3 : TEXCOORD2;
                float2 uv4 : TEXCOORD3;
                float2 uv5 : TEXCOORD4;
                float3 mNormal : NORMAL;
                float4 positionOS   : POSITION;
                float4 tangent : TANGENT;

            };

            struct Varyings
            {
                float2 uv : TEXCOORD0;
                float2 uv2 : TEXCOORD1;
                float2 uv3 : TEXCOORD2;
                float2 uv4 : TEXCOORD10;
                float2 uv5 : TEXCOORD11;
                float3 mNormal : NORMAL;

                float4 screenPos : TEXCOORD3;
                float3 lightDir : TEXCOORD4;
                float3 viewDir : TEXCOORD5;

                float4 positionHCS  : SV_POSITION;
                float4 PositionWS : TEXCOORD6;

                //TBN 
                float3 T : TEXCOORD7;
                float3 B : TEXCOORD8;
                float3 N : TEXCOORD9;

            

            };            

            float4 _WaterColor;
            float4 _SideWaterColor;
            
            TEXTURE2D_X_FLOAT(_CameraDepthTexture); 
            SAMPLER(sampler_CameraDepthTexture); // 뎁스 텍스처




            float _DepthRange;
            float _DepthHardness;
            float _WaterFresnel;
            float _WaterFresnelOffset;
            float _EdgeNoiseContrast;
            float _reflectamount;

            float _SpecularIntensity;
            float _BumpIntensity;
            float _Bumpspeed_x;
            float _Bumpspeed_y;

            float _Bump2Intensity;
            float _Bump2speed_x;
            float _Bump2speed_y;

            float _CubeMapIntenSity;
            float _SpecularLevel;
      
            float _WaterWaveSpeed;
            float _WaterWaveHeight;
         
            float4 _MainTex_ST;
            float4 _SpecularTex_ST;
            float4 _BumpTex1_ST;
            float4 _BumpTex2_ST;

            
            Texture2D _MainTex;
            Texture2D _SpecularTex;
            Texture2D _BumpTex1;
            Texture2D _BumpTex2;
            TextureCube _CubeTex;


            
            SamplerState sampler_MainTex;
            SamplerState sampler_SpecularTex;
            SamplerState sampler_CubeTex;
        
            Varyings vert(Attributes i)
            {
               
                Varyings OUT;
                OUT.uv = i.uv; /// albedo
                OUT.uv2 = i.uv2; // spec
                OUT.uv2 = i.uv2; // spec
                OUT.uv4 = i.uv4; // normal
                OUT.uv5 = i.uv5; // normal2


                OUT.positionHCS = TransformObjectToHClip(i.positionOS.xyz);

                //Vertex Animation 
                OUT.positionHCS.y += (sin(i.positionOS.x + i.positionOS.z + _Time  *_WaterWaveSpeed)) * _WaterWaveHeight; 
            
                OUT.PositionWS = mul(unity_ObjectToWorld,i.positionOS);

                OUT.screenPos = OUT.positionHCS;

                OUT.lightDir = normalize(_MainLightPosition.xyz);
                OUT.viewDir = (_WorldSpaceCameraPos) - (OUT.PositionWS);
                OUT.mNormal = mul(unity_ObjectToWorld,i.mNormal);

                //TBN
                float fTangentSign = i.tangent.w * unity_WorldTransformParams.w; 
                
                OUT.T = normalize(mul(unity_ObjectToWorld,i.tangent.xyz));
                OUT.N = normalize(OUT.mNormal);
                OUT.B = normalize(cross(OUT.N,OUT.T) * fTangentSign);

                
            


                return OUT;
            }

            half4 frag(Varyings i) : SV_Target
            {   
                // WS normal
                float3 N = normalize(i.mNormal);

                // make LightDir  
                float3 L = normalize(i.lightDir);

                // make ViewDir
                float3 V = normalize(i.viewDir);

             

                // UV
                float2 uv = i.uv.xy * _MainTex_ST.xy + _MainTex_ST.zw;
                float2 uv2 = i.uv2.xy * (_SpecularTex_ST.xy)+ (_SpecularTex_ST.zw);

                float2 uv4 = i.uv4.xy * _BumpTex1_ST.xy + _BumpTex1_ST.zw;
                float2 uv5 = i.uv5.xy * _BumpTex2_ST.xy + _BumpTex2_ST.zx;

                
                float Scrollx = _Time * _Bumpspeed_x;
                float Scrolly = _Time * _Bumpspeed_y;

                float Scrollx2 = _Time * _Bump2speed_x;
                float Scrolly2 = _Time * _Bump2speed_y;
             

                // 텍스처와 컬러 조절
                float4 FoamNoiseTex = saturate((_MainTex.Sample(sampler_MainTex, float2(uv.x + Scrollx, uv.y +Scrolly))));
                //FoamNoiseTex = saturate(pow(FoamNoiseTex,_EdgeNoiseContrast));
                float4 SpecularTex = _SpecularTex.Sample(sampler_SpecularTex, uv2);
            
               
                // TBN 행렬로 월드 포지션 노말맵 제작 
                float3 T = normalize(i.T);
                float3 B = normalize(i.B);
                

                
                float3x3 TBN = float3x3(T,B,N);
                TBN = transpose(TBN);

                
                float3 n1 = UnpackNormal(_BumpTex1.Sample(sampler_MainTex, float2(uv4.x + Scrollx, uv4.y + Scrolly)));
                float3 n2 = UnpackNormal(_BumpTex2.Sample(sampler_MainTex, float2(uv5.x + Scrollx2, uv5.y +  Scrolly2)));

                n1 *= _BumpIntensity;
                n2 *= _Bump2Intensity;

                float3 T2WN = (n1 + n2)/2;

                float3 worldNormal = mul(TBN,T2WN);
                

                float3 Wnormal = normalize(worldNormal);

                // Half Vector
                float3 H =  normalize(V + L);
                float nH = max(0,dot(Wnormal,H));


                // 스크린 포지션
                float4 screenPos = ComputeScreenPos(i.screenPos);
                float4 scrPos = normalize(screenPos);
                scrPos = float4 (scrPos.xyz,scrPos.w + 0.00000001);
                float4 screenPosNormalize = (scrPos/scrPos.w);

                

                //Depth 계산
                float4 objDepth = LinearEyeDepth(screenPosNormalize.z, _ZBufferParams);
                float SceneDepth = LinearEyeDepth(SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, sampler_CameraDepthTexture,scrPos.xy/scrPos.w),_ZBufferParams);




                // 실제 물 외곽선을 검출하는 과정 + 조절
                float Depth = clamp((SceneDepth - (objDepth +_DepthRange)),0,1);
                Depth = saturate(pow(Depth,_DepthHardness));



                // Foam Line 
                float4 FoamLine = saturate(((Depth + FoamNoiseTex) * (Depth)) *  _EdgeNoiseContrast);

                // UnderWater Fog
                

                 // 큐브맵 반사
                float3 viewReflect = normalize(reflect(V,Wnormal).xyz);
                float4 Environment = _CubeTex.Sample(sampler_CubeTex,viewReflect);
                Environment = saturate((Environment) * _CubeMapIntenSity);
              
                // 블린 퐁 스펙큘러
                float spec = clamp((pow(nH, _SpecularLevel) * SpecularTex),0,1) * _SpecularIntensity;
                
                 // 프레넬 효과
                float VdotN = saturate(dot(V,Wnormal) + _WaterFresnelOffset);
                VdotN =  saturate(pow(VdotN,_WaterFresnel));

                spec = spec * (1- VdotN);

                // 움직임에 반응하는 물 표현을 위한 부분
               
               Environment *= (1 - VdotN);

                float4 col = lerp(_SideWaterColor,_WaterColor,FoamLine);
                float4 Final = lerp(col,Environment,_reflectamount);

                return  Final + spec;
            }
            ENDHLSL
        }
    }
}