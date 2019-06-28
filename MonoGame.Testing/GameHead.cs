using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using Color = Microsoft.Xna.Framework.Color;

namespace MonoGame.Testings
{
    public class GameHead : Game
    {
        private GraphicsDeviceManager _graphicsManager;
        private SpriteBatch _spriteBatch;
        private Texture2D _pixel;
        private SpriteFont _font;

        private Stopwatch _watch;

        private Song _song1;
        private Song _song2;

        private SoundEffect _winJingle;

        private Texture2D _testTexture;

        private MouseCursor _grotCursor;

        private DynamicSoundEffectInstance _dynamicSound;

        public GameHead()
        {
            GC.Collect();

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

            Window.TaskbarState = TaskbarProgressState.Indeterminate;
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            _pixel = new Texture2D(GraphicsDevice, 1, 1);
            _pixel.SetData(new Color[] { Color.White }.AsSpan());

            _font = Content.Load<SpriteFont>("arial");

            _watch = new Stopwatch();

            _watch.Restart();
            _song1 = Content.Load<Song>("sinus");
            _song1.Volume = 0.2f;
            _song1.Pitch = 1.5f;
            _watch.Stop();
            Console.WriteLine("sinus Load Time: " + _watch.ElapsedMilliseconds + "ms");

            _winJingle = Content.Load<SoundEffect>("Win Jingle");

            _testTexture = Content.Load<Texture2D>("test");

            using (var fs = File.OpenRead("risgrot.png"))
            using (var img = Texture2D.LoadImage(fs))
                _grotCursor = MouseCursor.FromImage(img, new Point(img.Width - 1, img.Height / 2));
            Mouse.SetCursor(_grotCursor);

            //w.Restart();
            //_song2 = Content.Load<Song>("Win Jingle");
            //_song2.Volume = 0.2f;
            //w.Stop();
            //Console.WriteLine("Song Load Time: " + w.ElapsedMilliseconds + "ms");
            
            //w.Restart();
            //_hitReflectSound = Content.Load<SoundEffect>("hit_reflect_0");
            //w.Stop();
            //Console.WriteLine("Load Time: " + w.ElapsedMilliseconds + "ms");

            //testMusicStream = new FileStream("test.raw", FileMode.Open);
            _dynamicSound = new DynamicSoundEffectInstance(44100, AudioChannels.Stereo);
            _dynamicSound.BufferNeeded += DynamicSound_BufferNeeded;
            _dynamicSound.Pitch = 2;
            //_dynamicSound.Play();
        }

        private FileStream testMusicStream;

        private void DynamicSound_BufferNeeded(DynamicSoundEffectInstance sender)
        {
            if (testMusicStream == null)
                return;

            byte[] buffer = new byte[44100];
            int read = testMusicStream.Read(buffer, 0, buffer.Length);

            if (read == 0)
            {
                testMusicStream.Dispose();
                testMusicStream = null;
                Console.WriteLine("reached end of stream");
                return;
            }

            sender.SubmitBuffer(buffer.AsSpan(0, read));
            Console.WriteLine("Read " + read + " bytes");
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

        float f = 5;

        protected override void Update(GameTime time)
        {
            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            f += time.Delta;
            if (f >= 4f)
            {
                f = 0f;

                //var instance = _hitReflectSound.CreateInstance();
                //instance.Pitch = -0.6f;
                //instance.Play();

                //var instance = _winJingle.CreateInstance();
                //instance.Volume = 0.05f;
                //instance.Pitch = 1.2f;
                //instance.Play();

                _watch.Restart();
                _song1.Play(TimeSpan.Zero);
                _watch.Stop();
                Console.WriteLine("Moved next in " + _watch.Elapsed.TotalMilliseconds.ToString("0.00") + "ms");

                //int w = GraphicsDevice.PresentationParameters.BackBufferWidth;
                //int h = GraphicsDevice.PresentationParameters.BackBufferHeight;
                //var data = new Rgba32[w * h];
                //GraphicsDevice.GetBackBufferData(new Rectangle(0, 0, w, h), data.AsSpan());
                //
                //using (var image = Image.WrapMemory(data.AsMemory(), w, h))
                //using (var fs = new FileStream("yo mom.png", FileMode.Create))
                //{
                //    image.SaveAsPng(fs);
                //}
            }

            base.Update(time);
        }

        protected override void Draw(GameTime time)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            _spriteBatch.Begin();

            double avg = 0;
            for (int i = 0; i < Song.UpdateTime.Count; i++)
                avg += Song.UpdateTime[i].TotalMilliseconds;
            avg /= Song.UpdateTime.Count;

            _spriteBatch.Draw(_testTexture, new Vector2(0, 0), Color.White);

            DrawShadedString(_font, "Timing: " + avg.ToString("0.0000"), new Vector2(10, 5), Color.White, Color.Black);

            //using (var tex = new Texture2D(GraphicsDevice, 1, 1))
            //{
            //    tex.SetData(new[] { Color.White }.AsSpan());
            //    _spriteBatch.Draw(tex, new RectangleF(150, 50, 20, 20), Color.Red);
            //    _spriteBatch.Draw(tex, new RectangleF(150, 100, 20, 20), Color.Green);
            //    _spriteBatch.Draw(tex, new RectangleF(150, 150, 20, 20), Color.Blue);
            //    _spriteBatch.Draw(tex, new RectangleF(150, 200, 20, 20), Color.Yellow);

            _spriteBatch.End();

            //}

            base.Draw(time);
        }

        private void DrawShadedString(SpriteFont font, string value, Vector2 position, Color textColor, Color backgroundColor)
        {
            _spriteBatch.DrawString(font, value, position + new Vector2(1f), backgroundColor);
            _spriteBatch.DrawString(font, value, position, textColor);
        }
    }
}