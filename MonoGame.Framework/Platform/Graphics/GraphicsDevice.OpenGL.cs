// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Diagnostics;
using MonoGame.Utilities.Memory;

#if ANGLE
using OpenTK.Graphics;
#else
using MonoGame.OpenGL;
#endif

namespace MonoGame.Framework.Graphics
{
    public partial class GraphicsDevice
    {
#if DESKTOPGL || ANGLE
        internal IGraphicsContext Context { get; private set; }
#endif

#if !GLES
        private DrawBuffersEnum[] _drawBuffers;
#endif

        enum ResourceType
        {
            Texture,
            Buffer,
            Shader,
            Program,
            Query,
            Framebuffer
        }

        struct ResourceHandle
        {
            public ResourceType type;
            public int handle;

            public static ResourceHandle Texture(int handle)
            {
                return new ResourceHandle() { type = ResourceType.Texture, handle = handle };
            }

            public static ResourceHandle Buffer(int handle)
            {
                return new ResourceHandle() { type = ResourceType.Buffer, handle = handle };
            }

            public static ResourceHandle Shader(int handle)
            {
                return new ResourceHandle() { type = ResourceType.Shader, handle = handle };
            }

            public static ResourceHandle Program(int handle)
            {
                return new ResourceHandle() { type = ResourceType.Program, handle = handle };
            }

            public static ResourceHandle Query(int handle)
            {
                return new ResourceHandle() { type = ResourceType.Query, handle = handle };
            }

            public static ResourceHandle Framebuffer(int handle)
            {
                return new ResourceHandle() { type = ResourceType.Framebuffer, handle = handle };
            }

            public void Free()
            {
                switch (type)
                {
                    case ResourceType.Texture:
                        GL.DeleteTextures(1, ref handle);
                        break;

                    case ResourceType.Buffer:
                        GL.DeleteBuffers(1, ref handle);
                        break;

                    case ResourceType.Shader:
                        if (GL.IsShader(handle))
                            GL.DeleteShader(handle);
                        break;

                    case ResourceType.Program:
                        if (GL.IsProgram(handle))
                            GL.DeleteProgram(handle);
                        break;

#if !GLES
                    case ResourceType.Query:
                        GL.DeleteQueries(1, ref handle);
                        break;
#endif

                    case ResourceType.Framebuffer:
                        GL.DeleteFramebuffers(1, ref handle);
                        break;
                }
                GraphicsExtensions.CheckGLError();
            }
        }

        List<ResourceHandle> _disposeThisFrame = new List<ResourceHandle>();
        List<ResourceHandle> _disposeNextFrame = new List<ResourceHandle>();
        object _disposeActionsLock = new object();

        static List<IntPtr> _disposeContexts = new List<IntPtr>();
        static object _disposeContextsLock = new object();

        private ShaderProgramCache _programCache;
        private ShaderProgram _shaderProgram = null;

        static readonly float[] _posFixup = new float[4];

        private static BufferBindingInfo[] _bufferBindingInfos;
        private static bool[] _newEnabledVertexAttributes;
        internal static readonly HashSet<int> _enabledVertexAttributes = new HashSet<int>();
        internal static bool _attribsDirty;

        internal FramebufferHelper _framebufferHelper;

        internal int _glMajorVersion = 0;
        internal int _glMinorVersion = 0;
        internal int _glFramebuffer = 0;
        internal int MaxVertexAttributes;

        // Keeps track of last applied state to avoid redundant OpenGL calls
        internal bool _lastBlendEnable = false;
        internal BlendState _lastBlendState = new BlendState();
        internal DepthStencilState _lastDepthStencilState = new DepthStencilState();
        internal RasterizerState _lastRasterizerState = new RasterizerState();
        private Vector4 _lastClearColor = Vector4.Zero;
        private float _lastClearDepth = 1.0f;
        private int _lastClearStencil = 0;

        // Get a hashed value based on the currently bound shaders
        // throws an exception if no shaders are bound
        private int ShaderProgramHash
        {
            get
            {
                if (_vertexShader == null && _pixelShader == null)
                    throw new InvalidOperationException("There is no shader bound!");

                if (_vertexShader == null)
                    return _pixelShader.HashKey;
                if (_pixelShader == null)
                    return _vertexShader.HashKey;
                return _vertexShader.HashKey ^ _pixelShader.HashKey;
            }
        }

        internal void SetVertexAttributeArray(bool[] attrs)
        {
            for (int i = 0; i < attrs.Length; i++)
            {
                bool contains = _enabledVertexAttributes.Contains(i);
                if (attrs[i] && !contains)
                {
                    _enabledVertexAttributes.Add(i);
                    GL.EnableVertexAttribArray(i);
                    GraphicsExtensions.CheckGLError();
                }
                else if (!attrs[i] && contains)
                {
                    _enabledVertexAttributes.Remove(i);
                    GL.DisableVertexAttribArray(i);
                    GraphicsExtensions.CheckGLError();
                }
            }
        }

