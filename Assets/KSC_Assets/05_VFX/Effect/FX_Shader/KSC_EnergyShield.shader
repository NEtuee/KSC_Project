Shader "KSC/KSC_EnergyShield"
{

    Properties

    {   
        _MainShidTex ("Main Tex", 2D) = "White" {}
        _EdgeNoiseTex ("Edge Noise", 2D) = "White" {}
        _NoiseXspeed ("Noise X Speed", Range(-30,30)) = 1
        _NoiseYspeed ("Noise Y Speed", Range(-30,30)) = 1

        _RimRange ("Rim Range", Range (0,20)) = 1

        [HDR] _RimColor ("Rim Color", Color) = (1,1,1,1)
        [HDR] _FaceColor ("FaceColor", Color) = (1,1,1,1)
        [HDR] _EdgeColor ("Edge Color", Color) = (1,1,1,1)

        _MeshMoveSpeed ("Vertex Speed", Range(0,50)) = 1
        _MeshMoveAmount ("Vertex Move Amount", Range(0,10)) = 1



        //Shield Trim
        _HitTex ("Hit Noise Tex", 2D) = "white" {}
        [HDR]_HitColor ("Base Color", Color ) = (1,1,1,1)
        _Hitpos ("Collision Point", Vector) = (0.0,0.0,0.0,0.0)
        _GradientSize ("Gradient Size", float ) = 1.0
        _GradPower ("Gradient Power", float) = 1.0


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
                float4 uv3 : TEXCOORD2;
            };

            struct Varyings
            {
                float4 vertex  : SV_POSITION;
                float3 NormalDir : TEXCOORD0;
                
                float4 ObjPos : TEXCOORD1;

                float4 uv : TEXCOORD2;
                float4 uv2 : TEXCOORD3;
                float4 uv3 : TEXCOORD4; // 반응형용 
                float3 viewDir : TEXCOORD5;
            };            

            sampler2D _MainShidTex;
            sampler2D _EdgeNoiseTex;
            sampler2D _HitTex;

            float4 _EdgeNoiseTex_ST;
            float4 _MainShidTex_ST;
            float4 _HitTex_ST;
            
            float4 _RimColor;
            float4 _EdgeColor;
            float4 _FaceColor;

            float4 _HitColor;
            float4 _Hitpos;

            float _GradPower;
            float _GradientSize;
            float _RimRange;
            float _MeshMoveSpeed;
            float _MeshMoveAmount;

            float _NoiseXspeed;
            float _NoiseYspeed;


            Varyings vert(Attributes v)
            {
   
                Varyings o;
               
                v.vertex.xyz = (v.vertex.xyz + v.normal.xyz * ((sin(_Time.y * _MeshMoveSpeed) * 0.5 + 0.5) * _MeshMoveAmount));

                o.vertex = TransformObjectToHClip(v.vertex.xyz);
                o.NormalDir = v.normal;
                o.viewDir = normalize(_WorldSpaceCameraPos.xyz - TransformObjectToWorld(v.vertex.xyz));
                o.ObjPos = v.vertex;
                o.uv = v.uv;
                o.uv2 = v.uv2;
                o.uv3 = v.uv3;
                return o;
            }

               
            half4 frag(Varyings i) : SV_Target
            {
                
                float3 N = TransformObjectToWorldNormal(i.NormalDir);
                N = normalize(N);
                float3 V = normalize(i.viewDir);
               
                /// Rim Light
                float VdotN = 1- saturate(dot(N,V));
                VdotN = pow(VdotN,_RimRange);

                //Textures
                float4 MainShield = tex2D(_MainShidTex, i.uv * _MainShidTex_ST.xy + _MainShidTex_ST.zw);
                _EdgeNoiseTex_ST.z =  (_EdgeNoiseTex_ST.z + _Time * _NoiseXspeed);
                _EdgeNoiseTex_ST.w = (_EdgeNoiseTex_ST.w + _Time * _NoiseYspeed);
                float4 EdgeNoise = tex2D(_EdgeNoiseTex, i.uv2 * _EdgeNoiseTex_ST.xy + _EdgeNoiseTex_ST.zw);

                MainShield = (pow(MainShield * 1.2, 5)) * EdgeNoise;

                MainShield = lerp (_FaceColor, _EdgeColor, MainShield);
                MainShield = lerp(MainShield, _RimColor, VdotN);
                ///

                
                //----=------------------
                float Gradient =  0;

                float4 HitTex = tex2D(_HitTex, i.uv3 * _HitTex_ST.xy + _HitTex_ST.zw);

                float Dist = distance(_Hitpos.xyz , i.ObjPos.xyz);
                Gradient += saturate( (1- Dist * _GradientSize) * _Hitpos.w);
                Gradient = saturate(pow(Gradient,_GradPower));
                float3 col = ( Gradient * HitTex ) + (Gradient * _HitColor.rgb);
                //----=------------------


                float4 final = MainShield + float4(col,1);

                final.a = VdotN * MainShield.a + col;

                return final;
            }
            ENDHLSL
        }
    }
}