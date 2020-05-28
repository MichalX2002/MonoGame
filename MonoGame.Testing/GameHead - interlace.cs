using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MonoGame.Framework;
using MonoGame.Framework.Graphics;
using MonoGame.Framework.Input;
using MonoGame.Imaging;
using MonoGame.Imaging.Codecs.Decoding;

namespace MonoGame.Testing
{
    public class GameHead : Game
    {
        private GraphicsDeviceManager _graphicsManager;
        private SpriteBatch _spriteBatch;

        private SpriteFont _font;

        private Image _image;
        private Texture2D _tex;

        private int _lastRowFilled = 0;
        private int _lastRow = 0;
        private bool _finished = false;

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

            Task.Run(() =>
            {
                try
                {
                    Thread.Sleep(500);

                    var ww = new Stopwatch();

                    var http = new HttpClient();
                    
                    for (int i = 0; i < 1; i++)
                    {
                        using (var fs = new FileStream(
                            //@"C:\Users\Michal Piatkowski\Pictures\my-mind-is-like-an-internet-browser.jpg",
                            //"../../very big interlace.png",
                            "../../very_big_interlace pog.jpg",
                            FileMode.Open, FileAccess.Read, FileShare.Read, 1024 * 4))
                        //using(var fs = await http.GetStreamAsync(
                        //    "https://upload.wikimedia.org/wikipedia/commons/3/3d/LARGE_elevation.jpg"))
                        {
                            ww.Restart();
                            var x = Image.Load<Color>(fs, onProgress: OnProgress);

                            _finished = true;
                            ww.Stop();
                            Console.WriteLine("Time: " + ww.Elapsed.TotalMilliseconds + "ms");

                            Console.WriteLine(x.Width + "x" + x.Height);

                            Thread.Sleep(100);

                            //x.Save("very big recoded.png", ImageFormat.Png);
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
                    var span = ((Image<Color>)image).GetPixelByteSpan();
                    tex.SetData(span.Slice(image.ByteStride * _lastRowFilled), rect);
                }
                _lastRowFilled = lastRow;
            }

            _spriteBatch.Begin(samplerState: SamplerState.LinearClamp);
            if (tex != null)
            {
                float rot = 0; // -MathF.PI / 2;
                float scale = 1 / 5.5f; // 5.5f
                var pos = new Vector2(0, 0); // tex.Width * scale);

                _spriteBatch.Draw(
                    tex, pos, null, Color.White, rot, Vector2.Zero, scale, SpriteEffects.None, 0);
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
                var str =
                    $"Memory: {Math.Round(totalMb, totalMbDecimals).ToString("0." + new string('0', totalMbDecimals))}M \n" +
                    $"GC Heap: {Math.Round(gcMb, gcMbDecimals).ToString("0." + new string('0', gcMbDecimals))}M \n" +
                    $"GC Counts: 0>{gc0} 1>{gc1} 2>{gc2}";

                _spriteBatch.DrawString(_font, str, new Vector2(-1, -1), Color.White);
                _spriteBatch.DrawString(_font, str, new Vector2(1, 1), Color.Black);
                _spriteBatch.DrawString(_font, str, new Vector2(0, 0), Color.Cyan);
            }

            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
