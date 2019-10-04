// MIT License - Copyright (C) The Mono.Xna Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.Text;
using System.Runtime.Serialization;
using System.Diagnostics;
using MonoGame.Utilities.PackedVector;
using System.Runtime.CompilerServices;

namespace MonoGame.Framework
{
    /// <summary>
    /// Packed pixel type containing four 8-bit unsigned normalized values, ranging from 0 to 255.
    /// The color components are stored in red, green, blue, and alpha order (least significant to most significant byte).
    /// <para>
    /// Ranges from [0, 0, 0, 0] to [1, 1, 1, 1] in vector form.
    /// </para>
    /// </summary>
    [DataContract]
    [DebuggerDisplay("{DebugDisplayString,nq}")]
    public partial struct Color : IPixel, IPackedVector<uint>, IEquatable<Color>
    {
        /// <summary>
        /// Gets or sets the red component.
        /// </summary>
        [DataMember]
        public byte R;

        /// <summary>
        /// Gets or sets the green component.
        /// </summary>
        [DataMember]
        public byte G;

        /// <summary>
        /// Gets or sets the blue component.
        /// </summary>
        [DataMember]
        public byte B;

        /// <summary>
        /// Gets or sets the alpha component.
        /// </summary>
        [DataMember]
        public byte A;

        #region Data Properties

        /// <summary>
        /// Gets or sets the RGB components of this struct as <see cref="Rgb24"/>
        /// </summary>
        public Rgb24 Rgb
        {
            get => new Rgb24(R, G, B);
            set
            {
                R = value.R;
                G = value.G;
                B = value.B;
            }
        }

        /// <summary>
        /// Gets or sets the RGB components of this struct as <see cref="Bgr24"/> reverting the component order.
        /// </summary>
        public Bgr24 Bgr
        {
            get => new Bgr24(R, G, B);
            set
            {
                R = value.R;
                G = value.G;
                B = value.B;
            }
        }

        internal string DebugDisplayString
        {
            get => string.Concat(
                    R.ToString(), "  ",
                    G.ToString(), "  ",
                    B.ToString(), "  ",
                    A.ToString());
        }

        #endregion

        #region Public Constructors

        /// <summary>
        /// Constructs an RGBA color from a packed value.
        /// The value is a 32-bit unsigned integer, with R in the least significant octet.
        /// </summary>
        /// <param name="packedValue">The packed value.</param>
        public Color(uint packedValue) : this() => PackedValue = packedValue;

        /// <summary>
        /// Constructs an RGBA color from scalars representing red, green, blue and alpha values.
        /// </summary>
        /// <remarks>
        /// This overload sets the values directly without clamping and may therefore be faster than the other overloads.
        /// </remarks>
        /// <param name="r"></param>
        /// <param name="g"></param>
        /// <param name="b"></param>
        /// <param name="a"></param>
        public Color(byte r, byte g, byte b, byte a)
        {
            R = r;
            G = g;
            B = b;
            A = a;
        }

        /// <summary>
        /// Constructs an RGBA color from scalars representing red, green, blue and alpha values.
        /// </summary>
        /// <param name="r">Red component value from 0 to 255.</param>
        /// <param name="g">Green component value from 0 to 255.</param>
        /// <param name="b">Blue component value from 0 to 255.</param>
        /// <param name="a">Alpha component value from 0 to 255.</param>
        public Color(int r, int g, int b, int a) : this(
            MathHelper.Clamp(r, byte.MinValue, byte.MaxValue),
            MathHelper.Clamp(g, byte.MinValue, byte.MaxValue),
            MathHelper.Clamp(b, byte.MinValue, byte.MaxValue),
            MathHelper.Clamp(a, byte.MinValue, byte.MaxValue))
        {
        }

        /// <summary>
        /// Constructs an RGBA color from scalars representing red, green and blue values. Alpha value will be opaque.
        /// </summary>
        /// <param name="r">Red component value from 0 to 255.</param>
        /// <param name="g">Green component value from 0 to 255.</param>
        /// <param name="b">Blue component value from 0 to 255.</param>
        public Color(int r, int g, int b) : this(r, g, b, 255)
        {
        }

