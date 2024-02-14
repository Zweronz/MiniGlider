Shader "Triniti/ParticlesAdditive" {
Properties {
 _TintColor ("Tint Color", Color) = (0.5,0.5,0.5,0.5)
 _MainTex ("Particle Texture", 2D) = "white" {}
}
SubShader { 
 Tags { "QUEUE"="Transparent+15" "IGNOREPROJECTOR"="True" "RenderType"="Transparent" }
 Pass {
  Tags { "QUEUE"="Transparent+15" "IGNOREPROJECTOR"="True" "RenderType"="Transparent" }
  BindChannels {
   Bind "vertex", Vertex
   Bind "color", Color
   Bind "texcoord", TexCoord
  }
  ZTest False
  ZWrite Off
  Cull Off
  Fog {
   Color (0,0,0,0)
  }
  Blend SrcAlpha One
  ColorMask RGB
  Offset 2, -1250
  SetTexture [_MainTex] { ConstantColor [_TintColor] combine constant * primary }
  SetTexture [_MainTex] { combine texture * previous double }
 }
}
SubShader { 
 Tags { "QUEUE"="Transparent+15" "IGNOREPROJECTOR"="True" "RenderType"="Transparent" }
 Pass {
  Tags { "QUEUE"="Transparent+15" "IGNOREPROJECTOR"="True" "RenderType"="Transparent" }
  BindChannels {
   Bind "vertex", Vertex
   Bind "color", Color
   Bind "texcoord", TexCoord
  }
  ZTest False
  ZWrite Off
  Cull Off
  Fog {
   Color (0,0,0,0)
  }
  Blend SrcAlpha One
  ColorMask RGB
  Offset 2, -1250
  SetTexture [_MainTex] { combine texture * primary }
 }
}
}