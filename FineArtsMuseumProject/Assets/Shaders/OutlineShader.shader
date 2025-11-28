Shader "Custom/OutlineShader"
{
    Properties
    {
        _OutlineColor ("Outline Color", Color) = (1,0.8,0.2,1)
        _OutlineWidth ("Outline Width", Range(0, 0.1)) = 0.05
    }
    
    SubShader
    {
        Tags { "RenderType"="Opaque" "Queue"="Geometry" }
        
        // First pass: Draw outline
        Pass
        {
            Name "OUTLINE"
            Tags { "LightMode" = "Always" }
            
            Cull Front
            ZWrite On
            ColorMask RGB
            Blend SrcAlpha OneMinusSrcAlpha
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            
            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
            };
            
            struct v2f
            {
                float4 pos : SV_POSITION;
                fixed4 color : COLOR;
            };
            
            float _OutlineWidth;
            fixed4 _OutlineColor;
            
            v2f vert(appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                float3 norm = mul((float3x3)UNITY_MATRIX_IT_MV, v.normal);
                float2 offset = TransformViewToProjection(norm.xy);
                o.pos.xy += offset * _OutlineWidth;
                o.color = _OutlineColor;
                return o;
            }
            
            fixed4 frag(v2f i) : SV_Target
            {
                return i.color;
            }
            ENDCG
        }
        
        // Second pass: Draw object
        Pass
        {
            Name "OBJECT"
            Tags { "LightMode" = "ForwardBase" }
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            #include "Lighting.cginc"
            
            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
            };
            
            struct v2f
            {
                float4 pos : SV_POSITION;
                float3 worldNormal : TEXCOORD0;
            };
            
            v2f vert(appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.worldNormal = UnityObjectToWorldNormal(v.normal);
                return o;
            }
            
            fixed4 frag(v2f i) : SV_Target
            {
                // Simple lighting
                float3 lightDir = normalize(_WorldSpaceLightPos0.xyz);
                float NdotL = max(0, dot(i.worldNormal, lightDir));
                fixed4 col = fixed4(1, 0.8, 0.3, 1); // Orange color
                col.rgb *= NdotL * _LightColor0.rgb + UNITY_LIGHTMODEL_AMBIENT.rgb;
                return col;
            }
            ENDCG
        }
    }
    
    FallBack "Diffuse"
}