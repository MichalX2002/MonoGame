// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.Runtime.CompilerServices;
using MonoGame.Framework;

namespace MonoGame.Utilities.PackedVector
{
    /// <summary>
    /// Packed vector type containing a 32-bit XYZ luminance.
    /// <para>
    /// Ranges from [0, 0, 0, 1] to [1, 1, 1, 1] in vector form.
    /// </para>
    /// </summary>
    public struct Gray32 : IPackedVector<uint>, IEquatable<Gray32>, IPixel
    {
        public float Value;

        #region IPackedVector

        [CLSCompliant(false)]
        public uint PackedValue
        {
            get => Unsafe.As<Gray32, uint>(ref this);
            set => Unsafe.As<Gray32, uint>(ref this) = value;
        }

        /// <inheritdoc/>
        public void FromVector4(Vector4 vector)
        {
            vector = Vector4.Clamp(vector, Vector4.Zero, Vector4.One);
            Value = PackedVectorHelper.GetBT709Luminance(vector.X, vector.Y, vector.Z);
        }

        /// <inheritdoc/>
        public readonly Vector4 ToVector4() => new Vector4(Value, Value, Value, 1f);

        #endregion

        #region IPixel

        /// <inheritdoc/>
        public void FromScaledVector4(Vector4 vector) => FromVector4(vector);

        /// <inheritdoc/>
        public readonly Vector4 ToScaledVector4() => ToVector4();

        /// <inheritdoc/>
        public void FromGray8(Gray8 source) => Value = source.PackedValue / 255f;

        /// <inheritdoc/>
        public void FromGray16(Gray16 source) => Value = source.PackedValue / 65535f;

        /// <inheritdoc/>
        public void FromGrayAlpha16(GrayAlpha16 source) => FromGray8(source.Luminance);

        /// <inheritdoc/>
        public void FromRgb24(Rgb24 source) =>
            Value = PackedVectorHelper.GetBT709Luminance(source.R, source.G, source.B) / 255f;

        /// <inheritdoc/>
        public void FromColor(Color source) =>
            Value = PackedVectorHelper.GetBT709Luminance(source.R, source.G, source.B) / 255f;

        /// <inheritdoc/>
        public void FromRgb48(Rgb48 source) =>
            Value = PackedVectorHelper.GetBT709Luminance(source.R, source.G, source.B) / 65535f;

        /// <inheritdoc/>
        public void FromRgba64(Rgba64 source) =>
            Value = PackedVectorHelper.GetBT709Luminance(source.R, source.G, source.B) / 65535f;

        /// <inheritdoc/>
        public readonly void ToColor(ref Color destination)
        {
            destination.R = destination.G = destination.B = (byte)(Value * 255);
            destination.A = byte.MaxValue;
        }

        #endregion

        #region Equals

        public static bool operator ==(in Gray32 a, in Gray32 b) => a.PackedValue == b.PackedValue;
        public static bool operator !=(in Gray32 a, in Gray32 b) => a.PackedValue != b.PackedValue;

        public bool Equals(Gray32 other) => this == other;
        public override bool Equals(object obj) => obj is Gray32 other && Equals(other);

        #endregion

        #region Object Overrides

        public override string ToString() => Value.ToString();

        public override int GetHashCode() => Value.GetHashCode();

        #endregion
    }
}
