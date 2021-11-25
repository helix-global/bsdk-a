using System;
using System.IO;

namespace BinaryStudio.PortableExecutable
    {
    public class FileDumper : IFileDumper
        {
        private readonly IFileDumper o;
        public FileDumper(Type type, FileDumperFlags flags)
            {
            if (type == typeof(CommonObjectFileSource)) { o = new DefaultCommonObjectFileDumper(flags); }
            }

        public void Write(TextWriter writer, Object o)
            {
            this.o.Write(writer, o);
            }
        }
    }