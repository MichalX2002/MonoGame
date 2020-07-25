// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using MonoGame.OpenGL;

namespace MonoGame.Framework.Graphics
{
    public abstract partial class Texture
    {
        internal GLHandle _glTexture;
        internal TextureTarget _glTarget;
        internal TextureUnit _glTextureUnit = TextureUnit.Texture0;
        internal PixelInternalFormat _glInternalFormat;
        internal PixelFormat _glFormat;
        internal PixelType _glType;
        internal SamplerState? _glLastSamplerState;

        private void PlatformGraphicsDeviceResetting()
        {
            DeleteGLTexture();
            _glLastSamplerState = null;
        }

        private void PlatformDispose(bool disposing)
        {
            if (!IsDisposed)
            {
                DeleteGLTexture();
                _glLastSamplerState = null;
            }
        }

        private void DeleteGLTexture()
        {
            GraphicsDevice.DisposeResource(_glTexture);
            _glTexture = default;
        }
    }
}

