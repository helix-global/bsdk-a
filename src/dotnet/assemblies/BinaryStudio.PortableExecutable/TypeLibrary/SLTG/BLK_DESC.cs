using System;
using System.IO;

namespace BinaryStudio.PortableExecutable.TypeLibrary
    {
    internal class BLK_DESC
        {
        public Byte[] m_qbMemBlock { get; }
        public BLK_DESC(BinaryReader reader) {
            if (reader == null) { throw new ArgumentNullException(nameof(reader)); }
            var blocksize = reader.ReadInt32();
            m_qbMemBlock = reader.ReadBytes(blocksize);
            }
        }
    }