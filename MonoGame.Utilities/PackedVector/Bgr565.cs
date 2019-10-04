// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using MonoGame.Framework;

namespace MonoGame.Utilities.PackedVector
{
    /// <summary>
    /// <para>Packed vector type containing X, Y and Z values.</para>
    /// The X and Z components use 5 bits, and the Y component uses 6 bits.
    /// <para>
    /// Ranges from [0, 0, 0, 1] to [1, 1, 1, 1] in vector form.
    /// </para>
    /// </summary>
    public struct Bgr565 : IPackedVector<ushort>, IEquatable<Bgr565>, IPixel
    {
        [CLSCompliant(false)]
        public ushort Value;

        #region Constructors

        /// <summary>
        /// Constructs the packed vector with a packed value.
        /// </summary>
        /// <param name="alpha">The alpha component.</param>
        [CLSCompliant(false)]
        public Bgr565(ushort value) => Value = value;

        /// <summary>
        /// Constructs the packed vector with raw values.
        /// </summary>
        public Bgr565(float x, float y, float z) => Value = Pack(x, y, z);

        /// <summary>
        /// Constructs the packed vector with raw values.
        /// </summary>
        /// <param name="vector"><see cref="Vector3"/> containing the components.</param>
        public Bgr565(Vector3 vector) => Value = Pack(vector.X, vector.Y, vector.Z);
    
        #endregion

        private static ushort Pack(float x, float y, float z)
        {
            return (ushort)(
                (((int)Math.Round(MathHelper.Clamp(x, 0, 1) * 31f) & 0x1F) << 11) |
                (((int)Math.Round(MathHelper.Clamp(y, 0, 1) * 63.0f) & 0x3F) << 5) |
                ((int)Math.Round(MathHelper.Clamp(z, 0, 1) * 31f) & 0x1F));
        }

        #region IPixel



        #endregion

        #region IPackedVector

        ushort IPackedVector<ushort>.PackedValue
        {
            get => Value;
            set => Value = value;
        }

        /// <summary>
        /// Sets the packed vector from a <see cref="Vector4"/>.
        /// </summary>
        public void FromVector4(Vector4 vector)
        {
            Value = (ushort)(
                (((int)(vector.X * 31f) & 0x1F) << 11) |
                (((int)(vector.Y * 63f) & 0x3F) << 5) |
                ((int)(vector.Z * 31f) & 0x1F));
        }

        /// <summary>
        /// Gets the packed vector in <see cref="Vector4"/> format.
        /// </summary>
        public Vector4 ToVector4()
        {
            return new Vector4(
                ((Value >> 11) & 0x1F) * (1f / 31f),
                ((Value >> 5) & 0x3F) * (1f / 63f),
                (Value & 0x1F) * (1f / 31f),
                1f);
        }

        #endregion

        #region Equals

        public static bool operator ==(Bgr565 a, Bgr565 b) => a.Value == b.Value;
        public static bool operator !=(Bgr565 a, Bgr565 b) => a.Value != b.Value;

        public bool Equals(Bgr565 other) => this == other;
        public override bool Equals(object obj) => obj is Bgr565 value && Equals(value);

        #endregion

        #region Object Overrides

        /// <summary>
        /// Gets a string representation of the packed vector.
        /// </summary>
        public override string ToString() => this.ToVector3().ToString();

        /// <summary>
        /// Gets a hash code of the packed vector.
        /// </summary>
        public override int GetHashCode() => Value.GetHashCode();

        #endregion
    }
}
