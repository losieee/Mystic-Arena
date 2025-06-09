Shader "Custom/CylinderTransparency"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _CylinderStart ("Cylinder Start", Vector) = (0,0,0,0)
        _CylinderEnd ("Cylinder End", Vector) = (0,0,0,0)
        _Radius ("Radius", Float) = 2.0
        _EdgeSharpness ("Edge Sharpness", Range(0, 1)) = 0.1
        _AlphaMultiplier ("Alpha Multiplier", Float) = 0.3
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        LOD 100

        Pass
        {
            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off
            Cull Off

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float3 worldPos : TEXCOORD1;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _CylinderStart;
            float4 _CylinderEnd;
            float _Radius;
            float _EdgeSharpness;
            float _AlphaMultiplier;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv);

                float3 startPoint = _CylinderStart.xyz;
                float3 endPoint = _CylinderEnd.xyz;
                float3 pointToCylinder = i.worldPos - startPoint;
                float3 cylinderDirection = normalize(endPoint - startPoint);
                float cylinderLength = distance(startPoint, endPoint);

                // 픽셀을 원기둥 축에 투영한 거리
                float projection = dot(pointToCylinder, cylinderDirection);

                // 투영점이 원기둥 범위 내에 있는지 확인
                if (projection > 0 && projection < cylinderLength)
                {
                    // 픽셀에서 원기둥 축까지의 가장 가까운 점
                    float3 closestPoint = startPoint + cylinderDirection * projection;
                    // 픽셀과 가장 가까운 점 사이의 거리 (원기둥 축으로부터의 거리)
                    float distToAxis = distance(i.worldPos, closestPoint);
                    // 원기둥 내부 마스크 (안쪽이 0, 바깥쪽이 1)
                    float alphaMask = smoothstep(_Radius - _Radius * _EdgeSharpness, _Radius + _Radius * _EdgeSharpness, distToAxis / 1.0);
                    // 원기둥 안쪽만 투명하게 만들기 위해 1에서 alphaMask를 뺍니다.
                    col.a *= (1 - alphaMask) * _AlphaMultiplier;
                }
                else
                {
                    col.a = 1; // 원기둥 외부는 불투명
                }

                return col;
            }
            ENDCG
        }
    }
}