using System;

namespace BinaryStudio.PortableExecutable.Win32
    {
    [Flags]
    public enum CV_PFLAG : byte
        {
        CV_PFLAG_NOFPO      = 0x01,
        CV_PFLAG_INT        = 0x02,
        CV_PFLAG_FAR        = 0x04,
        CV_PFLAG_NEVER      = 0x08,
        CV_PFLAG_NOTREACHED = 0x10,
        CV_PFLAG_CUST_CALL  = 0x20,
        CV_PFLAG_NOINLINE   = 0x40,
        CV_PFLAG_OPTDBGINFO = 0x80
        }
    }