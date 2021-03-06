// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

namespace MonoGame.Framework.Graphics
{
    public partial class GraphicsCapabilities
    {
        private void PlatformInitialize(GraphicsDevice device)
        {
            if (device._d3dDevice.FeatureLevel >= SharpDX.Direct3D.FeatureLevel.Level_11_0)
            {
                MaxTexture2DSize = SharpDX.Direct3D11.Resource.MaximumTexture2DSize;
                MaxTexture3DSize = SharpDX.Direct3D11.Resource.MaximumTexture3DSize;
                MaxTextureCubeSize = SharpDX.Direct3D11.Resource.MaximumTextureCubeSize;
            }
            else if (device._d3dDevice.FeatureLevel >= SharpDX.Direct3D.FeatureLevel.Level_10_0)
            {
                MaxTexture2DSize = 8192;
                MaxTexture3DSize = 2048;
                MaxTextureCubeSize = 8192;
            }
            else
            {
                MaxTexture2DSize = 4096;
                MaxTexture3DSize = 256;
                MaxTextureCubeSize = 1024;
            }

            SupportsNonPowerOfTwo = device.GraphicsProfile == GraphicsProfile.HiDef;
            SupportsTextureFilterAnisotropic = true;

            SupportsDepth24 = true;
            SupportsPackedDepthStencil = true;
            SupportsDepthNonLinear = false;
            SupportsTextureMaxLevel = true;

            // Texture compression
            SupportsDxt1 = true;
            SupportsS3tc = true;

            SupportsSRgb = true;

            SupportsTextureArrays = device.GraphicsProfile == GraphicsProfile.HiDef;
            SupportsDepthClamp = device.GraphicsProfile == GraphicsProfile.HiDef;
            SupportsVertexTextures = device.GraphicsProfile == GraphicsProfile.HiDef;
            SupportsFloatTextures = true;
            SupportsHalfFloatTextures = true;
            SupportsNormalized = true;

            SupportsInstancing = true;
            SupportsBaseIndexInstancing = true;
            SupportsSeparateBlendStates = true;

            MaxTextureAnisotropy = (device.GraphicsProfile == GraphicsProfile.Reach) ? 2 : 16;

            MaxMultiSampleCount = GetMaxMultiSampleCount(device);

            // TODO: device._d3dDevice.CheckThreadingSupport()?
            SupportsAsync = true;
        }

        private int GetMaxMultiSampleCount(GraphicsDevice device)
        {
            var format = SharpDXHelper.ToFormat(device.PresentationParameters.BackBufferFormat);

            // Find the maximum supported level starting with the game's requested multisampling level
            // and halving each time until reaching 0 (meaning no multisample support).

            int maxLevel = MultiSampleCountLimit;
            while (maxLevel > 0)
            {
                int qualityLevels = device._d3dDevice.CheckMultisampleQualityLevels(format, maxLevel);
                if (qualityLevels > 0)
                    break;
                maxLevel /= 2;
            }
            return maxLevel;
        }
    }
}
