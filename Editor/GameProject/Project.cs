using Editor.Common;
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
public class Project : ProjectMetadata
{
    public static string Extension => ".elysium";

    [DataMember(Name = "Scenes")]
    public ObservableCollection<Scene> Scenes { get; private set; }

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

    public Project(string name)
    {
        this.Name = name;
    }
}
