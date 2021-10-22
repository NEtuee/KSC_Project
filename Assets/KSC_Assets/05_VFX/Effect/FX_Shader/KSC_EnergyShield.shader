Shader "KSC/KSC_EnergyShield"
{

    Properties

    {
        _BaseColor ("Base Color", Color ) = (1,1,1,1)
        _Hitpos ("Collision Point", Vector) = (0.0,0.0,0.0,0.0)
        _GradientSize ("Gradient Size", float ) = 1.0


    }


    SubShader
    {
        
        Tags { "RenderType" = "transparent" "RenderPipeline" = "UniversalRenderPipeline" "Queue" = "transparent"}

        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
           
            HLSLPROGRAM
            
            #pragma vertex vert
            #pragma fragment frag

           
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"            

        
            struct Attributes
            {
                float4 vertex   : POSITION;              
                float3 normal : NORMAL;
            };

            struct Varyings
            {
                float4 vertex  : SV_POSITION;
                float3 NormalDir : TEXCOORD0;
                float4 ObjPos : TEXCOORD1;
            };            



            float4 _BaseColor;
            float4 _Hitpos;

            float _GradientSize;



            Varyings vert(Attributes v)
            {
   
                Varyings o;

                o.vertex = TransformObjectToHClip(v.vertex.xyz);
                o.NormalDir = v.normal;

                o.ObjPos = v.vertex;

                return o;
            }

               
            half4 frag(Varyings i) : SV_Target
            {
                
                
                float Gradient =  0;

                float Dist = distance(_Hitpos.xyz , i.ObjPos.xyz);
                Gradient += saturate( (1- Dist * _GradientSize) * _Hitpos.w);

                float3 col = Gradient * _BaseColor.rgb; 

                return float4(col , 1 );
            }
            ENDHLSL
        }
    }
}