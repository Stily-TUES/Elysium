using Editor.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Editor.Components;

[DataContract]
public class Component : BaseViewModel
{
    [DataMember]
    public GameEntity ParentEntity { get; private set; }
    public Component(GameEntity owner)
    {
        Debug.Assert(owner != null);
        ParentEntity = owner;
    }
}
