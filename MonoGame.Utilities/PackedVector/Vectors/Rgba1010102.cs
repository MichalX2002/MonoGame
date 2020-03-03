// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.Runtime.InteropServices;

namespace MonoGame.Framework.PackedVector
{
    /// <summary>
    /// Packed vector type containing unsigned XYZW components.
    /// The XYZ components use 10 bits each, and the W component uses 2 bits.
    /// <para>
    /// Ranges from [0, 0, 0, 0] to [1, 1, 1, 1] in vector form.
    /// </para>
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct Rgba1010102 : IPackedVector<uint>, IEquatable<Rgba1010102>, IPixel
    {
        VectorComponentInfo IPackedVector.ComponentInfo => new VectorComponentInfo(
            new VectorComponent(VectorComponentType.Red, 10),
            new VectorComponent(VectorComponentType.Green, 10),
            new VectorComponent(VectorComponentType.Blue, 10),
            new VectorComponent(VectorComponentType.Alpha, 2));

        /// <summary>
        /// Constructs the packed vector with a packed value.
        /// </summary>
        [CLSCompliant(false)]
        public Rgba1010102(uint packed) => PackedValue = packed;

        /// <summary>
        /// Constructs the packed vector with vector form values.
        /// </summary>
        public Rgba1010102(float x, float y, float z, float w) : this(new Vector4(x, y, z, w))
        {
        }

        /// <summary>
        /// Constructs the packed vector with vector form values.
        /// </summary>
        /// <param name="vector"><see cref="Vector4"/> containing the components.</param>
        public Rgba1010102(Vector4 vector) => PackedValue = Pack(ref vector);

        private static uint Pack(ref Vector4 vector)
        {
            vector = Vector4.Clamp(vector, Vector4.Zero, Vector4.One);

            return (uint)(
                (((int)MathF.Round(vector.X * 1023f) & 0x03FF) << 0) |
                (((int)MathF.Round(vector.Y * 1023f) & 0x03FF) << 10) |
                (((int)MathF.Round(vector.Z * 1023f) & 0x03FF) << 20) |
                (((int)MathF.Round(vector.W * 3f) & 0x03) << 30));
        }

        #region IPackedVector

        [CLSCompliant(false)]
        public uint PackedValue { get; set; }

        public void FromVector4(Vector4 vector) => PackedValue = Pack(ref vector);

        public readonly Vector4 ToVector4() => new Vector4(
            ((PackedValue >> 0) & 0x03FF) / 1023f,
            ((PackedValue >> 10) & 0x03FF) / 1023f,
            ((PackedValue >> 20) & 0x03FF) / 1023f,
            ((PackedValue >> 30) & 0x03) / 3f);

        #endregion

        #region IPixel

        public readonly Vector4 ToScaledVector4() => ToVector4();

        public void FromScaledVector4(Vector4 vector) => FromVector4(vector);

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

        public static bool operator ==(Rgba1010102 a, Rgba1010102 b) => a.PackedValue == b.PackedValue;
        public static bool operator !=(Rgba1010102 a, Rgba1010102 b) => a.PackedValue != b.PackedValue;

        public bool Equals(Rgba1010102 other) => this == other;
        public override bool Equals(object obj) => obj is Rgba1010102 other && Equals(other);

        #endregion

        #region Object Overrides

        /// <summary>
        /// Gets a <see cref="string"/> representation of the packed vector.
        /// </summary>
        public override string ToString() => nameof(Rgba1010102) + $"({ToVector4().ToString()})";

        /// <summary>
        /// Gets a hash code of the packed vector.
        /// </summary>
        public override int GetHashCode() => PackedValue.GetHashCode();

        #endregion
    }
}
