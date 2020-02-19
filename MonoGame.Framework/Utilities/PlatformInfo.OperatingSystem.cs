// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

namespace MonoGame.Framework
{
    public static partial class PlatformInfo
    {
        public enum OperatingSystem
        {
            Windows,
            Linux,
            MacOSX,
            Unknown
        }

        private static class OperatingSystemProbe
        {
            private static bool _init = false;
            private static OperatingSystem _cachedOS;

            public static OperatingSystem Probe()
            {
                if (!_init)
                {
                    _cachedOS = ProbeCore();
                    _init = true;
                }
                return _cachedOS;
            }

            [DllImport("libc")]
            [SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "Interop")]
            static extern int uname(IntPtr buf);

            private static OperatingSystem ProbeCore()
            {
                switch (Environment.OSVersion.Platform)
                {
                    case PlatformID.Win32NT:
                    case PlatformID.Win32S:
                    case PlatformID.Win32Windows:
                    case PlatformID.WinCE:
                        return OperatingSystem.Windows;

                    case PlatformID.MacOSX:
                        return OperatingSystem.MacOSX;

                    case PlatformID.Unix:
                        // Mac can return a value of Unix sometimes, we need to double check it.
                        var buffer = Marshal.AllocHGlobal(8192);
                        try
                        {
                            if (uname(buffer) == 0)
                                if (Marshal.PtrToStringAnsi(buffer) == "Darwin")
                                    return OperatingSystem.MacOSX;
                        }
                        catch
                        {
                        }
                        finally
                        {
                            Marshal.FreeHGlobal(buffer);
                        }
                        return OperatingSystem.Linux;

                    default:
                        return OperatingSystem.Unknown;
                }
            }
        }
    }
}
