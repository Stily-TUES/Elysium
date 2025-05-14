using Editor.Components;
using Editor.Utils;
using OpenTK.Mathematics;
using System;

namespace Editor.Scripting
{
    public class GameMechanics
    {
        private readonly ProjectManager projectManager;

        public GameMechanics(ProjectManager projectManager)
        {
            this.projectManager = projectManager;
        }

        //public GameEntity CreateProjectile(Vector3 position, Vector3 direction, float speed)
        //{
        //    var projectile = new GameEntity(projectManager.GetActiveScene());
        //    projectile.Transform = new Transform(projectile)
        //    {
        //        Position = position
        //    };
        //    projectile.Physics = new Physics(projectile)
        //    {
        //        velocity = direction * speed
        //    };
        //    return projectile;
        //}

        //public void AddEntityToScene(GameEntity entity)
        //{
        //    projectManager.GetActiveScene().GameEntities.Add(entity);
        //}
    }
}
