Shader "emofani/AlphaBlendedOneSided" {
	Properties {
		_BlushIntensity ("Blush Intensity", Range (0,1)) = 0.5
		_MainTex ("Main Texture", 2D) = "white" {}
		_BlushTex("Blush Texture", 2D) = "white" {}
	}

	Category {
		Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }
		Blend SrcAlpha OneMinusSrcAlpha
		Cull Back Lighting Off ZWrite On Fog { Color (0,0,0,0) }
	
		BindChannels {
			Bind "Color", color
			Bind "Vertex", vertex
			Bind "TexCoord", texcoord
		}
	
		SubShader {
			Pass {
				SetTexture [_MainTex] {
					combine texture * primary
				}
				SetTexture[_BlushTex]{
					ConstantColor(0,0,0,[_BlushIntensity])
					combine texture Lerp(constant) previous
				}
			}
		}
	}
}
