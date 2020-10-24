// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Android.Content;
using Android.Media;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Javax.Microedition.Khronos.Egl;
using MonoGame.Framework.Graphics;
using MonoGame.Framework.Input;
using MonoGame.Framework.Input.Touch;

namespace MonoGame.Framework
{
    [CLSCompliant(false)]
    public partial class MonoGameAndroidGameView : SurfaceView, ISurfaceHolderCallback, View.IOnTouchListener
    {
        public static event DatalessEvent<MonoGameAndroidGameView> OnGameThreadPause;
        public static event DatalessEvent<MonoGameAndroidGameView> OnGameThreadResume;

        public event DataEvent<MonoGameAndroidGameView, FrameEventArgs> RenderFrame;
        public event DataEvent<MonoGameAndroidGameView, FrameEventArgs> UpdateFrame;

        private bool _disposed = false;
        private bool _loaded = false;
        private bool _androidSurfaceAvailable;
        private bool _glSurfaceAvailable;
        private bool _glContextAvailable;
        private bool _glContextLost;
        private volatile InternalState _internalState = InternalState.Exited_GameThread;

        private Task _renderTask;
        private CancellationTokenSource _renderCancellation = null;
        private readonly AndroidTouchEventManager _touchManager;
        private readonly AndroidGameWindow _gameWindow;
        private readonly Game _game;
        private Size _surfaceSize;

        private ManualResetEvent _waitForPausedStateProcessed = new ManualResetEvent(false);
        private ManualResetEvent _waitForResumedStateProcessed = new ManualResetEvent(false);
        private ManualResetEvent _waitForExitedStateProcessed = new ManualResetEvent(false);
        private AutoResetEvent _waitForMainGameLoop = new AutoResetEvent(false);
        private readonly object _lockObject = new object();

        private Stopwatch _stopWatch;
        private double _tick = 0;
        private int _frames = 0;
        private double _prev = 0;
        private double _avgFps = 0;

        private DateTime _prevUpdateTime;
        private DateTime _prevRenderTime;
        private DateTime _curUpdateTime;
        private DateTime _curRenderTime;
        private FrameEventArgs _updateEventArgs = new FrameEventArgs();

        public bool IsResuming { get; private set; }

        public bool TouchEnabled
        {
            get => _touchManager.Enabled;
            set
            {
                _touchManager.Enabled = value;
                SetOnTouchListener(value ? this : null);
            }
        }

        public MonoGameAndroidGameView(
            Context context, AndroidGameWindow gameWindow, Game game) : base(context)
        {
            _gameWindow = gameWindow;
            _game = game;
            _touchManager = new AndroidTouchEventManager(gameWindow);
            Init();
        }

        private void Init()
        {
            // Add callback to get the SurfaceCreated etc events
            Holder.AddCallback(this);

            if (Build.VERSION.SdkInt < BuildVersionCodes.IceCreamSandwichMr1)
#pragma warning disable CS0618 
                Holder.SetType(Android.Views.SurfaceType.Gpu);
#pragma warning restore CS0618
        }

        public void SurfaceChanged(ISurfaceHolder holder, Android.Graphics.Format format, int width, int height)
        {
            // Set flag to recreate gl surface or rendering can be bad on orienation change or if app 
            // is closed in one orientation and re-opened in another.
            lock (_lockObject)
            {
                // can only be triggered when main loop is running, is unsafe to overwrite other states
                if (_internalState == InternalState.Running_GameThread)
                {
                    _internalState = InternalState.ForceRecreateSurface;
                }
                else
                {
                    needToForceRecreateSurface = true;
                }
            }
        }

        public void SurfaceCreated(ISurfaceHolder holder)
        {
            lock (_lockObject)
            {
                _androidSurfaceAvailable = true;
            }
        }

        public void SurfaceDestroyed(ISurfaceHolder holder)
        {
            lock (_lockObject)
            {
                _androidSurfaceAvailable = false;
            }
        }

        public bool OnTouch(View v, MotionEvent e)
        {
            _touchManager.OnTouchEvent(e);
            return true;
        }

