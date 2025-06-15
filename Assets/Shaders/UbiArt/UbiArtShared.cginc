#ifndef SHARED_UBIART
#define SHARED_UBIART
#pragma multi_compile_instancing
#include "UnityCG.cginc"

// User-specified properties
UNITY_INSTANCING_BUFFER_START(Props)
UNITY_DEFINE_INSTANCED_PROP(uniform float4, _ColorFactor); // Primitive Params
UNITY_DEFINE_INSTANCED_PROP(uniform float4, _LightConfig);
UNITY_DEFINE_INSTANCED_PROP(uniform float4, _ColorFog);
UNITY_DEFINE_INSTANCED_PROP(uniform float4, _PrimitiveParams1);
UNITY_DEFINE_INSTANCED_PROP(uniform float4, _VertexAnimParams);

UNITY_DEFINE_INSTANCED_PROP(uniform float4, _UseTextures); // Textures
UNITY_DEFINE_INSTANCED_PROP(uniform float4, _UseTextures2);
UNITY_DEFINE_INSTANCED_PROP(uniform float4, _TextureScrollParams);
UNITY_DEFINE_INSTANCED_PROP(uniform float4, _TextureScrollParams2);

UNITY_DEFINE_INSTANCED_PROP(uniform float4, _ShaderParams); // Shader
UNITY_DEFINE_INSTANCED_PROP(uniform float4, _ShaderParams2);
UNITY_DEFINE_INSTANCED_PROP(uniform float4, _BezierParams);
UNITY_DEFINE_INSTANCED_PROP(StructuredBuffer<float4>, _BezierControlPoints);
UNITY_DEFINE_INSTANCED_PROP(StructuredBuffer<float4>, _BezierColors);
UNITY_INSTANCING_BUFFER_END(Props)

sampler2D _Diffuse;
sampler2D _BackLight;
sampler2D _Normal;
sampler2D _SeparateAlpha;
sampler2D _Diffuse2;
sampler2D _BackLight2;
sampler2D _AnimImpostor;
uniform float4 _Diffuse_ST;
uniform float4 _BackLight_ST;
uniform float4 _Normal_ST;
uniform float4 _SeparateAlpha_ST;
uniform float4 _Diffuse2_ST;
uniform float4 _BackLight2_ST;
uniform float4 _AnimImpostor_ST;

sampler2D _LightsBackLight;
sampler2D _LightsFrontLight;
uniform float4 _LightsBackLight_ST;
uniform float4 _LightsFrontLight_ST;

// Lighting
float _Luminosity = 0.5;
float _Saturate = 1.0;
float _EnableLighting = 1;
float _DisableLightingLocal = 0;
float _DisableFog = 0;
float4 _GlobalStaticFog = float4(0,0,0,0);
float4 _GlobalColor = float4(1,1,1,1);
float _EnableGlobalLighting = 0;

// Bezier
//StructuredBuffer<float4> g_BezierControlPoints;  // Replaces g_va0
//StructuredBuffer<float4> g_BezierColors;         // Replaces g_va1
//float4x4 g_mWorldViewProjection;
//float g_ZDepth; // z-depth constant used in Bezier patch

struct appdata
{
	float4 vertex    : POSITION;  // The vertex position in model space.
	float3 normal    : NORMAL;    // The vertex normal in model space.
	float4 texcoord  : TEXCOORD0; // The first UV coordinate.
	float4 texcoord1 : TEXCOORD1; // The second UV coordinate.
	float4 texcoord2 : TEXCOORD2; // The third UV coordinate.
	float4 texcoord3 : TEXCOORD3; // The fourth UV coordinate.
	float4 tangent   : TANGENT;   // The tangent vector in Model Space (used for normal mapping).
	float4 color     : COLOR;     // Per-vertex color
	UNITY_VERTEX_INPUT_INSTANCE_ID
};


struct v2f {
	float4 pos : SV_POSITION;
	float2 uv1 : TEXCOORD0; // The first UV coordinate.
	float4 uv2 : TEXCOORD1;
	float4 uv3 : TEXCOORD2;
	float2 uv4 : TEXCOORD3;
	float4 color : COLOR;
	float4 screenPos : TEXCOORD4;
	float2 separate_uv : TEXCOORD5;
	UNITY_VERTEX_INPUT_INSTANCE_ID
	//UNITY_FOG_COORDS(3)
};


