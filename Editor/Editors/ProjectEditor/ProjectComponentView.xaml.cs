using Editor.Commands;
using Editor.Components;
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
/// Interaction logic for ProjectComponentView.xaml
/// </summary>
public partial class ProjectComponentView : UserControl
{
    public static ProjectComponentView Instance { get; private set; }
    public ProjectManager ProjectManager { get; set; }
    public ProjectComponentView()
    {
        InitializeComponent();
        DataContext = null;
        Instance = this;
    }
    private void RenameTextBox_KeyDown(object sender, KeyEventArgs e)
    {
        var window = Window.GetWindow(this);
        ProjectManager = (ProjectManager)window.DataContext;
        if (e.Key == Key.Enter)
        {
            var gameEntity = (GameEntity)this.DataContext;
            if (gameEntity != null)
            {
                var newName = RenameTextBox.Text;
                var renameCommand = new RenameCommand(ProjectManager.Project, gameEntity, gameEntity.Name, newName);
                ProjectManager.Add(renameCommand);
                gameEntity.Rename(newName);
            }
        }
    }
    private void OnTextureDrop(object sender, DragEventArgs e)
    {
        if (e.Data.GetDataPresent(typeof(TextureFile)))
        {
            var textureFile = (TextureFile)e.Data.GetData(typeof(TextureFile));

            var gameEntity = (GameEntity)this.DataContext;
            if (gameEntity != null)
            {
                gameEntity.Texture = textureFile;
            }
        }
    }
}
