using System;
using System.Runtime.InteropServices;
using System.Threading;
using MonoGame.Framework;
using MonoGame.Framework.Memory;
using MonoGame.Framework.PackedVector;
using StbSharp;
using static StbSharp.StbImage;

namespace MonoGame.Imaging.Coding.Decoding
{
    public abstract class StbDecoderBase : IImageDecoder, IImageInfoDetector
    {
        public abstract ImageFormat Format { get; }

        #region DetectFormat Abstraction

        protected abstract bool TestFormat(ReadContext context, ImagingConfig config);

        private bool TryDetectFormat(
            ReadContext context, ImagingConfig config, out ImageFormat format)
        {
            if (TestFormat(context, config))
            {
                format = Format;
                return true;
            }
            format = default;
            return false;
        }

        public bool TryDetectFormat(
            ImageReadStream stream, ImagingConfig config, out ImageFormat format)
        {
            return TryDetectFormat(stream.Context, config, out format);
        }

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

        #endregion

        #region Identify Abstraction

        protected abstract bool GetInfo(
            ReadContext context, ImagingConfig config, out int w, out int h, out int n);

        private bool Identify(
            ReadContext context, ImagingConfig config, out ImageInfo info)
        {
            if (GetInfo(context, config, out int w, out int h, out int n))
            {
                info = new ImageInfo(w, h, n * 8, Format);
                return true;
            }
            info = default;
            return false;
        }

        public bool TryIdentify(
            ImageReadStream stream, ImagingConfig config, out ImageInfo info)
        {
            return Identify(stream.Context, config, out info);
        }

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

        private static ReadState CreateReadState(
            Type pixelType, ReadProgressCallback onProgress)
        {
            return new ReadState(onProgress)
            {
                BitsPerChannel = CanUtilize16BitData(pixelType) ? 16 : 8
            };
        }

        // TODO: FIXME: properly handle ImageCollection type

        private unsafe ImageCollection<TPixel, ImageFrame<TPixel>> Decode<TPixel>(
            ReadContext context, ImagingConfig config, int? frameLimit,
            DecodeProgressCallback<TPixel> onProgress = null)
            where TPixel : unmanaged, IPixel
        {
            var frames = GetSmallInitialCollection<TPixel>();
            int actualFrameLimit = frameLimit ?? int.MaxValue;
            int frameIndex = 0;

            while (frameIndex < actualFrameLimit)
            {
                var progressCallback = onProgress == null ? (ReadProgressCallback)null : (progress, r) =>
                {
                    Rectangle? rectangle = null;
                    if (r.HasValue)
                    {
                        var rect = r.Value;
                        rectangle = new Rectangle(rect.X, rect.Y, rect.W, rect.H);
                    }
                    onProgress.Invoke(frameIndex, frames, progress, rectangle);
                };
                ReadState state = CreateReadState(typeof(TPixel), progressCallback);

                void* result;
                if (frameIndex == 0)
                {
                    if (!ReadFirst(context, config, out result, ref state))
                        throw GetFailureException(context);
                }
                else if (!ReadNext(context, config, out result, ref state))
                {
                    break;
                }

                ParseResult(frames, config, result, state);
                frameIndex++;
            }
            return frames;
        }

        protected unsafe void ParseResult<TPixel>(
            FrameCollection<TPixel> frames, ImagingConfig config,
            void* result, ReadState state)
            where TPixel : unmanaged, IPixel
        {
            // TODO: use Image.Load functions instead

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
                var buffer = new Image<TPixel>.Buffer(dst, state.Width, false);
                var image = new Image<TPixel>(buffer, state.Width, state.Height);
                frames.Add(image, state.AnimationDelay);
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
        private FrameCollection<TPixel> GetSmallInitialCollection<TPixel>()
            where TPixel : unmanaged, IPixel
        {
            return new FrameCollection<TPixel>(ImplementsAnimation ? 0 : 1);
        }

        protected Exception GetFailureException(ReadContext context)
        {
            // TODO: get some error message from the context
            return new Exception();
        }

        #endregion

        #region Public Methods

        public virtual unsafe ImageCollection<TPixel, ImageFrame<TPixel>> Decode<TPixel>(
            ImageReadStream stream, ImagingConfig config, int? frameLimit,
            DecodeProgressCallback<TPixel> onProgress = null)
            where TPixel : unmanaged, IPixel
        {
            ImagingArgumentGuard.AssertValidFrameLimit(frameLimit, nameof(frameLimit));
            return Decode(stream.Context, config, frameLimit, onProgress);
        }

        public virtual unsafe ImageCollection<TPixel, ImageFrame<TPixel>> Decode<TPixel>(
            ReadOnlySpan<byte> data, ImagingConfig config, int? frameLimit,
            CancellationToken cancellation, DecodeProgressCallback<TPixel> onProgress = null)
            where TPixel : unmanaged, IPixel
        {
            ImagingArgumentGuard.AssertValidFrameLimit(frameLimit, nameof(frameLimit));
            fixed (byte* ptr = &MemoryMarshal.GetReference(data))
            {
                var context = new ReadContext(ptr, data.Length, cancellation);
                return Decode(context, config, frameLimit, onProgress);
            }
        }

        public ImageDecoderState<TPixel> DecodeFirst<TPixel>(
            ImageReadStream stream, ImagingConfig config, 
            DecodeProgressCallback<TPixel> onProgress = null)
            where TPixel : unmanaged, IPixel
        {

        }

        public Image<TPixel> DecodeNext<TPixel>(
            ImageDecoderState<TPixel> decoderState, ImagingConfig config,
            DecodeProgressCallback<TPixel> onProgress = null) 
            where TPixel : unmanaged, IPixel
        {

        }

        #endregion
    }
}
