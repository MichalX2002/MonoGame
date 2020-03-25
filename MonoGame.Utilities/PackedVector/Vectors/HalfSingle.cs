// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.Runtime.InteropServices;

namespace MonoGame.Framework.PackedVector
{
    /// <summary>
    /// Packed vector type containing a 16-bit floating-point X component.
    /// <para>Ranges from [-1, 0, 0, 1] to [1, 0, 0, 1] in vector form.</para>
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct HalfSingle : IPackedVector<ushort>, IEquatable<HalfSingle>, IPixel
    {
        VectorComponentInfo IPackedVector.ComponentInfo => new VectorComponentInfo(
            new VectorComponent(VectorComponentType.Red, sizeof(ushort) * 8));

        [CLSCompliant(false)]
        public ushort X;

        #region Constructors

        /// <summary>
        /// Constructs the packed vector with raw values.
        /// </summary>
        [CLSCompliant(false)]
        public HalfSingle(ushort x) => X = x;

        /// <summary>
        /// Constructs the packed vector with a vector form value.
        /// </summary>
        public HalfSingle(float single) : this(HalfTypeHelper.Pack(single))
        {
        }

        #endregion

        /// <summary>
        /// Gets the packed vector as a <see cref="float"/>.
        /// </summary>
        public readonly float ToSingle() => HalfTypeHelper.Unpack(PackedValue);

        #region IPackedVector

        [CLSCompliant(false)]
        public ushort PackedValue { readonly get => X; set => X = value; }

        public void FromVector4(in Vector4 vector) => X = HalfTypeHelper.Pack(vector.X);

        public readonly void ToVector4(out Vector4 vector)
        {
            vector.Base.X = ToSingle();
            vector.Base.Y = 0;
            vector.Base.Z = 0;
            vector.Base.W = 1;
        }

        public void FromScaledVector4(in Vector4 vector)
        {
            float scaled = vector.X;
            scaled *= 2;
            scaled -= 1;
            PackedValue = HalfTypeHelper.Pack(scaled);
        }

        public readonly void ToScaledVector4(out Vector4 scaledVector)
        {
            scaledVector.Base.X = (ToSingle() + 1) / 2f;
            scaledVector.Base.Y = 0;
            scaledVector.Base.Z = 0;
            scaledVector.Base.W = 1;
        }

        #endregion

        #region IPixel

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

        public void FromColor(Color source)
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

        public static implicit operator float(HalfSingle value) => value.ToSingle();

        #region Equals

        public static bool operator ==(HalfSingle a, HalfSingle b) => a.PackedValue == b.PackedValue;
        public static bool operator !=(HalfSingle a, HalfSingle b) => a.PackedValue != b.PackedValue;

        public bool Equals(HalfSingle other) => this == other;
        public override bool Equals(object obj) => obj is HalfSingle other && Equals(other);

        #endregion

        #region Object Overrides

        /// <summary>
        /// Gets a string representation of the packed vector.
        /// </summary>
        public override string ToString() => ToSingle().ToString();

        /// <summary>
        /// Gets a hash code of the packed vector.
        /// </summary>
        public override int GetHashCode() => PackedValue.GetHashCode();

        #endregion
    }
}
