
namespace MonoGame.Imaging.Attributes.Coder
{
    /// <summary>
    /// The coder will report a rectangle within the image, 
    /// depicting where it is currently working.
    /// <para>
    /// A rectangle the size of the image will be reported upon completion.
    /// </para>
    /// </summary>
    public interface IProgressRectangleReporterAttribute : IImageCoderAttribute
    {
    }
}
