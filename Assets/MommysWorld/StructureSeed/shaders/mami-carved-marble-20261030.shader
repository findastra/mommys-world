Shader "Mami/Carved Marble"
{
    Properties
    {
        _Color ("Marble body", Color) = (0.88, 0.9, 0.92, 1)
        _VeinColor ("Dark mineral veins", Color) = (0.025, 0.022, 0.03, 1)
        [HDR] _RootColor ("Pink mineral roots", Color) = (1, 0.06, 0.3, 1)
        _Scale ("Pattern scale per metre", Range(0.03, 2)) = 0.3
        _RootStrength ("Sparse root emission", Range(0, 3)) = 0.25
        _Glossiness ("Polished stone smoothness", Range(0, 1)) = 0.62
        _Metallic ("Mineral metallic", Range(0, 1)) = 0.05
    }
    SubShader
    {
        Tags { "RenderType" = "Opaque" }
        LOD 250
        CGPROGRAM
        #pragma surface surf Standard fullforwardshadows
        #pragma target 3.0
        #include "UnityCG.cginc"

        fixed4 _Color, _VeinColor;
        half4 _RootColor;
        float _Scale;
        half _RootStrength, _Glossiness, _Metallic;

        struct Input
        {
            float3 worldPos;
        };

        float MineralHash(float3 cell)
        {
            cell = frac(cell * 0.1031);
            cell += dot(cell, cell.yzx + 33.33);
            return frac((cell.x + cell.y) * cell.z);
        }

        void surf(Input IN, inout SurfaceOutputStandard o)
        {
            // Coherent world-space stone: adjacent modules share a continuous vein field.
            // These are surface minerals, not geometric cracks or structural damage.
            float3 p = IN.worldPos * _Scale;
            float warpA = sin(dot(p, float3(0.59, 1.07, 0.43)));
            float warpB = sin(dot(p, float3(1.31, -0.67, 0.81)) + 0.8 * warpA);
            float warpC = sin(dot(p, float3(-2.1, 0.91, 1.37)) + warpB);
            float phase = dot(p, float3(1.7, 0.62, 0.91))
                + 1.35 * warpA + 0.58 * warpB + 0.17 * warpC;
            float veinDistance = abs(sin(phase));
            float aa = max(fwidth(veinDistance), 0.002);

            float deepVein = 1.0 - smoothstep(0.025 - aa, 0.09 + aa, veinDistance);
            float softVein = 1.0 - smoothstep(0.06 - aa, 0.26 + aa, veinDistance);
            float satellite = abs(sin(phase * 2.71 + warpB * 0.75));
            float hairline = 1.0 - smoothstep(0.008, 0.035 + fwidth(satellite), satellite);
            float darkMineral = saturate(deepVein * 0.86 + softVein * 0.2 + hairline * 0.09);

            // Pink is confined to occasional narrow branches, leaving the marble dominant.
            float rootGate = smoothstep(0.69, 0.94,
                sin(dot(p, float3(0.47, -0.83, 0.61)) + warpA));
            float rootThread = 1.0 - smoothstep(0.006, 0.028 + aa, veinDistance);
            float roots = rootThread * rootGate;
            float softCloud = 0.97 + 0.03 * sin(dot(p, float3(2.31, 1.37, -0.71)) + warpB);
            half3 stone = lerp(_Color.rgb * softCloud, _VeinColor.rgb, darkMineral);
            o.Albedo = lerp(stone, _RootColor.rgb * 0.55, roots * 0.65);

            // A few fine mineral flecks disappear when too small to resolve on screen.
            float3 grainPosition = IN.worldPos * 65.0;
            float3 pixelSpan = fwidth(grainPosition);
            float sampleFade = saturate(1.0 - max(pixelSpan.x, max(pixelSpan.y, pixelSpan.z)));
            float distanceFade = 1.0 - smoothstep(2.5, 12.0, distance(IN.worldPos, _WorldSpaceCameraPos));
            float fleck = smoothstep(0.994, 0.9995, MineralHash(floor(grainPosition)));
            float mineralGlint = fleck * sampleFade * distanceFade;

            o.Emission = _RootColor.rgb * roots * _RootStrength + mineralGlint * 0.12;
            o.Metallic = saturate(_Metallic + mineralGlint * 0.2);
            o.Smoothness = saturate(_Glossiness - softVein * 0.08 + mineralGlint * 0.18);
            o.Occlusion = 1.0;
            o.Alpha = 1.0;
        }
        ENDCG
    }
    FallBack "Standard"
}