        /// <summary>
        /// Constructs an RGBA color from scalars representing red, green, blue and alpha values.
        /// </summary>
        /// <param name="r">Red component value from 0f to 1f.</param>
        /// <param name="g">Green component value from 0f to 1f.</param>
        /// <param name="b">Blue component value from 0f to 1f.</param>
        /// <param name="alpha">Alpha component value from 0f to 1f.</param>
        public Color(float r, float g, float b, float alpha)
            : this((int)(r * 255), (int)(g * 255), (int)(b * 255), (int)(alpha * 255))
        {
        }

        /// <summary>
        /// Constructs an RGBA color from the XYZW unit length components of a vector.
        /// </summary>
        /// <param name="color">A <see cref="Vector4"/> representing color.</param>
        public Color(Vector4 color) : this(color.X, color.Y, color.Z, color.W)
        {
        }

        /// <summary>
        /// Constructs an RGBA color from scalars representing red, green and blue values. Alpha value will be opaque.
        /// </summary>
        /// <param name="r">Red component value from 0f to 1f.</param>
        /// <param name="g">Green component value from 0f to 1f.</param>
        /// <param name="b">Blue component value from 0f to 1f.</param>
        public Color(float r, float g, float b) : this(r, g, b, 1f)
        {
        }

        /// <summary>
        /// Constructs an RGBA color from the XYZ unit length components of a vector. Alpha value will be opaque.
        /// </summary>
        /// <param name="color">A <see cref="Vector3"/> representing color.</param>
        public Color(Vector3 color) : this(color.X, color.Y, color.Z)
        {
        }

        /// <summary>
        /// Constructs an RGBA color from a <see cref="Color"/> and an alpha value.
        /// </summary>
        /// <param name="color">A <see cref="Color"/> for RGB values of new <see cref="Color"/> instance.</param>
        /// <param name="alpha">The alpha component value from 0 to 255.</param>
        public Color(Color color, byte alpha) : this(color.R, color.G, color.B, alpha)
        {
        }

        /// <summary>
        /// Constructs an RGBA color from color and alpha value.
        /// </summary>
        /// <param name="color">A <see cref="Color"/> for RGB values of new <see cref="Color"/> instance.</param>
        /// <param name="alpha">Alpha component value from 0f to 1f.</param>
        public Color(Color color, float alpha) : 
            this(color, MathHelper.Clamp((int)(alpha * 255), byte.MinValue, byte.MaxValue))
        {
        }

        #endregion

        #region IPackedVector

        /// <inheritdoc />
        [IgnoreDataMember]
        [CLSCompliant(false)]
        public uint PackedValue
        {
            get => Unsafe.As<Color, uint>(ref this);
            set => Unsafe.As<Color, uint>(ref this) = value;
        }

        /// <inheritdoc />
        public Vector4 ToVector4() => new Vector4(R / 255f, G / 255f, B / 255f, A / 255f);

        /// <inheritdoc />
        public void FromVector4(Vector4 vector)
        {
            vector *= PackedVectorHelper.MaxBytes;
            vector += PackedVectorHelper.Half;
            vector = Vector4.Clamp(vector, Vector4.Zero, PackedVectorHelper.MaxBytes);

            R = (byte)vector.X;
            G = (byte)vector.Y;
            B = (byte)vector.Z;
            A = (byte)vector.W;
        }

        #endregion

        #region IPixel

        /// <inheritdoc />
        public Rgba64 ToRgba64() => new Rgba64(
            PackedVectorHelper.UpScale8To16Bit(R),
            PackedVectorHelper.UpScale8To16Bit(G),
            PackedVectorHelper.UpScale8To16Bit(B),
            PackedVectorHelper.UpScale8To16Bit(A));

        /// <inheritdoc />
        public void ToColor(ref Color dest) => dest = this;

        /// <inheritdoc />
        public void FromRgba64(Rgba64 color) => FromVector4(color.ToVector4());

        /// <inheritdoc />
        public void FromColor(Color color) => this = color;

        /// <inheritdoc />
        public void FromRgb48(Rgb48 source)
        {
            R = PackedVectorHelper.DownScale16To8Bit(source.R);
            G = PackedVectorHelper.DownScale16To8Bit(source.G);
            B = PackedVectorHelper.DownScale16To8Bit(source.B);
            A = byte.MaxValue;
        }

