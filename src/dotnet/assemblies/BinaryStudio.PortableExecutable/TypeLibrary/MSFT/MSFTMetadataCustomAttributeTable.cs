using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.InteropServices;

namespace BinaryStudio.PortableExecutable.TypeLibrary
    {
    internal class MSFTMetadataCustomAttributeTable : MSFTMetadataTableDescriptor<IList<TypeLibraryCustomAttribute>>
        {
        private const Int32 MSFT_SEGMENT_INDEX_CUSTOM_DATA      = 11;
        private const Int32 MSFT_SEGMENT_INDEX_CUSTOM_DATA_GUID = 12;

        private unsafe Byte* offsets;
        private MSFTMetadataGuidTable g;
        private MSFTMetadataTypeLibrary storage;
        public unsafe MSFTMetadataCustomAttributeTable(MetadataScope scope, MSFTMetadataTypeLibrary storage, Byte* source, MSFTMetadataGuidTable g)
            : base(scope, source, MSFT_SEGMENT_INDEX_CUSTOM_DATA)
            {
            if (g == null) { throw new ArgumentNullException(nameof(g)); }
            if (storage == null) { throw new ArgumentNullException(nameof(storage)); }
            this.g = g;
            this.storage = storage;
            var header = (HEADER*)source;
            var fi = (SEGMENT*)(((UInt32*)(header + 1)) + (Int64)(header->TypeInfoCount));
            offsets = source + (fi + MSFT_SEGMENT_INDEX_CUSTOM_DATA_GUID)->Offset;
            }

        /// <summary>
        /// The custom data table directory entry.
        /// </summary>
        [StructLayout(LayoutKind.Explicit, Pack = 1)]
        private struct ENTRY
            {
            [FieldOffset(0)] public readonly Int32 GuidOffset;
            [FieldOffset(4)] public readonly Int32 DataOffset;
            [FieldOffset(8)] public readonly Int32 NextOffset;
            }

        public override unsafe IList<TypeLibraryCustomAttribute> this[Int32 offset] { get {
            if (IsDisposed) { throw new ObjectDisposedException(nameof(cache)); }
            if (offset < 0) { return EmptyList<TypeLibraryCustomAttribute>.Value; }
            if (!cache.TryGetValue(offset, out var r)) {
                r = new List<TypeLibraryCustomAttribute>();
                var i = offset;
                while (i >= 0) {
                    var e = (ENTRY*)(offsets + i);
                    r.Add(new TypeLibraryCustomAttribute(
                        g[e->GuidOffset].GetValueOrDefault(),
                        storage.Decode(Source, e->DataOffset)));
                    i = e->NextOffset;
                    }
                r = new ReadOnlyCollection<TypeLibraryCustomAttribute>(r);
                cache[offset] = r;
                }
            return r;
            }}

        protected override unsafe void Dispose(Boolean disposing) {
            offsets = null;
            g       = null;
            storage = null;
            base.Dispose(disposing);
            }
        }
    }