        private void ApplyAttribs(Shader shader, int baseVertex)
        {
            int programHash = ShaderProgramHash;
            bool bindingsChanged = false;

            int vertexBufferCount = _vertexBuffers.Count;
            for (var slot = 0; slot < vertexBufferCount; slot++)
            {
                var vertexBufferBinding = _vertexBuffers.Get(slot);
                var vertexDeclaration = vertexBufferBinding.VertexBuffer.VertexDeclaration;
                var attrInfo = vertexDeclaration.GetAttributeInfo(shader, programHash);

                var vertexStride = vertexDeclaration.VertexStride;
                var offset = new IntPtr(vertexDeclaration.VertexStride * (baseVertex + vertexBufferBinding.VertexOffset));

                if (!_attribsDirty &&
                    _bufferBindingInfos[slot].VertexOffset == offset &&
                    ReferenceEquals(_bufferBindingInfos[slot].AttributeInfo, attrInfo) &&
                    _bufferBindingInfos[slot].InstanceFrequency == vertexBufferBinding.InstanceFrequency &&
                    _bufferBindingInfos[slot].Vbo == vertexBufferBinding.VertexBuffer._vbo)
                    continue;

                bindingsChanged = true;

                GL.BindBuffer(BufferTarget.ArrayBuffer, vertexBufferBinding.VertexBuffer._vbo);
                GraphicsExtensions.CheckGLError();

                // If instancing is not supported, but InstanceFrequency of the buffer is not zero, throw an exception
                if (!GraphicsCapabilities.SupportsInstancing && vertexBufferBinding.InstanceFrequency > 0)
                    throw new PlatformNotSupportedException(
                        "Instanced geometry drawing requires at least OpenGL 3.2 or GLES 3.2. Try upgrading your graphics drivers.");

                var elements = attrInfo.Elements;
                for (int i = 0; i < elements.Count; i++)
                {
                    var element = elements[i];

                    GL.VertexAttribPointer(element.AttributeLocation,
                        element.NumberOfElements,
                        element.VertexAttribPointerType,
                        element.Normalized,
                        vertexStride,
                        new IntPtr(offset.ToInt64() + element.Offset));

                    // only set the divisor if instancing is supported
                    if (GraphicsCapabilities.SupportsInstancing)
                        GL.VertexAttribDivisor(element.AttributeLocation, vertexBufferBinding.InstanceFrequency);

                    GraphicsExtensions.CheckGLError();
                }

                _bufferBindingInfos[slot].VertexOffset = offset;
                _bufferBindingInfos[slot].AttributeInfo = attrInfo;
                _bufferBindingInfos[slot].InstanceFrequency = vertexBufferBinding.InstanceFrequency;
                _bufferBindingInfos[slot].Vbo = vertexBufferBinding.VertexBuffer._vbo;
            }

            _attribsDirty = false;

            if (bindingsChanged)
            {
                for (int i = 0; i < _newEnabledVertexAttributes.Length; i++)
                    _newEnabledVertexAttributes[i] = false;

                for (var slot = 0; slot < vertexBufferCount; slot++)
                {
                    var elements = _bufferBindingInfos[slot].AttributeInfo.Elements;
                    for (int i = 0, c = elements.Count; i < c; i++)
                        _newEnabledVertexAttributes[elements[i].AttributeLocation] = true;
                }
            }
            SetVertexAttributeArray(_newEnabledVertexAttributes);
        }

        private void PlatformSetup()
        {
            _programCache = new ShaderProgramCache(this);
#if DESKTOPGL || ANGLE
            var windowInfo = new WindowInfo(SdlGameWindow.Instance.Handle);

            if (Context == null || Context.IsDisposed)
            {
                Context = GL.CreateContext(windowInfo);
            }

            Context.MakeCurrent(windowInfo);
            Context.SwapInterval = PresentationParameters.PresentationInterval.GetSwapInterval();

            Context.MakeCurrent(windowInfo);
#endif
            MaxTextureSlots = 16;

            GL.GetInteger(GetPName.MaxTextureImageUnits, out MaxTextureSlots);
            GraphicsExtensions.CheckGLError();

            GL.GetInteger(GetPName.MaxTextureSize, out int maxTexture2DSize);
            GraphicsExtensions.CheckGLError();
            MaxTexture2DSize = maxTexture2DSize;

            GL.GetInteger(GetPName.MaxTextureSize, out int maxTexture3DSize);
            GraphicsExtensions.CheckGLError();
            MaxTexture3DSize = maxTexture2DSize;

            GL.GetInteger(GetPName.MaxTextureSize, out int maxTextureCubeSize);
            GraphicsExtensions.CheckGLError();
            MaxTextureCubeSize = maxTextureCubeSize;

            GL.GetInteger(GetPName.MaxVertexAttribs, out MaxVertexAttributes);
            GraphicsExtensions.CheckGLError();

            _maxVertexBufferSlots = MaxVertexAttributes;
            _newEnabledVertexAttributes = new bool[MaxVertexAttributes];

            // try getting the context version
            // GL_MAJOR_VERSION and GL_MINOR_VERSION are GL 3.0+ only, so we need to rely on the GL_VERSION string
            // for non GLES this string always starts with the version number in the "major.minor" format, but can be followed by
            // multiple vendor specific characters
            // For GLES this string is formatted as: OpenGL<space>ES<space><version number><space><vendor-specific information>
#if GLES
            try
            {
                string version = GL.GetString(StringName.Version);

                if (string.IsNullOrEmpty(version))
                    throw new NoSuitableGraphicsDeviceException("Unable to retrieve OpenGL version");

                string[] versionSplit = version.Split(' ');
                if (versionSplit.Length > 2 && versionSplit[0].Equals("OpenGL") && versionSplit[1].Equals("ES"))
                {
                    glMajorVersion = Convert.ToInt32(versionSplit[2].Substring(0, 1));
                    glMinorVersion = Convert.ToInt32(versionSplit[2].Substring(2, 1));
                }
                else
                {
                    glMajorVersion = 1;
                    glMinorVersion = 1;
                }
            }
            catch (FormatException)
            {
                //if it fails we default to 1.1 context
                glMajorVersion = 1;
                glMinorVersion = 1;
            }
#else
            try
            {
                string version = GL.GetString(StringName.Version);

                if (string.IsNullOrEmpty(version))
                    throw new NoSuitableGraphicsDeviceException("Unable to retrieve OpenGL version");

                _glMajorVersion = Convert.ToInt32(version.Substring(0, 1));
                _glMinorVersion = Convert.ToInt32(version.Substring(2, 1));
            }
            catch (FormatException)
            {
                // if it fails, we assume to be on a 1.1 context
                _glMajorVersion = 1;
                _glMinorVersion = 1;
            }

#endif
#if !GLES
#endif

#if !GLES
            // Initialize draw buffer attachment array
            GL.GetInteger(GetPName.MaxDrawBuffers, out int maxDrawBuffers);
            GraphicsExtensions.CheckGLError();
            _drawBuffers = new DrawBuffersEnum[maxDrawBuffers];
            for (int i = 0; i < maxDrawBuffers; i++)
                _drawBuffers[i] = (DrawBuffersEnum)(FramebufferAttachment.ColorAttachment0Ext + i);
#endif
        }

