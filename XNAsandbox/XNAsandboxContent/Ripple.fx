Texture2D tex;
float pixel;
float2 dPos;

sampler2D texSampl = sampler_state {
	Texture = (tex);
	MagFilter = Point;
	MinFilter = Point;
	MipFilter = Point;
	AddressU = Wrap;
	AddressV = Wrap;
};

struct VertexShaderInput
{
	float2 pos : POSITION0;
};

struct VertexShaderOutput
{
	float4 pos : POSITION0;
	float2 uv : TEXCOORD0;
};

VertexShaderOutput VertexShaderFunction(VertexShaderInput i)
{
    VertexShaderOutput o;

	o.pos = float4(i.pos, 1, 1);
	o.uv = i.pos * 0.5 + 0.5;
	o.uv.y = 1 - o.uv.y;
	o.uv += pixel * 0.5f;
	o.uv += dPos;

    return o;
}

float4 PixelShaderFunction(VertexShaderOutput i) : COLOR0
{
	float2 ul = tex2D(texSampl, i.uv + float2(-pixel, pixel));
	float2 bl = tex2D(texSampl, i.uv + float2(-pixel, -pixel));
	float2 ur = tex2D(texSampl, i.uv + float2(pixel, pixel));
	float2 br = tex2D(texSampl, i.uv + float2(pixel, -pixel));
	float2 c = tex2D(texSampl, i.uv);

	float accel = ul.x + bl.x + ur.x + br.x - 4 * c.x;

	float vel = (c.y + accel * 0.4f);
	float pos = c.x * 0.99 + vel * 0.1f;

    return float4(pos, vel, 0, 0);
}

technique Technique1
{
    pass Pass1
    {
        VertexShader = compile vs_3_0 VertexShaderFunction();
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
}
