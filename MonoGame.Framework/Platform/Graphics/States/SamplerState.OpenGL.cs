// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.Diagnostics;
using MonoGame.OpenGL;
using ExtTextureFilterAnisotropic = MonoGame.OpenGL.TextureParameterName;

namespace MonoGame.Framework.Graphics
{
    public partial class SamplerState
    {
        private readonly float[] _openGLBorderColor = new float[4];

        internal const ExtTextureFilterAnisotropic TextureParameterNameTextureMaxAnisotropy =
            ExtTextureFilterAnisotropic.TextureMaxAnisotropyExt;

        internal const ExtTextureFilterAnisotropic TextureParameterNameTextureMaxLevel =
            ExtTextureFilterAnisotropic.TextureMaxLevel;

        internal void Activate(GraphicsDevice device, TextureTarget target, bool useMipmaps = false)
        {
            if (GraphicsDevice == null)
            {
                // We're now bound to a device... no one should
                // be changing the state of this object now!
                GraphicsDevice = device;
            }
            Debug.Assert(GraphicsDevice == device, "The state was created for a different device!");

            switch (Filter)
            {
                case TextureFilter.Point:
                    if (GraphicsDevice.Capabilities.SupportsTextureFilterAnisotropic)
                    {
                        GL.TexParameter(target, TextureParameterNameTextureMaxAnisotropy, 1f);
                        GL.CheckError();
                    }
                    GL.TexParameter(
                        target, ExtTextureFilterAnisotropic.TextureMinFilter, 
                        (int)(useMipmaps ? TextureMinFilter.NearestMipmapNearest : TextureMinFilter.Nearest));
                    GL.CheckError();

                    GL.TexParameter(
                        target, ExtTextureFilterAnisotropic.TextureMagFilter, (int)TextureMagFilter.Nearest);
                    GL.CheckError();
                    break;

                case TextureFilter.Linear:
                    if (GraphicsDevice.Capabilities.SupportsTextureFilterAnisotropic)
                    {
                        GL.TexParameter(target, TextureParameterNameTextureMaxAnisotropy, 1f);
                        GL.CheckError();
                    }
                    GL.TexParameter(
                        target, ExtTextureFilterAnisotropic.TextureMinFilter, 
                        (int)(useMipmaps ? TextureMinFilter.LinearMipmapLinear : TextureMinFilter.Linear));
                    GL.CheckError();

                    GL.TexParameter(
                        target, ExtTextureFilterAnisotropic.TextureMagFilter, (int)TextureMagFilter.Linear);
                    GL.CheckError();
                    break;

                case TextureFilter.Anisotropic:
                    if (GraphicsDevice.Capabilities.SupportsTextureFilterAnisotropic)
                    {
                        GL.TexParameter(
                            target, TextureParameterNameTextureMaxAnisotropy,
                            MathHelper.Clamp(MaxAnisotropy, 1f, GraphicsDevice.Capabilities.MaxTextureAnisotropy));
                        GL.CheckError();
                    }
                    GL.TexParameter(
                        target, ExtTextureFilterAnisotropic.TextureMinFilter, 
                        (int)(useMipmaps ? TextureMinFilter.LinearMipmapLinear : TextureMinFilter.Linear));
                    GL.CheckError();

                    GL.TexParameter(
                        target, ExtTextureFilterAnisotropic.TextureMagFilter, (int)TextureMagFilter.Linear);
                    GL.CheckError();
                    break;

                case TextureFilter.PointMipLinear:
                    if (GraphicsDevice.Capabilities.SupportsTextureFilterAnisotropic)
                    {
                        GL.TexParameter(target, TextureParameterNameTextureMaxAnisotropy, 1f);
                        GL.CheckError();
                    }
                    GL.TexParameter(
                        target, ExtTextureFilterAnisotropic.TextureMinFilter, 
                        (int)(useMipmaps ? TextureMinFilter.NearestMipmapLinear : TextureMinFilter.Nearest));
                    GL.CheckError();

                    GL.TexParameter(
                        target, ExtTextureFilterAnisotropic.TextureMagFilter, (int)TextureMagFilter.Nearest);
                    GL.CheckError();
                    break;

                case TextureFilter.LinearMipPoint:
                    if (GraphicsDevice.Capabilities.SupportsTextureFilterAnisotropic)
                    {
                        GL.TexParameter(target, TextureParameterNameTextureMaxAnisotropy, 1f);
                        GL.CheckError();
                    }
                    GL.TexParameter(
                        target, ExtTextureFilterAnisotropic.TextureMinFilter, 
                        (int)(useMipmaps ? TextureMinFilter.LinearMipmapNearest : TextureMinFilter.Linear));
                    GL.CheckError();

                    GL.TexParameter(
                        target, ExtTextureFilterAnisotropic.TextureMagFilter, (int)TextureMagFilter.Linear);
                    GL.CheckError();
                    break;

                case TextureFilter.MinLinearMagPointMipLinear:
                    if (GraphicsDevice.Capabilities.SupportsTextureFilterAnisotropic)
                    {
                        GL.TexParameter(target, TextureParameterNameTextureMaxAnisotropy, 1f);
                        GL.CheckError();
                    }
                    GL.TexParameter(
                        target, ExtTextureFilterAnisotropic.TextureMinFilter, 
                        (int)(useMipmaps ? TextureMinFilter.LinearMipmapLinear : TextureMinFilter.Linear));
                    GL.CheckError();

                    GL.TexParameter(
                        target, ExtTextureFilterAnisotropic.TextureMagFilter, (int)TextureMagFilter.Nearest);
                    GL.CheckError();
                    break;

                case TextureFilter.MinLinearMagPointMipPoint:
                    if (GraphicsDevice.Capabilities.SupportsTextureFilterAnisotropic)
                    {
                        GL.TexParameter(target, TextureParameterNameTextureMaxAnisotropy, 1f);
                        GL.CheckError();
                    }
                    GL.TexParameter(
                        target, ExtTextureFilterAnisotropic.TextureMinFilter, 
                        (int)(useMipmaps ? TextureMinFilter.LinearMipmapNearest : TextureMinFilter.Linear));
                    GL.CheckError();

                    GL.TexParameter(
                        target, ExtTextureFilterAnisotropic.TextureMagFilter, (int)TextureMagFilter.Nearest);
                    GL.CheckError();
                    break;

                case TextureFilter.MinPointMagLinearMipLinear:
                    if (GraphicsDevice.Capabilities.SupportsTextureFilterAnisotropic)
                    {
                        GL.TexParameter(target, TextureParameterNameTextureMaxAnisotropy, 1f);
                        GL.CheckError();
                    }
                    GL.TexParameter(
                        target, ExtTextureFilterAnisotropic.TextureMinFilter, 
                        (int)(useMipmaps ? TextureMinFilter.NearestMipmapLinear : TextureMinFilter.Nearest));
                    GL.CheckError();

                    GL.TexParameter(
                        target, ExtTextureFilterAnisotropic.TextureMagFilter, (int)TextureMagFilter.Linear);
                    GL.CheckError();
                    break;

                case TextureFilter.MinPointMagLinearMipPoint:
                    if (GraphicsDevice.Capabilities.SupportsTextureFilterAnisotropic)
                    {
                        GL.TexParameter(target, TextureParameterNameTextureMaxAnisotropy, 1f);
                        GL.CheckError();
                    }
                    GL.TexParameter(
                        target, ExtTextureFilterAnisotropic.TextureMinFilter, 
                        (int)(useMipmaps ? TextureMinFilter.NearestMipmapNearest : TextureMinFilter.Nearest));
                    GL.CheckError();

                    GL.TexParameter(
                        target, ExtTextureFilterAnisotropic.TextureMagFilter, (int)TextureMagFilter.Linear);
                    GL.CheckError();
                    break;

                default:
                    throw new NotSupportedException();
            }

            // Set up texture addressing.
            GL.TexParameter(target, ExtTextureFilterAnisotropic.TextureWrapS, GetWrapMode(AddressU));
            GL.CheckError();
            GL.TexParameter(target, ExtTextureFilterAnisotropic.TextureWrapT, GetWrapMode(AddressV));
            GL.CheckError();
#if !GLES
            // Border color is not supported by glTexParameter in OpenGL ES 2.0
            BorderColor.ToScaledVector4().CopyTo(_openGLBorderColor);
            GL.TexParameter(target, ExtTextureFilterAnisotropic.TextureBorderColor, _openGLBorderColor);
            GL.CheckError();

            // LOD bias is not supported by glTexParameter in OpenGL ES 2.0
            GL.TexParameter(target, ExtTextureFilterAnisotropic.TextureLodBias, MipMapLevelOfDetailBias);
            GL.CheckError();

            // Comparison samplers are not supported in OpenGL ES 2.0 (without an extension, anyway)
            switch (FilterMode)
            {
                case TextureFilterMode.Comparison:
                    GL.TexParameter(
                        target,
                        ExtTextureFilterAnisotropic.TextureCompareMode,
                        (int)TextureCompareMode.CompareRefToTexture);
                    GL.CheckError();

                    GL.TexParameter(
                        target,
                        ExtTextureFilterAnisotropic.TextureCompareFunc,
                        (int)ComparisonFunction.GetDepthFunction());
                    GL.CheckError();
                    break;

                case TextureFilterMode.Default:
                    GL.TexParameter(
                        target, ExtTextureFilterAnisotropic.TextureCompareMode, (int)TextureCompareMode.None);
                    GL.CheckError();
                    break;

                default:
                    throw new InvalidOperationException("Invalid filter mode!");
            }
#endif
            if (GraphicsDevice.Capabilities.SupportsTextureMaxLevel)
            {
                if (MaxMipLevel > 0)
                    GL.TexParameter(TextureTarget.Texture2D, TextureParameterNameTextureMaxLevel, MaxMipLevel);
                else
                    GL.TexParameter(TextureTarget.Texture2D, TextureParameterNameTextureMaxLevel, 1000);
                GL.CheckError();
            }
        }

        private int GetWrapMode(TextureAddressMode textureAddressMode)
        {
            switch (textureAddressMode)
            {
                case TextureAddressMode.Clamp:
                    return (int)TextureWrapMode.ClampToEdge;

                case TextureAddressMode.Wrap:
                    return (int)TextureWrapMode.Repeat;

                case TextureAddressMode.Mirror:
                    return (int)TextureWrapMode.MirroredRepeat;
#if !GLES
                case TextureAddressMode.Border:
                    return (int)TextureWrapMode.ClampToBorder;
#endif
                default:
                    throw new ArgumentException($"No support for {textureAddressMode}.");
            }
        }
    }
}

