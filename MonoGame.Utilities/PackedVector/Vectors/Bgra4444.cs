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
        private static readonly Vector4 _maxValue = new Vector4(15);

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
        public Bgra4444(Vector4 vector) : this()
        {
            FromVector4(vector);
        }

        /// <summary>
        /// Constructs the packed vector with vector form values.
        /// </summary>
        public Bgra4444(float r, float g, float b, float a) : this(new Vector4(r, g, b, a))
        {
        }

        #endregion

        #region IPackedVector

        [CLSCompliant(false)]
        public ushort PackedValue { get; set; }

        public void FromVector4(in Vector4 vector)
        {
            var v = vector * 15f;
            v += Vector4.Half;
            v.Clamp(Vector4.Zero, _maxValue);

            PackedValue = (ushort)(
                (((int)v.W & 0x0F) << 12) |
                (((int)v.X & 0x0F) << 8) |
                (((int)v.Y & 0x0F) << 4) |
                ((int)v.Z & 0x0F));
        }

        public readonly void ToVector4(out Vector4 vector)
        {
            vector.Base.X = (PackedValue >> 8) & 0x0F;
            vector.Base.Y = (PackedValue >> 4) & 0x0F;
            vector.Base.Z = PackedValue & 0x0F;
            vector.Base.W = (PackedValue >> 12) & 0x0F;
            vector /= 15f;
        }

        public void FromScaledVector4(in Vector4 scaledVector)
        {
            FromVector4(scaledVector);
        }

        public readonly void ToScaledVector4(out Vector4 scaledVector)
        {
            ToVector4(out scaledVector);
        }

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

        public static bool operator ==(Bgra4444 a, Bgra4444 b) => a.PackedValue == b.PackedValue;
        public static bool operator !=(Bgra4444 a, Bgra4444 b) => a.PackedValue != b.PackedValue;

        public bool Equals(Bgra4444 other) => this == other;
        public override bool Equals(object obj) => obj is Bgra4444 other && Equals(other);

        #endregion

        #region Object Overrides

        /// <summary>
        /// Gets a string representation of the packed vector.
        /// </summary>
        public override string ToString() => nameof(Bgra4444) + $"({this.ToVector4()})";

        /// <summary>
        /// Gets a hash code of the packed vector.
        /// </summary>
        public override int GetHashCode() => PackedValue.GetHashCode();

        #endregion
    }
}
