using System;
using System.Runtime.CompilerServices;
using MonoGame.Framework;

namespace MonoGame.Utilities.PackedVector
{
    /// <summary>
    /// Packed vector type containing a single 32-bit floating-point gray value.
    /// <para>
    /// Ranges from [0, 0, 0, 1] to [1, 1, 1, 1] in vector form.
    /// </para>
    /// </summary>
    public struct PackedSingle : IPackedVector<uint>, IPixel
    {
        public float Value;

        #region IPackedVector

        [CLSCompliant(false)]
        public uint PackedValue
        {
            get => Unsafe.As<PackedSingle, uint>(ref this);
            set => Unsafe.As<PackedSingle, uint>(ref this) = value;
        }

        /// <inheritdoc/>
        public void FromVector4(Vector4 vector)
        {
            vector = Vector4.Clamp(vector, Vector4.Zero, Vector4.One);
            Value = PackedVectorHelper.GetBT709Luminance(vector.X, vector.Y, vector.Z);
        }

        /// <inheritdoc/>
        public Vector4 ToVector4() => new Vector4(Value, Value, Value, 1f);

        #endregion

        #region IPixel

        /// <inheritdoc/>
        public void FromScaledVector4(Vector4 vector) => FromVector4(vector);

        /// <inheritdoc/>
        public Vector4 ToScaledVector4() => ToVector4();

        /// <inheritdoc />
        public void FromGray8(Gray8 source) => Value = source.PackedValue / 255f;

        /// <inheritdoc />
        public void FromGray16(Gray16 source) => Value = source.PackedValue / 65535f;

        /// <inheritdoc />
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

        /// <inheritdoc />
        public void ToColor(ref Color dest)
        {
            dest.R = dest.G = dest.B = (byte)(Value * 255f);
            dest.A = byte.MaxValue;
        }

        #endregion
    }
}
