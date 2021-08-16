Shader "KSC/VFX/Hologram"
{

    Properties
    {
        _BaseMap("Base Map", 2D) = "white"
    }

    SubShader
    {
        Tags { "RenderType" = "Trasnparent" "RenderPipeline" = "UniversalPipeline" "Queue" = "3000+1"}

        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/UnityInput.hlsl"

            struct Attributes
            {
                float4 positionOS   : POSITION;
           
                float2 uv           : TEXCOORD0;
                float3 normal : NORMAL0;
            };

            struct Varyings
            {
                float4 positionWS  : SV_POSITION;

                float2 uv           : TEXCOORD0;
                float2 uv2          : TEXCOORD1;
                float3 uv3          : TEXCOORD2;
                float3 normal       : NORMAL0;
                float3 viewDirWS    : TEXCOORD3;
                
                
            };

            TEXTURE2D(_BaseMap);
            SAMPLER(sampler_BaseMap);
    /// // C버퍼어어어~~~!! 
            CBUFFER_START(UnityPerMaterial)
                float4 _BaseMap_ST;
            CBUFFER_END

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                OUT.positionWS = float4(TransformObjectToWorld(IN.positionOS.xyz),1);
               // OUT.PositionWS =mul(unity_ObjectToWorld,(IN.positionOS));
               // OUT.worldPos = TransformObjectToWorldNormal(float4(IN.positionOS.xyz,1));
                OUT.normal = TransformObjectToWorldNormal(IN.normal);

                //
                //OUT.viewDirWS = 

                OUT.uv = TRANSFORM_TEX(IN.uv, _BaseMap);
                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target
            {   
                float3 wNormal = normalize(IN.normal);

                // The SAMPLE_TEXTURE2D marco samples the texture with the given
                // sampler.
                half4 color = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, IN.uv);
                return color;
            }
            ENDHLSL
        }
    }
}