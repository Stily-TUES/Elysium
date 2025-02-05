using Editor.Components;
using Editor.GameProject;
using Editor.Utils;
using System;
using System.Globalization;
using System.Windows.Input;

namespace Editor.Commands;

public class UpdateTransformCommand : UndoRedo
{
    private readonly Transform _transform;
    private readonly string _propertyName;
    private readonly float _oldValue;
    private readonly float _newValue;

    public UpdateTransformCommand(Project project, Transform transform, string propertyName, float oldValue, float newValue)
        : base(project, $"Update {propertyName} from {oldValue} to {newValue}")
    {
        _transform = transform;
        _propertyName = propertyName;
        _oldValue = oldValue;
        _newValue = newValue;
    }

    public override void Apply()
    {
        _transform.GetType().GetProperty(_propertyName).SetValue(_transform, _newValue);
    }

    public override void Undo()
    {
        _transform.GetType().GetProperty(_propertyName).SetValue(_transform, _oldValue);
    }
}

