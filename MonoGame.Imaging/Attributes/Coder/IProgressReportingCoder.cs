using MonoGame.Framework;

namespace MonoGame.Imaging.Attributes.Coder
{
    /// <summary>
    /// Represents a progress update for an imaging component.
    /// </summary>
    public delegate void ImagingProgressCallback<T>(
        T sender,
        double percentage,
        Rectangle? rectangle)
        where T : IImagingConfigurable;

    /// <summary>
    /// The coder will report a progress percentage.
    /// It may also provide a rectangle depicting where it is currently working.
    /// </summary>
    public interface IProgressReportingCoder<TCoder> : IImageCoderExtension
        where TCoder : IImagingConfigurable
    {
        public bool CanReportProgressRectangle { get; }

        public event ImagingProgressCallback<TCoder>? Progress;
    }
}
