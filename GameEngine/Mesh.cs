using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.Serialization;
using static OpenTK.Graphics.OpenGL.GL;

public class Mesh
{
    private int VAO, VBO, EBO;

    public Mesh(float[] vertices, uint[] indices, string texturePath = null)
    {
        VAO = GL.GenVertexArray();
        VBO = GL.GenBuffer();
        EBO = GL.GenBuffer();

        GL.BindVertexArray(VAO);

        GL.BindBuffer(BufferTarget.ArrayBuffer, VBO);
        GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * sizeof(float), vertices, BufferUsageHint.StaticDraw);

        GL.BindBuffer(BufferTarget.ElementArrayBuffer, EBO);
        GL.BufferData(BufferTarget.ElementArrayBuffer, indices.Length * sizeof(uint), indices, BufferUsageHint.StaticDraw);

        GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 5 * sizeof(float), 0);
        GL.EnableVertexAttribArray(0);

        GL.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, 5 * sizeof(float), 3 * sizeof(float));
        GL.EnableVertexAttribArray(1);

        GL.BindVertexArray(0);
    }

    public void Render(int shaderProgram, int textureId)
    {
        if (textureId > 0)
        {
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
        GL.DrawElements(PrimitiveType.Triangles, 6, DrawElementsType.UnsignedInt, 0);
        GL.BindVertexArray(0);
    }

    public void Dispose()
    {
        GL.DeleteVertexArray(VAO);
        GL.DeleteBuffer(VBO);
        GL.DeleteBuffer(EBO);
    }

    public static Mesh CreateSquare(float sideLength)
    {
        float halfSide = sideLength / 2;

        float[] vertices = {
            // positions           // texture coords
            -halfSide, -halfSide, 0.0f,  0.0f, 1.0f,
            halfSide, -halfSide, 0.0f,  1.0f, 1.0f,
            halfSide,  halfSide, 0.0f,  1.0f, 0.0f,
            -halfSide,  halfSide, 0.0f,  0.0f, 0.0f
        };

        uint[] indices = {
            0, 1, 2,
            2, 3, 0
        };

        return new Mesh(vertices, indices);
    }

    //public static Mesh CreateCircle(float radius, int segments, string texturePath = null)
    //{
    //    List<float> vertices = new();
    //    List<uint> indices = new();

    //    vertices.AddRange(new float[] { 0.0f, 0.0f, 0.0f, 0.5f, 0.5f });

    //    for (int i = 0; i <= segments; i++)
    //    {
    //        float angle = MathHelper.TwoPi * i / segments;
    //        float x = radius * MathF.Cos(angle);
    //        float y = radius * MathF.Sin(angle);

    //        vertices.AddRange(new float[] { x, y, 0.0f, (x / (2 * radius)) + 0.5f, (y / (2 * radius)) + 0.5f });

    //        if (i > 0)
    //        {
    //            indices.Add(0);
    //            indices.Add((uint)i);
    //            indices.Add((uint)(i + 1));
    //        }
    //    }

    //    return new Mesh(vertices.ToArray(), indices.ToArray(), texturePath);
    //}
}
