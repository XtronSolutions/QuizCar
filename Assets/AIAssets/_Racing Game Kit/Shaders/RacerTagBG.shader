// From http://www.unifycommunity.com/wiki/index.php?title=Unlit
// Thanks for sharing Jaap Kreijkamp!

Shader "Racing Game Kit/RacerTagBG" {

    Properties {
        _Color ("Main Color", Color) = (1,1,1,1)
        _MainTex ("Base (RGB)", 2D) = "white" {}
    }
    Category {
       Lighting Off
       ZWrite On
       Cull Back
       SubShader {
            Pass {
               SetTexture [_MainTex] {
                    constantColor [_Color]
                    Combine texture * constant, texture * constant
                 }
            }
        }
    }
}