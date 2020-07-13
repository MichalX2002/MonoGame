
namespace MonoGame.Imaging.Attributes.Format
{
    /// <summary>
    /// The format allows to write a color palette on the first/last call.
    /// </summary>
    public interface ISingleColorPaletteAttribute : IImageFormatAttribute
    {
        ImageColorPalettePosition ColorPalettePosition { get; }
    }
}
