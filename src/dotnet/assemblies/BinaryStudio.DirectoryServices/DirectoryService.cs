using System;
using BinaryStudio.DirectoryServices.Internal;

namespace BinaryStudio.DirectoryServices
    {
    public class DirectoryService
        {
        public static IDirectoryService GetService(String[] entries) {
            return new VirtualDirectoryService(entries);
            }
        }
    }