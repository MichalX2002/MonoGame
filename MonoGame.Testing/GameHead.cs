using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;

namespace MonoGame.Testings
{
    public class GameHead : Game
    {
        private GraphicsDeviceManager _graphicsManager;
        private SpriteBatch _spriteBatch;
        private Texture2D _pixel;
        private SpriteFont _font;

        private Song _song1;
        private Song _song2;

        private SoundEffect _hitReflectSound;

        public GameHead()
        {
            _graphicsManager = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        protected override void Initialize()
        {
            Window.AllowUserResizing = true;
            IsMouseVisible = true;

            base.Initialize();

            //_song1.Play();
            //_song2.Play();
        }

        private System.Diagnostics.Stopwatch w;

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            _pixel = new Texture2D(GraphicsDevice, 1, 1);
            _pixel.SetData(new Color[] { Color.White });

            _font = Content.Load<SpriteFont>("arial");

            w = new System.Diagnostics.Stopwatch();
            w.Restart();
            _song1 = Content.Load<Song>("sinus");
            _song1.Volume = 0.5f;
            w.Stop();
            Console.WriteLine("Song Load Time: " + w.ElapsedMilliseconds + "ms");

            w.Restart();
            _song2 = Content.Load<Song>("Win Jingle");
            _song2.Volume = 0.2f;
            w.Stop();
            Console.WriteLine("Song Load Time: " + w.ElapsedMilliseconds + "ms");

            w.Restart();
            _hitReflectSound = Content.Load<SoundEffect>("hit_reflect_0");
            w.Stop();
            Console.WriteLine("Load Time: " + w.ElapsedMilliseconds + "ms");
        }

        //int ix = 0;
        //int j = 0;
        //int a1, b1, a2, b2;
        //int c1, c2, d1, d2;
        //
        //void GenerateMusic(short[] buf, int len)
        //{
        //    //a1 = b1 = a2 = b2 = 0;
        //    //c1 = c2 = d1 = d2 = 0;
        //
        //    //int j = 0;
        //
        //    for (int i = 0; i < len; i++)
        //    {
        //        uint r;
        //        int v1, v2;
        //        v1 = v2 = (((j * ((j >> 12) ^ ((j >> 10 | j >> 12) & 26 & j >> 7))) & 128) + 128) << 15;
        //        r = FastRand();
        //        v1 += (int)r & 65535;
        //        v1 -= (int)r >> 16;
        //        r = FastRand();
        //        v2 += (int)r & 65535;
        //        v2 -= (int)r >> 16;
        //        b1 = v1 - a1 + ((b1 * 61 + 32) >> 6); a1 = v1;
        //        b2 = v2 - a2 + ((b2 * 61 + 32) >> 6); a2 = v2;
        //        c1 = (30 * (c1 + b1 + d1) + 32) >> 6; d1 = b1;
        //        c2 = (30 * (c2 + b2 + d2) + 32) >> 6; d2 = b2;
        //        v1 = (c1 + 128) >> 8;
        //        v2 = (c2 + 128) >> 8;
        //        buf[i * 2] = (short)(v1 > 32767 ? 32767 : (v1 < -32768 ? -32768 : v1));
        //        buf[i * 2 + 1] = (short)(v2 > 32767 ? 32767 : (v2 < -32768 ? -32768 : v2));
        //
        //        if (ix % 6 == 0)
        //            j++;
        //        ix++;
        //    }
        //}
        //
        //uint Rz = 0, Rw = 0;
        //uint FastRand()
        //{
        //    Rz = 36969 * (Rz & 65535) + (Rz >> 16);
        //    Rw = 18000 * (Rw & 65535) + (Rw >> 16);
        //    return (Rz << 16) + Rw;
        //}

        protected override void UnloadContent()
        {
        }

        float f = 0;

        protected override void Update(GameTime time)
        {
            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            f += time.Delta;
            if (f >= 5)
            {
                //var instance = _hitReflectSound.CreateInstance();
                //instance.Pitch = -0.6f;
                //instance.Play();

                f = 0f;

                w.Restart();
                //_song2.Play(TimeSpan.Zero);
                w.Stop();
                Console.WriteLine("Moved next in " + w.Elapsed.TotalMilliseconds.ToString("0.00") + "ms");
            }

            base.Update(time);
        }

        protected override void Draw(GameTime time)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            _spriteBatch.Begin();
            
            double avg = 0;
            foreach (var value in Song.ThreadUpdateTiming)
                avg += value.TotalMilliseconds;
            avg /= Song.ThreadUpdateTiming.Count;
            
            _spriteBatch.DrawString(_font, "Timing: " + avg.ToString("0.0"), new Vector2(10, 5), Color.White);

            using (var tex = new Texture2D(GraphicsDevice, 1, 1))
            {
                tex.SetData(new[] { Color.White });
                _spriteBatch.Draw(tex, new RectangleF(150, 50, 20, 20), Color.Red);
                _spriteBatch.Draw(tex, new RectangleF(150, 100, 20, 20), Color.Green);
                _spriteBatch.Draw(tex, new RectangleF(150, 150, 20, 20), Color.Blue);
                _spriteBatch.Draw(tex, new RectangleF(150, 200, 20, 20), Color.Yellow);

                _spriteBatch.End();
            }

            base.Draw(time);
        }
    }
}