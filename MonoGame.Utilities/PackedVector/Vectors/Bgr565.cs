// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.Runtime.InteropServices;

namespace MonoGame.Framework.PackedVector
{
    /// <summary>
    /// Packed vector type containing unsigned XYZ components.
    /// The XZ components use 5 bits each, and the Y component uses 6 bits.
    /// <para>
    /// Ranges from [0, 0, 0, 1] to [1, 1, 1, 1] in vector form.
    /// </para>
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct Bgr565 : IPackedVector<ushort>, IEquatable<Bgr565>, IPixel
    {
        VectorComponentInfo IPackedVector.ComponentInfo => new VectorComponentInfo(
            new VectorComponent(VectorComponentType.Blue, 5),
            new VectorComponent(VectorComponentType.Green, 6),
            new VectorComponent(VectorComponentType.Red, 5));

        #region Constructors

        /// <summary>
        /// Constructs the packed vector with a packed value.
        /// </summary>
        /// <param name="alpha">The alpha component.</param>
        [CLSCompliant(false)]
        public Bgr565(ushort packed) => PackedValue = packed;

        /// <summary>
        /// Constructs the packed vector with vector form values.
        /// </summary>
        /// <param name="vector"><see cref="Vector3"/> containing the components.</param>
        public Bgr565(Vector3 vector) => PackedValue = Pack(vector);

        /// <summary>
        /// Constructs the packed vector with vector form values.
        /// </summary>
        public Bgr565(float r, float g, float b) : this(new Vector3(r, g, b))
        {
        }

        #endregion

        private static ushort Pack(in Vector3 vector)
        {
            var v = Vector3.Clamp(vector, Vector3.Zero, Vector3.One);
            v *= 31;

            return (ushort)(
                (((int)MathF.Round(v.X) & 0x1F) << 11) |
                (((int)MathF.Round(v.Y * 2.032258f) & 0x3F) << 5) |
                ((int)MathF.Round(v.Z) & 0x1F));
        }

        #region IPackedVector

        [CLSCompliant(false)]
        public ushort PackedValue { get; set; }

        public void FromVector4(in Vector4 vector)
        {
            Pack(vector.ToVector3());
        }

        public readonly void ToVector4(out Vector4 vector)
        {
            vector.Base.X = (PackedValue >> 11) & 0x1F;
            vector.Base.Y = ((PackedValue >> 5) & 0x3F) / 2.032258f;
            vector.Base.Z = PackedValue & 0x1F;
            vector.Base.W = 31f;
            vector /= 31f;
        }

        public void FromScaledVector4(in Vector4 scaledVector)
        {
            FromVector4(scaledVector);
        }

        public readonly void ToScaledVector4(out Vector4 scaledVector)
        {
            ToVector4(out scaledVector);
        }

        #endregion

        #region IPixel

        public void FromColor(Color source)
        {
            source.ToScaledVector4(out var vector);
            FromScaledVector4(vector);
        }

        public void FromGray8(Gray8 source)
        {
            source.ToScaledVector4(out var vector);
            FromScaledVector4(vector);
        }

        public void FromGray16(Gray16 source)
        {
            source.ToScaledVector4(out var vector);
            FromScaledVector4(vector);
        }

        public void FromGrayAlpha16(GrayAlpha16 source)
        {
            source.ToScaledVector4(out var vector);
            FromScaledVector4(vector);
        }

        public void FromRgb24(Rgb24 source)
        {
            source.ToScaledVector4(out var vector);
            FromScaledVector4(vector);
        }

        public void FromRgb48(Rgb48 source)
        {
            source.ToScaledVector4(out var vector);
            FromScaledVector4(vector);
        }

        public void FromRgba64(Rgba64 source)
        {
            source.ToScaledVector4(out var vector);
            FromScaledVector4(vector);
        }

        public readonly void ToColor(ref Color destination)
        {
            ToScaledVector4(out var vector);
            destination.FromScaledVector4(vector);
        }

        #endregion

        #region Equals

        public static bool operator ==(Bgr565 a, Bgr565 b) => a.PackedValue == b.PackedValue;
        public static bool operator !=(Bgr565 a, Bgr565 b) => a.PackedValue != b.PackedValue;

        public bool Equals(Bgr565 other) => this == other;
        public override bool Equals(object obj) => obj is Bgr565 other && Equals(other);

        #endregion

        #region Object Overrides

        /// <summary>
        /// Gets a string representation of the packed vector.
        /// </summary>
        public override string ToString() => nameof(Bgr565) + $"({this.ToVector4().XYZ})";

        /// <summary>
        /// Gets a hash code of the packed vector.
        /// </summary>
        public override int GetHashCode() => PackedValue.GetHashCode();

        #endregion
    }
}
