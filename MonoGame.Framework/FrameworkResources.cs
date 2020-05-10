// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

namespace MonoGame.Framework
{
    internal static class FrameworkResources
    {
        #region Error strings

        internal const string ResourceCreationWithNullDevice = 
            "The graphics device may not be null when creating new resources.";

        internal const string NoGraphicsDeviceService = "The graphics device service is missing.";

        internal const string OffThreadSpanNotSupported =
            "The method (which uses a Span<T> parameter) can only be called on the main thread on this platform. " +
            "Use a Memory<T> overload to work with the resource from outside the main thread.";

        internal const string OffThreadNotSupported = "The method must be called on the main thread.";

        internal const string WriteOnlyResource = "Cannot read from a writy-only resource.";

        #endregion
    }
}
