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
            MediaPlayer.Pitch = -1f;
            MediaPlayer.IsRepeating = true;
        
            MediaPlayer.Play(_songs);

            MediaPlayer.IsVisualizationEnabled = true;
            _visData = new VisualizationData(VisualizationData.MAX_SAMPLES / 2);
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
                //Content.Load<Song>("sinus"),
                Content.Load<Song>("Alphys Takes Action"),
                //Content.Load<Song>("Ambitions & Illusions"),
                //Content.Load<Song>("Creation From Another Era"),
                //Content.Load<Song>("Run with Me")
            };

            /*
            var def = Microphone.Default;
            def.BufferReady += Def_BufferReady;
            
            def.Start();
            */
        }

        private void Def_BufferReady(Microphone source, int sampleCount)
        {
            byte[] data = new byte[sampleCount * sizeof(short)];
            int readSamples = source.GetData(data) / sizeof(short);
            short[] audio = new short[readSamples];

            Buffer.BlockCopy(data, 0, audio, 0, readSamples * sizeof(short));
            samples.InsertRange(0, audio);

            const int threshold = 4410 * 15;
            if(samples.Count > threshold)
            {
                int diff = samples.Count - threshold;
                samples.RemoveRange(samples.Count - diff, diff);
            }
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
                _hej = 0;
            }

            //MediaPlayer.GetVisualizationData(_visData);

            base.Update(time);
        }

        const float baseScale = 1f;

        List<short> samples = new List<short>();

        protected override void Draw(GameTime time)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            float yOrigin = GraphicsDevice.Viewport.Height / 2f;

            _spriteBatch.Begin();

            for (int i = 0; i < _visData.Samples.Count; i += 16)
            {
                float x = i * baseScale / 56f + 10;

                float sample = _visData.Samples[i];
                //float sample = samples[i] / short.MaxValue;

                DrawLine(sample, i, x, yOrigin);
            }

            _spriteBatch.End();

            base.Draw(time);
        }

        void DrawLine(float sample, int i, float x, float yOrigin)
        {
            float scl = sample * 299f + baseScale;
            float yOff = scl > 0 ? -scl : 0;

            var pos = new Vector2(x, yOrigin + yOff);
            var color = new Color(255 - i / 150 + 20, 255 - i / 200, 255 - 150 - i / 150);
            var scale = new Vector2(baseScale + 1f, Math.Abs(scl));

            _spriteBatch.Draw(_pixel, pos, null, color, 0, Vector2.Zero, scale, SpriteEffects.None, 0);
        }
    }
}
