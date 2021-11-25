using System;
using System.Runtime.InteropServices;

namespace Microsoft.Win32
    {
    [StructLayout(LayoutKind.Sequential)]
    internal struct VS_FIXEDFILEINFO
        {
        public readonly UInt32 Signature;            /* e.g. 0xfeef04bd */
        public readonly UInt32 StrucVersion;         /* e.g. 0x00000042 = "0.42" */
        public readonly UInt32 FileVersionMS;        /* e.g. 0x00030075 = "3.75" */
        public readonly UInt32 FileVersionLS;        /* e.g. 0x00000031 = "0.31" */
        public readonly UInt32 ProductVersionMS;     /* e.g. 0x00030010 = "3.10" */
        public readonly UInt32 ProductVersionLS;     /* e.g. 0x00000031 = "0.31" */
        public readonly UInt32 FileFlagsMask;        /* = 0x3F for version "0.42" */
        public readonly UInt32 FileFlags;            /* e.g. VFF_DEBUG | VFF_PRERELEASE */
        public readonly UInt32 FileOS;               /* e.g. VOS_DOS_WINDOWS16 */
        public readonly UInt32 FileType;             /* e.g. VFT_DRIVER */
        public readonly UInt32 FileSubtype;          /* e.g. VFT2_DRV_KEYBOARD */
        public readonly UInt32 FileDateMS;           /* e.g. 0 */
        public readonly UInt32 FileDateLS;           /* e.g. 0 */
        }
    }