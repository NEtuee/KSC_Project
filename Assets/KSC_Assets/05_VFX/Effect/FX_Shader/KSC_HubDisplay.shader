
Shader "KSC/HubDisplay"
{
    
    Properties
    { 
        _DisplayTex ("DisplayTex", 2D) = "White" {}
        [HDR]_MainColor ("Display Color", Color) = (1,1,1,1)
        
        _RimNoiseTex ("Rim Noise Texture", 2D) = "Black" {}
        _RimRange ("RimLight Range", float) = 0.1
        [HDR] _RimColor ("RimLight Color", Color) = (1,1,1,1)

        _LineTex ("Line Tex", 2D) = "White" {}
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
            };

            struct Varyings
            {
                // The positions in this struct must have the SV_POSITION semantic.
                float4 positionHCS  : SV_POSITION;
                float4 uv : TEXCOORD0;
                
                float3 positionWS : TEXCOORD1;
                float3 NormalWS : TEXCOORD2;
                float3 viewDir : TEXCOORD3;

            };            

            sampler2D _DisplayTex;
            sampler2D _RimNoiseTex;
            sampler2D _LineTex;

            float4 _DisplayTex_ST;
            float4 _RimNoiseTex_ST;
            float4 _LineTex_ST;

            float4 _MainColor;
            float4 _RimColor;
            float4 _LineColor;

            float _LineSpeed;
            float _RimRange;
            

            Varyings vert(Attributes IN)
            {
                
                Varyings OUT;
                OUT.uv = IN.uv;
                OUT.NormalWS = TransformObjectToWorldNormal(IN.Normal);
                OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);
                OUT.positionWS = TransformObjectToWorld(IN.positionOS);

                OUT.viewDir = _WorldSpaceCameraPos.xyz - TransformObjectToWorld(IN.positionOS);
                return OUT;
            }

         
            half4 frag(Varyings v) : SV_Target
            {       
                // Position WS
                float3 posWS = v.positionWS;
                float3 NormalWS = normalize(v.NormalWS);
                float3 viewDir = normalize(v.viewDir);
                

                float4 WSnormaluv = float4( (NormalWS * 0.5 + 0.5),1);
                float2 RimUV = posWS;
                //float2 LineUV = float2(posWS.x, posWS.y + _Time * _LineSpeed);

                //Rim Light

                float VdotN = saturate(1- (dot(NormalWS,viewDir)));
                float4 Rimtex = tex2D (_RimNoiseTex,RimUV  * _RimNoiseTex_ST.xy + _RimNoiseTex_ST.zw );
                float4 Rim = saturate(pow(VdotN,_RimRange)) * Rimtex * _RimColor;//tex2D(_RimNoiseTex,uv * _RimNoiseTex_ST.xy + _RimNoiseTex_ST.zw) * _RimColor;
                float4 HDRmap = tex2D(_DisplayTex ,WSnormaluv * _DisplayTex_ST.xy + _DisplayTex_ST.zw);

                //Display Line
                float4 DisplayLine = tex2D(_LineTex, RimUV * _LineTex_ST.xy + _LineTex_ST.z + (_LineTex_ST.w + _Time * _LineSpeed)) * _LineColor;

                float4 OldNoise =  Rim + (saturate(pow(VdotN,_RimRange)) * DisplayLine);
                
                return _MainColor * HDRmap + OldNoise;
            }
            ENDHLSL
        }
    }
}