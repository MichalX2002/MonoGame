// Copyright (c) Six Labors and contributors.
// Licensed under the Apache License, Version 2.0.

using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Microsoft.Xna.Framework.Graphics.PackedVector
{
    /// <summary>
    /// Packed pixel type containing four 16-bit unsigned normalized values ranging from 0 to 65535.
    /// <para>
    /// Ranges from [0, 0, 0, 0] to [1, 1, 1, 1] in vector form.
    /// </para>
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public partial struct Rgba64 : IPixel, IPackedVector<ulong>
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
        /// Gets or sets the alpha component.
        /// </summary>
        public ushort A;

        #region Public Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Rgba64"/> struct.
        /// </summary>
        /// <param name="r">The red component.</param>
        /// <param name="g">The green component.</param>
        /// <param name="b">The blue component.</param>
        /// <param name="a">The alpha component.</param>
        public Rgba64(ushort r, ushort g, ushort b, ushort a)
        {
            R = r;
            G = g;
            B = b;
            A = a;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Rgba64"/> struct.
        /// </summary>
        /// <param name="color">A structure of 4 bytes in RGBA byte order.</param>
        public Rgba64(Color color)
        {
            R = VectorMaths.UpScale8To16Bit(color.R);
            G = VectorMaths.UpScale8To16Bit(color.G);
            B = VectorMaths.UpScale8To16Bit(color.B);
            A = VectorMaths.UpScale8To16Bit(color.A);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Rgba64"/> struct.
        /// </summary>
        /// <param name="color">A structure of 4 bytes in ARGB byte order.</param>
        public Rgba64(Argb32 color)
        {
            R = VectorMaths.UpScale8To16Bit(color.R);
            G = VectorMaths.UpScale8To16Bit(color.G);
            B = VectorMaths.UpScale8To16Bit(color.B);
            A = VectorMaths.UpScale8To16Bit(color.A);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Rgba64"/> struct.
        /// </summary>
        /// <param name="color">A structure of 3 bytes in RGB byte order.</param>
        public Rgba64(Rgb24 color)
        {
            R = VectorMaths.UpScale8To16Bit(color.R);
            G = VectorMaths.UpScale8To16Bit(color.G);
            B = VectorMaths.UpScale8To16Bit(color.B);
            A = ushort.MaxValue;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Rgba64"/> struct.
        /// </summary>
        /// <param name="source">A structure of 3 bytes in BGR byte order.</param>
        public Rgba64(Bgr24 source)
        {
            R = VectorMaths.UpScale8To16Bit(source.R);
            G = VectorMaths.UpScale8To16Bit(source.G);
            B = VectorMaths.UpScale8To16Bit(source.B);
            A = ushort.MaxValue;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Rgba64"/> struct.
        /// </summary>
        /// <param name="vector">The <see cref="Vector4"/>.</param>
        public Rgba64(Vector4 vector)
        {
            vector = Vector4.Clamp(vector, Vector4.Zero, Vector4.One) * Max;
            R = (ushort)Math.Round(vector.X);
            G = (ushort)Math.Round(vector.Y);
            B = (ushort)Math.Round(vector.Z);
            A = (ushort)Math.Round(vector.W);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Rgba64"/> struct.
        /// </summary>
        /// <param name="r">Red component value from 0.0f to 1.0f.</param>
        /// <param name="g">Green component value from 0.0f to 1.0f.</param>
        /// <param name="b">Blue component value from 0.0f to 1.0f.</param>
        /// <param name="alpha">Alpha component value from 0.0f to 1.0f.</param>
        public Rgba64(float r, float g, float b, float a) : this(new Vector4(r, g, b, a))
        {
        }

        #endregion

        /// <summary>
        /// Gets or sets the RGB components of this struct as <see cref="Rgb48"/>.
        /// </summary>
        public Rgb48 Rgb
        {
            get => Unsafe.As<Rgba64, Rgb48>(ref this);
            set => Unsafe.As<Rgba64, Rgb48>(ref this) = value;
        }

        /// <inheritdoc/>
        public ulong PackedValue
        {
            get => Unsafe.As<Rgba64, ulong>(ref this);
            set => Unsafe.As<Rgba64, ulong>(ref this) = value;
        }

        /// <summary>
        /// Converts an <see cref="Rgba64"/> to <see cref="Color"/>.
        /// </summary>
        /// <param name="source">The <see cref="Rgba64"/>.</param>
        /// <returns>The <see cref="Color"/>.</returns>
        public static implicit operator Color(Rgba64 source)
        {
            Color tmp = default;
            source.ToColor(ref tmp);
            return tmp;
        }

        /// <summary>
        /// Converts a <see cref="Color"/> to <see cref="Rgba64"/>.
        /// </summary>
        /// <param name="color">The <see cref="Color"/>.</param>
        /// <returns>The <see cref="Rgba64"/>.</returns>
        public static implicit operator Rgba64(Color color)
        {
            Rgba64 tmp = default;
            color.ToRgba64(ref tmp);
            return tmp;
        }

        /// <summary>
        /// Compares two <see cref="Rgba64"/> objects for equality.
        /// </summary>
        /// <param name="left">The <see cref="Rgba64"/> on the left side of the operand.</param>
        /// <param name="right">The <see cref="Rgba64"/> on the right side of the operand.</param>
        /// <returns>
        /// True if the <paramref name="left"/> parameter is equal to the <paramref name="right"/> parameter; otherwise, false.
        /// </returns>
        public static bool operator ==(Rgba64 left, Rgba64 right) => left.PackedValue == right.PackedValue;

        /// <summary>
        /// Compares two <see cref="Rgba64"/> objects for equality.
        /// </summary>
        /// <param name="left">The <see cref="Rgba64"/> on the left side of the operand.</param>
        /// <param name="right">The <see cref="Rgba64"/> on the right side of the operand.</param>
        /// <returns>
        /// True if the <paramref name="left"/> parameter is not equal to the <paramref name="right"/> parameter; otherwise, false.
        /// </returns>
        public static bool operator !=(Rgba64 left, Rgba64 right) => left.PackedValue != right.PackedValue;

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
            A = (ushort)Math.Round(vector.W);
        }

        /// <inheritdoc />
        public Vector4 ToVector4() => new Vector4(R, G, B, A) / Max;

        /// <inheritdoc />
        public void FromArgb32(Argb32 source)
        {
            R = VectorMaths.UpScale8To16Bit(source.R);
            G = VectorMaths.UpScale8To16Bit(source.G);
            B = VectorMaths.UpScale8To16Bit(source.B);
            A = VectorMaths.UpScale8To16Bit(source.A);
        }

        /// <inheritdoc />
        public void FromBgr24(Bgr24 source)
        {
            R = VectorMaths.UpScale8To16Bit(source.R);
            G = (source.G);
            B = (source.B);
            A = ushort.MaxValue;
        }

        /// <inheritdoc />
        public void FromBgra32(Bgra32 source)
        {
            R = VectorMaths.UpScale8To16Bit(source.R);
            G = VectorMaths.UpScale8To16Bit(source.G);
            B = VectorMaths.UpScale8To16Bit(source.B);
            A = VectorMaths.UpScale8To16Bit(source.A);
        }

        /// <inheritdoc/>
        public void FromBgra5551(Bgra5551 source) => FromScaledVector4(source.ToScaledVector4());

        /// <inheritdoc />
        public void FromGray8(Gray8 source)
        {
            ushort rgb = VectorMaths.UpScale8To16Bit(source.PackedValue);
            R = rgb;
            G = rgb;
            B = rgb;
            A = ushort.MaxValue;
        }

        /// <inheritdoc />
        public void FromGray16(Gray16 source)
        {
            R = source.PackedValue;
            G = source.PackedValue;
            B = source.PackedValue;
            A = ushort.MaxValue;
        }

        /// <inheritdoc />
        public void FromRgb24(Rgb24 source)
        {
            R = VectorMaths.UpScale8To16Bit(source.R);
            G = VectorMaths.UpScale8To16Bit(source.G);
            B = VectorMaths.UpScale8To16Bit(source.B);
            A = ushort.MaxValue;
        }

        /// <inheritdoc />
        public void FromColor(Color source)
        {
            R = VectorMaths.UpScale8To16Bit(source.R);
            G = VectorMaths.UpScale8To16Bit(source.G);
            B = VectorMaths.UpScale8To16Bit(source.B);
            A = VectorMaths.UpScale8To16Bit(source.A);
        }

        /// <inheritdoc/>
        public void FromRgb48(Rgb48 source)
        {
            Rgb = source;
            A = ushort.MaxValue;
        }

        /// <inheritdoc/>
        public void FromRgba64(Rgba64 source)
        {
            this = source;
        }

        /// <inheritdoc/>
        public void ToColor(ref Color dest)
        {
            dest.R = VectorMaths.DownScale16To8Bit(R);
            dest.G = VectorMaths.DownScale16To8Bit(G);
            dest.B = VectorMaths.DownScale16To8Bit(B);
            dest.A = VectorMaths.DownScale16To8Bit(A);
        }

        #endregion

        /// <summary>
        /// Convert to <see cref="Bgra32"/>.
        /// </summary>
        /// <returns>The <see cref="Bgra32"/>.</returns>
        public Bgra32 ToBgra32()
        {
            byte r = VectorMaths.DownScale16To8Bit(R);
            byte g = VectorMaths.DownScale16To8Bit(G);
            byte b = VectorMaths.DownScale16To8Bit(B);
            byte a = VectorMaths.DownScale16To8Bit(A);
            return new Bgra32(r, g, b, a);
        }

        /// <summary>
        /// Convert to <see cref="Argb32"/>.
        /// </summary>
        /// <returns>The <see cref="Argb32"/>.</returns>
        public Argb32 ToArgb32()
        {
            byte r = VectorMaths.DownScale16To8Bit(R);
            byte g = VectorMaths.DownScale16To8Bit(G);
            byte b = VectorMaths.DownScale16To8Bit(B);
            byte a = VectorMaths.DownScale16To8Bit(A);
            return new Argb32(r, g, b, a);
        }

        /// <summary>
        /// Convert to <see cref="Rgb24"/>.
        /// </summary>
        /// <returns>The <see cref="Rgb24"/>.</returns>
        public Rgb24 ToRgb24()
        {
            byte r = VectorMaths.DownScale16To8Bit(R);
            byte g = VectorMaths.DownScale16To8Bit(G);
            byte b = VectorMaths.DownScale16To8Bit(B);
            return new Rgb24(r, g, b);
        }

        /// <summary>
        /// Convert to <see cref="Bgr24"/>.
        /// </summary>
        /// <returns>The <see cref="Bgr24"/>.</returns>
        public Bgr24 ToBgr24()
        {
            byte r = VectorMaths.DownScale16To8Bit(R);
            byte g = VectorMaths.DownScale16To8Bit(G);
            byte b = VectorMaths.DownScale16To8Bit(B);
            return new Bgr24(r, g, b);
        }

        /// <inheritdoc />
        public override bool Equals(object obj) => obj is Rgba64 rgba64 && Equals(rgba64);

        /// <inheritdoc />
        public bool Equals(Rgba64 other) => PackedValue.Equals(other.PackedValue);

        /// <inheritdoc />
        public override string ToString() => $"Rgba64({R}, {G}, {B}, {A})";

        /// <inheritdoc />
        public override int GetHashCode() => PackedValue.GetHashCode();
    }
}
