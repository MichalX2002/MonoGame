using System;
using System.IO.Compression;
using System.Runtime.InteropServices;
using System.Threading;
using MonoGame.Utilities.Memory;
using StbSharp;

namespace MonoGame.Imaging.Encoding
{
    public abstract partial class StbEncoderBase : IImageEncoder
    {
        private static IMemoryResult CustomDeflateCompress(
            ReadOnlySpan<byte> data,
            CompressionLevel level,
            CancellationToken cancellation,
            StbImageWrite.WriteProgressCallback onProgress = null)
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

                uint adlerSum = StbImageWrite.Adler32.Calculate(data);
                byte[] adlerBytes = BitConverter.GetBytes(adlerSum);
                adlerBytes.AsSpan().Reverse();
                output.Write(adlerBytes, 0, adlerBytes.Length);

                return new RecyclableDeflateResult(output);
            }
            catch
            {
                RecyclableMemoryManager.Default.ReturnBlock(copyBuffer);
                output.Dispose();
                throw;
            }
        }

        private class RecyclableDeflateResult : IMemoryResult
        {
            private RecyclableMemoryStream _stream;
            private GCHandle _handle;

            public bool IsAllocated => _handle.IsAllocated;
            public int Length => (int)_stream.Length;
            public IntPtr Pointer => _handle.AddrOfPinnedObject();

            public RecyclableDeflateResult(RecyclableMemoryStream stream)
            {
                _stream = stream;

                byte[] buffer = _stream.GetBuffer();
                _handle = GCHandle.Alloc(buffer, GCHandleType.Pinned);
            }

            public void Dispose()
            {
                if (_handle.IsAllocated)
                    _handle.Free();

                _stream?.Dispose();
                _stream = null;
            }
        }
    }
}
