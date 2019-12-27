using System.IO;
using System.Reflection;

namespace MonoGame.Framework
{
    public static class BinaryWriterExtensions
    {
        private delegate int Write7BitEncodedIntDelegate(BinaryWriter writer, int value);

        private static Write7BitEncodedIntDelegate _write7BitEncodedInt;

        static BinaryWriterExtensions()
        {
            var bindingFlags = BindingFlags.NonPublic | BindingFlags.Instance;
            var writeMethod = typeof(BinaryWriter).GetMethod("Write7BitEncodedInt", bindingFlags);
            var writeDelegate = writeMethod.CreateDelegate(typeof(Write7BitEncodedIntDelegate));
            _write7BitEncodedInt = (Write7BitEncodedIntDelegate)writeDelegate;
        }
        
        public static int Write7BitEncodedInt(this BinaryWriter writer, int value)
        {
            return _write7BitEncodedInt.Invoke(writer, value);
        }
    }
}