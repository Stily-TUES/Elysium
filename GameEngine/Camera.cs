
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Vector2 = System.Numerics.Vector2;
using Vector3 = OpenTK.Mathematics.Vector3;

namespace GameEngine;

public class Camera
{
    public Vector2 Position { get; set; }
    public float Zoom { get; set; } = 0.5f;
    private float maxZoom = 50.0f;
    private float minZoom = 0.28f;
    private Vector2 targetPosition;
    private float moveSpeed = 5.0f;
    private float zoomSpeed = 0.05f;
    public Vector3 Up { get; set; } = Vector3.UnitY;

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

    public void Update(float deltaTime)
    {
        Position = targetPosition;
    }

    public Matrix4 GetViewMatrix()
    {
        return Matrix4.CreateTranslation(-Position.X, -Position.Y, 0);
    }
        return Matrix4.LookAt(cameraPosition, cameraTarget, Up);
    }

    public Matrix4 GetProjectionMatrix(float aspectRatio)
    {
        return Matrix4.CreateScale(Zoom, Zoom, 1) * Matrix4.CreateOrthographic(aspectRatio, 1, -1, 1);
    }
}
