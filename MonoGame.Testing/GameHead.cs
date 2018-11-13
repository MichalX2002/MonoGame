using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
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
        private Texture2D _pixel;

        private SongCollection _songs;
        private VisualizationData _visData;
        
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
            MediaPlayer.Pitch = -0.5f;
            MediaPlayer.IsRepeating = true;

            MediaPlayer.Play(_songs);

            MediaPlayer.IsVisualizationEnabled = true;
            _visData = new VisualizationData();
        }

        private void MediaPlayer_ActiveSongChanged(object s, EventArgs e)
        {
            Console.WriteLine(MediaPlayer.Queue.ActiveSong.Name);
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            _pixel = new Texture2D(GraphicsDevice, 1, 1);
            _pixel.SetData(new Color[] { Color.White });

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

        private float _hej;
        
        protected override void Update(GameTime time)
        {
            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            _hej += time.Delta;
            if (_hej > 0)
            {
                MediaPlayer.GetVisualizationData(_visData);
                _hej = 0;
            }

            base.Update(time);
        }

        protected override void Draw(GameTime time)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            int yOrigin = GraphicsDevice.Viewport.Height / 2;

            _spriteBatch.Begin();

            float baseScale = 1;
            for (int i = 0; i < _visData.Samples.Count; i++)
            {
                var pos = new Vector2(i * (baseScale / 12 + 0) + 10, yOrigin);
                var color = new Color(i / 10 + 20, i / 15, 150 - i / 10);
                var scale = new Vector2(baseScale, _visData.Samples[i] * baseScale * 499 + baseScale);

                _spriteBatch.Draw(_pixel, pos, null, color, 0, Vector2.Zero, scale, SpriteEffects.None, 0);
            }

            _spriteBatch.End();

            base.Draw(time);
        }
    }
}
