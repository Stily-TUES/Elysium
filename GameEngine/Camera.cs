
using OpenTK.Graphics.OpenGL4;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Vector2 = System.Numerics.Vector2;




namespace GameEngine;

public class Camera
{
    public Vector2 Position { get; set; }
    public float Zoom { get; set; } = 1.0f;
    private Vector2 targetPosition;
    private float moveSpeed = 5.0f;
    private float zoomSpeed = 0.1f;
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
        Zoom += amount;
    }

    public void ZoomOut(float amount)
    {
        Zoom -= amount;
    }

    public void Update(float deltaTime)
    {
        Position = targetPosition;
    }
    //public Matrix4 GetViewMatrix()
    //{
    //    return Matrix4.LookAt(Position, targetPosition, Up);
    //}
    //public Matrix4 GetProjectionMatrix(float aspectRatio, float fov = 45.0f, float near = 0.1f, float far = 100.0f)
    //{
    //    return Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(fov), aspectRatio, near, far);
    //}
}
