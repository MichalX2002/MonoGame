// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using MonoGame.OpenGL;
using GetParamName = MonoGame.OpenGL.GetPName;

namespace MonoGame.Framework.Graphics
{
    public partial class GraphicsCapabilities
    {
        /// <summary>
        /// True, if GL_ARB_framebuffer_object is supported; false otherwise.
        /// </summary>
        internal bool SupportsFramebufferObjectARB { get; private set; }

        /// <summary>
        /// True, if GL_EXT_framebuffer_object is supported; false otherwise.
        /// </summary>
        internal bool SupportsFramebufferObjectEXT { get; private set; }

        /// <summary>
        /// True, if GL_IMG_multisampled_render_to_texture is supported; false otherwise.
        /// </summary>
        internal bool SupportsFramebufferObjectIMG { get; private set; }

        private void PlatformInitialize(GraphicsDevice device)
        {
            GL.GetInteger(GetParamName.MaxTextureSize, out int maxTexture2DSize);
            GL.CheckError();
            MaxTexture2DSize = maxTexture2DSize;

            GL.GetInteger(GetParamName.MaxTextureSize, out int maxTexture3DSize);
            GL.CheckError();
            MaxTexture3DSize = maxTexture3DSize;

            GL.GetInteger(GetParamName.MaxTextureSize, out int maxTextureCubeSize);
            GL.CheckError();
            MaxTextureCubeSize = maxTextureCubeSize;

            if (GL.IsES)
            {
                SupportsNonPowerOfTwo = GL.HasExtension("GL_OES_texture_npot") ||
                    GL.HasExtension("GL_ARB_texture_non_power_of_two") ||
                    GL.HasExtension("GL_IMG_texture_npot") ||
                    GL.HasExtension("GL_NV_texture_npot_2D_mipmap");
            }
            else
            {
                // Unfortunately non PoT texture support is patchy even on desktop systems and we can't
                // rely on the fact that GL2.0+ supposedly supports npot in the core.
                // Reference: http://aras-p.info/blog/2012/10/17/non-power-of-two-textures/
                SupportsNonPowerOfTwo = MaxTexture2DSize >= 8192;
            }

            SupportsTextureFilterAnisotropic = GL.HasExtension("GL_EXT_texture_filter_anisotropic");

            if (GL.IsES)
            {
                SupportsDepth24 = GL.HasExtension("GL_OES_depth24");
                SupportsPackedDepthStencil = GL.HasExtension("GL_OES_packed_depth_stencil");
                SupportsDepthNonLinear = GL.HasExtension("GL_NV_depth_nonlinear");
                SupportsTextureMaxLevel = GL.HasExtension("GL_APPLE_texture_max_level");
            }
            else
            {
                SupportsDepth24 = true;
                SupportsPackedDepthStencil = true;
                SupportsDepthNonLinear = false;
                SupportsTextureMaxLevel = true;
            }

            // Texture compression
            SupportsS3tc =
                GL.HasExtension("GL_EXT_texture_compression_s3tc") ||
                GL.HasExtension("GL_OES_texture_compression_S3TC") ||
                GL.HasExtension("GL_EXT_texture_compression_dxt3") ||
                GL.HasExtension("GL_EXT_texture_compression_dxt5");

            SupportsDxt1 = SupportsS3tc || GL.HasExtension("GL_EXT_texture_compression_dxt1");
            SupportsPvrtc = GL.HasExtension("GL_IMG_texture_compression_pvrtc");
            SupportsEtc1 = GL.HasExtension("GL_OES_compressed_ETC1_RGB8_texture");
            SupportsAtitc =
                GL.HasExtension("GL_ATI_texture_compression_atitc") ||
                GL.HasExtension("GL_AMD_compressed_ATC_texture");

            if (GL.IsES)
                SupportsEtc2 = device._glMajorVersion >= 3;

            // Framebuffer objects
            if (GL.IsES)
            {
                SupportsFramebufferObjectARB = 
                    device._glMajorVersion >= 2 ||
                    GL.HasExtension("GL_ARB_framebuffer_object"); // always supported on GLES 2.0+

                SupportsFramebufferObjectEXT = GL.HasExtension("GL_EXT_framebuffer_object");
                
                SupportsFramebufferObjectIMG =
                    GL.HasExtension("GL_IMG_multisampled_render_to_texture") |
                    GL.HasExtension("GL_APPLE_framebuffer_multisample") |
                    GL.HasExtension("GL_EXT_multisampled_render_to_texture") |
                    GL.HasExtension("GL_NV_framebuffer_multisample");
            }
            else
            {
                // when on GL 3.0+, frame buffer extensions are guaranteed to be present, but extensions may be missing it
                // is then safe to assume that GL_ARB_framebuffer_object is present so that the standard function are loaded
                SupportsFramebufferObjectARB =
                    device._glMajorVersion >= 3 || GL.HasExtension("GL_ARB_framebuffer_object");

                SupportsFramebufferObjectEXT = GL.HasExtension("GL_EXT_framebuffer_object");
            }

            // Anisotropic filtering
            int anisotropy = 0;
            if (SupportsTextureFilterAnisotropic)
            {
                GL.GetInteger(GetParamName.MaxTextureMaxAnisotropyExt, out anisotropy);
                GL.CheckError();
            }
            MaxTextureAnisotropy = anisotropy;

            // sRGB
            if (GL.IsES)
            {
                SupportsSRgb = GL.HasExtension("GL_EXT_sRGB");

                SupportsFloatTextures = device._glMajorVersion >= 3 || GL.HasExtension("GL_EXT_color_buffer_float");
                SupportsHalfFloatTextures = device._glMajorVersion >= 3 || GL.HasExtension("GL_EXT_color_buffer_half_float");
                SupportsNormalized = device._glMajorVersion >= 3 && GL.HasExtension("GL_EXT_texture_norm16");
            }
            else
            {
                SupportsSRgb = GL.HasExtension("GL_EXT_texture_sRGB") && GL.HasExtension("GL_EXT_framebuffer_sRGB");
                SupportsFloatTextures = device._glMajorVersion >= 3 || GL.HasExtension("GL_ARB_texture_float");
                SupportsHalfFloatTextures = device._glMajorVersion >= 3 || GL.HasExtension("GL_ARB_half_float_pixel");
                SupportsNormalized = device._glMajorVersion >= 3 || GL.HasExtension("GL_EXT_texture_norm16");
            }

            // TODO: Implement OpenGL support for texture arrays
            // once we can author shaders that use texture arrays.
            SupportsTextureArrays = false;

            SupportsDepthClamp = GL.HasExtension("GL_ARB_depth_clamp");

            SupportsVertexTextures = false; // For now, until we implement vertex textures in OpenGL.

            GL.GetInteger(GetParamName.MaxSamples, out int maxSamples);
            MaxMultiSampleCount = maxSamples;

            SupportsInstancing = GL.VertexAttribDivisor != null;

            SupportsBaseIndexInstancing = GL.DrawElementsInstancedBaseInstance != null;

            if (GL.IsES)
            {
                SupportsSeparateBlendStates = false;
            }
            else
            {
                SupportsSeparateBlendStates =
                    device._glMajorVersion >= 4 || GL.HasExtension("GL_ARB_draw_buffers_blend");
            }

            SupportsAsync = false;
        }

    }
}
