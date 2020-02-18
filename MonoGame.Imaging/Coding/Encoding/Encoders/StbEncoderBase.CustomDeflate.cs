using System;
using System.Buffers.Binary;
using System.IO.Compression;
using System.Threading;
using MonoGame.Framework.Memory;
using StbSharp;

namespace MonoGame.Imaging.Coding.Encoding
{
    public abstract partial class StbEncoderBase : IImageEncoder
    {
        private static IMemoryHolder CustomDeflateCompress(
            ReadOnlySpan<byte> data,
            CompressionLevel level,
            CancellationToken cancellation,
            ImageWrite.WriteProgressCallback onProgress = null)
        {
            cancellation.ThrowIfCancellationRequested();

            var output = RecyclableMemoryManager.Default.GetMemoryStream();
            var copyBuffer = RecyclableMemoryManager.Default.GetBlock();
            try
            {
                var header = ZlibHeader.CreateForDeflateStream(level);
                output.WriteByte(header.GetCMF());
                output.WriteByte(header.GetFLG());

                cancellation.ThrowIfCancellationRequested();

                using (var deflate = new DeflateStream(output, level, leaveOpen: true))
                {
                    int totalRead = 0;
                    while (totalRead < data.Length)
                    {
                        cancellation.ThrowIfCancellationRequested();

                        int count = Math.Min(data.Length - totalRead, copyBuffer.Length);
                        data.Slice(totalRead, count).CopyTo(copyBuffer);
                        deflate.Write(copyBuffer, 0, count);

                        totalRead += count;
                        onProgress?.Invoke(totalRead / (double)data.Length);
                    }
                }

                uint adlerSum = ImageWrite.Adler32.Calculate(data);
                Span<byte> adlerSumBytes = stackalloc byte[sizeof(uint)];
                BinaryPrimitives.WriteUInt32BigEndian(adlerSumBytes, adlerSum);
                output.Write(adlerSumBytes);

                return new RecyclableMemoryHolder(output);
            }
            catch
            {
                RecyclableMemoryManager.Default.ReturnBlock(copyBuffer);
                output.Dispose();
                throw;
            }
        }

        private class RecyclableMemoryHolder : ByteMemoryHolder
        {
            private RecyclableMemoryStream _stream;

            public RecyclableMemoryHolder(RecyclableMemoryStream stream) :
                base(stream.GetBuffer().AsMemory(0, (int)stream.Length))
            {
                _stream = stream;
            }

            protected override void Dispose(bool disposing)
            {
                base.Dispose(disposing);

                if (disposing)
                {
                    _stream?.Dispose();
                    _stream = null;
                }
            }
        }
    }
}
