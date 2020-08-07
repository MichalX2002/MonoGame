using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
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

namespace MonoGame.Testing
{
    public class GameImageInterlace : Game
    {
        private GraphicsDeviceManager _graphicsManager;
        private SpriteBatch _spriteBatch;

        private SpriteFont _font;

        private StringBuilder _debugBuilder = new StringBuilder();

        private Image _image;
        private Texture2D _tex;

        private int _lastRowFilled = 0;
        private int _lastRow = 0;
        private bool _finished = false;

        public GameImageInterlace()
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

            void OnProgress(ImageDecoderState decoderState, double percentage, Rectangle? rect)
            {
                if (_image == null && decoderState.CurrentImage != null)
                {
                    _image = decoderState.CurrentImage;
                    (_image as Image<Color>)?.GetPixelSpan().Fill(Color.Red);
                    Console.WriteLine("setup");
                }

                _lastRow = rect?.Y ?? 0;
            }

            _tex = Texture2D.FromStream(
                //File.OpenRead(@"C:\Users\Michal Piatkowski\Pictures\my-mind-is-like-an-internet-browser.jpg"),
                File.OpenRead("../../../very big interlace.bmp"),
                GraphicsDevice);

            Task.Run(() =>
            {
                return;
                try
                {
                    var ww = new Stopwatch();

                    var http = new HttpClient();

                    for (int i = 0; i < 1; i++)
                    {
                        using (var fs = new FileStream(
                            @"C:\Users\Michal Piatkowski\Pictures\my-mind-is-like-an-internet-browser.jpg",
                            //"../../../very big interlace.png",
                            //"../../../very_big_interlace pog.jpg",
                            FileMode.Open, FileAccess.Read, FileShare.Read, 1024 * 8))
                        //using(var fs = await http.GetStreamAsync(
                        //    "https://upload.wikimedia.org/wikipedia/commons/3/3d/LARGE_elevation.jpg"))
                        {
                            Thread.Sleep(1000);

                            ww.Restart();
                            var x = Image.Load<Color>(fs, onProgress: OnProgress);

                            _finished = true;
                            ww.Stop();
                            Console.WriteLine("Time: " + ww.Elapsed.TotalMilliseconds + "ms");

                            Thread.Sleep(1000);

                            Console.WriteLine(x.Width + "x" + x.Height);

                            var lastRow = x.GetPixelRowSpan(x.Height - 1);
                            Console.WriteLine(lastRow[0]);

                            if (true)
                            {
                                Console.WriteLine("saving png...");
                                ww.Restart();
                                x.Save("very big recoded.png", ImageFormat.Png);
                                ww.Stop();
                                Console.WriteLine("saved: " + ww.Elapsed.TotalMilliseconds + "ms");

                                Thread.Sleep(1000);

                                Console.WriteLine("Reloading saved png...");
                                var xd = Image.Load<Color>(File.OpenRead("very big recoded.png"));
                                Console.WriteLine("Reloaded");
                                //
                                //var reLastRow = xd.GetPixelRowSpan(xd.Height - 1);
                                //Console.WriteLine(reLastRow[0]);

                                //Console.WriteLine("saving reloaded png...");
                                //ww.Restart();
                                //x.Save("very big reloaded.png", ImageFormat.Png);
                                //ww.Stop();
                                //Console.WriteLine("saved: " + ww.Elapsed.TotalMilliseconds + "ms");
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
            });
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

        protected override void Update(GameTime gameTime)
        {
            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
            {
                Exit();
                return;
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.White);

            var tex = _tex;
            var image = _image;
            if (image != null)
            {
                if (tex == null)
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
                float scale = 1;// / 5.5f;
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

                //var str =
                //    $"Memory: {Math.Round(totalMb, totalMbDecimals).ToString("0." + new string('0', totalMbDecimals))}M \n" +
                //    $"GC Heap: {Math.Round(gcMb, gcMbDecimals).ToString("0." + new string('0', gcMbDecimals))}M \n" +
                //    $"GC Counts: [{gc0}, {gc1}, {gc2}]";

                RuneEnumerator e = _debugBuilder;
                _spriteBatch.DrawString(_font, e, new Vector2(-1, -1), Color.White);
                _spriteBatch.DrawString(_font, e, new Vector2(1, 1), Color.Black);
                _spriteBatch.DrawString(_font, e, new Vector2(0, 0), Color.Cyan);
                _debugBuilder.Clear();
            }

            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
