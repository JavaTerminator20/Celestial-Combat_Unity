Shader "Celestial/GalacticDustFloor"
{
    Properties
    {
        _Noise ("Noise Texture", 2D) = "white" {}
        _Color ("Tint Color", Color) = (0.5, 0.7, 1, 0.5)
        _ScrollSpeed ("Scroll Speed", Float) = 0.01
        _Intensity ("Intensity", Float) = 1
    }

    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
        Cull Off
        Lighting Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _Noise;
            float4 _Color;
            float _ScrollSpeed;
            float _Intensity;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv + float2(_Time.y * _ScrollSpeed, 0);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float noise = tex2D(_Noise, i.uv).r;
                return _Color * noise * _Intensity;
            }
            ENDCG
        }
    }
}