        private void PlatformInitialize()
        {
            _viewport = new Viewport(0, 0, PresentationParameters.BackBufferWidth, PresentationParameters.BackBufferHeight);

            // Ensure the vertex attributes are reset
            _enabledVertexAttributes.Clear();

            // Free all the cached shader programs. 
            _programCache.Clear();
            _shaderProgram = null;

            _framebufferHelper = FramebufferHelper.Create(this);

            // Force resetting states
            PlatformApplyBlend(true);
            DepthStencilState.PlatformApplyState(this, true);
            RasterizerState.PlatformApplyState(this, true);

            _bufferBindingInfos = new BufferBindingInfo[_maxVertexBufferSlots];
            for (int i = 0; i < _bufferBindingInfos.Length; i++)
                _bufferBindingInfos[i] = new BufferBindingInfo(null, IntPtr.Zero, 0, -1);
        }

        private DepthStencilState clearDepthStencilState = new DepthStencilState { StencilEnable = true };

        internal void PlatformClear(ClearOptions options, Vector4 color, float depth, int stencil)
        {
            // TODO: We need to figure out how to detect if we have a
            // depth stencil buffer or not, and clear options relating
            // to them if not attached.

            // Unlike with XNA and DirectX...  GL.Clear() obeys several
            // different render states:
            //
            //  - The color write flags.
            //  - The scissor rectangle.
            //  - The depth/stencil state.
            //
            // So overwrite these states with what is needed to perform
            // the clear correctly and restore it afterwards.
            //
            var prevScissorRect = ScissorRectangle;
            var prevDepthStencilState = DepthStencilState;
            var prevBlendState = BlendState;
            ScissorRectangle = _viewport.Bounds;
            // DepthStencilState.Default has the Stencil Test disabled; 
            // make sure stencil test is enabled before we clear since
            // some drivers won't clear with stencil test disabled
            DepthStencilState = clearDepthStencilState;
            BlendState = BlendState.Opaque;
            ApplyState(false);

            ClearBufferMask bufferMask = 0;
            if ((options & ClearOptions.Target) == ClearOptions.Target)
            {
                if (color != _lastClearColor)
                {
                    GL.ClearColor(color.X, color.Y, color.Z, color.W);
                    GraphicsExtensions.CheckGLError();
                    _lastClearColor = color;
                }
                bufferMask |= ClearBufferMask.ColorBufferBit;
            }
            if ((options & ClearOptions.Stencil) == ClearOptions.Stencil)
            {
                if (stencil != _lastClearStencil)
                {
                    GL.ClearStencil(stencil);
                    GraphicsExtensions.CheckGLError();
                    _lastClearStencil = stencil;
                }
                bufferMask |= ClearBufferMask.StencilBufferBit;
            }

            if ((options & ClearOptions.DepthBuffer) == ClearOptions.DepthBuffer)
            {
                if (depth != _lastClearDepth)
                {
                    GL.ClearDepth(depth);
                    GraphicsExtensions.CheckGLError();
                    _lastClearDepth = depth;
                }
                bufferMask |= ClearBufferMask.DepthBufferBit;
            }

#if MONOMAC
            if (GL.CheckFramebufferStatus(FramebufferTarget.FramebufferExt) == FramebufferErrorCode.FramebufferComplete)
            {
#endif
            GL.Clear(bufferMask);
            GraphicsExtensions.CheckGLError();
#if MONOMAC
            }
#endif

            // Restore the previous render state.
            ScissorRectangle = prevScissorRect;
            DepthStencilState = prevDepthStencilState;
            BlendState = prevBlendState;
        }

        private void PlatformFlush()
        {
            GL.Flush();
        }

        private void PlatformReset()
        {
        }

        private void PlatformDispose()
        {
            // Free all the cached shader programs.
            _programCache.Dispose();

#if DESKTOPGL || ANGLE
            Context.Dispose();
            Context = null;
#endif
        }

        internal void DisposeTexture(int handle)
        {
            if (!IsDisposed)
            {
                lock (_disposeActionsLock)
                {
                    _disposeNextFrame.Add(ResourceHandle.Texture(handle));
                }
            }
        }

        internal void DisposeBuffer(int handle)
        {
            if (!IsDisposed)
            {
                lock (_disposeActionsLock)
                {
                    _disposeNextFrame.Add(ResourceHandle.Buffer(handle));
                }
            }
        }

        internal void DisposeShader(int handle)
        {
            if (!IsDisposed)
            {
                lock (_disposeActionsLock)
                {
                    _disposeNextFrame.Add(ResourceHandle.Shader(handle));
                }
            }
        }

        internal void DisposeProgram(int handle)
        {
            if (!IsDisposed)
            {
                lock (_disposeActionsLock)
                {
                    _disposeNextFrame.Add(ResourceHandle.Program(handle));
                }
            }
        }

        internal void DisposeQuery(int handle)
        {
            if (!IsDisposed)
            {
                lock (_disposeActionsLock)
                {
                    _disposeNextFrame.Add(ResourceHandle.Query(handle));
                }
            }
        }

