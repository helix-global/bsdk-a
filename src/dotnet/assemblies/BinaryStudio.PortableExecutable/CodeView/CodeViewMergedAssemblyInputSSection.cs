using BinaryStudio.PortableExecutable.Win32;
using System;

namespace BinaryStudio.PortableExecutable.CodeView
    {
    public class CodeViewMergedAssemblyInputSSection : CodeViewPrimarySSection
        {
        public override DEBUG_S Type { get { return DEBUG_S.DEBUG_S_MERGED_ASSEMBLYINPUT; }}
        internal unsafe CodeViewMergedAssemblyInputSSection(CodeViewSection section, Int32 offset, Byte* content, Int32 length)
            : base(section, offset, content, length)
            {
            }
        }
    }