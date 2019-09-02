using System;

namespace MonoGame.Testing
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