        internal void DisposeFramebuffer(int handle)
        {
            if (!IsDisposed)
            {
                lock (_disposeActionsLock)
                {
                    _disposeNextFrame.Add(ResourceHandle.Framebuffer(handle));
                }
            }
        }

#if DESKTOPGL || ANGLE
        static internal void DisposeContext(IntPtr resource)
        {
            lock (_disposeContextsLock)
            {
                _disposeContexts.Add(resource);
            }
        }

        static internal void DisposeContexts()
        {
            lock (_disposeContextsLock)
            {
                int count = _disposeContexts.Count;
                for (int i = 0; i < count; ++i)
                    Sdl.GL.DeleteContext(_disposeContexts[i]);
                _disposeContexts.Clear();
            }
        }
#endif

        internal void PlatformPresent()
        {
#if DESKTOPGL || ANGLE
            Context.SwapBuffers();
#endif
            GraphicsExtensions.CheckGLError();

            // Dispose of any GL resources that were disposed in another thread
            int count = _disposeThisFrame.Count;
            for (int i = 0; i < count; ++i)
                _disposeThisFrame[i].Free();
            _disposeThisFrame.Clear();

            lock (_disposeActionsLock)
            {
                // Swap lists so resources added during this draw will be released after the next draw
                var temp = _disposeThisFrame;
                _disposeThisFrame = _disposeNextFrame;
                _disposeNextFrame = temp;
            }
        }

        private void PlatformSetViewport(in Viewport value)
        {
            if (IsRenderTargetBound)
                GL.Viewport(value.X, value.Y, value.Width, value.Height);
            else
                GL.Viewport(value.X, PresentationParameters.BackBufferHeight - value.Y - value.Height, value.Width, value.Height);
            GraphicsExtensions.LogGLError("GraphicsDevice.Viewport_set() GL.Viewport");

            GL.DepthRange(value.MinDepth, value.MaxDepth);
            GraphicsExtensions.LogGLError("GraphicsDevice.Viewport_set() GL.DepthRange");

            // In OpenGL we have to re-apply the special "posFixup"
            // vertex shader uniform if the viewport changes.
            VertexShaderDirty = true;
        }

        private void PlatformApplyDefaultRenderTarget()
        {
            _framebufferHelper.BindFramebuffer(_glFramebuffer);

            // Reset the raster state because we flip vertices
            // when rendering offscreen and hence the cull direction.
            _rasterizerStateDirty = true;

            // Textures will need to be rebound to render correctly in the new render target.
            Textures.Dirty();
        }

        private class RenderTargetBindingArrayComparer : IEqualityComparer<RenderTargetBinding[]>
        {
            public bool Equals(RenderTargetBinding[] first, RenderTargetBinding[] second)
            {
                if (object.ReferenceEquals(first, second))
                    return true;

                if (first == null || second == null)
                    return false;

                if (first.Length != second.Length)
                    return false;

                for (var i = 0; i < first.Length; ++i)
                {
                    if ((first[i].RenderTarget != second[i].RenderTarget) || (first[i].ArraySlice != second[i].ArraySlice))
                    {
                        return false;
                    }
                }

                return true;
            }

            public int GetHashCode(RenderTargetBinding[] array)
            {
                if (array != null)
                {
                    unchecked
                    {
                        int code = 17;
                        foreach (var item in array)
                        {
                            if (item.RenderTarget != null)
                                code = code * 23 + item.RenderTarget.GetHashCode();
                            code = code * 23 + item.ArraySlice.GetHashCode();
                        }
                        return code;
                    }
                }
                return 0;
            }
        }

        // FBO cache, we create 1 FBO per RenderTargetBinding combination
        private Dictionary<RenderTargetBinding[], int> _glFramebuffers = new Dictionary<RenderTargetBinding[], int>(new RenderTargetBindingArrayComparer());
        // FBO cache used to resolve MSAA rendertargets, we create 1 FBO per RenderTargetBinding combination
        private Dictionary<RenderTargetBinding[], int> _glResolveFramebuffers = new Dictionary<RenderTargetBinding[], int>(new RenderTargetBindingArrayComparer());

        internal void PlatformCreateRenderTarget(IRenderTarget renderTarget, int width, int height, bool mipMap, SurfaceFormat preferredFormat, DepthFormat preferredDepthFormat, int preferredMultiSampleCount, RenderTargetUsage usage)
        {
            void Create()
            {
                var color = 0;
                var depth = 0;
                var stencil = 0;

                if (preferredMultiSampleCount > 0 && _framebufferHelper.SupportsBlitFramebuffer)
                {
                    _framebufferHelper.GenRenderbuffer(out color);
                    _framebufferHelper.BindRenderbuffer(color);
                    _framebufferHelper.RenderbufferStorageMultisample(preferredMultiSampleCount, (int)RenderbufferStorage.Rgba8, width, height);
                }

                if (preferredDepthFormat != DepthFormat.None)
                {
                    var depthInternalFormat = RenderbufferStorage.DepthComponent16;
                    var stencilInternalFormat = (RenderbufferStorage)0;
                    switch (preferredDepthFormat)
                    {
                        case DepthFormat.Depth16:
                            depthInternalFormat = RenderbufferStorage.DepthComponent16;
                            break;
#if GLES
                    case DepthFormat.Depth24:
                        if (GraphicsCapabilities.SupportsDepth24)
                            depthInternalFormat = RenderbufferStorage.DepthComponent24Oes;
                        else if (GraphicsCapabilities.SupportsDepthNonLinear)
                            depthInternalFormat = (RenderbufferStorage)0x8E2C;
                        else
                            depthInternalFormat = RenderbufferStorage.DepthComponent16;
                        break;
                    case DepthFormat.Depth24Stencil8:
                        if (GraphicsCapabilities.SupportsPackedDepthStencil)
                            depthInternalFormat = RenderbufferStorage.Depth24Stencil8Oes;
                        else
                        {
                            if (GraphicsCapabilities.SupportsDepth24)
                                depthInternalFormat = RenderbufferStorage.DepthComponent24Oes;
                            else if (GraphicsCapabilities.SupportsDepthNonLinear)
                                depthInternalFormat = (RenderbufferStorage)0x8E2C;
                            else
                                depthInternalFormat = RenderbufferStorage.DepthComponent16;
                            stencilInternalFormat = RenderbufferStorage.StencilIndex8;
                            break;
                        }
                        break;
#else
                        case DepthFormat.Depth24:
                            depthInternalFormat = RenderbufferStorage.DepthComponent24;
                            break;
                        case DepthFormat.Depth24Stencil8:
                            depthInternalFormat = RenderbufferStorage.Depth24Stencil8;
                            break;
#endif
                    }

                    if (depthInternalFormat != 0)
                    {
                        _framebufferHelper.GenRenderbuffer(out depth);
                        _framebufferHelper.BindRenderbuffer(depth);
                        _framebufferHelper.RenderbufferStorageMultisample(preferredMultiSampleCount, (int)depthInternalFormat, width, height);
                        if (preferredDepthFormat == DepthFormat.Depth24Stencil8)
                        {
                            stencil = depth;
                            if (stencilInternalFormat != 0)
                            {
                                _framebufferHelper.GenRenderbuffer(out stencil);
                                _framebufferHelper.BindRenderbuffer(stencil);
                                _framebufferHelper.RenderbufferStorageMultisample(preferredMultiSampleCount, (int)stencilInternalFormat, width, height);
                            }
                        }
                    }
                }

                if (color != 0)
                    renderTarget.GLColorBuffer = color;
                else
                    renderTarget.GLColorBuffer = renderTarget.GLTexture;
                renderTarget.GLDepthBuffer = depth;
                renderTarget.GLStencilBuffer = stencil;
            }

            if (Threading.IsOnUIThread())
                Create();
            else
                Threading.BlockOnUIThread(Create);
        }

