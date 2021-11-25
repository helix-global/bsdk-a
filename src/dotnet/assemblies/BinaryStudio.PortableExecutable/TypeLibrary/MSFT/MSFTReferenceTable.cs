using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using IMPLTYPEFLAGS = System.Runtime.InteropServices.ComTypes.IMPLTYPEFLAGS;

namespace BinaryStudio.PortableExecutable.TypeLibrary.MSFT
    {
    internal class MSFTReferenceTable : MSFTMetadataTableDescriptor<IList<ITypeLibraryTypeReference>>
        {
        private const Int32 MSFT_SEGMENT_INDEX_REF_TABLE = 3;

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        private struct ENTRY {
            public  readonly Int32  RefType;
            public  readonly IMPLTYPEFLAGS Flags;
            private readonly UInt32 CustomData;
            public  readonly Int32  Next;
            }

        private MSFTMetadataTypeLibrary storage;
        public unsafe MSFTReferenceTable(MetadataScope scope, Byte* source, MSFTMetadataTypeLibrary storage)
            : base(scope, source, MSFT_SEGMENT_INDEX_REF_TABLE)
            {
            this.storage = storage;
            }

        public override IList<ITypeLibraryTypeReference> this[Int32 i] { get {
            if (IsDisposed) { throw new ObjectDisposedException(nameof(cache)); }
            if (i < 0) { return null; }
            #if FEATURE_ADVANCE_NAME_TABLE_LOADING
            return cache[i];
            #else
            if (!cache.TryGetValue(i, out var r))
            unsafe
                {
                var ri = Source + Size;
                var source = Source;
                //var source = (Byte*)((ENTRY*)(Source + i) + 0);
                var offset  = i;
                r = new List<ITypeLibraryTypeReference>();
                Debug.Print($"Ref:Index:{i}");
                for (;;) {
                    var fi = source + offset;
                    if (fi > ri) { break; }
                    var e = (ENTRY*)(fi);
                    if (e->RefType % 4 == 0)
                        {
                        r.Add(new MSFTTypeReference(storage.D[e->RefType], e->Flags));
                        }
                    else
                        {
                        Debug.Print($"    TypeRef:{storage.I[e->RefType]}");
                        }
                    if (e->Next == -1) { break; }
                    offset = e->Next;
                    }
                cache[i] = r;
                }
            return r;
            #endif
            }}
        }
    }