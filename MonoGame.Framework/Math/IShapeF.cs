namespace Microsoft.Xna.Framework
{
    /// <summary>
    ///     Base class for shapes.
    /// </summary>
    /// <remarks>
    ///     Created to allow checking intersection between shapes of different types.
    /// </remarks>
    public interface IShapeF
    {
        /// <summary>
        /// Gets or sets the position of the shape.
        /// </summary>
        PointF Position { get; set; }
    }
}