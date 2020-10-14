using System;
using System.Collections.Generic;
using System.IO;
using MonoGame.Framework;
using MonoGame.Imaging;
using MonoGame.Imaging.Processing;
using Newtonsoft.Json;

namespace MonoGame.Testing
{
    public delegate void TextureAtlasProgress(float count, float total);

    public static class TextureAtlas
    {
        public static List<PackState> GetAtlas(
            IReadOnlyCollection<string> files,
            string cachePath,
            int maxTextureSize,
            ImageFormat format,
            TextureAtlasProgress? progress)
        {
            if (format == null)
                throw new ArgumentNullException(nameof(format));

            string atlasStatesFile = Path.Combine(cachePath, "states.json");
            if (File.Exists(atlasStatesFile))
            {
                // TODO: validate relevancy

                string statesJson = File.ReadAllText(atlasStatesFile);
                var cachedPackStates = JsonConvert.DeserializeObject<List<PackState>>(statesJson);
                for (int i = 0; i < cachedPackStates.Count; i++)
                {
                    var state = cachedPackStates[i];
                    string cacheImageFile = Path.Combine(cachePath, state.Id + format.Extension);
                    using var fs = File.OpenRead(cacheImageFile);
                    state.Image = Image.Load<Color>(fs, onProgress: (d, p, r) =>
                    {
                        progress?.Invoke(p * (i + 1f) / cachedPackStates.Count, cachedPackStates.Count);
                    });
                }
                return cachedPackStates;
            }

            using var images = GetImages(files).GetEnumerator();
            var createProgress = progress != null ? (i => progress!.Invoke(i, files.Count)) : (Action<int>?)null;
            var packStates = CreateAtlas(images, maxTextureSize, createProgress);

            Directory.CreateDirectory(cachePath);
            File.WriteAllText(atlasStatesFile, JsonConvert.SerializeObject(packStates));
            foreach (var state in packStates)
            {
                string cacheImageFile = Path.Combine(cachePath, state.Id + format.Extension);
                using var fs = new FileStream(cacheImageFile, FileMode.Create);
                state.Image.ProjectRows(x => x.Crop(0, 0, state.ActualWidth, state.ActualHeight)).Save(fs, format);
            }

            return packStates;
        }

        public static List<PackState> CreateAtlas(
            IEnumerator<(string Name, Image<Color> Image)> images,
            int maxTextureSize,
            Action<int>? progress)
        {
            var packStates = new List<PackState>();
            int stateIndex = 0;

            int index = 0;
            bool hasValue = false;
            do
            {
                if (stateIndex >= packStates.Count)
                {
                    packStates.Add(new PackState(id: stateIndex.ToString())
                    {
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

                    while (hasValue || images.MoveNext())
                    {
                        hasValue = true;
                        var (file, rawImage) = images.Current;

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
                        progress?.Invoke(index);

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

            return packStates;
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

        public class PackState
        {
            [JsonIgnore]
            public Image<Color>? Image { get; set; }

            public string Id { get; }
            public Dictionary<string, Rectangle> Entries { get; }
            public int ActualWidth { get; set; }
            public int ActualHeight { get; set; }

            [JsonConstructor]
            public PackState(string id, Dictionary<string, Rectangle> entries, int actualWidth, int actualHeight)
            {
                Id = id ?? throw new ArgumentNullException(nameof(id));
                Entries = entries ?? throw new ArgumentNullException(nameof(entries));
                ActualWidth = actualWidth;
                ActualHeight = actualHeight;
            }

            public PackState(string id)
            {
                Id = id ?? throw new ArgumentNullException(nameof(id));
                Entries = new Dictionary<string, Rectangle>();
            }
        }
    }
}
