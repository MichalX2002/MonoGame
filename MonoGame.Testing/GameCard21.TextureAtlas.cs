using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using MonoGame.Framework;
using MonoGame.Framework.Graphics;
using MonoGame.Imaging;
using MonoGame.Imaging.Processing;
using Newtonsoft.Json;

namespace MonoGame.Testing
{
    public partial class GameCard21
    {
        public delegate void AtlasProgressDelegate(float count, float total);

        public class TextureAtlas
        {
            public GameCard21 Game { get; }

            public TextureAtlas(GameCard21 game)
            {
                Game = game ?? throw new ArgumentNullException(nameof(game));
            }

            public static Dictionary<string, TextureRegion2D> GetCardRegions(
                Dictionary<string, string> map, (PackState State, Texture2D Texture)[] textures)
            {
                var regions = new Dictionary<string, TextureRegion2D>(map.Count);
                foreach (var (key, value) in map)
                {
                    foreach (var (state, texture) in textures)
                    {
                        if (state.Entries.TryGetValue(value, out Rectangle bounds))
                        {
                            regions.Add(key, new TextureRegion2D(texture, bounds));
                            break;
                        }
                    }
                }
                return regions;
            }

            public List<PackState> CreateCardAtlas(
                AtlasProgressDelegate? progress,
                out Dictionary<string, string> plainMap,
                out Dictionary<string, string> hdMap)
            {
                string cardsPath = Path.Combine(Game.Content.RootDirectory, "Cards");
                string[] cardFiles = Directory.GetFiles(cardsPath);

                // plainMap will contain all basic textures at first.
                plainMap = new Dictionary<string, string>();
                foreach (string cardFile in cardFiles)
                {
                    var fileName = Path.GetFileNameWithoutExtension(cardFile);
                    plainMap.Add(fileName, cardFile);
                }

                // hdMap will contain a mix of basic and HD textures.
                // plainMap will not contain HD textures.
                hdMap = new Dictionary<string, string>(plainMap);
                foreach (string key in plainMap.Keys.ToArray())
                {
                    string hdKey = key + "2";
                    if (hdMap.Remove(hdKey, out string? hdFile))
                    {
                        plainMap.Remove(hdKey);
                        hdMap.Remove(key);
                        hdMap.Add(key, hdFile);
                    }
                }

                string cachePath = Path.Combine(Game.Content.RootDirectory, "TextureCache");
                string atlasStatesFile = Path.Combine(cachePath, "states.json");
                if (File.Exists(atlasStatesFile))
                {
                    string statesJson = File.ReadAllText(atlasStatesFile);
                    var savedStates = JsonConvert.DeserializeObject<List<PackState>>(statesJson);
                    for (int i = 0; i < savedStates.Count; i++)
                    {
                        var state = savedStates[i];
                        string cacheImageFile = Path.Combine(cachePath, state.Id + ".png");
                        using var fs = File.OpenRead(cacheImageFile);
                        state.Image = Image.Load<Color>(fs, onProgress: (d, p, r) =>
                        {
                            progress?.Invoke(p * (i + 1f) / savedStates.Count, savedStates.Count);
                        });
                    }
                    return savedStates;
                }

                var packStates = new List<PackState>();
                int maxTextureSize = Math.Min(Game.GraphicsDevice.Capabilities.MaxTexture2DSize, 4096);
                int stateIndex = 0;

                int index = 0;
                using var cardImages = GetImages(cardFiles).GetEnumerator();
                bool hasValue = false;
                do
                {
                    if (stateIndex >= packStates.Count)
                    {
                        packStates.Add(new PackState
                        {
                            Id = stateIndex.ToString(),
                            Image = Image<Color>.Create(maxTextureSize, maxTextureSize)
                        });
                    }

                    var state = packStates[stateIndex];
                    state.Image.MutateBuffer(buffer =>
                    {
                        int padding = 2;
                        int x = padding;
                        int y = padding;
                        int largestHeight = 0;

                        while (hasValue || cardImages.MoveNext())
                        {
                            hasValue = true;
                            var (file, rawImage) = cardImages.Current;

                            int width = rawImage.Width / 2;
                            int height = rawImage.Height / 2;

                            int remainingHeight = buffer.Height - y;
                            if (remainingHeight < height)
                            {
                                stateIndex++;
                                break;
                            }

                            int remainingWidth = buffer.Width - x;
                            if (remainingWidth < width)
                                goto NextRow;

                            using (rawImage)
                            using (var image = rawImage.ProcessRows(
                                x => x.Resize(new Size(width, height), 0, null)))
                            {
                                Image.LoadPixels(image, buffer.Crop(x, y));
                            }

                            index++;
                            progress?.Invoke(index, cardFiles.Length);

                            state.Entries.Add(file, new Rectangle(x, y, width, height));

                            x += width + padding;
                            if (height > largestHeight)
                                largestHeight = height;

                            hasValue = false;
                            if (x < buffer.Width)
                                continue;

                            NextRow:
                            int filledWidth = x - padding;
                            if (filledWidth > state.ActualWidth)
                                state.ActualWidth = filledWidth;

                            y += largestHeight + padding;
                            largestHeight = 0;
                            x = padding;
                        }

                        int filledHeight = y + largestHeight;
                        if (filledHeight > state.ActualHeight)
                            state.ActualHeight = filledHeight;
                    });
                }
                while (hasValue);

                Directory.CreateDirectory(cachePath);
                File.WriteAllText(atlasStatesFile, JsonConvert.SerializeObject(packStates));
                foreach (var state in packStates)
                {
                    string cacheImageFile = Path.Combine(cachePath, state.Id + ".png");
                    using var fs = new FileStream(cacheImageFile, FileMode.Create);
                    state.Image.ProjectRows(x => x.Crop(0, 0, state.ActualWidth, state.ActualHeight)).Save(fs, ImageFormat.Png);
                }

                return packStates;
            }

            public class PackState
            {
                [JsonIgnore]
                public Image<Color>? Image { get; set; }

                public string Id { get; set; }
                public Dictionary<string, Rectangle>? Entries { get; set; } = new Dictionary<string, Rectangle>();
                public int ActualWidth { get; set; }
                public int ActualHeight { get; set; }
            }

            public static IEnumerable<(string, Image<Color>)> GetImages(IEnumerable<string> files)
            {
                if (files == null)
                    throw new ArgumentNullException(nameof(files));

                foreach (string file in files)
                {
                    Image<Color>? img;
                    using (var fs = File.OpenRead(file))
                        img = Image.Load<Color>(fs);

                    if (img == null)
                        throw new InvalidDataException("Could not decode " + file);

                    yield return (file, img);
                }
            }
        }
    }
}
