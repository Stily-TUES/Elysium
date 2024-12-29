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
