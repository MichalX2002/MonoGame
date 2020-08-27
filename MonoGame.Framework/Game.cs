// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime;
using MonoGame.Framework.Audio;
using MonoGame.Framework.Content;
using MonoGame.Framework.Graphics;
using MonoGame.Framework.Input.Touch;
using MonoGame.Framework.Utilities;

#if WINDOWS_UAP
using Windows.ApplicationModel.Activation;
#endif

namespace MonoGame.Framework
{
    public partial class Game : IDisposable
    {
        private ContentManager _content;

        private object SleepMutex { get; } = new object();

        internal GamePlatform Platform { get; }

        private SortingFilteringCollection<IDrawable> _drawables =
            new SortingFilteringCollection<IDrawable>(
                d => d.Visible,
                (d, handler) => d.VisibleChanged += handler,
                (d, handler) => d.VisibleChanged -= handler,
                (d1, d2) => Comparer<int>.Default.Compare(d1.DrawOrder, d2.DrawOrder),
                (d, handler) => d.DrawOrderChanged += handler,
                (d, handler) => d.DrawOrderChanged -= handler);

        private SortingFilteringCollection<IUpdateable> _updateables =
            new SortingFilteringCollection<IUpdateable>(
                u => u.Enabled,
                (u, handler) => u.EnabledChanged += handler,
                (u, handler) => u.EnabledChanged -= handler,
                (u1, u2) => Comparer<int>.Default.Compare(u1.UpdateOrder, u2.UpdateOrder),
                (u, handler) => u.UpdateOrderChanged += handler,
                (u, handler) => u.UpdateOrderChanged -= handler);

        private GraphicsDeviceManager? _graphicsDeviceManager;
        private IGraphicsDeviceService? _graphicsDeviceService;

        private TimeSpan _targetElapsedTime;
        private TimeSpan _inactiveSleepTime;
        private TimeSpan _maxElapsedTime;

        private long _targetElapsedTicks;
        private long _accumulatedElapsedTicks;
        private long _previousTicks;
        private int _updateFrameLag;

        private bool _isDisposed;
        private bool _shouldExit;
        private bool _suppressDraw;

        partial void PlatformConstruct();

        public Game()
        {
            LaunchParameters = new LaunchParameters();
            Services = new GameServiceContainer();
            Components = new GameComponentCollection();
            _content = new ContentManager(Services);

            TargetElapsedTime = TimeSpan.FromTicks(166667); // 60fps
            InactiveSleepTime = TimeSpan.FromTicks(333333); // 30fps
            MaxElapsedTime = TimeSpan.FromMilliseconds(500);

            // TODO: make Game instances depend on a GamePlatform instead
            Platform = GamePlatform.PlatformCreate(this);
            Platform.Activated += OnActivated;
            Platform.Deactivated += OnDeactivated;
            Services.AddService(Platform);

            // Calling Update() for first time initializes some systems
            FrameworkDispatcher.Update();

            // Allow some optional per-platform construction to occur too.
            PlatformConstruct();
        }

        [Conditional("DEBUG")]
        internal void Log(string Message)
        {
            if (Platform != null)
                Platform.Log(Message);
        }

        [DebuggerHidden]
        private void AssertNotDisposed()
        {
            if (_isDisposed)
                throw new ObjectDisposedException(GetType().Name);
        }

        #region Properties

        public GameTime Time { get; } = new GameTime();
        public GameServiceContainer Services { get; }

        public LaunchParameters LaunchParameters { get; private set; }
        public GameComponentCollection Components { get; private set; }

        public bool IsFixedTimeStep { get; set; } = true;

        public GameWindow? Window => Platform.Window;
        public bool IsActive => Platform.IsActive;

        public bool IsMouseVisible
        {
            get => Platform.IsMouseVisible;
            set => Platform.IsMouseVisible = value;
        }

        public ContentManager Content
        {
            get => _content;
            set => _content = value ?? throw new ArgumentNullException(nameof(value));
        }

        public TimeSpan InactiveSleepTime
        {
            get => _inactiveSleepTime;
            set
            {
                if (value < TimeSpan.Zero)
                    throw new ArgumentOutOfRangeException(
                        nameof(value), "The time must be positive.");

                _inactiveSleepTime = value;
            }
        }

