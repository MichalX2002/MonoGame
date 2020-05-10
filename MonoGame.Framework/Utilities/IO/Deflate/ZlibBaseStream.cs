// ZlibBaseStream.cs
// ------------------------------------------------------------------
//
// Copyright (c) 2009 Dino Chiesa and Microsoft Corporation.
// All rights reserved.
//
// This code module is part of DotNetZip, a zipfile class library.
//
// ------------------------------------------------------------------
//
// This code is licensed under the Microsoft Public License.
// See the file License.txt for the license details.
// More info on: http://dotnetzip.codeplex.com
//
// ------------------------------------------------------------------
//
// last saved (in emacs):
// Time-stamp: <2011-August-06 21:22:38>
//
// ------------------------------------------------------------------
//
// This module defines the ZlibBaseStream class, which is an intnernal
// base class for DeflateStream, ZlibStream and GZipStream.
//
// ------------------------------------------------------------------

using System;
using System.Buffers.Binary;
using System.IO;
using System.Text;
using MonoGame.Framework.Memory;

namespace MonoGame.Framework.Utilities.Deflate
{
    internal enum ZlibStreamFlavor
    {
        ZLIB = 1950,
        DEFLATE = 1951,
        GZIP = 1952
    }

    internal class ZlibBaseStream : Stream
    {
        protected internal ZlibCodec _z = null; // deferred init... new ZlibCodec();

        protected internal StreamMode _streamMode = StreamMode.Undefined;
        protected internal FlushType _flushMode;
        protected internal ZlibStreamFlavor _flavor;
        protected internal CompressionMode _compressionMode;
        protected internal CompressionLevel _level;
        protected internal bool _leaveOpen;
        protected internal byte[] _workingBuffer;
        protected internal int _bufferSize = ZlibConstants.WorkingBufferSizeDefault;

        protected internal Stream _stream;
        protected internal CompressionStrategy Strategy = CompressionStrategy.Default;

        protected internal string _GzipFileName;
        protected internal string _GzipComment;
        protected internal DateTime _GzipMtime;
        protected internal int _gzipHeaderByteCount;

        private bool nomoreinput = false;
        private CRC32 _crc32; // workitem 7159

        internal int Crc32 => _crc32.Crc32Result;

        public ZlibBaseStream(
            Stream stream,
            CompressionMode compressionMode,
            CompressionLevel level,
            ZlibStreamFlavor flavor,
            bool leaveOpen)
            : base()
        {
            _flushMode = FlushType.None;
            //this._workingBuffer = new byte[WORKING_BUFFER_SIZE_DEFAULT];
            _stream = stream;
            _leaveOpen = leaveOpen;
            _compressionMode = compressionMode;
            _flavor = flavor;
            _level = level;

            if (flavor == ZlibStreamFlavor.GZIP)
                _crc32 = new CRC32(reverseBits: false);
        }

        protected internal bool WantCompress => _compressionMode == CompressionMode.Compress;

        private ZlibCodec Codec
        {
            get
            {
                if (_z == null)
                {
                    _z = new ZlibCodec();

                    bool wantRfc1950Header = _flavor == ZlibStreamFlavor.ZLIB;
                    if (_compressionMode == CompressionMode.Decompress)
                    {
                        _z.InitializeInflate(wantRfc1950Header);
                    }
                    else
                    {
                        _z.Strategy = Strategy;
                        _z.InitializeDeflate(_level, wantRfc1950Header);
                    }
                }
                return _z;
            }
        }

        private byte[] WorkingBuffer
        {
            get
            {
                if (_workingBuffer == null)
                    _workingBuffer = new byte[_bufferSize];
                return _workingBuffer;
            }
        }


        public override void Write(byte[] buffer, int offset, int count)
        {
            // workitem 7159
            // calculate the CRC on the unccompressed data  (before writing)
            _crc32.SlurpBlock(buffer.AsSpan(offset, count));

            if (_streamMode == StreamMode.Undefined)
                _streamMode = StreamMode.Writer;
            else if (_streamMode != StreamMode.Writer)
                throw new ZlibException("Cannot Write after Reading.");

            if (count == 0)
                return;

            // first reference of z property will initialize the private var _z
            Codec.InputBuffer = buffer;
            _z.NextIn = offset;
            _z.AvailableBytesIn = count;

            bool done;
            do
            {
                _z.OutputBuffer = WorkingBuffer;
                _z.NextOut = 0;
                _z.AvailableBytesOut = _workingBuffer.Length;
                int rc = WantCompress
                    ? _z.Deflate(_flushMode)
                    : _z.Inflate(_flushMode);
                if (rc != ZlibConstants.Z_OK && rc != ZlibConstants.Z_STREAM_END)
                    throw new ZlibException((WantCompress ? "de" : "in") + "flating: " + _z.Message);

                //if (_workingBuffer.Length - _z.AvailableBytesOut > 0)
                _stream.Write(_workingBuffer, 0, _workingBuffer.Length - _z.AvailableBytesOut);

                done = _z.AvailableBytesIn == 0 && _z.AvailableBytesOut != 0;

                // If GZIP and de-compress, we're done when 8 bytes remain.
                if (_flavor == ZlibStreamFlavor.GZIP && !WantCompress)
                    done = _z.AvailableBytesIn == 8 && _z.AvailableBytesOut != 0;

            }
            while (!done);
        }



