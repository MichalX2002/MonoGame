// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.IO;
using MonoGame.Framework.Content.Pipeline.Graphics;
using MonoGame.Framework.Graphics;
using MonoGame.Framework.Vector;

namespace MonoGame.Framework.Content.Pipeline
{
    /// <summary>
    /// Loader class for DDS format image files.
    /// </summary>
    internal class DdsLoader
    {
        [Flags]
        private enum Ddsd : uint
        {
            /// <summary>
            /// Required in every DDS file.
            /// </summary>
            Caps = 0x1,

            /// <summary>
            /// Required in every DDS file.
            /// </summary>
            Height = 0x2,

            /// <summary>
            /// Required in every DDS file.
            /// </summary>
            Width = 0x4,

            /// <summary>
            /// Required when pitch is provided for an uncompressed texture
            /// </summary>
            Pitch = 0x8,

            /// <summary>
            /// Required in every DDS file.
            /// </summary>
            PixelFormat = 0x1000,

            /// <summary>
            /// Required in a mipmapped texture.
            /// </summary>
            MipMapCount = 0x2000,

            /// <summary>
            /// Required when pitch is provided for a compressed texture.
            /// </summary>
            LinearSize = 0x80000,

            /// <summary>
            /// Required in a depth texture.
            /// </summary>
            Depth = 0x800000
        }

        [Flags]
        private enum DdsCaps : uint
        {
            /// <summary>
            /// Optional; must be used on any file that contains more than one surface 
            /// (a mipmap, a cubic environment map, or mipmapped volume texture).
            /// </summary>
            Complex = 0x8,

            /// <summary>
            /// Optional; should be used for a mipmap.
            /// </summary>
            MipMap = 0x400000,

            /// <summary>
            /// Required.
            /// </summary>
            Texture = 0x1000,
        }

        [Flags]
        private enum DdsCaps2 : uint
        {
            Cubemap = 0x200,
            CubemapPositiveX = 0x400,
            CubemapNegativeX = 0x800,
            CubemapPositiveY = 0x1000,
            CubemapNegativeY = 0x2000,
            CubemapPositiveZ = 0x4000,
            CubemapNegativeZ = 0x8000,
            Volume = 0x200000,

            CubemapAllFaces = 
                Cubemap |
                CubemapPositiveX | CubemapNegativeX |
                CubemapPositiveY | CubemapNegativeY |
                CubemapPositiveZ | CubemapNegativeZ,
        }

        [Flags]
        private enum Ddpf : uint
        {
            AlphaPixels = 0x1,
            Alpha = 0x2,
            FourCC = 0x4,
            Rgb = 0x40,
            Yuv = 0x200,
            Luminance = 0x20000,
        }

        private static uint MakeFourCC(char c1, char c2, char c3, char c4)
        {
            return ((uint)c1 << 24) | ((uint)c2 << 16) | ((uint)c3 << 8) | c4;
        }

        private static uint MakeFourCC(string cc)
        {
            return ((uint)cc[0] << 24) | ((uint)cc[1] << 16) | ((uint)cc[2] << 8) | cc[3];
        }

        private enum FourCC : uint
        {
            A32B32G32R32F = 116,
            Dxt1 = 0x31545844,
            Dxt2 = 0x32545844,
            Dxt3 = 0x33545844,
            Dxt4 = 0x34545844,
            Dxt5 = 0x35545844,
            Dx10 = 0x30315844,
        }

        private struct DdsPixelFormat
        {
            public uint dwSize;
            public Ddpf dwFlags;
            public FourCC dwFourCC;
            public uint dwRgbBitCount;
            public uint dwRBitMask;
            public uint dwGBitMask;
            public uint dwBBitMask;
            public uint dwABitMask;
        }

        private struct DdsHeader
        {
            public uint dwSize;
            public Ddsd dwFlags;
            public uint dwHeight;
            public uint dwWidth;
            public uint dwPitchOrLinearSize;
            public uint dwDepth;
            public uint dwMipMapCount;
            public DdsPixelFormat ddspf;
            public DdsCaps dwCaps;
            public DdsCaps2 dwCaps2;
        }

