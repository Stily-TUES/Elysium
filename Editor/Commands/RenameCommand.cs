using Editor.GameProject;
using Editor.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Editor.Commands;

public class RenameCommand : UndoRedo
{
    private readonly dynamic _item;
    private readonly Project _project;
    private readonly string _oldName;
    private readonly string _newName;

    public RenameCommand(Project project, dynamic item, string oldName, string newName)
        : base(project, $"Rename {oldName} to {newName}")
    {
        _item = item;
        _project = project;
        _oldName = oldName;
        _newName = newName;
    }

    public override void Apply()
    {
        _item.Name = _newName;
    }

    public override void Undo()
    {
        _item.Name = _oldName;
    }
}
