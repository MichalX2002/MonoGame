// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;

namespace Microsoft.Xna.Framework
{
    public class DrawableGameComponent : GameComponent, IDrawable
    {
        private bool _initialized;
        private int _drawOrder;
        private bool _visible = true;

        public Graphics.GraphicsDevice GraphicsDevice => Game.GraphicsDevice;

        public int DrawOrder
        {
            get => _drawOrder;
            set
            {
                if (_drawOrder != value)
                {
                    _drawOrder = value;
                    OnDrawOrderChanged();
                }
            }
        }

        public bool Visible
        {
            get => _visible;
            set
            {
                if (_visible != value)
                {
                    _visible = value;
                    OnVisibleChanged();
                }
            }
        }

        public event SenderDelegate<object> DrawOrderChanged;
        public event SenderDelegate<object> VisibleChanged;

        public DrawableGameComponent(Game game) : base(game)
        {
        }

        public override void Initialize()
        {
            if (!_initialized)
            {
                _initialized = true;
                LoadContent();
            }
        }

        protected virtual void LoadContent() { }

        protected virtual void UnloadContent () { }

        public virtual void Draw(GameTime gameTime) { }

        protected virtual void OnVisibleChanged()
        {
            VisibleChanged?.Invoke(this);
        }

        protected virtual void OnDrawOrderChanged()
        {
            DrawOrderChanged?.Invoke(this);
        }
    }
}
