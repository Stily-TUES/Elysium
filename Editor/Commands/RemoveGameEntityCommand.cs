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
    private int EntityId;
    private GameEntity _removedEntity;
    private Scene _parentScene;

    public RemoveGameEntityCommand(Project project, int entityId, Scene parentScene)
        : base(project, $"Remove Game Entity {entityId}")
    {
        EntityId = entityId;
        _parentScene = parentScene;
    }

    public override void Apply()
    {
        _removedEntity = _parentScene.GameEntities.FirstOrDefault(e => e.EntityId == EntityId);
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
