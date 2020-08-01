// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.IO;

namespace MonoGame.Framework
{
    public static partial class TitleContainer
    {
        static partial void PlatformInit()
        {
#if WINDOWS || DESKTOPGL
#if DESKTOPGL
            // Check for the package Resources Folder first. This is where the assets will be bundled.
            if (PlatformInfo.CurrentOS == PlatformInfo.OS.MacOSX)
                Location = Path.Combine(AppContext.BaseDirectory, "..", "Resources");

            if (!Directory.Exists(Location))
#endif
            {
                Location = AppContext.BaseDirectory;
            }
#endif
        }

        private static Stream PlatformOpenStream(string safeName)
        {
            var absolutePath = Path.Combine(Location, safeName);
            return File.OpenRead(absolutePath);
        }
    }
}

