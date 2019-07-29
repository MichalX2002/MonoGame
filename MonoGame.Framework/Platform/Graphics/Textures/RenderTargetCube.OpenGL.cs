// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using MonoGame.OpenGL;

namespace MonoGame.Framework.Graphics
{
    public partial class RenderTargetCube
    {
        int IRenderTarget.GLTexture => _glTexture;

        TextureTarget IRenderTarget.GLTarget => _glTarget;

        int IRenderTarget.GLColorBuffer { get; set; }
        int IRenderTarget.GLDepthBuffer { get; set; }
        int IRenderTarget.GLStencilBuffer { get; set; }

        private void PlatformConstruct(
            GraphicsDevice graphicsDevice, bool mipMap,
            DepthFormat preferredDepthFormat, int preferredMultiSampleCount, RenderTargetUsage usage)
        {
            graphicsDevice.PlatformCreateRenderTarget(
                this, Size, Size, mipMap, Format, preferredDepthFormat, preferredMultiSampleCount, usage);
        }

        TextureTarget IRenderTarget.GetFramebufferTarget(RenderTargetBinding renderTargetBinding)
        {
            return TextureTarget.TextureCubeMapPositiveX + renderTargetBinding.ArraySlice;
        }

        protected override void Dispose(bool disposing)
        {
            if (!IsDisposed)
            {
                GraphicsDevice?.PlatformDeleteRenderTarget(this);
            }
            base.Dispose(disposing);
        }
    }
}
