using Editor.Components;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace Editor.Utils;
public class PhysicsManager
{
    private readonly List<GameEntity> _entities;
    private readonly Dictionary<GameEntity, Vector3> _initialPositions;

    public PhysicsManager(List<GameEntity> entities)
    {
        _entities = entities;
        _initialPositions = entities.ToDictionary(e => e, e => e.Transform.Position);
    }

    public void OnTick(float deltaTime)
    {
        foreach (var entity in _entities)
        {
            entity.ApplyPhysics(deltaTime);
        }
        CheckCollisions();

        // Reset collision state
        foreach (var entity in _entities)
        {
            entity.Physics.IsColliding = false;
        }
    }
    private void CheckCollisions()
    {
        for (int i = 0; i < _entities.Count; i++)
        {
            for (int j = i + 1; j < _entities.Count; j++)
            {
                var entityA = _entities[i];
                var entityB = _entities[j];

                if (entityA.IsActive && entityB.IsActive && entityA.HasCollision && entityB.HasCollision)
                {
                    if (entityA.CheckCollision(entityB))
                    {
                        ResolveCollision(entityA, entityB);
                    }
                }
            }
        }
    }

    private void ResolveCollision(GameEntity entityA, GameEntity entityB)
    {
        Vector3 direction = entityA.Transform.Position - entityB.Transform.Position;
        direction.Normalize();

        float distance = Vector3.Distance(entityA.Transform.Position, entityB.Transform.Position);
        float overlap = 1.0f - distance;

        entityA.Transform.Position += direction * overlap * 0.5f;
        entityB.Transform.Position -= direction * overlap * 0.5f;

        if (entityA.HasGravity)
        {
            entityA.Physics.Velocity = Vector3.Zero;
            entityA.Physics.IsColliding = true;
        }
        if (entityB.HasGravity)
        {
            entityB.Physics.Velocity = Vector3.Zero;
            entityB.Physics.IsColliding = true;
        }

        float minSeparation = 0.01f;
        if (Vector3.Distance(entityA.Transform.Position, entityB.Transform.Position) < minSeparation)
        {
            Vector3 correction = direction * (minSeparation - distance) * 0.5f;
            entityA.Transform.Position += correction;
            entityB.Transform.Position -= correction;
        }
    }


    public void Stop()
    {
        foreach (var entity in _entities)
        {
            entity.ResetPhysics();
            entity.Transform.Position = _initialPositions[entity];
        }
    }
}
