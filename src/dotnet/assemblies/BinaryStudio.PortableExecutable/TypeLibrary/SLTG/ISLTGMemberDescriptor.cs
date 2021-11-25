using System;

namespace BinaryStudio.PortableExecutable.TypeLibrary
    {
    internal interface ISLTGMemberDescriptor
        {
        unsafe Int32 HelpContext(Byte* block, Int32 context);
        }
    }