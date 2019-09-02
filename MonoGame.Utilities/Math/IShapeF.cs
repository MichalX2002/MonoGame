namespace MonoGame.Framework
{
    /// <summary>
    /// Base interace for different shapes.
    /// </summary>
    /// <remarks>
    /// Created to allow checking intersection between shapes of different types.
    /// </remarks>
    public interface IShapeF
    {
        /// <summary>
        /// Gets or sets the position of the shape.
        /// </summary>
        PointF Position { get; set; }
    }
}