using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.IO;
using System.IO.Enumeration;
using System.Linq;
using System.Linq.Expressions;
using System.Numerics;
using System.Runtime;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MonoGame.Framework;
using MonoGame.Framework.Graphics;
using MonoGame.Framework.Input;
using MonoGame.Framework.Vectors;
using MonoGame.Imaging;

namespace MonoGame.TestingC
{
    public class GameHead : Game
    {
        private GraphicsDeviceManager _graphicsManager;
        private SpriteBatch _spriteBatch;

        private Viewport _lastViewport;

        private static string _directory = @"C:\Users\Michal Piatkowski\Pictures";
        private List<string> _entries = new List<string>();
        private List<string> _readyEntries = new List<string>();
        private Task _searchTask;

        private bool _loadPreviews;
        private Thread _previewLoadThread;
        private CancellationTokenSource _previewLoadCancellation = new CancellationTokenSource();


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

            _searchTask = Task.Run(() =>
            {
                try
                {
                    var entryBuffer = new List<string>();

                    var watch = new Stopwatch();

                    watch.Start();

                    var lastElapsed = watch.Elapsed;

                    foreach (var entry in new FileSystemEnumerable<string>(
                        _directory,
                        (ref FileSystemEntry x) =>
                        {
                            int diff = x.Directory.Length - x.RootDirectory.Length;
                            var dirSep = Path.DirectorySeparatorChar;
                            string fileName = x.FileName.ToString();
                            return diff > 0
                                ? x.Directory.Slice(x.RootDirectory.Length + 1).ToString() + dirSep + fileName
                                : fileName;
                        },
                        new EnumerationOptions
                        {
                            IgnoreInaccessible = true,
                            AttributesToSkip = FileAttributes.System,
                            RecurseSubdirectories = true
                        })
                    {
                        ShouldIncludePredicate = (ref FileSystemEntry x) =>
                        {
                            return !x.IsDirectory && (x.FileName.EndsWith(".bmp") || x.FileName.EndsWith(".png"));
                        }
                    })
                    {
                        entryBuffer.Add(entry);

                        // Check buffer count to not buffer massive amounts
                        // and time to show slowly loading entries.
                        if (entryBuffer.Count >= 1024 * 8 ||
                            (watch.Elapsed - lastElapsed).TotalSeconds > 0.2)
                        {
                            lock (_readyEntries)
                                _readyEntries.AddRange(entryBuffer);

                            entryBuffer.Clear();
                            lastElapsed = watch.Elapsed;
                        }
                    }

                    watch.Stop();

                    lock (_readyEntries)
                        _readyEntries.AddRange(entryBuffer);
                    entryBuffer.Clear();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
            });

            _previewLoadThread = new Thread(PreviewLoadThread);
            _previewLoadThread.Start();
        }

        private void PreviewLoadThread()
        {
            _loadPreviews = true;
            while (_loadPreviews)
            {
                while (_previewLoadQueue.TryDequeue(out var frame))
                {
                    try
                    {
                        frame.LoadPreviewImage(_previewLoadCancellation.Token);
                    }
                    catch (OperationCanceledException)
                    {
                        Console.WriteLine("Preview load cancelled");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(
                            "Failed to load preview for \"" + frame.FileName + "\": " + ex.Message);
                    }
                }

                Thread.Sleep(5);
            }
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            base.LoadContent();
        }

        protected override void UnloadContent()
        {
            _loadPreviews = false;

            ClearFrames();

            base.UnloadContent();
        }

        private void ViewportChanged(in Viewport viewport)
        {
            _moved = true;
        }

