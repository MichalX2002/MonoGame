// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;

namespace MonoGame.Framework.Graphics
{
    public partial class SamplerState : GraphicsResource
    {
        public static SamplerState AnisotropicClamp { get; }
        public static SamplerState AnisotropicWrap { get; }
        public static SamplerState LinearClamp { get; }
        public static SamplerState LinearWrap { get; }
        public static SamplerState PointClamp { get; }
        public static SamplerState PointWrap { get; }

        private readonly bool _defaultStateObject;

        private TextureAddressMode _addressU;
        private TextureAddressMode _addressV;
        private TextureAddressMode _addressW;
        private Color _borderColor;
        private TextureFilter _filter;
        private int _maxAnisotropy;
        private int _maxMipLevel;
        private float _mipMapLevelOfDetailBias;
        private TextureFilterMode _filterMode;
        private CompareFunction _comparisonFunction;

        #region Properties

        public TextureAddressMode AddressU
        {
            get => _addressU;
            set
            {
                ThrowIfBound();
                _addressU = value;
            }
        }

        public TextureAddressMode AddressV
        {
            get => _addressV;
            set
            {
                ThrowIfBound();
                _addressV = value;
            }
        }

        public TextureAddressMode AddressW
        {
            get => _addressW;
            set
            {
                ThrowIfBound();
                _addressW = value;
            }
        }

        public Color BorderColor
        {
            get => _borderColor;
            set
            {
                ThrowIfBound();
                _borderColor = value;
            }
        }

        public TextureFilter Filter
        {
            get => _filter;
            set
            {
                ThrowIfBound();
                _filter = value;
            }
        }

        public int MaxAnisotropy
        {
            get => _maxAnisotropy;
            set
            {
                ThrowIfBound();
                _maxAnisotropy = value;
            }
        }

        public int MaxMipLevel
        {
            get => _maxMipLevel;
            set
            {
                ThrowIfBound();
                _maxMipLevel = value;
            }
        }

        public float MipMapLevelOfDetailBias
        {
            get => _mipMapLevelOfDetailBias;
            set
            {
                ThrowIfBound();
                _mipMapLevelOfDetailBias = value;
            }
        }

        /// <summary>
        /// When using comparison sampling, also set <see cref="FilterMode"/> to <see cref="TextureFilterMode.Comparison"/>.
        /// </summary>
        public CompareFunction ComparisonFunction
        {
            get => _comparisonFunction;
            set
            {
                ThrowIfBound();
                _comparisonFunction = value;
            }
        }

        public TextureFilterMode FilterMode
        {
            get => _filterMode;
            set
            {
                ThrowIfBound();
                _filterMode = value;
            }
        }

        #endregion

        #region Constructors

        static SamplerState()
        {
            AnisotropicClamp = new SamplerState("SamplerState.AnisotropicClamp", TextureFilter.Anisotropic, TextureAddressMode.Clamp);
            AnisotropicWrap = new SamplerState("SamplerState.AnisotropicWrap", TextureFilter.Anisotropic, TextureAddressMode.Wrap);
            LinearClamp = new SamplerState("SamplerState.LinearClamp", TextureFilter.Linear, TextureAddressMode.Clamp);
            LinearWrap = new SamplerState("SamplerState.LinearWrap", TextureFilter.Linear, TextureAddressMode.Wrap);
            PointClamp = new SamplerState("SamplerState.PointClamp", TextureFilter.Point, TextureAddressMode.Clamp);
            PointWrap = new SamplerState("SamplerState.PointWrap", TextureFilter.Point, TextureAddressMode.Wrap);
        }

        public SamplerState()
        {
            Filter = TextureFilter.Linear;
            AddressU = TextureAddressMode.Wrap;
            AddressV = TextureAddressMode.Wrap;
            AddressW = TextureAddressMode.Wrap;
            BorderColor = Color.White;
            MaxAnisotropy = 4;
            MaxMipLevel = 0;
            MipMapLevelOfDetailBias = 0f;
            ComparisonFunction = CompareFunction.Never;
            FilterMode = TextureFilterMode.Default;
        }

        private SamplerState(string name, TextureFilter filter, TextureAddressMode addressMode) : this()
        {
            Name = name;
            _filter = filter;
            _addressU = addressMode;
            _addressV = addressMode;
            _addressW = addressMode;
            _defaultStateObject = true;
        }

        private SamplerState(SamplerState cloneSource)
        {
            Name = cloneSource.Name;
            _filter = cloneSource._filter;
            _addressU = cloneSource._addressU;
            _addressV = cloneSource._addressV;
            _addressW = cloneSource._addressW;
            _borderColor = cloneSource._borderColor;
            _maxAnisotropy = cloneSource._maxAnisotropy;
            _maxMipLevel = cloneSource._maxMipLevel;
            _mipMapLevelOfDetailBias = cloneSource._mipMapLevelOfDetailBias;
            _comparisonFunction = cloneSource._comparisonFunction;
            _filterMode = cloneSource._filterMode;
        }

#endregion

        internal void BindToGraphicsDevice(GraphicsDevice device)
        {
            if (_defaultStateObject)
                throw new InvalidOperationException(
                    "You cannot bind a default state object.");

            if (GraphicsDevice != null && GraphicsDevice != device)
                throw new InvalidOperationException(
                    "This sampler state is already bound to a different graphics device.");

            GraphicsDevice = device;
        }

        internal void ThrowIfBound()
        {
            if (_defaultStateObject)
                throw new InvalidOperationException(
                    "You cannot modify a default sampler state object.");

            if (GraphicsDevice != null)
                throw new InvalidOperationException(
                    "You cannot modify the sampler state after it has been bound to the graphics device!");
        }

        internal SamplerState Clone()
        {
            return new SamplerState(this);
        }

        partial void PlatformDispose();

        protected override void Dispose(bool disposing)
        {
            if (!IsDisposed)
            {
                PlatformDispose();
            }
            base.Dispose(disposing);
        }
    }
}