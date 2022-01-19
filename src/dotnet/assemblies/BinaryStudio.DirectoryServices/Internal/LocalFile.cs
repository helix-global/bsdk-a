using System;
using System.IO;

namespace BinaryStudio.DirectoryServices
    {
    internal class LocalFile : IFileService
        {
        public String FileName { get; }
        public LocalFile(String filename) {
            FileName = filename;
            }

        public Byte[] ReadAllBytes()
            {
            return File.ReadAllBytes(FileName);
            }

        public Stream OpenRead()
            {
            return File.OpenRead(FileName);
            }

        public void MoveTo(String target)
            {
            File.Move(FileName, target);
            }
        }
    }