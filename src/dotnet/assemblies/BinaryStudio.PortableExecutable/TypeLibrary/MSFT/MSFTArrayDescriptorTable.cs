using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Microsoft.Win32;

namespace BinaryStudio.PortableExecutable.TypeLibrary
    {
    internal class MSFTArrayDescriptorTable : MSFTMetadataTableDescriptor<ITypeLibraryTypeDescriptor>
        {
        [StructLayout(LayoutKind.Explicit,Pack = 1, Size = 8)]
        private struct ENTRY
            {
            [FieldOffset( 0)] public readonly Int32 Type;
            [FieldOffset( 4)] public readonly Int16 Dimension;
            [FieldOffset( 6)] public readonly Int16 DataType;
            }

        private const Int32 MSFT_SEGMENT_INDEX_ARRAYDESC_TABLE = 10;
        public unsafe MSFTArrayDescriptorTable(MetadataScope scope, Byte* source)
            : base(scope, source, MSFT_SEGMENT_INDEX_ARRAYDESC_TABLE)
            {
            }

        public override ITypeLibraryTypeDescriptor this[Int32 i] { get {
            if (IsDisposed) { throw new ObjectDisposedException(nameof(cache)); }
            ITypeLibraryTypeDescriptor r = null;
            if (!cache.TryGetValue(i, out r))
            unsafe
                {
                var e = (ENTRY*)(Source + i);
                var c = e->Dimension;
                var source = (SAFEARRAYBOUND*)(e + 1);
                var bounds = new List<TypeLibraryFixedArrayBound>();
                for (var j = 0; j < c; ++j) {
                    bounds.Add(new TypeLibraryFixedArrayBound(
                        source->Elements,
                        source->LowerBound));
                    source++;
                    }
                if (e->Type < 0) {
                    var vt = (VARTYPE)(e->Type & 0xFFFF);
                    if (vt.HasFlag(VARTYPE.VT_BYREF)) {
                        vt &= ~VARTYPE.VT_BYREF;
                        cache.Add(i, r = new TypeLibraryFixedArray(
                            new TypeLibraryTypeReference(Scope.TypeOf(vt)),
                            bounds));
                        }
                    else
                        {
                        cache.Add(i, r = new TypeLibraryFixedArray(
                            Scope.TypeOf(vt),
                            bounds));
                        }
                    }
                else
                    {
                    throw new NotImplementedException();
                    }
                }
            return r;
            }}
        }
    }