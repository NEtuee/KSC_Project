
Shader "KSC/HubDisplay"
{
    
    Properties
    { 
        _DisplayTex ("DisplayTex", 2D) = "White" {} // HDR 
        [HDR]_MainColor ("Display Color", Color) = (1,1,1,1)
        
        _RimNoiseTex ("Rim Noise Texture", 2D) = "Black" {} // Rimlight pixel Texture
        _RimRange ("RimLight Range", float) = 0.1
        [HDR] _RimColor ("RimLight Color", Color) = (1,1,1,1)

        

        [Space][Space]
        _GlitchTex ("Glitch Tex", 2D) = "Black" {} // 화면 글리치 텍스처 
        _Glitch ("Glitch Amount", Range(0,1)) = 0.1
        _GlitchSpeed ("Glitch Speed", Range (0,1)) = 0.1

        _LineTex ("Line Tex", 2D) = "White" {} // 라인 지나가기 
        _LineSpeed ("Line Speed", Range(-2,2)) = 1 
       [HDR] _LineColor ("Line Color", Color) = (1,1,1,1)


    }

    SubShader
    {
      
        Tags { "RenderType" = "Opaque" "RenderPipeline" = "UniversalRenderPipeline" }

        Pass
        {
         
            HLSLPROGRAM
          
            #pragma vertex vert
         
            #pragma fragment frag


            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"            



            struct Attributes
            {
               
                float4 positionOS   : POSITION;
                float3 Normal : NORMAL;
                float4 uv : TEXCOORD0;   
                float4 color : COLOR;          
            };

            struct Varyings
            {
                // The positions in this struct must have the SV_POSITION semantic.
                float4 positionHCS  : SV_POSITION;
                float4 uv : TEXCOORD0;
                
                float3 positionWS : TEXCOORD1;
                float3 NormalWS : TEXCOORD2;
                float3 viewDir : TEXCOORD3;
                float4 color : COLOR;   

            };            

            sampler2D _DisplayTex;
            sampler2D _RimNoiseTex;
            sampler2D _LineTex;
            sampler2D _GlitchTex;
         

            float4 _DisplayTex_ST;
            float4 _RimNoiseTex_ST;
            float4 _LineTex_ST;
            float4 _GlitchTex_ST;

            float4 _MainColor;
            float4 _RimColor;
            float4 _LineColor;

            float _LineSpeed;
            float _RimRange;
            float _Glitch;
            float _GlitchSpeed;
            

            Varyings vert(Attributes IN)
            {
                
                Varyings OUT;
                OUT.uv = IN.uv;
                OUT.NormalWS = TransformObjectToWorldNormal(IN.Normal);
                OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);
                OUT.positionWS = TransformObjectToWorld(IN.positionOS.xyz);

                OUT.viewDir = _WorldSpaceCameraPos.xyz - TransformObjectToWorld(IN.positionOS);

                OUT.color = float4 (IN.color.rgb, IN.color.a);
                return OUT;
            }


            float4 TriPlanar (sampler2D s,float3 posWS, float3 NormalWS,float2 t) 
            {
              
                float2 topuv = float2( posWS.x,posWS.z);
                float2 sideuv = float2( posWS.z,posWS.y);
                float2 frontuv = float2( posWS.x,posWS.y);

                float4 topTex = tex2D(s,topuv * t);
                
                float4 sideTex = tex2D(s,sideuv * t);

                float4 frontTex = tex2D(s,frontuv * t);

                float4 lerp1 = lerp(topTex,frontTex, abs(NormalWS.z));
                float4 lerp2 = lerp(lerp1,sideTex,abs(NormalWS.x));
                
                return lerp2;

            }

            half4 frag(Varyings v) : SV_Target
            {       
                // Position WS
                float3 posWS = v.positionWS;
                float3 NormalWS = normalize(v.NormalWS);
                float3 viewDir = normalize(v.viewDir);

                float4 vcolor = v.color;
                

                float4 WSnormaluv = float4( (NormalWS * 0.5 + 0.5),1);

               
                


               
               //Glitch
                float2 GlitchTime = float2(_Time.y,0.5);
                float4 GlitchTex = (tex2D(_GlitchTex , GlitchTime *_Time.y * _GlitchSpeed) * _Glitch);
                float4 GlitchUV = TriPlanar(_GlitchTex,posWS,NormalWS,float2(GlitchTex.xy * _GlitchTex_ST.xy + _GlitchTex_ST.zw));
                
                
                //Rim Light

                float VdotN = saturate(1- (dot(NormalWS,viewDir)));
                float4 Rimtex = tex2D (_RimNoiseTex, posWS * _RimNoiseTex_ST.xy + _RimNoiseTex_ST.zw);
                float4 Rim = saturate(pow(VdotN,_RimRange)) * Rimtex * _RimColor;//tex2D(_RimNoiseTex,uv * _RimNoiseTex_ST.xy + _RimNoiseTex_ST.zw) * _RimColor;
                float4 HDRmap = tex2D(_DisplayTex ,WSnormaluv * _DisplayTex_ST.xy + _DisplayTex_ST.zw);

               // float4 RGBScreen = tex2D (_ScreenTex, float2((posWS + GlitchUV) * _ScreenTex_ST.xy + _ScreenTex_ST.zw));
               float4 RGBScreen = GlitchUV;//TriPlanar(_GlitchTex,posWS,NormalWS,float2((GlitchUV * _GlitchTex_ST.xy + _GlitchTex_ST.zw)));
               
                //Display Line
                float4 DisplayLine = tex2D(_LineTex, posWS * _LineTex_ST.xy + _LineTex_ST.z + (_LineTex_ST.w + _Time * _LineSpeed)) * _LineColor;

                
                

                float4 OldNoise = (_MainColor * HDRmap + (Rim * HDRmap)) + (saturate(pow(VdotN,_RimRange)) * DisplayLine);

                float4 mix1 = lerp(OldNoise, GlitchUV ,vcolor.r);
                float4 mix2 = lerp(mix1, 0, vcolor.g);
                return mix2;
            }
            ENDHLSL
        }
    }
}