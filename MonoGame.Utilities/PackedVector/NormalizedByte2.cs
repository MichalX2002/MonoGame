// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.Runtime.CompilerServices;
using MonoGame.Framework;

namespace MonoGame.Utilities.PackedVector
{
    /// <summary>
    /// Packed vector type containing signed 8-bit XY components.
    /// <para>
    /// Ranges from [-1, -1, 0, 1] to [1, 1, 0, 1] in vector form.
    /// </para>
    /// </summary>
    public struct NormalizedByte2 : IPackedVector<ushort>, IEquatable<NormalizedByte2>, IPixel
    {
        [CLSCompliant(false)]
        public sbyte X;

        [CLSCompliant(false)]
        public sbyte Y;

        #region Constructors

        /// <summary>
        /// Constructs the packed vector with raw values.
        /// </summary>
        [CLSCompliant(false)]
        public NormalizedByte2(sbyte x, sbyte y)
        {
            X = x;
            Y = y;
        }

        /// <summary>
        /// Constructs the packed vector with a packed value.
        /// </summary>
        [CLSCompliant(false)]
        public NormalizedByte2(ushort packed) : this() => PackedValue = packed;

        /// <summary>
        /// Constructs the packed vector with vector form values.
        /// </summary>
        /// <param name="vector"><see cref="Vector4"/> containing the components.</param>
        public NormalizedByte2(Vector2 vector) => this = Pack(vector);

        /// <summary>
        /// Constructs the packed vector with vector form values.
        /// </summary>
        public NormalizedByte2(float x, float y) : this(new Vector2(x, y))
        {
        }

        #endregion

        private static NormalizedByte2 Pack(Vector2 vector)
        {
            vector = Vector2.Clamp(vector, -Vector2.One, Vector2.One);
            vector *= 127f;

            return new NormalizedByte2((sbyte)vector.X, (sbyte)vector.Y);
        }

        public readonly Vector2 ToVector2() => new Vector2(X, Y) / 127f;

        #region IPackedVector

        /// <inheritdoc/>
        [CLSCompliant(false)]
        public ushort PackedValue
        {
            get => Unsafe.As<NormalizedByte2, ushort>(ref this);
            set => Unsafe.As<NormalizedByte2, ushort>(ref this) = value;
        }

        /// <inheritdoc/>
        public void FromVector4(Vector4 vector) => this = Pack(vector.ToVector2());

        /// <inheritdoc/>
        public readonly Vector4 ToVector4() => new Vector4(ToVector2(), 0, 1);

        #endregion

        #region IPixel

        /// <inheritdoc/>
        public void FromScaledVector4(Vector4 vector)
        {
            var scaled = vector.ToVector2();
            scaled *= 2;
            scaled -= Vector2.One;
            this = Pack(scaled);
        }

        /// <inheritdoc/>
        public readonly Vector4 ToScaledVector4()
        {
            var scaled = ToVector2();
            scaled += Vector2.One;
            scaled /= 2;
            return new Vector4(scaled, 0, 1);
        }

        #endregion

        #region Equals

        public static bool operator ==(in NormalizedByte2 a, in NormalizedByte2 b) => a.X == b.X && a.Y == b.Y;
        public static bool operator !=(in NormalizedByte2 a, in NormalizedByte2 b) => !(a == b);

        public override bool Equals(object obj) => obj is NormalizedByte2 other && Equals(other);

        public bool Equals(NormalizedByte2 other) => this == other;

        #endregion

        #region Object Overrides

        public override int GetHashCode() => PackedValue.GetHashCode();

        public override string ToString() => PackedValue.ToString("X");

        #endregion
    }
}
