using MonoGame.Framework;
using MonoGame.Framework.Audio;
using MonoGame.Framework.Graphics;
using MonoGame.Framework.Input;
using MonoGame.Framework.Media;
using MonoGame.Imaging;
using MonoGame.Imaging.Processing;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Color = MonoGame.Framework.Color;

namespace MonoGame.Testing
{
    public class GameHead : Game
    {
        private GraphicsDeviceManager _graphicsManager;
        private SpriteBatch _spriteBatch;

        private Texture2D _pixel;
        private Texture2D _test;

        private MouseCursor _customCursor;

        private SpriteFont _font;

        private Stopwatch _watch;

        private Song[] _songs;

        public GameHead()
        {
            SoundEffect.Initialize();

            _graphicsManager = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        protected override void Initialize()
        {
            Window.AllowUserResizing = true;
            IsMouseVisible = true;

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _watch = new Stopwatch();
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            _pixel = new Texture2D(GraphicsDevice, 1, 1);
            _pixel.SetData(new Color[] { Color.White }.AsSpan());

            _test = Content.Load<Texture2D>("test");

            _font = Content.Load<SpriteFont>("arial");

            using (var file = File.OpenRead("risgrot.png"))
            using (var img = Image.Load<Color>(file))
            using (var cropped = img.Project(x => x.Crop(8, 0, 20, 14)))
            {
                using (var croppedImg = Image.LoadPixelView(cropped))
                    croppedImg.Save("crusor.png");

                _customCursor = MouseCursor.FromPixels(cropped, new Point(2, 2)/*(img.GetSize() / 2).ToPoint()*/);
            }

            Mouse.SetCursor(_customCursor);

            string[] songs = new string[]
            {
                "Ending",
                "Title Screen"
            };

            _songs = new Song[songs.Length];
            for (int i = 0; i < songs.Length; i++)
            {
                _watch.Restart();
                _songs[i] = Content.Load<Song>(songs[i]);
                _songs[i].IsLooped = false;
                _songs[i].Volume = 0.1f;
                _songs[i].Pitch = 2f;
                _watch.Stop();
                Console.WriteLine("Content.Load<Song>('" + songs[i] + "') Time: " + _watch.ElapsedMilliseconds + "ms");
            }

            _songs[0].Volume *= 1.5f;
        }

        protected override void UnloadContent()
        {
        }

        private Song _lastSong;
        int songIndex = 0;
        float f = 3f;
        int frameIndex = 0;

        protected override void Update(GameTime time)
        {
            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            f += time.Delta;
            if (f >= 2f)
            {
                f = 0f;

                //_watch.Restart();
                //
                //_lastSong?.Stop();
                //_lastSong = _songs[songIndex++];
                //_lastSong.Play(immediate: false);
                //if (songIndex >= _songs.Length)
                //    songIndex = 0;
                //
                //_watch.Stop();
                //Console.WriteLine("Moved next in " + _watch.Elapsed.TotalMilliseconds.ToString("0.00") + "ms");

                int x = 0;
                int y = 0;
                int w = GraphicsDevice.Viewport.Width;
                int h = GraphicsDevice.Viewport.Height;

                var image = new Image<Color>(w, h);
                GraphicsDevice.GetBackBufferData(new Rectangle(x, y, w, h), image.GetPixelSpan());

                Task.Run(() =>
                {
                    bool OnProgress(
                        int frameIndex, ReadOnlyFrameCollection<Color> frames, double percentage)
                    {
                        Console.WriteLine("PNG write progress: " + Math.Round(percentage * 100f, 1) + "%");
                        return false;
                    }

                    frameIndex++;
                    image.Save("frames/yo mom " + frameIndex + ".png", null, null, OnProgress);
                    image.Dispose();
                });
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

            float xx = ((float)Math.Sin(time.TotalGameTime.TotalSeconds) + 1) / 2 * 50;
            float yy = ((float)Math.Cos(time.TotalGameTime.TotalSeconds) + 1) / 2 * 50;
            _spriteBatch.Draw(_test, new Vector2(0 + xx, 0 + yy), Color.White);

            DrawShadedString(
                _font, "Timing: " + avg.ToString("0.0000"), new Vector2(10, 5), Color.White, Color.Black);

            //using (var tex = new Texture2D(GraphicsDevice, 1, 1))
            //{
            //    tex.SetData(new[] { Color.White }.AsSpan());
            //    _spriteBatch.Draw(tex, new RectangleF(0, 0, 1, 1), Color.Red);
            //    _spriteBatch.Draw(tex, new RectangleF(0, 1, 1, 1), Color.Green);
            //    _spriteBatch.Draw(tex, new RectangleF(0, 2, 1, 1), Color.Blue);
            //    _spriteBatch.Draw(tex, new RectangleF(0, 3, 1, 1), Color.Yellow);
            //
            //_spriteBatch.End();
            //
            //}
            _spriteBatch.End();

            base.Draw(time);
        }

        private void DrawShadedString(SpriteFont font, string value, Vector2 position, Color textColor, Color backgroundColor)
        {
            _spriteBatch.DrawString(font, value, position + new Vector2(1f), backgroundColor);
            _spriteBatch.DrawString(font, value, position, textColor);
        }
    }
}