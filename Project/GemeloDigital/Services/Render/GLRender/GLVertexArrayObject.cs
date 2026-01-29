using Silk.NET.OpenGL;

namespace GemeloDigital
{
    public class GLVertexArrayObject<TVertexType, TIndexType> : IDisposable
        where TVertexType : unmanaged
        where TIndexType : unmanaged
    {
        uint handle;
        GL openGL;

        public GLVertexArrayObject(GL gl, GLBufferObject<TVertexType> vbo, GLBufferObject<TIndexType> ebo)
        {
            openGL = gl;

            handle = openGL.GenVertexArray();
            Bind();
            vbo.Bind();
            ebo.Bind();
        }

        public unsafe void VertexAttributePointer(uint index, int count, VertexAttribPointerType type, uint vertexSize, int offSet)
        {
            openGL.VertexAttribPointer(index, count, type, false, vertexSize * (uint)sizeof(TVertexType), (void*)(offSet * sizeof(TVertexType)));
            openGL.EnableVertexAttribArray(index);
        }

        public void Bind()
        {
            openGL.BindVertexArray(handle);
        }

        public void Dispose()
        {
            openGL.DeleteVertexArray(handle);
        }
    }
}
