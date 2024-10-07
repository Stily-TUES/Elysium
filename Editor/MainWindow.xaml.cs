using Editor.GameProject;
using System.ComponentModel;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Editor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            OpenBrowsingDialog();
            InitializeComponent();
            Loaded += MainWindowLoaded;
            Closing += MainWindowClosing;
        }

        private void MainWindowLoaded(object? sender, RoutedEventArgs e)
        {
            Loaded -= MainWindowLoaded;
        }

        private void MainWindowClosing(object? sender, CancelEventArgs e)
        {
            Closing -= MainWindowClosing;
            if (Project.CurrentLoadedProject != null)
            {
                Project.Unload();
            }
        }

        private void OpenBrowsingDialog()
        {
            var browsingDialog = new BrowsingDialog();

            if (browsingDialog.ShowDialog() == false || browsingDialog.DataContext == null)
            {
                Application.Current.Shutdown();
            }
            else
            {
                if (Project.CurrentLoadedProject != null)
                {
                    Project.Unload();
                }
                DataContext = browsingDialog.DataContext;
            }
        }

    }
}