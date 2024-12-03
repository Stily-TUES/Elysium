﻿using Editor.GameProject;
using Editor.Utils;
using GameEngine;
using OpenTK.GLControl;
using OpenTK.Graphics.OpenGL4;
using System.Numerics;
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
using System.Windows.Threading;
using OpenTK.Mathematics;
using Vector2 = System.Numerics.Vector2;

namespace Editor.Editors;

/// <summary>
/// Interaction logic for ProjectEditorView.xaml
/// </summary>
public partial class ProjectEditorView : UserControl
{
    private GLControl glControl;
    private ProjectManager projectManager;
    private DispatcherTimer renderTimer;
    private Renderer renderer;
    private Camera camera;
    private Point lastMousePosition;
    private float cameraSensitivity = 0.01f;

    public ProjectEditorView()
    {
        InitializeComponent();
        Loaded += ProjectEditorView_Loaded;
        Unloaded += ProjectEditorView_Unloaded;
        LoadTextures();
    }

    private void ProjectEditorView_Loaded(object sender, RoutedEventArgs e)
    {
        glControl = new GLControl();
        glControl.Paint += GlControl_Paint;
        glControl.Resize += GlControl_Resize;
        glControl.MouseMove += GlControl_MouseMove;
        glControl.MouseDown += GlControl_MouseDown;
        glControl.MouseUp += GlControl_MouseUp;
        glControl.MouseWheel += GlControl_MouseWheel;
        windowsFormsHost.Child = glControl;

        var window = Window.GetWindow(this);
        projectManager = (ProjectManager)window.DataContext;

        renderTimer = new DispatcherTimer();
        renderTimer.Interval = TimeSpan.FromMilliseconds(16); //60 FPS
        renderTimer.Tick += RenderTimer_Tick;
        renderTimer.Start();

        camera = new Camera();
        renderer = new Renderer();
    }

    private void ProjectEditorView_Unloaded(object sender, RoutedEventArgs e)
    {
        glControl.Paint -= GlControl_Paint;
        glControl.Resize -= GlControl_Resize;
        glControl.MouseMove -= GlControl_MouseMove;
        glControl.MouseDown -= GlControl_MouseDown;
        glControl.MouseUp -= GlControl_MouseUp;
        glControl.MouseWheel -= GlControl_MouseWheel;
        renderTimer.Stop();
        renderTimer.Tick -= RenderTimer_Tick;
    }

    private void GlControl_Paint(object sender, System.Windows.Forms.PaintEventArgs e)
    {
        Render();
    }

    private void GlControl_Resize(object sender, EventArgs e)
    {
        GL.Viewport(0, 0, glControl.Width, glControl.Height);
    }

    private void RenderTimer_Tick(object sender, EventArgs e)
    {
        camera.Update(0.016f);
        glControl.Invalidate();
    }

    private void GlControl_MouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
    {
        if (e.Button == System.Windows.Forms.MouseButtons.Right)
        {
            var deltaX = (float)(e.X - lastMousePosition.X) * cameraSensitivity;
            var deltaY = (float)(e.Y - lastMousePosition.Y) * cameraSensitivity;
            camera.Move(new Vector2(deltaX, -deltaY));
            lastMousePosition = new Point(e.X, e.Y);
        }
    }

    private void GlControl_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
    {
        if (e.Button == System.Windows.Forms.MouseButtons.Right)
        {
            lastMousePosition = new Point(e.X, e.Y);
        }
    }

    private void GlControl_MouseWheel(object sender, System.Windows.Forms.MouseEventArgs e)
    {
        if (e.Delta > 0)
        {
            camera.ZoomIn(0.1f);
        }
        else
        {
            camera.ZoomOut(0.1f);
        }
    }

    private void GlControl_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
    {
        if (e.Button == System.Windows.Forms.MouseButtons.Right)
        {
            lastMousePosition = new Point(0, 0);
        }
    }

    private void Render()
    {
        if (projectManager != null)
        {
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            if (projectManager.GetActiveScene().Background == null)
            {
                projectManager.GetActiveScene().Background = new TextureFile();
            }
            renderer.RenderBackground(projectManager.GetActiveScene().Background.ImagePath);
            projectManager.RenderProject(camera);
            glControl.SwapBuffers();
        }
    }

    private void LoadTextures()
    {
        if (TextureFile.TextureFiles.Any())
        {
            TexturesListBox.ItemsSource = TextureFile.TextureFiles;
        }
        else
        {
            MessageBox.Show("Textures folder not found.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void onUndoButton_Click(object sender, RoutedEventArgs e)
    {
        projectManager?.Undo();
    }

    private void onRedoButton_Click(object sender, RoutedEventArgs e)
    {
        projectManager?.Redo();
    }

    private void onSaveButton_Click(object sender, RoutedEventArgs e)
    {
        projectManager?.Save();
    }

    private void onTextureDrag_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        var stackPanel = sender as StackPanel;
        var textureFile = stackPanel.DataContext as TextureFile;

        if (textureFile != null)
        {
            DragDrop.DoDragDrop(stackPanel, textureFile, DragDropEffects.Copy);
        }
    }
}
