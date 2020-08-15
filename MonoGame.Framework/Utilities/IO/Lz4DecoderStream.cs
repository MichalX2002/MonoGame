// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

#define CHECK_EOF
//#define LOCAL_SHADOW

using System;
using System.IO;

namespace MonoGame.Framework
{
    internal class Lz4DecoderStream : Stream
    {
        private long _inputLength;
        private Stream _input;

        //because we might not be able to match back across invocations,
        //we have to keep the last window's worth of bytes around for reuse
        //we use a circular buffer for this - every time we write into this
        //buffer, we also write the same into our output buffer

        private const int DecBufLen = 65536;
        private const int DecBufMask = 0xFFFF;

        private const int InBufLen = 128;

        private readonly byte[] decodeBuffer = new byte[DecBufLen + InBufLen];
        private int decodeBufferPos, inBufPos, inBufEnd;

        //we keep track of which phase we're in so that we can jump right back
        //into the correct part of decoding

        private DecodePhase phase;

        public Lz4DecoderStream()
        {
        }

        public Lz4DecoderStream(Stream input, long inputLength = long.MaxValue)
        {
            Reset(input, inputLength);
        }

        public void Reset(Stream input, long inputLength = long.MaxValue)
        {
            _inputLength = inputLength;
            _input = input;

            phase = DecodePhase.ReadToken;

            decodeBufferPos = 0;

            litLen = 0;
            matLen = 0;
            matDst = 0;

            inBufPos = DecBufLen;
            inBufEnd = DecBufLen;
        }

        protected override void Dispose(bool disposing)
        {
            _input = null!;

            base.Dispose(disposing);
        }

        private enum DecodePhase
        {
            ReadToken,
            ReadExLiteralLength,
            CopyLiteral,
            ReadOffset,
            ReadExMatchLength,
            CopyMatch,
        }

        //state within interruptable phases and across phase boundaries is
        //kept here - again, so that we can punt out and restart freely

        private int litLen, matLen, matDst;

        public override int Read(byte[] buffer, int offset, int count)
        {
            return Read(buffer.AsSpan(offset, count));
        }

