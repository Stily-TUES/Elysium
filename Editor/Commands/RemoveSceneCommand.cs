﻿using Editor.GameProject;
using Editor.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Editor.Commands;

public class RemoveSceneCommand : UndoRedo
{
    public string Name { get; }
    private Scene _removedScene;

    public override void Apply()
    {
        _removedScene = Project.Scenes.FirstOrDefault(s => s.Name == Name);
        if (_removedScene != null && !_removedScene.isLoaded)
        {
            Debug.Assert(Project.Scenes.Any(s => s.Name == Name));
            Project.Scenes.Remove(_removedScene);
        }
    }

    public override void Undo()
    {
        if (_removedScene != null)
        {
            Debug.Assert(!Project.Scenes.Any(s => s.Name == Name));
            Project.Scenes.Add(_removedScene);
        }
    }

    public RemoveSceneCommand(Project project, string name) : base(project, $"Remove Scene {name}")
    {
        Name = name;
    }
}
