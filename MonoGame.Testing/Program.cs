using System;

namespace MonoGame.Testings
{
    public static class Program
    {
        [STAThread]
        static void Main()
        {
            using (var game = new GameHead())
                game.Run();
        }
    }
}
