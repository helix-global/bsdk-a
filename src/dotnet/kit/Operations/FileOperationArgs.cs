using System;

namespace Operations
    {
    internal class FileOperationArgs : EventArgs
        {
        public String TargetFolder { get;set; }
        public String Pattern { get;set; }
        }
    }