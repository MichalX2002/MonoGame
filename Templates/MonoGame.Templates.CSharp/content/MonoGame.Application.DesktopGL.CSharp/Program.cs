﻿using System;

namespace MGNamespace
{
    public static class Program
    {
        [STAThread]
        static void Main()
        {
            using (var game = new GameFrame())
                game.Run();
        }
    }
}
