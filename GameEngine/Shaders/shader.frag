#version 330 core

out vec4 FragColor;

in vec2 TexCoord;

uniform sampler2D texture1;
uniform bool useTexture;

void main()
{
    if (useTexture)
    {
        FragColor = texture(texture1, TexCoord);
    }
    else
    {
        FragColor = vec4(1.0, 1.0, 1.0, 1.0); 
    }
}