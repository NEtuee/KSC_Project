
Shader "KSC/HubDisplay"
{
    
    Properties
    { 
        _MainTex ("Use HDR Map", CUBE) = "White" {}
        [HDR]_MainColor ("Display Color", Color) = (1,1,1,1)



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
                float4 uv : TEXCOORD0;             
            };

            struct Varyings
            {
                // The positions in this struct must have the SV_POSITION semantic.
                float4 positionHCS  : SV_POSITION;
                
                float4 uv : TEXCOORD0;
            };            


            float4 _MainTex_ST;

            float4 _MainColor;
            samplerCUBE _MainTex;
            

            Varyings vert(Attributes IN)
            {
                
                Varyings OUT;
                OUT.uv = IN.uv;
                OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);
               
                return OUT;
            }

         
            half4 frag(Varyings v) : SV_Target
            {   
                float4 uv = float4(v.uv * _MainTex_ST.xy , v.uv + _MainTex_ST.zw);
                float4 CubeMap = texCUBE(_MainTex,uv);
  
                
                return _MainColor * CubeMap;
            }
            ENDHLSL
        }
    }
}