using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using MonoGame.Framework.IO;

namespace MonoGame.Imaging
{
    public class ImagingConfig
    {
        /// <summary>
        /// Gets the default <see cref="ImagingConfig"/>,
        /// often used for methods that don't take it as an argument.
        /// </summary>
        public static ImagingConfig Default { get; } = new ImagingConfig();
        
        private HashSet<Type> _throwingExceptions;

        #region Properties

        /// <summary>
        /// Gets whether operations should throw exceptions or return a default value.
        /// This is set to <see langword="true"/> by default.
        /// <para>
        /// IO exceptions caused by reading/writing to streams are thrown regardless of this.
        /// </para>
        /// </summary>
        public bool UseExceptions { get; set; } = true;

        /// <summary>
        /// Set to <see cref="StreamDisposalMethod.CancellableLeaveOpen"/> by default.
        /// </summary>
        public StreamDisposalMethod StreamDisposal { get; set; } = StreamDisposalMethod.CancellableLeaveOpen;

        #endregion

        public ImageReadStream CreateReadStream(Stream stream, CancellationToken cancellation) => 
            ImageReadStream.Create(stream, cancellation, StreamDisposal);

        #region Exception Management

        public bool AddThrowingException(Type type)
        {
            AssertExceptionType(type);
            return _throwingExceptions.Add(type);
        }

        public bool AddThrowingException<TException>()
            where TException : Exception => AddThrowingException(typeof(TException));

        public bool RemoveThrowingException(Type type)
        {
            AssertExceptionType(type);
            return _throwingExceptions.Remove(type);
        }

        public bool RemoveThrowingException<TException>()
            where TException : Exception => RemoveThrowingException(typeof(TException));

        public bool ShouldThrowOnException(Type type)
        {
            AssertExceptionType(type);

            return true; // TODO: FIXME

            if (UseExceptions)
                return _throwingExceptions.Contains(type);
            return false;
        }

        public bool ShouldThrowOnException<TException>()
            where TException : Exception => ShouldThrowOnException(typeof(TException));

        [DebuggerHidden]
        private static void AssertExceptionType(Type type)
        {
            if (!typeof(Exception).IsAssignableFrom(type))
                throw new ArgumentException("The type does not derive from Exception.", nameof(type));
        }

        #endregion
    }
}
