using System;
using MonoGame.Framework.Memory;
using StbSharp;

namespace MonoGame.Imaging.Coding.Decoding
{
    public abstract partial class StbDecoderBase
    {
        private class ResultWrapper : IMemory
        {
            private IMemoryHolder _memory;

            public Span<byte> Span => _memory.Span;
            ReadOnlySpan<byte> IReadOnlyMemory.Span => Span;

            public int Count => _memory.Length;
            public int ElementSize => sizeof(byte);

            public ResultWrapper(IMemoryHolder memory)
            {
                _memory = memory ?? throw new ArgumentNullException(nameof(memory));
            }

            public void Dispose()
            {
                _memory?.Dispose();
                _memory = null;
            }
        }
    }
}
