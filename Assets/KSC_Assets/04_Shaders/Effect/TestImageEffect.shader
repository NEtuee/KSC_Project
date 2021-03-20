Shader "Hidden/TestImageEffect"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Distance ("Distance",float) = 0

        _ScanWidth("Scan Width", float) = 10
		_LeadSharp("Leading Edge Sharpness", float) = 10
		_LeadColor("Leading Edge Color", Color) = (1, 1, 1, 0)
		_MidColor("Mid Color", Color) = (1, 1, 1, 0)
		_TrailColor("Trail Color", Color) = (1, 1, 1, 0)
		_HBarColor("Horizontal Bar Color", Color) = (0.5, 0.5, 0.5, 0)
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

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            sampler2D_float _CameraDepthTexture;
            float4x4 _InvProjectionMatrix;    //Pass this in via 'camera.projectionMatrix.inverse'
            float4x4 _ViewToWorld;    //Pass this in via 'camera.cameraToWorldMatrix'
            sampler2D _MainTex;
            float _Distance;
            float4 _WorldSpaceScannerPos;
            float _ScanWidth;
			float _LeadSharp;
			float4 _LeadColor;
			float4 _MidColor;
			float4 _TrailColor;
			float4 _HBarColor;
            
            struct v2f
            {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
                float4 viewDir : TEXCOORD1;
            };
            
            float4 horizBars(float2 p)
			{
				return 1 - saturate(round(abs(frac(p.y * 100) * 2)));
			}

            v2f vert (appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos (v.vertex);
                o.uv = v.uv;
                o.viewDir = mul (_InvProjectionMatrix, float4 (o.uv * 2.0 - 1.0, 1.0, 1.0));
                return o;
            }
            
            half4 frag (v2f i) : SV_Target
            {
                half4 col = tex2D(_MainTex, i.uv);
                float depth = Linear01Depth (tex2D (_CameraDepthTexture, i.uv).r);
            
                //Perspective divide and scale by depth to get view-space position
                float3 viewPos = (i.viewDir.xyz / i.viewDir.w) * depth;
                //Transform to world space
                float4 worldPos = mul (_ViewToWorld, float4 (viewPos, 1));

                float dist = distance (_WorldSpaceScannerPos, worldPos);
                half4 scannerCol = half4(0, 0, 0, 0);

                if (dist < _Distance && dist > _Distance - _ScanWidth && depth < 1)
				{
					float diff = 1 - (_Distance - dist) / (_ScanWidth);
					half4 edge = lerp(_MidColor, _LeadColor, pow(diff, _LeadSharp));
					scannerCol = lerp(_TrailColor, edge, diff) + horizBars(i.uv) * _HBarColor;
					scannerCol *= diff;
				}
            
                // if(distance (_WorldSpaceScannerPos, worldPos) < _Distance)
                //     return 1;

                //Fill a sphere around the origin
                return col + scannerCol;
            }

            // struct v2f
            // {
            //     float2 uv : TEXCOORD0;
            //     float4 vertex : SV_POSITION;
            //     float4 worldSpacePos : TEXCOORD1;
            // };

            // v2f vert (appdata v)
            // {
            //     v2f o;
            //     o.vertex = UnityObjectToClipPos(v.vertex);
            //     o.uv = v.uv;
            //     o.worldSpacePos = mul(unity_ObjectToWorld, v.vertex);
            //     return o;
            // }

            // sampler2D _MainTex;
            // float _Distance;

            // fixed4 frag (v2f i) : SV_Target
            // {
            //     fixed4 col = tex2D(_MainTex, i.uv);
            //     // just invert the colors
            //     float dist = distance(i.worldSpacePos,float4(0,0,0,1));
            //     if(dist < _Distance)
            //         return 1;
            //     col.rgb = col.rgb;
            //     return col;
            // }
            ENDCG
        }
    }
}
