using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using SYSKIND = System.Runtime.InteropServices.ComTypes.SYSKIND;

namespace BinaryStudio.PortableExecutable.TypeLibrary
    {
    internal class NameManager
        {
        private readonly BlockManager store;
        private readonly IDictionary<Int32, String> cache = new Dictionary<Int32, String>();
        private readonly SYSKIND syskind;
        private readonly Int32 lcid;

        public unsafe NameManager(BinaryReader reader, SYSKIND syskind, Int32 lcid) {
            this.syskind = syskind;
            this.lcid = lcid;
            var x = new BlockManager(reader);
            reader.ReadInt16();
            store = new BlockManager(reader);
            fixed (Byte* source = store.m_blkdesc.m_qbMemBlock) {
                var offset = 0;
                for (;;)
                    {
                    var ni = (NAME_INFO*)(source + offset);
                    var bytes = new List<Byte>();
                    var i = (Byte*)(ni + 1);
                    var j = 0;
                    while (*i != 0) {
                        bytes.Add(*i++);
                        j++;
                        }
                    var r = Encoding.ASCII.GetString(bytes.ToArray());
                    cache.Add(offset, r);
                    j++;
                    offset += j + sizeof(NAME_INFO);
                    if ((offset & 0x1) == 0x1) { offset++; }
                    if (offset > store.m_blkdesc.m_qbMemBlock.Length) { break; }
                    }
                }
            return;
            }

        [Flags]
        private enum NAME_FLAGS
            {
            APPTOKEN     = 0x01,
            PRESERVECASE = 0x02,
            GLOBAL       = 0x04,
            MULTIPLE     = 0x08,
            AMBIGUOUS    = 0x10,
            NONPARAM     = 0x20
            }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        private struct NAME_INFO {
            public  readonly UInt16 Hash;
            private readonly Int16 Left;
            private readonly Int16 Right;
            private readonly UInt16 flags;
            private NAME_FLAGS Flags { get { return (NAME_FLAGS)(flags & 0x3F); }}
            private Int32 Index      { get { return ((flags & 0xFFC0) >> 6);    }}
            }

        public unsafe String this[Int32 key] { get {
            String r;
            if (!cache.TryGetValue(key, out r))
                fixed (Byte* source = store.m_blkdesc.m_qbMemBlock) {
                    var ni = (NAME_INFO*)(source + key);
                    var bytes = new List<Byte>();
                    var i = (Byte*)(ni + 1);
                    while (*i != 0) {
                        bytes.Add(*i++);
                        }
                    r = Encoding.ASCII.GetString(bytes.ToArray());
                    cache.Add(key, r);
                    #if DEBUG
                    var hashval = LHashValOfNameSys(syskind, lcid, r) & 0xFFFF;
                    Debug.Assert(hashval == ni->Hash);
                    #endif
                    }
            return r;
            }}

        [DllImport("oleaut32.dll", CharSet = CharSet.Auto)] private static extern UInt32 LHashValOfNameSys(SYSKIND syskind, Int32 lcid, String name);
        }
    }