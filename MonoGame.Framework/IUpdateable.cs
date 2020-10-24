// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

namespace MonoGame.Framework
{
    /// <summary>
    /// Interface for updateable entities.
    /// </summary>
	public interface IUpdateable
	{
	    /// <summary>
	    /// Called when this <see cref="IUpdateable"/> should update itself.
	    /// </summary>
	    /// <param name="gameTime">The elapsed time since the last call to <see cref="Update"/>.</param>
		void Update(GameTime gameTime);

        /// <summary>
        /// Raised when <see cref="Enabled"/> changed.
        /// </summary>
		event EventHandler<EventArgs> EnabledChanged;
		
        /// <summary>
        /// Raised when <see cref="UpdateOrder"/> changed.
        /// </summary>
		event EventHandler<EventArgs> UpdateOrderChanged;

        /// <summary>
        /// Indicates if <see cref="Update"/> will be called.
        /// </summary>
		bool Enabled { get; }
		
        /// <summary>
        /// The update order of this <see cref="GameComponent"/> relative
        /// to other <see cref="GameComponent"/> instances.
        /// </summary>
		int UpdateOrder { get; }
	}
}
