using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Editor.Components;

//interface ITransform
//{
//    Matrix4x4 GetMatrix();
//}

[DataContract]
public class Transform : Component
{
    private Vector3 _position;
    [DataMember]
    public Vector3 Position
    {
        get => _position;
        set
        {
            if (_position != value)
            {
                _position = value;
                OnPropertyChanged(nameof(Position));
                OnPropertyChanged(nameof(PositionX));
                OnPropertyChanged(nameof(PositionY));
                OnPropertyChanged(nameof(PositionZ));
            }
        }
    }
    public float PositionX
    {
        get => Position.X;
        set
        {
            if (_position.X != value)
            {
                _position.X = value;
                OnPropertyChanged(nameof(Position));
                OnPropertyChanged(nameof(PositionX));
            }
        }
    }
    public float PositionY
    {
        get => Position.Y;
        set
        {
            if (_position.Y != value)
            {
                _position.Y = value;
                OnPropertyChanged(nameof(Position));
                OnPropertyChanged(nameof(PositionY));
            }
        }
    }
    public float PositionZ
    {
        get => Position.Z;
        set
        {
            if (_position.Z != value)
            {
                _position.Z = value;
                OnPropertyChanged(nameof(Position));
                OnPropertyChanged(nameof(PositionZ));
            }
        }
    }
    private Vector3 _rotation;
    [DataMember]
    public Vector3 Rotation
    {
        get => _rotation;
        set
        {
            if (_rotation != value)
            {
                _rotation = value;
                OnPropertyChanged(nameof(Rotation));
                OnPropertyChanged(nameof(RotationX));
                OnPropertyChanged(nameof(RotationY));
                OnPropertyChanged(nameof(RotationZ));
            }
        }
    }
    public float RotationX
    {
        get => Rotation.X;
        set
        {
            if (_rotation.X != value)
            {
                _rotation.X = value;
                OnPropertyChanged(nameof(Rotation));
                OnPropertyChanged(nameof(RotationX));
            }
        }
    }
    public float RotationY
    {
        get => Rotation.Y;
        set
        {
            if (_rotation.Y != value)
            {
                _rotation.Y = value;
                OnPropertyChanged(nameof(Rotation));
                OnPropertyChanged(nameof(RotationY));
            }
        }
    }
    public float RotationZ
    {
        get => Rotation.Z;
        set
        {
            if (_rotation.Z != value)
            {
                _rotation.Z = value;
                OnPropertyChanged(nameof(Rotation));
                OnPropertyChanged(nameof(RotationZ));
            }
        }
    }

    private Vector3 _scale;
    [DataMember]
    public Vector3 Scale
    {
        get => _scale;
        set
        {
            if (_scale != value)
            {
                _scale = value;
                OnPropertyChanged(nameof(Scale));
                OnPropertyChanged(nameof(ScaleX));
                OnPropertyChanged(nameof(ScaleY));
                OnPropertyChanged(nameof(ScaleZ));
            }
        }
    }
    public float ScaleX
    {
        get => Scale.X;
        set
        {
            if (_scale.X != value)
            {
                _scale.X = value;
                OnPropertyChanged(nameof(Scale));
                OnPropertyChanged(nameof(ScaleX));
            }
        }
    }
    public float ScaleY
    {
        get => Scale.Y;
        set
        {
            if (_scale.Y != value)
            {
                _scale.Y = value;
                OnPropertyChanged(nameof(Scale));
                OnPropertyChanged(nameof(ScaleY));
            }
        }
    }
    public float ScaleZ
    {
        get => Scale.Z;
        set
        {
            if (_scale.Z != value)
            {
                _scale.Z = value;
                OnPropertyChanged(nameof(Scale));
                OnPropertyChanged(nameof(ScaleZ));
            }
        }
    }

    public Transform(GameEntity owner) : base(owner)
    {
    }
}
