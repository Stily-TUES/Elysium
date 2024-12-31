﻿using Editor.Components;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
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
        //Debug.WriteLine("PhysicsManager.OnTick");
        foreach (var entity in _entities)
        {
            entity.ApplyPhysics(deltaTime);
            CheckCollisions(entity, deltaTime);
        }
        

        //// Reset collision state
        //foreach (var entity in _entities)
        //{
        //    entity.Physics.IsColliding = false;
        //}
    }
    private void CheckCollisions(GameEntity entityA, float deltaTime)
    {

        for (int j = 0; j < _entities.Count; j++)
        {
            var entityB = _entities[j];
            if (entityA == entityB)
            {
                continue;
            }

            //if (entityA.IsActive && entityB.IsActive && entityA.HasCollision && entityB.HasCollision)
            //{
                if (entityA.CheckCollision(entityB))
                {
                    ResolveCollision(entityA, entityB, deltaTime);
                    break;
                }
            //}
        }
    }

    private void ResolveCollision(GameEntity entityA, GameEntity entityB, float deltaTime)
    {
        entityA.Transform.Position -= entityA.Physics.Velocity * deltaTime;
        entityB.Transform.Position -= entityB.Physics.Velocity * deltaTime;
        entityA.Physics.Velocity = Vector3.Zero; 
        entityB.Physics.Velocity = Vector3.Zero;
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
