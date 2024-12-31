using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Editor.Components;

[DataContract]
public class Physics : Component
{
    public Vector3 Velocity;
    public bool IsColliding { get; set; }

    public Physics(GameEntity owner) : base(owner)
    {
        Velocity = Vector3.Zero;
    }

    public void ApplyGravityPhysics(float deltaTime, float mass)
    {
        if (!IsColliding)
        {
            float gravity = CalculateGravity(mass);
            Velocity.Y += gravity * deltaTime;
        }
        ParentComponent.Transform.Position += Velocity * deltaTime;
    }

    public void Reset()
    {
        Velocity = Vector3.Zero;
    }

    private float CalculateGravity(float mass)
    {
        return -9.81f * mass;
    }
}
