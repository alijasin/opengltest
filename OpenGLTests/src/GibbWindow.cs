using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenGLTests.src.Util;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;

namespace OpenGLTests.src
{
    class GibbWindow : GameWindow
    {
        public Action<DrawAdapter> RenderCallback { get; private set; }
        public const int WIDTH = 720;
        public const int HEIGHT = 720;
        public GibbWindow(Action<DrawAdapter> renderCallback) : base(WIDTH, HEIGHT, new OpenTK.Graphics.GraphicsMode(32, 24, 0, 8))
        {
            ImageLoader.Init();
            Console.WriteLine("Images loaded");
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
            RenderCallback = renderCallback;
        }

        private int _program;
        private int _vertexArray;
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            Console.WriteLine("Window loaded");
            /*_program = Shader.CompileShaders();
            GL.GenVertexArrays(1, out _vertexArray);
            GL.BindVertexArray(_vertexArray);*/
            Console.WriteLine("Shaders loaded");
            Closed += OnClosed;
        }

        private void OnClosed(object sender, EventArgs e)
        {
            GL.DeleteVertexArrays(1, ref _vertexArray);
            GL.DeleteProgram(_program);
            base.Exit();
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            base.OnRenderFrame(e);

            GL.Clear(ClearBufferMask.ColorBufferBit);
            GL.ClearColor(Color.DarkOliveGreen);
            GL.PushMatrix();

            var drawer = new DrawAdapter();
            RenderCallback(drawer);
            SwapBuffers();
            GL.PopMatrix();
        }

        protected override void OnMouseMove(MouseMoveEventArgs e)
        {
            GraphicsController.ActiveScreen?.HandleInput(new InputUnion(InputUnion.Directions.Down, e));
        }

        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            GraphicsController.ActiveScreen?.HandleInput(new InputUnion(InputUnion.Directions.Down, e));
        }

        protected override void OnMouseUp(MouseButtonEventArgs e)
        {
            GraphicsController.ActiveScreen?.HandleInput(new InputUnion(InputUnion.Directions.Up, e));
        }

        protected override void OnKeyDown(KeyboardKeyEventArgs e)
        {
            GraphicsController.ActiveScreen?.HandleInput(new InputUnion(e.IsRepeat ? InputUnion.Directions.Repeat : InputUnion.Directions.Down, e));
        }

        protected override void OnKeyUp(KeyboardKeyEventArgs e)
        {
            GraphicsController.ActiveScreen?.HandleInput(new InputUnion(InputUnion.Directions.Up, e));
        }

    }
}
