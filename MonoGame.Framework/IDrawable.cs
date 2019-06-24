// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

namespace Microsoft.Xna.Framework
{
    public interface IDrawable
    {
		event SenderEvent<object> DrawOrderChanged;
        event SenderEvent<object> VisibleChanged;

        int DrawOrder { get; }
        bool Visible { get; }

        void Draw(GameTime gameTime);
    }
}

