using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace MonoGame.Testing
{
    class Program
    {
        class GameFrame : Game
        {
            private readonly GraphicsDeviceManager _manager;
            private SpriteBatch _batch;

            private Texture2D[] _textures;

            public GameFrame()
            {
                _manager = new GraphicsDeviceManager(this);
            }

            protected override void Initialize()
            {
                base.Initialize();

                IsMouseVisible = true;
                Window.AllowUserResizing = true;
            }

            protected override void LoadContent()
            {
                base.LoadContent();

                _batch = new SpriteBatch(GraphicsDevice);

                _textures = new Texture2D[20];
                for (int i = 0; i < _textures.Length; i++)
                {
                    _textures [i]= new Texture2D(GraphicsDevice, 2, 2);
                    Color c = new Color((uint)(uint.MaxValue * r.NextDouble()))
                    {
                        A = 255
                    };
                    _textures[i].SetData(new Color[] { c, Color.Black, Color.Black, c });
                }
            }

            Random r = new Random();
            int t;

            Stopwatch watch = new Stopwatch();
            List<double> timings = new List<double>();
            List<double> endTimings = new List<double>();

            protected override void Draw(GameTime gameTime)
            {
                if (Keyboard.GetState().IsKeyDown(Keys.Escape))
                    Exit();

                GraphicsDevice.Clear(Color.CornflowerBlue);

                int drawCount = 30000;

                int size = 16;
                int offset = size + 8;
                Rectangle rect = new Rectangle(10, 10, size, size);

                Texture2D tex = _textures[t];

                for (int x = 0; x < 1; x++)
                {
                    watch.Restart();
                    _batch.Begin(SpriteSortMode.Texture);
                    for (int i = 0; i < drawCount; i++)
                    {
                        _batch.Draw(tex, rect, Color.White);
                    }
                    rect.X += offset;
                    watch.Stop();
                    timings.Add(watch.Elapsed.TotalMilliseconds);

                    watch.Restart();
                    _batch.End();
                    watch.Stop();
                    endTimings.Add(watch.Elapsed.TotalMilliseconds);

                    rect.X = 10;
                    rect.Y += offset;
                }

                if (timings.Count > 60)
                {
                    Console.WriteLine("SpriteBatch : " + Math.Round(timings.Average(), 2).ToString("N2") + "ms");
                    Console.WriteLine("SpriteBatch  Flush: " + Math.Round(endTimings.Average(), 2).ToString("N2") + "ms");

                    timings.Clear();
                    endTimings.Clear();
                    
                    t = r.Next(_textures.Length);
                }

                base.Draw(gameTime);
            }
        }

        static void Main(string[] args)
        {
            using (var frame = new GameFrame())
                frame.Run();
        }
    }
}