        internal void PlatformDeleteRenderTarget(IRenderTarget renderTarget)
        {
            void Delete()
            {
                int color = renderTarget.GLColorBuffer;
                int depth = renderTarget.GLDepthBuffer;
                int stencil = renderTarget.GLStencilBuffer;
                bool colorIsRenderbuffer = color != renderTarget.GLTexture;

                if (color != 0)
                {
                    if (colorIsRenderbuffer)
                        _framebufferHelper.DeleteRenderbuffer(color);
                    if (stencil != 0 && stencil != depth)
                        _framebufferHelper.DeleteRenderbuffer(stencil);
                    if (depth != 0)
                        _framebufferHelper.DeleteRenderbuffer(depth);

                    var bindingsToDelete = new List<RenderTargetBinding[]>();
                    foreach (var bindings in _glFramebuffers.Keys)
                    {
                        foreach (var binding in bindings)
                        {
                            if (binding.RenderTarget == renderTarget)
                            {
                                bindingsToDelete.Add(bindings);
                                break;
                            }
                        }
                    }

                    foreach (var bindings in bindingsToDelete)
                    {
                        if (_glFramebuffers.TryGetValue(bindings, out int fbo))
                        {
                            _framebufferHelper.DeleteFramebuffer(fbo);
                            _glFramebuffers.Remove(bindings);
                        }
                        if (_glResolveFramebuffers.TryGetValue(bindings, out fbo))
                        {
                            _framebufferHelper.DeleteFramebuffer(fbo);
                            _glResolveFramebuffers.Remove(bindings);
                        }
                    }
                }
            }
            if (Threading.IsOnUIThread())
                Delete();
            else
                Threading.BlockOnUIThread(Delete);
        }

        private void PlatformResolveRenderTargets()
        {
            if (RenderTargetCount == 0)
                return;

            var renderTargetBinding = _currentRenderTargetBindings[0];
            var renderTarget = renderTargetBinding.RenderTarget as IRenderTarget;
            if (renderTarget.MultiSampleCount > 0 && _framebufferHelper.SupportsBlitFramebuffer)
            {
                if (!_glResolveFramebuffers.TryGetValue(_currentRenderTargetBindings, out int glResolveFramebuffer))
                {
                    _framebufferHelper.GenFramebuffer(out glResolveFramebuffer);
                    _framebufferHelper.BindFramebuffer(glResolveFramebuffer);
                    for (var i = 0; i < RenderTargetCount; ++i)
                    {
                        var rt = _currentRenderTargetBindings[i].RenderTarget as IRenderTarget;
                        _framebufferHelper.FramebufferTexture2D((int)(FramebufferAttachment.ColorAttachment0 + i), (int)rt.GetFramebufferTarget(renderTargetBinding), rt.GLTexture);
                    }
                    _glResolveFramebuffers.Add((RenderTargetBinding[])_currentRenderTargetBindings.Clone(), glResolveFramebuffer);
                }
                else
                {
                    _framebufferHelper.BindFramebuffer(glResolveFramebuffer);
                }
                // The only fragment operations which affect the resolve are the pixel ownership test, the scissor test, and dithering.
                if (_lastRasterizerState.ScissorTestEnable)
                {
                    GL.Disable(EnableCap.ScissorTest);
                    GraphicsExtensions.CheckGLError();
                }
                var glFramebuffer = _glFramebuffers[_currentRenderTargetBindings];
                _framebufferHelper.BindReadFramebuffer(glFramebuffer);
                for (var i = 0; i < RenderTargetCount; ++i)
                {
                    renderTargetBinding = _currentRenderTargetBindings[i];
                    renderTarget = renderTargetBinding.RenderTarget as IRenderTarget;
                    _framebufferHelper.BlitFramebuffer(i, renderTarget.Width, renderTarget.Height);
                }
                if (renderTarget.RenderTargetUsage == RenderTargetUsage.DiscardContents && _framebufferHelper.SupportsInvalidateFramebuffer)
                    _framebufferHelper.InvalidateReadFramebuffer();
                if (_lastRasterizerState.ScissorTestEnable)
                {
                    GL.Enable(EnableCap.ScissorTest);
                    GraphicsExtensions.CheckGLError();
                }
            }
            for (var i = 0; i < RenderTargetCount; ++i)
            {
                renderTargetBinding = _currentRenderTargetBindings[i];
                renderTarget = renderTargetBinding.RenderTarget as IRenderTarget;
                if (renderTarget.LevelCount > 1)
                {
                    GL.BindTexture(renderTarget.GLTarget, renderTarget.GLTexture);
                    GraphicsExtensions.CheckGLError();
                    _framebufferHelper.GenerateMipmap((int)renderTarget.GLTarget);
                }
            }
        }

