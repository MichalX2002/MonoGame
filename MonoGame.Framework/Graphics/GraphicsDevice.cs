// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using MonoGame.Utilities;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace MonoGame.Framework.Graphics
{
    public partial class GraphicsDevice : IDisposable
    {
        #region Fields

        private Viewport _viewport;
        private Color _blendFactor = Color.White;

        private int _maxVertexBufferSlots;
        internal int MaxTextureSlots;
        internal int MaxVertexTextureSlots;

        private BlendState _blendState;
        private BlendState _actualBlendState;
        private bool _blendStateDirty;

        private BlendState _blendStateAdditive;
        private BlendState _blendStateAlphaBlend;
        private BlendState _blendStateNonPremultiplied;
        private BlendState _blendStateOpaque;

        private DepthStencilState _depthStencilState;
        private DepthStencilState _actualDepthStencilState;
        private bool _depthStencilStateDirty;

        private DepthStencilState _depthStencilStateDefault;
        private DepthStencilState _depthStencilStateDepthRead;
        private DepthStencilState _depthStencilStateNone;

        private RasterizerState _rasterizerState;
        private RasterizerState _actualRasterizerState;
        private bool _rasterizerStateDirty;

        private RasterizerState _rasterizerStateCullClockwise;
        private RasterizerState _rasterizerStateCullCounterClockwise;
        private RasterizerState _rasterizerStateCullNone;

        private Rectangle _scissorRectangle;
        private bool _scissorRectangleDirty;

        internal VertexBufferBindings _vertexBuffers;
        private bool _vertexBuffersDirty;

        private IndexBuffer _indexBuffer;
        private bool _indexBufferDirty;

        private readonly RenderTargetBinding[] _currentRenderTargetBindings = new RenderTargetBinding[4];
        private readonly RenderTargetBinding[] _tempRenderTargetBinding = new RenderTargetBinding[1];

        // On Intel Integrated graphics, there is a fast hw unit for doing
        // clears to colors where all components are either 0 or 255.
        // Despite XNA4 using Purple here, we use black (in Release) to avoid
        // performance warnings on Intel/Mesa
#if DEBUG
        private static readonly Color DiscardColor = new Color(68, 34, 136, 255);
#else
        private static readonly Color DiscardColor = new Color(0, 0, 0, 255);
#endif

        private Shader _vertexShader;
        private Shader _pixelShader;

        private readonly ConstantBufferCollection _vertexConstantBuffers = new ConstantBufferCollection(ShaderStage.Vertex, 16);
        private readonly ConstantBufferCollection _pixelConstantBuffers = new ConstantBufferCollection(ShaderStage.Pixel, 16);

        /// <summary>
        /// The cache of effects from unique byte streams.
        /// </summary>
        internal Dictionary<int, Effect> EffectCache;

        // Resources may be added to and removed from the list from many threads.
        private readonly object _resourcesLock = new object();

        // Use WeakReference for the global resources list as we do not know when a resource
        // may be disposed and collected. We do not want to prevent a resource from being
        // collected by holding a strong reference to it in this list.
        private readonly List<WeakReference> _resources = new List<WeakReference>();

        internal GraphicsMetrics _graphicsMetrics;

        #endregion 

        #region Auto Properties

        public int MaxTexture2DSize { get; private set; }
        public int MaxTexture3DSize { get; private set; }
        public int MaxTextureCubeSize { get; private set; }

        internal GraphicsCapabilities GraphicsCapabilities { get; private set; }
        public TextureCollection VertexTextures { get; private set; }
        public SamplerStateCollection VertexSamplerStates { get; private set; }
        public TextureCollection Textures { get; private set; }
        public SamplerStateCollection SamplerStates { get; private set; }

        public bool IsDisposed { get; private set; }

        private bool VertexShaderDirty { get; set; }
        private bool PixelShaderDirty { get; set; }

        public GraphicsAdapter Adapter { get; private set; }

        /// <summary>
        /// Access debugging APIs for the graphics subsystem.
        /// </summary>
        public GraphicsDebug GraphicsDebug { get; set; }

        public int RenderTargetCount { get; private set; }
        public bool ResourcesLost { get; set; }
        public bool BlendFactorDirty { get; private set; }

        public PresentationParameters PresentationParameters { get; private set; }
        public GraphicsProfile GraphicsProfile { get; }

        /// <summary>
        /// Indicates if DX9 style pixel addressing or current standard
        /// pixel addressing should be used. This flag is set to
        /// <c>false</c> by default. If `UseHalfPixelOffset` is
        /// `true` you have to add half-pixel offset to a Projection matrix.
        /// See also <see cref="GraphicsDeviceManager.PreferHalfPixelOffset"/>.
        /// </summary>
        /// <remarks>
        /// XNA uses DirectX9 for its graphics. DirectX9 interprets UV
        /// coordinates differently from other graphics API's. This is
        /// typically referred to as the half-pixel offset. MonoGame
        /// replicates XNA behavior if this flag is set to <c>true</c>.
        /// </remarks>
        public bool UseHalfPixelOffset { get; private set; }

        #endregion

        #region Simple Properties

        // We will just return IsDisposed for now
        // as that is the only case I can see for now
        public bool IsContentLost => IsDisposed;

        internal bool IsRenderTargetBound => RenderTargetCount > 0;

        internal DepthFormat ActiveDepthFormat
        {
            get => IsRenderTargetBound
                    ? _currentRenderTargetBindings[0].DepthFormat
                    : PresentationParameters.DepthStencilFormat;
        }

        /// <summary>
        /// The rendering information for debugging and profiling.
        /// The metrics are reset every frame after draw within <see cref="Present"/>. 
        /// </summary>
        public GraphicsMetrics Metrics { get => _graphicsMetrics; set => _graphicsMetrics = value; }

        public DisplayMode DisplayMode => Adapter.CurrentDisplayMode;
        public GraphicsDeviceStatus GraphicsDeviceStatus => GraphicsDeviceStatus.Normal;

        public IndexBuffer Indices { set => SetIndexBuffer(value); get => _indexBuffer; }

        internal Shader VertexShader
        {
            get => _vertexShader;
            set
            {
                if (_vertexShader != value)
                {
                    _vertexShader = value;
                    _vertexConstantBuffers.Clear();
                    VertexShaderDirty = true;
                }
            }
        }

        internal Shader PixelShader
        {
            get => _pixelShader;
            set
            {
                if (_pixelShader != value)
                {
                    _pixelShader = value;
                    _pixelConstantBuffers.Clear();
                    PixelShaderDirty = true;
                }
            }
        }

        public Viewport Viewport
        {
            get => _viewport;
            set
            {
                _viewport = value;
                PlatformSetViewport(value);
            }
        }

        public Rectangle ScissorRectangle
        {
            get => _scissorRectangle;
            set
            {
                if (_scissorRectangle != value)
                {
                    _scissorRectangle = value;
                    _scissorRectangleDirty = true;
                }
            }
        }

        /// <summary>
        /// The color used as blend factor when alpha blending.
        /// </summary>
        /// <remarks>
        /// When only changing BlendFactor, use this rather than <see cref="Graphics.BlendState.BlendFactor"/> to
        /// only update BlendFactor so the whole BlendState does not have to be updated.
        /// </remarks>
        public Color BlendFactor
        {
            get => _blendFactor;
            set
            {
                if (_blendFactor == value)
                    return;
                _blendFactor = value;
                BlendFactorDirty = true;
            }
        }

        #endregion

        #region Complex Properties

        public BlendState BlendState
        {
            get => _blendState;
            set
            {
                if (value == null)
                    throw new ArgumentNullException(nameof(value));

                // Don't set the same state twice!
                if (_blendState == value)
                    return;

                _blendState = value;

                // Static state properties never actually get bound;
                // instead we use our GraphicsDevice-specific version of them.
                var newBlendState = _blendState;
                if (ReferenceEquals(_blendState, BlendState.Additive))
                    newBlendState = _blendStateAdditive;
                else if (ReferenceEquals(_blendState, BlendState.AlphaBlend))
                    newBlendState = _blendStateAlphaBlend;
                else if (ReferenceEquals(_blendState, BlendState.NonPremultiplied))
                    newBlendState = _blendStateNonPremultiplied;
                else if (ReferenceEquals(_blendState, BlendState.Opaque))
                    newBlendState = _blendStateOpaque;

                // Blend state is now bound to a device... no one should
                // be changing the state of the blend state object now!
                newBlendState.BindToGraphicsDevice(this);

                _actualBlendState = newBlendState;

                BlendFactor = _actualBlendState.BlendFactor;

                _blendStateDirty = true;
            }
        }

        public DepthStencilState DepthStencilState
        {
            get => _depthStencilState;
            set
            {
                if (value == null)
                    throw new ArgumentNullException(nameof(value));

                // Don't set the same state twice!
                if (_depthStencilState == value)
                    return;

                _depthStencilState = value;

                // Static state properties never actually get bound;
                // instead we use our GraphicsDevice-specific version of them.
                var newDepthStencilState = _depthStencilState;
                if (ReferenceEquals(_depthStencilState, DepthStencilState.Default))
                    newDepthStencilState = _depthStencilStateDefault;
                else if (ReferenceEquals(_depthStencilState, DepthStencilState.DepthRead))
                    newDepthStencilState = _depthStencilStateDepthRead;
                else if (ReferenceEquals(_depthStencilState, DepthStencilState.None))
                    newDepthStencilState = _depthStencilStateNone;

                newDepthStencilState.BindToGraphicsDevice(this);

                _actualDepthStencilState = newDepthStencilState;

                _depthStencilStateDirty = true;
            }
        }

        public RasterizerState RasterizerState
        {
            get => _rasterizerState;
            set
            {
                if (value == null)
                    throw new ArgumentNullException(nameof(value));

                // Don't set the same state twice!
                if (_rasterizerState == value)
                    return;

                if (!value.DepthClipEnable && !GraphicsCapabilities.SupportsDepthClamp)
                    throw new InvalidOperationException("Cannot set RasterizerState.DepthClipEnable to false on this graphics device");

                _rasterizerState = value;

                // Static state properties never actually get bound;
                // instead we use our GraphicsDevice-specific version of them.
                var newRasterizerState = _rasterizerState;
                if (ReferenceEquals(_rasterizerState, RasterizerState.CullClockwise))
                    newRasterizerState = _rasterizerStateCullClockwise;
                else if (ReferenceEquals(_rasterizerState, RasterizerState.CullCounterClockwise))
                    newRasterizerState = _rasterizerStateCullCounterClockwise;
                else if (ReferenceEquals(_rasterizerState, RasterizerState.CullNone))
                    newRasterizerState = _rasterizerStateCullNone;

                newRasterizerState.BindToGraphicsDevice(this);

                _actualRasterizerState = newRasterizerState;

                _rasterizerStateDirty = true;
            }
        }

        #endregion

        #region Events

        // TODO Graphics Device events need implementing
        internal event DataEventHandler<GraphicsDevice, PresentationParameters> PresentationChanged;
        public event SimpleEventHandler<GraphicsDevice> DeviceLost;
        public event SimpleEventHandler<GraphicsDevice> DeviceReset;
        public event SimpleEventHandler<GraphicsDevice> DeviceResetting;
        public event DataEventHandler<GraphicsDevice, object> ResourceCreated;
        public event DataEventHandler<GraphicsDevice, ResourceDestroyedEvent> ResourceDestroyed;
        public event SimpleEventHandler<GraphicsDevice> Disposing;

        #endregion

        #region Constructors

        internal GraphicsDevice()
		{
            PresentationParameters = new PresentationParameters
            {
                DepthStencilFormat = DepthFormat.Depth24
            };
            Setup();
            GraphicsCapabilities = new GraphicsCapabilities();
            GraphicsCapabilities.Initialize(this);
            Initialize();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GraphicsDevice" /> class.
        /// </summary>
        /// <param name="adapter">The graphics adapter.</param>
        /// <param name="graphicsProfile">The graphics profile.</param>
        /// <param name="presentationParameters">The presentation options.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="presentationParameters"/> is <see langword="null"/>.
        /// </exception>
        public GraphicsDevice(GraphicsAdapter adapter, GraphicsProfile graphicsProfile, PresentationParameters presentationParameters)
        {
            Adapter = adapter ?? throw new ArgumentNullException(nameof(adapter));
            if (!adapter.IsProfileSupported(graphicsProfile))
                throw new NoSuitableGraphicsDeviceException(
                    $"Adapter '{ adapter.Description}' does not support the {graphicsProfile} profile.");
            
            PresentationParameters = presentationParameters ??
                throw new ArgumentNullException(nameof(presentationParameters));

            GraphicsProfile = graphicsProfile;
            Setup();
            GraphicsCapabilities = new GraphicsCapabilities();
            GraphicsCapabilities.Initialize(this);

            Initialize();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GraphicsDevice" /> class.
        /// </summary>
        /// <param name="adapter">The graphics adapter.</param>
        /// <param name="graphicsProfile">The graphics profile.</param>
        /// <param name="preferHalfPixelOffset"> Indicates if DX9 style pixel addressing or current standard pixel addressing should be used. This value is passed to <see cref="GraphicsDevice.UseHalfPixelOffset"/></param>
        /// <param name="presentationParameters">The presentation options.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="presentationParameters"/> is <see langword="null"/>.
        /// </exception>
        public GraphicsDevice(GraphicsAdapter adapter, GraphicsProfile graphicsProfile, bool preferHalfPixelOffset, PresentationParameters presentationParameters)
        {
            Adapter = adapter ?? throw new ArgumentNullException(nameof(adapter));
            if (!adapter.IsProfileSupported(graphicsProfile))
                throw new NoSuitableGraphicsDeviceException(
                    $"Adapter '{ adapter.Description}' does not support the {graphicsProfile} profile.");

            PresentationParameters = presentationParameters ??
                throw new ArgumentNullException(nameof(presentationParameters));

#if DIRECTX
            // TODO we need to figure out how to inject the half pixel offset into DX shaders
            preferHalfPixelOffset = false;
#endif

            Adapter = adapter;
            GraphicsProfile = graphicsProfile;
            UseHalfPixelOffset = preferHalfPixelOffset;
            Setup();
            GraphicsCapabilities = new GraphicsCapabilities();
            GraphicsCapabilities.Initialize(this);

            Initialize();
        }

        private void Setup()
        {
#if DEBUG
            if (DisplayMode == null)
            {
                throw new Exception(
                    "Unable to determine the current display mode.  This can indicate that the " +
                    "game is not configured to be HiDPI aware under Windows 10 or later.  See " +
                    "https://github.com/MonoGame/MonoGame/issues/5040 for more information.");
            }
#endif

            // Initialize the main viewport
            _viewport = new Viewport(0, 0, DisplayMode.Width, DisplayMode.Height)
            {
                MaxDepth = 1f
            };

            PlatformSetup();

            VertexTextures = new TextureCollection(this, MaxVertexTextureSlots, true);
            VertexSamplerStates = new SamplerStateCollection(this, MaxVertexTextureSlots, true);

            Textures = new TextureCollection(this, MaxTextureSlots, false);
            SamplerStates = new SamplerStateCollection(this, MaxTextureSlots, false);

            _blendStateAdditive = BlendState.Additive.Clone();
            _blendStateAlphaBlend = BlendState.AlphaBlend.Clone();
            _blendStateNonPremultiplied = BlendState.NonPremultiplied.Clone();
            _blendStateOpaque = BlendState.Opaque.Clone();

            BlendState = BlendState.Opaque;

            _depthStencilStateDefault = DepthStencilState.Default.Clone();
            _depthStencilStateDepthRead = DepthStencilState.DepthRead.Clone();
            _depthStencilStateNone = DepthStencilState.None.Clone();

            DepthStencilState = DepthStencilState.Default;

            _rasterizerStateCullClockwise = RasterizerState.CullClockwise.Clone();
            _rasterizerStateCullCounterClockwise = RasterizerState.CullCounterClockwise.Clone();
            _rasterizerStateCullNone = RasterizerState.CullNone.Clone();

            RasterizerState = RasterizerState.CullCounterClockwise;

            EffectCache = new Dictionary<int, Effect>();
        }

        internal void Initialize()
        {
            PlatformInitialize();

            // Force set the default render states.
            _blendStateDirty = _depthStencilStateDirty = _rasterizerStateDirty = true;
            BlendState = BlendState.Opaque;
            DepthStencilState = DepthStencilState.Default;
            RasterizerState = RasterizerState.CullCounterClockwise;

            // Clear the texture and sampler collections forcing
            // the state to be reapplied.
            VertexTextures.Clear();
            VertexSamplerStates.Clear();
            Textures.Clear();
            SamplerStates.Clear();

            // Clear constant buffers
            _vertexConstantBuffers.Clear();
            _pixelConstantBuffers.Clear();

            // Force set the buffers and shaders on next ApplyState() call
            _vertexBuffers = new VertexBufferBindings(_maxVertexBufferSlots);
            _vertexBuffersDirty = true;
            _indexBufferDirty = true;
            VertexShaderDirty = true;
            PixelShaderDirty = true;

            // Set the default scissor rect.
            _scissorRectangleDirty = true;
            ScissorRectangle = _viewport.Bounds;

            // Set the default render target.
            ApplyRenderTargets(null);
        }

        #endregion

        #region Clear

        public void Clear(Color color)
        {
            var options = ClearOptions.Target;
            options |= ClearOptions.DepthBuffer;
            options |= ClearOptions.Stencil;
            PlatformClear(options, color.ToVector4(), _viewport.MaxDepth, 0);

            unchecked
            {
                _graphicsMetrics._clearCount++;
            }
        }

        public void Clear(ClearOptions options, Color color, float depth, int stencil)
        {
            PlatformClear(options, color.ToVector4(), depth, stencil);

            unchecked
            {
                _graphicsMetrics._clearCount++;
            }
        }

		public void Clear(ClearOptions options, Vector4 color, float depth, int stencil)
		{
            PlatformClear(options, color, depth, stencil);

            unchecked
            {
                _graphicsMetrics._clearCount++;
            }
        }

        #endregion

        #region Uncategorized

        internal void ApplyState(bool applyShaders)
        {
            PlatformBeginApplyState();
            PlatformApplyBlend();

            if (_depthStencilStateDirty)
            {
                _actualDepthStencilState.PlatformApplyState(this);
                _depthStencilStateDirty = false;
            }

            if (_rasterizerStateDirty)
            {
                _actualRasterizerState.PlatformApplyState(this);
                _rasterizerStateDirty = false;
            }

            PlatformApplyState(applyShaders);
        }

        internal void AddResourceReference(WeakReference resourceReference)
        {
            lock (_resourcesLock)
            {
                _resources.Add(resourceReference);
            }
        }

        internal void RemoveResourceReference(WeakReference resourceReference)
        {
            lock (_resourcesLock)
            {
                _resources.Remove(resourceReference);
            }
        }

        public void Present()
        {
            // We cannot present with a RT set on the device.
            if (RenderTargetCount != 0)
                throw new InvalidOperationException("Cannot call Present while a render target is active.");

            _graphicsMetrics = new GraphicsMetrics();
            PlatformPresent();
        }

        /// <summary>
        /// Sends queued commands in the command buffer to the GPU.
        /// </summary>
        public void Flush()
        {
            PlatformFlush();
        }

        #endregion

        #region Reset

        public void Reset()
        {
            PlatformReset();

            DeviceResetting?.Invoke(this);

            // Update the back buffer.
            OnPresentationChanged();
            
            PresentationChanged?.Invoke(this, PresentationParameters);
            DeviceReset?.Invoke(this);
       }

        public void Reset(PresentationParameters presentationParameters)
        {
            PresentationParameters = presentationParameters ??
                throw new ArgumentNullException(nameof(presentationParameters));
            Reset();
        }

        /// <summary>
        /// Trigger the DeviceResetting event
        /// Currently internal to allow the various platforms to send the event at the appropriate time.
        /// </summary>
        internal void OnDeviceResetting()
        {
            DeviceResetting?.Invoke(this);

            lock (_resourcesLock)
            {
                foreach (var resource in _resources)
                {
                    if (resource.Target is GraphicsResource target)
                        target.GraphicsDeviceResetting();
                }

                // Remove references to resources that have been garbage collected.
                _resources.RemoveAll(wr => !wr.IsAlive);
            }
        }

        /// <summary>
        /// Trigger the DeviceReset event to allow games to be notified of a device reset.
        /// Currently internal to allow the various platforms to send the event at the appropriate time.
        /// </summary>
        internal void OnDeviceReset()
        {
            DeviceReset?.Invoke(this);
        }

#endregion

        #region RenderTargets

        public void SetRenderTarget(RenderTarget2D renderTarget)
		{
			if (renderTarget == null)
		    {
                SetRenderTargets(null);
		    }
			else
			{
				_tempRenderTargetBinding[0] = new RenderTargetBinding(renderTarget);
				SetRenderTargets(_tempRenderTargetBinding);
			}
		}

        public void SetRenderTarget(RenderTargetCube renderTarget, CubeMapFace cubeMapFace)
        {
            if (renderTarget == null)
            {
                SetRenderTargets(null);
            }
            else
            {
                _tempRenderTargetBinding[0] = new RenderTargetBinding(renderTarget, cubeMapFace);
                SetRenderTargets(_tempRenderTargetBinding);
            }
        }

		public void SetRenderTargets(params RenderTargetBinding[] renderTargets)
		{
            // Avoid having to check for null and zero length.
            var renderTargetCount = 0;
            if (renderTargets != null)
            {
                renderTargetCount = renderTargets.Length;
                if (renderTargetCount == 0)
                {
                    renderTargets = null;
                }
            }

            // Try to early out if the current and new bindings are equal.
            if (RenderTargetCount == renderTargetCount)
            {
                var isEqual = true;
                for (var i = 0; i < RenderTargetCount; i++)
                {
                    if (_currentRenderTargetBindings[i].RenderTarget != renderTargets[i].RenderTarget ||
                        _currentRenderTargetBindings[i].ArraySlice != renderTargets[i].ArraySlice)
                    {
                        isEqual = false;
                        break;
                    }
                }

                if (isEqual)
                    return;
            }

            ApplyRenderTargets(renderTargets);

            if (renderTargetCount == 0)
            {
                unchecked
                {
                    _graphicsMetrics._targetCount++;
                }
            }
            else
            {
                unchecked
                {
                    _graphicsMetrics._targetCount += renderTargetCount;
                }
            }
        }

        internal void ApplyRenderTargets(RenderTargetBinding[] renderTargets)
        {
            PlatformResolveRenderTargets();

            // Clear the current bindings.
            Array.Clear(_currentRenderTargetBindings, 0, _currentRenderTargetBindings.Length);

            bool clearTarget;
            int renderTargetWidth;
            int renderTargetHeight;
            if (renderTargets == null)
            {
                RenderTargetCount = 0;

                PlatformApplyDefaultRenderTarget();
                clearTarget = PresentationParameters.RenderTargetUsage == RenderTargetUsage.DiscardContents;

                renderTargetWidth = PresentationParameters.BackBufferWidth;
                renderTargetHeight = PresentationParameters.BackBufferHeight;
            }
            else
            {
                // Copy the new bindings.
                Array.Copy(renderTargets, _currentRenderTargetBindings, renderTargets.Length);
                RenderTargetCount = renderTargets.Length;

                var renderTarget = PlatformApplyRenderTargets();

                // We clear the render target if asked.
                clearTarget = renderTarget.RenderTargetUsage == RenderTargetUsage.DiscardContents;

                renderTargetWidth = renderTarget.Width;
                renderTargetHeight = renderTarget.Height;
            }

            // Set the viewport to the size of the first render target.
            Viewport = new Viewport(0, 0, renderTargetWidth, renderTargetHeight);

            // Set the scissor rectangle to the size of the first render target.
            ScissorRectangle = new Rectangle(0, 0, renderTargetWidth, renderTargetHeight);

            // In XNA 4, because of hardware limitations on Xbox, when
            // a render target doesn't have PreserveContents as its usage
            // it is cleared before being rendered to.
            if (clearTarget)
                Clear(DiscardColor);
        }

		public RenderTargetBinding[] GetRenderTargets()
		{
            // Return a correctly sized copy our internal array.
            var bindings = new RenderTargetBinding[RenderTargetCount];
            Array.Copy(_currentRenderTargetBindings, bindings, RenderTargetCount);
            return bindings;
		}

        public void GetRenderTargets(RenderTargetBinding[] outTargets)
        {
            Debug.Assert(outTargets.Length == RenderTargetCount, "Invalid outTargets array length!");
            Array.Copy(_currentRenderTargetBindings, outTargets, RenderTargetCount);
        }

        #endregion

        #region Buffers

        public void SetVertexBuffer(VertexBuffer vertexBuffer)
        {
            _vertexBuffersDirty |= vertexBuffer == null ?
                _vertexBuffers.Clear() : _vertexBuffers.Set(vertexBuffer, 0);
        }

        public void SetVertexBuffer(VertexBuffer vertexBuffer, int vertexOffset)
        {
            // Validate vertexOffset.
            if (vertexOffset < 0 ||
                vertexBuffer == null && vertexOffset != 0 ||
                vertexBuffer != null && vertexOffset >= vertexBuffer.Capacity)
            {
                throw new ArgumentOutOfRangeException(nameof(vertexOffset));
            }

            _vertexBuffersDirty |= vertexBuffer == null ?
                _vertexBuffers.Clear() : _vertexBuffers.Set(vertexBuffer, vertexOffset);
        }

        public void SetVertexBuffers(params VertexBufferBinding[] vertexBuffers)
        {
            if (vertexBuffers == null || vertexBuffers.Length == 0)
            {
                _vertexBuffersDirty |= _vertexBuffers.Clear();
            }
            else
            {
                if (vertexBuffers.Length > _maxVertexBufferSlots)
                    throw new ArgumentOutOfRangeException(
                        nameof(vertexBuffers), $"Max number of vertex buffers is {_maxVertexBufferSlots}.");

                _vertexBuffersDirty |= _vertexBuffers.Set(vertexBuffers);
            }
        }

        private void SetIndexBuffer(IndexBuffer indexBuffer)
        {
            if (_indexBuffer == indexBuffer)
                return;
            _indexBuffer = indexBuffer;
            _indexBufferDirty = true;
        }

        internal void SetConstantBuffer(ShaderStage stage, int slot, ConstantBuffer buffer)
        {
            if (stage == ShaderStage.Vertex)
                _vertexConstantBuffers[slot] = buffer;
            else
                _pixelConstantBuffers[slot] = buffer;
        }

        #endregion

        #region Drawing

        /// <summary>
        /// Draw geometry by indexing into the vertex buffer.
        /// </summary>
        /// <param name="primitiveType">The type of primitives in the index buffer.</param>
        /// <param name="baseVertex">Used to offset the vertex range indexed from the vertex buffer.</param>
        /// <param name="startIndex">The index within the index buffer to start drawing from.</param>
        /// <param name="primitiveCount">The number of primitives to render from the index buffer.</param>
        public void DrawIndexedPrimitives(
            PrimitiveType primitiveType, int baseVertex, int startIndex, int primitiveCount)
        {
            if (_vertexShader == null)
                throw new InvalidOperationException("Vertex shader must be set before calling this.");

            if (_vertexBuffers.Count == 0)
                throw new InvalidOperationException("Vertex buffer must be set before calling this.");

            if (_indexBuffer == null)
                throw new InvalidOperationException("Index buffer must be set before calling this.");

            if (primitiveCount <= 0)
                throw new ArgumentOutOfRangeException(nameof(primitiveCount));

            PlatformDrawIndexedPrimitives(primitiveType, baseVertex, startIndex, primitiveCount);

            unchecked
            {
                _graphicsMetrics._drawCount++;
                _graphicsMetrics._primitiveCount += primitiveCount;
            }
        }

        #region DrawUserPrimitives

        /// <summary>
        /// Draw primitives of the specified type from the data in a span of vertices without indexing.
        /// </summary>
        /// <typeparam name="T">The type of the vertex data.</typeparam>
        /// <param name="primitiveType">The type of primitives to draw with the vertices.</param>
        /// <param name="vertexData">The span of vertices to draw.</param>
        /// <remarks>
        /// The <see cref="VertexDeclaration"/> will be found by getting <see cref="IVertexType.VertexDeclaration"/>
        /// from an instance of <typeparamref name="T"/> and cached for subsequent calls.
        /// </remarks>
        public void DrawUserPrimitives<T>(
            PrimitiveType primitiveType, ReadOnlySpan<T> vertexData)
            where T : unmanaged, IVertexType
        {
            var declaration = VertexDeclarationCache<T>.VertexDeclaration;
            DrawUserPrimitives(primitiveType, vertexData, declaration);
        }

        /// <summary>
        /// Draw primitives of the specified type from the data in the given span of vertices without indexing.
        /// </summary>
        /// <typeparam name="T">The type of the vertex data.</typeparam>
        /// <param name="primitiveType">The type of primitives to draw with the vertices.</param>
        /// <param name="vertexData">The span of vertices to draw.</param>
        /// <param name="vertexDeclaration">The layout of the vertices.</param>
        public void DrawUserPrimitives<T>(
            PrimitiveType primitiveType, ReadOnlySpan<T> vertexData, VertexDeclaration vertexDeclaration)
            where T : unmanaged
        {
            if (vertexData.IsEmpty)
                throw new ArgumentEmptyException(nameof(vertexData));

            int vertexCount = GetElementCountForType(primitiveType, vertexData.Length);
            if (vertexCount > vertexData.Length)
                throw new ArgumentOutOfRangeException(nameof(vertexData));

            if (vertexDeclaration == null)
                throw new ArgumentNullException(nameof(vertexDeclaration));

            PlatformDrawUserPrimitives(primitiveType, vertexData, vertexDeclaration);

            unchecked
            {
                _graphicsMetrics._drawCount++;
                _graphicsMetrics._primitiveCount += vertexCount;
            }
        }

        #endregion

        /// <summary>
        /// Draw primitives of the specified type from the currently bound vertexbuffers without indexing.
        /// </summary>
        /// <param name="primitiveType">The type of primitives to draw.</param>
        /// <param name="vertexStart">Index of the vertex to start at.</param>
        /// <param name="primitiveCount">The number of primitives to draw.</param>
        public void DrawPrimitives(PrimitiveType primitiveType, int vertexStart, int primitiveCount)
        {
            if (_vertexShader == null)
                throw new InvalidOperationException("Vertex shader must be set before calling DrawPrimitives.");

            if (_vertexBuffers.Count == 0)
                throw new InvalidOperationException("Vertex buffer must be set before calling DrawPrimitives.");

            if (primitiveCount <= 0)
                throw new ArgumentOutOfRangeException(nameof(primitiveCount));

            int vertexCount = GetElementCountForType(primitiveType, primitiveCount);
            PlatformDrawPrimitives(primitiveType, vertexStart, vertexCount);

            unchecked
            {
                _graphicsMetrics._drawCount++;
                _graphicsMetrics._primitiveCount += primitiveCount;
            }
        }

        /// <summary>
        /// Draw instanced geometry from the bound vertex buffers and index buffer.
        /// </summary>
        /// <param name="primitiveType">The type of primitives in the index buffer.</param>
        /// <param name="baseVertex">Used to offset the vertex range indexed from the vertex buffer.</param>
        /// <param name="startIndex">The index within the index buffer to start drawing from.</param>
        /// <param name="primitiveCount">The number of primitives in a single instance.</param>
        /// <param name="instanceCount">The number of instances to render.</param>
        /// <remarks>Draw geometry with data from multiple bound vertex streams at different frequencies.</remarks>
        public void DrawInstancedPrimitives(
            PrimitiveType primitiveType, int baseVertex, int startIndex, int primitiveCount, int instanceCount)
        {
            if (_vertexShader == null)
                throw new InvalidOperationException("Vertex shader must be set before calling DrawInstancedPrimitives.");

            if (_vertexBuffers.Count == 0)
                throw new InvalidOperationException("Vertex buffer must be set before calling DrawInstancedPrimitives.");

            if (_indexBuffer == null)
                throw new InvalidOperationException("Index buffer must be set before calling DrawInstancedPrimitives.");

            if (primitiveCount <= 0)
                throw new ArgumentOutOfRangeException(nameof(primitiveCount));

            PlatformDrawInstancedPrimitives(primitiveType, baseVertex, startIndex, primitiveCount, instanceCount);

            unchecked
            {
                _graphicsMetrics._drawCount++;
                _graphicsMetrics._primitiveCount += primitiveCount * instanceCount;
            }
        }

        #region DrawUserIndexedPrimitives

        /// <summary>
        /// Draw primitives of the specified type by indexing into the given span of vertices with 
        /// 16- or 32-bit indices.
        /// </summary>
        /// <typeparam name="TVertex">The type of the vertex data.</typeparam>
        /// <typeparam name="TIndex">The type of the index data.</typeparam>
        /// <param name="primitiveType">The type of primitives to draw with the vertices.</param>
        /// <param name="vertexData">The span of vertices to draw.</param>
        /// <param name="indexData">The span of indices for indexing the vertices.</param>
        /// <param name="primitiveCount">The number of primitives to draw.</param>
        /// <remarks>
        /// The <see cref="VertexDeclaration"/> will be found by getting <see cref="IVertexType.VertexDeclaration"/>
        /// from an instance of <typeparamref name="TVertex"/> and cached for subsequent calls.
        /// </remarks>
        /// <remarks>
        /// All indices in the span are relative to the first vertex.
        /// An index of zero in the index span points to the first vertex in the vertex span.
        /// </remarks>
        public void DrawUserIndexedPrimitives<TVertex, TIndex>(
            PrimitiveType primitiveType, ReadOnlySpan<TVertex> vertexData,
            ReadOnlySpan<TIndex> indexData, int primitiveCount)
            where TVertex : unmanaged, IVertexType
            where TIndex : unmanaged
        {
            var declaration = VertexDeclarationCache<TVertex>.VertexDeclaration;
            DrawUserIndexedPrimitives(
                primitiveType, vertexData, indexData, primitiveCount, declaration);
        }

        /// <summary>
        /// Draw primitives of the specified type by indexing into the given span of vertices with 
        /// 16- or 32-bit indices.
        /// </summary>
        /// <typeparam name="TVertex">The type of the vertex data.</typeparam>
        /// <typeparam name="TIndex">The type of the index data.</typeparam>
        /// <param name="primitiveType">The type of primitives to draw with the vertices.</param>
        /// <param name="vertexData">The span of vertices to draw.</param>
        /// <param name="indexData">The span of indices for indexing the vertices.</param>
        /// <param name="primitiveCount">The number of primitives to draw.</param>
        /// <param name="vertexDeclaration">The layout of the vertices.</param>
        /// <remarks>
        /// All indices in the span are relative to the first vertex.
        /// An index of zero in the index span points to the first vertex in the vertex span.
        /// </remarks>
        public unsafe void DrawUserIndexedPrimitives<TVertex, TIndex>(
            PrimitiveType primitiveType, ReadOnlySpan<TVertex> vertexData,
            ReadOnlySpan<TIndex> indexData, int primitiveCount, VertexDeclaration vertexDeclaration)
            where TVertex : unmanaged
            where TIndex : unmanaged
        {
            if (vertexData.IsEmpty)
                throw new ArgumentEmptyException(nameof(vertexData));
            if (indexData.IsEmpty)
                throw new ArgumentEmptyException(nameof(indexData));

            if (primitiveCount <= 0 || GetElementCountForType(primitiveType, primitiveCount) > indexData.Length)
                throw new ArgumentOutOfRangeException(nameof(primitiveCount));

            if (vertexDeclaration == null)
                throw new ArgumentNullException(nameof(vertexDeclaration));

            if (vertexDeclaration.VertexStride < sizeof(TVertex))
                throw new ArgumentOutOfRangeException(nameof(vertexDeclaration),
                    $"Vertex stride of {nameof(vertexDeclaration)} should be at least as big as the stride of the actual vertices.");

            IndexElementSize indexElementSize;
            int indexSize = sizeof(TIndex);
            if (indexSize == 2)
                indexElementSize = IndexElementSize.SixteenBits;
            else if (indexSize == 4)
                indexElementSize = IndexElementSize.ThirtyTwoBits;
            else
                throw new ArgumentException(
                    "The only index element sizes supported are 2 or 4 bytes.", nameof(indexData));

            PlatformDrawUserIndexedPrimitives(
                primitiveType, vertexData, indexElementSize, indexData, primitiveCount, vertexDeclaration);
            
            unchecked
            {
                _graphicsMetrics._drawCount++;
                _graphicsMetrics._primitiveCount += primitiveCount;
            }
        }

        #endregion

        #endregion

        #region GetBackBufferData

        /// <summary>
        /// Gets the pixel data of what is currently drawn on screen.
        /// The format is the backbuffer's current format.
        /// </summary>
        public unsafe void GetBackBufferData<T>(Rectangle? rectangle, Span<T> destination)
            where T : unmanaged
        {
            Rectangle rect;
            if (rectangle.HasValue)
            {
                rect = rectangle.Value;
                if (rect.X < 0 || rect.Y < 0 || rect.Width <= 0 || rect.Height <= 0 ||
                    rect.Right > PresentationParameters.BackBufferWidth || rect.Top > PresentationParameters.BackBufferHeight)
                    throw new ArgumentException("The rectangle must fit in the backbuffer dimensions.", nameof(rectangle));
            }
            else
            {
                rect = new Rectangle(
                    0, 0, PresentationParameters.BackBufferWidth, PresentationParameters.BackBufferHeight);
            }

            int formatSize = PresentationParameters.BackBufferFormat.GetSize();
            if (sizeof(T) > formatSize || formatSize % sizeof(T) != 0)
                throw new ArgumentException($"{nameof(T)} is of an invalid size for the format of the backbuffer.", nameof(T));

            int spanBytes = destination.Length * sizeof(T);
            if (spanBytes > rect.Width * rect.Height * formatSize)
                throw new ArgumentOutOfRangeException(
                     nameof(destination), "The amount of data requested exceeds the backbuffer size.");

            PlatformGetBackBufferData(rect, destination);
        }

        #endregion

        #region Helpers

        private static int GetElementCountForType(PrimitiveType primitiveType, int primitiveCount)
        {
            switch (primitiveType)
            {
                case PrimitiveType.LineList: return primitiveCount * 2;
                case PrimitiveType.LineStrip: return primitiveCount + 1;
                case PrimitiveType.TriangleList: return primitiveCount * 3;
                case PrimitiveType.TriangleStrip: return primitiveCount + 2;
                default: throw new NotSupportedException();
            }
        }

        internal int GetClampedMultisampleCount(int multiSampleCount)
        {
            if (multiSampleCount > 1)
            {
                // Round down MultiSampleCount to the nearest power of two
                // hack from http://stackoverflow.com/a/2681094
                // Note: this will return an incorrect, but large value
                // for very large numbers. That doesn't matter because
                // the number will get clamped below anyway in this case.
                var msc = multiSampleCount;
                msc |= msc >> 1;
                msc |= msc >> 2;
                msc |= msc >> 4;
                msc -= msc >> 1;
                // and clamp it to what the device can handle
                if (msc > GraphicsCapabilities.MaxMultiSampleCount)
                    msc = GraphicsCapabilities.MaxMultiSampleCount;

                return msc;
            }
            else
                return 0;
        }

        // uniformly scales down the given rectangle by 10%
        internal static Rectangle GetDefaultTitleSafeArea(int x, int y, int width, int height)
        {
            var marginX = (width + 19) / 20;
            var marginY = (height + 19) / 20;
            x += marginX;
            y += marginY;

            width -= marginX * 2;
            height -= marginY * 2;
            return new Rectangle(x, y, width, height);
        }

        internal static Rectangle GetTitleSafeArea(int x, int y, int width, int height)
        {
            return PlatformGetTitleSafeArea(x, y, width, height);
        }

        #endregion

        #region IDisposable

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!IsDisposed)
            {
                if (disposing)
                {
                    // Dispose of all remaining graphics resources before disposing of the graphics device
                    lock (_resourcesLock)
                    {
                        foreach (var resource in _resources.ToArray())
                        {
                            if (resource.Target is IDisposable target)
                                target.Dispose();
                        }
                        _resources.Clear();
                    }

                    // Clear the effect cache.
                    EffectCache.Clear();

                    _blendState = null;
                    _actualBlendState = null;
                    _blendStateAdditive.Dispose();
                    _blendStateAlphaBlend.Dispose();
                    _blendStateNonPremultiplied.Dispose();
                    _blendStateOpaque.Dispose();

                    _depthStencilState = null;
                    _actualDepthStencilState = null;
                    _depthStencilStateDefault.Dispose();
                    _depthStencilStateDepthRead.Dispose();
                    _depthStencilStateNone.Dispose();

                    _rasterizerState = null;
                    _actualRasterizerState = null;
                    _rasterizerStateCullClockwise.Dispose();
                    _rasterizerStateCullCounterClockwise.Dispose();
                    _rasterizerStateCullNone.Dispose();

                    PlatformDispose();
                }

                IsDisposed = true;
                Disposing?.Invoke(this);
            }
        }

        ~GraphicsDevice()
        {
            Dispose(false);
        }

        #endregion
    }
}