using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using STATSTG = System.Runtime.InteropServices.ComTypes.STATSTG;

namespace BinaryStudio.IO
    {
    public class ComStream : Stream, IStream
        {
        private Boolean Disposed;
        private Stream UnderlyingStream;

        public ComStream(Stream source)
            {
            if (source == null) { throw new ArgumentNullException(nameof(source)); }
            UnderlyingStream = source;
            }

        /// <summary>When overridden in a derived class, clears all buffers for this stream and causes any buffered data to be written to the underlying device.</summary>
        /// <exception cref="T:System.IO.IOException">An I/O error occurs.</exception>
        public override void Flush()
            {
            UnderlyingStream.Flush();
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
            return UnderlyingStream.Seek(offset, origin);
            }

        /// <summary>When overridden in a derived class, sets the length of the current stream.</summary>
        /// <param name="value">The desired length of the current stream in bytes.</param>
        /// <exception cref="T:System.IO.IOException">An I/O error occurs.</exception>
        /// <exception cref="T:System.NotSupportedException">The stream does not support both writing and seeking, such as if the stream is constructed from a pipe or console output.</exception>
        /// <exception cref="T:System.ObjectDisposedException">Methods were called after the stream was closed.</exception>
        public override void SetLength(Int64 value)
            {
            UnderlyingStream.SetLength(value);
            }

        /// <summary>When overridden in a derived class, reads a sequence of bytes from the current stream and advances the position within the stream by the number of bytes read.</summary>
        /// <param name="buffer">An array of bytes. When this method returns, the buffer contains the specified byte array with the values between <paramref name="offset"/> and (<paramref name="offset"/> + <paramref name="count"/> - 1) replaced by the bytes read from the current source.</param>
        /// <param name="offset">The zero-based byte offset in <paramref name="buffer"/> at which to begin storing the data read from the current stream.</param>
        /// <param name="count">The maximum number of bytes to be read from the current stream.</param>
        /// <returns>The total number of bytes read into the buffer. This can be less than the number of bytes requested if that many bytes are not currently available, or zero (0) if the end of the stream has been reached.</returns>
        /// <exception cref="T:System.ArgumentException">The sum of <paramref name="offset" /> and <paramref name="count" /> is larger than the buffer length.</exception>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="buffer"/> is <see langword="null"/>.</exception>
        /// <exception cref="T:System.ArgumentOutOfRangeException"><paramref name="offset"/> or <paramref name="count"/> is negative.</exception>
        /// <exception cref="T:System.IO.IOException">An I/O error occurs.</exception>
        /// <exception cref="T:System.NotSupportedException">The stream does not support reading.</exception>
        /// <exception cref="T:System.ObjectDisposedException">Methods were called after the stream was closed.</exception>
        public override Int32 Read(Byte[] buffer, Int32 offset, Int32 count)
            {
            return UnderlyingStream.Read(buffer, offset, count);
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
            UnderlyingStream.Write(buffer, offset, count);
            }

        public override Boolean CanRead  { get { return UnderlyingStream.CanRead;  }}
        public override Boolean CanSeek  { get { return UnderlyingStream.CanSeek;  }}
        public override Boolean CanWrite { get { return UnderlyingStream.CanWrite; }}
        public override Int64 Length     { get { return UnderlyingStream.Length;   }}
        public override Int64 Position {
            get { return UnderlyingStream.Position;  }
            set { UnderlyingStream.Position = value; }
            }

        /// <summary>Reads a specified number of bytes from the stream object into memory starting at the current seek pointer.</summary>
        /// <param name="buffer">When this method returns, contains the data read from the stream. This parameter is passed uninitialized.</param>
        /// <param name="count">The number of bytes to read from the stream object.</param>
        /// <param name="pcbRead">A pointer to a <see langword="ULONG"/> variable that receives the actual number of bytes read from the stream object.</param>
        void IStream.Read(Byte[] buffer, Int32 count, IntPtr pcbRead)
            {
            var r = new[] { Read(buffer, 0, count) };
            if (pcbRead != IntPtr.Zero)
                {
                Marshal.Copy(r, 0, pcbRead, 1);
                }
            }

        /// <summary>Writes a specified number of bytes into the stream object starting at the current seek pointer.</summary>
        /// <param name="buffer">The buffer to write this stream to.</param>
        /// <param name="count">The number of bytes to write to the stream.</param>
        /// <param name="pcbWritten">On successful return, contains the actual number of bytes written to the stream object. If the caller sets this pointer to <see cref="F:System.IntPtr.Zero"/>, this method does not provide the actual number of bytes written.</param>
        void IStream.Write(Byte[] buffer, Int32 count, IntPtr pcbWritten)
            {
            Write(buffer, 0, count);
            if (pcbWritten != IntPtr.Zero)
                {
                var r = new[] { count };
                Marshal.Copy(r, 0, pcbWritten, 1);
                }
            }

        /// <summary>Changes the seek pointer to a new location relative to the beginning of the stream, to the end of the stream, or to the current seek pointer.</summary>
        /// <param name="offset">The displacement to add to <paramref name="origin"/>.</param>
        /// <param name="origin">The origin of the seek. The origin can be the beginning of the file, the current seek pointer, or the end of the file.</param>
        /// <param name="plibNewPosition">On successful return, contains the offset of the seek pointer from the beginning of the stream.</param>
        void IStream.Seek(Int64 offset, Int32 origin, IntPtr plibNewPosition)
            {
            Seek(offset, (SeekOrigin)origin);
            if (plibNewPosition != IntPtr.Zero)
                {
                var r = new[] { Position };
                Marshal.Copy(r, 0, plibNewPosition, 1);
                }
            }

        /// <summary>Changes the size of the stream object.</summary>
        /// <param name="value">The new size of the stream as a number of bytes.</param>
        void IStream.SetSize(Int64 value)
            {
            SetLength(value);
            }

        /// <summary>Copies a specified number of bytes from the current seek pointer in the stream to the current seek pointer in another stream.</summary>
        /// <param name="target">A reference to the destination stream.</param>
        /// <param name="count">The number of bytes to copy from the source stream.</param>
        /// <param name="pcbRead">On successful return, contains the actual number of bytes read from the source.</param>
        /// <param name="pcbWritten">On successful return, contains the actual number of bytes written to the destination.</param>
        void IStream.CopyTo(IStream target, Int64 count, IntPtr pcbRead, IntPtr pcbWritten)
            {
            throw new NotImplementedException();
            }

        /// <summary>Ensures that any changes made to a stream object that is open in transacted mode are reflected in the parent storage.</summary>
        /// <param name="flags">A value that controls how the changes for the stream object are committed.</param>
        void IStream.Commit(Int32 flags)
            {
            throw new NotImplementedException();
            }

        /// <summary>Discards all changes that have been made to a transacted stream since the last <see cref="M:System.Runtime.InteropServices.ComTypes.IStream.Commit(System.Int32)"/> call.</summary>
        void IStream.Revert()
            {
            throw new NotImplementedException();
            }

        /// <summary>Restricts access to a specified range of bytes in the stream.</summary>
        /// <param name="offset">The byte offset for the beginning of the range.</param>
        /// <param name="count">The length of the range, in bytes, to restrict.</param>
        /// <param name="type">The requested restrictions on accessing the range.</param>
        void IStream.LockRegion(Int64 offset, Int64 count, Int32 type)
            {
            throw new NotImplementedException();
            }

        /// <summary>Removes the access restriction on a range of bytes previously restricted with the <see cref="M:System.Runtime.InteropServices.ComTypes.IStream.LockRegion(System.Int64,System.Int64,System.Int32)"/> method.</summary>
        /// <param name="offset">The byte offset for the beginning of the range.</param>
        /// <param name="count">The length, in bytes, of the range to restrict.</param>
        /// <param name="type">The access restrictions previously placed on the range.</param>
        void IStream.UnlockRegion(Int64 offset, Int64 count, Int32 type)
            {
            throw new NotImplementedException();
            }

        /// <summary>Retrieves the <see cref="T:System.Runtime.InteropServices.STATSTG"/> structure for this stream.</summary>
        /// <param name="pstatstg">When this method returns, contains a <see langword="STATSTG"/> structure that describes this stream object. This parameter is passed uninitialized.</param>
        /// <param name="grfStatFlag">Members in the <see langword="STATSTG" /> structure that this method does not return, thus saving some memory allocation operations.</param>
        void IStream.Stat(out STATSTG pstatstg, Int32 grfStatFlag)
            {
            var filestream = UnderlyingStream as FileStream;
            pstatstg = new STATSTG{
                type = (Int32)STGTY.STGTY_STREAM,
                };
            if (filestream != null)
                {
                pstatstg.cbSize = filestream.Length;
                }
            if (grfStatFlag == (Int32)STATFLAG.STATFLAG_DEFAULT) {
                if (filestream != null) {
                    pstatstg.pwcsName = filestream.Name;
                    }
                }
            }

        /// <summary>Creates a new stream object with its own seek pointer that references the same bytes as the original stream.</summary>
        /// <param name="target">When this method returns, contains the new stream object. This parameter is passed uninitialized.</param>
        void IStream.Clone(out IStream target)
            {
            throw new NotImplementedException();
            }

        /// <summary>Releases the unmanaged resources used by the instance and optionally releases the managed resources.</summary>
        /// <param name="disposing"><see langword="true"/> to release both managed and unmanaged resources; <see langword="false"/> to release only unmanaged resources.</param>
        protected override void Dispose(Boolean disposing) {
            lock(this) {
                if (!Disposed) {
                    if (disposing) {
                        if (UnderlyingStream != null) {
                            UnderlyingStream.Dispose();
                            UnderlyingStream = null;
                            }
                        }
                    base.Dispose(disposing);
                    Disposed = true;
                    }
                }
            }
        }

    [Flags]
    public enum STATFLAG
        {
        STATFLAG_DEFAULT = 0x0,
        STATFLAG_NONAME  = 0x1,
        STATFLAG_NOOPEN  = 0x2
        }

    public enum STGTY
        {
        STGTY_STORAGE = 1,
        STGTY_STREAM,
        STGTY_LOCKBYTES,
        STGTY_PROPERTY
        }
    }