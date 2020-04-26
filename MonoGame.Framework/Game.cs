// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime;
using System.Threading;
#if WINDOWS_UAP
using System.Threading.Tasks;
using Windows.ApplicationModel.Activation;
#endif
using MonoGame.Framework.Audio;
using MonoGame.Framework.Content;
using MonoGame.Framework.Graphics;
using MonoGame.Framework.Input.Touch;
using MonoGame.Framework.Utilities;

namespace MonoGame.Framework
{
    public partial class Game : IDisposable
    {
        private static PlatformInfo.OperatingSystem _currentOS = PlatformInfo.OS;

        private ContentManager _content;

        private readonly object _locker = new object();

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

        private IGraphicsDeviceManager _graphicsDeviceManager;
        private IGraphicsDeviceService _graphicsDeviceService;

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

            Platform = GamePlatform.PlatformCreate(this);
            Platform.Activated += OnActivated;
            Platform.Deactivated += OnDeactivated;
            Services.AddService(typeof(GamePlatform), Platform);

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

        #region IDisposable Implementation

        [DebuggerHidden]
        private void AssertNotDisposed()
        {
            if (_isDisposed)
                throw new ObjectDisposedException(GetType().Name);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
            Disposed?.Invoke(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_isDisposed)
            {
                if (disposing)
                {
                    // Dispose loaded game components
                    for (int i = 0; i < Components.Count; i++)
                    {
                        if (Components[i] is IDisposable disposable)
                            disposable.Dispose();
                    }
                    Components = null;

                    if (_content != null)
                    {
                        _content.Dispose();
                        _content = null;
                    }

                    if (_graphicsDeviceManager is GraphicsDeviceManager gdm)
                    {
                        gdm.Dispose();
                        _graphicsDeviceManager = null;
                    }

                    if (Platform != null)
                    {
                        Platform.Activated -= OnActivated;
                        Platform.Deactivated -= OnDeactivated;
                        Services.RemoveService<GamePlatform>();

                        Platform.Dispose();
                    }

                    ContentTypeReaderManager.ClearTypeCreators();

                    SoundEffect.PlatformShutdown();
                }
                _isDisposed = true;
            }
        }

        ~Game()
        {
            Dispose(false);
        }

        #endregion IDisposable Implementation

        #region Properties

        public LaunchParameters LaunchParameters { get; private set; }
        public GameComponentCollection Components { get; private set; }
        public GameServiceContainer Services { get; }

        public bool IsFixedTimeStep { get; set; } = true;
        public GameTime Time { get; } = new GameTime();

