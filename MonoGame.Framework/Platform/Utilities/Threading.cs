// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.Collections.Generic;
using System.Threading;

#if IOS
using Foundation;
using OpenGLES;
#endif
#if DESKTOPGL || ANGLE || GLES
using MonoGame.OpenGL;
#endif

namespace MonoGame.Framework
{
    internal class Threading
    {
        public const int MaxWaitForMainThread = 10000; // In milliseconds

        private static int _mainThreadId;

#if ANDROID || WINDOWS || DESKTOPGL || ANGLE
        private static List<Action> actions = new List<Action>();
#elif IOS
        public static EAGLContext BackgroundContext;
#endif

        static Threading()
        {
            _mainThreadId = Thread.CurrentThread.ManagedThreadId;
        }
#if ANDROID
        internal static void ResetThread (int id)
        {
            _mainThreadId = id;
        }
#endif
        /// <summary>
        /// Gets whether the caller is running on the main thread.
        /// </summary>
        /// <returns><see langword="true"/> if the caller is running on the main thread.</returns>
        public static bool IsOnMainThread => _mainThreadId == Thread.CurrentThread.ManagedThreadId;

        /// <summary>
        /// Throws an exception if the caller is not running on the main thread.
        /// </summary>
        /// <exception cref="InvalidOperationException">
        /// The caller is not running on the main thread.
        /// </exception>
        public static void EnsureMainThread()
        {
            if (!IsOnMainThread)
                throw new InvalidOperationException("Method was not called on the main thread.");
        }

        /// <summary>
        /// Runs the given action on the main thread and blocks the current thread while the action is running.
        /// If the current thread is the main thread, the action will run immediately.
        /// </summary>
        /// <param name="action">The action to be run on the main thread</param>
        internal static void BlockOnMainThread(Action action)
        {
            if (action == null)
                throw new ArgumentNullException(nameof(action));

#if DIRECTX || PSM
            action();
#else
            // If we are already on the main thread, just call the action and be done with it
            if (IsOnMainThread)
            {
                action.Invoke();
                return;
            }

#if IOS
            lock (BackgroundContext)
            {
                // Make the context current on this thread if it is not already
                if (!Object.ReferenceEquals(EAGLContext.CurrentContext, BackgroundContext))
                    EAGLContext.SetCurrentContext(BackgroundContext);
                // Execute the action
                action();
                // Must flush the GL calls so the GPU asset is ready for the main context to use it
                GL.Flush();
                GraphicsExtensions.CheckGLError();
            }
#else
            var resetEvent = new ManualResetEventSlim(false);
            Add(() =>
            {
#if ANDROID
                //if (!Game.Instance.Window.GraphicsContext.IsCurrent)
                    ((AndroidGameWindow)Game.Instance.Window).GameView.MakeCurrent();
#endif
                action.Invoke();
                resetEvent.Set();
            });
            if (!resetEvent.Wait(MaxWaitForMainThread))
                throw new TimeoutException();
#endif // IOS
#endif // DIRECTX ||PSM
        }

#if ANDROID || WINDOWS || DESKTOPGL || ANGLE
        private static void Add(Action action)
        {
            lock (actions)
            {
                actions.Add(action);
            }
        }

        /// <summary>
        /// Runs all pending actions. Must be called from the main thread.
        /// </summary>
        internal static void Run()
        {
            EnsureMainThread();

            lock (actions)
            {
                foreach (Action action in actions)
                    action.Invoke();
                actions.Clear();
            }
        }
#endif
    }
}
