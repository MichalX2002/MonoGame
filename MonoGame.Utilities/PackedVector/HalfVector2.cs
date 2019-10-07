// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using MonoGame.Framework;

namespace MonoGame.Utilities.PackedVector
{
    /// <summary>
    /// Packed vector type containing 16-bit floating-point XY components.
    /// <para>Ranges from [0, 0, 0, 1] to [1, 1, 0, 1] in vector form.</para>
    /// </summary>
    public struct HalfVector2 : IPackedVector<uint>, IEquatable<HalfVector2>, IPixel
    {
        #region Constructors

        /// <summary>
        /// Constructs the packed vector with a packed value.
        /// </summary>
        [CLSCompliant(false)]
        public HalfVector2(uint value) => PackedValue = value;

        /// <summary>
        /// Constructs the packed vector with raw values.
        /// </summary>
        public HalfVector2(float x, float y) => PackedValue = Pack(x, y);

        /// <summary>
        /// Constructs the packed vector with raw values.
        /// </summary>
        /// <param name="vector"><see cref="Vector2"/> containing the components.</param>
        public HalfVector2(Vector2 vector) : this(vector.X, vector.Y)
        {
        }

        #endregion

        private static uint Pack(float x, float y)
        {
            uint num2 = HalfTypeHelper.Pack(x);
            uint num = (uint)(HalfTypeHelper.Pack(y) << 0x10);
            return num2 | num;
        }

        public Vector2 ToVector2() => new Vector2(
            HalfTypeHelper.Unpack((ushort)PackedValue),
            HalfTypeHelper.Unpack((ushort)(PackedValue >> 0x10)));

        #region IPackedVector

        /// <inheritdoc/>
        [CLSCompliant(false)]
        public uint PackedValue { get; set; }

        /// <inheritdoc />
        public void FromVector4(Vector4 vector) => PackedValue = Pack(vector.X, vector.Y);

        /// <inheritdoc />
        public Vector4 ToVector4()
        {
            var vector = ToVector2();
            return new Vector4(vector.X, vector.Y, 0, 1);
        }

        #endregion

        #region IPixel

        /// <inheritdoc/>
        public void FromScaledVector4(Vector4 vector)
        {
            var scaled = new Vector2(vector.X, vector.Y);
            scaled *= 2;
            scaled -= Vector2.One;
            PackedValue = Pack(scaled.X, scaled.Y);
        }

        /// <inheritdoc/>
        public Vector4 ToScaledVector4()
        {
            var scaled = ToVector2();
            scaled += Vector2.One;
            scaled /= 2F;
            return new Vector4(scaled, 0, 1);
        }

        #endregion

        #region Equals

        public static bool operator ==(in HalfVector2 a, in HalfVector2 b) => a.PackedValue.Equals(b.PackedValue);

        public static bool operator !=(in HalfVector2 a, in HalfVector2 b) => !(a == b);

        public bool Equals(HalfVector2 other) => this == other;

        public override bool Equals(object obj) => obj is HalfVector2 other && Equals(other);

        #endregion

        #region Object Overrides

        public override string ToString() => ToVector2().ToString();

        public override int GetHashCode() => PackedValue.GetHashCode();

        #endregion
    }
}
