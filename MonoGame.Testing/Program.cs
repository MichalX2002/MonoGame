using System;

namespace MonoGame.Testing
{
    public static class Program
    {
        [STAThread]
        static void Main()
        {
            using (var game = new GamePLC())
            //using (var game = new GameImageInterlace())
            //using (var game = new GameMusicTest())
                game.Run();
        }
    }
}
