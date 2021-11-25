using System;
using System.Collections.Generic;
using System.Text;
using BinaryStudio.PortableExecutable.CodeView;
using BinaryStudio.PortableExecutable.Win32;
using Microsoft.Win32;

namespace BinaryStudio.PortableExecutable.CommonObjectFile.Sections
    {
    public class MSC13CodeViewSection : CodeViewSection
        {
        public override CV_SIGNATURE Signature { get { return CV_SIGNATURE.CV_SIGNATURE_C13; }}
        public override Encoding Encoding { get { return Encoding.UTF8; }}
        public override Boolean IsLengthPrefixedString { get { return false; }}
        public IList<CodeViewMajorRecord> Records { get; }
        internal unsafe MSC13CodeViewSection(CommonObjectFileSource o, Int32 index, Byte* mapping, IMAGE_SECTION_HEADER* section)
            : base(o, index, mapping, section)
            {
            }
        }
    }