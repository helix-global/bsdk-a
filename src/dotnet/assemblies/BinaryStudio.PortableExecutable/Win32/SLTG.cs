using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using BinaryStudio.PortableExecutable;

namespace Microsoft.Win32
    {
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    internal struct SLTG_PARAM_DEFN
        {
        public readonly UInt16 m_hlnam;
        public readonly UInt16 m_htdefn;
        public Boolean IsSimpleType { get { return (m_hlnam & 0x01) == 0x01; }}

        #region P:NameOffset:Int32
        public Int32 NameOffset { get {
            var r = ((m_hlnam & 0xfffe) == 0xfffe)
                ? 0xffff
                : (m_hlnam & 0xfffe);
            return (r == 0xffff)
                    ? -1
                    : (Int32)r;
            }}
        #endregion
        }

    internal enum SLTG_REF_KIND
        {
        REF_NAME = 0,
        REF_QUALNAME = 1,
        REF_NONAME = 2,
        REF_BASE = 3
        }

    internal enum SLTG_DEPEND_KIND
        {
        DEP_NONE,
        DEP_CODE,
        DEP_FRAME,
        DEP_LAYOUT,
        DEP_NESTED,
        DEP_BASE
        }

    [StructLayout(LayoutKind.Explicit, Pack = 1)]
    internal struct SLTG_UB_IMPTYPE
        {
        [FieldOffset(0)] private readonly Int16 m_cRefs;
        [FieldOffset(2)] private readonly UInt16 flags;
        [FieldOffset(4)] private readonly Int16 m_himptypeNext;
        [FieldOffset(6)] public  readonly Int16 m_hlnam;
        [FieldOffset(6)] private readonly Int16 m_hrghlnam;
        public SLTG_REF_KIND m_refkind { get { return (SLTG_REF_KIND)(flags & 0x3F); }}
        public Boolean m_isDeclRef     { get { return (flags & 0x0040) == 0x0040; }}
        public Boolean m_isExcodeRef   { get { return (flags & 0x0080) == 0x0080; }}
        public Boolean m_isInternalRef { get { return (flags & 0x4000) == 0x4000; }}
        public SLTG_DEPEND_KIND m_depkind { get { return (SLTG_DEPEND_KIND)((flags >> 8) & 0x3F); }}
        }

    internal enum SLTG_PARAMKIND
        {
        PARAMKIND_IN,
        PARAMKIND_OUT,
        PARAMKIND_INOUT,
        PARAMKIND_IGNORE
        }

    internal enum SLTG_PTRKIND
        {
        PTRKIND_IGNORE,
        PTRKIND_NEAR,
        PTRKIND_FAR,
        PTRKIND_NEAR32,
        PTRKIND_FAR32,
        PTRKIND_BASED,
        PTRKIND_HUGE,
        PTRKIND_BASIC,
        }

    [DebuggerDisplay("{m_tdesckind}")]
    internal class SLTG_TYPE_DEF
        {
        public VARTYPE m_tdesckind { get; }
        public SLTG_PARAMKIND m_paramkind { get; }
        public Boolean m_isRetval { get; }
        public Boolean m_isResizable { get; }
        public Boolean m_isConst { get; }
        public Boolean m_isLCID { get; }
        public Boolean m_isInRecord { get; }
        public Boolean m_isOffset { get; }
        public SLTG_PTRKIND m_ptrkind { get; }
        public Int32 Offset { get; }

        public SLTG_TYPE_DEF(UInt16 source) {
            Offset = source;
            m_tdesckind = (VARTYPE)(source & 0x3F);
            if (m_tdesckind > VARTYPE.VT_LPWSTR) {
                m_isOffset = true;
                }
            m_paramkind = (SLTG_PARAMKIND)((source >> 14) & 0x03);
            m_ptrkind   = (SLTG_PTRKIND)((source >> 9) & 0x07);
            m_isConst      = (source & 0x0040) == 0x0040;
            m_isRetval     = (source & 0x0080) == 0x0080;
            m_isResizable  = (source & 0x0100) == 0x0100;
            m_isLCID       = (source & 0x2000) == 0x2000;
            m_isInRecord   = (source & 0x1000) == 0x1000;
            }
        }

    internal enum SLTG_DEFNKIND
        {
        DK_VarDefn,
        DK_ParamDefn,
        DK_MbrVarDefn,
        DK_FuncDefn,
        DK_VirtualFuncDefn,
        DK_DllEntryDefn,
        DK_RecTypeDefn
        }

    internal enum SLTG_VAR_KIND
        {
        VKIND_DataMember,
        VKIND_Base,
        VKIND_Enumerator,
        VKIND_Formal
        }

    [Flags]
    internal enum IMPLTYPEFLAG : ushort
        {
        IMPLTYPEFLAG_FDEFAULT       = 1,
        IMPLTYPEFLAG_FSOURCE        = 2,
        IMPLTYPEFLAG_FRESTRICTED    = 4,
        IMPLTYPEFLAG_FDEFAULTVTABLE = 8,
        IMPLTYPEFLAG_FSET           = 0x8000
        }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    internal struct SLTG_DEFN
        {
        private readonly UInt16 flags;
        public  readonly Int16 m_hdefnNext;
        private readonly Int16 Name;
        public SLTG_DEFNKIND Kind1 { get { return (SLTG_DEFNKIND)(flags & 0x007); }}
        public SLTG_VAR_KIND VKind { get { return (SLTG_VAR_KIND)((flags & 0x1C0) >> 6); }}
        public Boolean IsPublic      { get { return (flags & 0x0008) == 0x0008; }}
        public Boolean IsV2          { get { return (flags & 0x0020) == 0x0020; }}
        public Boolean IsSimpleType  { get { return (flags & 0x0200) == 0x0200; }}
        public Boolean IsStatic      { get { return (flags & 0x0400) == 0x0400; }}
        public Boolean IsSimpleConst { get { return (flags & 0x0800) == 0x0800; }}
        public Boolean HasConstVal   { get { return (flags & 0x1000) == 0x1000; }}
        public Boolean HasNew        { get { return (flags & 0x2000) == 0x2000; }}
        public Boolean IsDispatch    { get { return (flags & 0x4000) == 0x4000; }}
        public Boolean IsReadOnly    { get { return (flags & 0x8000) == 0x8000; }}
        }

    [StructLayout(LayoutKind.Explicit, Pack = 1)]
    internal struct SLTG_VAR_DEFN
        {
        [FieldOffset(0)] private readonly UInt16 flags;
        [FieldOffset(2)] public  readonly Int16  m_hdefnNext;
        [FieldOffset(4)] private readonly Int16  m_hlnam;
        [FieldOffset(6)] private readonly Int16  m_oVar;
        [FieldOffset(6)] private readonly Int16  m_sConstVal;
        [FieldOffset(6)] private readonly Int16  m_hchunkConstVal;
        [FieldOffset(6)] public  readonly Int16  m_fImplType;
        [FieldOffset(8)] public  readonly UInt16 m_htdefn;
        public SLTG_DEFNKIND Kind1        { get { return (SLTG_DEFNKIND)(flags & 0x007); }}
        public SLTG_VAR_KIND VKind        { get { return (SLTG_VAR_KIND)((flags & 0x1C0) >> 6); }}
        public Boolean IsPublic      { get { return (flags & 0x0008) == 0x0008; }}
        public Boolean IsV2          { get { return (flags & 0x0020) == 0x0020; }}
        public Boolean IsSimpleType  { get { return (flags & 0x0200) == 0x0200; }}
        public Boolean IsStatic      { get { return (flags & 0x0400) == 0x0400; }}
        public Boolean IsSimpleConst { get { return (flags & 0x0800) == 0x0800; }}
        public Boolean HasConstVal   { get { return (flags & 0x1000) == 0x1000; }}
        public Boolean HasNew        { get { return (flags & 0x2000) == 0x2000; }}
        public Boolean IsDispatch    { get { return (flags & 0x4000) == 0x4000; }}
        public Boolean IsReadOnly    { get { return (flags & 0x8000) == 0x8000; }}
        }

        [StructLayout(LayoutKind.Explicit, Pack = 1)]
        internal struct SLTG_MBR_VAR_DEFN
            {
            /* DEFN */         [FieldOffset( 0)] private readonly UInt16 flags;
            /* DEFN */         [FieldOffset( 2)] public  readonly Int16  m_hdefnNext;
            /* DEFN */         [FieldOffset( 4)] public  readonly Int16  m_hlnam;
            /* VAR_DEFN */     [FieldOffset( 6)] private readonly Int16  m_oVar;
            /* VAR_DEFN */     [FieldOffset( 6)] public  readonly Int16  m_sConstVal;
            /* VAR_DEFN */     [FieldOffset( 6)] private readonly Int16  m_hchunkConstVal;
            /* VAR_DEFN */     [FieldOffset( 6)] private readonly IMPLTYPEFLAG m_fImplType;
            /* VAR_DEFN */     [FieldOffset( 8)] public  readonly UInt16 m_htdefn;
            /* MEMBER_DEFN */  [FieldOffset(10)] private readonly Int32  m_hmember;
            /* MEMBER_DEFN */  [FieldOffset(14)] private readonly Int16  m_usHelpContext;
            /* MEMBER_DEFN */  [FieldOffset(16)] private readonly Int16  m_hstDocumentation;
            /* MBR_VAR_DEFN */ [FieldOffset(18)] public  readonly UInt16 m_fVarFlags;

            public SLTG_DEFNKIND Kind1        { get { return (SLTG_DEFNKIND)(flags & 0x007); }}
            public SLTG_VAR_KIND Kind2        { get { return (SLTG_VAR_KIND)((flags & 0x1C0) >> 6); }}
            public Boolean IsPublic      { get { return (flags & 0x0008) == 0x0008; }}
            public Boolean IsV2          { get { return (flags & 0x0020) == 0x0020; }}
            public Boolean IsSimpleType  { get { return (flags & 0x0200) == 0x0200; }}
            public Boolean IsStatic      { get { return (flags & 0x0400) == 0x0400; }}
            public Boolean IsSimpleConst { get { return (flags & 0x0800) == 0x0800; }}
            public Boolean HasConstVal   { get { return (flags & 0x1000) == 0x1000; }}
            public Boolean HasNew        { get { return (flags & 0x2000) == 0x2000; }}
            public Boolean IsDispatch    { get { return (flags & 0x4000) == 0x4000; }}
            public Boolean IsReadOnly    { get { return (flags & 0x8000) == 0x8000; }}
            public Boolean IsResizable   { get { return (flags & 0x0100) == 0x0100; }}
            public unsafe Int32 Size { get { return sizeof(SLTG_MBR_VAR_DEFN) - (IsV2 ? 0 : sizeof(UInt16)); }}
            }
    }