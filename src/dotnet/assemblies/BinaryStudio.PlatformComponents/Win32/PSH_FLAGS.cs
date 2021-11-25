using System;

namespace BinaryStudio.PlatformComponents.Win32
    {
    [Flags]
    public enum PSH_FLAGS
        {
        PSH_DEFAULT             = 0x00000000,
        PSH_PROPTITLE           = 0x00000001,
        PSH_USEHICON            = 0x00000002,
        PSH_USEICONID           = 0x00000004,
        PSH_PROPSHEETPAGE       = 0x00000008,
        PSH_WIZARDHASFINISH     = 0x00000010,
        PSH_WIZARD              = 0x00000020,
        PSH_USEPSTARTPAGE       = 0x00000040,
        PSH_NOAPPLYNOW          = 0x00000080,
        PSH_USECALLBACK         = 0x00000100,
        PSH_HASHELP             = 0x00000200,
        PSH_MODELESS            = 0x00000400,
        PSH_RTLREADING          = 0x00000800,
        PSH_WIZARDCONTEXTHELP   = 0x00001000,
        PSH_WIZARD97            = 0x01000000,
        PSH_WATERMARK           = 0x00008000,
        PSH_USEHBMWATERMARK     = 0x00010000,
        PSH_USEHPLWATERMARK     = 0x00020000,
        PSH_STRETCHWATERMARK    = 0x00040000,
        PSH_HEADER              = 0x00080000,
        PSH_USEHBMHEADER        = 0x00100000,
        PSH_USEPAGELANG         = 0x00200000,
        PSH_WIZARD_LITE         = 0x00400000,
        PSH_NOCONTEXTHELP       = 0x02000000,
        PSH_AEROWIZARD          = 0x00004000,
        PSH_RESIZABLE           = 0x04000000,
        PSH_HEADERBITMAP        = 0x08000000,
        PSH_NOMARGIN            = 0x10000000
        }
    }