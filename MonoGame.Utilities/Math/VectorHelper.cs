
using System.Numerics;

namespace MonoGame.Framework
{
    public static class VectorHelper
    {
        #region Barycentric

        /// <summary>
        /// Creates a new <see cref="Vector2"/> that contains the cartesian coordinates of 
        /// a vector specified in barycentric coordinates and relative to 2D-triangle.
        /// </summary>
        /// <param name="a">The first vector of 2D-triangle.</param>
        /// <param name="b">The second vector of 2D-triangle.</param>
        /// <param name="c">The third vector of 2D-triangle.</param>
        /// <param name="amount1">Barycentric scalar <c>b2</c> which represents a weighting factor towards second vector of 2D-triangle.</param>
        /// <param name="amount2">Barycentric scalar <c>b3</c> which represents a weighting factor towards third vector of 2D-triangle.</param>
        /// <returns>The cartesian translation of barycentric coordinates.</returns>
        public static Vector2 Barycentric(
            Vector2 a, Vector2 b, Vector2 c, float amount1, float amount2)
        {
            return new Vector2(
                MathHelper.Barycentric(a.X, b.X, c.X, amount1, amount2),
                MathHelper.Barycentric(a.Y, b.Y, c.Y, amount1, amount2));
        }

        /// <summary>
        /// Creates a new <see cref="Vector3"/> that contains the cartesian
        /// coordinates of a vector specified in barycentric coordinates and relative to 3D-triangle.
        /// </summary>
        /// <param name="a">The first vector of 3D-triangle.</param>
        /// <param name="b">The second vector of 3D-triangle.</param>
        /// <param name="c">The third vector of 3D-triangle.</param>
        /// <param name="amount1">Barycentric scalar <c>b2</c> which represents a weighting factor towards second vector of 3D-triangle.</param>
        /// <param name="amount2">Barycentric scalar <c>b3</c> which represents a weighting factor towards third vector of 3D-triangle.</param>
        /// <returns>The cartesian translation of barycentric coordinates.</returns>
        public static Vector3 Barycentric(Vector3 a, Vector3 b, Vector3 c, float amount1, float amount2)
        {
            return new Vector3(
                MathHelper.Barycentric(a.X, b.X, c.X, amount1, amount2),
                MathHelper.Barycentric(a.Y, b.Y, c.Y, amount1, amount2),
                MathHelper.Barycentric(a.Z, b.Z, c.Z, amount1, amount2));
        }

        /// <summary>
        /// Creates a new <see cref="Vector4"/> that contains the cartesian coordinates
        /// of a vector specified in barycentric coordinates and relative to 4D-triangle.
        /// </summary>
        /// <param name="a">The first vector of 4D-triangle.</param>
        /// <param name="b">The second vector of 4D-triangle.</param>
        /// <param name="c">The third vector of 4D-triangle.</param>
        /// <param name="amount1">Barycentric scalar <c>b2</c> which represents a weighting factor towards second vector of 4D-triangle.</param>
        /// <param name="amount2">Barycentric scalar <c>b3</c> which represents a weighting factor towards third vector of 4D-triangle.</param>
        /// <returns>The cartesian translation of barycentric coordinates.</returns>
        public static Vector4 Barycentric(
            Vector4 a, Vector4 b, Vector4 c, float amount1, float amount2)
        {
            return new Vector4(
                MathHelper.Barycentric(a.X, b.X, c.X, amount1, amount2),
                MathHelper.Barycentric(a.Y, b.Y, c.Y, amount1, amount2),
                MathHelper.Barycentric(a.Z, b.Z, c.Z, amount1, amount2),
                MathHelper.Barycentric(a.W, b.W, c.W, amount1, amount2));
        }

        #endregion

        #region CatmullRom

        /// <summary>
        /// Creates a new <see cref="Vector2"/> that contains CatmullRom interpolation of the specified vectors.
        /// </summary>
        /// <param name="a">The first vector in interpolation.</param>
        /// <param name="b">The second vector in interpolation.</param>
        /// <param name="c">The third vector in interpolation.</param>
        /// <param name="d">The fourth vector in interpolation.</param>
        /// <param name="amount">Weighting factor.</param>
        /// <returns>The result of CatmullRom interpolation.</returns>
        public static Vector2 CatmullRom(
            Vector2 a, Vector2 b, Vector2 c, Vector2 d, float amount)
        {
            return new Vector2(
                MathHelper.CatmullRom(a.X, b.X, c.X, d.X, amount),
                MathHelper.CatmullRom(a.Y, b.Y, c.Y, d.Y, amount));
        }

        #endregion

        #region Hermite

        /// <summary>
        /// Creates a new <see cref="Vector2"/> that contains hermite spline interpolation.
        /// </summary>
        /// <param name="position1">The first position vector.</param>
        /// <param name="tangent1">The first tangent vector.</param>
        /// <param name="position2">The second position vector.</param>
        /// <param name="tangent2">The second tangent vector.</param>
        /// <param name="amount">Weighting factor.</param>
        /// <returns>The hermite spline interpolation vector.</returns>
        public static Vector2 Hermite(
            Vector2 position1, Vector2 tangent1,
            Vector2 position2, Vector2 tangent2,
            float amount)
        {
            return new Vector2(
                MathHelper.Hermite(position1.X, tangent1.X, position2.X, tangent2.X, amount),
                MathHelper.Hermite(position1.Y, tangent1.Y, position2.Y, tangent2.Y, amount));
        }

        #endregion

        #region LerpPrecise

        /// <summary>
        /// Creates a new <see cref="Vector2"/> that contains linear interpolation of the specified vectors.
        /// Slightly less efficient but more precise compared to <see cref="Vector2.Lerp"/>.
        /// See remarks section of <see cref="MathHelper.LerpPrecise"/> for more info.
        /// </summary>
        /// <param name="value1">The first vector.</param>
        /// <param name="value2">The second vector.</param>
        /// <param name="amount">Weighting value(between 0.0 and 1.0).</param>
        /// <returns>The result of linear interpolation of the specified vectors.</returns>
        public static Vector2 LerpPrecise(Vector2 value1, Vector2 value2, float amount)
        {
            return new Vector2(
                MathHelper.LerpPrecise(value1.X, value2.X, amount),
                MathHelper.LerpPrecise(value1.Y, value2.Y, amount));
        }

        #endregion

        #region SmoothStep

        /// <summary>
        /// Creates a new <see cref="Vector2"/> that contains cubic interpolation of the specified vectors.
        /// </summary>
        /// <param name="a">Source <see cref="Vector2"/>.</param>
        /// <param name="b">Source <see cref="Vector2"/>.</param>
        /// <param name="amount">Weighting value.</param>
        /// <returns>Cubic interpolation of the specified vectors.</returns>
        public static Vector2 SmoothStep(Vector2 a, Vector2 b, float amount)
        {
            return new Vector2(
                MathHelper.SmoothStep(a.X, b.X, amount),
                MathHelper.SmoothStep(a.Y, b.Y, amount));
        }

        #endregion
    }
}
