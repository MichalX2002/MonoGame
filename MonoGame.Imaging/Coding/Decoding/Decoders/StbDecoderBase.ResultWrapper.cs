using System;
using MonoGame.Framework.Memory;
using StbSharp;

namespace MonoGame.Imaging.Coding.Decoding
{
    public abstract partial class StbDecoderBase
    {
        private class ResultWrapper : IMemory
        {
            private IMemoryResult _memoryResult;

            public unsafe Span<byte> Span => new Span<byte>(
                (void*)_memoryResult.Pointer, _memoryResult.Length);
            
            ReadOnlySpan<byte> IReadOnlyMemory.Span => Span;

            public int Count => _memoryResult.Length;
            public int ElementSize => sizeof(byte);

            public ResultWrapper(IMemoryResult memoryResult)
            {
                _memoryResult = memoryResult ?? throw new ArgumentNullException(nameof(memoryResult));
            }

            public void Dispose()
            {
                _memoryResult?.Dispose();
                _memoryResult = null;
            }
        }
    }
}
