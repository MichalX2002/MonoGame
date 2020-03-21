using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Android.Media;

namespace Microsoft.Xna.Framework.Audio
{
    public class MediaCodecPool
    {
        private class Entry
        {
            public int LifeTime;
            public MediaCodec Codec { get; }

            public Entry(MediaCodec codec)
            {
                LifeTime = 0;
                Codec = codec;
            }
        }

        private static Dictionary<string, List<Entry>> _decoders = new Dictionary<string, List<Entry>>();
        private static Task _allocationManager;
        private const int _maxDecodersInPool = 3;
        private const int _decoderLifeTime = 10 * 1000; // millis
        private const int _tickTime = 50; // millis

        public static MediaCodec RentDecoder(string mime)
        {
            lock (_decoders)
            {
                if (_decoders.TryGetValue(mime, out List<Entry> codecs) && codecs.Count > 0)
                {
                    int index = codecs.Count - 1;
                    var item = codecs[index];
                    codecs.RemoveAt(index);
                    return item.Codec;
                }
                return MediaCodec.CreateDecoderByType(mime);
            }
        }

        public static void ReturnDecoder(string mime, MediaCodec decoder)
        {
            lock (_decoders)
            {
                if (!_decoders.TryGetValue(mime, out List<Entry> codecs))
                {
                    codecs = new List<Entry>();
                    _decoders.Add(mime, codecs);
                }

                if (codecs.Count > _maxDecodersInPool)
                {
                    decoder.Dispose();
                }
                else
                {
                    codecs.Add(new Entry(decoder));

                    if (_allocationManager == null || _allocationManager.IsCompleted)
                        _allocationManager = Task.Factory.StartNew(
                            RemoveUnusedCodecs, CancellationToken.None, TaskCreationOptions.LongRunning, TaskScheduler.Default);
                }
            }
        }

        private static void RemoveUnusedCodecs()
        {
            int inactivity = 0;
            while (true)
            {
                lock (_decoders)
                {
                    bool isEmpty = true;
                    foreach (var pair in _decoders)
                    {
                        var list = pair.Value;
                        if (list.Count <= 0)
                            continue;

                        for (int i = 0; i < list.Count; i++)
                        {
                            var item = list[i];
                            item.LifeTime += _tickTime;
                            if (item.LifeTime >= _decoderLifeTime)
                            {
                                item.Codec.Dispose();
                                list.RemoveAt(i);
                                i--;
                            }
                        }
                        isEmpty = false;
                    }

                    if (isEmpty)
                    {
                        inactivity += _tickTime;
                        if (inactivity >= _decoderLifeTime)
                            break;
                    }
                }
                Thread.Sleep(_tickTime);
            }
        }

    }
}