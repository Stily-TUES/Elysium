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
using Editor.Utils;
using GameEngine;

namespace Editor.GameProject;

[DataContract]
public class Scene : BaseViewModel
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
                OnPropertyChanged(nameof(Name));
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
                OnPropertyChanged(nameof(isLoaded));
            }
        }
    }

    [DataMember]
    public ObservableCollection<GameEntity> GameEntities { get; set; }
    [DataMember]
    public TextureFile Background { get; set; }

    public void Render(Camera camera, float aspectRatio)
    {
        if (GameEntities == null) return;
        foreach (var entity in GameEntities)
        {
            if (entity.IsActive == true) entity.Render(camera, aspectRatio);
        }
    }
    public Scene(Project project, string name)
    {
        Debug.Assert(project != null);
        this.Project = project;
        this.Name = name;
        GameEntities = new ObservableCollection<GameEntity>();
    }
}
