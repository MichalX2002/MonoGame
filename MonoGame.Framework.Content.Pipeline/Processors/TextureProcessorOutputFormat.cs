// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using MonoGame.Framework.PackedVector;

namespace MonoGame.Framework.Content.Pipeline.Processors
{
    /// <summary>
    /// Specifies the target output type of the <see cref="TextureProcessor"/>.
    /// </summary>
    public enum TextureProcessorOutputFormat
    {
        /// <summary>
        /// The input texture is
        /// converted to <see cref="Framework.Color"/> by the processor.
        /// <para>Typically used for 2D graphics and overlays.</para>
        /// </summary>
        Color,

        /// <summary>
        /// The input texture is compressed to an appropriate DXT format by the processor. 
        /// If the input texture contains fractional alpha values,
        /// it is converted to DXT5 format (8 bits per texel);
        /// otherwise it is converted to DXT1 (4 bits per texel).
        /// This conversion reduces the resource's size in GPU memory.
        /// <para>Typically used for 3D textures such as 3D model textures.</para>
        /// </summary>
        DxtCompressed,

        /// <summary>
        /// The input texture is not changed by the processor.
        /// <para>Typically used for textures processed by an external tool.</para>
        /// </summary>
        NoChange,

        /// <summary>
        /// The input texture is compressed to an appropriate format for the target platform.
        /// This can include PVRTC for iOS, DXT for desktop,
        /// Windows 8 and Windows Phone 8, and ETC1 or <see cref="Bgra4444"/> for Android.
        /// </summary>
        Compressed,

        /// <summary>
        /// The pixel depth of the input texture is reduced to <see cref="Bgr565"/>
        /// for opaque textures, otherwise it's reduced to <see cref="Bgra4444"/>.
        /// </summary>
        Color16Bit,

        /// <summary>
        /// The input texture is compressed with ETC1 texture compression. 
        /// Used on Android platforms.
        /// </summary>
        Etc1Compressed,

        /// <summary>
        /// The input texture is compressed with PVR texture compression. 
        /// Used on iOS and some Android platforms.
        /// </summary>
        PvrCompressed,

        /// <summary>
        /// The input texture is compressed with ATI texture compression.
        /// Used on some Android platforms.
        /// </summary>
        AtcCompressed
    }
}
