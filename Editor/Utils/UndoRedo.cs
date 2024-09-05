using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Editor.Utils
{
    public interface IUndoRedo
    {
        string Name { get; }
        void Undo();
        void Redo();
    }

    public class UndoRedoAction : IUndoRedo
    {
        private readonly Action _undoAction;
        private readonly Action _redoAction;

        public string Name { get; }

        public UndoRedoAction(string name)
        {
            this.Name = name;
        }
        public UndoRedoAction(string name, Action undoAction, Action redoAction) : this(name)
        { 
            Debug.Assert(undoAction != null && redoAction != null);
            _undoAction = undoAction;
            _redoAction = redoAction;
        }

        public void Undo() => _undoAction();

        public void Redo() => _redoAction();

    }

    public class UndoRedo
    {
        private readonly Stack<IUndoRedo> _undoStack = new Stack<IUndoRedo>();
        private readonly Stack<IUndoRedo> _redoStack = new Stack<IUndoRedo>();

        public void Reset()
        {
            _undoStack.Clear();
            _redoStack.Clear();
        }

        public void Add(IUndoRedo undoRedo)
        {
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
            redo.Redo();
            _undoStack.Push(redo);
        }
    }
}
