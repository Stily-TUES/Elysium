using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Editor.Components;

[DataContract]
public class Physics : Component
{
    public Vector3 velocity;
    [DataMember]
    private readonly float _gravity = -9.81f;
    public bool IsColliding { get; set; }

    public Physics(GameEntity owner) : base(owner)
    {
        velocity = Vector3.Zero;
    }

    public void ApplyGravityPhysics(float deltaTime, float mass)
    {
        if (!IsColliding)
        {
            float gravity = CalculateGravity(mass);
            velocity.Y += gravity * deltaTime;
        }
        ParentEntity.Transform.Position += velocity * deltaTime;
    }

    public void Reset()
    {
        velocity = Vector3.Zero;
    }

    private float CalculateGravity(float mass)
    {
        return _gravity * mass;
    }
}
