
namespace MonoGame.Framework.Utilities
{
    public abstract partial class TaskbarList
    {
        private static TaskbarList PlatformCreate(GameWindow window)
        {
            switch(PlatformInfo.OS)
            {
                case PlatformInfo.OperatingSystem.Windows:
                    return new WindowsTaskbarList(window);

                default:
                    return new DummyTaskbarList(window);
            }
        }
    }
}
