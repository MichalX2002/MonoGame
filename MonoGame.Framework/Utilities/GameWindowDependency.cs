using System;

namespace MonoGame.Framework.Utilities
{
    public abstract class GameWindowDependency : IDisposable
    {
        /// <summary>
        /// Gets whether this dependency is disposed.
        /// </summary>
        public bool IsDisposed { get; private set; }

        /// <summary>
        /// Gets the window attached to this dependency.
        /// </summary>
        public GameWindow Window { get; }

        protected GameWindowDependency(GameWindow window)
        {
            Window = window ?? throw new ArgumentNullException(nameof(window));

            Window.WindowHandleChanged += Window_WindowHandleChanged;
        }

        private void Window_WindowHandleChanged(GameWindow sender)
        {
            WindowHandleChanged();
        }

        /// <summary>
        /// Called whenever <see cref="GameWindow.WindowHandleChanged"/> is 
        /// invoked on the attached window.
        /// </summary>
        protected abstract void WindowHandleChanged();

        protected virtual void Dispose(bool disposing)
        {
            if (!IsDisposed)
            {
                if (disposing)
                {
                    Window.WindowHandleChanged -= Window_WindowHandleChanged;
                }

                IsDisposed = true;
            }
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}