// MIT License - Copyright (C) The Mono.Xna Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.Text;
using System.Runtime.Serialization;
using System.Diagnostics;
using MonoGame.Framework.Vector;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

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
        /// Gets or sets the RGB components of this <see cref="Color"/> as <see cref="Rgb24"/>
        /// </summary>
        public Rgb24 Rgb
        {
            readonly get => UnsafeUtils.As<Color, Rgb24>(this);
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
        /// Constructs the <see cref="Color"/> with raw values.
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
        /// Constructs the <see cref="Color"/> with a packed value.
        /// R in the least significant byte.
        /// </summary>
        [CLSCompliant(false)]
        public Color(uint packed) : this()
        {
            PackedValue = packed;
        }


        /// <summary>
        /// Constructs the <see cref="Color"/> with clamped raw values. 
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
        /// Constructs the <see cref="Color"/> with clamped raw values. 
        /// Alpha value will be opaque.
        /// <para>
        /// The values are clamped between <see cref="byte.MinValue"/> and <see cref="byte.MaxValue"/>.
        /// </para>
        /// </summary>
        /// <param name="r">Red component value from 0 to 255.</param>
        /// <param name="g">Green component value from 0 to 255.</param>
        /// <param name="b">Blue component value from 0 to 255.</param>
        public Color(int r, int g, int b) : this(r, g, b, byte.MaxValue)
        {
        }

        /// <summary>
        /// Constructs the <see cref="Color"/> from vector form values.
        /// </summary>
        /// <param name="r">Red component value from 0 to 1.</param>
        /// <param name="g">Green component value from 0 to 1.</param>
        /// <param name="b">Blue component value from 0 to 1.</param>
        /// <param name="alpha">Alpha component value from 0 to 1.</param>
        public Color(float r, float g, float b, float alpha) : this(
            (int)(r * byte.MaxValue),
            (int)(g * byte.MaxValue),
            (int)(b * byte.MaxValue),
            (int)(alpha * byte.MaxValue))
        {
        }

        /// <summary>
        /// Constructs the <see cref="Color"/> with vector form values.
        /// </summary>
        /// <param name="vector"><see cref="Vector4"/> containing the components.</param>
        public Color(Vector4 color) : this(color.X, color.Y, color.Z, color.W)
        {
        }

        /// <summary>
        /// Constructs the <see cref="Color"/> with vector form values. 
        /// Alpha value will be opaque.
        /// </summary>
        /// <param name="r">Red component value from 0 to 1.</param>
        /// <param name="g">Green component value from 0 to 1.</param>
        /// <param name="b">Blue component value from 0 to 1.</param>
        public Color(float r, float g, float b) : this(r, g, b, 1f)
        {
        }

        /// <summary>
        /// Constructs the <see cref="Color"/> with vector form values.
        /// Alpha value will be opaque.
        /// <para>The values are clamped between 0 and 1.</para>
        /// </summary>
        /// <param name="color"><see cref="Vector3"/> containing the components.</param>
        public Color(Vector3 color) : this(color.X, color.Y, color.Z)
        {
        }

        /// <summary>
        /// Constructs <see cref="Color"/> from a <see cref="Color"/> and alpha value.
        /// </summary>
        /// <param name="color">The RGB values.</param>
        /// <param name="alpha">The alpha component value from 0 to 255.</param>
        public Color(Color color, byte alpha) : this(color.R, color.G, color.B, alpha)
        {
        }

        /// <summary>
        /// Constructs the <see cref="Color"/> from a <see cref="Color"/> and alpha value.
        /// </summary>
        /// <param name="color">The RGB values.</param>
        /// <param name="alpha">Alpha component value from 0 to 1.</param>
        public Color(Color color, float alpha) :
            this(color, MathHelper.Clamp((int)(alpha * byte.MaxValue), byte.MinValue, byte.MaxValue))
        {
        }

        /// <summary>
        /// Constructs <see cref="Color"/> from an <see cref="Rgb24"/> and alpha value.
        /// </summary>
        /// <param name="color">The RGB values.</param>
        /// <param name="alpha">The alpha component value from 0 to 255.</param>
        public Color(Rgb24 color, byte alpha) : this(color.R, color.G, color.B, alpha)
        {
        }

        /// <summary>
        /// Constructs <see cref="Rgb24"/> from an <see cref="Rgb24"/> and alpha value.
        /// </summary>
        /// <param name="color">The RGB values.</param>
        /// <param name="alpha">Alpha component value from 0 to 1.</param>
        public Color(Rgb24 color, int alpha) :
            this(color, MathHelper.Clamp(alpha, byte.MinValue, byte.MaxValue))
        {
        }

        /// <summary>
        /// Constructs <see cref="Rgb24"/> from an <see cref="Rgb24"/> and alpha value.
        /// </summary>
        /// <param name="color">The RGB values.</param>
        /// <param name="alpha">Alpha component value from 0 to 1.</param>
        public Color(Rgb24 color, float alpha) : this(color, (int)(alpha * byte.MaxValue))
        {
        }

        #endregion

        #region IPackedVector

        [IgnoreDataMember]
        [CLSCompliant(false)]
        public uint PackedValue
        {
            readonly get => UnsafeUtils.As<Color, uint>(this);
            set => Unsafe.As<Color, uint>(ref this) = value;
        }

        public readonly void ToScaledVector4(out Vector4 scaledVector)
        {
            scaledVector.Base.X = R;
            scaledVector.Base.Y = G;
            scaledVector.Base.Z = B;
            scaledVector.Base.W = A;
            scaledVector /= byte.MaxValue;
        }

        public void FromScaledVector4(in Vector4 scaledVector)
        {
            var v = scaledVector * byte.MaxValue;
            v += Vector4.Half;
            v.Clamp(Vector4.Zero, Vector4.MaxValueByte);

            R = (byte)v.X;
            G = (byte)v.Y;
            B = (byte)v.Z;
            A = (byte)v.W;
        }

        public readonly void ToVector4(out Vector4 vector)
        {
            ToScaledVector4(out vector);
        }

        public void FromVector4(in Vector4 vector)
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

        public void FromColor(Color color) => this = color;

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

        public readonly void ToColor(out Color destination)
        {
            destination = this;
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
        /// Gets the <see cref="Bgra32"/> representation of this <see cref="Color"/>.
        /// </summary>
        public readonly Bgra32 ToBgra32()
        {
            return new Bgra32(R, G, B, A);
        }

        /// <summary>
        /// Gets the <see cref="Argb32"/> representation of this <see cref="Color"/>.
        /// </summary>
        public readonly Argb32 ToArgb32()
        {
            return new Argb32(R, G, B, A);
        }

        /// <summary>
        /// Gets the <see cref="Rgb24"/> representation of this <see cref="Color"/>.
        /// </summary>
        public readonly Rgb24 ToRgb24()
        {
            return new Rgb24(R, G, B);
        }

        /// <summary>
        /// Gets the <see cref="Bgr24"/> representation of this <see cref="Color"/>.
        /// </summary>
        public readonly Bgr24 ToBgr24()
        {
            return new Bgr24(R, G, B);
        }

        /// <summary>
        /// Gets the <see cref="Gray8"/> representation of this <see cref="Color"/>.
        /// </summary>
        public readonly Gray8 ToGray8()
        {
            return new Gray8(A);
        }

        /// <summary>
        /// Gets the <see cref="GrayAlpha16 "/> representation of this <see cref="Color"/>.
        /// </summary>
        public readonly GrayAlpha16 ToGrayAlpha16()
        {
            return new GrayAlpha16(PackedVectorHelper.Get8BitBT709Luminance(R, G, B), A);
        }

        /// <summary>
        /// Gets the <see cref="Vector3"/> representation of this <see cref="Color"/>.
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
        /// <see cref="Color"/> that contains premultiplied alpha.
        /// </summary>
        /// <param name="vector">A <see cref="Vector4"/> representing color.</param>
        /// <returns>A <see cref="Color"/> which contains premultiplied alpha data.</returns>
        public static Color FromNonPremultiplied(in Vector4 vector)
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
            return value * scale;
        }

        /// <summary>
        /// Multiply <see cref="Color"/> by a scalar.
        /// </summary>
        /// <param name="value">Source <see cref="Color"/>.</param>
        /// <param name="scale">Factor.</param>
        /// <returns>Multiplication result.</returns>
        public static Color operator *(in Color value, float scale)
        {
            return new Color(
                (int)(value.R * scale),
                (int)(value.G * scale),
                (int)(value.B * scale),
                (int)(value.A * scale));
        }

        /// <summary>
        /// Gets the hexadecimal <see cref="string"/> representation of this <see cref="Color"/>.
        /// </summary>
        public string ToHex()
        {
            uint hexOrder = (uint)(A << 0 | B << 8 | G << 16 | R << 24);
            return hexOrder.ToString("x8");
        }

        #region Deconstruct

        /// <summary>
        /// Deconstruction method for <see cref="Color"/>.
        /// </summary>
        /// <param name="r">Red component value from 0 to 255.</param>
        /// <param name="g">Green component value from 0 to 255.</param>
        /// <param name="b">Blue component value from 0 to 255.</param>
        public void Deconstruct(out byte r, out byte g, out byte b)
        {
            r = R;
            g = G;
            b = B;
        }

        /// <summary>
        /// Deconstruction method for <see cref="Color"/>.
        /// </summary>
        /// <param name="r">Red component value from 0.0f to 1.0f.</param>
        /// <param name="g">Green component value from 0.0f to 1.0f.</param>
        /// <param name="b">Blue component value from 0.0f to 1.0f.</param>
        public void Deconstruct(out float r, out float g, out float b)
        {
            r = R / 255f;
            g = G / 255f;
            b = B / 255f;
        }

        /// <summary>
        /// Deconstruction method for <see cref="Color"/> with Alpha.
        /// </summary>
        /// <param name="r">Red component value from 0 to 255.</param>
        /// <param name="g">Green component value from 0 to 255.</param>
        /// <param name="b">Blue component value from 0 to 255.</param>
        /// <param name="a">Alpha component value from 0 to 255.</param>
        public void Deconstruct(out byte r, out byte g, out byte b, out byte a)
        {
            r = R;
            g = G;
            b = B;
            a = A;
        }

        /// <summary>
        /// Deconstruction method for <see cref="Color"/> with Alpha.
        /// </summary>
        /// <param name="r">Red component value from 0.0f to 1.0f.</param>
        /// <param name="g">Green component value from 0.0f to 1.0f.</param>
        /// <param name="b">Blue component value from 0.0f to 1.0f.</param>
        /// <param name="a">Alpha component value from 0.0f to 1.0f.</param>
        public void Deconstruct(out float r, out float g, out float b, out float a)
        {
            r = R / 255f;
            g = G / 255f;
            b = B / 255f;
            a = A / 255f;
        }

        #endregion

        #region Equals

        /// <summary>
        /// Compares whether two <see cref="Color"/> instances are equal.
        /// </summary>
        public static bool operator ==(in Color a, in Color b)
        {
            return a.PackedValue == b.PackedValue;
        }

        /// <summary>
        /// Compares whether two <see cref="Color"/> instances are not equal.
        /// </summary>
        public static bool operator !=(in Color a, in Color b)
        {
            return a.PackedValue != b.PackedValue;
        }

        /// <summary>
        /// Compares whether current instance is equal to specified <see cref="Color"/>.
        /// </summary>
        public bool Equals(Color other)
        {
            return this == other;
        }

        /// <summary>
        /// Compares whether current instance is equal to specified object.
        /// </summary>
        public override bool Equals(object obj)
        {
            return obj is Color other && Equals(other);
        }

        #endregion

        #region Object Overrides

        /// <summary>
        /// Returns a <see cref="string"/> representation of this <see cref="Color"/>.
        /// </summary>
        public override string ToString()
        {
            var sb = new StringBuilder(28);
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
        /// Gets a hash code of the <see cref="Color"/>.
        /// </summary>
        public override int GetHashCode() => PackedValue.GetHashCode();

        #endregion
    }
}
