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
public class ProjectMetadata
{
    [DataMember]
    public string FullPath { get; set; }
    [DataMember]
    public DateTime Date { get; set; }
    public TextureFile TextureFile { get; set; }
    public RecentProjectElement RecentProject { get; set; }

    public void Load()
    {
        if (File.Exists(FullPath))
        {
            var project = Serializer.FromFile<Project>(FullPath);
            RecentProject = new RecentProjectElement
            {
                IconPath = project.IconPath,
                ScreenshotPath = project.ScreenshotPath,
                Name = project.Name,
                Icon = project.Icon,
                Screenshot = project.Screenshot
            };
        }
    }
}
[DataContract]
public class ProjectDataList
{
    [DataMember]
    public List<ProjectMetadata> Projects { get; set; }
    public ProjectDataList Load()
    {
        Projects.ForEach(x => x.Load());
        return this;
    }
}

[DataContract]
public class RecentProjectElement : BaseViewModel
{
    [DataMember]
    public string Name { get; set; }
    [DataMember]
    public string IconPath { get; set; }
    [DataMember]
    public string ScreenshotPath { get; set; }
    public byte[] Icon { get; set; }
    public byte[] Screenshot { get; set; }
}
public class OpenProject
{
    private static readonly string _appDataPath = $@"{Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)}\ElysiumEditor\";
    private static readonly string _projectDataPath;
    private static readonly ObservableCollection<ProjectMetadata> _projects = new ObservableCollection<ProjectMetadata>();
    public static ReadOnlyObservableCollection<ProjectMetadata> Projects { get; }

    static OpenProject()
    {
        try
        {
            if (!Directory.Exists(_appDataPath)) Directory.CreateDirectory(_appDataPath);
            _projectDataPath = $@"{_appDataPath}ProjectData.xml";
            Projects = new ReadOnlyObservableCollection<ProjectMetadata>(_projects);
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
                    if (project.RecentProject != null)
                    {
                        if (!string.IsNullOrEmpty(project.RecentProject.IconPath) && File.Exists(project.RecentProject.IconPath))
                        {
                            project.RecentProject.Icon = File.ReadAllBytes(project.RecentProject.IconPath);
                        }
                        if (!string.IsNullOrEmpty(project.RecentProject.ScreenshotPath) && File.Exists(project.RecentProject.ScreenshotPath))
                        {
                            project.RecentProject.Screenshot = File.ReadAllBytes(project.RecentProject.ScreenshotPath);
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

    public static ProjectManager Open(ProjectMetadata project)
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
        //TODO: fix when creating new project RecentProject.Name get returns null
        if (project.RecentProject.Name != null)
        {
            string projectTexturesFolderPath = Path.GetFullPath(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "ElysiumProjects", project.RecentProject.Name, "Textures"));
            TextureFile.LoadTexturesFromDirectory(projectTexturesFolderPath);
        }
        

        return ProjectManager.Load(project.FullPath);
    }

   
}
