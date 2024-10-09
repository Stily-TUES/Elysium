using Editor.Components;
using Editor.GameProject;
using Editor.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Editor.Commands;

public class AddGameEntityCommand : UndoRedo
{
    public string EntityName { get; }
    private GameEntity _addedEntity;
    private Scene _parentScene;

    public AddGameEntityCommand(Project project, string entityName, Scene parentScene) : base(project, $"Add Game Entity {entityName}")
    {
        EntityName = entityName;
        _parentScene = parentScene;
    }

    public override void Apply()
    {

        _addedEntity = new GameEntity(_parentScene) { Name = EntityName };
        _parentScene.GameEntities.Add(_addedEntity);
    }

    public override void Undo()
    {
        Debug.Assert(_parentScene.GameEntities.Any(e => e.Name == EntityName));

        if (_addedEntity != null)
        {
            _parentScene.GameEntities.Remove(_addedEntity);
        }
    }
}
