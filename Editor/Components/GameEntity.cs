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
using System.Windows.Media.Media3D;
using OpenTK.Mathematics;
using Camera = GameEngine.Camera;
using Vector3 = OpenTK.Mathematics.Vector3;
using System.Windows.Media.Imaging;

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

    public bool IsInside(System.Numerics.Vector2 point)
    {
        Vector4 tempPt = new Vector4(point.X, point.Y, 0, 1);
        tempPt = tempPt * Transform.CreateModelMatrix(Transform.Position, Transform.Rotation, Transform.Scale, Vector3.Zero).Inverted();
        tempPt /= tempPt.W;
        point = new System.Numerics.Vector2(tempPt.X, tempPt.Y);
        if (point.X < 0 || point.X > 1 || point.Y < 0 || point.Y > 1)
        {
            return false;
        }
        return true;
    }
    public void Render(Camera camera, float aspectRatio)
    {
        Renderer renderer = new Renderer();
        if (Texture == null)
        {
            Texture = new TextureFile();
        }
        Mesh squareMesh = Mesh.CreateSquare(1.0f);
        Matrix4 modelMatrix = Transform.CreateModelMatrix();
        renderer.RenderMesh(squareMesh, modelMatrix, camera.GetViewMatrix(), camera.GetProjectionMatrix(aspectRatio), Texture.ImagePath);

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
