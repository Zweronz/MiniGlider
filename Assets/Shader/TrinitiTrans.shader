Shader "Triniti/Transparent" {
Properties {
 _MainTex ("MainTex", 2D) = "" {}
 _Color ("Color", Color) = (1,1,1,1)
}
SubShader { 
 Tags { "QUEUE"="Transparent" }
 Pass {
  Tags { "QUEUE"="Transparent" }
  Color [_Color]
  Cull Off
  Blend SrcAlpha OneMinusSrcAlpha
  SetTexture [_MainTex] { combine texture }
 }
}
}