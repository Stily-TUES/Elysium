﻿using Editor.Editors;
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
/// Interaction logic for OpenProjectView.xaml
/// </summary>
public partial class OpenProjectView : UserControl
{
    public OpenProjectView()
    {
        InitializeComponent();
        
    }
    private void OpenRecentProject()
    {
        var project = OpenProject.Open(recentProjectsList.SelectedItem as ProjectMetadata);
        bool dialogResult = false;
        var window = Window.GetWindow(this);
        if (project != null)
        {
            dialogResult = true;
            window.DataContext = project;
        }
        window.DialogResult = dialogResult;
        window.Close();
    }

    private void OnOpenProjectButton_Click(object sender, RoutedEventArgs e)
    {
        OpenRecentProject();
    }

}
