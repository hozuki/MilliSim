//=============================================================================
// Basic.fx by Frank Luna (C) 2011 All Rights Reserved.
//
// Basic effect that currently supports transformations, lighting, and texturing.
//=============================================================================

// Modified by MIC @ Sep 01 2017

// ================================================================================
// About redundant variables:
// We have to declare these two args (PS_IN.NormalW and PS_IN.TexCoord) as float4,
// not the usual float3 and float2. That's because this shader suffers from a
// problem mentioned in https://github.com/MonoGame/MonoGame/issues/2054.
// The original vertex shader reads like:
// ```
// vout.NormalW = mul(float4(vin.Normal, 1.0f), gWorldInvTranspose).xyz;
// vout.TexCoord = mul(float4(vin.TexCoord, 1.0f), (float3x3)gTextureTransform).xy;
// ```
// When reversed to GLSL from HLSL bytecode (this is what 2MGFX does), sizes are
// incorrectly computed. In the vertex shader, four float4x4 variables are used:
// gWorld, gWorldInvTranspose, gWorldViewProj, gTextureTransform. As you can see,
// only a part of gWorldInvTranspose and gTextureTransform are used. Therefore
// due to automatic optimization, their offsets are 0, 64, 112 and 176; their sizes
// are 64, 56, 64 and 48 (the total buffer size is 224). MonoGame does not recognize
// this, so when we pass in a Matrix (corresponding to float4x4), it assumes each of
// these has a size of 64 bytes (16 elements). So when it assigns gWorldViewProj, it
// actually corrupts the data of gWorldInvTranspose. Also, when it assigns
// gTextureTransform, the variable buffer overflows (an ArgumentException in
// ConstantBuffer.SetData(), raised by Buffer.BlockCopy()). MonoGame doesn't support
// assigning a value to a float3x3 (this may be achieved by assigning to a series of
// float3, by SetValue(Vector3[]), and reconstruct to a float3x3 in HLSL, I guess),
// so we add a little more redundancy in the data structure, i.e. changing all
// vectors to float4, and use all float4x4 elements. This trick fools the optimizer,
// and now their offsets are 0, 64, 128 and 192, with all sizes 64, exactly what we
// want, no more overflows. We are not rendering extremely complex objects, so the
// efficiency loss due to this trick (requiring a little more bandwidth) is not
// noticable.
// =================================================================================

#include "monogame.fxh"

cbuffer cbTexture {
    float4 gMaterialAmbient;
    float4 gMaterialDiffuse;
    float4 gMaterialSpecular; // w = SpecPower
    float4 gMaterialReflect;
}

struct VS_IN {
    float3 Position : POSITION;
    float3 Normal : NORMAL;
    float2 TexCoord : TEXCOORD;
};

struct PS_IN {
    float4 PositionT : SV_POSITION;
    float4 Position : COLOR0;
    float4 NormalW : NORMAL;
    float4 TexCoord : COLOR1;
};

cbuffer cbTransforms {
    float4x4 gWorld;
    float4x4 gWorldInvTranspose;
    float4x4 gWorldViewProj;
    float4x4 gTextureTransform;
}

float gCurrentTime; // Used for animations in CGSS.
float gOpacity;

texture gRibbonTexture;

sampler2D gDiffuseSampler = sampler_state {
    Texture = <gRibbonTexture>;
    MagFilter = Linear;
    MinFilter = Linear;
    AddressU = Clamp;
    AddressV = Clamp;
};

PS_IN VS(in VS_IN vin) {
    PS_IN vout;

    vout.PositionT = mul(float4(vin.Position, 1.0f), gWorldViewProj);

	vout.Position = mul(float4(vin.Position, 1.0f), gWorld);
    vout.NormalW = mul(float4(vin.Normal, 1.0f), gWorldInvTranspose);

    vout.TexCoord = mul(float4(vin.TexCoord, 0.0f, 1.0f), gTextureTransform);

    return vout;
}

float4 PS(in PS_IN pin) : SV_TARGET0 {
    float3 normalW = normalize(pin.NormalW.xyz);

    float4 texColor = tex2D(gDiffuseSampler, pin.TexCoord.xy);

    texColor.rgb *= texColor.a;
    texColor *= gOpacity;

	return texColor;
}

technique SimpleTexture {
    pass P0 {
        VertexShader = compile VS_SHADERMODEL VS();
        PixelShader = compile PS_SHADERMODEL PS();
    }
}
