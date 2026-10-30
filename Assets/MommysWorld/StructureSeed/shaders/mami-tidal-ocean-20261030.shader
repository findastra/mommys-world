Shader "Mami/Tidal Ocean"
{
    Properties
    {
        _Color ("Deep teal water", Color) = (0.025, 0.22, 0.26, 1)
        _WaveAmplitude ("Wave amplitude in metres (maximum 0.3)", Range(0, 0.3)) = 0.22
        _Smoothness ("Water smoothness", Range(0, 1)) = 0.9
        _DryRadius ("Dry basement radius in metres", Float) = 48
        _DryBottom ("Bottom of dry enclosure (world Y)", Float) = -120
        _Center ("Dry enclosure world centre (XZ)", Vector) = (0, 0, 0, 0)
    }
    SubShader
    {
        Tags { "RenderType" = "Opaque" "Queue" = "Geometry" }
        LOD 250
        Cull Back
        ZWrite On
        CGPROGRAM
        #pragma surface surf Standard fullforwardshadows vertex:vert addshadow
        #pragma target 3.0
        #include "UnityCG.cginc"

        fixed4 _Color;
        float _WaveAmplitude, _DryRadius, _DryBottom;
        float4 _Center;
        half _Smoothness;

        struct Input
        {
            float3 worldPos;
            float3 worldNormal;
            INTERNAL_DATA
        };

        void BroadWaves(float2 xz, out float height, out float2 slope)
        {
            float amplitude = clamp(_WaveAmplitude, 0.0, 0.3);
            float a = dot(xz, float2(0.075, 0.037)) + _Time.y * 0.62;
            float b = dot(xz, float2(-0.043, 0.091)) - _Time.y * 0.43;
            // Positive weights sum to one: displacement never exceeds the amplitude.
            height = amplitude * (0.62 * sin(a) + 0.38 * sin(b));
            slope = amplitude * (0.62 * cos(a) * float2(0.075, 0.037)
                + 0.38 * cos(b) * float2(-0.043, 0.091));
        }

        void vert(inout appdata_full v)
        {
            float3 worldPosition = mul(unity_ObjectToWorld, v.vertex).xyz;
            float waveHeight;
            float2 waveSlope;
            BroadWaves(worldPosition.xz, waveHeight, waveSlope);
            worldPosition.y += waveHeight;
            v.vertex = mul(unity_WorldToObject, float4(worldPosition, 1.0));
            float3 worldNormal = normalize(float3(-waveSlope.x, 1.0, -waveSlope.y));
            v.normal = normalize(mul(worldNormal, (float3x3)unity_ObjectToWorld));
        }

        void surf(Input IN, inout SurfaceOutputStandard o)
        {
            // Provisional opaque surface, not a fluid simulation. Move the ocean object
            // vertically to preview tides. The dry cylinder needs a retaining shell mesh.
            float radius = distance(IN.worldPos.xz, _Center.xz);
            float insideDry = step(radius, max(0.0, _DryRadius));
            float aboveBottom = step(_DryBottom, IN.worldPos.y);
            clip(0.5 - insideDry * aboveBottom);

            float waveHeight;
            float2 slope;
            BroadWaves(IN.worldPos.xz, waveHeight, slope);
            float rippleA = dot(IN.worldPos.xz, float2(0.91, 0.52)) + _Time.y * 1.1;
            float rippleB = dot(IN.worldPos.xz, float2(-0.63, 1.17)) - _Time.y * 0.87;
            float rippleDistance = 1.0 - smoothstep(80.0, 360.0,
                distance(IN.worldPos, _WorldSpaceCameraPos));
            slope += rippleDistance * (0.035 * cos(rippleA) * float2(0.91, 0.52)
                + 0.022 * cos(rippleB) * float2(-0.63, 1.17));

            float3 desiredNormal = normalize(float3(-slope.x, 1.0, -slope.y));
            float3 worldTangent = normalize(WorldNormalVector(IN, float3(1, 0, 0)));
            float3 worldBitangent = normalize(WorldNormalVector(IN, float3(0, 1, 0)));
            float3 worldNormal = normalize(WorldNormalVector(IN, float3(0, 0, 1)));
            o.Normal = normalize(float3(dot(desiredNormal, worldTangent),
                dot(desiredNormal, worldBitangent), dot(desiredNormal, worldNormal)));

            // Broad tint variation suggests depth without pretending to sample a seabed.
            float tint = 0.92 + 0.08 * sin(dot(IN.worldPos.xz, float2(0.017, 0.023)));
            o.Albedo = _Color.rgb * tint;
            o.Metallic = 0.0;
            o.Smoothness = _Smoothness;
            o.Occlusion = 1.0;
            o.Alpha = 1.0;
        }
        ENDCG
    }
    // No fallback that would silently fill the intentionally dry basement with water.
    FallBack Off
}
