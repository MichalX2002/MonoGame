// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.Runtime.CompilerServices;
using MonoGame.Framework;

namespace MonoGame.Utilities.PackedVector
{
    /// <summary>
    /// Packed pixel type containing signed 16-bit XY components.
    /// <para>
    /// Ranges from [0, 0, 0, 1] to [1, 1, 0, 1] in vector form.
    /// </para>
    /// </summary>
    public struct Rg32 : IPackedVector<uint>, IEquatable<Rg32>, IPackedVector
    {
        [CLSCompliant(false)]
        public ushort X;

        [CLSCompliant(false)]
        public ushort Y;

        #region Constructors

        [CLSCompliant(false)]
        public Rg32(ushort x, ushort y)
        {
            X = x;
            Y = y;
        }

        [CLSCompliant(false)]
        public Rg32(uint packed) : this() => PackedValue = packed;

        public Rg32(Vector2 vector) => this = Pack(vector);

        public Rg32(float x, float y) : this(new Vector2(x, y))
        {
        }

        #endregion

        private static Rg32 Pack(Vector2 vector)
        {
            vector = Vector2.Clamp(vector, Vector2.Zero, Vector2.One);
            vector *= ushort.MaxValue;
            vector.Round();

            return new Rg32((ushort)vector.X, (ushort)vector.Y);
        }

        /// <summary>
        /// Gets the packed vector in <see cref="Vector2"/> format.
        /// </summary>
        public readonly Vector2 ToVector2() => new Vector2(X, Y) / ushort.MaxValue;

        #region IPackedVector

        /// <inheritdoc/>
        [CLSCompliant(false)]
        public uint PackedValue
        {
            get => Unsafe.As<Rg32, uint>(ref this);
            set => Unsafe.As<Rg32, uint>(ref this) = value;
        }

        /// <inheritdoc/>
        public void FromVector4(Vector4 vector) => this = Pack(new Vector2(vector.X, vector.Y));

        /// <inheritdoc/>
        public readonly Vector4 ToVector4() => new Vector4(ToVector2(), 0, 1f);

        #endregion

        #region IPixel

        /// <inheritdoc/>
        public void FromScaledVector4(Vector4 vector) => FromVector4(vector);

        /// <inheritdoc/>
        public readonly Vector4 ToScaledVector4() => ToVector4();

        #endregion

        #region Equals

        public static bool operator ==(in Rg32 a, in Rg32 b) => a.X == b.X && a.Y == b.Y;
        public static bool operator !=(in Rg32 a, in Rg32 b) => !(a == b);

        /// <summary>
        /// Compares another Rg32 packed vector with the packed vector.
        /// </summary>
        /// <param name="other">The Rg32 packed vector to compare.</param>
        /// <returns>True if the packed vectors are equal.</returns>
        public bool Equals(Rg32 other) => this == other;


        /// <summary>
        /// Compares an object with the packed vector.
        /// </summary>
        /// <param name="obj">The object to compare.</param>
        /// <returns>True if the object is equal to the packed vector.</returns>
        public override bool Equals(object obj) => obj is Rg32 other && Equals(other);

        #endregion

        #region Object Overrides

        /// <summary>
        /// Gets a <see cref="string"/> representation of the packed vector.
        /// </summary>
        public override string ToString() => ToVector2().ToString();

        /// <summary>
        /// Gets a hash code of the packed vector.
        /// </summary>
        public override int GetHashCode() => PackedValue.GetHashCode();

        #endregion
    }
}
