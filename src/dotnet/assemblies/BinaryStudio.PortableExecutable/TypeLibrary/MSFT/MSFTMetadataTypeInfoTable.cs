using System;
using System.Collections;
using System.Collections.Generic;

namespace BinaryStudio.PortableExecutable.TypeLibrary
    {
    internal class MSFTMetadataTypeInfoTable : MSFTMetadataTableDescriptor<MSFTMetadataTypeDescriptor>, IEnumerable<MSFTMetadataTypeDescriptor>
        {
        public unsafe MSFTMetadataTypeInfoTable(MetadataScope scope, Byte* source, MSFTMetadataTypeLibrary storage)
            : base(scope, source, 0)
            {
            var header = (HEADER*)source;
            var offset = (UInt32*)(header + 1);
            var src = Source;
            for (var i = 0; i < header->TypeInfoCount; ++i) {
                cache.Add((Int32)(*offset), new MSFTMetadataTypeDescriptor(storage, source, (src + (Int64)(*offset))));
                offset++;
                }
            }

        public override MSFTMetadataTypeDescriptor this[Int32 i] { get {
            return cache[i];
            }}

        IEnumerator<MSFTMetadataTypeDescriptor> IEnumerable<MSFTMetadataTypeDescriptor>.GetEnumerator() { return cache.Values.GetEnumerator(); }
        IEnumerator IEnumerable.GetEnumerator() { return cache.Values.GetEnumerator(); }
        }
    }