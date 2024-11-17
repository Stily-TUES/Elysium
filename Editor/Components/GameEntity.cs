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
using GameEngine;
using System.Windows.Input.StylusPlugIns;
using Editor.Utils;

namespace Editor.Components;

[DataContract]
[KnownType(typeof(Transform))]
public class GameEntity : BaseViewModel
{
    private static int _nextId = 1;
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
    [DataMember]
    private bool _isActive = true;
    public bool IsActive
    {
        get => _isActive;
        set
        {
            if (_isActive != value)
            {
                _isActive = value;
                OnPropertyChanged(nameof(IsActive));
            }
        }
    }
    [DataMember]
    //adding id so when we rename the entity we can still find it
    public int EntityId { get; private set; }

    //[DataMember]
    //public ObservableCollection<Component> Components { get; } = new ObservableCollection<Component>();
    [DataMember]
    public Transform Transform { get; set; }
    [DataMember]
    public TextureFile Texture { get; set; }
    [IgnoreDataMember]
    public Scene ParentScene { get; private set; }
    public void Rename(string newName)
    {
        Name = newName;
        OnPropertyChanged(nameof(Name));
    }
    public void Render()
    {
        Renderer renderer = new Renderer();
        //renderer.drawSquare((double)Transform.Position.X, (double)Transform.Position.Y, (double)0.0f, (double)1.0f);
        if (Texture == null)
        {
            Texture = new TextureFile();
        }
        renderer.DrawSquare(-0.5, -0.5, 0.0, 1.0f, Texture.ImagePath);

    }
    public GameEntity(Scene parentScene)
    {
        Debug.Assert(parentScene != null);
        ParentScene = parentScene;
        EntityId = _nextId++;
        Transform = new Transform(this);
        //Components.Add(new Transform(this));
    }

}
