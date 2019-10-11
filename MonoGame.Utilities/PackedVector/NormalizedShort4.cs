// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.Runtime.CompilerServices;
using MonoGame.Framework;

namespace MonoGame.Utilities.PackedVector
{
    /// <summary>
    /// Packed vector type containing signed 16-bit XYZW components.
    /// <para>
    /// Ranges from [-1, -1, -1, -1] to [1, 1, 1, 1] in vector form.
    /// </para>
    /// </summary>
    public struct NormalizedShort4 : IPackedVector<ulong>, IEquatable<NormalizedShort4>
	{
        public short X;
        public short Y;
        public short Z;
        public short W;

        public NormalizedShort4(Vector4 vector) => this = Pack(ref vector);

        public NormalizedShort4(float x, float y, float z, float w) : this(new Vector4(x, y, z, w))
        {
        }

        private static NormalizedShort4 Pack(ref Vector4 vector)
        {
            vector = Vector4.Clamp(vector, -Vector4.One, Vector4.One);
            vector *= 32767f;
            
            return new NormalizedShort4(
                (short)vector.X,
                (short)vector.Y,
                (short)vector.Z,
                (short)vector.W);
        }

        #region IPackedVector

        /// <inheritdoc/>
        [CLSCompliant(false)]
        public ulong PackedValue
        {
            get => Unsafe.As<NormalizedShort4, ulong>(ref this);
            set => Unsafe.As<NormalizedShort4, ulong>(ref this) = value;
        }

        /// <inheritdoc/>
        public void FromVector4(Vector4 vector) => this = Pack(ref vector);

        /// <inheritdoc/>
        public Vector4 ToVector4() => new Vector4(X, Y, Z, W) / 32767f;

        #endregion

        #region Equals

        public static bool operator ==(NormalizedShort4 a, NormalizedShort4 b) => a.PackedValue == b.PackedValue;
        public static bool operator !=(NormalizedShort4 a, NormalizedShort4 b) => a.PackedValue == b.PackedValue;

        public bool Equals(NormalizedShort4 other) => this == other;
        public override bool Equals(object obj) => obj is NormalizedShort4 other && Equals(other);

        #endregion

        #region Object Overrides

        public override string ToString() => PackedValue.ToString("X");

        public override int GetHashCode() => PackedValue.GetHashCode();

        #endregion
    }
}
