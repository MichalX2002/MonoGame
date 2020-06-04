// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

namespace MonoGame.Framework
{
    public interface IDrawable
    {
		event Event<object> DrawOrderChanged;
        event Event<object> VisibleChanged;

        int DrawOrder { get; }
        bool Visible { get; }

        void Draw(GameTime gameTime);
    }
}

