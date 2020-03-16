// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using MonoGame.Framework.Graphics;
using MonoGame.Framework.Input.Touch;

namespace MonoGame.Framework
{
    /// <summary>
    /// Used to initialize and control the presentation of the graphics device.
    /// </summary>
    public partial class GraphicsDeviceManager : IGraphicsDeviceService, IDisposable, IGraphicsDeviceManager
    {
        private readonly Game _game;
        private bool _initialized = false;

        private int _preferredBackBufferHeight;
        private int _preferredBackBufferWidth;
        private SurfaceFormat _preferredBackBufferFormat;
        private DepthFormat _preferredDepthStencilFormat;
        private bool _preferMultiSampling;
        private DisplayOrientation _supportedOrientations;
        private PresentInterval _presentInterval;
        private bool _drawBegun;
        private bool _disposed;
        private bool _hardwareModeSwitch = true;
        private bool _preferHalfPixelOffset = false;
        private bool _wantFullScreen;
        private GraphicsProfile _graphicsProfile;
        // dirty flag for ApplyChanges
        private bool _shouldApplyChanges;

        /// <summary>
        /// The default back buffer width.
        /// </summary>
        public static readonly int DefaultBackBufferWidth = 800;

        /// <summary>
        /// The default back buffer height.
        /// </summary>
        public static readonly int DefaultBackBufferHeight = 480;

        /// <summary>
        /// Optional override for platform specific defaults.
        /// </summary>
        partial void PlatformConstruct();

        /// <summary>
        /// Associates this <see cref="GraphicsDeviceManager"/> to a <see cref="Game"/> instance.
        /// </summary>
        /// <param name="game">The game instance to attach to.</param>
        public GraphicsDeviceManager(Game game)
        {
            _game = game ?? throw new ArgumentNullException(nameof(game));

            _supportedOrientations = DisplayOrientation.Default;
            _preferredBackBufferFormat = SurfaceFormat.Color;
            _preferredDepthStencilFormat = DepthFormat.Depth24;
            _presentInterval = PresentInterval.One;

            // Assume the window client size as the default back 
            // buffer resolution in the landscape orientation.
            var clientBounds = _game.Window.ClientBounds;
            if (clientBounds.Width >= clientBounds.Height)
            {
                _preferredBackBufferWidth = clientBounds.Width;
                _preferredBackBufferHeight = clientBounds.Height;
            }
            else
            {
                _preferredBackBufferWidth = clientBounds.Height;
                _preferredBackBufferHeight = clientBounds.Width;
            }

            // Default to windowed mode... this is ignored on platforms that don't support it.
            _wantFullScreen = false;

            // XNA would read this from the manifest, but it would always default
            // to reach unless changed.  So lets mimic that without the manifest bit.
            GraphicsProfile = GraphicsProfile.Reach;

            // Let the plaform optionally overload construction defaults.
            PlatformConstruct();

            if (_game.Services.GetService<IGraphicsDeviceManager>() != null)
                throw new ArgumentException(
                    "A graphics device manager is already registered. " +
                    "The graphics device manager cannot be changed once it is set.");
            
            _game.GraphicsDeviceManager = this;

            _game.Services.AddService<IGraphicsDeviceManager>(this);
            _game.Services.AddService<IGraphicsDeviceService>(this);
        }

        /// <summary>
        /// Finalizes the <see cref="GraphicsDeviceManager"/> and disposes it.
        /// </summary>
        ~GraphicsDeviceManager()
        {
            Dispose(false);
        }

        private void CreateDevice()
        {
            if (GraphicsDevice != null)
                return;

            try
            {
                if (!_initialized)
                    Initialize();

                var gdi = DoPreparingDeviceSettings();
                CreateDevice(gdi);
            }
            catch (NoSuitableGraphicsDeviceException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new NoSuitableGraphicsDeviceException("Failed to create graphics device.", ex);
            }
        }

        private void CreateDevice(GraphicsDeviceInformation gdi)
        {
            if (GraphicsDevice != null)
                return;

            GraphicsDevice = new GraphicsDevice(
                gdi.Adapter, gdi.GraphicsProfile, PreferHalfPixelOffset, gdi.PresentationParameters);
            _shouldApplyChanges = false;

            // hook up reset events
            GraphicsDevice.DeviceReset     += (sender) => OnDeviceReset();
            GraphicsDevice.DeviceResetting += (sender) => OnDeviceResetting();

            // update the touchpanel display size when the graphicsdevice is reset
            GraphicsDevice.DeviceReset += UpdateTouchPanel;
            GraphicsDevice.PresentationChanged += OnPresentationChanged;

            OnDeviceCreated();
        }

