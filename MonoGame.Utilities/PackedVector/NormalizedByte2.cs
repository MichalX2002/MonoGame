// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using MonoGame.Framework;

namespace MonoGame.Utilities.PackedVector
{
    public struct NormalizedByte2 : IPackedVector<ushort>, IEquatable<NormalizedByte2>, IPixel
    {
        #region Constructors

        /// <summary>
        /// Constructs the packed vector with a packed value.
        /// </summary>
        [CLSCompliant(false)]
        public NormalizedByte2(ushort packed) => PackedValue = packed;

        /// <summary>
        /// Constructs the packed vector with raw values.
        /// </summary>
        /// <param name="vector"><see cref="Vector4"/> containing the components.</param>
        public NormalizedByte2(Vector2 vector) => PackedValue = Pack(vector);

        /// <summary>
        /// Constructs the packed vector with raw values.
        /// </summary>
        public NormalizedByte2(float x, float y) : this(new Vector2(x, y))
        { 
        }

        #endregion

        private static ushort Pack(in Vector2 vector)
        {
            int byte2 = (((ushort)Math.Round(MathHelper.Clamp(vector.X, 0, 1) * 255f)) & 0xFF) << 0;
            int byte1 = (((ushort)Math.Round(MathHelper.Clamp(vector.Y, 0, 1) * 255f)) & 0xFF) << 8;
            return (ushort)(byte2 | byte1);
        }

        public Vector2 ToVector2()
        {
            return new Vector2(
                ((byte)((PackedValue >> 0) & 0xFF)) / 255f,
                ((byte)((PackedValue >> 8) & 0xFF)) / 255f);
        }

        #region IPackedVector

        /// <inheritdoc/>
        [CLSCompliant(false)]
        public ushort PackedValue { get; set; }

        /// <inheritdoc/>
        public void FromVector4(Vector4 vector) => 
            PackedValue = Pack(new Vector2(vector.X, vector.Y));

        /// <inheritdoc/>
        public Vector4 ToVector4() => new Vector4(ToVector2(), 0, 1);

        #endregion

        #region IPixel

        /// <inheritdoc/>
        public void FromScaledVector4(Vector4 vector)
        {
            var scaled = new Vector2(vector.X, vector.Y);
            scaled *= 2;
            scaled -= Vector2.One;
            PackedValue = Pack(scaled);
        }

        /// <inheritdoc/>
        public Vector4 ToScaledVector4()
        {
            var scaled = ToVector2();
            scaled += Vector2.One;
            scaled /= 2;
            return new Vector4(scaled, 0, 1);
        }

        #endregion

        #region Equals

        public static bool operator ==(NormalizedByte2 a, NormalizedByte2 b) => a.PackedValue == b.PackedValue;

        public static bool operator !=(NormalizedByte2 a, NormalizedByte2 b) => !(a == b);

        public override bool Equals(object obj) => obj is NormalizedByte2 other && Equals(other);

        public bool Equals(NormalizedByte2 other) => this == other;

        #endregion

        #region Object Overrides

        public override int GetHashCode() => PackedValue.GetHashCode();

        public override string ToString() => PackedValue.ToString("X");

        #endregion
    }
}
