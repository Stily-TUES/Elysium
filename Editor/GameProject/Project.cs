using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Editor.GameProject
{
    [DataContract]
    public class Project : BaseViewModel
    {
        public static string Extension {get;} = ".elysium";
        [DataMember]
        public string Name { get; set;}
        [DataMember]
        public string Path { get; set; }

        public string FullPath => $"{Path}{Name}{Extension}";
        [DataMember(Name = "Scenes")]
        private ObservableCollection<Scene> _scenes = new ObservableCollection<Scene>();
        public ReadOnlyObservableCollection<Scene> Scenes { get; }


        public Project(string name, string path)
        {
            
            this.Name = name;
            this.Path = path;

            _scenes.Add(new Scene(this, "Default Scene"));
        }
    }
}
