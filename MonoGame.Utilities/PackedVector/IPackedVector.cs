// MIT License - Copyright (C) The Mono.Xna Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

namespace MonoGame.Framework.PackedVector
{
    // http://msdn.microsoft.com/en-us/library/microsoft.xna.framework.graphics.packedvector.ipackedvector.aspx
    /// <summary>
    /// Allows vectors to be converted to and from a <see cref="Vector4"/>
    /// representation with values scaled between 0 and 1.
    /// </summary>
    public interface IPackedVector
    {
        VectorChannelInfo ChannelInfo { get; }

        /// <summary>
        /// Sets the vector from a scaled <see cref="Vector4"/>.
        /// The XYZW vector components correspond to RGBA. 
        /// </summary>
        void FromScaledVector4(Vector4 vector);

        /// <summary>
        /// Gets the vector as a scaled <see cref="Vector4"/>.
        /// The XYZW vector components correspond to RGBA. 
        /// </summary>
        Vector4 ToScaledVector4();

        /// <summary>
        /// Sets the vector from a <see cref="Vector4"/>.
        /// The XYZW vector components correspond to RGBA. 
        /// </summary>
        void FromVector4(Vector4 vector);

        /// <summary>
        /// Gets the vector as a <see cref="Vector4"/>.
        /// The XYZW vector components correspond to RGBA. 
        /// </summary>
        Vector4 ToVector4();
    }

    // http://msdn.microsoft.com/en-us/library/bb197661.aspx
    public interface IPackedVector<TPacked> : IPackedVector
    {
        /// <summary>
        /// Gets or sets the packed representation of this vector.
        /// </summary>
        TPacked PackedValue { get; set; }
    }
}
