using System;
using BinaryStudio.PortableExecutable.Win32;
using Microsoft.Win32;

namespace BinaryStudio.PortableExecutable.CodeView
    {
    public class MSC07CodeViewSection : CodeViewSection
        {
        public override CV_SIGNATURE Signature { get { return CV_SIGNATURE.CV_SIGNATURE_C7; }}
        public unsafe MSC07CodeViewSection(CommonObjectFileSource o, Int32 index, Byte* mapping, IMAGE_SECTION_HEADER* section)
            : base(o, index, mapping, section)
            {
            Console.Error.WriteLine($"{Signature}");
            }
        }
    }