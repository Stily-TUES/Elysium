﻿using Editor.GameProject.Utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Editor.GameProject
{
    [DataContract]
    public class ProjectData
    {
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public string Path { get; set; }
        [DataMember]
        public DateTime Date { get; set; }
        public byte[] Icon { get; set; }
        public byte[] Screenshot { get; set; }
        public string FullPath { get => $"{Path}{Name}{Project.Extension}"; }
    }
    [DataContract]
    public class ProjectDataList
    {
        [DataMember]
        public List<ProjectData> Projects { get; set; }
    }
    public class OpenProject
    {
        private static readonly string _appDataPath = $@"{Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)}\ElysiumEditor\";
        private static readonly string _projectDataPath;
        private static readonly ObservableCollection<ProjectData> _projects = new ObservableCollection<ProjectData>();
        public static ReadOnlyObservableCollection<ProjectData> Projects { get; }

       

        static OpenProject()
        {
            try
            {
                if (!Directory.Exists(_appDataPath)) Directory.CreateDirectory(_appDataPath);
                _projectDataPath = $@"{_appDataPath}ProjectData.xml";
                Projects = new ReadOnlyObservableCollection<ProjectData>(_projects);
                ReadProjectData();
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }
        }

        private static void ReadProjectData()
        {
            if (File.Exists(_projectDataPath))
            {
                var projects = Serializer.FromFile<ProjectDataList>(_projectDataPath).Projects.OrderByDescending(x => x.Date);
                _projects.Clear();
                foreach (var project in projects)
                {
                    if (File.Exists(project.FullPath))
                    {
                        project.Icon = File.ReadAllBytes($@"{project.Path}\.elysium\icon.png");
                        project.Screenshot = File.ReadAllBytes($@"{project.Path}\.elysium\screenshot.png");
                        _projects.Add(project);
                    }
                }
            }
        }

        private static void WriteProjectData()
        {
            var projects = _projects.OrderBy(x => x.Date).ToList();
            Serializer.ToFile(new ProjectDataList() { Projects = projects}, _projectDataPath);
        }

        public static Project Open(ProjectData projectData)
        {
            ReadProjectData();
            var project = _projects.FirstOrDefault(x => x.FullPath == projectData.FullPath);
            if (project != null)
            {
                project.Date = DateTime.Now;
            }
            else 
            { 
                project = projectData;
                project.Date = DateTime.Now;
                _projects.Add(project);
            }
            WriteProjectData();

            return null;
        }

       
    }
}