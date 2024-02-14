Shader "Triniti/ModelTransparent5" {
Properties {
 _MainTex ("MainTex", 2D) = "" {}
}
SubShader { 
 Tags { "QUEUE"="Transparent+5" }
 Pass {
  Tags { "QUEUE"="Transparent+5" }
  Cull Off
  Blend SrcAlpha OneMinusSrcAlpha
  SetTexture [_MainTex] { combine texture, texture alpha }
 }
}
}