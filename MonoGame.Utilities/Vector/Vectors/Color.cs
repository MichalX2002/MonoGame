// MIT License - Copyright (C) The Mono.Xna Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Text;
using MonoGame.Framework.Vector;

namespace MonoGame.Framework
{
    /// <summary>
    /// Packed vector type containing unsigned 8-bit RGBA components.
    /// <para>Ranges from [0, 0, 0, 0] to [1, 1, 1, 1] in vector form.</para>
    /// </summary>
    [DataContract]
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    [StructLayout(LayoutKind.Sequential)]
    public partial struct Color : IPackedPixel<Color, uint>
    {
        VectorComponentInfo IVector.ComponentInfo => new VectorComponentInfo(
            new VectorComponent(VectorComponentType.Int8, VectorComponentChannel.Red),
            new VectorComponent(VectorComponentType.Int8, VectorComponentChannel.Green),
            new VectorComponent(VectorComponentType.Int8, VectorComponentChannel.Blue),
            new VectorComponent(VectorComponentType.Int8, VectorComponentChannel.Alpha));

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

        #region Properties

        /// <summary>
        /// Gets or sets the RGB components of this <see cref="Framework.Color"/> as <see cref="Rgb24"/>
        /// </summary>
        public Rgb24 Rgb
        {
            readonly get => UnsafeR.As<Color, Rgb24>(this);
            set => Unsafe.As<Color, Rgb24>(ref this) = value;
        }

        /// <summary>
        /// Gets or sets the RGB components of this struct as <see cref="Bgr24"/> reverting the component order.
        /// </summary>
        public Bgr24 Bgr
        {
            readonly get => new Bgr24(R, G, B);
            set
            {
                R = value.R;
                G = value.G;
                B = value.B;
            }
        }

        internal string DebuggerDisplay => string.Concat(
            R.ToString(), "  ",
            G.ToString(), "  ",
            B.ToString(), "  ",
            A.ToString());

        #endregion

        #region Constructors

        /// <summary>
        /// Constructs the <see cref="Framework.Color"/> with raw values.
        /// </summary>
        /// <remarks>
        /// This overload sets the values directly without clamping and 
        /// may therefore be faster than the other overloads.
        /// </remarks>
        public Color(byte r, byte g, byte b, byte a)
        {
            R = r;
            G = g;
            B = b;
            A = a;
        }

        /// <summary>
        /// Constructs the <see cref="Framework.Color"/> with raw values.
        /// </summary>
        /// <remarks>
        /// This overload sets the values directly without clamping and 
        /// may therefore be faster than the other overloads.
        /// </remarks>
        public Color(byte r, byte g, byte b) : this(r, g, b, byte.MaxValue)
        {
        }

        /// <summary>
        /// Constructs the <see cref="Framework.Color"/> with raw values.
        /// </summary>
        /// <remarks>
        /// This overload sets the values directly without clamping and 
        /// may therefore be faster than the other overloads.
        /// </remarks>
        public Color(byte luminance, byte alpha) : this(luminance, luminance, luminance, alpha)
        {
        }

        /// <summary>
        /// Constructs the <see cref="Framework.Color"/> with a packed value.
        /// R in the least significant byte.
        /// </summary>
        /// <remarks>
        /// This overload sets the values directly without clamping and 
        /// may therefore be faster than the other overloads.
        /// </remarks>
        [CLSCompliant(false)]
        public Color(uint packed) : this()
        {
            // TODO: Unsafe.SkipInit(out this)
            PackedValue = packed;
        }

        /// <summary>
        /// Constructs the <see cref="Framework.Color"/> with clamped raw values. 
        /// <para>
        /// The values are clamped between <see cref="byte.MinValue"/> and <see cref="byte.MaxValue"/>.
        /// </para>
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
        /// Constructs the <see cref="Framework.Color"/> with clamped raw values. 
        /// Alpha value will be opaque.
        /// <para>
        /// The values are clamped between <see cref="byte.MinValue"/> and <see cref="byte.MaxValue"/>.
        /// </para>
        /// </summary>
        /// <param name="r">Red component value from 0 to 255.</param>
        /// <param name="g">Green component value from 0 to 255.</param>
        /// <param name="b">Blue component value from 0 to 255.</param>
        public Color(int r, int g, int b) : this(
            MathHelper.Clamp(r, byte.MinValue, byte.MaxValue),
            MathHelper.Clamp(g, byte.MinValue, byte.MaxValue),
            MathHelper.Clamp(b, byte.MinValue, byte.MaxValue))
        {
        }

