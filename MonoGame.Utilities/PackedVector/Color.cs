// MIT License - Copyright (C) The Mono.Xna Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.Text;
using System.Runtime.Serialization;
using System.Diagnostics;
using MonoGame.Utilities.PackedVector;
using System.Runtime.CompilerServices;

// use this namespace as we want Color to be easily accessible
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
        /// Gets or sets packed value of this <see cref="Color"/>.
        /// </summary>
        [IgnoreDataMember]
        public uint PackedValue
        {
            get => Unsafe.As<Color, uint>(ref this);
            set => Unsafe.As<Color, uint>(ref this) = value;
        }

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
            get
            {
                return string.Concat(
                    R.ToString(), "  ",
                    G.ToString(), "  ",
                    B.ToString(), "  ",
                    A.ToString());
            }
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
        /// <param name="r">Red component value from 0.0f to 1.0f.</param>
        /// <param name="g">Green component value from 0.0f to 1.0f.</param>
        /// <param name="b">Blue component value from 0.0f to 1.0f.</param>
        /// <param name="alpha">Alpha component value from 0.0f to 1.0f.</param>
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
        /// <param name="r">Red component value from 0.0f to 1.0f.</param>
        /// <param name="g">Green component value from 0.0f to 1.0f.</param>
        /// <param name="b">Blue component value from 0.0f to 1.0f.</param>
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
        /// <param name="alpha">Alpha component value from 0.0f to 1.0f.</param>
        public Color(Color color, float alpha) : 
            this(color, MathHelper.Clamp((int)(alpha * 255), byte.MinValue, byte.MaxValue))
        {
        }

        #endregion

        #region IPixel Implementation

        /// <inheritdoc />
        public void ToRgba64(ref Rgba64 destination)
        {
            destination.R = VectorMaths.UpScale8To16Bit(R);
            destination.G = VectorMaths.UpScale8To16Bit(G);
            destination.B = VectorMaths.UpScale8To16Bit(B);
            destination.A = VectorMaths.UpScale8To16Bit(A);
        }

        /// <inheritdoc />
        public void ToColor(ref Color destination) => destination = this;

        /// <inheritdoc />
        public void ToRgb24(ref Rgb24 destination)
        {
            destination.R = R;
            destination.G = G;
            destination.B = B;
        }

        /// <inheritdoc />
        public Vector4 ToVector4() => new Vector4(R / 255f, G / 255f, B / 255f, A / 255f);

        /// <inheritdoc />
        public Vector4 ToScaledVector4() => ToVector4();

        /// <inheritdoc />
        public void FromVector4(Vector4 vector)
        {
            vector *= VectorMaths.MaxBytes;
            vector += VectorMaths.Half;
            vector = Vector4.Clamp(vector, Vector4.Zero, VectorMaths.MaxBytes);

            R = (byte)vector.X;
            G = (byte)vector.Y;
            B = (byte)vector.Z;
            A = (byte)vector.W;
        }

        /// <inheritdoc />
        public void FromScaledVector4(Vector4 vector) => FromVector4(vector);

        /// <inheritdoc />
        public void FromRgba64(Rgba64 color) => FromVector4(color.ToVector4());

        /// <inheritdoc />
        public void FromColor(Color color) => this = color;

        /// <inheritdoc />
        public void FromRgb48(Rgb48 source)
        {
            R = VectorMaths.DownScale16To8Bit(source.R);
            G = VectorMaths.DownScale16To8Bit(source.G);
            B = VectorMaths.DownScale16To8Bit(source.B);
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
        public void FromRgb24(Rgb24 color)
        {
            R = color.R;
            G = color.G;
            B = color.B;
            A = byte.MaxValue;
        }

        /// <inheritdoc />
        public void FromBgr24(Bgr24 source)
        {
            R = source.R;
            G = source.G;
            B = source.B;
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
            byte rgb = VectorMaths.DownScale16To8Bit(source.PackedValue);
            R = rgb;
            G = rgb;
            B = rgb;
            A = byte.MaxValue;
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
        /// Returns a <see cref="string"/> representation of this <see cref="Color"/> in the format
        /// {R:[red] G:[green] B:[blue] A:[alpha]}
        /// </summary>
        /// <returns><see cref="string"/> representation of this <see cref="Color"/>.</returns>
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
        public static Color FromNonPremultiplied(Vector4 vector)
        {
            return new Color(vector.X * vector.W, vector.Y * vector.W, vector.Z * vector.W, vector.W);
        }

        /// <summary>
        /// Translate a non-premultipled alpha color to a <see cref="Color"/> that contains premultiplied alpha.
        /// </summary>
        /// <param name="r">Red component value.</param>
        /// <param name="g">Green component value.</param>
        /// <param name="b">Blue component value.</param>
        /// <param name="a">Alpha component value.</param>
        /// <returns>A <see cref="Color"/> which contains premultiplied alpha data.</returns>
        public static Color FromNonPremultiplied(int r, int g, int b, int a)
        {
            return new Color(r * a / 255, g * a / 255, b * a / 255, a);
        }

        /// <summary>
        /// Performs linear interpolation of <see cref="Color"/>.
        /// </summary>
        /// <param name="value1">Source <see cref="Color"/>.</param>
        /// <param name="value2">Destination <see cref="Color"/>.</param>
        /// <param name="amount">Interpolation factor.</param>
        /// <returns>Interpolated <see cref="Color"/>.</returns>
        public static Color Lerp(Color value1, Color value2, float amount)
        {
            amount = MathHelper.Clamp(amount, 0, 1);
            return new Color(
                (int)MathHelper.Lerp(value1.R, value2.R, amount),
                (int)MathHelper.Lerp(value1.G, value2.G, amount),
                (int)MathHelper.Lerp(value1.B, value2.B, amount),
                (int)MathHelper.Lerp(value1.A, value2.A, amount));
        }

        /// <summary>
        /// Multiply <see cref="Color"/> by value.
        /// </summary>
        /// <param name="value">Source <see cref="Color"/>.</param>
        /// <param name="scale">Multiplicator.</param>
        /// <returns>Multiplication result.</returns>
        public static Color Multiply(Color value, float scale)
        {
            return new Color((int)(value.R * scale), (int)(value.G * scale), (int)(value.B * scale), (int)(value.A * scale));
        }

        /// <summary>
        /// Multiply <see cref="Color"/> by value.
        /// </summary>
        /// <param name="value">Source <see cref="Color"/>.</param>
        /// <param name="scale">Multiplicator.</param>
        /// <returns>Multiplication result.</returns>
        public static Color operator *(Color value, float scale)
        {
            return Multiply(value, scale);
        }

        /// <summary>
        /// Compares whether current instance is equal to specified object.
        /// </summary>
        /// <param name="obj">The <see cref="Color"/> to compare.</param>
        /// <returns><c>true</c> if the instances are equal; <c>false</c> otherwise.</returns>
        public override bool Equals(object obj)
        {
            if (obj is Color color)
                return Equals(color);
            return false;
        }

        /// <summary>
        /// Compares whether current instance is equal to specified <see cref="Color"/>.
        /// </summary>
        /// <param name="other">The <see cref="Color"/> to compare.</param>
        /// <returns><c>true</c> if the instances are equal; <c>false</c> otherwise.</returns>
        public bool Equals(Color other)
        {
            return PackedValue == other.PackedValue;
        }

        /// <summary>
        /// Compares whether two <see cref="Color"/> instances are equal.
        /// </summary>
        /// <param name="a"><see cref="Color"/> instance on the left of the equal sign.</param>
        /// <param name="b"><see cref="Color"/> instance on the right of the equal sign.</param>
        /// <returns><c>true</c> if the instances are equal; <c>false</c> otherwise.</returns>
        public static bool operator ==(Color a, Color b)
        {
            return a.PackedValue == b.PackedValue;
        }

        /// <summary>
        /// Compares whether two <see cref="Color"/> instances are not equal.
        /// </summary>
        /// <param name="a"><see cref="Color"/> instance on the left of the not equal sign.</param>
        /// <param name="b"><see cref="Color"/> instance on the right of the not equal sign.</param>
        /// <returns><c>true</c> if the instances are not equal; <c>false</c> otherwise.</returns>	
        public static bool operator !=(Color a, Color b)
        {
            return a.PackedValue != b.PackedValue;
        }

        /// <summary>
        /// Gets the hash code of this <see cref="Color"/>.
        /// </summary>
        /// <returns>Hash code of this <see cref="Color"/>.</returns>
        public override int GetHashCode()
        {
            return PackedValue.GetHashCode();
        }
    }
}
