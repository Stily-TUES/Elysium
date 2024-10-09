using Editor.Components;
using Editor.GameProject;
using Editor.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Editor.Commands;

public class RemoveGameEntityCommand : UndoRedo
{
    public string EntityName { get; }
    private GameEntity _removedEntity;
    private Scene _parentScene;

    public RemoveGameEntityCommand(Project project, string entityName, Scene parentScene) : base(project, $"Remove Game Entity {entityName}")
    {
        EntityName = entityName;
        _parentScene = parentScene;
    }

    public override void Apply()
    {
        Debug.Assert(_parentScene.GameEntities.Any(e => e.Name == EntityName));

        _removedEntity = _parentScene.GameEntities.FirstOrDefault(e => e.Name == EntityName);
        if (_removedEntity != null)
        {
            _parentScene.GameEntities.Remove(_removedEntity);
        }
    }

    public override void Undo()
    {
        if (_removedEntity != null)
        {
            _parentScene.GameEntities.Add(_removedEntity);
        }
    }
}
