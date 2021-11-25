using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using BinaryStudio.IO;
using BinaryStudio.PortableExecutable.TypeLibrary;
using BinaryStudio.PortableExecutable.TypeLibrary.MSFT;
using LIBFLAGS = System.Runtime.InteropServices.ComTypes.LIBFLAGS;
using SYSKIND = System.Runtime.InteropServices.ComTypes.SYSKIND;

namespace BinaryStudio.PortableExecutable
    {
    internal sealed class MSFTMetadataTypeLibrary : TypeLibraryDescriptor
        {
        private const UInt32 MSFT_SIGNATURE = 0x5446534D; /* "MSFT" */

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

        public override Version Version        { get { return storage.Version;          }}
        public override LIBFLAGS Flags { get; }
        public override String HelpFile { get; }
        public override Guid UniqueIdentifier { get { return storage.UniqueIdentifier; }}
        public override Int32 HelpContext { get; }
        //public override Boolean IsLoading      { get { return storage.IsLoading;        }}
        public override CultureInfo Culture    { get { return storage.Culture;          }}
        public override String HelpString      { get { return storage.HelpString;       }}
        public override String Name            { get { return storage.Name;             }}
        public override IList<ITypeLibraryTypeDescriptor>  DefinedTypes     { get { return storage.DefinedTypes; }}
        public override IList<TypeLibraryCustomAttribute> CustomAttributes { get { return storage.CustomAttributes; }}
        public override IList<ITypeLibraryDescriptor> ImportedLibraries { get { return storage.ImportedLibraries; }}
        public override SYSKIND TargetOperatingSystemPlatform { get { return storage.TargetOperatingSystemPlatform; }}

        private readonly Storage storage = new Storage();
        private class Storage
            {
            public Boolean IsLoading { get;set; }
            public CultureInfo Culture { get;set; }
            public SYSKIND TargetOperatingSystemPlatform { get;set; }
            public Guid UniqueIdentifier { get;set; }
            public String Name { get;set; }
            public String HelpString { get;set; }
            public Version Version { get;set; }
            public IList<IComponent> Components { get;set; }
            public IList<TypeLibraryCustomAttribute> CustomAttributes { get;set; }
            public IList<ITypeLibraryTypeDescriptor>  DefinedTypes { get;set; }
            public IList<ITypeLibraryDescriptor> ImportedLibraries { get;set; }
            }

        public MSFTMetadataTypeLibrary(MetadataScope scope, Encoding encoding)
            : base(scope, encoding)
            {
            storage.Components = new List<IComponent>();
            storage.CustomAttributes  = EmptyList<TypeLibraryCustomAttribute>.Value;
            storage.DefinedTypes      = EmptyList<ITypeLibraryTypeDescriptor>.Value;
            storage.ImportedLibraries = EmptyList<ITypeLibraryDescriptor>.Value;
            }

        public unsafe Boolean IsSupported(Byte* source) {
            if (source == null) { throw new ArgumentNullException(nameof(source)); }
            return (*(UInt32*)source == MSFT_SIGNATURE);
            }

        #region M:Load(Byte*,Int64)
        protected internal override unsafe Boolean Load(out Exception e, Byte* source, Int64 size) {
            if (source == null) { throw new ArgumentNullException(nameof(source)); }
            e = null;
            try
                {
                storage.IsLoading = true;
                if (*(UInt32*) source != MSFT_SIGNATURE) { return false; }
                using (var task = Task.Factory.StartNew(()=> Load((MSFT_HEADER*)source)))
                    {
                    task.Wait();
                    }
                }
            finally
                {
                storage.IsLoading = false;
                }
            return true;
            }
        #endregion
        #region M:Load(MSFT_HEADER*,Encoding)
        private unsafe void Load(MSFT_HEADER* header) {
            if (header == null) { throw new ArgumentNullException(nameof(header)); }
            storage.Culture = (header->LCID != 0) ? new CultureInfo(header->LCID) : null;
            storage.TargetOperatingSystemPlatform = (SYSKIND)(header->Flags & 0xF);
            #if !FEATURE_ADVANCE_NAME_TABLE_LOADING
            N = new MSFTMetadataNameTable(Scope, (Byte*)header, Encoding);
            #endif
            #if !FEATURE_ADVANCE_GUID_TABLE_LOADING
            G = new MSFTMetadataGuidTable(Scope, (Byte*)header);
            #endif
            #if !FEATURE_ADVANCE_STRING_TABLE_LOADING
            S = new MSFTMetadataStringTable(Scope, (Byte*)header, Encoding);
            #endif
            F = new MSFTFileImportTable(Scope, (Byte*)header, this);
            I = new MSFTTypeImportTable(Scope, (Byte*)header, this);
            R = new MSFTReferenceTable(Scope,  (Byte*)header, this);
            BeginInvoke(
                #if FEATURE_ADVANCE_NAME_TABLE_LOADING
                ()=>{ N = new MSFTMetadataNameTable(Scope, (Byte*)header, Encoding); },
                #endif
                #if FEATURE_ADVANCE_GUID_TABLE_LOADING
                ()=>{ G = new MSFTMetadataGuidTable(Scope, (Byte*)header); },
                #endif
                #if FEATURE_ADVANCE_STRING_TABLE_LOADING
                ()=>{ S = new MSFTMetadataStringTable(Scope, (Byte*)header, Encoding); },
                #endif
                null
                );
            storage.Name = N[header->NameIndex];
            storage.UniqueIdentifier = G[header->GuidIndex].GetValueOrDefault();
            storage.Version = new Version(header->MajorVersion, header->MinorVersion);
            C = new MSFTMetadataCustomAttributeTable(Scope, this, (Byte*)header, G);
            L = new MSFTTypeDescriptorTable(Scope, this, (Byte*)header);
            A = new MSFTArrayDescriptorTable(Scope, (Byte*)header);
            BeginInvoke(
                #region loading typeinfo table
                ()=>{ D = new MSFTMetadataTypeInfoTable(Scope, (Byte*)header, this); }
                #endregion
                );
            BeginInvoke(
                #region updating [DefinedTypes]
                ()=>{ storage.DefinedTypes = new ReadOnlyCollection<ITypeLibraryTypeDescriptor>(D.OfType<ITypeLibraryTypeDescriptor>().ToArray()); },
                #endregion
                #region loading type body
                ()=>
                    {
                    foreach (var i in D) { ((MSFTMetadataTypeDescriptor)i).Load(this); }
                    }
                #endregion
                );
            storage.CustomAttributes = C[header->CustomDataOffset];
            storage.HelpString       = S[header->HelpStringIndex];
            storage.ImportedLibraries = new ReadOnlyCollection<ITypeLibraryDescriptor>(F.ToArray());
            }
        #endregion
        //#region M:Dispose(Boolean)
        //protected override void Dispose(Boolean disposing) {
        //    if (storage != null) {
        //        if (storage.Components != null) {
        //            foreach (var component in storage.Components) { component.Dispose(); }
        //            storage.Components.Clear();
        //            storage.Components = null;
        //            }
        //        storage.Name = null;
        //        storage.Culture = null;
        //        storage.HelpString = null;
        //        storage.Version = null;
        //        storage.CustomAttributes = null;
        //        }
        //    N = null;
        //    C = null;
        //    S = null;
        //    G = null;
        //    D = null;
        //    base.Dispose(disposing);
        //    }
        //#endregion
        #region M:Decode(Byte*):Object
        public unsafe Object Decode(Byte* source, Int32 offset) {
            if (source == null) { throw new ArgumentNullException(nameof(source)); }
            if (offset < 0) {
                var var = new VARIANT
                    {
                    Type = (VARTYPE) ((offset & 0x7c000000) >> 26),
                    Int32 = (offset & 0x3ffffff)
                    };
                switch (var.Type)
                    {
                    case VARTYPE.VT_I1:       { return var.SByte;   }
                    case VARTYPE.VT_I2:       { return var.Int16;   }
                    case VARTYPE.VT_I4:       { return var.Int32;   }
                    case VARTYPE.VT_I8:       { return var.Int64;   }
                    case VARTYPE.VT_UI1:      { return var.Byte;    }
                    case VARTYPE.VT_UI2:      { return var.UInt16;  }
                    case VARTYPE.VT_UI4:      { return var.UInt32;  }
                    case VARTYPE.VT_UI8:      { return var.UInt64;  }
                    case VARTYPE.VT_R4:       { return var.Single;  }
                    case VARTYPE.VT_R8:       { return var.Double;  }
                    case VARTYPE.VT_INT:      { return var.Int32;   }
                    case VARTYPE.VT_UINT:     { return var.UInt32;  }
                    case VARTYPE.VT_INT_PTR:  { return var.IntPtr;  }
                    case VARTYPE.VT_UINT_PTR: { return var.UIntPtr; }
                    default: { throw new ArgumentOutOfRangeException(nameof(offset)); }
                    }
                }

            source += offset;
            var type = *(VARTYPE*)source;
            switch (type) {
                case VARTYPE.VT_I1:  { return  *(SByte*)(source + sizeof(VARTYPE)); }
                case VARTYPE.VT_I2:  { return  *(Int16*)(source + sizeof(VARTYPE)); }
                case VARTYPE.VT_I4:  { return  *(Int32*)(source + sizeof(VARTYPE)); }
                case VARTYPE.VT_I8:  { return  *(Int64*)(source + sizeof(VARTYPE)); }
                case VARTYPE.VT_UI1: { return   *(Byte*)(source + sizeof(VARTYPE)); }
                case VARTYPE.VT_UI2: { return *(UInt16*)(source + sizeof(VARTYPE)); }
                case VARTYPE.VT_UI4: { return *(UInt32*)(source + sizeof(VARTYPE)); }
                case VARTYPE.VT_UI8: { return *(UInt64*)(source + sizeof(VARTYPE)); }
                case VARTYPE.VT_BSTR:
                    {
                    var body = new Byte[*(Int32*)(source + sizeof(VARTYPE))];
                    var r = source + sizeof(VARTYPE) + sizeof(Int32);
                    for (var i = 0; i < body.Length; i++) {
                        body[i] = r[i];
                        }
                    return Encoding.UTF8.GetString(body);
                    }
                }
            return null;
            }
        #endregion

        public ITypeLibraryTypeDescriptor TypeOf(Int32 typeref) {
            if (typeref < 0) {
                return Scope.TypeOf((VARTYPE)((typeref & 0x7FFFFFFF) & 0xFFFF));
                }
            else
                {
                return L[typeref >> 3];
                }
            }

        /*[DebuggerBrowsable(DebuggerBrowsableState.Never)]*/ public MSFTMetadataNameTable            N { get;private set; }
        /*[DebuggerBrowsable(DebuggerBrowsableState.Never)]*/ public MSFTMetadataGuidTable            G { get;private set; }
        /*[DebuggerBrowsable(DebuggerBrowsableState.Never)]*/ public MSFTMetadataStringTable          S { get;private set; }
        /*[DebuggerBrowsable(DebuggerBrowsableState.Never)]*/ public MSFTMetadataCustomAttributeTable C { get;private set; }
        /*[DebuggerBrowsable(DebuggerBrowsableState.Never)]*/ public MSFTMetadataTypeInfoTable        D { get;private set; }
        /*[DebuggerBrowsable(DebuggerBrowsableState.Never)]*/ public MSFTTypeDescriptorTable          L { get;private set; }
        /*[DebuggerBrowsable(DebuggerBrowsableState.Never)]*/ public MSFTArrayDescriptorTable         A { get;private set; }
        /*[DebuggerBrowsable(DebuggerBrowsableState.Never)]*/ public MSFTTypeImportTable              I { get;private set; }
        /*[DebuggerBrowsable(DebuggerBrowsableState.Never)]*/ public MSFTFileImportTable              F { get;private set; }
        /*[DebuggerBrowsable(DebuggerBrowsableState.Never)]*/ public MSFTReferenceTable               R { get;private set; }

        }
    }