        /// <summary>
        /// The maximum amount of time we will frameskip over
        /// and only perform Update calls with no Draw calls.
        /// </summary>
        public TimeSpan MaxElapsedTime
        {
            get => _maxElapsedTime;
            set
            {
                if (value < TimeSpan.Zero)
                    throw new ArgumentOutOfRangeException(
                        nameof(value), "The time must be positive.");
                if (value < _targetElapsedTime)
                    throw new ArgumentOutOfRangeException(
                        nameof(value), "The time must be at least TargetElapsedTime.");

                _maxElapsedTime = value;
            }
        }

        public TimeSpan TargetElapsedTime
        {
            get => _targetElapsedTime;
            set
            {
                // Give GamePlatform implementations an opportunity to override the new value.
                value = Platform != null ? Platform.TargetElapsedTimeChanging(value) : value;
                if (value <= TimeSpan.Zero)
                    throw new ArgumentOutOfRangeException(
                        "The time must be positive and non-zero.", default(Exception));

                if (value != _targetElapsedTime)
                {
                    _targetElapsedTime = value;
                    _targetElapsedTicks = _targetElapsedTime.Ticks;
                    Platform?.TargetElapsedTimeChanged();
                }
            }
        }

        public GraphicsDevice GraphicsDevice
        {
            get
            {
                if (_graphicsDeviceService == null)
                {
                    _graphicsDeviceService = Services.GetService<IGraphicsDeviceService>();
                    if (_graphicsDeviceService == null)
                        throw new InvalidOperationException(FrameworkResources.NoGraphicsDeviceService);
                }

                var device = _graphicsDeviceService.GraphicsDevice;
                if (device == null)
                    throw new InvalidOperationException(FrameworkResources.NoGraphicsDeviceInService);

                return device;
            }
        }

        #endregion Properties

        // TODO: Internal members should be eliminated.
        // Currently Game.Initialized is used by the Mac game window class to
        // determine whether to raise DeviceResetting and DeviceReset on
        // GraphicsDeviceManager.
        internal bool Initialized { get; private set; }

        #region Events

        public event Event<Game>? Activated;
        public event Event<Game>? Deactivated;
        public event Event<Game>? Disposed;
        public event Event<Game>? Exiting;

#if WINDOWS_UAP
        [CLSCompliant(false)]
        public ApplicationExecutionState PreviousExecutionState { get; internal set; }
#endif

        #endregion

        #region Public Methods

#if IOS
        [Obsolete("This platform's policy does not allow programmatically closing.", true)]
#endif
        public void Exit()
        {
            _shouldExit = true;
            _suppressDraw = true;
        }

        public void ResetElapsedTime()
        {
            Platform.ResetElapsedTime();
            ResetGameTimer();
            Time.Elapsed = TimeSpan.Zero;
            _accumulatedElapsedTicks = 0;
            _previousTicks = 0;
        }

        public void SuppressDraw()
        {
            _suppressDraw = true;
        }

        public void RunOneFrame()
        {
            if (Platform == null)
                return;

            if (!Platform.BeforeRun())
                return;

            if (!Initialized)
            {
                DoInitialize();
                ResetGameTimer();
                Initialized = true;
            }

            BeginRun();

            //Not quite right..
            Tick();

            EndRun();

        }

        public void Run()
        {
            Run(Platform.DefaultRunBehavior);
        }

        public void Run(GameRunBehavior runBehavior)
        {
            AssertNotDisposed();

            if (!Platform.BeforeRun())
            {
                BeginRun();
                ResetGameTimer();
                return;
            }

            if (!Initialized)
            {
                DoInitialize();
                Initialized = true;
            }

            // Initializing involves loading content, which often creates lots of garbage.
            // Invoking a compacting GC collection before the game continues should help 
            // memory usage/fragmentation without causing performance penalties/hickups later.
            GCSettings.LargeObjectHeapCompactionMode = GCLargeObjectHeapCompactionMode.CompactOnce;
            GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced, blocking: true, compacting: true);

