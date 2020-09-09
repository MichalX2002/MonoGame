using System;
using System.Diagnostics;
using System.IO;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
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
        private StringBuilder _debugBuilder = new StringBuilder();

        private SpriteFont _spriteFont;
        private Font _font;
        private FontTextureCache _fontCache;

        private Stopwatch _watch;

        private float _renderScale = 1f;
        private RenderTarget2D _fontTarget;
        private Viewport _lastViewport;

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

            _lastViewport = GraphicsDevice.Viewport;
            ViewportChanged(_lastViewport);
        }

        private void ViewportChanged(in Viewport viewport)
        {
            _fontTarget?.Dispose();
            _fontTarget = new RenderTarget2D(
                GraphicsDevice,
                (int)(viewport.Width * _renderScale),
                (int)(viewport.Height * _renderScale));
        }

        protected override void LoadContent()
        {
            _watch = new Stopwatch();
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            _pixel = new Texture2D(GraphicsDevice, 1, 1);
            _pixel.SetData(stackalloc Color[] { Color.White });

            _spriteFont = Content.Load<SpriteFont>("consolas");

            var info = new TrueType.FontInfo();
            if (!TrueType.InitFont(info, File.ReadAllBytes("C:/Windows/Fonts/arial.ttf"), 0))
                throw new Exception("Failed to load font.");

            var watch = new Stopwatch();

            if (false)
            {
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

            if (false)
            {
                Directory.CreateDirectory("glyphs");
                for (int px = 8; px <= 32; px++)
                {
                    float scale = TrueType.ScaleForPixelHeight(info, px);

                    char ch = '@';
                    var glyphBitmap = TrueType.GetCodepointBitmap(
                        info, new Vector2(scale), new Rune(ch).Value,
                        out int glyphWidth, out int glyphHeight, out var glyphOffset);

                    using (var glyphImage = Image.LoadPixelData<Alpha8>(
                        glyphBitmap, new Rectangle(0, 0, glyphWidth, glyphHeight), null))
                    {
                        glyphImage.Save("glyphs/glyph_" + ch + "x" + px + ".png");
                    }
                }
            }

            _font = new Font(File.ReadAllBytes("C:/Windows/Fonts/consola.ttf"));
            //_font = new Font(File.ReadAllBytes("C:/Windows/Fonts/times.ttf"));
            //_font = new Font(File.ReadAllBytes("C:/Windows/Fonts/comic.ttf"));

            _fontCache = new FontTextureCache(GraphicsDevice, 2048, SurfaceFormat.Rgba32);
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
            var currentViewport = GraphicsDevice.Viewport;
            if (_lastViewport != currentViewport)
            {
                ViewportChanged(currentViewport);
                _lastViewport = currentViewport;
            }

            GraphicsDevice.SetRenderTarget(_fontTarget);
            GraphicsDevice.Clear(Color.Transparent);
            _spriteBatch.Begin(
                blendState: BlendState.NonPremultiplied,
                transformMatrix: Matrix4x4.CreateScale(_renderScale * 1f));
            {
                //string sicc = "#h€jio? h½llå ön you dis is the fönt cäche thing workin";
                string sicc = "@";

                var span = sicc.AsSpan();

                float spriteScale = (float)(Math.Sin(time.Total.TotalSeconds * 0.5)) * 128f;
                float requestedPixelHeight = Math.Max(spriteScale + 64f, 0f) + 8f;
                 
                float actualScaleF = _font.GetScaleByPixel(requestedPixelHeight);
                var actualScale = new Vector2(actualScaleF);

                float x = 10;
                float y = _lastViewport.Height / 2;
                do
                {
                    if (Rune.DecodeFromUtf16(span, out Rune rune, out int consumed) !=
                        System.Buffers.OperationStatus.Done)
                        break;

                    int glyphIndex = _font.GetGlyphIndex(rune.Value);

                    if (false)
                    {
                        using var tmpImg = _font.GetGlyphBitmap(glyphIndex, actualScale);
                        var tmpTex = Texture2D.FromImage(tmpImg, GraphicsDevice, false, SurfaceFormat.Rgba32);

                        var offsetRect = new RectangleF(
                            50,
                            50,
                            0,
                            0);

                        _spriteBatch.Draw(
                            tmpTex,
                            tmpTex.Bounds + offsetRect,
                            null,
                            Color.Red,
                            0,
                            tmpTex.Bounds.Size / 2f,
                            SpriteFlip.None,
                            0);
                    }

                    _font.GetGlyphHMetrics(glyphIndex, out int advanceWidth, out int leftSideBearing);
                    _font.GetFontVMetrics(out int ascent, out int descent, out int lineGap);

                    var glyph = _fontCache.GetGlyph(_font, requestedPixelHeight, glyphIndex);
                    if (glyph != null)
                    {
                        var scaleFactor = actualScale / glyph.Scale;

                        var texRect = glyph.TextureRect;

                        _font.GetGlyphBox(glyph.GlyphIndex, out var rawGlyphBox);
                        //GetGlyphBoxSubpixel(rawGlyphBox, glyph.Scale, default, out var cachedGlyphBox);
                        GetGlyphBoxSubpixel(rawGlyphBox, actualScale, default, out var actualGlyphBox);

                        TrueType.GetGlyphBitmapBox(_font.FontInfo, glyph.GlyphIndex, glyph.Scale, out var cachedIntGlyphBox);
                        //TrueType.GetGlyphBitmapBox(_font.FontInfo, glyph.GlyphIndex, actualScale, out var actualIntGlyphBox);

                        var drawOrig = new Vector2(cachedIntGlyphBox.W, cachedIntGlyphBox.H) / 2;

                        var pos = new Vector2(
                            x + leftSideBearing * actualScale.X,
                            y + actualGlyphBox.Y);

                        var drawPos = pos + new Vector2(actualGlyphBox.Width / 2, actualGlyphBox.Height / 2);

                        if (requestedPixelHeight > 16)
                        {
                            _spriteBatch.DrawRectangle(
                                new RectangleF(pos, actualGlyphBox.Size) +
                                new RectangleF(-1, -1, 2, 2), Color.Red);
                        }

                        //_spriteBatch.DrawRectangle(
                        //    new RectangleF(pos.X, pos.Y, cachedIntGlyphBox.W, cachedIntGlyphBox.H) +
                        //    new RectangleF(-1, -1, 2, 2), Color.Green);

                        _spriteBatch.Draw(
                            glyph.Texture, drawPos, texRect, Color.White,
                            0, drawOrig, scaleFactor, SpriteFlip.None, 0);

                        //_spriteBatch.Draw(
                        //    _pixel, pos - new Vector2(1, 0), null, Color.Red,
                        //    0, Vector2.Zero, new Vector2(1, texRect.Height), SpriteFlip.None, 0);
                        //
                        //_spriteBatch.Draw(
                        //    _pixel, new Vector2(x, y + 1), null, Color.Green,
                        //    0, Vector2.Zero, new Vector2(advanceWidth * actualScale.X, 1), SpriteFlip.None, 0);
                        //
                        //_spriteBatch.Draw(
                        //    _pixel, new Vector2(pos.X, y + 2), null, Color.Lime,
                        //    0, Vector2.Zero, new Vector2(leftSideBearing * actualScale.X, 1), SpriteFlip.None, 0);
                    }

                    x += advanceWidth * actualScale.X;

                    span = span.Slice(consumed);
                }
                while (!span.IsEmpty);

                if (requestedPixelHeight > 16)
                    _spriteBatch.DrawLine(new Vector2(10, _lastViewport.Height / 2), new Vector2(x, y), Color.Green);
            }
            _spriteBatch.End();
            GraphicsDevice.SetRenderTarget(null);

            GraphicsDevice.Clear(Color.MediumPurple);
            _spriteBatch.Begin(blendState: BlendState.AlphaBlend);
            _spriteBatch.Draw(_fontTarget, currentViewport.Bounds, _fontTarget.Bounds, Color.White);
            _spriteBatch.End();

            _spriteBatch.Begin(blendState: BlendState.AlphaBlend);
            //DrawDebug(new Vector2(3, 2));
            DrawDebug(new Vector2(3, _lastViewport.Height - 80));
            _spriteBatch.End();

            base.Draw(time);
        }

        private void DrawDebug(Vector2 position)
        {
            long totalMem = Environment.WorkingSet;
            long gcMem = GC.GetTotalMemory(false);
            double totalMb = totalMem / (1024 * 1024.0);
            double gcMb = gcMem / (1024 * 1024.0);

            int gc0 = GC.CollectionCount(0);
            int gc1 = GC.CollectionCount(1);
            int gc2 = GC.CollectionCount(2);

            int totalMbDecimals = (int)Math.Max(0, 4 - Math.Log10(totalMb));
            int gcMbDecimals = (int)Math.Max(0, 3 - Math.Log10(gcMb));

            _debugBuilder.Append("GC Counts: [")
                .Append(gc0).Append(',')
                .Append(gc1).Append(',')
                .Append(gc2).Append(']').AppendLine();
            _debugBuilder.Append("GC Heap: ").Append(Math.Round(gcMb, gcMbDecimals)).AppendLine();
            _debugBuilder.Append("Memory: ").Append(Math.Round(totalMb, totalMbDecimals)).AppendLine();

            RuneEnumerator e = _debugBuilder;
            _spriteBatch.DrawString(_spriteFont, e, position + new Vector2(1, -0.75f), Color.Gray);
            _spriteBatch.DrawString(_spriteFont, e, position + new Vector2(-0.75f, 1), Color.Gray);
            _spriteBatch.DrawString(_spriteFont, e, position + new Vector2(-0.75f, -0.75f), Color.White);
            _spriteBatch.DrawString(_spriteFont, e, position + new Vector2(1, 1), Color.Black);
            _spriteBatch.DrawString(_spriteFont, e, position + new Vector2(0, 0), Color.Cyan);
            _debugBuilder.Clear();
        }

        public static void GetGlyphBoxSubpixel(
            RectangleF rawGlyphBox, Vector2 scale, Vector2 shift, out RectangleF glyphBox)
        {
            var br = rawGlyphBox.BottomRight;

            glyphBox = FromEdgePoints(
                tlX: rawGlyphBox.X * scale.X + shift.X,
                tlY: -br.Y * scale.Y + shift.Y,
                brX: br.X * scale.X + shift.X,
                brY: -rawGlyphBox.Y * scale.Y + shift.Y);
        }

        public static RectangleF FromEdgePoints(float tlX, float tlY, float brX, float brY)
        {
            return new RectangleF(
                tlX,
                tlY,
                brX - tlX,
                brY - tlY);
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