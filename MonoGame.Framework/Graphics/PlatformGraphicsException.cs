using System;

namespace MonoGame.Framework.Graphics
{
    [Serializable]
    public class PlatformGraphicsException : Exception
    {
        public PlatformGraphicsException() 
        {
        }

        public PlatformGraphicsException(string? message) : base(message) 
        { 
        }

        public PlatformGraphicsException(string? message, Exception? innerException) : base(message, innerException) 
        { 
        }

        protected PlatformGraphicsException(
            System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context) : base(info, context) 
        { 
        }
    }
}
