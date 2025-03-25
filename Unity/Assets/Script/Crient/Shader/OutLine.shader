Shader "Custom/OutLine"
{
    Properties
    {
        _OutlineColor ("Outline Color", Color) = (1, 1, 1, 1)
        _OutlineWidth ("Outline Width", Range(0.002, 0.03)) = 0.005
    }

    SubShader
    {
        Tags { "Queue" = "Overlay" }
        Pass
        {
            Name "OUTLINE"
            Cull Front

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata_t
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
            };

            float _OutlineWidth;
            fixed4 _OutlineColor;

            v2f vert(appdata_t v)
            {
                v2f o;
                float3 norm = normalize(v.normal);
                float3 pos = v.vertex + norm * _OutlineWidth;
                o.pos = UnityObjectToClipPos(float4(pos, 1));
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                return _OutlineColor;
            }
            ENDCG
        }
    }
}
