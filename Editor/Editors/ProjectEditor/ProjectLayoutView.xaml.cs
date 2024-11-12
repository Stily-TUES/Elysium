using Editor.Commands;
using Editor.GameProject;
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
        var window = Window.GetWindow(this);
        ProjectManager = (ProjectManager)window.DataContext;
        if (ProjectManager != null)
        {
            var sceneName = "New Scene " + ProjectManager.Project.Scenes.Count;
            var addSceneCommand = new AddSceneCommand(ProjectManager.Project, sceneName);
            ProjectManager.Add(addSceneCommand);
        }
    }

    private void OnRemoveSceneButton_Click(object sender, RoutedEventArgs e)
    {
        var window = Window.GetWindow(this);
        ProjectManager = (ProjectManager)window.DataContext;
        if (ProjectManager != null)
        {
            var button = sender as Button;
            var sceneName = button?.Tag as string;
            if (!string.IsNullOrEmpty(sceneName))
            {
                var removeSceneCommand = new RemoveSceneCommand(ProjectManager.Project, sceneName);
                ProjectManager.Add(removeSceneCommand);
            }
        }
    }


    private void OnAddEntityButton_Click(object sender, RoutedEventArgs e)
    {
        var window = Window.GetWindow(this);
        ProjectManager = (ProjectManager)window.DataContext;
        if (ProjectManager != null)
        {
            var activeScene = ProjectManager.GetActiveScene();
            if (activeScene != null)
            {
                var entityName = "New Entity " + activeScene.GameEntities.Count;
                var addEntityCommand = new AddGameEntityCommand(ProjectManager.Project, entityName, activeScene);
                ProjectManager.Add(addEntityCommand);
            }
        }
    }
    private void OnRemoveEntityButton_Click(object sender, RoutedEventArgs e)
    {
        var window = Window.GetWindow(this);
        ProjectManager = (ProjectManager)window.DataContext;
        if (ProjectManager != null)
        {
            var button = sender as Button;
            var entityId = (int)button?.Tag;
            var activeScene = ProjectManager.GetActiveScene();
            if (activeScene != null && entityId != 0)
            {
                var removeGameEntityCommand = new RemoveGameEntityCommand(ProjectManager.Project, entityId, activeScene);
                ProjectManager.Add(removeGameEntityCommand);
            }
        }
    }

    private void ListBoxEntities_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        var listBox = sender as ListBox;
        if (listBox != null && listBox.SelectedItems.Count > 0)
        {
            var entity = listBox.SelectedItems[0];
            ProjectComponentView.Instance.DataContext = entity;
        }
        else if (listBox != null)
        {
            ProjectComponentView.Instance.DataContext = null;
        }
    }
    private void OnSceneName_Click(object sender, MouseButtonEventArgs e)
    {
        var textBlock = sender as TextBlock;
        if (textBlock != null)
        {
            var scene = textBlock.DataContext as Scene;
            if (scene != null)
            {
                if (ProjectManager.GetActiveScene().isLoaded != null)
                {
                    ProjectManager.GetActiveScene().isLoaded = false;
                }
                scene.isLoaded = !scene.isLoaded;
            }
        }
    }

}