        private static SurfaceFormat GetSurfaceFormat(ref DdsPixelFormat pixelFormat, out bool rbSwap)
        {
            rbSwap = false;
            if (pixelFormat.dwFlags.HasFlag(Ddpf.FourCC))
            {
                switch (pixelFormat.dwFourCC)
                {
                    case FourCC.A32B32G32R32F:
                        return SurfaceFormat.Vector4;
                    case FourCC.Dxt1:
                        return SurfaceFormat.Dxt1;
                    case FourCC.Dxt2:
                        throw new ContentLoadException("Unsupported compression format DXT2");
                    case FourCC.Dxt3:
                        return SurfaceFormat.Dxt3;
                    case FourCC.Dxt4:
                        throw new ContentLoadException("Unsupported compression format DXT4");
                    case FourCC.Dxt5:
                        return SurfaceFormat.Dxt5;
                }
            }
            else if (pixelFormat.dwFlags.HasFlag(Ddpf.Rgb))
            {
                // Uncompressed format
                if (pixelFormat.dwFlags.HasFlag(Ddpf.AlphaPixels))
                {
                    // Format contains RGB and A
                    if (pixelFormat.dwRgbBitCount == 16)
                    {
                        if (pixelFormat.dwABitMask == 0xF)
                        {
                            rbSwap = pixelFormat.dwBBitMask == 0xF0;
                            return SurfaceFormat.Bgra4444;
                        }
                        rbSwap = pixelFormat.dwBBitMask == 0x3E;
                        return SurfaceFormat.Bgra5551;
                    }
                    else if (pixelFormat.dwRgbBitCount == 32)
                    {
                        rbSwap = pixelFormat.dwBBitMask == 0xFF;
                        return SurfaceFormat.Rgba32;
                    }
                    throw new ContentLoadException("Unsupported RGBA pixel format");
                }
                else
                {
                    // Format contains RGB only
                    if (pixelFormat.dwRgbBitCount == 16)
                    {
                        rbSwap = pixelFormat.dwBBitMask == 0x1F;
                        return SurfaceFormat.Bgr565;
                    }
                    else if (pixelFormat.dwRgbBitCount == 24)
                    {
                        rbSwap = pixelFormat.dwBBitMask == 0xFF;
                        return SurfaceFormat.Rgba32;
                    }
                    else if (pixelFormat.dwRgbBitCount == 32)
                    {
                        rbSwap = pixelFormat.dwBBitMask == 0xFF;
                        return SurfaceFormat.Rgba32;
                    }
                    throw new ContentLoadException("Unsupported RGB pixel format");
                }
            }
            //else if (pixelFormat.dwFlags.HasFlag(Ddpf.Luminance))
            //{
            //    return SurfaceFormat.Alpha8;
            //}
            throw new ContentLoadException("Unsupported pixel format");
        }

        private static BitmapContent CreateBitmapContent(SurfaceFormat format, int width, int height)
        {
            switch (format)
            {
                case SurfaceFormat.Rgba32:
                    return new PixelBitmapContent<Color>(width, height);

                case SurfaceFormat.Bgra4444:
                    return new PixelBitmapContent<Bgra4444>(width, height);

                case SurfaceFormat.Bgra5551:
                    return new PixelBitmapContent<Bgra5551>(width, height);

                case SurfaceFormat.Bgr565:
                    return new PixelBitmapContent<Bgr565>(width, height);

                case SurfaceFormat.Dxt1:
                    return new Dxt1BitmapContent(width, height);

                case SurfaceFormat.Dxt3:
                    return new Dxt3BitmapContent(width, height);

                case SurfaceFormat.Dxt5:
                    return new Dxt5BitmapContent(width, height);

                case SurfaceFormat.Vector4:
                    return new PixelBitmapContent<RgbaVector>(width, height);
            }
            throw new ContentLoadException("Unsupported SurfaceFormat " + format);
        }

