using System;
using System.IO;

namespace BinaryStudio.PortableExecutable.TypeLibrary
    {
    internal class BlockManager
        {
        public BLK_DESC m_blkdesc { get; }
        public BlockManager(BinaryReader reader) {
            if (reader == null) { throw new ArgumentNullException(nameof(reader)); }
            var m_hfreechunk = reader.ReadInt16();
            var state     = reader.ReadByte();
            m_blkdesc = new BLK_DESC(reader);
            }
        }
    }