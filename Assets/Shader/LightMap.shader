Shader "Triniti/LightMap" {
Properties {
 _texBase ("MainTex", 2D) = "" {}
 _texLightmap ("LightMap", 2D) = "" {}
}
SubShader { 
 Pass {
  Tags { "QUEUE"="Background" }
  BindChannels {
   Bind "vertex", Vertex
   Bind "texcoord", TexCoord0
   Bind "texcoord1", TexCoord1
  }
  SetTexture [_texBase] { combine texture }
  SetTexture [_texLightmap] { combine previous * texture }
 }
}
}