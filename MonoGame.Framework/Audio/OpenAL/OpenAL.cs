// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.Runtime.InteropServices;
using Microsoft.Xna.Framework.Audio;
using MonoGame.Utilities;
using System.IO;

namespace MonoGame.OpenAL
{
    internal class AL
    {
        public static IntPtr NativeLibrary = GetNativeLibrary();

        private static IntPtr GetNativeLibrary()
        {
            var ret = IntPtr.Zero;

#if DESKTOPGL || DIRECTX
            // Load bundled library
            var assemblyLocation = Path.GetDirectoryName(typeof(AL).Assembly.Location);
            if (CurrentPlatform.OS == OS.Windows && Environment.Is64BitProcess)
                ret = FuncLoader.LoadLibrary(Path.Combine(assemblyLocation, "x64/soft_oal.dll"));
            else if (CurrentPlatform.OS == OS.Windows && !Environment.Is64BitProcess)
                ret = FuncLoader.LoadLibrary(Path.Combine(assemblyLocation, "x86/soft_oal.dll"));
            else if (CurrentPlatform.OS == OS.Linux && Environment.Is64BitProcess)
                ret = FuncLoader.LoadLibrary(Path.Combine(assemblyLocation, "x64/libopenal.so.1"));
            else if (CurrentPlatform.OS == OS.Linux && !Environment.Is64BitProcess)
                ret = FuncLoader.LoadLibrary(Path.Combine(assemblyLocation, "x86/libopenal.so.1"));
            else if (CurrentPlatform.OS == OS.MacOSX)
                ret = FuncLoader.LoadLibrary(Path.Combine(assemblyLocation, "libopenal.1.dylib"));

            // Load system library
            if (ret == IntPtr.Zero)
            {
                if (CurrentPlatform.OS == OS.Windows)
                    ret = FuncLoader.LoadLibrary("soft_oal.dll");
                else if (CurrentPlatform.OS == OS.Linux)
                    ret = FuncLoader.LoadLibrary("libopenal.so.1");
                else
                    ret = FuncLoader.LoadLibrary("libopenal.1.dylib");
            }
#elif ANDROID
            ret = FuncLoader.LoadLibrary("libopenal32.so");

            if (ret == IntPtr.Zero)
            {
                var appFilesDir = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
                var appDir = Path.GetDirectoryName(appFilesDir);
                var lib = Path.Combine(appDir, "lib", "libopenal32.so");

                ret = FuncLoader.LoadLibrary(lib);
            }
#else
            ret = FuncLoader.LoadLibrary("/System/Library/Frameworks/OpenAL.framework/OpenAL");
#endif

            return ret;
        }
        
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal delegate void d_alenable(int cap);
        internal static d_alenable Enable = FuncLoader.LoadFunction<d_alenable>(NativeLibrary, "alEnable");

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal delegate void d_albufferdata(uint bid, int format, IntPtr data, int size, int freq);
        internal static d_albufferdata alBufferData = FuncLoader.LoadFunction<d_albufferdata>(NativeLibrary, "alBufferData");

        internal static void BufferData(int bid, ALFormat format, IntPtr data, int size, int freq)
        {
            alBufferData((uint)bid, (int)format, data, size, freq);
        }

        internal static void BufferData(int bid, ALFormat format, byte[] data, int offset, int count, int freq)
        {
            var handle = GCHandle.Alloc(data, GCHandleType.Pinned);
            try
            {
                alBufferData((uint)bid, (int)format, handle.AddrOfPinnedObject() + offset, count, freq);
            }
            finally
            {
                handle.Free();
            }
        }

        internal static void BufferData(int bid, ALFormat format, short[] data, int offset, int count, int freq)
        {
            var handle = GCHandle.Alloc(data, GCHandleType.Pinned);
            try
            {
                IntPtr ptr = handle.AddrOfPinnedObject() + offset * sizeof(short);
                int size = count * sizeof(short);
                alBufferData((uint)bid, (int)format, ptr, size, freq);
            }
            finally
            {
                handle.Free();
            }
        }

        internal static void BufferData(int bid, ALFormat format, float[] data, int offset, int count, int freq)
        {
            var handle = GCHandle.Alloc(data, GCHandleType.Pinned);
            try
            {
                IntPtr ptr = handle.AddrOfPinnedObject() + offset * sizeof(float);
                int size = count * sizeof(float);
                alBufferData((uint)bid, (int)format, ptr, size, freq);
            }
            finally
            {
                handle.Free();
            }
        }

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal unsafe delegate void d_aldeletebuffers(int n, int* buffers);
        internal static d_aldeletebuffers alDeleteBuffers = FuncLoader.LoadFunction<d_aldeletebuffers>(NativeLibrary, "alDeleteBuffers");

