using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Editor.Common;
using Editor.Components;

namespace Editor.GameProject;

[DataContract]
public class Scene
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
            }
        }
    }
    [XmlIgnore]
    public Project Project { get; private set; }

    public bool _isLoaded;
    [DataMember]
    public bool isLoaded
    {
        get => _isLoaded;
        set
        {
            if (_isLoaded != value)
            {
                _isLoaded = value;
            }
        }
    }

    [DataMember]
    public ObservableCollection<GameEntity> GameEntities { get; set; }
    public Scene(Project project, string name)
    {
        Debug.Assert(project != null);
        this.Project = project;
        this.Name = name;
        //GameEntities = new ObservableCollection<GameEntity>();
    }
    //TODO: implement game entities 
}
