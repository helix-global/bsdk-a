using System.Runtime.InteropServices;

namespace BinaryStudio.PlatformComponents.Win32
    {
    [StructLayout(LayoutKind.Explicit, Pack = 1)]
    public struct RTL_USER_PROCESS_PARAMETERS32
        {
        [FieldOffset(56)] public readonly UNICODE_STRING32 ImagePathName;
        [FieldOffset(64)] public readonly UNICODE_STRING32 CommandLine;
        }
    }