        #region Constructors (float and Vector)

        /// <summary>
        /// Constructs the <see cref="Framework.Color"/> with vector form values.
        /// </summary>
        /// <param name="vector"><see cref="Vector4"/> containing the components.</param>
        public Color(Vector4 color)
        {
            color *= byte.MaxValue;
            color.Clamp(Vector4.Zero, Vector4.MaxValueByte);

            R = (byte)color.X;
            G = (byte)color.Y;
            B = (byte)color.Z;
            A = (byte)color.W;
        }

        /// <summary>
        /// Constructs the <see cref="Framework.Color"/> from vector form values.
        /// </summary>
        /// <param name="r">Red component value from 0 to 1.</param>
        /// <param name="g">Green component value from 0 to 1.</param>
        /// <param name="b">Blue component value from 0 to 1.</param>
        /// <param name="a">Alpha component value from 0 to 1.</param>
        public Color(float r, float g, float b, float a) : this(new Vector4(r, g, b, a))
        {
        }

        /// <summary>
        /// Constructs the <see cref="Framework.Color"/> with vector form values.
        /// Alpha value will be opaque.
        /// <para>The values are clamped between 0 and 1.</para>
        /// </summary>
        /// <param name="color"><see cref="Vector3"/> containing the components.</param>
        public Color(Vector3 color)
        {
            color *= byte.MaxValue;
            color.Clamp(Vector3.Zero, Vector3.MaxValueByte);

            R = (byte)color.X;
            G = (byte)color.Y;
            B = (byte)color.Z;
            A = byte.MaxValue;
        }

        /// <summary>
        /// Constructs the <see cref="Framework.Color"/> with vector form values. 
        /// Alpha value will be opaque.
        /// </summary>
        /// <param name="r">Red component value from 0 to 1.</param>
        /// <param name="g">Green component value from 0 to 1.</param>
        /// <param name="b">Blue component value from 0 to 1.</param>
        public Color(float r, float g, float b) : this(new Vector3(r, g, b))
        {
        }

        #endregion

        #region Constructors (Color)

        /// <summary>
        /// Constructs <see cref="Framework.Color"/> from a <see cref="Framework.Color"/> and alpha value.
        /// </summary>
        /// <param name="color">The RGB values.</param>
        /// <param name="alpha">The alpha component value from 0 to 255.</param>
        public Color(Color color, byte alpha) : this(color.R, color.G, color.B, alpha)
        {
        }

        /// <summary>
        /// Constructs <see cref="Framework.Color"/> from an <see cref="Framework.Color"/> and alpha value.
        /// </summary>
        /// <param name="color">The RGB values.</param>
        /// <param name="alpha">Alpha component value from 0 to 1.</param>
        public Color(Color color, int alpha) :
            this(color, MathHelper.Clamp(alpha, byte.MinValue, byte.MaxValue))
        {
        }

        /// <summary>
        /// Constructs the <see cref="Framework.Color"/> from a <see cref="Framework.Color"/> and alpha value.
        /// </summary>
        /// <param name="color">The RGB values.</param>
        /// <param name="alpha">Alpha component value from 0 to 1.</param>
        public Color(Color color, float alpha) :
            this(color, (int)(alpha * byte.MaxValue))
        {
        }

        #endregion

        #region Constructors (Rgb24)

        /// <summary>
        /// Constructs <see cref="Framework.Color"/> from an <see cref="Rgb24"/> and alpha value.
        /// </summary>
        /// <param name="color">The RGB values.</param>
        public Color(Rgb24 color) :
            this(color.R, color.G, color.B, byte.MaxValue)
        {
        }

        /// <summary>
        /// Constructs <see cref="Framework.Color"/> from an <see cref="Rgb24"/> and alpha value.
        /// </summary>
        /// <param name="color">The RGB values.</param>
        /// <param name="alpha">The alpha component value from 0 to 255.</param>
        public Color(Rgb24 color, byte alpha) :
            this(color.R, color.G, color.B, alpha)
        {
        }

