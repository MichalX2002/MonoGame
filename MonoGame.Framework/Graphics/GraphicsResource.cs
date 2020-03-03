// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.Diagnostics;

namespace MonoGame.Framework.Graphics
{
    public abstract class GraphicsResource : IDisposable
    {
        /// <summary>
        /// This field should only be accessed in <see cref="Dispose(bool)"/> if the disposing
        /// parameter is true. If disposing is false, this field may or may not be disposed yet.
        /// </summary>
        private GraphicsDevice _graphicsDevice;

        private WeakReference _selfReference;

        /// <summary>
        /// Gets whether the <see cref="GraphicsResource"/> is disposed.
        /// </summary>
        public bool IsDisposed { get; private set; }

        /// <summary>
        /// Gets or sets the name of this <see cref="GraphicsResource"/>.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the tag of this <see cref="GraphicsResource"/>.
        /// </summary>
        public object Tag { get; set; }

        /// <summary>
        /// Occurs when the <see cref="GraphicsResource"/> is disposed.
        /// </summary>
        public event DatalessEvent<GraphicsResource> Disposing;

        /// <summary>
        /// Gets the <see cref="Graphics.GraphicsDevice"/> assigned to this <see cref="GraphicsResource"/>.
        /// </summary>
        public GraphicsDevice GraphicsDevice
        {
            get => _graphicsDevice;
            internal set
            {
                if (value == null)
                    throw new ArgumentNullException(nameof(value));

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

        internal GraphicsResource(GraphicsDevice graphicsDevice = null)
        {
            GraphicsDevice = graphicsDevice ?? throw new ArgumentNullException(
                nameof(graphicsDevice), FrameworkResources.ResourceCreationWithNullDevice);
        }

        internal GraphicsResource()
        {
        }

        internal void InvokeGraphicsDeviceResetting()
        {
            GraphicsDeviceResetting();
        }

        /// <summary>
        /// Called before the device is reset. Allows graphics resources to 
        /// invalidate their state so they can be recreated after the device resets.
        /// <para>
        /// This may be called after <see cref="Dispose()"/> up until the resource is garbage collected.
        /// </para>
        /// </summary>
        protected virtual void GraphicsDeviceResetting()
        {
        }

        /// <summary>
        /// Throws an exception if the caller is not running on the main thread.
        /// </summary>
        /// <exception cref="InvalidOperationException">
        /// The caller is not running on the main thread.
        /// </exception>
        [DebuggerHidden]
        protected static void AssertOnMainThreadForSpan()
        {
            if (!Threading.IsOnMainThread)
                throw new AsyncResourceNotSupportedException(
                    FrameworkResources.AsyncResourceNotSupported);
        }

        /// <summary>
        /// Returns the string representation of this <see cref="GraphicsResource"/>.
        /// </summary>
        public override string ToString()
        {
            return string.IsNullOrEmpty(Name) ? base.ToString() : Name;
        }

        /// <summary>
        /// The method that derived classes should override to implement disposing of managed and native resources.
        /// </summary>
        /// <param name="disposing">True if managed objects should be disposed.</param>
        /// <remarks>
        /// Unmanaged resources should always be released regardless of the value of the disposing parameter.
        /// </remarks>
        protected virtual void Dispose(bool disposing)
        {
            if (!IsDisposed)
            {
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

        /// <summary>
        /// Releases resources used by the <see cref="GraphicsResource"/>.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Disposes the <see cref="GraphicsResource"/>.
        /// </summary>
        ~GraphicsResource()
        {
            Dispose(false);
        }
    }
}

