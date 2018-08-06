using MonoGame.OpenGL;
using System;

namespace Microsoft.Xna.Framework.Graphics
{
    public abstract partial class BufferBase : GraphicsResource
    {
        internal int _vbo;
        internal BufferUsageHint _usageHint;

        internal protected override void GraphicsDeviceResetting()
        {
            _vbo = 0;
        }

        internal virtual void DiscardCheck(BufferTarget target, SetDataOptions options, IntPtr bufferSize)
        {
            if (options == SetDataOptions.Discard)
            {
                // By assigning NULL data to the buffer this gives a hint
                // to the device to discard the previous content.
                var usageHint = _isDynamic ? BufferUsageHint.StreamDraw : BufferUsageHint.StaticDraw;
                GL.BufferData(target, bufferSize, IntPtr.Zero, usageHint);
                GraphicsExtensions.CheckGLError();
            }
        }

        protected virtual void PlatformConstruct()
        {
            _usageHint = _isDynamic ? BufferUsageHint.StreamDraw : BufferUsageHint.StaticDraw;
            Threading.BlockOnUIThread(GenerateIfRequired);
        }

        protected abstract void GenerateIfRequired();
    }
}
