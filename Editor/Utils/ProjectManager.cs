using Editor.GameProject;
using GameEngine;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Editor.Utils;

public class ProjectManager
{
    private readonly Stack<UndoRedo> _undoStack = new Stack<UndoRedo>();
    private readonly Stack<UndoRedo> _redoStack = new Stack<UndoRedo>();
    public Project Project { get; }
    public string Path { get; }

    public ProjectManager(Project project, string path) {
        Project = project;
        Path = path;
    }

    public void Reset()
    {
        _undoStack.Clear();
        _redoStack.Clear();
    }

    public void Add(UndoRedo undoRedo)
    {
        undoRedo.Apply();
        _undoStack.Push(undoRedo);
        _redoStack.Clear();
    }

    public void Undo()
    {
        if (_undoStack.Count == 0) return;
        var undo = _undoStack.Pop();
        undo.Undo();
        _redoStack.Push(undo);
    }

    public void Redo()
    {
        if (_redoStack.Count == 0) return;
        var redo = _redoStack.Pop();
        redo.Apply();
        _undoStack.Push(redo);
    }

    public void Save()
    {
        Project.Save(Path);
    }

    public Scene GetActiveScene()
    {
        return Project.Scenes.FirstOrDefault(scene => scene.isLoaded);
    }

    public void RenderProject(Camera camera)
    {
        foreach (var scene in Project.Scenes)
        {
            if (scene.isLoaded)
            {
                scene.Render(camera);
            }
        }
    }

    public static ProjectManager Load(string path)
    {
        var project = Project.Load(path);
        return new ProjectManager(project, path);
    }

}
