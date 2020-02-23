
namespace MonoGame.Imaging.Attributes.Codec
{
    /// <summary>
    /// The codec will report a rectangle within the image, 
    /// depicting where it is currently working.
    /// <para>
    /// A rectangle the size of the image will be reported upon completion.
    /// </para>
    /// </summary>
    public interface IProgressRectangleReporterAttribute : IImageCodecAttribute
    {
    }
}