        /// <summary>
        /// Constructs <see cref="Framework.Color"/> from an <see cref="Rgb24"/> and alpha value.
        /// </summary>
        /// <param name="color">The RGB values.</param>
        /// <param name="alpha">Alpha component value from 0 to 1.</param>
        public Color(Rgb24 color, int alpha) :
            this(color, MathHelper.Clamp(alpha, byte.MinValue, byte.MaxValue))
        {
        }

        /// <summary>
        /// Constructs <see cref="Framework.Color"/> from an <see cref="Rgb24"/> and alpha value.
        /// </summary>
        /// <param name="color">The RGB values.</param>
        /// <param name="alpha">Alpha component value from 0 to 1.</param>
        public Color(Rgb24 color, float alpha) :
            this(color, (int)(alpha * byte.MaxValue))
        {
        }

        #endregion

        #endregion

        #region IPackedVector

        [IgnoreDataMember]
        [CLSCompliant(false)]
        public uint PackedValue
        {
            readonly get => UnsafeR.As<Color, uint>(this);
            set => Unsafe.As<Color, uint>(ref this) = value;
        }

        public readonly Vector4 ToScaledVector4()
        {
            return new Vector4(R, G, B, A) / byte.MaxValue;
        }

        public void FromScaledVector4(Vector4 scaledVector)
        {
            scaledVector *= byte.MaxValue;
            scaledVector += Vector4.Half;
            scaledVector.Clamp(Vector4.Zero, Vector4.MaxValueByte);

            R = (byte)scaledVector.X;
            G = (byte)scaledVector.Y;
            B = (byte)scaledVector.Z;
            A = (byte)scaledVector.W;
        }

        public readonly Vector4 ToVector4()
        {
            return ToScaledVector4();
        }

        public void FromVector4(Vector4 vector)
        {
            FromScaledVector4(vector);
        }

        #endregion

        #region IPixel

        public void FromGray8(Gray8 source)
        {
            R = G = B = source.PackedValue;
            A = byte.MaxValue;
        }

        public void FromGray16(Gray16 source)
        {
            R = G = B = PackedVectorHelper.DownScale16To8Bit(source.PackedValue);
            A = byte.MaxValue;
        }

        public void FromGrayAlpha16(GrayAlpha16 source)
        {
            R = G = B = source.L;
            A = source.A;
        }

        public void FromRgba32(Color color) => this = color;

        public void FromRgb24(Rgb24 source)
        {
            Rgb = source;
            A = byte.MaxValue;
        }

        public void FromRgb48(Rgb48 source)
        {
            R = PackedVectorHelper.DownScale16To8Bit(source.R);
            G = PackedVectorHelper.DownScale16To8Bit(source.G);
            B = PackedVectorHelper.DownScale16To8Bit(source.B);
            A = byte.MaxValue;
        }

        public void FromRgba64(Rgba64 source)
        {
            R = PackedVectorHelper.DownScale16To8Bit(source.R);
            G = PackedVectorHelper.DownScale16To8Bit(source.G);
            B = PackedVectorHelper.DownScale16To8Bit(source.B);
            A = PackedVectorHelper.DownScale16To8Bit(source.A);
        }

        #endregion

        public void FromBgra5551(Bgra5551 source)
        {
            FromVector4(source.ToVector4());
        }

        public void FromBgr24(Bgr24 source)
        {
            Bgr = source;
            A = byte.MaxValue;
        }

        public void FromArgb32(Argb32 source)
        {
            R = source.R;
            G = source.G;
            B = source.B;
            A = source.A;
        }

        public void FromBgra32(Bgra32 source)
        {
            R = source.R;
            G = source.G;
            B = source.B;
            A = source.A;
        }

        /// <summary>
        /// Gets the <see cref="Bgra32"/> representation of this <see cref="Framework.Color"/>.
        /// </summary>
        public readonly Bgra32 ToBgra32()
        {
            return new Bgra32(R, G, B, A);
        }

        /// <summary>
        /// Gets the <see cref="Argb32"/> representation of this <see cref="Framework.Color"/>.
        /// </summary>
        public readonly Argb32 ToArgb32()
        {
            return new Argb32(R, G, B, A);
        }

