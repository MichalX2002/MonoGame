// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

namespace MonoGame.Framework.Graphics
{
    public partial class TargetBlendState
    {
        private readonly BlendState _parent;
        private BlendFunction _alphaBlendFunction;
        private Blend _alphaDestinationBlend;
        private Blend _alphaSourceBlend;
        private BlendFunction _colorBlendFunction;
        private Blend _colorDestinationBlend;
        private Blend _colorSourceBlend;
        private ColorWriteChannels _colorWriteChannels;

        internal TargetBlendState(BlendState parent)
        {
            _parent = parent;
            AlphaBlendFunction = BlendFunction.Add;
            AlphaDestinationBlend = Blend.Zero;
            AlphaSourceBlend = Blend.One;
            ColorBlendFunction = BlendFunction.Add;
            ColorDestinationBlend = Blend.Zero;
            ColorSourceBlend = Blend.One;
            ColorWriteChannels = ColorWriteChannels.All;
        }

        internal TargetBlendState Clone(BlendState parent)
        {
            return new TargetBlendState(parent)
            {
                AlphaBlendFunction = AlphaBlendFunction,
                AlphaDestinationBlend = AlphaDestinationBlend,
                AlphaSourceBlend = AlphaSourceBlend,
                ColorBlendFunction = ColorBlendFunction,
                ColorDestinationBlend = ColorDestinationBlend,
                ColorSourceBlend = ColorSourceBlend,
                ColorWriteChannels = ColorWriteChannels
            };
        }

        public BlendFunction AlphaBlendFunction
        {
            get => _alphaBlendFunction;
            set
            {
                _parent.ThrowIfBound();
                _alphaBlendFunction = value;
            }
        }

        public Blend AlphaDestinationBlend
        {
            get => _alphaDestinationBlend;
            set
            {
                _parent.ThrowIfBound();
                _alphaDestinationBlend = value;
            }
        }

        public Blend AlphaSourceBlend
        {
            get => _alphaSourceBlend;
            set
            {
                _parent.ThrowIfBound();
                _alphaSourceBlend = value;
            }
        }

        public BlendFunction ColorBlendFunction
        {
            get => _colorBlendFunction;
            set
            {
                _parent.ThrowIfBound();
                _colorBlendFunction = value;
            }
        }

        public Blend ColorDestinationBlend
        {
            get => _colorDestinationBlend;
            set
            {
                _parent.ThrowIfBound();
                _colorDestinationBlend = value;
            }
        }

        public Blend ColorSourceBlend
        {
            get => _colorSourceBlend;
            set
            {
                _parent.ThrowIfBound();
                _colorSourceBlend = value;
            }
        }

        public ColorWriteChannels ColorWriteChannels
        {
            get => _colorWriteChannels;
            set
            {
                _parent.ThrowIfBound();
                _colorWriteChannels = value;
            }
        }
    }
}

