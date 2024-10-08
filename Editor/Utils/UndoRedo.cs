using Editor.GameProject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Editor.Utils
{
    public abstract class UndoRedo
    {
        public Project Project { get; }
        public string Desc { get; }
        public abstract void Undo();
        public abstract void Apply();

        public UndoRedo(Project project, string name)
        {
            Project = project;
            Desc = name;
        }
    }
}
