using System;
using System.IO;

namespace BinaryStudio.IO.Compression
    {
    internal class RarArchiveEntry
        {
        public String FileName { get { return source.FileName; }}

        internal RarArchiveEntry(RarArchive archive, RarArchiveEntry previous, RarFileBlock source) {
            this.source = source;
            }

        public Stream Open()
            {
            return null;
            }

        private readonly RarFileBlock source;
        }
    }