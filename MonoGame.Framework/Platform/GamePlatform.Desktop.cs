// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

#if WINDOWS_UAP
using Windows.UI.ViewManagement;
#endif

namespace MonoGame.Framework
{
    partial class GamePlatform
    {
        internal static GamePlatform PlatformCreate(Game game)
        {
#if DESKTOPGL || ANGLE
            return new SDLGamePlatform(game);
#elif WINDOWS && DIRECTX
            return new WinFormsGamePlatform(game);
#elif WINDOWS_UAP
            return new UAPGamePlatform(game);
#endif
        }
   }
}
