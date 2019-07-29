using MonoGame.OpenAL;
using System;
using System.Runtime.InteropServices;

namespace MonoGame.Framework.Audio
{
    internal class EffectsExtension
    {
        #region Effect API
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate void alGenEffectsDelegate(int n, out uint effect);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate void alDeleteEffectsDelegate(int n, ref int effect);

        //[UnmanagedFunctionPointer (CallingConvention.Cdecl)]
        //private delegate bool alIsEffectDelegate (uint effect);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate void alEffectfDelegate(uint effect, EfxEffectf param, float value);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate void alEffectiDelegate(uint effect, EfxEffecti param, int value);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate void alGenAuxiliaryEffectSlotsDelegate(int n, out uint effectslots);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate void alDeleteAuxiliaryEffectSlotsDelegate(int n, ref int effectslots);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate void alAuxiliaryEffectSlotiDelegate(uint slot, EfxEffecti type, uint effect);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate void alAuxiliaryEffectSlotfDelegate(uint slot, EfxEffectSlotf param, float value);

        private alGenEffectsDelegate alGenEffects;
        private alDeleteEffectsDelegate alDeleteEffects;
        //private alIsEffectDelegate alIsEffect;
        private alEffectfDelegate alEffectf;
        private alEffectiDelegate alEffecti;
        private alGenAuxiliaryEffectSlotsDelegate alGenAuxiliaryEffectSlots;
        private alDeleteAuxiliaryEffectSlotsDelegate alDeleteAuxiliaryEffectSlots;
        private alAuxiliaryEffectSlotiDelegate alAuxiliaryEffectSloti;
        private alAuxiliaryEffectSlotfDelegate alAuxiliaryEffectSlotf;
        #endregion

        #region Filter API
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private unsafe delegate void alGenFiltersDelegate(int n, [Out] uint* filters);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate void alFilteriDelegate(uint fid, EfxFilteri param, int value);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate void alFilterfDelegate(uint fid, EfxFilterf param, float value);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private unsafe delegate void alDeleteFiltersDelegate(int n, [In] uint* filters);

        private alGenFiltersDelegate alGenFilters;
        private alFilteriDelegate alFilteri;
        private alFilterfDelegate alFilterf;
        private alDeleteFiltersDelegate alDeleteFilters;
        #endregion

        internal bool IsInitialized { get; private set; }

        internal static IntPtr _device;

        private static EffectsExtension _instance;
        public static EffectsExtension Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new EffectsExtension();
                return _instance;
            }
        }

        internal EffectsExtension()
        {
            IsInitialized = false;
            if (!ALC.IsExtensionPresent(_device, "ALC_EXT_EFX"))
                return;

            alGenEffects = Marshal.GetDelegateForFunctionPointer<alGenEffectsDelegate>(AL.alGetProcAddress("alGenEffects"));
            alDeleteEffects = Marshal.GetDelegateForFunctionPointer<alDeleteEffectsDelegate>(AL.alGetProcAddress("alDeleteEffects"));
            alEffectf = Marshal.GetDelegateForFunctionPointer<alEffectfDelegate>(AL.alGetProcAddress("alEffectf"));
            alEffecti = Marshal.GetDelegateForFunctionPointer<alEffectiDelegate>(AL.alGetProcAddress("alEffecti"));
            alGenAuxiliaryEffectSlots = Marshal.GetDelegateForFunctionPointer<alGenAuxiliaryEffectSlotsDelegate>(AL.alGetProcAddress("alGenAuxiliaryEffectSlots"));
            alDeleteAuxiliaryEffectSlots = Marshal.GetDelegateForFunctionPointer<alDeleteAuxiliaryEffectSlotsDelegate>(AL.alGetProcAddress("alDeleteAuxiliaryEffectSlots"));
            alAuxiliaryEffectSloti = Marshal.GetDelegateForFunctionPointer<alAuxiliaryEffectSlotiDelegate>(AL.alGetProcAddress("alAuxiliaryEffectSloti"));
            alAuxiliaryEffectSlotf = Marshal.GetDelegateForFunctionPointer<alAuxiliaryEffectSlotfDelegate>(AL.alGetProcAddress("alAuxiliaryEffectSlotf"));

            alGenFilters = Marshal.GetDelegateForFunctionPointer<alGenFiltersDelegate>(AL.alGetProcAddress("alGenFilters"));
            alFilteri = Marshal.GetDelegateForFunctionPointer<alFilteriDelegate>(AL.alGetProcAddress("alFilteri"));
            alFilterf = Marshal.GetDelegateForFunctionPointer<alFilterfDelegate>(AL.alGetProcAddress("alFilterf"));
            alDeleteFilters = Marshal.GetDelegateForFunctionPointer<alDeleteFiltersDelegate>(AL.alGetProcAddress("alDeleteFilters"));

            IsInitialized = true;
        }


        //alEffecti(effect, EfxEffecti.FilterType, (int) EfxEffectType.Reverb);
        //ALHelper.CheckError("Failed to set Filter Type.");      


        internal void GenAuxiliaryEffectSlots(int count, out uint slot)
        {
            alGenAuxiliaryEffectSlots(count, out slot);
            ALHelper.CheckError("Failed to Genereate Aux slot");
        }

        internal void GenEffect(out uint effect)
        {
            alGenEffects(1, out effect);
            ALHelper.CheckError("Failed to Generate Effect.");
        }

        internal void DeleteAuxiliaryEffectSlot(int slot)
        {
            alDeleteAuxiliaryEffectSlots(1, ref slot);
        }

        internal void DeleteEffect(int effect)
        {
            alDeleteEffects(1, ref effect);
        }

        internal void BindEffectToAuxiliarySlot(uint slot, uint effect)
        {
            alAuxiliaryEffectSloti(slot, EfxEffecti.SlotEffect, effect);
            ALHelper.CheckError("Failed to bind Effect");
        }

        internal void AuxiliaryEffectSlot(uint slot, EfxEffectSlotf param, float value)
        {
            alAuxiliaryEffectSlotf(slot, param, value);
            ALHelper.CheckError("Failes to set " + param + " " + value);
        }

        internal void BindSourceToAuxiliarySlot(int SourceId, int slot, int slotnumber, int filter)
        {
            AL.alSource3i(SourceId, ALSourcei.EfxAuxilarySendFilter, slot, slotnumber, filter);
        }

        internal void Effect(uint effect, EfxEffectf param, float value)
        {
            alEffectf(effect, param, value);
            ALHelper.CheckError("Failed to set " + param + " " + value);
        }

        internal void Effect(uint effect, EfxEffecti param, int value)
        {
            alEffecti(effect, param, value);
            ALHelper.CheckError("Failed to set " + param + " " + value);
        }

        internal unsafe int GenFilter()
        {
            uint filter = 0;
            alGenFilters(1, &filter);
            return (int)filter;
        }

        internal void Filter(int sourceId, EfxFilteri filter, int EfxFilterType)
        {
            alFilteri((uint)sourceId, filter, EfxFilterType);
        }

        internal void Filter(int sourceId, EfxFilterf filter, float EfxFilterType)
        {
            alFilterf((uint)sourceId, filter, EfxFilterType);
        }

        internal void BindFilterToSource(int sourceId, int filterId)
        {
            AL.Source(sourceId, ALSourcei.EfxDirectFilter, filterId);
        }

        internal unsafe void DeleteFilter(int filterId)
        {
            alDeleteFilters(1, (uint*)&filterId);
        }
    }
}
