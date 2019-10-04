// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using MonoGame.Framework;

namespace MonoGame.Utilities.PackedVector
{
    /// <summary>
    /// <para>Packed vector type containing X, Y, Z and W values.</para>
    /// Each component uses 4 bits.
    /// <para>
    /// Ranges from [0, 0, 0, 0] to [1, 1, 1, 1] in vector form.
    /// </para>
    /// </summary>
    public struct Bgra4444 : IPackedVector<ushort>, IEquatable<Bgra4444>
    {
        [CLSCompliant(false)]
        public ushort Value;

        #region Constructors

        /// <summary>
        /// Constructs the packed vector with a packed value.
        /// </summary>
        [CLSCompliant(false)]
        public Bgra4444(ushort value) => Value = value;

        /// <summary>
        /// Constructs the packed vector with raw values.
        /// </summary>
        public Bgra4444(float x, float y, float z, float w) => Value = Pack(x, y, z, w);

        /// <summary>
        /// Constructs the packed vector with raw values.
        /// </summary>
        /// <param name="vector"><see cref="Vector4"/> containing the components.</param>
        public Bgra4444(Vector4 vector) => Value = Pack(vector.X, vector.Y, vector.Z, vector.W);
    
        #endregion

        private static ushort Pack(float x, float y, float z, float w)
        {
            return (ushort)(
                (((int)Math.Round(MathHelper.Clamp(w, 0, 1) * 15f) & 0x0F) << 12) |
                (((int)Math.Round(MathHelper.Clamp(x, 0, 1) * 15f) & 0x0F) << 8) |
                (((int)Math.Round(MathHelper.Clamp(y, 0, 1) * 15f) & 0x0F) << 4) |
                ((int)Math.Round(MathHelper.Clamp(z, 0, 1) * 15f) & 0x0F));
        }

        #region IPixel



        #endregion

        #region IPackedVector

        ushort IPackedVector<ushort>.PackedValue
        {
            get => Value;
            set => Value = value;
        }

        public void FromVector4(Vector4 vector) =>
            Value = Pack(vector.X, vector.Y, vector.Z, vector.W);

        public Vector4 ToVector4()
        {
            const float maxValue = 1 / 15f;
            return new Vector4( 
                ((Value >> 8) & 0x0F) * maxValue,
                ((Value >> 4) & 0x0F) * maxValue,
                (Value & 0x0F) * maxValue,
                ((Value >> 12) & 0x0F) * maxValue);
        }

        #endregion

        #region Equals

        public static bool operator ==(Bgra4444 a, Bgra4444 b) => a.Value == b.Value;
        public static bool operator !=(Bgra4444 a, Bgra4444 b) => a.Value != b.Value;

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
        public override int GetHashCode() => Value.GetHashCode();

        #endregion
    }
}
