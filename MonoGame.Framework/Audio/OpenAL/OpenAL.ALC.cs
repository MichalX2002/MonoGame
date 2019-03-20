using MonoGame.OpenAL;
using MonoGame.Utilities;
using System;
using System.Runtime.InteropServices;

namespace Microsoft.Xna.Framework.Audio
{
    internal class ALC
    {
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal delegate IntPtr d_alccreatecontext(IntPtr device, int[] attributes);
        internal static d_alccreatecontext CreateContext = FuncLoader.LoadFunction<d_alccreatecontext>(AL.NativeLibrary, "alcCreateContext");

        internal static ALCError GetError()
        {
            return GetErrorForDevice(IntPtr.Zero);
        }

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal delegate ALCError d_alcgeterror(IntPtr device);
        internal static d_alcgeterror GetErrorForDevice = FuncLoader.LoadFunction<d_alcgeterror>(AL.NativeLibrary, "alcGetError");

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal delegate void d_alcgetintegerv(IntPtr device, int param, int size, int[] values);
        internal static d_alcgetintegerv alcGetIntegerv = FuncLoader.LoadFunction<d_alcgetintegerv>(AL.NativeLibrary, "alcGetIntegerv");

        internal static void GetInteger(IntPtr device, ALCGetInteger param, int size, int[] values)
        {
            alcGetIntegerv(device, (int)param, size, values);
        }

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal delegate IntPtr d_alcgetcurrentcontext();
        internal static d_alcgetcurrentcontext GetCurrentContext = FuncLoader.LoadFunction<d_alcgetcurrentcontext>(AL.NativeLibrary, "alcGetCurrentContext");

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal delegate void d_alcmakecontextcurrent(IntPtr context);
        internal static d_alcmakecontextcurrent MakeContextCurrent = FuncLoader.LoadFunction<d_alcmakecontextcurrent>(AL.NativeLibrary, "alcMakeContextCurrent");

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal delegate void d_alcdestroycontext(IntPtr context);
        internal static d_alcdestroycontext DestroyContext = FuncLoader.LoadFunction<d_alcdestroycontext>(AL.NativeLibrary, "alcDestroyContext");

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal delegate void d_alcclosedevice(IntPtr device);
        internal static d_alcclosedevice CloseDevice = FuncLoader.LoadFunction<d_alcclosedevice>(AL.NativeLibrary, "alcCloseDevice");

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal delegate IntPtr d_alcopendevice(string device);
        internal static d_alcopendevice OpenDevice = FuncLoader.LoadFunction<d_alcopendevice>(AL.NativeLibrary, "alcOpenDevice");

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal delegate IntPtr d_alccaptureopendevice(string device, uint sampleRate, int format, int sampleSize);
        internal static d_alccaptureopendevice alcCaptureOpenDevice = FuncLoader.LoadFunction<d_alccaptureopendevice>(AL.NativeLibrary, "alcCaptureOpenDevice");

        internal static IntPtr CaptureOpenDevice(string device, uint sampleRate, ALFormat format, int sampleSize)
        {
            return alcCaptureOpenDevice(device, sampleRate, (int)format, sampleSize);
        }

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal delegate IntPtr d_alccapturestart(IntPtr device);
        internal static d_alccapturestart CaptureStart = FuncLoader.LoadFunction<d_alccapturestart>(AL.NativeLibrary, "alcCaptureStart");

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal delegate void d_alccapturesamples(IntPtr device, IntPtr buffer, int samples);
        internal static d_alccapturesamples CaptureSamples = FuncLoader.LoadFunction<d_alccapturesamples>(AL.NativeLibrary, "alcCaptureSamples");

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal delegate IntPtr d_alccapturestop(IntPtr device);
        internal static d_alccapturestop CaptureStop = FuncLoader.LoadFunction<d_alccapturestop>(AL.NativeLibrary, "alcCaptureStop");

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal delegate IntPtr d_alccaptureclosedevice(IntPtr device);
        internal static d_alccaptureclosedevice CaptureCloseDevice = FuncLoader.LoadFunction<d_alccaptureclosedevice>(AL.NativeLibrary, "alcCaptureCloseDevice");

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal delegate bool d_alcisextensionpresent(IntPtr device, string extensionName);
        internal static d_alcisextensionpresent IsExtensionPresent = FuncLoader.LoadFunction<d_alcisextensionpresent>(AL.NativeLibrary, "alcIsExtensionPresent");

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal delegate IntPtr d_alcgetstring(IntPtr device, int p);
        internal static d_alcgetstring alcGetString = FuncLoader.LoadFunction<d_alcgetstring>(AL.NativeLibrary, "alcGetString");

        internal static string GetString(IntPtr device, int p)
        {
            return Marshal.PtrToStringAnsi(alcGetString(device, p));
        }

        internal static string GetString(IntPtr device, ALCGetString p)
        {
            return GetString(device, (int)p);
        }

#if IOS
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal delegate void d_alcsuspendcontext(IntPtr context);
        internal static d_alcsuspendcontext SuspendContext = FuncLoader.LoadFunction<d_alcsuspendcontext>(AL.NativeLibrary, "alcSuspendContext");

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal delegate void d_alcprocesscontext(IntPtr context);
        internal static d_alcprocesscontext ProcessContext = FuncLoader.LoadFunction<d_alcprocesscontext>(AL.NativeLibrary, "alcProcessContext");
#endif

#if ANDROID
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal delegate void d_alcdevicepausesoft(IntPtr device);
        internal static d_alcdevicepausesoft DevicePause = FuncLoader.LoadFunction<d_alcdevicepausesoft>(AL.NativeLibrary, "alcDevicePauseSOFT");

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal delegate void d_alcdeviceresumesoft(IntPtr device);
        internal static d_alcdeviceresumesoft DeviceResume = FuncLoader.LoadFunction<d_alcdeviceresumesoft>(AL.NativeLibrary, "alcDeviceResumeSOFT");
#endif
    }
}
