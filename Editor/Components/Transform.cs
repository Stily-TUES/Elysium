using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Editor.Components;

//interface ITransform
//{
//    Matrix4x4 GetMatrix();
//}

[DataContract]
class Transform : Component
{
    private Vector3 _position;
    public Vector3 Position
    {
        get => _position;
        set
        {
            if (_position != value)
            {
                _position = value;
                OnPropertyChanged(nameof(Position));
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
            }
        }
    }
    private Vector3 _scale;
    public Vector3 Scale
    {
        get => _scale;
        set
        {
            if (_scale != value)
            {
                _scale = value;
                OnPropertyChanged(nameof(Scale));
            }
        }
    }
    public Transform(GameEntity owner) : base(owner)
    {
    }
}