            BeginRun();
            ResetGameTimer();

            switch (runBehavior)
            {
                case GameRunBehavior.Asynchronous:
                    Platform.AsyncRunLoopEnded += Platform_AsyncRunLoopEnded;
                    Platform.StartRunLoop();
                    break;

                case GameRunBehavior.Synchronous:
                    // XNA runs one Update even before showing the window
                    DoUpdate(new GameTime());

                    if (!_shouldExit)
                        Platform.RunLoop();

                    EndRun();
                    DoExiting();
                    break;

                default:
                    throw new ArgumentException(
                        $"Handling for the run behavior {runBehavior} is not implemented.");
            }
        }

        private void ResetGameTimer()
        {
            _previousTicks = Stopwatch.GetTimestamp();
        }

        public void Tick()
        {
            // NOTE: This code is very sensitive and can break very badly
            // with even what looks like a safe change. Be sure to test 
            // any change fully in both the fixed and variable timestep 
            // modes across multiple devices and platforms.

            RetryTick:

            if (!IsActive && InactiveSleepTime.Ticks >= TimeSpan.TicksPerMillisecond)
                ThreadHelper.Instance.Sleep(SleepMutex, InactiveSleepTime.Milliseconds);

            // Advance the accumulated elapsed time.
            long currentTicks = Stopwatch.GetTimestamp();
            _accumulatedElapsedTicks += currentTicks - _previousTicks;
            _previousTicks = currentTicks;

            if (IsFixedTimeStep && _accumulatedElapsedTicks < _targetElapsedTicks)
            {
                // Sleep for as long as possible without overshooting the update time.
                long sleepTicks = _targetElapsedTicks - _accumulatedElapsedTicks;

                // Check if the sleep time is more than 1 millisecond.
                if (sleepTicks >= Stopwatch.Frequency / 1000)
                {
                    int sleepMillis = (int)(sleepTicks / TimeSpan.TicksPerMillisecond);
                    ThreadHelper.Instance.Sleep(SleepMutex, sleepMillis);
                }

                // Keep looping until it's time to perform the next update
                goto RetryTick;
            }

            // Do not allow any update to take longer than our maximum.
            if (_accumulatedElapsedTicks > _maxElapsedTime.Ticks)
                _accumulatedElapsedTicks = _maxElapsedTime.Ticks;

            if (IsFixedTimeStep)
            {
                Time.Elapsed = TargetElapsedTime;
                int stepCount = 0;

                // Perform as many full fixed length time steps as we can.
                while (_accumulatedElapsedTicks >= _targetElapsedTicks && !_shouldExit)
                {
                    Time.Total += TargetElapsedTime;
                    _accumulatedElapsedTicks -= _targetElapsedTicks;
                    stepCount++;

                    DoUpdate(Time);
                }

                //Every update after the first accumulates lag
                _updateFrameLag += Math.Max(0, stepCount - 1);

                //If we think we are running slowly, wait until the lag clears before resetting it
                if (Time.IsRunningSlowly)
                {
                    if (_updateFrameLag == 0)
                        Time.IsRunningSlowly = false;
                }
                else if (_updateFrameLag >= 5)
                {
                    //If we lag more than 5 frames, start thinking we are running slowly
                    Time.IsRunningSlowly = true;
                }

                // Every time we just do one update and one draw, 
                // then we are not running slowly, so decrease the lag
                if (stepCount == 1 && _updateFrameLag > 0)
                    _updateFrameLag--;

                // Draw needs to know the total elapsed time
                // that occured for the fixed length updates.
                Time.Elapsed = TimeSpan.FromTicks(TargetElapsedTime.Ticks * stepCount);
            }
            else
            {
                // Perform a single variable length update.
                Time.Elapsed = TimeSpan.FromTicks(_accumulatedElapsedTicks);
                Time.Total += Time.Elapsed;
                _accumulatedElapsedTicks = 0;

                DoUpdate(Time);
            }

            // Draw unless the update suppressed it.
            if (_suppressDraw)
            {
                _suppressDraw = false;
            }
            else
            {
                DoDraw(Time);
            }

            if (_shouldExit)
            {
                Platform.Exit();
                _shouldExit = false; //prevents perpetual exiting on platforms supporting resume.
            }
        }

