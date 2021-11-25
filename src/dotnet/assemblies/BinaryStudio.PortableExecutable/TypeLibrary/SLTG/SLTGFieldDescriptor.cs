using System;
using System.Runtime.InteropServices;
using Microsoft.Win32;

namespace BinaryStudio.PortableExecutable.TypeLibrary
    {
    internal class SLTGFieldDescriptor : TypeLibraryFieldDescriptor
        {
        public unsafe SLTGFieldDescriptor(SLTGTypeLibrary library, ITypeLibraryTypeDescriptor type, NameManager nmgr, Byte* block, ref Int32 offset, SLTGImportManager imgr)
            : base(type)
            {
            var desc = (SLTG_MBR_VAR_DEFN*)(block + offset);
            Name = nmgr[desc->m_hlnam];
            var attributes = default(TypeLibraryFieldAttributes);
            var flags      = default(TypeLibVarFlags);
                 if (desc->IsStatic)    { attributes = TypeLibraryFieldAttributes.VAR_STATIC;   }
            else if (desc->HasConstVal) { attributes = TypeLibraryFieldAttributes.VAR_CONST;    }
            else if (desc->IsDispatch)  { attributes = TypeLibraryFieldAttributes.VAR_DISPATCH; }
            if (desc->IsReadOnly) { flags |= TypeLibVarFlags.FReadOnly; }
            Attributes = attributes;
            Flags = flags;
            var typedef = new SLTG_TYPE_DEF(desc->m_htdefn);
            if (typedef.m_isOffset) {
                FieldType = library.TypeOf(typedef, block, 0, imgr);
                }
            else
                {
                FieldType = library.TypeOf(typedef);
                if (desc->HasConstVal) {
                    if (desc->IsSimpleType) {
                        IsLiteral = true;
                        LiteralValue = library.Decode(typedef.m_tdesckind, block + desc->m_sConstVal);
                        }
                    }
                }
            if (desc->m_hdefnNext != -1) { offset = desc->m_hdefnNext; }
            }

        public override String Name { get; }
        public override String HelpString { get; }
        public override Int32 Id { get; }
        public override ITypeLibraryTypeDescriptor FieldType { get; }
        public override TypeLibVarFlags Flags { get; }
        public override TypeLibraryFieldAttributes Attributes { get; }
        public override Boolean IsLiteral { get; }
        public override Object LiteralValue { get; }
        }
    }