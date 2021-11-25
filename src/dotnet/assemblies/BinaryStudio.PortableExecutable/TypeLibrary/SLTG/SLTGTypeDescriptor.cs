using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using BinaryStudio.PortableExecutable.TypeLibrary.SLTG;
using Microsoft.Win32;

namespace BinaryStudio.PortableExecutable.TypeLibrary
    {
    using TYPEKIND = System.Runtime.InteropServices.ComTypes.TYPEKIND;
    internal sealed class SLTGTypeDescriptor : TypeLibraryTypeDescriptor, ISLTGMemberDescriptor
        {
        private const Byte FIRSTBYTE = ('D' * 4 + 'T' * 2 + 'I') & 0xFF;
        private const Byte VERSION   = 5;

        public override Version Version { get; }
        public override String Name { get; }
        public override String HelpString { get; }
        public String HelpFile { get; }
        public override ITypeLibraryTypeDescriptor UnderlyingType { get; }
        public override ITypeLibraryTypeDescriptor BaseType { get; }
        public override ITypeLibraryDescriptor Library { get; }
        public override Boolean IsAlias { get; }
        public override Boolean IsPrimitive { get { return false; }}
        public override Boolean IsPointer   { get { return false; }}
        public override Boolean IsArray     { get { return false; }}
        public override Boolean IsEnum { get; }
        public override Boolean IsClass { get; }
        public override Boolean IsUnion { get; }
        public override Boolean IsInterface { get; }
        public override Boolean IsStructure { get; }
        public override Boolean IsModule { get; }
        public override Boolean IsDispatch { get; }
        public override Guid? UniqueIdentifier { get; }
        public Int32 HelpContext { get; }
        public override IList<ITypeLibraryFieldDescriptor>    DeclaredFields        { get; }
        public override IList<ITypeLibraryMethodDescriptor>   DeclaredMethods       { get; }
        public override IList<ITypeLibraryPropertyDescriptor> DeclaredProperties    { get; }
        public override IList<ITypeLibraryTypeReference>      ImplementedInterfaces { get; }
        public override TypeLibTypeFlags Flags { get; }

        private enum COMPSTATE : byte
            {
            CS_UNDECLARED,
            CS_SEMIDECLARED,
            CS_DECLARED,
            CS_REGENERATE,
            CS_COMPILED,
            CS_ADDRESSABLE,
            CS_QUASIDECLARED,
            CS_QUASIUNDECLARED,
            CS_RUNNABLE
            }

        //private COMPSTATE CompilationState { get; }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        private struct TYPE_DATA
            {
            public readonly Int16 m_cMeth;
            public readonly Int16 m_cDataMember;
            public readonly Int16 m_cBase;
            public readonly Int16 m_cNestedType;
            public readonly Int16 m_hdefnFirstMeth;
            public readonly Int16 m_hdefnFirstDataMbrNestedType;
            public readonly Int16 m_hdefnFirstBase;
            public readonly Int16 m_hdefnLastMeth;
            public readonly Int16 m_hdefnLastDataMbrNestedType;
            public readonly Int16 m_hdefnLastBase;
            public readonly Int16 m_htdefnAlias;
            public readonly Int16 m_hmvdefnPredeclared;
            public readonly Int32 m_uHelpContextBase;
            public readonly Int16 m_isSimpleTypeAlias;
            }

        private enum PTRKIND
            {
            PTRKIND_Ignore,
            PTRKIND_Near,
            PTRKIND_Far,
            PTRKIND_Near32,
            PTRKIND_Far32,
            PTRKIND_Based,
            PTRKIND_Huge,
            PTRKIND_Basic,
            }

        private enum PARAMKIND
            {
            PARAMKIND_In,
            PARAMKIND_Out,
            PARAMKIND_InOut,
            PARAMKIND_Ignore
            }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        private struct DYN_TYPEBIND
            {
            Int16 isProtocol;
            Int16 m_cbSize;
            Int16 m_cbAlignment;
            Int32 m_oPvft;
            Int16 m_cbPvft;
            Int32 m_hmemberConst;
            Int32 m_hmemberDest;
            }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        private struct DYN_BINDNAME_TABLE
            {
            Int16 m_cBuckets;
            Int16 m_hchunkBucketTbl;
            }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        private struct TYPE_DEFN
            {
            private readonly UInt16 flags;
            public VARTYPE m_tdesckind { get { return (VARTYPE)(flags & 0x3F); } }
            public Boolean m_isConst { get { return (flags & 0x40) == 0x40; } }
            public Boolean m_isRetval { get { return (flags & 0x80) == 0x80; } }
            public Boolean m_isResizable { get { return (flags & 0x100) == 0x100; } }
            public PTRKIND m_ptrkind { get { return (PTRKIND)((flags >> 9) & 0x7); } }
            public Boolean m_isInRecord { get { return (flags & 0x1000) == 0x1000; } }
            public Boolean m_isLCID { get { return (flags & 0x2000) == 0x2000; } }
            public PARAMKIND m_paramkind { get { return (PARAMKIND)((flags >> 14) & 0x3); } }
            }

        public unsafe SLTGTypeDescriptor(SLTGTypeLibrary library, Guid uniqueidentifier,
            TYPEKIND typekind, String name, String helpstring, String helpfile,
            Int32 helpcontext, Stream stream, NameManager nmgr, SLTGHelpManager docmgr)
            :base(null)
            {
            ImplementedInterfaces = EmptyList<ITypeLibraryTypeReference>.Value;
            UnderlyingType = null;
            var declaredfields  = new List<ITypeLibraryFieldDescriptor>();
            var mdesc = new List<ITypeLibraryMethodDescriptor>();
            var pdesc = new List<ITypeLibraryPropertyDescriptor>();
            var impltypes = new List<ITypeLibraryTypeReference>();
            Library = library;
            UniqueIdentifier = uniqueidentifier;
            IsAlias     = (typekind == TYPEKIND.TKIND_ALIAS);
            IsEnum      = (typekind == TYPEKIND.TKIND_ENUM);
            IsClass     = (typekind == TYPEKIND.TKIND_COCLASS);
            IsUnion     = (typekind == TYPEKIND.TKIND_UNION);
            IsInterface = (typekind == TYPEKIND.TKIND_INTERFACE);
            IsStructure = (typekind == TYPEKIND.TKIND_RECORD);
            IsModule    = (typekind == TYPEKIND.TKIND_MODULE);
            IsDispatch  = (typekind == TYPEKIND.TKIND_DISPATCH);
            Name = name;
            HelpString = helpstring;
            HelpFile = helpfile;
            HelpContext = helpcontext;
            Debug.Print("Type:{0}", Name);
            Debug.IndentLevel++;
            try
                {
                using (var reader = new SourceReader(stream)) {
                    var firstbyte = reader.ReadByte();
                    var version = reader.ReadByte();
                    if (firstbyte != FIRSTBYTE) { throw new NotSupportedException(); }
                    if (version   != VERSION)   { throw new NotSupportedException(); }
                    var m_lDtmbrs = -1;
                    var m_lImpMgr = -1;
                    fixed (Byte* bytes = reader.ReadBytes(26)) {
                        var r = (Int16*)bytes;
                        m_lImpMgr       = *(Int32*)(r);
                        var m_lEntryMgr = *(Int32*)(r + 2);
                        m_lDtmbrs       = *(Int32*)(r + 4);
                        var m_lTdata    = *(Int32*)(r + 6);
                        Version = new Version(r[8], r[9]);
                        var IsBadTypeLib = ((r[11] & 0x0001) == 0x0001);
                        //IsDual       = ((r[11] & 0x0002) != 0x0002);
                        //IsBasic      = ((r[12] & 0x0001) == 0x0001);
                        Flags        = (TypeLibTypeFlags)((r[12] & 0x0FF8) >> 3);
                        var CompilationState = (COMPSTATE)bytes[24];
                        //if ((TYPEKIND)bytes[25] != typekind) { throw new NotSupportedException(); }
                        if (IsBadTypeLib) {
                            Flags &= TypeLibTypeFlags.FCanCreate | TypeLibTypeFlags.FAppObject;
                            }
                        }
                    if (library.Version.Major > 2) { var lHrefOffset = reader.ReadInt32(); }
                    var imgr = new SLTGImportManager(reader, m_lImpMgr);
                    if (m_lDtmbrs != -1) {
                        reader.BaseStream.Seek(m_lDtmbrs, SeekOrigin.Begin);
                        var flags = reader.ReadUInt16();
                        //IsLaidOut = (flags & 0x0001) == 0x0001;
                        var m_blkmgr = new BlockManager(reader);
                        reader.ReadBlock(out TYPE_DATA data);
                        reader.ReadBlock(out DYN_TYPEBIND typebind);
                        reader.ReadBlock(out DYN_BINDNAME_TABLE m_dbindnametbl);
                        fixed (Byte* block = m_blkmgr.m_blkdesc.m_qbMemBlock) {
                            #region fields
                            if (data.m_cDataMember > 0) {
                                var src = (Int32)data.m_hdefnFirstDataMbrNestedType;
                                for (var i = 0; i < data.m_cDataMember; ++i) {
                                    declaredfields.Add(new SLTGFieldDescriptor(library, this, nmgr, block, ref src, imgr));
                                    }
                                }
                            #endregion
                            #region methods
                            if (data.m_cMeth > 0) {
                                Debug.Print("Methods:");
                                Debug.IndentLevel++;
                                try
                                    {
                                    var src = (Int32)data.m_hdefnFirstMeth;
                                    for (var i = 0; i < data.m_cMeth; ++i) {
                                        mdesc.Add(new SLTGMethodDescriptor(library, this, nmgr, block, ref src, imgr, docmgr));
                                        }
                                    }
                                finally
                                    {
                                    Debug.IndentLevel--;
                                    }
                                }
                            #endregion
                            #region types
                            if (data.m_hdefnFirstBase >= 0) {
                                var offset = data.m_hdefnFirstBase;
                                while (offset != -1) {
                                    var baseitem = (SLTG_MBR_VAR_DEFN*)(block + offset);
                                    var typeref = library.TypeOf(new SLTG_TYPE_DEF(*(UInt16*)(block + baseitem->m_htdefn)), block, baseitem->m_htdefn, imgr);
                                    impltypes.Add(new SLTGImplTypeReference(typeref));
                                    offset = baseitem->m_hdefnNext;
                                    }
                                }
                                 if (IsInterface) { if (data.m_cBase > 0) { BaseType = impltypes[0].Type; } }
                            else if (IsDispatch)  { if (data.m_cBase > 0) { BaseType = impltypes[0].Type; } }
                            else if (IsClass) { ImplementedInterfaces = new ReadOnlyCollection<ITypeLibraryTypeReference>(impltypes); }
                            else if (IsAlias) {
                                var baseitem = (SLTG_MBR_VAR_DEFN*)(block + data.m_htdefnAlias);
                                UnderlyingType = library.TypeOf(new SLTG_TYPE_DEF(*(UInt16*)(block + baseitem->m_htdefn)), block, baseitem->m_htdefn, imgr);
                                }
                            #endregion
                            }
                        }
                    }
                }
            finally
                {
                Debug.IndentLevel--; 
                }
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
            DeclaredFields     = new ReadOnlyCollection<ITypeLibraryFieldDescriptor>(declaredfields);
            DeclaredMethods    = new ReadOnlyCollection<ITypeLibraryMethodDescriptor>(mdesc);
            DeclaredProperties = new ReadOnlyCollection<ITypeLibraryPropertyDescriptor>(pdesc);
            }

        unsafe Int32 ISLTGMemberDescriptor.HelpContext(Byte* block, Int32 context) {
            if ((context & 0x0001) != 0x0001) {
                var chunk = (context & ~0x0001) & 0xffff;
                if (chunk == 0xfffe) { return 0; }
                return *(Int32*)(block + chunk);
                }
            var diff = context >> 2;
            return ((context & 0x0002) == 0x0002)
                ? context - diff
                : context + diff;
            }
        }
    }