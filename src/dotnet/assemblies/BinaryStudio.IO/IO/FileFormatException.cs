using System;

namespace BinaryStudio.IO
    {
    #if NETCOREAPP
    public class FileFormatException : Exception
        {
        public FileFormatException(Uri sourceuri)
            {
            }
        }
    #endif
    }
