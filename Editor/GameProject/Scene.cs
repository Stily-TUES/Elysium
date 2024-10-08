using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Editor.Common;

namespace Editor.GameProject
{
    [DataContract]
    public class Scene
    {
        private string _name;
        [DataMember]
        public string Name
        {
            get => _name;
            set
            {
                if (_name != value)
                {
                    _name = value;
                }
            }
        }
        [XmlIgnore]
        public Project Project { get; private set; }

        public bool _isLoaded;
        [DataMember]
        public bool isLoaded
        {
            get => _isLoaded;
            set
            {
                if (_isLoaded != value)
                {
                    _isLoaded = value;
                }
            }
        }

        public Scene(Project project, string name)
        {
            Debug.Assert(project != null);
            this.Project = project;
            this.Name = name;
        }
        //TODO: implement game entities 
        //private ObservableCollection<GameObject> _gameObjects = new ObservableCollection<GameObject>();
    }
}
