﻿using Editor.Components;
using NLua;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Editor.Scripting;

[DataContract]
public class ScriptManager
{
    private static ScriptManager _instance;
    public static ScriptManager Instance => _instance ??= new ScriptManager();

    [DataMember]
    private Dictionary<GameEntity, List<Lua>> _scripts = new Dictionary<GameEntity, List<Lua>>();

    public void AddScript(GameEntity entity, string scriptPath)
    {
        if (!_scripts.ContainsKey(entity))
        {
            _scripts[entity] = new List<Lua>();
        }

        var lua = new Lua();
        lua["ScriptPath"] = scriptPath;

        lua.DoFile(scriptPath);
        _scripts[entity].Add(lua);

        dynamic scriptInstance = lua["Script"];
        scriptInstance["entity"] = entity;
        CallLuaFunction(scriptInstance, "Start");

        var scriptFile = new ScriptFile { FilePath = scriptPath, FileName = System.IO.Path.GetFileName(scriptPath) };
        entity.Scripts.Add(scriptFile);
    }

    public void RemoveScript(GameEntity entity, string scriptPath)
    {
        if (_scripts.TryGetValue(entity, out List<Lua> luaScripts))
        {
            var lua = luaScripts.FirstOrDefault(l => l["ScriptPath"].ToString() == scriptPath);
            if (lua != null)
            {
                dynamic scriptInstance = lua["Script"];
                CallLuaFunction(scriptInstance, "OnDestroy");
                luaScripts.Remove(lua);
                var scriptFile = entity.Scripts.FirstOrDefault(s => s.FilePath == scriptPath);
                if (scriptFile != null)
                {
                    entity.Scripts.Remove(scriptFile);
                }
            }
        }
    }

    public void UpdateScript(GameEntity entity, float deltaTime)
    {
        if (_scripts.TryGetValue(entity, out List<Lua> luaScripts))
        {
            foreach (var lua in luaScripts)
            {
                dynamic scriptInstance = lua["Script"];
                CallLuaFunction(scriptInstance, "Update", deltaTime);
            }
        }
    }

    public void Dispose()
    {
        foreach (var luaScripts in _scripts.Values)
        {
            foreach (var lua in luaScripts)
            {
                dynamic scriptInstance = lua["Script"];
                CallLuaFunction(scriptInstance, "OnDestroy");
                lua.Dispose();
            }
        }
        _scripts.Clear();
    }

    private void CallLuaFunction(dynamic scriptInstance, string functionName, params object[] args)
    {
        var function = scriptInstance[functionName] as LuaFunction;
        function?.Call(scriptInstance, args);
    }
}

