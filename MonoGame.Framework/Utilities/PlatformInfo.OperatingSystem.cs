﻿// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

namespace MonoGame.Framework
{
    public static partial class PlatformInfo
    {
        /// <summary>
        /// Identifies an operating system.
        /// </summary>
        public enum OS
        {
            Windows,
            Linux,
            MacOSX,
            Unknown
        }

        private static class OperatingSystemProbe
        {
            [DllImport("libc")]
            [SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "Interop")]
            static extern int uname(IntPtr buf);

            private static bool _probed;
            private static OS _cachedOS;

            public static OS Probe()
            {
                if (!_probed)
                {
                    _cachedOS = ProbeCore();
                    _probed = true;
                }
                return _cachedOS;
            }

            private static OS ProbeCore()
            {
                switch (Environment.OSVersion.Platform)
                {
                    case PlatformID.Win32NT:
                    case PlatformID.Win32S:
                    case PlatformID.Win32Windows:
                    case PlatformID.WinCE:
                        return OS.Windows;

                    case PlatformID.MacOSX:
                        return OS.MacOSX;

                    case PlatformID.Unix:
                        // Mac can return a value of Unix sometimes, we need to double check it.
                        var buffer = Marshal.AllocHGlobal(8192);
                        try
                        {
                            if (uname(buffer) == 0)
                                if (Marshal.PtrToStringAnsi(buffer) == "Darwin")
                                    return OS.MacOSX;
                        }
                        catch
                        {
                        }
                        finally
                        {
                            Marshal.FreeHGlobal(buffer);
                        }
                        return OS.Linux;

                    default:
                        return OS.Unknown;
                }
            }
        }
    }
}
