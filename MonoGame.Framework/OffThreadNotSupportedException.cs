using System;
using System.Runtime.Serialization;

namespace MonoGame.Framework
{
    [Serializable]
    public class OffThreadNotSupportedException : NotSupportedException
    {
        public OffThreadNotSupportedException() : base(FrameworkResources.OffThreadNotSupported)
        {
        }

        public OffThreadNotSupportedException(string message) : 
            base(message ?? FrameworkResources.OffThreadNotSupported)
        {
        }

        public OffThreadNotSupportedException(string message, Exception inner) :
            base(message ?? FrameworkResources.OffThreadNotSupported, inner)
        {
        }

        protected OffThreadNotSupportedException(
            SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}