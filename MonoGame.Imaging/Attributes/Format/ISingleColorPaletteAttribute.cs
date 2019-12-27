
namespace MonoGame.Imaging.Attributes.Format
{
    /// <summary>
    /// Determines where in the stream the image color palette will be stored.
    /// </summary>
    public enum ImageColorPalettePosition
    {
        First,
        Last
    }

    /// <summary>
    /// The format only allows to write the color palette on the first/last call.
    /// </summary>
    public interface ISingleColorPaletteAttribute
    {
        ImageColorPalettePosition ColorPalettePosition { get; }
    }
}
