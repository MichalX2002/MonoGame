// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;

namespace MonoGame.Framework
{   
    public class GameComponent : IGameComponent, IUpdateable, IComparable<GameComponent>, IDisposable
    {
        bool _enabled = true;
        int _updateOrder;

        public Game Game { get; private set; }

        public bool Enabled
        {
            get => _enabled;
            set
            {
                if (_enabled != value)
                {
                    _enabled = value;
                    OnEnabledChanged();
                }
            }
        }

        public int UpdateOrder
        {
            get => _updateOrder;
            set
            {
                if (_updateOrder != value)
                {
                    _updateOrder = value;
                    OnUpdateOrderChanged();
                }
            }
        }

        public event SenderDelegate<object> EnabledChanged;
        public event SenderDelegate<object> UpdateOrderChanged;

        public GameComponent(Game game)
        {
            Game = game;
        }

        ~GameComponent()
        {
            Dispose(false);
        }

        public virtual void Initialize() { }

        public virtual void Update(GameTime gameTime) { }

        protected virtual void OnUpdateOrderChanged()
        {
            UpdateOrderChanged?.Invoke(this);
        }

        protected virtual void OnEnabledChanged()
        {
            EnabledChanged?.Invoke(this);
        }

        /// <summary>
        /// Shuts down the component.
        /// </summary>
        protected virtual void Dispose(bool disposing) { }
        
        /// <summary>
        /// Shuts down the component.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #region IComparable<GameComponent> Members
        // TODO: Should be removed, as it is not part of XNA 4.0
        public int CompareTo(GameComponent other)
        {
            return other.UpdateOrder - UpdateOrder;
        }

        #endregion
    }
}
