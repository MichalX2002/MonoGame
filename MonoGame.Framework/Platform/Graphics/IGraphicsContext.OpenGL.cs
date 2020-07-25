// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;

namespace MonoGame.OpenGL
{
    internal interface IGraphicsContext : IDisposable
    {
        bool IsDisposed { get; }
        bool IsCurrent { get; }
        int SwapInterval { get; set; }

        void MakeCurrent(IWindowHandle window);
        void SwapBuffers();
    }
}
