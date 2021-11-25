using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32;

namespace BinaryStudio.PortableExecutable.TypeLibrary
    {
    using LIBFLAGS = System.Runtime.InteropServices.ComTypes.LIBFLAGS;
    using TYPEKIND = System.Runtime.InteropServices.ComTypes.TYPEKIND;
    using SYSKIND = System.Runtime.InteropServices.ComTypes.SYSKIND;
    using UShort = UInt16;
    using Short = Int16;
    using ULong = UInt32;
    using Long = Int32;
    using LCID = UInt32;
    internal class SLTGTypeLibrary : TypeLibraryDescriptor
        {
        private const UInt32 SLTG_SIGNATURE  = 0x47544c53; /* "SLTG" */
        private const UInt16 SLTG_FIRST_WORD = 'G' * 256 + 'T' * 32 + 'L';
        private const UInt16 SLTG_DEFAULT_VERSION = 3;
        private const UInt16 SLTG_DUAL_VERSION    = 4;
        private const UInt16 SLTG_MAX_VERSION     = 4;
        private const Int32 GTLIBOLE_BUCKETS = 32;

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        private struct SLTG_HEADER
            {
            private readonly UInt32 Signature;          /* The signature at the beginning of the file. 0x47544c53  == "SLTG" */
            public  readonly Int16  NumberOfStreams;    /* The number of streams. */
            public  readonly Int16  ExtraSize;          /* The number of bytes between the name table and the first stream.  This is generally a small multiple of 8. */
            public  readonly Int16  NameTableSize;      /* The number of bytes in the name table. */
            public  readonly Int16  FirstStream;        /* The first SerStreamInfo in the linked list which is sorted by order of the actual stream data in the file. */
            private readonly Guid   Guid;
            }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        private struct SLTG_STREAM
            {
            public readonly Int32 Length;             /* Length of the stream. */
            public readonly Int16 NameOffset;         /* Offset of the name in the name table. */
            public readonly Int16 NextStream;         /* Index into this array of the next stream. This linked list specifies the order of the actual stream data in the file. */
            }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        private struct SLTG_DIR_CHNUK1
            {
            public readonly UInt16 FirstWord;
            public readonly UInt16 Version;
            public readonly UInt16 usHlnam;
            }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        private struct SLTG_DIR_CHNUK2
            {
            public  readonly Int32 HelpContext;
            public  readonly Int16 TargetOperatingSystemPlatform;
            public  readonly Int32 LCID;
            private readonly Int16 usTmp;
            public  readonly LIBFLAGS Flags;
            public  readonly Int16 MajorVersion;
            public  readonly Int16 MinorVersion;
            public  readonly Guid UniqueIdentifier;
            private unsafe fixed Int16 Buckets[GTLIBOLE_BUCKETS];
            public  readonly Int16 TypeEntries;
            }

        public SLTGTypeLibrary(MetadataScope scope,Encoding encoding)
            : base(scope, encoding)
            {
            }

        public override Version Version { get { return storage.Version; }}
        public override LIBFLAGS Flags  { get { return storage.Flags;   }}
        public override IList<TypeLibraryCustomAttribute> CustomAttributes { get; }
        public override IList<ITypeLibraryTypeDescriptor> DefinedTypes { get{ return storage.DefinedTypes; }}
        public override CultureInfo Culture { get { return storage.Culture; }}
        public override String Name       { get { return storage.Name;       }}
        public override String HelpString { get { return storage.HelpString; }}
        public override String HelpFile   { get { return storage.HelpFile;   }}
        public override SYSKIND TargetOperatingSystemPlatform { get { return storage.TargetOperatingSystemPlatform; }}
        public override Guid UniqueIdentifier { get { return storage.UniqueIdentifier; }}
        public override Int32 HelpContext { get { return storage.HelpContext; }}

        private static readonly String SLTG_DIR_NAME = "dir";

        private readonly Storage storage = new Storage();
        private class Storage
            {
            public Boolean IsLoading { get;set; }
            public CultureInfo Culture;
            public SYSKIND TargetOperatingSystemPlatform;
            public Guid UniqueIdentifier;
            public String Name { get;set; }
            public String HelpString;
            public String HelpFile;
            public Version Version;
            public IList<TypeLibraryCustomAttribute> CustomAttributes { get;set; }
            public IList<ITypeLibraryTypeDescriptor>  DefinedTypes { get;set; }
            public Int32 HelpContext = -1;
            public LIBFLAGS Flags;
            }

        //unsafe Boolean IMetadataTypeLibrary.IsSupported(Byte* source)
        //    {
        //    var magic = *(UInt32*)source;
        //    return (magic == SLTG_SIGNATURE);
        //    }

        private unsafe Boolean Load(Byte* source, Int64 size, Encoding encoding)
            {
            if (source == null) { throw new ArgumentNullException(nameof(source)); }
            if (encoding == null) { throw new ArgumentNullException(nameof(encoding)); }
            try
                {
                storage.IsLoading = true;
                if (*(UInt32*) source != SLTG_SIGNATURE) { return false; }
                using (var task = Task.Factory.StartNew(()=> Load((SLTG_HEADER*)source, size, encoding)))
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

        [DebuggerDisplay("{UniqueIdentifier}")]
        private class TypeInfo {
            public Tuple<Stream,Int32,Int32> Stream;
            public String LTypeId;
            public String GTypeId;
            public Byte[] HelpString;
            public String HelpFile;
            public Int32 HelpContext;
            public Int16 NextType;
            public UInt16 TypeName;
            public Guid UniqueIdentifier;
            public TYPEKIND Kind;
            }

        #region M:Load(SLTG_HEADER*,Encoding)
        private unsafe void Load(SLTG_HEADER* header, Int64 size, Encoding encoding) {
            if (header == null) { throw new ArgumentNullException(nameof(header)); }
            if (encoding == null) { throw new ArgumentNullException(nameof(encoding)); }
            var fi = (SLTG_STREAM*)(header + 1);
            var ni = (Byte*)(fi + header->NumberOfStreams);
            var di = ni + header->NameTableSize + header->ExtraSize;
            var streams = new Dictionary<String,Tuple<Stream,Int32,Int32>>();
            var j = 0;
            for (var i = header->FirstStream; i != -1; i = fi[i].NextStream) {
                var si = fi + i;
                Debug.WriteLine($"[{i}]:Length={si->Length,5};NameOffset={si->NameOffset:X4};NextStream={si->NextStream,2};Name={ToStreamName(ni + si->NameOffset)}");
                streams.Add(ToStreamName(ni + si->NameOffset), Tuple.Create<Stream,Int32,Int32>(new SourceStream(di, si->Length), i, j));
                di += si->Length;
                j++;
                }
            Tuple<Stream,Int32,Int32> catalogue;
            var rtypes = new List<TypeInfo>();
            if (streams.TryGetValue(SLTG_DIR_NAME, out catalogue)) {
                SLTGHelpManager docmgr = null;
                using (var reader = new SourceReader(catalogue.Item1)) {
                    reader.ReadBlock(out SLTG_DIR_CHNUK1 chunk1);
                    if (chunk1.FirstWord != SLTG_FIRST_WORD) { throw new ArgumentOutOfRangeException(nameof(header)); }
                    if (chunk1.Version > SLTG_MAX_VERSION)   { throw new ArgumentOutOfRangeException(nameof(header)); }
                    var deftypeid      = reader.ReadString();
                    storage.HelpString = reader.ReadString();
                    storage.HelpFile   = reader.ReadString();
                    reader.ReadBlock(out SLTG_DIR_CHNUK2 chunk2);
                    storage.Culture = (chunk2.LCID != 0) ? new CultureInfo(chunk2.LCID) : null;
                    storage.HelpContext = chunk2.HelpContext;
                    storage.Flags = chunk2.Flags;
                    storage.UniqueIdentifier = chunk2.UniqueIdentifier;
                    storage.Version = new Version(chunk2.MajorVersion, chunk2.MinorVersion);
                    storage.TargetOperatingSystemPlatform = (SYSKIND)(chunk2.TargetOperatingSystemPlatform & 0xF);
                    var typecount = chunk2.TypeEntries;
                    for (var i = 0; i < typecount; ++i) {
                        var stream = streams[reader.ReadString()];
                        var type = new TypeInfo
                            {
                            Stream           = stream,
                            LTypeId          = reader.ReadString(),
                            GTypeId          = reader.ReadString(),
                            TypeName         = reader.ReadUInt16(),
                            HelpString       = reader.ReadBytes(reader.ReadInt16()),
                            HelpFile         = reader.ReadString(),
                            HelpContext      = reader.ReadInt32(),
                            NextType         = reader.ReadInt16(),
                            UniqueIdentifier = reader.ReadGuid(),
                            Kind = (TYPEKIND)reader.ReadUInt16(),
                            };
                        rtypes.Add(type);
                        }
                    reader.ReadInt32();
                    docmgr = new SLTGHelpManager(reader);
                    N = new NameManager(reader, (SYSKIND)chunk2.TargetOperatingSystemPlatform, chunk2.LCID);
                    storage.Name = N[chunk1.usHlnam];
                    //var ntypes = rtypes.Where(i => i.Kind != TYPEKIND.TKIND_ALIAS).ToArray();
                    var ntypes = rtypes.ToArray();
                    T = new SLTGTypeReference[ntypes.Length];
                    for (var i = 0; i < T.Count; i++) { T[i] = new SLTGTypeReference(); }
                    for (var i = 0; i < T.Count; i++) {
                        var type = ntypes[i];
                        T[i].TypeReference = new SLTGTypeDescriptor(
                            this,
                            type.UniqueIdentifier,
                            type.Kind,
                            N[type.TypeName],
                            docmgr.Decode(type.HelpString),
                            type.HelpFile,
                            type.HelpContext,
                            type.Stream.Item1,
                            N, docmgr
                            );
                        continue;
                        }
                    }
                }
            storage.DefinedTypes = new ReadOnlyCollection<ITypeLibraryTypeDescriptor>(T.Select(i => i.TypeReference).ToArray());
            return;
            }
        #endregion
        #region M:ToStreamName(Byte*):String
        private static unsafe String ToStreamName(Byte* source) {
            var r = new StringBuilder();
            Char c;
            while ((c = (Char)(*source++)) != 0) {
                r.Append(c);
                }
            return r.ToString();
            }
        #endregion
        #region M:Decode(VARTYPE):Object
        public unsafe Object Decode(VARTYPE typedef, Byte* source) {
            if (source == null) { throw new ArgumentNullException(nameof(source)); }
            var type = typedef;
            switch (type) {
                case VARTYPE.VT_I1:   { return *(SByte*)source;  }
                case VARTYPE.VT_I2:   { return *(Int16*)source;  }
                case VARTYPE.VT_INT:
                case VARTYPE.VT_I4:   { return *(Int32*)source;  }
                case VARTYPE.VT_I8:   { return *(Int64*)source;  }
                case VARTYPE.VT_UI1:  { return *(Byte*)source;   }
                case VARTYPE.VT_UI2:  { return *(UInt16*)source; }
                case VARTYPE.VT_UINT:
                case VARTYPE.VT_UI4:  { return *(UInt32*)source; }
                case VARTYPE.VT_UI8:  { return *(UInt64*)source; }
                case VARTYPE.VT_BSTR: { return ReadString(ref source); }
                }
            return null;
            }
        #endregion

        public ITypeLibraryTypeDescriptor TypeOf(Int16 typeref) {
            var typedef = (VARTYPE)(typeref & 0x3F);
            if (typedef != VARTYPE.VT_ILLEGAL) {
                return Scope.TypeOf(typedef);
                }
            else
                {
                return null;
                }
            }

        public unsafe ITypeLibraryTypeDescriptor TypeOf(SLTG_TYPE_DEF typeref, Byte* block, Int32 offset, SLTGImportManager imgr) {
            if (typeref.m_isOffset) {
                return TypeOf(
                    new SLTG_TYPE_DEF(*(UInt16*)(block + typeref.Offset)),
                    block, typeref.Offset, imgr);
                }
            switch (typeref.m_tdesckind) {
                case VARTYPE.VT_USERDEFINED:
                    {
                    var reference = imgr.References[(*(block + offset + sizeof(Int16))) >> 2];
                    if (reference.GlobalIndex == -1) { return T[reference.Index]; }
                    var extref = Scope.LoadTypeLibrary(N[reference.GlobalIndex], TargetOperatingSystemPlatform);
                    return extref.DefinedTypes[reference.Index];
                    }
                case VARTYPE.VT_PTR:
                    {
                    var typedef = new SLTG_TYPE_DEF(*(block + offset + sizeof(Int16)));
                    return new TypeLibraryTypePointer(TypeOf(typedef, block, offset + sizeof(Int16), imgr));
                    }
                case VARTYPE.VT_CARRAY:
                    {
                    var source = block + *(Int16*)(block + offset + sizeof(Int16));
                    var typedef = TypeOf(new SLTG_TYPE_DEF(*(UInt16*)(block + offset + sizeof(Int16) * 2)));
                    return TypeLibraryFixedArray.FromSafeArray(TargetOperatingSystemPlatform, source, typedef);
                    }
                default:
                    {
                    return TypeOf(typeref.m_tdesckind);
                    }
                }
            }

        public ITypeLibraryTypeDescriptor TypeOf(SLTG_TYPE_DEF typeref) {
            var r = TypeOf(typeref.m_tdesckind);
            if (typeref.m_ptrkind != SLTG_PTRKIND.PTRKIND_IGNORE) {
                r = new TypeLibraryTypePointer(r);
                }
            return r;
            }

        public ITypeLibraryTypeDescriptor TypeOf(VARTYPE typeref) {
            return Scope.TypeOf(typeref);
            }

        #region M:ReadString(ref Byte*):String
        private static unsafe String ReadString(ref Byte* source) {
            var c = ReadShort(ref source);
            if (c == -1) { return null; }
            var r = new Byte[c];
            for (var i = 0; i < c; i++) {
                r[i] = *source++;
                }
            return Encoding.ASCII.GetString(r);
            }
        #endregion
        #region M:ReadUShort(ref Byte*):UShort
        private static unsafe UShort ReadUShort(ref Byte* source) {
            var r = *((UShort*)source);
            source += sizeof(UShort);
            return r;
            }
        #endregion
        #region M:ReadShort(ref Byte*):Short
        private static unsafe Short ReadShort(ref Byte* source) {
            var r = *((Short*)source);
            source += sizeof(Short);
            return r;
            }
        #endregion
        #region M:ReadLong(ref Byte*):Long
        private static unsafe Long ReadLong(ref Byte* source) {
            var r = *((Long*)source);
            source += sizeof(Long);
            return r;
            }
        #endregion
        #region M:ReadULong(ref Byte*):ULong
        private static unsafe ULong ReadULong(ref Byte* source) {
            var r = *((ULong*)source);
            source += sizeof(ULong);
            return r;
            }
        #endregion

        private static unsafe Guid ReadGuid(ref Byte* source) {
            var r = *((Guid*)source);
            source += sizeof(Guid);
            return r;
            }

        private static unsafe Byte[] ReadEncodingBlock(ref Byte* source) {
            var c = ReadShort(ref source);
            if (c == -1) { return null; }
            if (c ==  0) { return null; }
            //var header = ReadUShort(ref source);
            //var state = *source++;
            //var coalesce = (state & 0x1) == 0x1;
            //var roundup  = (state & 0x2) == 0x2;
            //var size = ReadLong(ref source);
            var r = new Byte[c];
            for (var i = 0; i < c; i++) {
                r[i] = *source++;
                }
            //r[c - 1] = 0;
            return r;
            }

        private NameManager N;
        public IList<SLTGTypeReference> T;

        protected internal override unsafe Boolean Load(out Exception e, Byte* source, Int64 size)
            {
            e = null;
            return Load(source, size, Encoding);
            }
        }
}