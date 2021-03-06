﻿using System;
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
using MonoGame.Imaging.Processing;

namespace MonoGame.Testing
{
    public class GameImageInterlace : Game
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

            void OnLoadProgress(IImageDecoder decoder, double percentage, Rectangle? rect)
            {
                if (_image == null && decoder.CurrentImage != null)
                {
                    _image = decoder.CurrentImage;
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

            //_tex = Texture2D.FromStream(
            //    //File.OpenRead(@"C:\Users\Michal Piatkowski\Pictures\my-mind-is-like-an-internet-browser.jpg"),
            //    //File.OpenRead("../../../very big interlace.bmp"),
            //    File.OpenRead("../../../very big interlace.png"),
            //    GraphicsDevice);

            void LoadBody()
            {
                var ww = new Stopwatch();

                var web = new WebClient();

                string[] saveFormats = new string[]
                {
                        ".png",
                        ".jpeg",
                        ".tga",
                    // ".bmp"
                };

                using (var fs = new FileStream(
                    //"../../../very big base.jpg",
                    //"../../../very big prog.jpg",
                    "../../../very big interlace.png",
                    //"../../../very_big_interlace pog.jpg",    
                    FileMode.Open, FileAccess.Read, FileShare.Read, 1024 * 8))
                //using(var fs = web.OpenRead(
                //    "https://upload.wikimedia.org/wikipedia/commons/3/3d/LARGE_elevation.jpg"))
                {
                    for (int i = 0; i < saveFormats.Length; i++)
                    {
                        Thread.Sleep(500);
                        fs.Seek(0, SeekOrigin.Begin);

                        ww.Restart();
                        var img = Image.Load<Color>(fs, onProgress: OnLoadProgress);

                        _finished = true;
                        ww.Stop();
                        Console.WriteLine("Time: " + ww.Elapsed.TotalMilliseconds + "ms");

                        Console.WriteLine(img.Width + "x" + img.Height);

                        var lastRow = img.GetPixelRowSpan(img.Height - 1);
                        Console.WriteLine(lastRow[0]);

                        Thread.Sleep(500);

                        //_tex = Texture2D.FromImage(img, GraphicsDevice);

                        //return;
                        //
                        //var newSize = new Size(img.Width, img.Height / 3);
                        //var procsed = img.ProcessRows(c => c.Resize(newSize, newSize, OnResizeProgress));
                        //
                        //if (false)
                        {
                            string saveFormat = saveFormats[i];
                            string savepath = $"recoded{saveFormat}";
                            Console.WriteLine($"saving {saveFormat}...");
                            ww.Restart();
                            img.Save(savepath);
                            ww.Stop();
                            Console.WriteLine("saved: " + ww.Elapsed.TotalMilliseconds + "ms");

                            Thread.Sleep(500);

                            Console.WriteLine($"Reloading saved {saveFormat}...");
                            var xd = Image.Load<Color>(File.OpenRead(savepath));
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

                        Console.WriteLine();
                    }
                }
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