        private void Finish()
        {
            if (_z == null)
                return;

            if (_streamMode == StreamMode.Writer)
            {
                bool done = false;
                do
                {
                    _z.OutputBuffer = WorkingBuffer;
                    _z.NextOut = 0;
                    _z.AvailableBytesOut = _workingBuffer.Length;
                    int rc = WantCompress
                        ? _z.Deflate(FlushType.Finish)
                        : _z.Inflate(FlushType.Finish);

                    if (rc != ZlibConstants.Z_STREAM_END && rc != ZlibConstants.Z_OK)
                    {
                        string verb = (WantCompress ? "de" : "in") + "flating";
                        if (_z.Message == null)
                            throw new ZlibException(string.Format("{0}: (rc = {1})", verb, rc));
                        else
                            throw new ZlibException(verb + ": " + _z.Message);
                    }

                    if (_workingBuffer.Length - _z.AvailableBytesOut > 0)
                        _stream.Write(_workingBuffer, 0, _workingBuffer.Length - _z.AvailableBytesOut);

                    done = _z.AvailableBytesIn == 0 && _z.AvailableBytesOut != 0;
                    // If GZIP and de-compress, we're done when 8 bytes remain.
                    if (_flavor == ZlibStreamFlavor.GZIP && !WantCompress)
                        done = _z.AvailableBytesIn == 8 && _z.AvailableBytesOut != 0;

                }
                while (!done);

                Flush();

                // workitem 7159
                if (_flavor == ZlibStreamFlavor.GZIP)
                {
                    if (WantCompress)
                    {
                        Span<byte> tmp = stackalloc byte[sizeof(int)];

                        // Emit the GZIP trailer: CRC32 and  size mod 2^32
                        int c1 = _crc32.Crc32Result;
                        BinaryPrimitives.WriteInt32LittleEndian(tmp, c1);
                        _stream.Write(tmp);

                        int c2 = (int)(_crc32.TotalBytesRead & 0x00000000FFFFFFFF);
                        BinaryPrimitives.WriteInt32LittleEndian(tmp, c2);
                        _stream.Write(tmp);
                    }
                    else
                    {
                        throw new ZlibException("Writing with decompression is not supported.");
                    }
                }
            }
            // workitem 7159
            else if (_streamMode == StreamMode.Reader)
            {
                if (_flavor == ZlibStreamFlavor.GZIP)
                {
                    if (!WantCompress)
                    {
                        // workitem 8501: handle edge case (decompress empty stream)
                        if (_z.TotalBytesOut == 0L)
                            return;

                        // Read and potentially verify the GZIP trailer:
                        // CRC32 and size mod 2^32
                        Span<byte> trailer = stackalloc byte[8];

                        // workitems 8679 & 12554
                        if (_z.AvailableBytesIn < 8)
                        {
                            // Make sure we have read to the end of the stream
                            _z.InputBuffer.AsSpan(_z.NextIn, _z.AvailableBytesIn).CopyTo(trailer);
                            int bytesNeeded = 8 - _z.AvailableBytesIn;
                            int bytesRead = _stream.Read(trailer.Slice(_z.AvailableBytesIn, bytesNeeded));
                            if (bytesNeeded != bytesRead)
                            {
                                throw new ZlibException(string.Format(
                                    "Missing or incomplete GZIP trailer. Expected 8 bytes, got {0}.",
                                    _z.AvailableBytesIn + bytesRead));
                            }
                        }
                        else
                        {
                            _z.InputBuffer.AsSpan(_z.NextIn, trailer.Length).CopyTo(trailer);
                        }

                        int crc32_expected = BinaryPrimitives.ReadInt32LittleEndian(trailer);
                        int crc32_actual = _crc32.Crc32Result;
                        int isize_expected = BinaryPrimitives.ReadInt32LittleEndian(trailer.Slice(4));
                        int isize_actual = (int)(_z.TotalBytesOut & 0x00000000FFFFFFFF);

                        if (crc32_actual != crc32_expected)
                            throw new ZlibException(string.Format(
                                "Bad CRC32 in GZIP trailer. (actual({0:X8})!=expected({1:X8}))",
                                crc32_actual, crc32_expected));

                        if (isize_actual != isize_expected)
                            throw new ZlibException(string.Format(
                                "Bad size in GZIP trailer. (actual({0})!=expected({1}))",
                                isize_actual, isize_expected));

                    }
                    else
                    {
                        throw new ZlibException("Reading with compression is not supported.");
                    }
                }
            }
        }


