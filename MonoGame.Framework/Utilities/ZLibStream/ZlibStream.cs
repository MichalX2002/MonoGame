// ZlibStream.cs
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
// Time-stamp: <2011-July-31 14:53:33>
//
// ------------------------------------------------------------------
//
// This module defines the ZlibStream class, which is similar in idea to
// the System.IO.Compression.DeflateStream and
// System.IO.Compression.GZipStream classes in the .NET BCL.
//
// ------------------------------------------------------------------

// The following notice applies to jzlib:
// -----------------------------------------------------------------------------

// Copyright (c) 2000,2001,2002,2003 ymnk, JCraft,Inc. All rights reserved.

// Redistribution and use in source and binary forms, with or without
// modification, are permitted provided that the following conditions are met:

// 1. Redistributions of source code must retain the above copyright notice,
// this list of conditions and the following disclaimer.

// 2. Redistributions in binary form must reproduce the above copyright
// notice, this list of conditions and the following disclaimer in
// the documentation and/or other materials provided with the distribution.

// 3. The names of the authors may not be used to endorse or promote products
// derived from this software without specific prior written permission.

// THIS SOFTWARE IS PROVIDED ``AS IS'' AND ANY EXPRESSED OR IMPLIED WARRANTIES,
// INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND
// FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL JCRAFT,
// INC. OR ANY CONTRIBUTORS TO THIS SOFTWARE BE LIABLE FOR ANY DIRECT, INDIRECT,
// INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT
// LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA,
// OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF
// LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING
// NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE,
// EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.

// -----------------------------------------------------------------------------

// jzlib is based on zlib-1.1.3.

// The following notice applies to zlib:

// -----------------------------------------------------------------------------

// Copyright (C) 1995-2004 Jean-loup Gailly and Mark Adler

// The ZLIB software is provided 'as-is', without any express or implied
// warranty.  In no event will the authors be held liable for any damages
// arising from the use of this software.

// Permission is granted to anyone to use this software for any purpose,
// including commercial applications, and to alter it and redistribute it
// freely, subject to the following restrictions:

// 1. The origin of this software must not be misrepresented; you must not
//    claim that you wrote the original software. If you use this software
//    in a product, an acknowledgment in the product documentation would be
//    appreciated but is not required.
// 2. Altered source versions must be plainly marked as such, and must not be
//    misrepresented as being the original software.
// 3. This notice may not be removed or altered from any source distribution.

// Jean-loup Gailly jloup@gzip.org
// Mark Adler madler@alumni.caltech.edu


