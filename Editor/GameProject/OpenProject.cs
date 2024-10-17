using Editor.Common;
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
    public string FullPath { get; set; }
    [DataMember]
    public DateTime Date { get; set; }
    public byte[] Icon { get; set; }
    public byte[] Screenshot { get; set; }
    public ProjectMetadata Metadata { get; set; }

    public void Load()
    {
        if (File.Exists(FullPath))
        {
            var project = Serializer.FromFile<Project>(FullPath);
            Metadata = new ProjectMetadata
            {
                IconPath = project.IconPath,
                ScreenshotPath = project.ScreenshotPath,
                Name = project.Name
            };
        }
    }
}
[DataContract]
public class ProjectDataList
{
    [DataMember]
    public List<RecentProjectElement> Projects { get; set; }
    public ProjectDataList Load()
    {
        Projects.ForEach(x => x.Load());
        return this;
    }
}

[DataContract]
public class ProjectMetadata : BaseViewModel
{
    [DataMember]
    public string Name { get; set; }
    [DataMember]
    public string IconPath { get; set; }
    [DataMember]
    public string ScreenshotPath { get; set; }
}
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
            var projects = Serializer.FromFile<ProjectDataList>(_projectDataPath).Load().Projects.OrderByDescending(x => x.Date);
            _projects.Clear();
            foreach (var project in projects)
            {
                if (File.Exists(project.FullPath))
                {
                    if (project.Metadata != null)
                    {
                        if (!string.IsNullOrEmpty(project.Metadata.IconPath) && File.Exists(project.Metadata.IconPath))
                        {
                            project.Icon = File.ReadAllBytes(project.Metadata.IconPath);
                        }
                        if (!string.IsNullOrEmpty(project.Metadata.ScreenshotPath) && File.Exists(project.Metadata.ScreenshotPath))
                        {
                            project.Screenshot = File.ReadAllBytes(project.Metadata.ScreenshotPath);
                        }
                    }
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

    public static ProjectManager Open(RecentProjectElement project)
    {
        ReadProjectData();
        var existingProject = _projects.FirstOrDefault(p => p.FullPath == project.FullPath);
        if (existingProject != null)
        {
            _projects.Remove(existingProject);
        }
        _projects.Insert(0, project);
        project.Date = DateTime.Now;

        WriteProjectData();

        return ProjectManager.Load(project.FullPath);
    }

   
}
