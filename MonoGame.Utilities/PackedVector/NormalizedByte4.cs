// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.Runtime.CompilerServices;
using MonoGame.Framework;

namespace MonoGame.Utilities.PackedVector
{
    /// <summary>
    /// Packed vector type containing signed 8-bit XYZW components.
    /// <para>
    /// Ranges from [-1, -1, -1, -1] to [1, 1, 1, 1] in vector form.
    /// </para>
    /// </summary>
    public struct NormalizedByte4 : IPackedVector<uint>, IEquatable<NormalizedByte4>, IPixel
    {
        [CLSCompliant(false)] 
        public sbyte X;

        [CLSCompliant(false)] 
        public sbyte Y;

        [CLSCompliant(false)] 
        public sbyte Z;

        [CLSCompliant(false)]
        public sbyte W;

        #region Constructors

        /// <summary>
        /// Constructs the packed vector with raw values.
        /// </summary>
        [CLSCompliant(false)]
        public NormalizedByte4(sbyte x, sbyte y, sbyte z, sbyte w)
        {
            X = x;
            Y = y;
            Z = z;
            W = w;
        }

        /// <summary>
        /// Constructs the packed vector with a packed value.
        /// </summary>
        [CLSCompliant(false)]
        public NormalizedByte4(uint packed) : this() => PackedValue = packed;

        /// <summary>
        /// Constructs the packed vector with vector form values.
        /// </summary>
        /// <param name="vector"><see cref="Vector4"/> containing the components.</param>
        public NormalizedByte4(Vector4 vector) => this = Pack(ref vector);

        /// <summary>
        /// Constructs the packed vector with vector form values.
        /// </summary>
        public NormalizedByte4(float x, float y, float z, float w) : this(new Vector4(x, y, z, w))
        {
        }

        #endregion

        private static NormalizedByte4 Pack(ref Vector4 vector)
        {
            vector = Vector4.Clamp(vector, -Vector4.One, Vector4.One);
            vector *= 127f;

            return new NormalizedByte4(
                (sbyte)vector.X,
                (sbyte)vector.Y,
                (sbyte)vector.Z,
                (sbyte)vector.W);
        }

        #region IPackedVector

        /// <inheritdoc/>
        [CLSCompliant(false)]
        public uint PackedValue
        {
            get => Unsafe.As<NormalizedByte4, uint>(ref this);
            set => Unsafe.As<NormalizedByte4, uint>(ref this) = value;
        }

        /// <inheritdoc/>
        public void FromVector4(Vector4 vector) => this = Pack(ref vector);

        /// <inheritdoc/>
        public Vector4 ToVector4() => new Vector4(X, Y, Z, W) / 127f;

        #endregion

        #region IPixel

        /// <inheritdoc/>
        public void FromScaledVector4(Vector4 vector)
        {
            vector *= 2;
            vector -= Vector4.One;
            this = Pack(ref vector);
        }

        /// <inheritdoc/>
        public Vector4 ToScaledVector4()
        {
            var scaled = ToVector4();
            scaled += Vector4.One;
            scaled /= 2;
            return scaled;
        }

        #endregion

        #region Equals

        public static bool operator ==(in NormalizedByte4 a, in NormalizedByte4 b) =>
            a.X == b.X && a.Y == b.Y && a.Z == b.Z && a.W == b.W;

        public static bool operator !=(in NormalizedByte4 a, in NormalizedByte4 b) => !(a == b);

        public bool Equals(NormalizedByte4 other) => this == other;
        public override bool Equals(object obj) => obj is NormalizedByte4 other && Equals(other);

        #endregion

        #region Object Overrides

        public override int GetHashCode() => PackedValue.GetHashCode();
        
        public override string ToString() => PackedValue.ToString("X");

        #endregion
    }
}