        public virtual void SwapBuffers()
        {
            AssertNotDisposed();
            if (!egl.EglSwapBuffers(eglDisplay, eglSurface))
            {
                if (egl.EglGetError() == 0)
                {
                    if (_glContextLost)
                        System.Diagnostics.Debug.WriteLine("Lost EGL context" + GetErrorAsString());
                    _glContextLost = true;
                }
            }
        }

        public virtual void MakeCurrent()
        {
            AssertNotDisposed();
            if (!egl.EglMakeCurrent(eglDisplay, eglSurface,
                    eglSurface, eglContext))
            {
                System.Diagnostics.Debug.WriteLine("Error Make Current" + GetErrorAsString());
            }
        }

        public virtual void ClearCurrent()
        {
            AssertNotDisposed();
            if (!egl.EglMakeCurrent(eglDisplay, EGL10.EglNoSurface,
                EGL10.EglNoSurface, EGL10.EglNoContext))
            {
                System.Diagnostics.Debug.WriteLine("Error Clearing Current" + GetErrorAsString());
            }
        }

        double updates;

        public bool LogFPS { get; set; }
        public bool RenderOnUIThread { get; set; }

        public virtual void Run()
        {
            Run(0.0);
        }

        public virtual void Run(double updatesPerSecond)
        {
            _renderCancellation = new CancellationTokenSource();
            if (LogFPS)
                _avgFps = 1;
            updates = 1000 / updatesPerSecond;

            // We always start a new task, regardless if we render on UI thread or not.
            var syncContext = SynchronizationContext.Current;

            _renderTask = Task.Factory.StartNew(
                () => WorkerThreadFrameDispatcher(syncContext),
                _renderCancellation.Token, TaskCreationOptions.LongRunning, TaskScheduler.Default);

            _renderTask.ContinueWith((t) => OnStopped());
        }

        public virtual void Pause()
        {
            AssertNotDisposed();

            // if triggered in quick succession and blocked by graphics device creation, 
            // pause can be triggered twice, without resume in between on some phones.
            if (_internalState != InternalState.Running_GameThread)
                return;

            // this guarantees that resume finished processing, since we cannot wait inside resume because we deadlock as surface wouldn't get created
            if (!RenderOnUIThread)
                _waitForResumedStateProcessed.WaitOne();

            _waitForMainGameLoop.Reset();  // in case it was enabled

            lock (_lockObject)
            {
                if (!_androidSurfaceAvailable)
                    _internalState = InternalState.Paused_GameThread; // prepare for next game loop iteration
            }

            lock (_lockObject)
            {
                // processing the pausing state only if the surface was created already
                if (_androidSurfaceAvailable)
                {
                    _waitForPausedStateProcessed.Reset();
                    _internalState = InternalState.Pausing_UIThread;
                }
            }

            if (!RenderOnUIThread)
                _waitForPausedStateProcessed.WaitOne();
        }

        public virtual void Resume()
        {
            AssertNotDisposed();

            lock (_lockObject)
            {
                _waitForResumedStateProcessed.Reset();
                _internalState = InternalState.Resuming_UIThread;
            }

            _waitForMainGameLoop.Set();

            try
            {
                if (!IsFocused)
                    RequestFocus();
            }
            catch
            {
            }

            // do not wait for state transition here since surface creation must be triggered first
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                Stop();
            }
            base.Dispose(disposing);
        }

        public void Stop()
        {
            AssertNotDisposed();
            if (_renderCancellation != null)
            {
                lock (_lockObject)
                {
                    _internalState = InternalState.Exiting;
                }

                _renderCancellation.Cancel();

                if (!RenderOnUIThread)
                    _waitForExitedStateProcessed.Reset();
            }
        }

        FrameEventArgs renderEventArgs = new FrameEventArgs();

