using System;
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
    public static void Main() { }
    public Renderer()
    {
        shaderProgram = CreateShaderProgram("../../../../GameEngine/Shaders/shader.vert", "../../../../GameEngine/Shaders/shader.frag");
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

    private Matrix4 CreateModelMatrix(Vector3 position, Vector3 rotation, Vector3 scale, Vector3 center)
    {
        Matrix4 scaleMatrix = Matrix4.CreateScale(scale);
        Matrix4 rotationX = Matrix4.CreateRotationX(MathHelper.DegreesToRadians(rotation.X));
        Matrix4 rotationY = Matrix4.CreateRotationY(MathHelper.DegreesToRadians(rotation.Y));
        Matrix4 rotationZ = Matrix4.CreateRotationZ(MathHelper.DegreesToRadians(rotation.Z));
        Matrix4 rotationMatrix = rotationX * rotationY * rotationZ;
        Matrix4 translationMatrix = Matrix4.CreateTranslation(position);

        return Matrix4.CreateTranslation(-center) * rotationMatrix * Matrix4.CreateTranslation(center) * scaleMatrix  * translationMatrix;
    }

    public void DrawSquare(Vector3 position, double sideLength, string texturePath, Vector3 rotation, Vector3 scale)//, Matrix4 viewMatrix, Matrix4 projectionMatrix)
    {
        double x1 = position.X;
        double y1 = position.Y;
        double z1 = position.Z;
        double x2 = x1 + sideLength;
        double y2 = y1 + sideLength;

        float[] vertices = {
            // positions          // texture coords
            (float)x1, (float)y1, (float)z1,  0.0f, 0.0f,
            (float)x2, (float)y1, (float)z1,  1.0f, 0.0f,
            (float)x2, (float)y2, (float)z1,  1.0f, 1.0f,
            (float)x1, (float)y2, (float)z1,  0.0f, 1.0f
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

        GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 5 * sizeof(float), 0);
        GL.EnableVertexAttribArray(0);

        GL.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, 5 * sizeof(float), 3 * sizeof(float));
        GL.EnableVertexAttribArray(1);

        GL.UseProgram(shaderProgram);

        Vector3 center = new Vector3((float)(x1 + x2) / 2, (float)(y1 + y2) / 2, (float)z1);

        Matrix4 modelMatrix = CreateModelMatrix(position, rotation, scale, center);

        Matrix4 transformMatrix = modelMatrix;
        //Matrix4 transformMatrix = projectionMatrix * viewMatrix * modelMatrix;

        int transformLoc = GL.GetUniformLocation(shaderProgram, "transform");
        GL.UniformMatrix4(transformLoc, false, ref transformMatrix);

        if (!string.IsNullOrEmpty(texturePath))
        {
            int textureId = LoadTexture(texturePath);
            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, textureId);
            GL.Uniform1(GL.GetUniformLocation(shaderProgram, "texture1"), 0);
            GL.Uniform1(GL.GetUniformLocation(shaderProgram, "useTexture"), 1);
        }
        else
        {
            GL.Uniform1(GL.GetUniformLocation(shaderProgram, "useTexture"), 0);
        }

        GL.BindVertexArray(VAO);
        GL.BindBuffer(BufferTarget.ElementArrayBuffer, EBO);
        GL.DrawElements(PrimitiveType.Triangles, indices.Length, DrawElementsType.UnsignedInt, IntPtr.Zero);

        GL.DisableVertexAttribArray(0);
        GL.DisableVertexAttribArray(1);
        GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
        GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);
        GL.BindVertexArray(0);

        GL.DeleteBuffer(VBO);
        GL.DeleteBuffer(EBO);
        GL.DeleteVertexArray(VAO);
    }

    public void RenderBackground(string texturePath)
    {
        float[] vertices = {
        -1.0f, -1.0f, 0.0f,  0.0f, 1.0f, 
         1.0f, -1.0f, 0.0f,  1.0f, 1.0f, 
         1.0f,  1.0f, 0.0f,  1.0f, 0.0f, 
        -1.0f,  1.0f, 0.0f,  0.0f, 0.0f  
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

        GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 5 * sizeof(float), 0);
        GL.EnableVertexAttribArray(0);

        GL.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, 5 * sizeof(float), 3 * sizeof(float));
        GL.EnableVertexAttribArray(1);

        GL.UseProgram(shaderProgram);

        int isBackgroundLoc = GL.GetUniformLocation(shaderProgram, "isBackground");
        GL.Uniform1(isBackgroundLoc, 1);

        if (!string.IsNullOrEmpty(texturePath))
        {
            int textureId = LoadTexture(texturePath);
            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, textureId);
            GL.Uniform1(GL.GetUniformLocation(shaderProgram, "texture1"), 0);
            GL.Uniform1(GL.GetUniformLocation(shaderProgram, "useTexture"), 1);
        }
        else
        {
            GL.Uniform1(GL.GetUniformLocation(shaderProgram, "useTexture"), 0);
        }

        GL.BindVertexArray(VAO);
        GL.BindBuffer(BufferTarget.ElementArrayBuffer, EBO);
        GL.DrawElements(PrimitiveType.Triangles, indices.Length, DrawElementsType.UnsignedInt, IntPtr.Zero);

        GL.DisableVertexAttribArray(0);
        GL.DisableVertexAttribArray(1);
        GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
        GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);
        GL.BindVertexArray(0);

        GL.DeleteBuffer(VBO);
        GL.DeleteBuffer(EBO);
        GL.DeleteVertexArray(VAO);
    }


    public int LoadTexture(string path)
    {
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
