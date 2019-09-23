// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

namespace MonoGame.Framework
{
	public interface IUpdateable
	{
		event SimpleEventHandler<object> EnabledChanged;
		event SimpleEventHandler<object> UpdateOrderChanged;
	
		bool Enabled { get; }
		int UpdateOrder { get; }

		void Update(GameTime gameTime);
	}
}
