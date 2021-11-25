using System.IO;

namespace BinaryStudio.PortableExecutable
    {
    public abstract class TypeLibrarySerializer
        {
        public abstract void Write(ITypeLibraryDescriptor source, TextWriter writer);
        }
    }