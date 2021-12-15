using System;
using System.Collections.Generic;
using System.IO;

namespace BinaryStudio.IO
    {
    public class ReadOnlyBlockSequenceStream : Stream
        {
        private Int64 position;
        private Byte[] block;
        private Int32 blockoffset;
        private IEnumerator<Byte[]> blocks;

        public ReadOnlyBlockSequenceStream(IEnumerable<Byte[]> blocks) {
            if (blocks == null) { throw new ArgumentNullException(nameof(blocks)); }
            this.blocks = blocks.GetEnumerator();
            }

        /// <summary>When overridden in a derived class, clears all buffers for this stream and causes any buffered data to be written to the underlying device.</summary>
        /// <exception cref="T:System.IO.IOException">An I/O error occurs.</exception>
        public override void Flush()
            {
            throw new NotSupportedException();
            }

        /// <summary>When overridden in a derived class, sets the position within the current stream.</summary>
        /// <param name="offset">A byte offset relative to the <paramref name="origin"/> parameter.</param>
        /// <param name="origin">A value of type <see cref="T:System.IO.SeekOrigin"/> indicating the reference point used to obtain the new position.</param>
        /// <returns>The new position within the current stream.</returns>
        /// <exception cref="T:System.IO.IOException">An I/O error occurs.</exception>
        /// <exception cref="T:System.NotSupportedException">The stream does not support seeking, such as if the stream is constructed from a pipe or console output.</exception>
        /// <exception cref="T:System.ObjectDisposedException">Methods were called after the stream was closed.</exception>
        public override Int64 Seek(Int64 offset, SeekOrigin origin)
            {
            throw new NotSupportedException();
            }

        /// <summary>When overridden in a derived class, sets the length of the current stream.</summary>
        /// <param name="value">The desired length of the current stream in bytes.</param>
        /// <exception cref="T:System.IO.IOException">An I/O error occurs.</exception>
        /// <exception cref="T:System.NotSupportedException">The stream does not support both writing and seeking, such as if the stream is constructed from a pipe or console output.</exception>
        /// <exception cref="T:System.ObjectDisposedException">Methods were called after the stream was closed.</exception>
        public override void SetLength(Int64 value)
            {
            throw new NotSupportedException();
            }

        /// <summary>When overridden in a derived class, reads a sequence of bytes from the current stream and advances the position within the stream by the number of bytes read.</summary>
        /// <param name="buffer">An array of bytes. When this method returns, the buffer contains the specified byte array with the values between <paramref name="offset"/> and (<paramref name="offset"/> + <paramref name="count"/> - 1) replaced by the bytes read from the current source.</param>
        /// <param name="offset">The zero-based byte offset in <paramref name="buffer"/> at which to begin storing the data read from the current stream.</param>
        /// <param name="count">The maximum number of bytes to be read from the current stream.</param>
        /// <returns>The total number of bytes read into the buffer. This can be less than the number of bytes requested if that many bytes are not currently available, or zero (0) if the end of the stream has been reached.</returns>
        /// <exception cref="T:System.ArgumentException">The sum of <paramref name="offset"/> and <paramref name="count"/> is larger than the buffer length.</exception>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="buffer"/> is <see langword="null"/>.</exception>
        /// <exception cref="T:System.ArgumentOutOfRangeException"><paramref name="offset"/> or <paramref name="count"/> is negative.</exception>
        /// <exception cref="T:System.IO.IOException">An I/O error occurs.</exception>
        /// <exception cref="T:System.NotSupportedException">The stream does not support reading.</exception>
        /// <exception cref="T:System.ObjectDisposedException">Methods were called after the stream was closed.</exception>
        public override Int32 Read(Byte[] buffer, Int32 offset, Int32 count)
            {
            if (buffer == null) { throw new ArgumentNullException(nameof(buffer)); }
            if (offset < 0) { throw new ArgumentOutOfRangeException(nameof(offset)); }
            if (count < 0)  { throw new ArgumentOutOfRangeException(nameof(count));  }
            if (buffer.Length - offset < count) { throw new ArgumentOutOfRangeException(nameof(offset)); }
            if (IsDisposed) { throw new ObjectDisposedException("Object already disposed."); }
            var sourcecount = count;
            var targetcount = 0;
            for (;sourcecount >= 0;) {
                if (block != null) {
                    var blockcount = block.Length - blockoffset;
                    if (sourcecount < blockcount) {
                        Array.Copy(block, blockoffset, buffer, offset, sourcecount);
                        blockoffset += sourcecount;
                        targetcount += sourcecount;
                        break;
                        }
                    if (sourcecount == blockcount) {
                        Array.Copy(block, blockoffset, buffer, offset, sourcecount);
                        targetcount += sourcecount;
                        blockoffset = 0;
                        block = null;
                        break;
                        }
                    Array.Copy(block, blockoffset, buffer, offset, blockcount);
                    offset += blockcount;
                    blockoffset = 0;
                    block = null;
                    targetcount += blockcount;
                    sourcecount -= blockcount;
                    }
                else
                    {
                    if (blocks == null) { break; }
                    if (blocks.MoveNext()) {
                        block = blocks.Current;
                        blockoffset = 0;
                        continue;
                        }
                    blocks = null;
                    break;
                    }
                }
            return targetcount;
            }

        /// <summary>When overridden in a derived class, writes a sequence of bytes to the current stream and advances the current position within this stream by the number of bytes written.</summary>
        /// <param name="buffer">An array of bytes. This method copies <paramref name="count"/> bytes from <paramref name="buffer"/> to the current stream.</param>
        /// <param name="offset">The zero-based byte offset in <paramref name="buffer"/> at which to begin copying bytes to the current stream.</param>
        /// <param name="count">The number of bytes to be written to the current stream.</param>
        /// <exception cref="T:System.ArgumentException">The sum of <paramref name="offset"/> and <paramref name="count"/> is greater than the buffer length.</exception>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="buffer"/> is <see langword="null"/>.</exception>
        /// <exception cref="T:System.ArgumentOutOfRangeException"><paramref name="offset"/> or <paramref name="count"/> is negative.</exception>
        /// <exception cref="T:System.IO.IOException">An I/O error occured, such as the specified file cannot be found.</exception>
        /// <exception cref="T:System.NotSupportedException">The stream does not support writing.</exception>
        /// <exception cref="T:System.ObjectDisposedException"><see cref="M:System.IO.Stream.Write(System.Byte[],System.Int32,System.Int32)"/> was called after the stream was closed.</exception>
        public override void Write(Byte[] buffer, Int32 offset, Int32 count)
            {
            throw new NotSupportedException();
            }

        /// <summary>Releases the unmanaged resources used by the <see cref="T:System.IO.Stream"/> and optionally releases the managed resources.</summary>
        /// <param name="disposing"><see langword="true" /> to release both managed and unmanaged resources; <see langword="false"/> to release only unmanaged resources.</param>
        protected override void Dispose(Boolean disposing) {
            if (disposing) {
                block = null;
                blockoffset = 0;
                if (blocks != null) {
                    blocks.Dispose();
                    blocks = null;
                    }
                }
            IsDisposed = true;
            }

        public override Boolean CanRead  { get { return true;  }}
        public override Boolean CanSeek  { get { return false; }}
        public override Boolean CanWrite { get { return true;  }}
        public override Int64 Length     { get { return -1; }}
        public override Int64 Position   {
            get { return position; }
            set { throw new NotSupportedException(); }
            }

        protected Boolean IsDisposed;
        }
    }