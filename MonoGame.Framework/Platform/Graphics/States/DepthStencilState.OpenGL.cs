// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using MonoGame.OpenGL;

namespace Microsoft.Xna.Framework.Graphics
{
    public partial class DepthStencilState
    {
        internal void PlatformApplyState(GraphicsDevice device, bool force = false)
        {
            if (force || DepthBufferEnable != device._lastDepthStencilState.DepthBufferEnable)
            {
                if (!DepthBufferEnable)
                {
                    GL.Disable(EnableCap.DepthTest);
                    GraphicsExtensions.CheckGLError();
                }
                else
                {
                    // enable Depth Buffer
                    GL.Enable(EnableCap.DepthTest);
                    GraphicsExtensions.CheckGLError();
                }
                device._lastDepthStencilState.DepthBufferEnable = DepthBufferEnable;
            }

            if (force || DepthBufferFunction != device._lastDepthStencilState.DepthBufferFunction)
            {
                GL.DepthFunc(DepthBufferFunction.GetDepthFunction());
                GraphicsExtensions.CheckGLError();
                device._lastDepthStencilState.DepthBufferFunction = DepthBufferFunction;
            }

            if (force || DepthBufferWriteEnable != device._lastDepthStencilState.DepthBufferWriteEnable)
            {
                GL.DepthMask(DepthBufferWriteEnable);
                GraphicsExtensions.CheckGLError();
                device._lastDepthStencilState.DepthBufferWriteEnable = DepthBufferWriteEnable;
            }

            if (force || StencilEnable != device._lastDepthStencilState.StencilEnable)
            {
                if (!StencilEnable)
                {
                    GL.Disable(EnableCap.StencilTest);
                    GraphicsExtensions.CheckGLError();
                }
                else
                {
                    // enable Stencil
                    GL.Enable(EnableCap.StencilTest);
                    GraphicsExtensions.CheckGLError();
                }
                device._lastDepthStencilState.StencilEnable = StencilEnable;
            }

            // set function
            if (TwoSidedStencilMode)
            {
                var cullFaceModeFront = StencilFace.Front;
                var cullFaceModeBack = StencilFace.Back;
                var stencilFaceFront = StencilFace.Front;
                var stencilFaceBack = StencilFace.Back;

                if (force ||
					TwoSidedStencilMode != device._lastDepthStencilState.TwoSidedStencilMode ||
					StencilFunction != device._lastDepthStencilState.StencilFunction ||
					ReferenceStencil != device._lastDepthStencilState.ReferenceStencil ||
					StencilMask != device._lastDepthStencilState.StencilMask)
				{
                    GL.StencilFuncSeparate(cullFaceModeFront, GetStencilFunc(StencilFunction),
                                           ReferenceStencil, StencilMask);
                    GraphicsExtensions.CheckGLError();
                    device._lastDepthStencilState.StencilFunction = StencilFunction;
                    device._lastDepthStencilState.ReferenceStencil = ReferenceStencil;
                    device._lastDepthStencilState.StencilMask = StencilMask;
                }

                if (force ||
                    TwoSidedStencilMode != device._lastDepthStencilState.TwoSidedStencilMode ||
                    CounterClockwiseStencilFunction != device._lastDepthStencilState.CounterClockwiseStencilFunction ||
                    ReferenceStencil != device._lastDepthStencilState.ReferenceStencil ||
                    StencilMask != device._lastDepthStencilState.StencilMask)
			    {
                    GL.StencilFuncSeparate(cullFaceModeBack, GetStencilFunc(CounterClockwiseStencilFunction),
                                           ReferenceStencil, StencilMask);
                    GraphicsExtensions.CheckGLError();
                    device._lastDepthStencilState.CounterClockwiseStencilFunction = CounterClockwiseStencilFunction;
                    device._lastDepthStencilState.ReferenceStencil = ReferenceStencil;
                    device._lastDepthStencilState.StencilMask = StencilMask;
                }

                
                if (force ||
					TwoSidedStencilMode != device._lastDepthStencilState.TwoSidedStencilMode ||
					StencilFail != device._lastDepthStencilState.StencilFail ||
					StencilDepthBufferFail != device._lastDepthStencilState.StencilDepthBufferFail ||
					StencilPass != device._lastDepthStencilState.StencilPass)
                {
                    GL.StencilOpSeparate(stencilFaceFront, GetStencilOp(StencilFail),
                                         GetStencilOp(StencilDepthBufferFail),
                                         GetStencilOp(StencilPass));
                    GraphicsExtensions.CheckGLError();
                    device._lastDepthStencilState.StencilFail = StencilFail;
                    device._lastDepthStencilState.StencilDepthBufferFail = StencilDepthBufferFail;
                    device._lastDepthStencilState.StencilPass = StencilPass;
                }

                if (force ||
                    TwoSidedStencilMode != device._lastDepthStencilState.TwoSidedStencilMode ||
                    CounterClockwiseStencilFail != device._lastDepthStencilState.CounterClockwiseStencilFail ||
                    CounterClockwiseStencilDepthBufferFail != device._lastDepthStencilState.CounterClockwiseStencilDepthBufferFail ||
                    CounterClockwiseStencilPass != device._lastDepthStencilState.CounterClockwiseStencilPass)
			    {
                    GL.StencilOpSeparate(stencilFaceBack, GetStencilOp(CounterClockwiseStencilFail),
                                         GetStencilOp(CounterClockwiseStencilDepthBufferFail),
                                         GetStencilOp(CounterClockwiseStencilPass));
                    GraphicsExtensions.CheckGLError();
                    device._lastDepthStencilState.CounterClockwiseStencilFail = CounterClockwiseStencilFail;
                    device._lastDepthStencilState.CounterClockwiseStencilDepthBufferFail = CounterClockwiseStencilDepthBufferFail;
                    device._lastDepthStencilState.CounterClockwiseStencilPass = CounterClockwiseStencilPass;
                }
            }
            else
            {
                if (force ||
					TwoSidedStencilMode != device._lastDepthStencilState.TwoSidedStencilMode ||
					StencilFunction != device._lastDepthStencilState.StencilFunction ||
					ReferenceStencil != device._lastDepthStencilState.ReferenceStencil ||
					StencilMask != device._lastDepthStencilState.StencilMask)
				{
                    GL.StencilFunc(GetStencilFunc(StencilFunction), ReferenceStencil, StencilMask);
                    GraphicsExtensions.CheckGLError();
                    device._lastDepthStencilState.StencilFunction = StencilFunction;
                    device._lastDepthStencilState.ReferenceStencil = ReferenceStencil;
                    device._lastDepthStencilState.StencilMask = StencilMask;
                }

                if (force ||
                    TwoSidedStencilMode != device._lastDepthStencilState.TwoSidedStencilMode ||
                    StencilFail != device._lastDepthStencilState.StencilFail ||
                    StencilDepthBufferFail != device._lastDepthStencilState.StencilDepthBufferFail ||
                    StencilPass != device._lastDepthStencilState.StencilPass)
                {
                    GL.StencilOp(GetStencilOp(StencilFail),
                                 GetStencilOp(StencilDepthBufferFail),
                                 GetStencilOp(StencilPass));
                    GraphicsExtensions.CheckGLError();
                    device._lastDepthStencilState.StencilFail = StencilFail;
                    device._lastDepthStencilState.StencilDepthBufferFail = StencilDepthBufferFail;
                    device._lastDepthStencilState.StencilPass = StencilPass;
                }
            }

            device._lastDepthStencilState.TwoSidedStencilMode = TwoSidedStencilMode;

            if (force || StencilWriteMask != device._lastDepthStencilState.StencilWriteMask)
            {
                GL.StencilMask(StencilWriteMask);
                GraphicsExtensions.CheckGLError();
                device._lastDepthStencilState.StencilWriteMask = StencilWriteMask;
            }
        }

