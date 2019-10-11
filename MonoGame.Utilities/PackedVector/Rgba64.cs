// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.Runtime.CompilerServices;
using MonoGame.Framework;

namespace MonoGame.Utilities.PackedVector
{
    /// <summary>
    /// Packed pixel type containing four unsigned 16-bit XYZW components.
    /// <para>
    /// Ranges from [0, 0, 0, 0] to [1, 1, 1, 1] in vector form.
    /// </para>
    /// </summary>
    public struct Rgba64 : IPackedVector<ulong>, IEquatable<Rgba64>, IPixel
    {
        private static readonly Vector4 Max = new Vector4(ushort.MaxValue);

        [CLSCompliant(false)]
        public ushort R;

        [CLSCompliant(false)]
        public ushort G;

        [CLSCompliant(false)]
        public ushort B;

        [CLSCompliant(false)]
        public ushort A;

        #region Constructors

        [CLSCompliant(false)]
        public Rgba64(ushort r, ushort g, ushort b, ushort a)
        {
            R = r;
            G = g;
            B = b;
            A = a;
        }

        public Rgba64(Vector4 vector) => this = Pack(ref vector);

        public Rgba64(float x, float y, float z, float w) : this(new Vector4(x, y, z, w))
        {
        }

        #endregion

        private static Rgba64 Pack(ref Vector4 vector)
        {
            vector *= ushort.MaxValue;
            vector = Vector4.Clamp(vector, Vector4.Zero, Max);

            return new Rgba64(
                (ushort)Math.Round(vector.X),
                (ushort)Math.Round(vector.Y),
                (ushort)Math.Round(vector.Z),
                (ushort)Math.Round(vector.W));
        }

        #region IPackedVector

        /// <inheritdoc/>
        [CLSCompliant(false)]
        public ulong PackedValue
        {
            get => Unsafe.As<Rgba64, ulong>(ref this);
            set => Unsafe.As<Rgba64, ulong>(ref this) = value;
        }

        /// <inheritdoc/>
        public void FromVector4(Vector4 vector) => FromScaledVector4(vector);

        /// <inheritdoc/>
        public Vector4 ToVector4() => ToScaledVector4();

        #endregion

        #region IPixel

        /// <inheritdoc/>
        public void FromScaledVector4(Vector4 vector) => this = Pack(ref vector);

        /// <inheritdoc/>
        public Vector4 ToScaledVector4() => new Vector4(R, G, B, A) / ushort.MaxValue;

        public void ToColor(ref Color destination)
        {
            destination.R = PackedVectorHelper.DownScale16To8Bit(R);
            destination.G = PackedVectorHelper.DownScale16To8Bit(G);
            destination.B = PackedVectorHelper.DownScale16To8Bit(B);
            destination.A = PackedVectorHelper.DownScale16To8Bit(A);
        }

        #endregion

        #region Equals

        public override bool Equals(object obj) => obj is Rgba64 other && Equals(other);
        public bool Equals(Rgba64 other) => this == other;

        public static bool operator ==(in Rgba64 a, in Rgba64 b) => a.PackedValue == b.PackedValue;
        public static bool operator !=(in Rgba64 a, in Rgba64 b) => a.PackedValue != b.PackedValue;

        #endregion

        #region Object Overrides

        /// <summary>
        /// Gets a <see cref="string"/> representation of the packed vector.
        /// </summary>
        public override string ToString() => ToVector4().ToString();

        /// <summary>
        /// Gets a hash code of the packed vector.
        /// </summary>
        public override int GetHashCode() => PackedValue.GetHashCode();

        #endregion
    }
}