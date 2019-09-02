using System;
using System.IO;
using System.Runtime.InteropServices;
using MonoGame.Utilities.Memory;
using StbImageSharp;
using static StbImageSharp.StbImage;

namespace MonoGame.Imaging
{
    public static unsafe class ImageReader
    {
        public static void* Read(
            bool bit16, Stream stream, out int x, out int y, out int comp, int req_comp)
        {
            void* result;
            byte[] buffer = RecyclableMemoryManager.Instance.GetBlock();
            try
            {
                var s = new ReadContext(stream, buffer, ReadCallback, SkipCallback);
                result = bit16
                    ? (void*)StbImage.stbi__load_and_postprocess_16bit(s, out x, out y, out comp, req_comp)
                    : (void*)StbImage.stbi__load_and_postprocess_8bit(s, out x, out y, out comp, req_comp);
            }
            finally
            {
                RecyclableMemoryManager.Instance.ReturnBlock(buffer);
            }

            if (result == null)
                throw new InvalidOperationException(StbImage.LastError);
            return result;
        }

        private static int SkipCallback(ReadContext context, int n)
        {
            if (n == 0)
                return 0;

            if (n < 0)
                throw new Exception();

            if (!context.Stream.CanSeek)
            {
                int skipped = 0;
                int left = n;
                while (left > 0)
                {
                    int count = Math.Min(left, context.ReadBuffer.Length);
                    int read = context.Stream.Read(context.ReadBuffer, 0, count);
                    if (read == 0)
                        break;

                    left -= read;
                    skipped += read;
                }
                return skipped;
            }
            else
            {
                long current = context.Stream.Position;
                long seeked = context.Stream.Seek(n, SeekOrigin.Current);
                return (int)(seeked - current);
            }
        }

        private static unsafe int ReadCallback(ReadContext context, Span<byte> data)
        {
            if (data.IsEmpty)
                return 0;

            byte[] buf = context.ReadBuffer;
            int left = data.Length;
            int read;
            while (left > 0 && (read = context.Stream.Read(buf, 0, Math.Min(buf.Length, left))) > 0)
            {
                int totalRead = data.Length - left;
                for (int i = 0; i < read; i++)
                    data[i + totalRead] = buf[i];

                //_infoBuffer?.Write(buffer, 0, read);
                left -= read;
            }

            // "size - left" gives us how much we've already read
            return data.Length - left;
        }

        public static void* Decode(
            bool bit16, ReadOnlySpan<byte> data, out int x, out int y, out int comp, int req_comp)
        {
            void* result;
            fixed (byte* b = &MemoryMarshal.GetReference(data))
            {
                var s = new ReadContext(b, data.Length);
                result = bit16
                    ? (void*)StbImage.stbi__load_and_postprocess_16bit(s, out x, out y, out comp, req_comp)
                    : (void*)StbImage.stbi__load_and_postprocess_8bit(s, out x, out y, out comp, req_comp);
            }

            if (result == null)
                throw new InvalidOperationException(StbImage.LastError);
            return result;
        }

        /*
        public AnimatedFrameResult[] ReadAnimatedGif(Stream stream, ColorEncoding required = ColorComponents.Source)
        {
            var res = new List<AnimatedFrameResult>();

            var context = new StbImage.stbi__context();
            StbImage.stbi__start_callbacks(context, _callbacks, null);

            if (StbImage.stbi__gif_test(context) == 0)
                throw new Exception("Input stream is not GIF file.");

            using (var g = new StbImage.stbi__gif())
            {

                do
                {
                    int comp;
                    var result = StbImage.stbi__gif_load_next(context, g, &comp, (int)required, null);
                    if (result == null)
                        break;

                    var frame = new AnimatedFrameResult
                    {
                        Width = g.w,
                        Height = g.h,
                        SourceComp = (ColorComponents)comp,
                        Comp = required == ColorComponents.Source ? (ColorComponents)comp : required,
                        Delay = g.delay
                    };

                    frame.Data = new byte[g.w * g.h * (int)frame.Comp];
                    Marshal.Copy(new IntPtr(result), frame.Data, 0, frame.Data.Length);

                    CRuntime.free(result);
                    res.Add(frame);
                } while (true);

                CRuntime.free(g._out_);
            }

            return res.ToArray();
        }
        */
    }
}
