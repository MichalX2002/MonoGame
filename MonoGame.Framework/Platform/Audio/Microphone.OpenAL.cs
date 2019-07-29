// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

#if OPENAL
using MonoGame.OpenAL;
#if IOS || MONOMAC
using AudioToolbox;
using AudioUnit;
using AVFoundation;
#endif
#endif

namespace Microsoft.Xna.Framework.Audio
{
    /// <summary>
    /// Provides microphones capture features.  
    /// </summary>
    public sealed partial class Microphone
    {
        private IntPtr _captureDevice = IntPtr.Zero;

        internal void CheckALCError(string operation)
        {
            ALCError error = ALC.GetErrorForDevice(_captureDevice);
            if (error == ALCError.NoError)
                return;

            string errorFmt = "OpenAL Error: {0}";
            throw new NoAudioHardwareException(string.Format("{0} - {1}",
                operation, string.Format(errorFmt, error)));
        }
       
        internal static void PopulateCaptureDevices()
        {
            // clear microphones
            _allMicrophones.Clear();

            Default = null;

            // default device
            string defaultDevice = ALC.GetString(IntPtr.Zero, ALCGetString.CaptureDefaultDeviceSpecifier);

#if WINDOWS || DESKTOPGL
            // enumarating capture devices
            IntPtr deviceList = ALC.alcGetString(IntPtr.Zero, (int)ALCGetString.CaptureDeviceSpecifier);

            // we need to marshal a string array
            string deviceIdentifier = Marshal.PtrToStringAnsi(deviceList);
            while (!string.IsNullOrEmpty(deviceIdentifier))
            {  
                var microphone = new Microphone(deviceIdentifier);
                _allMicrophones.Add(microphone);          
                if (deviceIdentifier == defaultDevice)
                    Default = microphone;

                deviceList += deviceIdentifier.Length + 1;
                deviceIdentifier = Marshal.PtrToStringAnsi(deviceList);
            }
#else
            // Xamarin platforms don't provide a handle to alGetString that allow to marshal string
            // arrays so we're basically only adding the default microphone
            Microphone microphone = new Microphone(defaultDevice);
            _allMicrophones.Add(microphone);
            _default = microphone;
#endif
        }

        internal void PlatformStart()
        {
            if (State == MicrophoneState.Started)
                return;

            _captureDevice = ALC.CaptureOpenDevice(
                Name,
                (uint)SampleRate,
                ALFormat.Mono16,
                GetSampleSizeInBytes(_bufferDuration));
            CheckALCError("Failed to open capture device.");

            if (_captureDevice != IntPtr.Zero)
            {
                ALC.CaptureStart(_captureDevice);
                CheckALCError("Failed to start capture.");

                State = MicrophoneState.Started;
            }
			else
            {
                throw new NoAudioHardwareException("Failed to open capture device.");
            }
        }

        internal void PlatformStop()
        {
            if (State == MicrophoneState.Started)
            {
                ALC.CaptureStop(_captureDevice);
                CheckALCError("Failed to stop capture.");

                Update(); // to ensure that BufferReady doesn't get invoked after Stop()

                ALC.CaptureCloseDevice(_captureDevice);
                CheckALCError("Failed to close capture device.");

                _captureDevice = IntPtr.Zero;
            }
            State = MicrophoneState.Stopped;
        }

        internal int GetQueuedSampleCount()
        {
            if (State == MicrophoneState.Stopped || BufferReady == null)
                return 0;

            Span<int> queuedSampleCountBuffer = stackalloc int[1];
            ALC.GetInteger(_captureDevice, ALCGetInteger.CaptureSamples, 1, queuedSampleCountBuffer);
            CheckALCError("Failed to query capture samples.");

            return queuedSampleCountBuffer[0];
        }

        internal void Update()
        {
            int sampleCount = GetQueuedSampleCount();
            if (sampleCount > 0)
                BufferReady.Invoke(this, sampleCount);
        }

        internal unsafe int PlatformGetData<T>(Span<T> buffer) where T : unmanaged
        {
            int sampleCount = GetQueuedSampleCount();
            sampleCount = Math.Min(buffer.Length * sizeof(T) / 2, sampleCount); // 16bit adjust

            if (sampleCount > 0)
            {
                var bufferBytes = MemoryMarshal.AsBytes(buffer);
                ALC.CaptureSamples(_captureDevice, bufferBytes, sampleCount);
                CheckALCError("Failed to capture samples.");
                return sampleCount;
            }
            return 0;
        }
    }
}
