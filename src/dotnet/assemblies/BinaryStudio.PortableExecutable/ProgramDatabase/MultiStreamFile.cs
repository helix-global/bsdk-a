using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using BinaryStudio.IO;
using Microsoft.Win32;

namespace BinaryStudio.PortableExecutable.ProgramDatabase
    {
    public class MultiStreamFile : MetadataObject
        {
        private static readonly Byte[] MSF_HDR_MAGIC    = Encoding.ASCII.GetBytes("Microsoft C/C++ program database 2.00\r\n\x1a\x4a\x47\0");
        private static readonly Byte[] BIGMSF_HDR_MAGIC = Encoding.ASCII.GetBytes("Microsoft C/C++ MSF 7.00\r\n\x1a\x44\x53\0\0\0");

        //[StructLayout(LayoutKind.Sequential)]
        //private struct MultiStreamFileHeader7
        //    {
        //    public unsafe fixed Byte Magic[0x1E];
        //    public readonly Int32 PageSize;
        //    public readonly Int32 PageNumber;
        //    public readonly Int32 CurrentPageNumber;
        //    }

        //[StructLayout(LayoutKind.Sequential)]
        //public struct StreamInfo
        //    {
            
        //    }

        public MultiStreamFile(MetadataScope scope)
            : base(scope)
            {
            }

        private class StreamDescriptor
            {
            public Int32 StreamSize;
            public Int32 StreamPageCount;
            public Int32[] PageIndices;

            #region M:GetByte(IntPtr,Int32):Byte
            private static unsafe Byte GetByte(IntPtr source, Int32 index)
                {
                return *(((Byte*)source) + index);
                }
            #endregion
            #region M:GetStream(Int32[],IntPtr,Int32):IEnumerable<Byte>
            public IEnumerable<Byte> GetStream(Int32[] pages, IntPtr source, Int32 pagesize) {
                var count = StreamSize;
                for (var i = 0; i < StreamPageCount; i++) {
                    var page = (IntPtr)((pages[PageIndices[i]]) + (Int64)source);
                    while (count > 0) {
                        var c = Math.Min(pagesize, count);
                        for (var j = 0; j < c; j++) { yield return GetByte(page, j); }
                        count -= c;
                        }
                    }
                }
            #endregion

            /**
             * <summary>Returns a string that represents the current object.</summary>
             * <returns>A string that represents the current object.</returns>
             */
            public override String ToString()
                {
                return $"StreamSize={StreamSize:D6},StreamPageCount={StreamPageCount:D3}";
                }
            }

        #region M:IsValidMagic(Byte[],[Ref]Byte*,[Ref]Int64):Boolean
        private static unsafe Boolean IsValidMagic(Byte[] magic, ref Byte* source, ref Int64 size) {
            if (magic.Length < size) {
                var c = magic.Length;
                for (var i = 0; i < c; ++i) {
                    if (source[i] != magic[i]) {
                        return false;
                        }
                    }
                source += c;
                size   -= c;
                return true;
                }
            return false;
            }
        #endregion
        #region M:ReadInt32([Ref]Byte*,[Ref]Int64):Int32
        private static unsafe Int32 ReadInt32(ref Byte* source, ref Int64 size)
            {
            if (size < sizeof(Int32)) { throw new InvalidDataException(); }
            var r = *(Int32*)source;
            source += sizeof(Int32);
            size   -= sizeof(Int32);
            return r;
            }
        #endregion
        #region M:ReadInt32Array(Int32,[Ref]Byte*,[Ref]Int64):Int32[]
        private static unsafe Int32[] ReadInt32Array(Int32 count,ref Byte* source, ref Int64 size)
            {
            if (size < sizeof(Int32)*count) { throw new InvalidDataException(); }
            var r = ReadInt32Array(count, (Int32*)source);
            source += sizeof(Int32)*count;
            size   -= sizeof(Int32)*count;
            return r;
            }
        #endregion
        #region M:ReadInt32Array(Int32,Int32*):Int32[]
        private static unsafe Int32[] ReadInt32Array(Int32 count,Int32* source)
            {
            var r = new Int32[count];
            for (var i = 0; i < count; i++) {
                r[i] = source[i];
                }
            return r;
            }
        #endregion
        #region M:LoadRootStream(Int32*,Int32):StreamDescriptor[]
        private static unsafe StreamDescriptor[] LoadRootStream(Int32* source, Int32 pagesize) {
            var streamcount = *source++;
            var r = new StreamDescriptor[streamcount];
            for (var i = 0; i < streamcount; i++) {
                var size = *source++;
                var pagecount = (size + pagesize - 1)/pagesize;
                r[i] = new StreamDescriptor
                    {
                    StreamSize = size,
                    StreamPageCount = pagecount,
                    PageIndices = new Int32[pagecount]
                    };
                }
            for (var i = 0; i < streamcount; i++) {
                for (var j = 0; j < r[i].StreamPageCount; j++) {
                    r[i].PageIndices[j] = *source++;
                    }
                }
            return r;
            }
        #endregion

        [StructLayout(LayoutKind.Sequential)]
        private struct NDBIHdr
            {
            public readonly Int32 verSignature;
            public readonly DBIImp verHdr;
            public readonly Int32 age;
            public readonly Int16 snGSSyms;
            public readonly Int16 usVerAll;
            public readonly Int16 snPSSyms;
            public readonly Int16 usVerPdbDllBuild;
            public readonly Int16 snSymRecs;
            public readonly Int16 usVerPdbDllRBld;
            public readonly Int32 cbGpModi;
            public readonly Int32 cbSC;
            public readonly Int32 cbSecMap;
            public readonly Int32 cbFileInfo;
            public readonly Int32 cbTSMap;
            public readonly Int32 iMFC;
            public readonly Int32 cbDbgHdr;
            public readonly Int32 cbECInfo;
            public readonly Int16 flags;
            public readonly IMAGE_FILE_MACHINE wMachine;
            public readonly Int32 rgulReserved;
            }

        private enum DBISCImp : uint
            {
            DBISCImpvV60  = 0xeffe0000 + 19970605,
            DBISCImpv     = DBISCImpvV60,
            DBISCImpv2    = 0xeffe0000 + 20140516
            }

        private enum DBIImp
            {
            DBIImpvV41  = 00930803,
            DBIImpvV50  = 19960307,
            DBIImpvV60  = 19970606,
            DBIImpvV70  = 19990903,
            DBIImpvV110 = 20091201,
            DBIImpv     = DBIImpvV70,
            }

        private unsafe void LoadStream(StreamDescriptor descriptor, Byte[] block) {
            if (block.Length > 0) {
                fixed(Byte* i = block) {
                    var nhdr = (NDBIHdr*)i;
                    if (nhdr->verSignature == -1) {
                        if (Enum.IsDefined(typeof(IMAGE_FILE_MACHINE), nhdr->wMachine)) {
                            LoadStream(descriptor, nhdr);
                            }
                        }
                    }
                }
            }

        private unsafe void LoadStream(StreamDescriptor descriptor, NDBIHdr* source)
            {

            }

        protected internal override unsafe Boolean Load(out Exception e, Byte* source, Int64 size) {
            if (source == null) { throw new ArgumentNullException(nameof(source)); }
            e = null;
            // TODO: ??????
            return false;
            var src = source;
            if (IsValidMagic(MSF_HDR_MAGIC, ref source, ref size))
                {
                throw new NotImplementedException();
                }
            if (IsValidMagic(BIGMSF_HDR_MAGIC, ref source, ref size)) {
                var pagesize   = ReadInt32(ref source, ref size); ReadInt32(ref source, ref size);
                var pagecount  = ReadInt32(ref source, ref size);
                var streamsize = ReadInt32(ref source, ref size); ReadInt32(ref source, ref size);
                var pagenumber = ReadInt32(ref source, ref size);
                var pages = new Int32[pagecount];
                var i = 0;
                for (i = 0; i < pagecount; i++)
                    {
                    pages[i] = (i*pagesize);
                    }
                var rootpageoffset = pages[pagenumber - 1];
                var rootpage = src + rootpageoffset;
                var streams = LoadRootStream((Int32*)(rootpage), pagesize);
                i = 0;
                streams.AsParallel().ForAll(stream=>{
                    var block = new Byte[stream.StreamSize];
                    using (var data = new ForwardOnlyByteStream(stream.GetStream(pages, (IntPtr)src, pagesize))) {
                        data.Read(block, 0, block.Length);
                        }
                    LoadStream(stream, block);
                    });
                //foreach (var stream in streams) {
                //    Debug.Print($@" Stream:{i:D2} Size=""{stream.StreamSize:D6}"",PageCount=""{stream.StreamPageCount}""");
                //    foreach (var pageindex in stream.PageIndices) {
                //        Debug.Print($@" PageIndex:{pageindex:D3}");
                //        }
                //    using (var data = new ForwardOnlyByteStream(stream.GetStream(pages, (IntPtr)src, pagesize))) {
                //        foreach (var L in Base32Formatter.Format(data, Base32FormattingFlags.None)) {
                //            //Debug.Print(L);
                //            }
                //        }
                //    i++;
                //    }
                //var streamsizes = ReadInt32Array(streamcount, (Int32*)(rootpage + sizeof(Int32)));
                //var offset = rootpageoffset + (streamcount + 1)*sizeof(Int32);
                //Debug.Print($"Count:{streamcount}");
                //for (var i = 0; i < streamcount; i++) {
                //    if (streamsizes[i] > 0) {
                //        var streampagecount = (streamsizes[i] + pagesize - 1)/pagesize;
                //        Debug.Print($@" Stream:{i:D2} Size=""{streamsizes[i]:D6}"",PageCount=""{streampagecount}"",Offset=""{offset:X8}""");
                //        var pageindexes = new Int32[streampagecount];
                //        for (var j = 0; j < streampagecount; j++) {
                //            pageindexes[j] = *(Int32*)(src + offset);
                //            Debug.Print($@" PageIndex:{pageindexes[j]}");
                //            offset += sizeof(Int32);
                //            }
                //        Debug.Print($@" Stream:{i:D2} EndOfStream=""{offset:X8}""");
                //        }
                //    }
                //throw new NotSupportedException();
                }
            //throw new NotSupportedException();
            }
        }
    }