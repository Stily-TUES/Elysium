using Editor.Utils;
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

namespace Editor.GameProject;

[DataContract]
public class RecentProjectElement
{
    [DataMember]
    public string ProjectPath { get; set; }
    [DataMember]
    public DateTime Date { get; set; }
    public byte[] Icon { get; set; }
    public byte[] Screenshot { get; set; }
    //public string FullPath { get => $"{ProjectPath}{Name}{Project.Extension}"; }
}
[DataContract]
public class ProjectDataList
{
    [DataMember]
    public List<RecentProjectElement> Projects { get; set; }
}
public class HollowProject {  }
public class OpenProject
{
    private static readonly string _appDataPath = $@"{Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)}\ElysiumEditor\";
    private static readonly string _projectDataPath;
    private static readonly ObservableCollection<RecentProjectElement> _projects = new ObservableCollection<RecentProjectElement>();
    public static ReadOnlyObservableCollection<RecentProjectElement> Projects { get; }

   

    static OpenProject()
    {
        try
        {
            if (!Directory.Exists(_appDataPath)) Directory.CreateDirectory(_appDataPath);
            _projectDataPath = $@"{_appDataPath}ProjectData.xml";
            Projects = new ReadOnlyObservableCollection<RecentProjectElement>(_projects);
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
                    project.Icon = File.ReadAllBytes($@"{project.ProjectPath}\.elysium\icon.png");
                    project.Screenshot = File.ReadAllBytes($@"{project.ProjectPath}\.elysium\screenshot.png");
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

    public static Project Open(RecentProjectElement projectData)
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

        return Project.Load(project.FullPath);
    }

   
}
