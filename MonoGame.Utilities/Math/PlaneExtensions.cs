using System.Numerics;

namespace MonoGame.Framework
{
    public static class PlaneExtensions
    {
        internal static PlaneIntersectionType Intersects(this Plane plane, Vector3 point)
        {
            float distance = Plane.DotCoordinate(plane, point);
            if (distance > 0)
                return PlaneIntersectionType.Front;
            if (distance < 0)
                return PlaneIntersectionType.Back;
            return PlaneIntersectionType.Intersecting;
        }
    }
}
