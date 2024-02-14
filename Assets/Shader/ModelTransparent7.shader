Shader "Triniti/ModelTransparent7" {
Properties {
 _MainTex ("MainTex", 2D) = "" {}
}
SubShader { 
 Tags { "QUEUE"="Transparent+7" }
 Pass {
  Tags { "QUEUE"="Transparent+7" }
  Cull Off
  Blend SrcAlpha OneMinusSrcAlpha
  SetTexture [_MainTex] { combine texture, texture alpha }
 }
}
}