using System;
using System.Runtime.Serialization;

namespace MonoGame.Framework
{
    [Serializable]
    public class AsyncResourceNotSupportedException : NotSupportedException
    {
        public AsyncResourceNotSupportedException()
        {
        }

        public AsyncResourceNotSupportedException(string message) : base(message)
        {
        }

        public AsyncResourceNotSupportedException(string message, Exception inner) : base(message, inner)
        {
        }

        protected AsyncResourceNotSupportedException(
            SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}