        [CLSCompliant(false)]
        public GameWindow Window => Platform.Window;

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
                        throw new InvalidOperationException("No Graphics Device Service");
                }
                return _graphicsDeviceService.GraphicsDevice;
            }
        }

        #endregion Properties

        // TODO: Internal members should be eliminated.
        // Currently Game.Initialized is used by the Mac game window class to
        // determine whether to raise DeviceResetting and DeviceReset on
        // GraphicsDeviceManager.
        internal bool Initialized { get; private set; } = false;

        #region Events

        public event DatalessEvent<Game> Activated;
        public event DatalessEvent<Game> Deactivated;
        public event DatalessEvent<Game> Disposed;
        public event DatalessEvent<Game> Exiting;

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
            Time.ElapsedGameTime = TimeSpan.Zero;
            _accumulatedElapsedTicks = 0;
            _previousTicks = 00;
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
                    throw new ArgumentException($"Handling for the run behavior {runBehavior} is not implemented.");
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

            if (!IsActive && (InactiveSleepTime.TotalMilliseconds >= 1.0))
            {
#if WINDOWS_UAP
                lock (_locker)
                    Monitor.Wait(_locker, InactiveSleepTime);
#else
                Thread.Sleep(InactiveSleepTime);
#endif
            }

            // Advance the accumulated elapsed time.
            var currentTicks = Stopwatch.GetTimestamp();
            _accumulatedElapsedTicks += currentTicks - _previousTicks;
            _previousTicks = currentTicks;

            if (IsFixedTimeStep && _accumulatedElapsedTicks < _targetElapsedTicks)
            {
                // Sleep for as long as possible without overshooting the update time.
                long sleepTicks = _targetElapsedTicks - _accumulatedElapsedTicks;
                
                // Check if the sleep time is more than 1 millisecond.
                if (sleepTicks >= Stopwatch.Frequency / 1000)
                {
                    var sleepTime = TimeSpan.FromTicks(sleepTicks);

                    switch (_currentOS)
                    {
                        case PlatformInfo.OperatingSystem.Windows:
                            if (PlatformInfo.Platform == MonoGamePlatform.WindowsUniversal)
                            {
                                lock (_locker)
                                    Monitor.Wait(_locker, sleepTime);
                            }
                            else
                            {
                                Thread.Sleep(sleepTime);
                            }
                            break;

                        default:
                            Thread.Sleep(sleepTime);
                            break;
                    }
                }

                // Keep looping until it's time to perform the next update
                goto RetryTick;
            }

            // Do not allow any update to take longer than our maximum.
            if (_accumulatedElapsedTicks > _maxElapsedTime.Ticks)
                _accumulatedElapsedTicks = _maxElapsedTime.Ticks;

            if (IsFixedTimeStep)
            {
                Time.ElapsedGameTime = TargetElapsedTime;
                int stepCount = 0;

                // Perform as many full fixed length time steps as we can.
                while (_accumulatedElapsedTicks >= _targetElapsedTicks && !_shouldExit)
                {
                    Time.TotalGameTime += TargetElapsedTime;
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

                //Every time we just do one update and one draw, then we are not running slowly, so decrease the lag
                if (stepCount == 1 && _updateFrameLag > 0)
                    _updateFrameLag--;

                // Draw needs to know the total elapsed time
                // that occured for the fixed length updates.
                Time.ElapsedGameTime = TimeSpan.FromTicks(TargetElapsedTime.Ticks * stepCount);
            }
            else
            {
                // Perform a single variable length update.
                Time.ElapsedGameTime = TimeSpan.FromTicks(_accumulatedElapsedTicks);
                Time.TotalGameTime += Time.ElapsedGameTime;
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

            // TODO: This should be removed once all platforms use the new GraphicsDeviceManager
#if !(WINDOWS && DIRECTX)
            InternalApplyChanges();
#endif

            // According to the information given on MSDN (see link below),
            // all GameComponents in Components at the time Initialize() is called are initialized.
            // http://msdn.microsoft.com/en-us/library/microsoft.xna.framework.game.initialize.aspx
            // Initialize all existing components
            InitializeExistingComponents();

            _graphicsDeviceService = Services.GetService<IGraphicsDeviceService>();

            if (_graphicsDeviceService != null &&
                _graphicsDeviceService.GraphicsDevice != null)
            {
                LoadContent();
            }
        }

        private static readonly Action<IDrawable, GameTime> DrawAction =
            (drawable, gameTime) => drawable.Draw(gameTime);

        protected virtual void Draw(GameTime gameTime)
        {
            _drawables.ForEachFilteredItem(DrawAction, gameTime);
        }

        private static readonly Action<IUpdateable, GameTime> UpdateAction =
            (updateable, gameTime) => updateable.Update(gameTime);

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
                TouchPanelState.CurrentTimestamp = gameTime.TotalGameTime;
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
            if (GraphicsDevice == null && GraphicsDeviceManager != null)
                _graphicsDeviceManager.CreateDevice();

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

        internal GraphicsDeviceManager GraphicsDeviceManager
        {
            get
            {
                if (_graphicsDeviceManager == null)
                    _graphicsDeviceManager = Services.GetService<IGraphicsDeviceManager>();
                return (GraphicsDeviceManager)_graphicsDeviceManager;
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

        /// <summary>
        /// Provides efficient, reusable sorting and filtering based on a
        /// configurable sort comparer, filter predicate, and associate change events.
        /// </summary>
        private class SortingFilteringCollection<T> : ICollection<T>
        {
            private readonly List<T> _items;
            private readonly List<AddJournalEntry<T>> _addJournal;
            private readonly Comparison<AddJournalEntry<T>> _addJournalSortComparison;
            private readonly List<int> _removeJournal;
            private readonly List<T> _cachedFilteredItems;
            private bool _shouldRebuildCache;

            private readonly Predicate<T> _filter;
            private readonly Comparison<T> _sort;
            private readonly Action<T, DatalessEvent<object>> _filterChangedSubscriber;
            private readonly Action<T, DatalessEvent<object>> _filterChangedUnsubscriber;
            private readonly Action<T, DatalessEvent<object>> _sortChangedSubscriber;
            private readonly Action<T, DatalessEvent<object>> _sortChangedUnsubscriber;

            public SortingFilteringCollection(
                Predicate<T> filter,
                Action<T, DatalessEvent<object>> filterChangedSubscriber,
                Action<T, DatalessEvent<object>> filterChangedUnsubscriber,
                Comparison<T> sort,
                Action<T, DatalessEvent<object>> sortChangedSubscriber,
                Action<T, DatalessEvent<object>> sortChangedUnsubscriber)
            {
                _items = new List<T>();
                _addJournal = new List<AddJournalEntry<T>>();
                _removeJournal = new List<int>();
                _cachedFilteredItems = new List<T>();
                _shouldRebuildCache = true;

                _filter = filter;
                _filterChangedSubscriber = filterChangedSubscriber;
                _filterChangedUnsubscriber = filterChangedUnsubscriber;
                _sort = sort;
                _sortChangedSubscriber = sortChangedSubscriber;
                _sortChangedUnsubscriber = sortChangedUnsubscriber;

                _addJournalSortComparison = CompareAddJournalEntry;
            }

            private int CompareAddJournalEntry(AddJournalEntry<T> x, AddJournalEntry<T> y)
            {
                int result = _sort(x.Item, y.Item);
                if (result != 0)
                    return result;
                return x.Order - y.Order;
            }

            public void ForEachFilteredItem<TUserData>(Action<T, TUserData> action, TUserData userData)
            {
                if (_shouldRebuildCache)
                {
                    ProcessRemoveJournal();
                    ProcessAddJournal();

                    // Rebuild the cache
                    _cachedFilteredItems.Clear();
                    for (int i = 0; i < _items.Count; ++i)
                        if (_filter(_items[i]))
                            _cachedFilteredItems.Add(_items[i]);

                    _shouldRebuildCache = false;
                }

                for (int i = 0; i < _cachedFilteredItems.Count; ++i)
                    action(_cachedFilteredItems[i], userData);

                // If the cache was invalidated as a result of processing items,
                // now is a good time to clear it and give the GC (more of) a
                // chance to do its thing.
                if (_shouldRebuildCache)
                    _cachedFilteredItems.Clear();
            }

            public void Add(T item)
            {
                // NOTE: We subscribe to item events after items in _addJournal have been merged.
                _addJournal.Add(new AddJournalEntry<T>(_addJournal.Count, item));
                InvalidateCache();
            }

            public bool Remove(T item)
            {
                if (_addJournal.Remove(AddJournalEntry<T>.CreateKey(item)))
                    return true;

                var index = _items.IndexOf(item);
                if (index >= 0)
                {
                    UnsubscribeFromItemEvents(item);
                    _removeJournal.Add(index);
                    InvalidateCache();
                    return true;
                }
                return false;
            }

            public void Clear()
            {
                for (int i = 0; i < _items.Count; ++i)
                {
                    _filterChangedUnsubscriber(_items[i], Item_FilterPropertyChanged);
                    _sortChangedUnsubscriber(_items[i], Item_SortPropertyChanged);
                }

                _addJournal.Clear();
                _removeJournal.Clear();
                _items.Clear();

                InvalidateCache();
            }

            public bool Contains(T item)
            {
                return _items.Contains(item);
            }

            public void CopyTo(T[] array, int arrayIndex)
            {
                _items.CopyTo(array, arrayIndex);
            }

            public int Count => _items.Count;

            public bool IsReadOnly => false;

            public IEnumerator<T> GetEnumerator()
            {
                return _items.GetEnumerator();
            }

            System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
            {
                return ((System.Collections.IEnumerable)_items).GetEnumerator();
            }

            private static readonly Comparison<int> RemoveJournalSortComparison =
                (x, y) => Comparer<int>.Default.Compare(y, x); // Sort high to low

            private void ProcessRemoveJournal()
            {
                if (_removeJournal.Count == 0)
                    return;

                // Remove items in reverse.  (Technically there exist faster
                // ways to bulk-remove from a variable-length array, but List<T>
                // does not provide such a method.)
                _removeJournal.Sort(RemoveJournalSortComparison);
                for (int i = 0; i < _removeJournal.Count; ++i)
                    _items.RemoveAt(_removeJournal[i]);
                _removeJournal.Clear();
            }

            private void ProcessAddJournal()
            {
                if (_addJournal.Count == 0)
                    return;

                // Prepare the _addJournal to be merge-sorted with _items.
                // _items is already sorted (because it is always sorted).
                _addJournal.Sort(_addJournalSortComparison);

                int iAddJournal = 0;
                int iItems = 0;

                while (iItems < _items.Count && iAddJournal < _addJournal.Count)
                {
                    var addJournalItem = _addJournal[iAddJournal].Item;
                    // If addJournalItem is less than (belongs before)
                    // _items[iItems], insert it.
                    if (_sort(addJournalItem, _items[iItems]) < 0)
                    {
                        SubscribeToItemEvents(addJournalItem);
                        _items.Insert(iItems, addJournalItem);
                        ++iAddJournal;
                    }
                    // Always increment iItems, either because we inserted and
                    // need to move past the insertion, or because we didn't
                    // insert and need to consider the next element.
                    ++iItems;
                }

                // If _addJournal had any "tail" items, append them all now.
                for (; iAddJournal < _addJournal.Count; ++iAddJournal)
                {
                    var addJournalItem = _addJournal[iAddJournal].Item;
                    SubscribeToItemEvents(addJournalItem);
                    _items.Add(addJournalItem);
                }

                _addJournal.Clear();
            }

            private void SubscribeToItemEvents(T item)
            {
                _filterChangedSubscriber(item, Item_FilterPropertyChanged);
                _sortChangedSubscriber(item, Item_SortPropertyChanged);
            }

            private void UnsubscribeFromItemEvents(T item)
            {
                _filterChangedUnsubscriber(item, Item_FilterPropertyChanged);
                _sortChangedUnsubscriber(item, Item_SortPropertyChanged);
            }

            private void InvalidateCache()
            {
                _shouldRebuildCache = true;
            }

            private void Item_FilterPropertyChanged(object sender)
            {
                InvalidateCache();
            }

            private void Item_SortPropertyChanged(object sender)
            {
                var item = (T)sender;
                var index = _items.IndexOf(item);

                _addJournal.Add(new AddJournalEntry<T>(_addJournal.Count, item));
                _removeJournal.Add(index);

                // Until the item is back in place, we don't care about its
                // events.  We will re-subscribe when _addJournal is processed.
                UnsubscribeFromItemEvents(item);
                InvalidateCache();
            }
        }

        private readonly struct AddJournalEntry<T>
        {
            public readonly int Order;
            public readonly T Item;

            public AddJournalEntry(int order, T item)
            {
                Order = order;
                Item = item;
            }

            public static AddJournalEntry<T> CreateKey(T item) => new AddJournalEntry<T>(-1, item);

            public override bool Equals(object obj)
            {
                if (obj is AddJournalEntry<T> entry)
                {
                    return Item is IEquatable<T> equatable
                        ? equatable.Equals(entry.Item)
                        : Equals(Item, entry.Item);
                }
                return false;
            }

            public override int GetHashCode() => Item.GetHashCode();
        }
    }
}