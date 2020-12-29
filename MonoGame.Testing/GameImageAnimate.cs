using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Numerics;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.X86;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MonoGame.Framework;
using MonoGame.Framework.Graphics;
using MonoGame.Framework.Input;
using MonoGame.Framework.Vectors;
using MonoGame.Imaging;
using MonoGame.Imaging.Coders.Decoding;
using MonoGame.Imaging.Coders.Formats;
using MonoGame.Imaging.Processing;

namespace MonoGame.Testing
{
    public class GameImageAnimate : Game
    {
        private GraphicsDeviceManager _graphicsManager;
        private SpriteBatch _spriteBatch;

        private SpriteFont _font;

        private StringBuilder _debugBuilder = new StringBuilder();

        private Image? _image;
        private Texture2D _tex;

        private int _lastRowFilled;
        private int _lastRow;
        private bool _finished;

        float scale = 1f / 2f; // 1f / 10.5f;

        public GameImageAnimate()
        {
            _graphicsManager = new GraphicsDeviceManager(this);
            _graphicsManager.GraphicsProfile = GraphicsProfile.HiDef;

            Content.RootDirectory = "Content";
        }

        protected override void Initialize()
        {
            Window.AllowUserResizing = true;
            IsMouseVisible = true;

            base.Initialize();

            void OnLoadProgress(ImageDecoderState decoderState, float percentage, Rectangle? rect)
            {
                if (_image == null && decoderState.CurrentImage != null)
                {
                    _image = decoderState.CurrentImage;
                    (_image as Image<Color>)?.GetPixelSpan().Fill(Color.Transparent);
                    Console.WriteLine("load setup");
                }

                _lastRow = rect?.Y ?? 0;
            }

            void OnResizeProgress(Size state, float percentage, Rectangle? rect)
            {
                if (_image == null)
                {
                    _image = Image<Color>.Create(state);
                    (_image as Image<Color>)?.GetPixelSpan().Fill(Color.Transparent);
                    Console.WriteLine("resize setup");
                }

                _lastRow = rect?.Y ?? 0;
            }

            void LoadBody()
            {
                var ww = new Stopwatch();

                string[] imageFiles = Directory.GetFiles("../../../../frames");

                IEnumerable<Image<Color>> images =
                    imageFiles
                    .OrderBy(x => x, StringComparer.OrdinalIgnoreCase)
                    .Select(path => Image.Load<Color>(path) ?? throw new InvalidDataException());

                images.Save("output.gif");

                //for (int i = 0; i < images.Length; i++)
                //{
                //    ww.Restart();
                //    Image<Color> image = images[i];
                //
                //
                //
                //    ww.Stop();
                //    Console.WriteLine("Frame Encode Time: " + ww.Elapsed.TotalMilliseconds + "ms");
                //}
            }

            if (true)
            {
                LoadBody();
            }
            else
            {
                Task.Run(() =>
                {
                    try
                    {
                        LoadBody();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex);
                    }
                });
            }
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            _font = Content.Load<SpriteFont>("arial");

            base.LoadContent();
        }

        protected override void UnloadContent()
        {
            base.UnloadContent();
        }

        protected override void Update(in FrameTime time)
        {
            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
            {
                Exit();
                return;
            }

            base.Update(time);
        }

        protected override void Draw(in FrameTime time)
        {
            GraphicsDevice.Clear(Color.White);

            var tex = _tex;
            var image = _image;
            if (image != null)
            {
                if (tex == null || tex.Width != image.Width || tex.Height != image.Height)
                {
                    var surfaceFormat = Texture2D.GetVectorFormat(image.PixelType.Type).SurfaceFormat;
                    tex = new Texture2D(GraphicsDevice, image.Width, image.Height, false, surfaceFormat);
                    _tex = tex;
                }

                if (_finished)
                {
                    _lastRow = image.Height;
                    _lastRowFilled = 0;
                    _finished = false;

                    _image = null;
                }

                int lastRow = _lastRow;
                var rect = new Rectangle(0, _lastRowFilled, image.Width, lastRow - _lastRowFilled);
                if (rect.Height > 0)
                {
                    var rowSpan = image.GetPixelByteSpan().Slice(
                        image.ByteStride * _lastRowFilled,
                        image.ByteStride * rect.Height);

                    tex.SetData(rowSpan, rect);
                }
                _lastRowFilled = lastRow;
            }

            _spriteBatch.Begin(samplerState: SamplerState.LinearClamp);
            if (tex != null)
            {
                float rot = 0;// -MathF.PI / 2;
                float y = 0;// tex.Width * scale;
                var pos = new Vector2(0, y);

                _spriteBatch.Draw(
                    tex, pos, null, Color.White, rot, Vector2.Zero, scale, SpriteFlip.None, 0);
            }

            if (true)
            {
                long totalMem = Environment.WorkingSet;
                long gcMem = GC.GetTotalMemory(false);
                double totalMb = totalMem / (1024 * 1024.0);
                double gcMb = gcMem / (1024 * 1024.0);

                int gc0 = GC.CollectionCount(0);
                int gc1 = GC.CollectionCount(1);
                int gc2 = GC.CollectionCount(2);

                int totalMbDecimals = (int)Math.Max(0, 4 - Math.Log10(totalMb));
                int gcMbDecimals = (int)Math.Max(0, 4 - Math.Log10(gcMb));

                _debugBuilder.Append("Memory: ").Append(Math.Round(totalMb, totalMbDecimals)).AppendLine();
                _debugBuilder.Append("GC Heap: ").Append(Math.Round(gcMb, gcMbDecimals)).AppendLine();
                _debugBuilder.Append("GC Counts: [")
                    .Append(gc0).Append(',')
                    .Append(gc1).Append(',')
                    .Append(gc2).Append(']').AppendLine();

                RuneEnumerator e = _debugBuilder;
                _spriteBatch.DrawString(_font, e, new Vector2(-0.75f, -0.75f), Color.White);
                _spriteBatch.DrawString(_font, e, new Vector2(1, -0.75f), Color.Gray);
                _spriteBatch.DrawString(_font, e, new Vector2(-0.75f, 1), Color.Gray);
                _spriteBatch.DrawString(_font, e, new Vector2(1, 1), Color.Black);
                _spriteBatch.DrawString(_font, e, new Vector2(0, 0), Color.Cyan);
                _debugBuilder.Clear();
            }

            _spriteBatch.End();

            base.Draw(time);
        }
    }
}
