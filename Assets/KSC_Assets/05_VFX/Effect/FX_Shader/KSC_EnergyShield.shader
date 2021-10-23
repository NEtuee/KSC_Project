Shader "KSC/KSC_EnergyShield"
{

    Properties

    {   
        _MainShidTex ("Main Tex", 2D) = "White"{}

        //Shield Trim
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
                float4 uv : TEXCOORD0;
                float4 uv2 : TEXCOORD1;
            };

            struct Varyings
            {
                float4 vertex  : SV_POSITION;
                float3 NormalDir : TEXCOORD0;
                
                float4 ObjPos : TEXCOORD1;

                float4 uv : TEXCOORD2;
                float4 uv2 : TEXCOORD3;
                float3 viewDir : TEXCOORD5;
            };            

            sampler2D _MainShidTex;
            float4 _MainShidTex_ST;
            
            float4 _BaseColor;
            float4 _Hitpos;

            float _GradientSize;



            Varyings vert(Attributes v)
            {
   
                Varyings o;

                o.vertex = TransformObjectToHClip(v.vertex.xyz);
                o.NormalDir = v.normal;
                o.viewDir = normalize(o.vertex.xyz - _WorldSpaceCameraPos.xyz);
                o.ObjPos = v.vertex;
                o.uv = v.uv;
                return o;
            }

               
            half4 frag(Varyings i) : SV_Target
            {
                
                float3 N = TransformObjectToWorldNormal(i.NormalDir);
                N = normalize(N);
                float3 V = normalize(i.viewDir);

                /// Rim Light

                float VdotN = 1- saturate(dot(V,N));
                VdotN = pow(VdotN,10);

                float4 MainShield = tex2D(_MainShidTex, i.uv * _MainShidTex_ST.xy + _MainShidTex_ST.zw);
                
                ///
                float Gradient =  0;

                float Dist = distance(_Hitpos.xyz , i.ObjPos.xyz);
                Gradient += saturate( (1- Dist * _GradientSize) * _Hitpos.w);

                float3 col = Gradient * _BaseColor.rgb; 

                float4 final = VdotN;// MainShield + float4(col,1);

                final.a = 1;

                return final;
            }
            ENDHLSL
        }
    }
}