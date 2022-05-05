using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace BinaryStudio.IO.Compression
    {
    internal class RarArchive : IDisposable
        {
        #region M:Dispose(Boolean)
        protected virtual void Dispose(Boolean disposing) {
            if (disposing && !IsDisposed) {
                cache.Clear();
                if (!LeaveOpen) {
                    stream.Close();
                    }
                stream = null;
                cache = null;
                IsDisposed = true;
                }
            }
        #endregion
        #region M:Dispose
        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
            {
            Dispose(true);
            GC.SuppressFinalize(this);
            }
        #endregion
        #region M:Finalize
        /// <summary>
        /// Allows an object to try to free resources and perform other cleanup operations before it is reclaimed by garbage collection.
        /// </summary>
        ~RarArchive()
            {
            Dispose(false);
            }
        #endregion

        public Boolean IsSolidArchive  { get { return mainblock.IsSolidArchive;  }}
        public Boolean IsLockedArchive { get { return mainblock.IsLockedArchive; }}

        public Encoding Encoding { get; }
        private Boolean LeaveOpen { get; }
        private Boolean IsDisposed;
        private RarArchiveMode Mode;
        private Stream stream;
        private Int32 state;
        private RAR_FORMAT type = RAR_FORMAT.RARFMT_NONE;
        private RarBlockReader BlockReader;
        private FileMapping FileMapping;
        private FileMappingMemory FileMappingMemory;
        private BinaryReader reader;

        /// <summary>
        /// Initializes a new instance of the <see cref="RarArchive"/> class from the specified stream.
        /// </summary>
        /// <param name="stream">The stream that contains the archive to be read.</param>
        public RarArchive(Stream stream)
            :this(stream, RarArchiveMode.Read)
            {
            }

        /// <summary>
        /// Initializes a new instance of the <see cref="RarArchive"/> class from the specified stream and with the specified mode.
        /// </summary>
        /// <param name="stream">The input or output stream.</param>
        /// <param name="mode">One of the enumeration values that indicates whether the rar archive is used to read, create, or update entries.</param>
        public RarArchive(Stream stream, RarArchiveMode mode)
            :this(stream, mode, false)
            {
            }

        /// <summary>
        /// Initializes a new instance of the <see cref="RarArchive"/> class on the specified stream for the specified mode, and optionally leaves the stream open.
        /// </summary>
        /// <param name="stream">The input or output stream.</param>
        /// <param name="mode">One of the enumeration values that indicates whether the rar archive is used to read, create, or update entries.</param>
        /// <param name="leaveopen"><see langword="true"/> to leave the stream open after the <see cref="RarArchive"/> object is disposed; otherwise, <see langword="false"/>.</param>
        public RarArchive(Stream stream, RarArchiveMode mode, Boolean leaveopen)
            :this(stream, mode, leaveopen, null)
            {
            }

        /// <summary>
        /// Initializes a new instance of the <see cref="RarArchive"/> class on the specified stream for the specified mode, uses the specified encoding for entry names, and optionally leaves the stream open.
        /// </summary>
        /// <param name="stream">The input or output stream.</param>
        /// <param name="mode">One of the enumeration values that indicates whether the rar archive is used to read, create, or update entries.</param>
        /// <param name="leaveopen"><see langword="true"/> to leave the stream open after the <see cref="RarArchive"/> object is disposed; otherwise, <see langword="false"/>.</param>
        /// <param name="encoding">The encoding to use when reading or writing entry names in this archive. Specify a value for this parameter only when an encoding is required for interoperability with rar archive tools and libraries that do not support UTF-8 encoding for entry names.</param>
        public unsafe RarArchive(Stream stream, RarArchiveMode mode, Boolean leaveopen, Encoding encoding)
            {
            if (stream == null) { throw new ArgumentNullException(nameof(stream)); }
            Encoding = encoding ?? Encoding.ASCII;
            LeaveOpen = leaveopen;
            this.stream = stream;
            switch (Mode = mode) {
                case RarArchiveMode.Read:
                    if (!stream.CanRead) { throw new ArgumentOutOfRangeException(nameof(stream)); }
                    if (!stream.CanSeek) { throw new ArgumentOutOfRangeException(nameof(stream)); }
                    if (stream is FileStream file) {
                        FileMapping = new FileMapping(file.SafeFileHandle);
                        FileMappingMemory = new FileMappingMemory(FileMapping);
                        reader = new BinaryReader(stream, Encoding);
                        var size = FileMapping.Size;
                        var mapping = (Byte*)(void*)FileMappingMemory;
                        var type = RAR_FORMAT.RARFMT_NONE;
                        if ((size >= 1) && (mapping[0] == 0x52)) {
                            if ((size >= 4) && (mapping[1] == 0x45) && (mapping[2] == 0x7e) && (mapping[3] == 0x5e)) { type = RAR_FORMAT.RARFMT14; }
                            else if ((size >= 7) && (mapping[1] == 0x61) && (mapping[2] == 0x72) && (mapping[3] == 0x21) && (mapping[4] == 0x1a) && (mapping[5] == 0x07))
                                {
                                     if (mapping[6] == 0) { type = RAR_FORMAT.RARFMT15; }
                                else if (mapping[6] == 1) { type = RAR_FORMAT.RARFMT50; }
                                else if ((mapping[6] > 1) && (mapping[6] < 5)) { type = RAR_FORMAT.RARFMT_FUTURE; }
                                }
                            }
                        if (type == RAR_FORMAT.RARFMT_NONE)   { throw new InvalidDataException();  }
                        if (type == RAR_FORMAT.RARFMT_FUTURE) { throw new NotSupportedException(); }
                        this.type = type;
                        switch (type)
                            {
                            case RAR_FORMAT.RARFMT50:
                                if ((mapping[7] != 0) && (mapping[7] != 1)) { throw new InvalidDataException(); }
                                BlockReader = new RarBlockReader50(Encoding, reader);
                                break;
                            case RAR_FORMAT.RARFMT14: BlockReader = new RarBlockReader14(Encoding, reader); break;
                            case RAR_FORMAT.RARFMT15: BlockReader = new RarBlockReader15(Encoding, reader); break;
                            default: throw new NotSupportedException();
                            }
                        stream.Seek(BlockReader.MarkHeadSize, SeekOrigin.Current);
                        position = stream.Position;
                        Entries.FirstOrDefault();
                        }
                    break;
                case RarArchiveMode.Create:
                    if (!stream.CanWrite) { throw new ArgumentOutOfRangeException(nameof(stream)); }
                    throw new NotImplementedException();
                case RarArchiveMode.Update:
                    if (!stream.CanWrite) { throw new ArgumentOutOfRangeException(nameof(stream)); }
                    throw new NotImplementedException();
                default: throw new ArgumentOutOfRangeException(nameof(mode), mode, null);
                }
            }

        private RarArchiveEntry NextEntry(RarArchiveEntry previous) {
            if (eof) { return null; }
            RarBaseBlock block;
            if (mainblock == null) {
                for (;;) {
                    block = BlockReader.NextBlock();
                    if (block == null) { return null; }
                    if (block is RarMainBlock r)
                        {
                        mainblock = r;
                        break;
                        }
                    }
                }
            if (mainblock != null) {
                for (;;)
                    {
                    block = BlockReader.NextBlock();
                    if (block == null) { return null; }
                    if (block is RarEndOfArcBlock) {
                        eof = true;
                        break;
                        }
                    if (block is RarFileBlock fileblock) {
                        return new RarArchiveEntry(
                            this,
                            previous,
                            fileblock);
                        }
                    Debug.Print($"block:{block.GetType()}:{block}");
                    }
                }
            return null;
            }

        public IEnumerable<RarArchiveEntry> Entries { get {
            if (IsDisposed) { throw new ObjectDisposedException("Object already disposed."); }
            foreach (var i in cache) { yield return i; }
            for (;;) {
                using (stream.StorePosition()) {
                    stream.Seek(position, SeekOrigin.Begin);
                    var i = NextEntry(cache.Last?.Value);
                    if (i != null)
                        {
                        position = stream.Position;
                        cache.AddLast(i);
                        yield return i;
                        }
                    else
                        {
                        break;
                        }
                    }
                }
            }}

        private const Int32 SIZEOF_MARKHEAD3 = 7;
        private const Int32 SIZEOF_MARKHEAD5 = 8;
        private RarMainBlock mainblock;
        private Boolean eof;
        private Int64 position;
        private LinkedList<RarArchiveEntry> cache = new LinkedList<RarArchiveEntry>();
        }
    }
