// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using MonoGame.OpenGL;

namespace Microsoft.Xna.Framework.Graphics
{
    public partial class BlendState
    {
        internal void PlatformApplyState(GraphicsDevice device, bool force = false)
        {
            var blendEnabled = !(ColorSourceBlend == Blend.One &&
                                 ColorDestinationBlend == Blend.Zero &&
                                 AlphaSourceBlend == Blend.One &&
                                 AlphaDestinationBlend == Blend.Zero);
            if (force || blendEnabled != device._lastBlendEnable)
            {
                if (blendEnabled)
                    GL.Enable(EnableCap.Blend);
                else
                    GL.Disable(EnableCap.Blend);
                GraphicsExtensions.CheckGLError();
                device._lastBlendEnable = blendEnabled;
            }
            if (_independentBlendEnable)
            {
                for (int i = 0; i < 4; i++)
                {
                    if (force ||
                        _targetBlendState[i].ColorBlendFunction != device._lastBlendState[i].ColorBlendFunction ||
                        _targetBlendState[i].AlphaBlendFunction != device._lastBlendState[i].AlphaBlendFunction)
                    {
                        GL.BlendEquationSeparatei(i,
                            _targetBlendState[i].ColorBlendFunction.GetBlendEquationMode(),
                            _targetBlendState[i].AlphaBlendFunction.GetBlendEquationMode());
                        GraphicsExtensions.CheckGLError();
                        device._lastBlendState[i].ColorBlendFunction = _targetBlendState[i].ColorBlendFunction;
                        device._lastBlendState[i].AlphaBlendFunction = _targetBlendState[i].AlphaBlendFunction;
                    }

                    if (force ||
                        _targetBlendState[i].ColorSourceBlend != device._lastBlendState[i].ColorSourceBlend ||
                        _targetBlendState[i].ColorDestinationBlend != device._lastBlendState[i].ColorDestinationBlend ||
                        _targetBlendState[i].AlphaSourceBlend != device._lastBlendState[i].AlphaSourceBlend ||
                        _targetBlendState[i].AlphaDestinationBlend != device._lastBlendState[i].AlphaDestinationBlend)
                    {
                        GL.BlendFuncSeparatei(i,
                            _targetBlendState[i].ColorSourceBlend.GetBlendFactorSrc(),
                            _targetBlendState[i].ColorDestinationBlend.GetBlendFactorDest(),
                            _targetBlendState[i].AlphaSourceBlend.GetBlendFactorSrc(),
                            _targetBlendState[i].AlphaDestinationBlend.GetBlendFactorDest());
                        GraphicsExtensions.CheckGLError();
                        device._lastBlendState[i].ColorSourceBlend = _targetBlendState[i].ColorSourceBlend;
                        device._lastBlendState[i].ColorDestinationBlend = _targetBlendState[i].ColorDestinationBlend;
                        device._lastBlendState[i].AlphaSourceBlend = _targetBlendState[i].AlphaSourceBlend;
                        device._lastBlendState[i].AlphaDestinationBlend = _targetBlendState[i].AlphaDestinationBlend;
                    }
                }
            }
            else
            {
                if (force ||
                    ColorBlendFunction != device._lastBlendState.ColorBlendFunction ||
                    AlphaBlendFunction != device._lastBlendState.AlphaBlendFunction)
                {
                    GL.BlendEquationSeparate(
                        ColorBlendFunction.GetBlendEquationMode(),
                        AlphaBlendFunction.GetBlendEquationMode());
                    GraphicsExtensions.CheckGLError();
                    for (int i = 0; i < 4; i++)
                    {
                        device._lastBlendState[i].ColorBlendFunction = ColorBlendFunction;
                        device._lastBlendState[i].AlphaBlendFunction = AlphaBlendFunction;
                    }
                }

                if (force ||
                    ColorSourceBlend != device._lastBlendState.ColorSourceBlend ||
                    ColorDestinationBlend != device._lastBlendState.ColorDestinationBlend ||
                    AlphaSourceBlend != device._lastBlendState.AlphaSourceBlend ||
                    AlphaDestinationBlend != device._lastBlendState.AlphaDestinationBlend)
                {
                    GL.BlendFuncSeparate(
                        ColorSourceBlend.GetBlendFactorSrc(),
                        ColorDestinationBlend.GetBlendFactorDest(),
                        AlphaSourceBlend.GetBlendFactorSrc(),
                        AlphaDestinationBlend.GetBlendFactorDest());
                    GraphicsExtensions.CheckGLError();
                    for (int i = 0; i < 4; i++)
                    {
                        device._lastBlendState[i].ColorSourceBlend = ColorSourceBlend;
                        device._lastBlendState[i].ColorDestinationBlend = ColorDestinationBlend;
                        device._lastBlendState[i].AlphaSourceBlend = AlphaSourceBlend;
                        device._lastBlendState[i].AlphaDestinationBlend = AlphaDestinationBlend;
                    }
                }
            }

            if (force || ColorWriteChannels != device._lastBlendState.ColorWriteChannels)
            {
                GL.ColorMask(
                    (ColorWriteChannels & ColorWriteChannels.Red) != 0,
                    (ColorWriteChannels & ColorWriteChannels.Green) != 0,
                    (ColorWriteChannels & ColorWriteChannels.Blue) != 0,
                    (ColorWriteChannels & ColorWriteChannels.Alpha) != 0);
                GraphicsExtensions.CheckGLError();
                device._lastBlendState.ColorWriteChannels = ColorWriteChannels;
            }
        }
    }
}

