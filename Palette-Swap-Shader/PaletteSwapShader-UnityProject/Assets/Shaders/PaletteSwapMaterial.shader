Shader "Custom/PaletteSwapMaterial"{

//Author : Francisco Antonio Mora Arambulo

	Properties
	{
		[PerRendererData] _MainTex("Texture", 2D) = "white" {}		
		_BasePaletteTex("BasePalette",2D) = "white"{}
		_SwapPaletteTex("Palette Swap Texture",2D) = "white"{}
		_SwapTexColorNumber("Swap color Number",float) = 1
		_Threshold("Swap Tolerance",Range(0.0,0.2)) = 0
	}

	SubShader
	{
		Tags{ "Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Transparent" }

		ZWrite Off
		Cull Off
		Blend SrcAlpha OneMinusSrcAlpha

		CGPROGRAM
		#pragma surface surf Lambert alpha:blend nofog nolightmap noshadow
		#pragma target 3.0
		
		struct Input {
			fixed2 uv_MainTex;
			fixed4 color : COLOR;
		};

		sampler2D _MainTex;
		sampler2D _BasePaletteTex;
		sampler2D _SwapPaletteTex;
		float _Threshold;
		float _SwapTexColorNumber;

		float dis(float4 c1,float4 c2) 
		{
			float result = sqrt(pow(c1.r - c2.r, 2.0) + pow(c1.g - c2.g, 2.0) + pow(c1.b - c2.b, 2.0));
			return result;
		}

		fixed4 getReplacementColor(fixed4 col)
		{
			bool found = false;
			float curDis,minDis=1000; 
			fixed swapIndex;
			fixed4 newColor,tmpColor;
			for(int i = 0 ; i < _SwapTexColorNumber ; i++)
			{
				tmpColor =  tex2Dlod(_BasePaletteTex,float4(i/_SwapTexColorNumber,0,0,0));
				curDis = dis(col,tmpColor);
				if (curDis <= _Threshold && curDis < minDis)
				{	
					minDis = curDis;
					swapIndex=i;
					found = true;
				}
			}

			if (found)
			{
				newColor = tex2D(_SwapPaletteTex, float2(swapIndex / _SwapTexColorNumber, 0));
				newColor.a = col.a;
			}
			else
			{
				newColor = col;
			}
			return newColor;
		}

		void surf(Input IN, inout SurfaceOutput o) 
		{
			fixed4 col = tex2D(_MainTex, IN.uv_MainTex);
			fixed4 editorTest = tex2D(_SwapPaletteTex,float2(0,0));
			fixed4 swapedColor;
			if(editorTest.a==1 && (_Threshold>0))
			{
				swapedColor = getReplacementColor(col);
			}
			else
			{
				swapedColor = col;
			}
			fixed4 finalSwap = swapedColor;

			o.Albedo = finalSwap.rgb * IN.color.rgb;
			o.Alpha = col.a * IN.color.a;
		}
		ENDCG
	}
}
