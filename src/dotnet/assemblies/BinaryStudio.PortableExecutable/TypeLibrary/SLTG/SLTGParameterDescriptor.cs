using System;
using System.Runtime.InteropServices.ComTypes;
using Microsoft.Win32;

namespace BinaryStudio.PortableExecutable.TypeLibrary
    {
    internal sealed class SLTGParameterDescriptor : TypeLibraryParameterDescriptor
        {
        public override String Name { get; }
        public override ITypeLibraryTypeDescriptor ParameterType { get; }
        public override PARAMFLAG Flags { get; }

        public unsafe SLTGParameterDescriptor(SLTGTypeLibrary library, NameManager nmgr, Byte* block, ref Int32 offset, SLTGImportManager imgr) {
            Flags = default(PARAMFLAG);
            var paramdef = (SLTG_PARAM_DEFN*)(block + offset);
            var nameoffset = paramdef->NameOffset;
            if (nameoffset != -1) {
                Name = nmgr[nameoffset];
                }
            if (paramdef->IsSimpleType) {
                var typedef = new SLTG_TYPE_DEF(paramdef->m_htdefn);
                switch (typedef.m_paramkind) {
                    case SLTG_PARAMKIND.PARAMKIND_IN:    { Flags |= PARAMFLAG.PARAMFLAG_FIN; } break;
                    case SLTG_PARAMKIND.PARAMKIND_OUT:   { Flags |= PARAMFLAG.PARAMFLAG_FOUT; } break;
                    case SLTG_PARAMKIND.PARAMKIND_INOUT: { Flags |= PARAMFLAG.PARAMFLAG_FIN | PARAMFLAG.PARAMFLAG_FOUT; } break;
                    default: throw new ArgumentOutOfRangeException();
                    }
                if (typedef.m_isRetval) { Flags |= PARAMFLAG.PARAMFLAG_FRETVAL; }
                if (typedef.m_isLCID)   { Flags |= PARAMFLAG.PARAMFLAG_FLCID;   }
                ParameterType = library.TypeOf(typedef);
                }
            else
                {
                var src = (UInt16*)(block + (Int32)paramdef->m_htdefn);
                var typedef = new SLTG_TYPE_DEF(*src);
                switch (typedef.m_paramkind) {
                    case SLTG_PARAMKIND.PARAMKIND_IN:    { Flags |= PARAMFLAG.PARAMFLAG_FIN; } break;
                    case SLTG_PARAMKIND.PARAMKIND_OUT:   { Flags |= PARAMFLAG.PARAMFLAG_FOUT; } break;
                    case SLTG_PARAMKIND.PARAMKIND_INOUT: { Flags |= PARAMFLAG.PARAMFLAG_FIN | PARAMFLAG.PARAMFLAG_FOUT; } break;
                    default: throw new ArgumentOutOfRangeException();
                    }
                if (typedef.m_isRetval) { Flags |= PARAMFLAG.PARAMFLAG_FRETVAL; }
                if (typedef.m_isLCID)   { Flags |= PARAMFLAG.PARAMFLAG_FLCID;   }
                ParameterType = library.TypeOf(typedef, block, (Int32)paramdef->m_htdefn, imgr);
                if (typedef.m_ptrkind != SLTG_PTRKIND.PTRKIND_IGNORE) {
                    ParameterType = new TypeLibraryTypePointer(ParameterType);
                    }
                }
            offset = offset + sizeof(SLTG_PARAM_DEFN);
            }

        //private unsafe ITypeLibraryTypeDescriptor TypeOf(VARTYPE type, UInt16* source, SLTGTypeLibrary library, IList<SLTGTypeRef> references) {
        //    switch (type) {
        //        case VARTYPE.VT_USERDEFINED:
        //            {
        //            var r = (*(source + 1)) >> 2;
        //            Debug.Print("Parameter:{0}", r);
        //            return library.T[references[r].Index];
        //            }
        //        case VARTYPE.VT_PTR:
        //            {
        //            var typedef = new SLTG_TYPE_DEF(*(source + 1));
        //            return new TypeLibraryTypePointer(TypeOf(typedef.m_tdesckind, source + 1, library, references));
        //            }
        //        default:
        //            {
        //            return library.TypeOf(type);
        //            }
        //        }
            //}
        }
    }