        protected void WorkerThreadFrameDispatcher(SynchronizationContext uiThreadSyncContext)
        {
            Threading.ResetThread(Thread.CurrentThread.ManagedThreadId);
            try
            {
                _stopWatch = Stopwatch.StartNew();
                _tick = 0;
                _prevUpdateTime = DateTime.Now;

                while (!_renderCancellation.IsCancellationRequested)
                {
                    // either use UI thread to render one frame or this worker thread
                    bool pauseThread = false;
                    if (RenderOnUIThread)
                    {
                        uiThreadSyncContext.Send((s) =>
                        {
                            pauseThread = RunIteration(_renderCancellation.Token);
                        }, null);
                    }
                    else
                    {
                        pauseThread = RunIteration(_renderCancellation.Token);
                    }


                    if (pauseThread)
                    {
                        _waitForPausedStateProcessed.Set();
                        _waitForMainGameLoop.WaitOne(); // pause this thread
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error("AndroidGameView", ex.ToString());
            }
            finally
            {
                bool c = _renderCancellation.IsCancellationRequested;

                _renderCancellation = null;

                if (_glSurfaceAvailable)
                    DestroyGLSurface();

                if (_glContextAvailable)
                {
                    DestroyGLContext();
                    ContextLostInternal();
                }

                lock (_lockObject)
                {
                    _internalState = InternalState.Exited_GameThread;
                }
            }

        }

        void ProcessStateDefault()
        {
            Log.Error("AndroidGameView", "Default case for switch on InternalState in main game loop, exiting");

            lock (_lockObject)
            {
                _internalState = InternalState.Exited_GameThread;
            }
        }

        void ProcessStateRunning(CancellationToken token)
        {
            // do not run game if surface is not avalible
            lock (_lockObject)
            {
                if (!_androidSurfaceAvailable)
                    return;
            }

            // check if app wants to exit
            if (token.IsCancellationRequested)
            {
                // change state to exit and skip game loop
                lock (_lockObject)
                {
                    _internalState = InternalState.Exiting;
                }

                return;
            }

            try
            {
                UpdateAndRenderFrame();
            }
            catch (MonoGameGLException ex)
            {
                Log.Error("AndroidGameView", "GL Exception occured during RunIteration {0}", ex.Message);
            }

            if (updates > 0)
            {
                var t = updates - (_stopWatch.Elapsed.TotalMilliseconds - _tick);
                if (t > 0)
                {
                    if (LogFPS)
                    {
                        Log.Verbose("AndroidGameView", "took {0:F2}ms, should take {1:F2}ms, sleeping for {2:F2}", _stopWatch.Elapsed.TotalMilliseconds - _tick, updates, t);
                    }

                }
            }

        }

        void ProcessStatePausing()
        {
            if (_glSurfaceAvailable)
            {
                // Surface we are using needs to go away
                DestroyGLSurface();

                if (_loaded)
                    OnUnload();
            }

            // trigger callbacks, must pause openAL device here
            OnGameThreadPause?.Invoke(this);

            // go to next state
            lock (_lockObject)
            {
                _internalState = InternalState.Paused_GameThread;
            }
        }

        void ProcessStateResuming()
        {
            bool isSurfaceAvalible = false;
            lock (_lockObject)
            {
                isSurfaceAvalible = _androidSurfaceAvailable;
            }

            // must sleep outside lock!
            if (!RenderOnUIThread && !isSurfaceAvalible)
            {
                Thread.Sleep(50); // sleep so UI thread easier acquires lock
                return;
            }

            // this can happen if pause is triggered immediately after resume so that SurfaceCreated callback doesn't get called yet,
            // in this case we skip the resume process and pause sets a new state.   
            lock (_lockObject)
            {
                if (!_androidSurfaceAvailable)
                    return;

                // create surface if context is avalible
                if (_glContextAvailable && !_glContextLost)
                {
                    try
                    {
                        CreateGLSurface();
                    }
                    catch (Exception ex)
                    {
                        // We failed to create the surface for some reason
                        Log.Verbose("AndroidGameView", ex.ToString());
                    }
                }

                // create context if not avalible
                if ((!_glContextAvailable || _glContextLost))
                {
                    // Start or Restart due to context loss
                    bool contextLost = false;
                    if (_glContextLost || _glContextAvailable)
                    {
                        // we actually lost the context
                        // so we need to free up our existing 
                        // objects and re-create one.
                        DestroyGLContext();
                        contextLost = true;

                        ContextLostInternal();
                    }

                    CreateGLContext();
                    CreateGLSurface();

                    if (!_loaded && _glContextAvailable)
                        OnLoad();

                    if (contextLost && _glContextAvailable)
                    {
                        // we lost the gl context, we need to let the programmer
                        // know so they can re-create textures etc.
                        ContextSetInternal();
                    }

                }
                else if (_glSurfaceAvailable) // finish state if surface created, may take a frame or two until the android UI thread callbacks fire
                {
                    // trigger callbacks, must resume openAL device here
                    OnGameThreadResume?.Invoke(this);

                    // go to next state
                    _internalState = InternalState.Running_GameThread;
                }
            }
        }

        void ProcessStateExiting()
        {
            // go to next state
            lock (_lockObject)
            {
                _internalState = InternalState.Exited_GameThread;
            }
        }

        void ProcessStateForceSurfaceRecreation()
        {
            // needed at app start
            lock (_lockObject)
            {
                if (!_androidSurfaceAvailable || !_glContextAvailable)
                    return;
            }

            DestroyGLSurface();
            CreateGLSurface();

            // go to next state
            lock (_lockObject)
            {
                _internalState = InternalState.Running_GameThread;
            }
        }

        // Return true to trigger worker thread pause
        bool RunIteration(CancellationToken token)
        {
            // set main game thread global ID
            Threading.ResetThread(Thread.CurrentThread.ManagedThreadId);

            InternalState currentState = InternalState.Exited_GameThread;

            lock (_lockObject)
            {
                if (needToForceRecreateSurface && _internalState == InternalState.Running_GameThread)
                {
                    _internalState = InternalState.ForceRecreateSurface;
                    needToForceRecreateSurface = false;
                }
                currentState = _internalState;
            }

            switch (currentState)
            {
                // exit states
                case InternalState.Exiting: // when ui thread wants to exit
                    ProcessStateExiting();
                    break;

                case InternalState.Exited_GameThread: // when game thread processed exiting event
                    lock (_lockObject)
                    {
                        _waitForExitedStateProcessed.Set();
                        _renderCancellation.Cancel();
                    }
                    break;

                // pause states
                case InternalState.Pausing_UIThread: // when ui thread wants to pause              
                    ProcessStatePausing();
                    break;

                case InternalState.Paused_GameThread: // when game thread processed pausing event

                    // this must be processed outside of this loop, in the new task thread!
                    return true; // trigger pause of worker thread

                // other states
                case InternalState.Resuming_UIThread: // when ui thread wants to resume
                    ProcessStateResuming();

                    // pause must wait for resume in case pause/resume is called in very quick succession
                    lock (_lockObject)
                    {
                        _waitForResumedStateProcessed.Set();
                    }
                    break;

                case InternalState.Running_GameThread: // when we are running game 
                    ProcessStateRunning(token);
                    break;

                case InternalState.ForceRecreateSurface:
                    ProcessStateForceSurfaceRecreation();
                    break;

                // default case, error
                default:
                    ProcessStateDefault();
                    _renderCancellation.Cancel();
                    break;
            }

            return false;
        }

        void UpdateFrameInternal(FrameEventArgs e)
        {
            OnUpdateFrame(e);
            UpdateFrame?.Invoke(this, e);
        }

        protected virtual void OnUpdateFrame(FrameEventArgs e)
        {
        }

        // this method is called on the main thread
        void UpdateAndRenderFrame()
        {
            _curUpdateTime = DateTime.Now;
            if (_prevUpdateTime.Ticks != 0)
            {
                var t = (_curUpdateTime - _prevUpdateTime).TotalMilliseconds;
                _updateEventArgs.Elapsed = t < 0 ? 0 : t;
            }

            try
            {
                UpdateFrameInternal(_updateEventArgs);
            }
            catch (Content.ContentLoadException ex)
            {
                if (RenderOnUIThread)
                    throw ex;
                else
                {
                    AndroidGameActivity.Instance.RunOnUiThread(() =>
                    {
                        throw ex;
                    });
                }
            }

            _prevUpdateTime = _curUpdateTime;

            _curRenderTime = DateTime.Now;
            if (_prevRenderTime.Ticks == 0)
            {
                var t = (_curRenderTime - _prevRenderTime).TotalMilliseconds;
                renderEventArgs.Elapsed = t < 0 ? 0 : t;
            }

            RenderFrameInternal(renderEventArgs);

            _prevRenderTime = _curRenderTime;
        }

        void RenderFrameInternal(FrameEventArgs e)
        {
            if (LogFPS)
                Mark();

            OnRenderFrame(e);
            RenderFrame?.Invoke(this, e);
        }

        protected virtual void OnRenderFrame(FrameEventArgs e)
        {

        }

        void Mark()
        {
            double cur = _stopWatch.Elapsed.TotalMilliseconds;
            if (cur < 2000)
                return;

            _frames++;

            if (cur - _prev >= 995)
            {
                _avgFps = 0.8 * _avgFps + 0.2 * _frames;

                Log.Verbose("AndroidGameView", "frames {0} elapsed {1}ms {2:F2} fps",
                    _frames,
                    cur - _prev,
                    _avgFps);

                _frames = 0;
                _prev = cur;
            }
        }

        protected void AssertNotDisposed()
        {
            if (_disposed)
                throw new ObjectDisposedException("");
        }

        protected void DestroyGLContext()
        {
            if (eglContext != null)
            {
                if (!egl.EglDestroyContext(eglDisplay, eglContext))
                    throw new Exception("Could not destroy EGL context" + GetErrorAsString());
                eglContext = null;
            }
            if (eglDisplay != null)
            {
                if (!egl.EglTerminate(eglDisplay))
                    throw new Exception("Could not terminate EGL connection" + GetErrorAsString());
                eglDisplay = null;
            }

            _glContextAvailable = false;
        }

        protected void DestroyGLSurface()
        {
            if (!(eglSurface == null || eglSurface == EGL10.EglNoSurface))
            {
                if (!egl.EglMakeCurrent(eglDisplay, EGL10.EglNoSurface,
                        EGL10.EglNoSurface, EGL10.EglNoContext))
                {
                    Log.Verbose("AndroidGameView", "Could not unbind EGL surface" + GetErrorAsString());
                }

                if (!egl.EglDestroySurface(eglDisplay, eglSurface))
                {
                    Log.Verbose("AndroidGameView", "Could not destroy EGL surface" + GetErrorAsString());
                }
            }
            eglSurface = null;
            _glSurfaceAvailable = false;

        }

        internal struct SurfaceConfig
        {
            public int Red;
            public int Green;
            public int Blue;
            public int Alpha;
            public int Depth;
            public int Stencil;

            public int[] ToConfigAttribs()
            {
                var attribs = new List<int>();

                if (Red != 0)
                {
                    attribs.Add(EGL11.EglRedSize);
                    attribs.Add(Red);
                }

                if (Green != 0)
                {
                    attribs.Add(EGL11.EglGreenSize);
                    attribs.Add(Green);
                }

                if (Blue != 0)
                {
                    attribs.Add(EGL11.EglBlueSize);
                    attribs.Add(Blue);
                }

                if (Alpha != 0)
                {
                    attribs.Add(EGL11.EglAlphaSize);
                    attribs.Add(Alpha);
                }

                if (Depth != 0)
                {
                    attribs.Add(EGL11.EglDepthSize);
                    attribs.Add(Depth);
                }

                if (Stencil != 0)
                {
                    attribs.Add(EGL11.EglStencilSize);
                    attribs.Add(Stencil);
                }

                attribs.Add(EGL11.EglRenderableType);
                attribs.Add(4);
                attribs.Add(EGL11.EglNone);

                return attribs.ToArray();
            }

            static int GetAttribute(EGLConfig config, IEGL10 egl, EGLDisplay eglDisplay, int attribute)
            {
                int[] data = new int[1];
                egl.EglGetConfigAttrib(eglDisplay, config, attribute, data);
                return data[0];
            }

            public static SurfaceConfig FromEGLConfig(EGLConfig config, IEGL10 egl, EGLDisplay eglDisplay)
            {
                return new SurfaceConfig()
                {
                    Red = GetAttribute(config, egl, eglDisplay, EGL11.EglRedSize),
                    Green = GetAttribute(config, egl, eglDisplay, EGL11.EglGreenSize),
                    Blue = GetAttribute(config, egl, eglDisplay, EGL11.EglBlueSize),
                    Alpha = GetAttribute(config, egl, eglDisplay, EGL11.EglAlphaSize),
                    Depth = GetAttribute(config, egl, eglDisplay, EGL11.EglDepthSize),
                    Stencil = GetAttribute(config, egl, eglDisplay, EGL11.EglStencilSize),
                };
            }

            public override string ToString()
            {
                return string.Format(
                    "Red:{0} Green:{1} Blue:{2} Alpha:{3} Depth:{4} Stencil:{5}",
                    Red, Green, Blue, Alpha, Depth, Stencil);
            }
        }

        protected void CreateGLContext()
        {
            _glContextLost = false;

            egl = EGLContext.EGL.JavaCast<IEGL10>();

            eglDisplay = egl.EglGetDisplay(EGL10.EglDefaultDisplay);
            if (eglDisplay == EGL10.EglNoDisplay)
                throw new Exception("Could not get EGL display" + GetErrorAsString());

            int[] version = new int[2];
            if (!egl.EglInitialize(eglDisplay, version))
                throw new Exception("Could not initialize EGL display" + GetErrorAsString());

            int depth = 0;
            int stencil = 0;
            switch (_game.GraphicsDeviceManager.PreferredDepthStencilFormat)
            {
                case DepthFormat.Depth16:
                    depth = 16;
                    break;
                case DepthFormat.Depth24:
                    depth = 24;
                    break;
                case DepthFormat.Depth24Stencil8:
                    depth = 24;
                    stencil = 8;
                    break;
                case DepthFormat.None:
                    break;
            }

            var configs = new List<SurfaceConfig>();
            if (depth > 0)
            {
                configs.Add(new SurfaceConfig() { Red = 8, Green = 8, Blue = 8, Alpha = 8, Depth = depth, Stencil = stencil });
                configs.Add(new SurfaceConfig() { Red = 5, Green = 6, Blue = 5, Depth = depth, Stencil = stencil });
                configs.Add(new SurfaceConfig() { Depth = depth, Stencil = stencil });
                if (depth > 16)
                {
                    configs.Add(new SurfaceConfig() { Red = 8, Green = 8, Blue = 8, Alpha = 8, Depth = 16 });
                    configs.Add(new SurfaceConfig() { Red = 5, Green = 6, Blue = 5, Depth = 16 });
                    configs.Add(new SurfaceConfig() { Depth = 16 });
                }
                configs.Add(new SurfaceConfig() { Red = 8, Green = 8, Blue = 8, Alpha = 8 });
                configs.Add(new SurfaceConfig() { Red = 5, Green = 6, Blue = 5 });
            }
            else
            {
                configs.Add(new SurfaceConfig() { Red = 8, Green = 8, Blue = 8, Alpha = 8 });
                configs.Add(new SurfaceConfig() { Red = 5, Green = 6, Blue = 5 });
            }
            configs.Add(new SurfaceConfig() { Red = 4, Green = 4, Blue = 4 });
            var numConfigs = new int[1];
            var results = new EGLConfig[1];

            if (!egl.EglGetConfigs(eglDisplay, null, 0, numConfigs))
            {
                throw new Exception("Could not get config count. " + GetErrorAsString());
            }

            var cfgs = new EGLConfig[numConfigs[0]];
            egl.EglGetConfigs(eglDisplay, cfgs, numConfigs[0], numConfigs);
            Log.Verbose("AndroidGameView", "Device Supports");
            foreach (var c in cfgs)
                Log.Verbose("AndroidGameView", string.Format(" {0}", SurfaceConfig.FromEGLConfig(c, egl, eglDisplay)));

            bool found = false;
            numConfigs[0] = 0;
            foreach (var config in configs)
            {
                Log.Verbose("AndroidGameView", string.Format("Checking Config : {0}", config));
                found = egl.EglChooseConfig(eglDisplay, config.ToConfigAttribs(), results, 1, numConfigs);
                Log.Verbose("AndroidGameView", "EglChooseConfig returned {0} and {1}", found, numConfigs[0]);
                if (!found || numConfigs[0] <= 0)
                {
                    Log.Verbose("AndroidGameView", "Config not supported");
                    continue;
                }
                Log.Verbose("AndroidGameView", string.Format("Selected Config : {0}", config));
                break;
            }

            if (!found || numConfigs[0] <= 0)
                throw new Exception("No valid EGL configs found" + GetErrorAsString());

            var createdVersion = new OpenGL.GLESVersion();
            foreach (var v in OpenGL.GLESVersion.GetSupportedGLESVersions())
            {
                Log.Verbose("AndroidGameView", "Creating GLES {0} Context", v);
                eglContext = egl.EglCreateContext(eglDisplay, results[0], EGL10.EglNoContext, v.GetAttributes());
                if (eglContext == null || eglContext == EGL10.EglNoContext)
                {
                    Log.Verbose("AndroidGameView", string.Format("GLES {0} Not Supported. {1}", v, GetErrorAsString()));
                    eglContext = EGL10.EglNoContext;
                    continue;
                }
                createdVersion = v;
                break;
            }
            if (eglContext == null || eglContext == EGL10.EglNoContext)
            {
                eglContext = null;
                throw new Exception("Could not create EGL context" + GetErrorAsString());
            }
            Log.Verbose("AndroidGameView", "Created GLES {0} Context", createdVersion);
            eglConfig = results[0];
            _glContextAvailable = true;
        }

        private string GetErrorAsString()
        {
            switch (egl.EglGetError())
            {
                case EGL10.EglSuccess:
                    return "Success";

                case EGL10.EglNotInitialized:
                    return "Not Initialized";

                case EGL10.EglBadAccess:
                    return "Bad Access";
                case EGL10.EglBadAlloc:
                    return "Bad Allocation";
                case EGL10.EglBadAttribute:
                    return "Bad Attribute";
                case EGL10.EglBadConfig:
                    return "Bad Config";
                case EGL10.EglBadContext:
                    return "Bad Context";
                case EGL10.EglBadCurrentSurface:
                    return "Bad Current Surface";
                case EGL10.EglBadDisplay:
                    return "Bad Display";
                case EGL10.EglBadMatch:
                    return "Bad Match";
                case EGL10.EglBadNativePixmap:
                    return "Bad Native Pixmap";
                case EGL10.EglBadNativeWindow:
                    return "Bad Native Window";
                case EGL10.EglBadParameter:
                    return "Bad Parameter";
                case EGL10.EglBadSurface:
                    return "Bad Surface";

                default:
                    return "Unknown Error";
            }
        }

        protected void CreateGLSurface()
        {
            if (_glSurfaceAvailable)
                return;

            try
            {
                // If there is an existing surface, destroy the old one
                DestroyGLSurface();

                eglSurface = egl.EglCreateWindowSurface(eglDisplay, eglConfig, (Java.Lang.Object)Holder, null);
                if (eglSurface == null || eglSurface == EGL10.EglNoSurface)
                    throw new Exception("Could not create EGL window surface" + GetErrorAsString());

                if (!egl.EglMakeCurrent(eglDisplay, eglSurface, eglSurface, eglContext))
                    throw new Exception("Could not make EGL current" + GetErrorAsString());

                _glSurfaceAvailable = true;

                // Must set viewport after creation, the viewport has correct values in it already as we call it, but
                // the surface is created after the correct viewport is already applied so we must do it again.
                if (_game.GraphicsDevice != null)
                    _game.GraphicsDeviceManager.ResetClientBounds();

                if (OpenGL.GL.GetError == null)
                    OpenGL.GL.LoadEntryPoints();
            }
            catch (Exception ex)
            {
                Log.Error("AndroidGameView", ex.ToString());
                _glSurfaceAvailable = false;
            }
        }

        protected EGLSurface CreatePBufferSurface(EGLConfig config, int[] attribList)
        {
            IEGL10 egl = EGLContext.EGL.JavaCast<IEGL10>();
            EGLSurface result = egl.EglCreatePbufferSurface(eglDisplay, config, attribList);
            if (result == null || result == EGL10.EglNoSurface)
                throw new Exception("EglCreatePbufferSurface");
            return result;
        }

        protected void ContextSetInternal()
        {
            if (_glContextLost)
            {
                if (_game.GraphicsDevice != null)
                {
                    _game.GraphicsDevice.Initialize();

                    IsResuming = true;
                    if (_gameWindow.Resumer != null)
                        _gameWindow.Resumer.LoadContent();

                    // Reload textures on a different thread so the resumer can be drawn
                    var bgThread = new Thread(o =>
                    {
                        Log.Debug("MonoGame", "Begin reloading graphics content");
                        Content.ContentManager.ReloadGraphicsContent();
                        Log.Debug("MonoGame", "End reloading graphics content");

                        // DeviceReset events
                        _game.GraphicsDeviceManager.OnDeviceReset();
                        _game.GraphicsDevice.OnDeviceReset();

                        IsResuming = false;
                    });

                    bgThread.Start();
                }
            }
            OnContextSet();
        }

        protected void ContextLostInternal()
        {
            OnContextLost();
            _game.GraphicsDeviceManager.OnDeviceResetting();
            if (_game.GraphicsDevice != null)
                _game.GraphicsDevice.OnDeviceResetting();
        }

        protected virtual void OnContextLost()
        {

        }

        protected virtual void OnContextSet()
        {

        }

        protected virtual void OnUnload()
        {

        }

        protected virtual void OnLoad()
        {

        }

        protected virtual void OnStopped()
        {

        }

        #region Key and Motion

        public override bool OnKeyDown(Keycode keyCode, KeyEvent e)
        {
            bool handled = false;
            if (GamePad.OnKeyDown(keyCode, e))
                return true;

            handled = Keyboard.KeyDown(keyCode);

#if !OUYA
            // we need to handle the Back key here because it doesnt work any other way
            if (keyCode == Keycode.Back)
            {
                GamePad.Back = true;
                handled = true;
            }
#endif
            if (keyCode == Keycode.VolumeUp)
            {
                AudioManager audioManager = (AudioManager)Context.GetSystemService(Context.AudioService);
                audioManager.AdjustStreamVolume(Stream.Music, Adjust.Raise, VolumeNotificationFlags.ShowUi);
                return true;
            }

            if (keyCode == Keycode.VolumeDown)
            {
                AudioManager audioManager = (AudioManager)Context.GetSystemService(Context.AudioService);
                audioManager.AdjustStreamVolume(Stream.Music, Adjust.Lower, VolumeNotificationFlags.ShowUi);
                return true;
            }

            return handled;
        }

        public override bool OnKeyUp(Keycode keyCode, KeyEvent e)
        {
            if (keyCode == Keycode.Back)
                GamePad.Back = false;

            if (GamePad.OnKeyUp(keyCode, e))
                return true;

            return Keyboard.KeyUp(keyCode);
        }

        public override bool OnGenericMotionEvent(MotionEvent e)
        {
            if (GamePad.OnGenericMotionEvent(e))
                return true;

            return base.OnGenericMotionEvent(e);
        }

        #endregion

        #region Properties

        private IEGL10 egl;
        private EGLDisplay eglDisplay;
        private EGLConfig eglConfig;
        private EGLContext eglContext;
        private EGLSurface eglSurface;

        /// <summary>The visibility of the window. Always returns true.</summary>
        /// <value></value>
        /// <exception cref="T:System.ObjectDisposed">The instance has been disposed</exception>
        public virtual bool Visible
        {
            get
            {
                AssertNotDisposed();
                return true;
            }
            set
            {
                AssertNotDisposed();
            }
        }

        /// <summary>The size of the current view.</summary>
        /// <value>A <see cref="Size" /> which is the size of the current view.</value>
        /// <exception cref="ObjectDisposedException">The instance has been disposed.</exception>
        public virtual Size Size
        {
            get
            {
                AssertNotDisposed();
                return _surfaceSize;
            }
            set
            {
                AssertNotDisposed();
                _surfaceSize = value;
            }
        }

        #endregion

        public BackgroundContext CreateBackgroundContext()
        {
            return new BackgroundContext(this);
        }
    }
}