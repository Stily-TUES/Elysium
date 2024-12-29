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
    private Vector3 _velocity;

    public Physics(GameEntity owner) : base(owner)
    {
        _velocity = Vector3.Zero;
    }

    public void ApplyPhysics(float deltaTime, float mass)
    {
        float gravity = CalculateGravity(mass);
        _velocity.Y += gravity * deltaTime;
        ParentComponent.Transform.Position += _velocity * deltaTime;
    }

    public void Reset()
    {
        _velocity = Vector3.Zero;
    }

    private float CalculateGravity(float mass)
    {
        return -9.81f * mass;
    }
}
