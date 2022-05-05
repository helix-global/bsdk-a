using System;

namespace BinaryStudio.IO.Compression
    {
    internal class RarBaseBlock
        {
        public Int64  DataOffset { get;internal set; }
        public Int64? DataSize { get;internal set; }
        }
    }