        private IRenderTarget PlatformApplyRenderTargets()
        {
            if (!_glFramebuffers.TryGetValue(_currentRenderTargetBindings, out int glFramebuffer))
            {
                _framebufferHelper.GenFramebuffer(out glFramebuffer);
                _framebufferHelper.BindFramebuffer(glFramebuffer);

                var renderTargetBinding = _currentRenderTargetBindings[0];
                var renderTarget = renderTargetBinding.RenderTarget as IRenderTarget;
                _framebufferHelper.FramebufferRenderbuffer((int)FramebufferAttachment.DepthAttachment, renderTarget.GLDepthBuffer, 0);
                _framebufferHelper.FramebufferRenderbuffer((int)FramebufferAttachment.StencilAttachment, renderTarget.GLStencilBuffer, 0);

                for (int i = 0; i < RenderTargetCount; ++i)
                {
                    renderTargetBinding = _currentRenderTargetBindings[i];
                    renderTarget = renderTargetBinding.RenderTarget as IRenderTarget;
                    var attachement = (int)(FramebufferAttachment.ColorAttachment0 + i);
                    if (renderTarget.GLColorBuffer != renderTarget.GLTexture)
                        _framebufferHelper.FramebufferRenderbuffer(attachement, renderTarget.GLColorBuffer, 0);
                    else
                        _framebufferHelper.FramebufferTexture2D(attachement, (int)renderTarget.GetFramebufferTarget(renderTargetBinding), renderTarget.GLTexture, 0, renderTarget.MultiSampleCount);
                }

#if DEBUG
                _framebufferHelper.CheckFramebufferStatus();
#endif
                _glFramebuffers.Add((RenderTargetBinding[])_currentRenderTargetBindings.Clone(), glFramebuffer);
            }
            else
            {
                _framebufferHelper.BindFramebuffer(glFramebuffer);
            }
#if !GLES
            GL.DrawBuffers(RenderTargetCount, _drawBuffers);
#endif

            // Reset the raster state because we flip vertices
            // when rendering offscreen and hence the cull direction.
            _rasterizerStateDirty = true;

            // Textures will need to be rebound to render correctly in the new render target.
            Textures.Dirty();

            return _currentRenderTargetBindings[0].RenderTarget as IRenderTarget;
        }

        private static GLPrimitiveType PrimitiveTypeGL(PrimitiveType primitiveType)
        {
            switch (primitiveType)
            {
                case PrimitiveType.LineList: return GLPrimitiveType.Lines;
                case PrimitiveType.LineStrip: return GLPrimitiveType.LineStrip;
                case PrimitiveType.TriangleList: return GLPrimitiveType.Triangles;
                case PrimitiveType.TriangleStrip: return GLPrimitiveType.TriangleStrip;
                default: throw new ArgumentOutOfRangeException();
            }
        }

        /// <summary>
        /// Activates the Current Vertex/Pixel shader pair into a program.         
        /// </summary>
        private unsafe void ActivateShaderProgram()
        {
            // Lookup the shader program.
            var shaderProgram = _programCache.GetProgram(VertexShader, PixelShader);
            if (shaderProgram.Program == -1)
                return;
            // Set the new program if it has changed.
            if (_shaderProgram != shaderProgram)
            {
                GL.UseProgram(shaderProgram.Program);
                GraphicsExtensions.CheckGLError();
                _shaderProgram = shaderProgram;
            }

            var posFixupLoc = shaderProgram.GetUniformLocation("posFixup");
            if (posFixupLoc == -1)
                return;

            // Apply vertex shader fix:
            // The following two lines are appended to the end of vertex shaders
            // to account for rendering differences between OpenGL and DirectX:
            //
            // gl_Position.y = gl_Position.y * posFixup.y;
            // gl_Position.xy += posFixup.zw * gl_Position.ww;
            //
            // (the following paraphrased from wine, wined3d/state.c and wined3d/glsl_shader.c)
            //
            // - We need to flip along the y-axis in case of offscreen rendering.
            // - D3D coordinates refer to pixel centers while GL coordinates refer
            //   to pixel corners.
            // - D3D has a top-left filling convention. We need to maintain this
            //   even after the y-flip mentioned above.
            // In order to handle the last two points, we translate by
            // (63.0 / 128.0) / VPw and (63.0 / 128.0) / VPh. This is equivalent to
            // translating slightly less than half a pixel. We want the difference to
            // be large enough that it doesn't get lost due to rounding inside the
            // driver, but small enough to prevent it from interfering with any
            // anti-aliasing.
            //
            // OpenGL coordinates specify the center of the pixel while d3d coords specify
            // the corner. The offsets are stored in z and w in posFixup. posFixup.y contains
            // 1.0 or -1.0 to turn the rendering upside down for offscreen rendering. PosFixup.x
            // contains 1.0 to allow a mad.

            _posFixup[0] = 1.0f;
            _posFixup[1] = 1.0f;
            if (!GraphicsDeviceManager.UseStandardPixelAddressing)
            {
                _posFixup[2] = (63.0f / 64.0f) / Viewport.Width;
                _posFixup[3] = -(63.0f / 64.0f) / Viewport.Height;
            }
            else
            {
                _posFixup[2] = 0f;
                _posFixup[3] = 0f;
            }

            //If we have a render target bound (rendering offscreen)
            if (IsRenderTargetBound)
            {
                //flip vertically
                _posFixup[1] *= -1.0f;
                _posFixup[3] *= -1.0f;
            }

            fixed (float* floatPtr = _posFixup)
            {
                GL.Uniform4(posFixupLoc, 1, floatPtr);
            }
            GraphicsExtensions.CheckGLError();
        }