        internal static void DeleteBuffers(int[] buffers)
        {
            DeleteBuffers(buffers.Length, ref buffers[0]);
        }

        internal unsafe static void DeleteBuffers(int n, ref int buffers)
        {
            fixed (int* pbuffers = &buffers)
            {
                alDeleteBuffers(n, pbuffers);
            }
        }

        internal unsafe static void DeleteBuffer(int buffer)
        {
            int* tmp = stackalloc int[1];
            tmp[0] = buffer;
            alDeleteBuffers(1, tmp);
        }
        
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal delegate void d_albufferi(int buffer, ALBufferi param, int value);
        internal static d_albufferi Bufferi = FuncLoader.LoadFunction<d_albufferi>(NativeLibrary, "alBufferi");

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal delegate void d_algetbufferi(int bid, ALGetBufferi param, out int value);
        internal static d_algetbufferi GetBufferi = FuncLoader.LoadFunction<d_algetbufferi>(NativeLibrary, "alGetBufferi");

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal delegate void d_albufferiv(int bid, ALBufferi param, int[] values);
        internal static d_albufferiv Bufferiv = FuncLoader.LoadFunction<d_albufferiv>(NativeLibrary, "alBufferiv");

        internal static void GetBuffer(int bid, ALGetBufferi param, out int value)
        {
            GetBufferi(bid, param, out value);
        }

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal unsafe delegate void d_algenbuffers(int count, int* buffers);
        internal static d_algenbuffers alGenBuffers = FuncLoader.LoadFunction<d_algenbuffers>(NativeLibrary, "alGenBuffers");

        internal static unsafe int GenBuffer()
        {
            int* tmp = stackalloc int[1];
            alGenBuffers(1, tmp);
            return tmp[0];
        }

        internal static unsafe int[] GenBuffers(int count)
        {
            int[] ret = new int[count];
            fixed(int* ptr = ret)
            {
                alGenBuffers(count, ptr);
            }
            return ret;
        }

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal delegate void d_algensources(int n, uint[] sources);
        internal static d_algensources alGenSources = FuncLoader.LoadFunction<d_algensources>(NativeLibrary, "alGenSources");


        internal static void GenSources(int[] sources)
        {
            uint[] tmp = new uint[sources.Length];
            alGenSources(tmp.Length, tmp);
            for (int i = 0; i < tmp.Length; i++)
                sources[i] = (int)tmp[i];
        }

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal delegate ALError d_algeterror();
        internal static d_algeterror GetError = FuncLoader.LoadFunction<d_algeterror>(NativeLibrary, "alGetError");

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal delegate bool d_alisbuffer(uint buffer);
        internal static d_alisbuffer alIsBuffer = FuncLoader.LoadFunction<d_alisbuffer>(NativeLibrary, "alIsBuffer");

        internal static bool IsBuffer(int buffer)
        {
            return alIsBuffer((uint)buffer);
        }

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal delegate void d_alsourcepause(uint source);
        internal static d_alsourcepause alSourcePause = FuncLoader.LoadFunction<d_alsourcepause>(NativeLibrary, "alSourcePause");

        internal static void SourcePause(int source)
        {
            alSourcePause((uint)source);
        }

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal delegate void d_alsourceplay(uint source);
        internal static d_alsourceplay alSourcePlay = FuncLoader.LoadFunction<d_alsourceplay>(NativeLibrary, "alSourcePlay");

        internal static void SourcePlay(int source)
        {
            alSourcePlay((uint)source);
        }

        internal static string GetErrorString(ALError errorCode)
        {
            return errorCode.ToString();
        }

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal delegate bool d_alissource(int source);
        internal static d_alissource IsSource = FuncLoader.LoadFunction<d_alissource>(NativeLibrary, "alIsSource");
        
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal delegate void d_aldeletesources(int n, ref int sources);
        internal static d_aldeletesources alDeleteSources = FuncLoader.LoadFunction<d_aldeletesources>(NativeLibrary, "alDeleteSources");

        internal static void DeleteSource(int source)
        {
            alDeleteSources(1, ref source);
        }
        
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal delegate void d_alsourcestop(int sourceId);
        internal static d_alsourcestop SourceStop = FuncLoader.LoadFunction<d_alsourcestop>(NativeLibrary, "alSourceStop");

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal delegate void d_alsourcei(int sourceId, int i, int a);
        internal static d_alsourcei alSourcei = FuncLoader.LoadFunction<d_alsourcei>(NativeLibrary, "alSourcei");
        
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal delegate void d_alsource3i(int sourceId, ALSourcei i, int a, int b, int c);
        internal static d_alsource3i alSource3i = FuncLoader.LoadFunction<d_alsource3i>(NativeLibrary, "alSource3i");

