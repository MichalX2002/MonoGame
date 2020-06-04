using System;

namespace MonoGame.OpenGL
{
    internal enum GLHandleType
    {
        Null,
        Texture,
        Buffer,
        Shader,
        Program,
        Query,
        Framebuffer
    }

    internal struct GLHandle : IEquatable<GLHandle>
    {
        public static GLHandle Null => new GLHandle(GLHandleType.Null, 0);

        private int _handle;

        public GLHandleType Type { get; }
        public int Handle => _handle;

        public bool IsNull => _handle == 0;

        public GLHandle(GLHandleType type, int handle)
        {
            if (handle < 0)
                throw new ArgumentOutOfRangeException(nameof(handle));

            Type = type;
            _handle = handle;
        }

        public readonly bool Equals(GLHandle other)
        {
            return Type.Equals(other.Type)
                && Handle.Equals(other.Handle);
        }

        public override readonly bool Equals(object obj)
        {
            return obj is GLHandle other && Equals(other);
        }

        public override readonly int GetHashCode()
        {
            return HashCode.Combine(Type, Handle);
        }

        public readonly void Free()
        {
            if (_handle == 0)
                return;

            switch (Type)
            {
                case GLHandleType.Null:
                    return;

                case GLHandleType.Texture:
                    GL.DeleteTextures(1, _handle);
                    break;

                case GLHandleType.Buffer:
                    GL.DeleteBuffers(1, _handle);
                    break;

                case GLHandleType.Shader:
                    if (GL.IsShader(_handle))
                        GL.DeleteShader(_handle);
                    break;

                case GLHandleType.Program:
                    if (GL.IsProgram(_handle))
                        GL.DeleteProgram(_handle);
                    break;

                case GLHandleType.Framebuffer:
                    GL.DeleteFramebuffers(1, _handle);
                    break;

#if !GLES
                case GLHandleType.Query:
                    GL.DeleteQueries(1, _handle);
                    break;
#endif

                default:
                    throw new NotSupportedException();
            }
            GL.CheckError();
        }

        public static GLHandle Texture(int handle)
        {
            return new GLHandle(GLHandleType.Texture, handle);
        }

        public static GLHandle Buffer(int handle)
        {
            return new GLHandle(GLHandleType.Buffer, handle);
        }

        public static GLHandle Shader(int handle)
        {
            return new GLHandle(GLHandleType.Shader, handle);
        }

        public static GLHandle Program(int handle)
        {
            return new GLHandle(GLHandleType.Program, handle);
        }

        public static GLHandle Framebuffer(int handle)
        {
            return new GLHandle(GLHandleType.Framebuffer, handle);
        }

        public static GLHandle Query(int handle)
        {
            return new GLHandle(GLHandleType.Query, handle);
        }
    }
}
