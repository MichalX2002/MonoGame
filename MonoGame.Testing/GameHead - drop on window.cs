using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
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

        private float _zmusc;

        public GameHead()
        {
            _graphicsManager = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        protected override void Initialize()
        {
            Window.AllowUserResizing = true;
            IsMouseVisible = true;

            Window.FilesDropped += Window_FilesDropped;

            base.Initialize();
        }

        private void Window_FilesDropped(GameWindow window, List<string> files)
        {
            Console.WriteLine(files.Count);
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

            _zmusc += gameTime.ElapsedTotalSeconds;

            if(_zmusc >= 5f)
            {
                Window.AllowFileDropping = !Window.AllowFileDropping;
                Console.WriteLine(Window.AllowFileDropping);

                _zmusc = 0;
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.White);

            _spriteBatch.Begin(samplerState: SamplerState.LinearClamp);

            var text = "hello";
            var textPos = new Vector2(20, 20);
            var textColor = Color.Blue;

            text += ": " + _font.MeasureString(text);

            _spriteBatch.DrawString(_font, text, textPos - new Vector2(1), Color.White);
            _spriteBatch.DrawString(_font, text, textPos + new Vector2(1), new Color(textColor * 0.1f, 255));
            _spriteBatch.DrawString(_font, text, textPos, textColor);

            _spriteBatch.DrawString(_font, _zmusc.ToString(), textPos + new Vector2(0, 50), Color.Red);

            _spriteBatch.End();

            base.Draw(gameTime);
        }


    }
}
