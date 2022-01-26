using System;
using System.Runtime.InteropServices;

namespace BinaryStudio.PlatformComponents.Win32
    {
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    [ComImport, Guid("1CF2B120-547D-101B-8E65-08002B2BD119")]
    public interface IErrorInfo
        {
        Guid   GetGUID();
        String GetSource();
        String GetDescription();
        String GetHelpFile();
        Int32  GetHelpContext();
        }
    }