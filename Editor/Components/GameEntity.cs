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
using System.IO;

namespace Editor.Components;

[DataContract]
[KnownType(typeof(Transform))]
[KnownType(typeof(Physics))]
public class GameEntity : BaseViewModel
{
    private static int _nextId = 1;
    private string _name;
    private TextureFile _texture = new TextureFile();
    private int _cachedTextureId = -1;
    private bool _textureInvalidated = true;
    private float _mass = 1.0f;
    private int _textureId
    {
        get
        {
            if (_textureInvalidated)
            {
                if (_cachedTextureId <= 0) GL.DeleteTexture(_cachedTextureId);
                _cachedTextureId = Renderer.LoadTexture(_texture.ImagePath);
                _textureInvalidated = false;
            }
            return _cachedTextureId;

        }
    }

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
    private bool _isActive = true;
    [DataMember]
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

    private bool _hasGravity = false;
    [DataMember]
    public bool HasGravity
    {
        get => _hasGravity;
        set
        {
            if (_hasGravity != value)
            {
                _hasGravity = value;
                OnPropertyChanged(nameof(HasGravity));
            }
        }
    }
    private bool _hasCollision = true;
    [DataMember]
    public bool HasCollision
    {
        get => _hasCollision;
        set
        {
            if (_hasCollision != value)
            {
                _hasCollision = value;
                OnPropertyChanged(nameof(HasCollision));
            }
        }
    }

    private static Mesh _squareMeshCache;

    private static Mesh _squareMesh
    {
        get
        {
            if (_squareMeshCache == null)
            {
                _squareMeshCache = Mesh.CreateSquare(1.0f);
            }
            return _squareMeshCache;
        }
    }

    [DataMember]
    public float Mass 
    {
        get => _mass;
        set
        {
            if (_mass != value)
            {
                _mass = value;
                OnPropertyChanged(nameof(Mass));
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

    public Physics Physics;

    [DataMember]
    public TextureFile Texture {
        get => _texture;
        set
        {
            _textureInvalidated = _texture != value;
            _texture = value;
        }
    }

    [IgnoreDataMember]
    public Scene ParentScene { get; private set; }
    public void Rename(string newName)
    {
        Name = newName;
        OnPropertyChanged(nameof(Name));
    }

    public bool IsInside(Vector4 point)
    {
        point = Transform.WorldToObjectSpacе(point);
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

        Matrix4 modelMatrix = Transform.CreateModelMatrix();
        renderer.RenderMesh(_textureId, _squareMesh, modelMatrix, camera.GetViewMatrix(), camera.GetProjectionMatrix(aspectRatio));
    }
    public void ApplyPhysics(float deltaTime)
    {
        if (HasGravity)
            Physics.ApplyGravityPhysics(deltaTime, Mass);
    }
    public bool CheckCollision(GameEntity other)
    {
        if (!this.HasCollision || !other.HasCollision) 
        {
            return false;
        }
        Matrix4 thisRotationMatrix = Matrix4.CreateRotationZ(MathHelper.DegreesToRadians(this.Transform.RotationZ));
        Matrix4 otherRotationMatrix = Matrix4.CreateRotationZ(MathHelper.DegreesToRadians(other.Transform.RotationZ));

        Vector4 thisPosition = new Vector4(this.Transform.Position, 1.0f);
        Vector4 otherPosition = new Vector4(other.Transform.Position, 1.0f);

        thisPosition = thisRotationMatrix * thisPosition;
        otherPosition = otherRotationMatrix * otherPosition;

        return thisPosition.X - this.Transform.ScaleX / 2.0f < otherPosition.X + other.Transform.ScaleX / 2.0f &&
               thisPosition.X + this.Transform.ScaleX / 2.0f > otherPosition.X - other.Transform.ScaleX / 2.0f &&
               thisPosition.Y - this.Transform.ScaleY / 2.0f < otherPosition.Y + other.Transform.ScaleY / 2.0f &&
               thisPosition.Y + this.Transform.ScaleY / 2.0f > otherPosition.Y - other.Transform.ScaleY / 2.0f;
    }

    public void ResetPhysics()
    {
        Physics.Reset();
    }
    public GameEntity(Scene parentScene)
    {
        Debug.Assert(parentScene != null);
        ParentScene = parentScene;
        EntityId = _nextId++;
        Physics = new Physics(this);
        Transform = new Transform(this);
        //Components.Add(new Transform(this));
    }

}
