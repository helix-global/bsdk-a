using System;
using System.IO;
using System.Threading;

namespace BinaryStudio.IO
    {
    public abstract class ReadOnlyMappingStream : Stream
        {
        protected abstract Int64 Offset { get; }
        #region P:CanRead:Boolean
        /**
         * <summary>
         * When overridden in a derived class, gets a value indicating whether the current stream supports reading.
         * </summary>
         * <returns>true if the stream supports reading; otherwise, false.</returns>
         * <filterpriority>1</filterpriority>
         * */
        public override Boolean CanRead { get { return true; }}
        #endregion
        #region P:CanSeek:Boolean
        /**
         * <summary>
         * When overridden in a derived class, gets a value indicating whether the current stream supports seeking.
         * </summary>
         * <returns>true if the stream supports seeking; otherwise, false.</returns>
         * <filterpriority>1</filterpriority>
         * */
        public override Boolean CanSeek { get { return true; }}
        #endregion
        #region P:CanWrite:Boolean
        /**
         * <summary>
         * When overridden in a derived class, gets a value indicating whether the current stream supports writing.
         * </summary>
         * <returns>true if the stream supports writing; otherwise, false.</returns>
         * <filterpriority>1</filterpriority>
         * */
        public override Boolean CanWrite { get { return false; }}
        #endregion
        #region P:Position:Int64
        /**
         * <summary>When overridden in a derived class, gets or sets the position within the current stream.</summary>
         * <returns>The current position within the stream.</returns>
         * <exception cref="IOException">An I/O error occurs.</exception>
         * <exception cref="NotSupportedException">The stream does not support seeking.</exception>
         * <exception cref="ObjectDisposedException">Methods were called after the stream was closed.</exception>
         * <filterpriority>1</filterpriority>
         * */
        public override Int64 Position
            {
            get { return position; }
            set
                {
                if (value < 0) { throw new ArgumentOutOfRangeException(nameof(value)); }
                Seek(value, SeekOrigin.Begin);
                }
            }
        #endregion
        public override Int32 ReadTimeout  { get { return 0; }}
        public override Int32 WriteTimeout { get { return 0; }}

        protected ReadOnlyMappingStream()
            {
            }

        #region M:Flush
        /**
         * <summary>
         * When overridden in a derived class, clears all buffers for this stream and causes any buffered data to be
         * written to the underlying device.
         * </summary>
         * <exception cref="IOException">An I/O error occurs.</exception>
         * <filterpriority>2</filterpriority>
         * */
        public override void Flush()
            {
            }
        #endregion
        #region M:SetLength(Int64)
        /**
         * <summary>When overridden in a derived class, sets the length of the current stream.</summary>
         * <param name="value">The desired length of the current stream in bytes.</param>
         * <exception cref="IOException">An I/O error occurs.</exception>
         * <exception cref="NotSupportedException">The stream does not support both writing and seeking, such as if the stream is constructed from a pipe or console output.</exception>
         * <exception cref="ObjectDisposedException">Methods were called after the stream was closed.</exception>
         * <filterpriority>2</filterpriority>
         * */
        public override void SetLength(Int64 value)
            {
            throw new NotSupportedException();
            }
        #endregion
        #region M:Seek(Int64,SeekOrigin):Int64
        /**
         * <summary>When overridden in a derived class, sets the position within the current stream.</summary>
         * <returns>The new position within the current stream.</returns>
         * <param name="offset">A byte offset relative to the <paramref name="origin"/> parameter.</param>
         * <param name="origin">A value of type <see cref="SeekOrigin"/> indicating the reference point used to obtain the new position.</param>
         * <exception cref="IOException">An I/O error occurs.</exception>
         * <exception cref="NotSupportedException">The stream does not support seeking, such as if the stream is constructed from a pipe or console output.</exception>
         * <exception cref="ObjectDisposedException">Methods were called after the stream was closed.</exception>
         * <filterpriority>1</filterpriority>
         * */
        public override Int64 Seek(Int64 offset, SeekOrigin origin) {
            lock (this) {
                if ((origin < SeekOrigin.Begin) || (origin > SeekOrigin.End)) { throw new ArgumentOutOfRangeException(nameof(origin)); }
                if (Disposed) { throw new ObjectDisposedException(nameof(Disposed)); }
                switch (origin) {
                    case SeekOrigin.Begin:
                        {
                        if (offset < 0)      { throw new ArgumentOutOfRangeException(nameof(offset)); }
                        if (offset > Length) { throw new ArgumentOutOfRangeException(nameof(offset)); }
                        position = offset;
                        return position;
                        }
                    case SeekOrigin.Current: { return Seek(position + offset, SeekOrigin.Begin); }
                    case SeekOrigin.End:     { return Seek(Length + offset, SeekOrigin.Begin);   }
                    default: throw new ArgumentOutOfRangeException(nameof(origin));
                    }
                }
            }
        #endregion
        #region M:Write(Byte[],Int32,Int32)
        /**
         * <summary>When overridden in a derived class, writes a sequence of bytes to the current stream and advances the current position within this stream by the number of bytes written.</summary>
         * <param name="buffer">An array of bytes. This method copies count bytes from buffer to the current stream.</param>
         * <param name="offset">The zero-based byte offset in buffer at which to begin copying bytes to the current stream.</param>
         * <param name="count">The number of bytes to be written to the current stream.</param>
         * <exception cref="T:System.ArgumentException">The sum of <paramref name="offset">offset</paramref> and <paramref name="count">count</paramref> is greater than the buffer length.</exception>
         * <exception cref="T:System.ArgumentNullException"><paramref name="buffer">buffer</paramref> is null.</exception>
         * <exception cref="T:System.ArgumentOutOfRangeException"><paramref name="offset">offset</paramref> or <paramref name="count">count</paramref> is negative.</exception>
         * <exception cref="T:System.IO.IOException">An I/O error occured, such as the specified file cannot be found.</exception>
         * <exception cref="T:System.NotSupportedException">The stream does not support writing.</exception>
         * <exception cref="T:System.ObjectDisposedException"><see cref="M:System.IO.Stream.Write(System.Byte[],System.Int32,System.Int32)"></see> was called after the stream was closed.</exception>
         * */
        public override void Write(Byte[] buffer, Int32 offset, Int32 count)
            {
            throw new NotSupportedException();
            }
        #endregion
        #region M:Dispose(Boolean)
        /**
         * <summary>
         * Releases the unmanaged resources used by the <see cref="Stream"/> and optionally releases the managed
         * resources.
         * </summary>
         * <param name="disposing">true to release both managed and unmanaged resources; false to release only unmanaged resources.</param>
         * */
        protected override void Dispose(Boolean disposing) {
            lock (this) {
                if (!Disposed) {
                    base.Dispose(disposing);
                    Disposed = true;
                    }
                }
            }
        #endregion
        #region M:ToArray:Byte[]
        public Byte[] ToArray()
            {
            var size = Length;
            var r = new Byte[size];
            var buffersize = 8;
            var buffer = new Byte[buffersize];
            var offset = 0L;
            var sz = size;
            Seek(0, SeekOrigin.Begin);
            for (;;) {
                Thread.Yield();
                var count = (Int32)Math.Max(0, Math.Min(buffersize, sz - offset));
                if (count == 0) { break; }
                Read(buffer, 0, count);
                for (var i = 0; i < count; i++) {
                    r[offset + i] = buffer[i];
                    }
                offset += buffersize;
                }
            return r;
            }
        #endregion
        #region M:ToString:String
        public override String ToString()
            {
            return $"{Position}:{Length}";
            }
        #endregion

        public abstract ReadOnlyMappingStream Clone();
        public abstract ReadOnlyMappingStream Clone(Int64 offset, Int64 length);
        public abstract ReadOnlyMappingStream Clone(Int64 length);

        private Int64 position;
        protected Boolean Disposed;
        }
    }
