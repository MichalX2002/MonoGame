using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace MonoGame.Imaging
{
    public class ImagingConfig
    {
        public static ImagingConfig Default { get; private set; }

        private HashSet<Type> _throwingExceptions;

        /// <summary>
        /// Gets whether operations should throw exceptions or return a default value.
        /// This is set to <see langword="true"/> by default.
        /// <para>
        /// IO exceptions caused by reading/writing to streams are thrown regardless of this.
        /// </para>
        /// </summary>
        public bool UseExceptions { get; set; } = true;

        public bool UseBufferedIO { get; set; } = true;

        static ImagingConfig()
        {
            Default = new ImagingConfig();
        }

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
