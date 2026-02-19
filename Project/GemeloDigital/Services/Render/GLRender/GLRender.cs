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
        Vector4 clearColor;


        Matrix4x4 modelMatrix; 
        Matrix4x4 viewMatrix;
        Matrix4x4 projectionMatrix;

        GL context;
        IWindow window;

        bool isRunning;
        Thread thread;

        List<Object3D> objects3DList;
        Camera3D camera;

        Dictionary<string, GLShader> shaderResources;
        Dictionary<string, GLTexture> textureResources;
        Dictionary<string, GLModel> modelResources;

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
            objects3DList = new List<Object3D>();
            shaderResources = new Dictionary<string, GLShader>();
            textureResources = new Dictionary<string, GLTexture>();
            modelResources = new Dictionary<string, GLModel>();

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
            context.ClearColor(clearColor.X / 255.0f, clearColor.Y / 255.0f, clearColor.Z / 255.0f, clearColor.W / 255.0f);
            context.Clear((uint)(AttribMask.ColorBufferBit | AttribMask.DepthBufferBit));

            lock(SimulatorCore.SceneLock)
            {

                List<SimulatedObject> objs = SimulatorCore.FindObjectsOfType(SimulatedObjectType.Camera3D);

                if(objs.Count <= 0) { return; }

                camera = SimulatorCore.AsCamera3D(objs[0]);

                Vector3 cameraPos = camera.Position;
                Vector3 cameraRot = DegreesToRadians(camera.Rotation);
                float cameraFov = DegreesToRadians(camera.FOV);
                float zNear = camera.ZNear;
                float zFar = camera.ZFar;

                float zRads = DegreesToRadians(0);
                float yRads = DegreesToRadians(180);
                float xRads = DegreesToRadians(0);

                float fovRads = DegreesToRadians(45);

                Vector2D<int> size = window.FramebufferSize;

                Matrix4x4 viewInverted = Matrix4x4.CreateTranslation(cameraPos) *
                             Matrix4x4.CreateRotationZ(cameraRot.Z) *
                             Matrix4x4.CreateRotationY(cameraRot.Y) *
                             Matrix4x4.CreateRotationX(cameraRot.X);

                Matrix4x4 viewMatrix;
                Matrix4x4.Invert(viewInverted, out viewMatrix);

                projectionMatrix = Matrix4x4.CreatePerspectiveFieldOfView(fovRads, (float)size.X / size.Y, zNear, zFar);

                context.Disable(GLEnum.Blend);
                context.Disable(GLEnum.PolygonOffsetFill);
                context.DepthMask(true);

                objs = SimulatorCore.FindObjectsOfType(SimulatedObjectType.Object3D);

                foreach(SimulatedObject o in objs)
                {
                    Object3D object3D = SimulatorCore.AsObject3D(o);

                    if(object3D.Material == null || object3D.Model == null) { continue; }
                    if(object3D.Material.Shader == null || object3D.Material.Texture == null) { continue; }

                    GLTexture texture = GetOrLoadTexture(object3D.Material.Texture.ResourceId);
                    GLShader shader = GetOrLoadShader(object3D.Material.Shader.ResourceId);
                    GLModel model = GetOrLoadModel(object3D.Model.ResourceId);

                    if(texture == null || shader == null || model == null) { continue; }

                    texture.Bind();
                    shader.Use();
                    shader.SetUniform("uTexture0", 0);
                    shader.SetUniform("uView", viewMatrix);
                    shader.SetUniform("uProjection", projectionMatrix);

                    Vector3 objPos = object3D.Position;
                    Vector3 objRot = DegreesToRadians(object3D.Rotation);
                    Vector3 objScale = object3D.Scale;

                    var modelMatrix = Matrix4x4.CreateTranslation(objPos) *
                             Matrix4x4.CreateRotationZ(objRot.Z) *
                             Matrix4x4.CreateRotationY(objRot.Y) *
                             Matrix4x4.CreateRotationX(objRot.X);

                    int c = model.meshes.Count;
                    for (int i = 0; i < c; i++)
                    {
                        GLMesh m = model.meshes[i];
                        m.Bind();
                        shader.SetUniform("uModel", modelMatrix);

                        context.DrawArrays(PrimitiveType.Triangles, 0, (uint)m.vertices.Length);
                    }
                }

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

            //model = new GLModel(context, "Resources\\Model.obj");
            //texture = new GLTexture(context, "Resources\\Texture.png");

            //string shaderSource = File.ReadAllText("Resources\\Shader.shader");
            //string[] subShadersSource = shaderSource.Split("----");

            //shader = new GLShader(context, subShadersSource[0], subShadersSource[1]);

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
                foreach(GLTexture t in textureResources.Values)
                {
                    t.Dispose();
                }

                foreach(GLShader s in shaderResources.Values)
                {
                    s.Dispose();
                }

                foreach(GLModel m in modelResources.Values)
                {
                    m.Dispose();
                }

                context.Dispose();

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

        GLTexture GetOrLoadTexture(string resourceId)
        {
            if(String.IsNullOrEmpty(resourceId))
            {
                return null;
            }
            else if(!SimulatorCore.ExistsResource(resourceId))
            {
                return null;
            }
            else if(!textureResources.ContainsKey(resourceId))
            {
                byte[] bytes = SimulatorCore.GetBinaryResource(resourceId);
                GLTexture texture = new GLTexture(context, bytes);
                textureResources.Add(resourceId, texture);

                return texture;
            }
            else
            {
                return textureResources[resourceId];
            }


        }

        GLModel GetOrLoadModel(string resourceId)
        {
            if(String.IsNullOrEmpty(resourceId))
            {
                return null;
            }
            else if(!SimulatorCore.ExistsResource(resourceId))
            {
                return null;
            }
            else if(!modelResources.ContainsKey(resourceId))
            {
                byte[] bytes = SimulatorCore.GetBinaryResource(resourceId);
                GLModel model = new GLModel(context, bytes);
                modelResources.Add(resourceId, model);

                return model;
            }
            else
            {
                return modelResources[resourceId];
            }


        }

        GLShader GetOrLoadShader(string resourceId)
        {
            if(String.IsNullOrEmpty(resourceId))
            {
                return null;
            }
            else if(!SimulatorCore.ExistsResource(resourceId))
            {
                return null;
            }
            else if(!shaderResources.ContainsKey(resourceId))
            {
                string text = SimulatorCore.GetTextResource(resourceId, Encoding.UTF8);
                string[] parts = text.Split("----");
                string vertex = parts[0];
                string fragment = parts[1];
                GLShader shader = new GLShader(context, vertex, fragment);
                shaderResources.Add(resourceId, shader);

                return shader;
            }
            else
            {
                return shaderResources[resourceId];
            }


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