        internal void PlatformBeginApplyState()
        {
            Threading.EnsureUIThread();
        }

        private void PlatformApplyBlend(bool force = false)
        {
            _actualBlendState.PlatformApplyState(this, force);
            ApplyBlendFactor(force);
        }

        private void ApplyBlendFactor(bool force)
        {
            if (force || BlendFactor != _lastBlendState.BlendFactor)
            {
                GL.BlendColor(
                    BlendFactor.R / 255.0f,
                    BlendFactor.G / 255.0f,
                    BlendFactor.B / 255.0f,
                    BlendFactor.A / 255.0f);
                GraphicsExtensions.CheckGLError();
                _lastBlendState.BlendFactor = BlendFactor;
            }
        }

        internal void PlatformApplyState(bool applyShaders)
        {
            if (_scissorRectangleDirty)
            {
                var scissorRect = _scissorRectangle;
                if (!IsRenderTargetBound)
                    scissorRect.Y = PresentationParameters.BackBufferHeight - (scissorRect.Y + scissorRect.Height);

                GL.Scissor(scissorRect.X, scissorRect.Y, scissorRect.Width, scissorRect.Height);
                GraphicsExtensions.CheckGLError();
                _scissorRectangleDirty = false;
            }

            // If we're not applying shaders then early out now.
            if (!applyShaders)
                return;

            if (_indexBufferDirty && _indexBuffer != null)
            {
                GL.BindBuffer(BufferTarget.ElementArrayBuffer, _indexBuffer._vbo);
                GraphicsExtensions.CheckGLError();
            }
            _indexBufferDirty = false;

            if (_vertexShader == null)
                throw new InvalidOperationException("A vertex shader must be set.");
            if (_pixelShader == null)
                throw new InvalidOperationException("A pixel shader must be set.");

            if (VertexShaderDirty || PixelShaderDirty)
            {
                ActivateShaderProgram();

                unchecked
                {
                    if (VertexShaderDirty)
                        _graphicsMetrics._vertexShaderCount++;

                    if (PixelShaderDirty)
                        _graphicsMetrics._pixelShaderCount++;
                }

                VertexShaderDirty = false;
                PixelShaderDirty = false;
            }

            _vertexConstantBuffers.SetConstantBuffers(this, _shaderProgram);
            _pixelConstantBuffers.SetConstantBuffers(this, _shaderProgram);

            Textures.SetTextures(this);
            SamplerStates.PlatformSetSamplers(this);
        }

        private void PlatformDrawIndexedPrimitives(
            PrimitiveType primitiveType, int baseVertex, int startIndex, int primitiveCount)
        {
            ApplyState(true);
            ApplyAttribs(_vertexShader, baseVertex);

            bool shortIndices = _indexBuffer.IndexElementSize == IndexElementSize.SixteenBits;
            int indexElementCount = GetElementCountForType(primitiveType, primitiveCount);
            var indexElementType = shortIndices ? IndexElementType.UnsignedShort : IndexElementType.UnsignedInt;
            var indexOffsetInBytes = new IntPtr(startIndex * (shortIndices ? 2 : 4));

            GL.DrawElements(PrimitiveTypeGL(primitiveType), indexElementCount, indexElementType, indexOffsetInBytes);
            GraphicsExtensions.CheckGLError();
        }

        private void PlatformDrawPrimitives(PrimitiveType primitiveType, int vertexStart, int vertexCount)
        {
            ApplyState(true);
            ApplyAttribs(_vertexShader, 0);

            if (vertexStart < 0)
                vertexStart = 0;

            GL.DrawArrays(PrimitiveTypeGL(primitiveType), vertexStart, vertexCount);
            GraphicsExtensions.CheckGLError();
        }

        private unsafe void PlatformDrawUserPrimitives<T>(
            PrimitiveType type, ReadOnlySpan<T> vertices, VertexDeclaration declaration)
            where T : unmanaged
        {
            ApplyState(true);

            // Unbind current VBOs.
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GraphicsExtensions.CheckGLError();

            GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);
            GraphicsExtensions.CheckGLError();
            _indexBufferDirty = true;

            fixed (T* vertexPtr = &MemoryMarshal.GetReference(vertices))
            {
                // Setup the vertex declaration to point at the VB data.
                declaration.GraphicsDevice = this;
                declaration.Apply(_vertexShader, (IntPtr)vertexPtr, ShaderProgramHash);

                //Draw
                GL.DrawArrays(PrimitiveTypeGL(type), 0, vertices.Length);
                GraphicsExtensions.CheckGLError();
            }
        }

        private unsafe void PlatformDrawUserIndexedPrimitives<TVertex, TIndex>(
            PrimitiveType type, ReadOnlySpan<TVertex> vertices,
            IndexElementSize indexElementSize, ReadOnlySpan<TIndex> indices, int primitiveCount, VertexDeclaration declaration)
            where TVertex : unmanaged
            where TIndex : unmanaged
        {
            int indexSize = sizeof(TIndex);
            var indexType = indexElementSize == IndexElementSize.SixteenBits ?
                IndexElementType.UnsignedShort : IndexElementType.UnsignedInt;

            ApplyState(true);

            // Unbind current VBOs.
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GraphicsExtensions.CheckGLError();

            GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);
            GraphicsExtensions.CheckGLError();
            _indexBufferDirty = true;

