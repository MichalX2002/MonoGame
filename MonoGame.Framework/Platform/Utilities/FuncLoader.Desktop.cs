using System;
using System.IO;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

namespace MonoGame.Framework
{
    [SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "Interop")]
    internal class FuncLoader
    {
        private class Windows
        {
            [DllImport("kernel32", CharSet = CharSet.Ansi, ExactSpelling = true, SetLastError = true)]
            public static extern IntPtr GetProcAddress(IntPtr hModule, string procName);

            [DllImport("kernel32", SetLastError = true, CharSet = CharSet.Unicode)]
            public static extern IntPtr LoadLibraryW(string lpszLib);
        }

        private class Linux
        {
            [DllImport("libdl.so.2")]
            public static extern IntPtr dlopen(string path, int flags);

            [DllImport("libdl.so.2")]
            public static extern IntPtr dlsym(IntPtr handle, string symbol);
        }

        private class OSX
        {
            [DllImport("/usr/lib/libSystem.dylib")]
            public static extern IntPtr dlopen(string path, int flags);

            [DllImport("/usr/lib/libSystem.dylib")]
            public static extern IntPtr dlsym(IntPtr handle, string symbol);
        }
        
        private const int RTLD_LAZY = 0x0001;

        public static IntPtr LoadLibraryExt(string libname)
        {
            var assemblyLocation = Path.GetDirectoryName(typeof(FuncLoader).Assembly.Location) ?? "./";
            IntPtr ret;

            // Try .NET Framework / Mono locations
            if (PlatformInfo.CurrentOS == PlatformInfo.OS.MacOSX)
            {
                ret = LoadLibrary(Path.Combine(assemblyLocation, libname));

                // Look in Frameworks for .app bundles
                if (ret == IntPtr.Zero)
                    ret = LoadLibrary(Path.Combine(assemblyLocation, "..", "Frameworks", libname));
            }
            else
            {
                if (Environment.Is64BitProcess)
                    ret = LoadLibrary(Path.Combine(assemblyLocation, "x64", libname));
                else
                    ret = LoadLibrary(Path.Combine(assemblyLocation, "x86", libname));
            }

            // Try .NET Core development locations
            if (ret == IntPtr.Zero)
                ret = LoadLibrary(
                    Path.Combine(assemblyLocation, "runtimes", PlatformInfo.Rid, "native", libname));

            // Try current folder (.NET Core will copy it there after publish)
            if (ret == IntPtr.Zero)
                ret = LoadLibrary(Path.Combine(assemblyLocation, libname));

            // Try loading system library
            if (ret == IntPtr.Zero)
                ret = LoadLibrary(libname);

            // Welp, all failed, PANIC!!!
            if (ret == IntPtr.Zero)
                throw new Exception("Failed to load library: " + libname);

            return ret;
        }

        public static IntPtr LoadLibrary(string libname)
        {
            if (PlatformInfo.CurrentOS == PlatformInfo.OS.Windows)
                return Windows.LoadLibraryW(libname);

            if (PlatformInfo.CurrentOS == PlatformInfo.OS.MacOSX)
                return OSX.dlopen(libname, RTLD_LAZY);

            return Linux.dlopen(libname, RTLD_LAZY);
        }

        public static T LoadFunction<T>(IntPtr library, string function, bool throwIfNotFound = false)
        {
            var ret = IntPtr.Zero;

            if (PlatformInfo.CurrentOS == PlatformInfo.OS.Windows)
                ret = Windows.GetProcAddress(library, function);
            else if (PlatformInfo.CurrentOS == PlatformInfo.OS.MacOSX)
                ret = OSX.dlsym(library, function);
            else
                ret = Linux.dlsym(library, function);

            if (ret == IntPtr.Zero)
            {
                if (throwIfNotFound)
                    throw new EntryPointNotFoundException(function);

                return default;
            }

#if NETSTANDARD
            return Marshal.GetDelegateForFunctionPointer<T>(ret);
#else
            return (T)(object)Marshal.GetDelegateForFunctionPointer(ret, typeof(T));
#endif
        }
    }
}
