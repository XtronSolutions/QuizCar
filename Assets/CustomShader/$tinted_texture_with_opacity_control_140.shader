Shader "My Shaders/Tinted Texture with Opacity Control" {


Properties
{
	_Opacity ("Opacity", Range (0, 1) ) = 1
	_Color ("Tint Color", Color) = (1,1,1)
	_MainTex ("Texture  (RGB)", 2D) = ""
}

SubShader 
{
	Tags {Queue = Transparent}
	ZWrite Off
	Colormask RGB
	Blend SrcAlpha OneMinusSrcAlpha
	
	Color [_Color]
	Pass
	{		
		SetTexture [_MainTex]
		{
			ConstantColor (0,0,0, [_Opacity])
			combine texture * primary, constant
		}				
	}
}

 
}