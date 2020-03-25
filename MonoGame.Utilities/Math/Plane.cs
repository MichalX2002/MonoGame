// MIT License - Copyright (C) The Mono.Xna Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.Diagnostics;
using System.Runtime.Serialization;
using FastPlane = System.Numerics.Plane;

namespace MonoGame.Framework
{
    [DataContract]
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    public struct Plane : IEquatable<Plane>
    {
        [IgnoreDataMember]
        public FastPlane Base;

        [DataMember]
        public Vector3 Normal { readonly get => Base.Normal; set => Base.Normal = value; }

        [DataMember]
        public float D { readonly get => Base.D; set => Base.D = value; }

        internal string DebuggerDisplay => 
            string.Concat(Normal.DebuggerDisplay, "  ", D.ToString());

        #region Constructors

        public Plane(in Vector4 value)
        {
            Base = new FastPlane(value);
        }

        public Plane(in Vector3 normal, float d)
        {
            Base = new FastPlane(normal, d);
        }

        public Plane(float x, float y, float z, float d)
        {
            Base = new FastPlane(x, y, z, d);
        }

        #endregion

        public readonly Vector4 ToVector4()
        {
            return UnsafeUtils.As<Plane, Vector4>(this);
        }

        #region CreateFromVertices

        public static Plane CreateFromVertices(in Vector3 a, in Vector3 b, in Vector3 c)
        {
            return FastPlane.CreateFromVertices(a, b, c);
        }

        #endregion

        public static float Dot(in Plane plane, in Vector4 value)
        {
            return FastPlane.Dot(plane, value);
        }

        public static float DotNormal(in Plane plane, in Vector3 value)
        {
            return FastPlane.DotNormal(plane, value);
        }

        public static float DotCoordinate(in Plane plane, in Vector3 value)
        {
            return FastPlane.DotCoordinate(plane, value);
        }

        /// <summary>
        /// Transforms a normalized plane by a matrix.
        /// </summary>
        /// <param name="plane">The normalized plane to transform.</param>
        /// <param name="matrix">The transformation matrix.</param>
        /// <returns>The transformed plane.</returns>
        public static Plane Transform(in Plane plane, in Matrix matrix)
        {
            return FastPlane.Transform(plane, matrix);
        }

        /// <summary>
        /// Transforms a normalized plane by a quaternion rotation.
        /// </summary>
        /// <param name="plane">The normalized plane to transform.</param>
        /// <param name="rotation">The quaternion rotation.</param>
        /// <returns>The transformed plane.</returns>
        public static Plane Transform(in Plane plane, in Quaternion rotation)
        {
            return FastPlane.Transform(plane, rotation);
        }

        #region Normalize

        public static Plane Normalize(in Plane value)
        {
            return FastPlane.Normalize(value);
        }

        public void Normalize()
        {
            this = Normalize(this);
        }

        #endregion

        #region Equals

        public override bool Equals(object obj)
        {
            return obj is Plane other && this == other;
        }

        public bool Equals(Plane other)
        {
            return Base.Equals(other.Base);
        }

        public static bool operator ==(in Plane plane1, in Plane plane2)
        {
            return plane1.Base == plane2.Base;
        }

        public static bool operator !=(in Plane plane1, in Plane plane2)
        {
            return plane1.Base != plane2.Base;
        }

        #endregion

        public override int GetHashCode() => Base.GetHashCode();

        internal PlaneIntersectionType Intersects(in Vector3 point)
        {
            float distance = DotCoordinate(this, point);
            if (distance > 0)
                return PlaneIntersectionType.Front;
            if (distance < 0)
                return PlaneIntersectionType.Back;
            return PlaneIntersectionType.Intersecting;
        }

        public readonly override string ToString() => Base.ToString();

        /// <summary>
        /// Deconstruction method for <see cref="Plane"/>.
        /// </summary>
        public readonly void Deconstruct(out Vector3 normal, out float d)
        {
            normal = Normal;
            d = D;
        }

        /// <summary>
        /// Returns a value indicating what side (positive/negative) of a plane a point is
        /// </summary>
        /// <param name="point">The point to check with</param>
        /// <param name="plane">The plane to check against</param>
        /// <returns>
        /// Greater than zero if on the positive side,
        /// less than zero if on the negative size, 
        /// zero otherwise.
        /// </returns>
        public static float ClassifyPoint(in Vector3 point, in Plane plane)
        {
            return Vector3.Dot(point, plane.Normal) + plane.D;
        }

        ///// <summary>
        ///// Returns the perpendicular distance from a point to a plane
        ///// </summary>
        ///// <param name="point">The point to check</param>
        ///// <param name="plane">The place to check</param>
        ///// <returns>The perpendicular distance from the point to the plane</returns>
        //public static float PerpendicularDistance(ref Vector3 point, ref Plane plane)
        //{
        //    // dist = (ax + by + cz + d) / sqrt(a*a + b*b + c*c)
        //    return Math.Abs((plane.Normal.X * point.X + plane.Normal.Y * point.Y + plane.Normal.Z * point.Z) 
        //        / MathF.Sqrt(plane.Normal.X * plane.Normal.X + plane.Normal.Y * plane.Normal.Y + plane.Normal.Z * plane.Normal.Z));
        //}

        public static implicit operator FastPlane(in Plane plane)
        {
            return plane.Base;
        }

        public static implicit operator Plane(in FastPlane plane)
        {
            return new Plane { Base = plane };
        }
    }
}