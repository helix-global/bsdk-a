namespace BinaryStudio.PortableExecutable
    {
    public interface ITypeLibraryFixedArrayTypeDescriptor
        {
        ITypeLibraryTypeDescriptor ElementType { get; }
        TypeLibraryFixedArrayBoundCollection Dimension { get; }
        }
    }