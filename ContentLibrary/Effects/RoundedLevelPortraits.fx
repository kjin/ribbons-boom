sampler2D tex : register(S0);
int Time;

float4 PixelShaderFunction(float2 texCoord: TEXCOORD0, float4 color: COLOR0) : COLOR0
{
	float4 ret = tex2D(tex, texCoord) * color;
	float t = frac(Time / 100.0) * 3;
	if (abs(-2 * (texCoord.x - t) - texCoord.y) < 0.1)
		return ret * float4(2, 2, 2, 1);
	return ret;
}

technique Technique1
{
    pass Pass1
    {
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}