        private static int GetBitmapSize(SurfaceFormat format, int width, int height)
        {
            // It is recommended that the dwPitchOrLinearSize field is ignored and we calculate it ourselves
            // https://msdn.microsoft.com/en-us/library/bb943991.aspx

            int pitch;
            int rows;
            switch (format)
            {
                case SurfaceFormat.Rgba32:
                case SurfaceFormat.Bgra4444:
                case SurfaceFormat.Bgra5551:
                case SurfaceFormat.Bgr565:
                case SurfaceFormat.Vector4:
                    pitch = width * format.GetSize();
                    rows = height;
                    break;

                case SurfaceFormat.Dxt1:
                case SurfaceFormat.Dxt3:
                case SurfaceFormat.Dxt5:
                    pitch = (width + 3) / 4 * format.GetSize();
                    rows = (height + 3) / 4;
                    break;

                default:
                    throw new ContentLoadException("Unsupported SurfaceFormat " + format);
            }

            return pitch * rows;
        }

        internal static TextureContent Import(string filename, ContentImporterContext context)
        {
            var identity = new ContentIdentity(filename);
            TextureContent output = null;

            using (var fileStream = new FileStream(filename, FileMode.Open, FileAccess.Read))
            using (var reader = new BinaryReader(fileStream))
            {
                // Signature ("DDS ")
                if (reader.ReadByte() != 0x44 ||
                    reader.ReadByte() != 0x44 ||
                    reader.ReadByte() != 0x53 ||
                    reader.ReadByte() != 0x20)
                    throw new ContentLoadException("Invalid file signature");

                var header = new DdsHeader
                {
                    // Read DDS_HEADER
                    dwSize = reader.ReadUInt32()
                };

                if (header.dwSize != 124)
                    throw new ContentLoadException("Invalid DDS_HEADER dwSize value");

                header.dwFlags = (Ddsd)reader.ReadUInt32();
                header.dwHeight = reader.ReadUInt32();
                header.dwWidth = reader.ReadUInt32();
                header.dwPitchOrLinearSize = reader.ReadUInt32();
                header.dwDepth = reader.ReadUInt32();
                header.dwMipMapCount = reader.ReadUInt32();
                // The next 11 DWORDs are reserved and unused
                for (int i = 0; i < 11; ++i)
                    reader.ReadUInt32();

                // Read DDS_PIXELFORMAT
                header.ddspf.dwSize = reader.ReadUInt32();
                if (header.ddspf.dwSize != 32)
                    throw new ContentLoadException("Invalid DDS_PIXELFORMAT dwSize value");

                header.ddspf.dwFlags = (Ddpf)reader.ReadUInt32();
                header.ddspf.dwFourCC = (FourCC)reader.ReadUInt32();
                header.ddspf.dwRgbBitCount = reader.ReadUInt32();
                header.ddspf.dwRBitMask = reader.ReadUInt32();
                header.ddspf.dwGBitMask = reader.ReadUInt32();
                header.ddspf.dwBBitMask = reader.ReadUInt32();
                header.ddspf.dwABitMask = reader.ReadUInt32();

                // Continue reading DDS_HEADER
                header.dwCaps = (DdsCaps)reader.ReadUInt32();
                header.dwCaps2 = (DdsCaps2)reader.ReadUInt32();

                reader.ReadUInt32(); // dwCaps3 unused
                reader.ReadUInt32(); // dwCaps4 unused
                reader.ReadUInt32(); // dwReserved2 unused

                // Check for the existence of the DDS_HEADER_DXT10 struct next
                if (header.ddspf.dwFlags == Ddpf.FourCC && header.ddspf.dwFourCC == FourCC.Dx10)
                    throw new ContentLoadException("Unsupported DDS_HEADER_DXT10 struct found");

                int faceCount = 1;
                int mipMapCount = (int)(header.dwCaps.HasFlag(DdsCaps.MipMap) ? header.dwMipMapCount : 1);
                if (header.dwCaps2.HasFlag(DdsCaps2.Cubemap))
                {
                    if (!header.dwCaps2.HasFlag(DdsCaps2.CubemapAllFaces))
                        throw new ContentLoadException("Incomplete cubemap in DDS file");
                    faceCount = 6;
                    output = new TextureCubeContent() { Identity = identity };
                }
                else
                {
                    output = new Texture2DContent() { Identity = identity };
                }

                var format = GetSurfaceFormat(ref header.ddspf, out bool rbSwap);

                for (int f = 0; f < faceCount; ++f)
                {
                    int w = (int)header.dwWidth;
                    int h = (int)header.dwHeight;
                    var mipMaps = new MipmapChain();
                    for (int m = 0; m < mipMapCount; ++m)
                    {
                        var content = CreateBitmapContent(format, w, h);
                        var byteCount = GetBitmapSize(format, w, h);
                        // A 24-bit format is slightly different
                        if (header.ddspf.dwRgbBitCount == 24)
                            byteCount = 3 * w * h;

                        var bytes = reader.ReadBytes(byteCount);
                        if (rbSwap)
                        {
                            switch (format)
                            {
                                case SurfaceFormat.Bgr565:
                                    ByteSwapBGR565(bytes);
                                    break;

                                case SurfaceFormat.Bgra4444:
                                    ByteSwapBGRA4444(bytes);
                                    break;

                                case SurfaceFormat.Bgra5551:
                                    ByteSwapBGRA5551(bytes);
                                    break;

                                case SurfaceFormat.Rgba32:
                                    if (header.ddspf.dwRgbBitCount == 32)
                                        ByteSwapRGBX(bytes);
                                    else if (header.ddspf.dwRgbBitCount == 24)
                                        ByteSwapRGB(bytes);
                                    break;
                            }
                        }
                        if ((format == SurfaceFormat.Rgba32) &&
                            header.ddspf.dwFlags.HasFlag(Ddpf.Rgb) && 
                            !header.ddspf.dwFlags.HasFlag(Ddpf.AlphaPixels))
                        {
                            // Fill or add alpha with opaque
                            if (header.ddspf.dwRgbBitCount == 32)
                                ByteFillAlpha(bytes);
                            else if (header.ddspf.dwRgbBitCount == 24)
                                ByteExpandAlpha(ref bytes);
                        }
                        content.SetPixelData(bytes);
                        mipMaps.Add(content);
                        w = Math.Max(1, w / 2);
                        h = Math.Max(1, h / 2);
                    }
                    output.Faces[f] = mipMaps;
                }
            }

            return output;
        }

