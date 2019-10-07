// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.Diagnostics;

namespace MonoGame.Framework.Graphics
{
    public abstract class GraphicsResource : IDisposable
    {
        // The GraphicsDevice property should only be accessed in Dispose(bool) if the disposing
        // parameter is true. If disposing is false, the GraphicsDevice may or may not be
        // disposed yet.
        private GraphicsDevice _graphicsDevice;

        private WeakReference _selfReference;

        public bool IsDisposed { get; private set; }
        public string Name { get; set; }
        public object Tag { get; set; }

        public event SimpleEventHandler<GraphicsResource> Disposing;

        public GraphicsDevice GraphicsDevice
        {
            get => _graphicsDevice;
            internal set
            {
                Debug.Assert(value != null);

                if (_graphicsDevice == value)
                    return;

                // VertexDeclaration objects can be bound to multiple GraphicsDevice objects
                // during their lifetime. But only one GraphicsDevice should retain ownership.
                if (_graphicsDevice != null)
                {
                    _graphicsDevice.RemoveResourceReference(_selfReference);
                    _selfReference = null;
                }
                _graphicsDevice = value;

                _selfReference = new WeakReference(this);
                _graphicsDevice.AddResourceReference(_selfReference);
            }
        }

        internal GraphicsResource()
        {
        }

        /// <summary>
        /// Called before the device is reset. Allows graphics resources to 
        /// invalidate their state so they can be recreated after the device reset.
        /// Warning: This may be called after a call to Dispose() up until
        /// the resource is garbage collected.
        /// </summary>
        internal protected virtual void GraphicsDeviceResetting()
        {

        }

        [DebuggerHidden]
        protected static void AssertIsOnUIThreadForSpan()
        {
            if (!Threading.IsOnUIThread())
                throw new NotSupportedException(
                    "This method (which utilizes Span<T>) can only be called on the UI thread.");
        }

        public override string ToString()
        {
            return string.IsNullOrEmpty(Name) ? base.ToString() : Name;
        }

        public void Dispose()
        {
            // Dispose of managed objects as well
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// The method that derived classes should override to implement disposing of managed and native resources.
        /// </summary>
        /// <param name="disposing">True if managed objects should be disposed.</param>
        /// <remarks>Native resources should always be released regardless of the value of the disposing parameter.</remarks>
        protected virtual void Dispose(bool disposing)
        {
            if (!IsDisposed)
            {
                if (disposing)
                {
                    // Release managed objects
                    // ...
                }

                // Release native objects
                // ...

                // Do not trigger the event if called from the finalizer
                if (disposing)
                    Disposing?.Invoke(this);

                // Remove from the global list of graphics resources
                if (_graphicsDevice != null)
                    _graphicsDevice.RemoveResourceReference(_selfReference);

                _selfReference = null;
                _graphicsDevice = null;
                IsDisposed = true;
            }
        }

        ~GraphicsResource()
        {
            // Pass false so the managed objects are not released
            Dispose(false);
        }
    }
}

