//world variables
float4x4 World;
float4x4 View;
float4x4 Projection;
float PulseCenter;
float ContactTime;
float Time;
float Pi = 3.1415926535897;
float TextureHeight;
float TotalLength;

float MinTexCoord;
float MaxTexCoord;

float4 RibbonColor;
float ActiveTime;
float UnlockedTime;

texture2D Texture : register(s0);

sampler2D TextureSampler = sampler_state 
{
    Texture = (Texture);
    MinFilter = Linear;
    MagFilter = Linear;
    AddressU = Clamp;
    AddressV = Clamp;
};

//diffuse lighting
float4 DiffuseColor;
float DiffuseIntensity;
float3 DiffuseLightDirection;

//cel shading
float ColorDivisions;

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
	float2 PositionXY : TEXCOORD0;
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
	output.PositionXY = output.Position.xy;
	output.TexCoord = input.TexCoord;
	output.Normal = normalize(mul(input.Normal, World));
    return output;
}

float Mod(float x, float m)
{
    float r = x % m;
    return r < 0 ? r + m : r;
}

float4 PixelShaderFunction(VertexShaderOutput input) : COLOR0
{   
	bool onRibbon = true;
	//See if the ribbon is within range.
	float minDistance = MinTexCoord - Mod(MinTexCoord, 1.0);
	float pos = input.TexCoord.y + minDistance;
	if (pos <= MinTexCoord || pos >= MaxTexCoord)
	{
		float maxDistance = MaxTexCoord - Mod(MaxTexCoord, 1.0);
		pos = input.TexCoord.y + maxDistance;
		if (pos <= MinTexCoord || pos >= MaxTexCoord)
			//Ribbon will NOT be drawn.
			onRibbon = false;
	}
	//Ribbon is within range.
	float4 color = float4(0.5, 0.5, 0.5, 0.5);
	if (onRibbon)
		color = tex2D(TextureSampler, float2(input.TexCoord.x, clamp(frac(TotalLength * (pos - MinTexCoord) / TextureHeight), 0.02, 0.98)));
	float dist = abs(pos - PulseCenter) * TotalLength / 100;
	if (ActiveTime > 0 && onRibbon)
	{
		float t = min(Time - ContactTime, 60);
		if (t != 60)
		{
			float amount = sin(500 * dist - Pi * (t / 4 - t * t / 480)) * (1 - 20 * min(dist, 0.05));
			color += amount * float4(0.1, 0.1, 0.1, -0.3) * (1 - t / 60);
		}
	}
	float mult = 1.5;
	if (ActiveTime <= 0 && UnlockedTime < 60)
		mult = 0.5 * UnlockedTime / 60 + 0.25;
	else if (dist * 150 > ActiveTime)
		mult = 0.75;
	return color * RibbonColor * float4(mult, mult, mult, 1);
}

technique CelShade
{
    pass Pass1
    {
        VertexShader = compile vs_2_0 VertexShaderFunction();
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}
