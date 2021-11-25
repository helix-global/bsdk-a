using System;
using System.IO;

namespace BinaryStudio.PortableExecutable
    {
    internal class SourceStream : Stream
        {
        private unsafe Byte* source;
        private unsafe Byte* self;
        private Int64 position;

        public unsafe SourceStream(Byte* source, Int64 size) {
            if (source == null) { throw new ArgumentNullException(nameof(source)); }
            this.source = source;
            this.self   = source;
            Length = size;
            }

        #region M:Flush
        /// <summary>Overrides <see cref="M:System.IO.Stream.Flush"/> so that no action is performed.</summary>
        /// <filterpriority>2</filterpriority>
        public override void Flush()
            {
            }
        #endregion
        #region M:Seek(Int64,SeekOrigin):Int64
        /// <summary>Sets the current position of this stream to the given value.</summary>
        /// <returns>The new position in the stream.</returns>
        /// <param name="offset">The point relative to <paramref name="origin"/> from which to begin seeking.</param>
        /// <param name="origin">Specifies the beginning, the end, or the current position as a reference point for <paramref name="origin"/>, using a value of type <see cref="SeekOrigin"/>.</param>
        /// <exception cref="IOException">An I/O error occurs.</exception>
        /// <exception cref="NotSupportedException">The stream does not support seeking, such as if the FileStream is constructed from a pipe or console output.</exception>
        /// <exception cref="ArgumentException">Attempted seeking before the beginning of the stream.</exception>
        /// <exception cref="ObjectDisposedException">Methods were called after the stream was closed.</exception>
        /// <filterpriority>1</filterpriority>
        public override unsafe Int64 Seek(Int64 offset, SeekOrigin origin)
            {
            if (origin < SeekOrigin.Begin || origin > SeekOrigin.End) { throw new ArgumentOutOfRangeException(nameof(origin)); }
            if (!CanSeek) { throw new NotSupportedException(); }
            switch (origin)
                {
                case SeekOrigin.Begin:
                    {
                    if (offset < 0) { throw new ArgumentOutOfRangeException(nameof(offset)); }
                    position = Math.Min(offset, Length);
                    }
                    break;
                case SeekOrigin.Current:
                    {
                    if (offset + position < 0) { throw new ArgumentOutOfRangeException(nameof(offset)); }
                    position = Math.Min(offset + position, Length);
                    }
                    break;
                case SeekOrigin.End:
                    {
                    if (Length + position < 0) { throw new ArgumentOutOfRangeException(nameof(offset)); }
                    position = Math.Min(offset + Length, Length);
                    }
                    break;
                default: throw new ArgumentOutOfRangeException(nameof(origin));
                }
            self = source + position;
            return position;
            }
        #endregion
        #region M:SetLength(Int64)
        /// <summary>Sets the length of the current stream to the specified value.</summary>
        /// <param name="value">The value at which to set the length. </param>
        /// <exception cref="NotSupportedException">The current stream is not resizable and <paramref name="value"/> is larger than the current capacity.-or- The current stream does not support writing.</exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="value" /> is negative or is greater than the maximum length of the <see cref="T:System.IO.MemoryStream" />, where the maximum length is(<see cref="F:System.Int32.MaxValue"/> - origin), and origin is the index into the underlying buffer at which the stream starts.</exception>
        /// <filterpriority>2</filterpriority>
        public override void SetLength(Int64 value)
            {
            throw new NotSupportedException();
            }
        #endregion
        #region M:Read(Byte[],Int32,Int32):Int32
        /// <summary>Reads a block of bytes from the current stream and writes the data to <paramref name="buffer"/>.</summary>
        /// <returns>The total number of bytes written into the buffer. This can be less than the number of bytes requested if that number of bytes are not currently available, or zero if the end of the stream is reached before any bytes are read.</returns>
        /// <param name="buffer">When this method returns, contains the specified byte array with the values between <paramref name="offset"/> and (<paramref name="offset"/> + <paramref name="count"/> - 1) replaced by the characters read from the current stream.</param>
        /// <param name="offset">The byte offset in <paramref name="buffer"/> at which to begin reading.</param>
        /// <param name="count">The maximum number of bytes to read.</param>
        /// <exception cref="ArgumentNullException"><paramref name="buffer"/> is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="offset"/> or <paramref name="count"/> is negative.</exception>
        /// <exception cref="ArgumentException"><paramref name="offset" /> subtracted from the buffer length is less than <paramref name="count"/>.</exception>
        /// <exception cref="ObjectDisposedException">The current stream instance is closed.</exception>
        /// <filterpriority>2</filterpriority>
        public override unsafe Int32 Read(Byte[] buffer, Int32 offset, Int32 count)
            {
            if (buffer == null) { throw new ArgumentNullException(nameof(buffer)); }
            if (offset < 0) { throw new ArgumentOutOfRangeException(nameof(offset)); }
            if (count < 0)  { throw new ArgumentOutOfRangeException(nameof(count));  }
            if (buffer.Length - offset < count) { throw new ArgumentOutOfRangeException(nameof(buffer)); }
            var n = Length - position;
            if (n <= 0) { return 0; }
            n = Math.Min(n, count);
            for (var i = 0; i < n; ++i) {
                buffer[offset + i] = source[position + i];
                }
            position += n;
            self = source + position;
            return (Int32)n;
            }
        #endregion
        #region M:ReadByte:Int32
        /// <summary>Reads a byte from the current stream.</summary>
        /// <returns>The byte cast to a <see cref="Int32" />, or -1 if the end of the stream has been reached.</returns>
        /// <exception cref="ObjectDisposedException">The current stream instance is closed.</exception>
        /// <filterpriority>2</filterpriority>
        public override unsafe Int32 ReadByte()
            {
            if (position >= Length) { return -1; }
            self = source + 1;
            return source[position++];
            }
        #endregion
        #region M:Write(Byte[],Int32,Int32)
        /// <summary>Writes a block of bytes to the current stream using data read from buffer.</summary>
        /// <param name="buffer">The buffer to write data from.</param>
        /// <param name="offset">The byte offset in <paramref name="buffer"/> at which to begin writing from.</param>
        /// <param name="count">The maximum number of bytes to write. </param>
        /// <exception cref="ArgumentNullException"><paramref name="buffer"/> is null.</exception>
        /// <exception cref="NotSupportedException">The stream does not support writing. For additional information see <see cref="P:System.IO.Stream.CanWrite"/>.-or- The current position is closer than <paramref name="count"/> bytes to the end of the stream, and the capacity cannot be modified.</exception>
        /// <exception cref="ArgumentException"><paramref name="offset"/> subtracted from the buffer length is less than <paramref name="count"/>.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="offset"/> or <paramref name="count"/> are negative.</exception>
        /// <exception cref="IOException">An I/O error occurs. </exception>
        /// <exception cref="ObjectDisposedException">The current stream instance is closed. </exception>
        /// <filterpriority>2</filterpriority>
        public override void Write(Byte[] buffer, Int32 offset, Int32 count)
            {
            throw new NotSupportedException();
            }
        #endregion
        #region M:Dispose(Boolean)
        /// <summary>Releases the unmanaged resources used by the <see cref="SourceStream" /> class and optionally releases the managed resources.</summary>
        /// <param name="disposing">true to release both managed and unmanaged resources; false to release only unmanaged resources.</param>
        protected override unsafe void Dispose(Boolean disposing)
            {
            try
                {
                if (disposing) {
                    //source = null;
                    //position = 0;
                    }
                }
            finally
                {
                base.Dispose(disposing);
                }
            }
        #endregion

        public override Boolean CanRead { get { return true; }}
        public override Boolean CanSeek { get { return true; }}
        public override Boolean CanWrite { get { return false; }}
        public override Int64 Length { get; }

        #region P:Position:Int64
        /// <summary>Gets or sets the current position of this stream.</summary>
        /// <returns>The current position of this stream.</returns>
        /// <exception cref="NotSupportedException">The stream does not support seeking.</exception>
        /// <exception cref="IOException">An I/O error occurs. - or -The position was set to a very large value beyond the end of the stream in Windows 98 or earlier.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Attempted to set the position to a negative value.</exception>
        /// <exception cref="EndOfStreamException">Attempted seeking past the end of a stream that does not support this.</exception>
        /// <filterpriority>1</filterpriority>
        public override Int64 Position {
            get { return position; }
            set
                {
                if (value < 0) { throw new ArgumentOutOfRangeException(nameof(value)); }
                position = 0;
                Seek(value, SeekOrigin.Begin);
                }
            }
        #endregion

        public override unsafe String ToString()
            {
            var i = ((Single)position/Length)*100.0;
            return $"{i:F2}%";
            //return (UIntPtr.Size == 4)
            //    ? $"${(UInt32)(UIntPtr)self:X8}"
            //    : $"${(UInt32)(UIntPtr)self:X16}";
            }
        }
    }