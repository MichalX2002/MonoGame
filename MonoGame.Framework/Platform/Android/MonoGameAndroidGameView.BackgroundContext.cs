// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using Javax.Microedition.Khronos.Egl;

namespace MonoGame.Framework
{
    public partial class MonoGameAndroidGameView
    {
        public class BackgroundContext
        {
            private MonoGameAndroidGameView _view;
            private EGLContext _eglContext;
            private EGLSurface _surface;

            public BackgroundContext(MonoGameAndroidGameView view)
            {
                _view = view ?? throw new ArgumentNullException(nameof(view));

                foreach (var v in OpenGL.GLESVersion.GetSupportedGLESVersions())
                {
                    _eglContext = view.egl.EglCreateContext(view.eglDisplay, view.eglConfig, EGL10.EglNoContext, v.GetAttributes());
                    if (_eglContext == null || _eglContext == EGL10.EglNoContext)
                        continue;
                    break;
                }

                if (_eglContext == null || _eglContext == EGL10.EglNoContext)
                {
                    _eglContext = null;
                    throw new Exception("Could not create EGL context" + view.GetErrorAsString());
                }

                var pbufferAttribList = new int[] { EGL10.EglWidth, 64, EGL10.EglHeight, 64, EGL10.EglNone };
                _surface = view.CreatePBufferSurface(view.eglConfig, pbufferAttribList);
                if (_surface == EGL10.EglNoSurface)
                    throw new Exception("Could not create Pbuffer Surface" + view.GetErrorAsString());
            }

            public void MakeCurrent()
            {
                _view.ClearCurrent();
                _view.egl.EglMakeCurrent(_view.eglDisplay, _surface, _surface, _eglContext);
            }
        }
    }
}