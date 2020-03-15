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
        /// <param name="vector"><see cref="Vector4"/> containing the components.</param>
        public Rgba1010102(in Vector4 vector) : this()
        {
            FromVector4(vector);
        }

        /// <summary>
        /// Constructs the packed vector with vector form values.
        /// </summary>
        public Rgba1010102(float x, float y, float z, float w) : this(new Vector4(x, y, z, w))
        {
        }

        #region IPackedVector

        [CLSCompliant(false)]
        public uint PackedValue { get; set; }

        public void FromVector4(in Vector4 vector)
        {
            Vector4.Clamp(vector, 0, 1, out var v);

            PackedValue = (uint)(
                (((int)MathF.Round(v.X * 1023f) & 0x03FF) << 0) |
                (((int)MathF.Round(v.Y * 1023f) & 0x03FF) << 10) |
                (((int)MathF.Round(v.Z * 1023f) & 0x03FF) << 20) |
                (((int)MathF.Round(v.W * 3f) & 0x03) << 30));
        }

        public readonly void ToVector4(out Vector4 vector)
        {
            vector.X = ((PackedValue >> 0) & 0x03FF) / 1023f;
            vector.Y = ((PackedValue >> 10) & 0x03FF) / 1023f;
            vector.Z = ((PackedValue >> 20) & 0x03FF) / 1023f;
            vector.W = ((PackedValue >> 30) & 0x03) / 3f;
        }

        public readonly void ToScaledVector4(out Vector4 scaledVector) => ToVector4(out scaledVector);

        public void FromScaledVector4(in Vector4 vector) => FromVector4(vector);

        #endregion

        #region IPixel

        public void FromColor(Color source)
        {
            source.ToScaledVector4(out var vector);
            FromScaledVector4(vector);
        }

        public void FromGray8(Gray8 source)
        {
            source.ToScaledVector4(out var vector);
            FromScaledVector4(vector);
        }

        public void FromGray16(Gray16 source)
        {
            source.ToScaledVector4(out var vector);
            FromScaledVector4(vector);
        }

        public void FromGrayAlpha16(GrayAlpha16 source)
        {
            source.ToScaledVector4(out var vector);
            FromScaledVector4(vector);
        }

        public void FromRgb24(Rgb24 source)
        {
            source.ToScaledVector4(out var vector);
            FromScaledVector4(vector);
        }

        public void FromRgb48(Rgb48 source)
        {
            source.ToScaledVector4(out var vector);
            FromScaledVector4(vector);
        }

        public void FromRgba64(Rgba64 source)
        {
            source.ToScaledVector4(out var vector);
            FromScaledVector4(vector);
        }

        public readonly void ToColor(ref Color destination)
        {
            ToScaledVector4(out var vector);
            destination.FromScaledVector4(vector);
        }

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
        public override string ToString() => nameof(Rgba1010102) + $"({this.ToVector4().ToString()})";

        /// <summary>
        /// Gets a hash code of the packed vector.
        /// </summary>
        public override int GetHashCode() => PackedValue.GetHashCode();

        #endregion
    }
}