        /// <inheritdoc />
        public void FromArgb32(Argb32 source)
        {
            R = source.R;
            G = source.G;
            B = source.B;
            A = source.A;
        }

        /// <inheritdoc />
        public void FromBgra32(Bgra32 source)
        {
            R = source.R;
            G = source.G;
            B = source.B;
            A = source.A;
        }

        /// <inheritdoc />
        public void FromBgra5551(Bgra5551 source) => FromVector4(source.ToVector4());

        /// <inheritdoc />
        public void FromRgb24(Rgb24 source)
        {
            Rgb = source;
            A = byte.MaxValue;
        }

        /// <inheritdoc />
        public void FromBgr24(Bgr24 source)
        {
            Bgr = source;
            A = byte.MaxValue;
        }

        /// <inheritdoc />
        public void FromGray8(Gray8 source)
        {
            R = source.PackedValue;
            G = source.PackedValue;
            B = source.PackedValue;
            A = byte.MaxValue;
        }

        /// <inheritdoc />
        public void FromGray16(Gray16 source)
        {
            byte lum = PackedVectorHelper.DownScale16To8Bit(source.PackedValue);
            R = lum;
            G = lum;
            B = lum;
            A = byte.MaxValue;
        }

        /// <inheritdoc />
        public void FromGrayAlpha(GrayAlpha16 source)
        {
            R = source.L;
            G = source.L;
            B = source.L;
            A = source.A;
        }

        #endregion

        /// <summary>
        /// Convert to <see cref="Bgra32"/>.
        /// </summary>
        /// <returns>The <see cref="Bgra32"/>.</returns>
        public Bgra32 ToBgra32() => new Bgra32(R, G, B, A);

        /// <summary>
        /// Convert to <see cref="Argb32"/>.
        /// </summary>
        /// <returns>The <see cref="Argb32"/>.</returns>
        public Argb32 ToArgb32() => new Argb32(R, G, B, A);

        /// <summary>
        /// Convert to <see cref="Rgb24"/>.
        /// </summary>
        /// <returns>The <see cref="Rgb24"/>.</returns>
        public Rgb24 ToRgb24() => new Rgb24(R, G, B);

        /// <summary>
        /// Convert to <see cref="Bgr24"/>.
        /// </summary>
        /// <returns>The <see cref="Bgr24"/>.</returns>
        public Bgr24 ToBgr24() => new Bgr24(R, G, B);

        /// <summary>
        /// Convert to <see cref="Gray8"/>.
        /// </summary>
        public Gray8 ToGray8() => new Gray8(A);

        /// <summary>
        /// Convert to <see cref="GrayAlpha16"/>.
        /// </summary>
        /// <returns>The <see cref="GrayAlpha16"/>.</returns>
        public GrayAlpha16 ToGrayAlpha16() => new GrayAlpha16(PackedVectorHelper.Get8BitBT709Luminance(R, G, B), A);
        
        /// <summary>
        /// Gets a <see cref="Vector3"/> representation for this object.
        /// </summary>
        /// <returns>A <see cref="Vector3"/> representation for this object.</returns>
        public Vector3 ToVector3() => new Vector3(R / 255f, G / 255f, B / 255f);

        /// <summary>
        /// Converts the value of this instance to a hexadecimal string.
        /// </summary>
        /// <returns>A hexadecimal string representation of the value.</returns>
        public string ToHex()
        {
            uint hexOrder = (uint)(A << 0 | B << 8 | G << 16 | R << 24);
            return hexOrder.ToString("X8");
        }

        /// <summary>
        /// Deconstruction method for <see cref="Color"/>.
        /// </summary>
        /// <param name="r"></param>
        /// <param name="g"></param>
        /// <param name="b"></param>
        public void Deconstruct(out float r, out float g, out float b)
        {
            r = R;
            g = G;
            b = B;
        }

        /// <summary>
        /// Deconstruction method for <see cref="Color"/> with Alpha.
        /// </summary>
        /// <param name="r"></param>
        /// <param name="g"></param>
        /// <param name="b"></param>
        /// <param name="a"></param>
        public void Deconstruct(out float r, out float g, out float b, out float a)
        {
            r = R;
            g = G;
            b = B;
            a = A;
        }

