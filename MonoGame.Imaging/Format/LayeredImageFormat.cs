using MonoGame.Framework.Collections;
using MonoGame.Imaging.Attributes.Format;

namespace MonoGame.Imaging
{
    public class LayeredImageFormat : ImageFormat, ILayeredFormatAttribute
    {
        public LayeredImageFormat(
            string fullName, string shortName,
            IReadOnlySet<string> mimeTypes,
            IReadOnlySet<string> extensions) :
            base(fullName, shortName, mimeTypes, extensions)
        {
        }
    }
}
