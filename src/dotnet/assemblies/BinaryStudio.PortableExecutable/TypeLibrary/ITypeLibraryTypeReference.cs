namespace BinaryStudio.PortableExecutable
    {
    using IMPLTYPEFLAGS = System.Runtime.InteropServices.ComTypes.IMPLTYPEFLAGS;
    public interface ITypeLibraryTypeReference
        {
        ITypeLibraryTypeDescriptor Type { get; }
        IMPLTYPEFLAGS Flags { get; }
        }
    }