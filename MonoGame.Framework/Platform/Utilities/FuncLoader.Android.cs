using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Runtime.InteropServices;
using Android.App;

namespace MonoGame.Framework
{
    internal class FuncLoader
    {
        [DllImport("dl")]
        [SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "DllImport")]
        public static extern IntPtr dlopen(string path, int flags);

        [DllImport("dl")]
        [SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "DllImport")]
        public static extern IntPtr dlsym(IntPtr handle, string symbol);

        private const int RTLD_LAZY = 0x0001;

        public static IntPtr LoadLibrary(string libname)
        {
            // Let the OS search for the library by default.
            IntPtr lib = dlopen(libname, RTLD_LAZY);
            if (lib != IntPtr.Zero)
            {
                Console.WriteLine("FuncLoader.LoadLibrary {0}", libname);
                return lib;
            }

            // Some Android devices won't search the native library path
            // for the library, so we have to do it manually here.
            string nlibpath = Application.Context.ApplicationInfo.NativeLibraryDir;
            string libpath = Path.Combine(nlibpath, libname);
            lib = dlopen(libpath, RTLD_LAZY);
            if (lib != IntPtr.Zero)
            {	
                Console.WriteLine("FuncLoader.LoadLibrary {0}", libpath);
                return lib;
            }

            Console.WriteLine("FuncLoader.LoadLibrary {0} Not Found!", libname);
            return IntPtr.Zero;
        }

        public static TDelegate LoadFunction<TDelegate>(
            IntPtr library, string function, bool throwIfNotFound = false)
        {
            var ret = dlsym(library, function);
            if (ret == IntPtr.Zero)
            {
                if (throwIfNotFound)
                    throw new EntryPointNotFoundException(function);
                return default;
            }

            // TODO: Use the function below once Protobuild gets axed
            // requires .NET Framework 4.5.1 and its useful for corert
            // return Marshal.GetDelegateForFunctionPointer<T>(ret);
            return Marshal.GetDelegateForFunctionPointer<TDelegate>(ret);
        }
    }
}
