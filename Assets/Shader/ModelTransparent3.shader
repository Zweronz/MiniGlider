Shader "Triniti/ModelTransparent3" {
Properties {
 _MainTex ("MainTex", 2D) = "" {}
}
SubShader { 
 Tags { "QUEUE"="Transparent+3" }
 Pass {
  Tags { "QUEUE"="Transparent+3" }
  Cull Off
  Blend SrcAlpha OneMinusSrcAlpha
  SetTexture [_MainTex] { combine texture, texture alpha }
 }
}
}