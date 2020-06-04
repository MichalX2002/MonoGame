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
        public static GLHandle Null => new GLHandle(0, GLHandleType.Null);

        public int Value { get; }
        public GLHandleType Type { get; }

        public bool IsNull => Value == 0;

        public GLHandle(int handle, GLHandleType type)
        {
            if (handle < 0)
                throw new ArgumentOutOfRangeException(nameof(handle));

            Value = handle;
            Type = type;
        }

        public readonly bool Equals(GLHandle other)
        {
            return this == other;
        }

        public override readonly bool Equals(object obj)
        {
            return obj is GLHandle other && Equals(other);
        }

        public override readonly int GetHashCode()
        {
            return HashCode.Combine(Value, Type);
        }

        public readonly void Free()
        {
            if (Value == 0)
                return;

            switch (Type)
            {
                case GLHandleType.Null:
                    return;

                case GLHandleType.Texture:
                    GL.DeleteTextures(1, Value);
                    break;

                case GLHandleType.Buffer:
                    GL.DeleteBuffers(1, Value);
                    break;

                case GLHandleType.Shader:
                    if (GL.IsShader(Value))
                        GL.DeleteShader(Value);
                    break;

                case GLHandleType.Program:
                    if (GL.IsProgram(Value))
                        GL.DeleteProgram(Value);
                    break;

                case GLHandleType.Framebuffer:
                    GL.DeleteFramebuffers(1, Value);
                    break;

#if !GLES
                case GLHandleType.Query:
                    GL.DeleteQueries(1, Value);
                    break;
#endif

                default:
                    throw new NotSupportedException();
            }
            GL.CheckError();
        }

        public static GLHandle Texture(int handle)
        {
            return new GLHandle(handle, GLHandleType.Texture);
        }

        public static GLHandle Buffer(int handle)
        {
            return new GLHandle(handle, GLHandleType.Buffer);
        }

        public static GLHandle Shader(int handle)
        {
            return new GLHandle(handle, GLHandleType.Shader);
        }

        public static GLHandle Program(int handle)
        {
            return new GLHandle(handle, GLHandleType.Program);
        }

        public static GLHandle Framebuffer(int handle)
        {
            return new GLHandle(handle, GLHandleType.Framebuffer);
        }

        public static GLHandle Query(int handle)
        {
            return new GLHandle(handle, GLHandleType.Query);
        }

        public static implicit operator int(GLHandle handle)
        {
            return handle.Value;
        }

        public static bool operator ==(GLHandle a, GLHandle b)
        {
            return a.Value.Equals(b.Value)
                && a.Type.Equals(b.Type);
        }

        public static bool operator !=(GLHandle a, GLHandle b)
        {
            return !(a == b);
        }
    }
}
