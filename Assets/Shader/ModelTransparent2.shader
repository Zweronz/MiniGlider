Shader "Triniti/ModelTransparent2" {
Properties {
 _MainTex ("MainTex", 2D) = "" {}
}
SubShader { 
 Tags { "QUEUE"="Transparent+2" }
 Pass {
  Tags { "QUEUE"="Transparent+2" }
  Cull Off
  Blend SrcAlpha OneMinusSrcAlpha
  SetTexture [_MainTex] { combine texture, texture alpha }
 }
}
}