//------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace MonoGame.Utilities
{
    /// <summary>
    /// Represents a Zlib stream for compression or decompression.
    /// </summary>
    /// <remarks>
    ///
    /// <para>
    /// The ZlibStream is a <see
    /// href="http://en.wikipedia.org/wiki/Decorator_pattern">Decorator</see> on a <see
    /// cref="System.IO.Stream"/>.  It adds ZLIB compression or decompression to any
    /// stream.
    /// </para>
    ///
    /// <para> Using this stream, applications can compress or decompress data via
    /// stream <c>Read()</c> and <c>Write()</c> operations.  Either compression or
    /// decompression can occur through either reading or writing. The compression
    /// format used is ZLIB, which is documented in <see
    /// href="http://www.ietf.org/rfc/rfc1950.txt">IETF RFC 1950</see>, "ZLIB Compressed
    /// Data Format Specification version 3.3". This implementation of ZLIB always uses
    /// DEFLATE as the compression method.  (see <see
    /// href="http://www.ietf.org/rfc/rfc1951.txt">IETF RFC 1951</see>, "DEFLATE
    /// Compressed Data Format Specification version 1.3.") </para>
    ///
    /// <para>
    /// The ZLIB format allows for varying compression methods, window sizes, and dictionaries.
    /// This implementation always uses the DEFLATE compression method, a preset dictionary,
    /// and 15 window bits by default.
    /// </para>
    ///
    /// <para>
    /// This class is similar to DeflateStream, except that it adds the
    /// RFC1950 header and trailer bytes to a compressed stream when compressing, or expects
    /// the RFC1950 header and trailer bytes when decompressing.  It is also similar to the
    /// <see cref="GZipStream"/>.
    /// </para>
    /// </remarks>
    /// <seealso cref="GZipStream" />
    public class ZlibStream : Stream
    {
        internal ZlibBaseStream _baseStream;
        bool _disposed;

        /// <summary>
        /// Create a <c>ZlibStream</c> using the specified <c>CompressionMode</c>.
        /// </summary>
        /// <remarks>
        ///
        /// <para>
        ///   When mode is <c>CompressionMode.Compress</c>, the <c>ZlibStream</c>
        ///   will use the default compression level. The "captive" stream will be
        ///   closed when the <c>ZlibStream</c> is closed.
        /// </para>
        ///
        /// </remarks>
        ///
        /// <example>
        /// This example uses a <c>ZlibStream</c> to compress a file, and writes the
        /// compressed data to another file.
        /// <code>
        /// using (System.IO.Stream input = System.IO.File.OpenRead(fileToCompress))
        /// {
        ///     using (var raw = System.IO.File.Create(fileToCompress + ".zlib"))
        ///     {
        ///         using (Stream compressor = new ZlibStream(raw, CompressionMode.Compress))
        ///         {
        ///             byte[] buffer = new byte[WORKING_BUFFER_SIZE];
        ///             int n;
        ///             while ((n= input.Read(buffer, 0, buffer.Length)) != 0)
        ///             {
        ///                 compressor.Write(buffer, 0, n);
        ///             }
        ///         }
        ///     }
        /// }
        /// </code>
        /// <code lang="VB">
        /// Using input As Stream = File.OpenRead(fileToCompress)
        ///     Using raw As FileStream = File.Create(fileToCompress &amp; ".zlib")
        ///     Using compressor As Stream = New ZlibStream(raw, CompressionMode.Compress)
        ///         Dim buffer As Byte() = New Byte(4096) {}
        ///         Dim n As Integer = -1
        ///         Do While (n &lt;&gt; 0)
        ///             If (n &gt; 0) Then
        ///                 compressor.Write(buffer, 0, n)
        ///             End If
        ///             n = input.Read(buffer, 0, buffer.Length)
        ///         Loop
        ///     End Using
        ///     End Using
        /// End Using
        /// </code>
        /// </example>
        ///
        /// <param name="stream">The stream which will be read or written.</param>
        /// <param name="mode">Indicates whether the ZlibStream will compress or decompress.</param>
        public ZlibStream(System.IO.Stream stream, CompressionMode mode)
            : this(stream, mode, CompressionLevel.Default, false)
        {
        }

        /// <summary>
        ///   Create a <c>ZlibStream</c> using the specified <c>CompressionMode</c> and
        ///   the specified <c>CompressionLevel</c>.
        /// </summary>
        ///
        /// <remarks>
        ///
        /// <para>
        ///   When mode is <c>CompressionMode.Decompress</c>, the level parameter is ignored.
        ///   The "captive" stream will be closed when the <c>ZlibStream</c> is closed.
        /// </para>
        ///
        /// </remarks>
        ///
        /// <example>
        ///   This example uses a <c>ZlibStream</c> to compress data from a file, and writes the
        ///   compressed data to another file.
        ///
        /// <code>
        /// using (System.IO.Stream input = System.IO.File.OpenRead(fileToCompress))
        /// {
        ///     using (var raw = System.IO.File.Create(fileToCompress + ".zlib"))
        ///     {
        ///         using (Stream compressor = new ZlibStream(raw,
        ///                                                   CompressionMode.Compress,
        ///                                                   CompressionLevel.BestCompression))
        ///         {
        ///             byte[] buffer = new byte[WORKING_BUFFER_SIZE];
        ///             int n;
        ///             while ((n= input.Read(buffer, 0, buffer.Length)) != 0)
        ///             {
        ///                 compressor.Write(buffer, 0, n);
        ///             }
        ///         }
        ///     }
        /// }
        /// </code>
        ///
        /// <code lang="VB">
        /// Using input As Stream = File.OpenRead(fileToCompress)
        ///     Using raw As FileStream = File.Create(fileToCompress &amp; ".zlib")
        ///         Using compressor As Stream = New ZlibStream(raw, CompressionMode.Compress, CompressionLevel.BestCompression)
        ///             Dim buffer As Byte() = New Byte(4096) {}
        ///             Dim n As Integer = -1
        ///             Do While (n &lt;&gt; 0)
        ///                 If (n &gt; 0) Then
        ///                     compressor.Write(buffer, 0, n)
        ///                 End If
        ///                 n = input.Read(buffer, 0, buffer.Length)
        ///             Loop
        ///         End Using
        ///     End Using
        /// End Using
        /// </code>
        /// </example>
        ///
        /// <param name="stream">The stream to be read or written while deflating or inflating.</param>
        /// <param name="mode">Indicates whether the ZlibStream will compress or decompress.</param>
        /// <param name="level">A tuning knob to trade speed for effectiveness.</param>
        public ZlibStream(System.IO.Stream stream, CompressionMode mode, CompressionLevel level)
            : this(stream, mode, level, false)
        {
        }

        /// <summary>
        ///   Create a <c>ZlibStream</c> using the specified <c>CompressionMode</c>, and
        ///   explicitly specify whether the captive stream should be left open after
        ///   Deflation or Inflation.
        /// </summary>
        ///
        /// <remarks>
        ///
        /// <para>
        ///   When mode is <c>CompressionMode.Compress</c>, the <c>ZlibStream</c> will use
        ///   the default compression level.
        /// </para>
        ///
        /// <para>
        ///   This constructor allows the application to request that the captive stream
        ///   remain open after the deflation or inflation occurs.  By default, after
        ///   <c>Close()</c> is called on the stream, the captive stream is also
        ///   closed. In some cases this is not desired, for example if the stream is a
        ///   <see cref="System.IO.MemoryStream"/> that will be re-read after
        ///   compression.  Specify true for the <paramref name="leaveOpen"/> parameter to leave the stream
        ///   open.
        /// </para>
        ///
        /// <para>
        /// See the other overloads of this constructor for example code.
        /// </para>
        ///
        /// </remarks>
        ///
        /// <param name="stream">The stream which will be read or written. This is called the
        /// "captive" stream in other places in this documentation.</param>
        /// <param name="mode">Indicates whether the ZlibStream will compress or decompress.</param>
        /// <param name="leaveOpen">true if the application would like the stream to remain
        /// open after inflation/deflation.</param>
        public ZlibStream(System.IO.Stream stream, CompressionMode mode, bool leaveOpen)
            : this(stream, mode, CompressionLevel.Default, leaveOpen)
        {
        }

        /// <summary>
        ///   Create a <c>ZlibStream</c> using the specified <c>CompressionMode</c>
        ///   and the specified <c>CompressionLevel</c>, and explicitly specify
        ///   whether the stream should be left open after Deflation or Inflation.
        /// </summary>
        ///
        /// <remarks>
        ///
        /// <para>
        ///   This constructor allows the application to request that the captive
        ///   stream remain open after the deflation or inflation occurs.  By
        ///   default, after <c>Close()</c> is called on the stream, the captive
        ///   stream is also closed. In some cases this is not desired, for example
        ///   if the stream is a <see cref="System.IO.MemoryStream"/> that will be
        ///   re-read after compression.  Specify true for the <paramref
        ///   name="leaveOpen"/> parameter to leave the stream open.
        /// </para>
        ///
        /// <para>
        ///   When mode is <c>CompressionMode.Decompress</c>, the level parameter is
        ///   ignored.
        /// </para>
        ///
        /// </remarks>
        ///
        /// <example>
        ///
        /// This example shows how to use a ZlibStream to compress the data from a file,
        /// and store the result into another file. The filestream remains open to allow
        /// additional data to be written to it.
        ///
        /// <code>
        /// using (var output = System.IO.File.Create(fileToCompress + ".zlib"))
        /// {
        ///     using (System.IO.Stream input = System.IO.File.OpenRead(fileToCompress))
        ///     {
        ///         using (Stream compressor = new ZlibStream(output, CompressionMode.Compress, CompressionLevel.BestCompression, true))
        ///         {
        ///             byte[] buffer = new byte[WORKING_BUFFER_SIZE];
        ///             int n;
        ///             while ((n= input.Read(buffer, 0, buffer.Length)) != 0)
        ///             {
        ///                 compressor.Write(buffer, 0, n);
        ///             }
        ///         }
        ///     }
        ///     // can write additional data to the output stream here
        /// }
        /// </code>
        /// <code lang="VB">
        /// Using output As FileStream = File.Create(fileToCompress &amp; ".zlib")
        ///     Using input As Stream = File.OpenRead(fileToCompress)
        ///         Using compressor As Stream = New ZlibStream(output, CompressionMode.Compress, CompressionLevel.BestCompression, True)
        ///             Dim buffer As Byte() = New Byte(4096) {}
        ///             Dim n As Integer = -1
        ///             Do While (n &lt;&gt; 0)
        ///                 If (n &gt; 0) Then
        ///                     compressor.Write(buffer, 0, n)
        ///                 End If
        ///                 n = input.Read(buffer, 0, buffer.Length)
        ///             Loop
        ///         End Using
        ///     End Using
        ///     ' can write additional data to the output stream here.
        /// End Using
        /// </code>
        /// </example>
        ///
        /// <param name="stream">The stream which will be read or written.</param>
        ///
        /// <param name="mode">Indicates whether the ZlibStream will compress or decompress.</param>
        ///
        /// <param name="leaveOpen">
        /// true if the application would like the stream to remain open after
        /// inflation/deflation.
        /// </param>
        ///
        /// <param name="level">
        /// A tuning knob to trade speed for effectiveness. This parameter is
        /// effective only when mode is <c>CompressionMode.Compress</c>.
        /// </param>
        public ZlibStream(System.IO.Stream stream, CompressionMode mode, CompressionLevel level, bool leaveOpen)
        {
            _baseStream = new ZlibBaseStream(stream, mode, level, ZlibStreamFlavor.ZLIB, leaveOpen);
        }

        #region Zlib properties

        /// <summary>
        /// This property sets the flush behavior on the stream.
        /// Sorry, though, not sure exactly how to describe all the various settings.
        /// </summary>
        virtual public FlushType FlushMode
        {
            get { return (this._baseStream._flushMode); }
            set
            {
                if (_disposed) throw new ObjectDisposedException("ZlibStream");
                this._baseStream._flushMode = value;
            }
        }

        /// <summary>
        ///   The size of the working buffer for the compression codec.
        /// </summary>
        ///
        /// <remarks>
        /// <para>
        ///   The working buffer is used for all stream operations.  The default size is
        ///   1024 bytes. The minimum size is 128 bytes. You may get better performance
        ///   with a larger buffer.  Then again, you might not.  You would have to test
        ///   it.
        /// </para>
        ///
        /// <para>
        ///   Set this before the first call to <c>Read()</c> or <c>Write()</c> on the
        ///   stream. If you try to set it afterwards, it will throw.
        /// </para>
        /// </remarks>
        public int BufferSize
        {
            get
            {
                return this._baseStream._bufferSize;
            }
            set
            {
                if (_disposed) throw new ObjectDisposedException("ZlibStream");
                if (this._baseStream._workingBuffer != null)
                    throw new ZlibException("The working buffer is already set.");
                if (value < ZlibConstants.WorkingBufferSizeMin)
                    throw new ZlibException(String.Format("Don't be silly. {0} bytes?? Use a bigger buffer, at least {1}.", value, ZlibConstants.WorkingBufferSizeMin));
                this._baseStream._bufferSize = value;
            }
        }

        /// <summary> Returns the total number of bytes input so far.</summary>
        virtual public long TotalIn
        {
            get { return this._baseStream._z.TotalBytesIn; }
        }

        /// <summary> Returns the total number of bytes output so far.</summary>
        virtual public long TotalOut
        {
            get { return this._baseStream._z.TotalBytesOut; }
        }

        #endregion

        #region System.IO.Stream methods

        /// <summary>
        ///   Dispose the stream.
        /// </summary>
        /// <remarks>
        ///   <para>
        ///     This may or may not result in a <c>Close()</c> call on the captive
        ///     stream.  See the constructors that have a <c>leaveOpen</c> parameter
        ///     for more information.
        ///   </para>
        ///   <para>
        ///     This method may be invoked in two distinct scenarios.  If disposing
        ///     == true, the method has been called directly or indirectly by a
        ///     user's code, for example via the public Dispose() method. In this
        ///     case, both managed and unmanaged resources can be referenced and
        ///     disposed.  If disposing == false, the method has been called by the
        ///     runtime from inside the object finalizer and this method should not
        ///     reference other objects; in that case only unmanaged resources must
        ///     be referenced or disposed.
        ///   </para>
        /// </remarks>
        /// <param name="disposing">
        ///   indicates whether the Dispose method was invoked by user code.
        /// </param>
        protected override void Dispose(bool disposing)
        {
            try
            {
                if (!_disposed)
                {
                    if (disposing && (this._baseStream != null))
                        this._baseStream.Dispose();
                    _disposed = true;
                }
            }
            finally
            {
                base.Dispose(disposing);
            }
        }


        /// <summary>
        /// Indicates whether the stream can be read.
        /// </summary>
        /// <remarks>
        /// The return value depends on whether the captive stream supports reading.
        /// </remarks>
        public override bool CanRead
        {
            get
            {
                if (_disposed) throw new ObjectDisposedException("ZlibStream");
                return _baseStream._stream.CanRead;
            }
        }

        /// <summary>
        /// Indicates whether the stream supports Seek operations.
        /// </summary>
        /// <remarks>
        /// Always returns false.
        /// </remarks>
        public override bool CanSeek
        {
            get { return false; }
        }

        /// <summary>
        /// Indicates whether the stream can be written.
        /// </summary>
        /// <remarks>
        /// The return value depends on whether the captive stream supports writing.
        /// </remarks>
        public override bool CanWrite
        {
            get
            {
                if (_disposed) throw new ObjectDisposedException("ZlibStream");
                return _baseStream._stream.CanWrite;
            }
        }

        /// <summary>
        /// Flush the stream.
        /// </summary>
        public override void Flush()
        {
            if (_disposed) throw new ObjectDisposedException("ZlibStream");
            _baseStream.Flush();
        }

        /// <summary>
        /// Reading this property always throws a <see cref="NotSupportedException"/>.
        /// </summary>
        public override long Length
        {
            get { throw new NotSupportedException(); }
        }

        /// <summary>
        ///   The position of the stream pointer.
        /// </summary>
        ///
        /// <remarks>
        ///   Setting this property always throws a <see
        ///   cref="NotSupportedException"/>. Reading will return the total bytes
        ///   written out, if used in writing, or the total bytes read in, if used in
        ///   reading.  The count may refer to compressed bytes or uncompressed bytes,
        ///   depending on how you've used the stream.
        /// </remarks>
        public override long Position
        {
            get
            {
                if (this._baseStream._streamMode == ZlibBaseStream.StreamMode.Writer)
                    return this._baseStream._z.TotalBytesOut;
                if (this._baseStream._streamMode == ZlibBaseStream.StreamMode.Reader)
                    return this._baseStream._z.TotalBytesIn;
                return 0;
            }

            set { throw new NotSupportedException(); }
        }

        /// <summary>
        /// Read data from the stream.
        /// </summary>
        ///
        /// <remarks>
        ///
        /// <para>
        ///   If you wish to use the <c>ZlibStream</c> to compress data while reading,
        ///   you can create a <c>ZlibStream</c> with <c>CompressionMode.Compress</c>,
        ///   providing an uncompressed data stream.  Then call <c>Read()</c> on that
        ///   <c>ZlibStream</c>, and the data read will be compressed.  If you wish to
        ///   use the <c>ZlibStream</c> to decompress data while reading, you can create
        ///   a <c>ZlibStream</c> with <c>CompressionMode.Decompress</c>, providing a
        ///   readable compressed data stream.  Then call <c>Read()</c> on that
        ///   <c>ZlibStream</c>, and the data will be decompressed as it is read.
        /// </para>
        ///
        /// <para>
        ///   A <c>ZlibStream</c> can be used for <c>Read()</c> or <c>Write()</c>, but
        ///   not both.
        /// </para>
        ///
        /// </remarks>
        ///
        /// <param name="buffer">
        /// The buffer into which the read data should be placed.</param>
        ///
        /// <param name="offset">
        /// the offset within that data array to put the first byte read.</param>
        ///
        /// <param name="count">the number of bytes to read.</param>
        ///
        /// <returns>the number of bytes read</returns>
        public override int Read(byte[] buffer, int offset, int count)
        {
            if (_disposed) throw new ObjectDisposedException("ZlibStream");
            return _baseStream.Read(buffer, offset, count);
        }

        /// <summary>
        /// Calling this method always throws a <see cref="NotSupportedException"/>.
        /// </summary>
        /// <param name="offset">
        ///   The offset to seek to....
        ///   IF THIS METHOD ACTUALLY DID ANYTHING.
        /// </param>
        /// <param name="origin">
        ///   The reference specifying how to apply the offset....  IF
        ///   THIS METHOD ACTUALLY DID ANYTHING.
        /// </param>
        ///
        /// <returns>nothing. This method always throws.</returns>
        public override long Seek(long offset, System.IO.SeekOrigin origin)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Calling this method always throws a <see cref="NotSupportedException"/>.
        /// </summary>
        /// <param name="value">
        ///   The new value for the stream length....  IF
        ///   THIS METHOD ACTUALLY DID ANYTHING.
        /// </param>
        public override void SetLength(long value)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Write data to the stream.
        /// </summary>
        ///
        /// <remarks>
        ///
        /// <para>
        ///   If you wish to use the <c>ZlibStream</c> to compress data while writing,
        ///   you can create a <c>ZlibStream</c> with <c>CompressionMode.Compress</c>,
        ///   and a writable output stream.  Then call <c>Write()</c> on that
        ///   <c>ZlibStream</c>, providing uncompressed data as input.  The data sent to
        ///   the output stream will be the compressed form of the data written.  If you
        ///   wish to use the <c>ZlibStream</c> to decompress data while writing, you
        ///   can create a <c>ZlibStream</c> with <c>CompressionMode.Decompress</c>, and a
        ///   writable output stream.  Then call <c>Write()</c> on that stream,
        ///   providing previously compressed data. The data sent to the output stream
        ///   will be the decompressed form of the data written.
        /// </para>
        ///
        /// <para>
        ///   A <c>ZlibStream</c> can be used for <c>Read()</c> or <c>Write()</c>, but not both.
        /// </para>
        /// </remarks>
        /// <param name="buffer">The buffer holding data to write to the stream.</param>
        /// <param name="offset">the offset within that data array to find the first byte to write.</param>
        /// <param name="count">the number of bytes to write.</param>
        public override void Write(byte[] buffer, int offset, int count)
        {
            if (_disposed) throw new ObjectDisposedException("ZlibStream");
            _baseStream.Write(buffer, offset, count);
        }
        #endregion


        /// <summary>
        ///   Compress a string into a byte array using ZLIB.
        /// </summary>
        ///
        /// <remarks>
        ///   Uncompress it with <see cref="ZlibStream.UncompressString(byte[])"/>.
        /// </remarks>
        ///
        /// <seealso cref="ZlibStream.UncompressString(byte[])"/>
        /// <seealso cref="ZlibStream.CompressBuffer(byte[])"/>
        /// <seealso cref="GZipStream.CompressString(string)"/>
        ///
        /// <param name="s">
        ///   A string to compress.  The string will first be encoded
        ///   using UTF8, then compressed.
        /// </param>
        ///
        /// <returns>The string in compressed form</returns>
        public static byte[] CompressString(String s)
        {
            using (var ms = new MemoryStream())
            {
                Stream compressor =
                    new ZlibStream(ms, CompressionMode.Compress, CompressionLevel.BestCompression);
                ZlibBaseStream.CompressString(s, compressor);
                return ms.ToArray();
            }
        }


        /// <summary>
        ///   Compress a byte array into a new byte array using ZLIB.
        /// </summary>
        ///
        /// <remarks>
        ///   Uncompress it with <see cref="ZlibStream.UncompressBuffer(byte[])"/>.
        /// </remarks>
        ///
        /// <seealso cref="ZlibStream.CompressString(string)"/>
        /// <seealso cref="ZlibStream.UncompressBuffer(byte[])"/>
        ///
        /// <param name="b">
        /// A buffer to compress.
        /// </param>
        ///
        /// <returns>The data in compressed form</returns>
        public static byte[] CompressBuffer(byte[] b)
        {
            using (var ms = new MemoryStream())
            {
                Stream compressor =
                    new ZlibStream(ms, CompressionMode.Compress, CompressionLevel.BestCompression);

                ZlibBaseStream.CompressBuffer(b, compressor);
                return ms.ToArray();
            }
        }


        /// <summary>
        ///   Uncompress a ZLIB-compressed byte array into a single string.
        /// </summary>
        ///
        /// <seealso cref="ZlibStream.CompressString(String)"/>
        /// <seealso cref="ZlibStream.UncompressBuffer(byte[])"/>
        ///
        /// <param name="compressed">
        ///   A buffer containing ZLIB-compressed data.
        /// </param>
        ///
        /// <returns>The uncompressed string</returns>
        public static String UncompressString(byte[] compressed)
        {
            using (var input = new MemoryStream(compressed))
            {
                Stream decompressor =
                    new ZlibStream(input, CompressionMode.Decompress);

                return ZlibBaseStream.UncompressString(compressed, decompressor);
            }
        }


        /// <summary>
        ///   Uncompress a ZLIB-compressed byte array into a byte array.
        /// </summary>
        ///
        /// <seealso cref="ZlibStream.CompressBuffer(byte[])"/>
        /// <seealso cref="ZlibStream.UncompressString(byte[])"/>
        ///
        /// <param name="compressed">
        ///   A buffer containing ZLIB-compressed data.
        /// </param>
        ///
        /// <returns>The data in uncompressed form</returns>
        public static byte[] UncompressBuffer(byte[] compressed)
        {
            using (var input = new MemoryStream(compressed))
            {
                Stream decompressor =
                    new ZlibStream(input, CompressionMode.Decompress);

                return ZlibBaseStream.UncompressBuffer(compressed, decompressor);
            }
        }

    }

    /// <summary>
    /// A bunch of constants used in the Zlib interface.
    /// </summary>
    internal static class ZlibConstants
    {
        /// <summary>
        /// The maximum number of window bits for the Deflate algorithm.
        /// </summary>
        internal const int WindowBitsMax = 15; // 32K LZ77 window

        /// <summary>
        /// The default number of window bits for the Deflate algorithm.
        /// </summary>
        internal const int WindowBitsDefault = WindowBitsMax;

        /// <summary>
        /// indicates everything is A-OK
        /// </summary>
        internal const int Z_OK = 0;

        /// <summary>
        /// Indicates that the last operation reached the end of the stream.
        /// </summary>
        internal const int Z_STREAM_END = 1;

        /// <summary>
        /// The operation ended in need of a dictionary. 
        /// </summary>
        internal const int Z_NEED_DICT = 2;

        /// <summary>
        /// There was an error with the stream - not enough data, not open and readable, etc.
        /// </summary>
        internal const int Z_STREAM_ERROR = -2;

        /// <summary>
        /// There was an error with the data - not enough data, bad data, etc.
        /// </summary>
        internal const int Z_DATA_ERROR = -3;

        /// <summary>
        /// There was an error with the working buffer.
        /// </summary>
        internal const int Z_BUF_ERROR = -5;

        /// <summary>
        /// The size of the working buffer used in the ZlibCodec class. Defaults to 8192 bytes.
        /// </summary>
#if NETCF        
        internal const int WorkingBufferSizeDefault = 8192;
#else
        internal const int WorkingBufferSizeDefault = 16384;
#endif
        /// <summary>
        /// The minimum size of the working buffer used in the ZlibCodec class.  Currently it is 128 bytes.
        /// </summary>
        internal const int WorkingBufferSizeMin = 1024;
    }

    /// <summary>
    /// Encoder and Decoder for ZLIB and DEFLATE (IETF RFC1950 and RFC1951).
    /// </summary>
    ///
    /// <remarks>
    /// This class compresses and decompresses data according to the Deflate algorithm
    /// and optionally, the ZLIB format, as documented in <see
    /// href="http://www.ietf.org/rfc/rfc1950.txt">RFC 1950 - ZLIB</see> and <see
    /// href="http://www.ietf.org/rfc/rfc1951.txt">RFC 1951 - DEFLATE</see>.
    /// </remarks>

    
    internal enum ZlibStreamFlavor { ZLIB = 1950, DEFLATE = 1951, GZIP = 1952 }

    
    /// <summary>
    /// Describes how to flush the current deflate operation.
    /// </summary>
    /// <remarks>
    /// The different <see cref="FlushType"/> values are useful when using a Deflate in a streaming application.
    /// </remarks>
    public enum FlushType
    {
        /// <summary>No flush at all.</summary>
        None = 0,

        /// <summary>Closes the current block, but doesn't flush it to
        /// the output. Used internally only in hypothetical
        /// scenarios. This was supposed to be removed by Zlib, but it is
        /// still in use in some edge cases.
        /// </summary>
        Partial,

        /// <summary>
        /// Use this during compression to specify that all pending output should be
        /// flushed to the output buffer and the output should be aligned on a byte
        /// boundary. You might use this in a streaming communication scenario, so that
        /// the decompressor can get all input data available so far. When using this
        /// with a <see cref="ZlibCodec"/>, <see cref="ZlibCodec.AvailableBytesIn"/> will be zero after the call if
        /// enough output space has been provided before the call. Flushing will
        /// degrade compression and so it should be used only when necessary.
        /// </summary>
        Sync,

        /// <summary>
        /// Use this during compression to specify that all output should be flushed, as
        /// with <see cref="FlushType.Sync"/>, but also, the compression state should be reset
        /// so that decompression can restart from this point if previous compressed
        /// data has been damaged or if random access is desired. Using
        /// <see cref="FlushType.Full"/> too often can significantly degrade the compression.
        /// </summary>
        Full,

        /// <summary>Signals the end of the compression/decompression stream.</summary>
        Finish,
    }


    /// <summary>
    /// The compression level to be used when using a DeflateStream or ZlibStream with CompressionMode.Compress.
    /// </summary>
    public enum CompressionLevel
    {
        /// <summary>
        /// None means that the data will be simply stored, with no change at all.
        /// If you are producing ZIPs for use on Mac OSX, be aware that archives produced with CompressionLevel.None
        /// cannot be opened with the default zip reader. Use a different CompressionLevel.
        /// </summary>
        None = 0,
        /// <summary>
        /// Same as None.
        /// </summary>
        Level0 = 0,

        /// <summary>
        /// The fastest but least effective compression.
        /// </summary>
        BestSpeed = 1,

        /// <summary>
        /// A synonym for BestSpeed.
        /// </summary>
        Level1 = 1,

        /// <summary>
        /// A little slower, but better, than level 1.
        /// </summary>
        Level2 = 2,

        /// <summary>
        /// A little slower, but better, than level 2.
        /// </summary>
        Level3 = 3,

        /// <summary>
        /// A little slower, but better, than level 3.
        /// </summary>
        Level4 = 4,

        /// <summary>
        /// A little slower than level 4, but with better compression.
        /// </summary>
        Level5 = 5,

        /// <summary>
        /// The default compression level, with a good balance of speed and compression efficiency.
        /// </summary>
        Default = 6,
        /// <summary>
        /// A synonym for Default.
        /// </summary>
        Level6 = 6,

        /// <summary>
        /// Pretty good compression!
        /// </summary>
        Level7 = 7,

        /// <summary>
        ///  Better compression than Level7!
        /// </summary>
        Level8 = 8,

        /// <summary>
        /// The "best" compression, where best means greatest reduction in size of the input data stream.
        /// This is also the slowest compression.
        /// </summary>
        BestCompression = 9,

        /// <summary>
        /// A synonym for BestCompression.
        /// </summary>
        Level9 = 9,
    }

    /// <summary>
    /// Describes options for how the compression algorithm is executed.  Different strategies
    /// work better on different sorts of data.  The strategy parameter can affect the compression
    /// ratio and the speed of compression but not the correctness of the compresssion.
    /// </summary>
    internal enum CompressionStrategy
    {
        /// <summary>
        /// The default strategy is probably the best for normal data.
        /// </summary>
        Default = 0,

        /// <summary>
        /// The <c>Filtered</c> strategy is intended to be used most effectively with data produced by a
        /// filter or predictor.  By this definition, filtered data consists mostly of small
        /// values with a somewhat random distribution.  In this case, the compression algorithm
        /// is tuned to compress them better.  The effect of <c>Filtered</c> is to force more Huffman
        /// coding and less string matching; it is a half-step between <c>Default</c> and <c>HuffmanOnly</c>.
        /// </summary>
        Filtered = 1,

        /// <summary>
        /// Using <c>HuffmanOnly</c> will force the compressor to do Huffman encoding only, with no
        /// string matching.
        /// </summary>
        HuffmanOnly = 2,
    }


    /// <summary>
    /// An enum to specify the direction of transcoding - whether to compress or decompress.
    /// </summary>
    public enum CompressionMode
    {
        /// <summary>
        /// Used to specify that the stream should compress the data.
        /// </summary>
        Compress = 0,
        /// <summary>
        /// Used to specify that the stream should decompress the data.
        /// </summary>
        Decompress = 1,
    }


    /// <summary>
    /// A general purpose exception class for exceptions in the Zlib library.
    /// </summary>
    internal class ZlibException : System.Exception
    {
        /// <summary>
        /// The ZlibException class captures exception information generated
        /// by the Zlib library.
        /// </summary>
        internal ZlibException()
            : base()
        {
        }

        /// <summary>
        /// This ctor collects a message attached to the exception.
        /// </summary>
        /// <param name="s">the message for the exception.</param>
        internal ZlibException(System.String s)
            : base(s)
        {
        }
    }


    internal class SharedUtils
    {
        /// <summary>
        /// Performs an unsigned bitwise right shift with the specified number
        /// </summary>
        /// <param name="number">Number to operate on</param>
        /// <param name="bits">Ammount of bits to shift</param>
        /// <returns>The resulting number from the shift operation</returns>
        internal static int URShift(int number, int bits)
        {
            return (int)((uint)number >> bits);
        }

#if NOT
        /// <summary>
        /// Performs an unsigned bitwise right shift with the specified number
        /// </summary>
        /// <param name="number">Number to operate on</param>
        /// <param name="bits">Ammount of bits to shift</param>
        /// <returns>The resulting number from the shift operation</returns>
        internal static long URShift(long number, int bits)
        {
            return (long) ((UInt64)number >> bits);
        }
#endif

        /// <summary>
        ///   Reads a number of characters from the current source TextReader and writes
        ///   the data to the target array at the specified index.
        /// </summary>
        ///
        /// <param name="sourceTextReader">The source TextReader to read from</param>
        /// <param name="target">Contains the array of characteres read from the source TextReader.</param>
        /// <param name="start">The starting index of the target array.</param>
        /// <param name="count">The maximum number of characters to read from the source TextReader.</param>
        ///
        /// <returns>
        ///   The number of characters read. The number will be less than or equal to
        ///   count depending on the data available in the source TextReader. Returns -1
        ///   if the end of the stream is reached.
        /// </returns>
        internal static System.Int32 ReadInput(System.IO.TextReader sourceTextReader, byte[] target, int start, int count)
        {
            // Returns 0 bytes if not enough space in target
            if (target.Length == 0) return 0;

            char[] charArray = new char[target.Length];
            int bytesRead = sourceTextReader.Read(charArray, start, count);

            // Returns -1 if EOF
            if (bytesRead == 0) return -1;

            for (int index = start; index < start + bytesRead; index++)
                target[index] = (byte)charArray[index];

            return bytesRead;
        }


        internal static byte[] ToByteArray(System.String sourceString)
        {
            return System.Text.UTF8Encoding.UTF8.GetBytes(sourceString);
        }


        internal static char[] ToCharArray(byte[] byteArray)
        {
            return System.Text.UTF8Encoding.UTF8.GetChars(byteArray);
        }
    }

    internal static class InternalConstants
    {
        internal static readonly int MAX_BITS = 15;
        internal static readonly int BL_CODES = 19;
        internal static readonly int D_CODES = 30;
        internal static readonly int LITERALS = 256;
        internal static readonly int LENGTH_CODES = 29;
        internal static readonly int L_CODES = (LITERALS + 1 + LENGTH_CODES);

        // Bit length codes must not exceed MAX_BL_BITS bits
        internal static readonly int MAX_BL_BITS = 7;

        // repeat previous bit length 3-6 times (2 bits of repeat count)
        internal static readonly int REP_3_6 = 16;

        // repeat a zero length 3-10 times  (3 bits of repeat count)
        internal static readonly int REPZ_3_10 = 17;

        // repeat a zero length 11-138 times  (7 bits of repeat count)
        internal static readonly int REPZ_11_138 = 18;

    }

    internal sealed class StaticTree
    {
        internal static readonly short[] lengthAndLiteralsTreeCodes = new short[] {
            12, 8, 140, 8, 76, 8, 204, 8, 44, 8, 172, 8, 108, 8, 236, 8,
            28, 8, 156, 8, 92, 8, 220, 8, 60, 8, 188, 8, 124, 8, 252, 8,
             2, 8, 130, 8, 66, 8, 194, 8, 34, 8, 162, 8, 98, 8, 226, 8,
            18, 8, 146, 8, 82, 8, 210, 8, 50, 8, 178, 8, 114, 8, 242, 8,
            10, 8, 138, 8, 74, 8, 202, 8, 42, 8, 170, 8, 106, 8, 234, 8,
            26, 8, 154, 8, 90, 8, 218, 8, 58, 8, 186, 8, 122, 8, 250, 8,
             6, 8, 134, 8, 70, 8, 198, 8, 38, 8, 166, 8, 102, 8, 230, 8,
            22, 8, 150, 8, 86, 8, 214, 8, 54, 8, 182, 8, 118, 8, 246, 8,
            14, 8, 142, 8, 78, 8, 206, 8, 46, 8, 174, 8, 110, 8, 238, 8,
            30, 8, 158, 8, 94, 8, 222, 8, 62, 8, 190, 8, 126, 8, 254, 8,
             1, 8, 129, 8, 65, 8, 193, 8, 33, 8, 161, 8, 97, 8, 225, 8,
            17, 8, 145, 8, 81, 8, 209, 8, 49, 8, 177, 8, 113, 8, 241, 8,
             9, 8, 137, 8, 73, 8, 201, 8, 41, 8, 169, 8, 105, 8, 233, 8,
            25, 8, 153, 8, 89, 8, 217, 8, 57, 8, 185, 8, 121, 8, 249, 8,
             5, 8, 133, 8, 69, 8, 197, 8, 37, 8, 165, 8, 101, 8, 229, 8,
            21, 8, 149, 8, 85, 8, 213, 8, 53, 8, 181, 8, 117, 8, 245, 8,
            13, 8, 141, 8, 77, 8, 205, 8, 45, 8, 173, 8, 109, 8, 237, 8,
            29, 8, 157, 8, 93, 8, 221, 8, 61, 8, 189, 8, 125, 8, 253, 8,
            19, 9, 275, 9, 147, 9, 403, 9, 83, 9, 339, 9, 211, 9, 467, 9,
            51, 9, 307, 9, 179, 9, 435, 9, 115, 9, 371, 9, 243, 9, 499, 9,
            11, 9, 267, 9, 139, 9, 395, 9, 75, 9, 331, 9, 203, 9, 459, 9,
            43, 9, 299, 9, 171, 9, 427, 9, 107, 9, 363, 9, 235, 9, 491, 9,
            27, 9, 283, 9, 155, 9, 411, 9, 91, 9, 347, 9, 219, 9, 475, 9,
            59, 9, 315, 9, 187, 9, 443, 9, 123, 9, 379, 9, 251, 9, 507, 9,
             7, 9, 263, 9, 135, 9, 391, 9, 71, 9, 327, 9, 199, 9, 455, 9,
            39, 9, 295, 9, 167, 9, 423, 9, 103, 9, 359, 9, 231, 9, 487, 9,
            23, 9, 279, 9, 151, 9, 407, 9, 87, 9, 343, 9, 215, 9, 471, 9,
            55, 9, 311, 9, 183, 9, 439, 9, 119, 9, 375, 9, 247, 9, 503, 9,
            15, 9, 271, 9, 143, 9, 399, 9, 79, 9, 335, 9, 207, 9, 463, 9,
            47, 9, 303, 9, 175, 9, 431, 9, 111, 9, 367, 9, 239, 9, 495, 9,
            31, 9, 287, 9, 159, 9, 415, 9, 95, 9, 351, 9, 223, 9, 479, 9,
            63, 9, 319, 9, 191, 9, 447, 9, 127, 9, 383, 9, 255, 9, 511, 9,
             0, 7, 64, 7, 32, 7, 96, 7, 16, 7, 80, 7, 48, 7, 112, 7,
             8, 7, 72, 7, 40, 7, 104, 7, 24, 7, 88, 7, 56, 7, 120, 7,
             4, 7, 68, 7, 36, 7, 100, 7, 20, 7, 84, 7, 52, 7, 116, 7,
             3, 8, 131, 8, 67, 8, 195, 8, 35, 8, 163, 8, 99, 8, 227, 8
        };

        internal static readonly short[] distTreeCodes = new short[] {
            0, 5, 16, 5, 8, 5, 24, 5, 4, 5, 20, 5, 12, 5, 28, 5,
            2, 5, 18, 5, 10, 5, 26, 5, 6, 5, 22, 5, 14, 5, 30, 5,
            1, 5, 17, 5, 9, 5, 25, 5, 5, 5, 21, 5, 13, 5, 29, 5,
            3, 5, 19, 5, 11, 5, 27, 5, 7, 5, 23, 5 };

        internal static readonly StaticTree Literals;
        internal static readonly StaticTree Distances;
        internal static readonly StaticTree BitLengths;

        internal short[] treeCodes; // static tree or null
        internal int[] extraBits;   // extra bits for each code or null
        internal int extraBase;     // base index for extra_bits
        internal int elems;         // max number of elements in the tree
        internal int maxLength;     // max bit length for the codes

        private StaticTree(short[] treeCodes, int[] extraBits, int extraBase, int elems, int maxLength)
        {
            this.treeCodes = treeCodes;
            this.extraBits = extraBits;
            this.extraBase = extraBase;
            this.elems = elems;
            this.maxLength = maxLength;
        }
        static StaticTree()
        {
            Literals = new StaticTree(lengthAndLiteralsTreeCodes, Tree.ExtraLengthBits, InternalConstants.LITERALS + 1, InternalConstants.L_CODES, InternalConstants.MAX_BITS);
            Distances = new StaticTree(distTreeCodes, Tree.ExtraDistanceBits, 0, InternalConstants.D_CODES, InternalConstants.MAX_BITS);
            BitLengths = new StaticTree(null, Tree.extra_blbits, 0, InternalConstants.BL_CODES, InternalConstants.MAX_BL_BITS);
        }
    }



    /// <summary>
    /// Computes an Adler-32 checksum.
    /// </summary>
    /// <remarks>
    /// The Adler checksum is similar to a CRC checksum, but faster to compute, though less
    /// reliable.  It is used in producing RFC1950 compressed streams.  The Adler checksum
    /// is a required part of the "ZLIB" standard.  Applications will almost never need to
    /// use this class directly.
    /// </remarks>
    ///
    /// <exclude/>
    internal sealed class Adler
    {
        // largest prime smaller than 65536
        private static readonly uint BASE = 65521;
        // NMAX is the largest n such that 255n(n+1)/2 + (n+1)(BASE-1) <= 2^32-1
        private static readonly int NMAX = 5552;


#pragma warning disable 3001
#pragma warning disable 3002

        /// <summary>
        ///   Calculates the Adler32 checksum.
        /// </summary>
        /// <remarks>
        ///   <para>
        ///     This is used within ZLIB.  You probably don't need to use this directly.
        ///   </para>
        /// </remarks>
        /// <example>
        ///    To compute an Adler32 checksum on a byte array:
        ///  <code>
        ///    var adler = Adler.Adler32(0, null, 0, 0);
        ///    adler = Adler.Adler32(adler, buffer, index, length);
        ///  </code>
        /// </example>
        internal static uint Adler32(uint adler, byte[] buf, int index, int len)
        {
            if (buf == null)
                return 1;

            uint s1 = (uint)(adler & 0xffff);
            uint s2 = (uint)((adler >> 16) & 0xffff);

            while (len > 0)
            {
                int k = len < NMAX ? len : NMAX;
                len -= k;
                while (k >= 16)
                {
                    //s1 += (buf[index++] & 0xff); s2 += s1;
                    s1 += buf[index++]; s2 += s1;
                    s1 += buf[index++]; s2 += s1;
                    s1 += buf[index++]; s2 += s1;
                    s1 += buf[index++]; s2 += s1;
                    s1 += buf[index++]; s2 += s1;
                    s1 += buf[index++]; s2 += s1;
                    s1 += buf[index++]; s2 += s1;
                    s1 += buf[index++]; s2 += s1;
                    s1 += buf[index++]; s2 += s1;
                    s1 += buf[index++]; s2 += s1;
                    s1 += buf[index++]; s2 += s1;
                    s1 += buf[index++]; s2 += s1;
                    s1 += buf[index++]; s2 += s1;
                    s1 += buf[index++]; s2 += s1;
                    s1 += buf[index++]; s2 += s1;
                    s1 += buf[index++]; s2 += s1;
                    k -= 16;
                }
                if (k != 0)
                {
                    do
                    {
                        s1 += buf[index++];
                        s2 += s1;
                    }
                    while (--k != 0);
                }
                s1 %= BASE;
                s2 %= BASE;
            }
            return (uint)((s2 << 16) | s1);
        }
#pragma warning restore 3001
#pragma warning restore 3002

    }

#pragma warning restore IDE1006 // Naming Styles
    
    internal static class InternalInflateConstants
    {
        // And'ing with mask[n] masks the lower n bits
        internal static readonly int[] InflateMask = new int[] {
            0x00000000, 0x00000001, 0x00000003, 0x00000007,
            0x0000000f, 0x0000001f, 0x0000003f, 0x0000007f,
            0x000000ff, 0x000001ff, 0x000003ff, 0x000007ff,
            0x00000fff, 0x00001fff, 0x00003fff, 0x00007fff, 0x0000ffff };
    }

    internal enum BlockState
    {
        NeedMore = 0,       // block not completed, need more input or more output
        BlockDone,          // block flush performed
        FinishStarted,              // finish started, need only more output at next deflate
        FinishDone          // finish done, accept no more input or output
    }

    internal enum DeflateFlavor
    {
        Store,
        Fast,
        Slow
    }
}