        private static void ByteFillAlpha(byte[] bytes)
        {
            for (int i = 0; i < bytes.Length; i += 4)
                bytes[i + 3] = 255;
        }

        private static void ByteExpandAlpha(ref byte[] bytes)
        {
            var rgba = new byte[bytes.Length + (bytes.Length / 3)];
            for (int i = 0, j = 0; i < bytes.Length; i += 3, j += 4)
            {
                rgba[j] = bytes[i];
                rgba[j + 1] = bytes[i + 1];
                rgba[j + 2] = bytes[i + 2];
                rgba[j + 3] = 255;
            }
            bytes = rgba;
        }

        private static void ByteSwapRGB(byte[] bytes)
        {
            for (int i = 0; i < bytes.Length; i += 3)
            {
                byte r = bytes[i];
                bytes[i] = bytes[i + 2];
                bytes[i + 2] = r;
            }
        }

        private static void ByteSwapRGBX(byte[] bytes)
        {
            for (int i = 0; i < bytes.Length; i += 4)
            {
                byte r = bytes[i];
                bytes[i] = bytes[i + 2];
                bytes[i + 2] = r;
            }
        }

        private static void ByteSwapBGRA4444(byte[] bytes)
        {
            for (int i = 0; i < bytes.Length; i += 2)
            {
                var r = bytes[i] & 0xF0;
                var b = bytes[i + 1] & 0xF0;
                bytes[i] = (byte)((bytes[i] & 0x0F) | b);
                bytes[i + 1] = (byte)((bytes[i + 1] & 0x0F) | r);
            }
        }

