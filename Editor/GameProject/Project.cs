using Editor.Common;
using Editor.Utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Editor.GameProject
{
    [DataContract]
    public class Project : BaseViewModel
    {
        public static string Extension {get;} = ".elysium";
        [DataMember]
        public string Name { get; set;} = "New Project";
        [DataMember]
        public string Path { get; set; }

        public string FullPath => $"{Path}{Name}{Extension}";
        [DataMember(Name = "Scenes")]
        private ObservableCollection<Scene> _scenes = new ObservableCollection<Scene>();
        public ReadOnlyObservableCollection<Scene> Scenes { get; private set; }

        private Scene _currentScene;

        public Scene CurrentScene
        {
            get => _currentScene;
            set
            {
                _currentScene = value;
                OnPropertyChanged(nameof(CurrentScene));
            }
        }

        public static Project CurrentLoadedProject => Application.Current.MainWindow.DataContext as Project; 

        public static void Unload()
        {

        }

        public static Project Load(string file)
        {
            Debug.Assert(File.Exists(file), "File does not exist");
            return Serializer.FromFile<Project>(file);

        }

        public static void Save(Project project)
        {
            Serializer.ToFile<Project>(project, project.FullPath);
        }


        [OnDeserialized]
        private void OnDeserialized(StreamingContext context)
        {
            if (_scenes != null)
            {
                Scenes = new ReadOnlyObservableCollection<Scene>(_scenes);
                OnPropertyChanged(nameof(Scenes));
            }

            foreach (var scene in Scenes)
            {
                if (scene.isLoaded)
                {
                    CurrentScene = scene;
                    break;
                }
            }
        }

        public Project(string name, string path)
        {

            this.Name = name;
            this.Path = path;

            //_scenes.Add(new Scene(this, "Default Scene"));
            OnDeserialized(new StreamingContext());
        }
    }
}
