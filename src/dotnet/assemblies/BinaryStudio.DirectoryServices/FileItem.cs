using System;
using System.IO;

namespace BinaryStudio.DirectoryServices
    {
    public class FileItem : IFileService
        {
        public String FileName { get; }
        public FileItem(String filename) {
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
        }
    }