        private static void ByteSwapBGRA5551(byte[] bytes)
        {
            for (int i = 0; i < bytes.Length; i += 2)
            {
                var r = (bytes[i] & 0xF8) >> 3;
                var b = (bytes[i + 1] & 0x3E) >> 1;
                bytes[i] = (byte)((bytes[i] & 0x07) | (b << 3));
                bytes[i + 1] = (byte)((bytes[i + 1] & 0xC1) | (r << 1));
            }
        }

        private static void ByteSwapBGR565(byte[] bytes)
        {
            for (int i = 0; i < bytes.Length; i += 2)
            {
                var r = (bytes[i] & 0xF8) >> 3;
                var b = bytes[i + 1] & 0x1F;
                bytes[i] = (byte)((bytes[i] & 0x07) | (b << 3));
                bytes[i + 1] = (byte)((bytes[i + 1] & 0xE0) | r);
            }
        }

        internal static void WriteUncompressed(string filename, BitmapContent bitmapContent)
        {
            using (var fileStream = new FileStream(filename, FileMode.Create, FileAccess.Write))
            using (var writer = new BinaryWriter(fileStream))
            {
                // Signature ("DDS ")
                writer.Write((byte)0x44);
                writer.Write((byte)0x44);
                writer.Write((byte)0x53);
                writer.Write((byte)0x20);

                var header = new DdsHeader
                {
                    dwSize = 124,
                    dwFlags = Ddsd.Caps | Ddsd.Width | Ddsd.Height | Ddsd.Pitch | Ddsd.PixelFormat,
                    dwWidth = (uint)bitmapContent.Width,
                    dwHeight = (uint)bitmapContent.Height,
                    dwPitchOrLinearSize = (uint)(bitmapContent.Width * 4),
                    dwDepth = 0,
                    dwMipMapCount = 0
                };

                writer.Write(header.dwSize);
                writer.Write((uint)header.dwFlags);
                writer.Write(header.dwHeight);
                writer.Write(header.dwWidth);
                writer.Write(header.dwPitchOrLinearSize);
                writer.Write(header.dwDepth);
                writer.Write(header.dwMipMapCount);

                // 11 unused and reserved DWORDS.
                for (int i = 0; i < 11; i++)
                    writer.Write((uint)0);

                if (!bitmapContent.TryGetFormat(out SurfaceFormat format) || format != SurfaceFormat.Rgba32)
                    throw new NotSupportedException("Unsupported bitmap content!");

                header.ddspf.dwSize = 32;
                header.ddspf.dwFlags = Ddpf.AlphaPixels | Ddpf.Rgb;
                header.ddspf.dwFourCC = 0;
                header.ddspf.dwRgbBitCount = 32;
                header.ddspf.dwRBitMask = 0x000000ff;
                header.ddspf.dwGBitMask = 0x0000ff00;
                header.ddspf.dwBBitMask = 0x00ff0000;
                header.ddspf.dwABitMask = 0xff000000;

                // Write the DDS_PIXELFORMAT
                writer.Write(header.ddspf.dwSize);
                writer.Write((uint)header.ddspf.dwFlags);
                writer.Write((uint)header.ddspf.dwFourCC);
                writer.Write(header.ddspf.dwRgbBitCount);
                writer.Write(header.ddspf.dwRBitMask);
                writer.Write(header.ddspf.dwGBitMask);
                writer.Write(header.ddspf.dwBBitMask);
                writer.Write(header.ddspf.dwABitMask);

                header.dwCaps = DdsCaps.Texture;
                header.dwCaps2 = 0;

                // Continue reading DDS_HEADER
                writer.Write((uint)header.dwCaps);
                writer.Write((uint)header.dwCaps2);

                // More reserved unused DWORDs.
                for (int i = 0; i < 3; i++)
                    writer.Write((uint)0);

                // Write out the face data.
                writer.Write(bitmapContent.GetPixelData());
            }
        }
    }
}
