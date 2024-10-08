using Editor.Common;
using Editor.GameProject;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Editor.Components;

[DataContract]
public class GameEntity : BaseViewModel
{
    private string _name;
    [DataMember]
    public string Name
    {
        get => _name;
        set
        {
            if (_name != value)
            {
                _name = value;
                OnPropertyChanged(Name);
            }
        }
    }
    [DataMember(Name = nameof(Components))]
    private readonly ObservableCollection<Component> _components = new ObservableCollection<Component>();
    public ReadOnlyObservableCollection<Component> Components {get; }
    [DataMember]
    public Scene ParentScene { get; private set; }
    public GameEntity(Scene parentScene)
    {
        Debug.Assert(parentScene != null);
        ParentScene = parentScene;
    }

}
