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
using System.Drawing.Imaging;
using System.Drawing;
using OpenTK.Graphics.OpenGL;

namespace Editor.Components;

[DataContract]
[KnownType(typeof(Transform))]
public class GameEntity : BaseViewModel
{
    private static int _nextId = 1;
    private string _name;
    private TextureFile _texture = null;
    internal int _textureId = -1;

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

    private Mesh _squareMesh;

    [DataMember]
    //adding id so when we rename the entity we can still find it
    public int EntityId { get; private set; }

    //[DataMember]
    //public ObservableCollection<Component> Components { get; } = new ObservableCollection<Component>();
    [DataMember]
    public Transform Transform { get; set; }
    [DataMember]
    public TextureFile Texture {
        get => _texture;
        set
        {
            if (_texture == value) return;

            var texturePath = value.ImagePath;

            if (!string.IsNullOrEmpty(texturePath)) _textureId = Renderer.LoadTexture(texturePath);
            else _textureId = -1;

            this._texture = value;
        }
    }
    [IgnoreDataMember]
    public Scene ParentScene { get; private set; }
    public void Rename(string newName)
    {
        Name = newName;
        OnPropertyChanged(nameof(Name));
    }
    static GameEntity()
    {
    }

    public bool IsInside(Vector4 point)
    {
        point = Transform.WorldToObjectSpace(point);
        var point2 = point.Xy / point.W;

        if (point2.X < -0.5f || point2.X > 0.5f || point2.Y < -0.5f || point2.Y > 0.5f)
        {
            return false;
        }
        return true;
    }
    public void Render(Camera camera, float aspectRatio)
    {
        Renderer renderer = new Renderer();

        //Mesh squareMesh = Mesh.CreateSquare(1.0f);
        Matrix4 modelMatrix = Transform.CreateModelMatrix();
        renderer.RenderMesh(_textureId, _squareMesh, modelMatrix, camera.GetViewMatrix(), camera.GetProjectionMatrix(aspectRatio));

    }
    public GameEntity(Scene parentScene)
    {
        Debug.Assert(parentScene != null);
        ParentScene = parentScene;
        EntityId = _nextId++;
        Transform = new Transform(this);
        _squareMesh = Mesh.CreateSquare(1.0f);
        //Components.Add(new Transform(this));
    }

}