        void IGraphicsDeviceManager.CreateDevice()
        {
            CreateDevice();
        }

        public bool BeginDraw()
        {
            if (GraphicsDevice == null)
                return false;

            _drawBegun = true;
            return true;
        }

        public void EndDraw()
        {
            if (GraphicsDevice != null && _drawBegun)
            {
                _drawBegun = false;
                GraphicsDevice.Present();
            }
        }

        #region IGraphicsDeviceService

        public event DatalessEvent<IGraphicsDeviceService> DeviceCreated;
        public event DatalessEvent<IGraphicsDeviceService> DeviceDisposing;
        public event DatalessEvent<IGraphicsDeviceService> DeviceReset;
        public event DatalessEvent<IGraphicsDeviceService> DeviceResetting;
        public event DataEvent<IGraphicsDeviceService, GraphicsDeviceInformation> PreparingDeviceSettings;
        public event DatalessEvent<IGraphicsDeviceService> Disposed;

        protected void OnDeviceDisposing() => DeviceDisposing?.Invoke(this);

        protected void OnDeviceResetting() => DeviceResetting?.Invoke(this);

        internal void OnDeviceReset() => DeviceReset?.Invoke(this);

        internal void OnDeviceCreated() => DeviceCreated?.Invoke(this);

        /// <summary>
        /// This populates a <see cref="GraphicsDeviceInformation"/> instance and invokes 
        /// <see cref="PreparingDeviceSettings"/> to allow users to change the settings. 
        /// Then returns that <see cref="GraphicsDeviceInformation"/>.
        /// </summary>
        /// <exception cref="NullReferenceException">
        /// <see cref="GraphicsDeviceInformation.PresentationParameters"/> or
        /// <see cref="GraphicsDeviceInformation.Adapter"/> was set to null.
        /// </exception>
        private GraphicsDeviceInformation DoPreparingDeviceSettings()
        {
            var gdi = new GraphicsDeviceInformation();
            PrepareGraphicsDeviceInformation(gdi);

            if (PreparingDeviceSettings != null)
            {
                // this allows users to overwrite settings through the argument
                PreparingDeviceSettings(this, gdi);

                if (gdi.PresentationParameters == null || gdi.Adapter == null)
                    throw new NullReferenceException(
                        $"Members in the {nameof(GraphicsDeviceInformation)} should not be set to null.");
            }

            return gdi;
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
            if (!_disposed)
            {
                if (disposing)
                {
                    if (GraphicsDevice != null)
                    {
                        GraphicsDevice.Dispose();
                        GraphicsDevice = null;
                    }
                }
                _disposed = true;
                Disposed?.Invoke(this);
            }
        }

        #endregion

        partial void PlatformApplyChanges();

        partial void PlatformPreparePresentationParameters(PresentationParameters presentationParameters);

        private void PreparePresentationParameters(PresentationParameters presentationParameters)
        {
            presentationParameters.BackBufferFormat = _preferredBackBufferFormat;
            presentationParameters.BackBufferWidth = _preferredBackBufferWidth;
            presentationParameters.BackBufferHeight = _preferredBackBufferHeight;
            presentationParameters.DepthStencilFormat = _preferredDepthStencilFormat;
            presentationParameters.IsFullScreen = _wantFullScreen;
            presentationParameters.HardwareModeSwitch = _hardwareModeSwitch;
            presentationParameters.PresentationInterval = _presentInterval;
            presentationParameters.DisplayOrientation = _game.Window.CurrentOrientation;
            presentationParameters.DeviceWindowHandle = _game.Window.Handle;

            if (_preferMultiSampling)
            {
                // always initialize MultiSampleCount to the maximum, if users want to overwrite
                // this they have to respond to the PreparingDeviceSettingsEvent and modify
                // args.GraphicsDeviceInformation.PresentationParameters.MultiSampleCount
                presentationParameters.MultiSampleCount = GraphicsDevice != null
                    ? GraphicsDevice.GraphicsCapabilities.MaxMultiSampleCount
                    : 32;
            }
            else
            {
                presentationParameters.MultiSampleCount = 0;
            }

            PlatformPreparePresentationParameters(presentationParameters);
        }

