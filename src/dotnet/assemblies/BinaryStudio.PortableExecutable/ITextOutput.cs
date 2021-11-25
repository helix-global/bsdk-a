using System.IO;

namespace BinaryStudio.PortableExecutable
    {
    public interface ITextOutput
        {
        void WriteText(TextWriter writer);
        }
    }