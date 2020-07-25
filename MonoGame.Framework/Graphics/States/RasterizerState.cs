// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;

namespace MonoGame.Framework.Graphics
{
    public partial class RasterizerState : GraphicsResource
    {
        public static RasterizerState CullClockwise { get; } =
             new RasterizerState("RasterizerState.CullClockwise", CullMode.CullClockwiseFace);

        public static RasterizerState CullCounterClockwise { get; } = 
            new RasterizerState("RasterizerState.CullCounterClockwise", CullMode.CullCounterClockwiseFace);

        public static RasterizerState CullNone { get; } =
            new RasterizerState("RasterizerState.CullNone", CullMode.None);

        private readonly bool _defaultStateObject;

        private CullMode _cullMode;
        private float _depthBias;
        private FillMode _fillMode;
        private bool _multiSampleAntiAlias;
        private bool _scissorTestEnable;
        private float _slopeScaleDepthBias;
        private bool _depthClipEnable;

        #region Properties

        public CullMode CullMode
        {
            get => _cullMode;
            set
            {
                ThrowIfBound();
                _cullMode = value;
            }
        }

        public float DepthBias
        {
            get => _depthBias;
            set
            {
                ThrowIfBound();
                _depthBias = value;
            }
        }

        public FillMode FillMode
        {
            get => _fillMode;
            set
            {
                ThrowIfBound();
                _fillMode = value;
            }
        }

        public bool MultiSampleAntiAlias
        {
            get => _multiSampleAntiAlias;
            set
            {
                ThrowIfBound();
                _multiSampleAntiAlias = value;
            }
        }

        public bool ScissorTestEnable
        {
            get => _scissorTestEnable;
            set
            {
                ThrowIfBound();
                _scissorTestEnable = value;
            }
        }

        public float SlopeScaleDepthBias
        {
            get => _slopeScaleDepthBias;
            set
            {
                ThrowIfBound();
                _slopeScaleDepthBias = value;
            }
        }

        public bool DepthClipEnable
        {
            get => _depthClipEnable;
            set
            {
                ThrowIfBound();
                _depthClipEnable = value;
            }
        }

        #endregion

        #region Constructors

        public RasterizerState()
        {
            CullMode = CullMode.CullCounterClockwiseFace;
            FillMode = FillMode.Solid;
            DepthBias = 0;
            MultiSampleAntiAlias = true;
            ScissorTestEnable = false;
            SlopeScaleDepthBias = 0;
            DepthClipEnable = true;
        }

        private RasterizerState(string name, CullMode cullMode) : this()
        {
            Name = name;
            _cullMode = cullMode;
            _defaultStateObject = true;
        }

        private RasterizerState(RasterizerState cloneSource)
        {
            Name = cloneSource.Name;
            _cullMode = cloneSource._cullMode;
            _fillMode = cloneSource._fillMode;
            _depthBias = cloneSource._depthBias;
            _multiSampleAntiAlias = cloneSource._multiSampleAntiAlias;
            _scissorTestEnable = cloneSource._scissorTestEnable;
            _slopeScaleDepthBias = cloneSource._slopeScaleDepthBias;
            _depthClipEnable = cloneSource._depthClipEnable;
        }

        #endregion

        internal void BindToGraphicsDevice(GraphicsDevice device)
        {
            if (_defaultStateObject)
                throw new InvalidOperationException(
                    "You cannot bind a default state object.");

            if (GraphicsDevice != null && GraphicsDevice != device)
                throw new InvalidOperationException(
                    "This rasterizer state is already bound to a different graphics device.");

            GraphicsDevice = device;
        }

        internal void ThrowIfBound()
        {
            if (_defaultStateObject)
                throw new InvalidOperationException(
                    "You cannot modify a default rasterizer state object.");

            if (GraphicsDevice != null)
                throw new InvalidOperationException(
                    "You cannot modify the rasterizer state after it has been bound to the graphics device!");
        }

        internal RasterizerState Clone()
        {
            return new RasterizerState(this);
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
