using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;

using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL4;

namespace GameEngine;

public class Renderer
{
    private int shaderProgram;
    public static void Main()
    {

    }
    public Renderer()
    {
        string vertexShaderSource = File.ReadAllText("../../../../GameEngine/Shaders/shader.vert");
        string fragmentShaderSource = File.ReadAllText("../../../../GameEngine/Shaders/shader.frag");

        int vertexShader = CompileShader(ShaderType.VertexShader, vertexShaderSource);
        int fragmentShader = CompileShader(ShaderType.FragmentShader, fragmentShaderSource);

        shaderProgram = GL.CreateProgram();
        GL.AttachShader(shaderProgram, vertexShader);
        GL.AttachShader(shaderProgram, fragmentShader);
        GL.LinkProgram(shaderProgram);

        GL.DeleteShader(vertexShader);
        GL.DeleteShader(fragmentShader);
    }

    private int CompileShader(ShaderType type, string source)
    {
        int shader = GL.CreateShader(type);
        GL.ShaderSource(shader, source);
        GL.CompileShader(shader);


        GL.GetShader(shader, ShaderParameter.CompileStatus, out int success);
        if (success == 0)
        {
            string infoLog = GL.GetShaderInfoLog(shader);
            throw new Exception($"Error compiling {type}: {infoLog}");
        }

        return shader;
    }
    public void drawSquare(double x1, double y1, double z1, double sidelength)
    {
        double x2 = x1 + sidelength;
        double y2 = y1 + sidelength;

        float[] vertices = {
            (float)x1, (float)y1, (float)z1,
            (float)x2, (float)y1, (float)z1, 
            (float)x2, (float)y2, (float)z1, 
            (float)x1, (float)y2, (float)z1
        };

        uint[] indices = {
            0, 1, 2, 
            2, 3, 0  
        };

        int VBO = GL.GenBuffer();
        GL.BindBuffer(BufferTarget.ArrayBuffer, VBO);
        GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * sizeof(float), vertices, BufferUsageHint.StaticDraw);

        int EBO = GL.GenBuffer();
        GL.BindBuffer(BufferTarget.ElementArrayBuffer, EBO);
        GL.BufferData(BufferTarget.ElementArrayBuffer, indices.Length * sizeof(uint), indices, BufferUsageHint.StaticDraw);

        int VAO = GL.GenVertexArray();
        GL.BindVertexArray(VAO);

        GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
        GL.EnableVertexAttribArray(0);

        GL.BindBuffer(BufferTarget.ArrayBuffer, VBO);
        GL.BindBuffer(BufferTarget.ElementArrayBuffer, EBO);
        GL.UseProgram(shaderProgram);

        GL.DrawElements(PrimitiveType.Triangles, indices.Length, DrawElementsType.UnsignedInt, 0);

        GL.DisableVertexAttribArray(0);
        GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
        GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);
        GL.BindVertexArray(0);

        GL.DeleteBuffer(VBO);
        GL.DeleteBuffer(EBO);
        GL.DeleteVertexArray(VAO);
    }
}
