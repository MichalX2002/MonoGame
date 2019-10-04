// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using MonoGame.Framework;

namespace MonoGame.Utilities.PackedVector
{
    /// <summary>
    /// Packed vector type containing a single 8-bit W component.
    /// <para>
    /// Ranges from [1, 1, 1, 0] to [1, 1, 1, 1] in vector form.
    /// </para>
    /// </summary>
    public struct Alpha8 : IPackedVector<byte>, IEquatable<Alpha8>, IPixel
    {
        public byte Value;

        #region Constructors

        /// <summary>
        /// Constructs the packed vector with a packed value.
        /// </summary>
        public Alpha8(byte value) => Value = value;

        /// <summary>
        /// Constructs the packed vector with a raw value.
        /// </summary>
        /// <param name="alpha">The W component.</param>
        public Alpha8(float alpha) => Value = Pack(alpha);

        #endregion

        /// <summary>
        /// Gets the packed vector in <see cref="float"/> format.
        /// </summary>
        public float ToAlpha() => Value / 255f;

        private static byte Pack(float alpha) => 
            (byte)Math.Round(MathHelper.Clamp(alpha, 0, 1) * 255f);

        #region IPixel



        #endregion

        #region IPackedVector

        byte IPackedVector<byte>.PackedValue
        {
            get => Value;
            set => Value = value;
        }

        /// <summary>
        /// Sets the packed vector from a <see cref="Vector4"/>.
        /// </summary>
        public void FromVector4(Vector4 vector) => Value = Pack(vector.W);

        /// <summary>
        /// Gets the packed vector in <see cref="Vector4"/> format.
        /// </summary>
        public Vector4 ToVector4() => new Vector4(0f, 0f, 0f, Value / 255f);

        #endregion

        #region Equals

        public static bool operator ==(Alpha8 a, Alpha8 b) => a.Value == b.Value;
        public static bool operator !=(Alpha8 a, Alpha8 b) => a.Value != b.Value;

        public bool Equals(Alpha8 other) => this == other;
        public override bool Equals(object obj) => obj is Alpha8 value && Equals(value);

        #endregion

        #region Object Overrides

        /// <summary>
        /// Gets a string representation of the packed vector.
        /// </summary>
        public override string ToString() => (Value / 255f).ToString();

        /// <summary>
        /// Gets a hash code of the packed vector.
        /// </summary>
        public override int GetHashCode() => Value.GetHashCode();

        #endregion
    }
}
