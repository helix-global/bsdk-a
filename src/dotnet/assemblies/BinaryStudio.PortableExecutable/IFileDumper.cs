using System;
using System.IO;

namespace BinaryStudio.PortableExecutable
    {
    public interface IFileDumper
        {
        void Write(TextWriter writer, Object o);
        }
    }