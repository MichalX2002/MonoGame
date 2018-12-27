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

        private short[] _dynamicBuff;
        private DynamicSoundEffectInstance _dynamicSound;
        
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

            //MediaPlayer.ActiveSongChanged += MediaPlayer_ActiveSongChanged;
            MediaPlayer.Volume = 0.01f;
            MediaPlayer.Pitch = 1f;
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

            //_dynamicBuff = new short[44100];
            //_dynamicSound = new DynamicSoundEffectInstance(44100, AudioChannels.Stereo);
            //_dynamicSound.BufferNeeded += _dynamicSound_BufferNeeded;
            //_dynamicSound.Play();

            _songs = new SongCollection
            {
                Content.Load<Song>("sinus"),
                //Content.Load<Song>("No More Magic")
            };

            /*
            var def = Microphone.Default;
            def.BufferReady += Def_BufferReady;
            
            def.Start();
            */
        }

        //private void _dynamicSound_BufferNeeded(object sender, EventArgs e)
        //{
        //    var instance = sender as DynamicSoundEffectInstance;
        //
        //    GenerateMusic(_dynamicBuff, _dynamicBuff.Length / 2);
        //    instance.SubmitBuffer(_dynamicBuff);
        //
        //    Console.WriteLine("Enqueued " + _dynamicBuff.Length + " samples, " + _dynamicSound.PendingBufferCount + " pending buffers");
        //}
        
        //int ix = 0;
        //int j = 0;
        //int a1, b1, a2, b2;
        //int c1, c2, d1, d2;
        //
        //void GenerateMusic(short[] buf, int len)
        //{
        //    //a1 = b1 = a2 = b2 = 0;
        //    //c1 = c2 = d1 = d2 = 0;
        //
        //    //int j = 0;
        //
        //    for (int i = 0; i < len; i++)
        //    {
        //        uint r;
        //        int v1, v2;
        //        v1 = v2 = (((j * ((j >> 12) ^ ((j >> 10 | j >> 12) & 26 & j >> 7))) & 128) + 128) << 15;
        //        r = FastRand();
        //        v1 += (int)r & 65535;
        //        v1 -= (int)r >> 16;
        //        r = FastRand();
        //        v2 += (int)r & 65535;
        //        v2 -= (int)r >> 16;
        //        b1 = v1 - a1 + ((b1 * 61 + 32) >> 6); a1 = v1;
        //        b2 = v2 - a2 + ((b2 * 61 + 32) >> 6); a2 = v2;
        //        c1 = (30 * (c1 + b1 + d1) + 32) >> 6; d1 = b1;
        //        c2 = (30 * (c2 + b2 + d2) + 32) >> 6; d2 = b2;
        //        v1 = (c1 + 128) >> 8;
        //        v2 = (c2 + 128) >> 8;
        //        buf[i * 2] = (short)(v1 > 32767 ? 32767 : (v1 < -32768 ? -32768 : v1));
        //        buf[i * 2 + 1] = (short)(v2 > 32767 ? 32767 : (v2 < -32768 ? -32768 : v2));
        //
        //        if (ix % 6 == 0)
        //            j++;
        //        ix++;
        //    }
        //}
        //
        //uint Rz = 0, Rw = 0;
        //uint FastRand()
        //{
        //    Rz = 36969 * (Rz & 65535) + (Rz >> 16);
        //    Rw = 18000 * (Rw & 65535) + (Rw >> 16);
        //    return (Rz << 16) + Rw;
        //}

        //private void Def_BufferReady(Microphone source, int sampleCount)
        //{
        //    byte[] data = new byte[sampleCount * sizeof(short)];
        //    int readSamples = source.GetData(data) / sizeof(short);
        //    short[] audio = new short[readSamples];
        //
        //    Buffer.BlockCopy(data, 0, audio, 0, readSamples * sizeof(short));
        //    samples.InsertRange(0, audio);
        //
        //    const int threshold = 4410 * 15;
        //    if(samples.Count > threshold)
        //    {
        //        int diff = samples.Count - threshold;
        //        samples.RemoveRange(samples.Count - diff, diff);
        //    }
        //}

        protected override void UnloadContent()
        {
        }
        
        protected override void Update(GameTime time)
        {
            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            //MediaPlayer.GetVisualizationData(_visData);

            base.Update(time);
        }

        const float baseScale = 1f;

        List<short> samples = new List<short>();

        protected override void Draw(GameTime time)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            //DrawAudioVis();

            base.Draw(time);
        }

        private void DrawAudioVis()
        {
            float yOrigin = GraphicsDevice.Viewport.Height / 2f;

            _spriteBatch.Begin();

            for (int i = 0; i < _visData.Samples.Count; i += 16)
            {
                float x = i * baseScale / 24f + 10;

                float sample = _visData.Samples[i];
                //float sample = samples[i] / short.MaxValue;

                DrawLine(sample, i, x, yOrigin);
            }

            _spriteBatch.End();
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
