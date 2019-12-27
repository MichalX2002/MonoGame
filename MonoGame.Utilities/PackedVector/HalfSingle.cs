// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using MonoGame.Framework;

namespace MonoGame.Framework.PackedVector
{
    /// <summary>
    /// Packed vector type containing a 16-bit floating-point X component.
    /// <para>Ranges from [-1, 0, 0, 1] to [1, 0, 0, 1] in vector form.</para>
    /// </summary>
    public struct HalfSingle : IPackedVector<ushort>, IEquatable<HalfSingle>, IPixel
    {
        #region Constructors
        
        /// <summary>
        /// Constructs the packed vector with a packed value.
        /// </summary>
        [CLSCompliant(false)]
        public HalfSingle(ushort value) : this() => PackedValue = value;

        /// <summary>
        /// Constructs the packed vector with a vector form value.
        /// </summary>
        public HalfSingle(float single) => PackedValue = HalfTypeHelper.Pack(single);

        #endregion

        /// <summary>
        /// Gets the packed vector as a <see cref="float"/>.
        /// </summary>
        public readonly float ToSingle() => HalfTypeHelper.Unpack(PackedValue);

        #region IPackedVector

        /// <inheritdoc/>
        [CLSCompliant(false)]
        public ushort PackedValue { get; set; }

        /// <inheritdoc/>
        public void FromVector4(Vector4 vector) => PackedValue = HalfTypeHelper.Pack(vector.X);

        /// <inheritdoc/>
        public readonly Vector4 ToVector4() => new Vector4(ToSingle(), 0, 0, 1);

        #endregion

        #region IPixel

        /// <inheritdoc/>
        public void FromScaledVector4(Vector4 vector)
        {
            float scaled = vector.X;
            scaled *= 2;
            scaled -= 1;
            PackedValue = HalfTypeHelper.Pack(scaled);
        }

        /// <inheritdoc/>
        public readonly Vector4 ToScaledVector4()
        {
            float single = ToSingle() + 1;
            single /= 2;
            return new Vector4(single, 0, 0, 1);
        }

        #endregion

        public static implicit operator float(HalfSingle value) => value.ToSingle();

        #region Equals

        public static bool operator ==(HalfSingle a, HalfSingle b) => a.PackedValue == b.PackedValue;
        public static bool operator !=(HalfSingle a, HalfSingle b) => a.PackedValue != b.PackedValue;

        public bool Equals(HalfSingle other) => this == other;
        public override bool Equals(object obj) => obj is HalfSingle other && Equals(other);

        #endregion

        #region Object Overrides

        /// <summary>
        /// Gets a string representation of the packed vector.
        /// </summary>
        public override string ToString() => ToSingle().ToString();

        /// <summary>
        /// Gets a hash code of the packed vector.
        /// </summary>
        public override int GetHashCode() => PackedValue.GetHashCode();

        #endregion
    }
}
