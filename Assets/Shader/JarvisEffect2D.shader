Shader "Custom/URP_Jarvis_Stable"
{
    Properties
    {
        // 最关键的一行：告诉 UGUI 把 Image 的图片传给这个 Shader
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)

        // 贾维斯效果参数
        _ChromaticAberration ("Color Split", Range(0, 0.1)) = 0.02
        _GlitchIntensity ("Glitch Force", Range(0, 1)) = 0.1
        _ScanlineCount ("Scanlines", Float) = 500
        _ScanlineIntensity ("Scanline Fade", Range(0, 1)) = 0.2
        _NoiseIntensity ("Static Noise", Range(0, 1)) = 0.1

        // UGUI 遮罩支持参数 (勿删)
        [HideInInspector] _StencilComp ("Stencil Comp", Float) = 8
        [HideInInspector] _Stencil ("Stencil ID", Float) = 0
        [HideInInspector] _StencilOp ("Stencil Op", Float) = 0
        [HideInInspector] _StencilWriteMask ("Stencil Write", Float) = 255
        [HideInInspector] _StencilReadMask ("Stencil Read", Float) = 255
        [HideInInspector] _ColorMask ("Color Mask", Float) = 15
    }

    SubShader
    {
        Tags
        {
            "Queue"="Transparent"
            "IgnoreProjector"="True"
            "RenderType"="Transparent"
            "PreviewType"="Plane"
            "CanUseSpriteAtlas"="True"
        }

        Stencil
        {
            Ref [_Stencil]
            Comp [_StencilComp]
            Pass [_StencilOp]
            ReadMask [_StencilReadMask]
            WriteMask [_StencilWriteMask]
        }

        Cull Off
        Lighting Off
        ZWrite Off
        ZTest [unity_GUIZTestMode]
        Blend SrcAlpha OneMinusSrcAlpha
        ColorMask [_ColorMask]

        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
                float4 color : COLOR;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float2 uv : TEXCOORD0;
                float4 color : COLOR;
            };

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);

            CBUFFER_START(UnityPerMaterial)
                float4 _Color;
                float _ChromaticAberration;
                float _GlitchIntensity;
                float _ScanlineCount;
                float _ScanlineIntensity;
                float _NoiseIntensity;
            CBUFFER_END

            // 稳定的噪声函数
            float SimpleNoise(float2 uv)
            {
                return frac(sin(dot(uv, float2(12.9898, 78.233))) * 43758.5453);
            }

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);
                OUT.uv = IN.uv;
                OUT.color = IN.color * _Color;
                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target
            {
                // 限制时间，防止 Windows 挂机久了动画卡死
                float time = fmod(_Time.y, 1000.0);

                // 计算随机故障断层
                float glitchGrip = step(0.98, SimpleNoise(float2(time, floor(IN.uv.y * 20.0))));
                float2 distortUV = IN.uv;
                distortUV.x += glitchGrip * _GlitchIntensity * (SimpleNoise(float2(time, IN.uv.y)) - 0.5) * 0.1;

                // 带有防越界保护 (clamp) 的色差采样
                float offset = _ChromaticAberration * 0.5;
                half4 colR = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex,
                    clamp(distortUV + float2(offset, 0.0), 0.0, 1.0));
                half4 colG = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, distortUV);
                half4 colB = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex,
                    clamp(distortUV - float2(offset, 0.0), 0.0, 1.0));

                // 组合颜色并应用 Image 组件自带的颜色 (包含透明度)
                half4 finalColor = half4(colR.r, colG.g, colB.b, colG.a);
                finalColor *= IN.color;

                // 如果像素透明，直接返回，不再渲染黑色的噪点
                if (finalColor.a <= 0.01) return finalColor;

                // 扫描线
                float scanline = sin(IN.uv.y * _ScanlineCount + time * 5.0);
                finalColor.rgb *= lerp(1.0, (scanline * 0.5 + 0.5), _ScanlineIntensity);

                // 噪点雪花
                float noise = (SimpleNoise(distortUV + time) - 0.5) * _NoiseIntensity;
                finalColor.rgb += noise;

                return finalColor;
            }
            ENDHLSL
        }
    }
    // 最后的安全兜底：如果设备不支持，退化为普通 UI 图片
    Fallback "UI/Default"
}