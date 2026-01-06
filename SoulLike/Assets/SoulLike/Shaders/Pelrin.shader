Shader "Custom/Sprite/PerlinMoyamoya_BigFlow_WorldSpace_URP_Unlit"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}

        _ColorA ("Color A (Low)", Color) = (0.15, 0.15, 0.15, 1)
        _ColorB ("Color B (High)", Color) = (1, 1, 1, 1)

        _ColorThreshold ("Color Threshold", Range(0,1)) = 0.5
        _ColorSharpness ("Color Sharpness", Range(0.001, 0.5)) = 0.12

        // ★ワールド基準の“波長”調整（値が小さいほど模様が大きい）
        _WorldFrequency ("World Frequency", Range(0.05, 10)) = 0.8

        // 大きい流れ（低周波）
        _FlowSpeed ("Flow Speed", Range(0, 10)) = 0.6
        _WarpStrength ("Warp Strength", Range(0, 0.5)) = 0.22

        // 細部（中〜高周波）
        _DetailFrequency ("Detail Frequency", Range(0.05, 30)) = 3.5
        _DetailSpeed ("Detail Speed", Range(0, 10)) = 1.2
        _DetailAmount ("Detail Amount", Range(0, 1)) = 0.25

        _AlphaPower ("Alpha Power", Range(0.1, 8)) = 1.7
        _AlphaMin ("Alpha Min", Range(0, 1)) = 0
        _AlphaMax ("Alpha Max", Range(0, 1)) = 1

        _EdgeSoftness ("Edge Softness (UV)", Range(0, 1)) = 0.15
    }

    SubShader
    {
        Tags
        {
            "RenderPipeline"="UniversalPipeline"
            "Queue"="Transparent"
            "RenderType"="Transparent"
            "CanUseSpriteAtlas"="True"
        }

        Cull Off
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);

            CBUFFER_START(UnityPerMaterial)
                float4 _MainTex_ST;
                float4 _ColorA;
                float4 _ColorB;

                float _ColorThreshold;
                float _ColorSharpness;

                float _WorldFrequency;

                float _FlowSpeed;
                float _WarpStrength;

                float _DetailFrequency;
                float _DetailSpeed;
                float _DetailAmount;

                float _AlphaPower;
                float _AlphaMin;
                float _AlphaMax;

                float _EdgeSoftness;
            CBUFFER_END

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv         : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float2 uv          : TEXCOORD0;
                float2 worldXY     : TEXCOORD1; // ★ノイズ入力用
            };

            float2 hash2(float2 p)
            {
                p = float2(dot(p, float2(127.1,311.7)),
                           dot(p, float2(269.5,183.3)));
                return -1 + 2 * frac(sin(p) * 43758.5453123);
            }

            float fade(float t) { return t*t*t*(t*(t*6-15)+10); }

            float gradNoise(float2 p)
            {
                float2 i = floor(p);
                float2 f = frac(p);

                float2 a = hash2(i);
                float2 b = hash2(i + float2(1,0));
                float2 c = hash2(i + float2(0,1));
                float2 d = hash2(i + 1);

                float va = dot(a, f);
                float vb = dot(b, f - float2(1,0));
                float vc = dot(c, f - float2(0,1));
                float vd = dot(d, f - 1);

                float2 u = float2(fade(f.x), fade(f.y));
                return lerp(lerp(va, vb, u.x), lerp(vc, vd, u.x), u.y) * 0.5 + 0.5;
            }

            float fbm3(float2 p)
            {
                float v = 0;
                float a = 0.5;
                v += a * gradNoise(p); p *= 2; a *= 0.5;
                v += a * gradNoise(p); p *= 2; a *= 0.5;
                v += a * gradNoise(p);
                return v;
            }

            float2 flowField(float2 p)
            {
                float nx = fbm3(p + float2(13.2, 7.1));
                float ny = fbm3(p + float2(3.7, 19.4));
                return (float2(nx, ny) * 2 - 1);
            }

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                float3 worldPos = TransformObjectToWorld(IN.positionOS.xyz);

                OUT.positionHCS = TransformWorldToHClip(worldPos);
                OUT.uv = TRANSFORM_TEX(IN.uv, _MainTex);

                // ★ワールド座標をそのまま使う（X/Yは2D想定。Zを使いたければ差し替え）
                OUT.worldXY = worldPos.xy;

                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target
            {
                // ここがキモ：UVではなく worldXY をノイズ入力にする
                float2 baseP = IN.worldXY * _WorldFrequency;

                float tFlow = _Time.y * _FlowSpeed;
                float2 pFlow = baseP + float2(tFlow, -tFlow * 0.5);

                float2 f1 = flowField(pFlow);
                float2 warpedP = baseP + f1 * _WarpStrength;

                float2 f2 = flowField(warpedP + float2(-tFlow * 0.7, tFlow * 0.3));
                warpedP += f2 * (_WarpStrength * 0.6);

                float tDet = _Time.y * _DetailSpeed;
                float nDet  = fbm3(warpedP * (_DetailFrequency / max(_WorldFrequency, 1e-4)) + float2(tDet * 0.9, -tDet * 0.6));
                float nBase = fbm3(warpedP + float2(tFlow * 0.4, tFlow * 0.2));

                float n = saturate(lerp(nBase, saturate(nBase + (nDet - 0.5) * 2), _DetailAmount));

                float nColor = smoothstep(
                    _ColorThreshold - _ColorSharpness,
                    _ColorThreshold + _ColorSharpness,
                    n
                );

                half4 col = lerp(_ColorA, _ColorB, nColor);

                float a = pow(saturate(nBase), _AlphaPower);
                a = lerp(_AlphaMin, _AlphaMax, a);

                // エッジ処理はUVのままでOK（バーの形を保ちたいから）
                float2 edge = smoothstep(0, _EdgeSoftness, IN.uv)
                            * (1 - smoothstep(1 - _EdgeSoftness, 1, IN.uv));
                col.a *= a * edge.x * edge.y;

                return col;
            }
            ENDHLSL
        }
    }
}
