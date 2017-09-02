#include "helper.fx"

cbuffer cbPerObject {
    float4x4 gWorldViewProj;
    Material gMaterial;
}

struct VS_IN {
    float3 Position : POSITION;
    float3 Normal : NORMAL;
    float2 Tex : TEXCOORD;
};

struct PS_IN {
    float4 Position : SV_POSITION;
    float4 Color : COLOR;
};

PS_IN VS(VS_IN input) {
    PS_IN output = (PS_IN)0;
	
    output.Position = mul(float4(input.Position, 1.0f), gWorldViewProj);
    output.Color = gMaterial.Diffuse;
	
    return output;
}

float4 PS(PS_IN input) : SV_Target {
    return input.Color;
}

technique11 VertexColor {
    pass P0 {
        SetVertexShader(CompileShader(vs_4_0, VS()));
        SetPixelShader(CompileShader(ps_4_0, PS()));
    }
}
