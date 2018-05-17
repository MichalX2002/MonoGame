// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;

namespace Microsoft.Xna.Framework
{
    public class GameComponentCollectionEventArgs : EventArgs
    {
        public GameComponentCollectionEventArgs(IGameComponent gameComponent)
        {
            GameComponent = gameComponent;
        }

        public IGameComponent GameComponent { get; }
    }
}

