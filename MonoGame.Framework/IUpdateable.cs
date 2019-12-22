// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

namespace MonoGame.Framework
{
	public interface IUpdateable
	{
		event DataEvent<object> EnabledChanged;
		event DataEvent<object> UpdateOrderChanged;
	
		bool Enabled { get; }
		int UpdateOrder { get; }

		void Update(GameTime gameTime);
	}
}
