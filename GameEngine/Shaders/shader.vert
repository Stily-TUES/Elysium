#version 330 core
layout(location = 0) in vec3 aPos;
layout(location = 1) in vec2 aTexCoord;

uniform mat4 modelMatrix;
uniform mat4 viewMatrix;
uniform mat4 projectionMatrix;
//uniform mat4 transform;
uniform bool isBackground;

out vec2 TexCoord;

void main()
{
    if (isBackground) {
		gl_Position = vec4(aPos, 1.0);
	} 
    else {
        gl_Position = projectionMatrix * viewMatrix * modelMatrix * vec4(aPos, 1.0); //gl_Position = transform * vec4(aPos, 1.0);
        TexCoord = aTexCoord;
    }
}