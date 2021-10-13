// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "KSC/TestImageEffect"
{
    Properties
    {
        [HideInInspector]_MainTex ("Texture", 2D) = "white" {}

        _NoiseTex ("Noise Texture", 2D) = "black" {}

        _Distance ("Distance",float) = 0

        _ScanHeightLimit("Scan Height", float) = 9999
        _ScanWidth("Scan Width", Range(1,100)) = 10
        _ScanAlpha("Scan Alpha", Range(0.05,1)) = 1
		_LeadSharp("Leading Edge Sharpness", Range(1,100)) = 10

        [HDR]_NosieColor("Main NoiseColor", Color) = (1,1,1,1)
        [Space][Space][Space]
		[HDR]_LeadColor("Leading Edge Color", Color) = (1, 1, 1, 0)
		[HDR]_MidColor("Mid Color", Color) = (1, 1, 1, 0)
		[HDR]_TrailColor("Trail Color", Color) = (1, 1, 1, 0)

		//[HDR]_HBarColor("Horizontal Bar Color", Color) = (0.5, 0.5, 0.5, 0)
    }
    SubShader
    {
        // No culling or depth
        Cull Off ZWrite Off ZTest Always

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"
//
            struct Vertin
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float2 uv2 : TEXCOORD2;
                float3 uv3 : TEXCOORD3;

            };

              
            struct VertOut
            {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
                
                float4 viewDir : TEXCOORD1;
                float2 uv2 : TEXCOORD2;
                float2 uv3 : TEXCOORD3;
                float2 uv_depth : TEXCOORD4;
            };

            sampler2D_float _CameraDepthTexture;
            float4x4 _InvProjectionMatrix;    //Pass this in via 'camera.projectionMatrix.inverse' (역 투영행렬 -> 뷰 스페이스 값이 나옴 )
            float4x4 _ViewToWorld;    //Pass this in via 'camera.cameraToWorldMatrix' (뷰 스페이스 값을 월드값으로 바꿈)
            // 서순은 모델 , 월드 , 뷰 , 프로젝션 이니까 역으로 프로젝션 -> 뷰 -> 월드 순임
           
            sampler2D _MainTex;
            sampler2D _NoiseTex;

            float4 _NoiseTex_ST;


            SamplerState sampler_PatternTex;

            

            float _Distance;
            float4 _WorldSpaceScannerPos;
            float4 _ForwardDirection;

            float _ScanWidth;
			float _LeadSharp;
            float _ScanAlpha;
            float _ScanArc;
            float _ScanHeightLimit = 9999;
            
            float4 _NosieColor;
			float4 _LeadColor;
			float4 _MidColor;
			float4 _TrailColor;
		
          
            
          

            VertOut vert (Vertin v)
            {
                VertOut o;
                o.pos = UnityObjectToClipPos (v.vertex); // MVP 끝남. 클릭 공간으로 슉 
                o.uv = v.uv;
                o.uv2 = v.uv2;//float2(o.pos.x,o.pos.z);
                o.uv3 = v.uv3;
                o.viewDir = mul(_InvProjectionMatrix, float4(o.uv * 2.0 - 1.0, 1.0, 1.0)); // View 공간 값이 나옴


                return o;



            }
            
        	float4 horizBars(float2 p)
			{
				return 1 - saturate(round(abs(frac(p.y * 100) * 2)));
			}

            half4 frag (VertOut i) : SV_Target
            {
               
                
                half4 col = tex2D(_MainTex, i.uv);
/*
                i.uv2.y += _Time.y * -0.2;
                float4 PatternTex2 = _PatternTex2.Sample(sampler_PatternTex, i.uv2 * _PatternTex2_ST.xy + _PatternTex2_ST.zw);
                i.uv2.y += _Time.y * -0.5;
                float4 PatternTex = _PatternTex.Sample(sampler_PatternTex, i.uv2 * _PatternTex_ST.xy + _PatternTex_ST.zw);
                
*/
                float depth = Linear01Depth (DecodeFloatRG(tex2D (_CameraDepthTexture, i.uv)));

                
                //Perspective divide and scale by depth to get view-space position
                float3 viewPos = (i.viewDir.xyz / i.viewDir.w) * depth;
                //Transform to world space
                float4 worldPos = mul ( _ViewToWorld, float4 (viewPos, 1)); // world 로 변환

                bool isUp = (_ScanHeightLimit > worldPos.y);
                if(!isUp)
                    return col;

                worldPos.y = 0;
                _WorldSpaceScannerPos.y = 0;
                float4 direction = normalize(worldPos - _WorldSpaceScannerPos);
                direction.y = 0;

                direction = normalize(direction);


                _ForwardDirection.y = 0;
                _ForwardDirection = normalize(_ForwardDirection);
                float angle = abs((dot(_ForwardDirection, direction) - 1)) * 90;
                float3 upvec = float3 (0,1,0);
                float side = dot(upvec,cross(direction,_ForwardDirection));
                if(side < 0)
                {
                    side = -1;

                
                }
                else
                {
                    side = 1;
                }
                
          

                half4 scannerCol = half4(0, 0, 0, 0); //스캐너 색상 초기화

                if(angle < _ScanArc * 0.5)
                {   
                    float dist = distance (_WorldSpaceScannerPos, worldPos);

                    float v = ((_ScanArc * 0.5 ) + (angle * side))/ _ScanArc;
                  
                   // i.uv3 = float2(dist, v);
                   // i.uv3 += _Time * -10; 
                    
                    float4 Mainnoise = tex2D(_NoiseTex, i.uv2 * _NoiseTex_ST.xy + _NoiseTex_ST.zw); 
                   
                    if (dist < _Distance && dist > _Distance - _ScanWidth && depth < 1)
				    {
				    	float diff = 1 - (_Distance - dist) / (_ScanWidth);
                       

                        half4 mid = _MidColor;
                       

				    	half4 edge = lerp(mid, _LeadColor, pow(diff, _LeadSharp));
				    	scannerCol = lerp(_TrailColor, edge, diff);// horizBars(i.uv);
                        scannerCol = lerp(scannerCol,_NosieColor, Mainnoise);
				    	scannerCol *= diff;
                        scannerCol *= _ScanAlpha;
                       

				    }
                }

                return col + scannerCol;
            }
            
            ENDCG
        }
    }
}
