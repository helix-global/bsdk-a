using System;
using BinaryStudio.PortableExecutable.Win32;

namespace BinaryStudio.PortableExecutable.CodeView
    {
    public class CodeViewCrossScopeExportsSSection : CodeViewPrimarySSection
        {
        public override DEBUG_S Type { get { return DEBUG_S.DEBUG_S_CROSSSCOPEEXPORTS; }}
        internal unsafe CodeViewCrossScopeExportsSSection(CodeViewSection section, Int32 offset, Byte* content, Int32 length)
            : base(section, offset, content, length)
            {
            Console.Error.WriteLine($"{Type}");
            }
        }
    }