Shader "Triniti/ModelTransparent1" {
Properties {
 _MainTex ("MainTex", 2D) = "" {}
}
SubShader { 
 Tags { "QUEUE"="Transparent+1" }
 Pass {
  Tags { "QUEUE"="Transparent+1" }
  Cull Off
  Blend SrcAlpha OneMinusSrcAlpha
  SetTexture [_MainTex] { combine texture, texture alpha }
 }
}
}