
namespace MonoGame.Imaging
{
    public interface IImagingConfig
    {
        T? GetModule<T>() where T : class;
    }
}
