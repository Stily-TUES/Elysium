using Editor.GameProject;
using Editor.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Editor.Commands;

public class AddSceneCommand : UndoRedo
{
    public string Name { get; }

    public override void Apply()
    {
        Debug.Assert(!Project.Scenes.Any(s => s.Name == Name));

        Project.Scenes.Add(new Scene(Project, Name));
    }

    public override void Undo()
    {
        Debug.Assert(Project.Scenes.Any(s => s.Name == Name));

        var sceneToRemove = Project.Scenes.FirstOrDefault(s => s.Name == Name);
        if (sceneToRemove != null)
        {
            Project.Scenes.Remove(sceneToRemove);
        }
    }

    public AddSceneCommand(Project project, string name) : base(project, $"Add Scene {name}")
    {
        Name = name;
    }
}
