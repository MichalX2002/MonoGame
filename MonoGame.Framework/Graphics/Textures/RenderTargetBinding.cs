// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

// Author: Kenneth James Pouncey

using System;

namespace MonoGame.Framework.Graphics
{
    // http://msdn.microsoft.com/en-us/library/ff434403.aspx
    public readonly struct RenderTargetBinding
    {
        public Texture RenderTarget { get; }
        public int ArraySlice { get; }
        public DepthFormat DepthFormat { get; }

        public RenderTargetBinding(RenderTarget2D renderTarget)
        {
            RenderTarget = renderTarget ?? throw new ArgumentNullException(nameof(renderTarget));
            ArraySlice = 0;
            DepthFormat = renderTarget.DepthStencilFormat;
        }

        public RenderTargetBinding(RenderTargetCube renderTarget, CubeMapFace cubeMapFace)
        {
            RenderTarget = renderTarget ?? throw new ArgumentNullException(nameof(renderTarget));

            if (cubeMapFace < CubeMapFace.PositiveX || cubeMapFace > CubeMapFace.NegativeZ)
                throw new ArgumentOutOfRangeException(nameof(cubeMapFace));

            ArraySlice = (int)cubeMapFace;
            DepthFormat = renderTarget.DepthStencilFormat;
        }

        public RenderTargetBinding(RenderTarget2D renderTarget, int arraySlice)
        {
            if (renderTarget == null)
                throw new ArgumentNullException(nameof(renderTarget));

            if (arraySlice < 0 || arraySlice >= renderTarget.ArraySize)
                throw new ArgumentOutOfRangeException(nameof(arraySlice));

            if (arraySlice > 0 && !renderTarget.GraphicsDevice.Capabilities.SupportsTextureArrays)
            {
                throw new InvalidOperationException(
                    "Texture arrays are not supported on this graphics device.");
            }

            RenderTarget = renderTarget;
            ArraySlice = arraySlice;
            DepthFormat = renderTarget.DepthStencilFormat;
        }

#if DIRECTX

        public RenderTargetBinding(RenderTarget3D renderTarget)
        {
            RenderTarget = renderTarget ?? throw new ArgumentNullException(nameof(renderTarget));
            ArraySlice = 0;
            DepthFormat = renderTarget.DepthStencilFormat;
        }

        public RenderTargetBinding(RenderTarget3D renderTarget, int arraySlice)
        {
            if (renderTarget == null)
                throw new ArgumentNullException(nameof(renderTarget));
            if (arraySlice < 0 || arraySlice >= renderTarget.Depth)
                throw new ArgumentOutOfRangeException(nameof(arraySlice));

            RenderTarget = renderTarget;
            ArraySlice = arraySlice;
            DepthFormat = renderTarget.DepthStencilFormat;
        }

        public static implicit operator RenderTargetBinding(RenderTarget3D renderTarget)
        {
            return new RenderTargetBinding(renderTarget);
        }

#endif

        public static implicit operator RenderTargetBinding(RenderTarget2D renderTarget)
        {
            return new RenderTargetBinding(renderTarget);
        }
    }
}
