using System;
using System.Text;

namespace BinaryStudio.PortableExecutable.TypeLibrary
    {
    internal class MSFTMetadataStringTable : MSFTMetadataTableDescriptor<String>
        {
        private const Int32 MSFT_SEGMENT_INDEX_STRING_TABLE = 8;
        public unsafe MSFTMetadataStringTable(MetadataScope scope, Byte* source, Encoding encoding)
            : base(scope, source, MSFT_SEGMENT_INDEX_STRING_TABLE)
            {
            if (encoding == null) { throw new ArgumentNullException(nameof(encoding)); }
            #if FEATURE_ADVANCE_STRING_TABLE_LOADING
            var sz = Size;
            var offset = 0;
            for (; sz > 0; ) {
                var n = (Int32)(*(UInt16*)(Source + offset));
                cache[offset] = encoding.GetString(Source + offset + 2, n);
                n += 2;
                if ((n % 4) != 0) { n += 4 - (n % 4); }
                offset += n;
                sz -= n;
                }
            #else
            Encoding = encoding;
            #endif
            }

        #if !FEATURE_ADVANCE_STRING_TABLE_LOADING
        public Encoding Encoding { get; }
        #endif

        public override String this[Int32 i] { get {
            if (IsDisposed) { throw new ObjectDisposedException(nameof(cache)); }
            if (i < 0) { return null; }
            #if FEATURE_ADVANCE_STRING_TABLE_LOADING
            return cache[i];
            #else
            if (!cache.TryGetValue(i, out var r))
            unsafe
                {
                var n = (Int32)(*(UInt16*)(Source + i));
                cache[i] = r = Encoding.GetString((Byte*)(Source + i + 2), n);
                }
            return r;
            #endif
            }}
        }
    }