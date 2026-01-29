using Silk.NET.Maths;
using Silk.NET.OpenGL;
using Silk.NET.Windowing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace GemeloDigital
{
    internal class GLRender : Render
    {
        Vector3 clearColor;


        Matrix4x4 modelMatrix; 
        Matrix4x4 viewMatrix;
        Matrix4x4 projectionMatrix;

        GL context;
        IWindow window;

        GLModel model;
        GLTexture texture;
        GLShader shader;  
        
        bool isRunning;
        Thread thread;

        internal override void Initialize()
        {
            Console.WriteLine("GLRender: Initializing");

            isRunning = true;
            thread = new Thread(Run);            
            thread.Start();

        }

        internal override void Finish()
        {
            Console.WriteLine("GLRender: Finish");

            isRunning = false;

            thread.Join();
        }

        private void Run()
        {
            var options = WindowOptions.Default;
            options.Size = new Vector2D<int>(1280, 720);
            options.Title = "Demo3D";
            window = Window.Create(options);


            window.Closing += OnClosing;
            window.Load += OnLoad;
            window.Update += OnUpdate;
            window.Render += OnRender;

            window.Run();

            window.Dispose();
        }

        private void OnRender(double deltaTime)
        {
            float zRads = DegreesToRadians(0);
            float yRads = DegreesToRadians(180);
            float xRads = DegreesToRadians(0);

            float fovRads = DegreesToRadians(45);

            Vector2D<int> size = window.FramebufferSize;

            viewMatrix = Matrix4x4.Identity;

            projectionMatrix = Matrix4x4.CreatePerspectiveFieldOfView(fovRads, (float)size.X / size.Y, 0.5f, 100.0f);

            context.Disable(GLEnum.Blend);
            context.Disable(GLEnum.PolygonOffsetFill);
            context.DepthMask(true);

            texture.Bind();
            shader.Use();
            shader.SetUniform("uTexture0", 0);
            shader.SetUniform("uView", viewMatrix);
            shader.SetUniform("uProjection", projectionMatrix);

            var modelMatrix = Matrix4x4.CreateTranslation(new Vector3(0, 0, -10));

            int c = model.meshes.Count;
            for (int i = 0; i < c; i++)
            {
                GLMesh m = model.meshes[i];
                m.Bind();
                shader.SetUniform("uModel", modelMatrix);

                context.DrawArrays(PrimitiveType.Triangles, 0, (uint)m.vertices.Length);
            }

        }

        void OnLoad()
        {
            window.SetDefaultIcon();

            context = GL.GetApi(window);

            context.Enable(GLEnum.CullFace);

            modelMatrix = Matrix4x4.Identity;
            viewMatrix = Matrix4x4.Identity;
            projectionMatrix = Matrix4x4.Identity;


            window.FramebufferResize += OnFramebufferResize;

            model = new GLModel(context, "Resources\\Model.obj");
            texture = new GLTexture(context, "Resources\\Texture.png");

            string shaderSource = File.ReadAllText("Resources\\Shader.shader");
            string[] subShadersSource = shaderSource.Split("----");

            shader = new GLShader(context, subShadersSource[0], subShadersSource[1]);

        }


        private void OnUpdate(double deltaTime)
        {
            if(!isRunning)
            {
                window.Close();
            }
        }

        private void OnClosing()
        {

            if(isRunning) { window.IsClosing = false; }
            else
            {
                model.Dispose();
                texture.Dispose();
                shader.Dispose();

                window.Closing -= OnClosing;
                window.Render -= OnRender;
                window.Update -= OnUpdate;
                window.Load -= OnLoad;
            }
        }

        void OnFramebufferResize(Vector2D<int> newSize)
        {
            context.Viewport(newSize);
        }


        float DegreesToRadians(float degrees)
        {
            return MathF.PI / 180f * degrees;
        }

        Vector3 DegreesToRadians(Vector3 v)
        {
            return new Vector3(DegreesToRadians(v.X),
                               DegreesToRadians(v.Y),
                               DegreesToRadians(v.Z));
        }

        float RadiansToDegrees(float rads)
        {
            return 180f / MathF.PI * rads;
        }

        Vector3 RadiansToDegrees(Vector3 v)
        {
            return new Vector3(RadiansToDegrees(v.X),
                               RadiansToDegrees(v.Y),
                               RadiansToDegrees(v.Z));
        }


    }
}
