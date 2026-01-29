using Silk.NET.OpenGL;

namespace GemeloDigital
{
    public class GLMesh : IDisposable
    {
        public float[] vertices { get; set; }
        public uint[] indices { get; set; }
        public GLVertexArrayObject<float, uint> vertexArrayObject { get; set; }
        public GLBufferObject<float> vertexBufferObject { get; set; }
        public GLBufferObject<uint> elementBufferObject { get; set; }
        public GL openGL { get; }

        public GLMesh(GL gl, float[] _vertices, uint[] _indices, List<GLTexture> _textures)
        {
            openGL = gl;
            vertices = _vertices;
            indices = _indices;
            SetupMesh();
        }

        public void SetupMesh()
        {
            elementBufferObject = new GLBufferObject<uint>(openGL, indices, BufferTargetARB.ElementArrayBuffer);
            vertexBufferObject = new GLBufferObject<float>(openGL, vertices, BufferTargetARB.ArrayBuffer);
            vertexArrayObject = new GLVertexArrayObject<float, uint>(openGL, vertexBufferObject, elementBufferObject);
            vertexArrayObject.VertexAttributePointer(0, 3, VertexAttribPointerType.Float, 8, 0);
            vertexArrayObject.VertexAttributePointer(1, 2, VertexAttribPointerType.Float, 8, 3);
            vertexArrayObject.VertexAttributePointer(2, 3, VertexAttribPointerType.Float, 8, 5);
        }

        public void Bind()
        {
            vertexArrayObject.Bind();
        }

        public void Dispose()
        {
            //textures = null;
            vertexArrayObject.Dispose();
            vertexBufferObject.Dispose();
            elementBufferObject.Dispose();
        }
    }
}