float2 lerp2D(float2 a, float2 b, float t) {
    return a * (1 - t) + b * t;
}

v2f process_vert(appdata v) {
	v2f o;
	UNITY_SETUP_INSTANCE_ID(v);
	UNITY_TRANSFER_INSTANCE_ID(v, o);
	o.uv2 = v.texcoord1;
	o.uv3 = v.texcoord2;
	o.uv4 = v.texcoord3;
	
	float4 vertexAdd = float4(0.0 , 0.0, 0.0, 0.0);
	if(_VertexAnimParams.x >= 1) {
		// Based on Origins PC shader: frize_PNC3T_VS
		/*
		g_vconst1.x = cos( (timeCur *global_speed) +global_sync )
		g_vconst1.y = sin( (timeCur *global_speed) +global_sync )
		g_vconst1.z = cos( global_rotation )
		g_vconst1.w = sin( global_rotation )

		uv2.x = speed X vertex
		uv2.y = speed Y vertex
		uv2.z = cos( synchro_X_vertex ). RL: not cos
		uv2.w = sin( synchro_Y_vertex ). RL: not sin

		uv3.x = amplitude X vertex
		uv3.y = amplitude Y vertex
		uv3.z = cos( synchro_vertex). RL: not cos
		uv3.w = sin( synchro_vertex). RL: not sin

		uv4.x = cos( angle_vertex)
		uv4.y = sin( angle_vertex)
		*/
		// Animate vertex position
		float timeCur = _Time.y;
		float global_sync =  _VertexAnimParams.y;
		float global_speed =  _VertexAnimParams.z;
		float global_rotation = timeCur * _VertexAnimParams.w;
        float global_time = (timeCur * global_speed) + global_sync;
		float4 g_vconst1 = float4(
			cos( global_time ),
			sin( global_time ),
			cos( global_rotation ),
			sin( global_rotation ));

		float4 uv2 = o.uv2;
		float4 uv3 = o.uv3;
		float2 uv4 = o.uv4;
		if(_VertexAnimParams.x >= 2) { // RL
			uv2.z = cos(uv2.z);
			uv2.w = sin(uv2.w);
			float synchro_vertex = uv3.z;
			uv3.z = cos(synchro_vertex);
			uv3.w += sin(synchro_vertex);
		}

		float cosTime = g_vconst1.x * uv3.z - g_vconst1.y * uv3.w;                  
		float sinTime = g_vconst1.y * uv3.z + g_vconst1.x * uv3.w;
                     
		float x1 = (cosTime * uv2.x + uv2.z) * uv3.x;
		float y1 = (sinTime * uv2.y + uv2.w) * uv3.y;

		if(uv4.x == 0 && uv4.y == 0) {
			vertexAdd = float4(x1, y1, 0, 0);
		} else {
			float cosAngle = uv4.x * g_vconst1.z - uv4.y * g_vconst1.w;
			float sinAngle = uv4.x * g_vconst1.w + uv4.y * g_vconst1.z;

			float x2 = x1 * cosAngle - y1 * sinAngle;
			float y2 = x1 * sinAngle + y1 * cosAngle;
			vertexAdd = float4(x2, y2, 0, 0);
		}
	}

	
	float4 BezierParams = UNITY_ACCESS_INSTANCED_PROP(Props, _BezierParams);
    if (BezierParams.x == 1)
    {
		float BezierZ = BezierParams.z;

		StructuredBuffer<float4> g_BezierControlPoints = UNITY_ACCESS_INSTANCED_PROP(Props, _BezierControlPoints);
		StructuredBuffer<float4> g_BezierColors = UNITY_ACCESS_INSTANCED_PROP(Props, _BezierColors);

        int patchIndex = (int)v.vertex.z;
        int pos = patchIndex * 8;
        int colorpos = patchIndex * 4;

        float2 uv = v.texcoord.xy;
        float UvInv = 1 - uv.x;
        float carreinv = UvInv * UvInv;
        float carre = uv.x * uv.x;
        float cube = uv.x * uv.x * uv.x;
        float cubeinv = UvInv * UvInv * UvInv;

        // lerp for xy position
        float2 PF1 = lerp2D(g_BezierControlPoints[pos].xy, g_BezierControlPoints[pos + 4].xy, uv.y);
        float2 PF1X = lerp2D(g_BezierControlPoints[pos + 1].xy, g_BezierControlPoints[pos + 5].xy, uv.y);
        float2 PFX2 = lerp2D(g_BezierControlPoints[pos + 2].xy, g_BezierControlPoints[pos + 6].xy, uv.y);
        float2 PF2 = lerp2D(g_BezierControlPoints[pos + 3].xy, g_BezierControlPoints[pos + 7].xy, uv.y);

        float2 positionXY = PF1 * cubeinv + 3 * PF1X * uv.x * carreinv + 3 * PFX2 * carre * UvInv + PF2 * cube;
        float3 positionWorld = float3(positionXY.x, positionXY.y, BezierZ);
        //o.pos = mul(g_mWorldViewProjection, float4(positionWorld, 1));
        o.pos = UnityObjectToClipPos(float4(positionWorld, 1));
        //o.pos = mul(UNITY_MATRIX_VP, float4(positionWorld, 1));

        // Calculate UV (zw) similarly
        PF1 = lerp2D(g_BezierControlPoints[pos].zw, g_BezierControlPoints[pos + 4].zw, uv.y);
        PF1X = lerp2D(g_BezierControlPoints[pos + 1].zw, g_BezierControlPoints[pos + 5].zw, uv.y);
        PFX2 = lerp2D(g_BezierControlPoints[pos + 2].zw, g_BezierControlPoints[pos + 6].zw, uv.y);
        PF2 = lerp2D(g_BezierControlPoints[pos + 3].zw, g_BezierControlPoints[pos + 7].zw, uv.y);

        o.uv1 = PF1 * cubeinv + 3 * PF1X * uv.x * carreinv + 3 * PFX2 * carre * UvInv + PF2 * cube;
		o.uv1 = TRANSFORM_TEX(o.uv1, _Diffuse);

        // Color lerp
        float4 c0 = g_BezierColors[colorpos];
        float4 c1 = g_BezierColors[colorpos + 1];
        float4 c2 = g_BezierColors[colorpos + 2];
        float4 c3 = g_BezierColors[colorpos + 3];

        float4 l1 = lerp(c0, c1, uv.x);
        float4 l2 = lerp(c2, c3, uv.x);
        o.color = lerp(l1, l2, uv.y);
        //o.color = v.color;
    }
    else
    {
        // Original vertex position with animation
        o.pos = UnityObjectToClipPos(v.vertex + vertexAdd);
		
		// UV scrolling
        float2 uv = v.texcoord.xy;
        float4 TextureScrollParams = UNITY_ACCESS_INSTANCED_PROP(Props, _TextureScrollParams);
        float2 scrollOffset = TextureScrollParams.xy;
        uv += _Time.y * float2(-scrollOffset.x, scrollOffset.y);
		
		// Separate UV ("useUV2" in FriseTextureConfig)
        float2 separateUV = o.uv1;
        if (TextureScrollParams.w == 1)
        {
            separateUV = v.texcoord.xy;
            float4 TextureScrollParams2 = UNITY_ACCESS_INSTANCED_PROP(Props, _TextureScrollParams2);
            float2 uvScale = TextureScrollParams2.zw;
            separateUV *= uvScale;
            float2 scrollOffset2 = TextureScrollParams2.xy;
            separateUV += _Time.y * float2(-scrollOffset2.x, scrollOffset2.y);
        }

        // Pass through UV and color
        o.uv1 = TRANSFORM_TEX(uv, _Diffuse);
        o.color = v.color;
		
        o.separate_uv = TRANSFORM_TEX(separateUV, _Diffuse);
    }

    o.screenPos = ComputeScreenPos(o.pos);
	
	return o;
}
float4 process_frag(v2f i) : SV_TARGET{
	UNITY_SETUP_INSTANCE_ID(i);
	float4 UseTextures = UNITY_ACCESS_INSTANCED_PROP(Props, _UseTextures);
	float4 UseTextures2 = UNITY_ACCESS_INSTANCED_PROP(Props, _UseTextures2);
	float4 PrimitiveParams1 = UNITY_ACCESS_INSTANCED_PROP(Props, _PrimitiveParams1);

	float4 c = float4(0.0, 0.0, 0.0, 0.0);
	float4 ColorFog = float4(0.0, 0.0, 0.0, 0.0);
	if(PrimitiveParams1.x == 1) { // UseStaticFog
		ColorFog = UNITY_ACCESS_INSTANCED_PROP(Props, _ColorFog);
		if(_EnableGlobalLighting == 1 && PrimitiveParams1.y == 1 && _EnableLighting == 1) { // UseGlobalLighting
			ColorFog = _GlobalStaticFog;
			//ColorFog = float4(lerp(ColorFog.xyz, _GlobalStaticFog.w, _GlobalStaticFog.w);
		}
	}

	if (UseTextures.x == 1) {
		float4 ColorFactor = UNITY_ACCESS_INSTANCED_PROP(Props, _ColorFactor);
		float4 ShaderParams = UNITY_ACCESS_INSTANCED_PROP(Props, _ShaderParams);
		float4 ShaderParams2 = UNITY_ACCESS_INSTANCED_PROP(Props, _ShaderParams2);
		
		if(_EnableGlobalLighting == 1 && PrimitiveParams1.y == 1 && _EnableLighting == 1) { // UseGlobalLighting
			ColorFactor = float4(lerp(ColorFactor.xyz, _GlobalColor.xyz, _GlobalColor.w), ColorFactor.w);
		}

		float4 texColor = tex2D(_Diffuse, i.uv1);
		c = c + i.color * ColorFactor * texColor;

        if (ShaderParams.x == 1)
        {
            if (_EnableLighting == 1 && ShaderParams2.z != 0 /*&& ShaderParams.y == 0 && ShaderParams.z == 0*/)
            {
                /*float4 backLightTex = float4(0, 0, 0, 0);
                if (UseTextures.y == 1)
                {
                    backLightTex = tex2D(_BackLight, i.uv1);

					// Fix issue where backlight alpha is set to 1 by default if it's missing in DXT1 textures
                    if (UseTextures2.w == 1)
                    {
                        backLightTex.w = 0;
                    }
                }*/
				// Optimized version of above
                float4 backLightTex = UseTextures.y * tex2D(_BackLight, i.uv1);
                backLightTex.w *= 1 - UseTextures2.w;
				
                float2 screenPos = i.screenPos.xy / i.screenPos.w;
                float4 LightConfig = UNITY_ACCESS_INSTANCED_PROP(Props, _LightConfig); // Front brightness, front contrast, Back brightness, back contrast
                float3 frontLight = tex2D(_LightsFrontLight, screenPos).xyz * 2; //+ float3(0.5f, 0.5f, 0.5f); //* 2;
                float3 lightColor = float3(LightConfig.x, LightConfig.x, LightConfig.x) // Front brightness
					+ float3(frontLight * LightConfig.y); // Front contrast
                lightColor = lerp(lightColor, float3(1, 1, 1), float3(backLightTex.w, backLightTex.w, backLightTex.w));
                c = clamp(float4(c.xyz * lightColor, c.w), 0, 1);

                if (UseTextures.y == 1)
                {
                    float3 backLight = tex2D(_LightsBackLight, screenPos).xyz;
                    float3 backLightColor = float3(LightConfig.z, LightConfig.z, LightConfig.z) // Back brightness
						+ float3(backLight * LightConfig.w); // Back contrast
                    backLightColor = float3(max(0, backLightColor.x), max(0, backLightColor.y), max(0, backLightColor.z));
                    c = clamp(float4(c.xyz + backLightColor.xyz * backLightTex.xyz, c.w), 0, 1);
                }
            }
            if (UseTextures.w == 1)
            {
                c = float4(c.xyz, c.a * tex2D(_SeparateAlpha, i.separate_uv).a);
            }
        }
    }
    c.xyz = lerp(c.xyz, ColorFog.xyz, ColorFog.w);
	//c = lerp(c, float4(ColorFog.xyz, c.w), float4(ColorFog.w, ColorFog.w, ColorFog.w, ColorFog.w));

	return c;
}
#endif // SHARED_UBIART