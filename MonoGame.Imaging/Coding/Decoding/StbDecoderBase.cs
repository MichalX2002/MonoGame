using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using MonoGame.Framework;
using MonoGame.Imaging.Utilities;
using MonoGame.Utilities.Memory;
using MonoGame.Utilities.PackedVector;
using StbSharp;

namespace MonoGame.Imaging.Decoding
{
    public abstract class StbDecoderBase : IImageDecoder
    {
        public abstract ImageFormat Format { get; }
        public virtual bool SupportsAnimation => false;

        #region DetectFormat Abstraction

        protected abstract bool TestFormat(StbImage.ReadContext context);

        private bool TryDetectFormat(StbImage.ReadContext context, out ImageFormat format)
        {
            if (TestFormat(context))
            {
                format = Format;
                return true;
            }
            format = default;
            return false;
        }

        public bool TryDetectFormat(ImageReadStream stream, out ImageFormat format)
        {
            return TryDetectFormat(stream.Context, out format);
        }

        public unsafe bool TryDetectFormat(ReadOnlySpan<byte> data, out ImageFormat format)
        {
            fixed (byte* ptr = &MemoryMarshal.GetReference(data))
            {
                var ctx = new StbImage.ReadContext(ptr, data.Length);
                return TryDetectFormat(ctx, out format);
            }
        }

        #endregion

        #region Identify Abstraction

        protected abstract bool GetInfo(StbImage.ReadContext context, out int w, out int h, out int n);

        private bool Identify(StbImage.ReadContext context, out ImageInfo info)
        {
            if (GetInfo(context, out int w, out int h, out int n))
            {
                info = new ImageInfo(w, h, n * 8, Format);
                return true;
            }
            info = default;
            return false;
        }

        public bool TryIdentify(
            ImageReadStream stream, out ImageInfo info)
        {
            return Identify(stream.Context, out info);
        }

        public unsafe bool TryIdentify(
            ReadOnlySpan<byte> data, out ImageInfo info)
        {
            fixed (byte* ptr = &MemoryMarshal.GetReference(data))
            {
                var ctx = new StbImage.ReadContext(ptr, data.Length);
                return Identify(ctx, out info);
            }
        }

        #endregion

        #region Decode Abstraction

        protected abstract unsafe bool ReadFirst(
            StbImage.ReadContext context, out void* data, ref StbImage.LoadState state);

