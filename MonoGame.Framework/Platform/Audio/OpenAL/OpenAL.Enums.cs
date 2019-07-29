
namespace MonoGame.Framework.Audio
{
    internal enum ALFormat
    {
        Mono8 = 0x1100,
        Mono16 = 0x1101,
        Stereo8 = 0x1102,
        Stereo16 = 0x1103,
        MonoIma4 = 0x1300,
        StereoIma4 = 0x1301,
        MonoMSAdpcm = 0x1302,
        StereoMSAdpcm = 0x1303,
        MonoFloat32 = 0x10010,
        StereoFloat32 = 0x10011,
    }

    internal enum ALError
    {
        NoError = 0,
        InvalidName = 0xA001,
        InvalidEnum = 0xA002,
        InvalidValue = 0xA003,
        InvalidOperation = 0xA004,
        OutOfMemory = 0xA005,
    }

    internal enum ALGetString
    {
        Extensions = 0xB004,
    }

    internal enum ALBufferi
    {
        UnpackBlockAlignmentSoft = 0x200C,
        LoopSoftPointsExt = 0x2015,
    }

    internal enum ALGetBufferi
    {
        Frequency = 0x2001,
        Bits = 0x2002,
        Channels = 0x2003,
        Size = 0x2004,
    }

    internal enum ALSourceb
    {
        Looping = 0x1007,
    }

    internal enum ALSourcei
    {
        SourceRelative = 0x202,
        Buffer = 0x1009,
        EfxDirectFilter = 0x20005,
        EfxAuxilarySendFilter = 0x20006,
    }

    internal enum ALSourcef
    {
        Pitch = 0x1003,
        Gain = 0x100A,
        ReferenceDistance = 0x1020
    }

    internal enum ALGetSourcei
    {
        SampleOffset = 0x1025,
        SourceState = 0x1010,
        BuffersQueued = 0x1015,
        BuffersProcessed = 0x1016,
    }

    internal enum ALSourceState
    {
        Initial = 0x1011,
        Playing = 0x1012,
        Paused = 0x1013,
        Stopped = 0x1014,
    }

    internal enum ALListener3f
    {
        Position = 0x1004,
    }

    internal enum ALSource3f
    {
        Position = 0x1004,
        Velocity = 0x1006,
    }

    internal enum ALDistanceModel
    {
        None = 0,
        InverseDistanceClamped = 0xD002,
    }

    internal enum ALCError
    {
        NoError = 0,
    }

    internal enum ALCGetString
    {
        CaptureDeviceSpecifier = 0x0310,
        CaptureDefaultDeviceSpecifier = 0x0311,
        Extensions = 0x1006,
    }

    internal enum ALCGetInteger
    {
        CaptureSamples = 0x0312,
    }

    internal enum EfxFilteri
    {
        FilterType = 0x8001,
    }

    internal enum EfxFilterf
    {
        LowpassGain = 0x0001,
        LowpassGainHF = 0x0002,
        HighpassGain = 0x0001,
        HighpassGainLF = 0x0002,
        BandpassGain = 0x0001,
        BandpassGainLF = 0x0002,
        BandpassGainHF = 0x0003,
    }

    internal enum EfxFilterType
    {
        None = 0x0000,
        Lowpass = 0x0001,
        Highpass = 0x0002,
        Bandpass = 0x0003,
    }

    internal enum EfxEffecti
    {
        EffectType = 0x8001,
        SlotEffect = 0x0001,
    }

    internal enum EfxEffectSlotf
    {
        EffectSlotGain = 0x0002,
    }

    internal enum EfxEffectf
    {
        EaxReverbDensity = 0x0001,
        EaxReverbDiffusion = 0x0002,
        EaxReverbGain = 0x0003,
        EaxReverbGainHF = 0x0004,
        EaxReverbGainLF = 0x0005,
        DecayTime = 0x0006,
        DecayHighFrequencyRatio = 0x0007,
        DecayLowFrequencyRation = 0x0008,
        EaxReverbReflectionsGain = 0x0009,
        EaxReverbReflectionsDelay = 0x000A,
        ReflectionsPain = 0x000B,
        LateReverbGain = 0x000C,
        LateReverbDelay = 0x000D,
        LateRevertPain = 0x000E,
        EchoTime = 0x000F,
        EchoDepth = 0x0010,
        ModulationTime = 0x0011,
        ModulationDepth = 0x0012,
        AirAbsorbsionHighFrequency = 0x0013,
        EaxReverbHFReference = 0x0014,
        EaxReverbLFReference = 0x0015,
        RoomRolloffFactor = 0x0016,
        DecayHighFrequencyLimit = 0x0017,
    }

    internal enum EfxEffectType
    {
        Reverb = 0x8000,
    }
}
