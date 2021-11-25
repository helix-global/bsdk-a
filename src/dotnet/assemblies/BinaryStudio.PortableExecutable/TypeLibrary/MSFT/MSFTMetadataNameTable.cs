using System;
using System.Runtime.InteropServices;
using System.Text;

namespace BinaryStudio.PortableExecutable.TypeLibrary
    {
    internal class MSFTMetadataNameTable : MSFTMetadataTableDescriptor<String>
        {
        private const Int32 MSFT_SEGMENT_INDEX_NAME_TABLE = 7;
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        private struct ENTRY {
            private readonly UInt32 RefType;
            private readonly UInt32 NextHash;
            public  readonly Byte   NameLength;
            private readonly Byte   Flags;
            private readonly UInt16 HashCode;
            }

        public override String this[Int32 i] { get {
            if (IsDisposed) { throw new ObjectDisposedException(nameof(cache)); }
            if (i < 0) { return null; }
            #if FEATURE_ADVANCE_NAME_TABLE_LOADING
            return cache[i];
            #else
            if (!cache.TryGetValue(i, out var r))
            unsafe
                {
                var e = (ENTRY*)(Source + i);
                var n = (Int32)e->NameLength;
                cache[i] = r = Encoding.GetString((Byte*)(e + 1), n);
                }
            return r;
            #endif
            }}

        #if !FEATURE_ADVANCE_NAME_TABLE_LOADING
        public Encoding Encoding { get; }
        #endif
        public unsafe MSFTMetadataNameTable(MetadataScope scope, Byte* source, Encoding encoding)
            : base(scope, source, MSFT_SEGMENT_INDEX_NAME_TABLE)
            {
            if (encoding == null) { throw new ArgumentNullException(nameof(encoding)); }
            #if FEATURE_ADVANCE_NAME_TABLE_LOADING
            var offset = 0;
            var r = Source;
            var c = Header->NameTableCount;
            for (var i = 0; i < c; ++i) {
                var ri = (ENTRY*)(r + offset);
                var n  = (Int32)ri->NameLength;
                cache[offset] = encoding.GetString((Byte*)(ri + 1), n);
                if ((n % 4) != 0) { n += 4 - (n % 4); }
                offset += sizeof(ENTRY) + n;
                }
            #else
            Encoding = encoding;
            #endif
            }
        }
    }