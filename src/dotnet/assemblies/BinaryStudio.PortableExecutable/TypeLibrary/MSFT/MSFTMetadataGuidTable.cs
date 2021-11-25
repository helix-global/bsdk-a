using System;
using System.Runtime.InteropServices;

namespace BinaryStudio.PortableExecutable.TypeLibrary
    {
    internal class MSFTMetadataGuidTable : MSFTMetadataTableDescriptor<Guid?>
        {
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        private struct ENTRY
            {
            public  readonly Guid   Value;
            private readonly UInt32 TypeReference;
            private readonly UInt32 NextHash;
            }

        private const Int32 MSFT_SEGMENT_INDEX_GUID_TABLE = 5;
        public unsafe MSFTMetadataGuidTable(MetadataScope scope, Byte* source)
            : base(scope, source, MSFT_SEGMENT_INDEX_GUID_TABLE)
            {
            #if FEATURE_ADVANCE_GUID_TABLE_LOADING
            var e = (ENTRY*)Source;
            var sz = Size;
            var offset = 0;
            for (;sz > 0;) {
                cache[offset] = e->Value;
                offset += sizeof(ENTRY);
                sz -= sizeof(ENTRY);
                e++;
                }
            #endif
            }

        public override Guid? this[Int32 i] { get {
            if (IsDisposed) { throw new ObjectDisposedException(nameof(cache)); }
            if (i < 0) { return null; }
            #if FEATURE_ADVANCE_GUID_TABLE_LOADING
            return cache[i];
            #else
            if (!cache.TryGetValue(i, out var r))
            unsafe
                {
                var e = (ENTRY*)(Source + i);
                cache[i] = r = e->Value;
                }
            return r;
            #endif
            }}
        }
    }