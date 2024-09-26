﻿using Editor.Common;
using Editor.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Editor.GameProject;

[DataContract]
public class Project
{
    public static string Extension {get;} = ".elysium";
    [DataMember]
    public string Name { get; set;} = "New Project";

    [DataMember(Name = "Scenes")]
    public Dictionary<string, Scene> Scenes = new Dictionary<string, Scene>();

    public static Project CurrentLoadedProject => Application.Current.MainWindow.DataContext as Project; 

    public static void Unload()
    {

    }

    public void Save(string path)
    {
        Serializer.ToFile<Project>(this, path);
    }


    public static Project Load(string file)
    {
        Debug.Assert(File.Exists(file), "File does not exist");
        return Serializer.FromFile<Project>(file);
    }

    //public static void Save(Project project)
    //{
    //    Serializer.ToFile<Project>(project, project.FullPath);
    //}



    //[OnDeserialized]
    //private void OnDeserialized(StreamingContext context)
    //{
    //    if (_scenes != null)
    //    {
    //        Scenes = new ReadOnlyObservableCollection<Scene>(_scenes);
            
    //    }

    //    foreach (var scene in Scenes)
    //    {
    //        if (scene.isLoaded)
    //        {
    //            CurrentScene = scene;
    //            break;
    //        }
    //    }
    //}

    public Project(string name, string path)
    {

        this.Name = name;
        
        //_scenes.Add(new Scene(this, "Default Scene"));
        //OnDeserialized(new StreamingContext());
    }
}
