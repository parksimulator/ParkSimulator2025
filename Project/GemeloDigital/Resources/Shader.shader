#version 330 core
layout (location = 0) in vec3 vPosition;
layout (location = 1) in vec2 vUv;
layout (location = 2) in vec3 vNormal;

uniform mat4 uModel;
uniform mat4 uView;
uniform mat4 uProjection;

out vec2 fUv;
out vec3 fNormal;

void main()
{
    //Multiplying our uniform with the vertex position, the multiplication order here does matter.
    gl_Position = uProjection * uView * uModel * vec4(vPosition, 1.0);
    fNormal = (uModel * vec4(vNormal, 0)).xyz;
    fUv = vUv;
}
----
#version 330 core
in vec2 fUv;
in vec3 fNormal;

uniform sampler2D uTexture0;

out vec4 fOut;

void main()
{
    fOut = texture(uTexture0, fUv);    
}
