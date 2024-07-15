Shader"Custom/SurfaceDither"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _Transparency ("Transparency", Range(0,1)) = 1.0
    }
    SubShader
    {
        Tags
        {
            "RenderType"="Opaque"
        }
        Pass
        {
            CGPROGRAM
#include "UnityCG.cginc"
            #pragma vertex vert
            #pragma fragment frag
 
float4 _Color;
float _Transparency;
       
struct v2f
{
    float4 pos : POSITION;
    float4 col : COLOR;
    float4 spos : TEXCOORD1;
};
 
v2f vert(appdata_base v)
{
    v2f o;
    o.pos = UnityObjectToClipPos(v.vertex);
    o.spos = ComputeScreenPos(o.pos);
 
    return o;
}
 
float4 frag(v2f i) : COLOR
{
    float4 col = _Color;
           
    float2 pos = i.spos.xy / i.spos.w;
    pos *= _ScreenParams.xy;
 
                // Define a dither threshold matrix which can
                // be used to define how a 4x4 set of pixels
                // will be dithered
    float DITHER_THRESHOLDS[16] =
    {
        1.0 / 17.0, 9.0 / 17.0, 3.0 / 17.0, 11.0 / 17.0,
                    13.0 / 17.0, 5.0 / 17.0, 15.0 / 17.0, 7.0 / 17.0,
                    4.0 / 17.0, 12.0 / 17.0, 2.0 / 17.0, 10.0 / 17.0,
                    16.0 / 17.0, 8.0 / 17.0, 14.0 / 17.0, 6.0 / 17.0
    };
 
    int index = (int(pos.x) % 4) * 4 + int(pos.y) % 4;
    clip(_Transparency - DITHER_THRESHOLDS[index]);
           
    return col;
}
            ENDCG
        }
    }
Fallback"VertexLit"
}