        protected virtual unsafe bool ReadNext(
            StbImage.ReadContext context, out void* data, ref StbImage.LoadState state)
        {
            ImagingArgumentGuard.AssertAnimationSupported(this);
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets whether the given <see cref="IPixel"/> type can utilize 16-bit 
        /// channels when converting from decoded data.
        /// <para>
        /// This is used to prevent waste of memory as the library can 
        /// decode channels as 8-bit or 16-bit based on the type's precision.
        /// </para>
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool CanUtilize16BitData(Type type)
        {
            return type == typeof(Short4) 
                || type == typeof(NormalizedShort4) 
                || type == typeof(Rgba1010102) 
                || type == typeof(Rgb48)
                || type == typeof(Rgba64)
                || type == typeof(Vector2) 
                || type == typeof(Vector4)
                || type == typeof(RgbaVector);
        }

        private static StbImage.LoadState CreateLoadState(Type pixelType)
        {
            return new StbImage.LoadState
            {
                BitsPerChannel = CanUtilize16BitData(pixelType) ? 16 : 8
            };
        }

        private unsafe FrameCollection<TPixel> Decode<TPixel>(
            StbImage.ReadContext context, ImagingConfig config, int? frameLimit = null,
            DecodeProgressDelegate<TPixel> onProgress = null)
            where TPixel : unmanaged, IPixel
        {
            StbImage.LoadState state = CreateLoadState(typeof(TPixel));
            if (ReadFirst(context, out void* result, ref state))
            {
                return ParseResult(result, state, config, onProgress);
            }
            else
            {
                throw GetFailureException(context);
            }
        }

        protected unsafe FrameCollection<TPixel> ParseResult<TPixel>(
            void* result, StbImage.LoadState state, ImagingConfig config,
            DecodeProgressDelegate<TPixel> onProgress = null)
            where TPixel : unmanaged, IPixel
        {
            UnmanagedPointer<TPixel> dst = null;
            try
            {
                int pixelCount = state.Width * state.Height;
                dst = new UnmanagedPointer<TPixel>(pixelCount);
                TPixel* dstPtr = dst.Ptr;

                switch (state.Components)
                {
                    case 1:
                        var gray8Ptr = (Gray8*)result;
                        for (int p = 0; p < pixelCount; p++)
                            dstPtr[p].FromScaledVector4(gray8Ptr[p].ToScaledVector4());
                        break;

                    case 2:
                        var gray16Ptr = (Gray16*)result;
                        for (int p = 0; p < pixelCount; p++)
                            dstPtr[p].FromScaledVector4(gray16Ptr[p].ToScaledVector4());
                        break;

                    case 3:
                        var rgbPtr = (Rgb24*)result;
                        for (int p = 0; p < pixelCount; p++)
                            dstPtr[p].FromScaledVector4(rgbPtr[p].ToScaledVector4());
                        break;

                    case 4:
                        if (typeof(TPixel) == typeof(Rgba64))
                        {
                            if (state.BitsPerChannel == 16)
                            {
                                int bytes = pixelCount * sizeof(Rgba64);
                                Buffer.MemoryCopy(result, dstPtr, bytes, bytes);
                            }
                            else
                            {
                                var src8 = (Color*)result;
                                var dst8 = (Rgba64*)dstPtr;
                                for (int i = 0; i < pixelCount; i++)
                                    src8[i].FromColor(dst8[i]);
                            }
                        }
                        else if (typeof(TPixel) == typeof(Color))
                        {
                            if (state.BitsPerChannel == 16)
                            {
                                var src16 = (Rgba64*)result;
                                var dst16 = (Color*)dstPtr;
                                for (int i = 0; i < pixelCount; i++)
                                    src16[i].ToColor(ref dst16[i]);
                            }
                            else
                            {
                                int bytes = pixelCount * sizeof(Color);
                                Buffer.MemoryCopy(result, dstPtr, bytes, bytes);
                            }
                        }
                        else
                        {
                            if (state.BitsPerChannel == 16)
                            {
                                var src16 = (Rgba64*)result;
                                for (int p = 0; p < pixelCount; p++)
                                    dstPtr[p].FromScaledVector4(src16[p].ToScaledVector4());
                            }
                            else
                            {
                                var src8 = (Color*)result;
                                for (int p = 0; p < pixelCount; p++)
                                    dstPtr[p].FromScaledVector4(src8[p].ToScaledVector4());
                            }
                        }
                        break;
                }

                var buffer = new Image<TPixel>.Buffer(dst, state.Width, false);
                var image = new Image<TPixel>(buffer, state.Width, state.Height);

                var frames = GetLowInitialCapacityList<TPixel>();
                frames.Add(new ImageFrame<TPixel>(image, 0));
                return new FrameCollection<TPixel>(frames);
            }
            catch
            {
                dst?.Dispose();
                throw;
            }
            finally
            {
                CRuntime.free(result);
            }
        }

        /// <summary>
        /// Most images will only have one frame so
        /// there's no need for the default list capacity of 4.
        /// </summary>
        /// <typeparam name="TPixel"></typeparam>
        /// <returns></returns>
        private List<ImageFrame<TPixel>> GetLowInitialCapacityList<TPixel>()
            where TPixel : unmanaged, IPixel
        {
            return Format.SupportsAnimation
                ? new List<ImageFrame<TPixel>>()
                : new List<ImageFrame<TPixel>>(1);
        }

        protected Exception GetFailureException(StbImage.ReadContext context)
        {
            // TODO get some error message from the context
            return new Exception();
        }

        #endregion

        public virtual unsafe FrameCollection<TPixel> Decode<TPixel>(
            ImageReadStream stream, ImagingConfig config, int? frameLimit = null,
            DecodeProgressDelegate<TPixel> onProgress = null)
            where TPixel : unmanaged, IPixel
        {
            ImagingArgumentGuard.AssertValidFrameLimit(frameLimit, nameof(frameLimit));
            return Decode(stream.Context, config, frameLimit, onProgress);
        }

        public virtual unsafe FrameCollection<TPixel> Decode<TPixel>(
            ReadOnlySpan<byte> data, ImagingConfig config, int? frameLimit = null,
            DecodeProgressDelegate<TPixel> onProgress = null)
            where TPixel : unmanaged, IPixel
        {
            ImagingArgumentGuard.AssertValidFrameLimit(frameLimit, nameof(frameLimit));
            fixed (byte* ptr = &MemoryMarshal.GetReference(data))
            {
                var context = new StbImage.ReadContext(ptr, data.Length);
                return Decode(context, config, frameLimit, onProgress);
            }
        }
    }
}
