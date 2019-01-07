// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;

namespace Microsoft.Xna.Framework.Graphics
{
    public partial class DepthStencilState : GraphicsResource
    {
        private readonly bool _defaultStateObject;

        private bool _depthBufferEnable;
        private bool _depthBufferWriteEnable;
        private StencilOperation _counterClockwiseStencilDepthBufferFail;
        private StencilOperation _counterClockwiseStencilFail;
        private CompareFunction _counterClockwiseStencilFunction;
        private StencilOperation _counterClockwiseStencilPass;
        private CompareFunction _depthBufferFunction;
        private int _referenceStencil;
        private StencilOperation _stencilDepthBufferFail;
        private bool _stencilEnable;
        private StencilOperation _stencilFail;
        private CompareFunction _stencilFunction;
        private int _stencilMask;
        private StencilOperation _stencilPass;
        private int _stencilWriteMask;
        private bool _twoSidedStencilMode;

        public bool DepthBufferEnable
        {
            get => _depthBufferEnable;
            set
            {
                ThrowIfBound();
                _depthBufferEnable = value;
            }
        }

        public bool DepthBufferWriteEnable
        {
            get => _depthBufferWriteEnable;
            set
            {
                ThrowIfBound();
                _depthBufferWriteEnable = value;
            }
        }

        public StencilOperation CounterClockwiseStencilDepthBufferFail
        {
            get => _counterClockwiseStencilDepthBufferFail;
            set
            {
                ThrowIfBound();
                _counterClockwiseStencilDepthBufferFail = value;
            }
        }

        public StencilOperation CounterClockwiseStencilFail
        {
            get => _counterClockwiseStencilFail;
            set
            {
                ThrowIfBound();
                _counterClockwiseStencilFail = value;
            }
        }

        public CompareFunction CounterClockwiseStencilFunction
        {
            get => _counterClockwiseStencilFunction;
            set
            {
                ThrowIfBound();
                _counterClockwiseStencilFunction = value;
            }
        }

        public StencilOperation CounterClockwiseStencilPass
        {
            get => _counterClockwiseStencilPass;
            set
            {
                ThrowIfBound();
                _counterClockwiseStencilPass = value;
            }
        }

        public CompareFunction DepthBufferFunction
        {
            get => _depthBufferFunction;
            set
            {
                ThrowIfBound();
                _depthBufferFunction = value;
            }
        }

        public int ReferenceStencil
        {
            get => _referenceStencil;
            set
            {
                ThrowIfBound();
                _referenceStencil = value;
            }
        }

        public StencilOperation StencilDepthBufferFail
        {
            get => _stencilDepthBufferFail;
            set
            {
                ThrowIfBound();
                _stencilDepthBufferFail = value;
            }
        }

        public bool StencilEnable
        {
            get => _stencilEnable;
            set
            {
                ThrowIfBound();
                _stencilEnable = value;
            }
        }

        public StencilOperation StencilFail
        {
            get => _stencilFail;
            set
            {
                ThrowIfBound();
                _stencilFail = value;
            }
        }

        public CompareFunction StencilFunction
        {
            get => _stencilFunction;
            set
            {
                ThrowIfBound();
                _stencilFunction = value;
            }
        }

        public int StencilMask
        {
            get => _stencilMask;
            set
            {
                ThrowIfBound();
                _stencilMask = value;
            }
        }

        public StencilOperation StencilPass
        {
            get => _stencilPass;
            set
            {
                ThrowIfBound();
                _stencilPass = value;
            }
        }

        public int StencilWriteMask
        {
            get => _stencilWriteMask;
            set
            {
                ThrowIfBound();
                _stencilWriteMask = value;
            }
        }

        public bool TwoSidedStencilMode
        {
            get => _twoSidedStencilMode;
            set
            {
                ThrowIfBound();
                _twoSidedStencilMode = value;
            }
        }

        internal void BindToGraphicsDevice(GraphicsDevice device)
        {
            if (_defaultStateObject)
                throw new InvalidOperationException("You cannot bind a default state object.");
            if (GraphicsDevice != null && GraphicsDevice != device)
                throw new InvalidOperationException("This depth stencil state is already bound to a different graphics device.");
            GraphicsDevice = device;
        }

        internal void ThrowIfBound()
        {
            if (_defaultStateObject)
                throw new InvalidOperationException("You cannot modify a default depth stencil state object.");
            if (GraphicsDevice != null)
                throw new InvalidOperationException("You cannot modify the depth stencil state after it has been bound to the graphics device!");
        }

        public DepthStencilState ()
		{
            DepthBufferEnable = true;
            DepthBufferWriteEnable = true;
			DepthBufferFunction = CompareFunction.LessEqual;
			StencilEnable = false;
			StencilFunction = CompareFunction.Always;
			StencilPass = StencilOperation.Keep;
			StencilFail = StencilOperation.Keep;
			StencilDepthBufferFail = StencilOperation.Keep;
			TwoSidedStencilMode = false;
			CounterClockwiseStencilFunction = CompareFunction.Always;
			CounterClockwiseStencilFail = StencilOperation.Keep;
			CounterClockwiseStencilPass = StencilOperation.Keep;
			CounterClockwiseStencilDepthBufferFail = StencilOperation.Keep;
			StencilMask = Int32.MaxValue;
			StencilWriteMask = Int32.MaxValue;
			ReferenceStencil = 0;
		}

        private DepthStencilState(string name, bool depthBufferEnable, bool depthBufferWriteEnable)
            : this()
	    {
	        Name = name;
            _depthBufferEnable = depthBufferEnable;
            _depthBufferWriteEnable = depthBufferWriteEnable;
	        _defaultStateObject = true;
	    }

        private DepthStencilState(DepthStencilState cloneSource)
	    {
	        Name = cloneSource.Name;
            _depthBufferEnable = cloneSource._depthBufferEnable;
            _depthBufferWriteEnable = cloneSource._depthBufferWriteEnable;
            _counterClockwiseStencilDepthBufferFail = cloneSource._counterClockwiseStencilDepthBufferFail;
            _counterClockwiseStencilFail = cloneSource._counterClockwiseStencilFail;
            _counterClockwiseStencilFunction = cloneSource._counterClockwiseStencilFunction;
            _counterClockwiseStencilPass = cloneSource._counterClockwiseStencilPass;
            _depthBufferFunction = cloneSource._depthBufferFunction;
            _referenceStencil = cloneSource._referenceStencil;
            _stencilDepthBufferFail = cloneSource._stencilDepthBufferFail;
            _stencilEnable = cloneSource._stencilEnable;
            _stencilFail = cloneSource._stencilFail;
            _stencilFunction = cloneSource._stencilFunction;
            _stencilMask = cloneSource._stencilMask;
            _stencilPass = cloneSource._stencilPass;
            _stencilWriteMask = cloneSource._stencilWriteMask;
            _twoSidedStencilMode = cloneSource._twoSidedStencilMode;
	    }

        public static readonly DepthStencilState Default;
        public static readonly DepthStencilState DepthRead;
        public static readonly DepthStencilState None;

		static DepthStencilState ()
		{
		    Default = new DepthStencilState("DepthStencilState.Default", true, true);
			DepthRead = new DepthStencilState("DepthStencilState.DepthRead", true, false);
		    None = new DepthStencilState("DepthStencilState.None", false, false);
		}

        internal DepthStencilState Clone()
        {
            return new DepthStencilState(this);
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

