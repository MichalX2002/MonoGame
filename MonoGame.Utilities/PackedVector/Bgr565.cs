// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;

namespace MonoGame.Framework.PackedVector
{
    /// <summary>
    /// Packed vector type containing unsigned XYZ components.
    /// The XZ components use 5 bits each, and the Y component uses 6 bits.
    /// <para>
    /// Ranges from [0, 0, 0, 1] to [1, 1, 1, 1] in vector form.
    /// </para>
    /// </summary>
    public struct Bgr565 : IPackedVector<ushort>, IEquatable<Bgr565>, IPixel
    {
        #region Constructors

        /// <summary>
        /// Constructs the packed vector with a packed value.
        /// </summary>
        /// <param name="alpha">The alpha component.</param>
        [CLSCompliant(false)]
        public Bgr565(ushort packed) => PackedValue = packed;

        /// <summary>
        /// Constructs the packed vector with vector form values.
        /// </summary>
        /// <param name="vector"><see cref="Vector3"/> containing the components.</param>
        public Bgr565(Vector3 vector) => PackedValue = Pack(vector);

        /// <summary>
        /// Constructs the packed vector with vector form values.
        /// </summary>
        public Bgr565(float r, float g, float b) : this(new Vector3(r, g, b))
        {
        }

        #endregion

        private static ushort Pack(Vector3 vector)
        {
            vector = Vector3.Clamp(vector, Vector3.Zero, Vector3.One);
            
            return (ushort)(
                (((int)Math.Round(vector.X * 31f) & 0x1F) << 11) |
                (((int)Math.Round(vector.Y * 63f) & 0x3F) << 5) |
                ((int)Math.Round(vector.Z * 31f) & 0x1F));
        }

        #region IPackedVector

        /// <inheritdoc/>
        [CLSCompliant(false)]
        public ushort PackedValue { get; set; }

        /// <inheritdoc/>
        public void FromVector4(Vector4 vector) => Pack(vector.XYZ);

        /// <inheritdoc/>
        public readonly Vector4 ToVector4()
        {
            return new Vector4(
                ((PackedValue >> 11) & 0x1F) * (1f / 31f),
                ((PackedValue >> 5) & 0x3F) * (1f / 63f),
                (PackedValue & 0x1F) * (1f / 31f),
                1f);
        }

        #endregion

        #region IPixel

        /// <inheritdoc/>
        public void FromScaledVector4(Vector4 vector) => FromVector4(vector);

        /// <inheritdoc/>
        public readonly Vector4 ToScaledVector4() => ToVector4();

        #endregion

        #region Equals

        public static bool operator ==(Bgr565 a, Bgr565 b) => a.PackedValue == b.PackedValue;
        public static bool operator !=(Bgr565 a, Bgr565 b) => a.PackedValue != b.PackedValue;

        public bool Equals(Bgr565 other) => this == other;
        public override bool Equals(object obj) => obj is Bgr565 other && Equals(other);

        #endregion

        #region Object Overrides

        /// <summary>
        /// Gets a string representation of the packed vector.
        /// </summary>
        public override string ToString() => $"Bgr565({ToVector4().XYZ.ToString()})";

        /// <summary>
        /// Gets a hash code of the packed vector.
        /// </summary>
        public override int GetHashCode() => PackedValue.GetHashCode();

        #endregion
    }
}
