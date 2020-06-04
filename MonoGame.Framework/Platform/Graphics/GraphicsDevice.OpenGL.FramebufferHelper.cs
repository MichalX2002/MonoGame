// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.Diagnostics;
using MonoGame.OpenGL;

namespace MonoGame.Framework.Graphics
{
    // ARB_framebuffer_object implementation
    public partial class GraphicsDevice
    {
        internal class FramebufferHelper
        {
            private static readonly FramebufferAttachment[] FramebufferAttachements = {
                FramebufferAttachment.ColorAttachment0,
                FramebufferAttachment.DepthAttachment,
                FramebufferAttachment.StencilAttachment,
            };

            private static FramebufferHelper _instance;

            public static FramebufferHelper Instance
            {
                get
                {

                    if (_instance == null)
                        throw new InvalidOperationException("The FramebufferHelper has not been created yet!");
                    return _instance;
                }
            }

            public bool SupportsInvalidateFramebuffer { get; private set; }
            public bool SupportsBlitFramebuffer { get; private set; }

            internal FramebufferHelper(GraphicsDevice graphicsDevice)
            {
                SupportsBlitFramebuffer = GL.BlitFramebuffer != null;
                SupportsInvalidateFramebuffer = GL.InvalidateFramebuffer != null;
            }

            public static FramebufferHelper Create(GraphicsDevice gd)
            {
                if (gd.Capabilities.SupportsFramebufferObjectARB ||
                    gd.Capabilities.SupportsFramebufferObjectEXT)
                {
                    _instance = new FramebufferHelper(gd);
                    return _instance;
                }
                else
                {
                    throw new PlatformNotSupportedException(
                        "MonoGame requires either ARB_framebuffer_object or EXT_framebuffer_object." +
                        "Try updating your graphics drivers.");
                }
            }

            internal virtual void GenRenderbuffer(out int renderbuffer)
            {
                GL.GenRenderbuffers(1, out renderbuffer);
                GL.CheckError();
            }

            internal virtual void BindRenderbuffer(int renderbuffer)
            {
                GL.BindRenderbuffer(RenderbufferTarget.Renderbuffer, renderbuffer);
                GL.CheckError();
            }

            internal virtual void DeleteRenderbuffer(int renderbuffer)
            {
                GL.DeleteRenderbuffers(1, renderbuffer);
                GL.CheckError();
            }

            internal virtual void RenderbufferStorageMultisample(int samples, int internalFormat, int width, int height)
            {
                if (samples > 0 && GL.RenderbufferStorageMultisample != null)
                    GL.RenderbufferStorageMultisample(
                        RenderbufferTarget.RenderbufferExt, samples, (RenderbufferStorage)internalFormat, width, height);
                else
                    GL.RenderbufferStorage(RenderbufferTarget.Renderbuffer, (RenderbufferStorage)internalFormat, width, height);
                GL.CheckError();
            }

            internal virtual void GenFramebuffer(out int framebuffer)
            {
                GL.GenFramebuffers(1, out framebuffer);
                GL.CheckError();
            }

            internal virtual void BindFramebuffer(int framebuffer)
            {
                GL.BindFramebuffer(FramebufferTarget.Framebuffer, framebuffer);
                GL.CheckError();
            }

            internal virtual void BindReadFramebuffer(int readFramebuffer)
            {
                GL.BindFramebuffer(FramebufferTarget.ReadFramebuffer, readFramebuffer);
                GL.CheckError();
            }

            internal virtual void InvalidateDrawFramebuffer()
            {
                Debug.Assert(SupportsInvalidateFramebuffer);
                GL.InvalidateFramebuffer (FramebufferTarget.Framebuffer, 3, FramebufferAttachements);
            }

            internal virtual void InvalidateReadFramebuffer()
            {
                Debug.Assert(SupportsInvalidateFramebuffer);
                GL.InvalidateFramebuffer(FramebufferTarget.Framebuffer, 3, FramebufferAttachements);
            }

            internal virtual void DeleteFramebuffer(int framebuffer)
            {
                GL.DeleteFramebuffers(1, framebuffer);
                GL.CheckError();
            }

            internal virtual void FramebufferTexture2D(int attachement, int target, int texture, int level = 0, int samples = 0)
            {
                GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, (FramebufferAttachment)attachement, (TextureTarget)target, texture, level);
                GL.CheckError();
            }

            internal virtual void FramebufferRenderbuffer(int attachement, int renderbuffer, int level = 0)
            {
                GL.FramebufferRenderbuffer(FramebufferTarget.Framebuffer, (FramebufferAttachment)attachement, RenderbufferTarget.Renderbuffer, renderbuffer);
                GL.CheckError();
            }

            internal virtual void GenerateMipmap(int target)
            {
                GL.GenerateMipmap((GenerateMipmapTarget)target);
                GL.CheckError();
            }

            internal virtual void BlitFramebuffer(int iColorAttachment, int width, int height)
            {
                GL.ReadBuffer(ReadBufferMode.ColorAttachment0 + iColorAttachment);
                GL.CheckError();

                GL.DrawBuffer(DrawBufferMode.ColorAttachment0 + iColorAttachment);
                GL.CheckError();

                GL.BlitFramebuffer(0, 0, width, height, 0, 0, width, height, ClearBufferMask.ColorBufferBit, BlitFramebufferFilter.Nearest);
                GL.CheckError();
            }

            internal virtual void CheckFramebufferStatus()
            {
                var status = GL.CheckFramebufferStatus(FramebufferTarget.Framebuffer);
                if (status != FramebufferErrorCode.FramebufferComplete)
                {
                    string message = "Framebuffer Incomplete.";
                    switch (status)
                    {
                        case FramebufferErrorCode.FramebufferIncompleteAttachment: message = "Not all framebuffer attachment points are framebuffer attachment complete."; break;
                        case FramebufferErrorCode.FramebufferIncompleteMissingAttachment: message = "No images are attached to the framebuffer."; break;
                        case FramebufferErrorCode.FramebufferUnsupported: message = "The combination of internal formats of the attached images violates an implementation-dependent set of restrictions."; break;
                        case FramebufferErrorCode.FramebufferIncompleteMultisample: message = "Not all attached images have the same number of samples."; break;
                    }
                    throw new InvalidOperationException(message);
                }
            }
        }
    }
}
