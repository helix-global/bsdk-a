using System;
using System.IO;

namespace BinaryStudio.DirectoryServices
    {
    public interface IFileService
        {
        String FileName { get; }
        Byte[] ReadAllBytes();
        Stream OpenRead();
        void MoveTo(String target);
        }
    }