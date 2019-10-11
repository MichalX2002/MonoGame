// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.Runtime.CompilerServices;
using MonoGame.Framework;

namespace MonoGame.Utilities.PackedVector
{
    /// <summary>
    /// Packed vector type containing 16-bit floating-point XY components.
    /// <para>Ranges from [0, 0, 0, 1] to [1, 1, 0, 1] in vector form.</para>
    /// </summary>
    public struct HalfVector2 : IPackedVector<uint>, IEquatable<HalfVector2>, IPixel
    {
        public HalfSingle X;
        public HalfSingle Y;

        #region Constructors

        /// <summary>
        /// Constructs the packed vector with raw values.
        /// </summary>
        public HalfVector2(HalfSingle x, HalfSingle y)
        {
            X = x;
            Y = y;
        }

        /// <summary>
        /// Constructs the packed vector with a packed value.
        /// </summary>
        [CLSCompliant(false)]
        public HalfVector2(uint value) : this() => PackedValue = value;

        /// <summary>
        /// Constructs the packed vector with vector form values.
        /// </summary>
        public HalfVector2(float x, float y) => this = Pack(x, y);

        /// <summary>
        /// Constructs the packed vector with vector form values.
        /// </summary>
        /// <param name="vector"><see cref="Vector2"/> containing the components.</param>
        public HalfVector2(Vector2 vector) : this(vector.X, vector.Y)
        {
        }

        #endregion

        private static HalfVector2 Pack(float x, float y)
        {
            //uint num2 = HalfTypeHelper.Pack(x);
            //uint num = (uint)(HalfTypeHelper.Pack(y) << 0x10);
            return new HalfVector2(new HalfSingle(x), new HalfSingle(y));
        }

        public Vector2 ToVector2() => new Vector2(X, Y);
        //    HalfTypeHelper.Unpack((ushort)PackedValue),
        //    HalfTypeHelper.Unpack((ushort)(PackedValue >> 0x10)));

        #region IPackedVector

        /// <inheritdoc/>
        [CLSCompliant(false)]
        public uint PackedValue
        {
            get => Unsafe.As<HalfVector2, uint>(ref this);
            set => Unsafe.As<HalfVector2, uint>(ref this) = value;
        }

        /// <inheritdoc/>
        public void FromVector4(Vector4 vector) => this = Pack(vector.X, vector.Y);

        /// <inheritdoc/>
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
            this = Pack(scaled.X, scaled.Y);
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

        public static bool operator ==(in HalfVector2 a, in HalfVector2 b) => a.PackedValue == b.PackedValue;
        public static bool operator !=(in HalfVector2 a, in HalfVector2 b) => a.PackedValue != b.PackedValue;

        public bool Equals(HalfVector2 other) => this == other;
        public override bool Equals(object obj) => obj is HalfVector2 other && Equals(other);

        #endregion

        #region Object Overrides

        public override string ToString() => ToVector2().ToString();

        public override int GetHashCode() => PackedValue.GetHashCode();

        #endregion
    }
}
