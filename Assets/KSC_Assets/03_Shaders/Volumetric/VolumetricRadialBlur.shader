Shader "Hidden/VolumetricRadialBlur"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}

        _BlurWidth("Blur Width", Range(0,1)) = 0.85
        _Intensity("Intensity", Range(0,1)) = 1
        _Center("Center", Vector) = (0.5,0.5,0,0)

        _Color ("Color", Color) = (0,0,0,1)

    }
    SubShader
    {
       Tags { "RenderType" = "Opaque" "RenderPipeline" = "UniversalRenderPipeline" }
       Blend One One

       

        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

             #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"


             #define NUM_SAMPLES 50
             
            struct Attributes
            {
                float4 positionOS   : POSITION; 
                float2 uv : TEXCOORD0;
            };

            struct Varyings
            {
                float2 uv : TEXCOORD0;
                float4 positionHCS  : SV_POSITION;
            };

            Varyings vert (Attributes IN)
            {
                Varyings OUT;
                OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);
                OUT.uv = IN.uv;
                return OUT;
            }

            sampler2D _MainTex;

        
            float _BlurWidth;
            float _Intensity;
            float4 _Center;
            float4 _Color;
            float4 frag (Varyings i) : SV_Target
            {
               
                float4 color = float4(0,0,0,1);
                //2
                float2 ray = i.uv - _Center.xy;

                //3
                for (int i = 0; i < NUM_SAMPLES; i++)
                {
                    float scale = 1.0f - _BlurWidth * (float(i) / float(NUM_SAMPLES - 1));
                    color.xyz += tex2D(_MainTex, (ray * scale) + _Center.xy).xyz / float(NUM_SAMPLES);
                }

                //4
                return color * _Intensity;
            }
            ENDHLSL
        }
    }
}
