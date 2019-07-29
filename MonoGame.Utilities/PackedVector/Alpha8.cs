// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using MonoGame.Framework;

namespace MonoGame.Utilities.PackedVector
{
    /// <summary>
    /// Packed vector type containing a single 8 bit normalized W values that is ranging from 0 to 1.
    /// </summary>
    public partial struct Alpha8 : IPixel, IPackedVector<byte>, IEquatable<Alpha8>
    {
        /// <summary>
        /// Gets and sets the packed value.
        /// </summary>
        public byte PackedValue { get; set; }

        /// <summary>
        /// Creates a new instance of Alpha8.
        /// </summary>
        /// <param name="alpha">The alpha component</param>
        public Alpha8(byte alpha)
        {
            PackedValue = alpha;
        }

        /// <summary>
        /// Creates a new instance of Alpha8.
        /// </summary>
        /// <param name="alpha">The alpha component</param>
        public Alpha8(float alpha)
        {
            PackedValue = Pack(alpha);
        }

        #region IPixel Implementation


        /// <inheritdoc/>
        public void FromScaledVector4(Vector4 vector) => FromVector4(vector);

        /// <inheritdoc/>
        public Vector4 ToScaledVector4() => ToVector4();

        /// <inheritdoc />
        public void FromVector4(Vector4 vector) => PackedValue = Pack(vector.W);

        /// <inheritdoc />
        public Vector4 ToVector4() => new Vector4(0, 0, 0, PackedValue / 255F);

        /// <inheritdoc/>
        public void FromArgb32(Argb32 source) => PackedValue = source.A;

        /// <inheritdoc/>
        public void FromBgr24(Bgr24 source) => PackedValue = byte.MaxValue;

        /// <inheritdoc/>
        public void FromBgra32(Bgra32 source) => PackedValue = source.A;

        /// <inheritdoc/>
        public void FromBgra5551(Bgra5551 source) => FromScaledVector4(source.ToScaledVector4());

        /// <inheritdoc/>
        public void FromGray8(Gray8 source) => PackedValue = byte.MaxValue;

        /// <inheritdoc/>
        public void FromGray16(Gray16 source) => PackedValue = byte.MaxValue;

        /// <inheritdoc/>
        public void FromRgb24(Rgb24 source) => PackedValue = byte.MaxValue;

        /// <inheritdoc />
        public void FromColor(Color source) => PackedValue = source.A;

        /// <inheritdoc/>
        public void FromRgb48(Rgb48 source) => PackedValue = byte.MaxValue;

        /// <inheritdoc/>
        public void FromRgba64(Rgba64 source) => FromScaledVector4(source.ToScaledVector4());

        /// <inheritdoc />
        public void ToColor(ref Color dest)
        {
            dest = default;
            dest.A = PackedValue;
        }
		
        #endregion

        /// <summary>
        /// Gets the packed vector in float format.
        /// </summary>
        /// <returns>The packed vector in Vector3 format</returns>
        public float ToAlpha() => PackedValue / 255.0f;

        /// <summary>
        /// Compares an object with the packed vector.
        /// </summary>
        /// <param name="obj">The object to compare.</param>
        /// <returns>True if the object is equal to the packed vector.</returns>
        public override bool Equals(object obj) => obj is Alpha8 other && Equals(other);

        /// <summary>
        /// Compares another Alpha8 packed vector with the packed vector.
        /// </summary>
        /// <param name="other">The Alpha8 packed vector to compare.</param>
        /// <returns>True if the packed vectors are equal.</returns>
        public bool Equals(Alpha8 other) => PackedValue == other.PackedValue;

        /// <summary>
        /// Gets a string representation of the packed vector.
        /// </summary>
        /// <returns>A string representation of the packed vector.</returns>
        public override string ToString() => (PackedValue / 255f).ToString();

        /// <summary>
        /// Gets a hash code of the packed vector.
        /// </summary>
        /// <returns>The hash code for the packed vector.</returns>
        public override int GetHashCode() => PackedValue.GetHashCode();

        public static bool operator ==(Alpha8 lhs, Alpha8 rhs) => lhs.PackedValue == rhs.PackedValue;

        public static bool operator !=(Alpha8 lhs, Alpha8 rhs) => lhs.PackedValue != rhs.PackedValue;

        private static byte Pack(float alpha)
        {
            return (byte)Math.Round(MathHelper.Clamp(alpha, 0, 1) * 255f);
        }
    }
}