        private void End()
        {
            if (Codec == null)
                return;

            if (WantCompress)
            {
                _z.EndDeflate();
            }
            else
            {
                _z.EndInflate();
            }
            _z = null;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_stream == null) return;
                try
                {
                    Finish();
                }
                finally
                {
                    End();
                    if (!_leaveOpen) _stream.Dispose();
                    _stream = null;
                }
            }
        }

        public override void Flush()
        {
            _stream.Flush();
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotImplementedException();
            //_outStream.Seek(offset, origin);
        }
        public override void SetLength(long value)
        {
            _stream.SetLength(value);
        }


        //public int Read()
        //{
        //
        //    if (Read(_buf1, 0, 1) == 0)
        //        return 0;
        //    // calculate CRC after reading
        //    _crc32.SlurpBlock(_buf1,0,1);
        //    return (_buf1[0] & 0xFF);
        //}

        private string ReadZeroTerminatedString()
        {
            var list = new System.Collections.Generic.List<byte>();
            bool done = false;
            do
            {
                // workitem 7740
                Span<byte> buffer = stackalloc byte[1];
                int n = _stream.Read(buffer);
                if (n != 1)
                    throw new ZlibException("Unexpected EOF reading GZIP header.");
                else
                {
                    if (buffer[0] == 0)
                        done = true;
                    else
                        list.Add(buffer[0]);
                }
            } while (!done);
            byte[] a = list.ToArray();
            return GZipStream.iso8859dash1.GetString(a, 0, a.Length);
        }


        private int ReadAndValidateGzipHeader()
        {
            // read the header on the first read
            Span<byte> header = stackalloc byte[10];
            int n = _stream.Read(header);

            // workitem 8501: handle edge case (decompress empty stream)
            if (n == 0)
                return 0;

            if (n != 10)
                throw new ZlibException("Not a valid GZIP stream.");

            if (header[0] != 0x1F || header[1] != 0x8B || header[2] != 8)
                throw new ZlibException("Bad GZIP header.");

            int timet = BinaryPrimitives.ReadInt32LittleEndian(header.Slice(4));
            _GzipMtime = GZipStream._unixEpoch.AddSeconds(timet);

            int totalBytesRead = n;
            if ((header[3] & 0x04) == 0x04)
            {
                // read and discard extra field
                n = _stream.Read(header.Slice(0, 2)); // 2-byte length field
                totalBytesRead += n;

                short extraLength = (short)(header[0] + header[1] * 256);
                byte[] extra = new byte[extraLength];
                n = _stream.Read(extra, 0, extra.Length);
                if (n != extraLength)
                    throw new ZlibException("Unexpected end-of-file reading GZIP header.");
                totalBytesRead += n;
            }
            if ((header[3] & 0x08) == 0x08)
                _GzipFileName = ReadZeroTerminatedString();
            if ((header[3] & 0x10) == 0x010)
                _GzipComment = ReadZeroTerminatedString();
            if ((header[3] & 0x02) == 0x02)
                ReadByte(); // CRC16, ignore

            return totalBytesRead;
        }



        public override int Read(byte[] buffer, int offset, int count)
        {
            // According to MS documentation, any implementation of the IO.Stream.Read function must:
            // (a) throw an exception if offset & count reference an invalid part of the buffer,
            //     or if count < 0, or if buffer is null
            // (b) return 0 only upon EOF, or if count = 0
            // (c) if not EOF, then return at least 1 byte, up to <count> bytes

            if (_streamMode == StreamMode.Undefined)
            {
                if (!_stream.CanRead) throw new ZlibException("The stream is not readable.");
                // for the first read, set up some controls.
                _streamMode = StreamMode.Reader;
                // (The first reference to _z goes through the private accessor which
                // may initialize it.)
                Codec.AvailableBytesIn = 0;
                if (_flavor == ZlibStreamFlavor.GZIP)
                {
                    _gzipHeaderByteCount = ReadAndValidateGzipHeader();
                    // workitem 8501: handle edge case (decompress empty stream)
                    if (_gzipHeaderByteCount == 0)
                        return 0;
                }
            }

            if (_streamMode != StreamMode.Reader)
                throw new ZlibException("Cannot Read after Writing.");

            if (count == 0) 
                return 0;
            if (nomoreinput && WantCompress)
                return 0;  // workitem 8557
            if (buffer == null) 
                throw new ArgumentNullException(nameof(buffer));
            if (count < 0) 
                throw new ArgumentOutOfRangeException(nameof(count));
            if (offset < buffer.GetLowerBound(0))
                throw new ArgumentOutOfRangeException(nameof(offset));
            if (offset + count > buffer.GetLength(0))
                throw new ArgumentOutOfRangeException(nameof(count));

            // set up the output of the deflate/inflate codec:
            _z.OutputBuffer = buffer;
            _z.NextOut = offset;
            _z.AvailableBytesOut = count;

            // This is necessary in case _workingBuffer has been resized. (new byte[])
            // (The first reference to _workingBuffer goes through the private accessor which
            // may initialize it.)
            _z.InputBuffer = WorkingBuffer;

            int rc;
            do
            {
                // need data in _workingBuffer in order to deflate/inflate.  Here, we check if we have any.
                if (_z.AvailableBytesIn == 0 && !nomoreinput)
                {
                    // No data available, so try to Read data from the captive stream.
                    _z.NextIn = 0;
                    _z.AvailableBytesIn = _stream.Read(_workingBuffer, 0, _workingBuffer.Length);
                    if (_z.AvailableBytesIn == 0)
                        nomoreinput = true;

                }
                // we have data in InputBuffer; now compress or decompress as appropriate
                rc = WantCompress
                    ? _z.Deflate(_flushMode)
                    : _z.Inflate(_flushMode);

                if (nomoreinput && rc == ZlibConstants.Z_BUF_ERROR)
                    return 0;

                if (rc != ZlibConstants.Z_OK && rc != ZlibConstants.Z_STREAM_END)
                    throw new ZlibException(string.Format(
                        "{0}flating:  rc={1}  msg={2}", 
                        WantCompress ? "de" : "in", rc, _z.Message));

                if ((nomoreinput || rc == ZlibConstants.Z_STREAM_END) &&
                    _z.AvailableBytesOut == count)
                    break; // nothing more to read
            }
            //while (_z.AvailableBytesOut == count && rc == ZlibConstants.Z_OK);
            while (_z.AvailableBytesOut > 0 && !nomoreinput && rc == ZlibConstants.Z_OK);


            // workitem 8557
            // is there more room in output?
            if (_z.AvailableBytesOut > 0)
            {
                if (rc == ZlibConstants.Z_OK && _z.AvailableBytesIn == 0)
                {
                    // deferred
                }

                // are we completely done reading?
                if (nomoreinput)
                {
                    // and in compression?
                    if (WantCompress)
                    {
                        // no more input data available; therefore we flush to
                        // try to complete the read
                        rc = _z.Deflate(FlushType.Finish);

                        if (rc != ZlibConstants.Z_OK && rc != ZlibConstants.Z_STREAM_END)
                            throw new ZlibException(string.Format(
                                "Deflating:  rc={0}  msg={1}", rc, _z.Message));
                    }
                }
            }


            rc = count - _z.AvailableBytesOut;

            // calculate CRC after reading
            _crc32.SlurpBlock(buffer.AsSpan(offset, rc));

            return rc;
        }

        public override bool CanRead => _stream.CanRead;
        public override bool CanSeek => _stream.CanSeek;
        public override bool CanWrite => _stream.CanWrite;
        public override long Length => _stream.Length;

        public override long Position
        {
            get => throw new NotImplementedException();
            set => throw new NotImplementedException();
        }

        internal enum StreamMode
        {
            Writer,
            Reader,
            Undefined,
        }

        public static void CompressString(ReadOnlySpan<char> value, Stream compressor)
        {
            using (var writer = new StreamWriter(compressor, Encoding.UTF8, 1024, leaveOpen: true))
                writer.Write(value);
        }

        public static string UncompressString(Stream decompressor)
        {
            // workitem 8460

            var sr = new StreamReader(decompressor, Encoding.UTF8, true, 1024, leaveOpen: true);
            return sr.ReadToEnd();
        }

        public static RecyclableMemoryStream UncompressBuffer(Stream decompressor)
        {
            // workitem 8460
            var output = RecyclableMemoryManager.Default.GetMemoryStream();

            Span<byte> working = stackalloc byte[2048];
            int n;
            while ((n = decompressor.Read(working)) != 0)
                output.Write(working.Slice(0, n));

            return output;
        }
    }
}
