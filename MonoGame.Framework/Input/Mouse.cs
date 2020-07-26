// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using MonoGame.Framework.Utilities;

namespace MonoGame.Framework.Input
{
    /// <summary>
    /// Allows reading position and button click information from mouse.
    /// </summary>
    public partial class Mouse : GameWindowDependency
    {
        /// <summary>
        /// Constructs the <see cref="Mouse"/>.
        /// </summary>
        /// <param name="window"></param>
        public Mouse(GameWindow window) : base(window)
        {
        }

        /// <summary>
        /// Gets mouse state information that includes position and button
        /// presses for the bound window.
        /// </summary>
        /// <returns>Snapshot of current mouse state.</returns>
        public MouseState GetState()
        {
            return PlatformGetState();
        }

        /// <summary>
        /// Sets the mouse cursor's position relative to the window.
        /// </summary>
        /// <param name="x">Relative horizontal position of the cursor.</param>
        /// <param name="y">Relative vertical position of the cursor.</param>
        public void SetPosition(int x, int y)
        {
            PlatformSetPosition(x, y);
        }

        /// <summary>
        /// Sets the cursor image to the specified <see cref="MouseCursor"/>.
        /// </summary>
        /// <param name="cursor">Mouse cursor to use for the cursor image.</param>
        public void SetCursor(MouseCursor cursor)
        {
            if (cursor == null)
                throw new ArgumentNullException(nameof(cursor));
            
            PlatformSetCursor(cursor);
        }
    }
}
