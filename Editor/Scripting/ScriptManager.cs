using Editor.Components;
using NLua;
using System;
using System.Collections.Generic;

namespace Editor.Scripting;

public class ScriptManager
{
    private readonly Dictionary<GameEntity, Lua> _scripts = new Dictionary<GameEntity, Lua>();

    public void AddScript(GameEntity entity, string scriptPath)
    {
        var lua = new Lua();
        lua["entity"] = entity;
        lua.DoFile(scriptPath);
        _scripts[entity] = lua;

        dynamic scriptInstance = lua["Script"];
        scriptInstance.Start();
    }

    public void RemoveScript(GameEntity entity)
    {
        if (_scripts.TryGetValue(entity, out Lua lua))
        {
            dynamic scriptInstance = lua["Script"];
            scriptInstance.OnDestroy();
            _scripts.Remove(entity);
        }
    }

    public void UpdateScript(GameEntity entity, float deltaTime)
    {
        if (_scripts.TryGetValue(entity, out Lua lua))
        {
            dynamic scriptInstance = lua["Script"];
            scriptInstance.Update(deltaTime);
        }
    }

    public void Dispose()
    {
        foreach (var lua in _scripts.Values)
        {
            dynamic scriptInstance = lua["Script"];
            scriptInstance.OnDestroy();
            lua.Dispose();
        }
        _scripts.Clear();
    }
}

