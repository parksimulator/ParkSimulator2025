using Silk.NET.Assimp;
using Silk.NET.OpenGL;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;


namespace GemeloDigital
{
    public class GLTexture : IDisposable
    {
        uint handle;
        GL openGL;

        public TextureType type { get; }

        public unsafe GLTexture(GL gl, string file, TextureType _type = TextureType.None) : this(gl, System.IO.File.ReadAllBytes(file), _type)
        {

        }

        public unsafe GLTexture(GL gl, byte[] bytes, TextureType _type = TextureType.None)
        {
            openGL = gl;
            type = _type;
            handle = openGL.GenTexture();
            Bind();

            using (var img = Image.Load<Rgba32>(bytes))
            {
                gl.TexImage2D(TextureTarget.Texture2D, 0, InternalFormat.Rgba8, (uint)img.Width, (uint)img.Height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, null);

                img.ProcessPixelRows(accessor =>
                {
                    for (int y = 0; y < accessor.Height; y++)
                    {
                        fixed (void* data = accessor.GetRowSpan(y))
                        {
                            gl.TexSubImage2D(TextureTarget.Texture2D, 0, 0, y, (uint)accessor.Width, 1, PixelFormat.Rgba, PixelType.UnsignedByte, data);
                        }
                    }
                });
            }

            SetParameters();
        }

        public unsafe GLTexture(GL gl, Span<byte> data, uint width, uint height)
        {
            openGL = gl;

            handle = openGL.GenTexture();
            Bind();

            fixed (void* d = &data[0])
            {
                openGL.TexImage2D(TextureTarget.Texture2D, 0, (int)InternalFormat.Rgba, width, height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, d);
                SetParameters();
            }
        }

        void SetParameters()
        {
            openGL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)GLEnum.Repeat);
            openGL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)GLEnum.Repeat);
            openGL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)GLEnum.LinearMipmapLinear);
            openGL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)GLEnum.Linear);
            openGL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureBaseLevel, 0);
            openGL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMaxLevel, 8);
            openGL.GenerateMipmap(TextureTarget.Texture2D);
        }

        public void Bind(TextureUnit textureSlot = TextureUnit.Texture0)
        {
            openGL.ActiveTexture(textureSlot);
            openGL.BindTexture(TextureTarget.Texture2D, handle);
        }

        public void Dispose()
        {
            openGL.DeleteTexture(handle);
        }
    }
}