        private void PrepareGraphicsDeviceInformation(GraphicsDeviceInformation gdi)
        {
            gdi.Adapter = GraphicsAdapter.DefaultAdapter;
            gdi.GraphicsProfile = GraphicsProfile;
            var pp = new PresentationParameters();
            PreparePresentationParameters(pp);
            gdi.PresentationParameters = pp;
        }

        /// <summary>
        /// Applies any pending property changes to the graphics device.
        /// </summary>
        public void ApplyChanges()
        {
            // If the device hasn't been created then create it now.
            if (GraphicsDevice == null)
                CreateDevice();

            if (!_shouldApplyChanges)
                return;

            _shouldApplyChanges = false;

            _game.Window.SetSupportedOrientations(_supportedOrientations);

            // Allow for optional platform specific behavior.
            PlatformApplyChanges();

            // populates a gdi with settings in this gdm and allows users to override them with
            // PrepareDeviceSettings event this information should be applied to the GraphicsDevice
            var gdi = DoPreparingDeviceSettings();

            if (gdi.GraphicsProfile != GraphicsDevice.GraphicsProfile)
            {
                // if the GraphicsProfile changed we need to create a new GraphicsDevice
                DisposeGraphicsDevice();
                CreateDevice(gdi);
                return;
            }

            GraphicsDevice.Reset(gdi.PresentationParameters);
        }

        private void DisposeGraphicsDevice()
        {
            GraphicsDevice.Dispose();
            DeviceDisposing?.Invoke(this);
            GraphicsDevice = null;
        }

        partial void PlatformInitialize(PresentationParameters presentationParameters);

        private void Initialize()
        {
            _game.Window.SetSupportedOrientations(_supportedOrientations);

            var presentationParameters = new PresentationParameters();
            PreparePresentationParameters(presentationParameters);

            // Allow for any per-platform changes to the presentation.
            PlatformInitialize(presentationParameters);

            _initialized = true;
        }

        private void UpdateTouchPanel(GraphicsDevice sender)
        {
            TouchPanel.DisplayWidth = GraphicsDevice.PresentationParameters.BackBufferWidth;
            TouchPanel.DisplayHeight = GraphicsDevice.PresentationParameters.BackBufferHeight;
            TouchPanel.DisplayOrientation = GraphicsDevice.PresentationParameters.DisplayOrientation;
        }

        /// <summary>
        /// Toggles between windowed and fullscreen modes.
        /// </summary>
        /// <remarks>
        /// Note that on platforms that do not support windowed modes this has no affect.
        /// </remarks>
        public void ToggleFullScreen()
        {
            IsFullScreen = !IsFullScreen;
            ApplyChanges();
        }

        private void OnPresentationChanged(object sender, PresentationParameters presentationParams)
        {
            _game.Platform.OnPresentationChanged(presentationParams);
        }

        /// <summary>
        /// The profile which determines the graphics feature level.
        /// </summary>
        public GraphicsProfile GraphicsProfile
        {
            get => _graphicsProfile;
            set
            {
                _shouldApplyChanges = true;
                _graphicsProfile = value;
            }
        }

        /// <summary>
        /// Returns the graphics device for this manager.
        /// </summary>
        public GraphicsDevice GraphicsDevice { get; private set; }

        /// <summary>
        /// Indicates the desire to switch into fullscreen mode.
        /// </summary>
        /// <remarks>
        /// When called at startup this will automatically set fullscreen mode during initialization.  If
        /// set after startup you must call <see cref="ApplyChanges"/> for the fullscreen mode to be changed.
        /// Note that for some platforms that do not support windowed modes this property has no affect.
        /// </remarks>
        public bool IsFullScreen
        {
            get => _wantFullScreen;
            set
            {
                _shouldApplyChanges = true;
                _wantFullScreen = value;
            }
        }

        /// <summary>
        /// Gets or sets the boolean which defines how window switches from windowed to fullscreen state.
        /// "Hard" mode (true) is slow to switch, but more effecient for performance, while "soft" mode (false) is vice versa.
        /// The default value is <see langword="true"/>.
        /// </summary>
        public bool HardwareModeSwitch
        {
            get => _hardwareModeSwitch;
            set
            {
                _shouldApplyChanges = true;
                _hardwareModeSwitch = value;
            }
        }

