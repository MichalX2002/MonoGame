using System;
using System.Buffers;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
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
using MathHelper = MonoGame.Framework.MathHelper;

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

        private float _renderScale = 2f;
        private RenderTarget2D _fontTarget;
        private Viewport _lastViewport;

        private string _loremIpsum;
        private string _loremIpsumSlice;

        private DateTime _lastWriteDate;
        private List<TextGlyph> _textGlyphs = new List<TextGlyph>();
        private Vector2 _textPos = new Vector2(0, 0);
        private SizeF _textSize;

        private Vector2 _lastScroll;
        private Vector2 _scrollDelta;

        private Vector2 _lastMousePos;
        private Vector2 _mousePosDelta;

        private float _scale = 1;
        private Vector2 _scrollOffset;
        private Vector2 _mouseOffset;
        private Vector2 ViewPosition => _scrollOffset + _mouseOffset;

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

            var mouseState = Mouse.GetState();
            _lastScroll = mouseState.Scroll;
            _lastMousePos = mouseState.Position;
        }

        private void ViewportChanged(in Viewport viewport)
        {
            _fontTarget?.Dispose();
            _fontTarget = new RenderTarget2D(
                GraphicsDevice,
                (int)(viewport.Width * _renderScale),
                (int)(viewport.Height * _renderScale),
                false,
                SurfaceFormat.Rgba32,
                DepthFormat.None,
                0,
                RenderTargetUsage.PlatformContents);
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

            _loremIpsum = File.ReadAllText("Content/loremipsum.txt");
            _loremIpsumSlice = _loremIpsum.Substring(0, _loremIpsum.Length / 32);

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
            //_font = new Font(File.ReadAllBytes("C:/Windows/Fonts/calibri.ttf"));
            //_font = new Font(File.ReadAllBytes("C:/Windows/Fonts/comic.ttf"));

            _fontCache = new FontTextureCache(GraphicsDevice, 2048, SurfaceFormat.Rgba32);
        }

        private void BuildText(string code)
        {
            var tokens = new PLCTokenizer(commentPrefix: "#");
            var codeSpan = code.AsSpan();
            PLCToken token;

            float requestedPixelHeight = 24;
            float virtualPixelHeight = requestedPixelHeight * _scale * _renderScale;

            float actualScaleValue = _font.GetScaleByPixel(requestedPixelHeight);
            var actualScale = new Vector2(actualScaleValue);

            _font.GetFontVMetrics(out int ascent, out int descent, out int lineGap);
            float lineHeight = (ascent - descent + lineGap) * actualScale.Y;

            Vector2 position = new Vector2(_textPos.X, _textPos.Y + ascent * actualScale.Y);
            float maxX = 0;

            _textGlyphs.Clear();

            while ((token = tokens.ReadToken(codeSpan, out int consumed)).Type != PLCTokenType.EndOfData)
            {
                codeSpan = codeSpan.Slice(consumed);

                Color color = token.Type switch
                {
                    PLCTokenType.Instruction => Color.OrangeRed,
                    PLCTokenType.Argument => Color.LightBlue,
                    PLCTokenType.Comment => Color.Green,
                    _ => Color.Yellow
                };

                var span = token.Value;
                while (Rune.DecodeFromUtf16(span, out Rune rune, out int consumedChars) == OperationStatus.Done)
                {
                    span = span.Slice(consumedChars);

                    if (rune.Value == '\n')
                    {
                        if (position.X > maxX)
                            maxX = position.X;

                        position.X = _textPos.X;
                        position.Y += lineHeight;
                    }
                    else
                    {
                        int glyphIndex = _font.GetGlyphIndex(rune.Value);
                        _font.GetGlyphHMetrics(glyphIndex, out int advanceWidth, out int leftSideBearing);

                        var glyph = _fontCache.GetGlyph(_font, glyphIndex, virtualPixelHeight);
                        if (glyph != null)
                        {
                            Vector2 scaleFactor = actualScale / glyph.Scale;
                            Rectangle texRect = glyph.TextureRect;

                            _font.GetGlyphBoxSubpixel(glyph.GlyphIndex, actualScale, default, out var actualGlyphBox);

                            var drawOrig = new Vector2(texRect.Width, texRect.Height) / 2;
                            drawOrig *= scaleFactor;

                            var pos = position + new Vector2(
                                leftSideBearing * actualScale.X,
                                actualGlyphBox.Y);

                            var drawPos = pos + new Vector2(actualGlyphBox.Width / 2, actualGlyphBox.Height / 2);

                            var texelSize = glyph.Texture.TexelSize;
                            var srcRect = glyph.TextureRect;
                            var texCoord = SpriteQuad.GetTexCoord(texelSize, srcRect);
                            float w = srcRect.Width * scaleFactor.X;
                            float h = srcRect.Height * scaleFactor.Y;

                            var quad = new SpriteQuad();
                            quad.Set(
                                drawPos.X - drawOrig.X,
                                drawPos.Y - drawOrig.Y,
                                w,
                                h,
                                color,
                                texCoord,
                                0);

                            _textGlyphs.Add(new TextGlyph(glyph, quad));
                        }

                        position.X += advanceWidth * actualScale.X;

                        var nextRuneStatus = Rune.DecodeFromUtf16(span, out Rune nextRune, out int _);
                        if (nextRuneStatus == OperationStatus.Done)
                        {
                            int glyphIndex2 = _font.GetGlyphIndex(nextRune.Value);
                            int? kernAdvance = _font.GetGlyphKernAdvance(glyphIndex, glyphIndex2);

                            if (kernAdvance.HasValue)
                                position.X += kernAdvance.GetValueOrDefault() * actualScale.X;
                        }
                    }
                }
            }

            if (position.X > maxX)
                maxX = position.X;

            _textSize = new SizeF(maxX, position.Y);
        }

        protected override void UnloadContent()
        {
        }

        protected override void Update(in FrameTime time)
        {
            var keyboard = Keyboard.GetState();
            if (keyboard.IsKeyDown(Keys.Escape))
                Exit();
            
            var mouse = Mouse.GetState();

            _scrollDelta = mouse.Scroll - _lastScroll;
            _scrollDelta.X = -_scrollDelta.X;
            _lastScroll = mouse.Scroll;

            _mousePosDelta = mouse.Position - _lastMousePos;
            _lastMousePos = mouse.Position;

            if (keyboard.Modifiers.HasAnyFlag(KeyModifiers.Control))
            {
                float scaleDelta = _scrollDelta.Y * 0.001f * ((_scale + 1) / 2f);
                _scale = MathHelper.Clamp(_scale + scaleDelta, 0.2f, 4f);

                if (mouse.LeftButton.HasAnyFlag(ButtonState.Pressed))
                    _mouseOffset += _mousePosDelta;
            }
            else
            {
                _scrollOffset += _scrollDelta * 0.1f;

                if (mouse.MiddleButton.HasAnyFlag(ButtonState.Pressed))
                    _mouseOffset += _mousePosDelta;
            }

            base.Update(time);
        }

        private static int GetLineCount(ReadOnlySpan<char> text)
        {
            if (text.IsEmpty)
                return 0;

            int count = 1;
            do
            {
                int index = text.IndexOf('\n');
                if (index == -1)
                    break;

                count++;
                text = text.Slice(index + 1);
            }
            while (!text.IsEmpty);
            return count;
        }

        private void DrawFreshGlyph(int glyphIndex, Vector2 scale, Vector2 shift)
        {
            using var tmpImg = _font.GetGlyphBitmap(glyphIndex, scale);
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

        protected override void Draw(in FrameTime time)
        {
            string path = @"plc il.txt";
            if (File.Exists(path))
            {
                DateTime writeDate = File.GetLastWriteTimeUtc(path);
                if (writeDate != _lastWriteDate)
                {
                    string? code = null;
                    try
                    {
                        using var stream = new FileStream(
                            path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);

                        code = new StreamReader(stream).ReadToEnd();
                    }
                    catch
                    {
                    }

                    if (code != null)
                    {
                        BuildText(code);
                        _lastWriteDate = writeDate;
                    }
                }
            }

            var currentViewport = GraphicsDevice.Viewport;
            if (_lastViewport != currentViewport)
            {
                ViewportChanged(currentViewport);
                _lastViewport = currentViewport;
            }

            GraphicsDevice.SetRenderTarget(_fontTarget, Color.Transparent.ToVector4());

            var viewPos = ViewPosition * _renderScale;
            _spriteBatch.Begin(
                //sortMode: SpriteSortMode.Deferred,
                blendState: BlendState.NonPremultiplied,
                transformMatrix:
                Matrix4x4.CreateScale(_renderScale * _scale) *
                Matrix4x4.CreateTranslation(viewPos.X, viewPos.Y, 0));

            if (true)
            {
                float rectPadding = 10;
                _spriteBatch.DrawRectangle(
                    new RectangleF(
                        _textPos - new Vector2(rectPadding),
                        _textSize + new SizeF(rectPadding * 2)),
                    Color.DarkBlue);

                for (int i = 0; i < _textGlyphs.Count; i++)
                {
                    var glyph = _textGlyphs[i];
                    _spriteBatch.Draw(glyph.FontGlyph.Texture, glyph.Quad);
                }
            }

            if (false)
            {
                var center = currentViewport.Bounds.Size.ToVector2() / 2f;

                float t = (float)time.Total.TotalSeconds * 0.01f;
                float range = 1000 * 100;

                int lines = 1000;
                for (int i = 0; i < lines; i++)
                {
                    float t1 = t + (i / (float)lines) * MathF.PI;
                    float t2 = t1 - MathF.PI;

                    var exter = center + new Vector2(MathF.Cos(t1), MathF.Sin(t1)) * range;
                    var inter = center + new Vector2(MathF.Cos(t2), MathF.Sin(t2)) * range;
                    _spriteBatch.DrawLine(inter, exter, Color.Red);
                }
            }

            if (false)
            {
                string sicc = _loremIpsumSlice
                    //"#h€jio? h½llå ön you dis\nis the fönt cäche thing workin\n" +
                    //"\u4E00\u4E01\u4E02\u4E03\u4E04\u4E05\u4E06\u4E07\u4E08\u4E09 \\n\n" +
                    //"\u4E10\u4E11\u4E12\u4E13\u4E14\u4E15\u4E16\u4E17\u4E18\u4E19 \\n\n" +
                    //"\uFFFD what" +
                    //"AWAY TO To Yo \n what / is dis \nnewline time" +
                    //""
                    ;

                var span = sicc.AsSpan();

                float spriteScale = (float)(Math.Sin(time.Total.TotalSeconds * 0.5)) * 2;
                float requestedPixelHeight = 16; // Math.Max(spriteScale + 2f, 0f) + 6f;

                float actualScaleF = _font.GetScaleByPixel(requestedPixelHeight);
                var actualScale = new Vector2(actualScaleF);

                _font.GetFontVMetrics(out int ascent, out int descent, out int lineGap);
                float lineHeight = (ascent - descent + lineGap) * actualScale.Y;

                //int lineCount = GetLineCount(span);
                //var basePos = new Vector2(10, _lastViewport.Height / 2f - lineHeight * (lineCount - 1) / 2f);
                var basePos = new Vector2(10, 10) + _scrollOffset;

                float x = basePos.X;
                float y = basePos.Y + ascent * actualScale.Y;

                while (Rune.DecodeFromUtf16(span, out Rune rune, out int consumed) == OperationStatus.Done)
                {
                    span = span.Slice(consumed);

                    if (rune.Value == '\n')
                    {
                        x = basePos.X;
                        y += lineHeight;
                    }
                    else
                    {
                        int glyphIndex = _font.GetGlyphIndex(rune.Value);

                        _font.GetGlyphHMetrics(glyphIndex, out int advanceWidth, out int leftSideBearing);

                        var glyph = _fontCache.GetGlyph(_font, glyphIndex, requestedPixelHeight);
                        if (glyph != null)
                        {
                            Vector2 scaleFactor = actualScale / glyph.Scale;
                            Rectangle texRect = glyph.TextureRect;

                            _font.GetGlyphBoxSubpixel(glyph.GlyphIndex, actualScale, default, out var actualGlyphBox);

                            var drawOrig = new Vector2(texRect.Width, texRect.Height) / 2;

                            var pos = new Vector2(
                                x + leftSideBearing * actualScale.X,
                                y + actualGlyphBox.Y);

                            var drawPos = pos + new Vector2(actualGlyphBox.Width / 2, actualGlyphBox.Height / 2);

                            //if (requestedPixelHeight > 24)
                            //    _spriteBatch.DrawRectangle(
                            //        new RectangleF(pos, actualGlyphBox.Size) +
                            //        new RectangleF(-1, -1, 2, 2), Color.Red);

                            _spriteBatch.Draw(
                                glyph.Texture, drawPos, texRect, Color.White,
                                0, drawOrig, scaleFactor, SpriteFlip.None, 0f);

                            //var str = glyphIndex.ToString();
                            //_spriteBatch.DrawString(_spriteFont, str, pos + new Vector2(-1, -1), Color.Black, 0, Vector2.Zero, 1, SpriteFlip.None, 1f);
                            //_spriteBatch.DrawString(_spriteFont, str, pos + new Vector2(1, 0), Color.Black, 0, Vector2.Zero, 1, SpriteFlip.None, 1f);
                            //_spriteBatch.DrawString(_spriteFont, str, pos + new Vector2(1, 1), Color.Black, 0, Vector2.Zero, 1, SpriteFlip.None, 1f);
                            //_spriteBatch.DrawString(_spriteFont, str, pos + new Vector2(-1, 0), Color.Black, 0, Vector2.Zero, 1, SpriteFlip.None, 1f);
                            //_spriteBatch.DrawString(_spriteFont, str, pos, Color.Red, 0, Vector2.Zero, 1, SpriteFlip.None, 2f);
                        }

                        x += advanceWidth * actualScale.X;

                        var nextRuneStatus = Rune.DecodeFromUtf16(span, out Rune nextRune, out int _);
                        if (nextRuneStatus == OperationStatus.Done)
                        {
                            int glyphIndex2 = _font.GetGlyphIndex(nextRune.Value);
                            int? kernAdvance = _font.GetGlyphKernAdvance(glyphIndex, glyphIndex2);

                            if (kernAdvance.HasValue)
                                x += kernAdvance.GetValueOrDefault() * actualScale.X;
                        }
                    }
                }

                //if (requestedPixelHeight > 24)
                //    _spriteBatch.DrawLine(basePos, new Vector2(x, y), Color.Green);

                _spriteBatch.DrawLine(basePos, 100, 0, Color.Red, 2);
            }
            _spriteBatch.End();

            GraphicsDevice.SetRenderTarget(null, new Color(Color.MediumPurple * 0.255f, 255).ToVector4());

            _spriteBatch.Begin(blendState: BlendState.AlphaBlend);
            _spriteBatch.Draw(_fontTarget, currentViewport.Bounds, _fontTarget.Bounds, Color.White);
            _spriteBatch.End();

            _spriteBatch.Begin(blendState: BlendState.AlphaBlend);
            //DrawDebug(new Vector2(3, 2));
            DrawDebug(new Vector2(4, _lastViewport.Height - 74));
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

        private void DrawShadedString(
            SpriteFont font, string value, Vector2 position,
            Color textColor, Color backgroundColor)
        {
            _spriteBatch.DrawString(font, value, position + new Vector2(1f), backgroundColor);
            _spriteBatch.DrawString(font, value, position, textColor);
        }

        public readonly struct TextGlyph
        {
            public FontGlyph FontGlyph { get; }
            public SpriteQuad Quad { get; }

            public TextGlyph(
                FontGlyph fontGlyph,
                SpriteQuad quad)
            {
                FontGlyph = fontGlyph ?? throw new ArgumentNullException(nameof(fontGlyph));
                Quad = quad;
            }
        }

        public class TextLine
        {
            public ReadOnlyMemory<TextGlyph> Glyphs { get; }


        }
    }
}