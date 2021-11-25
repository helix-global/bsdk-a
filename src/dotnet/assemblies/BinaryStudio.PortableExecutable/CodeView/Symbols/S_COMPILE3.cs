using System;
using BinaryStudio.PortableExecutable.Win32;

namespace BinaryStudio.PortableExecutable.CodeView
    {
    [CodeViewSymbol(DEBUG_SYMBOL_INDEX.S_COMPILE3)]
    internal class S_COMPILE3 : CodeViewSymbol
        {
        public override DEBUG_SYMBOL_INDEX Type { get { return DEBUG_SYMBOL_INDEX.S_COMPILE3; }}
        public String CompilerVersion { get; }
        public Version FrontEndVersion { get; }
        public Version BackEndVersion { get; }
        public CV_CFL_LANG Language { get; }
        public Boolean IsCompiledForEditAndContinue { get; }
        public Boolean IsCompiledWithDebugInfo { get; }
        public Boolean IsCompiledWithLTCG { get; }
        public Boolean IsNoDataAlign { get; }
        public Boolean IsManagedPresent { get; }
        public Boolean IsSecurityChecks { get; }
        public Boolean IsHotPatch { get; }
        public Boolean IsConvertedWithCVTCIL { get; }
        public Boolean IsMSILModule { get; }
        public Boolean IsCompiledWithSDL { get; }
        public Boolean IsCompiledWithPGO { get; }
        public Boolean IsEXPModule { get; }
        public CV_CPU_TYPE Machine { get; }

        public unsafe S_COMPILE3(CodeViewSymbolsSSection section, Int32 offset, IntPtr content, Int32 length)
            : base(section, offset, content, length)
            {
            var r = (COMPILESYM3*)content;
            FrontEndVersion = new Version(r->VersionFEMajor, r->VersionFEMinor, r->VersionFEBuild, r->VersionFEQFE);
            BackEndVersion  = new Version(r->VersionMajor, r->VersionMinor, r->VersionBuild, r->VersionFEQFE);
            Language = (CV_CFL_LANG)(r->Flags & 0xFF);
            IsCompiledForEditAndContinue = (r->Flags & 0x00100) == 0x00100;
            IsCompiledWithDebugInfo      = (r->Flags & 0x00200) == 0x00200;
            IsCompiledWithLTCG           = (r->Flags & 0x00400) == 0x00400;
            IsNoDataAlign                = (r->Flags & 0x00800) == 0x00800;
            IsManagedPresent             = (r->Flags & 0x01000) == 0x01000;
            IsSecurityChecks             = (r->Flags & 0x02000) == 0x02000;
            IsHotPatch                   = (r->Flags & 0x04000) == 0x04000;
            IsConvertedWithCVTCIL        = (r->Flags & 0x08000) == 0x08000;
            IsMSILModule                 = (r->Flags & 0x10000) == 0x10000;
            IsCompiledWithSDL            = (r->Flags & 0x20000) == 0x20000;
            IsCompiledWithPGO            = (r->Flags & 0x40000) == 0x40000;
            IsEXPModule                  = (r->Flags & 0x80000) == 0x80000;
            Machine = r->Machine;
            CompilerVersion = ToString(Section.Section.Encoding, (Byte*)(r + 1), Section.Section.IsLengthPrefixedString);
            section.Machine = Machine;
            if (section.Section.CommonObjectFile.CPU == null) {
                section.Section.CommonObjectFile.CPU = Machine;
                }
            }
        }
    }