// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;

namespace Microsoft.Xna.Framework.Graphics.PackedVector
{
    /// <summary>
    /// Packed vector type containing two 16-bit unsigned normalized values ranging from 0 to 1.
    /// </summary>
    public struct Rg32 : IPackedVector<uint>, IEquatable<Rg32>, IPackedVector
    {
        /// <summary>
        /// Gets and sets the packed value.
        /// </summary>
        [CLSCompliant(false)]
        public uint PackedValue { get; set; }

        /// <summary>
        /// Creates a new instance of Rg32.
        /// </summary>
        /// <param name="x">The x component</param>
        /// <param name="y">The y component</param>
        public Rg32(float x, float y)
        {
            PackedValue = Pack(x, y);
        }

        /// <summary>
        /// Creates a new instance of Rg32.
        /// </summary>
        /// <param name="vector">
        /// Vector containing the components for the packed vector.
        /// </param>
        public Rg32(Vector2 vector)
        {
            PackedValue = Pack(vector.X, vector.Y);
        }

        /// <summary>
        /// Gets the packed vector in Vector2 format.
        /// </summary>
        /// <returns>The packed vector in Vector2 format</returns>
        public Vector2 ToVector2()
        {
            return new Vector2(
                 (PackedValue & 0xFFFF) / 65535.0f,
                ((PackedValue >> 16) & 0xFFFF) / 65535.0f
            );
        }

        /// <summary>
        /// Sets the packed vector from a Vector4.
        /// </summary>
        /// <param name="vector">Vector containing the components.</param>
        void IPackedVector.PackFromVector4(Vector4 vector)
        {
            PackedValue = Pack(vector.X, vector.Y);
        }

        /// <summary>
        /// Gets the packed vector in Vector4 format.
        /// </summary>
        /// <returns>The packed vector in Vector4 format</returns>
        Vector4 IPackedVector.ToVector4()
        {
            return new Vector4(ToVector2(), 0.0f, 1.0f);
        }

        /// <summary>
        /// Compares an object with the packed vector.
        /// </summary>
        /// <param name="obj">The object to compare.</param>
        /// <returns>True if the object is equal to the packed vector.</returns>
        public override bool Equals(object obj)
        {
            return (obj is Rg32) && Equals((Rg32) obj);
        }

        /// <summary>
        /// Compares another Rg32 packed vector with the packed vector.
        /// </summary>
        /// <param name="other">The Rg32 packed vector to compare.</param>
        /// <returns>True if the packed vectors are equal.</returns>
        public bool Equals(Rg32 other)
        {
            return PackedValue == other.PackedValue;
        }

        /// <summary>
        /// Gets a string representation of the packed vector.
        /// </summary>
        /// <returns>A string representation of the packed vector.</returns>
        public override string ToString()
        {
            return ToVector2().ToString();
        }

        /// <summary>
        /// Gets a hash code of the packed vector.
        /// </summary>
        /// <returns>The hash code for the packed vector.</returns>
        public override int GetHashCode()
        {
            return PackedValue.GetHashCode();
        }

        public static bool operator ==(Rg32 lhs, Rg32 rhs)
        {
            return lhs.PackedValue == rhs.PackedValue;
        }

        public static bool operator !=(Rg32 lhs, Rg32 rhs)
        {
            return lhs.PackedValue != rhs.PackedValue;
        }

        private static uint Pack(float x, float y)
        {
            return (uint) (
                (((int) Math.Round(MathHelper.Clamp(x, 0, 1) * 65535.0f) & 0xFFFF) ) |
                (((int) Math.Round(MathHelper.Clamp(y, 0, 1) * 65535.0f) & 0xFFFF) << 16)
            );
        }
    }
}
