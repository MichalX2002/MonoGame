// Copyright (c) Six Labors and contributors.
// Licensed under the Apache License, Version 2.0.

using System;
using System.Runtime.InteropServices;
using MonoGame.Framework;

namespace MonoGame.Utilities.PackedVector
{
    /// <summary>
    /// Packed pixel type containing three 16-bit unsigned normalized values ranging from 0 to 635535.
    /// <para>
    /// Ranges from [0, 0, 0, 1] to [1, 1, 1, 1] in vector form.
    /// </para>
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct Rgb48 : IPixel
    {
        private const float Max = ushort.MaxValue;

        /// <summary>
        /// Gets or sets the red component.
        /// </summary>
        public ushort R;

        /// <summary>
        /// Gets or sets the green component.
        /// </summary>
        public ushort G;

        /// <summary>
        /// Gets or sets the blue component.
        /// </summary>
        public ushort B;

        /// <summary>
        /// Initializes a new instance of the <see cref="Rgb48"/> struct.
        /// </summary>
        /// <param name="r">The red component.</param>
        /// <param name="g">The green component.</param>
        /// <param name="b">The blue component.</param>
        public Rgb48(ushort r, ushort g, ushort b)
            : this()
        {
            R = r;
            G = g;
            B = b;
        }

        /// <summary>
        /// Compares two <see cref="Rgb48"/> objects for equality.
        /// </summary>
        /// <param name="left">The <see cref="Rgb48"/> on the left side of the operand.</param>
        /// <param name="right">The <see cref="Rgb48"/> on the right side of the operand.</param>
        /// <returns>
        /// True if the <paramref name="left"/> parameter is equal to the <paramref name="right"/> parameter; otherwise, false.
        /// </returns>
        public static bool operator ==(Rgb48 left, Rgb48 right) => left.Equals(right);

        /// <summary>
        /// Compares two <see cref="Rgb48"/> objects for equality.
        /// </summary>
        /// <param name="left">The <see cref="Rgb48"/> on the left side of the operand.</param>
        /// <param name="right">The <see cref="Rgb48"/> on the right side of the operand.</param>
        /// <returns>
        /// True if the <paramref name="left"/> parameter is not equal to the <paramref name="right"/> parameter; otherwise, false.
        /// </returns>
        public static bool operator !=(Rgb48 left, Rgb48 right) => !left.Equals(right);

        #region IPixel Implementation

        /// <inheritdoc/>
        public void FromScaledVector4(Vector4 vector) => FromVector4(vector);

        /// <inheritdoc/>
        public Vector4 ToScaledVector4() => ToVector4();

        /// <inheritdoc />
        public void FromVector4(Vector4 vector)
        {
            vector = Vector4.Clamp(vector, Vector4.Zero, Vector4.One) * Max;
            R = (ushort)Math.Round(vector.X);
            G = (ushort)Math.Round(vector.Y);
            B = (ushort)Math.Round(vector.Z);
        }

        /// <inheritdoc />
        public Vector4 ToVector4() => new Vector4(R / Max, G / Max, B / Max, 1F);

        /// <inheritdoc />
        public void FromArgb32(Argb32 source)
        {
            R = VectorMaths.UpScale8To16Bit(source.R);
            G = VectorMaths.UpScale8To16Bit(source.G);
            B = VectorMaths.UpScale8To16Bit(source.B);
        }

        /// <inheritdoc />
        public void FromBgr24(Bgr24 source)
        {
            R = VectorMaths.UpScale8To16Bit(source.R);
            G = VectorMaths.UpScale8To16Bit(source.G);
            B = VectorMaths.UpScale8To16Bit(source.B);
        }

        /// <inheritdoc />
        public void FromBgra32(Bgra32 source)
        {
            R = VectorMaths.UpScale8To16Bit(source.R);
            G = VectorMaths.UpScale8To16Bit(source.G);
            B = VectorMaths.UpScale8To16Bit(source.B);
        }

        /// <inheritdoc/>
        public void FromRgba64(Rgba64 source) => this = source.Rgb;

        /// <inheritdoc />
        public void FromBgra5551(Bgra5551 source) => FromScaledVector4(source.ToScaledVector4());

        /// <inheritdoc/>
        public void FromGray8(Gray8 source)
        {
            ushort rgb = VectorMaths.UpScale8To16Bit(source.PackedValue);
            R = rgb;
            G = rgb;
            B = rgb;
        }

        /// <inheritdoc/>
        public void FromGray16(Gray16 source)
        {
            R = source.PackedValue;
            G = source.PackedValue;
            B = source.PackedValue;
        }

        /// <inheritdoc />
        public void FromRgb24(Rgb24 source)
        {
            R = VectorMaths.UpScale8To16Bit(source.R);
            G = VectorMaths.UpScale8To16Bit(source.G);
            B = VectorMaths.UpScale8To16Bit(source.B);
        }

        /// <inheritdoc />
        public void FromColor(Color source)
        {
            R = VectorMaths.UpScale8To16Bit(source.R);
            G = VectorMaths.UpScale8To16Bit(source.G);
            B = VectorMaths.UpScale8To16Bit(source.B);
        }

        /// <inheritdoc/>
        public void FromRgb48(Rgb48 source) => this = source;

        /// <inheritdoc />
        public void ToColor(ref Color dest)
        {
            dest.R = VectorMaths.DownScale16To8Bit(R);
            dest.G = VectorMaths.DownScale16To8Bit(G);
            dest.B = VectorMaths.DownScale16To8Bit(B);
            dest.A = byte.MaxValue;
        }

        #endregion

        /// <inheritdoc />
        public override bool Equals(object obj) => obj is Rgb48 rgb48 && Equals(rgb48);

        /// <inheritdoc />
        public bool Equals(Rgb48 other) => R.Equals(other.R) && G.Equals(other.G) && B.Equals(other.B);

        /// <inheritdoc />
        public override string ToString() => $"Rgb48({R}, {G}, {B})";

        /// <inheritdoc />
        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 17;
                hash = hash * 23 + R.GetHashCode();
                hash = hash * 23 + G.GetHashCode();
                hash = hash * 23 + B.GetHashCode();
                return hash;
            }
        }
    }
}
