using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace BinaryStudio.PortableExecutable.TypeLibrary.MSFT
    {
    internal class MSFTFileImportTable : MSFTMetadataTableDescriptor<ITypeLibraryDescriptor>, IEnumerable<ITypeLibraryDescriptor>
        {
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        private struct ENTRY {
            public  readonly Int32  GuidOffset;
            private readonly Int32  LCID;
            public  readonly Int16  MajorVersion;
            public  readonly Int16  MinorVersion;
            private readonly Int16  Size;
            }

        private const Int32 MSFT_SEGMENT_INDEX_IMPFILE_TABLE = 2;
        private readonly MSFTMetadataTypeLibrary storage;
        public unsafe MSFTFileImportTable(MetadataScope scope, Byte* source, MSFTMetadataTypeLibrary storage)
            : base(scope, source, MSFT_SEGMENT_INDEX_IMPFILE_TABLE)
            {
            this.storage = storage;
            }

        public override ITypeLibraryDescriptor this[Int32 i] { get {
            if (IsDisposed) { throw new ObjectDisposedException(nameof(cache)); }
            if (i < 0) { return null; }
            #if FEATURE_ADVANCE_NAME_TABLE_LOADING
            return cache[i];
            #else
            if (!cache.TryGetValue(i, out var r))
            unsafe
                {
                var e = (ENTRY*)(Source + i);
                var g = storage.G[e->GuidOffset].GetValueOrDefault();
                var version = new Version(e->MajorVersion, e->MinorVersion);
                var syskind = storage.TargetOperatingSystemPlatform;
                r = Scope.LoadTypeLibrary(g, version, syskind);
                cache[i] = r;
                }
            return r;
            #endif
            }}

        public IEnumerator<ITypeLibraryDescriptor> GetEnumerator()
            {
            return cache.Values.GetEnumerator();
            }

        IEnumerator IEnumerable.GetEnumerator()
            {
            return cache.Values.GetEnumerator();
            }
        }
    }