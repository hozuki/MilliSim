//=============================================================================
// Basic.fx by Frank Luna (C) 2011 All Rights Reserved.
//
// Basic effect that currently supports transformations, lighting, and texturing.
//=============================================================================

// Modified by MIC @ Sep 01 2017

#include "helper.fx"

cbuffer cbPerObject {
    float4x4 gWorld;
    float4x4 gWorldInvTranspose;
    float4x4 gWorldViewProj;
    float4x4 gTexTransform;
    Material gMaterial;
};

cbuffer cbPerFrame {
    float gCurrentTime; // Used for animations in CGSS.
}

// Nonnumeric values cannot be added to a cbuffer.
Texture2D gDiffuseMap;

SamplerState samLinear {
	Filter = MIN_MAG_MIP_LINEAR;
	AddressU = WRAP;
	AddressV = WRAP;
};

// Using standard alpha blend.
// Equivalent to FrequentlyUsedStates.AlphaBlend.
BlendState AlphaBlend {
    BlendEnable[0] = true;
    SrcBlend = SRC_ALPHA;
    DestBlend = INV_SRC_ALPHA;
    BlendOp = ADD;
    SrcBlendAlpha = ONE;
    DestBlendAlpha = ZERO;
    BlendOpAlpha = ADD;
    RenderTargetWriteMask[0] = 0x0F;
};

// Enable depth copmarison, but the comparison always passes.
// Equivalent to FrequentlyUsedStates.NoDepth.
DepthStencilState NoDepth {
    DepthEnable = true;
    DepthFunc = ALWAYS;
    DepthWriteMask = ZERO;
};

// No culling.
// Equivalent to FrequentlyUsedStates.NoCull.
RasterizerState NoCull {
    FillMode = SOLID;
    CullMode = NONE;
    FrontCounterClockwise = false;
    DepthClipEnable = false;
};

struct PS_IN {
    float4 PosH : SV_POSITION;
    float3 PosW : POSITION;
    float3 NormalW : NORMAL;
    float2 Tex : TEXCOORD0;
};

PS_IN VS(VS_IN vin) {
    PS_IN vout;
	
	// Transform to world space space.
    vout.PosW = mul(float4(vin.Position, 1.0f), gWorld).xyz;
    vout.NormalW = mul(vin.Normal, (float3x3)gWorldInvTranspose);

	// Transform to homogeneous clip space.
    vout.PosH = mul(float4(vin.Position, 1.0f), gWorldViewProj);

	// Output vertex attributes for interpolation across triangle.
    vout.Tex = mul(float4(vin.TexCoord, 0.0f, 1.0f), gTexTransform).xy;

    return vout;
}
 
float4 PS(PS_IN pin, uniform bool gUseTexure, uniform bool gAlphaClip) : SV_Target {
	// Interpolating normal can unnormalize it, so normalize it.
    pin.NormalW = normalize(pin.NormalW);
	
    // Default to multiplicative identity.
    float4 texColor;
    if (gUseTexure) {
		// Sample texture.
        texColor = gDiffuseMap.Sample(samLinear, pin.Tex);

        if (gAlphaClip) {
			// Discard pixel if texture alpha < 0.1.  Note that we do this
			// test as soon as possible so that we can potentially exit the shader 
			// early, thereby skipping the rest of the shader code.
            clip(texColor.a - 0.1f);
        }

        // Common to take alpha from diffuse material and texture.
        texColor.a = gMaterial.Diffuse.a * texColor.a;
    } else {
        texColor = gMaterial.Diffuse;
    }

    // Demostration of using gCurrentTime to achieve ribbon animation like CGSS.
    //float d = 0.3f * sin(gCurrentTime * 3.0f) + 0.3f;
    //texColor.rgb += float3(d, d, d);
	 
	return texColor;
}

technique11 SimpleTexture {
    pass P0 {
        SetGeometryShader(NULL);
        SetVertexShader(CompileShader(vs_4_0, VS()));
        SetPixelShader(CompileShader(ps_4_0, PS(true, false)));
        SetBlendState(AlphaBlend, float4(0.0f, 0.0f, 0.0f, 0.0f), 0xffffffff);
        SetDepthStencilState(NoDepth, 0);
        SetRasterizerState(NoCull);
    }
}
