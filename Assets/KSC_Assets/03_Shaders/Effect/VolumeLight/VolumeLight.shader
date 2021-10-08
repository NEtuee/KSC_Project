Shader "Hidden/VolumeLight"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Color ("Light Color", Color) = (1,1,1,1)
        _Scattering ("Scattering",float) = 0.5
        _Intensity ("Intensity",float) = 1
        _SampleCount ("Sample Count",float) = 100
    }
    SubShader
    {

        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS_CASCADE
 
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DeclareDepthTexture.hlsl"


            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = TransformObjectToHClip(v.vertex.xyz);
                o.uv = v.uv.xy;
                return o;
            }

            sampler2D _MainTex;
            float _SampleCount;
            float _Scattering;
            float _Intensity;
            float4 _Color;
            uniform float4 _CamPosition;
            uniform float4 _LightDirection;
            uniform sampler2D _testTex;
            uniform float _check;

            float ComputeScattering(float lightDotView)
            {
                float result = 1.0f - _Scattering * _Scattering;
                result /= (4.0f * PI * pow(abs(1.0f + _Scattering * _Scattering - (2.0f * _Scattering) * lightDotView), 1.5f));
                return result;
            }

            float3 GetWorldPos(float2 uv)
            {
                #if UNITY_REVERSED_Z
                    float depth = SampleSceneDepth(uv);
                #else
                    // Adjust z to match NDC for OpenGL
                    float depth = lerp(UNITY_NEAR_CLIP_VALUE, 1, SampleSceneDepth(uv));
                #endif
                return ComputeWorldSpacePosition(uv, depth, UNITY_MATRIX_I_VP);
            }

            float4 frag (v2f i) : SV_Target
            {
                float2 uv = i.uv;
                float3 start = _CamPosition.xyz;
                float3 end = GetWorldPos(i.uv);
                float3 rayVector = end - start;
                float rayLength = length(rayVector);
                float3 rayDirection = rayVector / rayLength;

                float stepLength = rayLength / _SampleCount;
                float3 step = rayDirection * stepLength;
                float3 currentPosition = start;

                float3 color = 0.0f.xxx;

                for(int i = 0; i < _SampleCount; ++i)
                {
                    float shadow = MainLightRealtimeShadow(TransformWorldToShadowCoord(currentPosition));
                    color += ComputeScattering(dot(rayDirection.xyz,_LightDirection.xyz)) * shadow * _Color.xyz * _Intensity;

                    currentPosition += step;
                }

                color /= _SampleCount;

                float4 mainColor = tex2D(_MainTex, uv) + float4(color,0);
                return float4(mainColor);
            }
            ENDHLSL
        }
    }
}
