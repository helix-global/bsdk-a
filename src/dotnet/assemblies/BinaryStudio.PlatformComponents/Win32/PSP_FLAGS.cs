using System;

namespace BinaryStudio.PlatformComponents.Win32
    {
    [Flags]
    public enum PSP_FLAGS
        {
        PSP_DEFAULT             = 0x00000000,
        PSP_DLGINDIRECT         = 0x00000001,
        PSP_USEHICON            = 0x00000002,
        PSP_USEICONID           = 0x00000004,
        PSP_USETITLE            = 0x00000008,
        PSP_RTLREADING          = 0x00000010,
        PSP_HASHELP             = 0x00000020,
        PSP_USEREFPARENT        = 0x00000040,
        PSP_USECALLBACK         = 0x00000080,
        PSP_PREMATURE           = 0x00000400,
        PSP_HIDEHEADER          = 0x00000800,
        PSP_USEHEADERTITLE      = 0x00001000,
        PSP_USEHEADERSUBTITLE   = 0x00002000,
        PSP_USEFUSIONCONTEXT    = 0x00004000
        }
    }