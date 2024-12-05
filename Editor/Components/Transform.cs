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
    
    public Vector3 Rotation
    {
        get => _rotation;
        set
        {
            if (_rotation != value)
            {
                _rotation = value;
                OnPropertyChanged(nameof(Rotation));
                OnPropertyChanged(nameof(RotationZ));
            }
        }
    }
    [DataMember]
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

    private Vector3 _scale = Vector3.One;
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

    public Matrix4 CreateModelMatrix(Vector3 position, Vector3 rotation, Vector3 scale, Vector3 center)
    {
        Matrix4 scaleMatrix = Matrix4.CreateScale(scale);
        Matrix4 rotationX = Matrix4.CreateRotationX(MathHelper.DegreesToRadians(rotation.X));
        Matrix4 rotationY = Matrix4.CreateRotationY(MathHelper.DegreesToRadians(rotation.Y));
        Matrix4 rotationZ = Matrix4.CreateRotationZ(MathHelper.DegreesToRadians(rotation.Z));
        Matrix4 rotationMatrix = rotationX * rotationY * rotationZ;
        Matrix4 translationMatrix = Matrix4.CreateTranslation(position);

        return Matrix4.CreateTranslation(-center) * scaleMatrix * Matrix4.CreateTranslation(center) * rotationMatrix * translationMatrix;
    }

    public Transform(GameEntity owner) : base(owner)
    {
    }
}