            fixed (TVertex* vertexPtr = &MemoryMarshal.GetReference(vertices))
            {
                // Setup the vertex declaration to point at the data.
                declaration.GraphicsDevice = this;
                declaration.Apply(_vertexShader, (IntPtr)vertexPtr, ShaderProgramHash);

                fixed (TIndex* indexPtr = &MemoryMarshal.GetReference(indices))
                {
                    var glPrimitive = PrimitiveTypeGL(type);
                    int count = GetElementCountForType(type, primitiveCount);

                    GL.DrawElements(glPrimitive, count, indexType, (IntPtr)indexPtr);
                    GraphicsExtensions.CheckGLError();
                }
            }
        }

        private void PlatformDrawInstancedPrimitives(
            PrimitiveType primitiveType, int baseVertex, int startIndex, int primitiveCount, int instanceCount)
        {
            if (!GraphicsCapabilities.SupportsInstancing)
                throw new PlatformNotSupportedException(
                    "Instanced geometry drawing requires at least OpenGL 3.2 or GLES 3.2. Try upgrading your graphics card drivers.");

            ApplyState(true);

            var shortIndices = _indexBuffer.IndexElementSize == IndexElementSize.SixteenBits;
            var indexElementType = shortIndices ? IndexElementType.UnsignedShort : IndexElementType.UnsignedInt;
            var indexOffsetInBytes = new IntPtr(startIndex * (shortIndices ? 2 : 4));
            var indexElementCount = GetElementCountForType(primitiveType, primitiveCount);
            var target = PrimitiveTypeGL(primitiveType);

            ApplyAttribs(_vertexShader, baseVertex);

            GL.DrawElementsInstanced(target, indexElementCount, indexElementType, indexOffsetInBytes, instanceCount);
            GraphicsExtensions.CheckGLError();
        }

        private unsafe void PlatformGetBackBufferData<T>(Rectangle rect, Span<T> destination)
            where T : unmanaged
        {
            fixed (T* ptr = &MemoryMarshal.GetReference(destination))
            {
                int flippedY = PresentationParameters.BackBufferHeight - rect.Bottom;
                GL.ReadPixels(
                    rect.X, flippedY, rect.Width, rect.Height,
                    PixelFormat.Rgba, PixelType.UnsignedByte, (IntPtr)ptr);
            }

            // ReadPixels returns data upside down, so we must swap rows around
            int rowBytes = rect.Width * PresentationParameters.BackBufferFormat.GetSize();
            int rowSize = rowBytes / sizeof(T);
            int count = destination.Length;

            Span<byte> buffer = stackalloc byte[Math.Min(4096, rowBytes)];
            var rowBuffer = MemoryMarshal.Cast<byte, T>(buffer);

            for (int dy = 0; dy < rect.Height / 2; dy++)
            {
                int left = Math.Min(count, rowSize);
                if (left == 0)
                    break;

                int offset = 0;
                var topRow = destination.Slice(dy * rowSize, rowSize);
                var bottomRow = destination.Slice((rect.Height - dy - 1) * rowSize, rowSize);

                while (left > 0)
                {
                    int toCopy = Math.Min(left, rowBuffer.Length);

                    var bottomRowSlice = bottomRow.Slice(offset, toCopy);
                    bottomRowSlice.CopyTo(rowBuffer);

                    var topRowSlice = topRow.Slice(offset, toCopy);
                    topRowSlice.CopyTo(bottomRowSlice);

                    rowBuffer.Slice(0, toCopy).CopyTo(topRowSlice);

                    count -= toCopy;
                    offset += toCopy;
                    left -= toCopy;
                }
            }
        }

        private static Rectangle PlatformGetTitleSafeArea(int x, int y, int width, int height)
        {
            return new Rectangle(x, y, width, height);
        }

        internal void PlatformSetMultiSamplingToMaximum(
            PresentationParameters presentationParameters, out int quality)
        {
            presentationParameters.MultiSampleCount = 4;
            quality = 0;
        }

        internal void OnPresentationChanged()
        {
#if DESKTOPGL || ANGLE
            Context.MakeCurrent(new WindowInfo(SdlGameWindow.Instance.Handle));
            Context.SwapInterval = PresentationParameters.PresentationInterval.GetSwapInterval();
#endif

            ApplyRenderTargets(null);
        }

        // Holds information for caching
        private class BufferBindingInfo
        {
            public VertexDeclaration.VertexDeclarationAttributeInfo AttributeInfo;
            public IntPtr VertexOffset;
            public int InstanceFrequency;
            public int Vbo;

            public BufferBindingInfo(
                VertexDeclaration.VertexDeclarationAttributeInfo attributeInfo,
                IntPtr vertexOffset, int instanceFrequency, int vbo)
            {
                AttributeInfo = attributeInfo;
                VertexOffset = vertexOffset;
                InstanceFrequency = instanceFrequency;
                Vbo = vbo;
            }
        }

        // FIXME: why is this even here
        //private void GetModeSwitchedSize(out int width, out int height)
        //{
        //    var mode = new Sdl.Display.Mode
        //    {
        //        Width = PresentationParameters.BackBufferWidth,
        //        Height = PresentationParameters.BackBufferHeight,
        //        Format = 0,
        //        RefreshRate = 0,
        //        DriverData = IntPtr.Zero
        //    };
        //    Sdl.Display.GetClosestDisplayMode(0, mode, out Sdl.Display.Mode closest);
        //    width = closest.Width;
        //    height = closest.Height;
        //}
        //
        //private void GetDisplayResolution(out int width, out int height)
        //{
        //    Sdl.Display.GetCurrentDisplayMode(0, out Sdl.Display.Mode mode);
        //    width = mode.Width;
        //    height = mode.Height;
        //}
    }
}