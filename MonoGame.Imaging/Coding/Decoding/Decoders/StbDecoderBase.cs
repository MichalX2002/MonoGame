using System;
using MonoGame.Framework;
using MonoGame.Framework.Memory;
using MonoGame.Framework.PackedVector;
using MonoGame.Imaging.Pixels;
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

        #endregion

        #region Decode Abstraction

        protected abstract unsafe bool ReadFirst(
            ImagingConfig config, ReadContext context, out void* data, ref ReadState state);

        protected virtual unsafe bool ReadNext(
            ImagingConfig config, ReadContext context, out void* data, ref ReadState state)
        {
            ImagingArgumentGuard.AssertAnimationSupport(this, config);
            data = null;
            return false;
        }

        private static ReadState CreateReadState(
            ImageDecoderState decoderState,
            BufferReadyCallback onBufferReady,
            DecodeProgressCallback onProgress)
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

            return new ReadState(onBufferReady, progressCallback);
        }

        protected virtual unsafe Image<TPixel>.PixelBuffer ParseStbResult<TPixel>(
            ImagingConfig config, void* result, ReadState state)
            where TPixel : unmanaged, IPixel
        {
            // TODO: use some Image.LoadPixels function instead

            int pixelCount = state.Width * state.Height;
            var dstMemory = new UnmanagedMemory<TPixel>(pixelCount);
            try
            {
                var dstPtr = (TPixel*)dstMemory.Pointer;
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

                return new Image<TPixel>.PixelBuffer(dstMemory, state.Width, leaveOpen: false);
            }
            catch
            {
                dstMemory?.Dispose();
                throw;
            }
            finally
            {
                CRuntime.Free(result);
            }
        }

        protected Exception GetFailureException(ReadContext context)
        {
            // TODO: get some error message from the context
            return new Exception();
        }

        #endregion

        #region IImageDecoder

        public unsafe ImageDecoderState DecodeFirst(
            ImagingConfig imagingConfig,
            ImageReadStream stream,
            out Image image,
            DecodeProgressCallback onProgress = null)
        {
            var decoderState = new ImageDecoderState(this, imagingConfig, stream, true);
            var readState = CreateReadState(decoderState, null, onProgress);
            if (ReadFirst(imagingConfig, stream.Context, out void* result, ref readState))
            {
                var parsedBuffer = ParseStbResult<TPixel>(imagingConfig, result, readState);
                image = new Image<TPixel>(parsedBuffer, readState.Width, readState.Height);

                decoderState.CurrentImage = image;
                decoderState.ImageIndex = 0;
                return decoderState;
            }
            else
            {
                throw GetFailureException(stream.Context);
            }
        }

        public unsafe bool DecodeNext(
            ImagingConfig config,
            ImageDecoderState decoderState,
            out Image image,
            DecodeProgressCallback onProgress = null)
        {
            if (decoderState.ImageIndex < 0)
                throw new InvalidOperationException(
                    $"The decoder state has an invalid {nameof(decoderState.ImageIndex)}.");

            var readState = CreateReadState(decoderState, null, onProgress);
            if (ReadNext(config, decoderState.Stream.Context, out void* result, ref readState))
            {
                var parsedBuffer = ParseStbResult<TPixel>(config, result, readState);
                image = new Image<TPixel>(parsedBuffer, readState.Width, readState.Height);

                decoderState.CurrentImage = image;
                decoderState.ImageIndex++;
                return true;
            }
            else
            {
                throw GetFailureException(decoderState.Stream.Context);
            }
        }

        public virtual void FinishState(ImageDecoderState decoderState)
        {
        }

        #endregion
    }
}
