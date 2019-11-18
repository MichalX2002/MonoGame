using System;
using System.IO;
using System.Reflection;
using MonoGame.Utilities.Memory;

namespace MonoGame.Utilities
{
    public static class BinaryReaderExtensions
    {
        private delegate int Read7BitEncodedIntDelegate(BinaryReader reader);

        private static Read7BitEncodedIntDelegate _read7BitEncodedInt;

        static BinaryReaderExtensions()
        {
            var bindingFlags = BindingFlags.NonPublic | BindingFlags.Instance;
            var readMethod = typeof(BinaryReader).GetMethod("Read7BitEncodedInt", bindingFlags);
            var readDelegate = readMethod.CreateDelegate(typeof(Read7BitEncodedIntDelegate));
            _read7BitEncodedInt = (Read7BitEncodedIntDelegate)readDelegate;
        }

        public static int SkipString(this BinaryReader reader)
        {
            int length = reader.Read7BitEncodedInt();
            return Skip(reader, length);
        }

        public static int Skip(this BinaryReader reader, int count)
        {
            var stream = reader.BaseStream;
            int skipped = 0;

            if (stream.CanSeek)
            {
                long originalPosition = stream.Position;
                long diff = stream.Seek(count, SeekOrigin.Current) - originalPosition;
                skipped = (int)diff;
            }
            else
            {
                byte[] block = RecyclableMemoryManager.Default.GetBlock();
                try
                {
                    int read;
                    while (count > 0 && (read = stream.Read(block, 0, Math.Min(block.Length, count))) > 0)
                    {
                        count -= read;
                        skipped += read;
                    }
                }
                finally
                {
                    RecyclableMemoryManager.Default.ReturnBlock(block);
                }
            }
            return skipped;
        }

        public static int Read7BitEncodedInt(this BinaryReader reader)
        {
            return _read7BitEncodedInt.Invoke(reader);
        }
    }
}