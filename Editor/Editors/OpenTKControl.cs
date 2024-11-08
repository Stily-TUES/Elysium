using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using OpenTK.Graphics;
using OpenTK.GLControl;
using OpenTK.Graphics.OpenGL;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.Common;
using Editor.Utils;


namespace Editor.Editors
{
    public class OpenTKControl : GameWindow
    {
        public ProjectManager ProjectManager { get; set; }

        public OpenTKControl() : base(GameWindowSettings.Default, NativeWindowSettings.Default)
        {
            this.Load += OnLoad;
            this.RenderFrame += OnRenderFrame;
            this.Resize += OnResize;
        }

        private void OnLoad(object sender, EventArgs e)
        {

            GL.ClearColor(1.0f, 1.0f, 1.0f, 1.0f);
        }

        private void OnRenderFrame(object sender, FrameEventArgs e)
        {
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            ProjectManager.RenderProject();
            this.SwapBuffers();
        }

        private void OnResize(object sender, EventArgs e)
        {
            GL.Viewport(0, 0, this.Size.X, this.Size.Y);
        }
    }
}
