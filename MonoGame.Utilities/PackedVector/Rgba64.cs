// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace MonoGame.Framework.PackedVector
{
    /// <summary>
    /// Packed pixel type containing four unsigned 16-bit XYZW components.
    /// <para>
    /// Ranges from [0, 0, 0, 0] to [1, 1, 1, 1] in vector form.
    /// </para>
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct Rgba64 : IPackedVector<ulong>, IEquatable<Rgba64>, IPixel
    {
        private static readonly Vector4 Max = new Vector4(ushort.MaxValue);

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
            readonly get => new Rgb48(R, G, B);
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

        public Rgba64(Vector4 vector) => this = Pack(ref vector);

        public Rgba64(float x, float y, float z, float w) : this(new Vector4(x, y, z, w))
        {
        }

        #endregion

        private static Rgba64 Pack(ref Vector4 vector)
        {
            vector *= ushort.MaxValue;
            vector = Vector4.Clamp(vector, Vector4.Zero, Max);
            vector.Round();

            return new Rgba64(
                (ushort)vector.X,
                (ushort)vector.Y,
                (ushort)vector.Z,
                (ushort)vector.W);
        }

        #region IPackedVector

        /// <inheritdoc/>
        [CLSCompliant(false)]
        public ulong PackedValue
        {
            get => Unsafe.As<Rgba64, ulong>(ref this);
            set => Unsafe.As<Rgba64, ulong>(ref this) = value;
        }

        /// <inheritdoc/>
        public void FromVector4(Vector4 vector) => this = Pack(ref vector);

        /// <inheritdoc/>
        public readonly Vector4 ToVector4() => new Vector4(R, G, B, A) / ushort.MaxValue;

        #endregion

        #region IPixel

        /// <inheritdoc/>
        public void FromScaledVector4(Vector4 vector) => FromVector4(vector);

        /// <inheritdoc/>
        public readonly Vector4 ToScaledVector4() => ToVector4();

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
        public override string ToString() => $"Rgba64(R:{R}, G:{G}, B:{B}, A:{A})";

        /// <summary>
        /// Gets a hash code of the packed vector.
        /// </summary>
        public override int GetHashCode() => PackedValue.GetHashCode();

        #endregion
    }
}