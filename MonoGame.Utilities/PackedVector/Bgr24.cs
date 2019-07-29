// Copyright (c) Six Labors and contributors.
// Licensed under the Apache License, Version 2.0.

using System;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Microsoft.Xna.Framework.Graphics.PackedVector
{
    /// <summary>
    /// Pixel type containing three 8-bit unsigned normalized values ranging from 0 to 255.
    /// The color components are stored in blue, green, red order (least significant to most significant byte).
    /// <para>
    /// Ranges from [0, 0, 0, 1] to [1, 1, 1, 1] in vector form.
    /// </para>
    /// </summary>
    [StructLayout(LayoutKind.Explicit)]
    public partial struct Bgr24 : IPixel
    {
        /// <summary>
        /// The blue component.
        /// </summary>
        [FieldOffset(0)]
        public byte B;

        /// <summary>
        /// The green component.
        /// </summary>
        [FieldOffset(1)]
        public byte G;

        /// <summary>
        /// The red component.
        /// </summary>
        [FieldOffset(2)]
        public byte R;

        /// <summary>
        /// Initializes a new instance of the <see cref="Bgr24"/> struct.
        /// </summary>
        /// <param name="r">The red component.</param>
        /// <param name="g">The green component.</param>
        /// <param name="b">The blue component.</param>
        public Bgr24(byte r, byte g, byte b)
        {
            R = r;
            G = g;
            B = b;
        }

        /// <summary>
        /// Converts an <see cref="Bgr24"/> to <see cref="Color"/>.
        /// </summary>
        /// <param name="source">The <see cref="Bgr24"/>.</param>
        /// <returns>The <see cref="Color"/>.</returns>
        public static implicit operator Color(Bgr24 source) =>
            new Color(source.R, source.G, source.B, byte.MaxValue);

        /// <summary>
        /// Converts a <see cref="Color"/> to <see cref="Bgr24"/>.
        /// </summary>
        /// <param name="color">The <see cref="Color"/>.</param>
        /// <returns>The <see cref="Bgr24"/>.</returns>
        public static implicit operator Bgr24(Color color) => color.ToBgr24();

        /// <summary>
        /// Compares two <see cref="Bgr24"/> objects for equality.
        /// </summary>
        /// <param name="left">The <see cref="Bgr24"/> on the left side of the operand.</param>
        /// <param name="right">The <see cref="Bgr24"/> on the right side of the operand.</param>
        /// <returns>
        /// True if the <paramref name="left"/> parameter is equal to the <paramref name="right"/> parameter; otherwise, false.
        /// </returns>
        public static bool operator ==(Bgr24 left, Bgr24 right) => left.Equals(right);

        /// <summary>
        /// Compares two <see cref="Bgr24"/> objects for equality.
        /// </summary>
        /// <param name="left">The <see cref="Bgr24"/> on the left side of the operand.</param>
        /// <param name="right">The <see cref="Bgr24"/> on the right side of the operand.</param>
        /// <returns>
        /// True if the <paramref name="left"/> parameter is not equal to the <paramref name="right"/> parameter; otherwise, false.
        /// </returns>
        public static bool operator !=(Bgr24 left, Bgr24 right) => !left.Equals(right);

        /// <inheritdoc/>

        /// <inheritdoc/>
        public void FromScaledVector4(Vector4 vector) => FromVector4(vector);

        /// <inheritdoc/>
        public Vector4 ToScaledVector4() => ToVector4();

        /// <inheritdoc/>
        public void FromVector4(Vector4 vector)
        {
            Color rgba = default;
            rgba.FromVector4(vector);
            FromColor(rgba);
        }

        /// <inheritdoc/>
        public Vector4 ToVector4() => new Color(R, G, B, byte.MaxValue).ToVector4();

        /// <inheritdoc/>
        public void FromArgb32(Argb32 source)
        {
            R = source.R;
            G = source.G;
            B = source.B;
        }

        /// <inheritdoc/>
        public void FromBgr24(Bgr24 source) => this = source;

        /// <inheritdoc/>
        public void FromBgra5551(Bgra5551 source) => FromScaledVector4(source.ToScaledVector4());

        /// <inheritdoc/>
        public void FromBgra32(Bgra32 source)
        {
            R = source.R;
            G = source.G;
            B = source.B;
        }

        /// <inheritdoc/>
        public void FromGray8(Gray8 source)
        {
            R = source.PackedValue;
            G = source.PackedValue;
            B = source.PackedValue;
        }

        /// <inheritdoc/>
        public void FromGray16(Gray16 source)
        {
            byte rgb = VectorMaths.DownScale16To8Bit(source.PackedValue);
            R = rgb;
            G = rgb;
            B = rgb;
        }

        /// <inheritdoc/>
        public void FromRgb24(Rgb24 source)
        {
            R = source.R;
            G = source.G;
            B = source.B;
        }

        /// <inheritdoc/>
        public void FromColor(Color source) => this = source.Bgr;

        /// <inheritdoc />
        public void ToColor(ref Color dest)
        {
            dest.R = R;
            dest.G = G;
            dest.B = B;
            dest.A = byte.MaxValue;
        }

        /// <inheritdoc/>
        public void FromRgb48(Rgb48 source)
        {
            R = VectorMaths.DownScale16To8Bit(source.R);
            G = VectorMaths.DownScale16To8Bit(source.G);
            B = VectorMaths.DownScale16To8Bit(source.B);
        }

        /// <inheritdoc/>
        public void FromRgba64(Rgba64 source)
        {
            R = VectorMaths.DownScale16To8Bit(source.R);
            G = VectorMaths.DownScale16To8Bit(source.G);
            B = VectorMaths.DownScale16To8Bit(source.B);
        }

        /// <inheritdoc/>
        public bool Equals(Bgr24 other) => R.Equals(other.R) && G.Equals(other.G) && B.Equals(other.B);

        /// <inheritdoc/>
        public override bool Equals(object obj) => obj is Bgr24 other && Equals(other);

        /// <inheritdoc />
        public override string ToString() => $"Bgra({B}, {G}, {R})";

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 17;
                hash = hash * 23 + B.GetHashCode();
                hash = hash * 23 + G.GetHashCode();
                hash = hash * 23 + R.GetHashCode();
                return hash;
            }
        }
    }
}