        /// <summary>
        /// Gets the <see cref="Rgb24"/> representation of this <see cref="Framework.Color"/>.
        /// </summary>
        public readonly Rgb24 ToRgb24()
        {
            return new Rgb24(R, G, B);
        }

        /// <summary>
        /// Gets the <see cref="Bgr24"/> representation of this <see cref="Framework.Color"/>.
        /// </summary>
        public readonly Bgr24 ToBgr24()
        {
            return new Bgr24(R, G, B);
        }

        /// <summary>
        /// Gets the <see cref="Gray8"/> representation of this <see cref="Framework.Color"/>.
        /// </summary>
        public readonly Gray8 ToGray8()
        {
            return new Gray8(A);
        }

        /// <summary>
        /// Gets the <see cref="GrayAlpha16 "/> representation of this <see cref="Framework.Color"/>.
        /// </summary>
        public readonly GrayAlpha16 ToGrayAlpha16()
        {
            return new GrayAlpha16(PackedVectorHelper.Get8BitBT709Luminance(R, G, B), A);
        }

        /// <summary>
        /// Gets the <see cref="Vector3"/> representation of this <see cref="Framework.Color"/>.
        /// </summary>
        public readonly Vector3 ToVector3()
        {
            return new Vector3(R, G, B) / byte.MaxValue;
        }

        public readonly Rgba64 ToRgba64()
        {
            return new Rgba64(
                PackedVectorHelper.UpScale8To16Bit(R),
                PackedVectorHelper.UpScale8To16Bit(G),
                PackedVectorHelper.UpScale8To16Bit(B),
                PackedVectorHelper.UpScale8To16Bit(A));
        }

        /// <summary>
        /// Translate a non-premultipled alpha <see cref="Vector4"/> color to a
        /// <see cref="Framework.Color"/> that contains premultiplied alpha.
        /// </summary>
        /// <param name="vector">A <see cref="Vector4"/> representing color.</param>
        /// <returns>A <see cref="Framework.Color"/> which contains premultiplied alpha data.</returns>
        public static Color FromNonPremultiplied(Vector4 vector)
        {
            float w = vector.W;
            vector.W = 1;
            vector *= w;
            return new Color(vector);
        }

        /// <summary>
        /// Translate a non-premultipled alpha color to a <see cref="Framework.Color"/> that contains premultiplied alpha.
        /// </summary>
        /// <param name="r">Red component value.</param>
        /// <param name="g">Green component value.</param>
        /// <param name="b">Blue component value.</param>
        /// <param name="a">Alpha component value.</param>
        /// <returns>A <see cref="Framework.Color"/> which contains premultiplied alpha data.</returns>
        public static Color FromNonPremultiplied(int r, int g, int b, int a)
        {
            return new Color(r * a / 255, g * a / 255, b * a / 255, a);
        }

        /// <summary>
        /// Performs linear interpolation of <see cref="Framework.Color"/>.
        /// </summary>
        /// <param name="start">Source <see cref="Framework.Color"/>.</param>
        /// <param name="end">Destination <see cref="Framework.Color"/>.</param>
        /// <param name="amount">Interpolation factor.</param>
        /// <returns>Interpolated <see cref="Framework.Color"/>.</returns>
        public static Color Lerp(Color start, Color end, float amount)
        {
            amount = MathHelper.Clamp(amount, 0, 1);

            return new Color(
                (int)MathHelper.Lerp(start.R, end.R, amount),
                (int)MathHelper.Lerp(start.G, end.G, amount),
                (int)MathHelper.Lerp(start.B, end.B, amount),
                (int)MathHelper.Lerp(start.A, end.A, amount));
        }

        /// <summary>
        /// Multiply <see cref="Framework.Color"/> by value.
        /// </summary>
        /// <param name="value">Source <see cref="Framework.Color"/>.</param>
        /// <param name="scale">Factor.</param>
        /// <returns>Multiplication result.</returns>
        public static Color Multiply(Color value, float scale)
        {
            return value * scale;
        }

        /// <summary>
        /// Multiply <see cref="Framework.Color"/> by a scalar.
        /// </summary>
        /// <param name="value">Source <see cref="Framework.Color"/>.</param>
        /// <param name="scale">Factor.</param>
        /// <returns>Multiplication result.</returns>
        public static Color operator *(Color value, float scale)
        {
            return new Color(
                (int)(value.R * scale),
                (int)(value.G * scale),
                (int)(value.B * scale),
                (int)(value.A * scale));
        }

