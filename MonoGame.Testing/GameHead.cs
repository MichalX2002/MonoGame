using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;

namespace MonoGame.Testings
{
    public class GameHead : Game
    {
        private GraphicsDeviceManager _graphicsManager;
        private SpriteBatch _spriteBatch;

        private SongCollection _songs;
        
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

            MediaPlayer.ActiveSongChanged += MediaPlayer_ActiveSongChanged;
            MediaPlayer.Volume = 0.25f;
            MediaPlayer.Pitch = 1f;
            MediaPlayer.IsRepeating = true;

            MediaPlayer.Play(_songs);

            MediaPlayer.IsVisualizationEnabled = true;
        }

        private void MediaPlayer_ActiveSongChanged(object s, EventArgs e)
        {
            //Console.WriteLine(MediaPlayer.Queue.ActiveSong.Name);


        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            
            _songs = new SongCollection
            {
                Content.Load<Song>("Alphys Takes Action"),
                Content.Load<Song>("Ambitions & Illusions"),
                Content.Load<Song>("Creation From Another Era"),
                Content.Load<Song>("Run with Me")
            };
        }
        
        protected override void UnloadContent()
        {

        }

        private float t = 0;

        Queue<float> updates = new Queue<float>();
 
        protected override void Update(GameTime time)
        {
            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            t += time.Delta;
            if(t > 1f)
            {
                Console.WriteLine(MediaPlayer.IsRunningSlowly + " | " + MediaPlayer.UpdateTime);
                t = 0;
            }

            base.Update(time);
        }

        protected override void Draw(GameTime time)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            base.Draw(time);
        }
    }
}
