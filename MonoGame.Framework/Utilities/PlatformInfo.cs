// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using MonoGame.Framework.Utilities;

namespace MonoGame.Framework
{
    /// <summary>
    /// Utility class that returns information about the underlying system and platform.
    /// </summary>
    public static partial class PlatformInfo
    {
        /// <summary>
        /// Gets the executing operating system type.
        /// </summary>
        public static OS CurrentOS => OperatingSystemProbe.Probe();

        /// <summary>
        /// Gets the underlying game platform type.
        /// </summary>
        public static MonoGamePlatform Platform
        {
            get
            {
#if ANDROID
                return MonoGamePlatform.Android;
#elif DESKTOPGL
                return MonoGamePlatform.DesktopGL;
#elif IOS && !TVOS
                return MonoGamePlatform.iOS;
#elif TVOS
                return MonoGamePlatform.tvOS;
#elif WEB
                return MonoGamePlatform.WebGL;
#elif WINDOWS && DIRECTX
                return MonoGamePlatform.Windows;
#elif WINDOWS_UAP
                return MonoGamePlatform.WindowsUniversal;
#elif SWITCH
                return MonoGamePlatform.NintendoSwitch;
#elif XB1
                return MonoGamePlatform.XboxOne;
#elif PLAYSTATION4
                return MonoGamePlatform.PlayStation4;
#elif PSVITA
                return MonoGamePlatform.PSVita;
#elif STADIA
                return MonoGamePlatform.Stadia;
#else
                return MonoGamePlatform.Unknown;
#endif
            }
        }

        /// <summary>
        /// Gets the graphics backend type of the game platform.
        /// </summary>
        public static GraphicsBackend GraphicsBackend
        {
            get
            {
#if DIRECTX
                return GraphicsBackend.DirectX;
#else
                return GraphicsBackend.OpenGL;
#endif
            }
        }

        public static string Rid
        {
            get
            {
                if (CurrentOS == OS.Windows && Environment.Is64BitProcess)
                    return "win-x64";
                else if (CurrentOS == OS.Windows && !Environment.Is64BitProcess)
                    return "win-x86";
                else if (CurrentOS == OS.Linux)
                    return "linux-x64";
                else if (CurrentOS == OS.MacOSX)
                    return "osx";
                else
                    return "unknown";
            }
        }
    }
}
