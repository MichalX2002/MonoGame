using System;

namespace MonoGame.Framework.Vectors
{
    // http://msdn.microsoft.com/en-us/library/bb197661.aspx
    public interface IPackedVector<TPacked> : IVector, IEquatable<TPacked>
    {
        /// <summary>
        /// Gets or sets the packed representation of this vector.
        /// </summary>
        TPacked PackedValue { get; set; }
    }
}
