using System;
using System.IO;
using System.Reflection;
using BinaryStudio.DirectoryServices;

namespace BinaryStudio.IO
    {
    public class ReadOnlyStream : ReadOnlyMappingStream
        {
        private Stream source;
        private readonly Boolean closable;
        private const Int32 BlockSize = 10;

        public ReadOnlyStream(Stream source)
            :this(source, true)
            {
            }

        private ReadOnlyStream(Stream source, Boolean closable) {
            if (source == null) { throw new ArgumentNullException(nameof(source)); }
            this.source = source;
            this.closable = closable;
            }

        /// <summary>When overridden in a derived class, reads a sequence of bytes from the current stream and advances the position within the stream by the number of bytes read.</summary>
        /// <param name="buffer">An array of bytes. When this method returns, the buffer contains the specified byte array with the values between <paramref name="offset"/> and (<paramref name="offset"/> + <paramref name="count"/> - 1) replaced by the bytes read from the current source.</param>
        /// <param name="offset">The zero-based byte offset in <paramref name="buffer"/> at which to begin storing the data read from the current stream.</param>
        /// <param name="count">The maximum number of bytes to be read from the current stream.</param>
        /// <returns>The total number of bytes read into the buffer. This can be less than the number of bytes requested if that many bytes are not currently available, or zero (0) if the end of the stream has been reached.</returns>
        /// <exception cref="T:System.ArgumentException">The sum of <paramref name="offset"/> and <paramref name="count"/> is larger than the buffer length.</exception>
        /// <exception cref="T:System.ArgumentNullException"> <paramref name="buffer"/> is <see langword="null"/>.</exception>
        /// <exception cref="T:System.ArgumentOutOfRangeException"><paramref name="offset"/> or <paramref name="count"/> is negative.</exception>
        /// <exception cref="T:System.IO.IOException">An I/O error occurs.</exception>
        /// <exception cref="T:System.NotSupportedException">The stream does not support reading.</exception>
        /// <exception cref="T:System.ObjectDisposedException">Methods were called after the stream was closed.</exception>
        public override Int32 Read(Byte[] buffer, Int32 offset, Int32 count)
            {
            if (buffer == null) { throw new ArgumentNullException(nameof(buffer)); }
            if (offset < 0) { throw new ArgumentOutOfRangeException(nameof(offset)); }
            if (count  < 0) { throw new ArgumentOutOfRangeException(nameof(count));  }
            if (buffer.Length - offset < count) { throw new ArgumentOutOfRangeException(nameof(offset)); }
            if (IsDisposed) { throw new ObjectDisposedException("Object already disposed."); }
            return source.Read(buffer, offset, count);
            }

        public override Int64 Length { get { return source.Length; }}
        protected override Int64 Offset { get; }

        public override ReadOnlyMappingStream Clone() {
            if (CanSeek) {
                var offset = Position;
                var filename = Path.GetTempFileName();
                }
            throw new NotImplementedException();
            }

        public override ReadOnlyMappingStream Clone(Int64 offset, Int64 length)
            {
            throw new NotImplementedException();
            }

        public override ReadOnlyMappingStream Clone(Int64 length) {
            if (length < 0) { throw new ArgumentOutOfRangeException(nameof(length)); }
            if (CanSeek) {
                var offset = Position;
                var assembly = Assembly.GetEntryAssembly();
                var folder = Path.Combine(Path.GetTempPath(), $"{{{assembly.FullName}}}");
                if (!Directory.Exists(folder)) { Directory.CreateDirectory(folder); }
                var filename = PathUtils.GetTempFileName(folder, "str");
                if (File.Exists(filename)) { File.Delete(filename); }
                var block = new Byte[BlockSize];
                using (var output = File.OpenWrite(filename = Path.Combine(folder, Path.GetFileName(filename))))
                    {
                    while (length > 0) {
                        var blockcount = (Int32)Math.Min(block.Length, length);
                        var sourcecount = source.Read(block, 0, blockcount);
                        if (sourcecount == 0) { break; }
                        output.Write(block, 0, sourcecount);
                        length -= sourcecount;
                        }
                    }
                source.Seek(offset, SeekOrigin.Begin);
                return new ReadOnlyFileMappingStream(filename, true);
                }
            else
                {
                var assembly = Assembly.GetEntryAssembly();
                var folder = Path.Combine(Path.GetTempPath(), $"{{{assembly.FullName}}}");
                if (!Directory.Exists(folder)) { Directory.CreateDirectory(folder); }
                var filename = PathUtils.GetTempFileName(folder, "str");
                if (File.Exists(filename)) { File.Delete(filename); }
                var block = new Byte[BlockSize];
                using (var output = File.OpenWrite(filename = Path.Combine(folder, Path.GetFileName(filename))))
                    {
                    while (length > 0) {
                        var blockcount = (Int32)Math.Min(block.Length, length);
                        var sourcecount = source.Read(block, 0, blockcount);
                        if (sourcecount == 0) { break; }
                        output.Write(block, 0, sourcecount);
                        length -= sourcecount;
                        }
                    }
                return new ReadOnlyFileMappingStream(filename, true);
                }
            }

        public override Boolean CanSeek { get { return source.CanSeek; }}

        /// <summary>Releases the unmanaged resources used by the <see cref="T:System.IO.Stream"/> and optionally releases the managed resources.</summary>
        /// <param name="disposing"><see langword="true"/> to release both managed and unmanaged resources; <see langword="false"/> to release only unmanaged resources.</param>
        protected override void Dispose(Boolean disposing) {
            if (!IsDisposed) {
                lock(this) {
                    if (source != null) {
                        if (closable)
                            {
                            source.Dispose();
                            }
                        source = null;
                        }
                    }
                base.Dispose(disposing);
                }
            }
        }
    }