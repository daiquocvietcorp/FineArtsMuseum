Shader "Custom/URPHighlightShader"
{
    Properties
    {
        _BaseColor("Base Color", Color) = (1,0.8,0.2,1)
        _EmissionColor("Emission Color", Color) = (0.5,0.4,0.1,1)
        _EmissionStrength("Emission Strength", Range(0, 5)) = 2
    }
    
    SubShader
    {
        Tags 
        { 
            "RenderType" = "Opaque"
            "RenderPipeline" = "UniversalPipeline"
            "Queue" = "Geometry"
        }
        
        HLSLINCLUDE
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
        
        struct Attributes
        {
            float4 positionOS : POSITION;
            float3 normalOS : NORMAL;
        };

        struct Varyings
        {
            float4 positionHCS : SV_POSITION;
            float3 normalWS : TEXCOORD0;
        };
        
        CBUFFER_START(UnityPerMaterial)
            float4 _BaseColor;
            float4 _EmissionColor;
            float _EmissionStrength;
        CBUFFER_END
        ENDHLSL

        Pass
        {
            Name "ForwardLit"
            Tags { "LightMode" = "UniversalForward" }
            
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            Varyings vert(Attributes input)
            {
                Varyings output;
                
                VertexPositionInputs vertexInput = GetVertexPositionInputs(input.positionOS.xyz);
                output.positionHCS = vertexInput.positionCS;
                output.normalWS = TransformObjectToWorldNormal(input.normalOS);
                
                return output;
            }

            half4 frag(Varyings input) : SV_Target
            {
                // Simple directional light (approximation)
                float3 lightDir = normalize(float3(0.5, 1, 0.5)); // Approximate light direction
                float3 normalWS = normalize(input.normalWS);
                float NdotL = saturate(dot(normalWS, lightDir));
                
                // Combine base color with lighting and emission
                half4 color = _BaseColor;
                color.rgb *= NdotL * 0.7 + 0.3; // Mix with ambient
                color.rgb += _EmissionColor.rgb * _EmissionStrength; // Emission
                
                return color;
            }
            ENDHLSL
        }
    }
    
    FallBack "Universal Render Pipeline/Lit"
}