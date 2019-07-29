// Copyright (c) Six Labors and contributors.
// Licensed under the Apache License, Version 2.0.

using System.Runtime.InteropServices;
using MonoGame.Framework;

namespace MonoGame.Utilities.PackedVector
{
    /// <summary>
    /// Pixel type containing three 8-bit unsigned normalized values ranging from 0 to 255.
    /// The color components are stored in red, green, blue order (least significant to most significant byte).
    /// <para>
    /// Ranges from [0, 0, 0, 1] to [1, 1, 1, 1] in vector form.
    /// </para>
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public partial struct Rgb24 : IPixel
    {
        /// <summary>
        /// The red component.
        /// </summary>
        public byte R;

        /// <summary>
        /// The green component.
        /// </summary>
        public byte G;

        /// <summary>
        /// The blue component.
        /// </summary>
        public byte B;

        /// <summary>
        /// Initializes a new instance of the <see cref="Rgb24"/> struct.
        /// </summary>
        /// <param name="r">The red component.</param>
        /// <param name="g">The green component.</param>
        /// <param name="b">The blue component.</param>
        public Rgb24(byte r, byte g, byte b)
        {
            R = r;
            G = g;
            B = b;
        }

        /// <summary>
        /// Converts an <see cref="Rgb24"/> to <see cref="Color"/>.
        /// </summary>
        /// <param name="source">The <see cref="Rgb24"/>.</param>
        /// <returns>The <see cref="Color"/>.</returns>
        public static implicit operator Color(Rgb24 source) =>
            new Color(source.R, source.G, source.B, byte.MaxValue);

        /// <summary>
        /// Converts a <see cref="Color"/> to <see cref="Rgb24"/>.
        /// </summary>
        /// <param name="color">The <see cref="Color"/>.</param>
        /// <returns>The <see cref="Rgb24"/>.</returns>
        public static implicit operator Rgb24(Color color) => color.ToRgb24();

        /// <summary>
        /// Compares two <see cref="Rgb24"/> objects for equality.
        /// </summary>
        /// <param name="left">The <see cref="Rgb24"/> on the left side of the operand.</param>
        /// <param name="right">The <see cref="Rgb24"/> on the right side of the operand.</param>
        /// <returns>
        /// True if the <paramref name="left"/> parameter is equal to the <paramref name="right"/> parameter; otherwise, false.
        /// </returns>
        public static bool operator ==(Rgb24 left, Rgb24 right) => left.Equals(right);

        /// <summary>
        /// Compares two <see cref="Rgb24"/> objects for equality.
        /// </summary>
        /// <param name="left">The <see cref="Rgb24"/> on the left side of the operand.</param>
        /// <param name="right">The <see cref="Rgb24"/> on the right side of the operand.</param>
        /// <returns>
        /// True if the <paramref name="left"/> parameter is not equal to the <paramref name="right"/> parameter; otherwise, false.
        /// </returns>
        public static bool operator !=(Rgb24 left, Rgb24 right) => !left.Equals(right);

        /// <inheritdoc/>

        /// <inheritdoc/>
        public void FromScaledVector4(Vector4 vector) => FromVector4(vector);

        /// <inheritdoc/>
        public Vector4 ToScaledVector4() => ToVector4();

        /// <inheritdoc/>
        public void FromVector4(Vector4 vector) => Pack(ref vector);

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
        public void FromBgr24(Bgr24 source)
        {
            R = source.R;
            G = source.G;
            B = source.B;
        }

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

        /// <inheritdoc />
        public void FromBgra5551(Bgra5551 source) => FromScaledVector4(source.ToScaledVector4());

        /// <inheritdoc/>
        public void FromRgb24(Rgb24 source) => this = source;

        /// <inheritdoc/>
        public void FromColor(Color source) => this = source.Rgb;

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
        public override bool Equals(object obj) => obj is Rgb24 other && Equals(other);

        /// <inheritdoc/>
        public bool Equals(Rgb24 other) => R.Equals(other.R) && G.Equals(other.G) && B.Equals(other.B);
        
        /// <inheritdoc/>
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

        /// <inheritdoc/>
        public override string ToString() => $"Rgb24({R}, {G}, {B})";

        /// <summary>
        /// Packs a <see cref="Vector4"/> into a color.
        /// </summary>
        /// <param name="vector">The vector containing the values to pack.</param>
        private void Pack(ref Vector4 vector)
        {
            vector *= VectorMaths.MaxBytes;
            vector += VectorMaths.Half;
            vector = Vector4.Clamp(vector, Vector4.Zero, VectorMaths.MaxBytes);

            R = (byte)vector.X;
            G = (byte)vector.Y;
            B = (byte)vector.Z;
        }
    }
}