        protected override void Update(GameTime gameTime)
        {
            var keyboard = Keyboard.GetState();
            if (keyboard.IsKeyDown(Keys.Escape))
            {
                Exit();
                return;
            }

            var viewport = GraphicsDevice.Viewport;
            if (_lastViewport != viewport)
            {
                ViewportChanged(viewport);
                _lastViewport = viewport;
            }

            if (_searchTask != null)
            {
                lock (_readyEntries)
                {
                    if (_readyEntries.Count > 0)
                    {
                        Console.WriteLine("Files found: " + _readyEntries.Count);

                        _entries.AddRange(_readyEntries);
                        _readyEntries.Clear();

                        if (_searchTask.IsCompleted)
                        {
                            Console.WriteLine("Total files found: " + _entries.Count);

                            _searchTask = null;
                        }
                    }
                }
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            if (_moved)
            {
                UpdateFrames((_lastViewport.Bounds.Size.ToSizeF() * 1f / renderscale).ToSize());
                _moved = false;
            }

            DrawFrames(_spriteBatch);

            DrawOverlay(_spriteBatch, gameTime);

            base.Draw(gameTime);
        }

        // make these into dynamic variables
        const int frameTextHeight = 40;

        const int frameWidth = 150;
        const int frameHeight = 125 + frameTextHeight;

        const int frameSpacingX = 5;
        const int frameSpacingY = 5;

        private bool _moved = true;
        private List<FrameLayout> _frames = new List<FrameLayout>();
        private ConcurrentQueue<FrameLayout> _previewLoadQueue = new ConcurrentQueue<FrameLayout>();
        const int uploadsPerFrame = 10;

        static float renderscale = 0.5f;
        static Matrix4x4 transformation => Matrix4x4.CreateScale(renderscale);

        private void ClearFrames()
        {
            _previewLoadQueue.Clear();

            _previewLoadCancellation.Cancel();
            _previewLoadCancellation = new CancellationTokenSource();

            foreach (var frame in _frames)
                frame.Dispose();

            _frames.Clear();
        }

        private void UpdateFrames(Size viewportSize)
        {
            ClearFrames();

            int fullFrameWidth = frameWidth + frameSpacingX;
            int fullFrameHeight = frameHeight + frameSpacingY;

            int framesPerRow = viewportSize.Width / fullFrameWidth;
            int framesPerColumn = viewportSize.Height / fullFrameHeight;

            int x = 0;
            int y = 0;

            foreach (var entry in _entries)
            {
                var pos = new Point(x * fullFrameWidth, y * fullFrameHeight);
                var size = new Size(frameWidth, frameHeight);

                var bounds = new Rectangle(pos, size);
                var layout = new FrameLayout(entry, bounds);
                _frames.Add(layout);

                if (++x >= framesPerRow)
                {
                    x = 0;

                    if (++y >= framesPerColumn)
                        break;
                }
            }
        }

        private void DrawOverlay(SpriteBatch spriteBatch, GameTime time)
        {
            spriteBatch.Begin(
                SpriteSortMode.FrontToBack, BlendState.NonPremultiplied, SamplerState.LinearClamp,
                transformMatrix: transformation);

            float delta = Math.Min(time.ElapsedTotalSeconds, 0.1f);

            foreach (var frame in _frames)
            {
                if (frame.IsPreviewFaulted)
                    continue;

                if (frame.IsPreviewRequested && frame.PreviewTexture == null)
                {
                    frame.AnimationTime += delta;
                    float animX = frame.AnimationTime * 2.5f;
                    float alpha = animX - 0.25f;
                    if (alpha > 0.05f)
                    {
                        var boundsRect = frame.BoundsRect;
                        var center = boundsRect.Position + boundsRect.Size / 2;

                        _ = new Size() / 2;
                        _ = new Size() / 2f;
                        _ = new Size() * 2;
                        _ = new Size() * 2f;

                        _ = new SizeF() + new Size();
                        _ = new SizeF() + new Point();
                        _ = new SizeF() + new Vector2();
                        _ = new SizeF() / 2f;
                        _ = new SizeF() * 2f;

                        _ = new PointF() + new Size();
                        _ = new PointF() + new Point();
                        _ = new PointF() + new Vector2();

                        float radius1 = MathF.Sin(animX) * 10 + 15;
                        float radius2 = MathF.Cos(animX + MathF.PI / 2) * 10 + 15;

                        var color = new Color(Color.White, alpha);
                        spriteBatch.DrawCircle(center, radius1, 40, color, 2);
                        spriteBatch.DrawCircle(center, radius2, 40, color, 2);
                    }
                }
            }

            spriteBatch.End();
        }

        private void DrawFrames(SpriteBatch spriteBatch)
        {
            spriteBatch.Begin(
                SpriteSortMode.FrontToBack, BlendState.NonPremultiplied, SamplerState.LinearClamp,
                transformMatrix: transformation);

            int uploadsLeft = uploadsPerFrame;

            foreach (var frame in _frames)
            {
                DrawFrame(spriteBatch, frame, ref uploadsLeft);
            }

            spriteBatch.End();
        }

        private void DrawFrame(SpriteBatch spriteBatch, FrameLayout frame, ref int uploadsLeft)
        {
            if (!frame.IsPreviewRequested)
            {
                frame.IsPreviewRequested = true;

                _previewLoadQueue.Enqueue(frame);
            }
            else if (frame.HasPreviewImage && uploadsLeft > 0)
            {
                frame.UploadPreviewImage(GraphicsDevice);
                uploadsLeft--;

                if (frame.PreviewTexture != null)
                {
                    var previewTextureBounds = frame.PreviewTexture.Bounds;
                    var previewBoundsRect = frame.BoundsRect;
                    previewBoundsRect.Height -= frameTextHeight;

                    RectangleF previewRect;

                    if (previewTextureBounds.Width > previewBoundsRect.Width ||
                        previewTextureBounds.Height > previewBoundsRect.Height)
                    {
                        float widthRatio = previewBoundsRect.Width / (float)previewTextureBounds.Width;
                        float heightRatio = previewBoundsRect.Height / (float)previewTextureBounds.Height;
                        float ratio = Math.Min(widthRatio, heightRatio);

                        previewRect.Width = previewTextureBounds.Width * ratio;
                        previewRect.Height = previewTextureBounds.Height * ratio;
                    }
                    else
                    {
                        previewRect.Width = previewTextureBounds.Width;
                        previewRect.Height = previewTextureBounds.Height;
                    }

                    previewRect.X = previewBoundsRect.X + (previewBoundsRect.Width - previewRect.Width) / 2;
                    previewRect.Y = previewBoundsRect.Y + (previewBoundsRect.Height - previewRect.Height) / 2;

                    frame.PreviewRect = previewRect.ToRectangle();
                }
            }

            if (frame.PreviewTexture != null && frame.PreviewRect.HasValue)
            {
                spriteBatch.Draw(frame.PreviewTexture, frame.PreviewRect.Value, Color.White);
            }

            var boundsRecte = frame.BoundsRect;
            boundsRecte.X -= 1;
            boundsRecte.Y -= 1;
            boundsRecte.Width += 2;
            boundsRecte.Height += 2;
            spriteBatch.DrawRectangle(boundsRecte, Color.Blue);
        }

        public class FrameLayout : IDisposable
        {
            private static DecoderOptions DecoderOptions { get; } = new DecoderOptions
            {
                ClearImageMemory = true
            };

            public string FileName { get; }
            public Rectangle BoundsRect { get; }

            public bool IsDisposed { get; set; }
            public bool IsPreviewRequested { get; set; }
            public bool IsPreviewLoading { get; set; }
            public bool IsPreviewLoadFinished { get; set; }
            public bool IsPreviewFaulted { get; set; }

            public Image<Color> PreviewImage { get; private set; }
            public Texture2D PreviewTexture { get; private set; }
            public Rectangle? PreviewRect { get; set; }

            public float AnimationTime { get; set; }

            public bool HasPreviewImage => PreviewImage != null && IsPreviewLoadFinished;

            public FrameLayout(string fileName, Rectangle boundsRect)
            {
                FileName = fileName ?? throw new ArgumentNullException(nameof(fileName));
                BoundsRect = boundsRect;
            }

            public void LoadPreviewImage(CancellationToken cancellationToken)
            {
                if (IsPreviewLoading || IsPreviewLoadFinished)
                    throw new InvalidOperationException();

                try
                {
                    IsPreviewLoading = true;

                    using (var fs = File.OpenRead(_directory + "/" + FileName))
                    {
                        var pixelType = VectorType.Get<Color>();
                        var image = Image.Load(fs, pixelType, DecoderOptions, null, cancellationToken);
                        PreviewImage = (Image<Color>)image;
                    }
                }
                catch
                {
                    IsPreviewFaulted = true;
                    throw;
                }
                finally
                {
                    IsPreviewLoading = false;
                    IsPreviewLoadFinished = true;
                }
            }

            public void UploadPreviewImage(GraphicsDevice graphicsDevice)
            {
                var image = PreviewImage;
                if (image == null)
                    throw new InvalidOperationException();

                if (IsPreviewFaulted)
                {
                    PreviewImage = null;
                    return;
                }

                bool loadFinished = IsPreviewLoadFinished;

                try
                {
                    if (PreviewTexture == null)
                        PreviewTexture = new Texture2D(
                            graphicsDevice, image.Width, image.Height);

                    PreviewTexture.SetData(image.GetPixelSpan());
                }
                finally
                {
                    if (loadFinished)
                        DisposePreviewImage();
                }
            }

            public void DisposePreviewImage()
            {
                PreviewImage?.Dispose();
                PreviewImage = null;
            }

            public void Dispose()
            {
                DisposePreviewImage();

                PreviewTexture?.Dispose();
                PreviewTexture = null;
            }
        }
    }
}
