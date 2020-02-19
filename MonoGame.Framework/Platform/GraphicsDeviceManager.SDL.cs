// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using MonoGame.Framework.Graphics;

namespace MonoGame.Framework
{
    public partial class GraphicsDeviceManager
    {
        partial void PlatformInitialize(PresentationParameters presentationParameters)
        {
            var surfaceFormat = _game.GraphicsDeviceManager.PreferredBackBufferFormat.GetColorFormat();
            var depthStencilFormat = _game.GraphicsDeviceManager.PreferredDepthStencilFormat;

            // TODO: Need to get this data from the Presentation Parameters
            SDL.GL.SetAttribute(SDL.GL.Attribute.RedSize, surfaceFormat.R);
            SDL.GL.SetAttribute(SDL.GL.Attribute.GreenSize, surfaceFormat.G);
            SDL.GL.SetAttribute(SDL.GL.Attribute.BlueSize, surfaceFormat.B);
            SDL.GL.SetAttribute(SDL.GL.Attribute.AlphaSize, surfaceFormat.A);

            switch (depthStencilFormat)
            {
                case DepthFormat.None:
                    SDL.GL.SetAttribute(SDL.GL.Attribute.DepthSize, 0);
                    SDL.GL.SetAttribute(SDL.GL.Attribute.StencilSize, 0);
                    break;

                case DepthFormat.Depth16:
                    SDL.GL.SetAttribute(SDL.GL.Attribute.DepthSize, 16);
                    SDL.GL.SetAttribute(SDL.GL.Attribute.StencilSize, 0);
                    break;

                case DepthFormat.Depth24:
                    SDL.GL.SetAttribute(SDL.GL.Attribute.DepthSize, 24);
                    SDL.GL.SetAttribute(SDL.GL.Attribute.StencilSize, 0);
                    break;

                case DepthFormat.Depth24Stencil8:
                    SDL.GL.SetAttribute(SDL.GL.Attribute.DepthSize, 24);
                    SDL.GL.SetAttribute(SDL.GL.Attribute.StencilSize, 8);
                    break;
            }

            SDL.GL.SetAttribute(SDL.GL.Attribute.DoubleBuffer, 1);
            SDL.GL.SetAttribute(SDL.GL.Attribute.ContextMajorVersion, 2);
            SDL.GL.SetAttribute(SDL.GL.Attribute.ContextMinorVersion, 1);

            ((SdlGameWindow)_game.Window).CreateWindow();
        }
    }
}
