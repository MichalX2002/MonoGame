// Copyright (c) Six Labors and contributors.
// Licensed under the Apache License, Version 2.0.

using MonoGame.Framework;
using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace MonoGame.Utilities.PackedVector
{
    /// <summary>
    /// Packed pixel type containing four 8-bit unsigned normalized values ranging from 0 to 255.
    /// The color components are stored in blue, green, red, and alpha order (least significant to most significant byte).
    /// The format is binary compatible with System.Drawing.Imaging.PixelFormat.Format32bppArgb
    /// <para>
    /// Ranges from [0, 0, 0, 0] to [1, 1, 1, 1] in vector form.
    /// </para>
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public partial struct Bgra32 : IPixel, IPackedVector<uint>
    {
        /// <summary>
        /// Gets or sets the blue component.
        /// </summary>
        public byte B;

        /// <summary>
        /// Gets or sets the green component.
        /// </summary>
        public byte G;

        /// <summary>
        /// Gets or sets the red component.
        /// </summary>
        public byte R;

        /// <summary>
        /// Gets or sets the alpha component.
        /// </summary>
        public byte A;

        /// <summary>
        /// Initializes a new instance of the <see cref="Bgra32"/> struct.
        /// </summary>
        /// <param name="r">The red component.</param>
        /// <param name="g">The green component.</param>
        /// <param name="b">The blue component.</param>
        public Bgra32(byte r, byte g, byte b)
        {
            R = r;
            G = g;
            B = b;
            A = byte.MaxValue;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Bgra32"/> struct.
        /// </summary>
        /// <param name="r">The red component.</param>
        /// <param name="g">The green component.</param>
        /// <param name="b">The blue component.</param>
        /// <param name="a">The alpha component.</param>
        public Bgra32(byte r, byte g, byte b, byte a)
        {
            R = r;
            G = g;
            B = b;
            A = a;
        }

        /// <inheritdoc/>
        [CLSCompliant(false)]
        public uint PackedValue
        {
            get => Unsafe.As<Bgra32, uint>(ref this);
            set => Unsafe.As<Bgra32, uint>(ref this) = value;
        }

        /// <summary>
        /// Converts an <see cref="Bgra32"/> to <see cref="Color"/>.
        /// </summary>
        /// <param name="source">The <see cref="Bgra32"/>.</param>
        /// <returns>The <see cref="Color"/>.</returns>
        public static implicit operator Color(Bgra32 source) =>
            new Color(source.R, source.G, source.B, source.A);

        /// <summary>
        /// Converts a <see cref="Color"/> to <see cref="Bgra32"/>.
        /// </summary>
        /// <param name="color">The <see cref="Color"/>.</param>
        /// <returns>The <see cref="Bgra32"/>.</returns>
        public static implicit operator Bgra32(Color color) => color.ToBgra32();

        /// <summary>
        /// Compares two <see cref="Bgra32"/> objects for equality.
        /// </summary>
        /// <param name="left">The <see cref="Bgra32"/> on the left side of the operand.</param>
        /// <param name="right">The <see cref="Bgra32"/> on the right side of the operand.</param>
        /// <returns>
        /// True if the <paramref name="left"/> parameter is equal to the <paramref name="right"/> parameter; otherwise, false.
        /// </returns>
        public static bool operator ==(Bgra32 left, Bgra32 right) => left.Equals(right);

        /// <summary>
        /// Compares two <see cref="Bgra32"/> objects for equality.
        /// </summary>
        /// <param name="left">The <see cref="Bgra32"/> on the left side of the operand.</param>
        /// <param name="right">The <see cref="Bgra32"/> on the right side of the operand.</param>
        /// <returns>
        /// True if the <paramref name="left"/> parameter is not equal to the <paramref name="right"/> parameter; otherwise, false.
        /// </returns>
        public static bool operator !=(Bgra32 left, Bgra32 right) => !left.Equals(right);

        /// <inheritdoc/>

        /// <inheritdoc/>
        public void FromScaledVector4(Vector4 vector) => FromVector4(vector);

        /// <inheritdoc/>
        public Vector4 ToScaledVector4() => ToVector4();

        /// <inheritdoc/>
        public void FromVector4(Vector4 vector) => Pack(ref vector);

        /// <inheritdoc/>
        public Vector4 ToVector4() => new Vector4(R, G, B, A) / PackedVectorHelper.MaxBytes;

        /// <inheritdoc/>
        public void FromArgb32(Argb32 source)
        {
            R = source.R;
            G = source.G;
            B = source.B;
            A = source.A;
        }

        /// <inheritdoc/>
        public void FromBgr24(Bgr24 source)
        {
            R = source.R;
            G = source.G;
            B = source.B;
            A = byte.MaxValue;
        }

        /// <inheritdoc/>
        public void FromBgra5551(Bgra5551 source) => FromScaledVector4(source.ToScaledVector4());

        /// <inheritdoc/>
        public void FromBgra32(Bgra32 source) => this = source;

        /// <inheritdoc/>
        public void FromGray8(Gray8 source)
        {
            R = source.PackedValue;
            G = source.PackedValue;
            B = source.PackedValue;
            A = byte.MaxValue;
        }

        /// <inheritdoc/>
        public void FromGray16(Gray16 source)
        {
            byte rgb = PackedVectorHelper.DownScale16To8Bit(source.PackedValue);
            R = rgb;
            G = rgb;
            B = rgb;
            A = byte.MaxValue;
        }

        /// <inheritdoc/>
        public void FromColor(Color source)
        {
            R = source.R;
            G = source.G;
            B = source.B;
            A = source.A;
        }

        /// <inheritdoc/>
        public void FromRgb24(Rgb24 source)
        {
            R = source.R;
            G = source.G;
            B = source.B;
            A = byte.MaxValue;
        }

        /// <inheritdoc />
        public void ToColor(ref Color dest)
        {
            dest.R = R;
            dest.G = G;
            dest.B = B;
            dest.A = A;
        }

        /// <inheritdoc/>
        public void FromRgb48(Rgb48 source)
        {
            R = PackedVectorHelper.DownScale16To8Bit(source.R);
            G = PackedVectorHelper.DownScale16To8Bit(source.G);
            B = PackedVectorHelper.DownScale16To8Bit(source.B);
            A = byte.MaxValue;
        }

        /// <inheritdoc/>
        public void FromRgba64(Rgba64 source)
        {
            R = PackedVectorHelper.DownScale16To8Bit(source.R);
            G = PackedVectorHelper.DownScale16To8Bit(source.G);
            B = PackedVectorHelper.DownScale16To8Bit(source.B);
            A = PackedVectorHelper.DownScale16To8Bit(source.A);
        }

        /// <inheritdoc/>
        public override bool Equals(object obj) => obj is Bgra32 other && Equals(other);

        /// <inheritdoc/>
        public bool Equals(Bgra32 other) => PackedValue.Equals(other.PackedValue);

        /// <inheritdoc/>
        public override int GetHashCode() => PackedValue.GetHashCode();

        /// <inheritdoc />
        public override string ToString() => $"Bgra32({B}, {G}, {R}, {A})";

        /// <summary>
        /// Packs a <see cref="Vector4"/> into a color.
        /// </summary>
        /// <param name="vector">The vector containing the values to pack.</param>
        private void Pack(ref Vector4 vector)
        {
            vector *= PackedVectorHelper.MaxBytes;
            vector += PackedVectorHelper.Half;
            vector = Vector4.Clamp(vector, Vector4.Zero, PackedVectorHelper.MaxBytes);

            R = (byte)vector.X;
            G = (byte)vector.Y;
            B = (byte)vector.Z;
            A = (byte)vector.W;
        }
    }
}
