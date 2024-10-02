using Editor.Commands;
using Editor.Utils;
using System;
using System.Collections.Generic;
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

namespace Editor.Editors;

/// <summary>
/// Interaction logic for ProjectLayoutView.xaml
/// </summary>
public partial class ProjectLayoutView : UserControl
{
    public ProjectManager ProjectManager { get; set; }

    public ProjectLayoutView()
    {
        InitializeComponent();
    }

    private void OnAddSceneButton_Click(object sender, RoutedEventArgs e)
    {
        if (ProjectManager != null)
        {
            var sceneName = "New Scene " + ProjectManager.Project.Scenes.Count;
            var addSceneCommand = new AddNewSceneCommand(ProjectManager.Project, sceneName);
            ProjectManager.Add(addSceneCommand);
        }
    }
}
