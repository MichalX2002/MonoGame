﻿// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.ComponentModel;
using MonoGame.Framework.Content.Pipeline.Graphics;
using MonoGame.Framework.Graphics;
using MonoGame.Framework.Vectors;

namespace MonoGame.Framework.Content.Pipeline.Processors
{
    [ContentProcessor(DisplayName="Texture - MonoGame")]
    public class TextureProcessor : ContentProcessor<TextureContent, TextureContent>
    {
        public TextureProcessor()
        {
            ColorKeyColor = new Color(255, 0, 255, 255);
            ColorKeyEnabled = true;
            PremultiplyAlpha = true;
        }

        [DefaultValue(typeof(Color), "255,0,255,255")]
        public virtual Color ColorKeyColor { get; set; }

        [DefaultValue(true)]
        public virtual bool ColorKeyEnabled { get; set; }

        public virtual bool GenerateMipmaps { get; set; }

        [DefaultValue(true)]
        public virtual bool PremultiplyAlpha { get; set; }

        public virtual bool ResizeToPowerOfTwo { get; set; }

        public virtual bool MakeSquare { get; set; }

        public virtual TextureProcessorOutputFormat TextureFormat { get; set; }

        public override TextureContent Process(TextureContent input, ContentProcessorContext context)
        {
            if (input.Faces[0][0].TryGetFormat(out SurfaceFormat format))
            {
                // If it is already a compressed format, we cannot do anything else so just return it
                if (format.IsCompressedFormat())
                    return input;
            }

            if (ColorKeyEnabled || ResizeToPowerOfTwo || MakeSquare || PremultiplyAlpha || GenerateMipmaps)
            {
                // Convert to floating point format for modifications. Keep the original format for conversion back later on if required.
                var originalType = input.Faces[0][0].GetType();
                try
                {
                    input.ConvertBitmapType(typeof(PixelBitmapContent<RgbaVector>));
                }
                catch (Exception ex)
                {
                    context.Logger.LogImportantMessage("Could not convert input texture for processing. " + ex.ToString());
                    throw;
                }

                if (GenerateMipmaps)
                    input.GenerateMipmaps(true);

                for (int f = 0; f < input.Faces.Count; ++f)
                {
                    var face = input.Faces[f];
                    for (int m = 0; m < face.Count; ++m)
                    {
                        var bmp = (PixelBitmapContent<RgbaVector>)face[m];

                        if (ColorKeyEnabled)
                        {
                            var original = new RgbaVector(); // TODO: UNsafe.SkipInit
                            original.FromScaledVector(ColorKeyColor.ToScaledVector4());
                            bmp.ReplaceColor(original, new RgbaVector(0, 0, 0, 0));
                        }

                        if (ResizeToPowerOfTwo)
                        {
                            if (!GraphicsUtil.IsPowerOfTwo(bmp.Width) ||
                                !GraphicsUtil.IsPowerOfTwo(bmp.Height) ||
                                (MakeSquare && bmp.Height != bmp.Width))
                            {
                                var newWidth = GraphicsUtil.GetNextPowerOfTwo(bmp.Width);
                                var newHeight = GraphicsUtil.GetNextPowerOfTwo(bmp.Height);
                                if (MakeSquare)
                                    newWidth = newHeight = Math.Max(newWidth, newHeight);

                                var resized = new PixelBitmapContent<RgbaVector>(newWidth, newHeight);
                                BitmapContent.Copy(bmp, resized);
                                bmp = resized;
                            }
                        }
                        else if (MakeSquare && bmp.Height != bmp.Width)
                        {
                            var newSize = Math.Max(bmp.Width, bmp.Height);
                            var resized = new PixelBitmapContent<RgbaVector>(newSize, newSize);
                            BitmapContent.Copy(bmp, resized);
                        }

                        if (PremultiplyAlpha)
                        {
                            var pixels = bmp.GetPixelSpan();
                            for (int i = 0; i < pixels.Length; i++)
                            {
                                ref RgbaVector pixel = ref pixels[i];
                                pixel.R *= pixel.A;
                                pixel.G *= pixel.A;
                                pixel.B *= pixel.A;
                            }
                        }

                        face[m] = bmp;
                    }
                }

                // If no change to the surface format was desired, change it back now before it early outs
                if (TextureFormat == TextureProcessorOutputFormat.NoChange)
                    input.ConvertBitmapType(originalType);
            }

            // Get the texture profile for the platform and let it convert the texture.
            var texProfile = TextureProfile.ForPlatform(context.TargetPlatform);
            texProfile.ConvertTexture(context, input, TextureFormat, false);	

            return input;
        }
    }
}
