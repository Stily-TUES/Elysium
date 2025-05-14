using Editor.GameProject;
using Editor.Scripting;
using Editor.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Path = System.IO.Path;

namespace Editor.Editors;

/// <summary>
/// Interaction logic for NewProjectView.xaml
/// </summary>
public partial class NewProjectView : UserControl
{
    public NewProjectView()
    {
        InitializeComponent();
    }

    private void OnCreateButton_Click(object sender, RoutedEventArgs e)
    {
        var vm = DataContext as NewProject;
        var projectPath = vm.CreateProject((ProjectTemplate)templateListBox.SelectedItem);
        bool dialogResult = false;
        var window = Window.GetWindow(this);
        if(!string.IsNullOrEmpty(projectPath))
        {
            dialogResult = true;
            var projectManager = OpenProject.Open(new ProjectMetadata() { Date = new DateTime(), FullPath = projectPath });
            window.DataContext = projectManager;
            if (projectManager.Project.Name != null)
            {
                string projectTexturesFolderPath = Path.GetFullPath(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "ElysiumProjects", projectManager.Project.Name, "Textures"));
                TextureFile.LoadTexturesFromDirectory(projectTexturesFolderPath);
                string projectScriptsFolderPath = Path.GetFullPath(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "ElysiumProjects", projectManager.Project.Name, "Scripts"));
                ScriptFile.LoadScriptsFromDirectory(projectScriptsFolderPath);

            }
           
        }
        window.DialogResult = dialogResult;
        window.Close();
    }
}
