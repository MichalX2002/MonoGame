// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace MonoGame.Framework.PackedVector
{
    /// <summary>
    /// Packed vector type containing four unsigned 16-bit XYZW components.
    /// <para>
    /// Ranges from [0, 0, 0, 0] to [1, 1, 1, 1] in vector form.
    /// </para>
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct Rgba64 : IPackedVector<ulong>, IEquatable<Rgba64>, IPixel
    {
        VectorComponentInfo IPackedVector.ComponentInfo => new VectorComponentInfo(
            new VectorComponent(VectorComponentType.Red, sizeof(ushort) * 8),
            new VectorComponent(VectorComponentType.Green, sizeof(ushort) * 8),
            new VectorComponent(VectorComponentType.Blue, sizeof(ushort) * 8),
            new VectorComponent(VectorComponentType.Alpha, sizeof(ushort) * 8));

        [CLSCompliant(false)]
        public ushort R;

        [CLSCompliant(false)]
        public ushort G;

        [CLSCompliant(false)]
        public ushort B;

        [CLSCompliant(false)]
        public ushort A;

        /// <summary>
        /// Gets or sets the RGB components of this struct as <see cref="Rgb48"/>
        /// </summary>
        public Rgb48 Rgb
        {
            readonly get => UnsafeUtils.As<Rgba64, Rgb48>(this);
            set
            {
                R = value.R;
                G = value.G;
                B = value.B;
            }
        }

        #region Constructors

        [CLSCompliant(false)]
        public Rgba64(ushort r, ushort g, ushort b, ushort a)
        {
            R = r;
            G = g;
            B = b;
            A = a;
        }

        public Rgba64(Vector4 vector) : this()
        {
            FromVector4(vector);
        }

        public Rgba64(float x, float y, float z, float w) : this(new Vector4(x, y, z, w))
        {
        }

        #endregion

        #region IPackedVector

        [CLSCompliant(false)]
        public ulong PackedValue
        {
            readonly get => UnsafeUtils.As<Rgba64, ulong>(this);
            set => Unsafe.As<Rgba64, ulong>(ref this) = value;
        }

        public void FromVector4(in Vector4 vector) => FromScaledVector4(vector);

        public readonly void ToVector4(out Vector4 vector) => ToScaledVector4(out vector);

        public void FromScaledVector4(in Vector4 scaledVector)
        {
            Vector4.Multiply(scaledVector, ushort.MaxValue, out var v);
            v += Vector4.Half;
            v.Clamp(0, ushort.MaxValue);

            R = (ushort)v.X;
            G = (ushort)v.Y;
            B = (ushort)v.Z;
            A = (ushort)v.W;
        }

        public readonly void ToScaledVector4(out Vector4 scaledVector)
        {
            scaledVector.X = R / (float)ushort.MaxValue;
            scaledVector.Y = G / (float)ushort.MaxValue;
            scaledVector.Z = B / (float)ushort.MaxValue;
            scaledVector.W = A / (float)ushort.MaxValue;
        }

        #endregion

        #region IPixel

        public readonly void ToColor(ref Color destination)
        {
            destination.R = PackedVectorHelper.DownScale16To8Bit(R);
            destination.G = PackedVectorHelper.DownScale16To8Bit(G);
            destination.B = PackedVectorHelper.DownScale16To8Bit(B);
            destination.A = PackedVectorHelper.DownScale16To8Bit(A);
        }

        public void FromGray8(Gray8 source)
        {
            R = G = B = PackedVectorHelper.UpScale8To16Bit(source.L);
            A = byte.MaxValue;
        }

        public void FromGray16(Gray16 source)
        {
            R = G = B = source.L;
            A = byte.MaxValue;
        }

        public void FromGrayAlpha16(GrayAlpha16 source)
        {
            R = G = B = PackedVectorHelper.UpScale8To16Bit(source.L);
            A = source.A;
        }

        public void FromRgb24(Rgb24 source)
        {
            R = PackedVectorHelper.UpScale8To16Bit(source.R);
            G = PackedVectorHelper.UpScale8To16Bit(source.G);
            B = PackedVectorHelper.UpScale8To16Bit(source.B);
            A = byte.MaxValue;
        }

        public void FromColor(Color source)
        {
            R = PackedVectorHelper.UpScale8To16Bit(source.R);
            G = PackedVectorHelper.UpScale8To16Bit(source.G);
            B = PackedVectorHelper.UpScale8To16Bit(source.B);
            A = PackedVectorHelper.UpScale8To16Bit(source.A);
        }

        public void FromRgb48(Rgb48 source)
        {
            R = source.R;
            G = source.G;
            B = source.B;
            A = byte.MaxValue;
        }

        public void FromRgba64(Rgba64 source) => this = source;

        #endregion

        #region Equals

        public override bool Equals(object obj) => obj is Rgba64 other && Equals(other);
        public bool Equals(Rgba64 other) => this == other;

        public static bool operator ==(in Rgba64 a, in Rgba64 b) =>
            a.R == b.R && a.G == b.G && a.B == b.B && a.A == b.A;

        public static bool operator !=(in Rgba64 a, in Rgba64 b) => !(a == b);

        #endregion

        #region Object Overrides

        /// <summary>
        /// Gets a <see cref="string"/> representation of the packed vector.
        /// </summary>
        public override string ToString() => nameof(Rgba64) + $"(R:{R}, G:{G}, B:{B}, A:{A})";

        /// <summary>
        /// Gets a hash code of the packed vector.
        /// </summary>
        public override int GetHashCode() => PackedValue.GetHashCode();

        #endregion
    }
}