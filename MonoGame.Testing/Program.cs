using System;
using System.Runtime;

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