        public override int Read(Span<byte> buffer)
        {
            if (_input == null)
                throw new InvalidOperationException();

            int nRead, nToRead = buffer.Length;
            int offset = 0;

            var decBuf = decodeBuffer;

            //the stringy gotos are obnoxious, but their purpose is to
            //make it *blindingly* obvious how the state machine transitions
            //back and forth as it reads - remember, we can yield out of
            //this routine in several places, and we must be able to re-enter
            //and pick up where we left off!

#if LOCAL_SHADOW
            var phase = this.phase;
            var inBufPos = this.inBufPos;
            var inBufEnd = this.inBufEnd;
#endif
            switch (phase)
            {
                case DecodePhase.ReadToken:
                    goto readToken;

                case DecodePhase.ReadExLiteralLength:
                    goto readExLiteralLength;

                case DecodePhase.CopyLiteral:
                    goto copyLiteral;

                case DecodePhase.ReadOffset:
                    goto readOffset;

                case DecodePhase.ReadExMatchLength:
                    goto readExMatchLength;

                case DecodePhase.CopyMatch:
                    goto copyMatch;
            }

            readToken:
            int tok;
            if (inBufPos < inBufEnd)
            {
                tok = decBuf[inBufPos++];
            }
            else
            {
#if LOCAL_SHADOW
                this.inBufPos = inBufPos;
#endif

                tok = ReadByteCore();
#if LOCAL_SHADOW
                inBufPos = this.inBufPos;
                inBufEnd = this.inBufEnd;
#endif
#if CHECK_EOF
                if (tok == -1)
                    goto finish;
#endif
            }

            litLen = tok >> 4;
            matLen = (tok & 0xF) + 4;

            switch (litLen)
            {
                case 0:
                    phase = DecodePhase.ReadOffset;
                    goto readOffset;

                case 0xF:
                    phase = DecodePhase.ReadExLiteralLength;
                    goto readExLiteralLength;

                default:
                    phase = DecodePhase.CopyLiteral;
                    goto copyLiteral;
            }

            readExLiteralLength:
            int exLitLen;
            if (inBufPos < inBufEnd)
            {
                exLitLen = decBuf[inBufPos++];
            }
            else
            {
#if LOCAL_SHADOW
                this.inBufPos = inBufPos;
#endif
                exLitLen = ReadByteCore();
#if LOCAL_SHADOW
                inBufPos = this.inBufPos;
                inBufEnd = this.inBufEnd;
#endif

#if CHECK_EOF
                if (exLitLen == -1)
                    goto finish;
#endif
            }

            litLen += exLitLen;
            if (exLitLen == 255)
                goto readExLiteralLength;

            phase = DecodePhase.CopyLiteral;
            goto copyLiteral;

            copyLiteral:
            int nReadLit = litLen < nToRead ? litLen : nToRead;
            if (nReadLit != 0)
            {
                if (inBufPos + nReadLit <= inBufEnd)
                {
                    int ofs = 0;
                    for (int c = nReadLit; c-- != 0;)
                        buffer[ofs++] = decBuf[inBufPos++];

                    nRead = nReadLit;
                }
                else
                {
#if LOCAL_SHADOW
                    this.inBufPos = inBufPos;
#endif
                    nRead = ReadCore(buffer.Slice(offset, nReadLit));
#if LOCAL_SHADOW
                    inBufPos = this.inBufPos;
                    inBufEnd = this.inBufEnd;
#endif
#if CHECK_EOF
                    if (nRead == 0)
                        goto finish;
#endif
                }

                offset += nRead;
                nToRead -= nRead;

                litLen -= nRead;

                if (litLen != 0)
                    goto copyLiteral;
            }

            if (nToRead == 0)
                goto finish;

            phase = DecodePhase.ReadOffset;
            goto readOffset;

            readOffset:
            if (inBufPos + 1 < inBufEnd)
            {
                matDst = (decBuf[inBufPos + 1] << 8) | decBuf[inBufPos];
                inBufPos += 2;
            }
            else
            {
#if LOCAL_SHADOW
                this.inBufPos = inBufPos;
#endif
                matDst = ReadOffsetCore();
#if LOCAL_SHADOW
                inBufPos = this.inBufPos;
                inBufEnd = this.inBufEnd;
#endif
#if CHECK_EOF
                if (matDst == -1)
                    goto finish;
#endif
            }

            if (matLen == 15 + 4)
            {
                phase = DecodePhase.ReadExMatchLength;
                goto readExMatchLength;
            }
            else
            {
                phase = DecodePhase.CopyMatch;
                goto copyMatch;
            }

            readExMatchLength:
            int exMatLen;
            if (inBufPos < inBufEnd)
            {
                exMatLen = decBuf[inBufPos++];
            }
            else
            {
#if LOCAL_SHADOW
                this.inBufPos = inBufPos;
#endif
                exMatLen = ReadByteCore();
#if LOCAL_SHADOW
                inBufPos = this.inBufPos;
                inBufEnd = this.inBufEnd;
#endif
#if CHECK_EOF
                if (exMatLen == -1)
                    goto finish;
#endif
            }

            matLen += exMatLen;
            if (exMatLen == 255)
                goto readExMatchLength;

            phase = DecodePhase.CopyMatch;
            goto copyMatch;

            copyMatch:
            int nCpyMat = matLen < nToRead ? matLen : nToRead;
            if (nCpyMat != 0)
            {
                nRead = buffer.Length - nToRead;

                int bufDst = matDst - nRead;
                if (bufDst > 0)
                {
                    //offset is fairly far back, we need to pull from the buffer

                    int bufSrc = decodeBufferPos - bufDst;
                    if (bufSrc < 0)
                        bufSrc += DecBufLen;
                    int bufCnt = bufDst < nCpyMat ? bufDst : nCpyMat;

                    for (int c = bufCnt; c-- != 0;)
                        buffer[offset++] = decBuf[bufSrc++ & DecBufMask];
                }
                else
                {
                    bufDst = 0;
                }

                int sOfs = offset - matDst;
                for (int i = bufDst; i < nCpyMat; i++)
                    buffer[offset++] = buffer[sOfs++];

                nToRead -= nCpyMat;
                matLen -= nCpyMat;
            }

            if (nToRead == 0)
                goto finish;

            phase = DecodePhase.ReadToken;
            goto readToken;

            finish:
            nRead = buffer.Length - nToRead;

            int nToBuf = nRead < DecBufLen ? nRead : DecBufLen;
            int repPos = offset - nToBuf;

            if (nToBuf == DecBufLen)
            {
                buffer.Slice(repPos, DecBufLen).CopyTo(decBuf);
                decodeBufferPos = 0;
            }
            else
            {
                int decPos = decodeBufferPos;

                while (nToBuf-- != 0)
                    decBuf[decPos++ & DecBufMask] = buffer[repPos++];

                decodeBufferPos = decPos & DecBufMask;
            }

#if LOCAL_SHADOW
            this.phase = phase;
            this.inBufPos = inBufPos;
#endif
            return nRead;
        }

