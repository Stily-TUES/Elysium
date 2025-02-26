using Editor.Components;
using Editor.Scripting;
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
    private bool _isSimulationRunning;
    private float gridSize;
    private readonly ScriptManager _scriptManager;

    public PhysicsManager(List<GameEntity> entities)
    {
        _entities = entities;
        _initialPositions = entities.ToDictionary(e => e, e => e.Transform.Position);
        _scriptManager = ScriptManager.Instance;
    }

    public void OnTick(float deltaTime)
    {
        if (!_isSimulationRunning)
            return;
        var grid = new Dictionary<(int, int), List<GameEntity>>();
        foreach (var entity in _entities)
        {
            var gridPos = (x: (int)(entity.Transform.Position.X / gridSize), y: (int)(entity.Transform.Position.Y / gridSize));
            if (!grid.ContainsKey(gridPos))
            {
                grid[gridPos] = new List<GameEntity>();
            }
            grid[gridPos].Add(entity);
        }

        foreach (var entity in _entities)
        {
            _scriptManager.UpdateScripts(entity, deltaTime);
            entity.ApplyPhysics(deltaTime);
            CheckCollisions(entity, deltaTime, grid);
        }
    }

    private void CheckCollisions(GameEntity entityA, float deltaTime, Dictionary<(int, int), List<GameEntity>> grid)
    {
        var entityAGridPos = (x: (int)(entityA.Transform.Position.X / gridSize), y: (int)(entityA.Transform.Position.Y / gridSize));
        if (grid.ContainsKey(entityAGridPos))
        {
            foreach (var entityB in grid[entityAGridPos])
            {
                if (entityA != entityB && entityA.CheckCollision(entityB))
                {
                    ResolveCollision(entityA, entityB, deltaTime);
                    break;
                }
            }
        }
    }

    private void ResolveCollision(GameEntity entityA, GameEntity entityB, float deltaTime)
    {
        entityA.Transform.Position -= entityA.Physics.velocity * deltaTime;
        entityB.Transform.Position -= entityB.Physics.velocity * deltaTime;
        entityA.Physics.velocity = Vector3.Zero;
        entityB.Physics.velocity = Vector3.Zero;
    }

    public void Stop()
    {
        foreach (var entity in _entities)
        {
            if (entity.HasGravity || entity.Scripts.Count > 0)
            {
                entity.ResetPhysics();
                entity.Transform.Position = _initialPositions[entity];
            }
        }
    }

    public void StartSimulation()
    {
        gridSize = CalculateMaxEntitySize() * 1.5f;
        _isSimulationRunning = true;
    }

    public void StopSimulation()
    {
        _isSimulationRunning = false;
        Stop();
    }

    public void AddEntity(GameEntity entity)
    {
        _entities.Add(entity);
        _initialPositions[entity] = entity.Transform.Position;
    }

    public void RemoveEntity(GameEntity entity)
    {
        _entities.Remove(entity);
        _initialPositions.Remove(entity);
    }

    public void UpdateInitialPosition(GameEntity entity)
    {
        if (!_isSimulationRunning)
        {
            _initialPositions[entity] = entity.Transform.Position;
        }
    }

    private float CalculateMaxEntitySize()
    {
        float maxSize = 0;
        foreach (var entity in _entities)
        {
            float sizeX = entity.Transform.ScaleX;
            float sizeY = entity.Transform.ScaleY;
            float size = Math.Max(sizeX, sizeY);
            if (size > maxSize)
            {
                maxSize = size;
            }
        }
        return maxSize;
    }
}
