// Copyright (c) Six Labors and contributors.
// Licensed under the Apache License, Version 2.0.

using System;
using MonoGame.Framework;
using System.Runtime.InteropServices;

namespace MonoGame.Utilities.PackedVector
{
    /// <summary>
    /// Unpacked pixel type containing four 32-bit floating-point values typically ranging from 0 to 1.
    /// The color components are stored in red, green, blue, and alpha order.
    /// <para>
    /// Ranges from [0, 0, 0, 0] to [1, 1, 1, 1] in vector form.
    /// </para>
    /// </summary>
    /// <remarks>
    /// This struct is fully mutable. This is done (against the guidelines) for the sake of performance,
    /// as it avoids the need to create new values for modification operations.
    /// </remarks>
    [StructLayout(LayoutKind.Sequential)]
    public struct RgbaVector : IPixel
    {
        /// <summary>
        /// Gets or sets the red component.
        /// </summary>
        public float R;

        /// <summary>
        /// Gets or sets the green component.
        /// </summary>
        public float G;

        /// <summary>
        /// Gets or sets the blue component.
        /// </summary>
        public float B;

        /// <summary>
        /// Gets or sets the alpha component.
        /// </summary>
        public float A;

        /// <summary>
        /// Initializes a new instance of the <see cref="RgbaVector"/> struct.
        /// </summary>
        /// <param name="r">The red component.</param>
        /// <param name="g">The green component.</param>
        /// <param name="b">The blue component.</param>
        /// <param name="a">The alpha component.</param>
        public RgbaVector(float r, float g, float b, float a = 1)
        {
            R = r;
            G = g;
            B = b;
            A = a;
        }

        /// <summary>
        /// Compares two <see cref="RgbaVector"/> objects for equality.
        /// </summary>
        /// <param name="left">The <see cref="RgbaVector"/> on the left side of the operand.</param>
        /// <param name="right">The <see cref="RgbaVector"/> on the right side of the operand.</param>
        /// <returns>
        /// True if the <paramref name="left"/> parameter is equal to the <paramref name="right"/> parameter; otherwise, false.
        /// </returns>
        public static bool operator ==(RgbaVector left, RgbaVector right) => left.Equals(right);

        /// <summary>
        /// Compares two <see cref="RgbaVector"/> objects for equality.
        /// </summary>
        /// <param name="left">The <see cref="RgbaVector"/> on the left side of the operand.</param>
        /// <param name="right">The <see cref="RgbaVector"/> on the right side of the operand.</param>
        /// <returns>
        /// True if the <paramref name="left"/> parameter is not equal to the <paramref name="right"/> parameter; otherwise, false.
        /// </returns>
        public static bool operator !=(RgbaVector left, RgbaVector right) => !left.Equals(right);

        /// <inheritdoc/>
        public void FromScaledVector4(Vector4 vector) => FromVector4(vector);

        /// <inheritdoc/>
        public Vector4 ToScaledVector4() => ToVector4();

        /// <inheritdoc/>
        public void FromVector4(Vector4 vector)
        {
            vector = Vector4.Clamp(vector, Vector4.Zero, Vector4.One);
            R = vector.X;
            G = vector.Y;
            B = vector.Z;
            A = vector.W;
        }

        /// <inheritdoc/>
        public Vector4 ToVector4() => new Vector4(R, G, B, A);

        /// <inheritdoc/>
        public void FromArgb32(Argb32 source) => FromScaledVector4(source.ToScaledVector4());

        /// <inheritdoc />
        public void FromBgr24(Bgr24 source) => FromScaledVector4(source.ToScaledVector4());

        /// <inheritdoc/>
        public void FromBgra32(Bgra32 source) => FromScaledVector4(source.ToScaledVector4());

        /// <inheritdoc/>
        public void FromBgra5551(Bgra5551 source) => FromScaledVector4(source.ToScaledVector4());

        /// <inheritdoc />
        public void FromGray8(Gray8 source) => FromScaledVector4(source.ToScaledVector4());

        /// <inheritdoc />
        public void FromGray16(Gray16 source) => FromScaledVector4(source.ToScaledVector4());

        /// <inheritdoc />
        public void FromRgb24(Rgb24 source) => FromScaledVector4(source.ToScaledVector4());

        /// <inheritdoc/>
        public void FromColor(Color source) => FromScaledVector4(source.ToScaledVector4());

        /// <inheritdoc/>
        public void FromRgb48(Rgb48 source) => FromScaledVector4(source.ToScaledVector4());

        /// <inheritdoc/>
        public void FromRgba64(Rgba64 source) => FromScaledVector4(source.ToScaledVector4());

        /// <inheritdoc />
        public void ToColor(ref Color dest) => dest.FromScaledVector4(ToScaledVector4());

        /// <summary>
        /// Converts the value of this instance to a hexadecimal string.
        /// </summary>
        /// <returns>A hexadecimal string representation of the value.</returns>
        public string ToHex()
        {
            // Hex is RRGGBBAA
            Vector4 vector = ToVector4() * PackedVectorHelper.MaxBytes;
            vector += PackedVectorHelper.Half;
            uint hexOrder = (uint)((byte)vector.W | (byte)vector.Z << 8 | (byte)vector.Y << 16 | (byte)vector.X << 24);
            return hexOrder.ToString("X8");
        }

        /// <inheritdoc/>
        public override bool Equals(object obj) => obj is RgbaVector other && Equals(other);

        /// <inheritdoc/>
        public bool Equals(RgbaVector other)
        {
            return R.Equals(other.R)
                && G.Equals(other.G)
                && B.Equals(other.B)
                && A.Equals(other.A);
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return FormattableString.Invariant($"RgbaVector({R:#0.##}, {G:#0.##}, {B:#0.##}, {A:#0.##})");
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 17;
                hash = hash * 23 + R.GetHashCode();
                hash = hash * 23 + G.GetHashCode();
                hash = hash * 23 + B.GetHashCode();
                hash = hash * 23 + A.GetHashCode();
                return hash;
            }
        }
    }
}
