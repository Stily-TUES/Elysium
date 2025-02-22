using Editor.Commands;
using Editor.Components;
using Editor.GameProject;
using Editor.Scripting;
using Editor.Utils;
using System;
using System.Collections.Generic;
using System.Globalization;
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
    public ScriptManager ScriptManager { get; set; }
    public ProjectComponentView()
    {
        InitializeComponent();
        DataContext = null;
        Instance = this;
        ScriptManager = new ScriptManager();
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
    private void TransformTextBox_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Enter)
        {
            var textBox = sender as TextBox;
            if (textBox != null)
            {
                var viewModel = DataContext as GameEntity;
                if (viewModel != null)
                {
                    var propertyName = textBox.GetBindingExpression(TextBox.TextProperty).ResolvedSourcePropertyName;
                    var oldValue = (float)viewModel.Transform.GetType().GetProperty(propertyName).GetValue(viewModel.Transform);
                    //because float.Parse uses , as decimal separator not . we need to use InvariantCulture
                    var newValue = float.Parse(textBox.Text, CultureInfo.InvariantCulture.NumberFormat);
                    if (oldValue != newValue)
                    {
                        var window = Window.GetWindow(this);
                        ProjectManager = (ProjectManager)window.DataContext;
                        var updateCommand = new UpdateTransformCommand(Project.CurrentLoadedProject, viewModel.Transform, propertyName, oldValue, newValue);
                        updateCommand.Apply();
                        ProjectManager.Add(updateCommand);
                    }
                }
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
    private void OnScriptDrop(object sender, DragEventArgs e)
    {
        if (e.Data.GetDataPresent(typeof(ScriptFile)))
        {
            var scriptFile = (ScriptFile)e.Data.GetData(typeof(ScriptFile));

            var gameEntity = (GameEntity)this.DataContext;
            if (gameEntity != null)
            {
                ScriptManager.AddScript(gameEntity, scriptFile.FilePath);
            }
        }
    }

    private void OnRemoveScriptButton_Click(object sender, RoutedEventArgs e)
    {
        var button = sender as Button;
        var scriptFile = button?.DataContext as ScriptFile;

        var gameEntity = (GameEntity)this.DataContext;
        if (gameEntity != null && scriptFile != null)
        {
            ScriptManager.RemoveScript(gameEntity, scriptFile.FilePath);
        }
    }
}