        #endregion

        #region Protected Methods

        protected virtual bool BeginDraw() => true;
        protected virtual void EndDraw() => Platform.Present();

        protected virtual void BeginRun() { }
        protected virtual void EndRun() { }

        protected virtual void LoadContent() { }
        protected virtual void UnloadContent() { }

        protected virtual void Initialize()
        {
            if (_shouldExit)
                return;

            // According to the information given on MSDN (see link below),
            // all GameComponents in Components at the time Initialize() is called are initialized.
            // http://msdn.microsoft.com/en-us/library/microsoft.xna.framework.game.initialize.aspx
            // Initialize all existing components
            InitializeExistingComponents();

            _graphicsDeviceService = Services.GetService<IGraphicsDeviceService>();

            // TODO: This should be removed once all platforms use the new GraphicsDeviceManager
#if !(WINDOWS && DIRECTX)
            if (_graphicsDeviceService != null &&
                _graphicsDeviceService.GraphicsDevice != null)
            {
                InternalApplyChanges();
            }
#endif

            LoadContent();
        }

        private static Action<IDrawable, GameTime> DrawAction { get; } =
            (drawable, gameTime) => drawable.Draw(gameTime);

        private static Action<IUpdateable, GameTime> UpdateAction { get; } =
            (updateable, gameTime) => updateable.Update(gameTime);

        protected virtual void Draw(GameTime gameTime)
        {
            _drawables.ForEachFilteredItem(DrawAction, gameTime);
        }

        protected virtual void Update(GameTime gameTime)
        {
            _updateables.ForEachFilteredItem(UpdateAction, gameTime);
        }

        protected virtual void OnExiting(Game sender)
        {
            Exiting?.Invoke(sender);
        }

        protected virtual void OnActivated(Game sender)
        {
            AssertNotDisposed();
            Activated?.Invoke(sender);
        }

        protected virtual void OnDeactivated(Game sender)
        {
            AssertNotDisposed();
            Deactivated?.Invoke(sender);
        }

        #endregion Protected Methods

        #region Event Handlers

        private void Components_ComponentAdded(object sender, IGameComponent gameComponent)
        {
            // Since we only subscribe to ComponentAdded after the graphics
            // devices are set up, it is safe to just blindly call Initialize.
            gameComponent.Initialize();
            AddComponent(gameComponent);
        }

        private void Components_ComponentRemoved(object sender, IGameComponent gameComponent)
        {
            RemoveComponent(gameComponent);
        }

        private void Platform_AsyncRunLoopEnded(GamePlatform sender)
        {
            AssertNotDisposed();

            sender.AsyncRunLoopEnded -= Platform_AsyncRunLoopEnded;
            EndRun();
            DoExiting();
        }

        #endregion Event Handlers

        // TODO: We should work toward eliminating internal methods.  They
        //       break entirely the possibility that additional platforms could
        //       be added by third parties without changing MonoGame itself.

        #region Internal Methods

#if !(WINDOWS && DIRECTX)
        internal void InternalApplyChanges()
        {
            Platform.BeginScreenDeviceChange(GraphicsDevice.PresentationParameters.IsFullScreen);

            if (GraphicsDevice.PresentationParameters.IsFullScreen)
                Platform.EnterFullScreen();
            else
                Platform.ExitFullScreen();

            var viewport = new Viewport(
                0, 0,
                GraphicsDevice.PresentationParameters.BackBufferWidth,
                GraphicsDevice.PresentationParameters.BackBufferHeight);

            GraphicsDevice.Viewport = viewport;
            Platform.EndScreenDeviceChange(string.Empty, viewport.Width, viewport.Height);
        }
#endif

        internal void DoUpdate(GameTime gameTime)
        {
            AssertNotDisposed();

            if (Platform.BeforeUpdate(gameTime))
            {
                FrameworkDispatcher.Update();

                Update(gameTime);

                //The TouchPanel needs to know the time for when touches arrive
                TouchPanel.CurrentTimestamp = gameTime.Total;
            }
        }

