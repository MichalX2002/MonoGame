// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.Runtime.InteropServices;

namespace MonoGame.Framework.PackedVector
{
    /// <summary>
    /// Packed vector type containing unsigned 4-bit XYZW components.
    /// <para>
    /// Ranges from [0, 0, 0, 0] to [1, 1, 1, 1] in vector form.
    /// </para>
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct Bgra4444 : IPackedVector<ushort>, IEquatable<Bgra4444>, IPixel
    {
        VectorComponentInfo IPackedVector.ComponentInfo => new VectorComponentInfo(
            new VectorComponent(VectorComponentType.Blue, 4),
            new VectorComponent(VectorComponentType.Green, 4),
            new VectorComponent(VectorComponentType.Red, 4),
            new VectorComponent(VectorComponentType.Alpha, 4));

        #region Constructors

        /// <summary>
        /// Constructs the packed vector with a packed value.
        /// </summary>
        [CLSCompliant(false)]
        public Bgra4444(ushort packed) => PackedValue = packed;

        /// <summary>
        /// Constructs the packed vector with vector form values.
        /// </summary>
        /// <param name="vector"><see cref="Vector4"/> containing the components.</param>
        public Bgra4444(Vector4 vector) => PackedValue = Pack(ref vector);

        /// <summary>
        /// Constructs the packed vector with vector form values.
        /// </summary>
        public Bgra4444(float r, float g, float b, float a) : this(new Vector4(r, g, b, a))
        {
        }

        #endregion

        private static ushort Pack(ref Vector4 vector)
        {
            vector = Vector4.Clamp(vector, Vector4.Zero, Vector4.One);
            vector *= 15f;
            vector.Round();

            return (ushort)(
                (((int)vector.W & 0x0F) << 12) |
                (((int)vector.X & 0x0F) << 8) |
                (((int)vector.Y & 0x0F) << 4) |
                ((int)vector.Z & 0x0F));
        }

        #region IPackedVector

        [CLSCompliant(false)]
        public ushort PackedValue { get; set; }

        public void FromVector4(Vector4 vector) => PackedValue = Pack(ref vector);

        public readonly Vector4 ToVector4()
        {
            return Vector4.Multiply(new Vector4(
                (PackedValue >> 8) & 0x0F,
                (PackedValue >> 4) & 0x0F,
                PackedValue & 0x0F,
                (PackedValue >> 12) & 0x0F),
                scaleFactor: 1 / 15f);
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

        public static bool operator ==(Bgra4444 a, Bgra4444 b) => a.PackedValue == b.PackedValue;
        public static bool operator !=(Bgra4444 a, Bgra4444 b) => a.PackedValue != b.PackedValue;

        public bool Equals(Bgra4444 other) => this == other;
        public override bool Equals(object obj) => obj is Bgra4444 other && Equals(other);

        #endregion

        #region Object Overrides

        /// <summary>
        /// Gets a string representation of the packed vector.
        /// </summary>
        public override string ToString() => nameof(Bgra4444) + $"({ToVector4().ToString()})";

        /// <summary>
        /// Gets a hash code of the packed vector.
        /// </summary>
        public override int GetHashCode() => PackedValue.GetHashCode();

        #endregion
    }
}
