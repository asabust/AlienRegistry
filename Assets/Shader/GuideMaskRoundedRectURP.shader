Shader "UI/GuideMaskRoundedRectURP"
{
    Properties
    {
        _Color ("Overlay Color", Color) = (0,0,0,0.7)

        _Center ("Center (UV)", Vector) = (0.5,0.5,0,0)
        _Size ("Size (UV)", Vector) = (0.3,0.2,0,0)
        _Aspect ("Aspect", Float) = 1
        _Radius ("Corner Radius", Float) = 0.05
        _Softness ("Edge Softness", Float) = 0.01

        _BorderWidth ("Border Width", Float) = 0.01
        _BorderColor ("Border Color", Color) = (1,1,1,1)
        _BorderSoftness ("Border Softness", Float) = 0.01
    }


    SubShader
    {
        Tags
        {
            "RenderType"="Transparent"
            "Queue"="Transparent"
            "RenderPipeline"="UniversalPipeline"
        }

        Pass
        {
            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off
            Cull Off

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            float4 _Color;
            float2 _Center;
            float2 _Size;
            float _Aspect;
            float _Radius;
            float _Softness;

            float _BorderWidth;
            float4 _BorderColor;
            float _BorderSoftness;

            Varyings vert(Attributes v)
            {
                Varyings o;
                o.positionHCS = TransformObjectToHClip(v.positionOS.xyz);
                o.uv = v.uv;
                return o;
            }

            // SDF: 圆角矩形距离
            float sdRoundedBox(float2 p, float2 b, float r)
            {
                float2 q = abs(p) - b + r;
                return length(max(q, 0.0)) - r + min(max(q.x, q.y), 0.0);
            }

            half4 frag(Varyings i) : SV_Target
            {
                float2 p = i.uv - _Center;
                p.x *= _Aspect;

                float2 size = _Size;
                size.x *= _Aspect;

                // 距离（负数=内部）
                float dist = sdRoundedBox(p, size * 0.5, _Radius);

                // 挖洞（透明区域）
                float holeMask = smoothstep(0, _Softness, dist);

                // 遮罩alpha
                float alpha = holeMask * _Color.a;

                // ===== 描边（发光边）=====
                float borderOuter = smoothstep(_BorderWidth, _BorderWidth + _BorderSoftness, dist);
                float borderInner = smoothstep(0, _BorderSoftness, dist);
                float border = borderInner - borderOuter;

                float3 finalColor = _Color.rgb;
                finalColor = lerp(finalColor, _BorderColor.rgb, border);

                return half4(finalColor, alpha + border * _BorderColor.a);
            }
            ENDHLSL
        }
    }
}