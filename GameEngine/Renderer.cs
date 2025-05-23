﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using Vector3 = OpenTK.Mathematics.Vector3;

namespace GameEngine;

public class Renderer
{
    private int shaderProgram;
    private int _backgroundTextureId;
    public static void Main() { }
    public Renderer(string basePath)
    {
        shaderProgram = CreateShaderProgram(
            Path.Combine(basePath, "GameEngine", "Shaders", "shader.vert"),
            Path.Combine(basePath, "GameEngine", "Shaders", "shader.frag")
        );
    }

    private int CreateShaderProgram(string vertexPath, string fragmentPath)
    {
        string vertexShaderSource = File.ReadAllText(vertexPath);
        string fragmentShaderSource = File.ReadAllText(fragmentPath);

        int vertexShader = CompileShader(ShaderType.VertexShader, vertexShaderSource);
        int fragmentShader = CompileShader(ShaderType.FragmentShader, fragmentShaderSource);

        int program = GL.CreateProgram();
        GL.AttachShader(program, vertexShader);
        GL.AttachShader(program, fragmentShader);
        GL.LinkProgram(program);

        GL.DeleteShader(vertexShader);
        GL.DeleteShader(fragmentShader);

        return program;
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

    public void RenderMesh(int textureId, Mesh mesh, Matrix4 modelMatrix, Matrix4 viewMatrix, Matrix4 projectionMatrix)
    {
        GL.UseProgram(shaderProgram);

        //Matrix4 transformMatrix = modelMatrix * viewMatrix * projectionMatrix;
        //int transformLoc = GL.GetUniformLocation(shaderProgram, "transform");
        int modelLoc = GL.GetUniformLocation(shaderProgram, "modelMatrix");
        int viewLoc = GL.GetUniformLocation(shaderProgram, "viewMatrix");
        int projectionLoc = GL.GetUniformLocation(shaderProgram, "projectionMatrix");
        GL.UniformMatrix4(modelLoc, false, ref modelMatrix);
        GL.UniformMatrix4(viewLoc, false, ref viewMatrix);
        GL.UniformMatrix4(projectionLoc, false, ref projectionMatrix);
        //GL.UniformMatrix4(transformLoc, false, ref transformMatrix);
        mesh.Render(shaderProgram, textureId);
    }
    public void RenderBackground(Mesh mesh, string texturePath)
    {
        if (_backgroundTextureId <= 0 )
        {
            _backgroundTextureId = LoadTexture(texturePath);
        }
        GL.UseProgram(shaderProgram);

        int isBackgroundLoc = GL.GetUniformLocation(shaderProgram, "isBackground");
        GL.Uniform1(isBackgroundLoc, 1);

        Matrix4 modelMatrix = Matrix4.Identity;
        Matrix4 viewMatrix = Matrix4.Identity;
        Matrix4 projectionMatrix = Matrix4.Identity;

        RenderMesh(_backgroundTextureId, mesh, modelMatrix, viewMatrix, projectionMatrix);
    }

    public static int LoadTexture(string path)
    {
        if (path == null) return -1;

        int textureId = GL.GenTexture();
        GL.BindTexture(TextureTarget.Texture2D, textureId);

        Bitmap bitmap = new Bitmap(path);
        BitmapData data = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

        GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, data.Width, data.Height, 0, (OpenTK.Graphics.OpenGL4.PixelFormat)OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, data.Scan0);
        bitmap.UnlockBits(data);

        GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);

        return textureId;
    }
}
