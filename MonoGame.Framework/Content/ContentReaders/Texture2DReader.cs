// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.Buffers.Binary;
using System.IO;
using MonoGame.Framework.Graphics;

namespace MonoGame.Framework.Content
{
    internal class Texture2DReader : ContentTypeReader<Texture2D>
    {
        public Texture2DReader()
        {
        }

        protected internal override Texture2D Read(ContentReader reader, Texture2D existingInstance)
        {
            var surfaceFormat = (SurfaceFormat)reader.ReadInt32();
            int width = reader.ReadInt32();
            int height = reader.ReadInt32();
            int levelCount = reader.ReadInt32();
            int levelCountOutput = levelCount;

            // If the system does not fully support Power of Two textures,
            // skip any mip maps supplied with any non PoT textures.
            if (levelCount > 1 && !reader.GetGraphicsDevice().Capabilities.SupportsNonPowerOfTwo &&
                (!MathHelper.IsPowerOfTwo(width) || !MathHelper.IsPowerOfTwo(height)))
            {
                levelCountOutput = 1;
                System.Diagnostics.Debug.WriteLine(
                    "Device does not support non Power of Two textures. Skipping mipmaps.");
            }

            SurfaceFormat convertedFormat = surfaceFormat;
            switch (surfaceFormat)
            {
                case SurfaceFormat.Dxt1:
                case SurfaceFormat.Dxt1a:
                    if (!reader.GetGraphicsDevice().Capabilities.SupportsDxt1)
                        convertedFormat = SurfaceFormat.Rgba32;
                    break;

                case SurfaceFormat.Dxt1SRgb:
                    if (!reader.GetGraphicsDevice().Capabilities.SupportsDxt1)
                        convertedFormat = SurfaceFormat.Rgba32SRgb;
                    break;

                case SurfaceFormat.Dxt3:
                case SurfaceFormat.Dxt5:
                    if (!reader.GetGraphicsDevice().Capabilities.SupportsS3tc)
                        convertedFormat = SurfaceFormat.Rgba32;
                    break;

                case SurfaceFormat.Dxt3SRgb:
                case SurfaceFormat.Dxt5SRgb:
                    if (!reader.GetGraphicsDevice().Capabilities.SupportsS3tc)
                        convertedFormat = SurfaceFormat.Rgba32SRgb;
                    break;

                case SurfaceFormat.NormalizedByte4:
                    convertedFormat = SurfaceFormat.Rgba32;
                    break;
            }

            var texture = existingInstance ?? new Texture2D(
                reader.GetGraphicsDevice(), width, height, levelCountOutput > 1, convertedFormat);

            for (int level = 0; level < levelCountOutput; level++)
            {
                int levelDataSizeInBytes = reader.ReadInt32();
                using (var levelDataBuffer = reader.ContentManager.GetScratchBuffer(levelDataSizeInBytes))
                {
                    byte[] levelData = levelDataBuffer.Buffer;
                    if (reader.Read(levelData.AsSpan(0, levelDataSizeInBytes)) != levelDataSizeInBytes)
                        throw new InvalidDataException();

                    int levelWidth = Math.Max(width >> level, 1);
                    int levelHeight = Math.Max(height >> level, 1);

                    //Convert the image data if required
                    switch (surfaceFormat)
                    {
                        case SurfaceFormat.Dxt1:
                        case SurfaceFormat.Dxt1SRgb:
                        case SurfaceFormat.Dxt1a:
                            if (!reader.GetGraphicsDevice().Capabilities.SupportsDxt1 &&
                                convertedFormat == SurfaceFormat.Rgba32)
                            {
                                levelData = DxtUtil.DecompressDxt1(levelData, levelWidth, levelHeight);
                                levelDataSizeInBytes = levelData.Length;
                            }
                            break;

                        case SurfaceFormat.Dxt3:
                        case SurfaceFormat.Dxt3SRgb:
                            if (!reader.GetGraphicsDevice().Capabilities.SupportsS3tc &&
                                    convertedFormat == SurfaceFormat.Rgba32)
                            {
                                levelData = DxtUtil.DecompressDxt3(levelData, levelWidth, levelHeight);
                                levelDataSizeInBytes = levelData.Length;
                            }
                            break;

                        case SurfaceFormat.Dxt5:
                        case SurfaceFormat.Dxt5SRgb:
                            if (!reader.GetGraphicsDevice().Capabilities.SupportsS3tc &&
                                convertedFormat == SurfaceFormat.Rgba32)
                            {
                                levelData = DxtUtil.DecompressDxt5(levelData, levelWidth, levelHeight);
                                levelDataSizeInBytes = levelData.Length;
                            }
                            break;

                        case SurfaceFormat.NormalizedByte4:
                        {
                            int bytesPerPixel = surfaceFormat.GetSize();
                            int pitch = levelWidth * bytesPerPixel;
                            for (int y = 0; y < levelHeight; y++)
                            {
                                for (int x = 0; x < levelWidth; x++)
                                {
                                    int color = BinaryPrimitives.ReadInt32LittleEndian(
                                        levelData.AsSpan(y * pitch + x * bytesPerPixel));

                                    levelData[y * pitch + x * 4] = (byte)((color >> 16) & 0xff); //R:=W
                                    levelData[y * pitch + x * 4 + 1] = (byte)((color >> 8) & 0xff); //G:=V
                                    levelData[y * pitch + x * 4 + 2] = (byte)((color) & 0xff); //B:=U
                                    levelData[y * pitch + x * 4 + 3] = (byte)((color >> 24) & 0xff); //A:=Q
                                }
                            }
                        }
                        break;

#if OPENGL
                        case SurfaceFormat.Bgra5551:
                        {
                            // Shift the channels to suit OpenGL
                            int offset = 0;
                            for (int y = 0; y < levelHeight; y++)
                            {
                                for (int x = 0; x < levelWidth; x++)
                                {
                                    ushort pixel = BinaryPrimitives.ReadUInt16LittleEndian(levelData.AsSpan(offset));
                                    pixel = (ushort)(((pixel & 0x7FFF) << 1) | ((pixel & 0x8000) >> 15));
                                    levelData[offset] = (byte)pixel;
                                    levelData[offset + 1] = (byte)(pixel >> 8);
                                    offset += 2;
                                }
                            }
                        }
                        break;

                        case SurfaceFormat.Bgra4444:
                        {
                            // Shift the channels to suit OpenGL
                            int offset = 0;
                            for (int y = 0; y < levelHeight; y++)
                            {
                                for (int x = 0; x < levelWidth; x++)
                                {
                                    ushort pixel = BinaryPrimitives.ReadUInt16LittleEndian(levelData.AsSpan(offset));
                                    pixel = (ushort)(((pixel & 0x0FFF) << 4) | ((pixel & 0xF000) >> 12));
                                    levelData[offset] = (byte)pixel;
                                    levelData[offset + 1] = (byte)(pixel >> 8);
                                    offset += 2;
                                }
                            }
                        }
                        break;
#endif
                    }
                    texture.SetData(levelData.AsSpan(0, levelDataSizeInBytes), null, level);
                }
            }

            return texture;
        }
    }
}
