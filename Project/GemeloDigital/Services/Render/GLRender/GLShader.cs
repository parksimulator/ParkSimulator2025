using Silk.NET.OpenGL;
using System.Numerics;

namespace GemeloDigital
{
    public class GLShader : IDisposable
    {
        uint handle;
        GL openGL;

        public GLShader(GL gl, string vertexCode, string fragmentCode)
        {
            openGL = gl;

            uint vertex = LoadShader(ShaderType.VertexShader, vertexCode);
            uint fragment = LoadShader(ShaderType.FragmentShader, fragmentCode);
            handle = openGL.CreateProgram();
            openGL.AttachShader(handle, vertex);
            openGL.AttachShader(handle, fragment);
            openGL.LinkProgram(handle);
            openGL.GetProgram(handle, GLEnum.LinkStatus, out var status);
            if (status == 0)
            {
                throw new Exception($"Program failed to link with error: {openGL.GetProgramInfoLog(handle)}");
            }
            openGL.DetachShader(handle, vertex);
            openGL.DetachShader(handle, fragment);
            openGL.DeleteShader(vertex);
            openGL.DeleteShader(fragment);
        }

        public void Use()
        {
            openGL.UseProgram(handle);
        }

        public void SetUniform(string name, int value)
        {
            int location = openGL.GetUniformLocation(handle, name);
            if (location == -1)
            {
                throw new Exception($"{name} uniform not found on shader.");
            }
            openGL.Uniform1(location, value);
        }

        public unsafe void SetUniform(string name, Matrix4x4 value)
        {
            //A new overload has been created for setting a uniform so we can use the transform in our shader.
            int location = openGL.GetUniformLocation(handle, name);
            if (location == -1)
            {
                throw new Exception($"{name} uniform not found on shader.");
            }
            openGL.UniformMatrix4(location, 1, false, (float*)&value);
        }

        public void SetUniform(string name, float value)
        {
            int location = openGL.GetUniformLocation(handle, name);
            if (location == -1)
            {
                throw new Exception($"{name} uniform not found on shader.");
            }
            openGL.Uniform1(location, value);
        }

        public void SetUniform(string name, Vector3 vector)
        {
            int location = openGL.GetUniformLocation(handle, name);
            if(location == -1)
            {
                throw new Exception($"{name} uniform not found on shader.");
            }
            openGL.Uniform3(location, ref vector);
        }

        public void SetUniform(string name, Vector4 vector)
        {
            int location = openGL.GetUniformLocation(handle, name);
            if (location == -1)
            {
                throw new Exception($"{name} uniform not found on shader.");
            }
            openGL.Uniform4(location, ref vector);
        }

        public void Dispose()
        {
            openGL.DeleteProgram(handle);
        }

        uint LoadShader(ShaderType type, string code)
        {
            uint handle = openGL.CreateShader(type);
            openGL.ShaderSource(handle, code);
            openGL.CompileShader(handle);
            string infoLog = openGL.GetShaderInfoLog(handle);
            if (!string.IsNullOrWhiteSpace(infoLog))
            {
                throw new Exception($"Error compiling shader of type {type}, failed with error {infoLog}");
            }

            return handle;
        }
    }
}
