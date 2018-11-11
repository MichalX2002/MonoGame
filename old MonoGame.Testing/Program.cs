using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace MonoGame.Testing
{
    class Program
    {
        class GameFrame : Game
        {
            private readonly GraphicsDeviceManager _manager;
            private SpriteBatch _batch;

            public GameFrame()
            {
                _manager = new GraphicsDeviceManager(this);
            }

            protected override void Initialize()
            {
                base.Initialize();

                IsMouseVisible = true;
                Window.AllowUserResizing = true;
            }

            protected override void LoadContent()
            {
                base.LoadContent();

                _batch = new SpriteBatch(GraphicsDevice);


            }

            protected override void Draw(GameTime gameTime)
            {
                if (Keyboard.GetState().IsKeyDown(Keys.Escape))
                    Exit();

                GraphicsDevice.Clear(Color.CornflowerBlue);
                


                base.Draw(gameTime);
            }
        }

        static void Main(string[] args)
        {   
            using (var frame = new GameFrame())
                frame.Run();
        }
    }
}
