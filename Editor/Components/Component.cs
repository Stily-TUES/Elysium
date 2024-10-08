using Editor.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Editor.Components;

public class Component : BaseViewModel
{
    public GameEntity Parent { get; private set; }
    public Component(GameEntity owner)
    {
        Debug.Assert(owner != null);
        Parent = owner;
    }
}