        /// <summary>
        /// Gets the hexadecimal <see cref="string"/> representation of this <see cref="Framework.Color"/>.
        /// </summary>
        public string ToHex()
        {
            uint hexOrder = (uint)(A << 0 | B << 8 | G << 16 | R << 24);
            return hexOrder.ToString("x8");
        }

        #region Deconstruct

        /// <summary>
        /// Deconstruction method for <see cref="Framework.Color"/>.
        /// </summary>
        /// <param name="r">Red component value from 0 to 255.</param>
        /// <param name="g">Green component value from 0 to 255.</param>
        /// <param name="b">Blue component value from 0 to 255.</param>
        public readonly void Deconstruct(out byte r, out byte g, out byte b)
        {
            r = R;
            g = G;
            b = B;
        }

        /// <summary>
        /// Deconstruction method for <see cref="Framework.Color"/>.
        /// </summary>
        /// <param name="r">Red component value from 0.0f to 1.0f.</param>
        /// <param name="g">Green component value from 0.0f to 1.0f.</param>
        /// <param name="b">Blue component value from 0.0f to 1.0f.</param>
        public readonly void Deconstruct(out float r, out float g, out float b)
        {
            r = R / 255f;
            g = G / 255f;
            b = B / 255f;
        }

        /// <summary>
        /// Deconstruction method for <see cref="Framework.Color"/> with Alpha.
        /// </summary>
        /// <param name="r">Red component value from 0 to 255.</param>
        /// <param name="g">Green component value from 0 to 255.</param>
        /// <param name="b">Blue component value from 0 to 255.</param>
        /// <param name="a">Alpha component value from 0 to 255.</param>
        public readonly void Deconstruct(out byte r, out byte g, out byte b, out byte a)
        {
            r = R;
            g = G;
            b = B;
            a = A;
        }

        /// <summary>
        /// Deconstruction method for <see cref="Framework.Color"/> with Alpha.
        /// </summary>
        /// <param name="r">Red component value from 0.0f to 1.0f.</param>
        /// <param name="g">Green component value from 0.0f to 1.0f.</param>
        /// <param name="b">Blue component value from 0.0f to 1.0f.</param>
        /// <param name="a">Alpha component value from 0.0f to 1.0f.</param>
        public readonly void Deconstruct(out float r, out float g, out float b, out float a)
        {
            r = R / 255f;
            g = G / 255f;
            b = B / 255f;
            a = A / 255f;
        }

        #endregion

        #region Equals

        /// <summary>
        /// Compares whether current instance is equal to specified <see cref="Framework.Color"/>.
        /// </summary>
        public readonly bool Equals(Color other)
        {
            return this == other;
        }

        /// <summary>
        /// Compares whether current instance is equal to specified object.
        /// </summary>
        public override readonly bool Equals(object obj)
        {
            return obj is Color other && Equals(other);
        }

        /// <summary>
        /// Compares whether two <see cref="Framework.Color"/> instances are equal.
        /// </summary>
        public static bool operator ==(in Color a, in Color b)
        {
            return a.PackedValue == b.PackedValue;
        }

        /// <summary>
        /// Compares whether two <see cref="Framework.Color"/> instances are not equal.
        /// </summary>
        public static bool operator !=(in Color a, in Color b)
        {
            return a.PackedValue != b.PackedValue;
        }
        #endregion

        #region Object Overrides

        /// <summary>
        /// Returns a <see cref="string"/> representation of this <see cref="Framework.Color"/>.
        /// </summary>
        public override readonly string ToString()
        {
            var sb = new StringBuilder(28 + nameof(Framework.Color).Length);
            sb.Append(nameof(Framework.Color));
            sb.Append("(R:");
            sb.Append(R);
            sb.Append(", G:");
            sb.Append(G);
            sb.Append(", B:");
            sb.Append(B);
            sb.Append(", A:");
            sb.Append(A);
            sb.Append(")");
            return sb.ToString();
        }

        /// <summary>
        /// Gets a hash code of the <see cref="Framework.Color"/>.
        /// </summary>
        public override readonly int GetHashCode() => PackedValue.GetHashCode();

        #endregion
    }
}
