// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

namespace MonoGame.Framework.Graphics
{
    /// <summary>
    /// Defines types of surface formats.
    /// </summary>
    public enum SurfaceFormat
    {
        /// <summary>
        /// Unsigned 32-bit RGBA pixel format. 8 bits per channel. 
        /// </summary>
        Color,

        /// <summary>
        /// Unsigned 24-bit RGB pixel format. 8 bits per channel. 
        /// </summary>
        Rgb24,
        
        /// <summary>
        /// Unsigned 16-bit BGR pixel format. 5 bits for blue, 6 bits for green, and 5 bits for red.   
        /// </summary>
        Bgr565,
        
        /// <summary>
        /// Unsigned 16-bit BGRA pixel format. 5 bits for each color and 1 bit for alpha.
        /// </summary>
        Bgra5551,
        
        /// <summary>
        /// Unsigned 16-bit BGRA pixel format. 4 bits per channel.
        /// </summary>
        Bgra4444,
        
        /// <summary>
        /// DXT1; compressed texture format. Surface dimensions must be a multiple 4.
        /// </summary>
        Dxt1,
        
        /// <summary>
        /// DXT3; compressed texture format. Surface dimensions must be a multiple 4.
        /// </summary>
        Dxt3, 
        
        /// <summary>
        /// DXT5; compressed texture format. Surface dimensions must be a multiple 4.
        /// </summary>
        Dxt5,
        
        /// <summary>
        /// Signed 16-bit bump-map format. 8 bits for <c>U</c> and <c>V</c> data.
        /// </summary>
        NormalizedByte2,
        
        /// <summary>
        /// Signed 32-bit bump-map format. 8 bits per channel.
        /// </summary>
        NormalizedByte4,
        
        /// <summary>
        /// Unsigned 32-bit RGBA pixel format. 10 bits for each color and 2 bits for alpha.
        /// </summary>
        Rgba1010102,
        
        /// <summary>
        /// Unsigned 32-bit RG pixel format. 16 bits per channel.
        /// </summary>
        Rg32,
        
        /// <summary>
        /// Unsigned 64-bit RGBA pixel format. 16 bits per channel.
        /// </summary>
        Rgba64,
        
        /// <summary>
        /// Unsigned A 8-bit format. 8 bits for alpha channel.
        /// </summary>
        Alpha8,
        
        /// <summary>
        /// IEEE 32-bit R float format. 32 bits for red channel.
        /// </summary>
        Single,
        
        /// <summary>
        /// IEEE 64-bit RG float format. 32 bits per channel.
        /// </summary>
        Vector2,
        
        /// <summary>
        /// IEEE 128-bit RGBA float format. 32 bits per channel.
        /// </summary>
        Vector4,
        
        /// <summary>
        /// Float 16-bit R format. 16 bits for red channel.   
        /// </summary>
        HalfSingle,
        
        /// <summary>
        /// Float 32-bit RG format. 16 bits per channel. 
        /// </summary>
        HalfVector2,
        
        /// <summary>
        /// Float 64-bit ARGB format. 16 bits per channel. 
        /// </summary>
        HalfVector4,
        
        /// <summary>
        /// Float pixel format for high dynamic range data.
        /// </summary>
        HdrBlendable,

        #region Extensions

        /// <summary>
        /// For compatibility with WPF D3DImage.
        /// </summary>
        Bgr32,     // B8G8R8X8

        /// <summary>
        /// For compatibility with WPF D3DImage.
        /// </summary>
        Bgra32,    // B8G8R8A8    

        /// <summary>
        /// Unsigned 32-bit RGBA sRGB pixel format. 8 bits per channel.
        /// </summary>
        Rgba32SRgb = 30,

        /// <summary>
        /// Unsigned 32-bit sRGB pixel format. 8 bits per channel and 8 bits are unused.
        /// </summary>
        Bgr32SRgb = 31,

        /// <summary>
        /// Unsigned 32-bit sRGB pixel format. 8 bits per channel.
        /// </summary>
        Bgra32SRgb = 32,

        /// <summary>
        /// DXT1; compressed sRGB texture format. Surface dimensions must be a multiple of 4.
        /// </summary>
        Dxt1SRgb = 33,

        /// <summary>
        /// DXT3; compressed sRGB texture format. Surface dimensions must be a multiple of 4.
        /// </summary>
        Dxt3SRgb = 34,

        /// <summary>
        /// DXT5; compressed sRGB texture format. Surface dimensions must be a multiple of 4.
        /// </summary>
        Dxt5SRgb = 35,

        /// <summary>
        /// PowerVR texture compression format (iOS and Android).
        /// </summary>
        RgbPvrtc2Bpp = 50,
        
        /// <summary>
        /// PowerVR texture compression format (iOS and Android).
        /// </summary>
        RgbPvrtc4Bpp = 51,
        
        /// <summary>
        /// PowerVR texture compression format (iOS and Android).
        /// </summary>
        RgbaPvrtc2Bpp = 52,
        
        /// <summary>
        /// PowerVR texture compression format (iOS and Android).
        /// </summary>
        RgbaPvrtc4Bpp = 53,
        
        /// <summary>
        /// Ericcson Texture Compression (Android).
        /// </summary>
        RgbEtc1 = 60,
        
        /// <summary>
        /// DXT1 version where 1-bit alpha is used.
        /// </summary>
        Dxt1a = 70,
        
        /// <summary>
        /// ATC/ATITC compression (Android).
        /// </summary>
        RgbaAtcExplicitAlpha =  80,
        
        /// <summary>
        /// ATC/ATITC compression (Android).
        /// </summary>
        RgbaAtcInterpolatedAlpha = 81,

        /// <summary>
        /// Etc2 RGB8 (Android/iOS with OpenGL ES 3.0).
        /// </summary>
        Rgb8Etc2 = 90,

        /// <summary>
        /// Etc2 SRGB8 (Android/iOS with OpenGL ES 3.0).
        /// </summary>
        Srgb8Etc2 = 91,

        /// <summary>
        /// Etc2 RGB8A1 (Android/iOS with OpenGL ES 3.0).
        /// </summary>
        Rgb8A1Etc2 = 92,

        /// <summary>
        /// Etc2 SRGB8A1 (Android/iOS with OpenGL ES 3.0).
        /// </summary>
        Srgb8A1Etc2 = 93,

        /// <summary>
        /// Etc2 RGBA8 EAC (Android/iOS with OpenGL ES 3.0).
        /// </summary>
        Rgba8Etc2 = 94,

        /// <summary>
        /// Etc2 SRGB8A8 EAC (Android/iOS with OpenGL ES 3.0).
        /// </summary>
        SRgb8A8Etc2 = 95,

        #endregion
    }
}

