// MIT License - Copyright (C) The Mono.Xna Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace MonoGame.Framework
{
	internal class PlaneHelper
    {
        /// <summary>
        /// Returns a value indicating what side (positive/negative) of a plane a point is
        /// </summary>
        /// <param name="point">The point to check with</param>
        /// <param name="plane">The plane to check against</param>
        /// <returns>Greater than zero if on the positive side, less than zero if on the negative size, 0 otherwise</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float ClassifyPoint(Vector3 point, in Plane plane)
        {
            return point.X * plane.Normal.X + point.Y * plane.Normal.Y + point.Z * plane.Normal.Z + plane.D;
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
        //    return (float)Math.Abs((plane.Normal.X * point.X + plane.Normal.Y * point.Y + plane.Normal.Z * point.Z)
        //                            / Math.Sqrt(plane.Normal.X * plane.Normal.X + plane.Normal.Y * plane.Normal.Y + plane.Normal.Z * plane.Normal.Z));
        //}
    }
	
    [DataContract]
    [DebuggerDisplay("{DebugDisplayString,nq}")]
    public struct Plane : IEquatable<Plane>
    {
        [DataMember]
        public float D;

        [DataMember]
        public Vector3 Normal;

        public Plane(Vector3 normal, float d)
        {
            Normal = normal;
            D = d;
        }

        public Plane(in Vector4 value) : this(new Vector3(value.X, value.Y, value.Z), value.W)
        {

        }

        public Plane(Vector3 a, Vector3 b, Vector3 c)
        {
            Vector3 ab = b - a;
            Vector3 ac = c - a;

            Vector3 cross = Vector3.Cross(ab, ac);
            Normal = Vector3.Normalize(cross);
            D = -Vector3.Dot(Normal, a);
        }

        public Plane(float a, float b, float c, float d) : this(new Vector3(a, b, c), d)
        {
        }
        
        public float Dot(in Vector4 value)
        {
            return ((((Normal.X * value.X) + (Normal.Y * value.Y)) + (Normal.Z * value.Z)) + (D * value.W));
        }

        public float DotCoordinate(Vector3 value)
        {
            return ((((Normal.X * value.X) + (Normal.Y * value.Y)) + (Normal.Z * value.Z)) + D);
        }

        public float DotNormal(Vector3 value)
        {
            return (((Normal.X * value.X) + (Normal.Y * value.Y)) + (Normal.Z * value.Z));
        }

        /// <summary>
        /// Transforms a normalized plane by a matrix.
        /// </summary>
        /// <param name="plane">The normalized plane to transform.</param>
        /// <param name="matrix">The transformation matrix.</param>
        /// <returns>The transformed plane.</returns>
        public static Plane Transform(in Plane plane, in Matrix matrix)
        {
            // See "Transforming Normals" in http://www.glprogramming.com/red/appendixf.html
            // for an explanation of how this works.

            Matrix transformedMatrix = Matrix.Invert(matrix);
            transformedMatrix = Matrix.Transpose(transformedMatrix);

            var vector = new Vector4(plane.Normal, plane.D);
            var transformedVector = Vector4.Transform(vector, transformedMatrix);
            return new Plane(transformedVector);
        }

        /// <summary>
        /// Transforms a normalized plane by a quaternion rotation.
        /// </summary>
        /// <param name="plane">The normalized plane to transform.</param>
        /// <param name="rotation">The quaternion rotation.</param>
        /// <returns>The transformed plane.</returns>
        public static Plane Transform(in Plane plane, in Quaternion rotation)
        {
            return new Plane(Vector3.Transform(plane.Normal, rotation), plane.D);
        }

        public void Normalize()
        {
            float length = Normal.Length();
            float factor =  1f / length;
            Normal = Vector3.Multiply(Normal, factor);
            D *= factor;
        }

        public static Plane Normalize(in Plane value)
        {
            float length = value.Normal.Length();
            float factor =  1f / length;            
            return new Plane(Vector3.Multiply(value.Normal, factor), value.D * factor);
        }

        public static bool operator !=(in Plane plane1, in Plane plane2)
        {
            return !plane1.Equals(plane2);
        }

        public static bool operator ==(in Plane plane1, in Plane plane2)
        {
            return plane1.Normal == plane2.Normal && plane1.D == plane2.D;
        }

        public override bool Equals(object obj)
        {
            return (obj is Plane other) ? Equals(other) : false;
        }

        public bool Equals(Plane other)
        {
            return this == other;
        }

        public override int GetHashCode()
        {
            return Normal.GetHashCode() ^ D.GetHashCode();
        }

        internal PlaneIntersectionType Intersects(Vector3 point)
        {
            float distance = DotCoordinate(point);
            if (distance > 0)
                return PlaneIntersectionType.Front;
            if (distance < 0)
                return PlaneIntersectionType.Back;
            return PlaneIntersectionType.Intersecting;
        }

        internal string DebugDisplayString => string.Concat(Normal.DebugDisplayString, "  ", D.ToString());

        public override string ToString()
        {
            return "{Normal:" + Normal + " D:" + D + "}";
        }

        /// <summary>
        /// Deconstruction method for <see cref="Plane"/>.
        /// </summary>
        /// <param name="normal"></param>
        /// <param name="d"></param>
        public void Deconstruct(out Vector3 normal, out float d)
        {
            normal = Normal;
            d = D;
        }
    }
}