        private int ReadByteCore()
        {
            var buf = decodeBuffer;

            if (inBufPos == inBufEnd)
            {
                int nRead = _input.Read(buf, DecBufLen,
                    InBufLen < _inputLength ? InBufLen : (int)_inputLength);

#if CHECK_EOF
                if (nRead == 0)
                    return -1;
#endif

                _inputLength -= nRead;

                inBufPos = DecBufLen;
                inBufEnd = DecBufLen + nRead;
            }

            return buf[inBufPos++];
        }

        private int ReadOffsetCore()
        {
            var buf = decodeBuffer;

            if (inBufPos == inBufEnd)
            {
                int nRead = _input.Read(buf, DecBufLen,
                    InBufLen < _inputLength ? InBufLen : (int)_inputLength);

#if CHECK_EOF
                if (nRead == 0)
                    return -1;
#endif

                _inputLength -= nRead;

                inBufPos = DecBufLen;
                inBufEnd = DecBufLen + nRead;
            }

            if (inBufEnd - inBufPos == 1)
            {
                buf[DecBufLen] = buf[inBufPos];

                int nRead = _input.Read(buf, DecBufLen + 1,
                    InBufLen - 1 < _inputLength ? InBufLen - 1 : (int)_inputLength);

#if CHECK_EOF
                if (nRead == 0)
                {
                    inBufPos = DecBufLen;
                    inBufEnd = DecBufLen + 1;

                    return -1;
                }
#endif

                _inputLength -= nRead;

                inBufPos = DecBufLen;
                inBufEnd = DecBufLen + nRead + 1;
            }

            int ret = (buf[inBufPos + 1] << 8) | buf[inBufPos];
            inBufPos += 2;

            return ret;
        }

        private int ReadCore(Span<byte> buffer)
        {
            int nToRead = buffer.Length;
            int offset = 0;

            var buf = decodeBuffer;
            int inBufLen = inBufEnd - inBufPos;

            int fromBuf = nToRead < inBufLen ? nToRead : inBufLen;
            if (fromBuf != 0)
            {
                var bufPos = inBufPos;

                for (int c = fromBuf; c-- != 0;)
                    buffer[offset++] = buf[bufPos++];

                inBufPos = bufPos;
                nToRead -= fromBuf;
            }

            if (nToRead != 0)
            {
                int nRead;

                if (nToRead >= InBufLen)
                {
                    nRead = _input.Read(buffer.Slice(offset,
                        nToRead < _inputLength ? nToRead : (int)_inputLength));
                    nToRead -= nRead;
                }
                else
                {
                    nRead = _input.Read(buf, DecBufLen,
                        InBufLen < _inputLength ? InBufLen : (int)_inputLength);

                    inBufPos = DecBufLen;
                    inBufEnd = DecBufLen + nRead;

                    fromBuf = nToRead < nRead ? nToRead : nRead;

                    var bufPos = inBufPos;

                    for (int c = fromBuf; c-- != 0;)
                        buffer[offset++] = buf[bufPos++];

                    inBufPos = bufPos;
                    nToRead -= fromBuf;
                }

                _inputLength -= nRead;
            }

            return buffer.Length - nToRead;
        }

        #region Stream internals

        public override bool CanRead => true;
        public override bool CanSeek => false;
        public override bool CanWrite => false;

        public override void Flush()
        {
        }

        public override long Length => throw new NotSupportedException();

        public override long Position
        {
            get => throw new NotSupportedException();
            set => throw new NotSupportedException();
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotSupportedException();
        }

        public override void SetLength(long value)
        {
            throw new NotSupportedException();
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            throw new NotSupportedException();
        }

        #endregion
    }
}
