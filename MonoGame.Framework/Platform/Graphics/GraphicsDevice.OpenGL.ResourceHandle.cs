using MonoGame.OpenGL;

namespace MonoGame.Framework.Graphics
{
    public partial class GraphicsDevice
    {
        private enum ResourceType
        {
            Texture,
            Buffer,
            Shader,
            Program,
            Query,
            Framebuffer
        }

        private struct ResourceHandle
        {
            private int _handle;

            public ResourceType Type { get; }
            public int Handle => _handle;

            public ResourceHandle(ResourceType type, int handle)
            {
                Type = type;
                _handle = handle;
            }

            public static ResourceHandle Texture(int handle)
            {
                return new ResourceHandle(ResourceType.Texture, handle);
            }

            public static ResourceHandle Buffer(int handle)
            {
                return new ResourceHandle(ResourceType.Buffer, handle);
            }

            public static ResourceHandle Shader(int handle)
            {
                return new ResourceHandle(ResourceType.Shader, handle);
            }

            public static ResourceHandle Program(int handle)
            {
                return new ResourceHandle(ResourceType.Program, handle);
            }

            public static ResourceHandle Framebuffer(int handle)
            {
                return new ResourceHandle(ResourceType.Framebuffer, handle);
            }

            public static ResourceHandle Query(int handle)
            {
                return new ResourceHandle(ResourceType.Query, handle);
            }

            public void Free()
            {
                switch (Type)
                {
                    case ResourceType.Texture:
                        GL.DeleteTextures(1, ref _handle);
                        break;

                    case ResourceType.Buffer:
                        GL.DeleteBuffers(1, ref _handle);
                        break;

                    case ResourceType.Shader:
                        if (GL.IsShader(_handle))
                            GL.DeleteShader(_handle);
                        break;

                    case ResourceType.Program:
                        if (GL.IsProgram(_handle))
                            GL.DeleteProgram(_handle);
                        break;

                    case ResourceType.Framebuffer:
                        GL.DeleteFramebuffers(1, ref _handle);
                        break;

#if !GLES
                    case ResourceType.Query:
                        GL.DeleteQueries(1, ref _handle);
                        break;
#endif
                }
                GraphicsExtensions.CheckGLError();
            }
        }
    }
}
