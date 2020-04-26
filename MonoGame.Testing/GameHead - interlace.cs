using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MonoGame.Framework;
using MonoGame.Framework.Graphics;
using MonoGame.Imaging;
using MonoGame.Imaging.Coding.Decoding;

namespace MonoGame.Testing
{
    public class GameHead : Game
    {
        private GraphicsDeviceManager _graphicsManager;
        private SpriteBatch _spriteBatch;

        private Image _image;
        private Texture2D _tex;

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
                _image = decoderState.CurrentImage;
                (_image as Image<Color>)?.GetPixelSpan().Fill(Color.Red);
                Console.WriteLine("progress");
            }

            Task.Run(() =>
            {
                try
                {
                    Thread.Sleep(500);
                    var x = Image.Load(File.OpenRead("../../very big interlace.png"), onProgress: OnProgress);
                    Console.WriteLine(x.Width + "x" + x.Height);
                    Thread.Sleep(500);

                    //x.Save("very big recoded.png", ImageFormat.Png);
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

            base.LoadContent();
        }

        protected override void UnloadContent()
        {
            base.UnloadContent();
        }

        protected override void Update(GameTime gameTime)
        {

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.White);

            if (_image != null)
            {
                if (_tex == null)
                {
                    var surfaceFormat = Texture2D.GetVectorFormat(_image.PixelType.Type).SurfaceFormat;
                    _tex = new Texture2D(GraphicsDevice, _image.Width, _image.Height, false, surfaceFormat);
                }

                _tex.SetData(((Image<Color>)_image).GetPixelSpan());
            }

            _spriteBatch.Begin(samplerState: SamplerState.PointClamp);
            if (_tex != null)
            {
                float rot = -MathF.PI / 2;
                float scale = 1 / 5.5f;
                var pos = new Vector2(0, _tex.Width * scale);

                _spriteBatch.Draw(
                    _tex, pos, null, Color.White, rot, Vector2.Zero, scale, SpriteEffects.None, 0);
            }
            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
