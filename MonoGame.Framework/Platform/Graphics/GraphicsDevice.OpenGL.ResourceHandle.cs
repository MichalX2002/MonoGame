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
            public ResourceType Type { get; }
            public int Handle;

            public ResourceHandle(ResourceType type, int handle)
            {
                Type = type;
                Handle = handle;
            }

            public static ResourceHandle Texture(int handle) => new ResourceHandle(ResourceType.Texture, handle);
            public static ResourceHandle Buffer(int handle) => new ResourceHandle(ResourceType.Buffer, handle);
            public static ResourceHandle Shader(int handle) => new ResourceHandle(ResourceType.Shader, handle);
            public static ResourceHandle Program(int handle) => new ResourceHandle(ResourceType.Program, handle);
            public static ResourceHandle Framebuffer(int handle) => new ResourceHandle(ResourceType.Framebuffer, handle);
            public static ResourceHandle Query(int handle) => new ResourceHandle(ResourceType.Query, handle);

            public void Free()
            {
                switch (Type)
                {
                    case ResourceType.Texture:
                        GL.DeleteTextures(1, ref Handle);
                        break;

                    case ResourceType.Buffer:
                        GL.DeleteBuffers(1, ref Handle);
                        break;

                    case ResourceType.Shader:
                        if (GL.IsShader(Handle))
                            GL.DeleteShader(Handle);
                        break;

                    case ResourceType.Program:
                        if (GL.IsProgram(Handle))
                            GL.DeleteProgram(Handle);
                        break;

                    case ResourceType.Framebuffer:
                        GL.DeleteFramebuffers(1, ref Handle);
                        break;

#if !GLES
                    case ResourceType.Query:
                        GL.DeleteQueries(1, ref Handle);
                        break;
#endif
                }
                GraphicsExtensions.CheckGLError();
            }
        }
    }
}
