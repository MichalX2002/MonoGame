// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.


using System;
using System.Runtime.CompilerServices;
using MonoGame.Framework;

namespace MonoGame.Utilities.PackedVector
{
    /// <summary>
    /// Packed pixel type containing two signed 16-bit integers.
    /// <para>
    /// Ranges from [-32768, -32768, 0, 1] to [32767, 32767, 0, 1] in vector form.
    /// </para>
    /// </summary>
    public struct Short2 : IPackedVector<uint>, IEquatable<Short2>
    {
        private static Vector2 Offset = new Vector2(32768);
        private static Vector2 MinNeg = new Vector2(short.MinValue);
        private static Vector2 MaxPos = new Vector2(short.MaxValue);

        public short X;
        public short Y;

        #region Constructors

        /// <summary>
        /// Constructs the packed vector with raw values.
        /// </summary>
        [CLSCompliant(false)]
        public Short2(short x, short y)
        {
            X = x;
            Y = y;
        }

        /// <summary>
        /// Constructs the packed vector with a packed value.
        /// </summary>
        [CLSCompliant(false)]
        public Short2(uint packed) : this() => PackedValue = packed;

        /// <summary>
        /// Constructs the packed vector with vector form values.
        /// </summary>
        /// <param name="vector"><see cref="Vector4"/> containing the components.</param>
        public Short2(Vector2 vector) => this = Pack(vector);

        /// <summary>
        /// Constructs the packed vector with vector form values.
        /// </summary>
        public Short2(float x, float y) : this(new Vector2(x, y))
        {
        }

        #endregion

        public Vector2 ToVector2() => new Vector2(X, Y);

        private static Short2 Pack(Vector2 vector)
        {
            vector = Vector2.Clamp(vector, MinNeg, MaxPos);

            return new Short2(
                (short)Math.Round(vector.X),
                (short)Math.Round(vector.Y));
        }

        #region IPackedVector

        /// <inheritdoc />
        [CLSCompliant(false)]
        public uint PackedValue
        {
            get => Unsafe.As<Short2, uint>(ref this);
            set => Unsafe.As<Short2, uint>(ref this) = value;
        }

        /// <inheritdoc />
        public void FromVector4(Vector4 vector) => this = Pack(vector.ToVector2());

        /// <inheritdoc />
        public Vector4 ToVector4() => new Vector4(X, Y, 0, 1);

        #endregion

        #region IPixel

        /// <inheritdoc/>
        public void FromScaledVector4(Vector4 vector)
        {
            var scaled = vector.ToVector2() * 65535f;
            scaled -= Offset;
            this = Pack(scaled);
        }
        
        /// <inheritdoc/>
        public Vector4 ToScaledVector4()
        {
            var scaled = ToVector2();
            scaled += Offset;
            scaled /= 65535f;
            return new Vector4(scaled, 0, 1);
        }

        #endregion

        #region Equals

        public static bool operator ==(Short2 a, Short2 b) => a.PackedValue == b.PackedValue;
        public static bool operator !=(Short2 a, Short2 b) => a.PackedValue != b.PackedValue;

        public bool Equals(Short2 other) => this == other;
        public override bool Equals(object obj) => obj is Short2 other && Equals(other);

        #endregion

        #region Object Overrides

        public override string ToString() => PackedValue.ToString("X8");

        public override int GetHashCode() => PackedValue.GetHashCode();

        #endregion
    }
}