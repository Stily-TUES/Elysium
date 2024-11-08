using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL4;

namespace GameEngine;

public class Renderer
{
    public static void Main()
    {

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
