using MonoGame.OpenAL;
using System.Runtime.InteropServices;

namespace Microsoft.Xna.Framework.Audio
{
    internal class XRamExtension
    {
        internal enum XRamStorage
        {
            Automatic,
            Hardware,
            Accessible
        }

        private int RamSize;
        private int RamFree;
        private int StorageAuto;
        private int StorageHardware;
        private int StorageAccessible;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate bool SetBufferModeDelegate(int n, ref int buffers, int value);

        private SetBufferModeDelegate setBufferMode;

        public bool IsInitialized { get; private set; }

        internal XRamExtension()
        {
            IsInitialized = false;
            if (!AL.IsExtensionPresent("EAX-RAM"))
                return;

            RamSize = AL.alGetEnumValue("AL_EAX_RAM_SIZE");
            RamFree = AL.alGetEnumValue("AL_EAX_RAM_FREE");
            StorageAuto = AL.alGetEnumValue("AL_STORAGE_AUTOMATIC");
            StorageHardware = AL.alGetEnumValue("AL_STORAGE_HARDWARE");
            StorageAccessible = AL.alGetEnumValue("AL_STORAGE_ACCESSIBLE");
            if (RamSize == 0 || RamFree == 0 || StorageAuto == 0 || StorageHardware == 0 || StorageAccessible == 0)
                return;

            try
            {
                setBufferMode = Marshal.GetDelegateForFunctionPointer<SetBufferModeDelegate>(AL.alGetProcAddress("EAXSetBufferMode"));
                IsInitialized = true; // only initialize if setbuffermode doesn't throw
            }
            catch
            {
            }
        }

        public bool SetBufferMode(int id, XRamStorage storage)
        {
            return SetBufferMode(1, ref id, storage);
        }

        public bool SetBufferMode(int count, ref int id, XRamStorage storage)
        {
            if (storage == XRamStorage.Accessible)
                return setBufferMode(count, ref id, StorageAccessible);

            if (storage != XRamStorage.Hardware)
                return setBufferMode(count, ref id, StorageAuto);

            return setBufferMode(count, ref id, StorageHardware);
        }
    }
}
