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

namespace Editor.Editors
{
    /// <summary>
    /// Interaction logic for ProjectEditorView.xaml
    /// </summary>
    public partial class ProjectEditorView : UserControl
    {
        public ProjectEditorView()
        {
            InitializeComponent();
        }

        private void onUndoButton_Click(object sender, RoutedEventArgs e)
        {
            var window = Window.GetWindow(this);
            var projectManager = (ProjectManager)window.DataContext;
            projectManager?.Undo();
        }
        private void onRedoButton_Click(object sender, RoutedEventArgs e)
        {
            var window = Window.GetWindow(this);
            var projectManager = (ProjectManager)window.DataContext;
            projectManager?.Redo();
        }
        private void onSaveButton_Click(object sender, RoutedEventArgs e)
        {
            var window = Window.GetWindow(this);
            var projectManager = (ProjectManager)window.DataContext;
            projectManager?.Save();
        }
    }
}
