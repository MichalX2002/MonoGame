// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.Collections.ObjectModel;

namespace Microsoft.Xna.Framework
{
    public sealed class GameComponentCollection : Collection<IGameComponent>
    {
        /// <summary>
        /// Event that is triggered when a <see cref="GameComponent"/> is added
        /// to this <see cref="GameComponentCollection"/>.
        /// </summary>
        public event DataEvent<GameComponentCollection, GameComponentCollectionEvent> ComponentAdded;

        /// <summary>
        /// Event that is triggered when a <see cref="GameComponent"/> is removed
        /// from this <see cref="GameComponentCollection"/>.
        /// </summary>
        public event DataEvent<GameComponentCollection, GameComponentCollectionEvent> ComponentRemoved;

        /// <summary>
        /// Removes every <see cref="GameComponent"/> from this <see cref="GameComponentCollection"/>.
        /// Triggers <see cref="OnComponentRemoved"/> once for each <see cref="GameComponent"/> removed.
        /// </summary>
        protected override void ClearItems()
        {
            for (int i = 0; i < base.Count; i++)
                this.OnComponentRemoved(new GameComponentCollectionEvent(base[i]));

            base.ClearItems();
        }

        protected override void InsertItem(int index, IGameComponent item)
        {
            if (base.IndexOf(item) != -1)
                throw new ArgumentException("Cannot add same component multiple times.");

            base.InsertItem(index, item);
            if (item != null)
                this.OnComponentAdded(new GameComponentCollectionEvent(item));
        }

        private void OnComponentAdded(GameComponentCollectionEvent data)
        {
            ComponentAdded?.Invoke(this, data);
        }

        private void OnComponentRemoved(GameComponentCollectionEvent data)
        {
            ComponentRemoved?.Invoke(this, data);
        }

        protected override void RemoveItem(int index)
        {
            IGameComponent gameComponent = base[index];
            base.RemoveItem(index);
            if (gameComponent != null)
                this.OnComponentRemoved(new GameComponentCollectionEvent(gameComponent));
        }

        protected override void SetItem(int index, IGameComponent item)
        {
            throw new NotSupportedException();
        }
    }
}

