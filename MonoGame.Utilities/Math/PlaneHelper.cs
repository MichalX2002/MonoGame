using System;
using System.Numerics;

namespace MonoGame.Framework
{
    public static class PlaneHelper
    {
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
        public static float ClassifyPoint(Vector3 point, Plane plane)
        {
            return Vector3.Dot(point, plane.Normal) + plane.D;
        }
    }
}
