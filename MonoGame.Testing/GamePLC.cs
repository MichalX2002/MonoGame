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
using MonoGame.Framework.Content;
using MonoGame.Framework.Graphics;
using MonoGame.Framework.Input;
using MonoGame.Framework.Media;
using MonoGame.Framework.Vectors;
using MonoGame.Imaging;
using MonoGame.Imaging.Coders.Encoding;
using StbSharp;

namespace MonoGame.Testing
{
    public partial class GamePLC : Game
    {
        private GraphicsDeviceManager _graphicsManager;
        private SpriteBatch _spriteBatch;

        private Texture2D _pixel;

        private SpriteFont _spriteFont;
        private Font _font;
        private FontTextureCache _fontCache;

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

            //Draw(default);
            //
            //var states = _fontCache._textureStates;
            //for (int i = 0; i < states.Count; i++)
            //{
            //    var state = states[i];
            //    state.Texture.Save("state_" + i + ".png");
            //}
        }

        protected override void LoadContent()
        {
            _watch = new Stopwatch();
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            _pixel = new Texture2D(GraphicsDevice, 1, 1);
            _pixel.SetData(new Color[] { Color.White });

            _spriteFont = Content.Load<SpriteFont>("arial");

            var info = new TrueType.FontInfo();
            if (!TrueType.InitFont(info, File.ReadAllBytes("C:/Windows/Fonts/arial.ttf"), 0))
                throw new Exception("Failed to load font.");

            var watch = new Stopwatch();

            if (false)
            {
                Thread.Sleep(500);

                watch.Restart();
                char ch = '@';
                var glyphBitmap = TrueType.GetCodepointBitmap(
                    info, new Vector2(1f), new Rune(ch).Value,
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

            if (false)
            {
                Thread.Sleep(500);

                // used both to compute SDF and in 'shader'
                float pixel_dist_scale = 64;   // trades off precision w/ ability to handle *smaller* sizes
                byte onedge_value = 128;
                int padding = 3; // not used in shader

                // the larger the scale, the better large font sizes look

                watch.Restart();
                char ch = '@';
                var glyphBitmap = TrueType.GetCodepointSDF(
                    info, new Vector2(1f), new Rune(ch).Value, padding, onedge_value, pixel_dist_scale,
                    out int glyphWidth, out int glyphHeight, out var glyphOffset);
                watch.Stop();
                Console.WriteLine("GetCodepointSDF: " + watch.ElapsedMilliseconds + "ms");

                watch.Restart();
                using (var glyphImage = Image.LoadPixelData<Alpha8>(
                    glyphBitmap, new Rectangle(0, 0, glyphWidth, glyphHeight), null))
                {
                    glyphImage.Save("sdf_" + ch + ".png");
                }
                watch.Stop();
                Console.WriteLine("SDF Save: " + watch.ElapsedMilliseconds + "ms");
            }

            _font = new Font(File.ReadAllBytes("C:/Windows/Fonts/arial.ttf"));
            //var scale = new Vector2(font.GetScaleByPixel(32));
            //
            //var img  = font.GetGlyphBitmap(font.GetGlyphIndex(new Rune('%').Value), scale);

            _fontCache = new FontTextureCache(GraphicsDevice, 4096, SurfaceFormat.Rgba32);
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
            {
                DrawShadedString(
                    _spriteFont, "meh :[", new Vector2(50, 10), Color.White, Color.Black);
            }
            _spriteBatch.End();

            _spriteBatch.Begin(
                blendState: BlendState.NonPremultiplied,
                transformMatrix: Matrix4x4.CreateScale(1));
            {
                string sicc = "h€llo ön you dis is the cool fönt cäche thing";
                //string sicc = "@";
                var span = sicc.AsSpan();

                float x = 0;
                do
                {
                    if (Rune.DecodeFromUtf16(span, out Rune rune, out int consumed) !=
                        System.Buffers.OperationStatus.Done)
                        break;

                    float spriteScale = (float)(Math.Sin(time.Total.TotalSeconds * 0.5) + 1) * 32;
                    float actualPixelSize = spriteScale + 8f;
                    int visiblePixelHeight = (int)Math.Ceiling(actualPixelSize);

                    int glyphIndex = _font.GetGlyphIndex(rune.Value);
                    var glyph = _fontCache.GetGlyph(_font, visiblePixelHeight, glyphIndex);
                   
                    float actualScale = _font.GetScaleByPixel(actualPixelSize);
                    float visibleScale = _font.GetScaleByPixel(visiblePixelHeight);
                    float sizeFactor = actualScale / visibleScale;
                    
                    if (glyph != null)
                    {
                        var texRect = glyph.TextureRect;
                        var glyphRect = glyph.GlyphRect;

                        var pos = new Vector2(10 + x, 60 - glyphRect.Height * actualScale);
                        _spriteBatch.Draw(
                            glyph.Texture, pos, texRect, Color.Red,
                            0, Vector2.Zero, sizeFactor, SpriteFlip.None, 0);

                        x += glyphRect.Width * actualScale;
                    }
                    else
                    {
                        x += 16 * sizeFactor;
                    }

                    span = span.Slice(consumed);
                }
                while (!span.IsEmpty);
            }
            _spriteBatch.End();

            base.Draw(time);
        }

        private void DrawShadedString(
            SpriteFont font, string value, Vector2 position,
            Color textColor, Color backgroundColor)
        {
            _spriteBatch.DrawString(font, value, position + new Vector2(1f), backgroundColor);
            _spriteBatch.DrawString(font, value, position, textColor);
        }
    }
}