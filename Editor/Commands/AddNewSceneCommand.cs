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

public class AddNewSceneCommand : UndoRedo
{
    public string Name { get; }
    
    public override void Apply()
    {
        Debug.Assert(!Project.Scenes.ContainsKey(Name));
        Project.Scenes.Add(Name, new Scene(Project, Name));

    }
    public override void Undo()
    {
        Debug.Assert(Project.Scenes.ContainsKey(Name));
        Project.Scenes.Remove(Name);
        
    }

    public AddNewSceneCommand(Project project, string name) : base(project, $"Add Scene {name}")
    {
        Name = name;
    }
}
