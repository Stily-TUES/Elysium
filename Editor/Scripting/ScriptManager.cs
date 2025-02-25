using Editor.Components;
using MoonSharp.Interpreter;
using OpenTK.Graphics.ES11;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace Editor.Scripting;

public class RunningScript
{
    public Script script;
    public Table bindings;
    public Table callbacks;
    public Closure entry;

    public GameEntity Entity { get; init; }
    public ScriptFile File { get; init; }


    public object InvokeCallback(string name, params object[] args)
    {
        try {
            return callbacks.RawGet(name).Function.Call(args);
        }
        catch (ScriptRuntimeException ex)
        {
            Debug.WriteLine("Unhandled exception in lua script: " + ex.DecoratedMessage);
            foreach (var el in ex.CallStack)
            {
                Debug.WriteLine("\tat" + el.Location + " in " + el.Name);
            }
            throw ex;
        }
    }

    static RunningScript()
    {
        UserData.RegisterType<GameEntity>();
        UserData.RegisterType<Transform>();
    }

    private Table CreateBindings()
    {
        var res = new Table(script);
        res["is_key_down"] = DynValue.NewCallback((ctx, args) =>
        {
            var key = args.AsType(0, "is_key_down", MoonSharp.Interpreter.DataType.String).String;
            // ???
            var pressed = false;
            return DynValue.NewBoolean(pressed);
        });
        res["callbacks"] = callbacks = new Table(script);
        res["script_path"] = File.FilePath;
        res["entity"] = Entity;
        return res;
    }

    public void Load()
    {
        this.entry.Call();        
    }

    public RunningScript(ScriptFile file, GameEntity entity)
    {
        this.Entity = entity;
        this.File = file;
        this.script = new Script();
        this.bindings = CreateBindings();
        script.Globals["script"] = this.bindings;
        this.entry = script.LoadFile(File.FilePath).Function;
    }
}

public class ScriptManager
{
    private static ScriptManager _instance;
    public static ScriptManager Instance => _instance ??= new ScriptManager();

    private Dictionary<GameEntity, List<RunningScript>> _scripts = new Dictionary<GameEntity, List<RunningScript>>();

    public List<RunningScript> InvokeCallbacks(string name, GameEntity? entity = null, params object[] args)
    {
        List<RunningScript> scripts;

        if (entity == null)
        {
            scripts = new (_scripts.Values.SelectMany(v => v));
        }
        else
        {
            scripts = _scripts[entity];
        }

        foreach (var script in scripts)
        {
            script.InvokeCallback(name, args);
        }

        return scripts;
    }

    public void AddScript(GameEntity entity, string scriptPath)
    {

        var file = new ScriptFile { FilePath = scriptPath, FileName = System.IO.Path.GetFileName(scriptPath) };
        var script = new RunningScript(file, entity);

        _scripts[entity] ??= new List<RunningScript>();
        _scripts[entity].Add(script);

        entity.Scripts.Add(file);

    }

    public void RemoveScript(GameEntity entity, string scriptPath)
    {
        if (entity == null) throw new Exception("no.");

        _scripts[entity]?.Find(v => v.File.FilePath == scriptPath)?.InvokeCallback("destroy");
        var file = entity.Scripts.FirstOrDefault(v => v.FilePath == scriptPath);
        if (file != null)
        {
            entity.Scripts.Remove(file);
        }
    }

    public void UpdateScript(GameEntity entity, float deltaTime)
    {
        InvokeCallbacks("update", entity, deltaTime);
    }

    public void Dispose()
    {
        InvokeCallbacks("destroy");
        _scripts.Clear();
    }

    private void LoadSingleScript(GameEntity entity, ScriptFile file)
    {
        var script = new RunningScript(file, entity);

        script.Load();
        _scripts.TryAdd(entity, new());
        _scripts[entity].Add(script);
    }

    public void LoadScriptsForEntity(GameEntity entity)
    {
        foreach (var scriptFile in entity.Scripts)
        {
            LoadSingleScript(entity, scriptFile);
        }
    }

    public void LoadAllScripts(IEnumerable<GameEntity> entities)
    {
        foreach (var entity in entities)
        {
            LoadScriptsForEntity(entity);
        }
    }
}

