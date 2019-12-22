using System;
using System.Runtime.InteropServices;
using FL = MonoGame.Framework.FuncLoader;

namespace MonoGame.OpenAL
{
    internal unsafe class ALC
    {
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate IntPtr d_alccreatecontext(IntPtr device, int* attributes);
        private static d_alccreatecontext alcCreateContext = FL.LoadFunction<d_alccreatecontext>(AL.NativeLibrary, "alcCreateContext");

        public static IntPtr CreateContext(IntPtr device, Span<int> attributes)
        {
            fixed (int* ptr = &MemoryMarshal.GetReference(attributes))
                return alcCreateContext(device, ptr);
        }

        public static ALCError GetError()
        {
            return GetErrorForDevice(IntPtr.Zero);
        }

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate ALCError d_alcgeterror(IntPtr device);
        public static d_alcgeterror GetErrorForDevice = FL.LoadFunction<d_alcgeterror>(AL.NativeLibrary, "alcGetError");

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void d_alcgetintegerv(IntPtr device, int param, int size, int* values);
        private static d_alcgetintegerv alcGetIntegerv = FL.LoadFunction<d_alcgetintegerv>(AL.NativeLibrary, "alcGetIntegerv");

        public static void GetInteger(IntPtr device, ALCGetInteger param, int size, Span<int> values)
        {
            fixed (int* ptr = &MemoryMarshal.GetReference(values))
                alcGetIntegerv(device, (int)param, size, ptr);
        }

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate IntPtr d_alcgetcurrentcontext();
        public static d_alcgetcurrentcontext GetCurrentContext = FL.LoadFunction<d_alcgetcurrentcontext>(AL.NativeLibrary, "alcGetCurrentContext");

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void d_alcmakecontextcurrent(IntPtr context);
        public static d_alcmakecontextcurrent MakeContextCurrent = FL.LoadFunction<d_alcmakecontextcurrent>(AL.NativeLibrary, "alcMakeContextCurrent");

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void d_alcdestroycontext(IntPtr context);
        public static d_alcdestroycontext DestroyContext = FL.LoadFunction<d_alcdestroycontext>(AL.NativeLibrary, "alcDestroyContext");

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void d_alcclosedevice(IntPtr device);
        public static d_alcclosedevice CloseDevice = FL.LoadFunction<d_alcclosedevice>(AL.NativeLibrary, "alcCloseDevice");

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate IntPtr d_alcopendevice(string device);
        public static d_alcopendevice OpenDevice = FL.LoadFunction<d_alcopendevice>(AL.NativeLibrary, "alcOpenDevice");

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate IntPtr d_alccaptureopendevice(string device, uint sampleRate, int format, int sampleSize);
        public static d_alccaptureopendevice alcCaptureOpenDevice = FL.LoadFunction<d_alccaptureopendevice>(AL.NativeLibrary, "alcCaptureOpenDevice");

        public static IntPtr CaptureOpenDevice(string device, uint sampleRate, ALFormat format, int sampleSize)
        {
            return alcCaptureOpenDevice(device, sampleRate, (int)format, sampleSize);
        }

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate IntPtr d_alccapturestart(IntPtr device);
        public static d_alccapturestart CaptureStart = FL.LoadFunction<d_alccapturestart>(AL.NativeLibrary, "alcCaptureStart");

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate void d_alccapturesamples(IntPtr device, byte* buffer, int samples);
        private static d_alccapturesamples alcCaptureSamples = FL.LoadFunction<d_alccapturesamples>(AL.NativeLibrary, "alcCaptureSamples");

        public static void CaptureSamples(IntPtr device, Span<byte> buffer, int samples)
        {
            fixed (byte* ptr = &MemoryMarshal.GetReference(buffer))
                alcCaptureSamples(device, ptr, samples);
        }

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate IntPtr d_alccapturestop(IntPtr device);
        public static d_alccapturestop CaptureStop = FL.LoadFunction<d_alccapturestop>(AL.NativeLibrary, "alcCaptureStop");

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate IntPtr d_alccaptureclosedevice(IntPtr device);
        public static d_alccaptureclosedevice CaptureCloseDevice = FL.LoadFunction<d_alccaptureclosedevice>(AL.NativeLibrary, "alcCaptureCloseDevice");

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate bool d_alcisextensionpresent(IntPtr device, string extensionName);
        public static d_alcisextensionpresent IsExtensionPresent = FL.LoadFunction<d_alcisextensionpresent>(AL.NativeLibrary, "alcIsExtensionPresent");

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate IntPtr d_alcgetstring(IntPtr device, int p);
        public static d_alcgetstring alcGetString = FL.LoadFunction<d_alcgetstring>(AL.NativeLibrary, "alcGetString");

        public static string GetString(IntPtr device, int p)
        {
            return Marshal.PtrToStringAnsi(alcGetString(device, p));
        }

        public static string GetString(IntPtr device, ALCGetString p)
        {
            return GetString(device, (int)p);
        }

#if IOS
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void d_alcsuspendcontext(IntPtr context);
        public static d_alcsuspendcontext SuspendContext = FuncLoader.LoadFunction<d_alcsuspendcontext>(AL.NativeLibrary, "alcSuspendContext");

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void d_alcprocesscontext(IntPtr context);
        public static d_alcprocesscontext ProcessContext = FuncLoader.LoadFunction<d_alcprocesscontext>(AL.NativeLibrary, "alcProcessContext");
#endif

#if ANDROID
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void d_alcdevicepausesoft(IntPtr device);
        public static d_alcdevicepausesoft DevicePause = FuncLoader.LoadFunction<d_alcdevicepausesoft>(AL.NativeLibrary, "alcDevicePauseSOFT");

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void d_alcdeviceresumesoft(IntPtr device);
        public static d_alcdeviceresumesoft DeviceResume = FuncLoader.LoadFunction<d_alcdeviceresumesoft>(AL.NativeLibrary, "alcDeviceResumeSOFT");
#endif
    }
}
