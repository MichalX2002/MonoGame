using System;
using System.IO;

namespace MonoGame.Framework.Memory
{
    public sealed class RecyclableBuffer : IDisposable
    {
        public RecyclableMemoryManager Manager { get; }
        public int BaseLength { get; }
        public byte[] Buffer { get; private set; }
        public string? Tag { get; }

        public int BufferLength => Buffer.Length;

        public RecyclableBuffer(RecyclableMemoryManager manager, byte[] buffer, int baseLength, string? tag)
        {
            Manager = manager ?? throw new ArgumentNullException(nameof(manager));
            Buffer = buffer ?? throw new ArgumentNullException(nameof(buffer));
            BaseLength = baseLength;
            Tag = tag;
        }

        public static RecyclableBuffer ReadBytes(Stream stream, int bytes, string? tag)
        {
            if (stream == null)
                throw new ArgumentNullException(nameof(stream));

            var result = RecyclableMemoryManager.Default.GetBuffer(bytes, tag);
            try
            {
                var resultM = result.AsMemory(0, result.BaseLength);
                while (!resultM.IsEmpty)
                {
                    int read = stream.Read(resultM.Span);
                    if (read == 0)
                        break;
                    resultM = resultM[read..];
                }

                if (!resultM.IsEmpty)
                    throw new EndOfStreamException();

                return result;
            }
            catch
            {
                result.Dispose();
                throw;
            }
        }

        public Memory<byte> AsMemory(int start, int count) => Buffer.AsMemory(start, count);

        public Memory<byte> AsMemory(int start) => Buffer.AsMemory(start);

        public Memory<byte> AsMemory() => Buffer.AsMemory();

        public Span<byte> AsSpan(int start, int count) => Buffer.AsSpan(start, count);

        public Span<byte> AsSpan(int start) => Buffer.AsSpan(start);

        public Span<byte> AsSpan() => Buffer.AsSpan();

        public void Dispose()
        {
            if (Buffer != null)
            {
                Manager.ReturnBuffer(Buffer, Tag);
                Buffer = null!;
            }
        }
    }
}
