//world variables
float4x4 World;
float4x4 View;
float4x4 Projection;
float Time;
float TextureHeight;
float TotalLength;
float4 Color;
float Pi = 3.1415926535897;

texture2D Texture : register(s0);

sampler2D TextureSampler = sampler_state 
{
    Texture = (Texture);
    MinFilter = Linear;
    MagFilter = Linear;
    AddressU = Clamp;
    AddressV = Clamp;
};

// TODO: add effect parameters here.

struct VertexShaderInput
{
    float4 Position : POSITION0;
	float3 Normal : NORMAL0;
	float2 TexCoord : TEXCOORD0;
    // TODO: add input channels such as texture
    // coordinates and vertex colors here.
};

struct VertexShaderOutput
{
    float4 Position : POSITION0;
	float3 Normal : TEXCOORD1;
	float2 TexCoord : COLOR0;
    // TODO: add vertex shader outputs such as colors and texture
    // coordinates here. These values will automatically be interpolated
    // over the triangle, and provided as input to your pixel shader.
};

VertexShaderOutput VertexShaderFunction(VertexShaderInput input)
{
    VertexShaderOutput output;

    float4 worldPosition = mul(input.Position, World);
    float4 viewPosition = mul(worldPosition, View);
    output.Position = mul(viewPosition, Projection);
	output.TexCoord = input.TexCoord;
	output.Normal = normalize(mul(input.Normal, World));
    return output;
}

float4 PixelShaderFunction(VertexShaderOutput input) : COLOR0
{   
	float4 color = Color * tex2D(TextureSampler, float2(frac(input.TexCoord.x + Time / 120), clamp(frac((TotalLength * input.TexCoord.y) / TextureHeight), 0.01, 0.99)));
	return color;
}

technique CelShade
{
    pass Pass1
    {
        VertexShader = compile vs_2_0 VertexShaderFunction();
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}