        private static GLStencilFunction GetStencilFunc(CompareFunction function)
        {
            switch (function)
            {
                case CompareFunction.Always: 
                    return GLStencilFunction.Always;
                case CompareFunction.Equal:
                    return GLStencilFunction.Equal;
                case CompareFunction.Greater:
                    return GLStencilFunction.Greater;
                case CompareFunction.GreaterEqual:
                    return GLStencilFunction.Gequal;
                case CompareFunction.Less:
                    return GLStencilFunction.Less;
                case CompareFunction.LessEqual:
                    return GLStencilFunction.Lequal;
                case CompareFunction.Never:
                    return GLStencilFunction.Never;
                case CompareFunction.NotEqual:
                    return GLStencilFunction.Notequal;
                default:
                    return GLStencilFunction.Always;
            }
        }

        private static StencilOp GetStencilOp(StencilOperation operation)
        {
            switch (operation)
            {
                case StencilOperation.Keep:
                    return StencilOp.Keep;
                case StencilOperation.Decrement:
                    return StencilOp.DecrWrap;
                case StencilOperation.DecrementSaturation:
                    return StencilOp.Decr;
                case StencilOperation.IncrementSaturation:
                    return StencilOp.Incr;
                case StencilOperation.Increment:
                    return StencilOp.IncrWrap;
                case StencilOperation.Invert:
                    return StencilOp.Invert;
                case StencilOperation.Replace:
                    return StencilOp.Replace;
                case StencilOperation.Zero:
                    return StencilOp.Zero;
                default:
                    return StencilOp.Keep;
            }
        }
    }
}

