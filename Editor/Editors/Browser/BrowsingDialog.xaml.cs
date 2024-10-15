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
using System.Windows.Shapes;

namespace Editor.Editors;

/// <summary>
/// Interaction logic for BrowsingDialog.xaml
/// </summary>
public partial class BrowsingDialog : Window
{
    public BrowsingDialog()
    {
        InitializeComponent();
    }
    private void OnProjectButton_Click(object sender, RoutedEventArgs e)
    {
        if (sender == openProjectButton)
        {
            if (newProjectButton.IsChecked == true)
            {
                newProjectButton.IsChecked = false;
                BrowserContent.Margin = new Thickness(0);
            }
            openProjectButton.IsChecked = true;
        }
        else
        {
            if (openProjectButton.IsChecked == true)
            {
                openProjectButton.IsChecked = false;
                BrowserContent.Margin = new Thickness(-800, 0, 0, 0);
            }
            newProjectButton.IsChecked = true;
        }
    }
}
