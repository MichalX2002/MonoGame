using System;
using System.IO;
using System.IO.Compression;
using System.Runtime.InteropServices;
using MonoGame.Utilities;
using MonoGame.Utilities.Memory;
using StbSharp;

namespace MonoGame.Imaging.Encoding
{
    public abstract partial class StbEncoderBase : IImageEncoder
    {
        private static unsafe IMemoryResult CustomDeflateCompress(
            ReadOnlySpan<byte> data, CompressionLevel level, Action<double> onProgress = null)
        {
            var output = RecyclableMemoryManager.Default.GetMemoryStream();

            output.WriteByte(StbImageWrite.zlib_dotnet_deflate_CMF);
            output.WriteByte(StbImageWrite.zlib_dotnet_deflate_FLG);

            fixed (byte* dataPtr = &MemoryMarshal.GetReference(data))
            {
                using (var deflate = new DeflateStream(output, level, leaveOpen: true))
                using (var source = new UnmanagedMemoryStream(dataPtr, data.Length))
                {
                    double dlen = data.Length;
                    source.PooledCopyTo(deflate, (r, copied) => onProgress?.Invoke(copied / dlen));
                }
            }

            StbImageWrite.calc_adler32_checksum(data, out uint adlerA, out uint adlerB);
            uint adlerSum = (adlerB << 16) | adlerA;
            var adlerChecksum = new Span<byte>(&adlerSum, sizeof(uint));
            adlerChecksum.Reverse();
            output.Write(adlerChecksum);

            return new RecyclableDeflateResult(output);
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
