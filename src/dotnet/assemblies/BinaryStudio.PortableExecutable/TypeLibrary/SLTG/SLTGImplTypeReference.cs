using System.Runtime.InteropServices.ComTypes;

namespace BinaryStudio.PortableExecutable.TypeLibrary.SLTG
    {
    internal class SLTGImplTypeReference : ITypeLibraryTypeReference
        {
        public SLTGImplTypeReference(ITypeLibraryTypeDescriptor source)
            {
            Type = source;
            Flags = 0;
            }

        public ITypeLibraryTypeDescriptor Type { get; }
        public IMPLTYPEFLAGS Flags { get; }
        }
    }