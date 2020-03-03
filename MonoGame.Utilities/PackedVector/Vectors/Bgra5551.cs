// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.Runtime.InteropServices;

namespace MonoGame.Framework.PackedVector
{
    /// <summary>
    /// Packed vector type containing unsigned XYZW components.
    /// The XYZ components use 5 bits each, and the W component uses 1 bit.
    /// <para>
    /// Ranges from [0, 0, 0, 0] to [1, 1, 1, 1] in vector form.
    /// </para>
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct Bgra5551 : IPackedVector<ushort>, IEquatable<Bgra5551>, IPixel
    {
        VectorComponentInfo IPackedVector.ComponentInfo => new VectorComponentInfo(
            new VectorComponent(VectorComponentType.Blue, 5),
            new VectorComponent(VectorComponentType.Green, 5),
            new VectorComponent(VectorComponentType.Red, 5),
            new VectorComponent(VectorComponentType.Alpha, 1));

        #region Constructors

        /// <summary>
        /// Constructs the packed vector with a packed value.
        /// </summary>
        [CLSCompliant(false)]
        public Bgra5551(ushort packed) => PackedValue = packed;

        /// <summary>
        /// Constructs the packed vector with vector form values.
        /// </summary>
        /// <param name="vector"><see cref="Vector4"/> containing the components.</param>
        public Bgra5551(Vector4 vector) => PackedValue = Pack(ref vector);

        /// <summary>
        /// Constructs the packed vector with vector form values.
        /// </summary>
        public Bgra5551(float r, float g, float b, float a) : this(new Vector4(r, g, b, a))
        {
        }

        #endregion

        private static ushort Pack(ref Vector4 vector)
        {
            vector = Vector4.Clamp(vector, Vector4.Zero, Vector4.One);

            return (ushort)(
                (((int)MathF.Round(vector.X * 31f) & 0x1F) << 10) |
                (((int)MathF.Round(vector.Y * 31f) & 0x1F) << 5) |
                (((int)MathF.Round(vector.Z * 31f) & 0x1F) << 0) |
                (((int)MathF.Round(vector.W) & 0x1) << 15));
        }

        #region IPackedVector

        [CLSCompliant(false)]
        public ushort PackedValue { get; set; }

        public void FromVector4(Vector4 vector) => PackedValue = Pack(ref vector);

        public readonly Vector4 ToVector4()
        {
            return new Vector4(
                ((PackedValue >> 10) & 0x1F) / 31f,
                ((PackedValue >> 5) & 0x1F) / 31f,
                ((PackedValue >> 0) & 0x1F) / 31f,
                (PackedValue >> 15) & 0x01);
        }

        #endregion

        #region IPixel

        public void FromScaledVector4(Vector4 vector) => FromVector4(vector);

        public readonly Vector4 ToScaledVector4() => ToVector4();

        public readonly void ToColor(ref Color destination) => destination.FromScaledVector4(ToScaledVector4());

        public void FromGray8(Gray8 source) => FromScaledVector4(source.ToScaledVector4());

        public void FromGray16(Gray16 source) => FromScaledVector4(source.ToScaledVector4());

        public void FromGrayAlpha16(GrayAlpha16 source) => FromScaledVector4(source.ToScaledVector4());

        public void FromRgb24(Rgb24 source) => FromScaledVector4(source.ToScaledVector4());

        public void FromColor(Color source) => FromScaledVector4(source.ToScaledVector4());

        public void FromRgb48(Rgb48 source) => FromScaledVector4(source.ToScaledVector4());

        public void FromRgba64(Rgba64 source) => FromScaledVector4(source.ToScaledVector4());

        #endregion

        #region Equals

        public static bool operator ==(Bgra5551 a, Bgra5551 b) => a.PackedValue == b.PackedValue;
        public static bool operator !=(Bgra5551 a, Bgra5551 b) => a.PackedValue != b.PackedValue;

        public bool Equals(Bgra5551 other) => this == other;
        public override bool Equals(object obj) => obj is Bgra5551 other && Equals(other);

        #endregion

        #region Object Overrides

        /// <summary>
        /// Gets a string representation of the packed vector.
        /// </summary>
        public override string ToString() => nameof(Bgra5551) + $"({ToVector4().ToString()}";

        /// <summary>
        /// Gets a hash code of the packed vector.
        /// </summary>
        public override int GetHashCode() => PackedValue.GetHashCode();

        #endregion
    }
}