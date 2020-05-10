// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

namespace MonoGame.Framework.Graphics
{
    /// <summary>
    /// Provides information about the capabilities of the
    /// current graphics device. A very useful thread for investigating GL extenion names
    /// http://stackoverflow.com/questions/3881197/opengl-es-2-0-extensions-on-android-devices
    /// </summary>
    internal partial class GraphicsCapabilities
    {
        // The highest possible MSCount
        public const int MultiSampleCountLimit = 32;

        public void Initialize(GraphicsDevice device)
        {
            PlatformInitialize(device);
        }

        /// <summary>
        /// Whether the device fully supports non power-of-two textures, including
        /// mip maps and wrap modes other than CLAMP_TO_EDGE
        /// </summary>
        public bool SupportsNonPowerOfTwo { get; private set; }

        /// <summary>
        /// Whether the device supports anisotropic texture filtering
        /// </summary>
        public bool SupportsTextureFilterAnisotropic { get; private set; }

        public bool SupportsDepth24 { get; private set; }

        public bool SupportsPackedDepthStencil { get; private set; }

        public bool SupportsDepthNonLinear { get; private set; }

        /// <summary>
        /// Gets the support for DXT1
        /// </summary>
        public bool SupportsDxt1 { get; private set; }

        /// <summary>
        /// Gets the support for S3TC (DXT1, DXT3, DXT5)
        /// </summary>
        public bool SupportsS3tc { get; private set; }

        /// <summary>
        /// Gets the support for PVRTC
        /// </summary>
        public bool SupportsPvrtc { get; private set; }

        /// <summary>
        /// Gets the support for ETC1
        /// </summary>
        public bool SupportsEtc1 { get; private set; }

        /// <summary>
        /// Gets the support for ETC2
        /// </summary>
        public bool SupportsEtc2 { get; private set; }

        /// <summary>
        /// Gets the support for ATITC
        /// </summary>
        public bool SupportsAtitc { get; private set; }

        public bool SupportsTextureMaxLevel { get; private set; }

        /// <summary>
        /// True, if sRGB is supported. On Direct3D platforms, this is always <code>true</code>.
        /// On OpenGL platforms, it is <code>true</code> if both framebuffer sRGB
        /// and texture sRGB are supported.
        /// </summary>
        public bool SupportsSRgb { get; private set; }

        public bool SupportsTextureArrays { get; private set; }

        public bool SupportsDepthClamp { get; private set; }

        public bool SupportsVertexTextures { get; private set; }

        /// <summary>
        /// True, if the underlying platform supports floating-point textures. 
        /// For Direct3D platforms this is always <code>true</code>.
        /// For OpenGL Desktop platforms it is always <code>true</code>.
        /// For OpenGL Mobile platforms it requires `GL_EXT_color_buffer_float`.
        /// If the requested format is not supported an <code>NotSupportedException</code>
        /// will be thrown.
        /// </summary>
        public bool SupportsFloatTextures { get; private set; }

        /// <summary>
        /// True, if the underlying platform supports half floating-point textures. 
        /// For Direct3D platforms this is always <code>true</code>.
        /// For OpenGL Desktop platforms it is always <code>true</code>.
        /// For OpenGL Mobile platforms it requires `GL_EXT_color_buffer_half_float`.
        /// If the requested format is not supported an <code>NotSupportedException</code>
        /// will be thrown.
        /// </summary>
        public bool SupportsHalfFloatTextures { get; private set; }

        public bool SupportsNormalized { get; private set; }

        /// <summary>
        /// Gets the max texture anisotropy. This value typically lies
        /// between 0 and 16, where 0 means anisotropic filtering is not
        /// supported.
        /// </summary>
        public int MaxTextureAnisotropy { get; private set; }

        public int MaxMultiSampleCount { get; private set; }

        public bool SupportsInstancing { get; private set; }

        public bool SupportsBaseIndexInstancing { get; private set; }

        public bool SupportsSeparateBlendStates { get; private set; }

        public bool SupportsAsync { get; private set; }
    }
}
