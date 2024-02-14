Shader "Crazy/Shader_Particle-Additive-Effect" {
Properties {
 _TintColor ("Tint Color", Color) = (0.5,0.5,0.5,0.5)
 _MainTex ("Particle Texture", 2D) = "white" {}
}
SubShader { 
 Tags { "QUEUE"="Transparent+8" "IGNOREPROJECTOR"="True" "RenderType"="Transparent" }
 Pass {
  Tags { "QUEUE"="Transparent+8" "IGNOREPROJECTOR"="True" "RenderType"="Transparent" }
  BindChannels {
   Bind "vertex", Vertex
   Bind "color", Color
   Bind "texcoord", TexCoord
  }
  ZWrite Off
  Cull Off
  Fog {
   Color (0,0,0,0)
  }
  Blend SrcAlpha One
  Offset 0, -1250
  SetTexture [_MainTex] { ConstantColor [_TintColor] combine constant * primary }
  SetTexture [_MainTex] { combine texture * previous double }
 }
}
}