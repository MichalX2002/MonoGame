using System;

namespace MonoGame.Framework.Utilities
{
    public abstract class GameWindowDependency
    {
        /// <summary>
        /// Gets the window attached to this dependency.
        /// </summary>
        public GameWindow Window { get; }

        /// <summary>
        /// Attaches this dependency to a window.
        /// </summary>
        /// <param name="window">The window to attach to.</param>
        public GameWindowDependency(GameWindow window)
        {
            Window = window ?? throw new ArgumentNullException(nameof(window));

            Window.WindowHandleChanged += Window_WindowHandleChanged;
            Window.Disposing += Window_Disposing;
        }

        private void Window_WindowHandleChanged(GameWindow sender)
        {
            WindowHandleChanged();
        }

        private void Window_Disposing(GameWindow sender)
        {
            Disposing();
        }

        /// <summary>
        /// Called whenever <see cref="GameWindow.WindowHandleChanged"/> is 
        /// invoked on the attached window.
        /// </summary>
        protected virtual void WindowHandleChanged()
        {
        }

        /// <summary>
        /// Called whenever <see cref="GameWindow.Disposing"/> is 
        /// invoked on the attached window.
        /// </summary>
        protected virtual void Disposing()
        {
            Window.WindowHandleChanged -= Window_WindowHandleChanged;
            Window.Disposing -= Window_Disposing;
        }
    }
}