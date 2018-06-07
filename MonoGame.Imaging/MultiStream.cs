using System;
using System.Collections.Generic;
using System.IO;

namespace MonoGame.Imaging
{
    internal class MultiStream : Stream
    {
        private Queue<Stream> _streamQueue;
        private int _position;

        public override bool CanRead => true;
        public override bool CanSeek => false;
        public override bool CanWrite => false;

        public override long Length
        {
            get
            {
                long length = 0;
                foreach (var stream in _streamQueue)
                    length += stream.Length;
                return length;
            }
        }

        public override long Position
        {
            get => _position;
            set => throw new NotSupportedException();
        }

        public MultiStream()
        {
            _streamQueue = new Queue<Stream>();
        }

        public MultiStream(Stream a, Stream b)
        {
            _streamQueue = new Queue<Stream>(2);
            _streamQueue.Enqueue(a);
            _streamQueue.Enqueue(b);
        }

        public void AddStream(Stream stream)
        {
            if (stream == null)
                throw new ArgumentNullException(nameof(stream));

            _streamQueue.Enqueue(stream);
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotSupportedException();
        }

        public override void SetLength(long value)
        {
            throw new NotSupportedException();
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            Start:
            if (_streamQueue.Count > 0)
            {
                int read = _streamQueue.Peek().Read(buffer, offset, count);
                if (read == 0)
                {
                    _streamQueue.Dequeue();
                    goto Start;
                }
                else
                {
                    _position += read;
                    return read;
                }
            }
            return 0;
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            throw new NotSupportedException();
        }

        public override void Flush()
        {

        }

        protected override void Dispose(bool disposing)
        {
            _streamQueue = null;
        }
    }
}
