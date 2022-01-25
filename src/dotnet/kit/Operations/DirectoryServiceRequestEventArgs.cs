using System;
using BinaryStudio.DirectoryServices;

namespace Operations
    {
    internal class DirectoryServiceRequestEventArgs : EventArgs
        {
        public Boolean Handled { get;set; }
        public IFileService Source { get; }
        public IDirectoryService Service { get;set; }
        public DirectoryServiceRequestEventArgs(IFileService source)
            {
            if (source == null) { throw new ArgumentNullException(nameof(source)); }
            Source = source;
            }
        }
    }