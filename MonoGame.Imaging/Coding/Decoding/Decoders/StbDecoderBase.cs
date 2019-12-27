using System;
using System.Runtime.InteropServices;
using System.Threading;
using MonoGame.Framework;
using MonoGame.Utilities.Memory;
using MonoGame.Utilities.PackedVector;
using StbSharp;
using static StbSharp.StbImage;

namespace MonoGame.Imaging.Coding.Decoding
{
    public abstract class StbDecoderBase : IImageDecoder, IImageInfoDetector
    {
        public abstract ImageFormat Format { get; }

        #region DetectFormat Abstraction

        protected abstract bool TestFormat(ReadContext context, ImagingConfig config);

        private ImageFormat DetectFormat(ReadContext context, ImagingConfig config)
        {
            if (TestFormat(context, config))
                return Format;
            return default;
        }

        public ImageFormat DetectFormat(ImageReadStream stream, ImagingConfig config)
        {
            return DetectFormat(stream.Context, config);
        }

        /*
        public unsafe bool TryDetectFormat(
            ReadOnlySpan<byte> data, ImagingConfig config, 
            CancellationToken cancellation, out ImageFormat format)
        {
            fixed (byte* ptr = &MemoryMarshal.GetReference(data))
            {
                var ctx = new ReadContext(ptr, data.Length, cancellation);
                return TryDetectFormat(ctx, config, out format);
            }
        }
        */

        #endregion

        #region Identify Abstraction

        protected abstract bool GetInfo(
            ReadContext context, ImagingConfig config, out int w, out int h, out int n);

        private ImageInfo Identify(ReadContext context, ImagingConfig config)
        {
            if (GetInfo(context, config, out int w, out int h, out int n))
                return new ImageInfo(w, h, n * 8, Format);
            return default;
        }

        public ImageInfo Identify(ImageReadStream stream, ImagingConfig config)
        {
            return Identify(stream.Context, config);
        }

        /*
        public unsafe bool TryIdentify(
            ReadOnlySpan<byte> data, ImagingConfig config,
            CancellationToken cancellation, out ImageInfo info)
        {
            fixed (byte* ptr = &MemoryMarshal.GetReference(data))
            {
                var ctx = new ReadContext(ptr, data.Length, cancellation);
                return Identify(ctx, config, out info);
            }
        }
        */

        #endregion

        #region Decode Abstraction

        protected abstract unsafe bool ReadFirst(
            ReadContext context, ImagingConfig config, out void* data, ref ReadState state);

        protected virtual unsafe bool ReadNext(
            ReadContext context, ImagingConfig config, out void* data, ref ReadState state)
        {
            ImagingArgumentGuard.AssertAnimationSupport(this, config);
            data = null;
            return false;
        }

        /// <summary>
        /// Gets whether the given <see cref="IPixel"/> type can utilize 16-bit 
        /// channels when converting from decoded data.
        /// <para>
        /// This is used to prevent waste of memory as Stb can 
        /// decode channels as 8-bit or 16-bit based on the <paramref name="type"/>'s precision.
        /// </para>
        /// </summary>
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

        private static ReadState CreateReadState<TPixel>(
            ImageDecoderState<TPixel> decoderState,
            BufferReadyCallback onBufferReady,
            DecodeProgressCallback<TPixel> onProgress)
            where TPixel : unmanaged, IPixel
        {
            var progressCallback = onProgress == null
                ? (ReadProgressCallback)null
                : (percentage, rect) =>
                {
                    Rectangle? rectangle = null;
                    if (rect.HasValue)
                    {
                        var r = rect.Value;
                        rectangle = new Rectangle(r.X, r.Y, r.W, r.H);
                    }
                    onProgress.Invoke(decoderState, percentage, rectangle);
                };

            return new ReadState(onBufferReady, progressCallback)
            {
                BitsPerChannel = CanUtilize16BitData(typeof(TPixel)) ? 16 : 8
            };
        }

        protected virtual unsafe Image<TPixel>.Buffer ParseStbResult<TPixel>(
            ImagingConfig config, void* result, ReadState state)
            where TPixel : unmanaged, IPixel
        {
            // TODO: use some Image.LoadPixels function instead

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
                                    src8[i].FromRgba64(dst8[i]);
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

                // TODO: create an object that wraps the result as IMemory and can dispose it
                return new Image<TPixel>.Buffer(dst, state.Width, leaveOpen: false);
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

        protected Exception GetFailureException(ReadContext context)
        {
            // TODO: get some error message from the context
            return new Exception();
        }

        #endregion

        #region IImageDecoder

        /*
        public virtual unsafe ImageCollection<TPixel, ImageFrame<TPixel>> Decode<TPixel>(
            ReadOnlySpan<byte> data, ImagingConfig config, int? frameLimit,
            CancellationToken cancellation, DecodeProgressCallback<TPixel> onProgress = null)
            where TPixel : unmanaged, IPixel
        {
            fixed (byte* ptr = &MemoryMarshal.GetReference(data))
            {
                var context = new ReadContext(ptr, data.Length, cancellation);
                return Decode(context, config, frameLimit, onProgress);
            }
        }
        */

        public unsafe ImageDecoderState<TPixel> DecodeFirst<TPixel>(
            ImageReadStream stream, ImagingConfig config, out Image<TPixel> image,
            DecodeProgressCallback<TPixel> onProgress = null)
            where TPixel : unmanaged, IPixel
        {
            var decoderState = new ImageDecoderState<TPixel>(this, stream, true);
            var readState = CreateReadState(decoderState, null, onProgress);
            if (ReadFirst(stream.Context, config, out void* result, ref readState))
            {
                var parsedBuffer = ParseStbResult<TPixel>(config, result, readState);
                image = new Image<TPixel>(parsedBuffer, readState.Width, readState.Height);

                decoderState.CurrentImage = image;
                decoderState.ImageIndex = 0;
                return decoderState;
            }
            else
                throw GetFailureException(stream.Context);
        }

        public unsafe bool DecodeNext<TPixel>(
            ImageDecoderState<TPixel> decoderState, ImagingConfig config, out Image<TPixel> image, 
            DecodeProgressCallback<TPixel> onProgress = null) 
            where TPixel : unmanaged, IPixel
        {
            if (decoderState.ImageIndex < 0)
                throw new InvalidOperationException(
                    $"The decoder state has an invalid {nameof(decoderState.ImageIndex)}.");

            var readState = CreateReadState(decoderState, null, onProgress);
            if (ReadNext(decoderState.Stream.Context, config, out void* result, ref readState))
            {
                var parsedBuffer = ParseStbResult<TPixel>(config, result, readState);
                image = new Image<TPixel>(parsedBuffer, readState.Width, readState.Height);

                decoderState.CurrentImage = image;
                decoderState.ImageIndex++;
                return true;
            }
            else
                throw GetFailureException(decoderState.Stream.Context);
        }

        public virtual void FinishState<TPixel>(ImageDecoderState<TPixel> decoderState)
            where TPixel : unmanaged, IPixel
        {
        }

        #endregion
    }
}