        /// <summary>
        /// Translate a non-premultipled alpha <see cref="Vector4"/> color to a
        /// <see cref="Color"/> that contains premultiplied alpha.
        /// </summary>
        /// <param name="vector">A <see cref="Vector4"/> representing color.</param>
        /// <returns>A <see cref="Color"/> which contains premultiplied alpha data.</returns>
        public static Color FromNonPremultiplied(in Vector4 vector) => 
            new Color(vector.X * vector.W, vector.Y * vector.W, vector.Z * vector.W, vector.W);

        /// <summary>
        /// Translate a non-premultipled alpha color to a <see cref="Color"/> that contains premultiplied alpha.
        /// </summary>
        /// <param name="r">Red component value.</param>
        /// <param name="g">Green component value.</param>
        /// <param name="b">Blue component value.</param>
        /// <param name="a">Alpha component value.</param>
        /// <returns>A <see cref="Color"/> which contains premultiplied alpha data.</returns>
        public static Color FromNonPremultiplied(int r, int g, int b, int a) => 
            new Color(r * a / 255, g * a / 255, b * a / 255, a);

        /// <summary>
        /// Performs linear interpolation of <see cref="Color"/>.
        /// </summary>
        /// <param name="start">Source <see cref="Color"/>.</param>
        /// <param name="end">Destination <see cref="Color"/>.</param>
        /// <param name="amount">Interpolation factor.</param>
        /// <returns>Interpolated <see cref="Color"/>.</returns>
        public static Color Lerp(in Color start, in Color end, float amount)
        {
            amount = MathHelper.Clamp(amount, 0, 1);
            return new Color(
                (int)MathHelper.Lerp(start.R, end.R, amount),
                (int)MathHelper.Lerp(start.G, end.G, amount),
                (int)MathHelper.Lerp(start.B, end.B, amount),
                (int)MathHelper.Lerp(start.A, end.A, amount));
        }

        /// <summary>
        /// Multiply <see cref="Color"/> by value.
        /// </summary>
        /// <param name="value">Source <see cref="Color"/>.</param>
        /// <param name="scale">Factor.</param>
        /// <returns>Multiplication result.</returns>
        public static Color Multiply(in Color value, float scale)
        {
            return new Color(
                (int)(value.R * scale),
                (int)(value.G * scale),
                (int)(value.B * scale), 
                (int)(value.A * scale));
        }

        /// <summary>
        /// Multiply <see cref="Color"/> by value.
        /// </summary>
        /// <param name="value">Source <see cref="Color"/>.</param>
        /// <param name="scale">Factor.</param>
        /// <returns>Multiplication result.</returns>
        public static Color operator *(in Color value, float scale) => Multiply(value, scale);

        #region Equals

        /// <summary>
        /// Compares whether two <see cref="Color"/> instances are equal.
        /// </summary>
        public static bool operator ==(in Color a, in Color b)
        {
            return a.R == b.R
                && a.G == b.G
                && a.B == b.B
                && a.A == b.A;
        }

        /// <summary>
        /// Compares whether two <see cref="Color"/> instances are not equal.
        /// </summary>
        public static bool operator !=(in Color a, in Color b) => !(a == b);

        /// <summary>
        /// Compares whether current instance is equal to specified <see cref="Color"/>.
        /// </summary>
        public bool Equals(Color other) => this == other;

        /// <summary>
        /// Compares whether current instance is equal to specified object.
        /// </summary>
        public override bool Equals(object obj) => obj is Color other && Equals(other);

        #endregion

        #region Object Overrides

        /// <summary>
        /// Returns a <see cref="string"/> representation of this <see cref="Color"/> in the format
        /// {R:{0} G:{1} B:{2} A:{3}}.
        /// </summary>
        public override string ToString()
        {
            var sb = new StringBuilder(29);
            sb.Append("{R:");
            sb.Append(R);
            sb.Append(" G:");
            sb.Append(G);
            sb.Append(" B:");
            sb.Append(B);
            sb.Append(" A:");
            sb.Append(A);
            sb.Append("}");
            return sb.ToString();
        }

        /// <summary>
        /// Gets a hash code of the <see cref="Color"/>.
        /// </summary>
        public override int GetHashCode() => PackedValue.GetHashCode();

        #endregion
    }
}
