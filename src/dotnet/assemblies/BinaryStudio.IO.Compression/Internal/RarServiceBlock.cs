using System;
using System.IO;

namespace BinaryStudio.IO.Compression
    {
    internal class RarServiceBlock : RarBaseBlock
        {
        public String FileName { get; }
        public DateTime? CreationTime   { get;internal set; }
        public DateTime? LastAccessTime { get;internal set; }
        public DateTime? LastWriteTime  { get;internal set; }
        public UInt64 UnpackedSize { get;internal set; }
        public FileAttributes FileAttributes { get;internal set; }
        public UInt32? CRC32 { get;internal set; }
        public Int32 CompressionMethod  { get;internal set; }
        public Int32 CompressionVersion { get;internal set; }
        public Int32 HostOS { get;internal set; }
        public RarServiceBlock(String filename)
            {
            FileName = filename;
            }

        /// <summary>Returns a string that represents the current object.</summary>
        /// <returns>A string that represents the current object.</returns>
        public override String ToString()
            {
            return FileName;
            }
        }
    }