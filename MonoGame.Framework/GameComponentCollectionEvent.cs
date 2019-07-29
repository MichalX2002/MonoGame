// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

namespace MonoGame.Framework
{
    public readonly struct GameComponentCollectionEvent
    {
        public IGameComponent GameComponent { get; }

        public GameComponentCollectionEvent(IGameComponent gameComponent)
        {
            GameComponent = gameComponent;
        }
    }
}

