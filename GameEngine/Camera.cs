
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using System;
using System.Numerics;
using System.Diagnostics;
using Vector2 = OpenTK.Mathematics.Vector2;
using Vector3 = OpenTK.Mathematics.Vector3;
using Vector4 = OpenTK.Mathematics.Vector4;

namespace GameEngine;

public class Camera
{
    public Vector2 Position { get; set; }
    public float Zoom { get; set; } = 0.5f;
    private float maxZoom = 50.0f;
    private float minZoom = 0.28f;
    private Vector2 targetPosition;
    private float moveSpeed = 5.0f;
    private float zoomSpeed = 0.5f;

    public Camera()
    {
        Position = Vector2.Zero;
        targetPosition = Position;
    }

    public void Move(Vector2 delta)
    {
        targetPosition += delta;
    }

    public void ZoomIn(float amount)
    {
        Zoom *= (float)Math.Pow(2, amount);
    }

    public void ZoomOut(float amount)
    {
        Zoom *= (float)Math.Pow(2, -amount);
    }

    public void Update()
    {
        Position = targetPosition;
    }

    public Matrix4 GetViewMatrix()
    {
        return Matrix4.CreateTranslation(-Position.X, -Position.Y, 0);
    }

    public Vector4 ScreenToGLSpace(Vector2 pos, float w, float h)
    {
        var res = new Vector4(pos.X / w * 2 - 1, -(pos.Y / h * 2 - 1), 0, 1);
        return res;
    }
    public Vector4 GLToScreenSpace(Vector4 pos, float w, float h)
    {
        return new Vector4(pos.X / 2 * w + 1, -(pos.Y / 2 * h + 1), 0, 1);
    }

    public Vector4 ScreenToWorldSpace(Vector2 pos, float w, float h)
    {
        var res = ScreenToGLSpace(pos, w, h) * GetProjectionMatrix(w / h).Inverted() * GetViewMatrix().Inverted();
        return res;
    }
    public Vector2 WorldToScreenSpace(Vector4 pos, float w, float h)
    {
        var res = GLToScreenSpace(pos * GetProjectionMatrix(w / h) * GetViewMatrix(), w, h);
        return res.Xy / res.W;
    }

    public Matrix4 GetProjectionMatrix(float aspectRatio)
    {
        return Matrix4.CreateScale(Zoom, Zoom, 1) * Matrix4.CreateOrthographic(aspectRatio, 1, -1, 1);
    }
}
