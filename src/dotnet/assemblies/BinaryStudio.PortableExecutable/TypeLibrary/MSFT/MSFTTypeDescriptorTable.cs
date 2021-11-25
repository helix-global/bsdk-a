using System;

namespace BinaryStudio.PortableExecutable.TypeLibrary
    {
    internal class MSFTTypeDescriptorTable : MSFTMetadataTableDescriptor<ITypeLibraryTypeDescriptor>
        {
        private const Int32 MSFT_SEGMENT_INDEX_TYPEDESC_TABLE = 9;
        private readonly MSFTMetadataTypeLibrary storage;
        public unsafe MSFTTypeDescriptorTable(MetadataScope scope, MSFTMetadataTypeLibrary storage, Byte* source)
            : base(scope, source, MSFT_SEGMENT_INDEX_TYPEDESC_TABLE)
            {
            this.storage = storage;
            }

        public override ITypeLibraryTypeDescriptor this[Int32 i] { get {
            if (IsDisposed) { throw new ObjectDisposedException(nameof(cache)); }
            if (!cache.TryGetValue(i, out var r))
            unsafe
                {
                var source = ((UInt16*)Source) + i*4;
                var src = new UInt16[4];
                var vt  = new VARTYPE[4];
                src[0] = source[0]; /* Value1 */
                src[1] = source[1]; /* Value2 */
                src[2] = source[2]; /* Value3 */
                src[3] = source[3]; /* Value4 */
                vt[0]  = (VARTYPE)source[0];
                vt[1]  = (VARTYPE)source[1];
                vt[2]  = (VARTYPE)source[2];
                vt[3]  = (VARTYPE)source[3];
                var flag1 = (src[1] & 0x7FFE) == 0x7FFE;
                switch (vt[0]) {
                    #region VT_PTR
                    case VARTYPE.VT_PTR:
                        {
                        r = ((source[3] & 0x8000) == 0x8000)
                            ? new TypeLibraryTypePointer(Scope.TypeOf((VARTYPE)((UInt32)source[2] & 0x4FFF)))
                            : new TypeLibraryTypePointer(this[((Int32)source[2] >> 3)]);

                        //if (source[3] < 0)
                        //    {
                        //    r = Scope.PointerOf((VARTYPE)((Int16)source[2] & 0x4FFF));
                        //    }
                        //else
                        //    {
                        //    r = this[source[2] >> 3];
                        //    //if (flag1)
                        //    //    {
                        //    //    r = this[i + 1];
                        //    //    }
                        //    //else
                        //    //    {
                        //    //    var hreftype = (((UInt32)source[3]) << 16) | ((UInt32)source[2]);
                        //    //    if ((hreftype % 100) == 0)
                        //    //        {
                        //    //        r = storage.D[(Int32)hreftype];
                        //    //        }
                        //    //    }
                        //    }
                        //r = new TypeLibraryTypePointer(r);
                        }
                        break;
                    #endregion
                    #region VT_USERDEFINED
                    case VARTYPE.VT_USERDEFINED:
                        {
                        var hreftype = (((UInt32)source[3]) << 16) | ((UInt32)source[2]);
                        if ((hreftype % 100) == 0) {
                            r = storage.D[(Int32)hreftype];
                            }
                        else
                            {
                            r = storage.I[(Int32)hreftype];
                            }
                        }
                        break;
                    #endregion
                    #region VT_SAFEARRAY
                    case VARTYPE.VT_SAFEARRAY:
                        {
                        r = ((source[3] & 0x8000) == 0x8000)
                            ? new TypeLibrarySafeArrayType(Scope.TypeOf((VARTYPE)((UInt32)source[2] & 0x4FFF)))
                            : new TypeLibrarySafeArrayType(this[((Int32)source[2] >> 3)]);
                        }
                        break;
                    #endregion
                    #region VT_CARRAY
                    case VARTYPE.VT_CARRAY:
                        {
                        r = storage.A[(Int32)((((UInt32)source[3]) << 16) | ((UInt32)source[2]))];
                        }
                        break;
                    #endregion
                    }
                if (r != null) {
                    cache.Add(i, r);
                    }
                }
            return r;
            }}
        }
    }