using Editor.GameProject;
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
using Vector2 = OpenTK.Mathematics.Vector2;
using Editor.Components;
using System.Diagnostics;
using static OpenTK.Graphics.OpenGL.GL;
using Editor.Scripting;

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
    private float aspectRatio;
    private GameEntity? selectedEntity;
    private bool isDragging;
    private Vector2 dragDelta;
    private int backgroundTextureId;
    private Mesh backgroundMesh;
    private PhysicsManager physicsManager;
    private ScriptManager scriptManager;
    private bool isPhysicsRunning;
    private float backgroundMeshSize = 2.0f;
    private int fpsMilliseconds = 16;

    public ProjectEditorView()
    {
        InitializeComponent();
        Loaded += ProjectEditorView_Loaded;
        Unloaded += ProjectEditorView_Unloaded;
        LoadTextures();
        LoadScripts();
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
        renderTimer.Interval = TimeSpan.FromMilliseconds(fpsMilliseconds); //60 FPS
        renderTimer.Tick += RenderTimer_Tick;
        renderTimer.Start();

        camera = new Camera();
        renderer = new Renderer();
        backgroundMesh = Mesh.CreateSquare(backgroundMeshSize);

        scriptManager = ScriptManager.Instance;
        var entities = projectManager.GetActiveScene().GameEntities;
        entities.CollectionChanged += GameEntities_CollectionChanged;
        physicsManager = new PhysicsManager(entities.ToList());
        fpsComboBox.SelectedIndex = 1;

        scriptManager.LoadAllScripts(entities);
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

        backgroundMesh.Dispose();
        scriptManager.Dispose();
    }

    private void GameEntities_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
    {
        if (e.NewItems != null)
        {
            foreach (GameEntity newEntity in e.NewItems)
            {
                physicsManager.AddEntity(newEntity);
                scriptManager.LoadScriptsForEntity(newEntity);
            }
        }

        if (e.OldItems != null)
        {
            foreach (GameEntity oldEntity in e.OldItems)
            {
                physicsManager.RemoveEntity(oldEntity);
            }
        }
    }

    private void fpsComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (fpsComboBox.SelectedItem is ComboBoxItem selectedItem)
        {
            if (int.TryParse(selectedItem.Tag.ToString(), out int fpsMilliseconds))
            {
                renderTimer.Interval = TimeSpan.FromMilliseconds(fpsMilliseconds);
            }
        }
    }

    private void GlControl_Paint(object sender, System.Windows.Forms.PaintEventArgs e)
    {
        Render();
    }

    private void GlControl_Resize(object sender, EventArgs e)
    {
        GL.Viewport(0, 0, glControl.Width, glControl.Height);
        aspectRatio = (float)glControl.Width / glControl.Height;
    }

    private void RenderTimer_Tick(object sender, EventArgs e)
    {
        float deltaTime = (float)renderTimer.Interval.TotalSeconds;
        physicsManager.OnTick(deltaTime);
        camera.Update();
        glControl.Invalidate();
    }

    private void GlControl_MouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
    {
        var deltaX = (float)(e.X - lastMousePosition.X) / camera.Zoom / glControl.Height;
        var deltaY = (float)(e.Y - lastMousePosition.Y) / camera.Zoom / glControl.Height;

        if (e.Button == System.Windows.Forms.MouseButtons.Right)
        {
            camera.Move(new Vector2(-deltaX, deltaY));
        }
        if (e.Button == System.Windows.Forms.MouseButtons.Left)
        {
            if (selectedEntity != null)
            {
                selectedEntity.Transform.Position += new OpenTK.Mathematics.Vector3(deltaX, -deltaY, 0);
                physicsManager.UpdateInitialPosition(selectedEntity);
                glControl.Invalidate();
            }
        }
        lastMousePosition = new Point(e.X, e.Y);
    }

    private void GlControl_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
    {
        if (e.Button == System.Windows.Forms.MouseButtons.Right)
        {
            lastMousePosition = new Point(e.X, e.Y);
        }
        if (e.Button == System.Windows.Forms.MouseButtons.Left)
        {
            var entities = projectManager.GetActiveScene().GameEntities;
            List<GameEntity> selectedEntities = new List<GameEntity>();
            var mousePosition = camera.ScreenToWorldSpace(new(e.X, e.Y), glControl.Width, glControl.Height);

            foreach (var entity in entities)
            {
                if (entity.IsInside(mousePosition))
                {
                    selectedEntities.Add(entity);
                }
            }
            if (selectedEntities.Any())
            {
                selectedEntity = selectedEntities.OrderBy(x => x.Transform.Position.Z).Last();
                dragDelta = mousePosition.Xy;
            }
            else
            {
                selectedEntity = null;
            }
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
        if (e.Button == System.Windows.Forms.MouseButtons.Left)
        {
            isDragging = false;
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
            renderer.RenderBackground(backgroundMesh, projectManager.GetActiveScene().Background.ImagePath);
            projectManager.RenderProject(camera, aspectRatio);
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

    private void LoadScripts()
    {
        if (ScriptFile.ScriptFiles.Any())
        {
            ScriptsListBox.ItemsSource = ScriptFile.ScriptFiles;
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

    private void RunPhysicsButton_Click(object sender, RoutedEventArgs e)
    {
        physicsManager.StartSimulation();
    }

    private void StopPhysicsButton_Click(object sender, RoutedEventArgs e)
    {
        physicsManager.StopSimulation();
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

    private void onScriptDrag_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        var stackPanel = sender as StackPanel;
        var scriptFile = stackPanel.DataContext as ScriptFile;

        if (scriptFile != null)
        {
            DragDrop.DoDragDrop(stackPanel, scriptFile, DragDropEffects.Copy);
        }
    }
}
