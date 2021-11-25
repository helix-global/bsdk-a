using System;
using System.Runtime.InteropServices;
using PARAMFLAG = System.Runtime.InteropServices.ComTypes.PARAMFLAG;

namespace BinaryStudio.PortableExecutable.TypeLibrary
    {
    internal class MSFTParameterDescriptor : TypeLibraryParameterDescriptor
        {
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        internal struct MSFT_PARAMETER_INFO
            {
            public readonly Int32  DataType;
            public readonly Int32  NameIndex;
            public readonly UInt32 Flags;
            }

        public unsafe MSFTParameterDescriptor(MSFTMetadataTypeLibrary library, MSFTMethodDescriptor mi, MSFT_PARAMETER_INFO* pi, Int32 index)
            {
            Name = library.N[pi->NameIndex];
            Flags = (PARAMFLAG)(pi->Flags & 0xFF);
            ParameterType = library.TypeOf(pi->DataType);
            }

        public override String Name { get; }
        public override PARAMFLAG Flags { get; }
        public override ITypeLibraryTypeDescriptor ParameterType { get; }
        }
    }