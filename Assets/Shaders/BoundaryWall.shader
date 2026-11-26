// 경계 벽 표시용 셰이더 (URP, Unlit, Additive)
// 1. 바닥에서 올라오는 빛 커튼 (항상 은은하게 표시)
// 2. 플레이어가 가까워지면 격자 무늬가 드러남 (_BoundaryPlayerPos: BoundaryWall.cs가 전역으로 설정)
// 3. 부딪힌 지점에서 물결이 퍼짐 (_HitPoints: BoundaryWall.cs가 MaterialPropertyBlock으로 설정)
// 기본 Cube 메시 기준: 오브젝트 공간 y(-0.5 ~ 0.5)를 높이로 사용, 윗면/아랫면은 그리지 않음
Shader "Custom/BoundaryWall"
{
    Properties
    {
        [HDR] _Color ("Color", Color) = (0.3, 1.5, 3, 1)

        [Header(Curtain)]
        _BaseAlpha ("Base Alpha", Range(0, 1)) = 0.35
        _HeightFalloff ("Height Falloff", Range(0.5, 8)) = 2.5
        _StreakScale ("Streak Density", Float) = 1.5
        _ScrollSpeed ("Scroll Speed", Float) = 0.6

        [Header(Proximity Grid)]
        _GridSize ("Grid Size (m)", Float) = 1
        _GridWidth ("Grid Line Width", Range(0.01, 0.2)) = 0.04
        _RevealRadius ("Reveal Radius", Float) = 4

        [Header(Hit Ripple)]
        _RippleSpeed ("Ripple Speed", Float) = 6
        _RippleWidth ("Ripple Width", Float) = 0.4
        _RippleDuration ("Ripple Duration", Float) = 0.8

        [Header(Camera)]
        _CameraFadeDistance ("Camera Fade Distance", Float) = 2
    }

    SubShader
    {
        Tags
        {
            "RenderType" = "Transparent"
            "Queue" = "Transparent"
            "RenderPipeline" = "UniversalPipeline"
            "IgnoreProjector" = "True"
        }

        Pass
        {
            Name "Unlit"
            Blend SrcAlpha One
            ZWrite Off
            Cull Off

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            #define MAX_HITS 4

            CBUFFER_START(UnityPerMaterial)
                half4 _Color;
                float _BaseAlpha;
                float _HeightFalloff;
                float _StreakScale;
                float _ScrollSpeed;
                float _GridSize;
                float _GridWidth;
                float _RevealRadius;
                float _RippleSpeed;
                float _RippleWidth;
                float _RippleDuration;
                float _CameraFadeDistance;
            CBUFFER_END

            float4 _HitPoints[MAX_HITS];   // xyz = 월드 위치, w = 맞은 시각(_Time.y 기준)
            float4 _HitNormals[MAX_HITS];  // xyz = 맞은 면 쪽 방향(벽 → 공)
            float4 _BoundaryPlayerPos;

            struct Attributes
            {
                float4 positionOS : POSITION;
                float3 normalOS : NORMAL;
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float3 positionWS : TEXCOORD0;
                float3 normalOS : TEXCOORD1;
                float height01 : TEXCOORD2;
            };

            Varyings vert(Attributes input)
            {
                Varyings output;
                output.positionWS = TransformObjectToWorld(input.positionOS.xyz);
                output.positionCS = TransformWorldToHClip(output.positionWS);
                output.normalOS = input.normalOS;
                output.height01 = input.positionOS.y + 0.5;
                return output;
            }

            half4 frag(Varyings input) : SV_Target
            {
                float3 posWS = input.positionWS;
                float3 normalWS = TransformObjectToWorldNormal(input.normalOS);
                float3 n = abs(normalWS);
                float sideMask = 1.0 - step(0.5, abs(input.normalOS.y)); // 윗면/아랫면 제외

                // 1. 빛 커튼: 위로 갈수록 투명 + 위로 흐르는 줄기
                float h = saturate(input.height01);
                float heightMask = pow(1.0 - h, _HeightFalloff);
                float along = (posWS.x + posWS.z) * _StreakScale;
                float column = 0.5 + 0.5 * sin(along * 6.2832 + 1.7 * sin(along * 2.3));
                float rise = 0.5 + 0.5 * sin((posWS.y - _Time.y * _ScrollSpeed) * 4.0 + along * 3.0);
                float curtain = _BaseAlpha * heightMask * lerp(0.6, 1.0, column * rise);

                // 2. 근접 격자: 면 방향 축을 제외한 월드 격자선
                float3 d = abs(frac(posWS / _GridSize) - 0.5);
                float3 lines = smoothstep(0.5 - _GridWidth, 0.5, d) * (1.0 - n);
                float grid = max(max(lines.x, lines.y), lines.z);
                float reveal = saturate(1.0 - distance(posWS, _BoundaryPlayerPos.xyz) / _RevealRadius);
                reveal *= reveal;
                reveal *= step(0.0, dot(normalWS, _BoundaryPlayerPos.xyz - posWS)); // 공이 있는 쪽 면에만 표시
                float proximity = reveal * (grid + 0.15);

                // 3. 충돌 물결
                float ripple = 0.0;
                [unroll]
                for (int i = 0; i < MAX_HITS; i++)
                {
                    float t = _Time.y - _HitPoints[i].w;
                    float alive = step(0.0, t) * step(t, _RippleDuration);
                    alive *= step(0.5, dot(normalWS, _HitNormals[i].xyz)); // 맞은 면에만 표시
                    float dist = distance(posWS, _HitPoints[i].xyz);
                    float ring = 1.0 - saturate(abs(dist - t * _RippleSpeed) / _RippleWidth);
                    ripple += ring * (1.0 - t / _RippleDuration) * alive;
                }

                // 카메라가 벽에 가까우면 흐리게 해서 시야를 가리지 않음
                float cameraFade = saturate(distance(posWS, _WorldSpaceCameraPos) / _CameraFadeDistance);

                float alpha = saturate(curtain + proximity + ripple) * cameraFade * sideMask;
                return half4(_Color.rgb, alpha);
            }
            ENDHLSL
        }
    }
}
