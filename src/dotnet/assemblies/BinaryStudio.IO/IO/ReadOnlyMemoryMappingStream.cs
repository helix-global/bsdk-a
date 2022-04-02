using System;
using System.IO;

namespace BinaryStudio.IO
    {
    public class ReadOnlyMemoryMappingStream : ReadOnlyMappingStream
        {
        private Byte[] source;
        public ReadOnlyMemoryMappingStream(Byte[] source)
            {
            if (source == null) { throw new ArgumentNullException(nameof(source)); }
            Offset = 0;
            Length = source.LongLength;
            this.source = source;
            }

        public ReadOnlyMemoryMappingStream(Byte[] source, Int64 offset, Int64 length) {
            if (source == null) { throw new ArgumentNullException(nameof(source)); }
            Offset = offset;
            Length = length;
            this.source = source;
            }

        private ReadOnlyMemoryMappingStream(ReadOnlyMemoryMappingStream source, Int64 offset, Int64 length) {
            if (source == null) { throw new ArgumentNullException(nameof(source)); }
            if (offset < 0) { throw new ArgumentOutOfRangeException(nameof(offset)); }
            if (length < 0) { throw new ArgumentOutOfRangeException(nameof(length)); }
            if ((offset + length) > source.Length) {
                throw new ArgumentOutOfRangeException(nameof(length));
                }
            this.source = source.source;
            Offset = offset;
            Length = length;
            }

        private ReadOnlyMemoryMappingStream(ReadOnlyMemoryMappingStream source, Int64 length) {
            if (source == null) { throw new ArgumentNullException(nameof(source)); }
            if (length < 0) { throw new ArgumentOutOfRangeException(nameof(length)); }
            if ((source.Position + length) > source.Length) {
                throw new ArgumentOutOfRangeException(nameof(length));
                }
            Length = length;
            Offset = source.Offset + source.Position;
            this.source = source.source;
            }

        /**
         * <summary>When overridden in a derived class, reads a sequence of bytes from the current stream and advances the position within the stream by the number of bytes read.</summary>
         * <returns>The total number of bytes read into the buffer. This can be less than the number of bytes requested if that many bytes are not currently available, or zero (0) if the end of the stream has been reached.</returns>
         * <param name="buffer">An array of bytes. When this method returns, the buffer contains the specified byte array with the values between <paramref name="offset"/> and (<paramref name="offset"/> + <paramref name="count"/> - 1) replaced by the bytes read from the current source.</param>
         * <param name="offset">The zero-based byte offset in <paramref name="buffer"/> at which to begin storing the data read from the current stream.</param>
         * <param name="count">The maximum number of bytes to be read from the current stream.</param>
         * <exception cref="ArgumentException">The sum of <paramref name="offset"/> and <paramref name="count"/> is larger than the buffer length.</exception>
         * <exception cref="ArgumentNullException"><paramref name="buffer"/> is null.</exception>
         * <exception cref="ArgumentOutOfRangeException"><paramref name="offset"/> or <paramref name="count"/> is negative.</exception>
         * <exception cref="IOException">An I/O error occurs.</exception>
         * <exception cref="NotSupportedException">The stream does not support reading.</exception>
         * <exception cref="ObjectDisposedException">Methods were called after the stream was closed.</exception>
         * <filterpriority>1</filterpriority>
         * */
        public override Int32 Read(Byte[] buffer, Int32 offset, Int32 count)
            {
            if (buffer == null) { throw new ArgumentNullException(nameof(buffer)); }
            if (offset < 0) { throw new ArgumentOutOfRangeException(nameof(offset)); }
            if (count < 0)  { throw new ArgumentOutOfRangeException(nameof(count));  }
            if (buffer.Length - offset < count) { throw new ArgumentOutOfRangeException(nameof(offset)); }
            if (Disposed) { throw new ObjectDisposedException(nameof(source)); }
            #if TRACE
            //using (TraceManager.Instance.Trace(count))
            #endif
                {
                var sz = Length - (Position + count);
                if (sz < 0) { count = count + (Int32)sz; }
                for (var i = 0; i < count; i++) {
                    buffer[offset + i] = source[Offset + Position + i];
                    }
                Seek(count, SeekOrigin.Current);
                return count;
                }
            }

        /**
         * <summary>Releases the unmanaged resources used by the <see cref="T:System.IO.Stream"></see> and optionally releases the managed resources.</summary>
         * <param name="disposing">true to release both managed and unmanaged resources; false to release only unmanaged resources.</param>
         * */
        protected override void Dispose(Boolean disposing) {
            lock(this) {
                if (!Disposed) {
                    source = null;
                    base.Dispose(disposing);
                    }
                }
            }

        public override Int64 Length { get; }
        protected override Int64 Offset { get; }

        public override ReadOnlyMappingStream Clone()
            {
            var r = new ReadOnlyMemoryMappingStream(source, Offset, Length);
            r.Seek(Position, SeekOrigin.Begin);
            return r;
            }

        public override ReadOnlyMappingStream Clone(Int64 offset, Int64 length) { return new ReadOnlyMemoryMappingStream(this, offset, length); }
        public override ReadOnlyMappingStream Clone(Int64 length) { return new ReadOnlyMemoryMappingStream(this, length); }
        }
    }
