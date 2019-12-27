// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.Runtime.Serialization;

namespace MonoGame.Framework.Audio
{
    /// <summary>
    /// The exception thrown when no audio hardware is present, or driver issues are detected.
    /// </summary>
    [DataContract]
    public sealed class NoMicrophoneException : Exception
    {
        /// <param name="message">A message describing the error.</param>
        public NoMicrophoneException(string message) : base(message)
        {
        }

        /// <param name="message">A message describing the error.</param>
        /// <param name="inner">The exception that is the underlying cause of the current exception. If not null, the current exception is raised in a try/catch block that handled the innerException.</param>
        public NoMicrophoneException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}

