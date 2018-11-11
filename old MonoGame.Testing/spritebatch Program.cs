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
                    _textures [i] = new Texture2D(GraphicsDevice, 2, 2);
                    Color c = new Color(127, 255 * (i + 1) / _textures.Length, 127) //(uint)(uint.MaxValue * r.NextDouble()))
                    {
                        A = 255
                    };
                    _textures[i].SetData(new Color[] { c, Color.Black, Color.Black, c });
                }
            }

            Random r = new Random();

            Stopwatch watch = new Stopwatch();
            List<double> timings = new List<double>();
            List<double> endTimings = new List<double>();

            protected override void Draw(GameTime gameTime)
            {
                if (Keyboard.GetState().IsKeyDown(Keys.Escape))
                    Exit();

                GraphicsDevice.Clear(Color.CornflowerBlue);

                /*
                float sin1 = (float)Math.Sin(gameTime.TotalGameTime.TotalSeconds) / 4 + 0.7f;
                float sin2 = (float)Math.Sin(gameTime.TotalGameTime.TotalSeconds + Math.PI / 2) / 4 + 0.7f;

                _batch.Begin(SpriteSortMode.Deferred, depthStencilState: DepthStencilState.Default);
                _batch.Draw(_textures[0], new Vector2(100, 100), null, Color.Red, 0, Vector2.Zero, 50, SpriteEffects.None, sin1);
                _batch.End();

                _batch.Begin(SpriteSortMode.Deferred, depthStencilState: DepthStencilState.Default);
                _batch.Draw(_textures[0], new Vector2(150, 150), null, Color.White, 0, Vector2.Zero, 50, SpriteEffects.None, sin2);
                _batch.End();

                Console.WriteLine("RED: " + sin1 + " | GRAY:" + sin2);
                */
                
                int size = 16;
                int offset = size + 8;
                Rectangle rect = new Rectangle(10, 10, size, size);
                
                _batch.Begin();
                
                /*
                for (int i = 0; i < 10000; i++)
                {
                    _batch.Draw(_textures[_textures.Length - 1], new Rectangle(i * 32 + 16, 16, 32, 32), new Color(i * 16, 127, 127));
                }
                */

                watch.Restart();
                for (int t = 0; t < _textures.Length; t++)
                {
                    Texture2D tex = _textures[t];

                    //for (int x = 0; x < 1; x++)
                    //{
                    for (int yy = 0; yy < 1000; yy++)
                    {
                        //float dep = (float)r.NextDouble();
                        _batch.Draw(texture: tex, destinationRectangle: rect, color: Color.White, layerDepth: 0);

                        rect.X += offset;
                    }

                    //}

                    rect.X = 10;
                    rect.Y += offset;
                }
                watch.Stop();
                timings.Add(watch.Elapsed.TotalMilliseconds);

                watch.Restart();
                _batch.End();
                watch.Stop();
                endTimings.Add(watch.Elapsed.TotalMilliseconds);

                if (timings.Count > 60)
                {
                    string draw = Math.Round(timings.Average(), 4).ToString("N4");
                    string flush = Math.Round(endTimings.Average(), 4).ToString("N4");

                    Console.WriteLine($"Draw {draw}ms | Flush: {flush}ms");
                    Console.WriteLine(GraphicsDevice.Metrics.SpriteCount);

                    timings.Clear();
                    endTimings.Clear();
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
