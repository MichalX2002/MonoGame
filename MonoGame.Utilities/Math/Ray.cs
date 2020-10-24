// MIT License - Copyright (C) The Mono.Xna Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.Diagnostics;
using System.Numerics;
using System.Runtime.Serialization;

namespace MonoGame.Framework
{
    /// <summary>
    /// Represents a ray with an origin and a direction in 3D space.
    /// </summary>
    [DataContract]
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    public struct Ray : IEquatable<Ray>
    {
        /// <summary>
        /// The direction of this <see cref="Ray"/>.
        /// </summary>
        [DataMember]
        public Vector3 Direction;

        /// <summary>
        /// The origin of this <see cref="Ray"/>.
        /// </summary>
        [DataMember]
        public Vector3 Position;

        internal string DebuggerDisplay => string.Concat(
            "Position = ", Position.ToString(), ", ",
            "Direction = ", Direction.ToString());

        /// <summary>
        /// Create a <see cref="Ray"/>.
        /// </summary>
        /// <param name="position">The origin of the <see cref="Ray"/>.</param>
        /// <param name="direction">The direction of the <see cref="Ray"/>.</param>
        public Ray(Vector3 position, Vector3 direction)
        {
            Position = position;
            Direction = direction;
        }

        #region Public Methods

        // adapted from http://www.scratchapixel.com/lessons/3d-basic-lessons/lesson-7-intersecting-simple-shapes/ray-box-intersection/
        /// <summary>
        /// Check if this <see cref="Ray"/> intersects the specified <see cref="BoundingBox"/>.
        /// </summary>
        /// <param name="box">The <see cref="BoundingBox"/> to test for intersection.</param>
        /// <returns>
        /// The distance along the ray of the intersection or <c>null</c> if this
        /// <see cref="Ray"/> does not intersect the <see cref="BoundingBox"/>.
        /// </returns>
        public readonly bool Intersects(in BoundingBox box, out float distance)
        {
            const float Epsilon = 1e-6f;

            float? tMin = null;
            float? tMax = null;

            distance = 0;

            if (Math.Abs(Direction.X) < Epsilon)
            {
                if (Position.X < box.Min.X || Position.X > box.Max.X)
                    return false;
            }
            else
            {
                tMin = (box.Min.X - Position.X) / Direction.X;
                tMax = (box.Max.X - Position.X) / Direction.X;

                if (tMin > tMax)
                {
                    var temp = tMin;
                    tMin = tMax;
                    tMax = temp;
                }
            }

            if (Math.Abs(Direction.Y) < Epsilon)
            {
                if (Position.Y < box.Min.Y || Position.Y > box.Max.Y)
                    return false;
            }
            else
            {
                var tMinY = (box.Min.Y - Position.Y) / Direction.Y;
                var tMaxY = (box.Max.Y - Position.Y) / Direction.Y;

                if (tMinY > tMaxY)
                {
                    var temp = tMinY;
                    tMinY = tMaxY;
                    tMaxY = temp;
                }

                if ((tMin.HasValue && tMin > tMaxY) || (tMax.HasValue && tMinY > tMax))
                    return false;

                if (!tMin.HasValue || tMinY > tMin)
                    tMin = tMinY;
                if (!tMax.HasValue || tMaxY < tMax)
                    tMax = tMaxY;
            }

            if (Math.Abs(Direction.Z) < Epsilon)
            {
                if (Position.Z < box.Min.Z || Position.Z > box.Max.Z)
                    return false;
            }
            else
            {
                var tMinZ = (box.Min.Z - Position.Z) / Direction.Z;
                var tMaxZ = (box.Max.Z - Position.Z) / Direction.Z;

                if (tMinZ > tMaxZ)
                {
                    var temp = tMinZ;
                    tMinZ = tMaxZ;
                    tMaxZ = temp;
                }

                if ((tMin.HasValue && tMin > tMaxZ) || (tMax.HasValue && tMinZ > tMax))
                    return false;

                if (!tMin.HasValue || tMinZ > tMin)
                    tMin = tMinZ;
                if (!tMax.HasValue || tMaxZ < tMax)
                    tMax = tMaxZ;
            }

            // having a positive tMin and a negative tMax means the ray is inside the box
            // we expect the intesection distance to be 0 in that case
            if ((tMin.HasValue && tMin < 0) && tMax > 0)
                return true;

            distance = tMin.GetValueOrDefault();

            // a negative tMin means that the intersection point is behind the ray's origin
            // we discard these as not hitting the AABB
            if (distance < 0)
                return false;

            return true;
        }

        /// <summary>
        /// Check if this <see cref="Ray"/> intersects the specified <see cref="BoundingBox"/>.
        /// </summary>
        /// <param name="box">The <see cref="BoundingBox"/> to test for intersection.</param>
        /// <param name="distance">
        /// The distance along the ray of the intersection or <c>null</c> if this
        /// <see cref="Ray"/> does not intersect the <see cref="BoundingBox"/>.
        /// </param>
        public readonly float? Intersects(in BoundingBox box)
        {
            if (Intersects(box, out float distance))
                return distance;
            return null;
        }

        /*
        public readonly float? Intersects(BoundingFrustum frustum)
        {
            if (frustum == null)
            {
                throw new ArgumentNullException("frustum");
            }
            
            return frustum.Intersects(this);			
        }
        */

        /// <summary>
        /// Check if this <see cref="Ray"/> intersects the specified <see cref="BoundingSphere"/>.
        /// </summary>
        /// <param name="sphere">The <see cref="BoundingBox"/> to test for intersection.</param>
        /// <returns>
        /// The distance along the ray of the intersection or <c>null</c> if this
        /// <see cref="Ray"/> does not intersect the <see cref="BoundingSphere"/>.
        /// </returns>
        public readonly float? Intersects(in BoundingSphere sphere)
        {
            if (Intersects(sphere, out float distance))
                return distance;
            return null;
        }

        /// <summary>
        /// Check if this <see cref="Ray"/> intersects the specified <see cref="Plane"/>.
        /// </summary>
        /// <param name="plane">The <see cref="Plane"/> to test for intersection.</param>
        /// <returns>
        /// The distance along the ray of the intersection or <c>null</c> if this
        /// <see cref="Ray"/> does not intersect the <see cref="Plane"/>.
        /// </returns>
        public readonly float? Intersects(in Plane plane)
        {
            if (Intersects(plane, out float distance))
                return distance;
            return null;
        }

        /// <summary>
        /// Check if this <see cref="Ray"/> intersects the specified <see cref="Plane"/>.
        /// </summary>
        /// <param name="plane">The <see cref="Plane"/> to test for intersection.</param>
        /// <param name="distance">
        /// The distance along the ray of the intersection or <c>null</c> if this
        /// <see cref="Ray"/> does not intersect the <see cref="Plane"/>.
        /// </param>
        public readonly bool Intersects(in Plane plane, out float distance)
        {
            float den = Vector3.Dot(Direction, plane.Normal);
            if (Math.Abs(den) < 0.00001f)
            {
                distance = 0;
                return false;
            }

            distance = (-plane.D - Vector3.Dot(plane.Normal, Position)) / den;

            if (distance < 0.0f)
            {
                if (distance < -0.00001f)
                {
                    distance = 0;
                    return false;
                }

                distance = 0.0f;
            }

            return true;
        }

        /// <summary>
        /// Check if this <see cref="Ray"/> intersects the specified <see cref="BoundingSphere"/>.
        /// </summary>
        /// <param name="sphere">The <see cref="BoundingBox"/> to test for intersection.</param>
        /// <param name="distance">
        /// The distance along the ray of the intersection or <c>null</c> if this
        /// <see cref="Ray"/> does not intersect the <see cref="BoundingSphere"/>.
        /// </param>
        public readonly bool Intersects(in BoundingSphere sphere, out float distance)
        {
            // Find the vector between where the ray starts the the sphere's centre
            Vector3 diff = sphere.Center - Position;

            float diffLengthSquared = diff.LengthSquared();
            float sphereRadiusSquared = sphere.Radius * sphere.Radius;
            distance = 0;

            // If the distance between the ray start and the sphere's centre is less than
            // the radius of the sphere, it means we've intersected. N.B. checking the LengthSquared is faster.
            if (diffLengthSquared < sphereRadiusSquared)
                return true;

            float distanceAlongRay = Vector3.Dot(Direction, diff);
            // If the ray is pointing away from the sphere then we don't ever intersect
            if (distanceAlongRay < 0)
                return false;

            // Next we kinda use Pythagoras to check if we are within the bounds of the sphere
            // if x = radius of sphere
            // if y = distance between ray position and sphere centre
            // if z = the distance we've travelled along the ray
            // if x^2 + z^2 - y^2 < 0, we do not intersect
            float dist = sphereRadiusSquared + distanceAlongRay * distanceAlongRay - diffLengthSquared;
            if (dist < 0)
                return false;

            distance = distanceAlongRay - MathF.Sqrt(dist);
            return true;
        }

        internal string DebugDisplayString
        {
            get
            {
                return string.Concat(
                    "Pos( ", Position, " )  \n",
                    "Dir( ", Direction, " )"
                );
            }
        }

        /// <summary>
        /// Deconstruction method for <see cref="Ray"/>.
        /// </summary>
        /// <param name="position">Receives the start position of the ray.</param>
        /// <param name="direction">Receives the direction of the ray.</param>
        public readonly void Deconstruct(out Vector3 position, out Vector3 direction)
        {
            position = Position;
            direction = Direction;
        }

        #endregion

        #region Equals

        /// <summary>
        /// Check if the specified <see cref="object"/> is equal to this <see cref="Ray"/>.
        /// </summary>
        /// <param name="obj">The <see cref="object"/> to test for equality with this <see cref="Ray"/>.</param>
        /// <returns>
        /// <c>true</c> if the specified <see cref="object"/> is equal to this <see cref="Ray"/>,
        /// <c>false</c> if it is not.
        /// </returns>
        public override readonly bool Equals(object? obj)
        {
            return obj is Ray other && Equals(other);
        }

        /// <summary>
        /// Check if the specified <see cref="Ray"/> is equal to this <see cref="Ray"/>.
        /// </summary>
        /// <param name="other">The <see cref="Ray"/> to test for equality with this <see cref="Ray"/>.</param>
        /// <returns>
        /// <c>true</c> if the specified <see cref="Ray"/> is equal to this <see cref="Ray"/>,
        /// <c>false</c> if it is not.
        /// </returns>
        public readonly bool Equals(Ray other)
        {
            return this == other;
        }

        /// <summary>
        /// Check if two rays are equal.
        /// </summary>
        /// <param name="a">A ray to check for equality.</param>
        /// <param name="b">A ray to check for equality.</param>
        /// <returns><c>true</c> if the two rays are equals, <c>false</c> if they are not.</returns>
        public static bool operator ==(in Ray a, in Ray b)
        {
            return a.Position == b.Position
                && a.Direction == b.Direction;
        }

        /// <summary>
        /// Check if two rays are not equal.
        /// </summary>
        /// <param name="a">A ray to check for inequality.</param>
        /// <param name="b">A ray to check for inequality.</param>
        /// <returns><c>true</c> if the two rays are not equal, <c>false</c> if they are.</returns>
        public static bool operator !=(in Ray a, in Ray b)
        {
            return !(a == b);
        }

        #endregion

        #region Object overrides

        /// <summary>
        /// Get a <see cref="string"/> representation of this <see cref="Ray"/>.
        /// </summary>
        /// <returns>A <see cref="string"/> representation of this <see cref="Ray"/>.</returns>
        public override readonly string ToString()
        {
            return "{Position:" + Position.ToString() + " Direction:" + Direction.ToString() + "}";
        }

        /// <summary>
        /// Get a hash code for this <see cref="Ray"/>.
        /// </summary>
        /// <returns>A hash code for this <see cref="Ray"/>.</returns>
        public override readonly int GetHashCode()
        {
            return HashCode.Combine(Direction, Position);
        }

        #endregion
    }
}
