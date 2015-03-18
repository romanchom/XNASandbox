float4x4 view;
float4x4 viewProj;
Texture2D tex;
TextureCube enviro;
float pixel;
float3 lightPos;
float4x4 invView;
float fresnelParam;
float4 waterTint;
float2 globalPos;


sampler2D texSampl = sampler_state {
    Texture = (tex);
    MagFilter = Point;
    MinFilter = Point;
	MipFilter = Point;
    AddressU = Clamp;
    AddressV = Clamp;
};


samplerCUBE enviroSampl = sampler_state {
	Texture = (enviro);
	MagFilter = Linear;
	MinFilter = Linear;
	MipFilter = Linear;
};

// TODO: add effect parameters here.

struct VertexShaderInput
{
    float2 pos: POSITION0;
    float2 uv : TEXCOORD0;
};

struct VertexShaderOutput
{
    float4 pos : POSITION0;
	float2 uv : TEXCOORD0;
	float3 normal : TEXCOORD1;
	float3 viewPos : TEXCOORD2;
};

VertexShaderOutput VertexShaderFunction(VertexShaderInput i){
    VertexShaderOutput o;

	float4 uv = float4(i.uv, 0, 0);

	float4 pos = float4(i.pos.x + globalPos.x, tex2Dlod(texSampl, uv).r, -i.pos.y + globalPos.y, 1);
    o.pos = mul(pos, viewProj);
	o.uv = i.uv;


	float ul = tex2Dlod(texSampl, uv + float4(-pixel, pixel, 0, 0)).x;
	float bl = tex2Dlod(texSampl, uv + float4(-pixel, -pixel, 0, 0)).x;
	float ur = tex2Dlod(texSampl, uv + float4(pixel, pixel, 0, 0)).x;
	float br = tex2Dlod(texSampl, uv + float4(pixel, -pixel, 0, 0)).x;

	o.normal.x = (ur + br - ul - bl) * -0.25;
	o.normal.z = (ul + ur - bl - br) * 0.25;
	o.normal.y = 1;
	o.normal = mul(float4(o.normal, 0), view).xyz;
	o.viewPos = mul(pos, view).xyz;
	
    return o;
}

float4 PixelShaderFunction(VertexShaderOutput i) : COLOR0{
	float3 normal = normalize(i.normal);

	float3 lightDir = normalize(lightPos - i.viewPos);
	float3 viewDir = normalize(i.viewPos);
	float3 reflectViewDir = reflect(viewDir, normal);
	float3 refractViewDir = refract(viewDir, normal, 0.7f);

	float4 reflection = texCUBE(enviroSampl, mul(reflectViewDir, invView));
	float4 refraction = texCUBE(enviroSampl, mul(refractViewDir, invView));

	float specular = max(pow(dot(reflectViewDir, lightDir), 100), 0);

	float fresnelFactor = fresnelParam + (1 - fresnelParam) * pow(1 - dot(normal, -viewDir), 5);


	return (reflection + specular.xxxx) * fresnelFactor + waterTint * refraction * (1 - fresnelFactor);
}

technique Technique1{
    pass Pass1{
        VertexShader = compile vs_3_0 VertexShaderFunction();
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
}
