using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using MonoGame.Framework;
using MonoGame.Framework.Audio;
using MonoGame.Framework.Graphics;
using MonoGame.Framework.Input;

namespace MonoGame.Testing
{
    public partial class GameCard21 : Game
    {
        private GraphicsDeviceManager _graphicsManager;
        private SpriteBatch _spriteBatch;

        private Texture2D _pixel;
        private SpriteFont _spriteFont;

        private Viewport _lastViewport;

        private Dictionary<string, TextureRegion2D> _plainCardRegions;
        private Dictionary<string, TextureRegion2D> _hdCardRegions;

        public Color ClearColor { get; } = new Color(Color.DarkGreen * 0.333f, byte.MaxValue);

        public GameCard21()
        {
            SoundEffect.Initialize();

            _graphicsManager = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        protected override void Initialize()
        {
            Window.AllowUserResizing = true;
            IsMouseVisible = true;

            base.Initialize();

            _lastViewport = GraphicsDevice.Viewport;
            ViewportChanged(_lastViewport);
        }

        private void ViewportChanged(in Viewport viewport)
        {
        }

        private float _loadCount;
        private float _loadTotal;

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            _pixel = new Texture2D(GraphicsDevice, 1, 1);
            _pixel.SetData(stackalloc Color[] { Color.White });

            _spriteFont = Content.Load<SpriteFont>("consolas");

            Task.Run(() =>
            {
                try
                {
                    var atlas = new TextureAtlas(this);
                    var packStates = atlas.CreateCardAtlas(
                        (count, total) =>
                        {
                            _loadCount = count;
                            _loadTotal = total;
                        },
                        out var plainMap, out var hdMap);

                    _loadTotal = 0;


                    var textures = packStates.Select(state =>
                    {
                        using (var img = state.Image)
                        {
                            var tex = new Texture2D(GraphicsDevice, state.ActualWidth, state.ActualHeight);
                            tex.SetData(img, tex.Bounds);
                            return (state, tex);
                        }
                    }).ToArray();

                    _plainCardRegions = TextureAtlas.GetCardRegions(plainMap, textures);
                    _hdCardRegions = TextureAtlas.GetCardRegions(hdMap, textures);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
            });
        }

        public class PlayState
        {
            public List<Card> ActiveCards { get; }

            public PlayState()
            {
                ActiveCards = new List<Card>();
            }

            public int GetScore()
            {
                int score = 0;
                foreach (var card in ActiveCards)
                    score += card.Value;
                return score;
            }
        }

        [Flags]
        public enum CardType
        {
            Unknown = 0,

            Constant = 1 << 0,
            Clubs = 1 << 1 | Constant,
            Diamonds = 1 << 2 | Constant,
            Hearts = 1 << 3 | Constant,
            Spades = 1 << 4 | Constant,

            Special = 1 << 5,
            Ace = 1 << 6 | Special,
            Jack = 1 << 7 | Special,
            King = 1 << 8 | Special,
            Queen = 1 << 9 | Special,
            Joker = 1 << 10 | Special
        }

        public class Card
        {
            public CardType Type { get; }
            public int Value { get; }

            public Card(CardType type, int value)
            {
                Type = type;
                Value = value;
            }
        }

        protected override void UnloadContent()
        {
        }

        protected override void Update(in FrameTime time)
        {
            var keyboard = Keyboard.GetState();
            if (keyboard.IsKeyDown(Keys.Escape))
                Exit();

            var mouse = Mouse.GetState();



            base.Update(time);
        }

        protected override void Draw(in FrameTime time)
        {
            var currentViewport = GraphicsDevice.Viewport;
            if (_lastViewport != currentViewport)
            {
                ViewportChanged(currentViewport);
                _lastViewport = currentViewport;
            }

            GraphicsDevice.Clear(ClearColor);

                DrawLoadingBar(currentViewport);

            float seconds = (float)time.Total.TotalSeconds;

            _spriteBatch.Begin(
                sortMode: SpriteSortMode.FrontToBack,
                blendState: BlendState.NonPremultiplied);

            var cardRegions = _hdCardRegions;
            if (cardRegions != null)
            {
                var region = cardRegions["10_of_clubs"];

                //for (int i = 0; i < cards.Length; i++)
                //{
                //
                    var origin = new Vector2(region.Width / 2, region.Height / 2);
                    var scale = new Vector2(1f);
                    _spriteBatch.Draw(
                        region,
                        new Vector2(0, 0) + origin * scale,
                        new Color(Color.White, 1f),
                        0,
                        origin,
                        scale,
                        SpriteFlip.None,
                        0);
                //}
            }

            _spriteBatch.End();

            base.Draw(time);
        }

        private void DrawLoadingBar(in Viewport viewport)
        { 
            if (_loadTotal != 0)
            {
                _spriteBatch.Begin();

                float progress = _loadCount / _loadTotal;
                _spriteBatch.FillRectangle(
                    0, 0, viewport.Width * progress, 50, Color.Blue);

                _spriteBatch.End();

                Window.TaskbarList.ProgressState = Framework.Utilities.TaskbarProgressState.Normal;
                Window.TaskbarList.SetProgressValue((long)(100 * _loadCount), (long)(100 * _loadTotal));
            }
            else
            {
                Window.TaskbarList.ProgressState = Framework.Utilities.TaskbarProgressState.None;
            }
        }

        private string[] cardNames = new string[]
        {
            "10_of_clubs",
            "10_of_diamonds",
            "10_of_hearts",
            "10_of_spades",

            "9_of_clubs",
            "9_of_diamonds",
            "9_of_hearts",
            "9_of_spades",

            "8_of_clubs",
            "8_of_diamonds",
            "8_of_hearts",
            "8_of_spades",

            "7_of_clubs",
            "7_of_diamonds",
            "7_of_hearts",
            "7_of_spades",

            "6_of_clubs",
            "6_of_diamonds",
            "6_of_hearts",
            "6_of_spades",

            "5_of_clubs",
            "5_of_diamonds",
            "5_of_hearts",
            "5_of_spades",

            "4_of_clubs",
            "4_of_diamonds",
            "4_of_hearts",
            "4_of_spades",

            "3_of_clubs",
            "3_of_diamonds",
            "3_of_hearts",
            "3_of_spades",

            "2_of_clubs",
            "2_of_diamonds",
            "2_of_hearts",
            "2_of_spades",

            "ace_of_clubs",
            "ace_of_diamonds",
            "ace_of_hearts",
            "ace_of_spades"
        };
    }
}
