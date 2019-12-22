
namespace Microsoft.Xna.Framework
{
    /// <summary>
    /// Allows for platform specific handling of the Back button. 
    /// </summary>
    /// <a href="http://www.monogame.net/documentation/?page=Platform_Specific_Notes"/>
    public interface IPlatformBackButton
    {
        /// <summary>
        /// Return <see langword="true"/> if your game has handled the back button event.
        /// Return <see langword="false"/> if you want the operating system to handle it.
        /// </summary>
        bool Handled();
    }
}
