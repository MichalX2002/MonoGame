// MIT License - Copyright (C) The Mono.Xna Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace MonoGame.Framework
{
    [DataContract]
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    public struct Ray : IEquatable<Ray>
    {
        [DataMember]
        public Vector3 Direction;
      
        [DataMember]
        public Vector3 Position;

        public Ray(in Vector3 position, in Vector3 direction)
        {
            Position = position;
            Direction = direction;
        }

        #region Public Methods

        public override bool Equals(object obj)
        {
            return (obj is Ray other) ? Equals(other) : false;
        }

        public bool Equals(Ray other)
        {
            return this == other;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int code = Direction.GetHashCode();
                code = code * 23 + Position.GetHashCode();
                return code;
            }
        }

        // adapted from
        // http://www.scratchapixel.com/lessons/3d-basic-lessons/lesson-7-intersecting-simple-shapes/ray-box-intersection/
        public bool Intersects(in BoundingBox box, out float distance)
        {
            const float Epsilon = 1e-6f;
            distance = 0;

            float? tMin = null, tMax = null;

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

                if (!tMin.HasValue || tMinY > tMin) tMin = tMinY;
                if (!tMax.HasValue || tMaxY < tMax) tMax = tMaxY;
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

                if (!tMin.HasValue || tMinZ > tMin) tMin = tMinZ;
                if (!tMax.HasValue || tMaxZ < tMax) tMax = tMaxZ;
            }

            // having a positive tMin and a negative tMax means the ray is inside the box
            // we expect the intesection distance to be 0 in that case
            if (tMin.HasValue && tMin < 0 && tMax > 0)
                return true;

            // a negative tMin means that the intersection point is behind the ray's origin
            // we discard these as not hitting the AABB
            if (tMin < 0)
                return false;

            if(tMin.HasValue)
            {
                distance = tMin.Value;
                return true;
            }
            return false;
        }

        public bool Intersects(BoundingFrustum frustum, out float distance)
        {
            if (frustum == null)
				throw new ArgumentNullException(nameof(frustum));
			return frustum.Intersects(this, out distance);
        }

        public bool Intersects(in BoundingSphere sphere, out float distance)
        {
            // Find the vector between where the ray starts the the sphere's centre
            Vector3 difference = sphere.Center - Position;

            float differenceLengthSquared = difference.LengthSquared();
            float sphereRadiusSquared = sphere.Radius * sphere.Radius;
            distance = 0;

            // If the distance between the ray start and the sphere's centre is less than
            // the radius of the sphere, it means we've intersected. N.B. checking the LengthSquared is faster.
            if (differenceLengthSquared < sphereRadiusSquared)
                return true;
            
        
            float distanceAlongRay = Vector3.Dot(Direction, difference);
            // If the ray is pointing away from the sphere then we don't ever intersect
            if (distanceAlongRay < 0)
                return false;

            // Next we kinda use Pythagoras to check if we are within the bounds of the sphere
            // if x = radius of sphere
            // if y = distance between ray position and sphere centre
            // if z = the distance we've travelled along the ray
            // if x^2 + z^2 - y^2 < 0, we do not intersect
            float dist = sphereRadiusSquared + distanceAlongRay * distanceAlongRay - differenceLengthSquared;
            if (dist < 0)
                return false;

            distance = distanceAlongRay - (float)Math.Sqrt(dist);
            return true;
        }

        public bool Intersects(in Plane plane, out float distance)
        {
            float dot = Vector3.Dot(Direction, plane.Normal);
            if (Math.Abs(dot) < 0.00001f)
            {
                distance = 0;
                return false;
            }

            distance = (-plane.D - Vector3.Dot(plane.Normal, Position)) / dot;
            if (distance < 0f)
            {
                if (distance < -0.00001f)
                    return false;
                distance = 0f;
            }
            return true;
        }

        public static bool operator !=(in Ray a, in Ray b)
        {
            return !a.Equals(b);
        }

        public static bool operator ==(in Ray a, in Ray b)
        {
            return a.Position.Equals(b.Position)
                && a.Direction.Equals(b.Direction);
        }

        private string DebuggerDisplay => string.Concat(
            "Pos(", Position.DebuggerDisplay, ") \n",
            "Dir(", Direction.DebuggerDisplay, ")");

        public override string ToString()
        {
            return "{Position:" + Position.ToString() + " Direction:" + Direction.ToString() + "}";
        }

        /// <summary>
        /// Deconstruction method for <see cref="Ray"/>.
        /// </summary>
        /// <param name="position">Receives the start position of the ray.</param>
        /// <param name="direction">Receives the direction of the ray.</param>
        public void Deconstruct(out Vector3 position, out Vector3 direction)
        {
            position = Position;
            direction = Direction;
        }

        #endregion
    }
}
