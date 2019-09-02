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
    /// The color components are stored in alpha, red, green, and blue order (least significant to most significant byte).
    /// <para>
    /// Ranges from [0, 0, 0, 0] to [1, 1, 1, 1] in vector form.
    /// </para>
    /// </summary>
    /// <remarks>
    /// This struct is fully mutable. This is done (against the guidelines) for the sake of performance,
    /// as it avoids the need to create new values for modification operations.
    /// </remarks>
    [StructLayout(LayoutKind.Sequential)]
    public partial struct Argb32 : IPixel, IPackedVector<uint>
    {
        /// <summary>
        /// Gets or sets the alpha component.
        /// </summary>
        public byte A;

        /// <summary>
        /// Gets or sets the red component.
        /// </summary>
        public byte R;

        /// <summary>
        /// Gets or sets the green component.
        /// </summary>
        public byte G;

        /// <summary>
        /// Gets or sets the blue component.
        /// </summary>
        public byte B;

        /// <summary>
        /// Initializes a new instance of the <see cref="Argb32"/> struct.
        /// </summary>
        /// <param name="r">The red component.</param>
        /// <param name="g">The green component.</param>
        /// <param name="b">The blue component.</param>
        public Argb32(byte r, byte g, byte b)
        {
            R = r;
            G = g;
            B = b;
            A = byte.MaxValue;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Argb32"/> struct.
        /// </summary>
        /// <param name="r">The red component.</param>
        /// <param name="g">The green component.</param>
        /// <param name="b">The blue component.</param>
        /// <param name="a">The alpha component.</param>
        public Argb32(byte r, byte g, byte b, byte a)
        {
            R = r;
            G = g;
            B = b;
            A = a;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Argb32"/> struct.
        /// </summary>
        /// <param name="r">The red component.</param>
        /// <param name="g">The green component.</param>
        /// <param name="b">The blue component.</param>
        /// <param name="a">The alpha component.</param>
        public Argb32(float r, float g, float b, float a = 1) : this() => Pack(r, g, b, a);

        /// <summary>
        /// Initializes a new instance of the <see cref="Argb32"/> struct.
        /// </summary>
        /// <param name="vector">
        /// The vector containing the components for the packed vector.
        /// </param>
        public Argb32(Vector3 vector) : this() => Pack(ref vector);

        /// <summary>
        /// Initializes a new instance of the <see cref="Argb32"/> struct.
        /// </summary>
        /// <param name="vector">
        /// The vector containing the components for the packed vector.
        /// </param>
        public Argb32(Vector4 vector) : this() => Pack(ref vector);

        /// <summary>
        /// Initializes a new instance of the <see cref="Argb32"/> struct.
        /// </summary>
        /// <param name="packed">
        /// The packed value.
        /// </param>
        [CLSCompliant(false)]
        public Argb32(uint packed) : this() => PackedValue = packed;

        /// <inheritdoc/>
        [CLSCompliant(false)]
        public uint PackedValue
        {
            get => Unsafe.As<Argb32, uint>(ref this);
            set => Unsafe.As<Argb32, uint>(ref this) = value;
        }

        /// <summary>
        /// Converts an <see cref="Argb32"/> to <see cref="Color"/>.
        /// </summary>
        /// <param name="source">The <see cref="Argb32"/>.</param>
        /// <returns>The <see cref="Color"/>.</returns>
        public static implicit operator Color(Argb32 source) =>
            new Color(source.R, source.G, source.B, source.A);

        /// <summary>
        /// Converts a <see cref="Color"/> to <see cref="Argb32"/>.
        /// </summary>
        /// <param name="color">The <see cref="Color"/>.</param>
        /// <returns>The <see cref="Argb32"/>.</returns>
        public static implicit operator Argb32(Color color) => color.ToArgb32();

        /// <summary>
        /// Compares two <see cref="Argb32"/> objects for equality.
        /// </summary>
        /// <param name="left">The <see cref="Argb32"/> on the left side of the operand.</param>
        /// <param name="right">The <see cref="Argb32"/> on the right side of the operand.</param>
        /// <returns>
        /// True if the <paramref name="left"/> parameter is equal to the <paramref name="right"/> parameter; otherwise, false.
        /// </returns>
        public static bool operator ==(Argb32 left, Argb32 right) => left.Equals(right);

        /// <summary>
        /// Compares two <see cref="Argb32"/> objects for equality.
        /// </summary>
        /// <param name="left">The <see cref="Argb32"/> on the left side of the operand.</param>
        /// <param name="right">The <see cref="Argb32"/> on the right side of the operand.</param>
        /// <returns>
        /// True if the <paramref name="left"/> parameter is not equal to the <paramref name="right"/> parameter; otherwise, false.
        /// </returns>
        public static bool operator !=(Argb32 left, Argb32 right) => !left.Equals(right);

        /// <inheritdoc/>
        public void FromScaledVector4(Vector4 vector) => FromVector4(vector);

        /// <inheritdoc/>
        public Vector4 ToScaledVector4() => ToVector4();

        /// <inheritdoc/>
        public void FromVector4(Vector4 vector) => Pack(ref vector);

        /// <inheritdoc/>
        public Vector4 ToVector4() => new Vector4(R, G, B, A) / PackedVectorHelper.MaxBytes;

        /// <inheritdoc/>
        public void FromArgb32(Argb32 source) => PackedValue = source.PackedValue;

        /// <inheritdoc/>
        public void FromBgra5551(Bgra5551 source) => FromScaledVector4(source.ToScaledVector4());

        /// <inheritdoc/>
        public void FromBgr24(Bgr24 source)
        {
            R = source.R;
            G = source.G;
            B = source.B;
            A = byte.MaxValue;
        }

        /// <inheritdoc/>
        public void FromBgra32(Bgra32 source)
        {
            R = source.R;
            G = source.G;
            B = source.B;
            A = source.A;
        }

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
        public void FromRgb24(Rgb24 source)
        {
            R = source.R;
            G = source.G;
            B = source.B;
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
        public override bool Equals(object obj) => obj is Argb32 argb32 && Equals(argb32);

        /// <inheritdoc/>
        public bool Equals(Argb32 other) => PackedValue == other.PackedValue;

        /// <summary>
        /// Gets a string representation of the packed vector.
        /// </summary>
        /// <returns>A string representation of the packed vector.</returns>
        public override string ToString() => $"Argb({A}, {R}, {G}, {B})";

        /// <inheritdoc/>
        public override int GetHashCode() => PackedValue.GetHashCode();

        /// <summary>
        /// Packs the four floats into a color.
        /// </summary>
        /// <param name="x">The x-component</param>
        /// <param name="y">The y-component</param>
        /// <param name="z">The z-component</param>
        /// <param name="w">The w-component</param>
        private void Pack(float x, float y, float z, float w)
        {
            var value = new Vector4(x, y, z, w);
            Pack(ref value);
        }

        /// <summary>
        /// Packs a <see cref="Vector3"/> into a uint.
        /// </summary>
        /// <param name="vector">The vector containing the values to pack.</param>
        private void Pack(ref Vector3 vector)
        {
            var value = new Vector4(vector, 1);
            Pack(ref value);
        }

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
