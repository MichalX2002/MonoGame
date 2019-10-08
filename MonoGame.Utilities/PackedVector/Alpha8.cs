// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using MonoGame.Framework;
 
namespace MonoGame.Utilities.PackedVector
{
    /// <summary>
    /// Packed vector type containing an unsigned 8-bit W component.
    /// <para>Ranges from [1, 1, 1, 0] to [1, 1, 1, 1] in vector form.</para>
    /// </summary>
    public struct Alpha8 : IPackedVector<byte>, IEquatable<Alpha8>, IPixel
    {
        #region Constructors

        /// <summary>
        /// Constructs the packed vector with a raw value.
        /// </summary>
        public Alpha8(byte value) => PackedValue = value;

        /// <summary>
        /// Constructs the packed vector with a vector form value.
        /// </summary>
        /// <param name="alpha">The W component.</param>
        public Alpha8(float alpha) => PackedValue = Pack(alpha);

        #endregion

        /// <summary>
        /// Gets the packed vector as a <see cref="float"/>.
        /// </summary>
        public float ToAlpha() => PackedValue / 255f;

        private static byte Pack(float alpha)
        {
            return (byte)Math.Round(MathHelper.Clamp(alpha, 0, 1) * 255f);
        }

        #region IPackedVector

        /// <inheritdoc />
        public byte PackedValue { get; set; }

        /// <inheritdoc />
        public void FromVector4(Vector4 vector) => PackedValue = Pack(vector.W);

        /// <inheritdoc />
        public Vector4 ToVector4() => new Vector4(0, 0, 0, PackedValue / 255f);

        #endregion

        #region IPixel

        /// <inheritdoc />
        public void FromScaledVector4(Vector4 vector) => FromVector4(vector);

        /// <inheritdoc />
        public Vector4 ToScaledVector4() => ToVector4();

        #endregion

        #region Equals

        public static bool operator ==(Alpha8 a, Alpha8 b) => a.PackedValue == b.PackedValue;
        public static bool operator !=(Alpha8 a, Alpha8 b) => a.PackedValue != b.PackedValue;

        public bool Equals(Alpha8 other) => this == other;
        public override bool Equals(object obj) => obj is Alpha8 other && Equals(other);

        #endregion

        #region Object Overrides

        /// <summary>
        /// Gets a string representation of the packed vector.
        /// </summary>
        public override string ToString() => (PackedValue / 255f).ToString();

        /// <summary>
        /// Gets a hash code of the packed vector.
        /// </summary>
        public override int GetHashCode() => PackedValue.GetHashCode();

        #endregion
    }
}
