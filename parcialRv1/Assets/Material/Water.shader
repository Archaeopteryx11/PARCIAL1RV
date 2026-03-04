// ============================================================
//  Water.shader  —  Plumber Heroes
//  Shader URP compatible para el volumen de agua del subterráneo
//  Coloca este archivo en Assets/Shaders/Water.shader
// ============================================================
Shader "PlumberHeroes/Water"
{
    Properties
    {
        _BaseColor   ("Color base del agua",      Color)  = (0.2, 0.6, 0.9, 0.6)
        _DeepColor   ("Color profundo/turbio",    Color)  = (0.05, 0.15, 0.4, 0.85)
        _WaveSpeed   ("Velocidad de olas",        Float)  = 0.5
        _WaveHeight  ("Altura de olas",           Float)  = 0.05
        _FoamAmount  ("Espuma en bordes",         Range(0,1)) = 0.1
        _WaterLevel  ("Nivel de agua (0-1)",      Float)  = 0.0
        _Turbidity   ("Turbidez (0-1)",           Float)  = 0.0
        _TimeOffset  ("Offset de tiempo (animación)", Float) = 0.0
        _NoiseTex    ("Textura de ruido (olas)",  2D)     = "white" {}
        _FoamTex     ("Textura de espuma",        2D)     = "white" {}
    }

    SubShader
    {
        Tags
        {
            "RenderType"  = "Transparent"
            "Queue"       = "Transparent"
            "RenderPipeline" = "UniversalPipeline"
        }

        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
        Cull Back

        Pass
        {
            Name "WaterPass"
            Tags { "LightMode" = "UniversalForward" }

            HLSLPROGRAM
            #pragma vertex   vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            // ── Properties como variables ─────────────────────────
            CBUFFER_START(UnityPerMaterial)
                float4 _BaseColor;
                float4 _DeepColor;
                float  _WaveSpeed;
                float  _WaveHeight;
                float  _FoamAmount;
                float  _WaterLevel;
                float  _Turbidity;
                float  _TimeOffset;
                float4 _NoiseTex_ST;
                float4 _FoamTex_ST;
            CBUFFER_END

            TEXTURE2D(_NoiseTex); SAMPLER(sampler_NoiseTex);
            TEXTURE2D(_FoamTex);  SAMPLER(sampler_FoamTex);

            // ── Structs ───────────────────────────────────────────
            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv         : TEXCOORD0;
                float3 normalOS   : NORMAL;
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float2 uv         : TEXCOORD0;
                float3 worldPos   : TEXCOORD1;
                float  depth      : TEXCOORD2;
            };

            // ── Funciones auxiliares ──────────────────────────────

            // Genera ondas usando dos muestras de ruido desplazadas en el tiempo
            float2 WaveOffset(float2 uv, float time)
            {
                float2 uv1 = uv + float2(time * 0.08, time * 0.05);
                float2 uv2 = uv - float2(time * 0.06, time * 0.09);
                float  n1  = SAMPLE_TEXTURE2D(_NoiseTex, sampler_NoiseTex, uv1).r;
                float  n2  = SAMPLE_TEXTURE2D(_NoiseTex, sampler_NoiseTex, uv2).r;
                return float2(n1 - 0.5, n2 - 0.5) * 2.0;
            }

            // ── Vertex ────────────────────────────────────────────
            Varyings vert(Attributes IN)
            {
                Varyings OUT;

                // Desplazar vértices en Y para simular olas
                float2  waveOff = WaveOffset(IN.uv, _TimeOffset);
                float3  pos     = IN.positionOS.xyz;
                pos.y += waveOff.x * _WaveHeight;
                pos.x += waveOff.y * _WaveHeight * 0.4; // ligero movimiento lateral

                OUT.positionCS = TransformObjectToHClip(float4(pos, 1.0));
                OUT.uv         = IN.uv;
                OUT.worldPos   = TransformObjectToWorld(pos).xyz;

                // Depth usado para oscurecer las zonas "profundas"
                OUT.depth = saturate(1.0 - IN.uv.y);

                return OUT;
            }

            // ── Fragment ──────────────────────────────────────────
            half4 frag(Varyings IN) : SV_Target
            {
                float time = _TimeOffset;

                // 1. Mezcla de color base con color profundo
                float4 waterColor = lerp(_BaseColor, _DeepColor, _Turbidity);

                // 2. Añadir brillo de superficie (fake specular simple)
                float2 uvAnim = IN.uv + float2(sin(time * 0.4 + IN.uv.y * 6.0) * 0.02,
                                               cos(time * 0.3 + IN.uv.x * 6.0) * 0.02);
                float noise   = SAMPLE_TEXTURE2D(_NoiseTex, sampler_NoiseTex, uvAnim).r;
                float specular = pow(saturate(noise - 0.6), 3.0) * 0.4;
                waterColor.rgb += specular;

                // 3. Espuma en los bordes superiores del volumen
                float2 foamUV   = IN.uv * 3.0 + float2(time * 0.15, time * 0.1);
                float  foamMask = SAMPLE_TEXTURE2D(_FoamTex, sampler_FoamTex, foamUV).r;
                // La espuma solo aparece cerca de la superficie (Y alta)
                float  surfaceEdge = saturate((IN.uv.y - 0.85) * 10.0);
                float  foam        = foamMask * surfaceEdge * _FoamAmount;
                waterColor.rgb     = lerp(waterColor.rgb, float3(1,1,1), foam);

                // 4. Transparencia: más opaco en zonas profundas y conforme sube el nivel
                float baseAlpha  = lerp(_BaseColor.a, _DeepColor.a, _Turbidity);
                float depthAlpha = lerp(baseAlpha * 0.6, baseAlpha, IN.depth);
                waterColor.a = saturate(depthAlpha + foam * 0.3);

                return waterColor;
            }
            ENDHLSL
        }
    }

    FallBack "Universal Render Pipeline/Lit"
}
