#include "monogame.fxh"

struct VS_IN {
    float3 Position : POSITION;
    float3 Normal : NORMAL;
    float2 TexCoord : TEXCOORD;
};

struct PS_IN {
    float4 PositionT : SV_POSITION;
    float2 TexCoord : TEXCOORD;
};

cbuffer cbTransforms {
    float4x4 gWorldViewProj;
}

float4 gOutlineColor;
float gOutlineThickness;
float gOpacity;

texture gTexture;

sampler2D gDiffuseSampler = sampler_state {
    Texture = <gTexture>;
    MagFilter = Linear;
    MinFilter = Linear;
    AddressU = Clamp;
    AddressV = Clamp;
};

PS_IN vs_outline(VS_IN input) {
    PS_IN output = (PS_IN)0;

    float4 original = mul(float4(input.Position, 1.0f), gWorldViewProj);
    float4 extended = mul(float4(normalize(input.Normal), 1.0f), gWorldViewProj);

    output.PositionT = original + mul(gOutlineThickness, extended);

    return output;
}

float4 ps_outline(PS_IN input) : SV_TARGET {
    float4 output = gOutlineColor;

    output.rgb *= output.a;
    output *= gOpacity;

    return output;
}

PS_IN vs_copy(VS_IN input) {
    PS_IN output = (PS_IN)0;

    output.PositionT = mul(float4(input.Position, 1.0f), gWorldViewProj);
    output.TexCoord = input.TexCoord;

    return output;
}

float4 ps_copy(PS_IN input) : SV_TARGET {
    float4 output = tex2D(gDiffuseSampler, input.TexCoord.xy);
    
    output.rgb *= output.a;
    output *= gOpacity;

    return output;
}

technique SpriteOutline {
    pass Outline {
        VertexShader = compile VS_SHADERMODEL vs_outline();
        PixelShader = compile PS_SHADERMODEL ps_outline();
    }
    pass Copy {
        VertexShader = compile VS_SHADERMODEL vs_copy();
        PixelShader = compile PS_SHADERMODEL ps_copy();
    }
}
