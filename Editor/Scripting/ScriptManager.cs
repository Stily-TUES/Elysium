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
    public Table keyStates;
    public Closure entry;

    public GameEntity Entity { get; init; }
    public ScriptFile File { get; init; }

    public object InvokeCallback(string name, params object[] args)
    {
        try
        {
            return script.Call(callbacks[name], args);
        }
        catch (ScriptRuntimeException ex)
        {
            Debug.WriteLine($"Exception in InvokeCallback('{name}'): {ex.DecoratedMessage}");
            return DynValue.Nil;
        }

        //var callback = callbacks.RawGet(name).Function;
        //if (callback != null)
        //{
        //    return callback.Call(args);
        //}
        //else
        //{
        //    Debug.WriteLine($"Callback '{name}' not found.");
        //    return DynValue.Nil;
        //}
    }

    static RunningScript()
    {
        UserData.RegisterType<GameEntity>();
        UserData.RegisterType<Transform>();
    }

    private Table CreateBindings()
    {
        var res = new Table(script);
        keyStates = new Table(script);
        res["is_key_down"] = DynValue.NewCallback((ctx, args) =>
        {
            var key = args.AsType(0, "is_key_down", MoonSharp.Interpreter.DataType.String).String;
            var pressed = keyStates.Get(key).Boolean;
            return DynValue.NewBoolean(pressed);
        });
        res["callbacks"] = callbacks = new Table(script);
        res["script_path"] = File.FilePath;
        res["entity"] = Entity;

        callbacks["update"] = DynValue.NewCallback((ctx, args) => DynValue.Nil);
        callbacks["start"] = DynValue.NewCallback((ctx, args) => DynValue.Nil);
        callbacks["destroy"] = DynValue.NewCallback((ctx, args) => DynValue.Nil);

        return res;
    }

    public void Load()
    {
        this.entry.Call();
    }

    public void OnKeyDown(string key)
    {
        keyStates[key] = DynValue.NewBoolean(true);
    }

    public void OnKeyUp(string key)
    {
        keyStates[key] = DynValue.NewBoolean(false);
    }

    public RunningScript(ScriptFile file, GameEntity entity)
    {
        this.Entity = entity;
        this.File = file;
        this.script = new Script();
        this.bindings = CreateBindings();
        script.Globals["script"] = this.bindings;
        this.entry = script.LoadFile(File.FilePath).Function;
        Load();
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
            scripts = new(_scripts.Values.SelectMany(v => v));
        }
        else
        {
            scripts = _scripts.GetValueOrDefault(entity) ?? new ();
        }

        foreach (var script in scripts)
        {
            script.InvokeCallback(name, args);
        }

        return scripts;
    }

    public void AddScript(GameEntity entity, string scriptPath)
    {
        Debug.WriteLine($"Adding script: {scriptPath} to entity: {entity.Name}");

        RemoveScript(entity, scriptPath);

        var file = new ScriptFile { FilePath = scriptPath, FileName = System.IO.Path.GetFileName(scriptPath) };
        var script = new RunningScript(file, entity);

        if (!_scripts.ContainsKey(entity))
        {
            _scripts[entity] = new List<RunningScript>();
        }
        _scripts[entity].Add(script);

        script.InvokeCallback("start");

        entity.Scripts.Add(file);
    }

    public void RemoveScript(GameEntity entity, string scriptPath)
    {
        Debug.WriteLine($"Removing script: {scriptPath} from entity: {entity.Name}");

        if (_scripts.ContainsKey(entity))
        {
            var script = _scripts[entity]?.Find(v => v.File.FilePath == scriptPath);
            if (script != null)
            {
                script.InvokeCallback("destroy");
                _scripts[entity].Remove(script);
            }

            var file = entity.Scripts.FirstOrDefault(v => v.FilePath == scriptPath);
            if (file != null)
            {
                entity.Scripts.Remove(file);
            }
        }

    }

    public void UpdateScripts(GameEntity entity, float deltaTime)
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
        Debug.WriteLine($"Loading single script: {file.FilePath} for entity: {entity.Name}");

        var script = new RunningScript(file, entity);

        script.Load();
        _scripts.TryAdd(entity, new());
        _scripts[entity].Add(script);
        script.InvokeCallback("start");
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


