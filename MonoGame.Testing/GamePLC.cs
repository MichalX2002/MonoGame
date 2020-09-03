using System;
using System.Diagnostics;
using System.IO;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MonoGame.Framework;
using MonoGame.Framework.Audio;
using MonoGame.Framework.Graphics;
using MonoGame.Framework.Input;
using MonoGame.Framework.Media;
using MonoGame.Framework.Vectors;
using MonoGame.Imaging;
using MonoGame.Imaging.Coders.Encoding;
using StbSharp;

namespace MonoGame.Testing
{
    public class GamePLC : Game
    {
        private GraphicsDeviceManager _graphicsManager;
        private SpriteBatch _spriteBatch;

        private Texture2D _pixel;

        private MouseCursor _customCursor;

        private SpriteFont _font;

        private Stopwatch _watch;

        public GamePLC()
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
            _pixel.SetData(new Color[] { Color.White });

            _font = Content.Load<SpriteFont>("arial");

            var info = new TrueType.FontInfo();
            if (!TrueType.InitFont(info, File.ReadAllBytes("C:/Windows/Fonts/arial.ttf"), 0))
                throw new Exception("Failed to load font.");

            Thread.Sleep(1000);

            var watch = new Stopwatch();

            watch.Restart();
            char ch = '@';
            var glyphBitmap = TrueType.GetCodepointBitmap(
                info, new Vector2(1, 1), new Rune(ch).Value,
                out int glyphWidth, out int glyphHeight, out var glyphOffset);
            watch.Stop();
            Console.WriteLine("GetCodepointBitmap: " + watch.ElapsedMilliseconds + "ms");

            watch.Restart();
            using (var glyphImage = Image.LoadPixelData<Alpha8>(
                glyphBitmap, new Rectangle(0, 0, glyphWidth, glyphHeight), null))
            {
                glyphImage.Save("glyph_" + ch + ".png");
            }
            watch.Stop();
            Console.WriteLine("Glyph Save: " + watch.ElapsedMilliseconds + "ms");
        }

        protected override void UnloadContent()
        {
        }

        protected override void Update(in FrameTime time)
        {
            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();



            base.Update(time);
        }

        protected override void Draw(in FrameTime time)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            _spriteBatch.Begin();

            DrawShadedString(_font, "meh :[", new Vector2(50, 50), Color.White, Color.Black);

            _spriteBatch.End();

            base.Draw(time);
        }

        private void DrawShadedString(
            SpriteFont font, string value, Vector2 position, Color textColor, Color backgroundColor)
        {
            _spriteBatch.DrawString(font, value, position + new Vector2(1f), backgroundColor);
            _spriteBatch.DrawString(font, value, position, textColor);
        }
    }
}