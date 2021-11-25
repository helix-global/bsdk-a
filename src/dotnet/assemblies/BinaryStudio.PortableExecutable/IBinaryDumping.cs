using System.IO;

namespace BinaryStudio.PortableExecutable
    {
    public interface IBinaryDumping
        {
        void Write(TextWriter writer);
        }
    }