        internal static void Source(int sourceId, ALSourcei i, int a)
        {
            alSourcei(sourceId, (int)i, a);
        }

        internal static void Source(int sourceId, ALSourceb i, bool a)
        {
            alSourcei(sourceId, (int)i, a ? 1 : 0);
        }

        internal static void Source(int sourceId, ALSource3f i, float x, float y, float z)
        {
            alSource3f(sourceId, i, x, y, z);
        }

        internal static void Source(int sourceId, ALSourcef i, float dist)
        {
            alSourcef(sourceId, i, dist);
        }
        
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal delegate void d_alsourcef(int sourceId, ALSourcef i, float a);
        internal static d_alsourcef alSourcef = FuncLoader.LoadFunction<d_alsourcef>(NativeLibrary, "alSourcef");
        
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal delegate void d_alsource3f(int sourceId, ALSource3f i, float x, float y, float z);
        internal static d_alsource3f alSource3f = FuncLoader.LoadFunction<d_alsource3f>(NativeLibrary, "alSource3f");
        
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal delegate void d_algetsourcei(int sourceId, ALGetSourcei i, out int state);
        internal static d_algetsourcei GetSource = FuncLoader.LoadFunction<d_algetsourcei>(NativeLibrary, "alGetSourcei");

        internal static ALSourceState GetSourceState(int sourceId)
        {
            GetSource(sourceId, ALGetSourcei.SourceState, out int state);
            return (ALSourceState)state;
        }

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal delegate void d_algetlistener3f(ALListener3f param, out float value1, out float value2, out float value3);
        internal static d_algetlistener3f GetListener = FuncLoader.LoadFunction<d_algetlistener3f>(NativeLibrary, "alGetListener3f");
        
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal delegate void d_aldistancemodel(ALDistanceModel model);
        internal static d_aldistancemodel DistanceModel = FuncLoader.LoadFunction<d_aldistancemodel>(NativeLibrary, "alDistanceModel");
        
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal delegate void d_aldopplerfactor(float value);
        internal static d_aldopplerfactor DopplerFactor = FuncLoader.LoadFunction<d_aldopplerfactor>(NativeLibrary, "alDopplerFactor");
        
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal unsafe delegate void d_alsourcequeuebuffers(int sourceId, int numEntries, int* buffers);
        internal static d_alsourcequeuebuffers alSourceQueueBuffers = FuncLoader.LoadFunction<d_alsourcequeuebuffers>(NativeLibrary, "alSourceQueueBuffers");
        
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal unsafe delegate void d_alsourceunqueuebuffers(int sourceId, int numEntries, int* salvaged);
        internal static d_alsourceunqueuebuffers alSourceUnqueueBuffers = FuncLoader.LoadFunction<d_alsourceunqueuebuffers>(NativeLibrary, "alSourceUnqueueBuffers");
        
        internal static unsafe void SourceQueueBuffers(int sourceId, int numEntries, int[] buffers)
        {
            fixed (int* ptr = &buffers[0])
            {
                AL.alSourceQueueBuffers(sourceId, numEntries, ptr);
            }
        }

        internal unsafe static void SourceQueueBuffer(int sourceId, int buffer)
        {
            AL.alSourceQueueBuffers(sourceId, 1, &buffer);
        }

        internal static unsafe void SourceUnqueueBuffers(int sourceID, int numEntries)
        {
            if (numEntries <= 0)
                throw new ArgumentOutOfRangeException(nameof(numEntries), "Must be greater than zero.");

            int* tmp = stackalloc int[numEntries];
            alSourceUnqueueBuffers(sourceID, numEntries, tmp);
        }

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal delegate int d_algetenumvalue(string enumName);
        internal static d_algetenumvalue alGetEnumValue = FuncLoader.LoadFunction<d_algetenumvalue>(NativeLibrary, "alGetEnumValue");

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal delegate bool d_alisextensionpresent(string extensionName);
        internal static d_alisextensionpresent IsExtensionPresent = FuncLoader.LoadFunction<d_alisextensionpresent>(NativeLibrary, "alIsExtensionPresent");

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal delegate IntPtr d_algetprocaddress(string functionName);
        internal static d_algetprocaddress alGetProcAddress = FuncLoader.LoadFunction<d_algetprocaddress>(NativeLibrary, "alGetProcAddress");

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate IntPtr d_algetstring(int p);
        private static d_algetstring alGetString = FuncLoader.LoadFunction<d_algetstring>(NativeLibrary, "alGetString");

        internal static string GetString(int p)
        {
            return Marshal.PtrToStringAnsi(alGetString(p));
        }

        internal static string Get(ALGetString p)
        {
            return GetString((int)p);
        }
    }
}
