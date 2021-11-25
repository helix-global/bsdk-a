using System;
using BinaryStudio.PlatformUI;

namespace Microsoft.Win32
    {
    public class TrackBeforeWindowCreateEventArgs : EventArgs
        {
        public IntPtr InsertAfter { get; }
        internal TrackBeforeWindowCreateEventArgs(ref CREATESTRUCT source, IntPtr insertafter)
            {
            InsertAfter = insertafter;
            }
        }
    }