#define WINE
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Microsoft.Win32;

namespace BinaryStudio.PortableExecutable.TypeLibrary
    {
    using CALLCONV = System.Runtime.InteropServices.ComTypes.CALLCONV;
    using INVOKEKIND = System.Runtime.InteropServices.ComTypes.INVOKEKIND;

    internal sealed class SLTGMethodDescriptor : TypeLibraryMethodDescriptor
        {
        private enum VAR_KIND
            {
            VKIND_DataMember,
            VKIND_Base,
            VKIND_Enumerator,
            VKIND_Formal
            }

        private enum DEFNKIND
            {
            DK_VarDefn,
            DK_ParamDefn,
            DK_MbrVarDefn,
            DK_FuncDefn,
            DK_VirtualFuncDefn,
            DK_DllEntryDefn,
            DK_RecTypeDefn
            }

        private enum FUNC_KIND
            {
            FKIND_NonVirtual,
            FKIND_Virtual,
            FKIND_Static,
            FKIND_Dispatch
            }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        private struct FUNC_TYPE_DEFN
            {
            #if WINE
            private readonly UInt16 flags;
            public  readonly Int16  m_hdefnNext;
            public  readonly Int16  m_hlnam;
            public  readonly Int32  m_hmember;
            public  readonly Int16  m_usHelpContext;
            public  readonly Int16  m_hstDocumentation;

            public  readonly Int16  m_hdefnFormalFirst;
            private readonly UInt16 m_argsflags;
            public  readonly UInt16 m_htdefnResult;

            private readonly Int16  m_oVft;
            public  readonly UInt16 m_fFuncFlags;
            public DEFNKIND  m_defnkind   { get { return (DEFNKIND)(flags & 0x007); }}
            public FUNC_KIND m_kind       { get { return (FUNC_KIND)((flags & 0x1C0) >> 6); }}
            public Boolean IsPublic      { get { return (flags & 0x0008) == 0x0008; }}
            public Boolean m_fV2Flags    { get { return (flags & 0x0020) == 0x0020; }}
            public Boolean m_isPureOrSimpleType        { get { return (flags & 0x0200) == 0x0200; }}
            public Boolean m_isStatic                  { get { return (flags & 0x0400) == 0x0400; }}
            public Boolean m_isRestrictedOrSimpleConst { get { return (flags & 0x0800) == 0x0800; }}
            public Boolean IsMethod   { get { return (flags & 0x1000) == 0x1000; }}
            public Boolean IsPropertyGet        { get { return (flags & 0x2000) == 0x2000; }}
            public Boolean IsPropertyLet    { get { return (flags & 0x4000) == 0x4000; }}
            public Boolean IsPropertySet    { get { return (flags & 0x8000) == 0x8000; }}
            public CALLCONV m_cc { get { return (CALLCONV)(m_argsflags & 0x07); } }
            public Int16    m_cArgs { get { return (Int16)((m_argsflags >> 3) & 0x3F); }}
            public Int16    m_cArgsOpt { get { return (Int16)((m_argsflags >> 9) & 0x3F); }}
            public Boolean  m_isSimpleTypeResult { get { return (m_argsflags & 0x8000) == 0x8000; } }
            public TypeLibraryMethodAttributes InvokeKind { get { return (TypeLibraryMethodAttributes)((flags >> 12) & 0xF); } }
            #else
            private readonly UInt16 flags;
            public  readonly Int16 m_hdefnNext;
            public  readonly Int16 m_hlnam;
            public  readonly Int16 m_htdefn;
            private readonly Int32 m_hmember;
            private readonly Int16 m_usHelpContext;
            private readonly Int16 m_hstDocumentation;
            private readonly UInt16 m_argsflags;
            private readonly Int16 m_htdefnResult;
            public  readonly Int16 m_hdefnFormalFirst;
            private readonly Int16 m_oVft;
            public DEFNKIND Kind1        { get { return (DEFNKIND)(flags & 0x007); }}
            public FUNC_KIND Kind2        { get { return (FUNC_KIND)((flags & 0x1C0) >> 6); }}
            public Boolean IsPublic      { get { return (flags & 0x0008) == 0x0008; }}
            public Boolean IsV2          { get { return (flags & 0x0020) == 0x0020; }}
            public Boolean IsPure  { get { return (flags & 0x0200) == 0x0200; }}
            public Boolean IsStaticLocalVars      { get { return (flags & 0x0400) == 0x0400; }}
            public Boolean IsRestricted { get { return (flags & 0x0800) == 0x0800; }}
            public Boolean IsMethod   { get { return (flags & 0x1000) == 0x1000; }}
            public Boolean IsPropertyGet        { get { return (flags & 0x2000) == 0x2000; }}
            public Boolean IsPropertyLet    { get { return (flags & 0x4000) == 0x4000; }}
            public Boolean IsPropertySet    { get { return (flags & 0x8000) == 0x8000; }}
            public CALLCONV m_cc { get { return (CALLCONV)(m_argsflags & 0x07); } }
            public Int16    m_cArgs { get { return (Int16)((m_argsflags >> 3) & 0x3F); }}
            public Int16    m_cArgsOpt { get { return (Int16)((m_argsflags >> 9) & 0x3F); }}
            public Boolean  m_isSimpleTypeResult { get { return (m_argsflags & 0x8000) == 0x8000; } }
            public TypeLibraryMethodAttributes InvokeKind { get { return (TypeLibraryMethodAttributes)((flags >> 12) & 0xF); } }
            #endif
            }

        public unsafe SLTGMethodDescriptor(SLTGTypeLibrary library, SLTGTypeDescriptor declaringtype, NameManager nmgr, Byte* block, ref Int32 offset,
                SLTGImportManager imgr, SLTGHelpManager docmgr)
            : base(declaringtype)
            {
            var parameters = new List<ITypeLibraryParameterDescriptor>();
            var desc = (FUNC_TYPE_DEFN*)(block + offset);
            Name = nmgr[desc->m_hlnam];
            HelpString = (desc->m_hstDocumentation != -1)
                ? docmgr.Decode(block + desc->m_hstDocumentation)
                : null;
            Id = desc->m_hmember;
            switch (desc->m_kind) {
                case FUNC_KIND.FKIND_NonVirtual:
                    {
                    }
                    break;
                case FUNC_KIND.FKIND_Virtual:
                    {

                    }
                    break;
                case FUNC_KIND.FKIND_Static:
                    {
                    }
                    break;
                case FUNC_KIND.FKIND_Dispatch:
                    {
                    }
                    break;
                default: throw new ArgumentOutOfRangeException();
                }
            Attributes = desc->InvokeKind;
            if (desc->m_fV2Flags)                  { Flags = (TypeLibFuncFlags)desc->m_fFuncFlags; }
            if (desc->m_isRestrictedOrSimpleConst) { Flags |= TypeLibFuncFlags.FRestricted;        }
            CallingConvention = desc->m_cc;
            ReturnType = library.TypeOf(new SLTG_TYPE_DEF(desc->m_htdefnResult), block, 0, imgr);
            Debug.Print("Method:{0}", Name);
            Debug.IndentLevel++;
            try
                {
                if (desc->m_cArgs > 0) {
                    var src = (Int32)desc->m_hdefnFormalFirst;
                    for (var i = 0; i < desc->m_cArgs; i++) {
                        parameters.Add(new SLTGParameterDescriptor(library, nmgr, block, ref src, imgr));
                        }
                    }
                }
            finally
                {
                Debug.IndentLevel--;
                }
            Parameters = new ReadOnlyCollection<ITypeLibraryParameterDescriptor>(parameters);
            offset = desc->m_hdefnNext;
            }

        public override String Name { get; }
        public override String HelpString { get; }
        public override TypeLibFuncFlags Flags { get; }
        public override CALLCONV CallingConvention { get; }
        public override TypeLibraryMethodAttributes Attributes { get; }
        public override ITypeLibraryTypeDescriptor ReturnType { get; }
        public override Int32 Id { get; }
        public override IList<ITypeLibraryParameterDescriptor> Parameters { get; }
        public Int32 HelpContext { get; }
        }
    }