using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Win32;

namespace BinaryStudio.PortableExecutable.TypeLibrary
    {
    internal class SLTGImportManager
        {
        private const Byte FIRSTBYTE = ('I' * 2 + 'M') & 0xFF;

        public IList<SLTGTypeIndex> References { get; }
        public unsafe SLTGImportManager(SourceReader reader, Int32 offset) {
            References = new List<SLTGTypeIndex>();
            if (offset != -1) {
                reader.BaseStream.Seek(offset, SeekOrigin.Begin);
                var firstbyte = reader.ReadByte();
                var version   = reader.ReadByte();
                if (firstbyte != FIRSTBYTE) { throw new ArgumentOutOfRangeException(nameof(reader)); }
                if (version   != 0)         { throw new ArgumentOutOfRangeException(nameof(reader)); }
                var m_himptypeFreeList = reader.ReadInt16();
                var m_rghimptypeBucket = reader.ReadBytes(32*sizeof(Int16));
                var m_bdTubimptype = new BLK_DESC(reader);
                var m_bmData = new BlockManager(reader);
                var entries = new SLTG_UB_IMPTYPE[m_bdTubimptype.m_qbMemBlock.Length / sizeof(SLTG_UB_IMPTYPE)];
                fixed (Byte* r = m_bdTubimptype.m_qbMemBlock) {
                    for (var i = 0; i < entries.Length; ++i) {
                        entries[i] = ((SLTG_UB_IMPTYPE*)r)[i];
                        }
                    }
                for (var i = 0; i < entries.Length; ++i) {
                    References.Add(new SLTGTypeIndex(reader.ReadString()));
                    }
                }
            }
        }
    }