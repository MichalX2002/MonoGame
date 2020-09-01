using System;
using System.Numerics;
using MonoGame.Framework;
using MonoGame.Framework.Graphics;
using MonoGame.Framework.Input;

namespace MonoGame.Testing
{
    public class GameDropOnWindow : Game
    {
        private GraphicsDeviceManager _graphicsManager;
        private SpriteBatch _spriteBatch;

        private SpriteFont _font;

        public GameDropOnWindow()
        {
            _graphicsManager = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        protected override void Initialize()
        {
            Window.AllowUserResizing = true;
            IsMouseVisible = true;

            Window.FilesDropped += Window_FilesDropped;
            Window.TextDropped += Window_TextDropped;

            base.Initialize();
        }

        private void Window_FilesDropped(GameWindow window, FilesDroppedEventArgs ev)
        {
            Console.WriteLine("FilesDropped: " + ev.FilePaths.Length);
        }

        private void Window_TextDropped(GameWindow window, TextDroppedEventArgs ev)
        {
            Console.WriteLine("TextDropped: " + ev.Text);
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

            _spriteBatch.Begin(samplerState: SamplerState.LinearClamp);

            var text = "hello";
            var textPos = new Vector2(20, 20);
            var textColor = Color.Blue;

            text += ": " + _font.MeasureString(text);

            _spriteBatch.DrawString(_font, text, textPos - new Vector2(1), Color.White);
            _spriteBatch.DrawString(_font, text, textPos + new Vector2(1), new Color(textColor * 0.1f, 255));
            _spriteBatch.DrawString(_font, text, textPos, textColor);

            _spriteBatch.End();

            base.Draw(time);
        }


    }
}
