// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using MonoGame.Framework;

namespace MonoGame.Utilities.PackedVector
{
    /// <summary>
    /// Packed vector type containing unsigned 4-bit XYZW components.
    /// <para>Ranges from [0, 0, 0, 0] to [1, 1, 1, 1] in vector form.</para>
    /// </summary>
    public struct Bgra4444 : IPackedVector<ushort>, IEquatable<Bgra4444>, IPixel
    {
        #region Constructors

        /// <summary>
        /// Constructs the packed vector with a packed value.
        /// </summary>
        [CLSCompliant(false)]
        public Bgra4444(ushort value) => PackedValue = value;

        /// <summary>
        /// Constructs the packed vector with raw values.
        /// </summary>
        /// <param name="vector"><see cref="Vector4"/> containing the components.</param>
        public Bgra4444(Vector4 vector) => PackedValue = Pack(ref vector);

        /// <summary>
        /// Constructs the packed vector with raw values.
        /// </summary>
        public Bgra4444(float x, float y, float z, float w) : this(new Vector4(x, y, z, w))
        {
        }

        #endregion

        private static ushort Pack(ref Vector4 vector)
        {
            vector = Vector4.Clamp(vector, Vector4.Zero, Vector4.One);
            vector *= 15f;

            return (ushort)(
                (((int)Math.Round(vector.W) & 0x0F) << 12) |
                (((int)Math.Round(vector.X) & 0x0F) << 8) |
                (((int)Math.Round(vector.Y) & 0x0F) << 4) |
                ((int)Math.Round(vector.Z) & 0x0F));
        }

        #region IPixel



        #endregion

        #region IPackedVector

        /// <inheritdoc />
        [CLSCompliant(false)]
        public ushort PackedValue { get; set; }

        /// <inheritdoc />
        public void FromVector4(Vector4 vector) => PackedValue = Pack(ref vector);

        /// <inheritdoc />
        public Vector4 ToVector4()
        {
            return Vector4.Multiply(new Vector4(
                (PackedValue >> 8) & 0x0F,
                (PackedValue >> 4) & 0x0F,
                PackedValue & 0x0F,
                (PackedValue >> 12) & 0x0F), 
                scaleFactor: 1 / 15f);
        }

        #endregion

        #region Equals

        public static bool operator ==(Bgra4444 a, Bgra4444 b) => a.PackedValue == b.PackedValue;
        public static bool operator !=(Bgra4444 a, Bgra4444 b) => a.PackedValue != b.PackedValue;

        public bool Equals(Bgra4444 other) => this == other;
        public override bool Equals(object obj) => obj is Bgra4444 value && Equals(value);

        #endregion

        #region Object Overrides

        /// <summary>
        /// Gets a string representation of the packed vector.
        /// </summary>
        public override string ToString() => ToVector4().ToString();

        /// <summary>
        /// Gets a hash code of the packed vector.
        /// </summary>
        public override int GetHashCode() => PackedValue.GetHashCode();

        #endregion
    }
}
