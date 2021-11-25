using System;
using System.Linq;
using System.Runtime.InteropServices;

namespace BinaryStudio.PortableExecutable.TypeLibrary.MSFT
    {
    using TYPEKIND = System.Runtime.InteropServices.ComTypes.TYPEKIND;
    internal class MSFTTypeImportTable : MSFTMetadataTableDescriptor<ITypeLibraryTypeDescriptor>
        {
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        private struct ENTRY {
            public readonly Int16  Count;
            public readonly Byte   Flags;
            public readonly Byte   TypeKind;
            public readonly Int32  ImportFileOffset;
            public readonly Int32  GuidOrTypeInfoOffset;
            }

        private const Int32 MSFT_SEGMENT_INDEX_IMPINFO_TABLE = 1;
        private readonly MSFTMetadataTypeLibrary storage;

        public unsafe MSFTTypeImportTable(MetadataScope scope, Byte* source, MSFTMetadataTypeLibrary storage)
            : base(scope, source, MSFT_SEGMENT_INDEX_IMPINFO_TABLE)
            {
            this.storage = storage;
            }

        public override ITypeLibraryTypeDescriptor this[Int32 offset] { get {
            if (IsDisposed) { throw new ObjectDisposedException(nameof(cache)); }
            if (offset < 0) { return null; }
            #if FEATURE_ADVANCE_NAME_TABLE_LOADING
            return cache[i];
            #else
            if (!cache.TryGetValue(offset, out var r))
            unsafe
                {
                var e = (ENTRY*)(Source + offset - 1);
                var c  = e->Count;
                var kind = (TYPEKIND)e->TypeKind;
                var typelib = storage.F[e->ImportFileOffset];
                #region GUID
                if ((e->Flags & 0x01) == 0x01)
                    {
                    var g = storage.G[e->GuidOrTypeInfoOffset];
                    r = typelib.DefinedTypes.FirstOrDefault(i => i.UniqueIdentifier == g);
                    }
                #endregion
                else
                    {
                    r = typelib.DefinedTypes[e->GuidOrTypeInfoOffset];                    
                    }
                
                cache[offset] = r;
                }
            return r;
            #endif
            }}
        }
    }