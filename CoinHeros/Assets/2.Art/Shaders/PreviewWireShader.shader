Shader "Hidden/PreviewWireShader"
{
    SubShader
    {
        Pass
        {
            ZWrite Off
            ZTest Always
            Cull Off
            Lighting Off
            Fog { Mode Off }
            ColorMask RGB
            Blend SrcAlpha OneMinusSrcAlpha

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct v2f
            {
                float4 pos : SV_POSITION;
            };

            v2f vert(appdata_base v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                return o;
            }

            half4 frag(v2f i) : SV_Target
            {
                return half4(1, 1, 0, 0.1); // 반투명 노란색
            }
            ENDCG
        }
    }
}