        internal void DoDraw(GameTime gameTime)
        {
            AssertNotDisposed();

            // Draw and EndDraw should not be called if BeginDraw returns false.
            // http://stackoverflow.com/questions/4054936/manual-control-over-when-to-redraw-the-screen/4057180#4057180
            // http://stackoverflow.com/questions/4235439/xna-3-1-to-4-0-requires-constant-redraw-or-will-display-a-purple-screen
            if (Platform.BeforeDraw(gameTime) && BeginDraw())
            {
                Draw(gameTime);
                EndDraw();
            }
        }

        internal void DoInitialize()
        {
            AssertNotDisposed();

            GraphicsDeviceManager?.CreateDevice();

            Platform.BeforeInitialize();
            Initialize();

            // We need to do this after virtual Initialize(...) is called.
            // 1. Categorize components into IUpdateable and IDrawable lists.
            // 2. Subscribe to Added/Removed events to keep the categorized
            //    lists synced and to Initialize future components as they are added.            
            CategorizeComponents();
            Components.ComponentAdded += Components_ComponentAdded;
            Components.ComponentRemoved += Components_ComponentRemoved;
        }

        internal void DoExiting()
        {
            OnExiting(this);
            UnloadContent();
        }

        #endregion Internal Methods

        internal GraphicsDeviceManager? GraphicsDeviceManager
        {
            get
            {
                if (_graphicsDeviceManager == null)
                    _graphicsDeviceManager = Services.GetService<IGraphicsDeviceManager>() as GraphicsDeviceManager;

                return _graphicsDeviceManager;
            }
            set
            {
                if (_graphicsDeviceManager != null)
                    throw new InvalidOperationException(
                        "The GraphicsDeviceManager is already set and cannot be changed.");

                _graphicsDeviceManager = value;
            }
        }

        // NOTE: InitializeExistingComponents really should only be called once.
        //       Game.Initialize is the only method in a position to guarantee
        //       that no component will get a duplicate Initialize call.
        //       Further calls to Initialize occur immediately in response to
        //       Components.ComponentAdded.
        private void InitializeExistingComponents()
        {
            for (int i = 0; i < Components.Count; ++i)
                Components[i].Initialize();
        }

        private void CategorizeComponents()
        {
            ClearComponents();
            for (int i = 0; i < Components.Count; ++i)
                AddComponent(Components[i]);
        }

        private void ClearComponents()
        {
            _updateables.Clear();
            _drawables.Clear();
        }

        private void AddComponent(IGameComponent component)
        {
            if (component is IUpdateable updateable)
                _updateables.Add(updateable);

            if (component is IDrawable drawable)
                _drawables.Add(drawable);
        }

        private void RemoveComponent(IGameComponent component)
        {
            if (component is IUpdateable updateable)
                _updateables.Remove(updateable);

            if (component is IDrawable drawable)
                _drawables.Remove(drawable);
        }

        #region IDisposable

        protected virtual void Dispose(bool disposing)
        {
            if (!_isDisposed)
            {
                if (disposing)
                {
                    // Dispose loaded game components
                    for (int i = Components.Count; i-- > 0;)
                    {
                        if (Components[i] is IDisposable disposable)
                            disposable.Dispose();
                    }
                    Components = null!;

                    if (_content != null)
                    {
                        _content.Dispose();
                        _content = null!;
                    }

                    if (_graphicsDeviceManager != null)
                    {
                        _graphicsDeviceManager.Dispose();
                        _graphicsDeviceManager = null!;
                    }

                    if (Platform != null)
                    {
                        Platform.Activated -= OnActivated;
                        Platform.Deactivated -= OnDeactivated;
                        Services.RemoveService<GamePlatform>();

                        Platform.Dispose();
                    }

                    ContentTypeReaderManager.ClearReaderFactories();

                    SoundEffect.PlatformShutdown();
                }

                _isDisposed = true;
                Disposed?.Invoke(this);
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Disposes the <see cref="Game"/>.
        /// </summary>
        ~Game()
        {
            Dispose(false);
        }

        #endregion 
    }
}