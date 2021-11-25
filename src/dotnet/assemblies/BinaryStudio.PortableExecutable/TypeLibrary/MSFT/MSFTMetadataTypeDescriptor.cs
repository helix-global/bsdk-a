using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;

namespace BinaryStudio.PortableExecutable.TypeLibrary
    {
    using TYPEKIND = System.Runtime.InteropServices.ComTypes.TYPEKIND;
    internal class MSFTMetadataTypeDescriptor : TypeLibraryTypeDescriptor
        {
        [StructLayout(LayoutKind.Explicit, Pack = 1, Size = 100)]
        private struct MSFT_TYPEINFO
            {
            [FieldOffset( 0)] public  readonly UInt32 TypeKind;             /*  it is the TKIND_xxx */
            [FieldOffset( 4)] public  readonly UInt32 FunctionRecordOffset; /* points past the file, if no elements */
            [FieldOffset( 8)] private readonly UInt32 res2;                 /* zero if no element, N*0x40 */
            [FieldOffset(12)] private readonly UInt32 res3;                 /* -1 if no element, (N-1)*0x38 */
            [FieldOffset(16)] private readonly UInt32 res4;                 /* always? 3 */
            [FieldOffset(20)] private readonly UInt32 res5;                 /* always? zero */
            [FieldOffset(24)] public  readonly UInt16 FunctionCount;
            [FieldOffset(26)] public  readonly UInt16 PropertyCount;
            [FieldOffset(44)] public  readonly  Int32 GuidTableIndex;       /* position in guid table */
            [FieldOffset(48)] public  readonly UInt32 Flags;                /* TypeFlags */
            [FieldOffset(52)] public  readonly  Int32 NameTableOffset;      /* offset in name table */
            [FieldOffset(56)] public  readonly UInt32 Version;              /* element version */
            [FieldOffset(60)] public  readonly  Int32 HelpStringIndex;      /* offset of docstring in string tab */
            [FieldOffset(64)] private readonly UInt32 helpstringcontext;    /*  */
            [FieldOffset(68)] private readonly UInt32 helpcontext;          /* */
            [FieldOffset(72)] public  readonly  Int32 CustomDataOffset;     /* offset in customer data table */
            [FieldOffset(76)] public  readonly  Int16 ImplementedTypes;     /* nr of implemented interfaces */
            [FieldOffset(78)] private readonly UInt16 cbSizeVft;            /* virtual table size, not including inherits */
            [FieldOffset(80)] private readonly UInt32 size;                 /* size in bytes, at least for structures */
            /* FIXME: name of this field */
            [FieldOffset(84)] public  readonly  Int32 DataType1;            /* position in type description table */
                                                                            /* or in base interfaces */
                                                                            /* if coclass: offset in reftable */
                                                                            /* if interface: reference to inherited if */
                                                                            /* if module: offset to dllname in name table */
            [FieldOffset(88)] private readonly UInt32 DataType2;       /* if 0x8000, entry above is valid */
            /* actually dunno */
            /* else it is zero? */
            /* if interface: inheritance level | no of inherited funcs */
            [FieldOffset(92)] private readonly Int32      res18;            /* always? 0 */
            [FieldOffset(96)] private readonly Int32      res19;            /* always? -1 */
            }

        [StructLayout(LayoutKind.Explicit, Pack = 1)]
        private struct MSFT_HEADER
            {
            [FieldOffset( 0)] [DebuggerBrowsable(DebuggerBrowsableState.Never)] private readonly UInt32   Signature;          /* 0x5446534D "MSFT" */
            [FieldOffset( 8)] [DebuggerBrowsable(DebuggerBrowsableState.Never)] public  readonly Int32    GuidIndex;         /* position of libid in guid table (should be,  else -1) */
            [FieldOffset(12)] [DebuggerBrowsable(DebuggerBrowsableState.Never)] public  readonly Int32    LCID;              /* locale id */
            [FieldOffset(20)] public  readonly UInt32   Flags;          /* (largely) unknown flags ,seems to be always 41 */
                                                         /* becomes 0x51 with a helpfile defined */
                                                         /* if help dll defined it's 0x151 */
                                                         /* update : the lower nibble is syskind */
            [FieldOffset(24)] [DebuggerBrowsable(DebuggerBrowsableState.Never)] public  readonly UInt16   MajorVersion;
            [FieldOffset(26)] [DebuggerBrowsable(DebuggerBrowsableState.Never)] public  readonly UInt16   MinorVersion;
            [FieldOffset(28)] private readonly UInt32   flags;             /* set with SetFlags() */
            [FieldOffset(32)] [DebuggerBrowsable(DebuggerBrowsableState.Never)] public  readonly UInt32   TypeInfoCount;     /* number of typeinfo's (till so far) */
            [FieldOffset(36)] [DebuggerBrowsable(DebuggerBrowsableState.Never)] public  readonly  Int32   HelpStringIndex;   /* position of help string in stringtable */
            [FieldOffset(40)] private readonly UInt32   helpstringcontext;
            [FieldOffset(44)] private readonly UInt32   helpcontext;
            [FieldOffset(48)] [DebuggerBrowsable(DebuggerBrowsableState.Never)] public  readonly UInt32   NameTableCount;    /* number of names in name table */
            [FieldOffset(52)] [DebuggerBrowsable(DebuggerBrowsableState.Never)] public  readonly UInt32   NameTableChars;    /* nr of characters in name table */
            [FieldOffset(56)] [DebuggerBrowsable(DebuggerBrowsableState.Never)] public  readonly  Int32   NameIndex;
            [FieldOffset(60)] private readonly UInt32   helpfile;          /* position of helpfile in stringtable */
            [FieldOffset(64)] [DebuggerBrowsable(DebuggerBrowsableState.Never)] public  readonly  Int32   CustomDataOffset;  /* if -1 no custom data, else it is offset in customer data/guid offset table */
            [FieldOffset(68)] private readonly UInt32   res44;             /* unknown always: 0x20 (guid hash size?) */
            [FieldOffset(72)] private readonly UInt32   res48;             /* unknown always: 0x80 (name hash size?) */
            [FieldOffset(76)] private readonly UInt32   dispatchpos;       /* HREFTYPE to IDispatch, or -1 if no IDispatch */
            [FieldOffset(80)] private readonly UInt32   nimpinfos;         /* number of impinfos */
            }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)] private unsafe Byte* source;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)] private unsafe MSFT_HEADER* header;
        public unsafe MSFTMetadataTypeDescriptor(MSFTMetadataTypeLibrary storage, Byte* header, Byte* source)
            :base(null)
            {
            if (storage == null) { throw new ArgumentNullException(nameof(storage)); }
            if (source == null)  { throw new ArgumentNullException(nameof(source));  }
            Library = storage;
            this.source = source;
            this.header = (MSFT_HEADER*)header;
            var typeinfo = (MSFT_TYPEINFO*)source;
            var attributes = (TYPEKIND)(typeinfo->TypeKind & 0xF);
            Flags = (TypeLibTypeFlags)(typeinfo->Flags & 0xFFFF);
            Name = storage.N[typeinfo->NameTableOffset];
            IsAlias     = attributes == TYPEKIND.TKIND_ALIAS;
            IsClass     = attributes == TYPEKIND.TKIND_COCLASS;
            IsDispatch  = attributes == TYPEKIND.TKIND_DISPATCH;
            IsEnum      = attributes == TYPEKIND.TKIND_ENUM;
            IsModule    = attributes == TYPEKIND.TKIND_MODULE;
            IsInterface = attributes == TYPEKIND.TKIND_INTERFACE;
            IsStructure = attributes == TYPEKIND.TKIND_RECORD;
            IsUnion     = attributes == TYPEKIND.TKIND_UNION;
            if (typeinfo->Version != 0) {
                Version = new Version(
                    (UInt16)(typeinfo->Version & 0xFF),
                    (UInt16)(typeinfo->Version >> 8));
                }
            UniqueIdentifier = storage.G[typeinfo->GuidTableIndex];
            storage.Scope.RegisterType(this);
            }

        private readonly Storage storage = new Storage();
        private class Storage
            {
            public String HelpString;
            public IList<ITypeLibraryMethodDescriptor> DeclaredMethods = EmptyList<ITypeLibraryMethodDescriptor>.Value;
            public IList<ITypeLibraryFieldDescriptor> DeclaredFields   = EmptyList<ITypeLibraryFieldDescriptor>.Value;
            public IList<ITypeLibraryPropertyDescriptor> DeclaredProperties = EmptyList<ITypeLibraryPropertyDescriptor>.Value;
            public IList<TypeLibraryCustomAttribute> CustomAttributes = EmptyList<TypeLibraryCustomAttribute>.Value;
            public ITypeLibraryTypeDescriptor BaseType;
            public ITypeLibraryTypeDescriptor UnderlyingType = null;
            public IList<ITypeLibraryTypeReference> ImplementedInterfaces = EmptyList<ITypeLibraryTypeReference>.Value;
            }

        public unsafe void Load(MSFTMetadataTypeLibrary library)
            {
            var typeinfo = (MSFT_TYPEINFO*)source;
            storage.HelpString = library.S[typeinfo->HelpStringIndex];
            storage.CustomAttributes = library.C[typeinfo->CustomDataOffset];
                 if (IsAlias)     { storage.UnderlyingType = library.TypeOf(typeinfo->DataType1); }
            else if (IsInterface) {
                if (typeinfo->DataType1 >= 0) {
                    if ((typeinfo->DataType1 % 100) == 0) {
                        storage.BaseType = library.D[typeinfo->DataType1];
                        }
                    }
                if (storage.BaseType == null) {
                    if (UniqueIdentifier != new Guid("00000000-0000-0000-c000-000000000046")) {
                        storage.BaseType = library.Scope.TypeOf("IUnknown, stdole, Version=1.0, Culture=en-US, UniqueIdentifier=00020430-0000-0000-c000-000000000046");
                        }
                    }
                }
            else if (IsDispatch)
                {
                if (typeinfo->DataType1 >= 0) {
                    if ((typeinfo->DataType1 % 100) == 0) {
                        storage.BaseType = library.D[typeinfo->DataType1];
                        }
                    }
                storage.BaseType = storage.BaseType ?? library.Scope.TypeOf("IDispatch, stdole, UniqueIdentifier=00020430-0000-0000-c000-000000000046");
                }
            if (typeinfo->ImplementedTypes > 0) {
                if (IsClass) {
                    storage.ImplementedInterfaces = new List<ITypeLibraryTypeReference>(library.R[typeinfo->DataType1]);
                    }
                }
            if ((typeinfo->FunctionCount > 0) || (typeinfo->PropertyCount > 0)) {
                var r = ((Byte*)header) + (Int64)(typeinfo->FunctionRecordOffset);
                r += sizeof(UInt32);
                var identifiers = new Int32[typeinfo->FunctionCount + typeinfo->PropertyCount];
                var nameoffsets = new Int32[typeinfo->FunctionCount + typeinfo->PropertyCount];
                var rcrdoffsets = new Int32[typeinfo->FunctionCount + typeinfo->PropertyCount];
                var fbody = new Byte*[typeinfo->PropertyCount];
                var mbody = new Byte*[typeinfo->FunctionCount];
                var mdesc = new List<ITypeLibraryMethodDescriptor>();
                var fdesc = new List<MSFTFieldDescriptor>();
                var pdesc = new List<ITypeLibraryPropertyDescriptor>();
                for (var j = 0; j < typeinfo->FunctionCount; j++) {
                    var mi = (UInt32*)r;
                    mbody[j] = (Byte*)r;
                    r += (Int32)(*mi & 0xFFFF);
                    }
                for (var j = 0; j < typeinfo->PropertyCount; j++) {
                    fbody[j] = (Byte*)r;
                    r += *(UInt16*)r;
                    }
                for (var j = 0; j < identifiers.Length; ++j) {
                    identifiers[j] = *((Int32*)(r));
                    nameoffsets[j] = *((Int32*)(r + 1*identifiers.Length*sizeof(Int32)));
                    rcrdoffsets[j] = *((Int32*)(r + 2*identifiers.Length*sizeof(Int32)));
                    if (j < typeinfo->FunctionCount)
                        {
                        mdesc.Add(new MSFTMethodDescriptor(library, this, mbody[j],nameoffsets[j], identifiers[j]));
                        }
                    else
                        {
                        var i = j - typeinfo->FunctionCount;
                        fdesc.Add(new MSFTFieldDescriptor(library, this, fbody[i],nameoffsets[j], identifiers[j]));
                        }
                    r += sizeof(UInt32);
                    }

                if (IsDispatch) {
                    pdesc.AddRange(fdesc.OfType<ITypeLibraryFieldDescriptor>().Select(i => new MSFTFieldBasedPropertyDescriptor(this, i)));
                    fdesc.Clear();
                    }

                if (mdesc.Count > 0) {
                    foreach (var g in mdesc.GroupBy(i => i.Id).ToArray()) {
                        var items = g.ToArray();
                        if ((items.Length == 1) || (items.Length == 2)) {
                            if (items.Length == 1) {
                                /* [propget] or [propput(ref)] */
                                #region [propget]
                                if (items[0].Attributes.HasFlag(TypeLibraryMethodAttributes.INVOKE_PROPERTYGET)) {
                                    if (items[0].Parameters[items[0].Parameters.Count - 1].IsRetval) {
                                        pdesc.Add(new MSFTMethodBasedPropertyDescriptor(this, items[0], null));
                                        mdesc.Remove(items[0]);
                                        }
                                    }
                                #endregion
                                #region [propput]
                                else if (items[0].Attributes.HasFlag(TypeLibraryMethodAttributes.INVOKE_PROPERTYPUT) || items[0].Attributes.HasFlag(TypeLibraryMethodAttributes.INVOKE_PROPERTYPUTREF)) {
                                    pdesc.Add(new MSFTMethodBasedPropertyDescriptor(this, null, items[0]));
                                    mdesc.Remove(items[0]);
                                    }
                                #endregion
                                }
                            else
                                {
                                if (items[0].Parameters.Count == items[1].Parameters.Count) {
                                    var x = items[0].Attributes;
                                    var y = items[1].Attributes;
                                    #region [propget][propput]
                                    if (x.HasFlag(TypeLibraryMethodAttributes.INVOKE_PROPERTYGET) && (y.HasFlag(TypeLibraryMethodAttributes.INVOKE_PROPERTYPUT) || y.HasFlag(TypeLibraryMethodAttributes.INVOKE_PROPERTYPUTREF))) {
                                        if (items[0].Parameters[items[0].Parameters.Count - 1].IsRetval) {
                                            pdesc.Add(new MSFTMethodBasedPropertyDescriptor(this, items[0], items[1]));
                                            mdesc.Remove(items[0]);
                                            mdesc.Remove(items[1]);
                                            }
                                        }
                                    #endregion
                                    #region [propput][propget]
                                    else if ((x.HasFlag(TypeLibraryMethodAttributes.INVOKE_PROPERTYPUT) || x.HasFlag(TypeLibraryMethodAttributes.INVOKE_PROPERTYPUTREF)) && y.HasFlag(TypeLibraryMethodAttributes.INVOKE_PROPERTYGET)) {
                                        if (items[1].Parameters[items[1].Parameters.Count - 1].IsRetval) {
                                            pdesc.Add(new MSFTMethodBasedPropertyDescriptor(this, items[1], items[0]));
                                            mdesc.Remove(items[0]);
                                            mdesc.Remove(items[1]);
                                            }
                                        }
                                    #endregion
                                    }
                                }
                            }
                        }
                    }

                storage.DeclaredMethods     = new ReadOnlyCollection<ITypeLibraryMethodDescriptor>(mdesc.ToArray());
                storage.DeclaredFields      = new ReadOnlyCollection<ITypeLibraryFieldDescriptor>(fdesc.OfType<ITypeLibraryFieldDescriptor>().ToArray());
                storage.DeclaredProperties  = new ReadOnlyCollection<ITypeLibraryPropertyDescriptor>(pdesc.ToArray());
                }
            }

        public override String Name { get; }
        public override String HelpString { get { return storage.HelpString; }}
        public override Boolean IsInterface { get; }
        public override Boolean IsClass { get; }
        public override Boolean IsUnion { get; }
        public override Boolean IsStructure { get; }
        public override Boolean IsModule { get; }
        public override Boolean IsEnum { get; }
        public override Boolean IsAlias { get; }
        public override Boolean IsPointer   { get { return false; }}
        public override Boolean IsPrimitive { get { return false; }}
        public override Boolean IsDispatch { get; }
        public override Boolean IsArray { get { return false; }}
        public override TypeLibTypeFlags Flags { get; }
        public override Version Version { get; }
        public override Guid? UniqueIdentifier { get; }
        public override IList<ITypeLibraryMethodDescriptor>   DeclaredMethods       { get { return storage.DeclaredMethods;       }}
        public override IList<ITypeLibraryFieldDescriptor>    DeclaredFields        { get { return storage.DeclaredFields;        }}
        public override IList<ITypeLibraryPropertyDescriptor> DeclaredProperties    { get { return storage.DeclaredProperties;    }}
        public override IList<ITypeLibraryTypeReference>      ImplementedInterfaces { get { return storage.ImplementedInterfaces; }}
        public override ITypeLibraryTypeDescriptor UnderlyingType { get { return storage.UnderlyingType; }}
        public override ITypeLibraryTypeDescriptor BaseType       { get { return storage.BaseType;       }}
        public override ITypeLibraryDescriptor Library { get; }
        public override IList<TypeLibraryCustomAttribute> CustomAttributes { get { return storage.CustomAttributes; }}
        }
    }