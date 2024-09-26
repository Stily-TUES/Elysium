using Editor.Common;
using Editor.Utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Xps;

namespace Editor.GameProject
{
    
    [DataContract]
    public class ProjectTemplate
    {
        [DataMember]
        public string ProjectType { get; set; }
        [DataMember]
        public string ProjectFile { get; set; }
        [DataMember]
        public List<string> Folders { get; set; }
        public byte[] Icon { get; set; }
        public byte[] Screenshot { get; set; }
        public string IconPath { get; set; }
        public string ProjectPath { get; set; }
        public string ScreenshotPath { get; set; }
    }
    class NewProject : BaseViewModel
    {
        
        private readonly string _templatePath = @"../../../../Editor/Templates";
        private string _projectName = "New Project";
        public string ProjectName 
        { 
            get => _projectName;
            set 
            {  
                if(_projectName != value)
                {
                    _projectName = value;
                    ValidateProject();
                    OnPropertyChanged(nameof(ProjectName));
                }
            } 
        }
        private string _projectPath = $@"{Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)}\ElysiumProjects\";
        public string ProjectPath
        {
            get => _projectPath;
            set
            {
                if (_projectPath != value)
                {
                    _projectPath = value;
                    ValidateProject();
                    OnPropertyChanged(nameof(ProjectPath));
                }
            }
        }
        private string _errorMessage;
        public string ErrorMessage
        {
            get => _errorMessage;
            set
            {
                if (_errorMessage != value)
                {
                    _errorMessage = value;
                    OnPropertyChanged(nameof(ErrorMessage));
                }
            }
        }

        private ObservableCollection<ProjectTemplate> _projectTemplates = new ObservableCollection<ProjectTemplate>();
        
        public ReadOnlyObservableCollection<ProjectTemplate> ProjectTemplates { get; }
        private bool _isValid;
        public bool IsValid
        {
            get => _isValid;
            set
            {
                if (_isValid != value)
                {
                    _isValid = value;
                    OnPropertyChanged(nameof(IsValid));
                }
            }
        }

        private bool ValidateProject()
        {
            var path = ProjectPath;
            if(!Path.EndsInDirectorySeparator(path)) path += Path.DirectorySeparatorChar;
            path += $@"{ProjectName}\";

            IsValid = false;
            if (string.IsNullOrWhiteSpace(ProjectName.Trim()))
            {
                ErrorMessage = "Project name cannot be empty!";
            }
            else if(ProjectName.IndexOfAny(Path.GetInvalidFileNameChars()) != -1)
            {
                ErrorMessage = "Project name contains invalid characters!";
            }
            else if (string.IsNullOrWhiteSpace(ProjectPath.Trim()))
            {
                ErrorMessage = "Project path cannot be empty!";
            }
            else if (ProjectPath.IndexOfAny(Path.GetInvalidPathChars()) != -1)
            {
                ErrorMessage = "Project path contains invalid characters!";
            }
            else if(Directory.Exists(path) && Directory.EnumerateFileSystemEntries(path).Any())
            {
                ErrorMessage = "Project folder already exists and is not empty!";
            }
            else
            {
                ErrorMessage = string.Empty;
                IsValid = true;
            }
            return IsValid;

        }

        public string CreateProject(ProjectTemplate template)
        {
            
            if (ValidateProject())
            {
                if (!Path.EndsInDirectorySeparator(ProjectPath)) ProjectPath += Path.DirectorySeparatorChar;
                string path = $@"{ProjectPath}{ProjectName}\";
                try
                {
                    if (!Directory.Exists(path)) Directory.CreateDirectory(path);
                    foreach (var folder in template.Folders)
                    {
                        Directory.CreateDirectory(Path.GetFullPath(Path.Combine(Path.GetDirectoryName(path), folder)));
                    }
                    var dirInfo = new DirectoryInfo(path + @".elysium");
                    dirInfo.Attributes |= FileAttributes.Hidden;
                    File.Copy(template.IconPath, Path.GetFullPath(Path.Combine(dirInfo.FullName, "icon.png")));
                    File.Copy(template.ScreenshotPath, Path.GetFullPath(Path.Combine(dirInfo.FullName, "screenshot.png")));


                    var projectXml = File.ReadAllText(template.ProjectPath);
                    projectXml = string.Format(projectXml, ProjectName, ProjectPath);
                    var projectPath = Path.GetFullPath(Path.Combine(path, $"{ProjectName}{Project.Extension}"));
                    File.WriteAllText(projectPath, projectXml);

                    //generator for template placeholder.elysium

                    //var project = new Project(ProjectName, path);
                    //Serializer.ToFile(project, path + $"{ProjectName}" + Project.Extension);
                    return path;
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e.Message);
                    return string.Empty;
                }
                //Directory.CreateDirectory(path);
                //foreach (var folder in template.Folders)
                //{
                //    Directory.CreateDirectory(Path.Combine(path, folder));
                //}
                //File.Copy(template.ProjectPath, Path.Combine(path, ProjectName + ".elysium"));
            }
            else return string.Empty;
        }
         
        public NewProject()
        {
            ProjectTemplates = new ReadOnlyObservableCollection<ProjectTemplate>(_projectTemplates);
            try
            {
                var templateFiles = Directory.GetFiles(_templatePath, "template.xml", SearchOption.AllDirectories);
                Debug.Assert(templateFiles.Any());
                //Loading all templates
                foreach (var file in templateFiles)
                {
                    var template = Serializer.FromFile<ProjectTemplate>(file);
                    template.IconPath = Path.GetFullPath(Path.Combine(Path.GetDirectoryName(file), "icon.png"));
                    template.Icon = File.ReadAllBytes(template.IconPath);
                    template.ScreenshotPath = Path.GetFullPath(Path.Combine(Path.GetDirectoryName(file), "screenshot.png"));
                    template.Screenshot = File.ReadAllBytes(template.ScreenshotPath);
                    template.ProjectPath = Path.GetFullPath(Path.Combine(Path.GetDirectoryName(file), template.ProjectFile));

                    _projectTemplates.Add(template);
                }
                ValidateProject();
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }
            //TODO: log error
        }

    }


}