        /// <summary>
        /// Indicates if DX9 style pixel addressing or current standard
        /// pixel addressing should be used. This flag is set to
        /// <see langword="false"/> by default. It should be set to <see langword="true"/>
        /// for XNA compatibility. It is recommended to leave this flag
        /// set to <see langword="false"/> for projects that are not ported from
        /// XNA. This value is passed to <see cref="GraphicsDevice.UseHalfPixelOffset"/>.
        /// </summary>
        /// <remarks>
        /// XNA uses DirectX9 for its graphics. DirectX9 interprets UV
        /// coordinates differently from other graphics API's. This is
        /// typically referred to as the half-pixel offset. MonoGame
        /// replicates XNA behavior if this flag is set to <see langword="true"/>.
        /// </remarks>
        public bool PreferHalfPixelOffset
        {
            get => _preferHalfPixelOffset;
            set
            {
                if (GraphicsDevice != null)
                    throw new InvalidOperationException(
                        "Setting PreferHalfPixelOffset is not allowed after the creation of GraphicsDevice.");
                _preferHalfPixelOffset = value;
            }
        }

        /// <summary>
        /// Indicates the desire for a multisampled back buffer.
        /// </summary>
        /// <remarks>
        /// When called at startup this will automatically set the MSAA mode during initialization.  If
        /// set after startup you must call ApplyChanges() for the MSAA mode to be changed.
        /// </remarks>
        public bool PreferMultiSampling
        {
            get => _preferMultiSampling;
            set
            {
                _shouldApplyChanges = true;
                _preferMultiSampling = value;
            }
        }

        /// <summary>
        /// Indicates the desired back buffer color format.
        /// </summary>
        /// <remarks>
        /// When called at startup this will automatically set the format during initialization.  If
        /// set after startup you must call ApplyChanges() for the format to be changed.
        /// </remarks>
        public SurfaceFormat PreferredBackBufferFormat
        {
            get => _preferredBackBufferFormat;
            set
            {
                _shouldApplyChanges = true;
                _preferredBackBufferFormat = value;
            }
        }

        /// <summary>
        /// Indicates the desired back buffer height in pixels.
        /// </summary>
        /// <remarks>
        /// When called at startup this will automatically set the height during initialization.  If
        /// set after startup you must call ApplyChanges() for the height to be changed.
        /// </remarks>
        public int PreferredBackBufferHeight
        {
            get => _preferredBackBufferHeight;
            set
            {
                _shouldApplyChanges = true;
                _preferredBackBufferHeight = value;
            }
        }

        /// <summary>
        /// Indicates the desired back buffer width in pixels.
        /// </summary>
        /// <remarks>
        /// When called at startup this will automatically set the width during initialization.  If
        /// set after startup you must call ApplyChanges() for the width to be changed.
        /// </remarks>
        public int PreferredBackBufferWidth
        {
            get => _preferredBackBufferWidth;
            set
            {
                _shouldApplyChanges = true;
                _preferredBackBufferWidth = value;
            }
        }

        /// <summary>
        /// Indicates the desired depth-stencil buffer format.
        /// </summary>
        /// <remarks>
        /// The depth-stencil buffer format defines the scene depth precision and stencil bits available for effects during rendering.
        /// When called at startup this will automatically set the format during initialization.  If
        /// set after startup you must call ApplyChanges() for the format to be changed.
        /// </remarks>
        public DepthFormat PreferredDepthStencilFormat
        {
            get => _preferredDepthStencilFormat;
            set
            {
                _shouldApplyChanges = true;
                _preferredDepthStencilFormat = value;
            }
        }

        /// <summary>
        /// Indicates the desire for VSync when presenting the back buffer.
        /// </summary>
        /// <remarks>
        /// VSync limits the frame rate of the game to the monitor referesh rate to prevent screen tearing.
        /// When called at startup this will automatically set the VSync mode during initialization.
        /// If set after startup you must call ApplyChanges() for the VSync mode to be changed.
        /// </remarks>
        public PresentInterval VerticalSyncInterval
        {
            get => _presentInterval;
            set
            {
                _shouldApplyChanges = true;
                _presentInterval = value;
            }
        }

        /// <summary>
        /// Indicates the desired allowable display orientations when the device is rotated.
        /// </summary>
        /// <remarks>
        /// This property only applies to mobile platforms with automatic display rotation.
        /// When called at startup this will automatically apply the supported orientations during initialization.  If
        /// set after startup you must call ApplyChanges() for the supported orientations to be changed.
        /// </remarks>
        public DisplayOrientation SupportedOrientations
        {
            get => _supportedOrientations;
            set
            {
                _shouldApplyChanges = true;
                _supportedOrientations = value;
            }
        }
    }
}
